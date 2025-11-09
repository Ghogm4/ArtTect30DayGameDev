using Godot;
using System;
[GlobalClass]
public partial class FunctionCallResource : Resource
{
    public enum CallType
    {
        AreaEntered,
        AreaExited
    }
    [Export] public CallType Type = CallType.AreaEntered;
    [Export] public StringName FunctionName = "";
    [Export] public Godot.Collections.Array FunctionArgs = new();
    [Export] public bool OneShot = true;
    public bool HasBeenCalled = false;
}
