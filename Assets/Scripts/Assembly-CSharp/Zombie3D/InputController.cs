using UnityEngine;

namespace Zombie3D
{
	public abstract class InputController
	{
		protected Vector2 cameraRotation = new Vector2(0f, 0f);

		protected GameScene gameScene;

		protected Player player;

		protected Vector2 lastTouchPosition = default(Vector2);

		public bool EnableMoveInput { get; set; }

		public bool EnableTurningAround { get; set; }

		public bool EnableShootingInput { get; set; }

		public InputInfo InputInfo { get; set; }

		public Vector2 CameraRotation
		{
			get
			{
				return cameraRotation;
			}
			set
			{
				cameraRotation = value;
			}
		}

		public void Init()
		{
			gameScene = GameApp.GetInstance().GetGameScene();
			player = gameScene.GetPlayer();
			EnableMoveInput = true;
			EnableShootingInput = true;
			EnableTurningAround = true;
			InputInfo = new InputInfo();
		}

		public virtual void ProcessInput(float deltaTime, InputInfo inputInfo)
		{
		}

		public virtual void ProcessFireInput(int inputEventType, float distance, float angle, TUIInput data)
		{
		}

		public virtual void ProcessMoveInput(int inputEventType, float distance, float angle)
		{
		}

		public virtual void ProcessRotateInput(int inputEventType, TUIInput data)
		{
		}
	}
}
