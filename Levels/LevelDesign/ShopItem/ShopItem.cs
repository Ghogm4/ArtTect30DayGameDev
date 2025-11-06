using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodotDictionary = Godot.Collections.Dictionary;
public partial class ShopItem : Node2D, ISavable
{
	[Export] public int BasePrice = 75;
	[Export] public float PriceVariancePercentage = 0.2f;
	[Export] public float PriceRarityFactorDepression = 0f;
	[Export] public DropTable ItemDropTable;
	[Export] public Sprite2D ItemSprite;
	[Export] public Sprite2D DisplayBaseSprite;
	[Export] public Label PriceTag;
	[Export] public HBoxContainer PriceContainer;
	[Export] public bool SkipLevelInitialization = false;
	public string UniqueID => Name;
	private Vector2 _itemSpriteOriginalPosition;
	private float _timeElapsed = 0f;
	private float _hoverAmplitude = 5f;
	private float _hoverFrequency = 2f;
	private Boost _hoveringBoost = null;
	private bool _isPurchased = false;
	private int _purchasedTimes = 0;
	private string _itemSceneFilePath = "";
	private bool _isFirstEntering = true;
	private static readonly Dictionary<BoostRarity, float> BoostRarityBasedPriceMinFactorDict = new()
	{
		[BoostRarity.Common] = (float)BoostRarityBasedPriceMinFactor.Common / 100f,
		[BoostRarity.Uncommon] = (float)BoostRarityBasedPriceMinFactor.Uncommon / 100f,
		[BoostRarity.Rare] = (float)BoostRarityBasedPriceMinFactor.Rare / 100f,
		[BoostRarity.Epic] = (float)BoostRarityBasedPriceMinFactor.Epic / 100f,
		[BoostRarity.Legendary] = (float)BoostRarityBasedPriceMinFactor.Legendary / 100f
	};
	private static readonly Dictionary<BoostRarity, float> BoostRarityBasedPriceMaxFactorDict = new()
	{
		[BoostRarity.Common] = (float)BoostRarityBasedPriceMaxFactor.Common / 100f,
		[BoostRarity.Uncommon] = (float)BoostRarityBasedPriceMaxFactor.Uncommon / 100f,
		[BoostRarity.Rare] = (float)BoostRarityBasedPriceMaxFactor.Rare / 100f,
		[BoostRarity.Epic] = (float)BoostRarityBasedPriceMaxFactor.Epic / 100f,
		[BoostRarity.Legendary] = (float)BoostRarityBasedPriceMaxFactor.Legendary / 100f
	};
	private float GetRandomizedFactor(BoostRarity boostRarity)
	{
		float minFactor = BoostRarityBasedPriceMinFactorDict[boostRarity];
		float maxFactor = BoostRarityBasedPriceMaxFactorDict[boostRarity];
		return Mathf.Pow((float)GD.RandRange(minFactor, maxFactor), 1f / (1f + PriceRarityFactorDepression));
	}
	private int Price => Convert.ToInt32(BasePrice * (1f + PriceVariancePercentage * _purchasedTimes) * (_hoveringBoost?.Info.Rarity switch
	{
		BoostRarity.Common => GetRandomizedFactor(BoostRarity.Common),
		BoostRarity.Uncommon => GetRandomizedFactor(BoostRarity.Uncommon),
		BoostRarity.Rare => GetRandomizedFactor(BoostRarity.Rare),
		BoostRarity.Epic => GetRandomizedFactor(BoostRarity.Epic),
		BoostRarity.Legendary => GetRandomizedFactor(BoostRarity.Legendary),
		_ => BasePrice
	}));
	private int _finalPrice = 0;
	public override async void _Ready()
	{
		_itemSpriteOriginalPosition = ItemSprite.Position;
		if (!SkipLevelInitialization)
		{
			BaseLevel baseLevel = GetTree().CurrentScene as BaseLevel;
			if (baseLevel != null)
				await ToSignal(baseLevel, BaseLevel.SignalName.LevelInitialized);
		}
		if (_isFirstEntering)
		{
			Refresh();
			_isFirstEntering = false;
		}
		else if (!_isPurchased && !string.IsNullOrEmpty(_itemSceneFilePath))
		{
			_hoveringBoost = ResourceLoader.Load<PackedScene>(_itemSceneFilePath).Instantiate<Boost>();
			_hoveringBoost.Visible = false;
			_hoveringBoost.Pickable = false;
			_hoveringBoost.GlobalPosition = ItemSprite.GlobalPosition;
			GetTree().CurrentScene.AddChild(_hoveringBoost);
			PriceTag.Text = _finalPrice.ToString();
			ResetDisplayState();
		}
	}

