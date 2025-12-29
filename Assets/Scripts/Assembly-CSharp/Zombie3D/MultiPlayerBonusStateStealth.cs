using UnityEngine;

namespace Zombie3D
{
	public class MultiPlayerBonusStateStealth : PlayerBonusState
	{
		private Transform suit;

		private Transform cap;

		private Transform gun;

		private Transform gun2;

		private Transform legArms;

		private Transform shadow;

		private GameObject nameLabel;

		private Color previousLabelColor;

		private WeaponType gunType;

		public bool Exposure;

		private bool changedToExposure;

		private Shader exposureShader;

		private Shader solidTextureShader;

		public MultiPlayerBonusStateStealth()
		{
			stateType = PlayerBonusStateType.Stealth;
			solidTextureShader = Shader.Find("iPhone/SolidTexture");
			exposureShader = GameApp.GetInstance().GetGameResourceConfig().modelEdge_alpha;
			Exposure = false;
			changedToExposure = false;
		}

		public override void EnterState(Player player)
		{
			if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
			{
				suit = player.PlayerObject.transform.Find("Avatar_Suit");
				suit.GetComponent<Renderer>().enabled = false;
				cap = player.PlayerObject.transform.Find("Avatar_Cap");
				if (cap != null)
				{
					cap.GetComponent<Renderer>().enabled = false;
				}
				legArms = player.PlayerObject.transform.Find("Avatar_LegArms");
				if (legArms != null)
				{
					legArms.GetComponent<Renderer>().enabled = false;
				}
				if (player.AvatarType == AvatarType.Nerd)
				{
					Transform transform = player.PlayerObject.transform.Find("Wonk_Eyeglass");
					transform.GetComponent<Renderer>().enabled = false;
				}
				DisableGunRender(player.GetWeapon());
				SetShadowAndNameTransparent(player);
			}
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
			{
				if (player.GetWeapon().GetWeaponType() != gunType)
				{
					DisableGunRender(player.GetWeapon());
				}
				if (Exposure && !changedToExposure)
				{
					SetShaderModelEdge(player);
					changedToExposure = true;
				}
				else if (!Exposure && changedToExposure)
				{
					SetShaderBackFromModelEdge(player);
					changedToExposure = false;
				}
			}
		}

