using UnityEngine;

public class MenuConfigScript : MonoBehaviour
{
	public static MenuConfigScript instance;
	public AudioClip menuAudio;

	private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
