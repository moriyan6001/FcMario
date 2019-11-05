using UnityEngine;

public class DkJumpMain : MonoBehaviour
{
	// レトロゲ座標からUnity座標への変換用
	static readonly float BaseXPos = -120;
	static readonly float BaseYPos = 104;

	[SerializeField] private int FcPosX = 120;
	[SerializeField] private int FcPosY = 176;
	[SerializeField] Sprite[] images;

	private AcDkJump Player;

	// cache
	private Transform JumpManTransform;
	private SpriteRenderer JumpManSprite;

	void Start ()
	{
		JumpManTransform = GetComponent<Transform>();
		JumpManSprite = GetComponent<SpriteRenderer>();

		Player = new AcDkJump(FcPosY);
	}
	
	void Update ()
	{
		Player.Movement(Input.GetMouseButton(0));

		if (Player.PlayerState == AcDkJump.MovementState.Jumping)
		{
			JumpManSprite.sprite = images[1];
		}
		else
		{
			JumpManSprite.sprite = images[0];
		}

		JumpManTransform.position = FCposToUnityPos(FcPosX, Player.PosY);
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
