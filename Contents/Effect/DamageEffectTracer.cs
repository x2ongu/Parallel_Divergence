using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffectTracer : MonoBehaviour
{
    public Transform _target;

    public int _damage = 2;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().OnHit(_damage, _target);
        }
    }

    void Update()
    {
        if (_target != null)
        {
            transform.SetPositionAndRotation(new Vector3(_target.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        }
    }
}
