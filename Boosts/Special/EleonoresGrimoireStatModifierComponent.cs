using Godot;
using System;

public partial class EleonoresGrimoireStatModifierComponent : StatModifierComponent
{
    protected override void Modify(StatComponent statComponent, bool reverse = false)
    {
        PlayerStatComponent playerStats = statComponent as PlayerStatComponent;
        if (playerStats == null) return;
        playerStats.OnJumpActions.Add((ps, pos) =>
        {
            for (int i = 0; i < 5; i++)
            {
                Fireball fireball = Projectile.Factory.CreateFriendly<Fireball>("Fireball");
                fireball.GlobalPosition = pos;
                fireball.Velocity = fireball.BaseSpeed * Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
                float attack = ps.GetStatValue("Attack");
                float attackBase = ps.GetStatValue("AttackBase");
                float attackMult = ps.GetStatValue("AttackMult");
                float attackFinal = ps.GetStatValue("AttackFinal");
                fireball.Damage = (attack + attackBase) * attackMult + attackFinal;
                ps.GetTree().CurrentScene.AddChild(fireball);
            }
        });
        playerStats.OnDashActions.Add((ps, pos) =>
        {
            for (int i = 0; i < 5; i++)
            {
                Fireball fireball = Projectile.Factory.CreateFriendly<Fireball>("Fireball");
                fireball.GlobalPosition = pos;
                fireball.Velocity = fireball.BaseSpeed * Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
                float attack = ps.GetStatValue("Attack");
                float attackBase = ps.GetStatValue("AttackBase");
                float attackMult = ps.GetStatValue("AttackMult");
                float attackFinal = ps.GetStatValue("AttackFinal");
                fireball.Damage = (attack + attackBase) * attackMult + attackFinal;
                ps.GetTree().CurrentScene.AddChild(fireball);
            }
        });
        playerStats.OnEnemyDeathActions.Add((enemy, ps) =>
        {
            for (int i = 0; i < 7; i++)
            {
                Fireball fireball = Projectile.Factory.CreateFriendly<Fireball>("Fireball");
                fireball.GlobalPosition = enemy.GlobalPosition;
                fireball.Velocity = fireball.BaseSpeed * Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
                float attack = ps.GetStatValue("Attack");
                float attackBase = ps.GetStatValue("AttackBase");
                float attackMult = ps.GetStatValue("AttackMult");
                float attackFinal = ps.GetStatValue("AttackFinal");
                fireball.Damage = ((attack + attackBase) * attackMult + attackFinal) * ps.GetStatValue("ProjectileDamageMultiplier");
                ps.GetTree().CurrentScene.AddChild(fireball);
            }
        });
    }
}
