using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTracer : MonoBehaviour
{
    public GameObject _hitObj;
    public Transform _target;
    public Transform _attacker;

    public int _damage = 2;

    public bool _facingOnHit = true;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_damage == 0)
                return;

            if (collision.GetComponent<PlayerController>().CanHit)
                Managers.Resource.Instantiate(_hitObj).transform.position = transform.position;

            if (_facingOnHit)
                collision.GetComponent<PlayerController>().OnHit(_damage, _attacker);
            else
                collision.GetComponent<PlayerController>().OnHit(_damage);
        }
    }

    void Update()
    {
        if (_target != null)
        {
            transform.SetPositionAndRotation(new Vector3(_target.position.x, _target.position.y, transform.position.z), Quaternion.identity);
        }
    }

    public void SetTargetAndDamage(Transform transform, int damage, Transform attacker = null)
    {
        if (attacker == null)
            _facingOnHit = false;
        else
            _facingOnHit = true;

        _target = transform;
        _attacker = attacker;
        _damage = damage;
    }
}
