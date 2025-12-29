using UnityEngine;
using Zombie3D;

public class MsgBoxDelegate : MonoBehaviour
{
	public GameObject Content;

	public MsgBoxType m_type;

	public float activeTime;

	private TUIMeshText rebirth_timer;

	private GameObject revive_logo;

	public void Update()
	{
		if (m_type == MsgBoxType.Rebirth || m_type == MsgBoxType.CrystalRebirth)
		{
			activeTime -= Time.deltaTime;
			if (activeTime <= 0f)
			{
				m_type = MsgBoxType.None;
				activeTime = 0f;
				GameApp.GetInstance().GetGameScene().GetPlayer()
					.PlayerRealDead();
				Hide();
			}
			else if (activeTime <= 10f)
			{
				base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
			}
			rebirth_timer.text_Accessor = Mathf.Round(activeTime).ToString();
		}
	}

	public void SetMsgContent(string content, MsgBoxType type)
	{
		if ((bool)Content)
		{
			Content.GetComponent<TUIMeshText>().text_Accessor = content;
			m_type = type;
		}
	}

	public void SetContentOffset(Vector2 pos)
	{
		if ((bool)Content)
		{
			Content.transform.localPosition = new Vector3(pos.x, pos.y, -1f);
		}
	}

	public void Show(string content, MsgBoxType type)
	{
		if ((bool)Content)
		{
			Content.GetComponent<TUIMeshText>().horizontalAlignment_Accessor = TUIMeshText.HorizontalAlignment.Left;
			Content.GetComponent<TUIMeshText>().verticalAlignment_Accessor = TUIMeshText.VerticalAlignment.Top;
			if (type == MsgBoxType.NotEnoughUser || type == MsgBoxType.None)
			{
				Content.GetComponent<TUIMeshText>().fontName_Accessor = "CAI-14";
			}
			else
			{
				Content.GetComponent<TUIMeshText>().fontName_Accessor = "CAI-18";
			}
		}
		int num = 1;
		foreach (char c in content)
		{
			if (c == '\n')
			{
				num++;
			}
		}
		switch (num)
		{
		case 5:
			SetContentOffset(new Vector2(-124f, 50f));
			break;
		case 4:
			SetContentOffset(new Vector2(-124f, 43f));
			break;
		case 3:
			SetContentOffset(new Vector2(-118f, 35f));
			break;
		case 2:
			SetContentOffset(new Vector2(-118f, 21f));
			break;
		case 1:
			SetContentOffset(new Vector2(0f, 10f));
			if ((bool)Content)
			{
				Content.GetComponent<TUIMeshText>().horizontalAlignment = TUIMeshText.HorizontalAlignment.Center;
				Content.GetComponent<TUIMeshText>().verticalAlignment = TUIMeshText.VerticalAlignment.Center;
			}
			break;
		}
		SetMsgContent(content, type);
		switch (type)
		{
		case MsgBoxType.Rebirth:
			rebirth_timer = base.gameObject.transform.Find("BK/TimeLabel").GetComponent<TUIMeshText>();
			revive_logo = base.gameObject.transform.Find("BK/Tip").gameObject;
			revive_logo.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
			revive_logo.GetComponent<TUIMeshSprite>().frameName_Accessor = "revive";
			break;
		case MsgBoxType.CrystalRebirth:
			rebirth_timer = base.gameObject.transform.Find("BK/TimeLabel").GetComponent<TUIMeshText>();
			revive_logo = base.gameObject.transform.Find("BK/Tip").gameObject;
			revive_logo.transform.localScale = new Vector3(1f, 1f, 1f);
			revive_logo.GetComponent<TUIMeshSprite>().frameName_Accessor = "money_crystal_label";
			break;
		default:
			base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
			break;
		}
	}

	public void Hide()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, 1000f, base.gameObject.transform.localPosition.z);
	}

	public void SetOkButton(string name)
	{
		base.transform.Find("BK/Msg_OK_Button/label_n").GetComponent<TUIMeshText>().text_Accessor = name;
		base.transform.Find("BK/Msg_OK_Button/label_p").GetComponent<TUIMeshText>().text_Accessor = name;
	}
}
