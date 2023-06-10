using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemFilter))]
public class ItemFilterDrawer : PropertyDrawer {
    bool _foldout;
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var lines = 4;
        if (!_foldout) return EditorGUIUtility.singleLineHeight * lines;
        
        var itemFilter = property.GetValue<ItemFilter>();
        lines += itemFilter.FieldFilters.Sum(fieldFilter => fieldFilter.Enabled ? 2 : 1);

        return EditorGUIUtility.singleLineHeight * lines;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var itemFilter = property.GetValue<ItemFilter>();
        
        var filterTypeProperty = property.Find(nameof(ItemFilter.FilterType));
        var dataTypeProperty = property.Find(nameof(ItemFilter.DataType));
        var requireDataProperty = property.Find(nameof(ItemFilter.RequireData));
        
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, filterTypeProperty);
        
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, dataTypeProperty);
        
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, requireDataProperty);

        rect.y += EditorGUIUtility.singleLineHeight;
        _foldout = EditorGUI.Foldout(rect, _foldout, "Fields", true);

        if (_foldout) {
            EditorGUI.indentLevel++;
            var dataType = ItemDataUtil.GetDataType(itemFilter.DataType);
            var fields = ReflectionHelpers.GetFields(dataType);

            var i = 0;
            for (; i < fields.Length; i++) {
                var info = fields[i];
            
                if (itemFilter.FieldFilters.Count <= i) {
                    itemFilter.FieldFilters.Add(new ItemFieldFilter());
                }
            
                var filter = itemFilter.FieldFilters[i];
                itemFilter.FieldFilters[i] = DrawFieldFilter(filter, info, ref rect);
            }

            for (; i < itemFilter.FieldFilters.Count; i++) {
                itemFilter.FieldFilters.RemoveAt(i);
                i--;
            }
                    
            EditorGUI.indentLevel--;
        }
        
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    static ItemFieldFilter DrawFieldFilter(ItemFieldFilter filter, FieldInfo info, ref Rect rect) {
        filter.FieldName = info.Name;

        rect.y += EditorGUIUtility.singleLineHeight;
        filter.Enabled = EditorGUI.Toggle(rect, StringUtil.FormatCamelCase(info.Name), filter.Enabled);

        if (!filter.Enabled) return filter;
        EditorGUI.indentLevel++;
        
        filter.FilterType = info.FieldType == typeof(int) ? ItemFieldFilterType.Int : ItemFieldFilterType.Bool;

        rect.y += EditorGUIUtility.singleLineHeight;
        switch (filter.FilterType) {
            case ItemFieldFilterType.Int:
                filter.IntRange = EditorGUI.Vector2IntField(rect, new GUIContent("Range"), filter.IntRange);
                break;
            case ItemFieldFilterType.Bool:
                filter.BoolValue = EditorGUI.Toggle(rect, new GUIContent("Value"), filter.BoolValue);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
        
        EditorGUI.indentLevel--;
        return filter;
    }
}