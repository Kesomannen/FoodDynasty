using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dynasty.Library {

public static class MaterialHelpers {
    public static void ApplyMaterials(this IEnumerable<Renderer> renderers, IEnumerable<Material> materials) {
        var rendererArray = renderers.ToArray();
        var materialArray = materials.ToArray();

        for (var i = 0; i < rendererArray.Length; i++) {
            rendererArray[i].material = materialArray[i];
        }
    }
    
    public static void ApplyMaterial(this IEnumerable<Renderer> renderers, Material material) {
        var array = new[] { material };
        foreach (var renderer in renderers) {
            renderer.materials = array;
        }
    }
    
    public static IEnumerable<Material> GetMaterials(this IEnumerable<Renderer> renderers) {
        return renderers.Select(renderer => renderer.material);
    }
}

}