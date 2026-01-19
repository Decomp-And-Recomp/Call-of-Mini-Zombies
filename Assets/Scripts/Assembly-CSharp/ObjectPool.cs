using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
	public string Name;

	protected List<GameObject> objects;

	protected List<float> createdTime;

	protected float life;

	protected bool hasAnimation;

	protected bool hasParticleSystem;

	protected GameObject folderObject;

	public void Init(string poolName, GameObject prefab, int initNum, float life)
	{
		Name = poolName;
		objects = new List<GameObject>();
		createdTime = new List<float>();
		this.life = life;
		folderObject = new GameObject(poolName);
		for (int i = 0; i < initNum; i++)
		{
			GameObject gameObject = Object.Instantiate(prefab) as GameObject;
			objects.Add(gameObject);
			createdTime.Add(0f);
			gameObject.transform.parent = folderObject.transform;
			if (gameObject.GetComponent<Animation>() != null)
			{
				hasAnimation = true;
			}
			if (gameObject.GetComponent<ParticleSystem>() != null)
			{
				hasParticleSystem = true;
			}
			gameObject.SetActive(false);
		}
	}

	public GameObject CreateObject(Vector3 position, Quaternion rotation)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (!objects[i].activeInHierarchy)
			{
				objects[i].SetActive(true);
				objects[i].transform.position = position;
				objects[i].transform.rotation = rotation;
				createdTime[i] = Time.time;
				return objects[i];
			}
		}
		GameObject gameObject = Object.Instantiate(objects[0]) as GameObject;
		objects.Add(gameObject);
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		createdTime.Add(Time.time);
		gameObject.name = objects[0].name;
		gameObject.transform.parent = folderObject.transform;
		if (gameObject.GetComponent<Animation>() != null)
		{
			hasAnimation = true;
		}
		if (gameObject.GetComponent<ParticleSystem>() != null)
		{
			hasParticleSystem = true;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	public GameObject CreateObject(Vector3 position, Vector3 lookAtRotation)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (!objects[i].activeInHierarchy)
			{
				objects[i].SetActive(true);
				objects[i].transform.position = position;
				objects[i].transform.rotation = Quaternion.LookRotation(lookAtRotation);
				createdTime[i] = Time.time;
				return objects[i];
			}
		}
		GameObject gameObject = Object.Instantiate(objects[0]) as GameObject;
		objects.Add(gameObject);
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.LookRotation(lookAtRotation);
		createdTime.Add(Time.time);
		gameObject.name = objects[0].name;
		gameObject.transform.parent = folderObject.transform;
		if (gameObject.GetComponent<Animation>() != null)
		{
			hasAnimation = true;
		}
		if (gameObject.GetComponent<ParticleSystem>() != null)
		{
			hasParticleSystem = true;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	public void AutoDestruct()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].activeInHierarchy && Time.time - createdTime[i] > life)
			{
				objects[i].SetActive(false);
			}
		}
	}

	public GameObject DeleteObject(GameObject obj)
	{
		obj.SetActive(false);
		return obj;
	}
}
