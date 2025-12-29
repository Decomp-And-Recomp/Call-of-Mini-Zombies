using System.Collections.Generic;

namespace TNetSdk
{
	public class TNetUser
	{
		protected int id = -1;

		protected string name;

		protected int sit_index = -1;

		protected bool isItMe;

		protected Dictionary<TNetUserVarType, SFSObject> variables;

		public int Id
		{
			get
			{
				return id;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public int SitIndex
		{
			get
			{
				return sit_index;
			}
		}

		public bool IsItMe
		{
			get
			{
				return isItMe;
			}
		}

		public TNetUser(int id, string name)
		{
			Init(id, name, false);
		}

		public TNetUser(int id, string name, bool isItMe)
		{
			Init(id, name, isItMe);
		}

		private void Init(int id, string name, bool isItMe)
		{
			this.id = id;
			this.name = name;
			sit_index = -1;
			this.isItMe = isItMe;
			variables = new Dictionary<TNetUserVarType, SFSObject>();
		}

		public bool IsJoinedInRoom(TNetRoom room)
		{
			return room.ContainsUser(this);
		}

		public Dictionary<TNetUserVarType, SFSObject> GetVariables()
		{
			return variables;
		}

		public SFSObject GetVariable(TNetUserVarType name)
		{
			if (!variables.ContainsKey(name))
			{
				return null;
			}
			return variables[name];
		}

		public void SetVariable(TNetUserVarType name, SFSObject userVariable)
		{
			variables[name] = userVariable;
		}

		public bool ContainsVariable(TNetUserVarType name)
		{
			return variables.ContainsKey(name);
		}

		private void RemoveUserVariable(TNetUserVarType varName)
		{
			variables.Remove(varName);
		}

		public void SetIndex(int index)
		{
			sit_index = index;
		}

		public override string ToString()
		{
			return "[User: " + name + ", Id: " + id + ", isMe: " + isItMe + "]";
		}
	}
}
