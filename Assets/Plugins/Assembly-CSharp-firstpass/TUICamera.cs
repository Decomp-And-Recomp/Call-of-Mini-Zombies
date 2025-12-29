using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TUICamera : MonoBehaviour
{
	public Rect m_viewRect;

	public void Initialize(bool landscape, int layer, int depth)
	{
		float height;
		bool hd;
		float width;
		GetScreenInfo(out width, out height, out hd);
		if (landscape)
		{
			float num = width;
			width = height;
			height = num;
		}
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		base.GetComponent<Camera>().transform.localPosition = new Vector3(1f / ((!hd) ? 2f : 4f), -1f / ((!hd) ? 2f : 4f), 0f);
		base.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
		base.GetComponent<Camera>().backgroundColor = Color.white;
		base.GetComponent<Camera>().nearClipPlane = -128f;
		base.GetComponent<Camera>().farClipPlane = 128f;
		base.GetComponent<Camera>().orthographic = true;
		base.GetComponent<Camera>().depth = depth;
		base.GetComponent<Camera>().cullingMask = 1 << layer;
		m_viewRect = new Rect(0f, 0f, Screen.width, Screen.height);
		base.GetComponent<Camera>().pixelRect = m_viewRect;

        TUIVirtualScreen.use = false;

        if (Application.loadedLevelName.StartsWith("Zombie3D_"))
        {
			if (Screen.width > 1280 || Screen.height > 800)
            {
                TUIVirtualScreen.Calc();
                TUIVirtualScreen.use = true;

                m_viewRect.width = TUIVirtualScreen.res.x;
                m_viewRect.height = TUIVirtualScreen.res.y;
            }


            base.GetComponent<Camera>().aspect = m_viewRect.width / m_viewRect.height;
			if (hd)
			{
				base.GetComponent<Camera>().orthographicSize = m_viewRect.height / 4f;
			}
			else
			{
				base.GetComponent<Camera>().orthographicSize = height / 2f;
			}
		}
		else if (Screen.width >= 960 && Screen.height >= 640)
		{
			float left = ((float)Screen.width - 960f) / 2f;
			float top = ((float)Screen.height - 640f) / 2f;
			m_viewRect = new Rect(left, top, 960f, 640f);
			base.GetComponent<Camera>().aspect = m_viewRect.width / m_viewRect.height;
			base.GetComponent<Camera>().orthographicSize = m_viewRect.height / ((!hd) ? 2f : 4f);
		}
		else if (Screen.width >= 640 && Screen.height >= 960)
		{
			float left2 = ((float)Screen.width - 640f) / 2f;
			float top2 = ((float)Screen.height - 960f) / 2f;
			m_viewRect = new Rect(left2, top2, 640f, 960f);
			base.GetComponent<Camera>().aspect = m_viewRect.width / m_viewRect.height;
			base.GetComponent<Camera>().orthographicSize = m_viewRect.height / ((!hd) ? 2f : 4f);
		}
		else
		{
			base.GetComponent<Camera>().aspect = m_viewRect.width / m_viewRect.height;
			base.GetComponent<Camera>().orthographicSize = height / ((!hd) ? 2f : 4f);
		}
	}

	private void GetScreenInfo(out float width, out float height, out bool hd)
	{
		width = 0f;
		height = 0f;
		hd = false;
		if (Application.isPlaying)
		{
			if (Mathf.Max(Screen.width, Screen.height) > 1000)
			{
				if (Mathf.Min(Screen.width, Screen.height) > 700)
				{
					width = 768f;
					height = 1024f;
				}
				else
				{
					width = 640f;
					height = 960f;
				}
				hd = true;
			}
			else if (Mathf.Max(Screen.width, Screen.height) > 900)
			{
				width = 640f;
				height = 960f;
				hd = true;
			}
			else
			{
				width = 320f;
				height = 480f;
				hd = false;
			}
		}
		else
		{
			width = 320f;
			height = 480f;
			hd = false;
		}
	}
}
