using Godot;
using System;

public partial class Projectile : Node2D
{
	[Export] public Area2D Hitbox;
	[Export] public bool PierceWorld = false;
	[Export] public int PierceEnemyCount = 1;
	[Export] public float BaseSpeed = 400f;
	public enum ProjectileType
	{
		Friendly, Hostile
	}
	[Export] public ProjectileType Type = ProjectileType.Friendly;
	public class Factory
    {
		public static T CreateFriendly<T>(string projectileName) where T : Projectile
		{
			PackedScene projectileScene = ResourceLoader.Load<PackedScene>($"res://Projectiles/Friendly/{projectileName}.tscn");
			return projectileScene.Instantiate<T>();
		}
		public static T CreateHostile<T>(string projectileName) where T : Projectile
		{
			PackedScene projectileScene = ResourceLoader.Load<PackedScene>($"res://Projectiles/Hostile/{projectileName}.tscn");
			return projectileScene.Instantiate<T>();
		}
    }
	protected virtual void ReadyBehavior() { }
	public virtual void HitBehavior(Node2D body) { }
	public override void _Ready()
	{
		if (Type == ProjectileType.Friendly)
			Hitbox.SetCollisionMaskValue(3, true);
		else
			Hitbox.SetCollisionMaskValue(2, true);

		if (!PierceWorld)
			Hitbox.SetCollisionMaskValue(1, true);
		ReadyBehavior();
	}
}
