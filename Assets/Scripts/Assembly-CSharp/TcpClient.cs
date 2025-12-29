using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TcpClient : MonoBehaviour
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
			kTimeout = 3,
			kClosed = 4,
			kDisconnect = 5
		}

		public TYPE m_type;

		public Packet m_packet;
	}

	public const int DefaultBufferSize = 32768;

	private STATUS m_Status;

	private Socket m_socket;

	private byte[] m_recvDataBuffer = new byte[32768];

	private int m_iRcvLength;

	private CircleBuffer<Event> m_EventQueue = new CircleBuffer<Event>(512);

	protected uint packet_send_count;

	protected uint packet_get_count;

	public STATUS GetStatus()
	{
		return m_Status;
	}

	public virtual void Update()
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
			case Event.TYPE.kTimeout:
				OnTimeout();
				break;
			case Event.TYPE.kClosed:
				Debug.Log("!!!!!!!!!!!!!!!!!!!!!rcv a error, and onclose");
				m_Status = STATUS.kClosed;
				Close();
				OnClosed();
				break;
			case Event.TYPE.kDisconnect:
				m_Status = STATUS.kClosed;
				Close();
				OnKilled();
				break;
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
			m_socket.BeginConnect(end_point, Connected, null);
		}
	}

	public void Send(Packet packet)
	{
		if (m_Status != STATUS.kConnected)
		{
			Debug.Log("Send Error, STATUS.kConnected:" + m_Status);
			return;
		}
		try
		{
			m_socket.BeginSend(packet.ByteArray(), 0, packet.Length, SocketFlags.None, SendDataEnd, null);
		}
		catch (Exception ex)
		{
			Debug.Log("Send Error:" + ex);
		}
	}

	public void Close()
	{
		Debug.Log("close ----- ");
		if (m_Status != 0)
		{
			if (m_Status != STATUS.kClosed)
			{
				m_socket.BeginDisconnect(true, DisconnectCallback, m_socket);
				m_socket.Shutdown(SocketShutdown.Both);
			}
			else
			{
				m_socket = null;
				m_Status = STATUS.kReady;
			}
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
	}

	protected void SendDataEnd(IAsyncResult iar)
	{
		int num = m_socket.EndSend(iar);
	}

	protected void Connected(IAsyncResult iar)
	{
		try
		{
			m_socket.EndConnect(iar);
			m_Status = STATUS.kConnected;
			m_socket.BeginReceive(m_recvDataBuffer, m_iRcvLength, 32768 - m_iRcvLength, SocketFlags.None, RecvData, null);
			Event @event = new Event();
			@event.m_type = Event.TYPE.kConnected;
			m_EventQueue.write(@event);
		}
		catch (Exception)
		{
			Event event2 = new Event();
			event2.m_type = Event.TYPE.kTimeout;
			m_EventQueue.write(event2);
		}
	}

	protected void RecvData(IAsyncResult iar)
	{
		try
		{
			int num = m_socket.EndReceive(iar);
			if (num > 0)
			{
				m_iRcvLength += num;
				while (true)
				{
					int num2 = OnCheckPacket(m_recvDataBuffer, m_iRcvLength);
					if (num2 > 0)
					{
						Event @event = new Event();
						@event.m_type = Event.TYPE.kPacked;
						@event.m_packet = new Packet(m_recvDataBuffer, num2, true);
						m_EventQueue.write(@event);
						m_iRcvLength -= num2;
						for (int i = 0; i < m_iRcvLength; i++)
						{
							m_recvDataBuffer[i] = m_recvDataBuffer[num2 + i];
						}
						continue;
					}
					break;
				}
				m_socket.BeginReceive(m_recvDataBuffer, m_iRcvLength, 32768 - m_iRcvLength, SocketFlags.None, RecvData, null);
			}
			else
			{
				Event event2 = new Event();
				event2.m_type = Event.TYPE.kClosed;
				m_EventQueue.write(event2);
			}
		}
		catch (Exception)
		{
			Event event3 = new Event();
			event3.m_type = Event.TYPE.kClosed;
			m_EventQueue.write(event3);
		}
	}

	public static uint WatchUInt32(byte[] data, int pos)
	{
		return (uint)((data[pos] << 24) | (data[pos + 1] << 16) | (data[pos + 2] << 8) | data[pos + 3]);
	}

	public virtual void OnConnected()
	{
	}

	public virtual void OnPacket(Packet packet)
	{
	}

	public virtual void OnClosed()
	{
	}

	public virtual void OnKilled()
	{
	}

	public virtual void OnTimeout()
	{
	}

	public virtual int OnCheckPacket(byte[] data, int len)
	{
		if (len < 12)
		{
			return 0;
		}
		uint num = WatchUInt32(data, 0);
		if (len < num)
		{
			return 0;
		}
		return (int)num;
	}
}
