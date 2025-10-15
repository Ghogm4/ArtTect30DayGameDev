using Godot;
using System;

public partial class LoadSaveButton : ResponsiveButton
{
    public override void OnPressed()
    {
        SceneManager.Instance.ChangeScene("res://Levels/Level1.tscn");
    }
}
