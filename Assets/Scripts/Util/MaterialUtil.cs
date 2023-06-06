using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MaterialUtil {
    public static void ApplyMaterials(this IEnumerable<Renderer> renderers, IEnumerable<Material> materials) {
        var rendererArray = renderers.ToArray();
        var materialArray = materials.ToArray();

        for (var i = 0; i < rendererArray.Length; i++) {
            rendererArray[i].material = materialArray[i];
        }
    }
    
    public static void ApplyMaterial(this IEnumerable<Renderer> renderers, Material material) {
        foreach (var renderer in renderers) {
            renderer.material = material;
        }
    }
    
    public static IEnumerable<Material> GetMaterials(this IEnumerable<Renderer> renderers) {
        return renderers.Select(renderer => renderer.material);
    }
}