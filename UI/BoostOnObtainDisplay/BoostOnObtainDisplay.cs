using Godot;
using System;

public partial class BoostOnObtainDisplay : Control
{
	[Export] public Label BoostOnObtainBoostName;
	[Export] public Label BoostOnObtainBoostDescription;
	[Export] public TextureRect BoostOnObtainBoostIcon;
	[Export] public Timer DisplayTimer;
	public const float DisplayDuration = 3f;
	private Tween tween;
	private bool _duringDialogue = false;
	public override void _Ready()
	{
		SignalBus.Instance.Connect(SignalBus.SignalName.PlayerBoostPickedUp, Callable.From<BoostInfo, bool, bool>(DisplayBoost));
		SignalBus.Instance.Connect(SignalBus.SignalName.DialogueStarted, Callable.From(() =>
		{
			_duringDialogue = true;
			tween?.Kill();
			Modulate = new(1, 1, 1, 0);
			Visible = false;
		}));
		SignalBus.Instance.Connect(SignalBus.SignalName.DialogueEnded, Callable.From(() =>
		{
			_duringDialogue = false;
			Modulate = new(1, 1, 1, 0);
			Visible = true;
		}));
		DisplayTimer.Timeout += () =>
		{
			tween?.Kill();
			tween = CreateTween();
			tween.TweenProperty(this, "modulate:a", 0, 0.5f)
				.SetTrans(Tween.TransitionType.Linear)
				.SetEase(Tween.EaseType.InOut);
			tween.TweenCallback(Callable.From(() => Visible = false));
		};
	}

	public void DisplayBoost(BoostInfo info, bool displayWhenObtained, bool displayOnCurrentBoosts = false)
	{
		if (!displayWhenObtained || _duringDialogue)
			return;
		Modulate = new Color(1, 1, 1, 1);
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
