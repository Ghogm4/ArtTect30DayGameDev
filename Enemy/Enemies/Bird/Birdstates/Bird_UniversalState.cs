using Godot;
using System;
using System.Threading.Tasks;

public partial class Bird_UniversalState : State
{
    private EnemyBase _enemy = null;
    private StatWrapper _health;
    private AnimatedSprite2D _sprite;

    protected override void ReadyBehavior()
    {
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _health = new(Stats.GetStat("Health"));
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
    }
}