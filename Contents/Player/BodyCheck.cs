using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCheck : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GeneralEnemyData data = collision.GetComponent<GeneralEnemyData>();

            if (data.IsFixed)
                return;

            GetComponentInParent<PlayerController>().OnHit(1, null, true);
        }
    }
}