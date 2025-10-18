using Godot;
using System;
using Godot.Collections;
[GlobalClass]
public partial class DropTable : Node2D
{
    [Export] public PackedScene[] BoostList = [];
    [Export] public Dictionary<string, float> BoostProbabilityDict = [];
    [Export] public int TimesToRun = 1;
    private Dictionary<string, PackedScene> _boostDict = [];
    private Probability _probability = new();
    public override void _Ready()
    {
        InitDict();
        InitProbability();
        Drop();
    }

    private void InitDict()
    {
        foreach (var scene in BoostList)
        {
            if (scene == null)
                continue;

            string key = scene.ResourcePath.GetFile().GetBaseName();
            _boostDict[key] = scene;
        }
    }
    private void InitProbability()
    {
        foreach (var pair in BoostProbabilityDict)
        {
            if (_boostDict.TryGetValue(pair.Key, out PackedScene scene))
            {
                _probability.Register(pair.Value, () =>
                {
                    Boost boost = scene.Instantiate<Boost>();
                    boost.GlobalPosition = GlobalPosition;
                    Scheduler.Instance.ScheduleAction(2f, () =>
                    {
                        GetTree().CurrentScene.CallDeferred(MethodName.AddChild, boost);

                        float radian = (float)GD.RandRange(-Mathf.Pi * 2 / 3, -Mathf.Pi / 3);
                        float force = (float)GD.RandRange(100f, 500f);
                        boost.ApplyCentralImpulse(Vector2.Right.Rotated(radian) * force);
                    }, 0);
                });
            }
            else
            {
                GD.PushError($"Cannot find the boost named {pair.Key}.");
            }
        }
    }
    public void Drop()
    {
        for (int i = 0; i < TimesToRun; i++)
            _probability.Run();
    }
}
