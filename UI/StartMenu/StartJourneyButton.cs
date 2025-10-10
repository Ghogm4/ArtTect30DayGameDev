using Godot;
using System;

public partial class StartJourneyButton : ResponsiveButton
{
    public override void OnPressed()
    {
        SceneManager.Instance.ChangeScene("res://Levels/Level1.tscn");
    }
}
