using System.Collections.Generic;

namespace TNetSdk
{
	public class BufferWriter
	{
		private List<byte> m_data;

		public void SetData(List<byte> data)
		{
			m_data = data;
		}

		public void PushByte(byte b)
		{
			m_data.Add(b);
		}

		public void PushUInt16(ushort Num)
		{
			byte item = (byte)((uint)((Num & 0xFF00) >> 8) & 0xFFu);
			byte item2 = (byte)(Num & 0xFFu & 0xFFu);
			m_data.Add(item);
			m_data.Add(item2);
		}

		public void PushUInt32(uint Num)
		{
			byte item = (byte)(((Num & 0xFF000000u) >> 24) & 0xFFu);
			byte item2 = (byte)(((Num & 0xFF0000) >> 16) & 0xFFu);
			byte item3 = (byte)(((Num & 0xFF00) >> 8) & 0xFFu);
			byte item4 = (byte)(Num & 0xFFu & 0xFFu);
			m_data.Add(item);
			m_data.Add(item2);
			m_data.Add(item3);
			m_data.Add(item4);
		}

		public void PushUInt64(ulong Num)
		{
			uint num = (uint)(Num >> 32);
			uint num2 = (uint)Num;
			PushUInt32(num);
			PushUInt32(num2);
		}

		public void PushByteArray(byte[] buf, int length)
		{
			for (int i = 0; i < length; i++)
			{
				m_data.Add(buf[i]);
			}
		}

		public byte[] ToByteArray()
		{
			return m_data.ToArray();
		}
	}
}
