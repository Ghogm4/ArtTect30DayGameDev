using Godot;
using System;

public partial class Bird_MoveControlState : State
{
    private EnemyBase _enemy = null;
    private Player _player = null;
    private AnimatedSprite2D _sprite = null;
    // [Export] public float Speed = 70f;
    private StatWrapper _speed = null;
    private StatWrapper _damage = null;
    [Export] public float Acceleration = 100f;
    [Export] public float Deceleration = 10f;
    [Export] public float MaxForce = 300f;
    [Export] public float TurnSpeed = 80f;

    [Export] public float AttackCD = 5f;
    [Export] public float DiveSpeed = 400f;

    private float _currentRotation = 0f;
    private float _targetRotation = 0f;
    private bool _isPreparing = false;
    private bool _isDiving = false;
    private float _attackTimer = 0f;
    private float _prepareTimer = 0f;
    private Vector2 _lastPos = Vector2.Zero;
    private Vector2 _lastEnemyPos = Vector2.Zero;
    private float _diveWay = 0f;
    private float _diveLength = 0f;
    private Tween _flashTween = null;
    private bool _attacked = false;

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

        _speed = new(Stats.GetStat("Speed"));
        _damage = new(Stats.GetStat("Damage"));

        foreach (var name in AudioManager.Instance.SFXDict.Keys)
        {
            if (name == "BirdDive")
            {
                return;
            }
        }
        AudioManager.Instance.LoadSFX("BirdDive", "res://Assets/SFX/zijizuode/Dash.mp3");
    }

    protected override void PhysicsUpdate(double delta)
    {
        Vector2 velocity = _enemy.Velocity;
        pastTime += 1;
        if (pastTime >= 3)
        {
            pastPlayerPosition = _player.GlobalPosition;
            pastTime = 0;
        }
        if (!_isPreparing && !_isDiving && Storage.GetVariant<bool>("Is_Chasing"))
        {
            _attackTimer += (float)delta;
            if (_attackTimer >= AttackCD + GD.RandRange(-1f, 1f))
            {
                _isPreparing = true;
                _prepareTimer = 0f;
                _attackTimer = 0f;
            }
        }
        else if (_isPreparing)
        {
            if (_flashTween == null)
            {
                _flashTween = _enemy.CreateTween();
                flash();
            }
            _prepareTimer += (float)delta;

            _lastPos = _player.GlobalPosition;
            _lastEnemyPos = _enemy.GlobalPosition;
            _diveLength = _enemy.GlobalPosition.DistanceTo(_lastPos);
            if (_prepareTimer >= 0.5f)
            {
                _isPreparing = false;
                _isDiving = true;
                AudioManager.Instance.PlaySFX("BirdDive");
            }
        }
        else if (_isDiving)
        {
            _sprite.Modulate = new Color(0.1f, 0.1f, 0.1f, 1f);
            Vector2 direction = (_lastPos - _lastEnemyPos).Normalized();
            velocity = direction * DiveSpeed;
            _diveWay += DiveSpeed * (float)delta;
            if (_diveWay >= _diveLength + 50f || _enemy.IsOnFloor() || _diveWay >= 200)
            {
                _isDiving = false;
                _diveWay = 0f;
                _sprite.Modulate = new Color(1, 1, 1, 1);
                _attacked = false;
                return;
            }
            if (!_attacked)
            {
                Attack();
            }
            _targetRotation = velocity.Angle();
        }
        if (Storage.GetVariant<bool>("Is_Chasing") && !_isPreparing && !_isDiving)
        {
            Vector2 direction = (pastPlayerPosition - _enemy.GlobalPosition).Normalized();
            Vector2 desiredVelocity = direction * (float)_speed;

            Vector2 steering = desiredVelocity - velocity;
            float maxForceThisFrame = MaxForce * (float)delta;
            if (steering.Length() > maxForceThisFrame)
            {
                steering = steering.Normalized() * maxForceThisFrame;
            }
            velocity += steering;
            if (velocity.Length() > (float)_speed)
            {
                velocity = velocity.Normalized() * (float)_speed;
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
        Rotate((float)delta);
    }

    private void OnChase()
    {
        Storage.SetVariant("Is_Chasing", true);
    }

    private void Rotate(float delta)
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

    public async void flash()
    {
        Color originalColor = _sprite.Modulate;
        _flashTween.TweenProperty(_sprite, "modulate", Colors.Black, 0.1f);
        _flashTween.TweenProperty(_sprite, "modulate", originalColor, 0.1f);
        _flashTween.TweenProperty(_sprite, "modulate", Colors.Black, 0.1f);
        _flashTween.TweenProperty(_sprite, "modulate", originalColor, 0.1f);
        await ToSignal(_flashTween, "finished");
        _flashTween = null;
    }
    
    public void Attack()
    {
        var area = _enemy.GetNode<Area2D>("AttackArea");
        var bodies = area.GetOverlappingBodies();
        foreach (var body in bodies)
        {
            if (body is Player player)
            {
                _enemy.SendDamageRequest((float)_damage);
                _attacked = true;
            }
        }
    }
}