		public override void ExitState(Player player)
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				return;
			}
			suit.GetComponent<Renderer>().material.shader = ((player.AvatarType != AvatarType.EnegyArmor) ? solidTextureShader : Shader.Find("iPhone/SolidAndAlphaTextureEx"));
			suit.GetComponent<Renderer>().enabled = true;
			if (cap != null)
			{
				cap.GetComponent<Renderer>().enabled = true;
				if (player.AvatarType == AvatarType.LanboPixel)
				{
					cap.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/AlphaBlend");
				}
				else if (player.AvatarType != AvatarType.Pastor)
				{
					cap.GetComponent<Renderer>().material.shader = solidTextureShader;
				}
			}
			if (legArms != null)
			{
				legArms.GetComponent<Renderer>().enabled = true;
			}
			if (player.AvatarType == AvatarType.Nerd)
			{
				player.PlayerObject.transform.Find("Wonk_Eyeglass").GetComponent<Renderer>().enabled = true;
			}
			EnableGunRender(player);
			RecoverShadowAndName();
		}

		private void DisableGunRender(Weapon weapon)
		{
			gunType = weapon.GetWeaponType();
			if (gunType == WeaponType.Saw)
			{
				gun = weapon.GetWeaponObject().transform.Find("Saw01");
				gun2 = weapon.GetWeaponObject().transform.Find("Saw02");
				Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
				if (Exposure)
				{
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0.1f));
				}
				else
				{
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0f));
				}
			}
			else if (gunType == WeaponType.Sword)
			{
				gun = weapon.GetWeaponObject().transform.Find("GuangJian_01");
				gun2 = weapon.GetWeaponObject().transform.Find("GuangJian_02");
				Color color2 = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
				if (Exposure)
				{
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color2.r, color2.g, color2.b, 0.1f));
				}
				else
				{
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color2.r, color2.g, color2.b, 0f));
				}
				gun2.gameObject.GetComponent<AlphaAnimationScript>().enabled = false;
			}
			else
			{
				gun = weapon.GetWeaponObject().transform.Find("Bone01");
			}
			if (Exposure)
			{
				gun.GetComponent<Renderer>().enabled = true;
				gun.GetComponent<Renderer>().material.shader = exposureShader;
				gun.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
				gun.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_purple);
			}
			else
			{
				gun.GetComponent<Renderer>().enabled = false;
			}
		}

		private void EnableGunRender(Player player)
		{
			foreach (Weapon weapon in player.weaponList)
			{
				if (weapon.GetWeaponType() == WeaponType.Saw)
				{
					gun = weapon.GetWeaponObject().transform.Find("Saw01");
					gun.GetComponent<Renderer>().material.shader = solidTextureShader;
					gun2 = weapon.GetWeaponObject().transform.Find("Saw02");
					Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 1f));
				}
				else if (weapon.GetWeaponType() == WeaponType.Sword)
				{
					gun = weapon.GetWeaponObject().transform.Find("GuangJian_01");
					gun.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/SolidTextureBright");
					gun2 = weapon.GetWeaponObject().transform.Find("GuangJian_02");
					Color color2 = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color2.r, color2.g, color2.b, 1f));
					gun2.gameObject.GetComponent<AlphaAnimationScript>().enabled = true;
				}
				else
				{
					gun = weapon.GetWeaponObject().transform.Find("Bone01");
					gun.GetComponent<Renderer>().material.shader = solidTextureShader;
				}
				if (player.GetWeapon() == weapon)
				{
					weapon.GunOn();
				}
				else
				{
					weapon.GunOff();
				}
			}
		}

		private void SetShadowAndNameTransparent(Player player)
		{
			shadow = player.PlayerObject.transform.Find("shadow");
			Multiplayer multiplayer = player as Multiplayer;
			nameLabel = multiplayer.NickNameLabel;
			if (shadow != null)
			{
				shadow.gameObject.SetActive(false);
			}
			if (nameLabel != null)
			{
				nameLabel.SetActive(false);
			}
		}

		private void RecoverShadowAndName()
		{
			shadow.gameObject.SetActive(true);
			nameLabel.SetActive(true);
		}

		private void SetShaderModelEdge(Player player)
		{
			suit.GetComponent<Renderer>().enabled = true;
			suit.GetComponent<Renderer>().material.shader = exposureShader;
			suit.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_purple);
			suit.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
			if (cap != null && player.AvatarType != AvatarType.Pastor)
			{
				cap.GetComponent<Renderer>().enabled = true;
				cap.GetComponent<Renderer>().material.shader = exposureShader;
				cap.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_purple);
				cap.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
			}
			if (gun != null && gun.GetComponent<Renderer>() != null)
			{
				gun.GetComponent<Renderer>().enabled = true;
				if (gunType == WeaponType.Saw)
				{
					Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0.1f));
				}
				else if (gunType == WeaponType.Sword)
				{
					Color color2 = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color2.r, color2.g, color2.b, 0.1f));
				}
				gun.GetComponent<Renderer>().material.shader = exposureShader;
				gun.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_purple);
				gun.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
			}
		}

		private void SetShaderBackFromModelEdge(Player player)
		{
			suit.GetComponent<Renderer>().enabled = false;
			if (cap != null)
			{
				cap.GetComponent<Renderer>().enabled = false;
			}
			if (gun != null && gun.GetComponent<Renderer>() != null)
			{
				if (gunType == WeaponType.Saw)
				{
					Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0f));
				}
				else if (gunType == WeaponType.Sword)
				{
					Color color2 = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color2.r, color2.g, color2.b, 0f));
				}
				gun.GetComponent<Renderer>().material.shader = solidTextureShader;
				gun.GetComponent<Renderer>().enabled = false;
			}
		}
	}
}
