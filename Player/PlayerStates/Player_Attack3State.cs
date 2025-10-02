using Godot;
using System;

public partial class Player_Attack3State : State
{
    private AnimatedSprite2D _sprite = null;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
    }
    protected override void Enter()
    {
        _sprite.Play("Attack3");
    }
}
