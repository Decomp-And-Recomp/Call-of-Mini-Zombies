using System;

namespace TNetSdk
{
	public class BufferReader
	{
		protected byte[] m_data;

		protected int m_offset;

		public int Offset
		{
			get
			{
				return m_offset;
			}
			set
			{
				m_offset = value;
			}
		}

		public void SetData(byte[] data)
		{
			m_data = data;
			m_offset = 0;
		}

		public byte[] ByteArray()
		{
			return m_data;
		}

		public bool CheckBytesLeft(int left)
		{
			if (m_data.Length - m_offset < left)
			{
				return false;
			}
			return true;
		}

		public bool PopByte(ref byte val)
		{
			if (!CheckBytesLeft(1))
			{
				return false;
			}
			val = m_data[m_offset++];
			return true;
		}

		public bool PopUInt16(ref ushort val)
		{
			if (!CheckBytesLeft(2))
			{
				return false;
			}
			val = (ushort)((m_data[m_offset] << 8) | m_data[m_offset + 1]);
			m_offset += 2;
			return true;
		}

		public bool PopUInt32(ref uint val)
		{
			if (!CheckBytesLeft(4))
			{
				return false;
			}
			val = (uint)((m_data[m_offset] << 24) | (m_data[m_offset + 1] << 16) | (m_data[m_offset + 2] << 8) | m_data[m_offset + 3]);
			m_offset += 4;
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
			Array.Copy(m_data, m_offset, val, 0, len);
			m_offset += len;
			return true;
		}
	}
}
