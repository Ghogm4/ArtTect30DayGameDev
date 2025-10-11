using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

public partial class TextManager : Node
{
	public static TextManager Instance { get; private set; }


	public const string Path = "res://TextManager/TextScene.tscn";

	public TextureRect ProfileLeft;
	public TextureRect ProfileRight;
	public ColorRect BoxLeft;
	public ColorRect BoxRight;
	public RichTextLabel TextlabelLeft;
	public RichTextLabel TextlabelRight;
	
	[Serializable]
	public class DialogueLine
	{
		public string Id { get; set; }
		public string Side { get; set; }
		public string Profile { get; set; }
		public string Text { get; set; }
	}
	public DialogueLine[] Lines;
	public int Index = 0;

	public override void _Ready()
	{
		// 单例模式
		if (Instance == null)
		{
			Instance = this;
			// 确保切换场景时不被销毁
			ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			QueueFree();
		}

		SignalBus.Instance.ShowText += ShowText;
		SignalBus.Instance.WaitAdvance += WaitAdvance;
	}
	
	public void LoadLines(string path, string scene)
	{
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		var jsonText = file.GetAsText();
		file.Close();
		GD.Print(jsonText);
		var json = JsonSerializer.Deserialize<Dictionary<string, DialogueLine[]>>(jsonText);
		Lines = json[scene];
		GD.Print(Lines[0].Text);
	}

	public void LoadTextScene()
	{
		var sceneRoot = GetTree().CurrentScene;
		var textScene = GD.Load<PackedScene>(Path);
		sceneRoot.AddChild(textScene.Instantiate());
		ProfileLeft = sceneRoot.GetNode<CanvasLayer>("TextScene").GetNode<Node>("Leftside").GetNode<MarginContainer>("MarginContainer").GetNode<TextureRect>("Profile");
		ProfileRight = sceneRoot.GetNode<CanvasLayer>("TextScene").GetNode<Node>("Rightside").GetNode<MarginContainer>("MarginContainer").GetNode<TextureRect>("Profile");
		TextlabelLeft = sceneRoot.GetNode<CanvasLayer>("TextScene").GetNode<Node>("Leftside").GetNode<BoxContainer>("BoxContainer").GetNode<ColorRect>("ColorRect").GetNode<RichTextLabel>("Text");
		TextlabelRight = sceneRoot.GetNode<CanvasLayer>("TextScene").GetNode<Node>("Rightside").GetNode<BoxContainer>("BoxContainer").GetNode<ColorRect>("ColorRect").GetNode<RichTextLabel>("Text");
		BoxLeft = sceneRoot.GetNode<CanvasLayer>("TextScene").GetNode<Node>("Leftside").GetNode<BoxContainer>("BoxContainer").GetNode<ColorRect>("ColorRect");
		BoxRight = sceneRoot.GetNode<CanvasLayer>("TextScene").GetNode<Node>("Rightside").GetNode<BoxContainer>("BoxContainer").GetNode<ColorRect>("ColorRect");
	}

	public async void ShowText()
	{

		if (Lines == null || Index >= Lines.Length)
		{
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.WaitAdvance);
		}
		var line = Lines[Index];
		GD.Print("Showing line: " + line.Text);
		Tween tween = CreateTween();
		GD.Print(line.Profile);
		if (line.Side == "Left")
		{
			GD.Print(ProfileLeft.Name);
			ProfileLeft.Texture = GD.Load<Texture2D>(line.Profile);
			TextlabelLeft.Text = line.Text;
			TextlabelLeft.VisibleRatio = 0f;
			ProfileLeft.Visible = true;
			BoxLeft.Visible = true;
			ProfileRight.Visible = false;
			BoxRight.Visible = false;

			tween.TweenProperty(TextlabelLeft, "visible_ratio", 1f, 2f);

		}
		else if (line.Side == "Right")
		{
			ProfileRight.Texture = GD.Load<Texture2D>(line.Profile);
			TextlabelRight.Text = line.Text;
			TextlabelRight.VisibleRatio = 0f;
			ProfileRight.Visible = true;
			BoxRight.Visible = true;
			ProfileLeft.Visible = false;
			BoxLeft.Visible = false;

			tween.TweenProperty(TextlabelRight, "visible_ratio", 1f, 2f);
		}
		while (true)
		{
			if (Input.IsActionJustPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left))
			{
				GD.Print("Skipping text animation");
				tween.Kill();
				TextlabelRight.VisibleRatio = 1f;
				TextlabelLeft.VisibleRatio = 1f;

				while (Input.IsActionPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left))
				{
					await ToSignal(GetTree(), "process_frame");
				}

				await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
				break;
			}
			else if (TextlabelRight.VisibleRatio >= 0.999f && line.Side == "Right" || TextlabelLeft.VisibleRatio >= 0.999f && line.Side == "Left")
			{
				
				GD.Print("Text animation completed");
				break;
			}
			await ToSignal(GetTree(), "process_frame");
		}

		SignalBus.Instance.EmitSignal(SignalBus.SignalName.WaitAdvance);

	}
	
	public async void WaitAdvance()
	{
		while (true)
		{
			if (Input.IsActionJustPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left))
			{
				while (Input.IsActionPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left))
				{
					await ToSignal(GetTree(), "process_frame");
				}
				await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
				break;
			}
			await ToSignal(GetTree(), "process_frame");
			if (Index >= Lines.Length)
			{
				var sceneRoot = GetTree().CurrentScene;
				var textScene = sceneRoot.GetNode<CanvasLayer>("TextScene");
				textScene.QueueFree();
				break;
			}
		}
		Index++;
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.ShowText);
	}
}
