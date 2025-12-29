using System.Collections.Generic;

public class HCGameServerListPacket : BinaryPacket
{
	public class GameServerInfo
	{
		public uint iServiceIP;

		public ushort sServicePort;

		public uint iRoomNum;

		public uint iOnlineNum;
	}

	public List<GameServerInfo> m_GameServerList = new List<GameServerInfo>();

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
		for (int i = 0; i < val; i++)
		{
			GameServerInfo gameServerInfo = new GameServerInfo();
			if (!packet.PopUInt32(ref gameServerInfo.iServiceIP))
			{
				return false;
			}
			if (!packet.PopUInt16(ref gameServerInfo.sServicePort))
			{
				return false;
			}
			if (!packet.PopUInt32(ref gameServerInfo.iRoomNum))
			{
				return false;
			}
			if (!packet.PopUInt32(ref gameServerInfo.iOnlineNum))
			{
				return false;
			}
			m_GameServerList.Add(gameServerInfo);
		}
		return true;
	}
}
