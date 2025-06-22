using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Boss_Base : MonoBehaviour, IDamageable
{
    [Header("# Base Setting")]
    [SerializeField] protected Define.BossState _state;

    [SerializeField] protected Transform _center;

    [SerializeField] protected Transform _spawnPoint;
    [SerializeField] protected Transform _leftWall;
    [SerializeField] protected Transform _rightWall;
    protected Transform _nearbyWall;

    protected PlayerController _target;
    // Stat _stat;
    protected Animator_Base _animBase;
    protected Animator _anim;
    protected Rigidbody2D _rigidBody;
    protected Coroutine _coroutine;

    protected Vector2 _nextPos;

    protected int _previousPatternIndex;
    public int _phase;

    [SerializeField] protected float _moveSpeed;
    [SerializeField] public float _maxHp;
    [HideInInspector] public float _currentHp;
    protected float _duration;

    protected bool IsLive;
    protected bool _canMove;
    protected bool _isPlayingAnim;

    private void OnEnable()
    {
        Init();

        // 등장 씬... 플레이어 못 움직이고 카메라 이동? 하고 등장씬
        //StartCoroutine(BossAppear());
        IsLive = true;
        _coroutine = StartCoroutine(BossPatternRoutine());
    }

    public virtual void Init()
    {
        _target = Managers.Game.GetPlayer();

        _animBase = GetComponent<Animator_Base>();
        _anim = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();

        //_state = Define.BossState.Appear;
        _state = Define.BossState.Idle;

        _phase = 1;
        _previousPatternIndex = 1;

        _canMove = false;
        _currentHp = _maxHp;
    }

    public void OnAttacked(int damage, bool hasEffect = false)
    {
        TakeDamage(damage);

        if (hasEffect)
            Managers.Resource.Instantiate("Effect/UltimateSlash_Hit").transform.SetPositionAndRotation(_center.position, Quaternion.identity);
    }

    public virtual void TakeDamage(int damage)
    {
        if (!IsLive)
            return;

        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            IsLive = false;

            FaceTarget(_target.transform);

            StartCoroutine(BossDisappear());
        }
    }

    protected IEnumerator BossPatternRoutine()
    {
        while (true)
        {
            switch (_state)
            {
                case Define.BossState.Idle:
                    yield return StartCoroutine(Idle());
                    break;
                case Define.BossState.Move:
                    yield return StartCoroutine(Move());
                    break;
                case Define.BossState.PreAction:
                    yield return StartCoroutine(PreAction());
                    break;
                case Define.BossState.Pattern:
                    yield return StartCoroutine(DoPattern());
                    break;
            }
        }
    }

    protected virtual IEnumerator BossAppear()
    {
        _target.CanMove = false;
        _animBase.ViewController(false);
        yield return StartCoroutine(SetAnimationInfo("Appear"));

        _target.CanMove = true;
        _state = Define.BossState.Idle;
        _coroutine = StartCoroutine(BossPatternRoutine());
    }

    protected virtual IEnumerator BossDisappear()
    {
        _state = Define.BossState.Disappear;

        _animBase.ViewController(false);

        StartCoroutine(SetAnimationInfo("Disappear"));

        Time.timeScale = 0.25f;

        yield return new WaitForSeconds(1f);

        Time.timeScale = 1f;

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    protected IEnumerator Idle()
    {
        _animBase.ViewController(true);

        FaceTarget(_target.transform);
        StartCoroutine(SetAnimationInfo("Idle"));

        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        _animBase.ViewController(false);
        _state = Define.BossState.PreAction;
    }

    protected IEnumerator PreAction()
    {
        _animBase.ViewController(false);

        int patternIndex = ChoosePattern();
        if (patternIndex == -1)
        {
            yield break;
        }

        _previousPatternIndex = patternIndex;
        _nextPos = transform.position;
        yield return StartCoroutine(ExecutePreAction(_previousPatternIndex));
        _state = Define.BossState.Pattern;
    }

    protected IEnumerator DoPattern()
    {
        yield return StartCoroutine(ExecutePattern(_previousPatternIndex));
        _state = Define.BossState.Idle;
    }

    protected IEnumerator Move()
    {
        FaceTarget(_target.transform);
        _animBase.ViewController(false);
        StartCoroutine(SetAnimationInfo("Move"));

        while (!IsPatternUsable(_previousPatternIndex))
        {
            float moveDir = _target.transform.position.x - transform.position.x > 0 ? 1 : -1;

            Vector2 velocity = _rigidBody.velocity;
            velocity.x = moveDir * _moveSpeed;
            _rigidBody.velocity = velocity;

            yield return null;
        }

        Vector2 stopVelocity = _rigidBody.velocity;
        stopVelocity.x = 0f;
        _rigidBody.velocity = stopVelocity;

        _state = Define.BossState.PreAction;
    }

    protected virtual int ChoosePattern()
    {
        var usablePatterns = GetAllPatternIndices()
            .Where(pattern => pattern != _previousPatternIndex && IsPatternUsable(pattern))
            .ToList();

        if (usablePatterns.Count == 0)
        {
            if (IsPatternUsable(_previousPatternIndex))
                return _previousPatternIndex;

            _state = Define.BossState.Move;
            return -1;
        }

        int chosen = usablePatterns[Random.Range(0, usablePatterns.Count)];
        return chosen;
    }

    protected abstract IEnumerable<int> GetAllPatternIndices();
    protected abstract bool IsPatternUsable(int patternIndex);
    protected abstract IEnumerator ExecutePreAction(int index);
    protected abstract IEnumerator ExecutePattern(int index);

    public void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x = (scale.x > 0) ? -1 : 1;
        transform.localScale = scale;
    }

    protected void FaceTarget(Transform target, bool isFacing = true)
    {
        Vector3 scale = transform.localScale;
        if (isFacing)
            scale.x = transform.position.x < target.position.x ? 1 : -1;
        else
            scale.x = transform.position.x < target.position.x ? -1 : 1;

        transform.localScale = scale;
    }
    
    protected bool IsAnimationPlaying()
    {
        return _isPlayingAnim;
    }

    protected IEnumerator SetAnimationInfo(string stateName)
    {
        _isPlayingAnim = true;

        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            _anim.Play(stateName);

        yield return null;
        _duration = _anim.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).IsName(stateName));

        yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        _isPlayingAnim = false;
    }

    protected float ApproachToTarget(Vector2 startPos, float correctionDistance = 0)
    {
        FaceTarget(_target.transform);
        return _target.transform.position.x - startPos.x > 0 ?
            -correctionDistance : correctionDistance;
    }

    protected float MoveFowardToTarget(Vector2 startPos, float moveDistance)
    {
        FaceTarget(_target.transform);

        return _target.transform.position.x - startPos.x > 0 ?
        moveDistance: -moveDistance;
    }

    protected float MoveFoward(float moveDistance)
    {
        return transform.localScale.x > 0 ? moveDistance : -moveDistance;
    }

    protected float MoveAwayWithTarget(Vector2 startPos, bool isFacing, float moveDistance, float correctionDistance = 0, float patternDistance = 0)
    {
        float checkDistance = moveDistance;

        if (patternDistance != 0)
            checkDistance += patternDistance;

        if (WallCheck(checkDistance))
        {
            FaceTarget(_nearbyWall, isFacing);

            return _nearbyWall.position.x - startPos.x > 0 ?
               -(moveDistance + correctionDistance) : moveDistance + correctionDistance;
        }
        else
        {
            FaceTarget(_target.transform, isFacing);
            return _target.transform.position.x - startPos.x > 0 ? -moveDistance : moveDistance;
        }
    }

    protected float MoveAway(float moveDistance, bool isTurning = false)
    {
        if (isTurning)
            Turn();

        return transform.localScale.x > 0 ? -moveDistance : moveDistance;
    }

    protected bool WallCheck(float moveDistance)
    {
        float distanceToLeftWall = Mathf.Abs(_leftWall.position.x - transform.position.x);
        float distanceToRightWall = Mathf.Abs(_rightWall.position.x - transform.position.x); ;

        _nearbyWall = distanceToLeftWall < distanceToRightWall ? _leftWall : _rightWall;

        if (Mathf.Abs(_nearbyWall.position.x - transform.position.x) < moveDistance)
            return true;
        else
            return false;
    }
}