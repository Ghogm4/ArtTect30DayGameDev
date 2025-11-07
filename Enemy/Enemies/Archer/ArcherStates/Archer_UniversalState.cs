using Godot;
using System;
using System.Threading.Tasks;
public partial class Archer_UniversalState : State
{
    private EnemyBase _enemy = null;
    private AnimatedSprite2D _sprite = null;

    private bool _isChasing = false;
    protected override void ReadyBehavior()
    {
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        Storage.RegisterVariant<bool>("HeadingLeft", false);
    }

    protected override void FrameUpdate(double delta)
    {
        bool headingLeft = Storage.GetVariant<bool>("HeadingLeft");
        if (headingLeft)
            _sprite.FlipH = true;
        else
        {
            _sprite.FlipH = false;
        }
    }

    
}
