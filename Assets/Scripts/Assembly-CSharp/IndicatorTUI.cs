using UnityEngine;

public class IndicatorTUI : MonoBehaviour
{
	public enum IndicatorType
	{
		None = 0,
		ServerConnect = 1,
		HeartWaiting = 2,
		QuickMatch = 3,
		RoomSearch = 4,
		RoomJoin = 5
	}

	protected GameObject m_content;

	private bool is_show;

	public IndicatorType type;

	public OnAutomaticCloseIndicator automaticClose;

	private float automaticCloseTime = 15f;

	private float automatic_hide_startTime;

	private void Awake()
	{
		m_content = base.gameObject.transform.Find("BK/Content").gameObject;
		if (m_content != null)
		{
			m_content.GetComponent<TUIMeshText>().fontName_Accessor = "CAI-14";
		}
	}

	public void Update()
	{
		if (is_show && Time.time - automatic_hide_startTime >= automaticCloseTime)
		{
			Hide();
			if (automaticClose != null)
			{
				automaticClose();
			}
		}
	}

	public void SetContent(string str)
	{
		if (m_content != null)
		{
			m_content.GetComponent<TUIMeshText>().text_Accessor = str;
		}
	}

	public void Show(IndicatorType m_type)
	{
		if (is_show)
		{
			type = m_type;
			automatic_hide_startTime = Time.time;
			return;
		}
		if (m_type == IndicatorType.QuickMatch) automaticCloseTime = 3f;

		int num = 0;
		num = ((AutoRect.GetPlatform() != Platform.IPad) ? 1 : 0);
		Utils.ShowIndicatorSystem(num, (int)AutoRect.GetPlatform(), 1f, 1f, 1f, 1f);
		base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
		is_show = true;
		type = m_type;
		automatic_hide_startTime = Time.time;
	}

	public void Hide()
	{
		if (is_show)
		{
			Utils.HideIndicatorSystem();
			base.gameObject.transform.localPosition = new Vector3(0f, 2000f, base.gameObject.transform.localPosition.z);
			is_show = false;
		}
	}

	public void OnDestroy()
	{
		Hide();
	}
}
