using UnityEngine;

public class ComboTextScript : MonoBehaviour
{
	private string[] contents = new string[8] { "First Blood", "Double Kill", "Triple Kill", "Quadra Kill", "Mega Kill", "Ultra Kill", "Monster Kill", "Over kill" };

	public float timer = 3f;

	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer < 0f)
		{
			Hide();
		}
	}

	public void Hide()
	{
		base.enabled = false;
		base.GetComponent<Renderer>().enabled = false;
	}

	public void Show(string m_name, int count)
	{
		base.enabled = true;
		base.GetComponent<Renderer>().enabled = true;
		timer = 3f;
		string text = ((count <= contents.Length) ? contents[count - 1] : contents[contents.Length - 1]);
		string text2 = m_name + "  " + text;
		GetComponent<TUIMeshTextMx>().ChangeColor(text2.Length - text.Length, text2.Length, Color.red);
		GetComponent<TUIMeshTextMx>().text_Accessor = text2;
	}
}
