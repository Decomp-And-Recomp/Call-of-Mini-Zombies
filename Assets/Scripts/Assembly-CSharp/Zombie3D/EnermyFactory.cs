using UnityEngine;

namespace Zombie3D
{
	public static class EnermyFactory
	{
		public static Enemy CreateEnermyAvatar(EnemyType type)
		{
			Enemy result = null;
			switch (type)
			{
			case EnemyType.E_ZOMBIE:
				result = new Zombie();
				break;
			case EnemyType.E_NURSE:
				result = new Nurse();
				break;
			case EnemyType.E_TANK:
				result = new Tank();
				break;
			case EnemyType.E_BOOMER:
				result = new Boomer();
				break;
			case EnemyType.E_SWAT:
				result = new Swat();
				break;
			case EnemyType.E_DOG:
				result = new Dog();
				break;
			case EnemyType.E_POLICE:
				result = new Police();
				break;
			case EnemyType.E_DILO:
				result = new DiloDinosaur();
				break;
			case EnemyType.E_VELOCI:
				result = new VelociDinosaur();
				break;
			case EnemyType.E_HELL_FIRER:
				result = new HellFirer();
				break;
			case EnemyType.E_SUPER_DINO:
				result = new SuperDinosaur();
				break;
			}
			return result;
		}

		public static Enemy SpawnEnemy(int id, bool isElite, bool isPrey, bool isBoss, EnemyType enermyType, SpawnFromType fromWhere, Vector3 position)
		{
			if (enermyType == EnemyType.E_NONE || enermyType == EnemyType.E_COUNT)
			{
				return null;
			}
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			bool flag = false;
			if (enermyType > EnemyType.E_COUNT)
			{
				string text = "Prefabs/SuperEnemy/" + enermyType;
				if (enermyType == EnemyType.E_HELL_FIRER)
				{
					text += ((!isElite) ? "1" : "2");
				}
				gameObject = Object.Instantiate(Resources.Load(text)) as GameObject;
				gameObject2 = Object.Instantiate(gameObject.GetComponent<AvataConfigScript>().Avata_Instance, position, Quaternion.identity) as GameObject;
				gameObject2.layer = 9;
				flag = true;
				Object.Destroy(gameObject);
			}
			else if (isElite)
			{
				gameObject = GameApp.GetInstance().GetEnemyResourceConfig().enemy_elite[(int)enermyType];
				gameObject2 = Object.Instantiate(gameObject, position, Quaternion.identity) as GameObject;
				gameObject2.layer = 9;
				flag = true;
			}
			else if (isPrey || isBoss)
			{
				gameObject = GameApp.GetInstance().GetEnemyResourceConfig().enemy[(int)enermyType];
				gameObject2 = Object.Instantiate(gameObject, position, Quaternion.identity) as GameObject;
				gameObject2.layer = 9;
				flag = true;
			}
			else
			{
				gameObject2 = GameApp.GetInstance().GetGameScene().GetEnemyPool(enermyType)
					.CreateObject(position, Quaternion.identity);
				gameObject2.layer = 9;
				flag = false;
			}
			if (isBoss && enermyType != EnemyType.E_SUPER_DINO)
			{
				BoxCollider component = gameObject2.GetComponent<BoxCollider>();
				CharacterController component2 = gameObject2.GetComponent<CharacterController>();
				if (component != null && component.size.y < 2.8f)
				{
					float num = 2.8f / component.size.y;
					gameObject2.transform.localScale *= num;
					component.size = new Vector3(component.size.x, component.size.y / num, component.size.z);
					component.center = new Vector3(component.center.x, component.size.y / 2f, component.center.z);
				}
				else if (component2 != null && component2.height < 2.8f)
				{
					float num2 = 2.8f / component2.height;
					gameObject2.transform.localScale *= num2;
					component2.height /= num2;
					component2.center = new Vector3(component2.center.x, component2.height / 2f, component2.center.z);
				}
			}
			gameObject2.name = "E_" + id;
			Enemy enemy = CreateEnermyAvatar(enermyType);
			enemy.IsElite = isElite;
			enemy.IsPrey = isPrey;
			enemy.IsSuperBoss = isBoss;
			enemy.EnemyType = enermyType;
			enemy.Name = gameObject2.name;
			enemy.Init(gameObject2);
			enemy.DestroyAfterDead = flag;
			if (fromWhere == SpawnFromType.Grave)
			{
				enemy.SetInGrave(true);
			}
			GameApp.GetInstance().GetGameScene().GetEnemies()
				.Add(gameObject2.name, enemy);
			return enemy;
		}
	}
}
