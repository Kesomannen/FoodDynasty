using System;
using System.Linq;
using System.Reflection;
using Dynasty.Library.Helpers;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoodDataModifier))]
public class FoodDataModifierDrawer : PropertyDrawer {
    static readonly Type[] _allowedModiferTypes = { typeof(int), typeof(float) };
    
    bool _foldout = true;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!_foldout) return EditorGUIUtility.singleLineHeight;

        var dataModifier = property.GetValue<FoodDataModifier>();

        var lines = 4;
        
        var type = FoodDataUtil.GetDataType(dataModifier.DataType);
        if (!ReflectionHelpers.TryGetField(type, dataModifier.FieldName, out var field) ||
            !_allowedModiferTypes.Contains(field.FieldType)) 
        {
            return EditorGUIUtility.singleLineHeight * lines;   
        }

        if (dataModifier.OperationType == FoodModifierOperation.Set) {
            lines++;
        } else {
            var modifierPropertyHeight = EditorGUI.GetPropertyHeight(property.Find(nameof(FoodDataModifier.Modifier)));
            return EditorGUIUtility.singleLineHeight * lines + modifierPropertyHeight;
        }

        return EditorGUIUtility.singleLineHeight * lines;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        
        _foldout = EditorGUI.Foldout(rect, _foldout, "Data Modifier", true);
        if (!_foldout) return;
        
        EditorGUI.indentLevel++;
        
        var dataModifier = property.GetValue<FoodDataModifier>();
        
        rect.y += EditorGUIUtility.singleLineHeight;
        var dataTypeProperty = property.Find(nameof(FoodDataModifier.DataType));
        
        EditorGUI.PropertyField(rect, dataTypeProperty);
        
        var type = FoodDataUtil.GetDataType(dataModifier.DataType);
        var fields = ReflectionHelpers.GetFields(type);
        
        rect.y += EditorGUIUtility.singleLineHeight;
        var selectedFieldIndex = Array.FindIndex(fields, info => info.Name == dataModifier.FieldName);
        if (selectedFieldIndex == -1) selectedFieldIndex = 0;
        
        selectedFieldIndex = EditorGUI.Popup(
            rect,
            "Field", 
            selectedFieldIndex, 
            fields.Select(field => StringHelpers.FormatCamelCase(field.Name)).ToArray()
        );
        
        var selectedField = fields[selectedFieldIndex];
        dataModifier.FieldName = selectedField.Name;

        if (_allowedModiferTypes.Contains(selectedField.FieldType)) {
            rect.y += EditorGUIUtility.singleLineHeight;
            dataModifier.OperationType = (FoodModifierOperation) EditorGUI.EnumPopup(rect, "Operation", dataModifier.OperationType);
        } else {
            dataModifier.OperationType = FoodModifierOperation.Set;
        }

        rect.y += EditorGUIUtility.singleLineHeight;
        switch (dataModifier.OperationType) {
            case FoodModifierOperation.Modify: {
                var modifierProperty = property.Find(nameof(FoodDataModifier.Modifier));
                rect.height = EditorGUI.GetPropertyHeight(modifierProperty);
                EditorGUI.PropertyField(rect, modifierProperty, true);
                break;
            }
            case FoodModifierOperation.Set: {
                EditorGUI.PropertyField(rect, GetSetValueProperty(property, selectedField));
                break;
            }
            default: throw new ArgumentOutOfRangeException();
        }
        
        EditorGUI.indentLevel--;
        
        var serializedObject = property.serializedObject;
        if (GUI.changed) {
            EditorUtility.SetDirty(serializedObject.targetObject);
        } 
        
        serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    static SerializedProperty GetSetValueProperty(SerializedProperty property, FieldInfo field) {
        if (field.FieldType == typeof(int)) {
            return property.Find(nameof(FoodDataModifier.IntValue));
        }

        if (field.FieldType == typeof(float)) {
            return property.Find(nameof(FoodDataModifier.FloatValue));
        }

        if (field.FieldType == typeof(bool)) {
            return property.Find(nameof(FoodDataModifier.BoolValue));
        }

        throw new ArgumentOutOfRangeException();
    }
}