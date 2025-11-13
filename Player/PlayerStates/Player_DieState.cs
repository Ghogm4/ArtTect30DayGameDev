using Godot;
using System;

public partial class Player_DieState : State
{
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
	}
	protected override async void Enter()
	{
		_sprite.Play("Die");
		_player.Velocity = Vector2.Zero;
		await ToSignal(_sprite, AnimatedSprite2D.SignalName.AnimationFinished);
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerDied);
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameFinished);
		SceneManager.Instance.ChangeScenePath("res://UI/GameFinished/GameFinished.tscn");
	}
}
