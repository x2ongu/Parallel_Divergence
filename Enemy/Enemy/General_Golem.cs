using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General_Golem : General_Ground
{
    [Header("# Golem Setting")]
    public GameObject _attackTransform;

    public Vector2 _attackBox;
    public Vector2 _eliteAttackBox;

    private void Update()
    {
        InputInHacking();
    }

    private void FixedUpdate()
    {
        MoveInHacking();
    }

    public override void OnHitMethod(float targetPosX)
    {
        base.OnHitMethod(targetPosX);

        if (!EnemyData.SuperArmor)
            StartCoroutine(OnHitCoroutine(targetPosX));
        else
        {
            Vector3 pos = Vector3.up * 5f;
            Managers.Resource.Instantiate("Effect/AirealEnemyHit_Gray").transform.SetPositionAndRotation(transform.position + pos, Quaternion.identity);
            OnHit = false;
        }
    }

    public override void Dead()
    {
        base.Dead();

        Managers.Resource.Instantiate("Effect/EnemyDeadA").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        Managers.Resource.Destroy(gameObject);
    }

    public override void Attack()
    {
        Collider2D[] colls;

        if (EnemyData.SuperArmor)
            colls = Physics2D.OverlapBoxAll(_attackTransform.transform.position, _eliteAttackBox, 0f);
        else
            colls = Physics2D.OverlapBoxAll (_attackTransform.transform.position, _attackBox, 0f);

        foreach (var coll in colls)
        {
            PlayerController player = coll.GetComponent<PlayerController>();
            if (player != null)
            {
                player.OnHit(EnemyData.Damage, transform);
            }
        }
    }

    private IEnumerator OnHitCoroutine(float targetPosX)
    {
        float timer = 0;

        Vector2 startPos = transform.position;
        float dirX = targetPosX - transform.position.x > 0 ? -1 : 1;
        Vector2 targetPos = transform.position + (3f * dirX * Vector3.right);

        while (timer < 0.2f)
        {
            timer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(timer / 0.2f);

            if (StateManager.CheckGroundBack())
            {
                Vector2 nextPos = Vector2.Lerp(startPos, targetPos, t);
                RB.MovePosition(nextPos);
            }

            yield return new WaitForFixedUpdate();
        }

        OnHit = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(_attackTransform.transform.position, _eliteAttackBox);
        Gizmos.DrawWireCube(_attackTransform.transform.position, _attackBox);
    }
}