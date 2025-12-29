using System;

namespace TNetSdk
{
	public class SFSCodecError : Exception
	{
		public SFSCodecError(string message)
			: base(message)
		{
		}
	}
}
