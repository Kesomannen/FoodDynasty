using Dynasty.Grid;
using Dynasty.Library;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SerializedGridSize))]
public class SerializedGridSizeDrawer : PropertyDrawer {
    bool _foldout;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var lines = 1;
        var serializedGridSize = property.GetValue<SerializedGridSize>();
        if (serializedGridSize.Value.Type != GridSizeType.Custom) return Result();

        lines++;
        if (_foldout) {
            lines += serializedGridSize.Value.Bounds.y + 1;
        }

        return Result();
        
        float Result() => lines * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var serializedGridSize = property.GetValue<SerializedGridSize>();
        var gridSize = serializedGridSize.Value;

        var rect = EditorGUI.PrefixLabel(position, label);
        
        rect.width /= 2f;
        var newBounds = EditorGUI.Vector2IntField(rect, GUIContent.none, gridSize.Bounds);
        if (newBounds != gridSize.Bounds) {
            gridSize.CustomMatrix = new bool[newBounds.x, newBounds.y];
        }
        gridSize.Bounds = newBounds;
        
        rect.x += rect.width;
        gridSize.Type = (GridSizeType) EditorGUI.EnumPopup(rect, gridSize.Type);
            
        if (gridSize.Type == GridSizeType.Custom) {
            rect.x = position.x;
            rect.y += EditorGUIUtility.singleLineHeight;
            
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = position.width;

            _foldout = EditorGUI.Foldout(rect, _foldout, "Matrix", true);
            if (!_foldout) return;

            rect.x += 10;
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawCustomGrid(rect, ref gridSize);
        }
        
        if (!GUI.changed) return;
        var target = property.serializedObject.targetObject;
            
        serializedGridSize.Value = gridSize;

        EditorUtility.SetDirty(target);
        PrefabUtility.RecordPrefabInstancePropertyModifications(target);
    }

    static void DrawCustomGrid(Rect rect, ref GridSize gridSize) {
        var originalRect = rect;
        rect.width = rect.height = EditorGUIUtility.singleLineHeight;
        
        var matrix = gridSize.CustomMatrix;
        var matrixSize = gridSize.Bounds;
        
        for (var x = 0; x < matrixSize.x; x++) {
            for (var y = 0; y < matrixSize.y; y++) {
                matrix[x, y] = EditorGUI.Toggle(rect, matrix[x, y]);
                rect.y += rect.height;
            }
            
            rect.x += rect.width;
            rect.y = originalRect.y;
        }

        rect.height = EditorGUIUtility.singleLineHeight;
        rect.width = originalRect.width / 2f;
        
        rect.x = originalRect.x;
        rect.y += rect.height * matrixSize.y;
        
        if (GUI.Button(rect, "Enable all")) {
            SetAll(true);
        }
        
        rect.x += rect.width;
        
        if (GUI.Button(rect, "Disable all")) {
            SetAll(false);
        }
        
        gridSize.CustomMatrix = matrix;
        return;

        void SetAll(bool value) {
            for (var x = 0; x < matrixSize.x; x++) {
                for (var y = 0; y < matrixSize.y; y++) {
                    matrix[x, y] = value;
                }
            }
        }
    }
}