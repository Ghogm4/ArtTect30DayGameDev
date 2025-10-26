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
	public string UniqueID => Name;
	private bool _isInteractable = false;
	private bool _isPlayerNearby = false;
	private bool _isClaimed = false;
	public override void _Ready()
	{
		LinkedEnemyWaveController?.AllWavesCompleted += () => _isInteractable = true;
		if (!NeedToCompleteWaves)
			_isInteractable = true;
	}
	public void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		if (_isInteractable && !_isClaimed)
			ToggleWhiteOutline(true);
		_isPlayerNearby = true;
	}
	public void OnBodyExited(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		ToggleWhiteOutline(false);
		_isPlayerNearby = false;
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
		GD.Print($"NormalAltar {UniqueID} loaded state: IsClaimed = {_isClaimed}");
	}
}
