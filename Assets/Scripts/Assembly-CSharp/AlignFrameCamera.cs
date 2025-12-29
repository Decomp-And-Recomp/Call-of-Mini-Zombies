using UnityEngine;

public class AlignFrameCamera : MonoBehaviour
{
	public Camera FrameCamera;

	public Vector3 position = Vector3.zero;

	public void SetCamera()
	{
		FrameCamera.transform.position = position;
	}
}
