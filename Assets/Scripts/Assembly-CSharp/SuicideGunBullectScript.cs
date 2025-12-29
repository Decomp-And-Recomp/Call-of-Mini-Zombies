using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class SuicideGunBullectScript : MonoBehaviour
{
	protected int smallBulletNum = 20;

	protected ResourceConfigScript rConf;

	public float speed;

	protected float bigBulletHeight = 20f;

	protected List<Vector3> sceneBorders = new List<Vector3>();

	protected float deltaTime;

	private bool bigBulletExploded;

	public float explodeRadius { get; set; }

	public float hitForce { get; set; }

	public float damage { get; set; }

	public Player player { get; set; }

	private void Start()
	{
		rConf = GameApp.GetInstance().GetResourceConfig();
		sceneBorders = GameApp.GetInstance().GetGameScene().GetSceneBorders();
	}

	private void Update()
	{
		if (player.PlayerObject == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.03f)
		{
			return;
		}
		if (base.gameObject != null)
		{
			Vector3 vector = new Vector3(0f, 1f, 0f);
			base.transform.Translate(speed * vector * deltaTime, Space.World);
			if (bigBulletHeight <= 0f)
			{
				if (!bigBulletExploded)
				{
					StartCoroutine(GenerateSmallBullet());
					bigBulletExploded = true;
				}
			}
			else
			{
				bigBulletHeight -= speed * deltaTime;
			}
		}
		deltaTime = 0f;
	}

	private IEnumerator GenerateSmallBullet()
	{
		for (int i = 0; i < smallBulletNum; i++)
		{
			GameObject obj = Object.Instantiate(position: new Vector3
			{
				x = Random.Range(sceneBorders[2].x, sceneBorders[1].x),
				y = base.transform.position.y,
				z = Random.Range(sceneBorders[2].z, sceneBorders[1].z)
			}, original: rConf.suicideGunSmallBullet, rotation: Quaternion.identity) as GameObject;
			SuicideGunProjectileScript p = obj.GetComponent<SuicideGunProjectileScript>();
			p.dir = new Vector3(0f, -1f, 0f);
			p.flySpeed = Random.Range(speed, speed + 10f);
			p.explodeRadius = explodeRadius;
			p.hitForce = hitForce;
			p.damage = damage;
			p.player = player;
			yield return 1;
		}
		Object.Destroy(base.gameObject);
	}
}
