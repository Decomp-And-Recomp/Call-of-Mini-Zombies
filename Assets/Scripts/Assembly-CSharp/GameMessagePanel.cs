using UnityEngine;

public class GameMessagePanel : MonoBehaviour
{
	public GameObject[] message_cells;

	private void Start()
	{
		for (int i = 0; i < message_cells.Length; i++)
		{
			message_cells[i].SetActive(false);
		}
	}

	public void AddSFSRoom(string msg)
	{
		for (int num = message_cells.Length - 1; num > 0; num--)
		{
			message_cells[num].SetActive(true);
			message_cells[num].GetComponent<TUIMeshText>().text_Accessor = message_cells[num - 1].GetComponent<TUIMeshText>().text_Accessor;
			message_cells[num].GetComponent<VsMsgRemove>().timer = message_cells[num - 1].GetComponent<VsMsgRemove>().timer;
		}
		message_cells[0].SetActive(true);
		message_cells[0].GetComponent<TUIMeshText>().text_Accessor = msg;
		message_cells[0].GetComponent<VsMsgRemove>().timer = 0f;
	}
}
