using UnityEngine;

namespace Zombie3D
{
	public class AvatarFactory
	{
		protected static AvatarFactory instance;

		public static AvatarFactory GetInstance()
		{
			if (instance == null)
			{
				instance = new AvatarFactory();
			}
			return instance;
		}

		public GameObject CreateAvatar(AvatarType aType)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			string path = "Prefabs/Avatar/" + aType;
			gameObject = Object.Instantiate(Resources.Load(path)) as GameObject;
			AvataConfigScript component = gameObject.GetComponent<AvataConfigScript>();
			gameObject2 = Object.Instantiate(component.Avata_Instance, Vector3.zero, Quaternion.identity) as GameObject;
			Object.Destroy(gameObject);
			return gameObject2;
		}
	}
}
