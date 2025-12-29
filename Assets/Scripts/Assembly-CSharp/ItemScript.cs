using UnityEngine;
using Zombie3D;

public class ItemScript : MonoBehaviour
{
	public ItemType itemType;

	private bool moveUp;

	public Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

	public bool enableUpandDown = true;

	protected float deltaTime;

	public float moveSpeed = 0.2f;

	public float HighPos = 1.2f;

	public float LowPos = 1f;

	protected float floorY = 10000.1f;

	public int VSbonusIndex { get; set; }

	public int GameItemID { get; set; }

	private void Start()
	{
		Ray ray = new Ray(base.transform.position + Vector3.up * 1f, Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 32768))
		{
			floorY = hitInfo.point.y;
		}
		if (!enableUpandDown && itemType != ItemType.ShieldLogo)
		{
			base.transform.position = new Vector3(base.transform.position.x, floorY + LowPos, base.transform.position.z);
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.03f)
		{
			return;
		}
		base.transform.Rotate(rotationSpeed * deltaTime);
		if (enableUpandDown)
		{
			if (!moveUp)
			{
				float num = Mathf.MoveTowards(base.transform.position.y, floorY + LowPos, moveSpeed * deltaTime);
				base.transform.position = new Vector3(base.transform.position.x, num, base.transform.position.z);
				if (num <= floorY + LowPos)
				{
					moveUp = true;
				}
			}
			else
			{
				float num2 = Mathf.MoveTowards(base.transform.position.y, floorY + HighPos, moveSpeed * deltaTime);
				base.transform.position = new Vector3(base.transform.position.x, num2, base.transform.position.z);
				if (num2 >= floorY + HighPos)
				{
					moveUp = false;
				}
			}
		}
		deltaTime = 0f;
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.GetComponent<Collider>().gameObject.layer != 8)
		{
			return;
		}
		PlayerShell component = c.gameObject.GetComponent<PlayerShell>();
		if (!(component != null))
		{
			return;
		}
		Player player = component.m_player;
		if (!player.PlayerBonusState.CheckEnableEquipItem(player, itemType))
		{
			return;
		}
		if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
		{
			if (player.tnet_user.Id == GameApp.GetInstance().GetGameScene().GetPlayer()
				.tnet_user.Id)
			{
				player.OnVsPickUp(itemType, VSbonusIndex.ToString());
			}
		}
		else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
		{
			if (player.m_multi_id == GameApp.GetInstance().GetGameScene().GetPlayer()
				.m_multi_id && player.OnMultiplayerPickUp(itemType, GameItemID))
			{
				GameApp.GetInstance().GetGameScene().itemList.Remove(base.gameObject);
				Object.Destroy(base.gameObject);
			}
		}
		else if (player.OnPickUp(itemType))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
