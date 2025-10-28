using Godot;
using System;

public partial class GuardianOfTheForest_UniversalState : State
{
	[Export] public Node2D AreaContainer = null;
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		Stats.GetStat("HeadingRight").StatChanged += (float oldVal, float newVal) =>
        {
			_sprite.FlipH = Mathf.IsZeroApprox(newVal);
			AreaContainer.Scale = new Vector2(Mathf.IsEqualApprox(newVal, 1) ? 1 : -1, 1);
        };
	}
}
