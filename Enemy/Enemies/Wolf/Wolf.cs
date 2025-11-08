using Godot;
using System;
using System.Threading.Tasks;
public partial class Wolf : EnemyBase
{
	[Export] public Wolf_MoveControlState state;
	protected override async Task OnDeath()
	{
		state._maxFallSpeed = 0f;
		GD.Print("Wolf Enemy detected Death");
		Sprite.Play("Die");
		await ToSignal(Sprite, "animation_finished");
		return;
	}
}   