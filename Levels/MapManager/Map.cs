using Godot;
using System;
public enum MapType
{
    T, B, L, R, TB, TL, TR, BL, BR, LR, TBL, TBR, TLR, BLR, TBLR
}
public class Map
{
    public PackedScene Scene;
    public Vector2I Position = Vector2I.Left;

    public bool TopExit = false;
    public bool BottomExit = false;
    public bool LeftExit = false;
    public bool RightExit = false;
    
    public bool IsStartLevel = false;
    public bool IsEndLevel = false;
    public float RarityWeight = 1.0f;

    public Map LeftMap = null;
    public Map RightMap = null;
    public Map TopMap = null;
    public Map BottomMap = null;
    
    public MapType Type;
    public bool IsEnabled = false;
    public bool IsDiscovered = false;
    public Map(PackedScene scene = null, Vector2I position = default,
        bool topExit = false, bool bottomExit = false, bool leftExit = false, bool rightExit = false,
        bool isStartLevel = false, bool isEndLevel = false,
        float rarityWeight = 1.0f
        )
    {
        Scene = scene;
        Position = position;
        TopExit = topExit;
        BottomExit = bottomExit;
        LeftExit = leftExit;
        RightExit = rightExit;
        IsStartLevel = isStartLevel;
        IsEndLevel = isEndLevel;
        RarityWeight = rarityWeight;
        Type = JudgeMapType();
    }
    public Map GetMap(string entrance)
    {
        return entrance switch
        {
            "Top" => TopMap,
            "Bottom" => BottomMap,
            "Left" => LeftMap,
            "Right" => RightMap,
            _ => null,
        };
    }
    public int GetExitCount()
    {
        int count = 0;
        if (TopExit) count++;
        if (BottomExit) count++;
        if (LeftExit) count++;
        if (RightExit) count++;
        return count;
    }

    public MapType JudgeMapType()
    {
        if (TopExit && BottomExit && LeftExit && RightExit) return MapType.TBLR;
        else if (TopExit && BottomExit && LeftExit) return MapType.TBL;
        else if (TopExit && BottomExit && RightExit) return MapType.TBR;
        else if (TopExit && LeftExit && RightExit) return MapType.TLR;
        else if (BottomExit && LeftExit && RightExit) return MapType.BLR;
        else if (TopExit && BottomExit) return MapType.TB;
        else if (LeftExit && RightExit) return MapType.LR;
        else if (TopExit && LeftExit) return MapType.TL;
        else if (TopExit && RightExit) return MapType.TR;
        else if (BottomExit && LeftExit) return MapType.BL;
        else if (BottomExit && RightExit) return MapType.BR;
        else if (TopExit) return MapType.T;
        else if (BottomExit) return MapType.B;
        else if (LeftExit) return MapType.L;
        else if (RightExit) return MapType.R;
        else return MapType.T;
    }
}