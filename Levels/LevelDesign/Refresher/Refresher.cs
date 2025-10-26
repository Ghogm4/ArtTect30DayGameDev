using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GDDictionary = Godot.Collections.Dictionary;
public partial class Refresher : Node2D, ISavable
{
	[Export] public Label PriceTag;
	[Export] public ShopItem[] ShopItemsInLevel = [];
	public string UniqueID => Name;
	private int _usedTimes = 0;
	private int Price => Convert.ToInt32(50 * (_usedTimes * 0.5f + 1));
	private bool _isPlayerNearby = false;
	public override async void _Ready()
	{
		await ToSignal(GetTree().CurrentScene as BaseLevel, BaseLevel.SignalName.LevelInitialized);
		UpdatePriceTag();
	}
	public void OnBodyEntered(Node2D body)
	{
		_isPlayerNearby = true;
		if (!body.IsInGroup("Player"))
			return;
		ToggleWhiteOutline(true);
	}
	public void OnBodyExited(Node2D body)
	{
		_isPlayerNearby = false;
		if (!body.IsInGroup("Player"))
			return;
		ToggleWhiteOutline(false);
	}
	private void ToggleWhiteOutline(bool enabled)
	{
		ShaderMaterial refresherMaterial = GetNode<Sprite2D>("RefresherSprite").Material as ShaderMaterial;
		refresherMaterial.SetShaderParameter("outline_enabled", enabled);
	}
	private int GetPlayerCoin()
	{
		Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
		if (player is null) return -1;
		return (int)player.GetNode<PlayerStatComponent>("StatComponent").GetStatValue("Coin");
	}
	public override void _Process(double delta)
	{
		if (_isPlayerNearby && Input.IsActionJustPressed("Interact") && GetPlayerCoin() >= Price)
		{
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerPurchased, Price);
			_usedTimes++;
			UpdatePriceTag();
			foreach (ShopItem shopItem in ShopItemsInLevel)
				shopItem.Refresh();
		}
	}
	public GDDictionary SaveState()
	{
		return new()
		{
			["UsedTimes"] = _usedTimes
		};
	}
	public void LoadState(GDDictionary state)
	{
		if (state?.TryGetValue("UsedTimes", out var usedTimes) ?? false)
			_usedTimes = (int)usedTimes;
	}
	private void UpdatePriceTag()
	{
		PriceTag.Text = Price.ToString();
	}
}
