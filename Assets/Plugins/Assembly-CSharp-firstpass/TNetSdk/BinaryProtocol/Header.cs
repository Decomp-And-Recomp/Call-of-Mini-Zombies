namespace TNetSdk.BinaryProtocol
{
	public class Header
	{
		public const short HEADER_LENGTH = 10;

		public ushort m_sLength;

		public ushort m_sVersion;

		public ushort m_sProtocol;

		public ushort m_sCmd;

		public ushort m_sCompressType;
	}
}
