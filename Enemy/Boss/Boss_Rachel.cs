using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss_Rachel : Boss_Base
{
    protected enum Pattern
    {
        DoubleJab,
        Uppercut,
        Backstep,
        GroundSlam,
        ChargeFoward
    }

    CharacterTrailer _characterTrailer;

    [Header("# DoubleJab")]
    [SerializeField] float _djRange;

    [Header("# Uppercut")]
    [SerializeField] AnimationCurve _ucPreActionCurve;
    [SerializeField] AnimationCurve _ucPatternCurve;
    [SerializeField] float _ucPreActionDistance;
    [SerializeField] float _ucPatternDistance;

    [Header("# Backstep")]
    [SerializeField] AnimationCurve _bsPreActionCurve;
    [SerializeField] float _bsPreActionDistance;
    [SerializeField] float _bsCorrectionDistance;

    [Header(" GroundSlam")]
    [SerializeField] AnimationCurve _gsPreActionCurveX;
    [SerializeField] AnimationCurve _gsPreActionCurveY;
    [SerializeField] AnimationCurve _gsPatternCurve;
    [SerializeField] float _gsPreActionDistance;
    [SerializeField] float _gsCorrectionDistance;
    [SerializeField] float _gsHeight;

    [Header("ChargeFoward")]
    [SerializeField] AnimationCurve _cfPatternCurve;
    [SerializeField] float _cfPatternDistance;

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

        if (_characterTrailer == null)
            _characterTrailer = GetComponent<CharacterTrailer>();

        transform.position = _spawnPoint.position;
        _backstepRange = 8f;

        _state = Define.BossState.Idle;
    }

    public override void TakeDamage(int damage)
    {
        if (!IsLive)
            return;

        base.TakeDamage(damage);

        Managers.Resource.Instantiate("Effect/Rachel/Rachel_Hit").transform.SetPositionAndRotation(_center.position, Quaternion.identity);
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
            case Pattern.DoubleJab:
                return distance <= _djRange;

            case Pattern.Uppercut:
                return distance <= _ucPreActionDistance + _ucPatternDistance;

            case Pattern.Backstep:
                return distance <= _backstepRange && _previousPatternIndex != (int)Pattern.Backstep;

            case Pattern.GroundSlam:
                return distance <= _gsPreActionDistance;

            case Pattern.ChargeFoward:
                return distance <= _cfPatternDistance;

            default:
                return false;
        }
    }

    protected override IEnumerator ExecutePreAction(int index)
    {
        yield return null;

        switch (index)
        {
            case 0: yield return StartCoroutine(PreAction_DoubleJab()); break;
            case 1: yield return StartCoroutine(PreAction_Uppercut()); break;
            case 2: yield return StartCoroutine(PreAction_Backstep()); break;
            case 3: yield return StartCoroutine(PreAction_GroundSlam()); break;
            case 4: yield return StartCoroutine(PreAction_ChargeFoward()); break;
        }
    }

    protected override IEnumerator ExecutePattern(int index)
    {
        yield return null;

        switch (index)
        {
            case 0: yield return StartCoroutine(Pattern_DoubleJab()); break;
            case 1: yield return StartCoroutine(Pattern_Uppercut()); break;
            case 2: yield return StartCoroutine(Pattern_Backstep()); break;
            case 3: yield return StartCoroutine(Pattern_GroundSlam()); break;
            case 4: yield return StartCoroutine(Pattern_ChargeFoward()); break;
        }
    }

    protected override int ChoosePattern()
    {
        var allPatterns = GetAllPatternIndices();

        List<int> usablePatterns = allPatterns
            .Where(i => i != _previousPatternIndex && IsPatternUsable(i))
            .ToList();

        if (Mathf.Abs(_target.transform.position.x - transform.position.x) < _cfPatternDistance)
        {
            bool canBackstep = IsPatternUsable((int)Pattern.Backstep);

            if (canBackstep)
            {
                if (Random.value < 0.5)
                    return (int)Pattern.Backstep;
                else
                    return PatternProbability();
            }
            else
            {
                return PatternProbability();
            }
        }

        if (usablePatterns.Count == 0)
            return base.ChoosePattern();

        return usablePatterns[Random.Range(0, usablePatterns.Count)];
    }

    private int PatternProbability(int phase = 2)
    {
        float rand = Random.value;

        if (phase == 1)
        {
            if (rand < 0.3f)
            {
                _previousPatternIndex = (int)Pattern.ChargeFoward;
            }
            else if (rand < 0.6f)
            {
                _previousPatternIndex = (int)Pattern.GroundSlam;
            }
            else
            {
                _previousPatternIndex = (int)Pattern.DoubleJab;
            }
        }
        else
        {
            if (rand < 0.25f)
            {
                _previousPatternIndex = (int)Pattern.Uppercut;
            }
            else if (rand < 0.5f)
            {
                _previousPatternIndex = (int)Pattern.GroundSlam;
            }
            else if (rand < 0.75f)
            {
                _previousPatternIndex = (int)Pattern.ChargeFoward;
            }
            else
            {
                _previousPatternIndex = (int)Pattern.DoubleJab;
            }
        }

        if (IsPatternUsable(_previousPatternIndex))
        {
            return _previousPatternIndex;
        }
        else
        {
            _state = Define.BossState.Move;
            return -1;
        }
    }

    #region PreAction
    private IEnumerator PreAction_DoubleJab()
    {
        StartCoroutine(SetAnimationInfo("PreAction_DoubleJab"));

        FaceTarget(_target.transform);

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        yield return null;
    }

    private IEnumerator PreAction_Uppercut()
    {
        StartCoroutine(SetAnimationInfo("PreAction_Uppercut"));
        float timer = 0f;

        Vector2 startPos = transform.position;
        float dirX = MoveFowardToTarget(startPos, _ucPreActionDistance);
        Vector2 targetPos = new(startPos.x + dirX, startPos.y);

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValue = _ucPreActionCurve.Evaluate(progress);

            _nextPos = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        _canMove = false;
        yield return null;
    }

    private IEnumerator PreAction_Backstep()
    {
        StartCoroutine(SetAnimationInfo("PreAction_Backstep"));

        Vector2 startPos = transform.position;
        float dirX = MoveAwayWithTarget(startPos, true, _bsPreActionDistance, _bsCorrectionDistance);
        Vector2 targetPos = new(startPos.x + dirX, startPos.y);

        float timer = 0f;

        _canMove = true;

        _characterTrailer.StartDashTrail();

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValue = _bsPreActionCurve.Evaluate(progress);

            _nextPos = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        _characterTrailer.StopDashTrail();
        _canMove = false;
        yield return null;
    }

    private IEnumerator PreAction_GroundSlam()
    {
        StartCoroutine(SetAnimationInfo("PreAction_GroundSlam")); ;
        float timer = 0f;

        Vector2 startPos = transform.position;
        float dirX = ApproachToTarget(startPos, _gsCorrectionDistance);
        Vector2 targetPos = new(_target.transform.position.x + dirX, startPos.y + _gsHeight);

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValueX = _gsPreActionCurveX.Evaluate(progress);
            float curveValueY = _gsPreActionCurveY.Evaluate(progress);

            _nextPos.x = Mathf.Lerp(startPos.x, targetPos.x, curveValueX);
            _nextPos.y = Mathf.Lerp(startPos.y, targetPos.y, curveValueY);

            yield return null;
        }

        _canMove = false;
        yield return null;
    }

    private IEnumerator PreAction_ChargeFoward()
    {
        StartCoroutine(SetAnimationInfo("PreAction_ChargeFoward"));

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        yield return null;
    }
    #endregion

    #region Pattern
    private IEnumerator Pattern_DoubleJab()
    {
        StartCoroutine(SetAnimationInfo("Pattern_DoubleJab"));

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        yield return null;
    }

    private IEnumerator Pattern_Uppercut()
    {
        StartCoroutine(SetAnimationInfo("Pattern_Uppercut"));
        float timer = 0f;

        Vector2 startPos = transform.position;
        float dirX = MoveFoward(_ucPatternDistance);
        Vector2 targetPos = new(startPos.x + dirX, startPos.y);

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValue = _ucPatternCurve.Evaluate(progress);

            _nextPos = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        _canMove = false;
        yield return null;
    }

    private IEnumerator Pattern_Backstep()
    {
        StartCoroutine(SetAnimationInfo("Pattern_Backstep"));

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        yield return null;
    }

    private IEnumerator Pattern_GroundSlam()
    {
        StartCoroutine(SetAnimationInfo("Pattern_GroundSlam"));

        Vector2 startPos = transform.position;
        Vector2 targetPos = new(startPos.x, startPos.y - _gsHeight);

        float timer = 0f;

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValueY = _gsPatternCurve.Evaluate(progress);

            _nextPos.y = Mathf.Lerp(startPos.y, targetPos.y, curveValueY);

            yield return null;
        }

        _canMove = false;

        _rigidBody.MovePosition(targetPos);
    }

    private IEnumerator Pattern_ChargeFoward()
    {
        StartCoroutine(SetAnimationInfo("Pattern_ChargeFoward"));
        float timer = 0f;

        Vector2 startPos = transform.position;
        float dirX = MoveFoward(_cfPatternDistance);
        Vector2 targetPos = new(startPos.x + dirX, startPos.y);

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValue = _cfPatternCurve.Evaluate(progress);

            _nextPos = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        _canMove = false;
        yield return null;
    }
    #endregion
}
