using Godot;
using System;
using GDDictionary = Godot.Collections.Dictionary;
public partial class Mage : Node2D, ISavable
{
	[Export] public AnimatedSprite2D MageSprite;
	[Export] public DropTable FreeBoostDropTable;
	public string UniqueID => Name;
	private bool _isPlayerNearby = false;
	private bool _hasTalkedBefore = false;
	public override void _Ready()
	{
		TextManager.Instance.Connect(TextManager.SignalName.DialogueEnded, Callable.From(OnDialogueEnded), (uint)ConnectFlags.OneShot);
	}
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
				TextManager.Instance.RunLines("res://NPCs/Mage/MageDialogue.json", "MageFirstTime");
			else
				TextManager.Instance.RunLines("res://NPCs/Mage/MageDialogue.json", "MageRepeat");
		}
	}
	private void ToggleWhiteOutline(bool enabled)
	{
		ShaderMaterial material = MageSprite.Material as ShaderMaterial;
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
	private void OnDialogueEnded()
	{
		if (TextManager.Instance.CurrentDialogueScene == "MageFirstTime")
		{
			FreeBoostDropTable.Drop();
			_hasTalkedBefore = true;
		}
	}
}
