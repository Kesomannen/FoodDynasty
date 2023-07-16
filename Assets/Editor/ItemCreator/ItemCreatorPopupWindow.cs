using System;
using System.Collections.Generic;
using Dynasty.Core.Data;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Machine.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemCreatorPopupWindow : PopupWindowContent {
    readonly List<IItemModifier> _modifiers = new();
    readonly CreateMode _createMode;
    
    ObjectField _modelField;
    TextField _nameField;
    EnumField _tierField;
    
    public event Action OnItemCreated;

    public ItemCreatorPopupWindow(CreateMode createMode) {
        _createMode = createMode;
    }
    
    public override Vector2 GetWindowSize() {
        return new Vector2(400, 250);
    }

    public override void OnGUI(Rect rect) { }
    public override void OnClose() { }

    public override void OnOpen() {
        var root = editorWindow.rootVisualElement;
        CreateGeneralEditor();
        
        
        root.Add(CreateCreateButton());
    }

    void CreateGeneralEditor() {
        var container = CreateEditor(CreateMode.Machine_General);
        AddFieldModifier<string>("description-field", (description, item) => item.Description = description);
        
        _modelField = container.Q<ObjectField>("model-field");
        _nameField = container.Q<TextField>("name-field");
        _tierField = container.Q<EnumField>("tier-field");
        
        var conveyorSpeedField = container.Q<ObjectField>("conveyor-speed-field");
        conveyorSpeedField.objectType = typeof(DataObject<float>);
        AddFieldModifier(conveyorSpeedField, (speed, item) => {
            if (item is not IPrefabProvider<GridObject> provider) return;
            if (!provider.Prefab.TryGetComponent(out Conveyor conveyor)) return;
            conveyor.Speed = (DataObject<float>) speed;
        });

        _tierField.Init(ItemTier.Rusty);
    }

    Button CreateCreateButton() {
        var createButton = new Button(() => {
            var model = _modelField.value as GameObject;
            if (model == null) return;
            
            var name = _nameField.value;
            if (string.IsNullOrWhiteSpace(name)) return;
            
            var tier = (ItemTier) _tierField.value;
            var item = _createMode switch {
                CreateMode.Machine_General => ItemCreator.CreateGenericMachine(name, model, tier),
                CreateMode.Machine_ModifierWithoutSupply => ItemCreator.CreateModifierMachine(name, model, tier),
                CreateMode.Machine_ModifierWithSupply => ItemCreator.CreateModifierMachineWithSupply(name, model, tier),
                CreateMode.Machine_Seller => ItemCreator.CreateSellMachine(name, model, tier),
                CreateMode.Machine_Depositer => ItemCreator.CreateDepositMachine(name, model, tier),
                CreateMode.Machine_Splitter => ItemCreator.CreateSplitMachine(name, model, tier),
                _ => throw new ArgumentOutOfRangeException()
            };
                
            foreach (var modifier in _modifiers) {
                modifier.Apply(item);
            }

            OnItemCreated?.Invoke();
        }) {
            text = "Create"
        };
        
        return createButton;
    }

    void AddFieldModifier<T>(string name, Action<T, ItemData> modifier) {
        AddFieldModifier(editorWindow.rootVisualElement.Q<BaseField<T>>(name), modifier);
    }
    
    void AddFieldModifier<T>(INotifyValueChanged<T> field, Action<T, ItemData> modifier) {
        _modifiers.Add(new FieldModifier<T>(field, modifier));
    }

    VisualElement CreateCurrentEditor() {
        return CreateEditor(_createMode);
    }
    
    VisualElement CreateEditor(CreateMode createMode) {
        var editor = LoadUxml(createMode).CloneTree();
        editorWindow.rootVisualElement.Add(editor);
        return editor;
    }
    
    static VisualTreeAsset LoadUxml(CreateMode createMode) {
        return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"Assets/Editor/ItemCreator/{createMode}.uxml");
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
        Machine_Seller,
        Machine_Depositer,
        Machine_Splitter
    }
}