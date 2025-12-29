using System;
using UnityEngine;
using Zombie3D;

public class AlphaAnimationScriptSecond : MonoBehaviour
{
	private float maxAlpha = 1f;

	private float minAlpha;

	private float animationSpeed = 0.5f;

	private float alpha;

	private float deltaTime;

	private string colorPropertyName;

	public bool lowToHigh { get; set; }

	public float targetAlpha { get; set; }

	public bool isResetShader { get; set; }

	public Shader resetShader { get; set; }

	public Color resetColor { get; set; }

	private void Start()
	{
		deltaTime = 0f;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.02f)
		{
			return;
		}
		try
		{
			colorPropertyName = ((!(base.GetComponent<Renderer>().material.shader == GameApp.GetInstance().GetGameResourceConfig().modelEdge_alpha)) ? "_TintColor" : "_Color");
			alpha = base.GetComponent<Renderer>().material.GetColor(colorPropertyName).a;
		}
		catch (Exception)
		{
			base.enabled = false;
			return;
		}
		if (lowToHigh)
		{
			alpha += animationSpeed * deltaTime;
			alpha = Mathf.Clamp(alpha, minAlpha, targetAlpha);
			base.GetComponent<Renderer>().material.SetColor(colorPropertyName, new Color(1f, 1f, 1f, alpha));
			if (alpha == targetAlpha)
			{
				base.enabled = false;
				if (isResetShader)
				{
					base.gameObject.GetComponent<Renderer>().material.shader = resetShader;
					base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", resetColor);
					if (base.gameObject.GetComponent<Renderer>().materials.Length > 1)
					{
						base.gameObject.GetComponent<Renderer>().materials[1].shader = resetShader;
						base.gameObject.GetComponent<Renderer>().materials[1].SetColor("_TintColor", resetColor);
					}
				}
			}
		}
		else
		{
			alpha -= animationSpeed * deltaTime;
			alpha = Mathf.Clamp(alpha, targetAlpha, maxAlpha);
			base.GetComponent<Renderer>().material.SetColor(colorPropertyName, new Color(1f, 1f, 1f, alpha));
			if (alpha == targetAlpha)
			{
				base.enabled = false;
				if (isResetShader)
				{
					base.gameObject.GetComponent<Renderer>().material.shader = resetShader;
					base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", resetColor);
				}
			}
		}
		deltaTime = 0f;
	}
}
