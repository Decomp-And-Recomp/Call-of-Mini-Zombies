namespace TNetSdk
{
	public class UnLockSthRequest : TNetRequest
	{
		public UnLockSthRequest(string key)
			: base(RequestType.UnLockSth)
		{
			Init(key);
		}

		private void Init(string key)
		{
			packer = new RoomUnlockReqCmd(key);
			packet = ((RoomUnlockReqCmd)packer).MakePacket();
		}
	}
}
