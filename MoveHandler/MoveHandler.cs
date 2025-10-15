using Godot;
using System;

[GlobalClass]
[Tool]

public partial class MoveHandler : AnimatableBody2D
{
	private float _xoffset = 0f;
	[Export]
	public float Xoffset
	{
		get => _xoffset;
		set
		{
			_xoffset = value;
			QueueRedraw();
		}
	}
	
	private float _yoffset = 0f;
	[Export]
	public float Yoffset
	{
		get => _yoffset;
		set
		{
			_yoffset = value;
			QueueRedraw();
		}
	}
	private float _rotationOffset = 0f;
	[Export]
	public float RotationOffset
	{
		get => _rotationOffset;
		set
		{
			_rotationOffset = value;
			QueueRedraw();
		}
	}
	private bool _rotationClockwise = true;
	[Export]
	public bool RotationClockwise
	{
		get => _rotationClockwise;
		set
		{
			_rotationClockwise = value;
			QueueRedraw();
		}
	}
	private float _duration = 1f;
	[Export]
	public float Duration
	{
		get => _duration;
		set
		{
			_duration = value;
			QueueRedraw();
		}
	}
	private Tween.TransitionType _tweenType = Tween.TransitionType.Linear;
	[Export]
	public Tween.TransitionType TweenType
	{
		get => _tweenType;
		set
		{
			_tweenType = value;
			QueueRedraw();
		}
	}
	private Tween.EaseType _easeType = Tween.EaseType.InOut;
	[Export]
	public Tween.EaseType EaseType
	{
		get => _easeType;
		set
		{
			_easeType = value;
			QueueRedraw();
		}
	}
	private bool _loop = false;
	[Export] public bool Loop
	{
		get => _loop;
		set
		{
			_loop = value;
			QueueRedraw();
		}
	}
	private bool _reverse = false;
	[Export]
	public bool Reverse
	{
		get => _reverse;
		set
		{
			_reverse = value;
			if (!_loop)
			{
				GD.PrintErr("Reverse only works when Loop is enabled.");
				_reverse = false;
			}
			QueueRedraw();
		}
	}

	[Export] public bool AutoStart = true;
	

	private Vector2 _initialPosition;
	private float _initialRotation;
	private Tween _tween;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_initialPosition = GlobalPosition;
		_initialRotation = GlobalRotationDegrees;
		if (AutoStart) StartMove();
	}

	public void StartMove()
	{
		_tween = CreateTween();
		if (_loop && !_reverse)
		{
			_tween.SetLoops();
		}
		Vector2 targetPosition = _initialPosition + new Vector2(_xoffset, _yoffset);
		_tween.TweenProperty(this, "global_position", targetPosition, _duration).SetTrans(_tweenType).SetEase(_easeType);
		//_tween.Parallel().TweenProperty(this, "global_rotation_degrees", _initialRotation + (_rotationClockwise ? _rotationOffset : -_rotationOffset), _duration).SetTrans(_tweenType).SetEase(_easeType);
		if (_reverse)
		{
			_tween.TweenCallback(Callable.From(() => ReverseMove()));
		}
	}
	public void ReverseMove()
    {
		_tween = CreateTween();
		_tween.TweenProperty(this, "global_position", _initialPosition, _duration).SetTrans(_tweenType).SetEase(_easeType);
		//_tween.Parallel().TweenProperty(this, "global_rotation_degrees", _rotationOffset > 180 ? _initialRotation - 360 : _initialRotation, _duration).SetTrans(_tweenType).SetEase(_easeType);
		_tween.TweenCallback(Callable.From(() => StartMove()));
	}
    

    public override void _Draw()
    {
		if (Engine.IsEditorHint())
		{
			Vector2 start = Vector2.Zero;
			Vector2 end = new Vector2(_xoffset, _yoffset);
			DrawLine(start, end, Colors.Blue, 1);
			DrawLine(end + new Vector2(4, 0), end + new Vector2(-4, 0), Colors.White, 1);
			DrawLine(end + new Vector2(0, 4), end + new Vector2(0, -4), Colors.White, 1);

			if (RotationOffset != 0)
            {
                DrawArc(start, 20, 0, Mathf.DegToRad(RotationOffset), 16, Colors.OrangeRed, 1);
            }
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	
}
