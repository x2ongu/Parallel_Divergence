using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : FSM<StateManager>
{
    private GeneralEnemyData _enemyData;
    private General_Base m_baseEnemy;

    public GeneralEnemyData EnemyData { get { return _enemyData; } set { _enemyData = value; } }
    public General_Base BaseEnemy { get { return m_baseEnemy; } set { m_baseEnemy = value; } }

    public Animator Anim { get; set; }
    public Transform Player { get; set; }

    public float _timer;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        FSMUpdate();
    }

    private void FixedUpdate()
    {
        FSMFixedUpdate();
    }

    public void Init()
    {
        EnemyData = gameObject.GetOrAddComponent<GeneralEnemyData>();
        BaseEnemy = GetComponent<General_Base>();
        Anim = GetComponent<Animator>();
        InitState(this, StateStay.Instance);
    }

    public void FacingTarget(float targetPosX = 0f)
    {
        if (targetPosX == 0 && Player == null)
            return;

        float dir;

        if (targetPosX != 0)
            dir = transform.position.x <= targetPosX ? 1 : -1;
        else
            dir = transform.position.x <= Player.position.x ? 1 : -1;

        Vector2 scale = transform.localScale;
        scale.x = dir;
        transform.localScale = scale;
    }

    public Vector2 SetMovePosition(Transform target = null)
    {
        if (target)
        {
            FacingTarget(target.position.x);

            Vector3 adjustedTargetPos = target.position + new Vector3(0, 7f, 0);
            return BaseEnemy._destination = adjustedTargetPos;
        }

        float posX;
        Vector2 movePos;

        do
        {
            posX = Random.Range(-20, 20);

            movePos = new(EnemyData.SpawnPoint.x + posX, BaseEnemy.transform.position.y);

        } while (Mathf.Abs(movePos.x - transform.position.x) < (EnemyData.AttackRange * 0.5f));

        FacingTarget(movePos.x);

        if (!CheckGroundFront())
            movePos.x -= (posX * 2);

        FacingTarget(movePos.x);

        return BaseEnemy._destination = movePos;
    }

    public bool CheckDestination()
    {
        if (!BaseEnemy.IsMove)
            return false;

        if (Vector2.Distance(transform.position, BaseEnemy._destination) < 2)
        {
            return true;
        }

        return false;
    }

    public bool CheckGroundFront()
    {
        if (!EnemyData.IsGroundEnemy)
            return true;

        RaycastHit2D hit = Physics2D.Raycast(BaseEnemy._groundRayFront.position, Vector2.down, 5f + EnemyData.AirealHeight, BaseEnemy._groundCheckLayer);
        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            return true;
        }

        return false;
    }

    public bool CheckGroundBack()
    {
        if (!EnemyData.IsGroundEnemy)
            return true;

        RaycastHit2D hit = Physics2D.Raycast(BaseEnemy._groundRayBack.position, Vector2.down, 5f, BaseEnemy._groundCheckLayer);
        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            return true;
        }

        return false;
    }

    public bool CheckWallFront()
    {
        RaycastHit2D hit = Physics2D.Raycast(BaseEnemy._groundRayFront.position, Vector2.up, 1f, BaseEnemy._groundCheckLayer);
        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            return true;
        }

        return false;
    }

    public bool DetectTarget()
    {
        Collider2D coll = SetDetectCollider(BaseEnemy._type);
        if (coll == null)
        {
            Player = null;
            return false;
        }
        else
        {
            Player = coll.GetComponent<Transform>();
            return true;
        }
    }

    public bool CanAttack(float attackRange)
    {
        float distance = Vector2.Distance(transform.position, Player.position);

        if (distance <= attackRange)
            return true;

        return false;
    }

    public bool CanHit()
    {
        if (BaseEnemy.OnHit && !EnemyData.SuperArmor)
            return true;

        return false;
    }

    public bool Timer(float time)   // 수정 필요
    {
        if (_timer > time)
        {
            return true;
        }
        return false;
    }

    public bool AttackTimer()
    {
        if(_timer > EnemyData.AttackSpeed)
        {
            return true;
        }
        return false;
    }

    private Collider2D SetDetectCollider(Define.EnemyType type)
    {
        Collider2D coll;

        switch (type)
        {
            case Define.EnemyType.Ground:
                coll = Physics2D.OverlapBox(transform.position, new(EnemyData.GroundTraceDistanceX, EnemyData.GroundTraceDistanceY), 0, LayerMask.GetMask("Player"));
                break;

            case Define.EnemyType.Aireal:
                coll = Physics2D.OverlapCircle(transform.position, EnemyData.AirealTraceRange, LayerMask.GetMask("Player"));
                break;

            default:
                coll = null;
                break;
        }

        return coll;
    }
}