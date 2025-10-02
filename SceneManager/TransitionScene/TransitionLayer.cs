using Godot;
using System;
using System.Threading.Tasks;

public partial class TransitionLayer : CanvasLayer
{
	[Export]
	public ColorRect BlackScreen;

	public async Task FadeIn(float duration = 0.5f)
	{
		BlackScreen.Visible = true;
		var tween = CreateTween();
		tween.TweenProperty(BlackScreen, "modulate:a", 1.0f, duration);
		await ToSignal(tween, "finished");
	}
	public async Task FadeOut(float duration = 0.5f)
	{
		var tween = CreateTween();
		tween.TweenProperty(BlackScreen, "modulate:a", 0.0f, duration);
		await ToSignal(tween, "finished");
		BlackScreen.Visible = false;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BlackScreen.Visible = false;
		BlackScreen.Modulate = new Color(0, 0, 0, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
