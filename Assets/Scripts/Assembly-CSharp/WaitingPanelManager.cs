using UnityEngine;
using Zombie3D;

public class WaitingPanelManager : MonoBehaviour
{
	public TUIMeshTextColorBlink colorBlink;

	private void Awake()
	{
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop && GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		colorBlink.enabled = false;
	}

	private void Update()
	{
	}

	public void Show()
	{
		base.transform.localPosition = new Vector3(0f, 0f, -100f);
		colorBlink.enabled = true;
	}

	public void Hide()
	{
		base.transform.localPosition = new Vector3(0f, -5000f, -100f);
		colorBlink.enabled = false;
	}
}
