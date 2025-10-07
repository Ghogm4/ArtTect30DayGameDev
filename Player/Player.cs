using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public override void _Ready()
	{
		AudioManager.Instance.LoadSFX("Run", "res://Assets/SFX/zijizuode/foot2.mp3");
		AudioManager.Instance.LoadSFX("Jump", "res://Assets/SFX/zijizuode/jump2.mp3");
		AudioManager.Instance.LoadSFX("Fall", "res://Assets/SFX/zijizuode/fall2.mp3");
		AudioManager.Instance.LoadSFX("Attack1", "res://Assets/SFX/zijizuode/blade1.mp3");
		AudioManager.Instance.LoadSFX("Attack2", "res://Assets/SFX/zijizuode/blade2.mp3");
		AudioManager.Instance.LoadSFX("Attack3", "res://Assets/SFX/zijizuode/blade3.mp3");
	}
}
