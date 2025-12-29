using System.IO;
using UnityEngine;

namespace Zombie3D
{
	public class AchievementState
	{
		public const int ACHIEVEMENT_COUNT = 16;

		public int score;

		public int newWeaponsGot;

		public int newAvatarGot;

		public int enemyKills;

		public int sawKills;

		public int loseTimes;

		public int upgradeTenTimes;

		public AchievementInfo[] acheivements = new AchievementInfo[16];

		protected ScoreInfo scoreInfo = new ScoreInfo();

		public AchievementState()
		{
			GameCenterPlugin.Initialize();
			for (int i = 0; i < 16; i++)
			{
				acheivements[i] = new AchievementInfo();
				acheivements[i].id = "com.trinitigame.callofminizombies.a" + (i + 1);
			}
		}

		public void SubmitScore(int score)
		{
			scoreInfo.score = score;
		}

		public void SubmitAllToGameCenter()
		{
			if (!GameCenterPlugin.IsLogin())
			{
				return;
			}
			for (int i = 0; i < 16; i++)
			{
				if (acheivements[i].submitting && GameCenterPlugin.SubmitAchievement(acheivements[i].id, 100))
				{
					acheivements[i].submitting = false;
				}
			}
			if (scoreInfo.score != 0 && GameCenterPlugin.SubmitScore(scoreInfo.id, scoreInfo.score))
			{
				scoreInfo.score = 0;
			}
		}

		public void GotNewWeapon()
		{
			newWeaponsGot++;
			CheckAchievemnet_NewBattleAbility();
			CheckAchievemnet_WeaponHouseware();
			CheckAchievemnet_WeaponCollector();
		}

		public void GotNewAvatar()
		{
			newAvatarGot++;
			CheckAchievemnet_Avatar();
			CheckAchievemnet_AvatarMaster();
		}

		public void UpgradeTenTimes()
		{
			upgradeTenTimes++;
			CheckAchievemnet_Upgrade();
			CheckAchievemnet_UpgradeMaster();
		}

		public void SubmitEndlessScore(int score)
		{
			GameCenterPlugin.SubmitScore("com.trinitigame.callofminizombies.l2", score);
		}

		public void CheckAchievemnet_Endless(int time)
		{
		}

		public void KillEnemy()
		{
			enemyKills++;
			if (GameApp.GetInstance().GetGameScene().GetPlayer()
				.GetWeapon()
				.GetWeaponType() == WeaponType.Saw)
			{
				sawKills++;
				CheckAchievemnet_SawKillers();
			}
			CheckAchievemnet_TookAShoot();
			CheckAchievemnet_Killer();
		}

		public void LoseGame()
		{
			loseTimes++;
		}

		public void AddScore(int scoreAdd)
		{
			score += scoreAdd;
		}

		public void Save(BinaryWriter bw)
		{
			bw.Write(score);
			bw.Write(newWeaponsGot);
			bw.Write(enemyKills);
			bw.Write(loseTimes);
			bw.Write(newAvatarGot);
			bw.Write(upgradeTenTimes);
			for (int i = 0; i < 16; i++)
			{
				bw.Write(acheivements[i].submitting);
				bw.Write(acheivements[i].complete);
			}
		}

		public void Load(BinaryReader br)
		{
			score = br.ReadInt32();
			newWeaponsGot = br.ReadInt32();
			enemyKills = br.ReadInt32();
			loseTimes = br.ReadInt32();
			newAvatarGot = br.ReadInt32();
			upgradeTenTimes = br.ReadInt32();
			for (int i = 0; i < 16; i++)
			{
				acheivements[i].submitting = br.ReadBoolean();
				acheivements[i].complete = br.ReadBoolean();
			}
		}

		public void ResetData()
		{
			score = 0;
			newWeaponsGot = 0;
			newAvatarGot = 0;
			enemyKills = 0;
			sawKills = 0;
			loseTimes = 0;
			upgradeTenTimes = 0;
			for (int i = 0; i < 16; i++)
			{
				acheivements[i].submitting = false;
				acheivements[i].complete = false;
			}
		}

		public void SaveNew(Configure cfg)
		{
			cfg.AddValueSingle("Save", "Achi_Score", score.ToString(), string.Empty, string.Empty);
			cfg.AddValueSingle("Save", "Achi_NewWeaponsGot", newWeaponsGot.ToString(), string.Empty, string.Empty);
			cfg.AddValueSingle("Save", "Achi_EnemyKills", enemyKills.ToString(), string.Empty, string.Empty);
			cfg.AddValueSingle("Save", "Achi_LoseTimes", loseTimes.ToString(), string.Empty, string.Empty);
			cfg.AddValueSingle("Save", "Achi_NewAvatarGot", newAvatarGot.ToString(), string.Empty, string.Empty);
			cfg.AddValueSingle("Save", "Achi_UpgradeTenTimes", upgradeTenTimes.ToString(), string.Empty, string.Empty);
			StringLine stringLine = new StringLine();
			StringLine stringLine2 = new StringLine();
			for (int i = 0; i < acheivements.Length; i++)
			{
				stringLine.AddString(acheivements[i].submitting.ToString());
				stringLine2.AddString(acheivements[i].complete.ToString());
			}
			cfg.AddValueArray("Save", "Achi_Submit", stringLine.content, string.Empty, string.Empty);
			cfg.AddValueArray("Save", "Achi_Complete", stringLine2.content, string.Empty, string.Empty);
		}

