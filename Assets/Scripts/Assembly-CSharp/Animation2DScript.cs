using UnityEngine;

public class Animation2DScript : MonoBehaviour
{
	public float frameRate = 0.02f;

	protected int currentIndex;

	public Texture2D[] textures;

	public string texturePropertyName = "_MainTex";

	protected float deltaTime;

	private void Start()
	{
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > frameRate)
		{
			deltaTime = 0f;
			currentIndex++;
			if (currentIndex >= textures.Length)
			{
				currentIndex = 0;
			}
			base.GetComponent<Renderer>().material.SetTexture(texturePropertyName, textures[currentIndex]);
		}
	}
}