	public void Refresh()
	{
		if (_hoveringBoost != null && IsInstanceValid(_hoveringBoost))
			_hoveringBoost.QueueFree();

		RunItemDropTable();
		GetDroppedBoost();
		ResetDisplayState();
		TogglePurchase(false);
		SetPrice();
		if (_isPlayerNearby)
			ToggleWhiteOutline(true);
	}
	private void RunItemDropTable()
	{
		ItemDropTable.DroppedBoosts.Clear();
		ItemDropTable.IsBoostPickable = false;
		ItemDropTable.Drop();
	}
	private void GetDroppedBoost()
	{
		if (ItemDropTable.DroppedBoosts.Count > 0)
		{
			_hoveringBoost = ItemDropTable.DroppedBoosts[0];
			_hoveringBoost.Visible = false;
			_itemSceneFilePath = _hoveringBoost.SceneFilePath;
			_hoveringBoost.GlobalPosition = ItemSprite.GlobalPosition;
		}
	}
	private void ResetDisplayState()
	{
		_timeElapsed = 0f;
		ItemSprite.Texture = _hoveringBoost?.Info.Icon;
		ItemSprite.Visible = true;
		PriceContainer.Visible = true;
	}
	private void TogglePurchase(bool purchased)
	{
		_isPurchased = purchased;
	}
	private void SetPrice()
	{
		_finalPrice = Price;
		PriceTag.Text = _finalPrice.ToString();
	}
	private bool _isPlayerNearby = false;
	public void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		_isPlayerNearby = true;
		if (!_isPurchased)
			ToggleWhiteOutline(true);
	}
	public void OnBodyExited(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		_isPlayerNearby = false;
		if (!_isPurchased)
			ToggleWhiteOutline(false);
	}
	private int GetPlayerCoin()
	{
		Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
		if (player is null) return -1;
		return (int)player.GetNode<PlayerStatComponent>("StatComponent").GetStatValue("Coin");
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Use")) Refresh();
		_timeElapsed += (float)delta;
		float hoverOffset = _hoverAmplitude * Mathf.Sin(_hoverFrequency * _timeElapsed);
		ItemSprite.Position = _itemSpriteOriginalPosition + Vector2.Up * hoverOffset;
		if (_isPlayerNearby && Input.IsActionJustPressed("Interact") && !_isPurchased && GetPlayerCoin() >= _finalPrice)
		{
			ToggleWhiteOutline(false);
			EnableBoostPickup();
			HandleDisplayAfterPurchase();
			TogglePurchase(true);
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerPurchased, _finalPrice);
			_purchasedTimes++;
		}
	}
	private void EnableBoostPickup()
	{
		if (_hoveringBoost != null && IsInstanceValid(_hoveringBoost))
		{
			_hoveringBoost.Visible = true;
			_hoveringBoost.GlobalPosition = ItemSprite.GlobalPosition;
			_hoveringBoost.Pickable = true;
			//_hoveringBoost.TreeExiting += () => _hoveringBoost = null;
		}
	}
	private void HandleDisplayAfterPurchase()
	{
		ItemSprite.Visible = false;
		PriceContainer.Visible = false;
	}
	private void ToggleWhiteOutline(bool enabled)
	{
		ShaderMaterial itemMaterial = ItemSprite.Material as ShaderMaterial;
		ShaderMaterial displayBaseMaterial = DisplayBaseSprite.Material as ShaderMaterial;
		itemMaterial.SetShaderParameter("outline_enabled", enabled);
		displayBaseMaterial.SetShaderParameter("outline_enabled", enabled);
	}
	public GodotDictionary SaveState()
	{
		return new()
		{
			["IsPurchased"] = _isPurchased,
			["PurchasedTimes"] = _purchasedTimes,
			["CurrentPrice"] = _finalPrice,
			["ItemSceneFilePath"] = _itemSceneFilePath,
			["IsFirstEntering"] = _isFirstEntering
		};
	}
	public void LoadState(GodotDictionary state)
	{
		if (state?.TryGetValue("IsPurchased", out var isPurchased) ?? false)
			_isPurchased = (bool)isPurchased;
		if (state?.TryGetValue("PurchasedTimes", out var purchasedTimes) ?? false)
			_purchasedTimes = (int)purchasedTimes;
		if (state?.TryGetValue("CurrentPrice", out var currentPrice) ?? false)
			_finalPrice = (int)currentPrice;
		if (state?.TryGetValue("ItemSceneFilePath", out var itemSceneFilePath) ?? false)
			_itemSceneFilePath = (string)itemSceneFilePath;
		if (state?.TryGetValue("IsFirstEntering", out var isFirstEntering) ?? false)
			_isFirstEntering = (bool)isFirstEntering;
		if (_isPurchased)
			HandleDisplayAfterPurchase();
	}
}
