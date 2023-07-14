using System.Text;
using UnityEngine;

namespace Dynasty.Library.Extensions {

public static class MatrixExtensions {
    public static T[,] RotateCW<T>(this T[,] matrix, int steps) {
        var rotatedMatrix = matrix;
        for (var i = 0; i < steps; i++) {
            rotatedMatrix = rotatedMatrix.RotateCW();
        }
        return rotatedMatrix;
    }
    
    public static T[,] RotateCW<T>(this T[,] matrix) {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);
        var rotatedMatrix = new T[columns, rows];
        
        for (var i = 0; i < rows; i++) {
            for (var j = 0; j < columns; j++) {
                rotatedMatrix[j, rows - i - 1] = matrix[i, j];
            }
        }
        
        return rotatedMatrix;
    }
    
    public static T[,] RotateCCW<T>(this T[,] matrix, int steps) {
        var rotatedMatrix = matrix;
        for (var i = 0; i < steps; i++) {
            rotatedMatrix = rotatedMatrix.RotateCCW();
        }
        return rotatedMatrix;
    }
    
    public static T[,] RotateCCW<T>(this T[,] matrix) {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);
        var rotatedMatrix = new T[columns, rows];
        
        for (var i = 0; i < rows; i++) {
            for (var j = 0; j < columns; j++) {
                rotatedMatrix[columns - j - 1, i] = matrix[i, j];
            }
        }
        
        return rotatedMatrix;
    }

    public static T GetAt<T>(this T[,] matrix, Vector2Int pos) {
        return matrix[pos.x, pos.y];
    }
    
    public static void SetAt<T>(this T[,] matrix, Vector2Int pos, T value) {
        matrix[pos.x, pos.y] = value;
    }

    public static string GetString<T>(this T[,] matrix) {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);
        var stringBuilder = new StringBuilder();
        
        for (var i = 0; i < rows; i++) {
            stringBuilder.Append("[");
            for (var j = 0; j < columns; j++) {
                stringBuilder.Append(matrix[i, j]);
                if (j < columns - 1) {
                    stringBuilder.Append(", ");
                }
            }
            stringBuilder.Append("]");
            if (i < rows - 1) {
                stringBuilder.Append(", ");
            }
        }
        
        return stringBuilder.ToString();
    }
}

}