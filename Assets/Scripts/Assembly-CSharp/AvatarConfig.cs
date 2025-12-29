using Zombie3D;

public class AvatarConfig
{
	public AvatarType name;

	public float damageInitial;

	public float damageFinal;

	public float hpInitial;

	public float hpFinal;

	public float speedInitial;

	public float speedFinal;

	public SafeFloat hpBuyWeight;

	public SafeFloat hpBuyPriceWeight;

	public bool isCrystalBuy;

	public SafeInteger unlockDay;

	public SafeInteger price;

	public bool isPixel;

	public static float vsHpFactor = 1f;
}
