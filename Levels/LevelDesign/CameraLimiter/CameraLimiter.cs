using Godot;
using System;
[GlobalClass, Tool]
public partial class CameraLimiter : Marker2D
{
	public enum LimitType
	{
		TopLeft,
		BottomRight,
	}
	[Export]
	public LimitType Type
	{
		get => _type;
		set
		{
			_type = value;
			QueueRedraw();
		}
	}
	private LimitType _type = LimitType.TopLeft;
	[Export] public Color LineColor = Colors.Red;
	private float _lineLength = 1000.0f;
	[Export]
	public float LineLength
	{
		get => _lineLength;
		set { _lineLength = value; QueueRedraw(); }
	}
	private float _lineThickness = 2.0f;
	[Export] public float LineThickness {
		get => _lineThickness;
		set { _lineThickness = value; QueueRedraw(); }
	}

	public override void _Draw()
	{
		if (Engine.IsEditorHint())
		{
			if (Type == LimitType.TopLeft)
			{
				DrawLine(Vector2.Zero, Vector2.Right * LineLength, LineColor, LineThickness);
				DrawLine(Vector2.Zero, Vector2.Down * LineLength, LineColor, LineThickness);
			}
			else
			{
				DrawLine(Vector2.Zero, Vector2.Left * LineLength, LineColor, LineThickness);
				DrawLine(Vector2.Zero, Vector2.Up * LineLength, LineColor, LineThickness);
			}
		}
	}
	public override void _Ready()
    {
		Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
		if (player is null)
		{
			GD.PushError("CameraLimiter: Player node not found in the scene tree.");
			return;
		}
		Camera2D camera = player.GetNode<Camera2D>("Camera2D");
		if (Type == LimitType.TopLeft)
		{
			camera.LimitLeft = (int)GlobalPosition.X;
			camera.LimitTop = (int)GlobalPosition.Y;
		}
		else if (Type == LimitType.BottomRight)
		{
			camera.LimitRight = (int)GlobalPosition.X;
			camera.LimitBottom = (int)GlobalPosition.Y;
		}
    }
}
