using UnityEngine;
using Zombie3D;

public class ArenaTriggerManager : MonoBehaviour
{
	private void Awake()
	{
		switch (GameApp.GetInstance().GetGameState().gameMode)
		{
		case GameMode.Solo:
		case GameMode.SoloBoss:
		case GameMode.Hunting:
		case GameMode.DinoHunting:
			base.gameObject.AddComponent<ArenaTriggerFromConfigScript>();
			break;
		case GameMode.Instance:
			base.gameObject.AddComponent<ArenaTriggerInstanceScript>();
			break;
		case GameMode.Coop:
			base.gameObject.AddComponent<ArenaTriggerBossScript>();
			break;
		case GameMode.Vs:
		case GameMode.Tutorial:
			break;
		}
	}
}
