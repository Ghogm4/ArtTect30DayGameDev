using Godot;
using System;
using Godot.Collections;
public interface ISavable
{
    string UniqueID { get; }

    Dictionary SaveState();

    void LoadState(Dictionary state);
}
