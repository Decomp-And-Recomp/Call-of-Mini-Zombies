using System.Runtime.InteropServices;
using System.Text;

public class SFSAuth
{
	[DllImport("SFSAuth")]
	protected static extern void SFSAuth_Encode(string challenge, [Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder response);

	public static string Encode(string challenge)
	{
		StringBuilder stringBuilder = new StringBuilder(1024);
		SFSAuth_Encode(challenge, stringBuilder);
		return stringBuilder.ToString();
	}
}
