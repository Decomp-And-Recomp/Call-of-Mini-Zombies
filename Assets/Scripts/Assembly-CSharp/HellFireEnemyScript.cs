using UnityEngine;
using Zombie3D;

public class HellFireEnemyScript : MonoBehaviour
{
	protected GameScene gameScene;

	protected Player player;

	protected Weapon weapon;

	public float damage { get; set; }

	public void Start()
	{
		gameScene = GameApp.GetInstance().GetGameScene();
		if (gameScene != null)
		{
			player = gameScene.GetPlayer();
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (other.layer == 8 && player != null && Time.time - player.lastFireDamagedTime > 0.5f)
		{
			player.OnHit(damage);
			player.lastFireDamagedTime = Time.time;
		}
	}
}
