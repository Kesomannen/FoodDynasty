using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dynasty.Library.Helpers;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoodFilter))]
public class FoodFilterDrawer : PropertyDrawer {
    bool _foldout = true;
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var lines = 2;
        
        var foodFilter = property.GetValue<FoodFilter>();
        var dataType = FoodDataUtil.GetDataType(foodFilter.DataType);
        var fields = ReflectionHelpers.GetFields(dataType);

        if (fields.Length == 0) {
            return EditorGUIUtility.singleLineHeight * lines;
        }

        lines += 2;

        if (!_foldout) {
            return EditorGUIUtility.singleLineHeight * lines;
        }
        
        lines += foodFilter.FieldFilters.Sum(fieldFilter => fieldFilter.Enabled ? 2 : 1);

        return EditorGUIUtility.singleLineHeight * lines;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var foodFilter = property.GetValue<FoodFilter>();
        
        var filterTypeProperty = property.Find(nameof(FoodFilter.FilterType));
        var dataTypeProperty = property.Find(nameof(FoodFilter.DataType));
        var requireDataProperty = property.Find(nameof(FoodFilter.RequireData));
        
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, filterTypeProperty);
        
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, dataTypeProperty);
        
        var dataType = FoodDataUtil.GetDataType(foodFilter.DataType);
        var fields = ReflectionHelpers.GetFields(dataType);

        if (fields.Length == 0) {
            foodFilter.RequireData = true;
        } else {
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, requireDataProperty);
            
            DrawFieldsSection(ref rect, fields, foodFilter);
        }
        
        // Trim extra field filters
        while (foodFilter.FieldFilters.Count > fields.Length) {
            foodFilter.FieldFilters.RemoveAt(foodFilter.FieldFilters.Count - 1);
        }

        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    void DrawFieldsSection(ref Rect rect, IReadOnlyList<FieldInfo> fields, FoodFilter itemFilter) {
        rect.y += EditorGUIUtility.singleLineHeight;
        _foldout = EditorGUI.Foldout(rect, _foldout, "Fields", true);

        if (!_foldout) return;
        
        EditorGUI.indentLevel++;
        
        for (var i = 0; i < fields.Count; i++) {
            var info = fields[i];

            if (itemFilter.FieldFilters.Count <= i) {
                itemFilter.FieldFilters.Add(new FoodFieldFilter());
            }

            var filter = itemFilter.FieldFilters[i];
            itemFilter.FieldFilters[i] = DrawFieldFilter(filter, info, ref rect);
        }

        EditorGUI.indentLevel--;
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