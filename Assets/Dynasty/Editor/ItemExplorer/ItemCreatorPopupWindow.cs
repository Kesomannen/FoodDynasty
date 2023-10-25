using System;
using System.Collections.Generic;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Food;
using Dynasty.Library;
using Dynasty.Machines;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemCreatorPopupWindow : PopupWindowContent {
    readonly CreateMode _createMode;

    readonly List<Action<MachineCreator>> _processors = new();
    Func<MachineCreator> _creatorFactory;

    public event Action OnItemCreated;

    public ItemCreatorPopupWindow(CreateMode createMode) {
        _createMode = createMode;
    }
    
    public override Vector2 GetWindowSize() {
        return new Vector2(400, 500);
    }

    public override void OnGUI(Rect rect) { }
    public override void OnClose() { }

    public override void OnOpen() {
        CreateGeneralEditor();

        switch (_createMode) {
            case CreateMode.Machine_General: break;
            case CreateMode.Machine_ModifierWithoutSupply: 
                CreateModifierEditor();
                break;
            case CreateMode.Machine_ModifierWithSupply:
                CreateModifierWithSupplyEditor();
                break;
            case CreateMode.Machine_Seller:
                CreateSellerEditor();
                break;
            case CreateMode.Machine_Dispenser: break;
            case CreateMode.Machine_Splitter: break;
            default: throw new ArgumentOutOfRangeException();
        }
        
        editorWindow.rootVisualElement.Add(CreateCreateButton());
    }

    void CreateGeneralEditor() {
        var nameField = Add(new TextField("Name") { value = "New Item" });
        var descriptionField = Add(new TextField("Description") { value = "New Item Description" });
        var priceField = Add(new DoubleField("Price") { value = 1000 });
        var sizeField = Add(new Vector2IntField("Size") { value = new Vector2Int(4, 4) });
        var tierField = Add(new EnumField("Tier", ItemTier.Rusty));
        var modelField = Add(new ObjectField("Model") { objectType = typeof(GameObject) });
        
        _creatorFactory = () => {
            if (string.IsNullOrEmpty(nameField.value)) {
                EditorUtility.DisplayDialog("Invalid argument", "Name cannot be empty", "Ok");
                return null;
            }
            
            if (modelField.value == null) {
                EditorUtility.DisplayDialog("Invalid argument", "Model cannot be empty", "Ok");
                return null;
            }

            return new MachineCreator(
                nameField.value,
                descriptionField.value,
                priceField.value,
                ItemType.Other,
                (ItemTier) tierField.value,
                (GameObject) modelField.value
            ).SetSize(new GridSize(sizeField.value));
        };
    }

    void CreateSellerEditor() {
        AddSpacer();
        var modifierFactory = AddModifierField();

        _processors.Add(creator => {
            creator
                .SetType(ItemType.Seller)
                .AddTrigger(out var triggerEvent)
                .AddSeller(
                    modifierFactory(), 
                    new FilteredFoodEvent(triggerEvent)
                );
        });
    }
    
    void CreateModifierEditor() {
        AddSpacer();
        var modifierGroupFactory = AddModifierGroupField();

        _processors.Add(creator => {
            creator
                .SetType(ItemType.Modifier)
                .GenerateFilterGroup(out var filterGroup)
                .AddTrigger(out var triggerEvent, filter: filterGroup)
                .AddModifier(
                    modifierGroupFactory(creator), 
                    new FilteredFoodEvent(triggerEvent)
                );
        });
    }

    void CreateModifierWithSupplyEditor() {
        AddSpacer();
        var modifierGroupFactory = AddModifierGroupField();
        AddSpacer();
        var refillItemName = Add(new TextField("Refill Name") { value = "Refill" });
        var generateRefillItem = Add(new Toggle("Generate Refill Item") { value = true });

        _processors.Add(creator => {
            CheckEvent<bool> checkEvent;
            Event<FoodBehaviour> consumeEvent;
            
            if (generateRefillItem.value) {
                creator
                    .GenerateTopping(out var topping, refillItemName.value)
                    .AddSupply(out checkEvent, out consumeEvent, topping);
            } else {
                creator
                    .AddSupply(out checkEvent, out consumeEvent, refillItemName.value);
            }
            
            creator
                .SetType(ItemType.Modifier)
                .GenerateFilterGroup(out var filterGroup)
                .AddTrigger(out var triggerEvent, checkEvent, filterGroup)
                .AddModifier(
                    modifierGroupFactory(creator), 
                    new FilteredFoodEvent(triggerEvent), 
                    itemModifiedEvent: consumeEvent
                );
        });
    }

    Func<MachineCreator, FoodModifierGroup> AddModifierGroupField() {
        var getModifier = AddModifierField();
        var hasModel = Add(new Toggle("Apply Model Modifier") { value = true });
        var modelPrefab = Add(new ObjectField("Model Prefab") { objectType = typeof(GameObject) });
        var modelType = Add(new EnumField("Model Type", ItemModelType.Topping));
        
        hasModel.RegisterValueChangedCallback(evt => {
            modelPrefab.SetEnabled(evt.newValue);
            modelType.SetEnabled(evt.newValue);
        });

        return creator => {
            FoodModifierGroup modifierGroup;
            if (hasModel.value) {
                var model = (GameObject)modelPrefab.value;
                var type = (ItemModelType)modelType.value;

                creator.GenerateModifierGroup(getModifier(), model, type, out modifierGroup);
            }
            else {
                creator.GenerateModifierGroup(getModifier(), out modifierGroup);
            }
            
            return modifierGroup;
        };
    }

    Func<Modifier> AddModifierField() {
        var additive = Add(new FloatField("Additive"));
        var percentual = Add(new FloatField("Percentual"));
        var multiplicative = Add(new FloatField("Multiplicative") { value = 1 });
        
        return () => new Modifier {
            Percentual = percentual.value,
            Multiplicative = multiplicative.value,
            Additive = additive.value
        };
    }

    Button CreateCreateButton() {
        var createButton = new Button(() => {
            var creator = _creatorFactory();
            if (creator == null) return;
            
            foreach (var processor in _processors) {
                processor(creator);
            }
            
            creator.Save();
            OnItemCreated?.Invoke();
        }) {
            text = "Create"
        };
        
        return createButton;
    }

    T Add<T>(T element) where T : VisualElement {
        editorWindow.rootVisualElement.Add(element);
        return element;
    }
    
    void AddSpacer(float height = 10) {
        Add(new VisualElement { style = { height = height } });
    }
    
    public enum CreateMode {
        Machine_General,
        Machine_ModifierWithoutSupply,
        Machine_ModifierWithSupply,
        Machine_Seller,
        Machine_Dispenser,
        Machine_Splitter
    }
}