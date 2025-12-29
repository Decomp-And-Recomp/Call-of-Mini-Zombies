using System;
using UnityEngine;

namespace Zombie3D
{
	public class AvatarAttributes
	{
		public AvatarType name;

		public SafeInteger level;

		public SafeInteger EXP;

        public SafeFloat maxHp;

        public SafeFloat damage;

        public SafeFloat moveSpeed;

        public SafeInteger UpgradeExp;

		public AvatarState existState;

		public SafeInteger hpBuyCount;

		public SafeInteger hpBuyNext;

		public SafeInteger hpBuyPrice;

		public AvatarConfig aConf;

		public void ComputeAttributes()
		{
			damage = Mathf.Round(aConf.damageInitial + (aConf.damageFinal - aConf.damageInitial) / (float)(UpgradeParas.AvatarMaxLevel - 1) * (float)(level - 1));
			moveSpeed = Mathf.Round(aConf.speedInitial + (aConf.speedFinal - aConf.speedInitial) / (float)(UpgradeParas.AvatarMaxLevel - 1) * (float)(level - 1));
			ComputeMaxHp();
		}

		public void ComputeUpgradeExp()
		{
			int num = (int)(UpgradeParas.PlayerParas["a"] * Mathf.Pow(level + 1, UpgradeParas.PlayerParas["k"]) + UpgradeParas.PlayerParas["b"]);
			int num2 = (int)(UpgradeParas.PlayerParas["a"] * Mathf.Pow(level, UpgradeParas.PlayerParas["k"]) + UpgradeParas.PlayerParas["b"]);
			UpgradeExp = num - num2;
		}

		public void BuyHp()
		{
			hpBuyCount++;
			ComputeMaxHp();
		}

		public void ComputeMaxHp()
		{
			maxHp = Mathf.Round(aConf.hpInitial + (aConf.hpFinal - aConf.hpInitial) / (float)(UpgradeParas.AvatarMaxLevel - 1) * (float)(level - 1));
			int num = 0;
			if (hpBuyCount > 0)
			{
				num = (int)((UpgradeParas.AvatarHpParas["a"] * Mathf.Pow(hpBuyCount, 2f) + UpgradeParas.AvatarHpParas["b"] * (float)hpBuyCount + UpgradeParas.AvatarHpParas["c"]) * aConf.hpBuyWeight);
				num = ((num % 10 >= 5) ? (num - num % 10 + 10) : (num - num % 10));
				maxHp += num;
			}
			int num2 = (int)((UpgradeParas.AvatarHpParas["a"] * Mathf.Pow(hpBuyCount + 1, 2f) + UpgradeParas.AvatarHpParas["b"] * (float)(hpBuyCount + 1) + UpgradeParas.AvatarHpParas["c"]) * aConf.hpBuyWeight);
			num2 = ((num2 % 10 >= 5) ? (num2 - num2 % 10 + 10) : (num2 - num2 % 10));
			hpBuyNext = num2 - num;
			hpBuyPrice = (int)((UpgradeParas.AvatarHpPriceParas["a"] * Mathf.Pow(hpBuyCount + 1, UpgradeParas.AvatarHpPriceParas["k"]) + UpgradeParas.AvatarHpPriceParas["b"]) * aConf.hpBuyPriceWeight);
		}

		public int GetLastMaxHp()
		{
			int num = (int)((UpgradeParas.AvatarHpParas["a"] * Mathf.Pow(UpgradeParas.AvatarHpMaxLevel, 2f) + UpgradeParas.AvatarHpParas["b"] * (float)UpgradeParas.AvatarHpMaxLevel + UpgradeParas.AvatarHpParas["c"]) * aConf.hpBuyWeight);
			num = ((num % 10 >= 5) ? (num - num % 10 + 10) : (num - num % 10));
			return Mathf.RoundToInt(aConf.hpFinal) + num;
		}

		public bool CheckLevelUp()
		{
			if (EXP >= UpgradeExp)
			{
				level++;
				EXP = 0;
				ComputeAttributes();
				ComputeUpgradeExp();
				return true;
			}
			return false;
		}

		public float GetExpPercent()
		{
			return (float)Math.Round((float)EXP / (float)UpgradeExp * 100f, 2);
		}
	}
}
