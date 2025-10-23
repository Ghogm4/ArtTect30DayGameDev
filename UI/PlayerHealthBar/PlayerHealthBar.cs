using Godot;
using System;

public partial class PlayerHealthBar : Control
{
    public static PlayerHealthBar Instance;
    private Vector2 _textureSize = new Vector2(18, 16);
    private ShaderMaterial _healthPotionCooldownShader = null;
    [Export] private TextureRect _fullHearts = null;
    [Export] private TextureRect _emptyHearts = null;
    [Export] private TextureRect _shields = null;
    [Export] private Label _coinAmountLabel = null;
    [Export] private Label _healthPotionAmountLabel = null;
    [Export] private Panel _healthPotionCooldownPanel = null;
    [Export] private Timer _healthPotionCooldownTimer = null;
    public override void _Ready()
    {
        Instance = this;
        SignalBus.Instance.PlayerHealthStatusUpdated += Update;
        SignalBus.Instance.PlayerDied += () => Visible = false;
        _healthPotionCooldownShader = _healthPotionCooldownPanel.Material as ShaderMaterial;
        SignalBus.Instance.PlayerHealthPotionUsed += OnHealthPotionUsed;
    }
    public void Update(int health = -1, int maxHealth = -1, int shield = -1, int coin = -1, int healthPotion = -1)
    {
        _fullHearts.Size = new Vector2(_textureSize.X * health, _textureSize.Y);
        _emptyHearts.Size = new Vector2(_textureSize.X * maxHealth, _textureSize.Y);
        _shields.Size = new Vector2(_textureSize.X * shield, _textureSize.Y);
        _coinAmountLabel.Text = coin.ToString();
        _healthPotionAmountLabel.Text = healthPotion.ToString();
    }
    public override void _Process(double delta)
    {
        _healthPotionCooldownShader.SetShaderParameter(
            "currentAngle",
            Mathf.Lerp(-90f, 269f, 1f - (float)_healthPotionCooldownTimer.TimeLeft / _healthPotionCooldownTimer.WaitTime));
    }

    private void OnHealthPotionUsed(int cooldownSeconds)
    {
        _healthPotionCooldownTimer.WaitTime = cooldownSeconds;
        _healthPotionCooldownTimer.Start();
    }
}
