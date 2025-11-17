using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GuardianOfTheForest_DecisionState : State
{
	private Tuple<string, float>[] _nextStates = [
		Tuple.Create("Dash", 1f),
		Tuple.Create("LaserCast", 1f),
		Tuple.Create("ArmLaunch", 1f),
		Tuple.Create("Immune", 1f),
		Tuple.Create("Glow", 1f),
		Tuple.Create("Armor", 1f)
	];
	private bool _wasNormalDecided = false;
	protected override void Enter()
	{
		string nextState = "";
		if (!_wasNormalDecided)
		{
			_wasNormalDecided = true;
			nextState = "Normal";
		}
		else
		{
			nextState = Probability.RunWeightedChoose(_nextStates.Select(x => x.Item1).ToArray(),
				_nextStates.Select(x => x.Item2).ToArray());
			_wasNormalDecided = false;
		}
		AskTransit(nextState);
	}
}
