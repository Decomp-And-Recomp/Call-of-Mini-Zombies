using UnityEngine;

public class FatZombieControllerHitScript : MonoBehaviour
{
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.collider.gameObject.layer == 13)
		{
			Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
			if (!(attachedRigidbody == null))
			{
				float num = 20f;
				Vector3 vector = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
				attachedRigidbody.linearVelocity = vector * num;
			}
		}
	}
}
