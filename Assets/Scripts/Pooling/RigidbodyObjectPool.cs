using Dynasty.Library.Pooling;
using UnityEngine;

[CreateAssetMenu(menuName = "Pooling/Rigidbody")]
public class RigidbodyObjectPool : CustomObjectPool<PoolableComponent<Rigidbody>> { }