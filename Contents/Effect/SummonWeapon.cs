using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonWeapon : MonoBehaviour
{
    [Header("# Setting")]
    public Rigidbody2D _rigidBody;

    public float _moveSpeed = 20f;
    public float _explodeTimer = 0.3f;
    public float _explodeRange = 5f;

    public void DoAttack(float startTime)
    {
        _rigidBody.bodyType = RigidbodyType2D.Static;
        StartCoroutine(SuicideCoroutine(startTime));
    }

    private void Explosion()
    {
        Collider2D[] coll = Physics2D.OverlapCircleAll(transform.position, _explodeRange);

        foreach (var item in coll)
        {
            if (item.CompareTag("Player"))
            {
                item.GetComponent<PlayerController>().OnHit(3, transform);
            }
        }

        Managers.Game.GetCamera()._impulseSource.GenerateImpulse();
        Destroy();
    }

    private void Destroy()
    {
        Managers.Resource.Instantiate("Effect/Archive/SummonWeapon_Hit").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        Managers.Resource.Destroy(gameObject);
    }

    private IEnumerator SuicideCoroutine(float startTime)
    {
        yield return new WaitForSeconds(startTime);

        _rigidBody.bodyType = RigidbodyType2D.Dynamic;

        Transform target = Managers.Game.GetPlayer().transform;

        float timer = 0f;
        float duration = 2f;

        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;

            Vector3 dir = (target.position + (Vector3.up * 5f)) - transform.position;

            if (Vector3.Distance(target.position + (Vector3.up * 5f), transform.position) < 1f)
            {
                _rigidBody.velocity = Vector3.zero;
                _rigidBody.bodyType = RigidbodyType2D.Static;

                StartCoroutine(ExplodeCoroutine());
                yield break;
            }
            else
            {
                _rigidBody.velocity = dir.normalized * _moveSpeed;
            }

            yield return new WaitForFixedUpdate();
        }

        Explosion();
    }

    private IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        Explosion();
    }
}
