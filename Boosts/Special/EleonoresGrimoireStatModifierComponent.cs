using Godot;
using System;

public partial class EleonoresGrimoireStatModifierComponent : StatModifierComponent
{
    public static float DamageMultiplier = 0.2f;
    private static void CreateFireball(PlayerStatComponent ps, Vector2 pos, float? radian = null)
    {
        Fireball fireball = Projectile.Factory.CreateFriendly<Fireball>("Fireball");
        fireball.GlobalPosition = pos;
        float fireballRadian = radian ?? (float)GD.RandRange(0, Mathf.Tau);
        fireball.Velocity = fireball.BaseSpeed * Vector2.Right.Rotated(fireballRadian) * (float)GD.RandRange(1f, 1.3f);
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
            for (int i = 0; i < 6; i++)
            {
                float spread = Mathf.Pi / 3f;
                float radianOffset = (float)GD.RandRange(-spread, spread);
                float radian = Mathf.Pi / 2 + radianOffset;
                CreateFireball(ps, pos, radian);
            }
        });
        playerStats.OnDashActions.Add((ps, pos) =>
        {
            for (int i = 0; i < 6; i++)
            {
                bool headingLeft = ps.GetStatValue("HeadingLeft") > 0;
                float spread = Mathf.Pi / 7f;
                float radianOffset = (float)GD.RandRange(-spread, spread);
                float radian = (headingLeft ? Mathf.Pi : 0) + radianOffset;
                CreateFireball(ps, pos, radian);
            }
        });
        playerStats.OnEnemyDeathActions.Add((enemy, ps) =>
        {
            for (int i = 0; i < 4; i++)
                CreateFireball(ps, enemy.GlobalPosition);
        });
        playerStats.OnAttackActions.Add((ps, pos) =>
        {
            for (int i = 0; i < 2; i++)
            {
                bool headingLeft = ps.GetStatValue("HeadingLeft") > 0;
                float spread = Mathf.Pi / 10f;
                float radianOffset = (float)GD.RandRange(-spread, spread);
                float radian = (headingLeft ? Mathf.Pi : 0) + radianOffset;
                CreateFireball(ps, pos, radian);
            }
        });
    }
}
