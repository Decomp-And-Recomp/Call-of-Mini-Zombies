using System;
using UnityEngine;
using Zombie3D;

public class InstanceModeGameStatisticsUI : MonoBehaviour
{
	public TUIMeshText wave;

	public TUIMeshText score;

	public TUIMeshText time;

	public GameObject effect;

	public GameObject alertbk;

	private int waveNum = -1;

	private int scoreNum = -1;

	public OnTimerBegin OnBegin;

	public OnTimerEnd OnEnd;

	private float deltaTime;

	private bool addingTimeEffect;

	private float timeEffectTimer = 5f;

	public float timer { get; set; }

	private void Start()
	{
		deltaTime = 0f;
		effect.SetActive(false);
		alertbk.SetActive(false);
		if (OnBegin != null)
		{
			OnBegin();
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if ((double)deltaTime <= 0.02)
		{
			return;
		}
		timer -= deltaTime;
		if (addingTimeEffect)
		{
			if (timer <= 0f)
			{
				timer = 0f;
			}
			TimeSpan timeSpan = new TimeSpan(0, 0, (int)timer);
			time.text_Accessor = timeSpan.ToString() + " +" + InstanceModeConfig.TimeAdded + "'";
			timeEffectTimer -= deltaTime;
			if (timeEffectTimer <= 0f)
			{
				addingTimeEffect = false;
				effect.SetActive(false);
				timer += InstanceModeConfig.TimeAdded;
			}
		}
		else if (timer <= 0f)
		{
			if (OnEnd != null)
			{
				OnEnd();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			TimeSpan timeSpan2 = new TimeSpan(0, 0, (int)timer);
			time.text_Accessor = timeSpan2.ToString();
		}
		if (timer <= 10f)
		{
			time.color_Accessor = ColorName.fontColor_red;
			alertbk.SetActive(true);
		}
		else
		{
			time.color_Accessor = ColorName.fontColor_orange;
			alertbk.SetActive(false);
		}
		deltaTime = 0f;
	}

	public void AddTimeEffect()
	{
		addingTimeEffect = true;
		effect.SetActive(true);
		timeEffectTimer = 5f;
	}

	public void UpdateWave(int value)
	{
		if (value != waveNum)
		{
			wave.text_Accessor = "WAVE: " + value;
			waveNum = value;
			if (waveNum > 1)
			{
				ScaleEff scaleEff = wave.gameObject.AddComponent<ScaleEff>();
				scaleEff.destroyAfterFinished = true;
				scaleEff.time_step1 = 0.6f;
				scaleEff.time_step2 = 0.6f;
				scaleEff.time_step3 = 0f;
				scaleEff.scale_step0 = new Vector3(0.75f, 0.75f, 1f);
				scaleEff.scale_step1 = new Vector3(1.25f, 1.25f, 1f);
				scaleEff.scale_step2 = new Vector3(1f, 1f, 1f);
				scaleEff.scale_step3 = new Vector3(1f, 1f, 1f);
			}
		}
	}

	public void UpdateScore(int value)
	{
		if (value != scoreNum)
		{
			score.text_Accessor = "SCORE: " + value;
			scoreNum = value;
		}
	}
}
