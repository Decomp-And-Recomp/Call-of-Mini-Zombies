using UnityEngine;

public class GetMoneyPanel : MonoBehaviour
{
	public TUIMeshText m_content;

	public void SetContent(string content)
	{
		m_content.text_Accessor = content;
	}

	public void Show()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
	}

	public void Hide()
	{
		base.transform.localPosition = new Vector3(0f, -1000f, base.transform.localPosition.z);
	}
}
