using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCulling : MonoBehaviour
{
    private Transform _player;
    private MeshRenderer _rend;

    private readonly float _showDistance = 1000;

    void Start()
    {
        _player = Managers.Game.GetPlayer().transform;
        _rend = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        float distX = Mathf.Abs(_player.transform.position.x - transform.position.x);
        bool shouldShow = distX < _showDistance;

        if (_rend.enabled != shouldShow)
        {
            _rend.enabled = shouldShow;
        }
    }
}
