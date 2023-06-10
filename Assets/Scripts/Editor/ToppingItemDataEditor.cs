using System.Linq;
using UnityEditor;

[CustomEditor(typeof(ToppingItemData))]
public class ToppingItemDataEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        var data = (ToppingItemData) target;
        var associatedMachine = data.AssociatedMachine;
        var inheritedInfo = data.InheritedInfo;

        if (associatedMachine == null) return;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Inherited Info", EditorStyles.boldLabel);
        
        var machineInfo = associatedMachine.GetInfo();
        foreach (var (infoName, value) in machineInfo) {
            EditorGUILayout.BeginHorizontal();
            
            var inherited = inheritedInfo.Any(info => info == infoName);
            var enabled = EditorGUILayout.Toggle($"{infoName} {value}", inherited);
            switch (enabled) {
                case true when !inherited:
                    inheritedInfo.Add(infoName); break;
                case false when inherited:
                    inheritedInfo.Remove(infoName); break;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}