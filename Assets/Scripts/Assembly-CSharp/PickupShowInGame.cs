using UnityEngine;
using Zombie3D;

public class PickupShowInGame : MonoBehaviour
{
	private float delta_time;

	private float timer;

	private Vector3 direction;

	private bool enlarge;

	private bool shrink;

	public ItemType itemType { get; set; }

	public Vector3 targetPosition { get; set; }

	private void Start()
	{
		delta_time = 0f;
		timer = 1f;
		enlarge = true;
		shrink = false;
		direction = (targetPosition - base.transform.localPosition).normalized;
		direction = new Vector3(direction.x, direction.y, 0f);
		GetComponent<TUIMeshSprite>().frameName_Accessor = "item_" + itemType;
	}

	private void Update()
	{
		delta_time += Time.deltaTime;
		if ((double)delta_time < 0.02)
		{
			return;
		}
		if (timer < 0f)
		{
			if (base.transform.localPosition.x < targetPosition.x)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				base.transform.localPosition += direction * delta_time * 800f;
				base.transform.localScale -= new Vector3(1f, 1f, 1f) * delta_time * 1.5f;
			}
		}
		else
		{
			if (enlarge)
			{
				base.transform.localScale += new Vector3(1f, 1f, 1f) * delta_time * 2f;
				if (base.transform.localScale.x >= 2f)
				{
					enlarge = false;
					shrink = true;
				}
			}
			else if (shrink)
			{
				base.transform.localScale -= new Vector3(1f, 1f, 1f) * delta_time * 2f;
				if ((double)base.transform.localScale.x <= 0.5)
				{
					shrink = false;
				}
			}
			timer -= delta_time;
		}
		delta_time = 0f;
	}
}
