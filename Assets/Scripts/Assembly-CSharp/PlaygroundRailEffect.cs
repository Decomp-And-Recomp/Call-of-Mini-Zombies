using UnityEngine;

public class PlaygroundRailEffect : MonoBehaviour
{
	private float alpha = 1f;

	private bool zeroToOne = true;

	private void Update()
	{
		if (zeroToOne)
		{
			if (alpha >= 1f)
			{
				zeroToOne = false;
			}
			else
			{
				alpha += Time.deltaTime * 0.2f;
			}
		}
		else if ((double)alpha <= 0.2)
		{
			zeroToOne = true;
		}
		else
		{
			alpha -= Time.deltaTime * 0.4f;
		}
		base.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, alpha));
	}
}
