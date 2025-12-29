namespace TNetSdk
{
	public class CircleBuffer<T>
	{
		private T[] m_datas;

		private int m_read_index;

		private int m_write_index;

		public CircleBuffer(int size)
		{
			m_datas = new T[size];
			Clear();
		}

		public void Clear()
		{
			m_datas.Initialize();
			m_read_index = 0;
			m_write_index = 0;
		}

		public int Count()
		{
			if (m_read_index < m_write_index)
			{
				return m_write_index - m_read_index;
			}
			if (m_read_index > m_write_index)
			{
				return m_datas.Length - (m_read_index - m_write_index);
			}
			return 0;
		}

		public bool read(ref T data)
		{
			if (m_read_index == m_write_index)
			{
				return false;
			}
			data = m_datas[m_read_index];
			m_datas[m_read_index] = default(T);
			if (m_read_index < m_datas.Length - 1)
			{
				m_read_index++;
			}
			else
			{
				m_read_index = 0;
			}
			return true;
		}

		public bool write(T data)
		{
			if (m_read_index == 0)
			{
				if (m_write_index == m_datas.Length - 1)
				{
					return false;
				}
			}
			else if (m_write_index == m_read_index - 1)
			{
				return false;
			}
			m_datas[m_write_index] = data;
			if (m_write_index < m_datas.Length - 1)
			{
				m_write_index++;
			}
			else
			{
				m_write_index = 0;
			}
			return true;
		}
	}
}
