using System.Collections.Generic;
using System.Linq;
using GenericUnityObjects;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateGenericAssetMenu]
public class ContainerGridObjectControl<T> : GridObjectControl {
    [SerializeField] Container<T> _containerPrefab;

    readonly List<Container<T>> _storedContainers = new();

    public sealed override bool GetControls(GridObject gridObject, out IEnumerable<Transform> uiElements) {
        uiElements = null;
        var data = gridObject.GetComponents<T>();
        if (data.Length == 0) return false;
        
        for (var i = 0; i < data.Length; i++) {
            Container<T> container;
            if (i < _storedContainers.Count) {
                container = _storedContainers[i];
            } else {
                container = Instantiate(_containerPrefab);
                _storedContainers.Add(container);
            }
            container.SetContent(data[i]);
        }

        while (_storedContainers.Count > data.Length) {
            var lastIndex = _storedContainers.Count - 1;
            Destroy(_storedContainers[lastIndex].gameObject);
            _storedContainers.RemoveAt(lastIndex);
        }

        uiElements = _storedContainers.Select(container => container.transform);
        return true;
    }
}