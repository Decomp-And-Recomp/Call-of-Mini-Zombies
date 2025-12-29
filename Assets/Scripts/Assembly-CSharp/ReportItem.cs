using UnityEngine;

public class ReportItem : MonoBehaviour
{
	public TUIMeshText Label_Content_Name;

	public TUIMeshText Label_Content_KillCount;

	public TUIMeshText Label_Content_Death;

	public TUIMeshText Label_Content_Money;

	public TUIMeshText Label_Content_Combo;

	public GameObject Win_Tip;

	public GameObject Win_Bk;

	public void SetContent(string name, int reward, bool winner)
	{
		Label_Content_Name.text_Accessor = name;
		Label_Content_Money.text_Accessor = "$" + reward;
		if (winner)
		{
			Win_Tip.SetActive(true);
			Win_Bk.SetActive(true);
			Label_Content_Name.color_Accessor = new Color(0.4f, 0.28f, 0.12f, 1f);
			Label_Content_Money.color_Accessor = new Color(0.4f, 0.28f, 0.12f, 1f);
		}
		else
		{
			Win_Tip.SetActive(false);
			Win_Bk.SetActive(false);
		}
	}

	public void SetContent(string name, int kill_count, int death_count, int cash_loot, int combo_count, bool winner)
	{
		Label_Content_Name.text_Accessor = name;
		Label_Content_KillCount.text_Accessor = kill_count.ToString();
		Label_Content_Death.text_Accessor = death_count.ToString();
		Label_Content_Money.text_Accessor = "$" + cash_loot;
		Label_Content_Combo.text_Accessor = combo_count.ToString();
		if (winner)
		{
			Win_Tip.SetActive(true);
			Win_Bk.SetActive(true);
			Label_Content_Name.color_Accessor = new Color(0.4f, 0.28f, 0.12f, 1f);
			Label_Content_KillCount.color_Accessor = new Color(0.4f, 0.28f, 0.12f, 1f);
			Label_Content_Death.color_Accessor = new Color(0.4f, 0.28f, 0.12f, 1f);
			Label_Content_Money.color_Accessor = new Color(0.4f, 0.28f, 0.12f, 1f);
			Label_Content_Combo.color_Accessor = new Color(0.4f, 0.28f, 0.12f, 1f);
		}
		else
		{
			Win_Tip.SetActive(false);
			Win_Bk.SetActive(false);
		}
	}
}
