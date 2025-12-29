using UnityEngine;

namespace Zombie3D
{
	public class PlayerRunAndShootState : PlayerState
	{
		private bool checkLastStateAnimation = true;

		public PlayerRunAndShootState()
		{
			state_type = PlayerStateType.RunShoot;
		}

		public override void EnterState(Player player)
		{
			checkLastStateAnimation = true;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			InputInfo inputInfo = player.InputController.InputInfo;
			player.ZoomIn(deltaTime);
			player.Move(deltaTime * 0.8f);
			Weapon weapon = player.GetWeapon();
			if (weapon == null)
			{
				return;
			}
			if (weapon.GetWeaponType() == WeaponType.Sniper)
			{
				HandleSniper(player, deltaTime, inputInfo);
			}
			else if (!inputInfo.fire && !inputInfo.IsMoving)
			{
				player.SetState(PlayerStateType.Idle);
				player.StopFire();
			}
			else if (!inputInfo.fire && inputInfo.IsMoving)
			{
				player.SetState(PlayerStateType.Run);
				player.StopFire();
			}
			else if (inputInfo.fire && !inputInfo.IsMoving)
			{
				player.SetState(PlayerStateType.Shoot);
				if (weapon.GetWeaponType() == WeaponType.ElectricGun)
				{
					player.StopFire();
				}
			}
			else if (weapon.GetWeaponType() == WeaponType.Sword)
			{
				if (inputInfo.fire && weapon.CouldMakeNextShoot())
				{
					player.Fire(deltaTime);
				}
				SwordEffectTrail swordEffectTrail = weapon.gun.GetComponent("SwordEffectTrail") as SwordEffectTrail;
				if (inputInfo.fire)
				{
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
				else if (null != swordEffectTrail)
				{
					swordEffectTrail.ShowTrail(false);
				}
			}
			else if (!weapon.HaveBullets(true))
			{
				player.SetState(PlayerStateType.Run);
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
					weapon.FireUpdate(deltaTime);
					player.Fire(deltaTime);
				}
				else
				{
					weapon.FireUpdate(deltaTime);
					player.Fire(deltaTime);
				}
				PlayRunShootAnimation(player);
			}
		}

		public override void ExitState(Player player)
		{
			if (player.GetWeapon().GetWeaponType() == WeaponType.Sword)
			{
				(player.GetWeapon() as Sword).EffectTrailSwitch(false);
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

		private void HandleSniper(Player player, float deltaTime, InputInfo inputinfo)
		{
			Sniper sniper = player.GetWeapon() as Sniper;
			if (!sniper.HaveBullets(true))
			{
				player.SetState(PlayerStateType.Run);
			}
			else if (inputinfo.fire)
			{
				sniper.AutoAim(deltaTime);
				player.Animate("Run01" + player.WeaponNameEnd, WrapMode.Loop);
			}
			else if (sniper.AimedTarget())
			{
				player.Fire(deltaTime);
				player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Once);
			}
			if (!inputinfo.fire && !inputinfo.IsMoving)
			{
				player.SetState(PlayerStateType.Idle);
				player.StopFire();
			}
			else if (!inputinfo.fire && inputinfo.IsMoving)
			{
				player.SetState(PlayerStateType.Run);
				player.StopFire();
			}
			else if (inputinfo.fire && !inputinfo.IsMoving)
			{
				player.SetState(PlayerStateType.Shoot);
				player.StopFire();
			}
		}
	}
}
