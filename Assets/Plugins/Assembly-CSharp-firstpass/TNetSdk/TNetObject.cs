using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace TNetSdk
{
	public class TNetObject : TDispatchable
	{
		public enum STATUS
		{
			kReady = 0,
			kConnecting = 1,
			kConnected = 2,
			kClosed = 3
		}

		private class Event
		{
			public enum TYPE
			{
				kUnknown = 0,
				kConnected = 1,
				kPacked = 2,
				kConnectTimeout = 3,
				kDisconnect = 4,
				kSystemClose = 5,
				kSystemDisconnect = 6
			}

			public TYPE m_type;

			public Packet m_packet;
		}

		public const int DefaultBufferSize = 32768;

		private BlowFish m_blow_fish;

		public bool m_need_security = true;

		public bool m_need_decrpty = true;

		private STATUS m_Status;

		private Socket m_socket;

		private byte[] m_RecvDataBuffer = new byte[32768];

		private int m_iRcvLength;

		private CircleBuffer<Event> m_EventQueue = new CircleBuffer<Event>(512);

		private TEventDispatcher dispatcher;

		private TNetRoom cur_room;

		private TNetUser cur_user;

		private TNetTimeManager time_manager;

		public float heart_beat_rate = 3f;

		public float heart_beat_waiting = 10f;

		public float heart_beat_timeout = 30f;

		private float heart_beat_interval;

		private float m_reverse_heart_time;

		private bool inited;

		private bool is_wait_server;

		private bool is_time_out;

		public TEventDispatcher Dispatcher
		{
			get
			{
				return dispatcher;
			}
		}

		public TNetRoom CurRoom
		{
			get
			{
				return cur_room;
			}
			set
			{
				cur_room = value;
			}
		}

		public TNetUser Myself
		{
			get
			{
				return cur_user;
			}
			set
			{
				cur_user = value;
			}
		}

		public TNetTimeManager TimeManager
		{
			get
			{
				return time_manager;
			}
		}

		public TNetObject()
		{
			Initialize();
		}

		public STATUS GetStatus()
		{
			return m_Status;
		}

		private void Initialize()
		{
			if (dispatcher == null)
			{
				dispatcher = new TEventDispatcher(this);
			}
			if (time_manager == null)
			{
				time_manager = new TNetTimeManager(this);
			}
			inited = true;
			m_reverse_heart_time = Time.time;
			string key = KeyHolder.GetKey();
			if (key != null && key.Length > 0)
			{
				m_need_security = true;
				m_blow_fish = new BlowFish(key);
			}
			else
			{
				m_need_security = false;
			}
		}

		public void Update(float deltaTime)
		{
			Event data = new Event();
			while (m_EventQueue.read(ref data))
			{
				switch (data.m_type)
				{
				case Event.TYPE.kConnected:
					OnConnected();
					break;
				case Event.TYPE.kPacked:
					OnPacket(data.m_packet);
					break;
				case Event.TYPE.kConnectTimeout:
					OnConnectTimeout();
					break;
				case Event.TYPE.kDisconnect:
					OnClosed();
					break;
				case Event.TYPE.kSystemClose:
					SystemClose();
					break;
				case Event.TYPE.kSystemDisconnect:
					OnKilled();
					break;
				}
			}
			if (inited && m_Status == STATUS.kConnected)
			{
				heart_beat_interval += deltaTime;
				if (heart_beat_interval >= heart_beat_rate)
				{
					Debug.Log("last ping:" + (int)TimeManager.LastPing);
					Send(new HeartBeatRequest((int)TimeManager.LastPing));
					time_manager.TimeSyncRequest();
					heart_beat_interval = 0f;
				}
				if (Time.time - m_reverse_heart_time > heart_beat_timeout && !is_time_out)
				{
					dispatcher.DispatchEvent(PROTOCOLS.version, (CMD)995, null);
					is_time_out = true;
				}
				else if (Time.time - m_reverse_heart_time > heart_beat_waiting && !is_wait_server)
				{
					dispatcher.DispatchEvent(PROTOCOLS.version, (CMD)994, null);
					is_wait_server = true;
				}
			}
		}

		public bool IsContected()
		{
			if (m_socket != null)
			{
				return m_socket.Connected;
			}
			return false;
		}

		public void Connect(string ip, int port)
		{
			if (m_Status == STATUS.kReady)
			{
				m_Status = STATUS.kConnecting;
				m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint end_point = new IPEndPoint(IPAddress.Parse(ip), port);
				m_socket.BeginConnect(end_point, Connected, m_socket);
			}
		}

		protected void SendPacket(Packet packet)
		{
			if (m_Status != STATUS.kConnected)
			{
				Debug.Log("Send Error, STATUS.kConnected:" + m_Status);
				return;
			}
			try
			{
				m_socket.BeginSend(packet.ByteArray(), 0, packet.Length, SocketFlags.None, SendDataEnd, m_socket);
			}
			catch
			{
			}
		}

		public void Send(TNetRequest request)
		{
			if (request != null && request.Message != null)
			{
				if (m_need_security)
				{
					request.Message.Position = 0;
					ulong val = 0uL;
					request.Message.PopUInt64(ref val);
					m_blow_fish.Encrypt(ref val);
					request.Message.Position = 0;
					request.Message.PushUInt64(val);
				}
				SendPacket(request.Message);
			}
		}

		public void Close()
		{
			if (m_Status == STATUS.kReady)
			{
				return;
			}
			if (m_Status == STATUS.kConnected)
			{
				try
				{
					if (m_socket.Connected)
					{
						m_socket.Shutdown(SocketShutdown.Both);
					}
					m_socket.BeginDisconnect(true, DisconnectCallback, m_socket);
				}
				catch (Exception message)
				{
					Debug.Log(message);
				}
				m_Status = STATUS.kClosed;
			}
			else if (m_Status == STATUS.kConnecting)
			{
				try
				{
					m_socket.BeginDisconnect(true, DisconnectCallback, m_socket);
				}
				catch (Exception message2)
				{
					Debug.Log(message2);
				}
				m_Status = STATUS.kClosed;
			}
			else if (m_Status == STATUS.kClosed)
			{
				m_socket = null;
				m_Status = STATUS.kReady;
				inited = false;
			}
		}

		protected void DisconnectCallback(IAsyncResult ar)
		{
			Socket socket = (Socket)ar.AsyncState;
			try
			{
				socket.EndDisconnect(ar);
			}
			catch (Exception)
			{
			}
			finally
			{
				Event @event = new Event();
				@event.m_type = Event.TYPE.kDisconnect;
				m_EventQueue.write(@event);
			}
			m_socket = null;
			m_Status = STATUS.kReady;
			inited = false;
		}

		private void SystemClose()
		{
			if (m_Status != STATUS.kConnected)
			{
				return;
			}
			try
			{
				if (m_socket.Connected)
				{
				}
				m_socket.BeginDisconnect(true, SystemDisconnectCallback, m_socket);
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
			m_Status = STATUS.kClosed;
		}

		protected void SystemDisconnectCallback(IAsyncResult ar)
		{
			Socket socket = (Socket)ar.AsyncState;
			Debug.Log("SystemDisconnectCallback " + Thread.CurrentThread.GetHashCode());
			try
			{
				socket.EndDisconnect(ar);
			}
			catch (Exception)
			{
			}
			finally
			{
				Event @event = new Event();
				@event.m_type = Event.TYPE.kSystemDisconnect;
				m_EventQueue.write(@event);
			}
			m_socket = null;
			m_Status = STATUS.kReady;
			inited = false;
		}

		protected void SendDataEnd(IAsyncResult iar)
		{
			if (m_Status == STATUS.kClosed)
			{
				return;
			}
			Socket socket = (Socket)iar.AsyncState;
			try
			{
				int num = socket.EndSend(iar);
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}

		protected void Connected(IAsyncResult iar)
		{
			if (m_Status != STATUS.kConnecting)
			{
				return;
			}
			Socket socket = (Socket)iar.AsyncState;
			try
			{
				socket.EndConnect(iar);
				m_Status = STATUS.kConnected;
				socket.BeginReceive(m_RecvDataBuffer, m_iRcvLength, 32768 - m_iRcvLength, SocketFlags.None, RecvData, socket);
				Event @event = new Event();
				@event.m_type = Event.TYPE.kConnected;
				m_EventQueue.write(@event);
			}
			catch (Exception)
			{
				Event event2 = new Event();
				event2.m_type = Event.TYPE.kConnectTimeout;
				m_EventQueue.write(event2);
			}
		}

		protected void RecvData(IAsyncResult iar)
		{
			if (m_Status != STATUS.kConnected || !m_socket.Connected)
			{
				return;
			}
			try
			{
				int num = m_socket.EndReceive(iar);
				if (num > 0)
				{
					m_iRcvLength += num;
					bool flag = false;
					int num2;
					while (true)
					{
						num2 = OnCheckPacket(ref m_RecvDataBuffer, m_iRcvLength);
						if (num2 <= 0)
						{
							break;
						}
						Event @event = new Event();
						@event.m_type = Event.TYPE.kPacked;
						@event.m_packet = new Packet(m_RecvDataBuffer, num2, true);
						m_EventQueue.write(@event);
						m_iRcvLength -= num2;
						for (int i = 0; i < m_iRcvLength; i++)
						{
							m_RecvDataBuffer[i] = m_RecvDataBuffer[num2 + i];
						}
					}
					if (num2 < 0)
					{
						flag = true;
					}
					if (flag)
					{
						Event event2 = new Event();
						event2.m_type = Event.TYPE.kSystemClose;
						m_EventQueue.write(event2);
						Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!packet error!!!!!!!!!!!!!!!");
					}
					else
					{
						m_socket.BeginReceive(m_RecvDataBuffer, m_iRcvLength, 32768 - m_iRcvLength, SocketFlags.None, RecvData, null);
					}
				}
				else
				{
					Event event3 = new Event();
					event3.m_type = Event.TYPE.kSystemClose;
					m_EventQueue.write(event3);
				}
			}
			catch (Exception)
			{
				Event event4 = new Event();
				event4.m_type = Event.TYPE.kSystemClose;
				m_EventQueue.write(event4);
			}
		}

		public static ushort WatchUInt16(byte[] data, int pos)
		{
			return (ushort)((data[pos] << 8) | data[pos + 1]);
		}

		public static uint WatchUInt32(byte[] data, int pos)
		{
			return (uint)((data[pos] << 24) | (data[pos + 1] << 16) | (data[pos + 2] << 8) | data[pos + 3]);
		}

		protected virtual void OnConnected()
		{
			dispatcher.DispatchEvent(PROTOCOLS.version, (CMD)999, null);
		}

		protected virtual void OnClosed()
		{
			dispatcher.DispatchEvent(PROTOCOLS.version, (CMD)998, null);
		}

		protected virtual void OnKilled()
		{
			dispatcher.DispatchEvent(PROTOCOLS.version, (CMD)997, null);
		}

		protected virtual void OnConnectTimeout()
		{
			dispatcher.DispatchEvent(PROTOCOLS.version, (CMD)996, null);
		}

		protected virtual int OnCheckPacket(ref byte[] data, int len)
		{
			if (len < 10)
			{
				return 0;
			}
			if (m_need_security && m_need_decrpty)
			{
				uint num = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
				uint num2 = (uint)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
				ulong num3 = num;
				num3 = (num3 << 32) + num2;
				m_blow_fish.Decrypt(ref num3);
				num = (uint)(num3 >> 32);
				num2 = (uint)num3;
				data[0] = (byte)(((num & 0xFF000000u) >> 24) & 0xFFu);
				data[1] = (byte)(((num & 0xFF0000) >> 16) & 0xFFu);
				data[2] = (byte)(((num & 0xFF00) >> 8) & 0xFFu);
				data[3] = (byte)(num & 0xFFu & 0xFFu);
				data[4] = (byte)(((num2 & 0xFF000000u) >> 24) & 0xFFu);
				data[5] = (byte)(((num2 & 0xFF0000) >> 16) & 0xFFu);
				data[6] = (byte)(((num2 & 0xFF00) >> 8) & 0xFFu);
				data[7] = (byte)(num2 & 0xFFu & 0xFFu);
			}
			ushort num4 = WatchUInt16(data, 0);
			if (num4 > 4096)
			{
				return -1;
			}
			if (len < num4)
			{
				if (m_need_security)
				{
					m_need_decrpty = false;
				}
				return 0;
			}
			if (m_need_security)
			{
				m_need_decrpty = true;
			}
			return num4;
		}

		public void AddEventListener(TNetEventSystem eventType, EventListenerDelegate listener)
		{
			dispatcher.AddEventListener(eventType, listener);
		}

		public void AddEventListener(TNetEventRoom eventType, EventListenerDelegate listener)
		{
			dispatcher.AddEventListener(eventType, listener);
		}

		public void RemoveEventListener(TNetEventSystem eventType, EventListenerDelegate listener)
		{
			dispatcher.RemoveEventListener(eventType, listener);
		}

		public void RemoveEventListener(TNetEventRoom eventType, EventListenerDelegate listener)
		{
			dispatcher.RemoveEventListener(eventType, listener);
		}

		public void RemoveAllEventListeners()
		{
			dispatcher.RemoveAll();
		}

		protected void OnPacket(Packet packet)
		{
			ushort val = 0;
			if (packet.WatchUInt16(ref val, 4))
			{
				ushort val2 = 0;
				if (packet.WatchUInt16(ref val2, 6))
				{
					dispatcher.DispatchEvent((PROTOCOLS)val, (CMD)val2, packet);
				}
			}
		}

		public void OnHeartBeatProess(double timeValue)
		{
			if (time_manager != null)
			{
				time_manager.Synchronize(timeValue);
			}
			m_reverse_heart_time = Time.time;
			if (is_wait_server)
			{
				is_wait_server = false;
				dispatcher.DispatchEvent(PROTOCOLS.version, (CMD)993, null);
			}
		}

		public string GetHostAddresses(string hostNameOrAddress)
		{
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(hostNameOrAddress);
				return hostAddresses[0].ToString();
			}
			catch (Exception)
			{
				dispatcher.DispatchEvent(PROTOCOLS.version, (CMD)992, null);
				return string.Empty;
			}
		}
	}
}
