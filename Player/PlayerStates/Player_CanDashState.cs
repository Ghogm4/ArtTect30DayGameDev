using Godot;
using System;

public partial class Player_CanDashState : State
{
	private int AvailableDashes
	{
		get => (int)Stats.GetStatValue("AvailableDashes");
		set => Stats.GetStat("AvailableDashes").AddFinal(value - (int)Stats.GetStatValue("AvailableDashes"));
	}
	protected override void PhysicsUpdate(double delta)
	{
		if (AvailableDashes > 0 && Input.IsActionJustPressed("Dash"))
		{
			AskTransit("Dash");
			AvailableDashes--;
			GetTree().CreateTimer(Stats.GetStatValue("DashCooldown")).Timeout += () =>
			{
				if (AvailableDashes < (int)Stats.GetStatValue("MaxDash"))
					AvailableDashes++;
			};
		}
    }
}