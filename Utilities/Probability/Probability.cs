using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Probability : RefCounted
{
    public const int MaxProbabilityConvertedSum = 1000;
    private List<Tuple<int, Action>> _probableActions = new();
    private int _currentProbabilityConvertedSum = 0;
    private RandomNumberGenerator _rng = new();
    public Probability Register(float probability, Action action)
    {
        int converted = ConvertProbability(probability);
        if (_currentProbabilityConvertedSum + converted > MaxProbabilityConvertedSum)
        {
            GD.PushError("Total probability registered to a Probability exceeded 1. Registration failed.");
            return this;
        }
        _currentProbabilityConvertedSum += converted;
        _probableActions.Add(new(converted, action));
        return this;
    }
    public void RegisterCertainAction(Action action)
    {
        Register(1, action);
    }
    public void Run()
    {
        if (_probableActions.Count == 0)
        {
            GD.PushError("Can't run the Probability because there's no probable action.");
            return;
        }
        _rng.Randomize();
        float[] probabilities = _probableActions.Select(x => (float)x.Item1).ToArray();
        int resultIndex = (int)_rng.RandWeighted(probabilities);
        _probableActions[resultIndex].Item2?.Invoke();
    }
    public static void Run(params Tuple<float, Action>[] probableActions)
    {
        using Probability probability = new();
        foreach (var tuple in probableActions)
        {
            probability.Register(tuple.Item1, tuple.Item2);
        }
        probability.Run();
    }
    public static void RunIfElse(float firstProbability, Action first, Action second)
    {
        using Probability probability = new();
        float firstProb = Mathf.Clamp(firstProbability, 0f, 1f);
        probability
            .Register(firstProbability, first)
            .Register(1f - firstProbability, second);
        probability.Run();
    }
    public static void RunSingle(float probability, Action action) => RunIfElse(probability, action, () => { });
    public int ConvertProbability(float probability) => Convert.ToInt32(probability * MaxProbabilityConvertedSum);
}
