namespace TNetSdk
{
	public class CreateRoomRequest : TNetRequest
	{
		public CreateRoomRequest(string room_name, string pwd, int group_id, int max_user, RoomCreateCmd.RoomType room_limit, RoomCreateCmd.RoomSwitchMasterType master_type, string comment = "")
			: base(RequestType.CreateRoom)
		{
			Init(room_name, pwd, group_id, max_user, room_limit, master_type, comment);
		}

		private void Init(string room_name, string pwd, int group_id, int max_user, RoomCreateCmd.RoomType room_limit, RoomCreateCmd.RoomSwitchMasterType master_type, string comment)
		{
			packer = new RoomCreateCmd(room_name, pwd, (ushort)group_id, (ushort)max_user, room_limit, master_type, comment);
			packet = ((RoomCreateCmd)packer).MakePacket();
		}
	}
}
