using Godot;
using System;
using System.Threading.Tasks;

public partial class GuardianOfTheForest : EnemyBase
{
    [Export] public Node2D DropTables;
    private int PlayerHitTimes
    {
        get => Storage.GetVariant<int>("PlayerHitTimes");
        set => Storage.SetVariant("PlayerHitTimes", value);
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage * Mathf.Clamp(1f - Stats.GetStatValue("DamageReduction"), 0f, 1f));
    }
    protected override void DisplayDamageText(float damage)
    {
        if (Storage.GetVariant<bool>("IsInDieState"))
            return;
        base.DisplayDamageText(damage);
    }
    protected override void CheckDeath(float newValue)
    {
        if (newValue <= 0f)
            IsDead = true;
    }
    protected override async Task OnDeath()
    {
        await base.OnDeath();

        DropTables.GetNode<DropTable>("DropTableLowTier").Drop();
        DropTables.GetNode<DropTable>("DropTableHighTier").Drop();
        DropTables.GetNode<DropTable>("DropTableGeneral").Drop();

        DropTable dropTableBonus = DropTables.GetNode<DropTable>("DropTableBonus");
        dropTableBonus.MinTimesToRun -= Mathf.Min(PlayerHitTimes, dropTableBonus.MinTimesToRun);
        dropTableBonus.MaxTimesToRun -= Mathf.Min(PlayerHitTimes, dropTableBonus.MaxTimesToRun);
        dropTableBonus.Drop();
    }
    protected override void AfterDamageRequestSent() => PlayerHitTimes++;
}
