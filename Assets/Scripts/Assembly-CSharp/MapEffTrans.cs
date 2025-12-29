using UnityEngine;

public class MapEffTrans : MonoBehaviour
{
	public float speed = 1f;

	public MapEffTransType m_type;

	public TUIMeshSprite m_eff;

	public MapEffTransIn m_MapEffTransIn;

	public MapEffTransOut m_MapEffTransOut;

	private void Start()
	{
	}

	private void Update()
	{
		if (m_type == MapEffTransType.In)
		{
			base.transform.Translate(Vector3.right * speed * Time.deltaTime);
			if (base.transform.localPosition.x >= 0f)
			{
				base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
				m_type = MapEffTransType.None;
				m_eff.gameObject.SetActive(false);
				if (m_MapEffTransIn != null)
				{
					m_MapEffTransIn();
				}
			}
		}
		else
		{
			if (m_type != MapEffTransType.Out)
			{
				return;
			}
			base.transform.Translate(Vector3.right * speed * Time.deltaTime * -1f);
			if (base.transform.localPosition.x <= -480f)
			{
				base.transform.localPosition = new Vector3(-480f, 0f, base.transform.localPosition.z);
				m_type = MapEffTransType.None;
				m_eff.gameObject.SetActive(false);
				if (m_MapEffTransOut != null)
				{
					m_MapEffTransOut();
				}
			}
		}
	}

	public void MapEffIn()
	{
		base.transform.localPosition = new Vector3(-480f, 0f, base.transform.localPosition.z);
		m_type = MapEffTransType.In;
		m_eff.gameObject.SetActive(true);
	}

	public void MapEffOut()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		m_type = MapEffTransType.Out;
		m_eff.gameObject.SetActive(true);
	}
}
