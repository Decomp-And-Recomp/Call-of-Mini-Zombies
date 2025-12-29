using UnityEngine;
using Zombie3D;

public class EnemyMark : MonoBehaviour
{
	public Enemy m_enemy;

	protected float ori_x;

	protected float ori_y;

	protected float tar_x;

	protected float tar_y;

	protected Vector3 tem_pos;

	protected Vector2 tem;

	private void Start()
	{
		base.transform.localPosition = new Vector3(-5000f, 0f, base.transform.localPosition.z);
		base.gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = "EnemyMark";
	}

	private void Update()
	{
		if (m_enemy != null && m_enemy.GetEnemyObject() != null && GameUIScriptNew.GetGameUIScript().uiInited)
		{
			tem_pos = GameApp.GetInstance().GetGameScene().GetPlayer()
				.GetTransform()
				.InverseTransformPoint(m_enemy.GetTransform().position);
			tem = new Vector2(tem_pos.x, tem_pos.z);
			if (tem.sqrMagnitude > 1444f)
			{
				tem_pos = tem_pos.normalized * 38f;
			}
			base.transform.localPosition = new Vector3(tem_pos.x, tem_pos.z, base.transform.localPosition.z);
		}
	}
}
