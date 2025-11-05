using Godot;
using System;
public partial class BlackMarketDealer : Node2D
{
	[Export] public AnimatedSprite2D BlackMarketDealerSprite;
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
			{
				_hasTalkedBefore = true;
				TextManager.Instance.RunLines("res://NPCs/BlackMarketDealer/BlackMarketDealerDialogue.json", "BlackMarketDealerFirstTime");
			}
			else
			{
				TextManager.Instance.RunLines("res://NPCs/BlackMarketDealer/BlackMarketDealerDialogue.json", "BlackMarketDealerRepeat");
			}
		}
	}
	private void ToggleWhiteOutline(bool enabled)
	{
		ShaderMaterial material = BlackMarketDealerSprite.Material as ShaderMaterial;
		material.SetShaderParameter("outline_enabled", enabled);
	}
}
