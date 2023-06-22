using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoodFilter))]
public class ItemFilterDrawer : PropertyDrawer {
    bool _foldout = true;
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var lines = 4;
        if (!_foldout) return EditorGUIUtility.singleLineHeight * lines;
        
        var itemFilter = property.GetValue<FoodFilter>();
        lines += itemFilter.FieldFilters.Sum(fieldFilter => fieldFilter.Enabled ? 2 : 1);

        return EditorGUIUtility.singleLineHeight * lines;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var itemFilter = property.GetValue<FoodFilter>();
        
        var filterTypeProperty = property.Find(nameof(FoodFilter.FilterType));
        var dataTypeProperty = property.Find(nameof(FoodFilter.DataType));
        var requireDataProperty = property.Find(nameof(FoodFilter.RequireData));
        
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
            var dataType = FoodDataUtil.GetDataType(itemFilter.DataType);
            var fields = ReflectionHelpers.GetFields(dataType);

            var i = 0;
            for (; i < fields.Length; i++) {
                var info = fields[i];
            
                if (itemFilter.FieldFilters.Count <= i) {
                    itemFilter.FieldFilters.Add(new FoodFieldFilter());
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

    static FoodFieldFilter DrawFieldFilter(FoodFieldFilter filter, FieldInfo info, ref Rect rect) {
        filter.FieldName = info.Name;

        rect.y += EditorGUIUtility.singleLineHeight;
        filter.Enabled = EditorGUI.Toggle(rect, StringHelpers.FormatCamelCase(info.Name), filter.Enabled);

        if (!filter.Enabled) return filter;
        EditorGUI.indentLevel++;
        
        filter.FilterType = info.FieldType == typeof(int) ? FoodFieldFilterType.Int : FoodFieldFilterType.Bool;

        rect.y += EditorGUIUtility.singleLineHeight;
        switch (filter.FilterType) {
            case FoodFieldFilterType.Int:
                filter.IntRange = EditorGUI.Vector2IntField(rect, new GUIContent("Range"), filter.IntRange);
                break;
            case FoodFieldFilterType.Bool:
                filter.BoolValue = EditorGUI.Toggle(rect, new GUIContent("Value"), filter.BoolValue);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
        
        EditorGUI.indentLevel--;
        return filter;
    }
}