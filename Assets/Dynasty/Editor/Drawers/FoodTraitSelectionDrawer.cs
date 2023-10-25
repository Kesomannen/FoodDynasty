using System.Linq;
using Dynasty.Food;
using Dynasty.Library;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoodTraitSelection))]
public class FoodTraitSelectionDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var height = EditorGUIUtility.singleLineHeight;
        var hasProperty = property.FindPropertyRelative("_hash");

        if (!FoodTraitDatabase.Singleton.TryGetEntry(hasProperty.intValue, out _)) {
            height = height * 2 + 10;
        }
        
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var database = FoodTraitDatabase.Singleton;

        var valueProperty = property.FindPropertyRelative("_value");
        var hashProperty = property.FindPropertyRelative("_hash");
        var typeProperty = property.FindPropertyRelative("_type");

        var rect = position;
        rect.width -= 120;
        rect.height = EditorGUIUtility.singleLineHeight;
        
        EditorGUI.PropertyField(rect, valueProperty, label);
        
        var value = valueProperty.stringValue;
        var validValue = database.TryGetEntry(value.GetHashCode(), out var selectedEntry);

        if (validValue) {
            hashProperty.intValue = selectedEntry.Hash;
            typeProperty.enumValueIndex = (int) selectedEntry.Type;
        } else {
            hashProperty.intValue = 0;
            typeProperty.enumValueIndex = 0;
        }

        rect.x += rect.width + 5;
        rect.width = 75;
        
        if (GUI.Button(rect, "Select")) {
            var menu = new GenericMenu();
            
            foreach (var entry in database.Entries) {
                var isOn = entry.Hash == hashProperty.intValue;
                
                menu.AddItem(new GUIContent(entry.Name), isOn, () => {
                    valueProperty.stringValue = entry.Name;
                    hashProperty.intValue = entry.Hash;
                    
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            
            menu.DropDown(new Rect(rect.x, rect.y + rect.height, 0, 0));
        }
        
        rect.x += rect.width + 5;
        rect.width = 35;

        EditorGUI.BeginDisabledGroup(validValue || string.IsNullOrWhiteSpace(value));
        
        if (GUI.Button(rect, EditorGUIUtility.IconContent("CreateAddNew"))) {
            var menu = new GenericMenu();
            
            foreach (var type in EnumHelpers.GetValues<FoodTraitType>()) {
                menu.AddItem(new GUIContent(type.ToString()), false, () => {
                    var newEntry = database.AddEntry(valueProperty.stringValue, type);
                    hashProperty.intValue = newEntry.Hash;
                });
            }
            
            menu.ShowAsContext();
        }
        
        EditorGUI.EndDisabledGroup();
        
        if (!validValue) {
            rect.x = position.x;
            rect.y += rect.height + 5;
            rect.width = position.width;
            rect.height += 5;
            
            EditorGUI.HelpBox(rect, "Invalid value. Use Add button to make new entry in trait database.", MessageType.Warning);
        }

        EditorGUI.EndProperty();
    }
}