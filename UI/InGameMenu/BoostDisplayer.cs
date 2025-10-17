using Godot;
using System;

public partial class BoostDisplayer : Control
{
	public BoostInfo Info = null;
	private TextureRect _displayRect = null;
	private Label _boostAmountLabel = null;
	private Control _floatingBoostInfo = null;
	public override void _Ready()
	{
		_displayRect = GetNode<TextureRect>("DisplayRect");
		_boostAmountLabel = GetNode<Label>("BoostAmountLabel");
		MouseEntered += ShowFloatingBoostInfo;
		MouseExited += HideFloatingBoostInfo;
	}
	public void GetFloatingBoostInfoNode() => _floatingBoostInfo = GetNode<Control>("%FloatingBoostInfo");
	public void UpdateDisplay()
	{
		if (Info is null)
			return;
		_displayRect.Texture = Info.Icon;
		_boostAmountLabel.Text = Info.Amount <= 1 ? "" : Info.Amount.ToString();
	}
	private void ShowFloatingBoostInfo()
	{
		Label boostNameLabel = _floatingBoostInfo.GetNode<Label>("%BoostName");
		Label boostDescriptionLabel = _floatingBoostInfo.GetNode<Label>("%BoostDescription");
		LabelSettings boostNameLabelSettings = boostNameLabel.LabelSettings;
		boostNameLabel.Text = Info.Name;
		boostNameLabelSettings.FontColor = Info.RarityColorMap[Info.BoostRarity];
		boostDescriptionLabel.Text = Info.Description;
		_floatingBoostInfo.Visible = true;
	}
	private void HideFloatingBoostInfo()
    {
        _floatingBoostInfo.Visible = false;
    }
}
