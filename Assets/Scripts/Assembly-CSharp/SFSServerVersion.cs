using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class SFSServerVersion : MonoBehaviour
{
	public enum State { Failed, Done }

	public static State stateCoop = State.Failed;
	public static State stateVs = State.Failed;

	public string url = "http://account.trinitigame.com/game/callofminizombies/CoMZombies_VS_version.bytes";

	protected string content = string.Empty;

	public OnSFSServerVersion callback;

	public OnSFSServerVersionError callback_error;

	public static string VsDomain = string.Empty;

	public string VsStandbyServerIP = string.Empty;

	public static int VsServerPort;

	public int VsGroupIdMin;

	public int VsGroupIdMax;

	public static string CoopDomain = string.Empty;

	public string CoopStandbyServerIP = string.Empty;

	public static int CoopServerPort;

	public int CoopMapIdMin;

	public static string versionCoop, versionVs;

	private IEnumerator Start()
	{
		VSHallTUI.groupIdMax = 100000;

        if (Application.loadedLevelName == "CoopHallTUI")
		{
			yield return CoopRequest();

			if (stateCoop == State.Failed)
			{
				callback_error();
				Debug.LogError("Failed Coop");
				yield break;
			}

			if (PrivateData.Version != versionCoop)
			{
				callback(false);
				Debug.LogError("Version mismatch Coop");
				yield break;
			}
		}
		else
		{
			yield return VsRequest();

			if (stateVs == State.Failed)
			{
				callback_error();
				Debug.LogError("Failed Vs");
				yield break;
			}

			if (PrivateData.Version != versionVs)
			{
				callback(false);
				Debug.LogError("Version mismatch Vs");
				yield break;
			}
		}

		callback(true);
		yield break;
	}
	IEnumerator CoopRequest()
	{
		if (stateCoop == State.Done) yield break;

		UnityWebRequest request = new UnityWebRequest(PrivateData.BossRaidDataAddress, "POST");

		byte[] requestData = Encoding.UTF8.GetBytes(T.External.XXTEAUtils.Encrypt("CoMZBossRaid", PrivateData.ServerEncryptionKey));

		request.uploadHandler = new UploadHandlerRaw(requestData);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "text/plain");

		yield return request.SendWebRequest();

		if (request.isNetworkError || request.isHttpError)
		{
			Debug.LogError("HTTP ERROR CODE: " + request.responseCode);
			Debug.LogError("HTTP ERROR: " + request.error);
			stateCoop = State.Failed;
			yield break;
		}

		string result = request.downloadHandler.text;

		if (!string.IsNullOrEmpty(result)) result = Regex.Unescape(T.External.XXTEAUtils.Decrypt(result, PrivateData.ServerEncryptionKey));

		string[] data = result.Split('|');

		if (data.Length < 3)
		{
			stateCoop = State.Failed;
			Debug.LogError("Coop response too low");
			yield break;
		}

		CoopDomain = data[0];
		
		if (!int.TryParse(data[1], out CoopServerPort))
		{
			stateCoop = State.Failed;
			Debug.LogError("Cant parse Coop port");
			yield break;
		}

		versionCoop = data[2];

		stateCoop = State.Done;
	}

	IEnumerator VsRequest()
	{
		if (stateVs == State.Done) yield break;

		UnityWebRequest request = new UnityWebRequest(PrivateData.VsDataAddress, "POST");

		byte[] requestData = Encoding.UTF8.GetBytes(T.External.XXTEAUtils.Encrypt("CoMZVS", PrivateData.ServerEncryptionKey));

		request.uploadHandler = new UploadHandlerRaw(requestData);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "text/plain");

		yield return request.SendWebRequest();

		if (request.isNetworkError || request.isHttpError)
		{
			Debug.LogError("HTTP ERROR CODE: " + request.responseCode);
			Debug.LogError("HTTP ERROR: " + request.error);
			stateVs = State.Failed;
			yield break;
		}

		string result = request.downloadHandler.text;

		if (!string.IsNullOrEmpty(result)) result = Regex.Unescape(T.External.XXTEAUtils.Decrypt(result, PrivateData.ServerEncryptionKey));

		string[] data = result.Split('|');

		if (data.Length < 3)
		{
			stateVs = State.Failed;
			Debug.LogError("Vs response too low");
			yield break;
		}

		VsDomain = data[0];

		if (!int.TryParse(data[1], out VsServerPort))
		{
			stateVs = State.Failed;
			Debug.LogError("Cant parse Vs port");
			yield break;
		}

		versionVs = data[2];

		stateVs = State.Done;
	}


	public void FileWrite(string FileName, string WriteString)
	{
		FileStream fileStream = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		streamWriter.WriteLine(WriteString);
		streamWriter.Flush();
		streamWriter.Close();
		fileStream.Close();
	}
}
