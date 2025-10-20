using Godot;
using System;

public partial class BoostOnObtainDisplay : CanvasLayer
{
	[Export] public Label BoostOnObtainBoostName;
	[Export] public Label BoostOnObtainBoostDescription;
	[Export] public TextureRect BoostOnObtainBoostIcon;
	[Export] public Timer DisplayTimer;
	[Export] public Control BoostOnObtainDisplayPanel;
	public const float DisplayDuration = 3f;
	private Tween tween;
    public override void _Ready()
    {
		SignalBus.Instance.Connect(SignalBus.SignalName.PlayerBoostPickedUp, Callable.From<BoostInfo, bool>(DisplayBoost));
		DisplayTimer.Timeout += async () =>
		{
			tween?.Kill();
			tween = CreateTween();
			tween.TweenProperty(BoostOnObtainDisplayPanel, "modulate:a", 0, 0.5f)
				.SetTrans(Tween.TransitionType.Linear)
				.SetEase(Tween.EaseType.InOut);
			tween.TweenCallback(Callable.From(() => Visible = false));
		};
    }

	public void DisplayBoost(BoostInfo info, bool needDisplay)
	{
		if (!needDisplay)
			return;
		BoostOnObtainDisplayPanel.Modulate = new Color(1, 1, 1, 1);
		tween?.Kill();
		Visible = true;
		DisplayTimer.Start(DisplayDuration);
		BoostOnObtainBoostName.Text = info.Name;
		BoostOnObtainBoostName.LabelSettings.FontColor = BoostInfo.RarityColorMap[info.Rarity];
		BoostOnObtainBoostDescription.Text = info.Description;
		BoostOnObtainBoostIcon.Texture = info.Icon;

		Vector2 iconPivotOffset = BoostOnObtainBoostIcon.PivotOffset;
		iconPivotOffset.Y = BoostOnObtainBoostIcon.Texture.GetSize().Y / 2;
		BoostOnObtainBoostIcon.PivotOffset = iconPivotOffset;
	}
}
