using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScanner : MonoBehaviour
{
    public Collider2D _scanRange;
    public Color _color;

    public float _intensity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Managers.Game.GetPlayer()._light.color = _color;
        Managers.Game.GetPlayer()._light.intensity = _intensity;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Managers.Game.GetPlayer()._light.color = Color.white;
        Managers.Game.GetPlayer()._light.intensity = 15;
    }
}
