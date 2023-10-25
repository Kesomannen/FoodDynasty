using System;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Machines {

[Serializable]
public class FloatDataProperty {
    [SerializeField] DataObject<float> _dataObject;

    public Modifier Modifier = new(multiplicative: 1);

    public float Value => _dataObject == null ? 0 : (float) Modifier.Apply(_dataObject.Value);
}

}