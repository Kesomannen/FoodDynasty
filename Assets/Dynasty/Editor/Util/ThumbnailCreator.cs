using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ThumbnailCreator {
    static readonly Vector2Int _imageSize = new(1024, 1024);
    static readonly Vector3 _position = new(0, 50, 0);

    const string ImagePath = "Sprites/Thumbnails";
    const float CameraSize = 1.5f;
    const int Depth = 24;

    public static Sprite Create(Component prefab, string name, bool cleanup = true) {
        return Create(prefab.gameObject, name, cleanup);
    }
    
    public static Sprite Create(GameObject prefab, string name, bool cleanup = true) {
        var target = Object.Instantiate(prefab, _position, Quaternion.identity);

        var thumbnailHandlers = target.GetComponentsInChildren<IOnThumbnailCreateHandler>();
        foreach (var handler in thumbnailHandlers) {
            handler.OnThumbnailCreate();
        }

        var camera = SetupCamera();

        var renderTexture = new RenderTexture(_imageSize.x, _imageSize.y, Depth);
        var rect = new Rect(0, 0, _imageSize.x, _imageSize.y);
        var texture = new Texture2D(_imageSize.x, _imageSize.y, TextureFormat.RGBA32, false);

        camera.targetTexture = renderTexture;
        camera.Render();

        var oldRenderTexture = RenderTexture.active;
        
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();
        
        camera.targetTexture = null;
        RenderTexture.active = oldRenderTexture;

        Object.DestroyImmediate(renderTexture);
        
        if (cleanup) {
            Object.DestroyImmediate(camera.gameObject);
            Object.DestroyImmediate(target.gameObject);   
        }

        return SaveAsset(name, texture);
    }

    static Camera SetupCamera() {
        var aspect = _imageSize.x / (float)_imageSize.y;
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
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = Color.clear;

        return camera;
    }

    static Sprite SaveAsset(string name, Texture2D texture) {
        var projectPath = Path.Combine(ImagePath, $"{name}.png");
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

    [MenuItem("Dynasty/Generate Thumbnail")]
    static void GenerateThumbnail() {
        foreach (var gameObject in Selection.gameObjects) {
            Create(gameObject, gameObject.name);
        }
    }
    
    [MenuItem("Dynasty/Generate Thumbnail Without Cleanup")]
    static void GenerateThumbnailWithoutCleanup() {
        foreach (var gameObject in Selection.gameObjects) {
            Create(gameObject, gameObject.name, false);
        }
    }
}

public interface IOnThumbnailCreateHandler {
    void OnThumbnailCreate();
}