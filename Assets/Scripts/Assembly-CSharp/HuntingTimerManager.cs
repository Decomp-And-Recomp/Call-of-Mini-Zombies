using System;
using UnityEngine;
using Zombie3D;

public class HuntingTimerManager : MonoBehaviour
{
	public OnHuntingTimerBegin onBegin;

	public OnHuntingTimerEnd onEnd;

	public Enemy enermy { get; set; }

	public float timer { get; set; }

	private void Start()
	{
		if (onBegin != null)
		{
			onBegin();
		}
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		TimeSpan timeSpan = new TimeSpan(0, 0, (int)timer);
		base.gameObject.GetComponent<TUIMeshText>().text_Accessor = timeSpan.ToString();
		if (timer <= 0f)
		{
			if (enermy != null)
			{
				enermy.RemovePreyEnemy();
			}
			if (onEnd != null)
			{
				onEnd();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
