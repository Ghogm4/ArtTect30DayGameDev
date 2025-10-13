using Godot;
using System;

public partial class PlayerHealthBar : CanvasLayer
{
    public static PlayerHealthBar Instance;
    private Vector2 _textureSize = new Vector2(18, 15);
    [Export] private TextureRect _fullHearts = null;
    [Export] private TextureRect _emptyHearts = null;
    [Export] private TextureRect _shields = null;
    public override void _Ready()
    {
        Instance = this;
        SignalBus.Instance.PlayerHealthStatusUpdated += Update;
        SignalBus.Instance.PlayerDied += () => Visible = false;
    }
    public void Update(int health = -1, int maxHealth = -1, int shield = -1)
    {
        _fullHearts.Size = new Vector2(_textureSize.X * health, _textureSize.Y);
        _emptyHearts.Size = new Vector2(_textureSize.X * maxHealth, _textureSize.Y);
        _shields.Size = new Vector2(_textureSize.X * shield, _textureSize.Y);
    }
}
