using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateStay : FSMSingleton<StateStay>, IFSMState<StateManager>
{
    public void Enter(StateManager e)
    {
        e.BaseEnemy.IsMove = false;
        e._timer = 0f;
        e.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void Execute(StateManager e)
    {
        if (e.EnemyData.Hp <= 0)
            e.ChangeState(StateDie.Instance);

        if (e.CanHit())
            e.ChangeState(StateHit.Instance);

        e.Anim.Play("Idle");
        e._timer += Time.deltaTime;

        if (e.Timer(3f) && !e.EnemyData.IsFixed)
        {
            e.ChangeState(StatePatrol.Instance);
        }

        if (e.CheckGroundFront() && e.DetectTarget())
        {
            e.ChangeState(StateTrace.Instance);
        }
    }

    public void FixedExcute(StateManager e)
    {
        
    }

    public void Exit(StateManager e)
    {
        
    }
}
