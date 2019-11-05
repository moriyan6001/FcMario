using UnityEngine;

public class SmbJumpMain : MonoBehaviour
{
	// レトロゲ座標からUnity座標への変換用
	static readonly float BaseXPos = -120;
	static readonly float BaseYPos = 104;

	[SerializeField] private int FcPosX = 120;
	[SerializeField] private int FcPosY = 176;
	[SerializeField] Sprite[] Images;

	private FcSmb FcSmb;

	// cache
	private Transform JumpManTransform;
	private SpriteRenderer JumpManSprite;

	void Start ()
	{
		JumpManTransform = GetComponent<Transform>();
		JumpManSprite = GetComponent<SpriteRenderer>();

		FcSmb = new FcSmb();
		FcSmb.ResetParam((byte)FcPosY);
	}
	
	void Update ()
	{
		FcSmb.Movement(Input.GetMouseButton(0));

		// 着地
		if ( FcSmb.GetPlayerState == FcSmb.MovementState.Jumping && FcSmb.PosY >= FcPosY )
		{
			FcSmb.ResetParam(FcPosY);
		}

		if (FcSmb.GetPlayerState == FcSmb.MovementState.Jumping)
		{
			JumpManSprite.sprite = Images[1];
		}
		else
		{
			JumpManSprite.sprite = Images[0];
		}
		JumpManTransform.position = FCposToUnityPos(FcPosX, FcSmb.PosY);
	}

	/// <summary>
	/// 左上原点の下方向がプラスとなる座標系からUnityの座標系への変換
	/// </summary>
	/// <param name="x">ファミ座標X</param>
	/// <param name="y">ファミ座標Y</param>
	/// <returns>Unity Sprite座標</returns>
	private Vector2 FCposToUnityPos(int x, int y)
	{
		return new Vector2(x + BaseXPos, -y + BaseYPos);
	}
}
