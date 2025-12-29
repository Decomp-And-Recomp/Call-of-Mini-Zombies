using System;
using System.Runtime.Serialization;

namespace ICSharpCode.SharpZipLib.LZW
{
	[Serializable]
	public class LzwException : SharpZipBaseException
	{
		protected LzwException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public LzwException()
		{
		}

		public LzwException(string message)
			: base(message)
		{
		}

		public LzwException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
