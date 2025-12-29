using UnityEngine;

public class IapItemData : MonoBehaviour
{
	public IAPName iapName;

	private IAPItem iap_item;

	public IAPItem GetIapItem()
	{
		return iap_item;
	}

	private void Start()
	{
		iap_item = new IAPItem();
		switch (iapName)
		{
		case IAPName.Cash1:
			iap_item.ID = "com.trinitigame.callofminizombies.199centsv40b";
			iap_item.Name = IAPName.Cash1;
			iap_item.Desc = "120,000 COIN PACK";
			break;
		case IAPName.Cash2:
			iap_item.ID = "com.trinitigame.callofminizombies.499centsv40b";
			iap_item.Name = IAPName.Cash2;
			iap_item.Desc = "330,000 COIN PACK";
			break;
		case IAPName.Cash3:
			iap_item.ID = "com.trinitigame.callofminizombies.999centsv40b";
			iap_item.Name = IAPName.Cash3;
			iap_item.Desc = "760,000 COIN PACK";
			break;
		case IAPName.Cash4:
			iap_item.ID = "com.trinitigame.callofminizombies.1999centsv40b";
			iap_item.Name = IAPName.Cash4;
			iap_item.Desc = "1,500,000 COIN PACK";
			break;
		case IAPName.Cash5:
			iap_item.ID = "com.trinitigame.callofminizombies.4999centsv40b";
			iap_item.Name = IAPName.Cash5;
			iap_item.Desc = "4,200,000 COIN PACK";
			break;
		case IAPName.Cash6:
			iap_item.ID = "com.trinitigame.callofminizombies.9999centsv43b";
			iap_item.Name = IAPName.Cash6;
			iap_item.Desc = "9,600,000 COIN PACK";
			break;
		case IAPName.Crystal1:
			iap_item.ID = "com.trinitigame.callofminizombies.199centsv40";
			iap_item.Name = IAPName.Crystal1;
			iap_item.Desc = "50 tCRYSTAL PACK";
			break;
		case IAPName.Crystal2:
			iap_item.ID = "com.trinitigame.callofminizombies.499centsv40";
			iap_item.Name = IAPName.Crystal2;
			iap_item.Desc = "150 tCRYSTAL PACK";
			break;
		case IAPName.Crystal3:
			iap_item.ID = "com.trinitigame.callofminizombies.999centsv40";
			iap_item.Name = IAPName.Crystal3;
			iap_item.Desc = "350 tCRYSTAL PACK";
			break;
		case IAPName.Crystal4:
			iap_item.ID = "com.trinitigame.callofminizombies.1999centsv40";
			iap_item.Name = IAPName.Crystal4;
			iap_item.Desc = "800 tCRYSTAL PACK";
			break;
		case IAPName.Crystal5:
			iap_item.ID = "com.trinitigame.callofminizombies.4999centsv40";
			iap_item.Name = IAPName.Crystal5;
			iap_item.Desc = "2500 tCRYSTAL PACK";
			break;
		case IAPName.Crystal6:
			iap_item.ID = "com.trinitigame.callofminizombies.9999centsv43";
			iap_item.Name = IAPName.Crystal6;
			iap_item.Desc = "6200 tCRYSTAL PACK";
			break;
		case IAPName.Exp15:
			iap_item.ID = "com.trinitigame.callofminizombies.099centsv40b";
			iap_item.Name = IAPName.Exp15;
			iap_item.Desc = "1.5x EXP CARD";
			break;
		case IAPName.Exp25:
			iap_item.ID = "com.trinitigame.callofminizombies.099centsv40c";
			iap_item.Name = IAPName.Exp25;
			iap_item.Desc = "2.5x EXP CARD";
			break;
		case IAPName.Newbie1:
			iap_item.ID = "com.trinitigame.callofminizombies.099centsv40e";
			iap_item.Name = IAPName.Newbie1;
			iap_item.Desc = "$0.99 NEWBIE PACK";
			break;
		case IAPName.Newbie2:
			iap_item.ID = "com.trinitigame.callofminizombies.599centsv40";
			iap_item.Name = IAPName.Newbie2;
			iap_item.Desc = "$5.99 NEWBIE PACK";
			break;
		case IAPName.PaperJoy:
		case IAPName.PaperJoyRestore:
		case IAPName.IapDiscountActive:
		case IAPName.IapDiscountInactive1:
		case IAPName.IapDiscountInactive2:
		case IAPName.IapDiscountInactive3:
			break;
		}
	}
}
