using Godot;
using System;

public partial class DarkSoul : EnemyBase
{
    protected override void CheckDeath(float newValue)
    {
        if (newValue <= 0)
            IsDead = true;
    }
}
