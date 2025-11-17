using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Signal] public delegate void PlayerJumpedEventHandler();
	[Signal] public delegate void PlayerLandedEventHandler();
	[Signal] public delegate void PlayerDashedEventHandler();
	[Export] public PlayerStatComponent PlayerStats = null;
	public override void _Ready()
	{
		AudioManager.Instance.LoadSFX("Run", "res://Assets/SFX/zijizuode/foot2.mp3");
		AudioManager.Instance.LoadSFX("Jump", "res://Assets/SFX/zijizuode/jump2.mp3");
		AudioManager.Instance.LoadSFX("Fall", "res://Assets/SFX/zijizuode/fall2.mp3");
		AudioManager.Instance.LoadSFX("Attack1", "res://Assets/SFX/zijizuode/blade1.mp3");
		AudioManager.Instance.LoadSFX("Attack2", "res://Assets/SFX/zijizuode/blade2.mp3");
		AudioManager.Instance.LoadSFX("Attack3", "res://Assets/SFX/zijizuode/blade3.mp3");
		AudioManager.Instance.LoadSFX("Hit", "res://Assets/SFX/zijizuode/gethit2.mp3");
		AudioManager.Instance.LoadSFX("DashAttack", "res://Assets/SFX/zijizuode/dashattack.mp3");
		AudioManager.Instance.LoadSFX("Fireball", "res://Assets/SFX/zijizuode/fireball.mp3");
		AudioManager.Instance.LoadSFX("AltarClaim", "res://Assets/SFX/zijizuode/altar.mp3");
	}
	public void TakeDamage(float damage, Callable customBehavior)
	{
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerHit, damage, customBehavior);
		AudioManager.Instance.PlaySFX("Hit");
	}
}
