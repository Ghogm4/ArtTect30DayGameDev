using Godot;
using System;

public partial class DarkSoul_UniversalState : State
{
	[Export] public float LifeTime = 5f;
	private EnemyBase _enemy;
	private AnimatedSprite2D _sprite;

	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");

		GetTree().CreateTimer(LifeTime).Timeout += () =>
		{
			if (IsInstanceValid(_enemy) && !_enemy.IsDead)
				AskTransit("Die");
		};
	}
    protected override void FrameUpdate(double delta)
	{
		if (_enemy.IsDead)
		{
			AskTransit("Die");
			return;
		}
		if (Storage.GetVariant<bool>("CanTurnAround"))
        	_sprite.FlipH = _enemy.Velocity.X < 0;
    }
}
