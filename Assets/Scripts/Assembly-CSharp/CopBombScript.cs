using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class CopBombScript : MonoBehaviour
{
	protected float startTime;

	public float explodeTime = 4f;

	public float radius = 5f;

	public float damage = 20f;

	public Vector3 speed;

	private void Start()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		if (!(Time.time - startTime > explodeTime))
		{
			return;
		}
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
		{
			Player player = GameApp.GetInstance().GetGameScene().GetPlayer();
			float num = Mathf.Sqrt((base.transform.position - player.GetTransform().position).sqrMagnitude);
			if (num < radius)
			{
				Ray ray = new Ray(base.transform.position, player.GetTransform().position - base.transform.position);
				RaycastHit hitInfo;
				if (!Physics.Raycast(ray, out hitInfo, num, 67584))
				{
					player.OnHit(damage);
				}
			}
		}
		else
		{
			List<Player> list = new List<Player>();
			foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
			{
				float num2 = Mathf.Sqrt((base.transform.position - item.GetTransform().position).sqrMagnitude);
				if (num2 < radius)
				{
					Ray ray2 = new Ray(base.transform.position, item.GetTransform().position - base.transform.position);
					RaycastHit hitInfo2;
					if (!Physics.Raycast(ray2, out hitInfo2, num2, 67584))
					{
						list.Add(item);
					}
				}
			}
			foreach (Player item2 in list)
			{
				item2.OnHit(damage);
			}
		}
		ResourceConfigScript resourceConfig = GameApp.GetInstance().GetResourceConfig();
		GameObject gameObject = Object.Instantiate(resourceConfig.rocketExlposion, base.transform.position, Quaternion.identity) as GameObject;
		gameObject.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
		Object.Destroy(base.gameObject);
	}
}
