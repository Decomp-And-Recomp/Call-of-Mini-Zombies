using System.Collections.Generic;
using UnityEngine;

public class TUIMeshSpriteCull : TUIMeshSprite
{
	public TUIRect cull;

	public override void UpdateMesh()
	{
		if (cull == null)
		{
			base.UpdateMesh();
		}
		else
		{
			if (meshFilter == null || meshRender == null)
			{
				return;
			}
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
			float num = (uv.x + uv.z) * 0.5f;
			float num2 = (uv.z - uv.x) / frame.size.x;
			float num3 = (uv.y + uv.w) * 0.5f;
			float num4 = (uv.y - uv.w) / frame.size.y;
			Rect rect = new Rect(frame.size.x * -0.5f, frame.size.y * -0.5f, frame.size.x, frame.size.y);
			Rect rectLocal = cull.GetRectLocal(base.transform);
			Rect rect2 = TUIRect.RectIntersect(rect, rectLocal);
			if (rect2.width <= 0f || rect2.height <= 0f)
			{
				base.UpdateMesh();
				return;
			}
			List<Vector3> list = new List<Vector3>();
			List<Vector2> list2 = new List<Vector2>();
			List<Color> list3 = new List<Color>();
			List<int> list4 = new List<int>();
			int num5 = 0;
			if (rectLocal.xMin > rect.xMin)
			{
				Rect rect3 = new Rect(rect.xMin, rect.yMin, rectLocal.xMin - rect.xMin, rect.height);
				Vector4 zero = Vector4.zero;
				zero.x = num + num2 * rect3.xMin;
				zero.z = num + num2 * rect3.xMax;
				zero.w = num3 + num4 * rect3.yMin;
				zero.y = num3 + num4 * rect3.yMax;
				list.Add(new Vector3(rect3.xMin, rect3.yMax, 0f));
				list.Add(new Vector3(rect3.xMax, rect3.yMax, 0f));
				list.Add(new Vector3(rect3.xMax, rect3.yMin, 0f));
				list.Add(new Vector3(rect3.xMin, rect3.yMin, 0f));
				list2.Add(new Vector2(zero.x, zero.y));
				list2.Add(new Vector2(zero.z, zero.y));
				list2.Add(new Vector2(zero.z, zero.w));
				list2.Add(new Vector2(zero.x, zero.w));
				list3.Add(color);
				list3.Add(color);
				list3.Add(color);
				list3.Add(color);
				list4.Add(num5 * 4);
				list4.Add(num5 * 4 + 1);
				list4.Add(num5 * 4 + 2);
				list4.Add(num5 * 4);
				list4.Add(num5 * 4 + 2);
				list4.Add(num5 * 4 + 3);
				num5++;
			}
			if (rectLocal.xMax < rect.xMax)
			{
				Rect rect4 = new Rect(rectLocal.xMax, rect.yMin, rect.xMax - rectLocal.xMax, rect.height);
				Vector4 zero2 = Vector4.zero;
				zero2.x = num + num2 * rect4.xMin;
				zero2.z = num + num2 * rect4.xMax;
				zero2.w = num3 + num4 * rect4.yMin;
				zero2.y = num3 + num4 * rect4.yMax;
				list.Add(new Vector3(rect4.xMin, rect4.yMax, 0f));
				list.Add(new Vector3(rect4.xMax, rect4.yMax, 0f));
				list.Add(new Vector3(rect4.xMax, rect4.yMin, 0f));
				list.Add(new Vector3(rect4.xMin, rect4.yMin, 0f));
				list2.Add(new Vector2(zero2.x, zero2.y));
				list2.Add(new Vector2(zero2.z, zero2.y));
				list2.Add(new Vector2(zero2.z, zero2.w));
				list2.Add(new Vector2(zero2.x, zero2.w));
				list3.Add(color);
				list3.Add(color);
				list3.Add(color);
				list3.Add(color);
				list4.Add(num5 * 4);
				list4.Add(num5 * 4 + 1);
				list4.Add(num5 * 4 + 2);
				list4.Add(num5 * 4);
				list4.Add(num5 * 4 + 2);
				list4.Add(num5 * 4 + 3);
				num5++;
			}
			if (rectLocal.yMax < rect.yMax)
			{
				Rect rect5 = new Rect(rect2.xMin, rectLocal.yMax, rect2.width, rect.yMax - rectLocal.yMax);
				Vector4 zero3 = Vector4.zero;
				zero3.x = num + num2 * rect5.xMin;
				zero3.z = num + num2 * rect5.xMax;
				zero3.w = num3 + num4 * rect5.yMin;
				zero3.y = num3 + num4 * rect5.yMax;
				list.Add(new Vector3(rect5.xMin, rect5.yMax, 0f));
				list.Add(new Vector3(rect5.xMax, rect5.yMax, 0f));
				list.Add(new Vector3(rect5.xMax, rect5.yMin, 0f));
				list.Add(new Vector3(rect5.xMin, rect5.yMin, 0f));
				list2.Add(new Vector2(zero3.x, zero3.y));
				list2.Add(new Vector2(zero3.z, zero3.y));
				list2.Add(new Vector2(zero3.z, zero3.w));
				list2.Add(new Vector2(zero3.x, zero3.w));
				list3.Add(color);
				list3.Add(color);
				list3.Add(color);
				list3.Add(color);
				list4.Add(num5 * 4);
				list4.Add(num5 * 4 + 1);
				list4.Add(num5 * 4 + 2);
				list4.Add(num5 * 4);
				list4.Add(num5 * 4 + 2);
				list4.Add(num5 * 4 + 3);
				num5++;
			}
			if (rect.yMin < rectLocal.yMin)
			{
				Rect rect6 = new Rect(rect2.xMin, rect.yMin, rect2.width, rectLocal.yMin - rect.yMin);
				Vector4 zero4 = Vector4.zero;
				zero4.x = num + num2 * rect6.xMin;
				zero4.z = num + num2 * rect6.xMax;
				zero4.w = num3 + num4 * rect6.yMin;
				zero4.y = num3 + num4 * rect6.yMax;
				list.Add(new Vector3(rect6.xMin, rect6.yMax, 0f));
				list.Add(new Vector3(rect6.xMax, rect6.yMax, 0f));
				list.Add(new Vector3(rect6.xMax, rect6.yMin, 0f));
				list.Add(new Vector3(rect6.xMin, rect6.yMin, 0f));
				list2.Add(new Vector2(zero4.x, zero4.y));
				list2.Add(new Vector2(zero4.z, zero4.y));
				list2.Add(new Vector2(zero4.z, zero4.w));
				list2.Add(new Vector2(zero4.x, zero4.w));
				list3.Add(color);
				list3.Add(color);
				list3.Add(color);
				list3.Add(color);
				list4.Add(num5 * 4);
				list4.Add(num5 * 4 + 1);
				list4.Add(num5 * 4 + 2);
				list4.Add(num5 * 4);
				list4.Add(num5 * 4 + 2);
				list4.Add(num5 * 4 + 3);
				num5++;
			}
			meshFilter.sharedMesh.Clear();
			meshFilter.sharedMesh.vertices = list.ToArray();
			meshFilter.sharedMesh.uv = list2.ToArray();
			meshFilter.sharedMesh.colors = list3.ToArray();
			meshFilter.sharedMesh.triangles = list4.ToArray();
		}
	}
}
