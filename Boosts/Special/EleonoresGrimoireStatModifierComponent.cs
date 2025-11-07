using Godot;
using System;

public partial class EleonoresGrimoireStatModifierComponent : StatModifierComponent
{
    public static float DamageMultiplier = 0.5f;
    private static void CreateFireball(PlayerStatComponent ps, Vector2 pos)
    {
        Fireball fireball = Projectile.Factory.CreateFriendly<Fireball>("Fireball");
        fireball.GlobalPosition = pos;
        fireball.Velocity = fireball.BaseSpeed * Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
        float attack = ps.GetStatValue("Attack");
        float attackBase = ps.GetStatValue("AttackBase");
        float attackMult = ps.GetStatValue("AttackMult");
        float attackFinal = ps.GetStatValue("AttackFinal");
        fireball.Damage = ((attack + attackBase) * attackMult + attackFinal) * ps.GetStatValue("ProjectileDamageMultiplier") * DamageMultiplier;
        ps.GetTree().CurrentScene.CallDeferred(MethodName.AddChild, fireball);
    }
    protected override void Modify(StatComponent statComponent, bool reverse = false)
    {
        PlayerStatComponent playerStats = statComponent as PlayerStatComponent;
        if (playerStats == null) return;
        playerStats.OnJumpActions.Add((ps, pos) =>
        {
            for (int i = 0; i < 5; i++)
                CreateFireball(ps, pos);
        });
        playerStats.OnDashActions.Add((ps, pos) =>
        {
            for (int i = 0; i < 5; i++)
                CreateFireball(ps, pos);
        });
        playerStats.OnEnemyDeathActions.Add((enemy, ps) =>
        {
            for (int i = 0; i < 3; i++)
                CreateFireball(ps, enemy.GlobalPosition);
        });
    }
}
