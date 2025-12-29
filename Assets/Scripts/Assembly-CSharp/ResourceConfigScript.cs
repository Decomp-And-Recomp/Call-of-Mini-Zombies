using UnityEngine;

[AddComponentMenu("TPS/PrefabObjectManager")]
public class ResourceConfigScript : MonoBehaviour
{
	public static ResourceConfigScript instance;

	public GameObject hitBlood;

	public GameObject deadBlood;

	public GameObject deadFoorblood;

	public GameObject deadFoorblood2;

	public GameObject hitparticles;

	public GameObject projectile;

	public GameObject isnipertile;

	public GameObject m32tile;

	public GameObject rocketExlposion;

	public GameObject boomerExplosion;

	public GameObject shotgunfire;

	public GameObject rpgFloor;

	public GameObject rpgFloor_Playground;

	public GameObject laser;

	public GameObject laserHit;

	public GameObject fireline;

	public GameObject bullets;

	public GameObject shotgunBullet;

	public GameObject nurseSaliva;

	public GameObject nurseSalivaProjectile;

	public GameObject salivaExplosion;

	public GameObject diloVenom;

	public GameObject diloVenomProjectile;

	public GameObject diloVenomExplosion;

	public GameObject diloVenomHitFloor;

	public GameObject superDinoRushEffect;

	public GameObject woodExplode;

	public GameObject copBomb;

	public GameObject graveRock;

	public GameObject powerEffect;

	public GameObject superHens;

	public GameObject swordAttack;

	public GameObject pixelDead;

	public GameObject pixelExplosion;

	public GameObject electricGunBullet;

	public GameObject electricGunExplosion;

	public GameObject airGunBullet;

	public GameObject airGunExplosion;

	public GameObject pixelAirGunBullet;

	public GameObject pixelAirGunExplosion;

	public GameObject longinusGoldBullet;

	public GameObject longinusGoldExplosion;

	public GameObject longinusSilverBullet;

	public GameObject longinusSilverExplosion;

	public GameObject pixelRpgBullet;

	public GameObject pixelRpgExplosion;

	public GameObject crossbowBullet;

	public GameObject crossbowHit;

	public GameObject crossbowExplosion;

	public GameObject crossbowRedpoint;

	public GameObject suicideGunSpark;

	public GameObject suicideGunBullet;

	public GameObject suicideGunSmallBullet;

	public GameObject suicideGunExplosion;

	public GameObject itemHP;

	public GameObject itemPower;

	public GameObject itemGold;

	public GameObject itemGold_Big;

	public GameObject itemStealth;

	public GameObject itemSuper;

	public GameObject itemShield;

	public GameObject itemAssaultGun;

	public GameObject itemShotGun;

	public GameObject itemRocketLauncer;

	public GameObject itemGatlin;

	public GameObject itemLaser;

	public GameObject itemMissle;

	public GameObject itemM32;

	public GameObject itemFire;

	public GameObject itemElectricGun;

	public GameObject itemCrossbow;

	public GameObject itemSuicideGun;

	public GameObject itemSmallHpEffect;

	public GameObject itemBigHpEffect;

	public GameObject itemMedpackEffect;

	public GameObject itemFullReviveEffect;

	public GameObject AvatarLevelUpEffect;

	public GameObject shieldLogo;

	public GameObject crown;

	public GameObject PreyTip;

	public GameObject mucilage_M;

	public GameObject foot_print;

	public GameObject snow_explosion_eff;

	public Shader modelEdge_alpha;

	public GameObject GetExplosionFloor(string sceneName)
	{
		return (!(sceneName == "Zombie3D_PlayGound")) ? rpgFloor : rpgFloor_Playground;
	}
	private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
