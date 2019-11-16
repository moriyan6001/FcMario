class FcSmb
{
	private int VerticalPositionOrigin;		// ジャンプ開始時の位置
	private int VerticalPosition;			// 現在位置
	private int VerticalSpeed;				// 速度
	private int VerticalForce;				// 現在の加速度
	private int VerticalForceFall;			// 降下時の加速度
	private int VerticalForceDecimalPart;	// 加速度の増加値
	private int CorrectionValue;			// 累積計算での補正値？
	
	private int HorizontalSpeed = 00;		// 横方向速度

	// ジャンプ開始時の初期パラメータ
	private static readonly byte[]  VerticalForceDecimalPartData	= { 0x20, 0x20, 0x1e, 0x28, 0x28 }; // 加速度の増加値
	private static readonly byte[]  VerticalFallForceData			= { 0x70, 0x70, 0x60, 0x90, 0x90 };	// 降下時の加速度
	private static readonly sbyte[] InitialVerticalSpeedData		= {   -4,   -4,   -4,   -5,   -5 };	// 初速度
	private static readonly byte[]  InitialVerticalForceData		= { 0x00, 0x00, 0x00, 0x00, 0x00 };	// 初期加速度

	// 落下時の最大速度
	private static readonly sbyte DOWN_SPEED_LIMIT = 0x04;

	// 1フレ前のジャンプボタンの押下状態
	private bool JumpBtnPrevPress = false;

	// 地面にいるかジャンプ中か
	public enum MovementState
	{
		OnGround,
		Jumping
	}
	private MovementState CurrentState = MovementState.OnGround;

	public void ResetParam(int initVerticalPos )
	{
		VerticalSpeed = 0;
		VerticalForce = 0;
		VerticalForceFall = 0;
		VerticalForceDecimalPart = 0;
		CurrentState = MovementState.OnGround;
		CorrectionValue = 0;

		VerticalPosition = initVerticalPos;
	}

	public int PosY
	{
		//set { VerticalPosition = value; }
		get { return VerticalPosition; }
	}

	public MovementState GetPlayerState
	{
		get
		{
			return CurrentState;
		}
	}

	public void Movement(bool jumpBtnPress)
	{
		JumpCheck(jumpBtnPress);
		MoveProcess(jumpBtnPress);

		JumpBtnPrevPress = jumpBtnPress;
	}

	private void JumpCheck(bool jumpBtnPress)
	{
		// 初めてジャンプボタンが押された？
		if (jumpBtnPress == false) return;
		if (JumpBtnPrevPress == true) return;

		// 地面上にいる状態？
		if (CurrentState == 0)
		{
			// ジャンプ開始準備
			PreparingJump();
		}
	}

	private void PreparingJump()
	{
		VerticalForceDecimalPart = 0;
		VerticalPositionOrigin = VerticalPosition;

		CurrentState = MovementState.Jumping;

		int idx = 0;
		if (HorizontalSpeed >= 0x1c) idx++;
		if (HorizontalSpeed >= 0x19) idx++;
		if (HorizontalSpeed >= 0x10) idx++;
		if (HorizontalSpeed >= 0x09) idx++;

		VerticalForce				= VerticalForceDecimalPartData[idx];
		VerticalForceFall			= VerticalFallForceData[idx];
		VerticalForceDecimalPart	= InitialVerticalForceData[idx];
		VerticalSpeed				= InitialVerticalSpeedData[idx];
	}

	private void MoveProcess( bool jumpBtnPress)
	{
		// 速度が0かプラスなら画面下へ進んでいるものとして落下状態の加速度に切り替える
		if (VerticalSpeed >= 0)
		{
			VerticalForce = VerticalForceFall;
		}
		else
		{
			// Aボタンが離された&上昇中？
			if (jumpBtnPress == false && JumpBtnPrevPress == true)
			{
				if (VerticalPositionOrigin - VerticalPosition  >= 1)
				{
					// 落下状態の加速度値に切り替える
					VerticalForce = VerticalForceFall;
				}
			}
		}

		Physics();
	}

	private void Physics()
	{
		// 累積計算での補正値っぽい（Qiitaの記事参照）
		int cy = 0;
		CorrectionValue += VerticalForceDecimalPart;
		if (CorrectionValue >= 256)
		{
			CorrectionValue -= 256;
			cy = 1;
		}

		// 現在位置に速度を加算 (累積計算での補正値も加算)
		VerticalPosition += VerticalSpeed + cy;

		// 加速度の固定少数点部への加算
		// 1バイトをオーバーフローしたら、速度が加算される。その時、加速度の整数部は0に戻される
		VerticalForceDecimalPart += VerticalForce;
		if ( VerticalForceDecimalPart >= 256)
		{
			VerticalForceDecimalPart -= 256;
			VerticalSpeed++;
		}

		// 速度の上限チェック
		if ( VerticalSpeed >= DOWN_SPEED_LIMIT)
		{
			// 謎の判定
			if ( VerticalForceDecimalPart >= 0x80)
			{
				VerticalSpeed = DOWN_SPEED_LIMIT;
				VerticalForceDecimalPart = 0x00;
			}
		}
	}
}
