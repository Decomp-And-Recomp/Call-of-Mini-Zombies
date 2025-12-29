using System;

namespace TNetSdk
{
	public class SFSError : Exception
	{
		public SFSError(string message)
			: base(message)
		{
		}
	}
}
