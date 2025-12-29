using UnityEngine;
using Zombie3D;

public class GameUIWeaponInfo : MonoBehaviour
{
	public GameObject logo;

	public GameObject bulletLogo;

	public GameObject bulletCount;

	public GameObject buyAmmo;

	public GameObject weaponBackground;

	private Player player;

	private GameState gameState;

	private bool inited;

	private float lastUpdateTime;

	public void Init()
	{
		player = GameApp.GetInstance().GetGameScene().GetPlayer();
		gameState = GameApp.GetInstance().GetGameState();
		buyAmmo.SetActive(false);
		logo.GetComponent<TUIMeshSprite>().frameName_Accessor = "weaponLogo_" + player.GetWeapon().Name;
		if (player.GetWeapon().GetWeaponType() != WeaponType.Saw && player.GetWeapon().GetWeaponType() != WeaponType.Sword)
		{
			bulletLogo.GetComponent<TUIMeshSprite>().frameName_Accessor = player.GetWeapon().WeaponBulletName;
			if (player.GetWeapon().BulletCount == 0 && gameState.GetCash() >= player.GetWeapon().WConf.bulletPrice)
			{
				buyAmmo.SetActive(true);
				bulletCount.SetActive(false);
				bulletLogo.SetActive(false);
			}
			else
			{
				buyAmmo.SetActive(false);
				bulletCount.GetComponent<TUIMeshText>().text_Accessor = "x" + player.GetWeapon().BulletCount;
			}
		}
		else
		{
			bulletCount.SetActive(false);
			bulletLogo.SetActive(false);
		}
	}

	public void SetInited()
	{
		inited = true;
		lastUpdateTime = Time.time;
	}

	private void Update()
	{
		if (!inited || player == null || Time.time - lastUpdateTime < 0.03f)
		{
			return;
		}
		Weapon weapon = player.GetWeapon();
		if (weapon.WeaponBulletName != bulletLogo.GetComponent<TUIMeshSprite>().frameName_Accessor)
		{
			if (weapon.GetWeaponType() == WeaponType.Saw || weapon.GetWeaponType() == WeaponType.Sword)
			{
				bulletCount.SetActive(false);
				bulletLogo.SetActive(false);
				bulletLogo.GetComponent<TUIMeshSprite>().frameName_Accessor = string.Empty;
				buyAmmo.SetActive(false);
			}
			else
			{
				bulletLogo.SetActive(true);
				bulletCount.SetActive(true);
				bulletLogo.GetComponent<TUIMeshSprite>().frameName_Accessor = weapon.WeaponBulletName;
			}
		}
		else if (weapon.GetWeaponType() != WeaponType.Saw && weapon.GetWeaponType() != WeaponType.Sword)
		{
			if (weapon.GetWeaponType() != WeaponType.SuicideGun)
			{
				if (weapon.BulletCount == 0 && gameState.GetCash() >= weapon.WConf.bulletPrice)
				{
					buyAmmo.SetActive(true);
					bulletCount.SetActive(false);
					bulletLogo.SetActive(false);
				}
				else
				{
					buyAmmo.SetActive(false);
					bulletLogo.SetActive(true);
					bulletCount.SetActive(true);
					bulletCount.GetComponent<TUIMeshText>().text_Accessor = "x" + weapon.BulletCount;
				}
			}
			else if (weapon.BulletCount == 0)
			{
				bulletCount.SetActive(false);
				bulletLogo.SetActive(false);
			}
			else
			{
				buyAmmo.SetActive(false);
				bulletLogo.SetActive(true);
				bulletCount.SetActive(true);
				bulletCount.GetComponent<TUIMeshText>().text_Accessor = "x" + weapon.BulletCount;
			}
		}
		lastUpdateTime = Time.time;
	}

	public void SetWeaponLogo(string name)
	{
		if (logo != null)
		{
			logo.GetComponent<TUIMeshSprite>().frameName_Accessor = "weaponLogo_" + name;
		}
	}
}
