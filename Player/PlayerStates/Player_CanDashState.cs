using Godot;
using System;

public partial class Player_CanDashState : State
{
	protected override void PhysicsUpdate(double delta)
	{
		if (Input.IsActionJustPressed("Dash"))
			AskTransit("Dash");
    }
}