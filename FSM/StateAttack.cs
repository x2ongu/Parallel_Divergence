using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : FSMSingleton<StateAttack>, IFSMState<StateManager>
{
    public void Enter(StateManager e)
    {
        e.BaseEnemy.IsMove = false;
        e.BaseEnemy.IsAttack = true;
        e._timer = 0;
        e.Anim.Play("Attack");
        e.FacingTarget();
    }

    public void Execute(StateManager e)
    {
        if (e.EnemyData.Hp <= 0 || !e.BaseEnemy.IsLive)
            e.ChangeState(StateDie.Instance);

        if (e.BaseEnemy.OnHit && !e.EnemyData.SuperArmor)
            e.ChangeState(StateHit.Instance);

        e._timer += Time.deltaTime;

        if(!e.BaseEnemy.IsAttack)
        {
            e.Anim.Play("Idle");

            if (e.AttackTimer())
                e.ChangeState(StateStay.Instance);
        }
    }

    public void FixedExcute(StateManager e)
    {
        e.BaseEnemy.RB.velocity = Vector2.zero;
    }

    public void Exit(StateManager e)
    {
        e.BaseEnemy.IsMove = true;
        e.BaseEnemy.IsAttack = false;
    }
}