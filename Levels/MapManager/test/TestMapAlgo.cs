using Godot;
using System;
using System.Collections.Generic;
using System.Security;

public partial class TestMapAlgo : Node2D
{
	[Export] public int Width = 7;
	[Export] public int Height = 5;
	[Export] public int Depth = 4;
	[Export] public Vector2 startPos = new Vector2(0, 0);

	public class Room
	{
		public Tuple<int, int> Position;
		public bool TopExit = false;
		public bool BottomExit = false;
		public bool LeftExit = false;
		public bool RightExit = false;
		public bool IsEnabled = false;
		
		public int getValid()
		{
			int count = 0;
			if (TopExit) count++;
			if (BottomExit) count++;
			if (LeftExit) count++;
			if (RightExit) count++;
			return count;
		}
	}
	public List<Room> Roomlist = new();
	public List<Room> EndRooms = new();
	public void InitMap()
	{
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				Room newRoom = new Room();
				newRoom.Position = new Tuple<int, int>(x, y);
				Roomlist.Add(newRoom);
			}
		}
	}

	public void PrintMap()
	{
		List<List<string>> map = new();
		for (int x = 0; x < 3*Width; x++)
		{
			var col = new List<string>();
			for (int y = 0; y < 3*Height; y++)
				col.Add("   ");
			map.Add(col);
		}
		for (int x = 0; x < Width; x += 1)
		{
			for (int y = 0; y < Height; y += 1)
			{
				Room room = Get(x, y);
				int gx = x * 3 + 1;
				int gy = y * 3 + 1;
				if (room != null && room.IsEnabled)
				{
					map[gx][gy] = "[Y]";
					if (EndRooms.Contains(room) && room.getValid() == 1)
					{
						map[gx][gy] = "[E]";
					}
					if (x == (int)startPos.X && y == (int)startPos.Y)
					{
						map[gx][gy] = "[S]";
					}
					if (room.TopExit && gy - 1 >= 0)
					{
						map[gx][gy - 1] = " | ";
					}
					if (room.BottomExit && gy + 1 < 3 * Height)
					{
						map[gx][gy + 1] = " | ";
					}
					if (room.LeftExit && gx - 1 >= 0)
					{
						map[gx - 1][gy] = "---";
					}
					if (room.RightExit && gx + 1 < 3 * Width)
					{
						map[gx + 1][gy] = "---";
					}
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

	public Room Get(int x, int y)
	{
		foreach (Room room in Roomlist)
		{
			if (room.Position.Item1 == x && room.Position.Item2 == y)
			{
				return room;
			}
		}
		return null;
	}

	public void StartRoom()
	{
		Room startRoom = Get((int)startPos.X, (int)startPos.Y);
		startRoom.IsEnabled = true;
		startRoom.RightExit = true;
		Walk(startRoom);
	}

	public void Randmize(Room room, Room fromRoom, int depth = 0)
	{
		if (depth >= Depth)
		{
			EndRooms.Add(room);
			return;
		}
		Room top = Get(room.Position.Item1, room.Position.Item2 - 1);
		Room bottom = Get(room.Position.Item1, room.Position.Item2 + 1);
		Room left = Get(room.Position.Item1 - 1, room.Position.Item2);
		Room right = Get(room.Position.Item1 + 1, room.Position.Item2);
		List<Room> neighbors = new();
		if (top != null && !top.IsEnabled) neighbors.Add(top);
		if (bottom != null && !bottom.IsEnabled) neighbors.Add(bottom);
		if (left != null && !left.IsEnabled) neighbors.Add(left);
		if (right != null && !right.IsEnabled) neighbors.Add(right);

		foreach (Room neighbor in neighbors)
		{
			if (neighbor == fromRoom)
			{
				if (neighbor == top)
				{
					room.TopExit = true;
				}
				else if (neighbor == bottom)
				{
					room.BottomExit = true;
				}
				else if (neighbor == left)
				{
					room.LeftExit = true;
				}
				else if (neighbor == right)
				{
					room.RightExit = true;
				}
				break;
			}
		}
		neighbors.Remove(fromRoom);
		if (neighbors.Count == 0)
		{
			EndRooms.Add(room);
			return;
		}

		var rng = new Godot.RandomNumberGenerator();
		rng.Randomize();
		for (int i = neighbors.Count - 1; i >= 0; i--)
		{
			int j = rng.RandiRange(0, i);
			Room temp = neighbors[i];
			neighbors[i] = neighbors[j];
			neighbors[j] = temp;
		}

		int exits = rng.RandiRange(1, neighbors.Count);
		for (int i = 0; i < exits; i++)
		{
			Room neighbor = neighbors[i];
			if (neighbor == top)
			{
				room.TopExit = true;
			}
			else if (neighbor == bottom)
			{
				room.BottomExit = true;
			}
			else if (neighbor == left)
			{
				room.LeftExit = true;
			}
			else if (neighbor == right)
			{
				room.RightExit = true;
			}
		}
	}
	public void Walk(Room room, int depth = 0)
	{
		List<Room> neighbors = new();
		if (room.TopExit)
		{
			Room topRoom = Get(room.Position.Item1, room.Position.Item2 - 1);
			if (topRoom != null && !topRoom.IsEnabled)
			{
				neighbors.Add(topRoom);
			}
		}
		if (room.BottomExit)
		{
			Room bottomRoom = Get(room.Position.Item1, room.Position.Item2 + 1);
			if (bottomRoom != null && !bottomRoom.IsEnabled)
			{
				neighbors.Add(bottomRoom);
			}
		}
		if (room.LeftExit)
		{
			Room leftRoom = Get(room.Position.Item1 - 1, room.Position.Item2);
			if (leftRoom != null && !leftRoom.IsEnabled)
			{
				neighbors.Add(leftRoom);
			}
		}
		if (room.RightExit)
		{
			Room rightRoom = Get(room.Position.Item1 + 1, room.Position.Item2);
			if (rightRoom != null && !rightRoom.IsEnabled)
			{
				neighbors.Add(rightRoom);
			}
		}
		if (neighbors.Count == 0 && !EndRooms.Contains(room))
		{
			EndRooms.Add(room);
			return;
		}
		
		foreach (Room neighbor in neighbors)
		{
			neighbor.IsEnabled = true;
			Randmize(neighbor, room, depth);
			if (room.TopExit && neighbor.Position.Item1 == room.Position.Item1 && neighbor.Position.Item2 == room.Position.Item2 - 1)
				neighbor.BottomExit = true;
			if (room.BottomExit && neighbor.Position.Item1 == room.Position.Item1 && neighbor.Position.Item2 == room.Position.Item2 + 1)
				neighbor.TopExit = true;
			if (room.LeftExit && neighbor.Position.Item1 == room.Position.Item1 - 1 && neighbor.Position.Item2 == room.Position.Item2)
				neighbor.RightExit = true;
			if (room.RightExit && neighbor.Position.Item1 == room.Position.Item1 + 1 && neighbor.Position.Item2 == room.Position.Item2)
				neighbor.LeftExit = true;
			Walk(neighbor, depth + 1);
		}
	}
	public override void _Ready()
	{
		GD.Print("TestMapAlgo is ready.");
		InitMap();
		StartRoom();
		PrintMap();
	}
}
