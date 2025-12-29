using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TNetSdk.BinaryProtocol
{
	public class UnPacker : BufferReader
	{
		public virtual bool ParserPacket(Packet packet)
		{
			SetData(packet.ByteArray());
			Header header = new Header();
			if (!PopUInt16(ref header.m_sLength))
			{
				return false;
			}
			if (!PopUInt16(ref header.m_sVersion))
			{
				return false;
			}
			if (!PopUInt16(ref header.m_sProtocol))
			{
				return false;
			}
			if (!PopUInt16(ref header.m_sCmd))
			{
				return false;
			}
			if (!PopUInt16(ref header.m_sCompressType))
			{
				return false;
			}
			if (header.m_sCompressType == 1)
			{
				InflaterInputStream inflaterInputStream = new InflaterInputStream(new MemoryStream(m_data, m_offset, m_data.Length - m_offset));
				MemoryStream memoryStream = new MemoryStream();
				int num = 0;
				byte[] array = new byte[4096];
				while ((num = inflaterInputStream.Read(array, 0, array.Length)) != 0)
				{
					memoryStream.Write(array, 0, num);
				}
				SetData(memoryStream.ToArray());
			}
			return true;
		}

		public virtual void ToTNetEventData(Packet packet, ref TNetEventData data, TNetObject target)
		{
		}
	}
}
