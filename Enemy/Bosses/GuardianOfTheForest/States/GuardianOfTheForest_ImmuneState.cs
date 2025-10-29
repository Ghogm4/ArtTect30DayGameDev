using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;
public partial class GuardianOfTheForest_ImmuneState : State
{
    [Export] public float SummonRadius = 50f;
    [Export] public float SummonInterval = 0.5f;
    [Export] public float SummonDuration = 5f;
    [Export] public PackedScene[] SummonList = [];
    private EnemyBase _enemy = null;
    private AnimatedSprite2D _sprite = null;
    private float _previousDamageReduction = 0;
    private float _timeElapsed = 0f;
    private bool _isSummoning = false;
    protected override void ReadyBehavior()
    {
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
    }
    protected override void Enter()
    {
        _sprite.Play("Immune");
        _previousDamageReduction = Stats.GetStatValue("DamageReduction");
        Stats.SetValue("DamageReduction", 0.95f);
        _sprite.AnimationFinished += OnAnimationFinished;
        _enemy.Velocity = Vector2.Zero;
        GetTree().CreateTimer(SummonDuration).Timeout += () =>
        {
            if (!_enemy.IsDead)
                AskTransit("Decision");
        };
    }
    private Vector2 GetRandomPosition(Vector2 pivot)
    {
        Vector2 offset = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau)) * (float)GD.RandRange(0, SummonRadius);
        return pivot + offset;
    }
    protected override void FrameUpdate(double delta)
    {
        if (!_isSummoning) return;

        _timeElapsed += (float)delta;
        if (_timeElapsed >= SummonInterval)
        {
            Vector2 _randomPos = GetRandomPosition(_enemy.GlobalPosition + Vector2.Up * 30);
            _timeElapsed -= SummonInterval;
            PackedScene enemyScene = Probability.RunUniformChoose(SummonList);
            EnemyBase enemy = enemyScene.Instantiate<EnemyBase>();
            enemy.GlobalPosition = _randomPos;
            enemy.WillDropItems = false;
            GetTree().CurrentScene.CallDeferred(MethodName.AddChild, enemy);
        }
    }
    protected override void Exit()
    {
        Stats.SetValue("DamageReduction", _previousDamageReduction);
        _isSummoning = false;
        _sprite.AnimationFinished -= OnAnimationFinished;
    }
    private void OnAnimationFinished() => _isSummoning = true;
}
