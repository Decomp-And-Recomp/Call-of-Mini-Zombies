using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomUserStatusNotifyCmd : UnPacker
	{
		public ushort m_user_id;

		public ushort m_key;

		public SFSObject sfs_object;

		public override bool ParserPacket(Packet packet)
		{
			if (!base.ParserPacket(packet))
			{
				return false;
			}
			if (!PopUInt16(ref m_user_id))
			{
				return false;
			}
			if (!PopUInt16(ref m_key))
			{
				return false;
			}
			ushort val = 0;
			if (!PopUInt16(ref val))
			{
				return false;
			}
			byte[] val2 = new byte[val];
			if (!PopByteArray(ref val2, val))
			{
				return false;
			}
			ByteArray ba = new ByteArray(val2);
			sfs_object = SFSObject.NewFromBinaryData(ba);
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			event_data.data.Add("userId", m_user_id);
			event_data.data.Add("key", (TNetUserStatusType)m_key);
			event_data.data.Add("value", sfs_object);
		}
	}
}
