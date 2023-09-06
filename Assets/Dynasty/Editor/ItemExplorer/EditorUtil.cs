using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public static class EditorUtil {
    public static DropdownField CreateDropdown<T>(
        SerializedProperty property, 
        string label,
        Func<T, string> getName,
        Func<string, T> getFromName,
        IEnumerable<T> options
    ) where T : class {
        var current = property.GetValue<T>();
        var stringOptions = new List<string> { "None" };
        stringOptions.AddRange(options.Select(getName));

        var dropdown = new DropdownField(
            label: label,
            choices: stringOptions,
            defaultValue: getName(current)
        );

        dropdown.RegisterValueChangedCallback(ValueChangedCallback);
        
        return dropdown;
        
        void ValueChangedCallback(ChangeEvent<string> changeEvent) {
            property.SetValue(getFromName(changeEvent.newValue));
            
            var serializedObject = property.serializedObject;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(serializedObject.targetObject); 
        }
    }
    
    public static string FormatDisplay(string input) {
        var formattedString = Regex.Replace(input, "([A-Z])", " $1").Trim();
        formattedString = char.ToUpper(formattedString[0]) + formattedString[1..];
        return formattedString;
    }

    public static IEnumerable<T> FetchAll<T>() where T : Object {
        return AssetDatabase.FindAssets($"t:{typeof(T).Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>);
    }
}