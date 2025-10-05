using Godot;
using System;

public partial class TitleLabel : Label
{
	[Export] public float Distance = 10.0f;
	[Export] public float MoveSpeed = 2f;
	private Vector2 _initialPos = Vector2.Zero;
	private float _elapsedTime = 0;

	public override void _Ready()
	{
		_initialPos = Position;
	}
	public override void _Process(double delta)
	{
		_elapsedTime += (float)delta;
		Vector2 newPos = Position;
		newPos.Y = _initialPos.Y + Distance * Mathf.Sin(_elapsedTime * MoveSpeed);
		Position = newPos;
	}
}
