using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Elysia_Drone : MonoBehaviour, IDamageable
{
    [Header("# Setting")]
    public Rigidbody2D _rigidBody;
    public Animator _anim;

    public AnimationCurve _curve;

    public float _maxDistance;
    public float _explodeRange;

    public void OnAttacked(int damage, bool hasEffect = false)
    {
        Dead();
    }

    public void DoAttack(float startTime)
    {
        _rigidBody.bodyType = RigidbodyType2D.Static;
        _anim.Play("Idle");
        StartCoroutine(SuicideCoroutine(startTime));
    }

    public void Dead()
    {
        Managers.Resource.Instantiate("Effect/EnemyDeadA").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        Managers.Resource.Destroy(gameObject);
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
        Dead();
    }

    private void FacingTarget()
    {
        Vector3 scale = transform.localScale;
        scale.x = Managers.Game.GetPlayer().transform.position.x - transform.position.x > 0 ? -1 : 1;
        transform.localScale = scale;
    }

    private IEnumerator SuicideCoroutine(float startTime)
    {
        yield return new WaitForSeconds(startTime);

        _anim.Play("Attack");
        FacingTarget();
        _rigidBody.bodyType = RigidbodyType2D.Dynamic;

        Vector2 startPos = transform.position;
        float distX = Managers.Game.GetPlayer().transform.position.x - transform.position.x > 0 ? _maxDistance : -_maxDistance;
        float maxDistX = Mathf.Abs(transform.position.x - Managers.Game.GetPlayer().transform.position.x) > _maxDistance ? transform.position.x + distX : Managers.Game.GetPlayer().transform.position.x;
        Vector2 targetPos = new(maxDistX, Managers.Game.GetPlayer().transform.position.y);

        float timer = 0f;
        float duration = 1.33f;

        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float curveT = _curve.Evaluate(t);

            Vector2 nextPos = Vector2.Lerp(startPos, targetPos, curveT);
            _rigidBody.MovePosition(nextPos);

            yield return new WaitForFixedUpdate();
        }

        Explosion();
    }
}
