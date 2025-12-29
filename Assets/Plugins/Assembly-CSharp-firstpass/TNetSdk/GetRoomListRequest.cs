namespace TNetSdk
{
	public class GetRoomListRequest : TNetRequest
	{
		public GetRoomListRequest(int group_id, int page, int page_split, RoomDragListCmd.ListType type)
			: base(RequestType.GetRoomList)
		{
			Init(group_id, page, page_split, type);
		}

		private void Init(int group_id, int page, int page_split, RoomDragListCmd.ListType type)
		{
			packer = new RoomDragListCmd((ushort)group_id, (ushort)page, (ushort)page_split, type);
			packet = ((RoomDragListCmd)packer).MakePacket();
		}
	}
}
