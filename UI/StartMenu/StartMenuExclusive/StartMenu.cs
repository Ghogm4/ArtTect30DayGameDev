using Godot;
using System;

public partial class StartMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AudioManager.Instance.LoadBGM("StartBGM", "res://Assets/BGM/suspicious opening.mp3");
		AudioManager.Instance.LoadSFX("Confirm", "res://Assets/SFX/confirm.wav");
		AudioManager.Instance.LoadSFX("Select", "res://Assets/SFX/zijizuode/select.wav");
		AudioManager.Instance.PlayBGM("StartBGM", 57.78f, 86.22f, 26f, 5f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		AudioManager.Instance.StopBGM(1f);
	}
}
