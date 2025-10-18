using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Probability : RefCounted
{
    private List<Tuple<float, Action>> _probableActions = new();
    private RandomNumberGenerator _rng = new();
    public Probability Register(float weight, Action action)
    {
        _probableActions.Add(new(weight, action));
        return this;
    }
    public void Run()
    {
        if (_probableActions.Count == 0)
        {
            GD.PushError("Can't run the Probability because there's no probable action.");
            return;
        }
        _rng.Randomize();
        float[] weights = _probableActions.Select(x => (float)x.Item1).ToArray();
        int resultIndex = (int)_rng.RandWeighted(weights);
        _probableActions[resultIndex].Item2?.Invoke();
    }
    public static void Run(params Tuple<float, Action>[] probableActions)
    {
        using Probability probability = new();

        foreach (var tuple in probableActions)
            probability.Register(tuple.Item1, tuple.Item2);
        
        probability.Run();
    }
    public static void RunIfElse(float firstProbability, Action first, Action second)
    {
        if (firstProbability < 0.0f || firstProbability > 1.0f)
        {
            GD.PushError("RunIfElse: firstProbability must be between 0.0 and 1.0");
            return;
        }
        Run(
            new Tuple<float, Action>(firstProbability, first),
            new Tuple<float, Action>(1.0f - firstProbability, second)
        );
    }
    
    public static void RunSingle(float probability, Action action) => RunIfElse(probability, action, () => { });
}
