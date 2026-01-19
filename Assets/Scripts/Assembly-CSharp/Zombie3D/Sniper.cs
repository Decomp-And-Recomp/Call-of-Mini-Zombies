using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

namespace Zombie3D
{
	public class Sniper : Weapon
	{
		protected float trimWidth = 25f;

		protected float trimHeight = 25f;

		protected List<NearestEnemyInfo> nearestEnemyInfoList;

		protected int maxLocks = 5;

		protected bool locked;

		protected float flySpeed;

		public static Rect lockAreaRect = AutoRect.AutoPos(new Rect(230f, 200f, 500f, 250f));

		public int MaxLocks
		{
			get
			{
				return maxLocks;
			}
		}

		public Sniper()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
		}

		public List<NearestEnemyInfo> GetNearestEnemyInfoList()
		{
			return nearestEnemyInfoList;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Sniper;
		}

		public override void Init()
		{
			base.Init();
			mAttributes.hitForce = 60f;
			base.WeaponBulletObject = rConf.itemMissle;
			nearestEnemyInfoList = new List<NearestEnemyInfo>();
			TimerManager.GetInstance().SetTimer(2, base.AttackFrequency, false);
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				maxLocks = 1;
			}
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			flySpeed = base.WConf.flySpeed;
			base.WeaponBulletName = "Bullet_Sniper";
		}

		public override void AutoAim(float deltaTime)
		{
			foreach (NearestEnemyInfo nearestEnemyInfo3 in nearestEnemyInfoList)
			{
				if (nearestEnemyInfo3.transform != null)
				{
					Vector2 vector = cameraComponent.WorldToScreenPoint(nearestEnemyInfo3.transform.position);
					nearestEnemyInfo3.screenPos = new Vector2(vector.x, (float)Screen.height - vector.y);
					float x = Mathf.Lerp(nearestEnemyInfo3.currentScreenPos.x, nearestEnemyInfo3.screenPos.x, deltaTime * 10f);
					float y = Mathf.Lerp(nearestEnemyInfo3.currentScreenPos.y, nearestEnemyInfo3.screenPos.y, deltaTime * 10f);
					nearestEnemyInfo3.currentScreenPos = new Vector2(x, y);
				}
			}
			if (!TimerManager.GetInstance().Ready(2))
			{
				return;
			}
			for (int num = nearestEnemyInfoList.Count - 1; num >= 0; num--)
			{
				NearestEnemyInfo nearestEnemyInfo = nearestEnemyInfoList[num];
				if (nearestEnemyInfo.transform != null)
				{
					Vector3 vector2 = cameraComponent.WorldToScreenPoint(nearestEnemyInfo.transform.position);
					if (vector2.z < 0f || vector2.x < lockAreaRect.xMin || vector2.x > lockAreaRect.xMax || vector2.y < lockAreaRect.yMin || vector2.y > lockAreaRect.yMax)
					{
						nearestEnemyInfoList.Remove(nearestEnemyInfo);
						if (nearestEnemyInfo.transform.gameObject.layer == 23)
						{
							Object.Destroy(nearestEnemyInfo.transform.gameObject);
						}
					}
				}
			}
			if (nearestEnemyInfoList.Count >= maxLocks)
			{
				return;
			}
			float num2 = 99999f;
			NearestEnemyInfo nearestEnemyInfo2 = null;
			foreach (Enemy value in gameScene.GetEnemies().Values)
			{
				if (value.GetState() == Enemy.DEAD_STATE)
				{
					continue;
				}
				Transform aimedTransform = value.GetAimedTransform();
				Vector3 vector3 = cameraComponent.WorldToScreenPoint(aimedTransform.position);
				if (vector3.z < 0f || !(vector3.x > lockAreaRect.xMin) || !(vector3.x < lockAreaRect.xMax) || !(vector3.y > lockAreaRect.yMin) || !(vector3.y < lockAreaRect.yMax))
				{
					continue;
				}
				Ray ray = new Ray(rightGun.position, aimedTransform.position - rightGun.position);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 1000f, 35328) && !hitInfo.collider.gameObject.name.StartsWith("E_"))
				{
					continue;
				}
				float sqrMagnitude = (rightGun.position - aimedTransform.position).sqrMagnitude;
				bool flag = false;
				foreach (NearestEnemyInfo nearestEnemyInfo4 in nearestEnemyInfoList)
				{
					if (nearestEnemyInfo4.transform == aimedTransform)
					{
						flag = true;
					}
				}
				if (!flag && sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					nearestEnemyInfo2 = new NearestEnemyInfo();
					nearestEnemyInfo2.transform = aimedTransform;
					nearestEnemyInfo2.screenPos = new Vector2(vector3.x, (float)Screen.height - vector3.y);
					nearestEnemyInfo2.distance = sqrMagnitude;
					nearestEnemyInfo2.currentScreenPos = gameScene.GetCamera().ReticlePosition;
				}
			}
			if (nearestEnemyInfo2 != null)
			{
				nearestEnemyInfoList.Add(nearestEnemyInfo2);
				if (shootAudio != null)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				GameVSScene gameVSScene = gameScene as GameVSScene;
				foreach (Player value2 in gameVSScene.SFS_Player_Arr.Values)
				{
					if (value2 == null || value2 == player || (value2.GetPlayerState() != null && value2.GetPlayerState().GetStateType() == PlayerStateType.Dead) || (value2.PlayerBonusState != null && value2.PlayerBonusState.StateType == PlayerBonusStateType.Stealth))
					{
						continue;
					}
					Transform aimedTransform2 = value2.GetAimedTransform();
					Vector3 vector4 = cameraComponent.WorldToScreenPoint(aimedTransform2.position);
					if (vector4.z < 0f || !(vector4.x > lockAreaRect.xMin) || !(vector4.x < lockAreaRect.xMax) || !(vector4.y > lockAreaRect.yMin) || !(vector4.y < lockAreaRect.yMax))
					{
						continue;
					}
					Ray ray2 = new Ray(rightGun.position, aimedTransform2.position - rightGun.position);
					RaycastHit hitInfo2;
					if (Physics.Raycast(ray2, out hitInfo2, 1000f, 35072) && hitInfo2.collider.gameObject.layer != 8)
					{
						continue;
					}
					float sqrMagnitude2 = (rightGun.position - aimedTransform2.position).sqrMagnitude;
					bool flag2 = false;
					foreach (NearestEnemyInfo nearestEnemyInfo5 in nearestEnemyInfoList)
					{
						if (nearestEnemyInfo5.transform == aimedTransform2)
						{
							flag2 = true;
						}
					}
					if (!flag2 && sqrMagnitude2 < num2)
					{
						num2 = sqrMagnitude2;
						nearestEnemyInfo2 = new NearestEnemyInfo();
						nearestEnemyInfo2.transform = aimedTransform2;
						nearestEnemyInfo2.screenPos = new Vector2(vector4.x, (float)Screen.height - vector4.y);
						nearestEnemyInfo2.distance = sqrMagnitude2;
						nearestEnemyInfo2.currentScreenPos = gameScene.GetCamera().ReticlePosition;
					}
				}
				if (nearestEnemyInfo2 != null)
				{
					nearestEnemyInfoList.Add(nearestEnemyInfo2);
					if (shootAudio != null)
					{
						AudioPlayer.PlayAudio(shootAudio, true);
					}
				}
			}
			else
			{
				GameObject[] woodBoxes = gameScene.GetWoodBoxes();
				GameObject[] array = woodBoxes;
				foreach (GameObject gameObject in array)
				{
					if (!(gameObject != null))
					{
						continue;
					}
					Transform transform = gameObject.transform;
					Vector3 vector5 = cameraComponent.WorldToScreenPoint(transform.position + Vector3.up * 0.6f);
					if (vector5.z < 0f || !(vector5.x > lockAreaRect.xMin) || !(vector5.x < lockAreaRect.xMax) || !(vector5.y > lockAreaRect.yMin) || !(vector5.y < lockAreaRect.yMax))
					{
						continue;
					}
					Ray ray3 = new Ray(rightGun.position, transform.position + Vector3.up * 0.6f - rightGun.position);
					RaycastHit hitInfo3;
					if (Physics.Raycast(ray3, out hitInfo3, 1000f, 559104) && hitInfo3.collider.gameObject.layer != 19)
					{
						continue;
					}
					float sqrMagnitude3 = (rightGun.position - transform.position).sqrMagnitude;
					bool flag3 = false;
					foreach (NearestEnemyInfo nearestEnemyInfo6 in nearestEnemyInfoList)
					{
						if (transform.childCount > 0)
						{
							flag3 = true;
						}
					}
					if (!flag3 && sqrMagnitude3 < num2)
					{
						num2 = sqrMagnitude3;
						nearestEnemyInfo2 = new NearestEnemyInfo();
						GameObject gameObject2 = new GameObject();
						gameObject2.transform.position = transform.position + Vector3.up * 0.6f;
						gameObject2.transform.parent = transform;
						gameObject2.layer = 23;
						nearestEnemyInfo2.transform = gameObject2.transform;
						nearestEnemyInfo2.screenPos = new Vector2(vector5.x, (float)Screen.height - vector5.y);
						nearestEnemyInfo2.distance = sqrMagnitude3;
						nearestEnemyInfo2.currentScreenPos = gameScene.GetCamera().ReticlePosition;
					}
				}
				if (nearestEnemyInfo2 != null)
				{
					nearestEnemyInfoList.Add(nearestEnemyInfo2);
					if (shootAudio != null)
					{
						AudioPlayer.PlayAudio(shootAudio, true);
					}
				}
			}
			TimerManager.GetInstance().Do(2);
		}

		public bool AimedTarget()
		{
			if (nearestEnemyInfoList.Count == 0)
			{
				return false;
			}
			return true;
		}

		public override void Fire(float deltaTime)
		{
			foreach (NearestEnemyInfo nearestEnemyInfo in nearestEnemyInfoList)
			{
				if (nearestEnemyInfo.transform != null)
				{
					Vector3 normalized = (nearestEnemyInfo.transform.position - rightGun.position).normalized;
					GameObject gameObject = Object.Instantiate(rConf.isnipertile, rightGun.position + Vector3.up, Quaternion.LookRotation(-normalized)) as GameObject;
					ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
					component.dir = normalized;
					component.life = 10f;
					component.damage = player.Damage;
					component.flySpeed = flySpeed;
					component.hitForce = mAttributes.hitForce;
					component.targetTransform = nearestEnemyInfo.transform;
					component.GunType = WeaponType.Sniper;
					component.explodeRadius = mAttributes.range;
					component.hitObjectEffect = GameApp.GetInstance().GetGameResourceConfig().rocketExlposion;
					component.player = player;
					lastShootTime = Time.time;
					ConsumeBullet(1);
					if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
					{
						Packet packet = CGUserSniperFirePacket.MakePacket((uint)player.net_com.m_netUserInfo.user_id, nearestEnemyInfo.transform.position);
						player.net_com.Send(packet);
					}
					else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
					{
						SFSObject sFSObject = new SFSObject();
						sFSObject.PutFloat("pgm_x", nearestEnemyInfo.transform.position.x);
						sFSObject.PutFloat("pgm_y", nearestEnemyInfo.transform.position.y);
						sFSObject.PutFloat("pgm_z", nearestEnemyInfo.transform.position.z);
						SFSObject sFSObject2 = new SFSObject();
						sFSObject2.PutSFSObject("pgmFire", sFSObject);
						TNetConnection.Connection.Send(new BroadcastMessageRequest(sFSObject2));
					}
				}
			}
			nearestEnemyInfoList.Clear();
			locked = false;
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
		}

		public override void StopFire()
		{
			if (nearestEnemyInfoList != null)
			{
				nearestEnemyInfoList.Clear();
			}
		}
	}
}
