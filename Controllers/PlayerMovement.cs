using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerMovement : MonoBehaviour
{
    public PlayerMovementData Data;

    #region Components
    public Rigidbody2D RB { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public Animator_Player AnimHandler { get; private set; }
    public PartsController PartsController { get; private set; }
    #endregion

    #region State Parameters
    public bool IsFacingRight { get; private set; }     // 오른쪽을 보고 있나요?
    public bool IsJumping { get; set; }         // 점프 중 인가요?
    public bool IsWallJumping { get; private set; }     // 벽 점프 중 인가요?
    public bool IsDashing { get; private set; }         // 대쉬 중 인가요?
    public bool IsFalling { get; private set; }         // 낙하 중 인가요?
    public bool IsSliding { get; private set; }         // 슬라이딩 중 인가요?
    public bool IsStartSliding { get; private set; }    // 슬라이딩을 시작했나요?
    public bool OnSlope { get; private set; }           // 경사에 있나요?
    public bool OnGroundObject { get; private set; }    // 땅(Ground_Object)과 닿아있나요?
    public bool HoldAirealPos { get; set; }

    //Timer
    public float LastOnGroundTime { get; private set; }     // 마지막으로 땅에 있던 시간
    public float LastOnWallTime { get; private set; }       // 마지막으로 벽에 붙어있던 시간
    public float LastOnWallRightTime { get; private set; }  // 마지막으로 오른쪽 벽에 붙어있던 시간
    public float LastOnWallLeftTime { get; private set; }   // 마지막으로 왼쪽 벽에 붙어있던 시간

    //Jump    
    private bool m_isJumpCut;        // 점프가 끝났나요? > 점프키에서 손을 떼면 true로 변해 점프가 끝났다 판단 시 점프가 끝난 중력을 적용
    private bool m_isJumpFalling;    // 점프 후 떨어지고 있나요?
    public int m_jumpsLeft;

    //Wall Jump
    private float m_wallJumpStartTime;   // 벽 점프 시작 시간
    private int m_lastWallJumpDir;       // 마지막 '벽 점프' 방향 > 왼, 오

    //Dash
    private int m_dashesLeft;            // 대쉬 남은 갯수
    private bool m_dashRefilling;        // 대쉬가 리필 되었나요?
    private bool m_isDashAttacking;     // 대쉬를 하기 위해 velocity에 속도를 대입하는 시간: 대쉬에 힘 주는 시간
    #endregion

    #region Input Parameters
    private Vector2 m_moveInput;
    public Vector2 MoveInput { get { return m_moveInput; } }       // 입력 값 Vector
    private Vector2 m_nomalVector;

    public float LastPressedJumpTime { get; private set; }  // 마지막으로 점프를 누른 시간
    public float LastPressedDashTime { get; private set; }  // 마지막으로 대쉬를 누른 시간
    #endregion

    #region Check Parameters
    [Header("Checks")]
    [SerializeField] private Transform m_groundCheckPoint;

    [SerializeField] private Vector2 m_groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform m_frontWallCheckPoint;
    [SerializeField] private Transform m_backWallCheckPoint;
    [SerializeField] private Vector2 m_wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region Layers & Tags
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] public LayerMask m_groundWallLayer;
    #endregion

    private void OnEnable()
    {
        RB = GetComponent<Rigidbody2D>();
        PlayerController = GetComponent<PlayerController>();
        AnimHandler = GetComponent<Animator_Player>();
        PartsController = GetComponent<PartsController>();

        ApplyCustomGravity(Vector2.down, Data.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {
        #region Timers
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;
        #endregion

        #region Collision Check
        //if (!IsDashing && !IsJumping)
        if (!IsJumping)
        {
            //바닥 체크
            if (Physics2D.OverlapBox(m_groundCheckPoint.position, m_groundCheckSize, 0, m_groundLayer | m_groundWallLayer))
            {
                if (LastOnGroundTime < -0.1f)
                {
                    PlayerController.JustLanded = true;
                }
                LastOnGroundTime = Data.coyoteTime;
            }

            //오른쪽 벽 체크
            if (((Physics2D.OverlapBox(m_frontWallCheckPoint.position, m_wallCheckSize, 0, m_groundWallLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(m_backWallCheckPoint.position, m_wallCheckSize, 0, m_groundWallLayer) && !IsFacingRight)) && !IsWallJumping)
                LastOnWallRightTime = Data.coyoteTime;

            //왼쪽 벽 체크
            if (((Physics2D.OverlapBox(m_frontWallCheckPoint.position, m_wallCheckSize, 0, m_groundWallLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(m_backWallCheckPoint.position, m_wallCheckSize, 0, m_groundWallLayer) && IsFacingRight)) && !IsWallJumping)
                LastOnWallLeftTime = Data.coyoteTime;

            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }

        if (Physics2D.Raycast(transform.position, Vector2.down, 2f, (1 << (int)Define.Layer.Ground_Object) | (1 << (int)Define.Layer.Ground_Wall_Object)))
        {
            OnGroundObject = true;
        }
        else
        {
            OnGroundObject = false;
        }

        // 슬로프 체크
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up, Vector2.down, 2f, m_groundLayer);

        if (hit.collider != null && Mathf.Abs(hit.normal.x) != 0 && Mathf.Abs(hit.normal.x) < 0.45f)
        {
            m_nomalVector = hit.normal;

            OnSlope = true;
        }
        else
        {
            OnSlope = false;
        }
        #endregion

        if (RB.velocity.y < 0f && LastOnGroundTime < -Data.jumpTimeToApex * 1.5f)
            IsFalling = true;
        else
            IsFalling = false;

        #region Jump Check
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false;

            m_isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - m_wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            m_isJumpCut = false;

            m_isJumpFalling = false;
        }

        if (!IsDashing)
        {
            if (CanJump() && LastPressedJumpTime > 0)           //Jump
            {
                IsJumping = true;
                IsWallJumping = false;
                m_isJumpCut = false;
                m_isJumpFalling = false;
                Jump();

                PlayerController.StartedJumping = true;
            }
            else if (CanWallJump() && LastPressedJumpTime > 0)  //WALL Jump
            {
                IsWallJumping = true;
                IsJumping = false;
                m_isJumpCut = false;
                m_isJumpFalling = false;

                m_wallJumpStartTime = Time.time;
                m_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;
                WallJump(m_lastWallJumpDir);
            }
        }
        #endregion

        #region Dash Check
        if (CanDash() && LastPressedDashTime > 0)
        {
            Sleep(Data.dashSleepTime);

            IsDashing = true;
            IsJumping = false;
            IsWallJumping = false;
            m_isJumpCut = false;
            AnimHandler.Dash(IsDashing);

            StartCoroutine(nameof(StartDash), IsFacingRight ? Vector2.right : Vector2.left);
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((LastOnWallLeftTime > 0 && m_moveInput.x < 0) || (LastOnWallRightTime > 0 && m_moveInput.x > 0)) && PlayerController._wallStick)
            IsSliding = true;
        else
            IsSliding = false;
        #endregion
    }

    private void FixedUpdate()
    {
        SetGravityScale();

        //Handle Run
        if (!IsDashing)
        {
            if (IsWallJumping)
                Run(Data.wallJumpRunLerp);
            else
                Run(1);
        }
        else if (m_isDashAttacking)
        {
            Run(Data.dashEndRunLerp);
        }

        //Handle Slide
        if (IsSliding)
            Slide();
    }

    public void OnKeyboard()
    {
        if (!PlayerController.CanMove)
        {
            m_moveInput.x = 0;
            return;
        }

        if (PlayerController.IsAttacking)
        {
            if (LastOnGroundTime > 0)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                    PlayerController.InputVectorX = -1;
                else if (Input.GetKey(KeyCode.RightArrow))
                    PlayerController.InputVectorX = 1;
                else
                    PlayerController.InputVectorX = 0;

                return;
            }

            return;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_moveInput.x = Input.GetAxisRaw("Horizontal");
            PlayerController.InputVectorX = m_moveInput.x > 0 ? 1 : -1;
        }
        else
        {
            m_moveInput.x = 0;
            PlayerController.InputVectorX = 0;
        }

        m_moveInput.y = Input.GetAxisRaw("Vertical");

        if (MoveInput.x != 0 && !IsWallJumping && !IsDashing)
            CheckDirectionToFace(MoveInput.x > 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUpInput();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnDashInput();
        }
    }

    #region INPUT CALLBACKS
    //Methods which whandle input detected in Update()
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            m_isJumpCut = true;
    }

    public void OnDashInput()
    {
        LastPressedDashTime = Data.dashInputBufferTime;
    }
    #endregion

    #region General Methods
    public void SetGravityScale()
    {
        if (!m_isDashAttacking)
        {
            if (IsSliding)
            {
                ApplyCustomGravity(Vector2.down, 0);
            }
            else if (OnSlope)
            {
                ApplyCustomGravity(-m_nomalVector, Data.gravityScale);
            }
            else if (HoldAirealPos)
            {
                ApplyCustomGravity(Vector2.down, 0);
            }
            else if (m_isJumpCut)
            {
                ApplyCustomGravity(Vector2.down, Data.gravityScale * Data.jumpCutGravityMult);
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else if ((IsJumping || IsWallJumping || m_isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
            {
                ApplyCustomGravity(Vector2.down, Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (RB.velocity.y < 0 || LastOnGroundTime < -Data.jumpTimeToApex)
            {
                ApplyCustomGravity(Vector2.down, Data.gravityScale * Data.fallGravityMult);
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else
            {
                ApplyCustomGravity(Vector2.down, Data.gravityScale);
            }
        }
        else
        {
            ApplyCustomGravity(Vector2.down, 0);
        }
    }

    public void ApplyCustomGravity(Vector2 gravityDir, float gravityStrength)
    {
        if (RB == null)
            RB = GetComponent<Rigidbody2D>();

        RB.gravityScale = 0;

        RB.AddForce(gravityDir * gravityStrength, ForceMode2D.Force);
    }

    public void Sleep(float duration)
    {
        //Method used so we don't need to call StartCoroutine everywhere
        //nameof() notation means we don't need to input a string directly.
        //Removes chance of spelling mistakes and will improve error messages if any
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
        Time.timeScale = 1;
    }
    #endregion

    #region Run Methods
    private void Run(float lerpAmount)
    {
        if (PlayerController.IsAttacking && LastOnGroundTime > 0)
            return;

        float targetSpeed = m_moveInput.x * Data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || m_isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        if (OnSlope)
        {
            Vector2 moveDir = Vector2.Perpendicular(m_nomalVector).normalized;

            // 경사면의 방향에 따라 moveDir이 왼쪽인지 오른쪽인지 결정해줘야 함
            if (movement < 0 && moveDir.x > 0)
                moveDir *= -1;
            else if (movement > 0 && moveDir.x < 0)
                moveDir *= -1;

            RB.AddForce(moveDir * Mathf.Abs(movement), ForceMode2D.Force);
        }

        RB.AddForce(Vector2.right * movement, ForceMode2D.Force);
        //Convert this to a vector and apply to rigidbody
        //RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    public void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion

    #region Jump Methods
    private void Jump()
    {
        m_jumpsLeft--;

        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = Data.jumpForce;

        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        if (Data.doTurnOnWallJump)
            CheckDirectionToFace(dir > 0);

        #region Perform Wall Jump
        Vector2 force = new(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
        {
            force.x -= RB.velocity.x;
        }

        if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
        {
            force.y -= RB.velocity.y;
        }

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        RB.AddForce(force, ForceMode2D.Impulse);
        #endregion
    }
    #endregion

    #region Dash Methods
    private IEnumerator StartDash(Vector2 dir)
    {
        //Overall this method of dashing aims to mimic Celeste, if you're looking for
        // a more physics-based approach try a method similar to that used in the jump

        LastOnGroundTime = 0;
        LastPressedDashTime = 0;
        float startTime = Time.time;

        m_dashesLeft--;
        m_isDashAttacking = true;

        ApplyCustomGravity(Vector2.down, 0);

        PlayerController.CanHit = false;
        PartsController.StartDashTrail();
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            RB.velocity = dir.normalized * Data.dashSpeed;
            //Pauses the loop until the next frame, creating something of a Update loop. 
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            yield return null;
        }
        startTime = Time.time;

        m_isDashAttacking = false;

        //Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
        ApplyCustomGravity(Vector2.down, Data.gravityScale);
        //SetGravityScale(Data.gravityScale);
        RB.velocity = Data.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= Data.dashEndTime)
        {
            yield return null;
        }
        PartsController.StopDashTrail();
        PlayerController.CanHit = true;
        IsDashing = false;
        AnimHandler.Dash(IsDashing);
    }

    private IEnumerator RefillDash(int amount)
    {
        //SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
        // 확실히 여기 코드를 다시 짜야 대쉬 구현에서 우리 게임이 원하는 방향에 따라 대쉬 할 수 있을 듯?
        m_dashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        m_dashRefilling = false;
        // 대쉬 양(3연대쉬같은거)이랑 남은 대쉬 중에 작은 걸 고르는 거 같은데 흠...이게 한 번에 다 차는게 낫지 않나??
        m_dashesLeft = Mathf.Min(Data.dashAmount, m_dashesLeft + 1);
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        if (IsStartSliding)
            RB.velocity = Vector2.zero;

        //We remove the remaining upwards Impulse to prevent upwards sliding
        if (RB.velocity.y > 0)
        {
            RB.AddForce(-RB.velocity.y * Vector2.up, ForceMode2D.Impulse);
        }

        //Works the same as the Run but only in the y-axis
        //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
        float speedDif = Data.slideSpeed - RB.velocity.y;
        float movement = speedDif * Data.slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        RB.AddForce(movement * Vector2.up);

        IsStartSliding = true;
    }
    #endregion

    #region Check Methods
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        if (LastOnGroundTime > 0 && !IsJumping)
        {
            m_jumpsLeft = Data.jumpAmount;
        }

        if (m_jumpsLeft < Data.jumpAmount)
            return LastOnWallTime <= 0 && RB.velocity.y < 0 && m_jumpsLeft > 0;

        return m_jumpsLeft > 0;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
             (LastOnWallRightTime > 0 && m_lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && m_lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return IsJumping && RB.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && RB.velocity.y > 0;
    }

    private bool CanDash()
    {
        if (!IsDashing && m_dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !m_dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return m_dashesLeft > 0;
    }

    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
            return true;
        else
            return IsStartSliding = false;
    }
    #endregion

    #region Editor Methods
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_groundCheckPoint.position, m_groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(m_frontWallCheckPoint.position, m_wallCheckSize);
        Gizmos.DrawWireCube(m_backWallCheckPoint.position, m_wallCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 1f, Vector3.down * 2f);
    }
    #endregion
}
