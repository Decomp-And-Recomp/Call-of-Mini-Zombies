using Zombie3D;

public interface ITutorialStep
{
	void UpdateTutorialStep(float deltaTime, Player player);

	void StartStep(TutorialScript ts, Player player);

	void EndStep(Player player);
}
