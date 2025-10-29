using Godot;
using System;
using System.Threading.Tasks;
public partial class GuardianOfTheForest_ArmLaunchState : State
{
    [Export] public PackedScene ArmScene = null;
    [Export] public PackedScene GlowingArmScene = null;
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
    protected async override void Exit()
    {
        _sprite.AnimationFinished -= OnAnimationFinished;
    }
    private async Task LaunchArm(int count = 1, float interval = 0.2f)
    {
        for (int i = 0; i < count - 1; i++)
        {
            SpawnArmProjectile(false);
            await ToSignal(GetTree().CreateTimer(interval), SceneTreeTimer.SignalName.Timeout);
        }
        SpawnArmProjectile(true);
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
        await LaunchArm(3);
        AskTransit("Dash");
    }
}
