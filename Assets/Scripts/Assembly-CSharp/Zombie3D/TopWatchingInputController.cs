using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class TopWatchingInputController : InputController
	{
		public override void ProcessInput(float deltaTime, InputInfo inputInfo)
		{
			Vector3 getHitFlySpeed = player.GetHitFlySpeed;
			List<Weapon> battleWeapons = GameApp.GetInstance().GetGameState().GetBattleWeapons();
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				if (Input.GetButton("Fire1"))
				{
					player.Fire(deltaTime);
				}
				else
				{
					player.StopFire();
				}
			}
			if (base.EnableMoveInput)
			{
				inputInfo.moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
				inputInfo.moveDirection = player.GetTransform().TransformDirection(inputInfo.moveDirection);
				inputInfo.IsMoving = true;
			}
			inputInfo.moveDirection += Physics.gravity * deltaTime * 20f;
			player.SetMoveDirection();
			if (inputInfo.moveDirection.x != 0f || inputInfo.moveDirection.z != 0f)
			{
				inputInfo.IsMoving = true;
			}
			else
			{
				inputInfo.IsMoving = false;
			}
			getHitFlySpeed.x = Mathf.Lerp(getHitFlySpeed.x, 0f, 5f * Time.deltaTime);
			getHitFlySpeed.y = Mathf.Lerp(getHitFlySpeed.y, 0f, (0f - Physics.gravity.y) * Time.deltaTime);
			getHitFlySpeed.z = Mathf.Lerp(getHitFlySpeed.z, 0f, 5f * Time.deltaTime);
			for (int i = 1; i <= 3; i++)
			{
				if (Input.GetButton("Weapon" + i) && battleWeapons[i - 1] != null)
				{
					player.ChangeWeaponAndSendMsg(i - 1);
				}
			}
		}
	}
}
