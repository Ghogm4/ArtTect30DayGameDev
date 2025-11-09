using Godot;
using System;

public partial class ChangeableTileMapLayer : TileMapLayer
{
	public void PutCell(Vector2I cellPos, Vector2I cellAtlasPos) => SetCell(cellPos, 0, cellAtlasPos, 0);
}
