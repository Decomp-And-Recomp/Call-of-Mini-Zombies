namespace TNetSdk
{
	public class LoginRequest : TNetRequest
	{
		public static readonly string KEY_ZONE_NAME = "zn";

		public static readonly string KEY_USER_NAME = "un";

		public static readonly string KEY_PASSWORD = "pw";

		public static readonly string KEY_PARAMS = "p";

		public static readonly string KEY_PRIVILEGE_ID = "pi";

		public static readonly string KEY_ID = "id";

		public static readonly string KEY_ROOMLIST = "rl";

		public static readonly string KEY_RECONNECTION_SECONDS = "rs";

		public LoginRequest(string userName, string password, string account)
			: base(RequestType.Login)
		{
			Init(userName, password, account);
		}

		public LoginRequest(string userName, string password)
			: base(RequestType.Login)
		{
			Init(userName, password, string.Empty);
		}

		public LoginRequest(string userName)
			: base(RequestType.Login)
		{
			Init(userName, string.Empty, string.Empty);
		}

		private void Init(string userName, string password, string account)
		{
			packer = new SysLoginCmd(account, password, userName);
			packet = ((SysLoginCmd)packer).MakePacket();
		}
	}
}
