using Godot;
using System;
using System.Numerics;

public partial class StatWrapper : RefCounted
{
    private Stat _stat = null;
    public StatWrapper(Stat stat) => _stat = stat;
    public StatWrapper(StatComponent statComponent, string statName)
    {
        _stat = statComponent.GetStat(statName);
        if (_stat == null)
            GD.PushError($"Stat '{statName}' not found in StatComponent.");
    }
    public static StatWrapper operator +(StatWrapper a, float b)
    {
        a._stat.AddFinal(b);
        return a;
    }
    public static StatWrapper operator -(StatWrapper a, float b)
    {
        return a + (-b);
    }
    public static StatWrapper operator *(StatWrapper a, float b)
    {
        a._stat.Mult(b);
        return a;
    }
    public static StatWrapper operator /(StatWrapper a, float b)
    {
        if (Mathf.IsZeroApprox(b))
        {
            GD.PushError("Division by zero is not allowed.");
            return a;
        }
        return a * (1f / b);
    }
    public static StatWrapper operator ++(StatWrapper a)
    {
        return a + 1f;
    }
    public static StatWrapper operator --(StatWrapper a)
    {
        return a - 1f;
    }
}
