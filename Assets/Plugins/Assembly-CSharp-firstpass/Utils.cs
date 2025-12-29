using System;
using System.IO;
using UnityEngine;

public class Utils
{
	private static string m_SavePath;

	static Utils()
	{
		string dataPath = Application.dataPath;
		dataPath = Application.persistentDataPath + "/";
		if (!Directory.Exists(dataPath))
		{
			Directory.CreateDirectory(dataPath);
		}
		m_SavePath = dataPath;
	}

	public static bool CreateDocumentSubDir(string dirname)
	{
		string path = m_SavePath + "/" + dirname;
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
			return true;
		}
		return false;
	}

	public static void DeleteDocumentDir(string dirname)
	{
		string path = m_SavePath + "/" + dirname;
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}
	}

	public static string SavePath()
	{
		return m_SavePath;
	}

	public static string GetTextAsset(string txt_name)
	{
		TextAsset textAsset = Resources.Load(txt_name) as TextAsset;
		if (null != textAsset)
		{
			return textAsset.text;
		}
		return string.Empty;
	}

	public static void FileSaveString(string name, string content)
	{
		string text = SavePath() + "/" + name;
		try
		{
			FileStream fileStream = new FileStream(text, FileMode.Create);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(content);
			streamWriter.Close();
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Save" + text + " error");
		}
	}

	public static void FileGetString(string name, ref string content)
	{
		string text = SavePath() + "/" + name;
		if (!File.Exists(text))
		{
			return;
		}
		try
		{
			FileStream fileStream = new FileStream(text, FileMode.Open);
			StreamReader streamReader = new StreamReader(fileStream);
			content = streamReader.ReadToEnd();
			streamReader.Close();
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Load" + text + " error");
		}
	}

	public static string SaveBinaryData(string name, byte[] data)
	{
		string text = SavePath() + "/" + name;
		FileStream fileStream = new FileStream(text, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(data);
		binaryWriter.Close();
		fileStream.Close();
		return text;
	}

	public static bool IsChineseLetter(string input)
	{
		for (int i = 0; i < input.Length; i++)
		{
			int num = Convert.ToInt32(Convert.ToChar(input.Substring(i, 1)));
			if (num >= 128)
			{
				return true;
			}
		}
		return false;
	}

	public static string GetMacAddr()
	{
		return "000000000000";
	}

	public static void ToSendMail(string address, string subject, string content)
	{
	}

	public static void AvataTakePhotoWin32(string photo_key)
	{
		try
		{
			Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			texture2D.Apply();
			byte[] buffer = texture2D.EncodeToPNG();
			string path = SavePath() + "/Avatar/" + photo_key + "_photo.png";
			FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(buffer);
			binaryWriter.Close();
			fileStream.Close();
			UnityEngine.Object.Destroy(texture2D);
		}
		catch
		{
			Debug.Log("AvataTakePhotoWin32 error");
		}
	}

	public static int ShowMessageBox1(string title, string message, string button)
	{
		return 0;
	}

	public static int ShowMessageBox2(string title, string message, string button1, string button2)
	{
		return 0;
	}

	public static void SavePhoto(int photo_index, int width, int height)
	{
	}

	public static long GetSystemSecond()
	{
		TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
		return (long)timeSpan.TotalSeconds;
	}

	public static int OnCheckPhotoSaveStatus()
	{
		return 1;
	}

	public static void OnResetPhotoSaveStatus()
	{
	}

	public static void OpenLocalCameraRoll()
	{
	}

	public static void ShowIndicatorSystem(int style, int device_type, float r, float g, float b, float a)
	{
	}

	public static void HideIndicatorSystem()
	{
	}

	public static int GetIOSYear()
	{
		return DateTime.Now.Year;
	}

	public static int GetIOSMonth()
	{
		return DateTime.Now.Month;
	}

	public static int GetIOSDay()
	{
		return DateTime.Now.Day;
	}

	public static int GetIOSHour()
	{
		return DateTime.Now.Hour;
	}

	public static int GetIOSMin()
	{
		return DateTime.Now.Minute;
	}

	public static int GetIOSSec()
	{
		return DateTime.Now.Second;
	}

	public static bool IsJailbreak()
	{
		return true;
	}

	public static bool IsIAPCrack()
	{
		return false;
	}

	public static int GetAppAcctiveTimes()
	{
		return 0;
	}
}
