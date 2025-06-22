using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General_Drone : General_Aireal
{
    public override void Init()
    {
        base.Init();
    }

    public override void Move()
    {
        Vector2 direction = (_destination - (Vector2)transform.position).normalized;
        Vector2 newVelocity = direction * EnemyData.MoveSpeed * 1.5f;

        RB.velocity = newVelocity;
    }

    public override void Attack()
    {
        RB.velocity = Vector3.zero;

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

        string name = gameObject.name;
        int index = name.LastIndexOf('_');

        if (index >= 0)
            name = name.Substring(index + 1);

        GameObject obj;

        if (name.Equals("A"))
            obj = Managers.Resource.Instantiate("Effect/AirealEnemyHit_Blue");
        else
            obj = Managers.Resource.Instantiate("Effect/AirealEnemyHit_Green");

        obj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
    }

    public override void Dead()
    {
        base.Dead();
        Managers.Resource.Instantiate("Effect/EnemyDeadA").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        Managers.Resource.Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 50f);
    }
}
