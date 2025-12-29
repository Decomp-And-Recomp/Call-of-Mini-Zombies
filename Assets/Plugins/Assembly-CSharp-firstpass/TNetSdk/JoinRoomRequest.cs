namespace TNetSdk
{
	public class JoinRoomRequest : TNetRequest
	{
		public JoinRoomRequest(int room_id, string pwd)
			: base(RequestType.JoinRoom)
		{
			Init(room_id, pwd);
		}

		private void Init(int room_id, string pwd)
		{
			packer = new RoomJoinCmd((ushort)room_id, pwd);
			packet = ((RoomJoinCmd)packer).MakePacket();
		}
	}
}
