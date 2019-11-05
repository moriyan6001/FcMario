public class AcDkJump
{
	private int StartYpos;

	private int Counter;
	private int PosYi;
	private int PosYd;

	static readonly int DY_I = 1;
	static readonly int DY_D = 0x48;

	public int PosY
	{
		get { return PosYi; }
	}

	// 地面にいるかジャンプ中か
	public enum MovementState
	{
		OnGround,
		Jumping
	}
	private MovementState CurrentState = MovementState.OnGround;
	public MovementState PlayerState
	{
		get
		{
			return CurrentState;
		}
	}

	// 1フレ前のジャンプボタンの押下状態
	private bool JumpBtnPrevPress = false;

	// constructor
	public AcDkJump(int y)
	{
		StartYpos = y;
		ResetParam();
	}

	public void ResetParam()
	{
		CurrentState = MovementState.OnGround;
		PosYi = StartYpos;
		PosYd = 0;
		Counter = 0;
	}

	public void Movement(bool jumpBtnPress)
	{
		if (jumpBtnPress == true && JumpBtnPrevPress == false && CurrentState == MovementState.OnGround)
		{
			ResetParam();
			CurrentState = MovementState.Jumping;
		}

		int prevYpos = PosYi;

		JumpUpdate();

		// 着地判定
		if (PosYi >= StartYpos)
		{
			PosYi = StartYpos;
			CurrentState = MovementState.OnGround;
		}

		JumpBtnPrevPress = jumpBtnPress;
}

	private void JumpUpdate()
	{
		if (CurrentState == MovementState.OnGround) return;

		// 定数での上昇
		PosYi = PosYi - DY_I;
		PosYd = PosYd - DY_D;
		if (PosYd < 0)
		{
			PosYi--;
			PosYd = 256 + PosYd;
		}

		// フレームカウンタ値から整数・少数を算出する
		int Breg = (Counter >> 4) & 0x0f;
		int Creg = 8 * (Counter * 2 + 1) & 0xff;

		// フレームカウンタ値による位置更新
		PosYi = PosYi + Breg;
		PosYd = PosYd + Creg;
		if ( PosYd >= 256)
		{
			PosYi++;
			PosYd = PosYd - 256;
		}

		Counter++;
    }

}
