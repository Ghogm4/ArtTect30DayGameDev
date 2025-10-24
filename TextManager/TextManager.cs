using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

public partial class TextManager : Node
{
	public static TextManager Instance { get; private set; }

	public Control DialoguePanel;
	public TextureRect ProfileLeft;
	public TextureRect ProfileRight;
	public Label DialogueTextLabel;
	private bool _isTextShowing = false;
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

		GetTextSceneNodes();
	}
	public void StartDialogue()
	{
		if (_isTextShowing)
			return;
		Index = 0;
		ShowText();
	}
	private void GetTextSceneNodes()
	{
		TextScene textScene = TextScene.Instance;
		ProfileLeft = textScene.GetNode<TextureRect>("%LeftsideProfile");
		ProfileRight = textScene.GetNode<TextureRect>("%RightsideProfile");
		DialogueTextLabel = textScene.GetNode<Label>("%DialogueTextLabel");
		DialoguePanel = textScene.GetNode<Control>("%DialoguePanel");
	}
	public void LoadLines(string path, string scene)
	{
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		var jsonText = file.GetAsText();
		file.Close();
		var json = JsonSerializer.Deserialize<Dictionary<string, DialogueLine[]>>(jsonText);
		Lines = json[scene];
	}

	private async void ShowText()
	{
		if (Lines is null)
			return;

		if (Index >= Lines.Length)
		{
			TextScene.Instance.Visible = false;
			_isTextShowing = false;
			return;
		}

		_isTextShowing = true;
		TextScene.Instance.Visible = true;
		var line = Lines[Index];
		Tween tween = CreateTween();
		if (line.Side == "Left")
		{
			ProfileLeft.Visible = true;
			ProfileRight.Visible = false;
		}
		else if (line.Side == "Right")
		{
			ProfileRight.Visible = true;
			ProfileLeft.Visible = false;
		}

		ProfileLeft.Texture = ResourceLoader.Load<Texture2D>(line.Profile);
		DialogueTextLabel.Text = line.Text;
		DialogueTextLabel.VisibleRatio = 0f;
		tween.TweenProperty(DialogueTextLabel, "visible_ratio", 1f, 2f);

		while (true)
		{
			if (Input.IsActionJustPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left))
			{
				tween.Kill();
				DialogueTextLabel.VisibleRatio = 1f;

				while (Input.IsActionPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left))
					await ToSignal(GetTree(), "process_frame");

				await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
				break;
			}
			else if (DialogueTextLabel.VisibleRatio >= 0.999f)
			{
				break;
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}

		WaitAdvance();
	}

	public async void WaitAdvance()
	{
		while (true)
		{
			if (Input.IsActionJustPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left))
			{
				while (Input.IsActionPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left))
					await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

				await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
				break;
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			if (Index >= Lines.Length)
			{
				TextScene.Instance.Visible = false;
				break;
			}
		}
		Index++;
		ShowText();
	}
}
