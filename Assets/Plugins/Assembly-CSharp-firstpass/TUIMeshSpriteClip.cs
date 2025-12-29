using UnityEngine;

public class TUIMeshSpriteClip : TUIMeshSprite
{
	public TUIRect clip;

	public override void UpdateMesh()
	{
		if (clip == null)
		{
			base.UpdateMesh();
		}
		else if (!(meshFilter == null) && !(meshRender == null))
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
			Rect rect = new Rect(frame.size.x * -0.5f, frame.size.y * -0.5f, frame.size.x, frame.size.y);
			Rect rectLocal = clip.GetRectLocal(base.transform);
			Rect rect2 = TUIRect.RectIntersect(rect, rectLocal);
			if (rect2.width <= 0f || rect2.height <= 0f)
			{
				meshFilter.sharedMesh.Clear();
				return;
			}
			Vector4 zero = Vector4.zero;
			float num = (uv.x + uv.z) * 0.5f;
			float num2 = (uv.z - uv.x) / frame.size.x;
			float num3 = (uv.y + uv.w) * 0.5f;
			float num4 = (uv.y - uv.w) / frame.size.y;
			zero.x = num + num2 * rect2.xMin;
			zero.z = num + num2 * rect2.xMax;
			zero.w = num3 + num4 * rect2.yMin;
			zero.y = num3 + num4 * rect2.yMax;
			Vector3 vector = new Vector3(base.transform.position.x - (float)(int)base.transform.position.x, base.transform.position.y - (float)(int)base.transform.position.y, 0f);
			Vector3 vector2 = new Vector3(frame.size.x % 2f / 2f, frame.size.y % 2f / 2f, 0f);
			Vector3 vector3 = vector + vector2;
			meshFilter.sharedMesh.Clear();
			meshFilter.sharedMesh.vertices = new Vector3[4]
			{
				new Vector3(rect2.xMin, rect2.yMax, 0f) - vector3,
				new Vector3(rect2.xMax, rect2.yMax, 0f) - vector3,
				new Vector3(rect2.xMax, rect2.yMin, 0f) - vector3,
				new Vector3(rect2.xMin, rect2.yMin, 0f) - vector3
			};
			meshFilter.sharedMesh.uv = new Vector2[4]
			{
				new Vector2(zero.x, zero.y),
				new Vector2(zero.z, zero.y),
				new Vector2(zero.z, zero.w),
				new Vector2(zero.x, zero.w)
			};
			meshFilter.sharedMesh.colors = new Color[4] { color, color, color, color };
			meshFilter.sharedMesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		}
	}
}
