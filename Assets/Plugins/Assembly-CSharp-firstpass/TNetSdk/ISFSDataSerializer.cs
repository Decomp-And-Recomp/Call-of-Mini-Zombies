namespace TNetSdk
{
	public interface ISFSDataSerializer
	{
		ByteArray Object2Binary(ISFSObject obj);

		ByteArray Array2Binary(ISFSArray array);

		ISFSObject Binary2Object(ByteArray data);

		ISFSArray Binary2Array(ByteArray data);
	}
}
