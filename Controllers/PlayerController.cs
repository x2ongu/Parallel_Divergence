using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public PlayerData _playerData;
    public Animator m_anim;

    CameraController m_camController;
    PlayerMovement m_playerMovement;
    PartsController m_partsController;

    [SerializeField] private Vector2 _hitReactMagnitude;
    [SerializeField] AnimationCurve _hitReactCurveX;
    [SerializeField] AnimationCurve _hitReactCurveY;
    [SerializeField] float _hitReactDuration;

    public GameObject _hitFace;

    #region Upgrade
    [Header("# Upgrade")]
    public bool _wallStick;

    [Space(5f)] // Bash
    public bool _bashAirJump;
    //public bool _efficientBash;

    //[Space(5f)] // BlastJump

    [Space(5f)] // Stick Boom
    public bool _multipleCarriage;
    public bool _attachedExplosion;

    [Space(5f)] // Magnet Glove
    //public bool _grabRangeIncrease;
    public bool _autoCollect;

    [Space(5f)] // Hacking
    public bool _hackEnemy;
    //public bool _hackSelfExplode;
    #endregion

    #region Bash Components
    [Header("# Bash")]
    public Transform _helperSpwanTransform;

    public AnimationCurve m_bashAnimationCurve;

    public float m_bashDistance;
    public float m_bashDuration;
    #endregion

    #region Magnet Glove Components
    [Header("# Magnet Glove")]
    public Vector2 _magneticGloveBox;
    public Vector2 _magneticGloveBoxOffset;

    public AutoCollect _autoCollector;

    public float _autoCollectorRadius;

    [HideInInspector] public List<Collider2D> _magneticColliderList = new();
    [HideInInspector] public Collider2D _magneticGroundCollider;
    #endregion

    #region Sticky Boom Components
    [Header("# Sticky Boom")]
    public Transform _stickyBombTransform;

    public int _maxBoomCount = 3;
    public int _currentBoomCount = 0;
    #endregion

    #region Hacking Components
    [Header("# Hacking")]
    public Vector2 m_hackingBox;

    public float m_timeToHack = 10f;
    public float m_hackingMaintenanceTime = 30f;
    public float m_hackEnemyCoolTime = 30f;

    private Coroutine m_hackingCoroutine;
    #endregion

    #region Light
    [Header("# Light Component")]
    public Light _light;
    #endregion

    public int InputVectorX { get; set; }

    public bool IsAttacking { get; set; }
    public bool StartedJumping { get; set; }
    public bool JustLanded { get; set; }
    public bool CanMove { get; set; }
    public bool CanHit { get; set; }
    public bool SuperArmor { get; set; }

    private void OnEnable() { Init(); }
    void OnDisable() { m_partsController.SetOriginMaterial(); }
    public void Init()
    {
        m_camController = Managers.Game.GetCamera();
        _playerData = GetComponent<PlayerData>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_partsController = GetComponentInChildren<PartsController>();
        m_anim = GetComponent<Animator>();

        m_partsController.SetOriginMaterial();

        Managers.Input.KeyAction -= m_playerMovement.OnKeyboard;
        Managers.Input.KeyAction += m_playerMovement.OnKeyboard;
        Managers.Input.KeyAction -= OnKeyBoard;
        Managers.Input.KeyAction += OnKeyBoard;

        IsAttacking = false;
        CanMove = true;
        CanHit = true;
    }

    private void OnKeyBoard()
    {
        AutoCollectSwitch();

        if (!CanMove || IsAttacking)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
            OnDead();

        InputSkill();
        InputInteraction();
        InputBash();
        InputStickyBomb();
        InputMagneticGlove();

        if (m_playerMovement.LastOnGroundTime >= 0)
            InputHacking();
    }

    #region Hit & Hit Reaction
    public void OnHit(int damage, Transform attacker = null, bool superArmor = false)
    {
        if (!CanHit)
            return;

        _playerData.TakeDamage(damage);

        if (_playerData.Hp <= 0)
        {
            OnDead();
            return;
        }

        StartCoroutine(OnHitReaction(attacker, superArmor));
    }

    IEnumerator OnHitReaction(Transform attacker, bool superArmor = false)
    {
        CanMove = false;
        CanHit = false;

        _hitFace.SetActive(true);

        if (!superArmor && !SuperArmor)
        {
            if (attacker != null)
            {
                float dirX = attacker.position.x - transform.position.x >= 0 ? 1 : -1;

                if (dirX != transform.localScale.x)
                {
                    m_playerMovement.Turn();
                }
            }

            m_playerMovement.RB.velocity = Vector2.zero;
            m_playerMovement.RB.isKinematic = true;

            m_anim.Play("OnHit");
            m_playerMovement.Sleep(0.15f);

            float timer = 0f;
            Vector2 startPos = m_playerMovement.RB.position;
            float dir = -PlayerDirectionX();

            while (timer < _hitReactDuration)
            {
                timer += Time.fixedDeltaTime;
                float t = Mathf.Clamp01(timer / _hitReactDuration);

                float curveX = _hitReactCurveX.Evaluate(t);
                float curveY = _hitReactCurveY.Evaluate(t);

                Vector2 offset = new(curveX * _hitReactMagnitude.x * dir, curveY * _hitReactMagnitude.y);

                m_playerMovement.RB.MovePosition(startPos + offset);

                yield return new WaitForFixedUpdate();
            }

            m_playerMovement.RB.isKinematic = false;            
        }

        CanMove = true;
        _hitFace.SetActive(false);

        yield return StartCoroutine(m_partsController.SetBlink(1f));

        m_partsController.SetOriginMaterial();

        CanHit = true;
    }

    private void OnDead()
    {
        CanMove = false;
        CanHit = false;

        m_anim.Play("Dead");

        _playerData.Dead();
    }
    #endregion

    #region Skill
    public void InputSkill()
    {
        if (m_playerMovement.LastOnGroundTime > 0)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                StartCoroutine(DoRapidStrike());
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                StartCoroutine(DoUltimateSlash());
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(DoRisingStrike());
        }
    }

    IEnumerator DoRapidStrike()
    {
        CanMove = false;
        IsAttacking = true;
        SuperArmor = true;

        m_anim.Play("RapidStrike");

        float timer = 0f;

        m_playerMovement.RB.velocity = Vector2.zero;

        while (timer < 0.8f)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return null;

        CanMove = true;
        CanHit = true;
        IsAttacking = false;
        SuperArmor = false;
    }

    IEnumerator DoRisingStrike()
    {
        CanMove = false;
        IsAttacking = true;
        SuperArmor = true;
        m_playerMovement.HoldAirealPos = true;

        m_anim.Play("RisingStrike");

        float timer = 0f;

        m_playerMovement.RB.velocity = Vector2.zero;

        while (timer < 1.5f)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return null;

        CanMove = true;
        CanHit = true;
        IsAttacking = false;
    }

    IEnumerator DoUltimateSlash()
    {
        CanMove = false;
        CanHit = false;
        IsAttacking = true;

        m_anim.Play("UltimateSlash");

        float timer = 0f;

        m_playerMovement.RB.velocity = Vector2.zero;

        while (timer < 2f)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return null;

        CanMove = true;
        CanHit = true;
        IsAttacking = false;
    }
    #endregion

    #region Interaction
    public void InputInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractSwitch();
        }
    }

    private void InteractSwitch()
    {
        if (m_playerMovement.LastOnGroundTime < 0)
            return;

        Collider2D switchCollider = Physics2D.OverlapCircle(transform.position, 2f, 1 << (int)Define.Layer.Switch);
        Collider2D teleporterCollider = Physics2D.OverlapCircle(transform.position, 2f, 1 << (int)Define.Layer.Teleport);

        if (switchCollider != null && switchCollider.CompareTag("InteractableObject"))
        {
            MovingPlatformSwitch platformSwitch = switchCollider.GetComponent<MovingPlatformSwitch>();

            if (platformSwitch != null)
                platformSwitch.OnTriggerMovingPlatformSwitch();
        }

        if (teleporterCollider != null && teleporterCollider.CompareTag("InteractableObject"))
        {
            Teleporter teleport = teleporterCollider.GetComponent<Teleporter>();
            if (teleport != null)
            {
                CanMove = false;
                m_anim.Play("Idle");
                teleport.DoTeleport();
            }
        }
    }
    #endregion

    #region Bash
    public void InputBash()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(DoBash(GetInputVector()));
        }
    }

    IEnumerator DoBash(Vector2 inputVec)
    {
        IsAttacking = true;
        SuperArmor = true;
        m_playerMovement.IsJumping = false;

        if (m_playerMovement.LastOnGroundTime > 0)
        {
            if (inputVec == Vector2.up) m_anim.Play("Bash_Up");
            else if (inputVec == Vector2.down) m_anim.Play("Bash_Down");
            else m_anim.Play("Bash_Front");
        }
        else
        {
            if (inputVec == Vector2.up) m_anim.Play("Bash_Up_InAir");
            else if (inputVec == Vector2.down) m_anim.Play("Bash_Down_InAir");
            else m_anim.Play("Bash_Front_InAir");
        }

        SetHelperRachel(inputVec, m_bashDuration);

        Vector2 startPos = gameObject.transform.position;
        Vector2 targetPos = startPos + (inputVec * m_bashDistance);

        float timer = 0f;
        float percentage;

        m_playerMovement.RB.velocity = Vector2.zero;

        while (timer < m_bashDuration)
        {
            timer += Time.fixedDeltaTime;

            percentage = timer / m_bashDuration;

            Vector2 nextPos = Vector2.Lerp(startPos, targetPos, m_bashAnimationCurve.Evaluate(percentage));
            m_playerMovement.RB.MovePosition(nextPos);

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.1f);

        if (_bashAirJump)
            m_playerMovement.m_jumpsLeft++;

        IsAttacking = false;
    }

    private void SetHelperRachel(Vector2 inputVec, float duration)
    {
        GameObject obj = Managers.Resource.Instantiate("Player/Helper_Rachel");
        obj.transform.SetPositionAndRotation(_helperSpwanTransform.position, Quaternion.identity);

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x > 0 ? 1 : -1;
        obj.transform.localScale = scale;

        StartCoroutine(obj.GetComponent<Helper_Rachel>().HelpBash(inputVec, duration));
    }
    #endregion

    #region StickyBoom
    public void InputStickyBomb()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetStickyBombAnim(m_playerMovement.LastOnGroundTime > 0);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            for (int i = StickyBomb.AllBombs.Count - 1; i >= 0; i--)
            {
                StickyBomb.AllBombs[i].ExplodeBomb();
            }
        }
    }

    private void SetStickyBombAnim(bool isGround)
    {
        string animName;

        if (isGround)
            animName = "StickyBomb";
        else
            animName = "StickyBomb_Jump";

        if (_multipleCarriage)
        {
            if (_currentBoomCount >= _maxBoomCount)
                return;

            m_anim.Play(animName);
        }
        else
        {
            if (_currentBoomCount >= 1)
                return;

            m_anim.Play(animName);
        }

        m_playerMovement.RB.velocity = Vector2.zero;
        m_playerMovement.HoldAirealPos = true;
        CanMove = false;
    }

    public void SetStickyBomb()
    {
        _currentBoomCount++;

        GameObject obj = Managers.Resource.Instantiate("Player/StickyBomb");
        obj.transform.position = _stickyBombTransform.position;

        StickyBomb boom = obj.GetComponent<StickyBomb>();
        boom.AttachedExplosion = _attachedExplosion;
        boom.ThrowBomb(PlayerDirectionX());
    }
    #endregion

    #region MagneticGlove
    public void InputMagneticGlove()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                _magneticGroundCollider = GetMagneticGroundCollider();

                if (_magneticGroundCollider == null)
                    return;

                CanMove = false;
                m_anim.Play("MagnetGlove_Down");
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            _magneticColliderList = GetMagneticColliderList();

            SetMagnticGlove(m_playerMovement.LastOnGroundTime > 0);
        }
    }

    public void SetMagnticGlove(bool isGround)
    {
        string animName;

        if (isGround)
            animName = "MagnetGlove";
        else
            animName = "MagnetGlove_Jump";

        m_anim.Play(animName);

        m_playerMovement.RB.velocity = Vector2.zero;
        m_playerMovement.HoldAirealPos = true;
        CanMove = false;
    }

    public void MagneticGlove(bool isTheFront = true)
    {
        if (!isTheFront)
        {
            MagneticObject groundCollider;

            if (_magneticGroundCollider != null)
            {
                groundCollider = _magneticGroundCollider.GetComponent<MagneticObject>();

                if (groundCollider != null)
                {
                    groundCollider.PopUpObject();
                    _magneticGroundCollider = null;
                }
            }

            return;
        }

        if(_magneticColliderList.Count >= 1)
        {
            _magneticColliderList[0].GetComponent<MagneticObject>().PopUpObject();
        }
        else
        {
            Debug.Log("자석 장갑을 적용 할 객체가 없습니다.");
        }

        _magneticGroundCollider = null;
        _magneticColliderList.Clear();
    }

    private List<Collider2D> GetMagneticColliderList()
    {
        Vector2 offsetPosition = new(transform.position.x + (_magneticGloveBoxOffset.x * PlayerDirectionX()), transform.position.y + _magneticGloveBoxOffset.y);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(offsetPosition, _magneticGloveBox, 0, 1 << (int)Define.Layer.Ground_Object);

        List<Collider2D> filtered = new();
        _magneticGroundCollider = GetMagneticGroundCollider();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].CompareTag("MagneticObject"))
                continue;

            if (colliders[i] != _magneticGroundCollider)
                filtered.Add(colliders[i]);
        }

        Collider2D[] objects = filtered.ToArray();

        return SortColliderList(objects);
    }

    private Collider2D GetMagneticGroundCollider()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, m_playerMovement.Data.jumpHeight, 1 << (int)Define.Layer.Ground_Object);

        if (hit && !hit.collider.CompareTag("MagneticObject"))
            return null;

        return hit.collider;
    }

    private void AutoCollectSwitch()
    {
        if (_autoCollect && !_autoCollector._onAutoCollect)
            _autoCollector.SetMagnet(true, _autoCollectorRadius);
        else if (!_autoCollect && _autoCollector._onAutoCollect)
            _autoCollector.SetMagnet(false, _autoCollectorRadius);
    }
    #endregion

    #region Hacking
    public void InputHacking()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            CanMove = false;

            m_anim.Play("Hacking");
        }
    }

    public void Hacking()
    {
        if (m_camController == null)
            m_camController = Managers.Game.GetCamera();

        m_camController.m_virtualCams[(int)Define.VirtualCamera.HackingCam].gameObject.SetActive(true);
        m_camController.m_virtualCams[(int)Define.VirtualCamera.HackingCam].Follow = m_camController.m_followProxy;

        int layerMask;

        if (_hackEnemy)
            layerMask = (1 << (int)Define.Layer.Humanoid) | (1 << (int)Define.Layer.Enemy);
        else
            layerMask = 1 << (int)Define.Layer.Humanoid;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, m_hackingBox, 0, layerMask);

        if (colliders.Length > 0)
        {
            if (m_hackingCoroutine != null)
                StopCoroutine(m_hackingCoroutine);

            m_hackingCoroutine = StartCoroutine(TryHacking(colliders));
        }
        else
        {
            Debug.Log("근처에 해킹 할 대상이 없습니다.");
            EndHacking();
            return;
        }
    }

    IEnumerator TryHacking(Collider2D[] colliders)
    {
        Time.timeScale = 0f;

        List<Collider2D> collList = SortColliderList(colliders);        

        m_camController.m_followProxy.position = collList[0].transform.position;

        GameObject hackingEffect = Managers.Resource.Instantiate("Effect/Hacking");
        hackingEffect.transform.SetPositionAndRotation(m_camController.m_followProxy.position, Quaternion.identity);

        float timer = 0f;
        while (timer < m_timeToHack)
        {
            timer += Time.unscaledDeltaTime;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Managers.Resource.Destroy(hackingEffect);
                Managers.Resource.Destroy(m_camController.m_hackingEffect);
                EndHacking();
                yield break;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Managers.Resource.Destroy(hackingEffect);
                m_camController.SwitchHackingTarget(collList, -1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Managers.Resource.Destroy(hackingEffect);
                m_camController.SwitchHackingTarget(collList, 1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                Managers.Resource.Destroy(hackingEffect);
                Managers.Resource.Destroy(m_camController.m_hackingEffect);
                StartHacking(collList);
                yield break;
            }

            yield return null;
        }

        Managers.Resource.Destroy(hackingEffect);
        Managers.Resource.Destroy(m_camController.m_hackingEffect);
        EndHacking();
    }

    private void StartHacking(List<Collider2D> collList)
    {
        GameObject obj = collList[m_camController.HackingIndex].gameObject;

        if (m_hackingCoroutine != null)
            StopCoroutine(m_hackingCoroutine);

        if (obj.layer == (int)Define.Layer.Humanoid)
        {
            m_hackingCoroutine = StartCoroutine(StartHackingHumanoid(obj));
        }
        else if(obj.layer == (int)Define.Layer.Enemy)
        {
            m_hackingCoroutine = StartCoroutine(StartHackingEnemy(obj));
        }
    }

    private IEnumerator StartHackingHumanoid(GameObject obj)
    {
        Time.timeScale = 1;

        Vector3 originPos = obj.transform.position;

        HackableObject humanoid = obj.GetComponent<HackableObject>();
        humanoid.IsHacked = true;

        m_camController.m_virtualCams[(int)Define.VirtualCamera.HackingCam].Follow = obj.transform;

        float timer = 0f;
        while (timer < m_hackingMaintenanceTime)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || humanoid.IsHacked == false)
            {
                humanoid.IsHacked = false;
                EndHacking();
                obj.transform.position = originPos;
            }

            yield return null;
        }

        humanoid.IsHacked = false;
        EndHacking();
        obj.transform.position = originPos;
    }

    private IEnumerator StartHackingEnemy(GameObject obj)
    {
        Time.timeScale = 1;

        General_Base enemy = obj.GetComponent<General_Base>();
        StateManager stateManager = obj.GetComponent<StateManager>();

        m_camController.m_virtualCams[(int)Define.VirtualCamera.HackingCam].Follow = obj.transform;
        enemy.IsHacked = true;
        stateManager.enabled = false;

        float timer = 0f;
        while (timer < m_hackingMaintenanceTime)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                enemy.IsHacked = false;
                stateManager.enabled = true;
                enemy.EndHacking();
                EndHacking();
            }

            yield return null;
        }

        enemy.IsHacked = false;
        stateManager.enabled = true;
        enemy.EndHacking();
        EndHacking();
    }

    private void EndHacking()
    {
        if (m_camController.m_virtualCams[(int)Define.VirtualCamera.HackingCam].gameObject.activeSelf)
            m_camController.m_virtualCams[(int)Define.VirtualCamera.HackingCam].gameObject.SetActive(false);
        Time.timeScale = 1;
        CanMove = true;
    }
    #endregion

    private List<Collider2D> SortColliderList(Collider2D[] colliders)
    {
        List<Collider2D> collList = new(colliders);

        collList.Sort((a, b) =>
        {
            float distA = Vector2.Distance(transform.position, a.transform.position);
            float distB = Vector2.Distance(transform.position, b.transform.position);

            return distA.CompareTo(distB);
        });

        return collList;
    }

    private Vector2 GetInputVector()
    {
        if (m_playerMovement.MoveInput == Vector2.zero)
        {
            return m_playerMovement.IsFacingRight ? Vector2.right : Vector2.left;
        }
        else if (m_playerMovement.MoveInput.x == 1)
        {
            return Vector2.right;
        }
        else if (m_playerMovement.MoveInput.x == -1)
        {
            return Vector2.left;
        }
        else if (m_playerMovement.MoveInput.y == 1)
        {
            return Vector2.up;
        }
        else if (m_playerMovement.MoveInput.y == -1)
        {
            return Vector2.down;
        }

        return Vector2.zero;
    }

    private float PlayerDirectionX()
    {
        return m_playerMovement.IsFacingRight ? 1 : -1;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, m_hackingBox);
        Vector2 pos = new Vector2(transform.position.x + _magneticGloveBoxOffset.x, _magneticGloveBoxOffset.y);
        Gizmos.DrawWireCube(pos, _magneticGloveBox);
    }
    #endregion
}