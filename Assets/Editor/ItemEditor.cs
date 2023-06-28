using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow  {
    ItemData _selectedItem;
    List<ItemData> _items;

    VisualElement _currentEditor;
    
    [MenuItem("Window/Item Editor %i")]
    public static void ShowExample() {
        var window = GetWindow<ItemEditor>();
        
        window.titleContent = new GUIContent("Item Editor");
        window.minSize = new Vector2(500, 300);
    }

    public void CreateGUI() {
        var root = rootVisualElement;
        
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ItemEditor.uxml");
        var templateContainer = visualTree.CloneTree();
        root.Add(templateContainer);

        FetchItems();
        ConfigureItemList(templateContainer.Q<ListView>("itemlist"));
    }

    void ConfigureItemList(BaseVerticalCollectionView listView) {
        var listItem = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ItemEditorListItem.uxml");

        listView.makeItem = MakeItem;
        listView.bindItem = BindItem;
        listView.itemsSource = _items;

        listView.onSelectionChange += objects => {
            var item = objects.FirstOrDefault() as ItemData;
            SelectItem(item);
        };
        
        VisualElement MakeItem() {
            return listItem.CloneTree();
        }

        void BindItem(VisualElement e, int i) {
            e.Q<Label>().text = _items[i].Name;
            e.Q(name: "item-icon").style.backgroundImage = _items[i].Image.texture;
        }
    }

    void FetchItems() {
        _items = AssetDatabase.FindAssets("t:ItemData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ItemData>)
            .ToList();
        
        SelectItem(_items.FirstOrDefault());
    }

    void SelectItem(ItemData itemData) {
        if (itemData == null || _selectedItem == itemData) return;
        _selectedItem = itemData;
        
        var itemEditor = rootVisualElement.Q("item-editor");
        var generalItemEditor = itemEditor.Q("editor-general");
        
        generalItemEditor.Unbind();
        generalItemEditor.Bind(new SerializedObject(_selectedItem));

        _currentEditor?.RemoveFromHierarchy();
        
        if (itemData is MachineItemData machineItemData) {
            _currentEditor = CreateMachineEditor(machineItemData);
            itemEditor.Add(_currentEditor);
        }
    }

    VisualElement CreateMachineEditor(MachineItemData machineData) {
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MachineItemEditor.uxml");
        var machineItemEditor = template.CloneTree();

        machineItemEditor.Q<ObjectField>(name: "machine-prefab").objectType = typeof(GridObject);
        machineItemEditor.Bind(new SerializedObject(machineData));

        var prefab = machineData.Prefab;
        if (prefab == null) {
            return machineItemEditor;
        }

        if (prefab.TryGetComponent(out MachineEntity entity)) {
            entity.Data = machineData;
        }
        
        var componentTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MachineItemComponentEditor.uxml");

        var components = prefab
            .GetComponentsInChildren<IMachineComponent>()
            .Select(c => c.Component).ToList();

        components.Insert(0, prefab);
        
        foreach (var component in components) {
            var componentEditor = componentTemplate.CloneTree();

            var icon = componentEditor.Q(name: "component-icon");
            var label = componentEditor.Q<Label>(name: "component-name");
            var element = componentEditor.Q(name: "machine-component");
            
            icon.style.backgroundImage = EditorGUIUtility.GetIconForObject(component);
            label.text = component.GetType().Name;
            
            var serializedObject = new SerializedObject(component);

            var property = serializedObject.GetIterator();
            if (property.NextVisible(true)) {
                do {
                    if (property.name == "m_Script") continue;

                    var field = new PropertyField(property) {
                        style = { height = 20 }
                    };
                    element.Add(field);
                } while (property.NextVisible(false));
            }
            
            machineItemEditor.Add(componentEditor);
            componentEditor.Bind(serializedObject);
        }

        return machineItemEditor;
    }
    
    
}