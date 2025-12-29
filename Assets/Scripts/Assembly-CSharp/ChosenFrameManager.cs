using UnityEngine;

public class ChosenFrameManager : MonoBehaviour
{
	public GameObject ChosenObject { get; set; }

	private void Start()
	{
	}

	private void Update()
	{
		if (ChosenObject != null && (ChosenObject.transform.position.x != base.transform.position.x || ChosenObject.transform.position.y != base.transform.position.y))
		{
			GetComponent<TUIMeshSprite>().Static = false;
			base.transform.position = new Vector3(ChosenObject.transform.position.x, ChosenObject.transform.position.y, ChosenObject.transform.position.z - 0.5f);
		}
	}
}
