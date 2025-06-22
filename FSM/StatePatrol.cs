using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePatrol : FSMSingleton<StatePatrol>, IFSMState<StateManager>
{
    public void Enter(StateManager e)
    {
        e.SetMovePosition();
        e.BaseEnemy.IsMove = true;
    }

    public void Execute(StateManager e)
    {
        if (e.EnemyData.Hp <= 0)
            e.ChangeState(StateDie.Instance);

        if (e.CanHit())
            e.ChangeState(StateHit.Instance);

        e.Anim.Play("Move");

        if (!e.CheckGroundFront() || e.CheckWallFront() || e.CheckDestination())
        {
            e.ChangeState(StateStay.Instance);
        }

        if (e.DetectTarget())
        {
            e.ChangeState(StateTrace.Instance);
        }
    }

    public void FixedExcute(StateManager e)
    {
        if (e.BaseEnemy.IsMove)
            e.BaseEnemy.Move();
    }

    public void Exit(StateManager e)
    {
        e.BaseEnemy.IsMove = false;
    }
}
