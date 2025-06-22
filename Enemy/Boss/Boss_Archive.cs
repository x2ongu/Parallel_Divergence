using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss_Archive : Boss_Base
{
    protected enum Pattern
    {
        SummonWeapon,
        ExplosionWave,
        VerticalDrop
    }

    [Header("# SummonWeapon")]
    [SerializeField] float _swRange;

    [Header("# ExplosionWave")]
    [SerializeField] float _ex;

    [Header("# VerticalDrop")]
    [SerializeField] AnimationCurve _vdPreActionCurve;
    [SerializeField] AnimationCurve _vdPatternCurve;
    [SerializeField] float _vdPatternDistance;
    [SerializeField] float _vdCorrectionDistance;
    [SerializeField] float _vdHeight;

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
            case Pattern.SummonWeapon:
                return distance <= _swRange && _previousPatternIndex != (int)Pattern.SummonWeapon;

            case Pattern.ExplosionWave:
                return true;

            case Pattern.VerticalDrop:
                return distance <= _vdPatternDistance;

            default:
                return false;
        }
    }

    protected override IEnumerator ExecutePreAction(int index)
    {
        yield return null;

        switch (index)
        {
            case 0: yield return StartCoroutine(PreAction_SummonWeapon()); break;
            case 1: yield return StartCoroutine(PreAction_ExplosionWave()); break;
            case 2: yield return StartCoroutine(PreAction_VerticalDrop()); break;
        }
    }

    protected override IEnumerator ExecutePattern(int index)
    {
        yield return null;

        switch (index)
        {
            case 0: yield return StartCoroutine(Pattern_SummonWeapon()); break;
            case 1: yield return StartCoroutine(Pattern_ExplosionWave()); break;
            case 2: yield return StartCoroutine(Pattern_VerticalDrop()); break;
        }
    }

    protected override int ChoosePattern()
    {
        var allPatterns = GetAllPatternIndices();

        List<int> usablePatterns = allPatterns
            .Where(i => i != _previousPatternIndex && IsPatternUsable(i))
            .ToList();

        bool canSW = IsPatternUsable((int)Pattern.SummonWeapon);

        if (canSW)
        {
            return (int)Pattern.SummonWeapon;
        }
        else if (!canSW)
        {
            return PatternProbability();
        }

        if (usablePatterns.Count == 0)
            return base.ChoosePattern();

        return usablePatterns[Random.Range(0, usablePatterns.Count)];
    }

    private int PatternProbability()
    {
        float rand = Random.value;

        if (rand < 0.5f)
        {
            _previousPatternIndex = (int)Pattern.ExplosionWave;
        }
        else
        {
            _previousPatternIndex = (int)Pattern.VerticalDrop;
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
    private IEnumerator PreAction_SummonWeapon()
    {
        StartCoroutine(SetAnimationInfo("PreAction_SummonWeapon"));

        FaceTarget(_target.transform);

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        yield return null;
    }

    private IEnumerator PreAction_ExplosionWave()
    {
        StartCoroutine(SetAnimationInfo("PreAction_ExplosionWave"));

        FaceTarget(_target.transform);

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        yield return null;
    }

    private IEnumerator PreAction_VerticalDrop()
    {
        StartCoroutine(SetAnimationInfo("PreAction_VerticalDrop"));

        float timer = 0f;

        Vector2 startPos = transform.position;
        Vector2 targetPos = new(startPos.x, startPos.y + _vdHeight);

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValueY = _vdPreActionCurve.Evaluate(progress);

            _nextPos.y = Mathf.Lerp(startPos.y, targetPos.y, curveValueY);

            yield return null;
        }

        _canMove = false;

        _rigidBody.MovePosition(targetPos);
    }
    #endregion

    #region Pattern
    private IEnumerator Pattern_SummonWeapon()
    {
        StartCoroutine(SetAnimationInfo("Pattern_SummonWeapon"));

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        yield return null;
    }

    private IEnumerator Pattern_ExplosionWave()
    {
        StartCoroutine(SetAnimationInfo("Pattern_ExplosionWave"));

        while (IsAnimationPlaying())
        {
            yield return null;
        }

        yield return null;
    }

    private IEnumerator Pattern_VerticalDrop()
    {
        StartCoroutine(SetAnimationInfo("Pattern_VerticalDrop"));

        float timer = 0;

        Vector2 startPos = transform.position;
        float dirX;
        Vector2 targetPos;

        if (_vdPatternDistance > Mathf.Abs(_target.transform.position.x - transform.position.x))
        {
            dirX = ApproachToTarget(startPos, _vdCorrectionDistance);
            targetPos = new(_target.transform.position.x + dirX, startPos.y - _vdHeight);
        }
        else
        {
            dirX = MoveFowardToTarget(startPos, _vdPatternDistance);
           targetPos = new(startPos.x + dirX, startPos.y - _vdHeight);
        }

        _canMove = true;

        while (IsAnimationPlaying())
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / _duration);
            float curveValueX = _vdPatternCurve.Evaluate(progress);
            float curveValueY = _vdPatternCurve.Evaluate(progress);

            _nextPos.x = Mathf.Lerp(startPos.x, targetPos.x, curveValueX);
            _nextPos.y = Mathf.Lerp(startPos.y, targetPos.y, curveValueY);

            yield return null;
        }

        _canMove = false;
        yield return null;
    }
    #endregion
}
