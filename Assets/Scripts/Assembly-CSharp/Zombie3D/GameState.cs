using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Zombie3D
{
    public class GameState
    {
        public string last_scene = string.Empty;

        protected SafeInteger cash = new SafeInteger(0);

        protected SafeInteger crystal = new SafeInteger(0);

        public int tapjoyPoints;

        protected List<Weapon> weaponList = new List<Weapon>();

        protected List<Item> itemList = new List<Item>();

        protected List<AvatarAttributes> avatarList = new List<AvatarAttributes>();

        protected int[] rectToWeaponMap = new int[3];

        public SafeInteger Medpack = 1;

        public SafeInteger Revivepack;

        public List<AvatarType> unlockNewAvatar = new List<AvatarType>();

        public List<int> unlockNewWeapon = new List<int>();

        protected Dictionary<string, bool> checkInited = new Dictionary<string, bool>();

        protected GameConfig gConfig;

        public int[] MultiAchievementData;

        public int[] VsAchievementData;

        public int gameCounterForPlayerExpBuff = -1;

        public GameMode gameMode;

        public int review_count;

        public float Game_ver;

        public float Game_last_ver;

        public bool show_update_ad;

        public bool show_avatar_update;

        public bool show_weapon_update;

        public bool show_pixelMap_update;

        public bool makeUpCrystal;

        public bool show_zombies2_link = true;

        public int Hunting_val;

        public string cur_net_map = string.Empty;

        public SafeInteger loot_exp;

        public SafeInteger loot_cash = new SafeInteger(0);

        public string nick_name = string.Empty;

        public bool[] tutorialTriggers = new bool[11];

        public bool soldOutNewbie1;

        public bool soldOutNewbie2;

        public int coopLoseCount;

        public int vsLoseCount;

        public NetworkObj net_com;

        public NetworkObj net_com_hall;

        public float macos_sen = 20f;

        public IapDiscountManager iapDiscount = new IapDiscountManager();

        public DateTime lastTimeLogged = DateTime.MinValue;

        protected string path = Application.persistentDataPath + "/";

        public int LevelNum { get; set; }

        public AvatarType Avatar { get; set; }

        public AchievementState Achievement { get; set; }

        public bool MusicOn { get; set; }

        public bool SoundOn { get; set; }

        public bool PaperModelShow { get; set; }

        public bool AlreadyReviewCountered { get; set; }

        public int instance_score
        {
            get
            {
                return Mathf.RoundToInt((float)loot_cash * InstanceModeConfig.ScoreAdjust);
            }
        }

        public int[] RectToWeaponMap
        {
            get
            {
                return rectToWeaponMap;
            }
            set
            {
                rectToWeaponMap = value;
            }
        }

        public GameState()
        {
            checkInited.Add("weapon", false);
            checkInited.Add("item", false);
            checkInited.Add("avatar", false);
            checkInited.Add("basic", false);
        }

        public bool GetInited()
        {
            return checkInited["basic"];
        }

        public bool GotAllWeapons()
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                if (weaponList[i].Exist != WeaponExistState.Owned)
                {
                    return false;
                }
            }
            return true;
        }

        public string GetWeaponNameByIndex(int index)
        {
            string result = "null";
            if (index < weaponList.Count)
            {
                result = weaponList[index].Name;
            }
            return result;
        }

        public Weapon GetWeaponByName(string m_name)
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                if (weaponList[i].Name == m_name)
                {
                    return weaponList[i];
                }
            }
            return null;
        }

        public AvatarAttributes GetAvatarByType(AvatarType aType)
        {
            return avatarList[(int)aType];
        }

        public Item GetItemByType(ItemType type)
        {
            foreach (Item item in itemList)
            {
                if (item.iType == type)
                {
                    return item;
                }
            }
            return null;
        }

        private void AdjustWeaponSelected()
        {
            UnselectAllWeapons();
            if (LevelNum == 0)
            {
                rectToWeaponMap[0] = 0;
                rectToWeaponMap[1] = -1;
                rectToWeaponMap[2] = -1;
                weaponList[0].IsSelectedForBattle = true;
                return;
            }
            bool flag = false;
            for (int i = 0; i < rectToWeaponMap.Length; i++)
            {
                if (rectToWeaponMap[i] != -1)
                {
                    weaponList[rectToWeaponMap[i]].IsSelectedForBattle = true;
                    flag = true;
                }
            }
            if (!flag)
            {
                rectToWeaponMap[0] = 0;
                weaponList[0].IsSelectedForBattle = true;
                rectToWeaponMap[1] = GetWeaponByName("Chainsaw").weapon_index;
                weaponList[rectToWeaponMap[1]].IsSelectedForBattle = true;
            }
        }

        public static string GameSaveStringEncipher(string str, int key)
        {
            char[] array = str.ToCharArray();
            char[] array2 = str.ToCharArray();
            char[] array3 = new char[2] { '\0', '\0' };
            for (int i = 0; i < array.Length; i++)
            {
                char c = array[i];
                array3[0] = c;
                string s = new string(array3);
                int num = char.ConvertToUtf32(s, 0);
                num ^= key;
                array2[i] = char.ConvertFromUtf32(num)[0];
            }
            return new string(array2);
        }

        public void LoadData(BinaryReader br)
        {
            Init();
            InitWeapons();
            InitAvatars();
            InitItems();
            cash = br.ReadInt32();
            int num = br.ReadInt32();
            LevelNum = br.ReadInt32();
            int num2 = br.ReadInt32();
            Debug.Log("weapon counts = " + num2 + " cfg counts = " + weaponList.Count);
            for (int i = 0; i < num2; i++)
            {
                int num3 = br.ReadInt32();
                string text = br.ReadString();
                switch (text)
                {
                    case "Winchester 1200":
                        text = "Winchester-1200";
                        break;
                    case "Remington 870":
                        text = "Remington-870";
                        break;
                    case "XM 1014":
                        text = "XM-1014";
                        break;
                }
                Weapon weaponByName = GetWeaponByName(text);
                if (weaponByName != null)
                {
                    weaponByName.BulletCount = br.ReadInt32();
                    float num4 = (float)br.ReadDouble();
                    float num5 = (float)br.ReadDouble();
                    float num6 = (float)br.ReadDouble();
                    int num7 = br.ReadInt32();
                    int num8 = br.ReadInt32();
                    int num9 = br.ReadInt32();
                    bool flag = br.ReadBoolean();
                    weaponByName.Level = num7 + num8 + num9;
                    if (weaponByName.Level > UpgradeParas.WeaponMaxLevel)
                    {
                        weaponByName.Level = UpgradeParas.WeaponMaxLevel;
                    }
                    weaponByName.ComputeAttributes();
                    weaponByName.Exist = (WeaponExistState)br.ReadInt32();
                    if (LevelNum >= weaponByName.WConf.UnlockLevel && weaponByName.Exist == WeaponExistState.Locked)
                    {
                        weaponByName.Exist = WeaponExistState.Unlocked;
                    }
                }
            }
            for (int j = 0; j < rectToWeaponMap.Length; j++)
            {
                rectToWeaponMap[j] = br.ReadInt32();
                if (rectToWeaponMap[j] >= weaponList.Count)
                {
                    rectToWeaponMap[j] = -1;
                }
            }
            AdjustWeaponSelected();
            if (num2 == 13)
            {
                for (int k = 0; k < 8; k++)
                {
                    switch (br.ReadInt32())
                    {
                        case 0:
                            avatarList[k].existState = AvatarState.Avaliable;
                            break;
                        case 2:
                            if (LevelNum >= avatarList[k].aConf.unlockDay)
                            {
                                avatarList[k].existState = AvatarState.ToBuy;
                            }
                            else
                            {
                                avatarList[k].existState = AvatarState.Locked;
                            }
                            break;
                    }
                }
            }
            else
            {
                int num10 = br.ReadInt32();
                for (int l = 0; l < num10; l++)
                {
                    switch (br.ReadInt32())
                    {
                        case 0:
                            avatarList[l].existState = AvatarState.Avaliable;
                            break;
                        case 2:
                            if (LevelNum >= avatarList[l].aConf.unlockDay)
                            {
                                avatarList[l].existState = AvatarState.ToBuy;
                            }
                            else
                            {
                                avatarList[l].existState = AvatarState.Locked;
                            }
                            break;
                    }
                }
            }
            Avatar = (AvatarType)br.ReadInt32();
            int num11 = br.ReadInt32();
            Achievement.Load(br);
        }

        public void LoadDataNew(bool isNew)
        {
            Init();
            InitWeapons();
            InitAvatars();
            InitItems();
            Configure configure = new Configure();
#if UNITY_EDITOR
            const string savePath = "Editor_CallMini_New.save";
#else
            const string savePath = "CallMini_New.save";
#endif
            Stream stream = ((!isNew) ? File.Open(path + savePath, FileMode.Open) : File.Open(path + savePath, FileMode.Open));
            BinaryReader binaryReader = new BinaryReader(stream);
            string str = binaryReader.ReadString();
            binaryReader.Close();
            stream.Close();
            //str = GameSaveStringEncipher(str, 71);
#if !UNITY_EDITOR
            str = T.External.XXTEAUtils.Decrypt(str, PrivateData.SaveEncryptionKey);
#endif
            configure.Load(str);
            Debug.Log(str);
            cash.Set(int.Parse(configure.GetSingle("Save", "Cash")));
            if (configure.GetSingle("Save", "Crystal") != null)
            {
                crystal.Set(int.Parse(configure.GetSingle("Save", "Crystal")));
            }
            if (configure.GetSingle("Save", "TapjoyPoints") != null)
            {
                tapjoyPoints = int.Parse(configure.GetSingle("Save", "TapjoyPoints"));
            }
            Avatar = (AvatarType)int.Parse(configure.GetSingle("Save", "AvatarType"));
            LevelNum = int.Parse(configure.GetSingle("Save", "LevelNum"));
            if (configure.GetSingle("Save", "ExpBuffGameCounter") != null)
            {
                gameCounterForPlayerExpBuff = int.Parse(configure.GetSingle("Save", "ExpBuffGameCounter"));
            }
            else
            {
                gameCounterForPlayerExpBuff = -1;
            }
            bool flag = true;
            if (configure.GetSingle("Save", "NewWeaponConfigData") == null)
            {
                flag = false;
            }
            int num = int.Parse(configure.GetSingle("Save", "WeaponListCount"));
            for (int i = 0; i < num; i++)
            {
                int num2 = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 0));
                string text = configure.GetArray2("Save", "WeaponsCfg", i, 1);
                if (text == "HellFire")
                {
                    text = "Flamethrower";
                }
                else if (text == "ElectricGun")
                {
                    text = "Ion-Cannon";
                }
                Weapon weaponByName = GetWeaponByName(text);
                if (weaponByName == null)
                {
                    continue;
                }
                if (flag)
                {
                    weaponByName.BulletCount = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 2));
                    weaponByName.Level = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 3));
                    weaponByName.ComputeAttributes();
                    weaponByName.Exist = (WeaponExistState)int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 4));
                    if (LevelNum >= weaponByName.WConf.UnlockLevel && weaponByName.Exist == WeaponExistState.Locked)
                    {
                        weaponByName.Exist = WeaponExistState.Unlocked;
                    }
                    continue;
                }
                weaponByName.BulletCount = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 2));
                int num3 = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 6));
                int num4 = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 7));
                int num5 = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 8));
                weaponByName.Level = num3 + num4 + num5;
                if (weaponByName.Level <= 1)
                {
                    weaponByName.Level.Set(1);
                }
                weaponByName.ComputeAttributes();
                weaponByName.Exist = (WeaponExistState)int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 10));
                if (LevelNum >= weaponByName.WConf.UnlockLevel && weaponByName.Exist == WeaponExistState.Locked)
                {
                    weaponByName.Exist = WeaponExistState.Unlocked;
                }
            }
            for (int j = num; j < weaponList.Count; j++)
            {
                if (weaponList[j].Exist == WeaponExistState.Locked && LevelNum >= weaponList[j].WConf.UnlockLevel)
                {
                    weaponList[j].Exist = WeaponExistState.Unlocked;
                }
            }
            for (int k = 0; k < rectToWeaponMap.Length; k++)
            {
                rectToWeaponMap[k] = int.Parse(configure.GetArray("Save", "RectToWeaponMap", k));
                if (rectToWeaponMap[k] >= weaponList.Count)
                {
                    rectToWeaponMap[k] = -1;
                }
            }
            AdjustWeaponSelected();
            int num6 = 0;
            if (configure.GetArray2("Save", "AvatarInfo", 0, 0) != null)
            {
                num6 = configure.CountArray2("Save", "AvatarInfo");
                for (int l = 0; l < num6; l++)
                {
                    int num7 = int.Parse(configure.GetArray2("Save", "AvatarInfo", l, 0));
                    if (num7 < avatarList.Count)
                    {
                        avatarList[num7].level = int.Parse(configure.GetArray2("Save", "AvatarInfo", l, 1));
                        avatarList[num7].existState = (AvatarState)int.Parse(configure.GetArray2("Save", "AvatarInfo", l, 2));
                        if (avatarList[num7].existState == AvatarState.Locked && LevelNum >= avatarList[num7].aConf.unlockDay)
                        {
                            avatarList[num7].existState = AvatarState.ToBuy;
                        }
                        avatarList[num7].EXP = int.Parse(configure.GetArray2("Save", "AvatarInfo", l, 3));
                        if (configure.GetArray2("Save", "AvatarInfo", l, 4) != null)
                        {
                            avatarList[num7].hpBuyCount = int.Parse(configure.GetArray2("Save", "AvatarInfo", l, 4));
                        }
                        avatarList[num7].ComputeAttributes();
                        avatarList[num7].ComputeUpgradeExp();
                    }
                }
            }
            else if (configure.GetSingle("Save", "AvatarDataLength") != null && configure.GetArray("Save", "avatarData", 0) != null)
            {
                num6 = int.Parse(configure.GetSingle("Save", "AvatarDataLength"));
                for (int m = 0; m < num6; m++)
                {
                    switch (int.Parse(configure.GetArray("Save", "avatarData", m)))
                    {
                        case 0:
                            avatarList[m].existState = AvatarState.Avaliable;
                            break;
                        case 2:
                            if (LevelNum >= avatarList[m].aConf.unlockDay)
                            {
                                avatarList[m].existState = AvatarState.ToBuy;
                            }
                            else
                            {
                                avatarList[m].existState = AvatarState.Locked;
                            }
                            break;
                    }
                }
            }
            for (int n = num6; n < avatarList.Count; n++)
            {
                if (avatarList[n].existState == AvatarState.Locked && LevelNum >= avatarList[n].aConf.unlockDay)
                {
                    avatarList[n].existState = AvatarState.ToBuy;
                }
            }
            if (configure.GetSingle("Save", "MultiAchievementNew") != null)
            {
                for (int num8 = 0; num8 < 54; num8++)
                {
                    MultiAchievementData[num8] = int.Parse(configure.GetArray("Save", "MultiAchievementDataNew", num8));
                }
            }
            if (configure.GetSingle("Save", "VsAchievement") != null)
            {
                for (int num9 = 0; num9 < 36; num9++)
                {
                    VsAchievementData[num9] = int.Parse(configure.GetArray("Save", "VsAchievementData", num9));
                }
            }
            Achievement.LoadNew(configure);
            if (configure.GetArray2("Save", "GameItemsPacket", 0, 0) != null)
            {
                int num10 = configure.CountArray2("Save", "GameItemsPacket");
                int num11 = 1;
                for (int num12 = 0; num12 < num10; num12++)
                {
                    if (configure.GetArray2("Save", "GameItemsPacket", num12, 0) == null)
                    {
                        continue;
                    }
                    int type = int.Parse(configure.GetArray2("Save", "GameItemsPacket", num12, 0));
                    int ownedCount = int.Parse(configure.GetArray2("Save", "GameItemsPacket", num12, 1));
                    Item itemByType = GetItemByType((ItemType)type);
                    if (itemByType == null)
                    {
                        continue;
                    }
                    itemByType.OwnedCount = ownedCount;
                    if (configure.GetArray2("Save", "GameItemsPacket", num12, 2) != null)
                    {
                        int num13 = int.Parse(configure.GetArray2("Save", "GameItemsPacket", num12, 2));
                        if (num13 == 1)
                        {
                            itemByType.isCarryInGame = true;
                            itemByType.carryPacketIndex = num11;
                            num11++;
                        }
                        else
                        {
                            itemByType.isCarryInGame = false;
                            itemByType.carryPacketIndex = 0;
                        }
                    }
                }
            }
            if (configure.GetSingle("Save", "RescuePacketCount") != null)
            {
                Medpack.Set(int.Parse(configure.GetSingle("Save", "RescuePacketCount")));
            }
            else
            {
                Medpack.Set(1);
            }
            if (configure.GetSingle("Save", "RebirthPacketCount") != null)
            {
                Revivepack.Set(int.Parse(configure.GetSingle("Save", "RebirthPacketCount")));
            }
            else
            {
                Revivepack.Set(0);
            }
            if (configure.GetArray2("Save", "IapDiscount", 0, 0) != null)
            {
                iapDiscount.gotActiveDiscount = int.Parse(configure.GetArray2("Save", "IapDiscount", 0, 0)) == 1;
                string text2 = string.Empty;
                for (int num14 = 0; configure.GetArray2("Save", "IapDiscount", 1, num14) != null; num14++)
                {
                    text2 += configure.GetArray2("Save", "IapDiscount", 1, num14);
                    text2 += " ";
                }
                lastTimeLogged = DateTime.Parse(text2.Substring(0, text2.Length - 1));
                iapDiscount.CheckActiveDiscount(lastTimeLogged);
                if (configure.GetArray2("Save", "IapDiscount", 2, 0) != "NoIAPDiscount")
                {
                    IapDiscountManager.DiscountInfo discountInfo = new IapDiscountManager.DiscountInfo();
                    discountInfo.type = (IapDiscountManager.DiscountType)int.Parse(configure.GetArray2("Save", "IapDiscount", 2, 0));
                    text2 = string.Empty;
                    for (int num14 = 1; configure.GetArray2("Save", "IapDiscount", 2, num14) != null; num14++)
                    {
                        text2 += configure.GetArray2("Save", "IapDiscount", 2, num14);
                        text2 += " ";
                    }
                    discountInfo.startTime = DateTime.Parse(text2.Substring(0, text2.Length - 1));
                    iapDiscount.CheckFinalDiscount(discountInfo);
                }
            }
            if (configure.GetArray("Save", "NewbiePackInfo", 0) != null)
            {
                soldOutNewbie1 = int.Parse(configure.GetArray("Save", "NewbiePackInfo", 0)) == 1;
                soldOutNewbie2 = int.Parse(configure.GetArray("Save", "NewbiePackInfo", 1)) == 1;
                coopLoseCount = int.Parse(configure.GetArray("Save", "NewbiePackInfo", 2));
                vsLoseCount = int.Parse(configure.GetArray("Save", "NewbiePackInfo", 3));
            }
            else
            {
                soldOutNewbie1 = false;
                soldOutNewbie2 = false;
                coopLoseCount = 0;
                vsLoseCount = 0;
            }
        }

        public void SaveDataNew()
        {
            Init();
            InitWeapons();
            InitAvatars();
            InitItems();
            Configure configure = new Configure();
            configure.AddSection("Save", string.Empty, string.Empty);
            configure.AddValueSingle("Save", "Cash", cash.ToString(), string.Empty, string.Empty);
            configure.AddValueSingle("Save", "Crystal", crystal.ToString(), string.Empty, string.Empty);
            configure.AddValueSingle("Save", "TapjoyPoints", tapjoyPoints.ToString(), string.Empty, string.Empty);
            configure.AddValueSingle("Save", "AvatarType", ((int)Avatar).ToString(), string.Empty, string.Empty);
            configure.AddValueSingle("Save", "LevelNum", LevelNum.ToString(), string.Empty, string.Empty);
            configure.AddValueSingle("Save", "ExpBuffGameCounter", gameCounterForPlayerExpBuff.ToString(), string.Empty, string.Empty);
            configure.AddValueSingle("Save", "NewWeaponConfigData", "1", string.Empty, string.Empty);
            configure.AddValueSingle("Save", "WeaponListCount", weaponList.Count.ToString(), string.Empty, string.Empty);
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < weaponList.Count; i++)
            {
                StringLine stringLine = new StringLine();
                stringLine.AddString(((int)weaponList[i].GetWeaponType()).ToString());
                stringLine.AddString(weaponList[i].Name);
                stringLine.AddString(weaponList[i].BulletCount.ToString());
                stringLine.AddString(weaponList[i].Level.ToString());
                stringLine.AddString(((int)weaponList[i].Exist).ToString());
                arrayList.Add(stringLine.content);
            }
            configure.AddValueArray2("Save", "WeaponsCfg", arrayList, string.Empty, string.Empty);
            StringLine stringLine2 = new StringLine();
            for (int j = 0; j < rectToWeaponMap.Length; j++)
            {
                stringLine2.AddString(rectToWeaponMap[j].ToString());
            }
            configure.AddValueArray("Save", "RectToWeaponMap", stringLine2.content, string.Empty, string.Empty);
            ArrayList arrayList2 = new ArrayList();
            for (int k = 0; k < avatarList.Count; k++)
            {
                StringLine stringLine3 = new StringLine();
                int name = (int)avatarList[k].name;
                stringLine3.AddString(name.ToString());
                stringLine3.AddString(avatarList[k].level.ToString());
                int existState = (int)avatarList[k].existState;
                stringLine3.AddString(existState.ToString());
                stringLine3.AddString(avatarList[k].EXP.ToString());
                stringLine3.AddString(avatarList[k].hpBuyCount.ToString());
                arrayList2.Add(stringLine3.content);
            }
            configure.AddValueArray2("Save", "AvatarInfo", arrayList2, string.Empty, string.Empty);
            configure.AddValueSingle("Save", "MultiAchievementNew", "1", string.Empty, string.Empty);
            StringLine stringLine4 = new StringLine();
            for (int l = 0; l < MultiAchievementData.Length; l++)
            {
                stringLine4.AddString(MultiAchievementData[l].ToString());
            }
            configure.AddValueArray("Save", "MultiAchievementDataNew", stringLine4.content, string.Empty, string.Empty);
            configure.AddValueSingle("Save", "VsAchievement", "1", string.Empty, string.Empty);
            StringLine stringLine5 = new StringLine();
            for (int m = 0; m < VsAchievementData.Length; m++)
            {
                stringLine5.AddString(VsAchievementData[m].ToString());
            }
            configure.AddValueArray("Save", "VsAchievementData", stringLine5.content, string.Empty, string.Empty);
            Achievement.SaveNew(configure);
            ArrayList arrayList3 = new ArrayList();
            for (int n = 0; n < itemList.Count; n++)
            {
                StringLine stringLine6 = new StringLine();
                stringLine6.AddString(((int)itemList[n].iType).ToString());
                stringLine6.AddString(itemList[n].OwnedCount.ToString());
                if (itemList[n].isCarryInGame)
                {
                    stringLine6.AddString("1");
                }
                else
                {
                    stringLine6.AddString("0");
                }
                arrayList3.Add(stringLine6.content);
            }
            configure.AddValueArray2("Save", "GameItemsPacket", arrayList3, string.Empty, string.Empty);
            if (Medpack > 1)
            {
                configure.AddValueSingle("Save", "RescuePacketCount", Medpack.ToString(), string.Empty, string.Empty);
            }
            if (Revivepack > 0)
            {
                configure.AddValueSingle("Save", "RebirthPacketCount", Revivepack.ToString(), string.Empty, string.Empty);
            }
            ArrayList arrayList4 = new ArrayList();
            StringLine stringLine7 = new StringLine();
            stringLine7.AddString((iapDiscount.gotActiveDiscount ? 1 : 0).ToString());
            arrayList4.Add(stringLine7.content);
            stringLine7 = new StringLine();
            stringLine7.AddString(DateTime.Now.ToString());
            arrayList4.Add(stringLine7.content);
            if (iapDiscount.discountInfo == null || (iapDiscount.discountInfo != null && iapDiscount.discountInfo.type == IapDiscountManager.DiscountType.None))
            {
                stringLine7 = new StringLine();
                stringLine7.AddString("NoIAPDiscount");
                arrayList4.Add(stringLine7.content);
            }
            else
            {
                stringLine7 = new StringLine();
                StringLine stringLine8 = stringLine7;
                int type = (int)iapDiscount.discountInfo.type;
                stringLine8.AddString(type.ToString());
                stringLine7.AddString(iapDiscount.discountInfo.startTime.ToString());
                arrayList4.Add(stringLine7.content);
            }
            configure.AddValueArray2("Save", "IapDiscount", arrayList4, string.Empty, string.Empty);
            StringLine stringLine9 = new StringLine();
            stringLine9.AddString((soldOutNewbie1 ? 1 : 0).ToString());
            stringLine9.AddString((soldOutNewbie2 ? 1 : 0).ToString());
            stringLine9.AddString(coopLoseCount.ToString());
            stringLine9.AddString(vsLoseCount.ToString());
            configure.AddValueArray("Save", "NewbiePackInfo", stringLine9.content, string.Empty, string.Empty);
            string str = configure.Save();
