using Godot;
using System;

public partial class TravellingMerchant : Node2D
{
	[Export] public AnimatedSprite2D MerchantSprite;
	private bool _isPlayerNearby = false;
	public override void _Ready()
    {
        TextManager.Instance.LoadLines("res://NPCs/TravellingMerchant/TravellingMerchantDialogue.json", "TravellingMerchant");
    }
	public void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
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
		if (_isPlayerNearby && Input.IsActionJustPressed("Interact"))
			TextManager.Instance.StartDialogue();
	}
	private void ToggleWhiteOutline(bool enabled)
	{
		ShaderMaterial material = MerchantSprite.Material as ShaderMaterial;
		material.SetShaderParameter("outline_enabled", enabled);
	}
}
