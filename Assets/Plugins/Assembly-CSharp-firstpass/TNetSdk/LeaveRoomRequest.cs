namespace TNetSdk
{
	public class LeaveRoomRequest : TNetRequest
	{
		public LeaveRoomRequest()
			: base(RequestType.LeaveRoom)
		{
			Init();
		}

		private void Init()
		{
			packer = new RoomLeaveCmd();
			packet = ((RoomLeaveCmd)packer).MakePacket();
		}
	}
}
