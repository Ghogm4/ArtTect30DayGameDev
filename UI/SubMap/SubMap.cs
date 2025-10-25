using Godot;
using System;
using System.Collections.Generic;

public partial class SubMap : Control
{
	private Dictionary<Vector2I, TextureRect> _roomTextureRects = new();
	[Export] public Panel SubMapPanel = null;
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
	[Export] public TextureRect HighlightRoom;
	private Vector2 _subMapPanelSize;
	public override void _Ready()
	{
		MapManager.Instance.Connect(MapManager.SignalName.MapGenerated, Callable.From(OnMapGenerated));
		MapManager.Instance.Connect(MapManager.SignalName.MapChanged, Callable.From(OnMapChanged));
		Visible = false;
		HighlightAnimation();
		_subMapPanelSize = SubMapPanel.Size;
	}
	public void HighlightAnimation()
	{
		Tween tween = HighlightRoom.CreateTween();
		tween.SetLoops();
		tween.TweenProperty(HighlightRoom, "modulate:a", 0.2f, 0.5f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(HighlightRoom, "modulate:a", 0.8f, 0.5f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
	}
	public void SetHighlightRoomPosition(Map map)
	{
		Vector2 position = new();
		position.X = map.Position.X * _subMapPanelSize.X / MapALG.Instance.Width;
		position.Y = map.Position.Y * _subMapPanelSize.Y / MapALG.Instance.Height;
		HighlightRoom.Position = position;
		HighlightRoom.Size = _subMapPanelSize / new Vector2(MapALG.Instance.Width, MapALG.Instance.Height);
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ToggleSubMap"))
			Visible = true;
		if (Input.IsActionJustReleased("ToggleSubMap"))
			Visible = false;
	}
	private void OnMapGenerated()
	{
		GD.Print("Map generated!");
		_roomTextureRects.Clear();

		Vector2 roomSize = _subMapPanelSize / new Vector2(MapALG.Instance.Width, MapALG.Instance.Height);
		foreach (var room in MapALG.Instance.Roomlist)
		{
			var roomRect = new TextureRect();
			_roomTextureRects[room.Position] = roomRect;
			SubMapPanel.AddChild(roomRect);
			roomRect.Size = roomSize;
			roomRect.Position = new Vector2(room.Position.X * roomSize.X, room.Position.Y * roomSize.Y);
		}
		DrawMap();
	}
	public void OnMapChanged()
	{
		GD.Print("Map changed!");
		DrawMap();
		SetHighlightRoomPosition(MapManager.Instance.NowMap);
	}
	public void DrawMap()
	{
		foreach (var room in MapALG.Instance.Roomlist)
		{
			Map map = MapManager.Instance.GetMapAtPosition(room.Position);
			if (!room.IsEnabled) continue;

			Texture2D roomTexture = GetRoomTexture(room);
			TextureRect roomTextureRect = GetRoomTextureRect(room);
			roomTextureRect.Texture = roomTexture;

			if (!map.IsDiscovered) roomTextureRect.Visible = false;
			else roomTextureRect.Visible = true;
		}
	}
	public Texture2D GetRoomTexture(Map room)
	{
		return room.JudgeMapType() switch
		{
			MapType.T => RoomT,
			MapType.B => RoomB,
			MapType.L => RoomL,
			MapType.R => RoomR,
			MapType.TL => RoomTL,
			MapType.TR => RoomTR,
			MapType.BL => RoomBL,
			MapType.BR => RoomBR,
			MapType.TB => RoomTB,
			MapType.LR => RoomLR,
			MapType.TBL => RoomTBL,
			MapType.TBR => RoomTBR,
			MapType.TLR => RoomTLR,
			MapType.BLR => RoomBLR,
			MapType.TBLR => RoomTBLR,
			_ => RoomTBLR,
		};
	}

	public TextureRect GetRoomTextureRect(Map room)
	{
		return _roomTextureRects.GetValueOrDefault(room.Position);
	}
}
