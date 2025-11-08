using Godot;
using System;
using System.Threading.Tasks;

public partial class BringerOfDeath : EnemyBase
{
    protected override async Task OnDeath()
    {
        await ToSignal(Sprite, AnimatedSprite2D.SignalName.AnimationFinished);
    }
    protected override void CheckDeath(float newValue)
    {
        if (newValue <= 0)
            IsDead = true;
    }
}
