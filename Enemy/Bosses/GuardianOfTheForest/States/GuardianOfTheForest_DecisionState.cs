using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GuardianOfTheForest_DecisionState : State
{
	private Tuple<string, float>[] _nextStates = [
		Tuple.Create("Dash", 0.5f),
		Tuple.Create("Normal", 1.0f),
		Tuple.Create("LaserCast", 0.1f),
		Tuple.Create("ArmLaunch", 0.5f),
		Tuple.Create("Immune", 1.0f),
		Tuple.Create("Glow", 0.1f),
		Tuple.Create("Armor", 0.3f)
	];
	private bool _wasNormalDecided = false;
	protected override void Enter()
    {
		string nextState = Probability.RunWeightedChoose(_nextStates.Select(x => x.Item1).ToArray(), _nextStates.Select(x => x.Item2).ToArray());
		if (nextState == "Normal")
		{
			if (!_wasNormalDecided)
			{
				_wasNormalDecided = true;
			}
			else
			{
				IEnumerable<Tuple<string, float>> otherStates = _nextStates.Where(x => x.Item1 != "Normal");
				nextState = Probability.RunWeightedChoose(otherStates.Select(x => x.Item1).ToArray(),
					otherStates.Select(x => x.Item2).ToArray());
				_wasNormalDecided = false;
			}
			AskTransit(nextState);
		}
    }
}
