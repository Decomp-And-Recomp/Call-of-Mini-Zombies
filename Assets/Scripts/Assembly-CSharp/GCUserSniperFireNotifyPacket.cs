using UnityEngine;

public class GCUserSniperFireNotifyPacket : BinaryPacket
{
	public uint m_iUserId;

	public Vector3 m_Position;

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
		return true;
	}
}
