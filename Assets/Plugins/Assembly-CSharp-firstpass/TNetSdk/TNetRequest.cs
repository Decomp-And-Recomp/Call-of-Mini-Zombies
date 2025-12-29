using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class TNetRequest
	{
		protected int id;

		protected Packet packet;

		protected Packer packer;

		public Packet Message
		{
			get
			{
				return packet;
			}
		}

		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public RequestType Type
		{
			get
			{
				return (RequestType)id;
			}
			set
			{
				id = (int)value;
			}
		}

		public TNetRequest(RequestType tp)
		{
			id = (int)tp;
		}

		public TNetRequest(int id)
		{
			this.id = id;
		}
	}
}
