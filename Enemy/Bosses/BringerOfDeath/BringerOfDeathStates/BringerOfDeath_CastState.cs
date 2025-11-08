using Godot;
using System;
using System.Collections.Generic;

public partial class BringerOfDeath_CastState : State
{
	[ExportGroup("Summon Reapers Settings")]
	[Export] public float MinReaperSummonRadius = 150f;
	[Export] public float MaxReaperSummonRadius = 200f;
	[Export] public int MaxReaperSummonCount = 5;
	[Export] public int MinReaperSummonCount = 2;
	[Export] public PackedScene ReaperScene;
	[ExportGroup("Summon Hands Settings")]
	[Export] public float MinHandSummonGap = 150f;
	[Export] public float MaxHandSummonGap = 200f;
	[Export] public PackedScene HandScene;
	private const float HandSummonYOffset = 38f;
	private float SummonGap => Mathf.Lerp(MinHandSummonGap, MaxHandSummonGap, Ratio);
	private float Ratio
	{
		get
		{
			float maxHealth = Stats.GetStatValue("MaxHealth");
			float health = Stats.GetStatValue("Health");
			return health / maxHealth;
		}
	}
	private readonly List<Tuple<float, Action>> CastActions = new();
	private Player PlayerInst => Storage.GetNode<Player>("Player");
	private EnemyBase _enemy;
	private AnimatedSprite2D _sprite;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");

		CastActions.Add(Tuple.Create(0.5f, SummonReapers));
		CastActions.Add(Tuple.Create(0.5f, SummonHands));
	}
	protected override void Enter()
	{
		_sprite.Play("Cast");
		_sprite.AnimationFinished += OnAnimationFinished;
	}
	protected override void Exit()
	{
		_sprite.AnimationFinished -= OnAnimationFinished;
		Probability.Run(CastActions);
	}
	private void OnAnimationFinished()
	{
		AskTransit("Decision");
	}
	private void SummonReapers()
	{
		for (int i = 0; i < Mathf.Lerp(MaxReaperSummonCount, MinReaperSummonCount, Ratio); i++)
		{
			float radian = (float)GD.RandRange(0, Mathf.Tau);
			float summonRadius = (float)GD.RandRange(MinReaperSummonRadius, MaxReaperSummonRadius);
			Vector2 summonPos = PlayerInst.GlobalPosition + Vector2.Right.Rotated(radian) * summonRadius;
			Reaper reaper = ReaperScene.Instantiate<Reaper>();
			reaper.GlobalPosition = summonPos;
			GetTree().CurrentScene.CallDeferred(MethodName.AddChild, reaper);
		}
	}
	private void SummonHand(Vector2 summonPos, float offset = 0)
	{
		Hand hand = HandScene.Instantiate<Hand>();
		hand.GlobalPosition = summonPos + Vector2.Up * HandSummonYOffset + Vector2.Right * offset;
		GetTree().CurrentScene.CallDeferred(MethodName.AddChild, hand);
	}
	private async void SummonHands()
	{
		float randomOffset = (float)GD.RandRange(-SummonGap / 2, SummonGap / 2);
		float summonGap = SummonGap;
		for (int i = 0; i < 10; i++)
		{
			Vector2 summonPosLeft = _enemy.GlobalPosition + Vector2.Right * i * summonGap;
			Vector2 summonPosRight = _enemy.GlobalPosition + Vector2.Left * i * summonGap;
			SummonHand(summonPosLeft, randomOffset);
			SummonHand(summonPosRight, randomOffset);
		}
		await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
		for (int i = 0; i < 10; i++)
		{
			Vector2 summonPosLeft = _enemy.GlobalPosition + Vector2.Right * i * summonGap;
			Vector2 summonPosRight = _enemy.GlobalPosition + Vector2.Left * i * summonGap;
			SummonHand(summonPosLeft, summonGap / 2 + randomOffset);
			SummonHand(summonPosRight, summonGap / 2 + randomOffset);
		}
	}
}
