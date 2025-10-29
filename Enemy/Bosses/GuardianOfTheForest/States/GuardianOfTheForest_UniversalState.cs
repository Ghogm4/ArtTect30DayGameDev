using Godot;
using System;

public partial class GuardianOfTheForest_UniversalState : State
{
	[Export] public Node2D AreaContainer = null;
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	private Player _player = null;
	private int HeadingRight
	{
		get => (int)Stats.GetStatValue("HeadingRight");
		set => Stats.SetValue("HeadingRight", value);
	}
	protected override void ReadyBehavior()
	{
		InitializeStorageVariants();
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		Stats.GetStat("HeadingRight").StatChanged += (float oldVal, float newVal) =>
		{
			if (!Storage.GetVariant<bool>("CanTurnAround") || _enemy.IsDead)
				return;
			_sprite.FlipH = Mathf.IsZeroApprox(newVal);
			AreaContainer.Scale = new Vector2(Mathf.IsEqualApprox(newVal, 1) ? 1 : -1, 1);
		};
		
	}
	protected override void FrameUpdate(double delta)
	{
		if (!Storage.GetVariant<bool>("CanTurnAround"))
			return;
		HeadingRight = (_player.GlobalPosition.X >= _enemy.GlobalPosition.X) ? 1 : 0;
		if (_enemy.IsDead && !Storage.GetVariant<bool>("IsInDieState"))
			AskTransit("Die");
	}
	private void InitializeStorageVariants()
    {
		Storage.SetVariant("MeleeAreaOffset", GetNode<Area2D>("%MeleeArea").Position);
		Storage.SetVariant("CanTurnAround", true);
    }
}
