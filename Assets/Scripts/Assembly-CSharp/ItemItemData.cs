using UnityEngine;
using Zombie3D;

public class ItemItemData : MonoBehaviour
{
	protected Item item;

	public void SetItem(Item i)
	{
		if (i != null)
		{
			item = i;
		}
	}

	public Item GetItem()
	{
		return item;
	}
}
