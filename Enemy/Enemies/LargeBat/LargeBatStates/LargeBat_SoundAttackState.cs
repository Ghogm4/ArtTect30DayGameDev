using Godot;
using System;

public partial class LargeBat_SoundAttackState : State
{
    [Export] public PackedScene SoundScene = null;
    private Vector2 PlayerPos
    {
        get
        {
            Player player = Storage.GetNode<Player>("Player");
            return player?.GlobalPosition ?? Vector2.Zero;
        }
    }
    private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
    }
    protected override void Enter()
    {
        _sprite.Play("SoundAttack");
        _enemy.Velocity = Vector2.Zero;
        _sprite.AnimationFinished += OnAnimationFinished;
    }
    protected override void Exit()
    {
        _sprite.AnimationFinished -= OnAnimationFinished;
    }
    private void CreateSound(float radian)
    {
        Sound sound = SoundScene.Instantiate<Sound>();
        Vector2 enemyPos = _enemy.GlobalPosition;
        sound.GlobalPosition = enemyPos;
        sound.Velocity = Vector2.Right.Rotated(radian) * sound.BaseSpeed;
        GetTree().CurrentScene.CallDeferred(MethodName.AddChild, sound);
    }
    private void OnAnimationFinished()
    {
        Vector2 enemyPos = _enemy.GlobalPosition;
        float direction = (PlayerPos - _enemy.GlobalPosition).Angle();
        float spread = Mathf.Pi / 30f;
        float[] radians = [direction, direction + spread, direction - spread];
        foreach (float radian in radians)
            CreateSound(radian);
        AskTransit("Decision");
    }
}
