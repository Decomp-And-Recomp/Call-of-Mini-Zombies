using System.Collections.Generic;

namespace TNetSdk
{
	public class TNetRoom
	{
		protected int id;

		protected string name;

		protected int groupId;

		protected bool isJoined;

		protected bool isPasswordProtected;

		protected bool isGaming;

		protected string creater_name;

		protected string comment;

		protected int room_master_id;

		protected TNetUser room_master;

		protected TNetUserManager userManager;

		protected Dictionary<TNetRoomVarType, SFSObject> variables = new Dictionary<TNetRoomVarType, SFSObject>();

		protected int maxUsers;

		protected int userCount;

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
			set
			{
				name = value;
			}
		}

		public string CreaterName
		{
			get
			{
				return creater_name;
			}
			set
			{
				creater_name = value;
			}
		}

		public int GroupId
		{
			get
			{
				return groupId;
			}
		}

		public bool IsJoined
		{
			get
			{
				return isJoined;
			}
			set
			{
				isJoined = value;
			}
		}

		public bool IsGaming
		{
			get
			{
				return isGaming;
			}
			set
			{
				isGaming = value;
			}
		}

		public bool IsPasswordProtected
		{
			get
			{
				return isPasswordProtected;
			}
			set
			{
				isPasswordProtected = value;
			}
		}

		public TNetUser RoomMaster
		{
			get
			{
				return room_master;
			}
			set
			{
				room_master = value;
			}
		}

		public int RoomMasterID
		{
			get
			{
				return room_master_id;
			}
			set
			{
				room_master_id = value;
			}
		}

		public int UserCount
		{
			get
			{
				if (isJoined)
				{
					return userManager.UserCount;
				}
				return userCount;
			}
			set
			{
				userCount = value;
			}
		}

		public int MaxUsers
		{
			get
			{
				return maxUsers;
			}
			set
			{
				maxUsers = value;
			}
		}

		public string Commnet
		{
			get
			{
				return comment;
			}
			set
			{
				comment = value;
			}
		}

		public List<TNetUser> UserList
		{
			get
			{
				return userManager.GetUserList();
			}
		}

		public TNetRoom(int id, string name)
		{
			Init(id, name, 0);
		}

		public TNetRoom(int id, string name, int groupId)
		{
			Init(id, name, groupId);
		}

		public static TNetRoom FromRoomInfo(RoomDragListResCmd.RoomInfo info)
		{
			TNetRoom tNetRoom = new TNetRoom(info.m_room_id, info.m_room_name, info.m_group_id);
			tNetRoom.IsPasswordProtected = ((info.m_passworded != 0) ? true : false);
			tNetRoom.UserCount = info.m_online_user;
			tNetRoom.MaxUsers = info.m_max_user;
			tNetRoom.isGaming = info.m_state == 1;
			tNetRoom.CreaterName = info.m_creater_name;
			tNetRoom.room_master_id = info.m_master_id;
			tNetRoom.room_master = null;
			tNetRoom.Commnet = info.m_comment;
			return tNetRoom;
		}

		private void Init(int id, string name, int groupId)
		{
			this.id = id;
			this.name = name;
			this.groupId = groupId;
			isJoined = false;
			userCount = 0;
			variables = new Dictionary<TNetRoomVarType, SFSObject>();
			userManager = new TNetUserManager(this);
		}

		public Dictionary<TNetRoomVarType, SFSObject> GetVariables()
		{
			return variables;
		}

		public SFSObject GetVariable(TNetRoomVarType name)
		{
			if (!variables.ContainsKey(name))
			{
				return null;
			}
			return variables[name];
		}

		public TNetUser GetUserById(int id)
		{
			return userManager.GetUserById(id);
		}

		public void RemoveUser(TNetUser user)
		{
			userManager.RemoveUser(user);
		}

		public void SetVariable(TNetRoomVarType name, SFSObject userVariable)
		{
			variables[name] = userVariable;
		}

		public bool ContainsVariable(TNetRoomVarType name)
		{
			return variables.ContainsKey(name);
		}

		private void RemoveUserVariable(TNetRoomVarType varName)
		{
			variables.Remove(varName);
		}

		public void AddUser(TNetUser user)
		{
			userManager.AddUser(user);
		}

		public bool ContainsUser(TNetUser user)
		{
			return userManager.ContainsUser(user);
		}

		public override string ToString()
		{
			return "[Room: " + name + ", Id: " + id + ", GroupId: " + groupId + "]";
		}

		public void Joined()
		{
			IsJoined = true;
		}
	}
}
