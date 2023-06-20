using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemDataModifier))]
public class ItemDataModifierDrawer : PropertyDrawer {
    static readonly Type[] _allowedModiferTypes = { typeof(int), typeof(float) };
    
    bool _foldout = true;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!_foldout) return EditorGUIUtility.singleLineHeight;

        var dataModifier = property.GetValue<ItemDataModifier>();

        var lines = 4;
        
        var type = ItemDataUtil.GetDataType(dataModifier.DataType);
        if (!ReflectionHelpers.TryGetField(type, dataModifier.FieldName, out var field)) {
            return EditorGUIUtility.singleLineHeight * lines;
        }
        
        if (_allowedModiferTypes.Contains(field.FieldType)) {
            lines++;
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
        
        var dataModifier = property.GetValue<ItemDataModifier>();
        
        rect.y += EditorGUIUtility.singleLineHeight;
        var dataTypeProperty = property.Find(nameof(ItemDataModifier.DataType));
        
        EditorGUI.PropertyField(rect, dataTypeProperty);
        
        var type = ItemDataUtil.GetDataType(dataModifier.DataType);
        var fields = ReflectionHelpers.GetFields(type);
        
        rect.y += EditorGUIUtility.singleLineHeight;
        var selectedFieldIndex = Array.FindIndex(fields, info => info.Name == dataModifier.FieldName);
        selectedFieldIndex = EditorGUI.Popup(rect, "Field", selectedFieldIndex, fields.Select(info => info.Name).ToArray());
        var selectedField = fields[selectedFieldIndex];
        dataModifier.FieldName = selectedField.Name;

        if (_allowedModiferTypes.Contains(selectedField.FieldType)) {
            rect.y += EditorGUIUtility.singleLineHeight;
            dataModifier.OperationType = (ItemModifierOperation) EditorGUI.EnumPopup(rect, "Operation", dataModifier.OperationType);
        } else {
            dataModifier.OperationType = ItemModifierOperation.Set;
        }

        rect.y += EditorGUIUtility.singleLineHeight;
        switch (dataModifier.OperationType) {
            case ItemModifierOperation.Modify: {
                var modifierProperty = property.Find(nameof(ItemDataModifier.Modifier));
                EditorGUI.PropertyField(rect, modifierProperty);
                break;
            }
            case ItemModifierOperation.Set: {
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
            return property.Find(nameof(ItemDataModifier.IntValue));
        }

        if (field.FieldType == typeof(float)) {
            return property.Find(nameof(ItemDataModifier.FloatValue));
        }

        if (field.FieldType == typeof(bool)) {
            return property.Find(nameof(ItemDataModifier.BoolValue));
        }

        throw new ArgumentOutOfRangeException();
    }
}