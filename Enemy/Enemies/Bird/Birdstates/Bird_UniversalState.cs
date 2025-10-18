using Godot;
using System;
using System.Threading.Tasks;

public partial class Bird_UniversalState : State
{
    private EnemyBase _enemy = null;
    private StatWrapper _health;
    private AnimatedSprite2D _sprite;

    private float preHealth;
    protected override void ReadyBehavior()
    {
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _health = new(Stats.GetStat("Health"));
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");

        Stats.GetStat("Health").StatChanged += OnHealthChanged;
        preHealth = (float)_health;
    }

    private async void OnHealthChanged()
    {
        GD.Print((float)_health);
        if ((float)_health < preHealth)
        {
            GetHit();
            GD.Print("Bird got hit!");
        }
        preHealth = (float)_health;


        if ((float)_health <= 0)
        {
            await Die();
        }
    }

    private async Task Die()
    {
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        _enemy.QueueFree();
    }
    
    private void GetHit()
    {
        if (_sprite != null)
        {
            Tween _tween = _enemy.CreateTween();
            _tween.TweenProperty(
                _sprite, "modulate", 
                new Color(1, 0.5f, 0.5f), 
                0.1f
            ).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
                _tween.TweenProperty(
                _sprite, "modulate", 
                new Color(1, 1, 1), 
                0.3f
            ).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
        }
    }
}