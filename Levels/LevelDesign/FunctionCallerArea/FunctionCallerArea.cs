using Godot;
using System;
using GDArray = Godot.Collections.Array;
using GDDictionary = Godot.Collections.Dictionary;
public partial class FunctionCallerArea : Area2D, ISavable
{
	[Export] public Node Callee = null;
	[ExportGroup("Area Entered Settings")]
	[Export] public StringName AreaEnteredFunctionName = "";
	[Export] public GDArray AreaEnteredFunctionArgs = new();
	[Export] public bool AreaEnteredOneShot = true;
	[ExportGroup("Area Exited Settings")]
	[Export] public StringName AreaExitedFunctionName = "";
	[Export] public GDArray AreaExitedFunctionArgs = new();
	[Export] public bool AreaExitedOneShot = true;
	public string UniqueID => Name;
	private bool _areaEnteredTriggered = false;
	private bool _areaExitedTriggered = false;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}
	private void OnBodyEntered(Node2D body)
	{
		GD.Print("FunctionCallerArea: AreaEntered triggered.");
		if (body is not Player) return;
		if (Callee == null)
		{
			GD.PushError("FunctionCallerArea: Callee is null.");
			return;
		}
		if (string.IsNullOrEmpty(AreaEnteredFunctionName) || (AreaEnteredOneShot && _areaEnteredTriggered)) return;

		if (AreaEnteredFunctionArgs.Count == 0)
			Callee.CallDeferred(AreaEnteredFunctionName);
		else
			Callee.CallDeferred(AreaEnteredFunctionName, AreaEnteredFunctionArgs);
		GD.Print($"FunctionCallerArea: Called {AreaEnteredFunctionName} on {Callee.Name}.");
		_areaEnteredTriggered = true;

	}
	private void OnBodyExited(Node2D body)
	{
		GD.Print("FunctionCallerArea: AreaExited triggered.");
		if (body is not Player) return;
		if (Callee == null)
		{
			GD.PushError("FunctionCallerArea: Callee is null.");
			return;
		}
		if (string.IsNullOrEmpty(AreaEnteredFunctionName) || (AreaEnteredOneShot && _areaEnteredTriggered)) return;

		if (AreaExitedFunctionArgs.Count == 0)
			Callee.CallDeferred(AreaExitedFunctionName);
		else
			Callee.CallDeferred(AreaExitedFunctionName, AreaExitedFunctionArgs);
		GD.Print($"FunctionCallerArea: Called {AreaExitedFunctionName} on {Callee.Name}.");
		_areaExitedTriggered = true;
	}
	public GDDictionary SaveState()
	{
		return new()
		{
			["AreaEnteredTriggered"] = _areaEnteredTriggered,
			["AreaExitedTriggered"] = _areaExitedTriggered
		};
	}
	public void LoadState(GDDictionary state)
	{
		if (state?.TryGetValue("AreaEnteredTriggered", out var areaEnteredTriggered) ?? false)
			_areaEnteredTriggered = (bool)areaEnteredTriggered;
		if (state?.TryGetValue("AreaExitedTriggered", out var areaExitedTriggered) ?? false)
			_areaExitedTriggered = (bool)areaExitedTriggered;
	}
}
