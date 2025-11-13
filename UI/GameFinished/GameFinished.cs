using Godot;
using System;

public partial class GameFinished : Control
{
	public override void _Ready()
    {
		Label finishWordLabel = GetNode<Label>("%FinishWordLabel");
		Label statisticsLabel = GetNode<Label>("%StatisticsLabel");
        if (GameData.Instance.VictoryAchieved)
			finishWordLabel.Text = "The victory is yours!";
		else
			finishWordLabel.Text = "You failed to endure the pain, and fell...";
		int minute = Convert.ToInt32(GameData.Instance.TotalPlayTimeInSeconds / 60f);
		int seconds = (int)GameData.Instance.TotalPlayTimeInSeconds % 60;
		string minuteStr = minute.ToString().PadLeft(2, '0');
		string secondsStr = seconds.ToString().PadLeft(2, '0');
		statisticsLabel.Text = $"Play time: {minuteStr}:{secondsStr}, Boosts collected: {GameData.Instance.TotalBoostsCollected}";
    }
}
