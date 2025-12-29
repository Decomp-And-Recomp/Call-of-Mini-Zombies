using UnityEngine;
using Zombie3D;

public class ControllerHitScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		string text = hit.collider.name;
		if (!text.StartsWith("E_"))
		{
			return;
		}
		Enemy enemyByID = GameApp.GetInstance().GetGameScene().GetEnemyByID(text);
		if (enemyByID.EnemyType == EnemyType.E_ZOMBIE || enemyByID.EnemyType == EnemyType.E_NURSE)
		{
			Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
			if (!(attachedRigidbody == null) && !attachedRigidbody.isKinematic)
			{
				float num = 2f;
				Vector3 vector = new Vector3(hit.moveDirection.z, hit.moveDirection.y, hit.moveDirection.x);
				attachedRigidbody.velocity = vector * num;
			}
		}
	}
}
