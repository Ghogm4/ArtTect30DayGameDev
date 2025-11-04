using Godot;
using System;
using GDDictionary = Godot.Collections.Dictionary;
public partial class TravellingMerchant : Node2D, ISavable
{
	[Export] public AnimatedSprite2D MerchantSprite;
	public string UniqueID => Name;
	private bool _isPlayerNearby = false;
	private bool _hasTalkedBefore = false;
	public void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		_isPlayerNearby = true;
		ToggleWhiteOutline(true);
	}
	public void OnBodyExited(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		_isPlayerNearby = false;
		ToggleWhiteOutline(false);
		TextManager.Instance.EndDialogue();
	}
	public override void _Process(double delta)
	{
		if (_isPlayerNearby && Input.IsActionJustPressed("AdvanceDialogue"))
		{
			if (!_hasTalkedBefore)
				TextManager.Instance.RunLines("res://NPCs/TravellingMerchant/TravellingMerchantDialogue.json", "TravellingMerchantFirstTime");
			else
				TextManager.Instance.RunLines("res://NPCs/TravellingMerchant/TravellingMerchantDialogue.json", "TravellingMerchantRepeat");

			_hasTalkedBefore = true;
		}
	}
	private void ToggleWhiteOutline(bool enabled)
	{
		ShaderMaterial material = MerchantSprite.Material as ShaderMaterial;
		material.SetShaderParameter("outline_enabled", enabled);
	}
	public GDDictionary SaveState()
	{
		return new()
		{
			["HasTalkedBefore"] = _hasTalkedBefore
		};
	}
	public void LoadState(GDDictionary state)
	{
		if (state.TryGetValue("HasTalkedBefore", out var hasTalkedBefore))
			_hasTalkedBefore = (bool)hasTalkedBefore;
	}
}
