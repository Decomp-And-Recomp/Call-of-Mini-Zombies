using UnityEngine;

public class DebuffArea : MonoBehaviour
{
	public float _Debuff_Interval = 1f;

	public float _Debuff_Damage = 5f;

	protected float _Debuff_Time_Start;

	private void Start()
	{
		_Debuff_Time_Start = Time.time;
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			PlayerShell component = other.gameObject.GetComponent<PlayerShell>();
			if (component != null)
			{
				component.m_player.OnHit(_Debuff_Damage);
			}
			_Debuff_Time_Start = Time.time;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 8 && Time.time - _Debuff_Time_Start > _Debuff_Interval)
		{
			PlayerShell component = other.gameObject.GetComponent<PlayerShell>();
			if (component != null)
			{
				component.m_player.OnHit(_Debuff_Damage);
			}
			_Debuff_Time_Start = Time.time;
		}
	}
}
