using Godot;
using System;
using Godot.Collections;
using System.Threading.Tasks;
public partial class NormalAltar : Node2D, ISavable
{
	[Export] public Sprite2D AltarSprite;
	[Export] public DropTable BoostDropTable;
	[Export] public EnemyWaveController LinkedEnemyWaveController;
	[Export] public bool NeedToCompleteWaves = true;
	[Export] public bool DebugMode = false;
	public string UniqueID => Name;
	private bool _isInteractable
	{
		get
		{
			return field;
		}
		set
		{
			field = value;
			if (value && _isPlayerNearby && !_isClaimed)
				ToggleWhiteOutline(true);
		}
	} = false;
	private bool _isPlayerNearby = false;
	private bool _isClaimed = false;
	public override async void _Ready()
	{
		if (!DebugMode)
		{
			BaseLevel baseLevel = GetTree().CurrentScene as BaseLevel;
			if (baseLevel != null)
				await ToSignal(baseLevel, BaseLevel.SignalName.LevelInitialized);
		}
		LinkedEnemyWaveController?.AllWavesCompleted += () => _isInteractable = true;
		if (!NeedToCompleteWaves)
			_isInteractable = true;
	}
	public void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		_isPlayerNearby = true;
		if (_isInteractable && !_isClaimed)
			ToggleWhiteOutline(true);
	}
	public void OnBodyExited(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		_isPlayerNearby = false;
		ToggleWhiteOutline(false);
	}
	public override void _Process(double delta)
	{
		if (_isInteractable && !_isClaimed && _isPlayerNearby && Input.IsActionJustPressed("Interact"))
		{
			BoostDropTable.Drop();
			_isInteractable = false;
			ToggleWhiteOutline(false);
			_isClaimed = true;
		}
	}
	private void ToggleWhiteOutline(bool enabled)
	{
		ShaderMaterial altarMaterial = AltarSprite.Material as ShaderMaterial;
		altarMaterial.SetShaderParameter("outline_enabled", enabled);
	}
	public Dictionary SaveState()
	{
		return new()
		{
			["IsClaimed"] = _isClaimed
		};
	}

	public void LoadState(Dictionary state)
	{
		if (state?.TryGetValue("IsClaimed", out var isClaimed) ?? false)
			_isClaimed = (bool)isClaimed;
	}
}
