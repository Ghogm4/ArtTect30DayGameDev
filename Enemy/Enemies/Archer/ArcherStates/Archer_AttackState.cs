using Godot;
using System;
using System.Threading.Tasks;

public partial class Archer_AttackState : State
{
    private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy = null;
    private Player _player = null;
    private Vector2 _playerLastPosition = Vector2.Zero;
    private float _maxDegree = 30f;

    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _player = GetTree().GetFirstNodeInGroup("Player") as Player;

        _sprite.AnimationFinished += () =>
        {
            _sprite.Stop();
            AskTransit("AttackIdle");
        };

        
    }

    protected override void Enter()
    {

        _sprite.Play("Attack");
        GD.Print("Enter Archer Attack State");

        // 停止奔跑
        Storage.SetVariant("IsAttackIdling", false);
        Storage.SetVariant("IsAttacking", true);
        Storage.SetVariant("IsRunning", false);
        Storage.SetVariant("IsRolling", false);
        _enemy.Velocity = new Vector2(0, _enemy.Velocity.Y);
        Storage.SetVariant("HasSpawnedArrow", false);

        if (_player.GlobalPosition.X < _enemy.GlobalPosition.X)
        {
            Storage.SetVariant("HeadingLeft", true);
            _sprite.FlipH = true;
        }
        else
        {
            Storage.SetVariant("HeadingLeft", false);
            _sprite.FlipH = false;
        }
    }
    protected override async void FrameUpdate(double delta)
    {


        if (_sprite.Frame == 6 && Storage.GetVariant<bool>("HasSpawnedArrow") == false)
        {
            SpawnArrow();
            Storage.SetVariant("HasSpawnedArrow", true);
        }
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        _playerLastPosition = _player.GlobalPosition;
    }
    public void SpawnArrow()
    {
        Vector2 startPosition = _enemy.GlobalPosition + new Vector2(Storage.GetVariant<bool>("HeadingLeft") ? -20f : 20f, -5f);
        Vector2 direction = new Vector2();
        if (_playerLastPosition.X < _enemy.GlobalPosition.X)
        {
            Storage.SetVariant("HeadingLeft", true);
        }
        else
        {
            Storage.SetVariant("HeadingLeft", false);
        }
        direction = _enemy.GlobalPosition.DirectionTo(_playerLastPosition);
        if (Mathf.Abs(direction.Y / direction.X) > Mathf.Tan(Mathf.DegToRad(_maxDegree)))
        {
            direction.Y = direction.Y * Mathf.Tan(Mathf.DegToRad(_maxDegree)) * Math.Abs(direction.X);
        }
        Vector2 velocity = direction * 1000f;
        var arrow = Projectile.Factory.CreateHostile<Arrow>("Arrow");
        arrow.Position = startPosition;
        arrow.Rotation = direction.Angle();
        arrow.Velocity = velocity;
        GetTree().CurrentScene.AddChild(arrow);
    }
    
    
}
