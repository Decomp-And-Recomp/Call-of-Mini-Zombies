using UnityEngine;
using Zombie3D;

public class GameUIItemInfo : MonoBehaviour
{
	public TUIMeshSprite[] itemLogo;

	public TUIMeshText[] itemCount;

	public GameObject[] crystals;

	public TUIMeshText[] buyPrice;

	public GameObject[] backgrounds;

	public bool[] isBuyItem = new bool[2];

	private Player player;

#if UNITY_STANDALONE
	void Awake()
	{
		transform.position = new Vector3(130, -164, -10);
	}
#endif

	public void Init()
	{
		if (GameApp.GetInstance().GetGameState().LevelNum == 0)
		{
			base.gameObject.SetActive(false);
			return;
		}
		player = GameApp.GetInstance().GetGameScene().GetPlayer();
		int num = 0;
		foreach (ItemType key in player.carryItemsPacket.Keys)
		{
			itemLogo[num].frameName_Accessor = "item_" + key;
			UpdateCarryItemPacket(key, player.carryItemsPacket[key]);
			num++;
		}
	}

	public void SetInited()
	{
	}

	public void UpdateCarryItemPacket(ItemType type, int count)
	{
		ItemConfig itemConfig = GameApp.GetInstance().GetGameConfig().GetItemConfig(type);
		if (itemLogo[0].frameName_Accessor == "item_" + type)
		{
			if (count == 0)
			{
				itemLogo[0].color_Accessor = new Color(1f, 1f, 1f, 0.5f);
				itemCount[0].gameObject.SetActive(false);
				isBuyItem[0] = true;
				if (itemConfig.isCrystalBuy)
				{
					crystals[0].SetActive(true);
					buyPrice[0].gameObject.SetActive(true);
					buyPrice[0].text_Accessor = itemConfig.price.ToString();
				}
				else
				{
					crystals[0].SetActive(false);
					buyPrice[0].gameObject.SetActive(true);
					buyPrice[0].text_Accessor = "$" + itemConfig.price;
				}
			}
			else
			{
				itemLogo[0].color_Accessor = new Color(1f, 1f, 1f, 1f);
				itemCount[0].gameObject.SetActive(true);
				itemCount[0].text_Accessor = count.ToString();
				crystals[0].SetActive(false);
				buyPrice[0].gameObject.SetActive(false);
				isBuyItem[0] = false;
			}
		}
		else
		{
			if (!(itemLogo[1].frameName_Accessor == "item_" + type))
			{
				return;
			}
			if (count == 0)
			{
				itemLogo[1].color_Accessor = new Color(1f, 1f, 1f, 0.5f);
				itemCount[1].gameObject.SetActive(false);
				isBuyItem[1] = true;
				if (itemConfig.isCrystalBuy)
				{
					crystals[1].SetActive(true);
					buyPrice[1].gameObject.SetActive(true);
					buyPrice[1].text_Accessor = itemConfig.price.ToString();
				}
				else
				{
					crystals[1].SetActive(false);
					buyPrice[1].gameObject.SetActive(true);
					buyPrice[1].text_Accessor = "$" + itemConfig.price;
				}
			}
			else
			{
				itemLogo[1].color_Accessor = new Color(1f, 1f, 1f, 1f);
				itemCount[1].gameObject.SetActive(true);
				itemCount[1].text_Accessor = count.ToString();
				crystals[1].SetActive(false);
				buyPrice[1].gameObject.SetActive(false);
				isBuyItem[1] = false;
			}
		}
	}
}
