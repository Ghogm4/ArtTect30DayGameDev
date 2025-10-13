using Godot;
using System;

public partial class Testenemy : Node2D
{
	public override void _Ready()
	{
		EnemyGenerator.Instance.LoadEnemy("GreenSlime", "res://Enemy/Enemies/GreenSlime/GreenSlime.tscn");
		EnemyGenerator.Instance.SummonEnemy("GreenSlime", new Vector2(520, 714));
	}
}
