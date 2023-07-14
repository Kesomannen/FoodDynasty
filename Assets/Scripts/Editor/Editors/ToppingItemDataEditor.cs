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
        foreach (var info in machineInfo) {
            EditorGUILayout.BeginHorizontal();
            
            var inherited = inheritedInfo.Any(inherited => info.Name == inherited);
            var enabled = EditorGUILayout.Toggle($"{info.Name} {info.Value}", inherited);
            switch (enabled) {
                case true when !inherited:
                    inheritedInfo.Add(info.Name); break;
                case false when inherited:
                    inheritedInfo.Remove(info.Name); break;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}