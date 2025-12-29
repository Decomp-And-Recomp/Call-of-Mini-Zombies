namespace TNetSdk
{
	public class SetRoomVariableRequest : TNetRequest
	{
		public SetRoomVariableRequest(TNetRoomVarType key, SFSObject value)
			: base(RequestType.SetRoomVariables)
		{
			Init(key, value);
		}

		private void Init(TNetRoomVarType key, SFSObject value)
		{
			packer = new RoomSetVarCmd((ushort)key, value.ToBinary().Bytes);
			packet = ((RoomSetVarCmd)packer).MakePacket();
		}
	}
}
