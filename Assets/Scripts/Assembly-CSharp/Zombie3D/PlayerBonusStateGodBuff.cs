using UnityEngine;

namespace Zombie3D
{
	public class PlayerBonusStateGodBuff : PlayerBonusState
	{
		private const float GODBUFFTIME = 5f;

		protected float lastTime;

		private WeaponType gunType;

		private Transform gun;

		private Transform gun2;

		private Transform suit;

		private Transform cap;

		private Color[] previousColor = new Color[2];

		public PlayerBonusStateGodBuff()
		{
			stateType = PlayerBonusStateType.GodBuff;
		}

		public override void EnterState(Player player)
		{
			lastTime = 0f;
			if (player.GetWeapon().GetWeaponType() == WeaponType.SuicideGun)
			{
				player.GetWeapon().enableShoot = false;
			}
			player.GetWeapon().StopFire();
			suit = player.PlayerObject.transform.Find("Avatar_Suit");
			previousColor[0] = Color.white;
			if (player.AvatarType != AvatarType.EnegyArmor)
			{
				suit.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/AlphaBlend_Color");
			}
			suit.GetComponent<Renderer>().material.SetColor("_TintColor", Color.white);
			AlphaAnimationScript alphaAnimationScript = suit.GetComponent<AlphaAnimationScript>();
			if (alphaAnimationScript == null)
			{
				alphaAnimationScript = suit.gameObject.AddComponent<AlphaAnimationScript>();
			}
			alphaAnimationScript.enableAlphaAnimation = true;
			alphaAnimationScript.enableBrightAnimation = false;
			alphaAnimationScript.animationSpeed = 3f;
			cap = player.PlayerObject.transform.Find("Avatar_Cap");
			if (cap != null)
			{
				if (player.AvatarType != AvatarType.Pastor)
				{
					cap.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/AlphaBlend_Color");
				}
				cap.GetComponent<Renderer>().material.SetColor("_TintColor", Color.white);
				AlphaAnimationScript alphaAnimationScript2 = cap.GetComponent<AlphaAnimationScript>();
				if (alphaAnimationScript2 == null)
				{
					alphaAnimationScript2 = cap.gameObject.AddComponent<AlphaAnimationScript>();
				}
				alphaAnimationScript2.enableAlphaAnimation = true;
				alphaAnimationScript2.enableBrightAnimation = false;
				alphaAnimationScript2.animationSpeed = 3f;
			}
			SetGunAlphaBlenderAnimation(player.GetWeapon());
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			lastTime += deltaTime;
			if (lastTime >= 5f)
			{
				player.SetBonusState(PlayerBonusStateType.Normal);
			}
			if (player.GetWeapon().GetWeaponType() != gunType)
			{
				SetGunSolidTexture();
				SetGunAlphaBlenderAnimation(player.GetWeapon());
			}
		}

		public override void ExitState(Player player)
		{
			suit.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = false;
			suit.GetComponent<AlphaAnimationScript>().enableBrightAnimation = false;
			Object.Destroy(suit.GetComponent<AlphaAnimationScript>());
			if (player.AvatarType != AvatarType.EnegyArmor)
			{
				suit.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/SolidTexture");
			}
			suit.GetComponent<Renderer>().material.SetColor("_TintColor", previousColor[0]);
			if (cap != null)
			{
				cap.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = false;
				cap.GetComponent<AlphaAnimationScript>().enableBrightAnimation = false;
				Object.Destroy(cap.GetComponent<AlphaAnimationScript>());
				if (player.AvatarType == AvatarType.Pastor)
				{
					cap.GetComponent<Renderer>().material.SetColor("_TintColor", previousColor[0]);
				}
				else if (player.AvatarType == AvatarType.LanboPixel)
				{
					cap.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/AlphaBlend");
				}
				else
				{
					cap.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/SolidTexture");
					cap.GetComponent<Renderer>().material.SetColor("_TintColor", previousColor[0]);
				}
			}
			SetGunSolidTexture();
		}

		private void SetGunAlphaBlenderAnimation(Weapon weapon)
		{
			gunType = weapon.GetWeaponType();
			if (gunType == WeaponType.Saw)
			{
				gun = weapon.GetWeaponObject().transform.Find("Saw01");
				gun2 = weapon.GetWeaponObject().transform.Find("Saw02");
			}
			else if (gunType == WeaponType.Sword)
			{
				gun = weapon.GetWeaponObject().transform.Find("GuangJian_01");
				gun2 = weapon.GetWeaponObject().transform.Find("GuangJian_02");
			}
			else
			{
				gun = weapon.GetWeaponObject().transform.Find("Bone01");
			}
			previousColor[1] = Color.white;
			if (!(gun == null) && !(gun.GetComponent<Renderer>() == null))
			{
				gun.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/AlphaBlend_Color");
				gun.GetComponent<Renderer>().material.SetColor("_TintColor", Color.white);
				AlphaAnimationScript alphaAnimationScript = gun.gameObject.AddComponent<AlphaAnimationScript>();
				alphaAnimationScript.enableAlphaAnimation = true;
				alphaAnimationScript.enableBrightAnimation = false;
				alphaAnimationScript.animationSpeed = 3f;
				if (gun2 != null)
				{
					Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0.1f));
				}
			}
		}

		private void SetGunSolidTexture()
		{
			if (!(gun == null) && !(gun.GetComponent<Renderer>() == null))
			{
				gun.gameObject.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = false;
				gun.gameObject.GetComponent<AlphaAnimationScript>().enableBrightAnimation = false;
				Object.Destroy(gun.gameObject.GetComponent<AlphaAnimationScript>());
				gun.GetComponent<Renderer>().material.shader = ((gunType != WeaponType.Sword) ? Shader.Find("iPhone/SolidTexture") : Shader.Find("iPhone/SolidTextureBright"));
				gun.GetComponent<Renderer>().material.SetColor("_TintColor", previousColor[1]);
				if (gun2 != null)
				{
					Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 1f));
				}
			}
		}
	}
}
