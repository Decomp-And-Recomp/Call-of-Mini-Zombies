namespace TNetSdk
{
	public class SetUserStatusRequest : TNetRequest
	{
		public SetUserStatusRequest(TNetUserStatusType key, SFSObject value)
			: base(RequestType.SetUserStatus)
		{
			Init(key, value);
		}

		private void Init(TNetUserStatusType key, SFSObject value)
		{
			packer = new RoomSetUserStatusCmd((ushort)key, value.ToBinary().Bytes);
			packet = ((RoomSetUserStatusCmd)packer).MakePacket();
		}
	}
}
