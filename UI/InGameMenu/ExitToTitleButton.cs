using Godot;
using System;

public partial class ExitToTitleButton : ResponsiveButton
{
    public override void OnPressed()
    {
		SceneManager.Instance.ChangeScene("res://UI/StartMenu/StartMenuExclusive/StartMenu.tscn");
    }
}
