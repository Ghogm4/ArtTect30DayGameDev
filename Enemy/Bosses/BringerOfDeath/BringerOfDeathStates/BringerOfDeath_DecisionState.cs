using Godot;
using System;
using System.Linq;
public partial class BringerOfDeath_DecisionState : State
{
	private Tuple<string, float>[] _nextStates = [
		Tuple.Create("Walk", 1f),
		Tuple.Create("Cast", 1.5f),
	];
	private bool _wasNormalDecided = false;
	protected override void Enter()
	{
		string nextState = "";
		if (!_wasNormalDecided)
		{
			_wasNormalDecided = true;
			nextState = "Idle";
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
