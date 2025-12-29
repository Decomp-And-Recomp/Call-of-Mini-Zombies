using UnityEngine;
using Zombie3D;

public class DelayShootBullet : MonoBehaviour
{
	private ParticleSystem spark;

	private float duration;

	public float startTime { get; set; }

	public Weapon weapon { get; set; }

	private void Awake()
	{
		base.enabled = false;
	}

	private void Start()
	{
		spark = base.gameObject.GetComponent<ParticleSystem>();
		if (weapon.Name == "Ion-Cannon")
		{
			duration = 1.58f;
		}
		else if (weapon.Name == "Dragon-Breath" || weapon.Name == "Pixel-Cannon")
		{
			duration = 0.6f;
		}
	}

	private void Update()
	{
		if (spark.isPlaying && Time.time - startTime >= duration)
		{
			weapon.ReleaseBullet();
			base.enabled = false;
		}
	}
}
