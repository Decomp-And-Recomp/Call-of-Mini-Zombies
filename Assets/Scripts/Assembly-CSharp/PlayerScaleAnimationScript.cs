using UnityEngine;

public class PlayerScaleAnimationScript : MonoBehaviour
{
	private Vector3 normalizedScale = default(Vector3);

	private Vector3 lastScale;

	private float originalRadius;

	private bool finishSmallToBig;

	public float scaleSpeed { get; set; }

	public bool smallToBig { get; set; }

	public Vector3 targetScale { get; set; }

	private void Start()
	{
		lastScale = base.transform.localScale;
		normalizedScale = base.transform.localScale.normalized;
	}

	private void Update()
	{
		if (smallToBig)
		{
			if (!finishSmallToBig)
			{
				if (base.transform.localScale.magnitude < targetScale.magnitude)
				{
					base.transform.localScale += normalizedScale * Time.deltaTime * scaleSpeed;
					GetComponent<CharacterController>().radius /= base.transform.localScale.x / lastScale.x;
					lastScale = base.transform.localScale;
				}
				else
				{
					finishSmallToBig = true;
				}
				return;
			}
			Vector3 origin = base.transform.position + GetComponent<CharacterController>().center;
			Vector3 vector = base.transform.forward;
			for (int i = 0; i < 12; i++)
			{
				vector = Quaternion.AngleAxis(-30 * i, base.transform.up) * vector;
				RaycastHit hitInfo;
				if (Physics.Raycast(origin, vector, out hitInfo, 10f, 100352))
				{
					break;
				}
				GetComponent<CharacterController>().radius = 0.5f;
				base.enabled = false;
			}
		}
		else if (base.transform.localScale.magnitude > targetScale.magnitude)
		{
			base.transform.localScale -= normalizedScale * Time.deltaTime * scaleSpeed;
			if (GetComponent<CharacterController>().radius == 0.5f)
			{
				return;
			}
			Vector3 origin2 = base.transform.position + GetComponent<CharacterController>().center;
			Vector3 vector2 = base.transform.forward;
			for (int j = 0; j < 12; j++)
			{
				vector2 = Quaternion.AngleAxis(-30 * j, base.transform.up) * vector2;
				RaycastHit hitInfo2;
				if (Physics.Raycast(origin2, vector2, out hitInfo2, 10f, 100352))
				{
					break;
				}
				GetComponent<CharacterController>().radius = 0.5f;
			}
		}
		else
		{
			Object.Destroy(GetComponent<PlayerScaleAnimationScript>());
		}
	}
}
