using System.Collections.Generic;
using UnityEngine;

public class VsSeatState : MonoBehaviour
{
	public List<TUIMeshSprite> vs_seat_list = new List<TUIMeshSprite>();

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void RefrashSeatList(int user_count)
	{
		for (int i = 0; i < vs_seat_list.Count; i++)
		{
			if (i < user_count)
			{
				vs_seat_list[i].frameName_Accessor = "vs_full";
			}
			else
			{
				vs_seat_list[i].frameName_Accessor = "vs_empty";
			}
		}
	}
}
