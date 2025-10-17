using Godot;
using System;

public partial class Bird_MoveControlState : State
{
    private EnemyBase _enemy = null;
    private Player _player = null;
    private AnimatedSprite2D _sprite = null;
    [Export] public float Speed = 100f;
    [Export] public float Acceleration = 100f;
    [Export] public float Deceleration = 500f;

    private Vector2 pastPlayerPosition = Vector2.Zero;
    private int pastTime = 0;

    protected override void ReadyBehavior()
    {
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _player = GetTree().GetFirstNodeInGroup("Player") as Player;
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");

        var chaseState = GetNode<State>("Chase");
        chaseState.Connect("Chase", new Callable(this, nameof(OnChase)));
        pastPlayerPosition = _player.GlobalPosition;
    }

    protected override void PhysicsUpdate(double delta)
    {
        Vector2 velocity = _enemy.Velocity;
        pastTime += 1;
        if (pastTime >= 10)
        {
            pastPlayerPosition = _player.GlobalPosition;
            pastTime = 0;
        }
        if (Storage.GetVariant<bool>("Is_Chasing"))
        {
            Vector2 direction = new Vector2(pastPlayerPosition.X - _enemy.GlobalPosition.X, pastPlayerPosition.Y - _enemy.GlobalPosition.Y).Normalized();
            if (Mathf.Abs(velocity.Length()) < Speed)
            {
                velocity += direction * Acceleration * (float)delta;
            }
            else
            {
                velocity = direction * Speed;
            }
            Storage.SetVariant("HeadingLeft", velocity.X < 0);
        }
        else
        {
            velocity = Vector2.Zero;
        }

        _enemy.Velocity = velocity;
    }

    protected override void FrameUpdate(double delta)
    {
        if (Storage.GetVariant<bool>("HeadingLeft"))
        {
            _sprite.FlipH = false;
        }
        else
        {
            _sprite.FlipH = true;
        }
    }

    private void OnChase()
    {
        GD.Print("Chase event triggered");
        Storage.SetVariant("Is_Chasing", true);
    }
}