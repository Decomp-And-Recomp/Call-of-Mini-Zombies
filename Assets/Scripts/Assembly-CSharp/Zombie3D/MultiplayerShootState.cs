using UnityEngine;

namespace Zombie3D
{
	public class MultiplayerShootState : PlayerState
	{
		private bool checkLastStateAnimation;

		public MultiplayerShootState()
		{
			state_type = PlayerStateType.Shoot;
		}

		public override void EnterState(Player player)
		{
			if (player.GetWeapon().GetWeaponType() == WeaponType.SuicideGun)
			{
				player.Fire(0f);
				PlayShootAnimation(player);
			}
			checkLastStateAnimation = true;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			Weapon weapon = player.GetWeapon();
			if (weapon == null || weapon.GetWeaponType() == WeaponType.SuicideGun)
			{
				return;
			}
			if (weapon.GetWeaponType() == WeaponType.Sniper)
			{
				MultiSniper multiSniper = weapon as MultiSniper;
				if (multiSniper.AimedTarget())
				{
					player.Fire(deltaTime);
				}
			}
			else if (weapon.GetWeaponType() == WeaponType.Sword)
			{
				if (weapon.CouldMakeNextShoot())
				{
					player.Fire(deltaTime);
				}
				SwordEffectTrail swordEffectTrail = weapon.gun.GetComponent("SwordEffectTrail") as SwordEffectTrail;
				if (player.IsPlayingAnimation("Shoot01" + player.WeaponNameEnd))
				{
					if (player.IsAnimationPlayedPercentage("Shoot01" + player.WeaponNameEnd, 1f))
					{
						if (null != swordEffectTrail)
						{
							swordEffectTrail.ShowTrail(true);
						}
						weapon.AutoAim(0f);
						player.RandomSwordAnimation();
						player.PlayAnimate("Shoot01" + player.WeaponNameEnd, WrapMode.Once);
					}
				}
				else
				{
					if (null != swordEffectTrail)
					{
						swordEffectTrail.ShowTrail(true);
					}
					weapon.AutoAim(0f);
					player.RandomSwordAnimation();
					player.PlayAnimate("Shoot01" + player.WeaponNameEnd, WrapMode.Once);
				}
			}
			else if (!weapon.CouldMakeNextShoot())
			{
				weapon.FireUpdate(deltaTime);
				if (!CheckFinishPlayingAnimation(player))
				{
					return;
				}
				if (weapon.GetWeaponType() == WeaponType.ShotGun || weapon.GetWeaponType() == WeaponType.Crossbow || weapon.GetWeaponType() == WeaponType.RocketLauncher || weapon.GetWeaponType() == WeaponType.M32)
				{
					player.Animate("Idle01" + player.WeaponNameEnd, WrapMode.Loop);
				}
				else if (weapon.GetWeaponType() == WeaponType.ElectricGun)
				{
					if (weapon.Name == "Ion-Cannon" || weapon.Name == "Longinus-Gold")
					{
						player.Animate("Idle01_Shotgun", WrapMode.Loop);
					}
					else if (weapon.Name == "Dragon-Breath" || weapon.Name == "Pixel-Cannon")
					{
						player.Animate("Idle01", WrapMode.Loop);
					}
				}
				else
				{
					player.Animate("Shoot01" + player.WeaponNameEnd, WrapMode.Loop);
				}
			}
			else
			{
				if (weapon.GetWeaponType() == WeaponType.AssaultRifle || weapon.GetWeaponType() == WeaponType.MachineGun)
				{
					weapon.AutoAim(deltaTime);
					weapon.FireUpdate(deltaTime);
					player.Fire(deltaTime);
				}
				else
				{
					weapon.FireUpdate(deltaTime);
					player.Fire(deltaTime);
				}
				PlayShootAnimation(player);
			}
		}

		public override void ExitState(Player player)
		{
			if (player.GetWeapon().GetWeaponType() == WeaponType.Sword)
			{
				(player.GetWeapon() as MultiSword).EffectTrailSwitch(false);
			}
			else if (player.GetWeapon().GetWeaponType() == WeaponType.SuicideGun)
			{
				player.StopAnimation("Shoot01_SuicideGun");
			}
		}

		private void PlayShootAnimation(Player player)
		{
			switch (player.GetWeapon().GetWeaponType())
			{
			case WeaponType.ShotGun:
			case WeaponType.RocketLauncher:
			case WeaponType.M32:
			case WeaponType.ElectricGun:
			case WeaponType.SuicideGun:
			case WeaponType.Crossbow:
				player.Animate("Shoot01" + player.WeaponNameEnd, WrapMode.Once);
				break;
			case WeaponType.Saw:
				player.RandomSawAnimation();
				player.Animate("Shoot01" + player.WeaponNameEnd, WrapMode.Loop);
				break;
			default:
				player.Animate("Shoot01" + player.WeaponNameEnd, WrapMode.Loop);
				break;
			}
		}

		private bool CheckFinishPlayingAnimation(Player player)
		{
			bool flag = player.IsPlayingAnimation(player.CurrentAnimationName);
			bool flag2 = player.AnimationEnds(player.CurrentAnimationName);
			if (checkLastStateAnimation)
			{
				checkLastStateAnimation = false;
				return true;
			}
			return (flag && flag2) || !flag;
		}
	}
}
