using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class SubMap : Control
{
	[Export] public TextureRect Room00 = null;
	[Export] public TextureRect Room10 = null;
	[Export] public TextureRect Room20 = null;
	[Export] public TextureRect Room30 = null;
	[Export] public TextureRect Room01 = null;
	[Export] public TextureRect Room11 = null;
	[Export] public TextureRect Room21 = null;
	[Export] public TextureRect Room31 = null;
	[Export] public TextureRect Room02 = null;
	[Export] public TextureRect Room12 = null;
	[Export] public TextureRect Room22 = null;
	[Export] public TextureRect Room32 = null;

	[Export] public Texture2D RoomT;
	[Export] public Texture2D RoomB;
	[Export] public Texture2D RoomL;
	[Export] public Texture2D RoomR;
	[Export] public Texture2D RoomTL;
	[Export] public Texture2D RoomTR;
	[Export] public Texture2D RoomBL;
	[Export] public Texture2D RoomBR;
	[Export] public Texture2D RoomTB;
	[Export] public Texture2D RoomLR;
	[Export] public Texture2D RoomTBL;
	[Export] public Texture2D RoomTBR;
	[Export] public Texture2D RoomTLR;
	[Export] public Texture2D RoomBLR;
	[Export] public Texture2D RoomTBLR;

	[Export] public Control HighlightRoom;
	public override void _Ready()
	{
		MapManager.Instance.Connect(MapManager.SignalName.MapGenerated, Callable.From(OnMapGenerated));
		MapManager.Instance.Connect(MapManager.SignalName.MapChanged, Callable.From(OnMapChanged));
		this.GetNode<CanvasLayer>("CanvasLayer").Visible = false;
		HighlightAnimation();
	}
	public void HighlightAnimation()
	{
		Tween tween = HighlightRoom.CreateTween();
		tween.SetLoops();
		tween.TweenProperty(HighlightRoom, "modulate:a", 0.2f, 0.5f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(HighlightRoom, "modulate:a", 0.8f, 0.5f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
	}
	public void SetHighlightRoom(MapManager.Map map)
	{
		Vector2 position = new Vector2();
		position.X = map.Position.Item1 * 64;
		position.Y = map.Position.Item2 * 48;
		HighlightRoom.Position = position;
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ToggleSubMap"))
			this.GetNode<CanvasLayer>("CanvasLayer").Visible = true;
		if (Input.IsActionJustReleased("ToggleSubMap"))
			this.GetNode<CanvasLayer>("CanvasLayer").Visible = false;
	}
	private void OnMapGenerated()
	{
		GD.Print("Map generated!");
		DrawMap();
	}
	public void OnMapChanged()
	{
		GD.Print("Map changed!");
		DrawMap();
		SetHighlightRoom(MapManager.Instance.NowMap);
		
	}
	public void DrawMap()
	{
		foreach (var room in MapALG.Instance.Roomlist)
		{
			MapManager.Map map = MapManager.Instance.GetMapAtPosition(room.Position);
			if (!room.IsEnabled) continue;
			Texture2D roomTexture = GetRoomTexture(room);
			TextureRect position = GetPosition(room);
			position.Texture = roomTexture;
			if (!map.IsDiscovered) position.Visible = false;
			else position.Visible = true;
		}
	}
	public Texture2D GetRoomTexture(MapALG.Room room)
	{
		switch (room.JudgeMapType())
		{
			case MapALG.MapType.T:
				return RoomT;
			case MapALG.MapType.B:
				return RoomB;
			case MapALG.MapType.L:
				return RoomL;
			case MapALG.MapType.R:
				return RoomR;
			case MapALG.MapType.TL:
				return RoomTL;
			case MapALG.MapType.TR:
				return RoomTR;
			case MapALG.MapType.BL:
				return RoomBL;
			case MapALG.MapType.BR:
				return RoomBR;
			case MapALG.MapType.TB:
				return RoomTB;
			case MapALG.MapType.LR:
				return RoomLR;
			case MapALG.MapType.TBL:
				return RoomTBL;
			case MapALG.MapType.TBR:
				return RoomTBR;
			case MapALG.MapType.TLR:
				return RoomTLR;
			case MapALG.MapType.BLR:
				return RoomBLR;
			case MapALG.MapType.TBLR:
				return RoomTBLR;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public TextureRect GetPosition(MapALG.Room room)
	{
		int x = room.Position.Item1;
		int y = room.Position.Item2;
		if (x == 0 && y == 0) return Room00;
		else if (x == 1 && y == 0) return Room10;
		else if (x == 2 && y == 0) return Room20;
		else if (x == 3 && y == 0) return Room30;
		else if (x == 0 && y == 1) return Room01;
		else if (x == 1 && y == 1) return Room11;
		else if (x == 2 && y == 1) return Room21;
		else if (x == 3 && y == 1) return Room31;
		else if (x == 0 && y == 2) return Room02;
		else if (x == 1 && y == 2) return Room12;
		else if (x == 2 && y == 2) return Room22;
		else if (x == 3 && y == 2) return Room32;
		else
		{
			GD.PrintErr($"SubMap: No TextureRect for room at ({room.Position.Item1},{room.Position.Item2})");
			return null;
		}

	}
	
	public TextureRect GetPositionMap(MapManager.Map map)
	{
		int x = map.Position.Item1;
		int y = map.Position.Item2;
		if (x == 0 && y == 0) return Room00;
		else if (x == 1 && y == 0) return Room10;
		else if (x == 2 && y == 0) return Room20;
		else if (x == 3 && y == 0) return Room30;
		else if (x == 0 && y == 1) return Room01;
		else if (x == 1 && y == 1) return Room11;
		else if (x == 2 && y == 1) return Room21;
		else if (x == 3 && y == 1) return Room31;
		else if (x == 0 && y == 2) return Room02;
		else if (x == 1 && y == 2) return Room12;
		else if (x == 2 && y == 2) return Room22;
		else if (x == 3 && y == 2) return Room32;
		else
		{
			GD.PrintErr($"SubMap: No TextureRect for room at ({map.Position.Item1},{map.Position.Item2})");
			return null;
		}

	}
}
