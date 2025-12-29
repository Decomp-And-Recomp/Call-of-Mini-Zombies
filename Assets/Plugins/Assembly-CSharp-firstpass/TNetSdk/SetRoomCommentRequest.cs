namespace TNetSdk
{
	public class SetRoomCommentRequest : TNetRequest
	{
		public SetRoomCommentRequest(int room_id, string comment)
			: base(RequestType.ChangeRoomComment)
		{
			Init(room_id, comment);
		}

		private void Init(int room_id, string comment)
		{
			packer = new RoomCommentChangeReqCmd((ushort)room_id, comment);
			packet = ((RoomCommentChangeReqCmd)packer).MakePacket();
		}
	}
}
