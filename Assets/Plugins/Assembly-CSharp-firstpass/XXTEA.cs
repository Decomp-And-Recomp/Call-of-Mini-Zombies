using System;
using System.Text;

public class XXTEA
{
	public static string Encrypt(string source, string key)
	{
		Encoding uTF = Encoding.UTF8;
		byte[] bytes = uTF.GetBytes(base64Encode(source));
		byte[] bytes2 = uTF.GetBytes(key);
		if (bytes.Length == 0)
		{
			return string.Empty;
		}
		return Convert.ToBase64String(ToByteArray(Encrypt(ToUInt32Array(bytes, true), ToUInt32Array(bytes2, false)), false));
	}

	public static string Decrypt(string source, string key)
	{
		if (source.Length == 0)
		{
			return string.Empty;
		}
		Encoding uTF = Encoding.UTF8;
		byte[] data = Convert.FromBase64String(source);
		byte[] bytes = uTF.GetBytes(key);
		return base64Decode(uTF.GetString(ToByteArray(Decrypt(ToUInt32Array(data, false), ToUInt32Array(bytes, false)), true)));
	}

	private static uint[] Encrypt(uint[] v, uint[] k)
	{
		int num = v.Length - 1;
		if (num < 1)
		{
			return v;
		}
		if (k.Length < 4)
		{
			uint[] array = new uint[4];
			k.CopyTo(array, 0);
			k = array;
		}
		uint num2 = v[num];
		uint num3 = v[0];
		uint num4 = 2654435769u;
		uint num5 = 0u;
		int num6 = 6 + 52 / (num + 1);
		while (num6-- > 0)
		{
			num5 += num4;
			uint num7 = (num5 >> 2) & 3u;
			int i;
			for (i = 0; i < num; i++)
			{
				num3 = v[i + 1];
				num2 = (v[i] += (((num2 >> 5) ^ (num3 << 2)) + ((num3 >> 3) ^ (num2 << 4))) ^ ((num5 ^ num3) + (k[(i & 3) ^ num7] ^ num2)));
			}
			num3 = v[0];
			num2 = (v[num] += (((num2 >> 5) ^ (num3 << 2)) + ((num3 >> 3) ^ (num2 << 4))) ^ ((num5 ^ num3) + (k[(i & 3) ^ num7] ^ num2)));
		}
		return v;
	}

	private static uint[] Decrypt(uint[] v, uint[] k)
	{
		int num = v.Length - 1;
		if (num < 1)
		{
			return v;
		}
		if (k.Length < 4)
		{
			uint[] array = new uint[4];
			k.CopyTo(array, 0);
			k = array;
		}
		uint num2 = v[num];
		uint num3 = v[0];
		uint num4 = 2654435769u;
		int num5 = 6 + 52 / (num + 1);
		for (uint num6 = (uint)(num5 * num4); num6 != 0; num6 -= num4)
		{
			uint num7 = (num6 >> 2) & 3u;
			int num8;
			for (num8 = num; num8 > 0; num8--)
			{
				num2 = v[num8 - 1];
				num3 = (v[num8] -= (((num2 >> 5) ^ (num3 << 2)) + ((num3 >> 3) ^ (num2 << 4))) ^ ((num6 ^ num3) + (k[(num8 & 3) ^ num7] ^ num2)));
			}
			num2 = v[num];
			num3 = (v[0] -= (((num2 >> 5) ^ (num3 << 2)) + ((num3 >> 3) ^ (num2 << 4))) ^ ((num6 ^ num3) + (k[(num8 & 3) ^ num7] ^ num2)));
		}
		return v;
	}

	private static uint[] ToUInt32Array(byte[] Data, bool IncludeLength)
	{
		int num = ((((uint)Data.Length & 3u) != 0) ? ((Data.Length >> 2) + 1) : (Data.Length >> 2));
		uint[] array;
		if (IncludeLength)
		{
			array = new uint[num + 1];
			array[num] = (uint)Data.Length;
		}
		else
		{
			array = new uint[num];
		}
		num = Data.Length;
		for (int i = 0; i < num; i++)
		{
			array[i >> 2] |= (uint)(Data[i] << ((i & 3) << 3));
		}
		return array;
	}

	private static byte[] ToByteArray(uint[] Data, bool IncludeLength)
	{
		int num = ((!IncludeLength) ? (Data.Length << 2) : ((int)Data[Data.Length - 1]));
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = (byte)(Data[i >> 2] >> ((i & 3) << 3));
		}
		return array;
	}

	public static string base64Decode(string data)
	{
		try
		{
			UTF8Encoding uTF8Encoding = Encoding.UTF8 as UTF8Encoding;
			byte[] bytes = Convert.FromBase64String(data);
			return uTF8Encoding.GetString(bytes);
		}
		catch (Exception ex)
		{
			throw new Exception("Error in base64Decode" + ex.Message);
		}
	}

	public static string base64Encode(string data)
	{
		try
		{
			byte[] array = new byte[data.Length];
			array = Encoding.UTF8.GetBytes(data);
			return Convert.ToBase64String(array);
		}
		catch (Exception ex)
		{
			throw new Exception("Error in base64Encode" + ex.Message);
		}
	}
}
