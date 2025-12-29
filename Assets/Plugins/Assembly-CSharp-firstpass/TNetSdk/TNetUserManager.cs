using System.Collections.Generic;

namespace TNetSdk
{
	public class TNetUserManager
	{
		private Dictionary<int, TNetUser> usersById;

		protected TNetRoom room;

		public int UserCount
		{
			get
			{
				return usersById.Count;
			}
		}

		public TNetUserManager(TNetRoom room)
		{
			this.room = room;
			usersById = new Dictionary<int, TNetUser>();
		}

		public bool ContainsUserId(int userId)
		{
			return usersById.ContainsKey(userId);
		}

		public bool ContainsUser(TNetUser user)
		{
			return usersById.ContainsKey(user.Id);
		}

		public TNetUser GetUserById(int userId)
		{
			if (!usersById.ContainsKey(userId))
			{
				return null;
			}
			return usersById[userId];
		}

		public virtual void AddUser(TNetUser user)
		{
			if (!usersById.ContainsKey(user.Id))
			{
				AddUserInternal(user);
			}
		}

		protected void AddUserInternal(TNetUser user)
		{
			usersById[user.Id] = user;
		}

		public virtual void RemoveUser(TNetUser user)
		{
			usersById.Remove(user.Id);
		}

		public void RemoveUserById(int id)
		{
			if (usersById.ContainsKey(id))
			{
				TNetUser user = usersById[id];
				RemoveUser(user);
			}
		}

		public List<TNetUser> GetUserList()
		{
			return new List<TNetUser>(usersById.Values);
		}

		public void ClearAll()
		{
			usersById = new Dictionary<int, TNetUser>();
		}
	}
}
