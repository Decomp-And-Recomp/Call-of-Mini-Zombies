using UnityEngine;
using Zombie3D;

public class Step2Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
{
	protected ITutorialGameUI guis;

	protected TutorialScript ts;

	protected GameObject halo;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void StartStep(TutorialScript ts, Player player)
	{
		this.ts = ts;
		player.InputController.EnableMoveInput = true;
		halo = GameObject.Find("Halo");
	}

	public void UpdateTutorialStep(float deltaTime, Player player)
	{
		if ((halo.transform.position - player.GetTransform().position).sqrMagnitude < 1f)
		{
			Object.Destroy(halo);
			GameObject gameObject = GameObject.Find("Arrow");
			gameObject.transform.position = GameObject.Find("WoodBox").transform.TransformPoint(Vector3.forward * 2f);
			ItemScript component = gameObject.GetComponent<ItemScript>();
			component.HighPos = 2.2f;
			component.LowPos = 2f;
			ts.GoToNextStep();
		}
	}

	public void EndStep(Player player)
	{
	}

	public void SetGameGUI(ITutorialGameUI guis)
	{
		this.guis = guis;
	}

	public void OK(Player player)
	{
	}
}
