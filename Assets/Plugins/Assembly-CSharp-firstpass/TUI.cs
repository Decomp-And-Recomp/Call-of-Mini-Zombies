using UnityEngine;

[ExecuteInEditMode]
public class TUI : MonoBehaviour
{
	public bool landscape;

	public int layer;

	public int depth;

	private TUICamera tuiCamera;

	private TUIControlManager tuiControlManager;

	private TUIMeshManager tuiMeshManager;

	public void Awake()
	{
		base.transform.position = new Vector3((int)base.transform.position.x, (int)base.transform.position.y, (int)base.transform.position.z);
		base.transform.rotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		tuiCamera = GetModule<TUICamera>("TUICamera");
		tuiControlManager = GetModule<TUIControlManager>("TUIControl");
		tuiMeshManager = GetModule<TUIMeshManager>("TUIControl");
	}

	public void Start()
	{
		tuiCamera.Initialize(landscape, layer, depth);
		tuiControlManager.Initialize();
		tuiMeshManager.Initialize(layer);
	}

	public static TUI Instance(string name)
	{
		return GameObject.Find(name).GetComponent<TUI>();
	}

	public void SetHandler(TUIHandler handler)
	{
		tuiControlManager.SetHandler(handler);
	}

	public bool HandleInput(TUIInput input)
	{
		Vector3 position = new Vector3(input.position.x, input.position.y, tuiCamera.GetComponent<Camera>().nearClipPlane);
		Vector3 vector = tuiCamera.GetComponent<Camera>().ScreenToWorldPoint(position);
		input.position.x = vector.x;
		input.position.y = vector.y;
		return tuiControlManager.HandleInput(input);
	}

	public bool HandleKeyboard(KeyCode key, int x, int y)
	{
		if (Input.GetKeyDown(key) || Input.GetKeyUp(key))
		{
			TUIInput input = default(TUIInput);
			input.fingerId = (int)key;
			input.inputType = ((!Input.GetKeyDown(key)) ? TUIInputType.Ended : TUIInputType.Began);
			Vector3 position = new Vector3(x, y, tuiCamera.GetComponent<Camera>().nearClipPlane);
			Vector3 vector = tuiCamera.GetComponent<Camera>().ScreenToWorldPoint(position);
			input.position.x = vector.x;
			input.position.y = vector.y;
			return tuiControlManager.HandleInput(input);
		}
		return false;
	}

	private T GetModule<T>(string name) where T : MonoBehaviour
	{
		GameObject gameObject = null;
		Transform transform = base.transform.Find(name);
		if ((bool)transform)
		{
			gameObject = transform.gameObject;
		}
		else
		{
			gameObject = new GameObject(name);
			gameObject.transform.parent = base.transform;
		}
		T val = gameObject.GetComponent<T>();
		if (!(Object)val)
		{
			val = gameObject.AddComponent<T>();
		}
		return val;
	}
}
