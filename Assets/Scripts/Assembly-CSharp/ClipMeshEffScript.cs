using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class ClipMeshEffScript : MonoBehaviour
{
	public bool Static = true;

	public Material material;

	public string frameName;

	public Color color = Color.white;

	public bool flipX;

	public bool flipY;

	public bool Clip = true;

	protected Material materialClone;

	protected MeshFilter meshFilter;

	protected MeshRenderer meshRender;

	public Rect OriMeshRect = new Rect(0f, 0f, 0f, 0f);

	public float total_eff_time;

	protected float cur_eff_time;

	protected float temp_time;

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
		UpdateMesh(0f);
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
		if (!Clip)
		{
		}
	}

	public void UpdateMesh(float passed_time)
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
			meshFilter.sharedMesh.Clear();
			if (passed_time >= total_eff_time)
			{
				meshFilter.sharedMesh.vertices = new Vector3[4]
				{
					new Vector3(OriMeshRect.x, OriMeshRect.yMax, 0f),
					new Vector3(OriMeshRect.xMax, OriMeshRect.yMax, 0f),
					new Vector3(OriMeshRect.xMax, OriMeshRect.y, 0f),
					new Vector3(OriMeshRect.x, OriMeshRect.y, 0f)
				};
				meshFilter.sharedMesh.uv = new Vector2[4]
				{
					new Vector2(uv.x, uv.y),
					new Vector2(uv.z, uv.y),
					new Vector2(uv.z, uv.w),
					new Vector2(uv.x, uv.w)
				};
				temp_time = 0f;
			}
			else
			{
				meshFilter.sharedMesh.vertices = new Vector3[4]
				{
					new Vector3(OriMeshRect.x, OriMeshRect.y + OriMeshRect.height * (passed_time / total_eff_time), 0f),
					new Vector3(OriMeshRect.xMax, OriMeshRect.y + OriMeshRect.height * (passed_time / total_eff_time), 0f),
					new Vector3(OriMeshRect.xMax, OriMeshRect.y, 0f),
					new Vector3(OriMeshRect.x, OriMeshRect.y, 0f)
				};
				meshFilter.sharedMesh.uv = new Vector2[4]
				{
					new Vector2(uv.x, uv.w + (uv.y - uv.w) * (passed_time / total_eff_time)),
					new Vector2(uv.z, uv.w + (uv.y - uv.w) * (passed_time / total_eff_time)),
					new Vector2(uv.z, uv.w),
					new Vector2(uv.x, uv.w)
				};
			}
			meshFilter.sharedMesh.colors = new Color[4] { color, color, color, color };
			meshFilter.sharedMesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		}
	}

	public void StartClip()
	{
		Clip = true;
		cur_eff_time = Time.time;
	}
}
