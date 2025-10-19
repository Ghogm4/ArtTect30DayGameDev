using Godot;
using System;

public partial class NormalAltar : Node2D
{
	[Export] public Sprite2D AltarSprite;
	[Export] public DropTable BoostDropTable;
	[Export] public EnemyWaveController LinkedEnemyWaveController;
	private bool _isInteractable = false;
	private bool _isPlayerNearby = false;
	public override void _Ready()
	{
		LinkedEnemyWaveController.AllEnemiesDefeated += () => _isInteractable = true;
	}
	public void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		if (_isInteractable)
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
		if (_isInteractable && _isPlayerNearby && Input.IsActionJustPressed("Interact"))
		{
			BoostDropTable.Drop();
			_isInteractable = false;
			ToggleWhiteOutline(false);
		}
	}
	private void ToggleWhiteOutline(bool enabled)
	{
		ShaderMaterial altarMaterial = AltarSprite.Material as ShaderMaterial;
		altarMaterial.SetShaderParameter("outline_enabled", enabled);
	}
}
