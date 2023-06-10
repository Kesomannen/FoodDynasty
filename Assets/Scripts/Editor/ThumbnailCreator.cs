using System.IO;
using UnityEditor;
using UnityEngine;

public static class ThumbnailCreator {
    static readonly Vector2Int _size = new(512, 512);
    static readonly Vector3 _position = new(0, 50, 0);

    const string AssetPath = "Sprites/Thumbnails";
    const float CameraSize = 1.5f;
    const int Depth = 24;

    public static Sprite Create(Component prefab, string name) {
        var target = Object.Instantiate(prefab, _position, Quaternion.identity);

        var thumbnailHandlers = target.GetComponentsInChildren<IOnThumbnailCreateHandler>();
        foreach (var handler in thumbnailHandlers) {
            handler.OnThumbnailCreate();
        }

        var camera = SetupCamera();

        var renderTexture = new RenderTexture(_size.x, _size.y, Depth);
        var rect = new Rect(0, 0, _size.x, _size.y);
        var texture = new Texture2D(_size.x, _size.y, TextureFormat.RGBA32, false);

        camera.targetTexture = renderTexture;
        camera.Render();

        var currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();
        
        camera.targetTexture = null;
        RenderTexture.active = currentRenderTexture;
        
        Object.DestroyImmediate(renderTexture);
        Object.DestroyImmediate(camera.gameObject);
        Object.DestroyImmediate(target.gameObject);

        return SaveAsset(name, texture);
    }

    static Camera SetupCamera() {
        var aspect = _size.x / (float)_size.y;
        var cameraObject = new GameObject {
            transform = {
                position = _position + new Vector3(-7.5f, 11f, -7.5f),
                rotation = Quaternion.Euler(45, 45, 0)
            }
        };
        var camera = cameraObject.AddComponent<Camera>();

        camera.aspect = aspect;
        camera.orthographic = true;
        camera.orthographicSize = CameraSize;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear;
        return camera;
    }

    static Sprite SaveAsset(string name, Texture2D texture) {
        var projectPath = Path.Combine(AssetPath, $"{name}.png");
        var absolutePath = Path.Combine(Application.dataPath, projectPath);
        projectPath = Path.Combine("Assets", projectPath);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
        File.WriteAllBytes(absolutePath, texture.EncodeToPNG());

        AssetDatabase.Refresh();
        var textureImporter = (TextureImporter)AssetImporter.GetAtPath(projectPath);
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;

        EditorUtility.SetDirty(textureImporter);
        textureImporter.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(projectPath);
    }
}

public interface IOnThumbnailCreateHandler {
    void OnThumbnailCreate();
}