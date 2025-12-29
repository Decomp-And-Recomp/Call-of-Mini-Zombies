using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class TUIMeshSprite : MonoBehaviour
{
	public bool Static = true;

	public Material material;

	public string frameName;

	public Color color = Color.white;

	public bool flipX;

	public bool flipY;

	protected Material materialClone;

	protected MeshFilter meshFilter;

	protected MeshRenderer meshRender;

	public bool Static_Accessor
	{
		get
		{
			return Static;
		}
		set
		{
			Static = value;
		}
	}

	public Material material_Accessor
	{
		get
		{
			return material;
		}
		set
		{
			material = value;
			Static = false;
		}
	}

	public string frameName_Accessor
	{
		get
		{
			return frameName;
		}
		set
		{
			frameName = value;
			Static = false;
		}
	}

	public Color color_Accessor
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
			Static = false;
		}
	}

	public bool flipX_Accessor
	{
		get
		{
			return flipX;
		}
		set
		{
			flipX = value;
			Static = false;
		}
	}

	public bool flipY_Accessor
	{
		get
		{
			return flipY;
		}
		set
		{
			flipY = value;
			Static = false;
		}
	}

	public void Awake()
	{
		if ((bool)material)
		{
			materialClone = Object.Instantiate(material) as Material;
			materialClone.hideFlags = HideFlags.DontSave;
		}
		else
		{
			materialClone = null;
		}
		meshFilter = base.gameObject.GetComponent<MeshFilter>();
		meshRender = base.gameObject.GetComponent<MeshRenderer>();
		meshFilter.sharedMesh = new Mesh();
		meshFilter.sharedMesh.hideFlags = HideFlags.DontSave;
		meshRender.castShadows = false;
		meshRender.receiveShadows = false;
	}

	public void Start()
	{
		UpdateMesh();
	}

	private void OnDestroy()
	{
		if ((bool)meshFilter && (bool)meshFilter.sharedMesh)
		{
			Object.DestroyImmediate(meshFilter.sharedMesh);
		}
		if ((bool)materialClone)
		{
			Object.DestroyImmediate(materialClone);
		}
	}

	public void Update()
	{
		if (!Static)
		{
			UpdateMesh();
		}
	}

	public virtual void UpdateMesh()
	{
		if (!(meshFilter == null) && !(meshRender == null))
		{
			TUITextureManager.Frame frame = TUITextureManager.Instance().GetFrame(frameName);
			if ((bool)materialClone)
			{
				materialClone.mainTexture = frame.texture;
				meshRender.sharedMaterial = materialClone;
			}
			else if ((bool)frame.material)
			{
				meshRender.sharedMaterial = frame.material;
			}
			Vector4 uv = frame.uv;
			if (flipX)
			{
				float x = uv.x;
				uv.x = uv.z;
				uv.z = x;
			}
			if (flipY)
			{
				float y = uv.y;
				uv.y = uv.w;
				uv.w = y;
			}
			Vector3 vector = new Vector3(base.transform.position.x - (float)(int)base.transform.position.x, base.transform.position.y - (float)(int)base.transform.position.y, 0f);
			Vector3 vector2 = new Vector3(frame.size.x % 2f / 2f, frame.size.y % 2f / 2f, 0f);
			Vector3 vector3 = vector + vector2;
			if (meshFilter == null || meshFilter.sharedMesh == null)
			{
				Debug.Log(base.gameObject.name);
			}
			meshFilter.sharedMesh.Clear();
			meshFilter.sharedMesh.vertices = new Vector3[4]
			{
				new Vector3(frame.size.x * -0.5f, frame.size.y * 0.5f, 0f) - vector3,
				new Vector3(frame.size.x * 0.5f, frame.size.y * 0.5f, 0f) - vector3,
				new Vector3(frame.size.x * 0.5f, frame.size.y * -0.5f, 0f) - vector3,
				new Vector3(frame.size.x * -0.5f, frame.size.y * -0.5f, 0f) - vector3
			};
			meshFilter.sharedMesh.uv = new Vector2[4]
			{
				new Vector2(uv.x, uv.y),
				new Vector2(uv.z, uv.y),
				new Vector2(uv.z, uv.w),
				new Vector2(uv.x, uv.w)
			};
			meshFilter.sharedMesh.colors = new Color[4] { color, color, color, color };
			meshFilter.sharedMesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
			Static = true;
		}
	}
}
