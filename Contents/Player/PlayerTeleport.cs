using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public Transform[] _teleportPos;

    private int _index;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TeleportPlayer();
        }
    }

    private void Start()
    {
        _index = 0;
    }

    public void TeleportPlayer()
    {
        Managers.Game.GetPlayer().transform.position = _teleportPos[_index].position;

        _index++;

        if (_index == _teleportPos.Length)
            _index = 0;
    }
}
