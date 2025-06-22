using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField]
    protected Define.State m_state = Define.State.Idle;
    // State�� �ٲ� �� Animation���� ���� ���·� �ٲ�� ����
    public virtual Define.State State       // �ش� ������Ƽ��� ���¸� �����ص� ������ Ȥ�� �ٸ��� �����ϰ� ���� �� �����Ƿ� virtual ����
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
    protected GameObject _lockTarget;   // ã�ƾ� �� Target(��� �� Script ���� Enemy Object)

    [SerializeField]
    protected Vector3 _destPos;         // �̵� �� ��� �� ������

    // GameManagerEx���� Despawn()�� �ش� Object�� Type�� ���� �����ϱ� ���� ������Ƽ ����
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
