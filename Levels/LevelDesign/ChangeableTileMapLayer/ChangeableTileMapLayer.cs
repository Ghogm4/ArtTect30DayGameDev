using Godot;
using System;

public partial class ChangeableTileMapLayer : TileMapLayer
{
	public void PutCell(Vector2I cellPos, Vector2I cellAtlasPos, int width, int height)
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
				SetCell(cellPos + new Vector2I(i, j), 0, cellAtlasPos, 0);
	}
}
