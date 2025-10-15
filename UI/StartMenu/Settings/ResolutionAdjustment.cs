using Godot;
using System;

public partial class ResolutionAdjustment : OptionButton
{
	public override void _Ready()
    {
		ItemSelected += OnItemSelected;
    }
	private void OnItemSelected(long index)
    {
		switch (index)
		{
			case 0:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetSize(new Vector2I(1920, 1080));
				break;
			case 1:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
			default:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
		}
    }
}
