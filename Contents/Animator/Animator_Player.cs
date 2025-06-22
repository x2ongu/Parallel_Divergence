using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Animator_Player : Animator_Base
{
    public PlayerController PlayerController { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    [SerializeField] private AttackIndicator[] m_attackIndicators;

    public AnimationCurve _curveAttack1;
    public AnimationCurve _curveAttack2;
    public AnimationCurve _curveAttack3;

    [Header("Effect Position")]
    [SerializeField] private Transform _posAttack1;
    [SerializeField] private Transform _posAttack2;
    [SerializeField] private Transform _posAttack3;
    [SerializeField] private Transform _backHandPos;
    [SerializeField] private Transform _posJumpAttack;
    [SerializeField] private Transform _posRapidStrike;

    private int _jumpAttackCount = 1;
    private int _currentAttackDirection;
    private bool _canReceiveInput = false;
    private bool _inputAttackBuffered = false;
    private bool _comboQueued = false;

    public void OnAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!PlayerController.IsAttacking)
            {
                _comboQueued = false;

                if (PlayerMovement.LastOnGroundTime <= 0)
                {
                    if(_jumpAttackCount > 0)
                    {
                        _jumpAttackCount--;
                        _anim.SetTrigger("Attack");
                    }

                    return;
                }

                _anim.SetTrigger("Attack");
            }
            else if (_canReceiveInput)
            {
                _inputAttackBuffered = true;
            }
        }

        if (_canReceiveInput && _inputAttackBuffered)
        {
            ProceedCombo();
        }
    }

    public override void Init()
    {
        base.Init();

        PlayerController = GetComponent<PlayerController>();
        PlayerMovement = GetComponent<PlayerMovement>();
        m_attackIndicators = GetComponentsInChildren<AttackIndicator>();
    }

    private void Update()
    {
        CheckAnimationIdle();
    }

    private void LateUpdate()
    {
        CheckAnimationState();
    }

    private void CheckAnimationState()
    {        

        if (!PlayerController.CanMove)
        {
            if (PlayerMovement.LastOnGroundTime > 0)
            {
                _anim.SetBool("IsGround", true);
                _anim.SetBool("IsRunning", false);
            }
            return;
        }

        OnAttackInput();

        if (PlayerMovement.IsDashing)
        {
            _anim.Play("Dash");
        }
        else if (PlayerMovement.LastOnGroundTime > 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            _anim.SetBool("IsGround", true);
            _anim.SetBool("IsRunning", false);
        }
        else if (PlayerMovement.LastOnGroundTime > 0 && Input.GetAxisRaw("Horizontal") != 0)
        {
            _anim.SetBool("IsGround", true);
            _anim.SetBool("IsRunning", true);
        }
        else if (PlayerMovement.LastOnGroundTime <= 0)
        {
            _anim.SetBool("IsGround", false);
            _anim.SetFloat("VerticalVelocity", PlayerMovement.RB.velocity.y);
        }

        if (PlayerController.StartedJumping)
        {
            Managers.Resource.Instantiate("Effect/JumpFX").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            PlayerController.StartedJumping = false;
            return;
        }

        if (PlayerController.JustLanded)
        {
            Managers.Resource.Instantiate("Effect/JumpFX").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            _anim.SetBool("IsGround", true);

            _jumpAttackCount = 1;
            if (PlayerController.IsAttacking)
                PlayerController.IsAttacking = false;

            PlayerController.JustLanded = false;

            return;
        }
    }

    private void ProceedCombo()
    {
        //_comboIndex++;
        //if (_comboIndex > 3) _comboIndex = 1;

        _canReceiveInput = false;
        _inputAttackBuffered = false;
        _comboQueued = true;

        _anim.SetBool("ComboQueued", _comboQueued);
        //_anim.SetInteger("ComboIndex", _comboIndex);
    }

    public void Dash(bool isDashing)
    {
        _anim.SetBool("IsDashing", isDashing);
    }

    #region 
    public void Attack(HashSet<Collider2D> collHashSet)
    {
        var copyHashSet = collHashSet.ToList();

        if (copyHashSet.Count != 0)
        {
            Managers.Game.GetCamera()._impulseSource.GenerateImpulse();
        }

        foreach (var coll in copyHashSet)
        {
            if (coll == null)
                continue;

            FindHittableObject(coll, 1);
        }
    }

    public void Bash(HashSet<Collider2D> collHashSet)
    {
        var copyHashSet = collHashSet.ToList();

        if (copyHashSet.Count != 0)
        {
            Managers.Game.GetCamera()._impulseSource.GenerateImpulse();
        }

        foreach (var coll in copyHashSet)
        {
            if (coll == null)
                continue;

            FindHittableObject(coll, 2, true);
        }
    }

    private void FindHittableObject(Collider2D coll, int damage, bool isBash = false)
    {
        //if (coll.gameObject.CompareTag("Enemy"))
        //{
        //    General_Base target = coll.gameObject.GetComponent<General_Base>();

        //    if (target != null)
        //        target.TakeDamage(damage, transform.position.x);
        //    else
        //    {
        //        Debug.Log($"{coll.gameObject.name} is null!!");
        //    }
        //}
        //else if (coll.gameObject.CompareTag("Boss"))
        //{
        //    Boss_Base boss = coll.gameObject.GetComponent<Boss_Base>();

        //    if (boss != null)
        //        boss.TakeDamage(damage);
        //}
        //else if (coll.gameObject.CompareTag("Summoner"))
        //{
        //    Boss_Elysia_Drone target = coll.gameObject.GetComponent<Boss_Elysia_Drone>();

        //    if (target != null)
        //        target.Dead();
        //    else
        //    {
        //        Debug.Log($"{coll.gameObject.name} is null!!");
        //    }
        //}
        //else 
        if (coll.gameObject.CompareTag("DestructibleObject"))
        {
            coll.GetComponent<DestructibleObject>().HitDestructibleObject(isBash);
            RemoveAllTagets(coll);
        }
        else
        {
            var damageable = coll.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnAttacked(damage);
            }
        }
    }
    #endregion

    #region Animation Event
    public void CanMove()
    {
        PlayerController.CanMove = true;
        PlayerController.SuperArmor = false;
        PlayerMovement.HoldAirealPos = false;
    }

    public void Attack1_Start()
    {
        _currentAttackDirection = PlayerController.InputVectorX;

        PlayerController.IsAttacking = true;

        StartCoroutine(AttackWithDash(_curveAttack1, 7f));
    }

    public void Attack2_Start()
    {
        _currentAttackDirection = PlayerController.InputVectorX;

        PlayerController.IsAttacking = true;

        StartCoroutine(AttackWithDash(_curveAttack2, 5f));
    }

    public void Attack3_Start()
    {
        _currentAttackDirection = PlayerController.InputVectorX;

        PlayerController.IsAttacking = true;

        StartCoroutine(AttackWithDash(_curveAttack3, 12f));
    }

    public void EnableComboInputWindow()
    {
        _canReceiveInput = true;
        _inputAttackBuffered = false;
        _comboQueued = false;
        _anim.SetBool("ComboQueued", _comboQueued);
    }

    public void DisableComboInputWindow()
    {
        _canReceiveInput = false;

        if (!_comboQueued)
        {
            _anim.SetBool("ComboQueued", _comboQueued);
        }

        PlayerController.IsAttacking = false;

        _inputAttackBuffered = false;
        _comboQueued = false;
    }

    public void DoAttack()
    {
        Attack(m_attackIndicators[(int)Define.PlayerAttackCollider.Attack].TargetHashSet);
    }

    public void DoJumpAttack()
    {
        Attack(m_attackIndicators[(int)Define.PlayerAttackCollider.JumpAttack].TargetHashSet);
    }

    public void DoBash_Forward()
    {
        Bash(m_attackIndicators[(int)Define.PlayerAttackCollider.Bash_Forward].TargetHashSet);
    }

    public void DoBash_Up()
    {
        Bash(m_attackIndicators[(int)Define.PlayerAttackCollider.Bash_Up].TargetHashSet);
    }

    public void DoBash_Down()
    {
        Bash(m_attackIndicators[(int)Define.PlayerAttackCollider.Bash_Down].TargetHashSet);
    }

    public void RapidStrike()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/RapidStrike");

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        obj.transform.localScale = scale;

        obj.transform.SetPositionAndRotation(_posRapidStrike.position, Quaternion.identity);

        obj.GetComponent<RapidStrike>().DoRapidStrike();
    }

    public void RisingStrike()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/RisingStrike");

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        obj.transform.localScale = scale;

        obj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);

        obj.GetComponent<RisingStrike>().DoRisingStrike();
    }

    public void UltimateSlash()
    {
        // ·¹ÀÌ ½÷¼­
        Vector2 ray = transform.localScale.x == 1 ? Vector2.right : Vector2.left;
        float dirX = transform.localScale.x == 1 ? 1 : -1;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3.up * 5f), ray, 50f, PlayerMovement.m_groundWallLayer);
        float nextDirX;

        if (hit)
        {
            nextDirX = hit.collider.transform.position.x;
            PlayerMovement.RB.MovePosition(hit.collider.transform.position);
        }
        else
        {
            Vector2 nextPos = 50f * dirX * Vector2.right;
            nextDirX = nextPos.x;
            PlayerMovement.RB.MovePosition(transform.position + (Vector3)nextPos);
        }

        Vector3 effectPos = new(transform.position.x + nextDirX, transform.position.y + 7f);

        StartCoroutine(SpawnEffectAfterDelay(effectPos));
    }

    private IEnumerator SpawnEffectAfterDelay(Vector3 effectPos)
    {
        yield return new WaitForSeconds(0.1f);
        GameObject obj = Managers.Resource.Instantiate("Effect/UltimateSlash");

        obj.transform.SetPositionAndRotation(effectPos, Quaternion.identity);

        obj.GetComponent<UltimateSlash>().DoUltimateSlash();
    }

    public void MagnetGloveEffect()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/MagnetGlove");

        obj.transform.SetPositionAndRotation(_backHandPos.position, Quaternion.identity);

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x > 0 ? 1 : -1;
        obj.transform.localScale = scale;

        obj.GetComponent<EffectTracer>().SetTargetAndDamage(_backHandPos, 0);
    }

    public void DoMagneticGlove()
    {
        PlayerController.MagneticGlove();
    }

    public void DoMagnetcGlove_Down()
    {
        PlayerController.MagneticGlove(false);
    }

    public void DoHacking()
    {
        PlayerController.Hacking();
    }

    public void DoStickyBomb()
    {
        PlayerController.SetStickyBomb();
    }

    public void Effect_Attack1()
    {
        SetEffectWithDir(_posAttack1, "Attack1");
    }

    public void Effect_Attack2()
    {
        SetEffectWithDir(_posAttack2, "Attack2");
    }

    public void Effect_Attack3()
    {
        SetEffectWithDir(_posAttack3, "Attack3");
    }

    public void Effect_JumpAttack()
    {
        SetEffectWithDir(_posJumpAttack, "JumpAttack");
    }
    #endregion

    private IEnumerator AttackWithDash(AnimationCurve curve, float distance)
    {
        PlayerMovement.RB.velocity = Vector2.zero;

        Vector2 startPos = transform.position;
        int attackDirection = _currentAttackDirection;

        if (attackDirection != 0)
            PlayerMovement.CheckDirectionToFace(attackDirection > 0);

        Vector2 direction = Vector2.right * attackDirection;

        float animLength = _anim.GetCurrentAnimatorStateInfo(0).length;
        float timer = 0f;

        while (timer < animLength)
        {
            PlayerMovement.RB.velocity = Vector2.zero;


            float progress = timer / animLength;
            float dist = curve.Evaluate(progress) * distance;

            Vector2 nextPos = startPos + direction * dist;
            PlayerMovement.RB.MovePosition(nextPos);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        PlayerMovement.RB.velocity = Vector2.zero;
        yield return null;
    }

    private void RemoveAllTagets(Collider2D collider)
    {
        for (int i = 0; i < m_attackIndicators.Length; i++)
        {
            if (m_attackIndicators[i].TargetHashSet.Contains(collider))
                m_attackIndicators[i].TargetHashSet.Remove(collider);
        }
    }

    private void SetEffectWithDir(Transform spawnTransform,string effectName)
    {
        GameObject obj =  Managers.Resource.Instantiate("Effect/" + effectName, spawnTransform);

        Vector3 scale = obj.transform.localScale;
        scale.x = 1;
        obj.transform.localScale = scale;

        obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}