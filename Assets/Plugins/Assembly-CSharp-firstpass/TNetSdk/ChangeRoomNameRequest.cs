namespace TNetSdk
{
	public class ChangeRoomNameRequest : TNetRequest
	{
		public ChangeRoomNameRequest(string room_name)
			: base(RequestType.ChangeRoomName)
		{
			Init(room_name);
		}

		private void Init(string room_name)
		{
			packer = new RoomRenameCmd(room_name);
			packet = ((RoomRenameCmd)packer).MakePacket();
		}
	}
}
