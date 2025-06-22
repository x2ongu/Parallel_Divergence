using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Idle,
    PreAction,
    Pattern,
    Cooldown
}

public class BossPatternController : MonoBehaviour
{
    private BossState _state = BossState.Idle;

    [SerializeField] private float idleTime = 1f;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _player;

    private void Start()
    {
        StartCoroutine(BossPatternRoutine());
    }

    private IEnumerator BossPatternRoutine()
    {
        while (true)
        {
            switch (_state)
            {
                case BossState.Idle:
                    yield return StartCoroutine(Idle());
                    break;
                case BossState.PreAction:
                    yield return StartCoroutine(PreAction());
                    break;
                case BossState.Pattern:
                    yield return StartCoroutine(DoPattern());
                    break;
                case BossState.Cooldown:
                    yield return StartCoroutine(Cooldown());
                    break;
            }
        }
    }

    private IEnumerator Idle()
    {
        FacePlayer();
        _anim.Play("Idle");
        yield return new WaitForSeconds(idleTime);
        _state = BossState.PreAction;
    }

    private IEnumerator PreAction()
    {
        // ��ġ �̵� or ���� ����Ʈ ��
        yield return new WaitForSeconds(0.5f); // ����
        _state = BossState.Pattern;
    }

    private IEnumerator DoPattern()
    {
        int patternIndex = ChoosePattern();
        yield return StartCoroutine(ExecutePattern(patternIndex));
        _state = BossState.Cooldown;
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.5f); // ��� ��
        _state = BossState.Idle;
    }

    private int ChoosePattern()
    {
        // ü�� or ������ or ����
        return Random.Range(0, 3); // ����
    }

    private IEnumerator ExecutePattern(int index)
    {
        switch (index)
        {
            case 0: yield return StartCoroutine(PatternDash()); break;
            case 1: yield return StartCoroutine(PatternJumpAttack()); break;
            case 2: yield return StartCoroutine(PatternAOE()); break;
        }
    }

    private void FacePlayer()
    {
        Vector3 scale = transform.localScale;
        scale.x = (_player.position.x < transform.position.x) ? -1 : 1;
        transform.localScale = scale;
    }

    // �� ���� ���ϵ�
    private IEnumerator PatternDash() { /* ���� */ yield return null; }
    private IEnumerator PatternJumpAttack() { /* ���� */ yield return null; }
    private IEnumerator PatternAOE() { /* ���� */ yield return null; }
}