using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerMovementData : ScriptableObject
{
	[Header("Gravity")]
	[HideInInspector] public float gravityStrength;
	[HideInInspector] public float gravityScale;

	[Space(5)]
	[Tooltip("플레이어 낙하 시 중력에 곱해질 배율")]
	public float fallGravityMult; //플레이어가 떨어질 때 gravityScale에 곱해지는 배수.
	[Tooltip("최고 낙하 속도")]
	public float maxFallSpeed; //떨어지고 있는 플레이어의 최고 낙하 속도.

	[Space(5)]
	[Header("FastFall")]
	[Tooltip("FastFall 시 중력에 곱해질 배율")]
	public float fastFallGravityMult; //플레이어가 떨어지는 중에 아래 키를 누를 때 gravityScale에 곱해지는 더 큰 배수.
									  //fastFall: 셀레스트(Celeste)게임 이동처럼 보여짐, 플레이어가 원하면 더 빨리 떨어질 수 있도록 해줌.
	[Tooltip("FastFall 최고 낙하 속도")]
	public float maxFastFallSpeed; //빠르게 떨어지고 있는 플레이어의 최고 낙하 속도.

	[Space(20)]

	[Header("Run")]
	[Tooltip("플레이어의 최고 속도")]
	public float runMaxSpeed; //플레이어가 도달하기 원하는 목표 속도.
	[Tooltip("플레이어가 최고 속도까지 가속하는 속도")]
	public float runAcceleration; //플레이어가 최고속도로 가속하는 속도, 순간 가속도를 0으로 낮추기 위해 runMaxSpeed로 세팅할 수 있다.
								  // > runMaxSpeed와 값이 같으면 가속 없이 바로 최고 속도로 진행
	[HideInInspector] public float runAccelAmount; //실제로 플레이어에게 적용되는 힘
												   // > 맨 밑 OnValidate()에서 구현
	[Tooltip("플레이어가 현재 속도에서 감속하는 속도")]
	public float runDecceleration; //플레이어가 현재속도에서 감속하는 속도, 순간 감속도를 0으로 낮추기 위해 runMaxSpeed로 세팅할 수 있다. > runMaxSpeed와 값이 같으면 감속 없이 진행
	[HideInInspector] public float runDeccelAmount; //실제로 플레이어에게 적용되는 힘
													// > 맨 밑 OnValidate()에서 구현
	[Space(5)]
	[Tooltip("공중에 있을 때 가속에 곱해질 배율")]
	[Range(0f, 1)] public float accelInAir; //공중에 떠있을 때 가속에 관련된 배수.
	[Tooltip("공중에 있을 때 감속에 곱해질 배율")]
	[Range(0f, 1)] public float deccelInAir; //공중에 떠있을 때 감속에 관련된 배수.
	[Space(5)]
	public bool doConserveMomentum = true;  // 순간 보존하기? > 플레이어가 가는 방향 + maxSpeed 이상으로 갈 경우 속도 늦추지 않게 순간 보존해줌

	[Space(20)]

	[Header("Jump")]
	[Tooltip("점프 개수")]
	public int jumpAmount;  // 점프 갯수
	[Tooltip("플레이어의 점프 높이")]
	public float jumpHeight; //플레이어의 점프 높이
	[Tooltip("점프 시 정점에 도달하는 시간")]
	public float jumpTimeToApex; //점프력을 적용하며 원하는 높이에 도달하는 시간. 이 값은 gravityStrength와 아래 jumpForce를 계산 하는 데 사용 됨.
	[HideInInspector] public float jumpForce; //실제로 플레이어에게 적용되는 점프력
											  // > 맨 밑 OnValidate()에서 구현

	[Header("Both Jumps")]
	[Tooltip("점프가 끝날 때(점프 키를 뗄 때) 중력에 곱해질 배율")]
	public float jumpCutGravityMult; //플레이어가 점프하는 동안 점프 버튼을 떼면 중력을 증가시키는 배수.
	[Tooltip("(점프 키를 누르며)높점 시 중력에 곱해질 배율")]
	[Range(0f, 1)] public float jumpHangGravityMult; //점프의 정점(원하는 최대 높이)에 가까워지면서 중력을 줄임.
													 // > 줄여서 정점에 조금 떠 있는 느낌을 줌
	[Tooltip("높점을 시작하는 y축 속도")]
	public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
	[Space(0.5f)]
	[Tooltip("높점 시 가속도에 곱해질 배율")]
	public float jumpHangAccelerationMult;
	[Tooltip("높점 시 최대 속도에 곱해질 배율")]
	public float jumpHangMaxSpeedMult;

	[Header("Wall Jump")]
	[Tooltip("벽 점프 시 적용되는 힘")]
	public Vector2 wallJumpForce; //벽 점프할 때 적용되는 실제 힘 > x가 클 수록 좌우로, y가 클 수록 위로 뜀
	[Space(5)]
	[Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
	[Tooltip("벽 점프 후 다시 벽 점프가 가능한 시간")]
	[Range(0f, 1.5f)] public float wallJumpTime; //벽 점프 상태를 유지하는 시간 > 두 개의 길고 가까운 벽을 벽타기로 올라갈 때 생각
	[Tooltip("벽 점프 후 고개를 돌릴지 여부")]
	public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction

	[Space(20)]

	[Header("Slide")]
	[Tooltip("벽 슬라이딩 할 때 목표 속도")]
	public float slideSpeed;
	[Tooltip("벽 슬라이딩 가속도")]
	public float slideAccel;

	[Header("Assists")]
	[Tooltip("코요테 타임")]
	[Range(0.01f, 0.5f)] public float coyoteTime; //코요테 타임: 플랫폼에서 떨어진 후에도 점프할 수 있는 유예 기간
	[Tooltip("점프 선 입력 가능 시간")]
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; //점프 선입력 시간: Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

	[Space(20)]

	[Header("Dash")]
	[Tooltip("대쉬 개수")]
	public int dashAmount;  // 대쉬 갯수
	[Tooltip("대쉬 속도")]
	public float dashSpeed; // 대쉬 속도 > 늘리면 대쉬 거리 증가
	[Tooltip("대쉬 전 게임이 '아주 잠깐' 멈추는 시간")]
	public float dashSleepTime; //대시를 눌러 방향 입력을 읽고 힘을 가하기 전에 게임이 멈추는 시간
	[Space(5)]
	[Tooltip("DashSpeed 지속 시간")]
	public float dashAttackTime; // 대쉬를 시작 해 velocity에 속도를 대입하는 시간(대쉬에 힘이 들어가는 시간)
	[Space(5)]
	[Tooltip("대쉬 중 남는 시간:\n대쉬가 끝난 시간에서 대쉬 지속시간을 뺀 시간")]
	public float dashEndTime; //dashAttackTime이후 idle폼 혹은 다른 애니메이션 돌아갈 때 스무스 하게 하기 위해 있는 시간

	[Tooltip("대쉬 지속시간이 끝날 때 적용할 속도")]
	public float dashEndSpeed; //대쉬가 끝날 때 적용 할 속도: 대쉬가 끝나는 순간 속도를 적용 시켜서 다음 동작으로 이어지기 좋게(값이 쎄면 쭉 나감)
	[Range(0f, 1f)] public float dashEndRunLerp; //Slows the affect of player movement while dashing
	[Space(5)]
	[Tooltip("대쉬 쿨타임")]
	public float dashRefillTime; //대쉬 쿨타임
	[Space(5)]
	[Tooltip("대쉬 선 입력 가능 시간")]
	[Range(0.01f, 0.5f)] public float dashInputBufferTime; // 대쉬 선입력 시간


	//인스펙터 상에서 수치나 변수를 수정할 때 호출되는 함수
	private void OnValidate()
	{
		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		gravityScale = 9.81f * gravityStrength / Physics.gravity.y;

		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		#region Variable Ranges
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
		#endregion
	}
}