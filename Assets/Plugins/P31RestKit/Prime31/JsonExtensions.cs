using System.Collections.Generic;

namespace Prime31
{
	public static class JsonExtensions
	{
		public static Dictionary<string, object> dictionaryFromJson(this string json)
		{
			return Json.jsonDecode(json, true) as Dictionary<string, object>;
		}
	}
}
