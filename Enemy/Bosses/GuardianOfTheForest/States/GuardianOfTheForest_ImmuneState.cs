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
    private CancellationTokenSource _cancellationTokenSource;
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
        _cancellationTokenSource = new();
        GetTree().CreateTimer(SummonDuration).Timeout += () => AskTransit("Normal");
    }
    private Vector2 GetRandomPosition(Vector2 pivot)
    {
        Vector2 offset = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau)) * (float)GD.RandRange(0, SummonRadius);
        return pivot + offset;
    }
    protected override async void FrameUpdate(double delta)
    {
        if (!_isSummoning || _cancellationTokenSource.IsCancellationRequested) return;

        _timeElapsed += (float)delta;
        if (_timeElapsed >= SummonInterval)
        {
            Vector2 _randomPos = GetRandomPosition(_enemy.GlobalPosition + Vector2.Up * 30);
            _timeElapsed -= SummonInterval;
            PackedScene enemyScene = Probability.RunUniformChoose(SummonList);
            
            try
            {
                await MakeEnemySpawnHint(_randomPos, _cancellationTokenSource.Token);
                
                if (!IsInstanceValid(_enemy) || _cancellationTokenSource.IsCancellationRequested)
                    return;
                
                EnemyBase enemy = enemyScene.Instantiate<EnemyBase>();
                enemy.GlobalPosition = _randomPos;
                enemy.WillDropItems = false;
                GetTree().CurrentScene.CallDeferred(MethodName.AddChild, enemy);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
    protected override void Exit()
    {
        Stats.SetValue("DamageReduction", _previousDamageReduction);
        _isSummoning = false;
        _sprite.AnimationFinished -= OnAnimationFinished;
        
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
    private async Task MakeEnemySpawnHint(Vector2 position, CancellationToken cancellationToken)
    {
        Sprite2D iconSpriteNode = new();
        iconSpriteNode.Texture = ResourceLoader.Load<Texture2D>("res://Assets/Special Tiles/EnemySpawnIcon/EnemySpawnIcon.png");
        iconSpriteNode.Position = position;
        GetTree().CurrentScene.CallDeferred(Node.MethodName.AddChild, iconSpriteNode);
        float scaleFactor = 0.5f;
        iconSpriteNode.Scale *= scaleFactor;
        
        Tween tween = iconSpriteNode.CreateTween();
        tween.SetLoops(3);
        tween.TweenProperty(iconSpriteNode, "modulate:a", 0f, 0.1f);
        tween.TweenProperty(iconSpriteNode, "modulate:a", 1f, 0.1f);
        
        while (tween.IsRunning())
        {
            if (cancellationToken.IsCancellationRequested || !IsInstanceValid(iconSpriteNode))
            {
                if (IsInstanceValid(iconSpriteNode))
                    iconSpriteNode.QueueFree();
                cancellationToken.ThrowIfCancellationRequested();
            }
            await Task.Delay(16, cancellationToken); // ~60 FPS
        }
        
        if (IsInstanceValid(iconSpriteNode))
            iconSpriteNode.QueueFree();
    }
    private void OnAnimationFinished() => _isSummoning = true;
}
