using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDie : FSMSingleton<StateDie>, IFSMState<StateManager>
{
    public void Enter(StateManager e)
    {
        e.BaseEnemy.IsLive = false;
        e.BaseEnemy.IsMove = false;
        e.BaseEnemy.Dead();
    }

    public void Execute(StateManager e)
    {
        if (e.BaseEnemy.IsLive)
            e.ChangeState(StateStay.Instance);
    }

    public void FixedExcute(StateManager e)
    {

    }

    public void Exit(StateManager e)
    {
        e.BaseEnemy.IsLive = true;
    }
}