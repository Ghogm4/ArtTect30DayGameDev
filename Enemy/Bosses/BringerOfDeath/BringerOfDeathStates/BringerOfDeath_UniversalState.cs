using Godot;
using System;

public partial class BringerOfDeath_UniversalState : State
{
	private const float SpriteXOffset = -35f;
	private bool HeadingLeft
	{
		get => Storage.GetVariant<bool>("HeadingLeft");
		set
		{
			if (value == Storage.GetVariant<bool>("HeadingLeft")) return;
			Storage.SetVariant("HeadingLeft", value);
			ChangeFacing(value);
		}
	}
	private Player PlayerInst
	{
		get => Storage.GetNode<Player>("Player");
		set => Storage.SetNode("Player", value);
	}
	private float Ratio => Stats.GetStatValue("Health") / Stats.GetStatValue("MaxHealth");

	private EnemyBase _enemy;
	private AnimatedSprite2D _sprite;
	private Node2D _areaContainer;
	protected override void EnterTreeBehavior()
	{
		Storage.RegisterNode<Player>("Player", null);
		TryGetPlayer();
	}

	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_areaContainer = Storage.GetNode<Node2D>("AreaContainer");
	}
	protected override void FrameUpdate(double delta)
	{
		if (_enemy.IsDead)
			AskTransit("Die");
	}
	private void ApplyGravity(double delta)
	{
		if (_enemy.IsDead) return;
		Vector2 velocity = _enemy.Velocity;
		velocity += _enemy.GetGravity() * (float)delta;
		_enemy.Velocity = velocity;
	}
	protected override void PhysicsUpdate(double delta)
	{
		ApplyGravity(delta);
		if (!Storage.GetVariant<bool>("CanTurnAround")) return;
		HeadingLeft = Mathf.IsEqualApprox(_enemy.Velocity.X, 0) ? (PlayerInst.GlobalPosition.X <= _enemy.GlobalPosition.X) : (_enemy.Velocity.X < 0);
	}
	private void ChangeFacing(bool facingLeft)
	{
		Vector2 position = _sprite.Position;
		if (facingLeft)
		{
			_sprite.FlipH = false;
			position.X = SpriteXOffset;
			_areaContainer.Scale = Vector2.One;
		}
		else
		{
			_sprite.FlipH = true;
			position.X = -SpriteXOffset;
			_areaContainer.Scale = new Vector2(-1, 1);
		}
		_sprite.Position = position;
	}
	private void TryGetPlayer()
	{
		PlayerInst = GetTree().GetFirstNodeInGroup("Player") as Player;
		if (PlayerInst == null)
			CallDeferred(MethodName.TryGetPlayer);
	}
}
