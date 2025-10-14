using Godot;
using System;

public partial class Testenemy : Node2D
{
	public override void _Ready()
	{
		EnemyGenerator.Instance.LoadEnemy("GreenSlime", "res://Enemy/Enemies/GreenSlime/GreenSlime.tscn");
		EnemyGenerator.Instance.LoadEnemy("RedSlime", "res://Enemy/Enemies/RedSlime/RedSlime.tscn");
		EnemyGenerator.Instance.SummonEnemy("GreenSlime", new Vector2(520, 714));
		EnemyGenerator.Instance.SummonEnemy("RedSlime", new Vector2(510, 714));
		EnemyGenerator.Instance.SummonEnemy("RedSlime", new Vector2(510, 774));
		EnemyGenerator.Instance.SummonEnemy("RedSlime", new Vector2(510, 754));
		EnemyGenerator.Instance.SummonEnemy("RedSlime", new Vector2(510, 734));
	}
}
