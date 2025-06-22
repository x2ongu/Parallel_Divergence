using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float _rotateSpeed = 90f;

    void Update()
    {
        transform.Rotate(0f, 0f, _rotateSpeed * Time.deltaTime);
    }
}
