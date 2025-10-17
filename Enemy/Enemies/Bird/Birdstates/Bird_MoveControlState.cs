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
    [Export] public float MaxForce = 300f;
    [Export] public float TurnSpeed = 80f;

    private float _currentRotation = 0f;
    private float _targetRotation = 0f;

    

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
            Vector2 direction = (pastPlayerPosition - _enemy.GlobalPosition).Normalized();
            Vector2 desiredVelocity = direction * Speed;

            Vector2 steering = desiredVelocity - velocity;
            float maxForceThisFrame = MaxForce * (float)delta;
            if (steering.Length() > maxForceThisFrame)
            {
                steering = steering.Normalized() * maxForceThisFrame;
            }
            velocity += steering;
            if (velocity.Length() > Speed)
            {
                velocity = velocity.Normalized() * Speed;
            }   
            _targetRotation = velocity.Angle();
            Storage.SetVariant("HeadingLeft", velocity.X < 0);

        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0f, Deceleration * (float)delta);
            velocity.Y = Mathf.MoveToward(velocity.Y, 0f, Deceleration * (float)delta);
        }

        _enemy.Velocity = velocity;
    }

    protected override void FrameUpdate(double delta)
    {
        if (Storage.GetVariant<bool>("HeadingLeft"))
        {
            _sprite.FlipH = true;
        }
        else
        {
            _sprite.FlipH = true;
        }
        _currentRotation = Mathf.LerpAngle(_currentRotation, _targetRotation, TurnSpeed * (float)delta);
        _sprite.Rotation = _currentRotation;
        if (_enemy.Velocity.X < 0)
        {
            _sprite.FlipV = true;
        }
        else
        {
            _sprite.FlipV = false;
        }
        
        
        
    }

    private void OnChase()
    {
        GD.Print("Chase event triggered");
        Storage.SetVariant("Is_Chasing", true);
    }

    
}