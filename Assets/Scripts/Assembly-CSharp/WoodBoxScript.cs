using UnityEngine;
using Zombie3D;

public class WoodBoxScript : MonoBehaviour
{
	public float hp = 10f;

	protected ResourceConfigScript rConf;

	protected Transform boxTransform;

	protected float startTime;

	private void Start()
	{
		boxTransform = base.gameObject.transform;
		base.GetComponent<Rigidbody>().useGravity = false;
		startTime = Time.time;
		if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (Time.time - startTime > 20f && !base.GetComponent<Rigidbody>().useGravity)
		{
			base.GetComponent<Rigidbody>().useGravity = true;
		}
		if (base.transform.position.y < 10030.1f)
		{
			base.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			base.GetComponent<Renderer>().enabled = false;
		}
	}

	public bool OnHit(float damage)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		bool result = false;
		rConf = GameApp.GetInstance().GetResourceConfig();
		hp -= damage;
		if (hp <= 0f)
		{
			Player player = GameApp.GetInstance().GetGameScene().GetPlayer();
			Weapon weapon = player.GetWeapon();
			if (weapon.GetWeaponType() == WeaponType.Sword)
			{
				Object.Instantiate(rConf.swordAttack, base.transform.position + base.transform.up * 1f, Quaternion.identity);
			}
			Object.Destroy(base.gameObject);
			GameObject gameObject = Object.Instantiate(rConf.woodExplode, base.transform.position, Quaternion.identity) as GameObject;
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.mute = !GameApp.GetInstance().GetGameState().SoundOn;
			}
			SendMessage("OnLoot", false);
			result = true;
		}
		return result;
	}
}
