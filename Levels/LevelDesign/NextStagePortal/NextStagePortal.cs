using Godot;
using System;
using GDDictionary = Godot.Collections.Dictionary;
public partial class NextStagePortal : Node2D, ISavable
{
	public string UniqueID => Name;
	[Export] public AnimatedSprite2D PortalSprite;
	[Export] public bool NeedToCompleteWaves = true;
	[Export] public bool SkipLevelInitialization = false;
	[Export] public EnemyWaveController LinkedEnemyWaveController = null;
	private bool IsInteractable
	{
		get => field;
		set
		{
			if (field == value) return;
			field = value;
			if (field && NeedToCompleteWaves)
			{
				MapManager.Instance.MapPoolIndex++;
				Tween tween = CreateTween();
				tween.TweenProperty(this, "scale", Vector2.One, 1f)
					.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
			}
		}
	} = false;
	private bool _isPlayerNearby = false;
	private bool _isTeleporting = false;
	private bool _isEntered = false;
	public void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		_isPlayerNearby = true;
		if (IsInteractable)
			ToggleWhiteOutline(true);
	}
	public void OnBodyExited(Node2D body)
	{
		if (!body.IsInGroup("Player"))
			return;
		_isPlayerNearby = false;
		ToggleWhiteOutline(false);
	}
	private void ToggleWhiteOutline(bool enable)
	{
		ShaderMaterial material = PortalSprite.Material as ShaderMaterial;
		if (material == null) return;
		material.SetShaderParameter("outline_enabled", enable);
	}
	public override async void _Ready()
	{
		if (!SkipLevelInitialization)
		{
			BaseLevel baseLevel = GetTree().CurrentScene as BaseLevel;
			if (baseLevel != null)
				await ToSignal(baseLevel, BaseLevel.SignalName.LevelInitialized);
		}
		LinkedEnemyWaveController?.AllWavesCompleted += () => IsInteractable = true;
		if (!NeedToCompleteWaves)
			IsInteractable = true;
		else if (!IsInteractable)
			Scale = Vector2.Zero;
		if (_isEntered)
		{
			Visible = false;
			IsInteractable = false;
		}
	}

	public override void _Process(double delta)
	{
		if (!_isPlayerNearby || _isTeleporting || !Input.IsActionJustPressed("Interact") || !IsInteractable)
			return;
		_isTeleporting = true;
		MapManager.Instance.InitMaps();
		MapManager.Instance.StartLevel();
	}
	public GDDictionary SaveState()
	{
		return new()
		{
			["IsInteractable"] = IsInteractable,
			["IsEntered"] = _isEntered
		};
	}
	public void LoadState(GDDictionary state)
	{
		if (state?.TryGetValue("IsInteractable", out var isInteractable) ?? false)
			IsInteractable = (bool)isInteractable;
		if (state?.TryGetValue("IsEntered", out var isEntered) ?? false)
			_isEntered = (bool)isEntered;
	}
}
