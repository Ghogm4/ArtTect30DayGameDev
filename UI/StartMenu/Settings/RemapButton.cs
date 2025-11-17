using Godot;
using System;

public partial class RemapButton : Button
{
	[Signal] public delegate void RemapRequestedEventHandler(RemapButton button);
	[Export] public string OriginalActionName = "";
	private Label ButtonTextLabel => GetNode<Label>("ButtonText");
	private RemapManager RemapButtonManager => GetNode<RemapManager>("%RemapManager");
    public override void _Ready()
    {
		if (string.IsNullOrEmpty(OriginalActionName))
		{
			GD.PushError("RemapButton: OriginalActionName is not set for RemapButton.");
			return;
		}
        ButtonTextLabel.Text = InputMap.ActionGetEvents(OriginalActionName).Count > 0
			? InputMap.ActionGetEvents(OriginalActionName)[0].AsText().TrimSuffix(" (Physical)")
			: OriginalActionName;
		Connect(SignalName.RemapRequested, Callable.From<RemapButton>(button => RemapButtonManager.OnRemapRequested(this)));

		MouseFilter = MouseFilterEnum.Stop;
		FocusMode = FocusModeEnum.None;
    }
	public void StartRemap()
    {
        ButtonTextLabel.Text = "Press any key...";
		Disabled = true;
    }
	public void StopRemap()
	{
		GD.Print("Remap cancelled.");
		ButtonTextLabel.Text = InputMap.ActionGetEvents(OriginalActionName).Count > 0
			? InputMap.ActionGetEvents(OriginalActionName)[0].AsText().TrimSuffix(" (Physical)")
			: OriginalActionName;
		Disabled = false;
	}
	public void DoRemap(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			InputMap.ActionEraseEvents(OriginalActionName);
			InputMap.ActionAddEvent(OriginalActionName, keyEvent);
			ButtonTextLabel.Text = @event.AsText();
			Disabled = false;
		}
	}
    public override void _Pressed()
    {
        EmitSignal(SignalName.RemapRequested, this);
    }
}
