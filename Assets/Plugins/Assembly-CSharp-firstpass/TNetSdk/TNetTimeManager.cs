using UnityEngine;

namespace TNetSdk
{
	public class TNetTimeManager
	{
		private float timeBeforeSync;

		private bool synchronized;

		private double lastServerTime;

		private double lastLocalTime;

		private double averagePing;

		private double min_Ping = 9999.0;

		private double last_Ping = -1.0;

		private int pingCount;

		private readonly int averagePingCount = 10;

		private double[] pingValues;

		private int pingValueIndex;

		private TNetObject target;

		public double NetworkTime
		{
			get
			{
				return ((double)Time.time - lastLocalTime) * 1000.0 + lastServerTime;
			}
		}

		public double AveragePing
		{
			get
			{
				return averagePing;
			}
		}

		public double MinPing
		{
			get
			{
				return min_Ping / 2.0;
			}
		}

		public double LastPing
		{
			get
			{
				return last_Ping;
			}
		}

		public TNetTimeManager(TNetObject target)
		{
			Init(target);
		}

		private void Init(TNetObject target)
		{
			this.target = target;
			pingValues = new double[averagePingCount];
			pingCount = 0;
			pingValueIndex = 0;
		}

		public void Synchronize(double timeValue)
		{
			double num = (Time.time - timeBeforeSync) * 1000f;
			CalculateAveragePing(num);
			last_Ping = num;
			if (num < min_Ping)
			{
				min_Ping = num;
				double num2 = num / 2.0;
				lastServerTime = timeValue + num2;
				lastLocalTime = Time.time;
			}
			if (!synchronized)
			{
				synchronized = true;
			}
		}

		public void TimeSyncRequest()
		{
			timeBeforeSync = Time.time;
		}

		public bool IsSynchronized()
		{
			return synchronized;
		}

		private void CalculateAveragePing(double ping)
		{
			pingValues[pingValueIndex] = ping;
			pingValueIndex++;
			if (pingValueIndex >= averagePingCount)
			{
				pingValueIndex = 0;
			}
			if (pingCount < averagePingCount)
			{
				pingCount++;
			}
			double num = 0.0;
			for (int i = 0; i < pingCount; i++)
			{
				num += pingValues[i];
			}
			averagePing = num / (double)pingCount;
		}
	}
}
