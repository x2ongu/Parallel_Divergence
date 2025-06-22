using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCollect : MonoBehaviour
{
    [Header("# Component")]
    public CircleCollider2D _collider;

    public bool _onAutoCollect;

    public void SetMagnet(bool onAutoCollect, float radius)
    {
        _collider.offset = Vector2.up * 2f;
        _collider.radius = radius;

        _onAutoCollect = onAutoCollect;
    }
}