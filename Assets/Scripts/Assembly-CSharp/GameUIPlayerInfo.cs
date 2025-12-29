using UnityEngine;
using Zombie3D;

public class GameUIPlayerInfo : MonoBehaviour
{
	public GameObject avatar;

	public GameObject logo;

	public GameObject hpBar;

	public GameObject level;

	public GameObject levelBar;

	public GameObject medpack;

	public GameObject[] buffLogos = new GameObject[2];

	public GameObject buffTimer;

	private Player player;

	private GameState gameState;

	private bool inited;

	private float lastUpdateTime;

	private float buffTimerStart;

	public void Init()
	{
		player = GameApp.GetInstance().GetGameScene().GetPlayer();
		gameState = GameApp.GetInstance().GetGameState();
		logo.GetComponent<TUIMeshSprite>().frameName_Accessor = "playerLogo_" + (int)player.AvatarType;
		if (player.Level >= UpgradeParas.AvatarMaxLevel)
		{
			level.GetComponent<TUIMeshText>().text_Accessor = "LV:30";
			levelBar.GetComponent<TUIMeshSpriteClip>().clip.rect = new Rect(-67.5f, -5.5f, 135f, 11f);
		}
		else
		{
			level.GetComponent<TUIMeshText>().text_Accessor = "LV:" + player.Level;
		}
		if (gameState.gameMode == GameMode.Coop)
		{
			avatar.GetComponent<TUIMeshText>().text_Accessor = gameState.nick_name;
			avatar.GetComponent<TUIMeshText>().color_Accessor = ColorName.GetPlayerMarkColor((int)player.birth_point_index);
			medpack.GetComponent<TUIMeshText>().text_Accessor = "x" + gameState.Medpack;
		}
		else
		{
			Object.Destroy(avatar);
			Object.Destroy(medpack);
			Object.Destroy(base.transform.Find("medpack_logo").gameObject);
		}
		if (player.EXPBuff == 1f)
		{
			buffLogos[0].SetActive(false);
			buffLogos[1].SetActive(false);
			buffLogos[1].transform.localPosition = new Vector3(0f, 0f, 0f);
			buffTimer.SetActive(false);
			buffTimer.transform.localPosition = new Vector3(0f, -15.8f, 0f);
		}
		else if (player.EXPBuff == 1.5f)
		{
			buffLogos[0].SetActive(true);
			buffLogos[0].GetComponent<TUIMeshSprite>().frameName_Accessor = "Buff_Exp15";
			buffLogos[1].SetActive(false);
			buffLogos[1].transform.localPosition = new Vector3(30f, 0f, 0f);
			buffTimer.SetActive(false);
			buffTimer.transform.localPosition = new Vector3(30f, -15.8f, 0f);
		}
		else if (player.EXPBuff == 2.5f)
		{
			buffLogos[0].SetActive(true);
			buffLogos[0].GetComponent<TUIMeshSprite>().frameName_Accessor = "Buff_Exp25";
			buffLogos[1].SetActive(false);
			buffLogos[1].transform.localPosition = new Vector3(30f, 0f, 0f);
			buffTimer.SetActive(false);
			buffTimer.transform.localPosition = new Vector3(30f, -15.8f, 0f);
		}
	}

	public void SetInited()
	{
		inited = true;
		lastUpdateTime = Time.time;
	}

	private void Update()
	{
		if (!inited || player == null || Time.time - lastUpdateTime < 0.03f)
		{
			return;
		}
		int num = (int)(144f * player.GuiHp / player.MaxHp);
		if (num % 2 != 0)
		{
			num++;
		}
		hpBar.GetComponent<TUIMeshSpriteClip>().clip.rect = new Rect(-72f, -25f, num, 50f);
		if (player.Level < UpgradeParas.AvatarMaxLevel)
		{
			num = (int)(135f * (player.ExpProgress / 100f));
			if (num % 2 != 0)
			{
				num++;
			}
			levelBar.GetComponent<TUIMeshSpriteClip>().clip.rect = new Rect(-67.5f, -5.5f, num, 11f);
		}
		UpdateBuffReminder();
		lastUpdateTime = Time.time;
	}

	public void SetPlayerLevel()
	{
		level.GetComponent<TUIMeshText>().text_Accessor = "LV:" + player.Level;
	}

	public void SetMedpackCount()
	{
		medpack.GetComponent<TUIMeshText>().text_Accessor = "x" + gameState.Medpack;
	}

	public void AddBonusStateReminder(ItemType type)
	{
		buffLogos[1].SetActive(true);
		buffLogos[1].GetComponent<TUIMeshSprite>().frameName_Accessor = "Buff_" + type;
		buffTimer.SetActive(true);
		switch (type)
		{
		case ItemType.Power:
			buffTimerStart = GameApp.GetInstance().GetGameState().GetItemByType(ItemType.Power)
				.iConf.lastDuration;
			break;
		case ItemType.InstantStealth:
			buffTimerStart = GameApp.GetInstance().GetGameState().GetItemByType(ItemType.InstantStealth)
				.iConf.lastDuration;
			break;
		case ItemType.InstantSuper:
			buffTimerStart = GameApp.GetInstance().GetGameState().GetItemByType(ItemType.InstantSuper)
				.iConf.lastDuration;
			break;
		case ItemType.Shield:
			buffTimerStart = GameApp.GetInstance().GetGameState().GetItemByType(ItemType.Shield)
				.iConf.lastDuration;
			break;
		}
	}

	private void UpdateBuffReminder()
	{
		if (!buffTimer.activeInHierarchy)
		{
			return;
		}
		if (player.GetPlayerState() != null && player.GetPlayerState().GetStateType() == PlayerStateType.Dead)
		{
			buffLogos[1].SetActive(false);
			buffTimer.SetActive(false);
			return;
		}
		buffTimerStart -= Time.time - lastUpdateTime;
		if ((int)buffTimerStart < 0)
		{
			buffLogos[1].SetActive(false);
			buffTimer.SetActive(false);
		}
		else if ((int)buffTimerStart <= 5)
		{
			buffTimer.GetComponent<TUIMeshText>().text_Accessor = (int)buffTimerStart + "'";
			buffTimer.GetComponent<TUIMeshText>().color_Accessor = ColorName.fontColor_red;
		}
		else
		{
			buffTimer.GetComponent<TUIMeshText>().text_Accessor = (int)buffTimerStart + "'";
			buffTimer.GetComponent<TUIMeshText>().color_Accessor = ColorName.fontColor_orange;
		}
	}
}
