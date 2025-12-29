using UnityEngine;

public class ShooterGameCamera : MonoBehaviour
{
	public Transform player;

	public Transform aimTarget;

	public float smoothingTime = 0.5f;

	public Vector3 pivotOffset = new Vector3(1.3f, 0.4f, 0f);

	public Vector3 camOffset = new Vector3(0f, 0.7f, -2.4f);

	public Vector3 closeOffset = new Vector3(0.35f, 1.7f, 0f);

	public float horizontalAimingSpeed = 270f;

	public float verticalAimingSpeed = 270f;

	public float maxVerticalAngle = 80f;

	public float minVerticalAngle = -80f;

	public float mouseSensitivity = 0.1f;

	public Texture reticle;

	private float angleH;

	private float angleV;

	private Transform cam;

	private float maxCamDist = 1f;

	private LayerMask mask;

	private Vector3 smoothPlayerPos;

	private void Start()
	{
		mask = 1 << player.gameObject.layer;
		mask = (int)mask | (1 << (LayerMask.NameToLayer("Ignore Raycast") & 0x1F));
		mask = ~(int)mask;
		cam = base.transform;
		smoothPlayerPos = player.position;
		maxCamDist = 3f;
	}

	private void LateUpdate()
	{
		if (Time.deltaTime != 0f && Time.timeScale != 0f && !(player == null))
		{
			angleH += Mathf.Clamp(Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal"), -1f, 1f) * horizontalAimingSpeed * Time.deltaTime;
			angleV += Mathf.Clamp(Input.GetAxis("Mouse Y") + Input.GetAxis("Vertical"), -1f, 1f) * verticalAimingSpeed * Time.deltaTime;
			angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
			float magnitude = (aimTarget.position - cam.position).magnitude;
			Quaternion quaternion = Quaternion.Euler(0f - angleV, angleH, 0f);
			Quaternion quaternion2 = Quaternion.Euler(0f, angleH, 0f);
			cam.rotation = quaternion;
			smoothPlayerPos = Vector3.Lerp(smoothPlayerPos, player.position, smoothingTime * Time.deltaTime);
			smoothPlayerPos.x = player.position.x;
			smoothPlayerPos.z = player.position.z;
			Vector3 vector = smoothPlayerPos + quaternion2 * pivotOffset + quaternion * camOffset;
			Vector3 vector2 = player.position + quaternion2 * closeOffset;
			float num = Vector3.Distance(vector, vector2);
			maxCamDist = Mathf.Lerp(maxCamDist, num, 5f * Time.deltaTime);
			Vector3 vector3 = (vector - vector2) / num;
			float num2 = 0.3f;
			RaycastHit hitInfo;
			if (Physics.Raycast(vector2, vector3, out hitInfo, maxCamDist + num2, mask))
			{
				maxCamDist = hitInfo.distance - num2;
			}
			cam.position = vector2 + vector3 * maxCamDist;
			float num3 = ((!Physics.Raycast(cam.position, cam.forward, out hitInfo, 100f, mask)) ? Mathf.Max(5f, magnitude) : (hitInfo.distance + 0.05f));
			aimTarget.position = cam.position + cam.forward * num3;
		}
	}

	private void OnGUI()
	{
	}
}
