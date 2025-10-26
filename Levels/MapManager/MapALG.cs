using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MapALG : Node2D
{
	[Export] public int Width = 4;
	[Export] public int Height = 3;
	[Export] public int Depth = 4;
	[Export] public Vector2I startPos = new Vector2I(2, 2);
	public static MapALG Instance { get; private set; }
	public List<Map> Roomlist = new();
	public List<Map> EndRooms = new();
	public void InitMap()
	{
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				Map newRoom = new Map();
				newRoom.Position = new(x, y);
				Roomlist.Add(newRoom);
			}
		}
	}

	public void PrintMap(Vector2I nowPos = default)
	{
		List<List<string>> map = new();
		for (int x = 0; x < 3 * Width; x++)
		{
			var col = new List<string>();
			for (int y = 0; y < 3 * Height; y++)
				col.Add("   ");
			map.Add(col);
		}
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				Vector2I pos = new(x, y);
				Map Map = GetMapAtPosition(pos);
				int gx = x * 3 + 1;
				int gy = y * 3 + 1;
				if (Map != null && Map.IsEnabled)
				{
					map[gx][gy] = "[Y]";
					if (EndRooms.Contains(Map) && Map.GetExitCount() == 1)
						map[gx][gy] = "[E]";
					if (startPos == pos)
						map[gx][gy] = "[S]";
					if (nowPos != Vector2I.Left && nowPos == pos)
						map[gx][gy] = "[X]";
					if (Map.TopExit && gy - 1 >= 0)
						map[gx][gy - 1] = " | ";
					if (Map.BottomExit && gy + 1 < 3 * Height)
						map[gx][gy + 1] = " | ";
					if (Map.LeftExit && gx - 1 >= 0)
						map[gx - 1][gy] = "---";
					if (Map.RightExit && gx + 1 < 3 * Width)
						map[gx + 1][gy] = "---";
				}
				else
				{
					map[gx][gy] = "[N]";
				}
			}
		}
		for (int y = 0; y < 3 * Height; y++)
		{
			string line = "";
			for (int x = 0; x < 3 * Width; x++)
			{
				line += map[x][y];
			}
			GD.Print(line);
		}
	}

	public Map GetMapAtPosition(int x, int y) => Roomlist.FirstOrDefault(map => map.Position == new Vector2I(x, y));
	public Map GetMapAtPosition(Vector2I position) => GetMapAtPosition(position.X, position.Y);
	public void StartRoom()
	{
		Map startRoom = GetMapAtPosition(startPos);
		startRoom.IsEnabled = true;
		startRoom.RightExit = true;
		startRoom.LeftExit = true;
		Walk(startRoom);
	}
	public void Randomize(Map Map, Map fromRoom, int depth = 0)
	{
		if (depth >= Depth)
		{
			EndRooms.Add(Map);
			return;
		}
		Map top = GetMapAtPosition(Map.Position.X, Map.Position.Y - 1);
		Map bottom = GetMapAtPosition(Map.Position.X, Map.Position.Y + 1);
		Map left = GetMapAtPosition(Map.Position.X - 1, Map.Position.Y);
		Map right = GetMapAtPosition(Map.Position.X + 1, Map.Position.Y);

		List<Map> neighbors = new();
		if (top != null && !top.IsEnabled) neighbors.Add(top);
		if (bottom != null && !bottom.IsEnabled) neighbors.Add(bottom);
		if (left != null && !left.IsEnabled) neighbors.Add(left);
		if (right != null && !right.IsEnabled) neighbors.Add(right);

		foreach (Map neighbor in neighbors)
		{
			if (neighbor == fromRoom)
			{
				if (neighbor == top)
					Map.TopExit = true;
				else if (neighbor == bottom)
					Map.BottomExit = true;
				else if (neighbor == left)
					Map.LeftExit = true;
				else if (neighbor == right)
					Map.RightExit = true;

				break;
			}
		}
		neighbors.Remove(fromRoom);
		if (neighbors.Count == 0)
		{
			EndRooms.Add(Map);
			return;
		}
		Shuffle(neighbors);
		
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		int exits = rng.RandiRange(1, neighbors.Count);
		for (int i = 0; i < exits; i++)
		{
			Map neighbor = neighbors[i];
			if (neighbor == top)
				Map.TopExit = true;
			else if (neighbor == bottom)
				Map.BottomExit = true;
			else if (neighbor == left)
				Map.LeftExit = true;
			else if (neighbor == right)
				Map.RightExit = true;
		}
	}
	private void Shuffle<T>(List<T> list)
    {
        var rng = new RandomNumberGenerator();
		rng.Randomize();
		for (int i = list.Count - 1; i >= 0; i--)
		{
			int j = rng.RandiRange(0, i);
			T temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}
    }
	public void Walk(Map Map, int depth = 0)
	{
		List<Map> neighbors = new();
		if (Map.TopExit)
		{
			Map topRoom = GetMapAtPosition(Map.Position.X, Map.Position.Y - 1);
			if (topRoom != null && !topRoom.IsEnabled)
				neighbors.Add(topRoom);
		}
		if (Map.BottomExit)
		{
			Map bottomRoom = GetMapAtPosition(Map.Position.X, Map.Position.Y + 1);
			if (bottomRoom != null && !bottomRoom.IsEnabled)
				neighbors.Add(bottomRoom);
		}
		if (Map.LeftExit)
		{
			Map leftRoom = GetMapAtPosition(Map.Position.X - 1, Map.Position.Y);
			if (leftRoom != null && !leftRoom.IsEnabled)
				neighbors.Add(leftRoom);
		}
		if (Map.RightExit)
		{
			Map rightRoom = GetMapAtPosition(Map.Position.X + 1, Map.Position.Y);
			if (rightRoom != null && !rightRoom.IsEnabled)
				neighbors.Add(rightRoom);
		}
		if (neighbors.Count == 0 && !EndRooms.Contains(Map))
		{
			EndRooms.Add(Map);
			return;
		}

		foreach (Map neighbor in neighbors)
		{
			neighbor.IsEnabled = true;
			Randomize(neighbor, Map, depth);
			if (Map.TopExit && neighbor.Position.X == Map.Position.X && neighbor.Position.Y == Map.Position.Y - 1)
				neighbor.BottomExit = true;
			if (Map.BottomExit && neighbor.Position.X == Map.Position.X && neighbor.Position.Y == Map.Position.Y + 1)
				neighbor.TopExit = true;
			if (Map.LeftExit && neighbor.Position.X == Map.Position.X - 1 && neighbor.Position.Y == Map.Position.Y)
				neighbor.RightExit = true;
			if (Map.RightExit && neighbor.Position.X == Map.Position.X + 1 && neighbor.Position.Y == Map.Position.Y)
				neighbor.LeftExit = true;
			Walk(neighbor, depth + 1);
		}
	}
	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
			ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			QueueFree();
		}
		GD.Print("MapALG is ready.");
		// InitMap();
		// StartRoom();
		// PrintMap();
	}
}
