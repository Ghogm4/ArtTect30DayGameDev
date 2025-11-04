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
	public Label SpeakerNameLabel;
	public MarginContainer TextMarginContainer;
	public string CurrentDialogueScene = "";
	private bool _isTextShowing = false;
	[Serializable]
	public class DialogueLine
	{
		public string Id { get; set; }
		public string Side { get; set; }
		public string Profile { get; set; }
		public string Text { get; set; }
		public string SpeakerName { get; set; }
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
	private void StartDialogue()
	{
		if (_isTextShowing)
			return;
		Index = 0;
		_isTextShowing = true;
		ShowText();
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.DialogueStarted);
	}
	private void GetTextSceneNodes()
	{
		TextScene textScene = TextScene.Instance;
		ProfileLeft = textScene.GetNode<TextureRect>("%LeftsideProfile");
		ProfileRight = textScene.GetNode<TextureRect>("%RightsideProfile");
		DialogueTextLabel = textScene.GetNode<Label>("%DialogueTextLabel");
		DialoguePanel = textScene.GetNode<Control>("%DialoguePanel");
		SpeakerNameLabel = textScene.GetNode<Label>("%SpeakerNameLabel");
		TextMarginContainer = textScene.GetNode<MarginContainer>("%TextMarginContainer");
	}
	private void LoadLines(string path, string scene)
	{
		if (_isTextShowing)
			return;
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		var jsonText = file.GetAsText();
		file.Close();
		var json = JsonSerializer.Deserialize<Dictionary<string, DialogueLine[]>>(jsonText);
		Lines = json[scene];
		CurrentDialogueScene = scene;
	}
	public void RunLines(string path, string scene)
	{
		if (_isTextShowing)
			return;
		LoadLines(path, scene);
		StartDialogue();
	}
	public void EndDialogue()
	{
		if (!_isTextShowing)
			return;
			
		Lines = null;
		Index = 0;
		_isTextShowing = false;
		TextScene.Instance.Visible = false;
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.DialogueEnded);
	}
	private async void ShowText()
	{
		if (!_isTextShowing || Lines is null || Index >= Lines.Length)
		{
			EndDialogue();
			return;
		}

		_isTextShowing = true;
		TextScene.Instance.Visible = true;
		var line = Lines[Index];
		Tween tween = CreateTween();
		if (line.Side == "Left")
		{
			ProfileLeft.Texture = ResourceLoader.Load<Texture2D>(line.Profile);
			ProfileLeft.Visible = true;
			ProfileRight.Visible = false;
			DialogueTextLabel.HorizontalAlignment = HorizontalAlignment.Left;
			SpeakerNameLabel.HorizontalAlignment = HorizontalAlignment.Left;
			TextMarginContainer.Set("theme_override_constants/margin_right", 50);
			TextMarginContainer.Set("theme_override_constants/margin_left", 300);
		}
		else if (line.Side == "Right")
		{
			ProfileRight.Texture = ResourceLoader.Load<Texture2D>(line.Profile);
			ProfileRight.Visible = true;
			ProfileLeft.Visible = false;
			DialogueTextLabel.HorizontalAlignment = HorizontalAlignment.Right;
			SpeakerNameLabel.HorizontalAlignment = HorizontalAlignment.Right;
			TextMarginContainer.Set("theme_override_constants/margin_left", 50);
			TextMarginContainer.Set("theme_override_constants/margin_right", 300);
		}

		DialogueTextLabel.Text = line.Text;
		DialogueTextLabel.VisibleRatio = 0f;
		SpeakerNameLabel.Text = line.SpeakerName;
		float durationFactor = 0.035f;
		tween.TweenProperty(DialogueTextLabel, "visible_ratio", 1f, line.Text.Length * durationFactor);

		while (true)
		{
			if (!_isTextShowing) return;

			if (IsSkipping())
			{
				tween.Kill();
				DialogueTextLabel.VisibleRatio = 1f;

				await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
				break;
			}
			else if (DialogueTextLabel.VisibleRatio >= 0.999f)
			{
				break;
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			if (Lines == null || Index >= Lines.Length)
			{
				EndDialogue();
				return;
			}
		}

		WaitAdvance();
	}

	public async void WaitAdvance()
	{
		while (true)
		{
			if (!_isTextShowing) return;

			if (IsSkipping())
			{
				await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
				break;
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

			if (Lines == null || Index >= Lines.Length)
			{
				EndDialogue();
				return;
			}
		}
		Index++;
		GD.Print("TextManager: Advancing to line index: " + Index);
		ShowText();
	}
	private bool IsSkipping()
	{
		return Input.IsActionJustPressed("AdvanceDialogue");
	}
}
