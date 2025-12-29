using UnityEngine;
using Zombie3D;

public class Step4Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
{
	protected ITutorialGameUI guis;

	protected TutorialScript ts;

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
		player.InputController.EnableTurningAround = true;
		player.InputController.EnableShootingInput = false;
	}

	public void UpdateTutorialStep(float deltaTime, Player player)
	{
		Transform transform = GameObject.Find("WoodBox").transform;
		Vector3 vector = player.GetTransform().InverseTransformPoint(transform.position);
		float num = Mathf.Tan(0.9599311f);
		if (vector.z > 0f && Mathf.Abs(vector.z / vector.x) > num)
		{
			ts.GoToNextStep();
		}
	}

	public void EndStep(Player player)
	{
		player.InputController.CameraRotation = new Vector2(0f, 0f);
	}

	public void SetGameGUI(ITutorialGameUI guis)
	{
		this.guis = guis;
	}

	public void OK(Player player)
	{
	}
}
