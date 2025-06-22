using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss_Elysia : Boss_Base
{
    protected enum Pattern
    {
        HomingBoltVolley,
        StrafeAndShoot,
        DashAndBash,
        DroneTracking,
        OverdriveLaser
    }

    [Header("# Homing Bolt Volley")]
    [SerializeField] AnimationCurve _hbvPreActionCurve;
    [SerializeField] AnimationCurve _hbvPatternCurveX;
    [SerializeField] AnimationCurve _hbvPatternCurveY;
    [SerializeField] float _hbvPreActionDistance;
    [SerializeField] float _hbvPatternDistance;
    [SerializeField] float _hbvCorrectionDistance;
    [SerializeField] float _hbvHeight;

    [Header("# Strafe And Shoot")]
    [SerializeField] AnimationCurve _sasPreActionCurve;
    [SerializeField] float _sasPreActionDistance;
    [SerializeField] float _sasCorrectionDistance;

    [Header("# Dash And Bash")]
    [SerializeField] AnimationCurve _dabPreActionCurve;
    [SerializeField] float _dabPreActionDistance;
    [SerializeField] float _dabCorrectionDistance;

    [Header("# Drone Tracking")]
    [SerializeField] Transform _droneFrontPos;
    [SerializeField] Transform _droneBackPos;
    GameObject _drone1;
    GameObject _drone2;

    [Header("# Overdrive Laser")]
    [SerializeField] AnimationCurve _olPreActionCurve;
    [SerializeField] AnimationCurve _olPatternCurve;
    [SerializeField] float _olPreActionDistance;
    [SerializeField] float _olPatternDistance;
    [SerializeField] float _olCorrectionDistance;

    float _backstepRange;

    private void FixedUpdate()
    {
        if (_canMove && IsLive)
        {
            _rigidBody.MovePosition(_nextPos);
        }
    }

    public override void Init()
    {
        base.Init();

        transform.position = _spawnPoint.position;
        _backstepRange = _dabPreActionDistance - (_dabCorrectionDistance * 2);

        _state = Define.BossState.Idle;
    }

    public override void TakeDamage(int damage)
    {
        if (!IsLive)
            return;

        base.TakeDamage(damage);

        Managers.Resource.Instantiate("Effect/Elysia/Elysia_Hit").transform.SetPositionAndRotation(_center.position, Quaternion.identity);
    }

    protected override IEnumerable<int> GetAllPatternIndices()
    {
        return System.Enum.GetValues(typeof(Pattern)).Cast<int>();
    }

    protected override bool IsPatternUsable(int patternIndex)
    {
        float distance = Mathf.Abs(_target.transform.position.x - transform.position.x);

        switch ((Pattern)patternIndex)
        {
            case Pattern.DashAndBash:
                return distance <= _dabPreActionDistance;

            case Pattern.StrafeAndShoot:
            case Pattern.HomingBoltVolley:
            case Pattern.OverdriveLaser:
                return distance <= _backstepRange;

            case Pattern.DroneTracking:
                return distance <= _dabPreActionDistance && _previousPatternIndex != (int)Pattern.DroneTracking;

            default:
                return true;
        }
    }

    protected override int ChoosePattern()
    {
        var allPatterns = GetAllPatternIndices();

        List<int> usablePatterns = allPatterns
            .Where(i => i != _previousPatternIndex && IsPatternUsable(i))
            .ToList();

        if (Mathf.Abs(_target.transform.position.x - transform.position.x) > _backstepRange)
        {
            bool canDash = IsPatternUsable((int)Pattern.DashAndBash);
            bool canDrone = IsPatternUsable((int)Pattern.DroneTracking);

            if (canDash && canDrone)
            {
                if (Random.value < 0.1f)
                    return (int)Pattern.DroneTracking;
                else
                    return (int)Pattern.DashAndBash;
            }
            else
            {
                _previousPatternIndex = (int)Pattern.DashAndBash;
                _state = Define.BossState.Move;
                return -1;
            }
        }
        else
        {
            float rand = Random.value;

            if (rand < 0.1f)
            {
                _previousPatternIndex = (int)Pattern.OverdriveLaser;
            }
            else if (rand < 0.25f)
            {
                _previousPatternIndex = (int)Pattern.DroneTracking;
            }
            else if (rand < 0.55f)
            {
                _previousPatternIndex = (int)Pattern.StrafeAndShoot;
            }
            else
            {
                _previousPatternIndex = (int)Pattern.HomingBoltVolley;
            }

            if (IsPatternUsable(_previousPatternIndex))
                return _previousPatternIndex;
        }

        if (usablePatterns.Count == 0)
            return base.ChoosePattern(); // base에서 Move 상태 전환 포함

        return usablePatterns[Random.Range(0, usablePatterns.Count)];
    }

    protected override IEnumerator ExecutePreAction(int index)
    {
        yield return null;

        switch (index)
        {
            case 0: yield return StartCoroutine(PreAction_HomingBoltVolley()); break;
            case 1: yield return StartCoroutine(PreAction_StrafeAndShoot()); break;
            case 2: yield return StartCoroutine(PreAction_DashAndBash()); break;
            case 3: yield return StartCoroutine(PreAction_DroneTracking()); break;
            case 4: yield return StartCoroutine(PreAction_OverdriveLaser()); break;
        }
    }
    protected override IEnumerator ExecutePattern(int index)
    {
        yield return null;

        switch (index)
        {
            case 0: yield return StartCoroutine(Pattern_HomingBoltVolley()); break;
            case 1: yield return StartCoroutine(Pattern_StrafeAndShoot()); break;
            case 2: yield return StartCoroutine(Pattern_DashAndBash()); break;
            case 3: yield return StartCoroutine(Pattern_DroneTracking()); break;
            case 4: yield return StartCoroutine(Pattern_OverdriveLaser()); break;
        }
    }

    #region PreAction
    private IEnumerator PreAction_HomingBoltVolley()
    {
        StartCoroutine(SetAnimationInfo("PreAction_HomingBoltVolley"));

        float timer = 0f;

        Vector2 startPos = transform.position;
        float dirX = MoveAwayWithTarget(startPos, false, _hbvPreActionDistance, _hbvCorrectionDistance, _hbvPatternDistance);
        Vector2 targetPos = new(startPos.x + dirX, startPos.y + _hbvHeight);

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValue = _hbvPreActionCurve.Evaluate(progress);

            _nextPos = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        _canMove = false;
        _rigidBody.MovePosition(targetPos);
    }
    private IEnumerator PreAction_StrafeAndShoot()
    {
        StartCoroutine(SetAnimationInfo("PreAction_StrafeAndShoot"));

        Vector2 startPos = transform.position;
        float dirX = MoveAwayWithTarget(startPos, true,_sasPreActionDistance, _sasCorrectionDistance);
        Vector2 targetPos = new(startPos.x + dirX, startPos.y);

        float timer = 0f;

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValue = _sasPreActionCurve.Evaluate(progress);

            _nextPos = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        _canMove = false;
        yield return null;
    }
    private IEnumerator PreAction_DashAndBash()
    {
        if (Vector2.Distance(_target.transform.position, transform.position) < _dabCorrectionDistance)
            yield break;

        StartCoroutine(SetAnimationInfo("PreAction_DashAndBash"));;
        float timer = 0f;

        Vector2 startPos = transform.position;
        float dirX = ApproachToTarget(startPos, _dabCorrectionDistance);
        Vector2 targetPos = new(_target.transform.position.x + dirX, startPos.y);

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValue = _dabPreActionCurve.Evaluate(progress);

            _nextPos = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        _canMove = false;
        yield return null;
    }
    private IEnumerator PreAction_DroneTracking()
    {
        StartCoroutine(SetAnimationInfo("PreAction_DroneTracking"));

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        yield return null;
    }
    private IEnumerator PreAction_OverdriveLaser()
    {
        StartCoroutine(SetAnimationInfo("PreAction_OverdriveLaser"));

        Vector2 startPos = transform.position;
        float dirX = MoveAwayWithTarget(startPos, false, _olPreActionDistance, _olCorrectionDistance);
        Vector2 targetPos = new(startPos.x + dirX, startPos.y);

        float timer = 0f;

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValue = _olPreActionCurve.Evaluate(progress);

            _nextPos = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        _canMove = false;
        yield return null;
    }
    #endregion

    #region Pattern
    private IEnumerator Pattern_HomingBoltVolley()
    {
        StartCoroutine(SetAnimationInfo("Pattern_HomingBoltVolley"));

        Vector2 startPos = transform.position;
        float dirX = MoveAway(_hbvPatternDistance, true);
        Vector2 targetPos = new(startPos.x + dirX, startPos.y - _hbvHeight);

        float timer = 0f;

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValueX = _hbvPatternCurveX.Evaluate(progress);
            float curveValueY = _hbvPatternCurveY.Evaluate(progress);

            _nextPos.x = Mathf.Lerp(startPos.x, targetPos.x, curveValueX);
            _nextPos.y = Mathf.Lerp(startPos.y, targetPos.y, curveValueY);

            yield return null;
        }

        _canMove = false;

        _rigidBody.MovePosition(targetPos);
    }
    private IEnumerator Pattern_StrafeAndShoot()
    {
        StartCoroutine(SetAnimationInfo("Pattern_StrafeAndShoot"));

        yield return null;
        _duration = _anim.GetCurrentAnimatorStateInfo(0).length;

        float timer = 0f;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            yield return null;
        }
    }    
    private IEnumerator Pattern_DashAndBash()
    {
        FaceTarget(_target.transform);
        StartCoroutine(SetAnimationInfo("Pattern_DashAndBash"));

        float timer = 0f;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            yield return null;
        }
    }
    private IEnumerator Pattern_DroneTracking()
    {
        _drone1 = Managers.Resource.Instantiate("Enemy/Elyisa_Drone");
        _drone2 = Managers.Resource.Instantiate("Enemy/Elyisa_Drone");

        _drone1.transform.position = _droneFrontPos.position;
        _drone2.transform.position = _droneBackPos.position;

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? -1 : 1;
        _drone1.transform.localScale = scale;
        _drone2.transform.localScale = scale;

        _drone1.GetComponent<Boss_Elysia_Drone>().DoAttack(1f);
        _drone2.GetComponent<Boss_Elysia_Drone>().DoAttack(2f);


        yield return null;
    }    
    private IEnumerator Pattern_OverdriveLaser()
    {
        StartCoroutine(SetAnimationInfo("Pattern_OverdriveLaser"));

        Vector2 startPos = transform.position;
        float dirX = MoveAway(_olPatternDistance);
        Vector2 targetPos = new(startPos.x + dirX, startPos.y);

        float timer = 0f;

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValue = _olPatternCurve.Evaluate(progress);

            _nextPos = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        _canMove = false;
    }
    #endregion
}