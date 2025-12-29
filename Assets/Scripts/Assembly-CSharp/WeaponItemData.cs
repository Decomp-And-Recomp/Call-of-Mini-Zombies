using UnityEngine;
using Zombie3D;

public class WeaponItemData : MonoBehaviour
{
	protected Weapon weapon;

	public string weapon_name;

	public Weapon GetWeapon()
	{
		return weapon;
	}

	public void SetWeapon(Weapon w)
	{
		if (w != null)
		{
			weapon_name = w.Name;
		}
		else
		{
			weapon_name = "none";
		}
		weapon = w;
	}
}
