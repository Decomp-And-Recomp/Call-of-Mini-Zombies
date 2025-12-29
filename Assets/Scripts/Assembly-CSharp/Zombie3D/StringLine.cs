namespace Zombie3D
{
	public class StringLine
	{
		public string content = string.Empty;

		public string AddString(string val)
		{
			if (content != string.Empty)
			{
				content = content + "\t" + val;
			}
			else
			{
				content = val;
			}
			return content;
		}
	}
}
