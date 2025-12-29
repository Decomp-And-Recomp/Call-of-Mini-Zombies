using UnityEngine;

public class DsutStormScript : MonoBehaviour
{
	public GameObject Game_Camera;

	protected Transform m_tansform;

	private void Start()
	{
		m_tansform = base.gameObject.GetComponent<Transform>();
	}

	private void Update()
	{
		if (Game_Camera != null)
		{
			m_tansform.position = Vector3.Lerp(m_tansform.position, Game_Camera.transform.position, Time.deltaTime);
		}
	}
}
