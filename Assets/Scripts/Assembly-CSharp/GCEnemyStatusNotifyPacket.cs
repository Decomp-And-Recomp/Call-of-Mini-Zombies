using System.Text;
using UnityEngine;

public class GCEnemyStatusNotifyPacket : BinaryPacket
{
	public string m_enemyID;

	public Vector3 m_Position;

	public Vector3 m_Rotation;

	public Vector3 m_Direction;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		uint val = 0u;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		if (!packet.CheckBytesLeft((int)val))
		{
			return false;
		}
		m_enemyID = Encoding.ASCII.GetString(packet.ByteArray(), packet.Position, (int)val);
		packet.Position += (int)val;
		uint val2 = 0u;
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		m_Position.x = (float)(int)val2 * 1f / 100f;
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		m_Position.y = (float)(int)val2 * 1f / 100f;
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		m_Position.z = (float)(int)val2 * 1f / 100f;
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		m_Rotation.x = (float)(int)val2 * 1f / 100f;
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		m_Rotation.y = (float)(int)val2 * 1f / 100f;
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		m_Rotation.z = (float)(int)val2 * 1f / 100f;
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		m_Direction.x = (float)(int)val2 * 1f / 100f;
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		m_Direction.y = (float)(int)val2 * 1f / 100f;
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		m_Direction.z = (float)(int)val2 * 1f / 100f;
		return true;
	}
}
