namespace TNetSdk
{
	public class KickUserRequest : TNetRequest
	{
		public KickUserRequest(int user_id)
			: base(RequestType.KickUser)
		{
			Init(user_id);
		}

		private void Init(int user_id)
		{
			packer = new RoomKickUserCmd((ushort)user_id);
			packet = ((RoomKickUserCmd)packer).MakePacket();
		}
	}
}
