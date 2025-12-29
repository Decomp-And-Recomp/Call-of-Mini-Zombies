using UnityEngine;

public class GCEnemyBirthNotifyPacket : BinaryPacket
{
	public uint m_enemy_wave;

	public uint m_enemy_Id;

	public uint m_enemy_type;

	public uint m_isElite;

	public uint m_isGrave;

	public uint m_isSuperBoss;

	public Vector3 m_Position;

	public uint m_target_id;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_enemy_wave))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_enemy_Id))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_enemy_type))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_isElite))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_isGrave))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_isSuperBoss))
		{
			return false;
		}
		uint val = 0u;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		m_Position.x = (float)(int)val * 1f / 100f;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		m_Position.y = (float)(int)val * 1f / 100f;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		m_Position.z = (float)(int)val * 1f / 100f;
		if (!packet.PopUInt32(ref m_target_id))
		{
			return false;
		}
		return true;
	}
}
