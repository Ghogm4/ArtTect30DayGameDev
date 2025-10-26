using Godot;
using System;

public partial class InGameUI : CanvasLayer
{
	public static InGameUI Instance { get; private set; }
	public override void _Ready() => Instance = this;
}
