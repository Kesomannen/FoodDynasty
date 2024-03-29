﻿using Dynasty.Library;
using UnityEngine;

[CreateAssetMenu(menuName = "Pooling/Sprite Renderer")]
public class SpriteRendererPool : UIObjectPool<PoolableComponent<SpriteRenderer>> { }