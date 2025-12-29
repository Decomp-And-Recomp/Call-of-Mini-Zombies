using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomMsgNotifyCmd : UnPacker
	{
		public ushort m_user_id;

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
			if (target != null && target.CurRoom != null)
			{
				TNetUser userById = target.CurRoom.GetUserById(m_user_id);
				if (userById != null)
				{
					event_data.data.Add("user", userById);
				}
			}
			event_data.data.Add("message", sfs_object);
		}
	}
}
