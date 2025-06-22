using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosition : MonoBehaviour
{
    [SerializeField] GameObject[] _objects;

    [SerializeField] float _playerStartPosX;
    [SerializeField] float _playerStartPosY;

    void Start()
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            _objects[i].transform.position += Vector3.right * -_playerStartPosX;
            _objects[i].transform.position += Vector3.up * -_playerStartPosY;
        }
    }
}
