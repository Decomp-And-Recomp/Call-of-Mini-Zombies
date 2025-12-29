namespace TNetSdk
{
	public class SetUserVariableRequest : TNetRequest
	{
		public SetUserVariableRequest(TNetUserVarType key, SFSObject value)
			: base(RequestType.SetUserVariables)
		{
			Init(key, value);
		}

		private void Init(TNetUserVarType key, SFSObject value)
		{
			packer = new RoomSetUserVarCmd((ushort)key, value.ToBinary().Bytes);
			packet = ((RoomSetUserVarCmd)packer).MakePacket();
		}
	}
}
