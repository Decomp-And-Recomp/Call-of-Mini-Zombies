using System.Collections.Generic;
using UnityEngine;

public class SwordEffectTrail : MonoBehaviour
{
	public class TronTrailSection
	{
		public Vector3 m_vertexUp;

		public Vector3 m_vertexDown;
	}

	public Material m_material;

	public float m_height = 1f;

	public float m_minDistance = 0.1f;

	private bool m_bCollectPoints;

	private float m_lastTime = 0.5f;

	private float m_time;

	private Mesh mesh;

	private Color m_startColor = new Color(1f, 1f, 1f, 0f);

	private Color m_endColor = Color.white;

	private List<TronTrailSection> m_sections = new List<TronTrailSection>();

	private void Start()
	{
		GameObject gameObject = new GameObject("Trail");
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>() as MeshFilter;
		meshFilter.mesh = new Mesh();
		mesh = meshFilter.mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>() as MeshRenderer;
		meshRenderer.material = m_material;
	}

	private void Update()
	{
		UpdateMesh();
	}

	private void UpdateMesh()
	{
		if (m_bCollectPoints)
		{
			TronTrailSection tronTrailSection = new TronTrailSection();
			tronTrailSection.m_vertexDown = base.transform.Find("point1").position;
			tronTrailSection.m_vertexUp = base.transform.Find("point2").position;
			m_sections.Add(tronTrailSection);
			m_time = m_lastTime;
		}
		if (m_sections.Count < 2 || !(m_time > 0f))
		{
			return;
		}
		m_time -= Time.deltaTime;
		if (m_time < 0f)
		{
			mesh.Clear();
			m_sections.Clear();
			return;
		}
		mesh.Clear();
		Vector3[] array = new Vector3[m_sections.Count * 2];
		Color[] array2 = new Color[m_sections.Count * 2];
		Vector2[] array3 = new Vector2[m_sections.Count * 2];
		TronTrailSection tronTrailSection2 = m_sections[0];
		for (int i = 0; i < m_sections.Count; i++)
		{
			tronTrailSection2 = m_sections[i];
			float num = i;
			num = Mathf.Clamp01(num / (float)m_sections.Count);
			array[i * 2] = tronTrailSection2.m_vertexUp;
			array[i * 2 + 1] = tronTrailSection2.m_vertexDown;
			array3[i * 2] = new Vector2(num, 0f);
			array3[i * 2 + 1] = new Vector2(num, 1f);
			Color color = Color.Lerp(m_startColor, m_endColor, m_time / m_lastTime);
			array2[i * 2] = color;
			array2[i * 2 + 1] = color;
		}
		int[] array4 = new int[(m_sections.Count - 1) * 2 * 3];
		for (int j = 0; j < array4.Length / 6; j++)
		{
			array4[j * 6] = j * 2;
			array4[j * 6 + 1] = j * 2 + 1;
			array4[j * 6 + 2] = j * 2 + 2;
			array4[j * 6 + 3] = j * 2 + 2;
			array4[j * 6 + 4] = j * 2 + 1;
			array4[j * 6 + 5] = j * 2 + 3;
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
	}

	public void ShowTrail(bool bShow)
	{
		if (mesh != null)
		{
			mesh.Clear();
			m_sections.Clear();
		}
		m_bCollectPoints = bShow;
	}
}
