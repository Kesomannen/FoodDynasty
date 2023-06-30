using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemCreatorPopupWindow : PopupWindowContent {
    readonly List<IItemModifier> _modifiers = new();
    readonly CreateMode _createMode;
    
    public event Action OnItemCreated;

    public ItemCreatorPopupWindow(CreateMode createMode) {
        _createMode = createMode;
    }
    
    public override Vector2 GetWindowSize() {
        return new Vector2(400, 250);
    }

    public override void OnGUI(Rect rect) { }

    public override void OnOpen() {
        var root = editorWindow.rootVisualElement;
        CreateGeneralEditor(root);
    }

    void CreateGeneralEditor(VisualElement container) {
        LoadUxml("General").CloneTree(container);
        AddFieldModifier<string>("description-field", (description, item) => item.Description = description);

        var modelField = container.Q<ObjectField>("model-field");
        var nameField = container.Q<TextField>("name-field");
        var tierField = container.Q<EnumField>("tier-field");
        tierField.Init(ItemTier.Rusty);

        var createButton = new Button(() => {
            var model = modelField.value as GameObject;
            if (model == null) return;
            
            var name = nameField.value;
            if (string.IsNullOrWhiteSpace(name)) return;
            
            var tier = (ItemTier) tierField.value;

            var item = _createMode switch {
                CreateMode.Machine_General => ItemCreator.CreateGenericMachine(name, model, tier),
                CreateMode.Machine_Sell => null,
                CreateMode.Machine_Deposit => null,
                CreateMode.Machine_Split => null,
                CreateMode.Machine_ModifierWithoutSupply => null,
                CreateMode.Machine_ModifierWithSupply => null,
                _ => throw new ArgumentOutOfRangeException()
            };
                
            foreach (var modifier in _modifiers) {
                modifier.Apply(item);
            }

            OnItemCreated?.Invoke();
        }) {
            text = "Create"
        };

        container.Add(createButton);
    }

    public override void OnClose() {
        
    }

    void AddFieldModifier<T>(string name, Action<T, ItemData> modifier) {
        AddFieldModifier(editorWindow.rootVisualElement.Q<BaseField<T>>(name), modifier);
    }
    
    void AddFieldModifier<T>(INotifyValueChanged<T> field, Action<T, ItemData> modifier) {
        _modifiers.Add(new FieldModifier<T>(field, modifier));
    }
    
    static VisualTreeAsset LoadUxml(string name) {
        return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"Assets/Editor/ItemCreator/{name}.uxml");
    }

    interface IItemModifier {
        void Apply(ItemData item);
    }
    
    class FieldModifier<T> : IItemModifier {
        readonly INotifyValueChanged<T> _field;
        readonly Action<T, ItemData> _modifier;

        public FieldModifier(INotifyValueChanged<T> field, Action<T, ItemData> modifier) {
            _field = field;
            _modifier = modifier;
        }
        
        public void Apply(ItemData item) {
            _modifier(_field.value, item);
        }
    }
    
    public enum CreateMode {
        Machine_General,
        Machine_ModifierWithoutSupply,
        Machine_ModifierWithSupply,
        Machine_Sell,
        Machine_Deposit,
        Machine_Split
    }
}