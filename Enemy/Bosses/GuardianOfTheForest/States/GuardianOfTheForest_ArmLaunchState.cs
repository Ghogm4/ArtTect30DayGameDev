using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public partial class GuardianOfTheForest_ArmLaunchState : State
{
    [Export] public PackedScene ArmScene = null;
    [Export] public PackedScene GlowingArmScene = null;
    [Export] public int NormalArmCount = 2;
    [Export] public int GlowingArmCount = 2;
    [Export] public float ArmInterval = 0.2f;
    private EnemyBase _enemy = null;
    private AnimatedSprite2D _sprite = null;
    private Player _player = null;
    private Marker2D _armlaunchMarker = null;

    protected override void ReadyBehavior()
    {
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _player = GetTree().GetFirstNodeInGroup("Player") as Player;
        _armlaunchMarker = Storage.GetNode<Marker2D>("ArmLaunchMarker");
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
    }

    protected override void Enter()
    {
        _enemy.Velocity = Vector2.Zero;
        _sprite.AnimationFinished += OnAnimationFinished;
        _sprite.Play("ArmLaunch");
        Storage.SetVariant("CanTurnAround", false);
    }
    protected override void Exit()
    {
        _sprite.AnimationFinished -= OnAnimationFinished;
    }
    private async Task LaunchArm(int normalArmCount = 2, int glowingArmCount = 1, float interval = 0.2f)
    {
        List<bool> armTypes = new();
        for (int i = 0; i < normalArmCount; i++)
            armTypes.Add(false);
        for (int i = 0; i < glowingArmCount; i++)
            armTypes.Add(true);
        for (int i = armTypes.Count - 1; i > 0; i--)
        {
            int j = Convert.ToInt32(GD.Randi() % (i + 1));
            bool temp = armTypes[i];
            armTypes[i] = armTypes[j];
            armTypes[j] = temp;
        }
        for (int i = 0; i < armTypes.Count; i++)
        {
            if (_enemy.IsDead)
                break;
            SpawnArmProjectile(armTypes[i]);
            await ToSignal(GetTree().CreateTimer(interval), SceneTreeTimer.SignalName.Timeout);
        }
        Storage.SetVariant("CanTurnAround", true);
    }
    private void SpawnArmProjectile(bool glowing = false)
    {
        Arm armProjectile;
        if (glowing)
            armProjectile = Projectile.Factory.CreateProjectile<GlowingArm>(GlowingArmScene);
        else
            armProjectile = Projectile.Factory.CreateProjectile<Arm>(ArmScene);
        Vector2 direction = (_player.GlobalPosition - _armlaunchMarker.GlobalPosition).Normalized();
        armProjectile.Position = _armlaunchMarker.GlobalPosition;
        armProjectile.Velocity = direction * armProjectile.BaseSpeed;
        armProjectile.Rotation = direction.Angle();
        armProjectile.Damage = (int)Stats.GetStatValue("ArmDamage");
        GetTree().CurrentScene.AddChild(armProjectile);
    }
    private async void OnAnimationFinished()
    {
        await LaunchArm(NormalArmCount, GlowingArmCount, ArmInterval);
        AskTransit("Decision");
    }
}
