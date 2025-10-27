using Godot;
using System;
using GDDictionary = Godot.Collections.Dictionary;
public partial class Blacksmith : Node2D, ISavable
{
	[Export] public AnimatedSprite2D BlacksmithSprite;
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
	}
	public override void _Process(double delta)
	{
		if (_isPlayerNearby && Input.IsActionJustPressed("Interact"))
		{
			if (!_hasTalkedBefore)
				TextManager.Instance.RunLines("res://NPCs/Blacksmith/BlacksmithDialogue.json", "BlacksmithFirstTime");
			else
				TextManager.Instance.RunLines("res://NPCs/Blacksmith/BlacksmithDialogue.json", "BlacksmithRepeat");

			_hasTalkedBefore = true;
		}
	}
	private void ToggleWhiteOutline(bool enabled)
	{
		ShaderMaterial material = BlacksmithSprite.Material as ShaderMaterial;
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
