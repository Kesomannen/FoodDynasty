using System;
using System.Collections.Generic;
using System.Linq;
using SolidUtilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class ItemMachineEditor : IDisposable {
    readonly VisualElement _root;
    readonly ObjectField _prefabField;
    
    Event<Food>[] _events;
    CheckEvent<bool>[] _checkEvents;

    readonly List<VisualElement> _activeComponentEditors = new();
    readonly ObjectPool<VisualElement> _componentEditorPool;

    public ItemMachineEditor(VisualElement root, VisualTreeAsset componentEditorUxml) {
        _root = root;
        _prefabField = _root.Q<ObjectField>(name: "machine-prefab");
        
        _componentEditorPool = new ObjectPool<VisualElement>(
            createFunc: componentEditorUxml.CloneTree,
            actionOnGet: element => {
                _activeComponentEditors.Add(element);
                element.style.display = DisplayStyle.Flex;
            },
            actionOnRelease: element => {
                _activeComponentEditors.Remove(element);
                
                var childrenToRemove = element.Q(name: "component-properties").Children().Skip(1).ToArray();
                foreach (var child in childrenToRemove) {
                    child.RemoveFromHierarchy();
                }
                
                element.style.display = DisplayStyle.None;
            },
            actionOnDestroy: element => element.RemoveFromHierarchy()
        );
    }

    public void Show(MachineItemData item) {
        Cleanup();
        
        _root.style.display = DisplayStyle.Flex;
        Selection.activeObject = item.Prefab;
        
        _prefabField.objectType = typeof(GridObject);
        _root.Bind(new SerializedObject(item));

        if (item.Prefab != null) {
            SetupEditor(item, item.Prefab);
        }
    }
    
    public void Hide() {
        Cleanup();
        
        _root.style.display = DisplayStyle.None;
    }
    
    void Cleanup() {
        while (_activeComponentEditors.Count > 0) {
            _componentEditorPool.Release(_activeComponentEditors[0]);
        }
    }

    void SetupEditor(MachineItemData item, Component prefab) {
        if (prefab.TryGetComponent(out MachineEntity entity)) entity.Data = item;

        _events = prefab.GetComponentsInChildren<Event<Food>>();
        _checkEvents = prefab.GetComponentsInChildren<CheckEvent<bool>>();

        var machineComponents = new List<Component> { prefab };
        machineComponents.AddRange(prefab.GetComponentsInChildren<IMachineComponent>()
            .Select(component => component.Component));

        foreach (var editor in machineComponents.Select(CreateComponentEditor)) {
            _root.Add(editor);
        }
    }

    VisualElement CreateComponentEditor(Component component) {
        var componentEditor = _componentEditorPool.Get();

        var icon = componentEditor.Q(name: "component-icon");
        var label = componentEditor.Q<Label>(name: "component-name");
        var propertyContainer = componentEditor.Q(name: "component-properties");

        icon.style.backgroundImage = EditorGUIUtility.GetIconForObject(component);
        label.text = EditorUtil.FormatDisplay(component.GetType().Name);
        
        DrawComponentProperties(component, propertyContainer);
        return componentEditor;
    }
    
    void DrawComponentProperties(Component component, VisualElement container) {
        var serializedObject = new SerializedObject(component);
        var iterator = serializedObject.GetIterator();

        if (iterator.NextVisible(true)) {
            do {
                var property = iterator.Copy();
                
                if (property.name.StartsWith("m_")) continue;
                
                var element = property.propertyType switch {
                    SerializedPropertyType.Vector3 => null,
                    SerializedPropertyType.ObjectReference => CreateObjectField(property),
                    SerializedPropertyType.Generic => CreateGenericField(property),
                    _ => new PropertyField(property)
                };

                if (element != null) {
                    container.Add(element);
                }
            } while (iterator.NextVisible(false));
        }

        container.Bind(serializedObject);
    }

    VisualElement CreateGenericField(SerializedProperty property) {
        var type = property.GetObjectType();
        return type == typeof(FilteredItemEvent) ? CreateFilteredItemEvent(property) : null;
    }

    VisualElement CreateObjectField(SerializedProperty property) {
        var type = property.GetObjectType();
        
        if (type == typeof(Event<Food>) || type == typeof(GenericEvent)) {
            return CreateEventDropdown(property, property.displayName);
        }
        if (type == typeof(CheckEvent<bool>)) {
            return CreateConditionDropdown(property, property.displayName);
        }

        return new ObjectField(property.displayName) {
            bindingPath = property.propertyPath,
            objectType = type 
        };
    }
    
    VisualElement CreateFilteredItemEvent(SerializedProperty property) {
        var container = new VisualElement();

        container.Add(CreateEventDropdown(property.FindPropertyRelative("_event")));
        container.Add(CreateConditionDropdown(property.FindPropertyRelative("_condition")));
        container.Add(new PropertyField(property.FindPropertyRelative("_filter")));
        container.style.marginBottom = 5;

        return container;
    }

    DropdownField CreateConditionDropdown(SerializedProperty property, string label = "Condition") {
        return EditorUtil.CreateDropdown(
            property, 
            label,
            condition => {
                var index = Array.IndexOf(_checkEvents, condition);
                return index == -1 ? "None" : $"Condition {index + 1}";
            },
            name => name == "None" ? null : _checkEvents[name[^1] - '1'],
            _checkEvents
        );
    }
    
    DropdownField CreateEventDropdown(SerializedProperty property, string label = "Event") {
        return EditorUtil.CreateDropdown(
            property, 
            label,
            foodEvent => {
                var index = Array.IndexOf(_events, foodEvent);
                return index == -1 ? "None" : $"Event {index + 1}";
            }, 
            name => name == "None" ? null : _events[name[^1] - '1'], 
            _events
        );
    }

    public void Dispose() {
        _componentEditorPool?.Dispose();
    }
}