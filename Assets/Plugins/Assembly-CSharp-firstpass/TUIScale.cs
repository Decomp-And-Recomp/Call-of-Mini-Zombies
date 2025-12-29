using UnityEngine;

public class TUIScale : TUIControlImpl
{
	protected enum State
	{
		Idle = 0,
		ScaleIn = 1,
		ScaleOut = 2
	}

	public TUIMeshSprite scale_sprite;

	public Vector3 scaleInBegin = new Vector3(0.5f, 0.5f, 1f);

	public Vector3 scaleInEnd = new Vector3(1.2f, 1.2f, 1f);

	public float scaleInTime = 0.35f;

	public Vector3 scaleOutBegin = new Vector3(1.2f, 1.2f, 1f);

	public Vector3 scaleOutEnd = new Vector3(1f, 1f, 1f);

	public float scaleOutTime = 0.35f;

	protected State state;

	protected float time;

	protected string scaleOutScene = string.Empty;

	public void ScaleIn()
	{
		state = State.ScaleIn;
		time = 0f;
		if (scale_sprite != null)
		{
			scale_sprite.gameObject.SetActive(true);
			scale_sprite.transform.localScale = scaleInBegin;
			scale_sprite.Static = false;
		}
	}

	public void ScaleOut()
	{
		ScaleOut(string.Empty);
	}

	public void ScaleOut(string scene)
	{
		state = State.ScaleOut;
		time = 0f;
		scaleOutScene = scene;
		if (scale_sprite != null)
		{
			scale_sprite.gameObject.SetActive(true);
			scale_sprite.transform.localScale = scaleOutBegin;
			scale_sprite.Static = false;
		}
	}

	public new void Start()
	{
		base.Start();
		if (scale_sprite != null)
		{
			scale_sprite.gameObject.SetActive(false);
		}
	}

	public void Update()
	{
		if (state != 0)
		{
			if (state == State.ScaleIn)
			{
				UpdateFadeIn(Mathf.Clamp(Time.deltaTime, 0f, 0.05f));
			}
			else if (state == State.ScaleOut)
			{
				UpdateFadeOut(Mathf.Clamp(Time.deltaTime, 0f, 0.05f));
			}
		}
	}

	public override bool HandleInput(TUIInput input)
	{
		if (state == State.Idle)
		{
			return false;
		}
		if (!PtInControl(input.position))
		{
			return false;
		}
		return true;
	}

	protected virtual void UpdateFadeIn(float deltaTime)
	{
		time += deltaTime;
		if (scale_sprite != null)
		{
			float t = Mathf.Clamp(time / scaleInTime, 0f, 1f);
			scale_sprite.transform.localScale = Vector3.Lerp(scaleInBegin, scaleInEnd, t);
		}
		if (time > scaleInTime)
		{
			state = State.Idle;
			if (scale_sprite != null)
			{
				scale_sprite.Static = true;
			}
		}
	}

	protected virtual void UpdateFadeOut(float deltaTime)
	{
		time += deltaTime;
		if (scale_sprite != null)
		{
			float t = Mathf.Clamp(time / scaleOutTime, 0f, 1f);
			scale_sprite.transform.localScale = Vector3.Lerp(scaleOutBegin, scaleOutEnd, t);
		}
		if (time > scaleOutTime)
		{
			state = State.Idle;
			if (scale_sprite != null)
			{
				scale_sprite.transform.localScale = scaleOutEnd;
			}
			if (scaleOutScene.Length > 0)
			{
				Application.LoadLevel(scaleOutScene);
			}
		}
	}
}
