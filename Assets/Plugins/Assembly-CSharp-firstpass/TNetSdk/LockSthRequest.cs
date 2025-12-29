namespace TNetSdk
{
	public class LockSthRequest : TNetRequest
	{
		public LockSthRequest(string key)
			: base(RequestType.LockSth)
		{
			Init(key);
		}

		private void Init(string key)
		{
			packer = new RoomLockReqCmd(key);
			packet = ((RoomLockReqCmd)packer).MakePacket();
		}
	}
}
