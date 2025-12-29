using UnityEngine;

public class CameraFadeScript : MonoBehaviour
{
	private GUIStyle m_BackgroundStyle = new GUIStyle();

	private Texture2D m_FadeTexture;

	private Color m_CurrentScreenOverlayColor = new Color(0f, 0f, 0f, 0f);

	private Color m_TargetScreenOverlayColor = new Color(0f, 0f, 0f, 0f);

	private Color m_DeltaColor = new Color(0f, 0f, 0f, 0f);

	private int m_FadeGUIDepth = -1000;

	public Color startColor = Color.black;

	public Color endColor = new Color(0f, 0f, 0f, 0f);

	public float duration = 3f;

	private void Awake()
	{
		m_FadeTexture = new Texture2D(1, 1);
		m_BackgroundStyle.normal.background = m_FadeTexture;
		SetScreenOverlayColor(m_CurrentScreenOverlayColor);
	}

	private void OnGUI()
	{
		if (m_CurrentScreenOverlayColor != m_TargetScreenOverlayColor)
		{
			if (Mathf.Abs(m_CurrentScreenOverlayColor.a - m_TargetScreenOverlayColor.a) < Mathf.Abs(m_DeltaColor.a) * Time.deltaTime)
			{
				m_CurrentScreenOverlayColor = m_TargetScreenOverlayColor;
				SetScreenOverlayColor(m_CurrentScreenOverlayColor);
				m_DeltaColor = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				SetScreenOverlayColor(m_CurrentScreenOverlayColor + m_DeltaColor * Time.deltaTime);
			}
		}
		if (m_CurrentScreenOverlayColor.a > 0f)
		{
			GUI.depth = m_FadeGUIDepth;
			GUI.Label(new Rect(-10f, -10f, Screen.width + 10, Screen.height + 10), m_FadeTexture, m_BackgroundStyle);
		}
	}

	public float GetAlpha()
	{
		return m_CurrentScreenOverlayColor.a;
	}

	public void StartFade(Color start, Color end, float duration)
	{
		SetScreenOverlayColor(start);
		StartFade(end, duration);
	}

	public void FadeOutBlack()
	{
		StartFade(Color.black, new Color(0f, 0f, 0f, 0f), 1f);
	}

	public void FadeInBlack()
	{
		StartFade(new Color(0f, 0f, 0f, 0f), Color.black, 1f);
	}

	private void Start()
	{
		SetScreenOverlayColor(startColor);
		StartFade(endColor, duration);
	}

	public void SetScreenOverlayColor(Color newScreenOverlayColor)
	{
		m_CurrentScreenOverlayColor = newScreenOverlayColor;
		m_FadeTexture.SetPixel(0, 0, m_CurrentScreenOverlayColor);
		m_FadeTexture.Apply();
	}

	public static CameraFadeScript GetInstance()
	{
		return GameObject.Find("CameraFade").GetComponent<CameraFadeScript>();
	}

	public void StartFade(Color newScreenOverlayColor, float fadeDuration)
	{
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		m_TargetScreenOverlayColor = newScreenOverlayColor;
		m_DeltaColor = (m_TargetScreenOverlayColor - m_CurrentScreenOverlayColor) / fadeDuration;
	}
}
