using UnityEngine;

namespace Zombie3D
{
	public class PlayerBonusStateStealth : PlayerBonusState
	{
		protected float lastTime;

		private Transform suit;

		private Transform cap;

		private Transform legArms;

		private Transform gun;

		private Transform gun2;

		private WeaponType gunType;

		private Shader stealthShader;

		private Color[] previousColor = new Color[2];

		public PlayerBonusStateStealth()
		{
			stateType = PlayerBonusStateType.Stealth;
			stealthShader = GameApp.GetInstance().GetGameResourceConfig().modelEdge_alpha;
		}

		public override void EnterState(Player player)
		{
			lastTime = GameApp.GetInstance().GetGameState().GetItemByType(ItemType.InstantStealth)
				.iConf.lastDuration;
			suit = player.PlayerObject.transform.Find("Avatar_Suit");
			previousColor[0] = Color.white;
			suit.GetComponent<Renderer>().material.shader = stealthShader;
			suit.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
			suit.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_purple);
			AlphaAnimationScriptSecond alphaAnimationScriptSecond = suit.GetComponent<AlphaAnimationScriptSecond>();
			if (alphaAnimationScriptSecond == null)
			{
				alphaAnimationScriptSecond = suit.gameObject.AddComponent<AlphaAnimationScriptSecond>();
			}
			alphaAnimationScriptSecond.enabled = true;
			alphaAnimationScriptSecond.targetAlpha = 0f;
			alphaAnimationScriptSecond.lowToHigh = false;
			alphaAnimationScriptSecond.isResetShader = false;
			cap = player.PlayerObject.transform.Find("Avatar_Cap");
			if (cap != null)
			{
				if (player.AvatarType != AvatarType.Pastor)
				{
					cap.GetComponent<Renderer>().material.shader = stealthShader;
					cap.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
					cap.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_purple);
				}
				AlphaAnimationScriptSecond alphaAnimationScriptSecond2 = cap.GetComponent<AlphaAnimationScriptSecond>();
				if (alphaAnimationScriptSecond2 == null)
				{
					alphaAnimationScriptSecond2 = cap.gameObject.AddComponent<AlphaAnimationScriptSecond>();
				}
				alphaAnimationScriptSecond2.enabled = true;
				if (player.AvatarType != AvatarType.Pastor)
				{
					alphaAnimationScriptSecond2.targetAlpha = 0f;
				}
				else
				{
					alphaAnimationScriptSecond2.targetAlpha = 0.1f;
				}
				alphaAnimationScriptSecond2.lowToHigh = false;
				alphaAnimationScriptSecond2.isResetShader = false;
			}
			legArms = player.PlayerObject.transform.Find("Avatar_LegArms");
			if (legArms != null)
			{
				legArms.GetComponent<Renderer>().material.shader = stealthShader;
				legArms.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
				legArms.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_purple);
				AlphaAnimationScriptSecond alphaAnimationScriptSecond3 = legArms.GetComponent<AlphaAnimationScriptSecond>();
				if (alphaAnimationScriptSecond3 == null)
				{
					alphaAnimationScriptSecond3 = legArms.gameObject.AddComponent<AlphaAnimationScriptSecond>();
				}
				alphaAnimationScriptSecond3.enabled = true;
				alphaAnimationScriptSecond3.targetAlpha = 0f;
				alphaAnimationScriptSecond3.lowToHigh = false;
				alphaAnimationScriptSecond3.isResetShader = false;
			}
			SetGunModelEdgeAnimation(player.GetWeapon(), true);
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			if (CheckPlayerInDeadState(player))
			{
				return;
			}
			lastTime -= deltaTime;
			if (lastTime < 0f)
			{
				player.SetBonusState(PlayerBonusStateType.Normal);
			}
			if (player.GetWeapon().GetWeaponType() == gunType)
			{
				return;
			}
			if (gun != null && gun.GetComponent<Renderer>() != null)
			{
				gun.GetComponent<Renderer>().material.shader = ((gunType != WeaponType.Sword) ? Shader.Find("iPhone/SolidTexture") : Shader.Find("iPhone/SolidTextureBright"));
				gun.GetComponent<Renderer>().material.SetColor("_TintColor", previousColor[1]);
				if (gunType == WeaponType.Saw)
				{
					Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 1f));
				}
				else if (gunType == WeaponType.Sword)
				{
					Color color2 = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color2.r, color2.g, color2.b, 1f));
				}
			}
			SetGunModelEdgeAnimation(player.GetWeapon(), false);
		}

		public override void ExitState(Player player)
		{
			AlphaAnimationScriptSecond component = suit.GetComponent<AlphaAnimationScriptSecond>();
			if (component != null)
			{
				component.enabled = true;
				component.targetAlpha = 1f;
				component.lowToHigh = true;
				component.isResetShader = true;
				component.resetColor = previousColor[0];
				if (player.AvatarType == AvatarType.EnegyArmor)
				{
					component.resetShader = Shader.Find("iPhone/SolidAndAlphaTextureEx");
				}
				else
				{
					component.resetShader = Shader.Find("iPhone/SolidTexture");
				}
			}
			if (cap != null)
			{
				AlphaAnimationScriptSecond component2 = cap.GetComponent<AlphaAnimationScriptSecond>();
				if (component2 != null)
				{
					component2.enabled = true;
					component2.targetAlpha = 1f;
					component2.lowToHigh = true;
					if (player.AvatarType == AvatarType.Pastor)
					{
						component2.isResetShader = false;
					}
					else if (player.AvatarType == AvatarType.LanboPixel)
					{
						component2.isResetShader = true;
						component2.resetShader = Shader.Find("iPhone/AlphaBlend");
					}
					else
					{
						component2.isResetShader = true;
						component2.resetShader = Shader.Find("iPhone/SolidTexture");
					}
					component2.resetColor = previousColor[0];
				}
			}
			if (legArms != null)
			{
				AlphaAnimationScriptSecond component3 = legArms.GetComponent<AlphaAnimationScriptSecond>();
				if (component3 != null)
				{
					component3.enabled = true;
					component3.targetAlpha = 1f;
					component3.lowToHigh = true;
					component3.isResetShader = true;
					component3.resetShader = Shader.Find("iPhone/SolidTexture");
					component3.resetColor = Color.white;
				}
			}
			SetGunSolideTextureAnimation();
		}

		private void SetGunModelEdgeAnimation(Weapon weapon, bool isAnimation)
		{
			gunType = weapon.GetWeaponType();
			if (gunType == WeaponType.Saw)
			{
				gun = weapon.GetWeaponObject().transform.Find("Saw01");
				gun2 = weapon.GetWeaponObject().transform.Find("Saw02");
				Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
				gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0.1f));
			}
			else if (gunType == WeaponType.Sword)
			{
				gun = weapon.GetWeaponObject().transform.Find("GuangJian_01");
				gun2 = weapon.GetWeaponObject().transform.Find("GuangJian_02");
				Color color2 = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
				gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color2.r, color2.g, color2.b, 0.1f));
			}
			else
			{
				gun = weapon.GetWeaponObject().transform.Find("Bone01");
			}
			previousColor[1] = Color.white;
			if (!(gun == null) && !(gun.GetComponent<Renderer>() == null))
			{
				gun.GetComponent<Renderer>().material.shader = stealthShader;
				gun.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
				gun.GetComponent<Renderer>().material.SetColor("_AtmoColor", ColorName.modelEdgeColor_purple);
				AlphaAnimationScriptSecond alphaAnimationScriptSecond = gun.GetComponent<AlphaAnimationScriptSecond>();
				if (alphaAnimationScriptSecond == null)
				{
					alphaAnimationScriptSecond = gun.gameObject.AddComponent<AlphaAnimationScriptSecond>();
				}
				if (isAnimation)
				{
					alphaAnimationScriptSecond.enabled = true;
					alphaAnimationScriptSecond.targetAlpha = 0f;
					alphaAnimationScriptSecond.lowToHigh = false;
					alphaAnimationScriptSecond.isResetShader = false;
				}
				else
				{
					alphaAnimationScriptSecond.enabled = false;
					gun.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
				}
			}
		}

		private void SetGunSolideTextureAnimation()
		{
			if (!(gun == null) && !(gun.GetComponent<Renderer>() == null))
			{
				AlphaAnimationScriptSecond component = gun.GetComponent<AlphaAnimationScriptSecond>();
				if (component != null)
				{
					component.enabled = true;
					component.targetAlpha = 1f;
					component.lowToHigh = true;
					component.isResetShader = true;
					component.resetShader = ((gunType != WeaponType.Sword) ? Shader.Find("iPhone/SolidTexture") : Shader.Find("iPhone/SolidTextureBright"));
					component.resetColor = previousColor[1];
				}
				else
				{
					gun.GetComponent<Renderer>().material.shader = ((gunType != WeaponType.Sword) ? Shader.Find("iPhone/SolidTexture") : Shader.Find("iPhone/SolidTextureBright"));
				}
				if (gunType == WeaponType.Saw || gunType == WeaponType.Sword)
				{
					Color color = gun2.GetComponent<Renderer>().material.GetColor("_TintColor");
					gun2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 1f));
				}
			}
		}
	}
}
