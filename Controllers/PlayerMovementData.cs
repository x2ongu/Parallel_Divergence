using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerMovementData : ScriptableObject
{
	[Header("Gravity")]
	[HideInInspector] public float gravityStrength;
	[HideInInspector] public float gravityScale;

	[Space(5)]
	[Tooltip("�÷��̾� ���� �� �߷¿� ������ ����")]
	public float fallGravityMult; //�÷��̾ ������ �� gravityScale�� �������� ���.
	[Tooltip("�ְ� ���� �ӵ�")]
	public float maxFallSpeed; //�������� �ִ� �÷��̾��� �ְ� ���� �ӵ�.

	[Space(5)]
	[Header("FastFall")]
	[Tooltip("FastFall �� �߷¿� ������ ����")]
	public float fastFallGravityMult; //�÷��̾ �������� �߿� �Ʒ� Ű�� ���� �� gravityScale�� �������� �� ū ���.
									  //fastFall: ������Ʈ(Celeste)���� �̵�ó�� ������, �÷��̾ ���ϸ� �� ���� ������ �� �ֵ��� ����.
	[Tooltip("FastFall �ְ� ���� �ӵ�")]
	public float maxFastFallSpeed; //������ �������� �ִ� �÷��̾��� �ְ� ���� �ӵ�.

	[Space(20)]

	[Header("Run")]
	[Tooltip("�÷��̾��� �ְ� �ӵ�")]
	public float runMaxSpeed; //�÷��̾ �����ϱ� ���ϴ� ��ǥ �ӵ�.
	[Tooltip("�÷��̾ �ְ� �ӵ����� �����ϴ� �ӵ�")]
	public float runAcceleration; //�÷��̾ �ְ�ӵ��� �����ϴ� �ӵ�, ���� ���ӵ��� 0���� ���߱� ���� runMaxSpeed�� ������ �� �ִ�.
								  // > runMaxSpeed�� ���� ������ ���� ���� �ٷ� �ְ� �ӵ��� ����
	[HideInInspector] public float runAccelAmount; //������ �÷��̾�� ����Ǵ� ��
												   // > �� �� OnValidate()���� ����
	[Tooltip("�÷��̾ ���� �ӵ����� �����ϴ� �ӵ�")]
	public float runDecceleration; //�÷��̾ ����ӵ����� �����ϴ� �ӵ�, ���� ���ӵ��� 0���� ���߱� ���� runMaxSpeed�� ������ �� �ִ�. > runMaxSpeed�� ���� ������ ���� ���� ����
	[HideInInspector] public float runDeccelAmount; //������ �÷��̾�� ����Ǵ� ��
													// > �� �� OnValidate()���� ����
	[Space(5)]
	[Tooltip("���߿� ���� �� ���ӿ� ������ ����")]
	[Range(0f, 1)] public float accelInAir; //���߿� ������ �� ���ӿ� ���õ� ���.
	[Tooltip("���߿� ���� �� ���ӿ� ������ ����")]
	[Range(0f, 1)] public float deccelInAir; //���߿� ������ �� ���ӿ� ���õ� ���.
	[Space(5)]
	public bool doConserveMomentum = true;  // ���� �����ϱ�? > �÷��̾ ���� ���� + maxSpeed �̻����� �� ��� �ӵ� ������ �ʰ� ���� ��������

	[Space(20)]

	[Header("Jump")]
	[Tooltip("���� ����")]
	public int jumpAmount;  // ���� ����
	[Tooltip("�÷��̾��� ���� ����")]
	public float jumpHeight; //�÷��̾��� ���� ����
	[Tooltip("���� �� ������ �����ϴ� �ð�")]
	public float jumpTimeToApex; //�������� �����ϸ� ���ϴ� ���̿� �����ϴ� �ð�. �� ���� gravityStrength�� �Ʒ� jumpForce�� ��� �ϴ� �� ��� ��.
	[HideInInspector] public float jumpForce; //������ �÷��̾�� ����Ǵ� ������
											  // > �� �� OnValidate()���� ����

	[Header("Both Jumps")]
	[Tooltip("������ ���� ��(���� Ű�� �� ��) �߷¿� ������ ����")]
	public float jumpCutGravityMult; //�÷��̾ �����ϴ� ���� ���� ��ư�� ���� �߷��� ������Ű�� ���.
	[Tooltip("(���� Ű�� ������)���� �� �߷¿� ������ ����")]
	[Range(0f, 1)] public float jumpHangGravityMult; //������ ����(���ϴ� �ִ� ����)�� ��������鼭 �߷��� ����.
													 // > �ٿ��� ������ ���� �� �ִ� ������ ��
	[Tooltip("������ �����ϴ� y�� �ӵ�")]
	public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
	[Space(0.5f)]
	[Tooltip("���� �� ���ӵ��� ������ ����")]
	public float jumpHangAccelerationMult;
	[Tooltip("���� �� �ִ� �ӵ��� ������ ����")]
	public float jumpHangMaxSpeedMult;

	[Header("Wall Jump")]
	[Tooltip("�� ���� �� ����Ǵ� ��")]
	public Vector2 wallJumpForce; //�� ������ �� ����Ǵ� ���� �� > x�� Ŭ ���� �¿��, y�� Ŭ ���� ���� ��
	[Space(5)]
	[Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
	[Tooltip("�� ���� �� �ٽ� �� ������ ������ �ð�")]
	[Range(0f, 1.5f)] public float wallJumpTime; //�� ���� ���¸� �����ϴ� �ð� > �� ���� ��� ����� ���� ��Ÿ��� �ö� �� ����
	[Tooltip("�� ���� �� ���� ������ ����")]
	public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction

	[Space(20)]

	[Header("Slide")]
	[Tooltip("�� �����̵� �� �� ��ǥ �ӵ�")]
	public float slideSpeed;
	[Tooltip("�� �����̵� ���ӵ�")]
	public float slideAccel;

	[Header("Assists")]
	[Tooltip("�ڿ��� Ÿ��")]
	[Range(0.01f, 0.5f)] public float coyoteTime; //�ڿ��� Ÿ��: �÷������� ������ �Ŀ��� ������ �� �ִ� ���� �Ⱓ
	[Tooltip("���� �� �Է� ���� �ð�")]
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; //���� ���Է� �ð�: Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

	[Space(20)]

	[Header("Dash")]
	[Tooltip("�뽬 ����")]
	public int dashAmount;  // �뽬 ����
	[Tooltip("�뽬 �ӵ�")]
	public float dashSpeed; // �뽬 �ӵ� > �ø��� �뽬 �Ÿ� ����
	[Tooltip("�뽬 �� ������ '���� ���' ���ߴ� �ð�")]
	public float dashSleepTime; //��ø� ���� ���� �Է��� �а� ���� ���ϱ� ���� ������ ���ߴ� �ð�
	[Space(5)]
	[Tooltip("DashSpeed ���� �ð�")]
	public float dashAttackTime; // �뽬�� ���� �� velocity�� �ӵ��� �����ϴ� �ð�(�뽬�� ���� ���� �ð�)
	[Space(5)]
	[Tooltip("�뽬 �� ���� �ð�:\n�뽬�� ���� �ð����� �뽬 ���ӽð��� �� �ð�")]
	public float dashEndTime; //dashAttackTime���� idle�� Ȥ�� �ٸ� �ִϸ��̼� ���ư� �� ������ �ϰ� �ϱ� ���� �ִ� �ð�

	[Tooltip("�뽬 ���ӽð��� ���� �� ������ �ӵ�")]
	public float dashEndSpeed; //�뽬�� ���� �� ���� �� �ӵ�: �뽬�� ������ ���� �ӵ��� ���� ���Ѽ� ���� �������� �̾����� ����(���� ��� �� ����)
	[Range(0f, 1f)] public float dashEndRunLerp; //Slows the affect of player movement while dashing
	[Space(5)]
	[Tooltip("�뽬 ��Ÿ��")]
	public float dashRefillTime; //�뽬 ��Ÿ��
	[Space(5)]
	[Tooltip("�뽬 �� �Է� ���� �ð�")]
	[Range(0.01f, 0.5f)] public float dashInputBufferTime; // �뽬 ���Է� �ð�


	//�ν����� �󿡼� ��ġ�� ������ ������ �� ȣ��Ǵ� �Լ�
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