using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField]
    protected Define.State m_state = Define.State.Idle;
    // State를 바꿀 때 Animation또한 같은 상태로 바뀌도록 설정
    public virtual Define.State State       // 해당 프로퍼티대로 상태를 구현해도 되지만 혹시 다르게 구현하고 싶을 수 있으므로 virtual 선언
    {
        get { return m_state; }
        set
        {
            m_state = value;

            Animator anim = GetComponent<Animator>();
            switch (m_state)
            {
                case Define.State.Die:
                    break;
                case Define.State.Idle:
                    anim.CrossFade("WAIT", 0.15f);
                    break;
                case Define.State.Moving:
                    anim.CrossFade("RUN", 0.15f);
                    break;
                case Define.State.Skill:
                    anim.CrossFade("ATTACK", 0.15f, -1, 0);
                    break;
            }
        }
    }

    [SerializeField]
    protected GameObject _lockTarget;   // 찾아야 할 Target(사용 할 Script 기준 Enemy Object)

    [SerializeField]
    protected Vector3 _destPos;         // 이동 시 사용 할 목적지

    // GameManagerEx에서 Despawn()시 해당 Object의 Type을 쉽게 구분하기 위한 프로퍼티 선언
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

    private void Start()
    {
        Init();
    }

    public abstract void Init();

    void Update()
    {
        switch (State)
        {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
        }
    }

    protected virtual void UpdateDie() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateSkill() { }
}
