namespace TNetSdk
{
	public class BroadcastMessageRequest : TNetRequest
	{
		public BroadcastMessageRequest(SFSObject sfs_object)
			: base(RequestType.BroadcastMessage)
		{
			Init(sfs_object);
		}

		private void Init(SFSObject sfs_object)
		{
			packer = new RoomBroadcastMsgCmd(sfs_object.ToBinary().Bytes);
			packet = ((RoomBroadcastMsgCmd)packer).MakePacket();
		}
	}
}
