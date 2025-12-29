using UnityEngine;
using Zombie3D;

public class VSAdUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public TUIScroll scroll;

	public GameObject[] ad960;

	public GameObject[] ad1136;

	public GameObject[] ad1024;

	public GameObject tapButton;

	public float scrollStartSpeed;

	public float scrollIncreaseSpeed;

	private float deltaTime;

	public GameObject[] allPages { get; set; }

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		if (AutoRect.GetPlatform() == Platform.iPhone5)
		{
			for (int i = 0; i < ad960.Length; i++)
			{
				Object.Destroy(ad960[i]);
				Object.Destroy(ad1024[i]);
			}
			scroll.size = new Vector2(568f, 320f);
			allPages = ad1136;
		}
		else if (AutoRect.GetPlatform() == Platform.IPad)
		{
			for (int j = 0; j < ad960.Length; j++)
			{
				Object.Destroy(ad960[j]);
				Object.Destroy(ad1136[j]);
			}
			scroll.size = new Vector2(512f, 384f);
			allPages = ad1024;
		}
		else
		{
			for (int k = 0; k < ad960.Length; k++)
			{
				Object.Destroy(ad1024[k]);
				Object.Destroy(ad1136[k]);
			}
			scroll.size = new Vector2(480f, 320f);
			allPages = ad960;
		}
		scroll.rangeXMin = (0f - scroll.size.x) * (float)(allPages.Length - 1);
		scroll.borderXMin = scroll.rangeXMin;
		scroll.pageX = new float[allPages.Length];
		for (int l = 0; l < scroll.pageX.Length; l++)
		{
			scroll.pageX[l] = (0f - scroll.size.x) * (float)l;
		}
		tapButton.transform.position = allPages[allPages.Length - 1].transform.position + new Vector3(0f, 0f, -11f);
		deltaTime = 0f;
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Button_Tap" && eventType == 3)
		{
			GameApp.GetInstance().GetGameState().show_update_ad = false;
			if (GameApp.GetInstance().GetGameState().Game_last_ver < 4.29f)
			{
				GameApp.GetInstance().GetGameState().makeUpCrystal = true;
			}
			GameApp.GetInstance().GetGameState().tutorialTriggers[3] = true;
			GameApp.GetInstance().GetGameState().tutorialTriggers[2] = true;
			GameApp.GetInstance().PlayerPrefsSave();
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("StartMenuTUI");
			control.gameObject.SetActive(false);
		}
	}

	private bool CheckEnableAutoScroll()
	{
		bool result = false;
		for (int i = 0; i < scroll.pageX.Length; i++)
		{
			if (allPages[0].transform.parent.position.x == scroll.pageX[i])
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
