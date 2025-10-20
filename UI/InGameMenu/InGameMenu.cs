using Godot;
using System;
using System.Collections.Generic;

public partial class InGameMenu : CanvasLayer
{
	[Export] public PackedScene BoostDisplayerScene;
	[Export] public VFlowContainer BoostDisplayContainer;
	[Export] public Control FloatingBoostInfo;
	[Export] public Control InGameMenuPanel;
	private Dictionary<string, BoostDisplayer> _boostDisplayers = new();
	public override void _Ready()
	{
		Visible = false;
		SignalBus.Instance.Connect(SignalBus.SignalName.PlayerBoostPickedUp, Callable.From<BoostInfo, bool>(AddBoost));
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ToggleInGameMenu"))
			Visible = !Visible;

		if (FloatingBoostInfo.Visible)
		{
			Vector2 mousePosition = GetViewport().GetMousePosition();
			FloatingBoostInfo.Position = mousePosition + new Vector2(16, 16);
		}
	}
	public void AddBoost(BoostInfo info, bool needDisplay)
	{
		if (!needDisplay)
			return;
		if (_boostDisplayers.TryGetValue(info.Name, out BoostDisplayer existingDisplayer))
		{
			BoostInfo existingInfo = existingDisplayer.Info;
			existingInfo.Amount += info.Amount;
		}
		else
		{
			BoostDisplayer displayer = BoostDisplayerScene.Instantiate<BoostDisplayer>();
			displayer.Info = info;
			_boostDisplayers[info.Name] = displayer;
			BoostDisplayContainer.AddChild(displayer);
			displayer.Owner = this;
			displayer.GetFloatingBoostInfoNode();
			existingDisplayer = displayer;
		}
		existingDisplayer.UpdateDisplay();	
	}
}
