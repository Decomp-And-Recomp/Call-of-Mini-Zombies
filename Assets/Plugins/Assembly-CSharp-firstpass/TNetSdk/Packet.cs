using System;

namespace TNetSdk
{
	public class Packet
	{
		public const int MAX_PACKET_SIZE = 4096;

		private byte[] TEMP_BYTE_ARRAY;

		private int CURRENT_LENGTH;

		private int CURRENT_POSITION;

		public int Length
		{
			get
			{
				return CURRENT_LENGTH;
			}
		}

		public int Position
		{
			get
			{
				return CURRENT_POSITION;
			}
			set
			{
				CURRENT_POSITION = value;
			}
		}

		public Packet(int length)
		{
			Initialize();
			CURRENT_LENGTH = length;
		}

		public Packet(byte[] bytes, int length, bool bCopy)
		{
			if (bCopy)
			{
				Initialize();
				Array.Copy(bytes, 0, TEMP_BYTE_ARRAY, 0, length);
			}
			else
			{
				TEMP_BYTE_ARRAY = bytes;
				CURRENT_POSITION = 0;
			}
			CURRENT_LENGTH = length;
		}

		public static bool LengthIsVaild(int length)
		{
			return length <= 4096;
		}

		public void Initialize()
		{
			TEMP_BYTE_ARRAY = new byte[4096];
			TEMP_BYTE_ARRAY.Initialize();
			CURRENT_LENGTH = 0;
			CURRENT_POSITION = 0;
		}

		public byte[] ByteArray()
		{
			return TEMP_BYTE_ARRAY;
		}

		public bool CheckBytesLeft(int inLenLeft)
		{
			if (CURRENT_LENGTH - CURRENT_POSITION < inLenLeft)
			{
				return false;
			}
			return true;
		}

		public bool WatchUInt32(ref uint val, int pos)
		{
			if (CURRENT_LENGTH - pos < 4)
			{
				return false;
			}
			val = (uint)((TEMP_BYTE_ARRAY[pos] << 24) | (TEMP_BYTE_ARRAY[pos + 1] << 16) | (TEMP_BYTE_ARRAY[pos + 2] << 8) | TEMP_BYTE_ARRAY[pos + 3]);
			return true;
		}

		public bool WatchUInt16(ref ushort val, int pos)
		{
			if (CURRENT_LENGTH - pos < 2)
			{
				return false;
			}
			val = (ushort)((TEMP_BYTE_ARRAY[pos] << 8) | TEMP_BYTE_ARRAY[pos + 1]);
			return true;
		}

		public bool PushByte(byte by)
		{
			if (!CheckBytesLeft(1))
			{
				return false;
			}
			TEMP_BYTE_ARRAY[CURRENT_POSITION++] = by;
			return true;
		}

		public bool PushUInt16(ushort Num)
		{
			if (!CheckBytesLeft(2))
			{
				return false;
			}
			TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)((uint)((Num & 0xFF00) >> 8) & 0xFFu);
			TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)(Num & 0xFFu & 0xFFu);
			return true;
		}

		public bool PushUInt32(uint Num)
		{
			if (!CheckBytesLeft(4))
			{
				return false;
			}
			TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)(((Num & 0xFF000000u) >> 24) & 0xFFu);
			TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)(((Num & 0xFF0000) >> 16) & 0xFFu);
			TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)(((Num & 0xFF00) >> 8) & 0xFFu);
			TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)(Num & 0xFFu & 0xFFu);
			return true;
		}

		public bool PushUInt64(ulong Num)
		{
			if (!CheckBytesLeft(8))
			{
				return false;
			}
			uint num = (uint)(Num >> 32);
			uint num2 = (uint)Num;
			PushUInt32(num);
			PushUInt32(num2);
			return true;
		}

		public bool PushByteArray(byte[] buf, int length)
		{
			if (!CheckBytesLeft(length))
			{
				return false;
			}
			Array.Copy(buf, 0, TEMP_BYTE_ARRAY, CURRENT_POSITION, length);
			CURRENT_POSITION += length;
			return true;
		}

		public bool PopByte(ref byte val)
		{
			if (!CheckBytesLeft(1))
			{
				return false;
			}
			val = TEMP_BYTE_ARRAY[CURRENT_POSITION++];
			return true;
		}

		public bool PopUInt16(ref ushort val)
		{
			if (!CheckBytesLeft(2))
			{
				return false;
			}
			val = (ushort)((TEMP_BYTE_ARRAY[CURRENT_POSITION] << 8) | TEMP_BYTE_ARRAY[CURRENT_POSITION + 1]);
			CURRENT_POSITION += 2;
			return true;
		}

		public bool PopUInt32(ref uint val)
		{
			if (!CheckBytesLeft(4))
			{
				return false;
			}
			val = (uint)((TEMP_BYTE_ARRAY[CURRENT_POSITION] << 24) | (TEMP_BYTE_ARRAY[CURRENT_POSITION + 1] << 16) | (TEMP_BYTE_ARRAY[CURRENT_POSITION + 2] << 8) | TEMP_BYTE_ARRAY[CURRENT_POSITION + 3]);
			CURRENT_POSITION += 4;
			return true;
		}

		public bool PopUInt64(ref ulong val)
		{
			if (!CheckBytesLeft(8))
			{
				return false;
			}
			uint val2 = 0u;
			uint val3 = 0u;
			PopUInt32(ref val2);
			PopUInt32(ref val3);
			ulong num = val2;
			val = (num << 32) + val3;
			return true;
		}

		public bool PopByteArray(ref byte[] val, int len)
		{
			if (!CheckBytesLeft(len))
			{
				return false;
			}
			Array.Copy(TEMP_BYTE_ARRAY, CURRENT_POSITION, val, 0, len);
			CURRENT_POSITION += len;
			return true;
		}
	}
}
