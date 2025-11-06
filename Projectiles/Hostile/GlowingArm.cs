using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class GlowingArm : Arm
{
	[Export] public int LaunchOrbCount = 5;
	[Export] public float LaunchOrbSpeed = 300f;
	[Export] public int OnExpireOrbCount = 10;
	[Export] public float OnExpireLaunchOrbSpeed = 400f;
	[Export] public float OrbInBurstInterval = 0.1f;
	[Export] public float OrbBetweenBurstInterval = 1.0f;


	[Export] public PackedScene BlueOrbScene;

	public float _timeElapsed = 0f;
	protected override void ReadyBehavior()
	{
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		GetTree().CreateTimer(LifeTime).Timeout += () =>
		{
			if (IsInstanceValid(this))
				Explode();
		};
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		_timeElapsed += (float)delta;
		if (_timeElapsed >= OrbBetweenBurstInterval)
		{
			_timeElapsed -= OrbBetweenBurstInterval;
			LaunchOrbAtPlayer();
		}
	}
	private async void LaunchOrbAtPlayer()
	{
		if (!IsInstanceValid(_player))
			return;
		for (int i = 0; i < LaunchOrbCount; i++)
		{
			float spread = (float)GD.RandRange(-0.1f, 0.1f);
			var direction = (_player.GlobalPosition - GlobalPosition).Normalized().Rotated(spread);
			SpawnOrb(LaunchOrbSpeed, direction.Angle(), false);
			await ToSignal(GetTree().CreateTimer(OrbInBurstInterval), SceneTreeTimer.SignalName.Timeout);
		}
	}
	protected override void ExtraExplodeBehavior()
	{
		float currentRadian = (float)GD.RandRange(0, Mathf.Tau);
		float radianIncrement = Mathf.Tau / OnExpireOrbCount;
		for (int i = 0; i < OnExpireOrbCount; i++)
		{
			SpawnOrb(OnExpireLaunchOrbSpeed, currentRadian, true);
			currentRadian += radianIncrement;
		}
	}
	private void SpawnOrb(float speed, float radian, bool canPierceWorld)
	{
		var orb = BlueOrbScene.Instantiate<BlueOrb>();
		orb.GlobalPosition = GlobalPosition;
		orb.Velocity = Vector2.Right.Rotated(radian) * speed;
		orb.Rotation = orb.Velocity.Angle();
		orb.PierceWorld = canPierceWorld;
		GetTree().CurrentScene.CallDeferred(MethodName.AddChild, orb);
	}
}
