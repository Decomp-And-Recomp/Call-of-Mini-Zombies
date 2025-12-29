using System;
using System.Text;

namespace TNetSdk
{
	public class ByteArray
	{
		private byte[] buffer;

		private int position;

		public byte[] Bytes
		{
			get
			{
				return buffer;
			}
			set
			{
				buffer = value;
			}
		}

		public int Length
		{
			get
			{
				return buffer.Length;
			}
		}

		public int Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		public int BytesAvailable
		{
			get
			{
				int num = buffer.Length - position;
				if (num > buffer.Length || num < 0)
				{
					num = 0;
				}
				return num;
			}
		}

		public ByteArray()
		{
			buffer = new byte[0];
		}

		public ByteArray(byte[] buf)
		{
			buffer = buf;
		}

		public byte[] ReverseOrder(byte[] dt)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return dt;
			}
			byte[] array = new byte[dt.Length];
			int num = 0;
			for (int num2 = dt.Length - 1; num2 >= 0; num2--)
			{
				array[num++] = dt[num2];
			}
			return array;
		}

		public void WriteByte(SFSDataType tp)
		{
			WriteByte(Convert.ToByte((int)tp));
		}

		public void WriteByte(byte b)
		{
			WriteBytes(new byte[1] { b });
		}

		public void WriteBytes(byte[] data)
		{
			WriteBytes(data, 0, data.Length);
		}

		public void WriteBytes(byte[] data, int ofs, int count)
		{
			byte[] dst = new byte[count + buffer.Length];
			Buffer.BlockCopy(buffer, 0, dst, 0, buffer.Length);
			Buffer.BlockCopy(data, ofs, dst, buffer.Length, count);
			buffer = dst;
		}

		public void WriteBool(bool b)
		{
			WriteBytes(new byte[1] { (byte)(b ? 1 : 0) });
		}

		public void WriteInt(int i)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteUShort(ushort us)
		{
			byte[] bytes = BitConverter.GetBytes(us);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteShort(short s)
		{
			byte[] bytes = BitConverter.GetBytes(s);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteLong(long l)
		{
			byte[] bytes = BitConverter.GetBytes(l);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteFloat(float f)
		{
			byte[] bytes = BitConverter.GetBytes(f);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteDouble(double d)
		{
			byte[] bytes = BitConverter.GetBytes(d);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteUTF(string str)
		{
			int num = 0;
			foreach (int num2 in str)
			{
				num = ((num2 >= 1 && num2 <= 127) ? (num + 1) : ((num2 <= 2047) ? (num + 2) : (num + 3)));
			}
			if (num > 32768)
			{
				throw new FormatException("String length cannot be greater then 32768 !");
			}
			WriteUShort(Convert.ToUInt16(num));
			WriteBytes(Encoding.UTF8.GetBytes(str));
		}

		public byte ReadByte()
		{
			return buffer[position++];
		}

		public byte[] ReadBytes(int count)
		{
			byte[] array = new byte[count];
			Buffer.BlockCopy(buffer, position, array, 0, count);
			position += count;
			return array;
		}

		public bool ReadBool()
		{
			return buffer[position++] == 1;
		}

		public int ReadInt()
		{
			byte[] value = ReverseOrder(ReadBytes(4));
			return BitConverter.ToInt32(value, 0);
		}

		public ushort ReadUShort()
		{
			byte[] value = ReverseOrder(ReadBytes(2));
			return BitConverter.ToUInt16(value, 0);
		}

		public short ReadShort()
		{
			byte[] value = ReverseOrder(ReadBytes(2));
			return BitConverter.ToInt16(value, 0);
		}

		public long ReadLong()
		{
			byte[] value = ReverseOrder(ReadBytes(8));
			return BitConverter.ToInt64(value, 0);
		}

		public float ReadFloat()
		{
			byte[] value = ReverseOrder(ReadBytes(4));
			return BitConverter.ToSingle(value, 0);
		}

		public double ReadDouble()
		{
			byte[] value = ReverseOrder(ReadBytes(8));
			return BitConverter.ToDouble(value, 0);
		}

		public string ReadUTF()
		{
			ushort num = ReadUShort();
			string @string = Encoding.UTF8.GetString(buffer, position, num);
			position += num;
			return @string;
		}
	}
}
