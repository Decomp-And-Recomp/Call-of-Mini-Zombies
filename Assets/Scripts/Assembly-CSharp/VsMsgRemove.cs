using UnityEngine;

public class VsMsgRemove : MonoBehaviour
{
	public float timer { get; set; }

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > 5f)
		{
			base.gameObject.GetComponent<TUIMeshText>().text_Accessor = string.Empty;
			base.gameObject.SetActive(false);
		}
	}
}
