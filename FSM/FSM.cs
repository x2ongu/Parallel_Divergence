using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T> : MonoBehaviour
{
    private T m_owner;
    private IFSMState<T> m_currentState = null;
    private IFSMState<T> m_previousState = null;
    public IFSMState<T> CurrentState { get { return m_currentState; } }
    public IFSMState<T> PreviousSTate { get { return m_previousState; } }

    protected void InitState(T owner, IFSMState<T> initialState)
    {
        m_owner = owner;
        ChangeState(initialState);
    }

    protected void FSMUpdate()
    {
        if (m_currentState != null)
            m_currentState.Execute(m_owner);
    }

    protected void FSMFixedUpdate()
    {
        if (m_currentState != null)
            m_currentState.FixedExcute(m_owner);
    }

    public void ChangeState(IFSMState<T> newState)
    {
        m_previousState = m_currentState;

        if (m_previousState != null)
        {
            m_previousState.Exit(m_owner);
        }

        m_currentState = newState;

        if (m_currentState != null)
        {
            m_currentState.Enter(m_owner);
        }
    }
}