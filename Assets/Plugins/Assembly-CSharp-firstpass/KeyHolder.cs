using System.Collections.Generic;
using UnityEngine;

public class KeyHolder
{
	public static string GetKey()
	{
		return PrivateData.TNetEncryptionKey;
		//int[] keyMap = GetKeyMap();
		//string empty = string.Empty;
		//return ((char)keyMap[116]).ToString() + (char)keyMap[114] + (char)keyMap[105] + (char)keyMap[110] + (char)keyMap[105] + (char)keyMap[116] + (char)keyMap[105] + (char)keyMap[45] + (char)keyMap[34] + (char)keyMap[108] + (char)keyMap[49] + (char)keyMap[107];
	}

	public static void Create(string key)
	{
		int[] keyMap = GetKeyMap();
		string text = string.Empty;
		for (int i = 0; i < key.Length; i++)
		{
			char c = key[i];
			for (int j = 0; j < keyMap.Length; j++)
			{
				if (c == (ushort)keyMap[j])
				{
					text = text + "((char)(key_map[" + j + "])).ToString()";
					break;
				}
			}
			if (i != key.Length - 1)
			{
				text += " + ";
			}
		}
		Debug.Log("ret = " + text);
	}

	private static int[] GetKeyMap()
	{
		List<int> list = new List<int>();
		for (int i = 50; i < 100; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < 50; j++)
		{
			list.Add(j);
		}
		for (int k = 100; k < 128; k++)
		{
			list.Add(k);
		}
		return list.ToArray();
	}
}
