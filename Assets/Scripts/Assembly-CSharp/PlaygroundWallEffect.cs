using UnityEngine;

public class PlaygroundWallEffect : MonoBehaviour
{
	private float vertical;

	private float rate = 0.2f;

	private bool lowToHight = true;

	private float timeInterval = 8f;

	public Material[] m_materials;

	public void Start()
	{
	}

	private void Update()
	{
		if (timeInterval <= 0f)
		{
			if (base.GetComponent<Renderer>().material != m_materials[0])
			{
				base.GetComponent<Renderer>().material = m_materials[0];
			}
			if (lowToHight)
			{
				if ((double)vertical >= 0.5)
				{
					lowToHight = false;
					timeInterval = 8f;
				}
				else
				{
					vertical += Time.deltaTime * rate;
				}
			}
			else if ((double)vertical <= -0.5)
			{
				lowToHight = true;
				timeInterval = 8f;
			}
			else
			{
				vertical -= Time.deltaTime * rate;
			}
			base.GetComponent<Renderer>().material.SetTextureOffset("_tex2", new Vector2(0f, vertical));
		}
		else
		{
			timeInterval -= Time.deltaTime;
			if (base.GetComponent<Renderer>().material != m_materials[1])
			{
				base.GetComponent<Renderer>().material = m_materials[1];
			}
		}
	}
}
