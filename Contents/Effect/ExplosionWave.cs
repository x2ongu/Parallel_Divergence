using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionWave : MonoBehaviour
{
    [SerializeField] public Collider2D _coll;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().OnHit(4, transform);
        }
    }
}
