using UnityEngine;

public class GCUserStatusNotifyPacket : BinaryPacket
{
	public uint m_iUserId;

	public Vector3 m_Position;

	public Vector3 m_Rotation;

	public Vector3 m_direct;

	public ulong m_iPingTime;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iUserId))
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
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		m_Rotation.x = (float)(int)val * 1f / 100f;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		m_Rotation.y = (float)(int)val * 1f / 100f;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		m_Rotation.z = (float)(int)val * 1f / 100f;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		m_direct.x = (float)(int)val * 1f / 100f;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		m_direct.y = (float)(int)val * 1f / 100f;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		m_direct.z = (float)(int)val * 1f / 100f;
		if (!packet.PopUInt64(ref m_iPingTime))
		{
			return false;
		}
		return true;
	}
}
