using UnityEngine;

namespace Zombie3D
{
	public abstract class PlayerBonusState
	{
		protected PlayerBonusStateType stateType;

		public PlayerBonusStateType StateType
		{
			get
			{
				return stateType;
			}
			set
			{
				stateType = value;
			}
		}

		public abstract void EnterState(Player player);

		public abstract void DoStateLogic(Player player, float deltaTime);

		public abstract void ExitState(Player player);

		protected virtual bool CheckPlayerInDeadState(Player player)
		{
			if (player.GetPlayerState() != null && player.GetPlayerState().GetStateType() == PlayerStateType.Dead)
			{
				player.SetBonusState(PlayerBonusStateType.Normal);
				return true;
			}
			return false;
		}

		public bool CheckEnableEquipItem(Player player, ItemType itemType)
		{
			if (player.PlayerBonusState.StateType != PlayerBonusStateType.Normal)
			{
				if (itemType == ItemType.Shield || itemType == ItemType.Power || itemType == ItemType.InstantStealth || itemType == ItemType.InstantSuper || itemType == ItemType.SuicideGun)
				{
					return false;
				}
			}
			else if (itemType == ItemType.Shield || itemType == ItemType.InstantStealth)
			{
				Transform transform = player.PlayerObject.transform.Find("Avatar_Suit");
				if (transform.GetComponent<Renderer>().material.shader != Shader.Find("iPhone/SolidTexture") && transform.GetComponent<Renderer>().material.shader != Shader.Find("iPhone/SolidAndAlphaTextureEx"))
				{
					return false;
				}
				if (player.GetWeapon().GetWeaponType() == WeaponType.Saw)
				{
					Transform transform2 = player.GetWeapon().GetWeaponObject().transform.Find("Saw01");
					if (transform2.GetComponent<Renderer>().material.shader != Shader.Find("iPhone/SolidTexture"))
					{
						return false;
					}
				}
				else if (player.GetWeapon().GetWeaponType() == WeaponType.Sword)
				{
					Transform transform2 = player.GetWeapon().GetWeaponObject().transform.Find("GuangJian_01");
					if (transform2.GetComponent<Renderer>().material.shader != Shader.Find("iPhone/SolidTextureBright"))
					{
						return false;
					}
				}
				else
				{
					Transform transform2 = player.GetWeapon().GetWeaponObject().transform.Find("Bone01");
					if (transform2.GetComponent<Renderer>().material.shader != Shader.Find("iPhone/SolidTexture"))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