		public void LoadNew(Configure cfg)
		{
			score = int.Parse(cfg.GetSingle("Save", "Achi_Score"));
			newWeaponsGot = int.Parse(cfg.GetSingle("Save", "Achi_NewWeaponsGot"));
			enemyKills = int.Parse(cfg.GetSingle("Save", "Achi_EnemyKills"));
			loseTimes = int.Parse(cfg.GetSingle("Save", "Achi_LoseTimes"));
			newAvatarGot = int.Parse(cfg.GetSingle("Save", "Achi_NewAvatarGot"));
			upgradeTenTimes = int.Parse(cfg.GetSingle("Save", "Achi_UpgradeTenTimes"));
			for (int i = 0; i < 16; i++)
			{
				acheivements[i].submitting = bool.Parse(cfg.GetArray("Save", "Achi_Submit", i));
				acheivements[i].complete = bool.Parse(cfg.GetArray("Save", "Achi_Complete", i));
			}
		}

		public void CheckAchievemnet_NewBattleAbility()
		{
			if (!acheivements[0].complete && newWeaponsGot == 1)
			{
				Debug.Log("Achievement: NewBattleAbility!");
				acheivements[0].submitting = true;
				acheivements[0].complete = true;
			}
		}

		public void CheckAchievemnet_WeaponHouseware()
		{
			if (!acheivements[1].complete && newWeaponsGot == 5)
			{
				Debug.Log("Achievement: Weapon Store!");
				acheivements[1].submitting = true;
				acheivements[1].complete = true;
			}
		}

		public void CheckAchievemnet_SawKillers()
		{
			if (!acheivements[2].complete && sawKills == 300)
			{
				Debug.Log("Achievement: SawKillers!");
				acheivements[2].submitting = true;
				acheivements[2].complete = true;
			}
		}

		public void CheckAchievemnet_TookAShoot()
		{
			if (!acheivements[3].complete && enemyKills == 10)
			{
				Debug.Log("Achievement: Took A Shoot!");
				acheivements[3].submitting = true;
				acheivements[3].complete = true;
			}
		}

		public void CheckAchievemnet_BraveHeart()
		{
			if (!acheivements[4].complete)
			{
				Debug.Log("Achievement: BraveHeart!");
				acheivements[4].submitting = true;
				acheivements[4].complete = true;
			}
		}

		public void CheckAchievemnet_Killer()
		{
			if (!acheivements[5].complete && enemyKills == 3000)
			{
				Debug.Log("Achievement: Killer!");
				acheivements[5].submitting = true;
				acheivements[5].complete = true;
			}
		}

		public void CheckAchievemnet_RichMan(int cash)
		{
			if (!acheivements[6].complete && cash > 100000)
			{
				Debug.Log("Achievement: Rich Man!");
				acheivements[6].submitting = true;
				acheivements[6].complete = true;
			}
		}

		public void CheckAchievemnet_Survivior(int level)
		{
			if (!acheivements[7].complete && level == 30)
			{
				Debug.Log("Achievement: Survivior!");
				acheivements[7].submitting = true;
				acheivements[7].complete = true;
			}
		}

		public void CheckAchievemnet_LastSurvivior(int level)
		{
			if (!acheivements[8].complete && level == 50)
			{
				Debug.Log("Achievement: LastSurvivior!");
				acheivements[8].submitting = true;
				acheivements[8].complete = true;
			}
		}

		public void CheckAchievemnet_Avatar()
		{
			if (!acheivements[9].complete && newAvatarGot == 1)
			{
				Debug.Log("Achievement: Avatar!");
				acheivements[9].submitting = true;
				acheivements[9].complete = true;
			}
		}

		public void CheckAchievemnet_AvatarMaster()
		{
			if (!acheivements[10].complete && newAvatarGot == 4)
			{
				Debug.Log("Achievement: AvatarMaster!");
				acheivements[10].submitting = true;
				acheivements[10].complete = true;
			}
		}

		public void CheckAchievemnet_UpgradeMaster()
		{
			if (!acheivements[11].complete && upgradeTenTimes == 3)
			{
				Debug.Log("Achievement: UpgradeMaster!");
				acheivements[11].submitting = true;
				acheivements[11].complete = true;
			}
		}

		public void CheckAchievemnet_Upgrade()
		{
			if (!acheivements[12].complete)
			{
				Debug.Log("Achievement: Upgrade!");
				acheivements[12].submitting = true;
				acheivements[12].complete = true;
			}
		}

		public void CheckAchievemnet_WeaponMaster()
		{
			if (!acheivements[13].complete)
			{
				Debug.Log("Achievement: WeaponMaster!");
				acheivements[13].submitting = true;
				acheivements[13].complete = true;
			}
		}

		public void CheckAchievemnet_NeverGiveUp()
		{
			if (!acheivements[14].complete && loseTimes == 100)
			{
				Debug.Log("Achievement: NeverGiveUp!");
				acheivements[14].submitting = true;
				acheivements[14].complete = true;
			}
		}

		public void CheckAchievemnet_WeaponCollector()
		{
			if (!acheivements[15].complete && GameApp.GetInstance().GetGameState().GotAllWeapons())
			{
				Debug.Log("Achievement: WeaponCollector!");
				acheivements[15].submitting = true;
				acheivements[15].complete = true;
			}
		}
	}
}
