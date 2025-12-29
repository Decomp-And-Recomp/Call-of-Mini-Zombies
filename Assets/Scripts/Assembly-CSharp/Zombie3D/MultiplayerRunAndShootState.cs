using UnityEngine;

namespace Zombie3D
{
	public class MultiplayerRunAndShootState : PlayerState
	{
		private bool checkLastStateAnimation;

		public MultiplayerRunAndShootState()
		{
			state_type = PlayerStateType.RunShoot;
		}

		public override void EnterState(Player player)
		{
			checkLastStateAnimation = true;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			Multiplayer multiplayer = player as Multiplayer;
			if (multiplayer.m_is_lerp_position)
			{
				player.Move(Mathf.Lerp(deltaTime + player.net_ping_sum, deltaTime, deltaTime));
			}
			else
			{
				player.Move(deltaTime);
			}
			Weapon weapon = player.GetWeapon();
			if (weapon == null)
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
				if (player.IsPlayingAnimation("RunShoot01" + player.WeaponNameEnd))
				{
					if (player.IsAnimationPlayedPercentage("RunShoot01" + player.WeaponNameEnd, 1f))
					{
						if (null != swordEffectTrail)
						{
							swordEffectTrail.ShowTrail(true);
						}
						weapon.AutoAim(0f);
						player.RandomSwordAnimation();
						player.PlayAnimate("RunShoot01" + player.WeaponNameEnd, WrapMode.ClampForever);
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
					player.PlayAnimate("RunShoot01" + player.WeaponNameEnd, WrapMode.ClampForever);
				}
			}
			else if (!weapon.CouldMakeNextShoot())
			{
				weapon.FireUpdate(deltaTime);
				if (!CheckFinishPlayingAnimation(player))
				{
					return;
				}
				switch (weapon.GetWeaponType())
				{
				case WeaponType.RocketLauncher:
					player.Animate("Run01" + player.WeaponNameEnd, WrapMode.Loop);
					break;
				case WeaponType.ShotGun:
				case WeaponType.M32:
				case WeaponType.Crossbow:
					player.Animate("Run02" + player.WeaponNameEnd, WrapMode.Loop);
					break;
				case WeaponType.ElectricGun:
					if (weapon.Name == "Ion-Cannon" || weapon.Name == "Longinus-Gold")
					{
						player.Animate("Run02_Shotgun", WrapMode.Loop);
					}
					else if (weapon.Name == "Dragon-Breath" || weapon.Name == "Pixel-Cannon")
					{
						player.Animate("Run01_AirGun", WrapMode.Loop);
					}
					break;
				case WeaponType.AssaultRifle:
				case WeaponType.MachineGun:
				case WeaponType.LaserGun:
				case WeaponType.FireGun:
					player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Loop);
					break;
				case WeaponType.Sniper:
				case WeaponType.Saw:
				case WeaponType.NurseSaliva:
				case WeaponType.Sword:
				case WeaponType.SuicideGun:
					break;
				}
			}
			else
			{
				if (weapon.GetWeaponType() == WeaponType.AssaultRifle)
				{
					weapon.AutoAim(deltaTime);
					player.Fire(deltaTime);
				}
				else
				{
					player.Fire(deltaTime);
				}
				PlayRunShootAnimation(player);
			}
		}

		public override void ExitState(Player player)
		{
			if (player.GetWeapon().GetWeaponType() == WeaponType.Sword)
			{
				(player.GetWeapon() as MultiSword).EffectTrailSwitch(false);
			}
		}

		private void PlayRunShootAnimation(Player player)
		{
			switch (player.GetWeapon().GetWeaponType())
			{
			case WeaponType.ShotGun:
			case WeaponType.Crossbow:
				player.PlayAnimate("RunShoot01" + player.WeaponNameEnd, WrapMode.Once);
				break;
			case WeaponType.AssaultRifle:
				player.PlayAnimate("RunShoot01" + player.WeaponNameEnd, WrapMode.Loop);
				break;
			case WeaponType.RocketLauncher:
			case WeaponType.M32:
			case WeaponType.ElectricGun:
				player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Once);
				break;
			case WeaponType.Saw:
				player.RandomSawAnimation();
				player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Loop);
				break;
			default:
				player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Loop);
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
