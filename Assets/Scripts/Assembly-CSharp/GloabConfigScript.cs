using UnityEngine;

public class GloabConfigScript : MonoBehaviour
{
	public static GloabConfigScript instance;

	public TextAsset configXml;

	public TextAsset shareHtm;

	private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
