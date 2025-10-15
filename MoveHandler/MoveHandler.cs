using Godot;
using System;

[GlobalClass]
[Tool]

public partial class MoveHandler : StaticBody2D
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
			QueueRedraw();
		}
	}

	[Export] public bool AutoStart = true;
	

	private Vector2 _initialPosition;
	private float _initialRotation;
	private Tween _tween;

	private Vector2 _startPos;
	private float _startRot;
	private Vector2 _targetPos;
	private float _targetRot;
	private float _elapsedTime = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_initialPosition = GlobalPosition;
		_initialRotation = GlobalRotationDegrees;
		_startPos = _initialPosition;
		_targetPos = _initialPosition + new Vector2(_xoffset, _yoffset);
		_startRot = _initialRotation;
		_targetRot = _initialRotation + (_rotationClockwise ? _rotationOffset : -_rotationOffset);

		if (AutoStart) StartMove();
	}

	public void StartMove()
	{
		_elapsedTime = 0f;
	}

	public void ReverseMove()
	{
		(_startPos, _targetPos) = (_targetPos, _startPos);
    	(_startRot, _targetRot) = (_targetRot, _startRot);
		_elapsedTime = 0f;
    }

	public override void _PhysicsProcess(double delta)
	{
		if (_duration <= 0) return;

		_elapsedTime += (float)delta;
		float t = Mathf.Clamp(_elapsedTime / _duration, 0f, 1f);
		float easedT = Ease(t, _tweenType);

		Vector2 currentPos = _startPos.Lerp(_targetPos, easedT);
		float currentRot = Mathf.LerpAngle(_startRot, _targetRot, easedT);

		GlobalPosition = currentPos;
		GlobalRotationDegrees = currentRot;
		

		if (t >= 1f)
		{
			if (_reverse)
			{
				ReverseMove();
			}

			else if (_loop)
			{
				StartMove();
			}
		}

	}

	private float Ease(float t, Tween.TransitionType type)
    {
        switch (type)
        {
            case Tween.TransitionType.Linear:
				return t;
			case Tween.TransitionType.Sine:
				return Mathf.Sin(t * Mathf.Pi / 2);
			case Tween.TransitionType.Quad:
				return t * t;
			case Tween.TransitionType.Cubic:
				return t * t * t;
			default:
				return t;
        }
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
