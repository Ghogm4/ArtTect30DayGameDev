using Godot;
using System;

public partial class PlayerHealthBar : CanvasLayer
{
    public static PlayerHealthBar Instance;
    private Vector2 _textureSize = new Vector2(18, 15);
    private TextureRect _fullHearts = null;
    private TextureRect _emptyHearts = null;
    private TextureRect _shields = null;
    public override void _Ready()
    {
        Instance = this;
        _fullHearts = GetNode<TextureRect>("%FullHearts");
        _emptyHearts = GetNode<TextureRect>("%EmptyHearts");
        _shields = GetNode<TextureRect>("%Shields");
        Update();
        SignalBus.Instance.PlayerHit += Update;
    }
    public void Update()
    {
        int health = GameData.Instance.PlayerHealth;
        int maxHealth = GameData.Instance.PlayerMaxHealth;
        int shield = GameData.Instance.PlayerShield;
        _fullHearts.Size = new Vector2(_textureSize.X * health, _textureSize.Y);
        _emptyHearts.Size = new Vector2(_textureSize.X * maxHealth, _textureSize.Y);
        _shields.Size = new Vector2(_textureSize.X * shield, _textureSize.Y);
    }
}
