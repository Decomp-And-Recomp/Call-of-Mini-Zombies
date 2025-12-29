using System.Collections;

public class Sprite2DAnimated : Sprite2DStatic
{
	public class Animation
	{
		public Frame[] Frames;

		public int FrameRate;

		public bool Loop;
	}

	private Hashtable m_Animations = new Hashtable();

	private Animation m_CurrentAnimation;

	private float m_AnimationTime;

	public override void Update(float delta_time)
	{
		base.Update(delta_time);
		UpdateAnimation(delta_time);
	}

	public void AddAnimation(string name, Animation animation)
	{
		m_Animations[name] = animation;
	}

	public void PlayAnimation(string name)
	{
		if (m_Animations.Contains(name))
		{
			m_CurrentAnimation = (Animation)m_Animations[name];
			m_AnimationTime = 0f;
			UpdateAnimation(0f);
		}
	}

	public void StopAnimation()
	{
		m_CurrentAnimation = null;
		m_AnimationTime = 0f;
	}

	private void UpdateAnimation(float delta_time)
	{
		if (m_CurrentAnimation != null)
		{
			m_AnimationTime += delta_time;
			int num = (int)(m_AnimationTime * (float)m_CurrentAnimation.FrameRate);
			if (num >= m_CurrentAnimation.Frames.Length)
			{
				num = ((!m_CurrentAnimation.Loop) ? (m_CurrentAnimation.Frames.Length - 1) : (num % m_CurrentAnimation.Frames.Length));
			}
			base.ImageFrame = m_CurrentAnimation.Frames[num];
		}
	}
}
