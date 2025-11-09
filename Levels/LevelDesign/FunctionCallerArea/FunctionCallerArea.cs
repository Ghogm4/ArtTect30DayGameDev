using Godot;
using System;
using System.Linq;
using GDArray = Godot.Collections.Array;
using GDDictionary = Godot.Collections.Dictionary;
public partial class FunctionCallerArea : Area2D, ISavable
{
	[Export] public Node[] Callees = null;
	[Export] public Godot.Collections.Array<FunctionCallResource> FunctionCalls;
	private Godot.Collections.Array<bool> _callConditionArray = new();
	public string UniqueID => Name;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
		if (_callConditionArray.Count == 0) return;
		for (int i = 0; i < FunctionCalls.Count; i++)
			FunctionCalls[i].HasBeenCalled = _callConditionArray[i];
	}
	private Variant[] ProcessFunctionArgs(GDArray functionArgs) => functionArgs.ToArray();

	private void OnBodyEntered(Node2D body)
	{
		GD.Print("FunctionCallerArea: AreaEntered triggered.");
		if (body is not Player) return;
		if (Callees.Length != FunctionCalls.Count)
		{
			GD.PushError("FunctionCallerArea: Callees count does not match FunctionCalls count.");
			return;
		}
		for (int i = 0; i < FunctionCalls.Count; i++)
		{
			var call = FunctionCalls[i];
			var callee = Callees[i];
			if (call.Type != FunctionCallResource.CallType.AreaEntered) continue;
			if (call.OneShot && call.HasBeenCalled) continue;

			if (call.FunctionArgs.Count == 0)
				callee.CallDeferred(call.FunctionName);
			else
				callee.CallDeferred(call.FunctionName, ProcessFunctionArgs(call.FunctionArgs));
			GD.Print($"FunctionCallerArea: Called {call.FunctionName} on {callee.Name}.");
			call.HasBeenCalled = true;
		}
	}
	private void OnBodyExited(Node2D body)
	{
		GD.Print("FunctionCallerArea: AreaExited triggered.");
		if (body is not Player) return;
		if (Callees.Length != FunctionCalls.Count)
		{
			GD.PushError("FunctionCallerArea: Callees count does not match FunctionCalls count.");
			return;
		}
		for (int i = 0; i < FunctionCalls.Count; i++)
		{
			var call = FunctionCalls[i];
			var callee = Callees[i];
			if (call.Type != FunctionCallResource.CallType.AreaExited) continue;
			if (call.OneShot && call.HasBeenCalled) continue;

			if (call.FunctionArgs.Count == 0)
				callee.CallDeferred(call.FunctionName);
			else
				callee.CallDeferred(call.FunctionName, ProcessFunctionArgs(call.FunctionArgs));
			GD.Print($"FunctionCallerArea: Called {call.FunctionName} on {callee.Name}.");
			call.HasBeenCalled = true;
		}
		TileMapLayer a = new();
		a.SetCell(Vector2I.Zero, 0, Vector2I.Zero, 0);
	}
	public GDDictionary SaveState()
	{
		Godot.Collections.Array<bool> callConditionArray = new();
		foreach (var call in FunctionCalls)
			callConditionArray.Add(call.HasBeenCalled);
		return new()
		{
			["CallConditionArray"] = callConditionArray
		};
	}
	public void LoadState(GDDictionary state)
	{
		if (state?.TryGetValue("CallConditionArray", out var callConditionArray) ?? false)
			_callConditionArray = (Godot.Collections.Array<bool>)callConditionArray;
	}
}
