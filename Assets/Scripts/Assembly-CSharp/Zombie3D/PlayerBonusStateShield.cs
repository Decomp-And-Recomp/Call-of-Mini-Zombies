using UnityEngine;

namespace Zombie3D
{
	public class PlayerBonusStateShield : PlayerBonusState
	{
		protected float lastTime;

		private Transform suit;

		private Transform cap;

		private Transform gun;

		private Transform gun2;

		private WeaponType gunType;

		private Shader effectShader;

		private Color previousColor;

		private Color previousColor_weapon;

		public PlayerBonusStateShield()
		{
			stateType = PlayerBonusStateType.Shield;
			effectShader = GameApp.GetInstance().GetGameResourceConfig().modelEdge_alpha;
		}

		public override void EnterState(Player player)
		{
			lastTime = GameApp.GetInstance().GetGameState().GetItemByType(ItemType.Shield)
				.iConf.lastDuration;
			player.DamageBuff = 0.3f;
			GameObject shieldLogo = GameApp.GetInstance().GetResourceConfig().shieldLogo;
			if (player.PowerObj != null)
			{
				Object.Destroy(player.PowerObj);
			}
			player.PowerObj = Object.Instantiate(shieldLogo, player.GetTransform().TransformPoint(Vector3.up * 2.1f), Quaternion.identity) as GameObject;
			player.PowerObj.transform.parent = player.GetTransform();
			suit = player.PlayerObject.transform.Find("Avatar_Suit");
			previousColor = Color.white;
			suit.GetComponent<Renderer>().material.shader = effectShader;
			suit.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
			suit.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_blue_min);
			AlphaAnimationScript alphaAnimationScript = suit.GetComponent<AlphaAnimationScript>();
			if (alphaAnimationScript == null)
			{
				alphaAnimationScript = suit.gameObject.AddComponent<AlphaAnimationScript>();
			}
			alphaAnimationScript.enableBrightAnimation = true;
			alphaAnimationScript.enableAlphaAnimation = false;
			alphaAnimationScript.colorPropertyName = "_AtmoColor";
			alphaAnimationScript.maxBright = ColorName.modelEdgeColor_blue_max;
			alphaAnimationScript.minBright = ColorName.modelEdgeColor_blue_min;
			alphaAnimationScript.animationSpeed = 5f;
			cap = player.PlayerObject.transform.Find("Avatar_Cap");
			if (cap != null && player.AvatarType != AvatarType.Pastor)
			{
				cap.GetComponent<Renderer>().material.shader = effectShader;
				cap.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
				cap.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_blue_min);
				AlphaAnimationScript alphaAnimationScript2 = cap.GetComponent<AlphaAnimationScript>();
				if (alphaAnimationScript2 == null)
				{
					alphaAnimationScript2 = cap.gameObject.AddComponent<AlphaAnimationScript>();
				}
				alphaAnimationScript2.enableBrightAnimation = true;
				alphaAnimationScript2.enableAlphaAnimation = false;
				alphaAnimationScript2.colorPropertyName = "_AtmoColor";
				alphaAnimationScript2.maxBright = ColorName.modelEdgeColor_blue_max;
				alphaAnimationScript2.minBright = ColorName.modelEdgeColor_blue_min;
				alphaAnimationScript2.animationSpeed = 5f;
			}
			SetGunModelEdgeAnimation(player.GetWeapon());
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			if (!CheckPlayerInDeadState(player))
			{
				lastTime -= deltaTime;
				if (lastTime < 0f)
				{
					player.SetBonusState(PlayerBonusStateType.Normal);
				}
				if (player.GetWeapon().GetWeaponType() != gunType)
				{
					SetGunSolidTextureAnimation();
					SetGunModelEdgeAnimation(player.GetWeapon());
				}
			}
		}

		public override void ExitState(Player player)
		{
			player.DamageBuff = 1f;
			if (player.PowerObj != null)
			{
				Object.Destroy(player.PowerObj);
				player.PowerObj = null;
			}
			if (suit.GetComponent<AlphaAnimationScript>() != null)
			{
				suit.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = false;
				suit.GetComponent<AlphaAnimationScript>().enableBrightAnimation = false;
				Object.Destroy(suit.GetComponent<AlphaAnimationScript>());
			}
			if (player.AvatarType != AvatarType.EnegyArmor)
			{
				suit.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/SolidTexture");
				suit.GetComponent<Renderer>().material.SetColor("_TintColor", previousColor);
			}
			else
			{
				suit.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/SolidAndAlphaTextureEx");
				suit.GetComponent<Renderer>().material.SetColor("_TintColor", previousColor);
			}
			if (cap != null && player.AvatarType != AvatarType.Pastor)
			{
				if (cap.GetComponent<AlphaAnimationScript>() != null)
				{
					cap.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = false;
					cap.GetComponent<AlphaAnimationScript>().enableBrightAnimation = false;
					Object.Destroy(cap.GetComponent<AlphaAnimationScript>());
				}
				if (player.AvatarType == AvatarType.LanboPixel)
				{
					cap.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/AlphaBlend");
				}
				else
				{
					cap.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/SolidTexture");
					cap.GetComponent<Renderer>().material.SetColor("_TintColor", previousColor);
				}
			}
			SetGunSolidTextureAnimation();
		}

		private void SetGunModelEdgeAnimation(Weapon weapon)
		{
			gunType = weapon.GetWeaponType();
			if (gunType == WeaponType.Saw)
			{
				gun = weapon.GetWeaponObject().transform.Find("Saw01");
				gun2 = weapon.GetWeaponObject().transform.Find("Saw02");
				Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
				gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0.5f));
			}
			else if (gunType == WeaponType.Sword)
			{
				gun = weapon.GetWeaponObject().transform.Find("GuangJian_01");
				gun2 = weapon.GetWeaponObject().transform.Find("GuangJian_02");
				Color color2 = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
				gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color2.r, color2.g, color2.b, 0.2f));
			}
			else
			{
				gun = weapon.GetWeaponObject().transform.Find("Bone01");
			}
			previousColor_weapon = Color.white;
			if (!(gun == null) && !(gun.GetComponent<Renderer>() == null))
			{
				gun.GetComponent<Renderer>().material.shader = effectShader;
				gun.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
				gun.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_blue_min);
				AlphaAnimationScript alphaAnimationScript = gun.GetComponent<AlphaAnimationScript>();
				if (alphaAnimationScript == null)
				{
					alphaAnimationScript = gun.gameObject.AddComponent<AlphaAnimationScript>();
				}
				alphaAnimationScript.enableBrightAnimation = true;
				alphaAnimationScript.enableAlphaAnimation = false;
				alphaAnimationScript.colorPropertyName = "_AtmoColor";
				alphaAnimationScript.maxBright = ColorName.modelEdgeColor_blue_max;
				alphaAnimationScript.minBright = ColorName.modelEdgeColor_blue_min;
				alphaAnimationScript.animationSpeed = 5f;
			}
		}

		private void SetGunSolidTextureAnimation()
		{
			if (!(gun == null) && !(gun.GetComponent<Renderer>() == null))
			{
				if (gun.GetComponent<AlphaAnimationScript>() != null)
				{
					gun.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = false;
					gun.GetComponent<AlphaAnimationScript>().enableBrightAnimation = false;
					Object.Destroy(gun.GetComponent<AlphaAnimationScript>());
				}
				gun.GetComponent<Renderer>().material.shader = ((gunType != WeaponType.Sword) ? Shader.Find("iPhone/SolidTexture") : Shader.Find("iPhone/SolidTextureBright"));
				gun.GetComponent<Renderer>().material.SetColor("_TintColor", previousColor_weapon);
				if (gunType == WeaponType.Saw || gunType == WeaponType.Sword)
				{
					Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 1f));
				}
			}
		}
	}
}
