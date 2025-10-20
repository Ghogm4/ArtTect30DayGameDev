using Godot;
using System;

public partial class BoostDisplayer : Control
{
	public BoostInfo Info = null;
	public Control FloatingBoostInfo = null;
	private TextureRect _displayRect = null;
	private Label _boostAmountLabel = null;
	public override void _Ready()
	{
		_displayRect = GetNode<TextureRect>("DisplayRect");
		_boostAmountLabel = GetNode<Label>("BoostAmountLabel");
		MouseEntered += ShowFloatingBoostInfo;
		MouseExited += HideFloatingBoostInfo;
	}
	public void UpdateDisplay()
	{
		if (Info is null)
			return;
		_displayRect.Texture = Info.Icon;
		_boostAmountLabel.Text = Info.Amount <= 1 ? "" : Info.Amount.ToString();
	}
	private void ShowFloatingBoostInfo()
	{
		Label boostNameLabel = FloatingBoostInfo.GetNode<Label>("%BoostName");
		Label boostDescriptionLabel = FloatingBoostInfo.GetNode<Label>("%BoostDescription");
		LabelSettings boostNameLabelSettings = boostNameLabel.LabelSettings;
		boostNameLabel.Text = Info.Name;
		boostNameLabelSettings.FontColor = BoostInfo.RarityColorMap[Info.Rarity];
		boostDescriptionLabel.Text = Info.Description;
		FloatingBoostInfo.Visible = true;
	}
	private void HideFloatingBoostInfo()
    {
        FloatingBoostInfo.Visible = false;
    }
}
