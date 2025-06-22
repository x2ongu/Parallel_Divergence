using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General_Spider : General_Ground
{
    private void Update()
    {
        InputInHacking();
    }

    private void FixedUpdate()
    {
        MoveInHacking();
    }

    public override void OnEnableInit()
    {
        base.OnEnableInit();

        EnemyData.SuperArmor = false;
    }

    public override void Move()
    {
        float movement = _destination.x - transform.position.x;
        Vector2 newVelocity = (movement * Vector2.right).normalized * EnemyData.MoveSpeed * 1.5f;        

        newVelocity.y = 0f;

        RB.velocity = newVelocity;
    }

    public void StartAttack()
    {
        EnemyData.SuperArmor = true;
    }

    public override void Attack()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, EnemyData.AttackRange);

        foreach (var coll in colls)
        {
            PlayerController player = coll.GetComponent<PlayerController>();
            if (player != null)
            {
                player.OnHit(EnemyData.Damage, transform);
            }
        }

        EnemyData.Hp = 0;
    }

    public override void OnHitMethod(float targetPosX)
    {
        base.OnHitMethod(targetPosX);

        StartCoroutine(OnHitCoroutine(targetPosX));
    }

    public override void Dead()
    {
        base.Dead();

        Managers.Resource.Instantiate("Effect/EnemyDeadA").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        Managers.Resource.Destroy(gameObject);
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
}
