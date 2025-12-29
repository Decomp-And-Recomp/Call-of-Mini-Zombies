namespace TNetSdk
{
	public class ObjectMessageRequest : TNetRequest
	{
		public ObjectMessageRequest(int user_id, SFSObject msg)
			: base(RequestType.ObjectMessage)
		{
			Init(user_id, msg);
		}

		private void Init(int user_id, SFSObject msg)
		{
			packer = new RoomSendMsgCmd((ushort)user_id, msg.ToBinary().Bytes);
			packet = ((RoomSendMsgCmd)packer).MakePacket();
		}
	}
}
