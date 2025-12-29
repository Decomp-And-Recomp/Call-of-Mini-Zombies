using System;
using UnityEngine;

public class IapDiscountManager
{
	public enum DiscountType
	{
		None = 0,
		Active = 1,
		Inactive1 = 2,
		Inactive2 = 3,
		Inactive3 = 4
	}

	public class DiscountInfo
	{
		public DiscountType type;

		public DateTime startTime;

		public bool beginTimer;
	}

	public DiscountInfo discountInfo;

	public bool gotActiveDiscount;

	public void Init()
	{
		CheckPushIapDiscount();
	}

	public void CheckPushIapDiscount()
	{
	}

	public void CheckActiveDiscount(DateTime lastTime)
	{
		if (!gotActiveDiscount && DateTime.Now.CompareTo(lastTime.AddDays(1.0)) > 0 && DateTime.Now.CompareTo(lastTime.AddDays(2.0)) < 0)
		{
			discountInfo = new DiscountInfo();
			discountInfo.type = DiscountType.Active;
			discountInfo.startTime = DateTime.MinValue;
			discountInfo.beginTimer = false;
		}
	}

	public void CheckFinalDiscount(DiscountInfo tem_discount)
	{
		if (discountInfo == null && DateTime.MinValue.CompareTo(tem_discount.startTime) < 0 && DateTime.Now.CompareTo(tem_discount.startTime.AddMinutes(10.0)) < 0)
		{
			discountInfo = new DiscountInfo();
			discountInfo.type = tem_discount.type;
			discountInfo.startTime = tem_discount.startTime;
			discountInfo.beginTimer = true;
		}
		if (discountInfo == null)
		{
			Debug.Log("no discount");
			return;
		}
		Debug.Log("get current discount :" + discountInfo.type.ToString() + "start time: " + discountInfo.startTime.ToString() + "  " + discountInfo.beginTimer);
	}

	public bool CheckSystemTimeValid()
	{
		if (DateTime.MinValue.CompareTo(discountInfo.startTime) == 0)
		{
			return false;
		}
		if (DateTime.Now.CompareTo(discountInfo.startTime) < 0)
		{
			discountInfo = null;
			return false;
		}
		if (DateTime.Now.CompareTo(discountInfo.startTime.AddMinutes(10.0)) >= 0)
		{
			discountInfo = null;
			return false;
		}
		return true;
	}

	public void SucceedBuyDiscount()
	{
		discountInfo = null;
	}
}