#if !UNITY_EDITOR
            str = T.External.XXTEAUtils.Encrypt(str, PrivateData.SaveEncryptionKey);
#endif
            //str = GameSaveStringEncipher(str, 71);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
#if UNITY_EDITOR
            const string savePath = "Editor_CallMini_New.save";
#else
            const string savePath = "CallMini_New.save";
#endif
            Stream stream = File.Open(path + savePath, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            //binaryWriter.Write(str);
            binaryWriter.Write(str);
            binaryWriter.Close();
            stream.Close();
        }

        public void ClearState()
        {
            checkInited["weapon"] = false;
            checkInited["item"] = false;
            checkInited["avatar"] = false;
            checkInited["basic"] = false;
            Init();
            InitWeapons();
            InitItems();
            InitAvatars();
            LevelNum = 1;
            GetWeaponByName("Chainsaw").IsSelectedForBattle = true;
            rectToWeaponMap[1] = GetWeaponByName("Chainsaw").weapon_index;
        }

        public void Init()
        {
            if (!checkInited["basic"])
            {
                gConfig = GameApp.GetInstance().GetGameConfig();
                cash.Set(3000);
                crystal.Set(0);
                tapjoyPoints = 0;
                SoundOn = true;
                MusicOn = true;
                AlreadyReviewCountered = false;
                Achievement = new AchievementState();
                MultiAchievementData = new int[54];
                for (int i = 0; i < MultiAchievementData.Length; i++)
                {
                    MultiAchievementData[i] = 0;
                }
                VsAchievementData = new int[36];
                for (int j = 0; j < VsAchievementData.Length; j++)
                {
                    VsAchievementData[j] = 0;
                }
                LevelNum = 0;
                iapDiscount.Init();
                for (int k = 0; k < tutorialTriggers.Length; k++)
                {
                    tutorialTriggers[k] = true;
                }
                checkInited["basic"] = true;
            }
        }

        public Weapon InitMultiWeapon(int index)
        {
            List<WeaponConfig> weapons = gConfig.GetWeapons();
            Weapon weapon = null;
            int num = 0;
            foreach (WeaponConfig item in weapons)
            {
                if (num == index)
                {
                    weapon = WeaponFactory.GetInstance().CreateWeapon(item.wType, true);
                    weapon.Exist = item.startEquip;
                    weapon.Name = item.name;
                    weapon.WConf = item;
                    weapon.LoadConfig();
                    break;
                }
                num++;
            }
            return weapon;
        }

        public void InitWeapons()
        {
            if (checkInited["weapon"])
            {
                return;
            }
            weaponList.Clear();
            rectToWeaponMap[0] = 0;
            rectToWeaponMap[1] = -1;
            rectToWeaponMap[2] = -1;
            List<WeaponConfig> weapons = gConfig.GetWeapons();
            int num = 0;
            foreach (WeaponConfig item in weapons)
            {
                Weapon weapon = WeaponFactory.GetInstance().CreateWeapon(item.wType, false);
                weapon.weapon_index = num;
                if (item.name == "MP5")
                {
                    weapon.IsSelectedForBattle = true;
                    rectToWeaponMap[0] = num;
                }
                weapon.Exist = item.startEquip;
                weapon.Name = item.name;
                weapon.Level = 1;
                if (weapon.Level > UpgradeParas.WeaponMaxLevel)
                {
                    weapon.Level = UpgradeParas.WeaponMaxLevel;
                }
                weapon.WConf = item;
                weapon.LoadConfig();
                weaponList.Add(weapon);
                num++;
            }
            checkInited["weapon"] = true;
        }

        public void InitAvatars()
        {
            if (checkInited["avatar"])
            {
                return;
            }
            Avatar = AvatarType.Human;
            avatarList.Clear();
            List<AvatarConfig> avatars = gConfig.GetAvatars();
            foreach (AvatarConfig item in avatars)
            {
                AvatarAttributes avatarAttributes = new AvatarAttributes();
                avatarAttributes.name = item.name;
                avatarAttributes.level = 1;
                avatarAttributes.EXP = 0;
                avatarAttributes.damage = item.damageInitial;
                avatarAttributes.maxHp = item.hpInitial;
                avatarAttributes.moveSpeed = item.speedInitial;
                avatarAttributes.aConf = item;
                avatarAttributes.ComputeUpgradeExp();
                avatarAttributes.hpBuyCount = 0;
                avatarAttributes.ComputeMaxHp();
                if (avatarAttributes.name == AvatarType.Human)
                {
                    avatarAttributes.existState = AvatarState.Avaliable;
                }
                else if (item.unlockDay > 0)
                {
                    avatarAttributes.existState = AvatarState.Locked;
                }
                else
                {
                    avatarAttributes.existState = AvatarState.ToBuy;
                }
                avatarList.Add(avatarAttributes);
            }
            checkInited["avatar"] = true;
        }

        public void InitItems()
        {
            // Ensure checkInited exists
            if (checkInited == null)
                checkInited = new Dictionary<string, bool>();

            // If already initialized, just return
            if (checkInited.ContainsKey("item") && checkInited["item"])
                return;

            // Ensure itemList exists
            if (itemList == null)
                itemList = new List<Item>();
            else
                itemList.Clear();

            // Ensure gConfig exists
            if (gConfig != null)
            {
                List<ItemConfig> items = gConfig.GetItems();
                if (items != null)
                {
                    foreach (ItemConfig itemConf in items)
                    {
                        if (itemConf != null)
                        {
                            Item item = new Item(itemConf.iType);
                            item.iConf = itemConf;
                            itemList.Add(item);
                        }
                    }
                }
            }

            // Initialize Medpack and Revivepack safely
            if (Medpack == null) Medpack = new SafeInteger();
            Medpack.Set(1);

            if (Revivepack == null) Revivepack = new SafeInteger();
            Revivepack.Set(0);

            // Mark as initialized
            checkInited["item"] = true;
        }

        public void DeliverIAPItem(IAPName iapName)
        {
            switch (iapName)
            {
                case IAPName.Cash1:
                    AddCash(120000);
                    break;
                case IAPName.Cash2:
                    AddCash(330000);
                    break;
                case IAPName.Cash3:
                    AddCash(760000);
                    break;
                case IAPName.Cash4:
                    AddCash(1500000);
                    break;
                case IAPName.Cash5:
                    AddCash(4200000);
                    break;
                case IAPName.Cash6:
                    AddCash(9600000);
                    break;
                case IAPName.Crystal1:
                    AddCrystal(50);
                    break;
                case IAPName.Crystal2:
                    AddCrystal(150);
                    break;
                case IAPName.Crystal3:
                    AddCrystal(350);
                    break;
                case IAPName.Crystal4:
                    AddCrystal(800);
                    break;
                case IAPName.Crystal5:
                    AddCrystal(2500);
                    break;
                case IAPName.Crystal6:
                    AddCrystal(6200);
                    break;
                case IAPName.Exp15:
                    gameCounterForPlayerExpBuff = 10;
                    break;
                case IAPName.Exp25:
                    gameCounterForPlayerExpBuff = 115;
                    break;
                case IAPName.IapDiscountActive:
                    AddCrystal(298);
                    iapDiscount.gotActiveDiscount = true;
                    break;
                case IAPName.IapDiscountInactive1:
                case IAPName.IapDiscountInactive2:
                case IAPName.IapDiscountInactive3:
                    AddCrystal(150);
                    break;
                case IAPName.Newbie1:
                    GetItemByType(ItemType.BigHp).OwnedCount += 2;
                    GetItemByType(ItemType.Shield).OwnedCount += 2;
                    GetItemByType(ItemType.InstantStealth).OwnedCount += 2;
                    GetItemByType(ItemType.InstantSuper).OwnedCount += 2;
                    gameCounterForPlayerExpBuff = 10;
                    AddCrystal(40);
                    soldOutNewbie1 = true;
                    break;
                case IAPName.Newbie2:
                    GetItemByType(ItemType.BigHp).OwnedCount += 5;
                    GetItemByType(ItemType.Shield).OwnedCount += 5;
                    GetItemByType(ItemType.InstantStealth).OwnedCount += 5;
                    GetItemByType(ItemType.InstantSuper).OwnedCount += 5;
                    gameCounterForPlayerExpBuff = 115;
                    AddCrystal(200);
                    soldOutNewbie2 = true;
                    break;
            }
            GameApp.GetInstance().Save();
            FlurryStatistics.IapBuy(iapName);
        }

        public List<Weapon> GetBattleWeapons()
        {
            List<Weapon> list = new List<Weapon>();
            for (int i = 0; i < rectToWeaponMap.Length; i++)
            {
                if (rectToWeaponMap[i] != -1)
                {
                    list.Add(weaponList[rectToWeaponMap[i]]);
                }
            }
            return list;
        }

        public void DayUp()
        {
            LevelNum++;
            if (LevelNum > 999)
            {
                LevelNum = 999;
            }
            int crystalReward = (LevelNum / 10) + 1;
            AddCrystal(crystalReward);
            foreach (Weapon weapon in weaponList)
            {
                if (LevelNum == weapon.WConf.UnlockLevel && weapon.Exist == WeaponExistState.Locked)
                {
                    weapon.Exist = WeaponExistState.Unlocked;
                    GameApp.GetInstance().GetGameScene().UnlockWeapon = weapon;
                    unlockNewWeapon.Add(weapon.weapon_index);
                    break;
                }
            }
            foreach (AvatarAttributes avatar in avatarList)
            {
                if (LevelNum == avatar.aConf.unlockDay && avatar.existState == AvatarState.Locked)
                {
                    avatar.existState = AvatarState.ToBuy;
                    GameApp.GetInstance().GetGameScene().UnlockAvatar = avatar.name;
                    unlockNewAvatar.Add(avatar.name);
                    break;
                }
            }
            if (LevelNum == 5 && !soldOutNewbie1)
            {
                MainMapTUI.showNewbie1Intro = true;
            }
        }

        public void AddAvatarExp(int m_exp)
        {
            if (avatarList[(int)Avatar].level < UpgradeParas.AvatarMaxLevel)
            {
                int num = Mathf.RoundToInt((float)m_exp * GameApp.GetInstance().GetGameScene().GetPlayer()
                    .EXPBuff);
                avatarList[(int)Avatar].EXP += num;
                if (avatarList[(int)Avatar].CheckLevelUp())
                {
                    GameApp.GetInstance().GetGameScene().GetPlayer()
                        .Level = avatarList[(int)Avatar].level;
                    GameApp.GetInstance().GetGameScene().GetPlayer()
                        .HP = GameApp.GetInstance().GetGameScene().GetPlayer()
                        .MaxHp;
                    GameApp.GetInstance().GetGameScene().GameGUI.playerInfo.SetPlayerLevel();
                    GameObject gameObject = UnityEngine.Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().AvatarLevelUpEffect, Vector3.zero, Quaternion.identity) as GameObject;
                    gameObject.transform.parent = GameApp.GetInstance().GetGameScene().GetPlayer()
                        .GetTransform();
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
                    GameApp.GetInstance().GetGameScene().GetPlayer()
                        .GetAudioPlayer()
                        .PlayAudio("LevelUp");
                }
                GameApp.GetInstance().GetGameScene().GetPlayer()
                    .ExpProgress = avatarList[(int)Avatar].GetExpPercent();
                loot_exp += num;
            }
        }

        public void AddCash(int cashGot)
        {
            cash = new SafeInteger(Mathf.Clamp((int)cash + cashGot, 0, 99999999));
            Achievement.CheckAchievemnet_RichMan(cash);
        }

        public void AddCashForRecord(int cashGot)
        {
            loot_cash += cashGot;
            AddCash(cashGot);
            GameApp.GetInstance().GetGameScene().UpdateGameStatistics();
        }

        public void LoseCash(int cashSpend)
        {
            cash = new SafeInteger(Mathf.Max(0, (int)cash - cashSpend));
        }

        public int GetCash()
        {
            return cash.Get();
        }

        public int GetCrystal()
        {
            return crystal.Get();
        }

        public void AddCrystal(int crystalGot)
        {
            crystal = new SafeInteger(Mathf.Clamp((int)crystal + crystalGot, 0, 99999));
        }

        public void LoseCrystal(int crystalSpend)
        {
            crystal = new SafeInteger(Mathf.Max(0, (int)crystal - crystalSpend));
        }

        public List<Item> GetItems()
        {
            return itemList;
        }

        public List<Item> GetItemsCarried()
        {
            List<Item> list = new List<Item>();
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].isCarryInGame)
                {
                    list.Add(itemList[i]);
                }
            }
            if (list.Count == 2)
            {
                if (list[0].carryPacketIndex != 1)
                {
                    Item value = list[0];
                    list[0] = list[1];
                    list[1] = value;
                }
            }
            else if (list.Count < 2)
            {
                list.Clear();
                GetItemByType(ItemType.InstantSuper).isCarryInGame = true;
                GetItemByType(ItemType.InstantSuper).carryPacketIndex = 1;
                list.Add(GetItemByType(ItemType.InstantSuper));
                GetItemByType(ItemType.SmallHp).isCarryInGame = true;
                GetItemByType(ItemType.SmallHp).carryPacketIndex = 2;
                list.Add(GetItemByType(ItemType.SmallHp));
            }
            return list;
        }

        public List<Weapon> GetWeapons()
        {
            return weaponList;
        }

        private void UnselectAllWeapons()
        {
            foreach (Weapon weapon in weaponList)
            {
                weapon.IsSelectedForBattle = false;
            }
        }

        public BuyStatus BuyWeapon(Weapon w)
        {
            if (w.WConf.isCrystalBuy)
            {
                if (crystal >= w.WConf.price)
                {
                    LoseCrystal(w.WConf.price);
                    w.Exist = WeaponExistState.Owned;
                    Achievement.GotNewWeapon();
                    return BuyStatus.Succeed;
                }
                return BuyStatus.NotEnoughCrystal;
            }
            if (cash >= w.WConf.price)
            {
                w.Exist = WeaponExistState.Owned;
                LoseCash(w.WConf.price);
                Achievement.GotNewWeapon();
                return BuyStatus.Succeed;
            }
            return BuyStatus.NotEnoughCash;
        }

        public BuyStatus BuyItem(Item m_item)
        {
            if (m_item.iConf.isCrystalBuy)
            {
                if (crystal >= m_item.iConf.price)
                {
                    m_item.OwnedCount += m_item.iConf.buyCount;
                    LoseCrystal(m_item.iConf.price);
                    return BuyStatus.Succeed;
                }
                return BuyStatus.NotEnoughCrystal;
            }
            if (cash >= m_item.iConf.price)
            {
                m_item.OwnedCount += m_item.iConf.buyCount;
                LoseCash(m_item.iConf.price);
                return BuyStatus.Succeed;
            }
            return BuyStatus.NotEnoughCash;
        }

        public BuyStatus BuyAvatar(AvatarType aType)
        {
            AvatarConfig aConf = GetAvatarByType(aType).aConf;
            if (aConf.isCrystalBuy)
            {
                if (crystal >= aConf.price)
                {
                    avatarList[(int)aType].existState = AvatarState.Avaliable;
                    LoseCrystal(aConf.price);
                    Achievement.GotNewAvatar();
                    return BuyStatus.Succeed;
                }
                return BuyStatus.NotEnoughCrystal;
            }
            if (cash >= aConf.price)
            {
                avatarList[(int)aType].existState = AvatarState.Avaliable;
                LoseCash(aConf.price);
                Achievement.GotNewAvatar();
                return BuyStatus.Succeed;
            }
            return BuyStatus.NotEnoughCash;
        }

        public UpgradeStatus BuyAvatarHp(AvatarType aType)
        {
            AvatarAttributes avatarByType = GetAvatarByType(aType);
            if (avatarByType.hpBuyCount < UpgradeParas.AvatarHpMaxLevel)
            {
                if (crystal >= avatarByType.hpBuyPrice)
                {
                    LoseCrystal(avatarByType.hpBuyPrice);
                    avatarByType.BuyHp();
                    return UpgradeStatus.Succeed;
                }
                return UpgradeStatus.NotEnoughCash;
            }
            return UpgradeStatus.MaxLevel;
        }

        public bool BuyBullets(Weapon w, SafeInteger bulletsNum, int price)
        {
            if (cash >= price)
            {
                w.AddBullets(bulletsNum);
                LoseCash(price);
                return true;
            }
            return false;
        }

        public UpgradeStatus UpgradeWeapon(Weapon w)
        {
            SafeInteger upgradePrice = w.UpgradePrice;
            if (w.Level < UpgradeParas.WeaponMaxLevel)
            {
                if (cash >= upgradePrice)
                {
                    LoseCash(upgradePrice);
                    w.Upgrade();
                    return UpgradeStatus.Succeed;
                }
                return UpgradeStatus.NotEnoughCash;
            }
            return UpgradeStatus.MaxLevel;
        }

        public Weapon RandomBattleWeapons()
        {
            List<Weapon> list = new List<Weapon>();
            foreach (Weapon weapon in weaponList)
            {
                if (weapon.IsSelectedForBattle && weapon.GetWeaponType() != WeaponType.Saw && weapon.GetWeaponType() != WeaponType.Sword)
                {
                    list.Add(weapon);
                }
            }
            int count = list.Count;
            int index = UnityEngine.Random.Range(0, count);
            if (count != 0)
            {
                return list[index];
            }
            return weaponList[0];
        }
    }
}
