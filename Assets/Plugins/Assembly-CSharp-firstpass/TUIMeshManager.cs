using UnityEngine;

public class TUIMeshManager : MonoBehaviour
{
	private int layer;

	public void Initialize(int layer)
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		this.layer = layer;
		UpdateLayer();
	}

	private void UpdateLayer()
	{
		Renderer[] componentsInChildren = base.transform.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layer;
		}
	}
}
