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
    public static StatWrapper operator -(StatWrapper a, float b) => a + (-b);
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
    public static StatWrapper operator ++(StatWrapper a) => a + 1f;
    public static StatWrapper operator --(StatWrapper a) => a - 1f;
    public static explicit operator float(StatWrapper a) => a._stat.FinalValue;
    public static explicit operator int(StatWrapper a) => (int)a._stat.FinalValue;
    public static bool operator ==(StatWrapper a, float b) => Mathf.IsEqualApprox((float)a, b);
    public static bool operator ==(StatWrapper a, int b) => (int)a == b;
    public static bool operator !=(StatWrapper a, float b) => !(a == b);
    public static bool operator !=(StatWrapper a, int b) => !(a == b);
    public static bool operator <(StatWrapper a, float b) => (float)a < b;
    public static bool operator <(StatWrapper a, int b) => (int)a < b;
    public static bool operator <=(StatWrapper a, float b) => (float)a <= b;
    public static bool operator <=(StatWrapper a, int b) => (int)a <= b;
    public static bool operator >=(StatWrapper a, float b) => (float)a >= b;
    public static bool operator >=(StatWrapper a, int b) => (int)a >= b;
    public static bool operator >(StatWrapper a, float b) => (float)a > b;
    public static bool operator >(StatWrapper a, int b) => (int)a > b;
    public override bool Equals(object obj)
    {
        if (obj is StatWrapper other)
            return this == (float)other;
        return false;
    }
    public override int GetHashCode() => base.GetHashCode();
}
