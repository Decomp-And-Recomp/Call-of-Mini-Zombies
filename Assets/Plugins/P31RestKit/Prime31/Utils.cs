using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Prime31
{
	public static class Utils
	{
		private static System.Random _random;

		public static void logObject(object obj)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (obj != null)
			{
				addObjectToString(stringBuilder, obj);
			}
			try
			{
				Debug.Log(stringBuilder.ToString());
			}
			catch (Exception)
			{
				Console.WriteLine(stringBuilder.ToString());
			}
		}

		private static void addObjectToString(StringBuilder builder, object obj, string indenter = "")
		{
			if (obj is string)
			{
				builder.AppendFormat("{0}{1}\n", indenter, obj);
			}
			else if (obj is IList)
			{
				addIListToString(builder, obj as IList, indenter + "\t");
			}
			else if (obj is IDictionary)
			{
				addIDictionaryToString(builder, obj as IDictionary, indenter);
			}
			else
			{
				builder.AppendFormat("{0}{1}\n", indenter, obj);
			}
		}

		private static void addIDictionaryToString(StringBuilder builder, IDictionary dict, string indenter = "")
		{
			builder.AppendFormat("{0} --------- IDictionary ---------\n", indenter);
			foreach (object key in dict.Keys)
			{
				if (dict[key] is IList)
				{
					builder.AppendFormat("\n{0} --------- IList ---------\n", indenter);
				}
				builder.AppendFormat("{0}{1}: ", indenter, key);
				addObjectToString(builder, dict[key], indenter);
			}
		}

		private static void addIListToString(StringBuilder builder, IList result, string indenter = "")
		{
			builder.Append("\n");
			foreach (object item in result)
			{
				addObjectToString(builder, item, indenter);
			}
		}
	}
}
