using UnityEngine;

public class PlayAnimationScript : MonoBehaviour
{
	public string animationName;

	protected float lastUpdateTime;

	private void Start()
	{
	}

	private void Update()
	{
		if (!(Time.time - lastUpdateTime < 0.02f))
		{
			lastUpdateTime = Time.time;
		}
	}
}
