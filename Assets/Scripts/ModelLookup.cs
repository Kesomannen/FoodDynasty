using System.Linq;
using Dynasty.Library;
using Dynasty.Persistent.Mapping;
using UnityEngine;

[CreateAssetMenu(menuName = "Saving/Model Lookup")]
public class ModelLookup : Lookup<CustomObjectPool<Poolable>> { }