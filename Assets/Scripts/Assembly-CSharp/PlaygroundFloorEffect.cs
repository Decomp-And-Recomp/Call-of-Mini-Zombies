using UnityEngine;

public class PlaygroundFloorEffect : MonoBehaviour
{
	public Material[] m_mtlGridLighted;

	private float horizontal;

	private float vertical;

	private float rate = 0.3f;

	private float timeElapsed;

	private bool updateTime = true;

	private bool isHorizontal = true;

	private void Update()
	{
		if (updateTime)
		{
			if (timeElapsed > 3f)
			{
				updateTime = false;
				if (isHorizontal)
				{
					isHorizontal = false;
					horizontal = 0.5f;
				}
				else
				{
					isHorizontal = true;
					vertical = -0.5f;
				}
			}
			else
			{
				timeElapsed += Time.deltaTime;
			}
		}
		else if (isHorizontal)
		{
			UpdateHorizontal();
		}
		else
		{
			UpdateVertical();
		}
	}

	private void UpdateHorizontal()
	{
		horizontal -= Time.deltaTime * rate;
		if (horizontal <= -1f)
		{
			timeElapsed = 0f;
			updateTime = true;
		}
		base.GetComponent<Renderer>().material = m_mtlGridLighted[0];
		base.GetComponent<Renderer>().material.SetTextureOffset("_tex2", new Vector2(horizontal, 0f));
	}

	private void UpdateVertical()
	{
		vertical += Time.deltaTime * rate;
		if (vertical >= 1f)
		{
			timeElapsed = 0f;
			updateTime = true;
		}
		base.GetComponent<Renderer>().material = m_mtlGridLighted[1];
		base.GetComponent<Renderer>().material.SetTextureOffset("_tex2", new Vector2(0f, vertical));
	}
}
