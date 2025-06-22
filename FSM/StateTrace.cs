using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTrace : FSMSingleton<StateTrace>, IFSMState<StateManager>
{
    public void Enter(StateManager e)
    {
        e.SetMovePosition(e.Player);
        e.BaseEnemy.IsMove = true;
        e._timer = 0;
    }

    public void Execute(StateManager e)
    {
        if (e.EnemyData.Hp <= 0)
            e.ChangeState(StateDie.Instance);

        if (e.CanHit())
            e.ChangeState(StateHit.Instance);

        e.Anim.Play("Move");

        if (e.EnemyData.IsFixed && !e.EnemyData.IsGroundEnemy)
        {
            if (!e.CanAttack(e.EnemyData.AttackRange))
                e.SetMovePosition(e.Player);
            else
            {
                e.ChangeState(StateAttack.Instance);
            }

            e._timer += Time.deltaTime;

            if (e.Timer(2f) || e.CheckWallFront())
            {
                e.ChangeState(StateAttack.Instance);
            }
        }
        else
        {
            if (e.DetectTarget())
            {
                e.SetMovePosition(e.Player);

                if (e.CheckWallFront())
                    e.ChangeState(StatePatrol.Instance);

                if (e.CanAttack(e.EnemyData.AttackRange))
                    e.ChangeState(StateAttack.Instance);
            }
            else
                e.ChangeState(StateStay.Instance);

            if (!e.CheckGroundFront() || e.CheckDestination())
                e.ChangeState(StateStay.Instance);
        }
    }

    public void FixedExcute(StateManager e)
    {
        if (e.BaseEnemy.IsMove)
            e.BaseEnemy.Move();
    }

    public void Exit(StateManager e)
    {
        e.BaseEnemy.RB.velocity = Vector3.zero;
        e.BaseEnemy.IsMove = false;
        Debug.Log("Trace Done");
    }
}
