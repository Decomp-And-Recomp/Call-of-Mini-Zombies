using UnityEngine;
using Zombie3D;

public class GameUIMultiplayerInfo : MonoBehaviour
{
	public GameObject avatar;

	public GameObject logo;

	public GameObject hpBar;

	public GameObject helpMsg;

	private float lastUpdateTime;

	public Multiplayer player { get; set; }

	public void Init()
	{
		avatar.GetComponent<TUIMeshText>().text_Accessor = player.nick_name;
		avatar.GetComponent<TUIMeshText>().color_Accessor = ColorName.GetPlayerMarkColor((int)player.birth_point_index);
		logo.GetComponent<TUIMeshSprite>().frameName_Accessor = "playerLogo_" + (int)player.AvatarType;
		helpMsg.SetActive(false);
		lastUpdateTime = Time.time;
	}

	private void Update()
	{
		if (player == null || player.PlayerObject == null)
		{
			Object.Destroy(base.gameObject);
		}
		else if (!(Time.time - lastUpdateTime < 0.03f))
		{
			UpdateMultiplayerInfo();
			if (player.PlayerObject.GetComponent<SphereCollider>().enabled)
			{
				helpMsg.SetActive(true);
			}
			else
			{
				helpMsg.SetActive(false);
			}
			lastUpdateTime = Time.time;
		}
	}

	private void UpdateMultiplayerInfo()
	{
		float num = 144f * player.HP / player.MaxHp;
		int num2 = (int)num;
		if (num2 % 2 != 0)
		{
			num2++;
		}
		hpBar.GetComponent<TUIMeshSpriteClip>().clip.rect = new Rect(-72f, -12.5f, num2, 25f);
	}
}
