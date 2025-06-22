using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHit : FSMSingleton<StateHit>, IFSMState<StateManager>
{
    public void Enter(StateManager e)
    {
        e.BaseEnemy.IsMove = false;
        e.BaseEnemy.OnHit = true;
    }

    public void Execute(StateManager e)
    {
        if (e.EnemyData.Hp <= 0)
            e.ChangeState(StateDie.Instance);

        e.Anim.Play("OnHit");

        if (!e.BaseEnemy.OnHit)
        {
            e.ChangeState(StateStay.Instance);
        }
    }
    public void FixedExcute(StateManager e)
    {

    }

    public void Exit(StateManager e)
    {
        e.BaseEnemy.OnHit = false;
        e.BaseEnemy.IsMove = true;
    }    
}
