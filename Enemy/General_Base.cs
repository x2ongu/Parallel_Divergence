using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class General_Base : MonoBehaviour, IDamageable
{
    [SerializeField] public Rigidbody2D RB;
    protected GeneralEnemyData EnemyData { get; set; }
    protected StateManager StateManager { get; set; }

    [Header("#Enemy_Base")]
    public Define.EnemyType _type;

    public Vector2 _destination;

    [Header("# Ground Check")]
    public Transform _groundRayFront;
    public Transform _groundRayBack;
    public LayerMask _groundCheckLayer;

    public bool IsAttack { get; set; }
    public bool IsLive { get; set; }
    public bool IsMove { get; set; }
    public bool OnHit { get; set; }

    #region Hacking Setting
    protected Vector2 _inputVector;

    private bool _isHacked;
    public bool IsHacked { get { return _isHacked; } set { _isHacked = value; } }
    #endregion

    void Start() { Init(); }

    private void OnEnable() { OnEnableInit(); }

    public virtual void Init()
    {
        EnemyData = gameObject.GetOrAddComponent<GeneralEnemyData>();
        StateManager = gameObject.GetOrAddComponent<StateManager>();
        RB = GetComponent<Rigidbody2D>();
    }

    public virtual void OnEnableInit()
    {
        IsLive = true;

        if (EnemyData == null)
            EnemyData = gameObject.GetOrAddComponent<GeneralEnemyData>();

        if (EnemyData.SpawnPoint == Vector2.zero)
            EnemyData.SpawnPoint = transform.position;

        EnemyData.Hp = EnemyData.MaxHp;
        transform.position = EnemyData.SpawnPoint;
    }

    public abstract void Move();
    public abstract void Attack();

    public void FinsihAttack()
    {
        IsAttack = false;
    }

    public void OnAttacked(int damage, bool hasEffect = false)
    {
        if (hasEffect)
        {
            Managers.Resource.Instantiate("Effect/UltimateSlash_Hit").transform.SetPositionAndRotation(transform.position, Quaternion.identity);

            TakeDamage(damage);

            OnHit = false;
        }
        else
            TakeDamage(damage, Managers.Game.GetPlayer().transform.position.x);
    }

    public void TakeDamage(int damage, float targetPosX = 0)
    {
        Debug.Log($"{gameObject.name}은 공격을 받았다!!");
        OnHit = true;

        if (damage == -1)
        {
            Dead();
            return;
        }

        EnemyData.TakeDamage(damage);

        if (targetPosX != 0)
            OnHitMethod(targetPosX);
    }

    public virtual void OnHitMethod(float targetPosX)
    {
        if (!EnemyData.SuperArmor)
            StateManager.FacingTarget(targetPosX);
    }

    public virtual void Dead()
    {
        IsLive = false;
        EnemyData.Hp = 0;
        RB.simulated = true;
    }

    public virtual void InputInHacking() { }

    public virtual void MoveInHacking()
    {
        if (!IsHacked)
            return;

        if (StateManager.CheckGroundFront())
            RB.velocity = _inputVector * EnemyData.MoveSpeed;
        else
            RB.velocity = Vector3.zero;
    }

    public void EndHacking()
    {
        TakeDamage(-1, transform.position.x);
        Debug.Log("폭파...");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(RB.transform.position, 10f, 1 << (int)Define.Layer.Enemy);

        for (int i = colliders.Length - 1; i >= 0; i--)
        {
            colliders[i].GetComponent<General_Base>().TakeDamage(5, transform.position.x);
        }

        Destroy(gameObject);
    }    
}
