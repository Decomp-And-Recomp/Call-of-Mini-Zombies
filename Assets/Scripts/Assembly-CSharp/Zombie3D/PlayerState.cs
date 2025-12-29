namespace Zombie3D
{
	public abstract class PlayerState
	{
		protected PlayerStateType state_type;

		public PlayerStateType GetStateType()
		{
			return state_type;
		}

		public abstract void EnterState(Player player);

		public abstract void DoStateLogic(Player player, float deltaTime);

		public abstract void ExitState(Player player);

		public virtual void OnHit(Player player)
		{
			if (player.HP <= 0f)
			{
				player.StopFire();
				player.SetState(PlayerStateType.Dead);
				player.OnDead();
			}
			else
			{
				if (!player.CouldGetAnotherHit())
				{
					return;
				}
				player.CreateScreenBlood();
				if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop || GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
				{
					return;
				}
				Weapon weapon = player.GetWeapon();
				if (weapon.GetWeaponType() == WeaponType.ElectricGun)
				{
					if (player.GetPlayerState() != null && player.GetPlayerState().GetStateType() != PlayerStateType.Shoot && player.GetPlayerState().GetStateType() != PlayerStateType.RunShoot)
					{
						player.SetState(PlayerStateType.GotHit);
					}
				}
				else if (weapon.GetWeaponType() != WeaponType.Saw && weapon.GetWeaponType() != WeaponType.Sword && weapon.GetWeaponType() != WeaponType.SuicideGun)
				{
					player.SetState(PlayerStateType.GotHit);
				}
				else if (weapon.GetWeaponType() == WeaponType.SuicideGun && !player.IsPlayingAnimation("Shoot01_SuicideGun"))
				{
					player.SetState(PlayerStateType.GotHit);
				}
			}
		}
	}
}
