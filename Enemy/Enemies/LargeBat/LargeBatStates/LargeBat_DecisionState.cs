using Godot;
using System;
using System.Linq;
public partial class LargeBat_DecisionState : State
{
	private Tuple<string, float>[] _nextStates = [
		Tuple.Create("SoundAttack", 1f),
		Tuple.Create("Summon", 0.8f),
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
