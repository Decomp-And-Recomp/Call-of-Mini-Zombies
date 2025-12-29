using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TNetSdk.BinaryProtocol
{
	public class Packer : BufferWriter
	{
		private const int COMPRESS_SIZE = 256;

		private List<byte> m_data = new List<byte>();

		public Packer()
		{
			SetData(m_data);
		}

		public Packet MakePacket(ushort protocol, ushort cmd, bool allow_compress = true)
		{
			byte[] array = m_data.ToArray();
			ushort sCompressType = 0;
			if (allow_compress && array.Length >= 256)
			{
				MemoryStream memoryStream = new MemoryStream();
				DeflaterOutputStream deflaterOutputStream = new DeflaterOutputStream(memoryStream);
				deflaterOutputStream.Write(array, 0, array.Length);
				deflaterOutputStream.Close();
				array = memoryStream.ToArray();
				sCompressType = 1;
			}
			int num = 10 + array.Length;
			if (Packet.LengthIsVaild(num))
			{
				Header header = new Header();
				header.m_sLength = (ushort)num;
				header.m_sVersion = 1;
				header.m_sProtocol = protocol;
				header.m_sCmd = cmd;
				header.m_sCompressType = sCompressType;
				Packet packet = new Packet(num);
				packet.PushUInt16(header.m_sLength);
				packet.PushUInt16(header.m_sVersion);
				packet.PushUInt16(header.m_sProtocol);
				packet.PushUInt16(header.m_sCmd);
				packet.PushUInt16(header.m_sCompressType);
				packet.PushByteArray(array, array.Length);
				return packet;
			}
			return null;
		}
	}
}
