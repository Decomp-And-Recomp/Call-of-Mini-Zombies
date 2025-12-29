using UnityEngine;

public class PanelParameter : MonoBehaviour
{
	public void ShowPanel()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
	}

	public void HidePanel()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, 1000f, base.gameObject.transform.localPosition.z);
	}
}
