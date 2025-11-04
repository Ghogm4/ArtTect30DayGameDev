using Godot;
using System;
using GDDictionary = Godot.Collections.Dictionary;
public partial class Portal : Node2D, ISavable
{
	public string UniqueID => Name;
	public enum PortalType
	{
		NormalLevelEntrance,
		ExtraLevelEntrance,
		ExtraLevelExit
	}
	[Export] public AnimatedSprite2D PortalSprite;
	[Export] public PortalType Type = PortalType.NormalLevelEntrance;
	[Export] public bool NeedToCompleteWaves = true;
	[Export] public bool DebugMode = false;
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
				Tween tween = CreateTween();
				tween.TweenProperty(this, "scale", Vector2.One, 1f)
					.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
			}
		}
	} = false;
	private bool _isPlayerNearby = false;
	private PackedScene _targetScene = null;
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
		if (!DebugMode)
		{
			BaseLevel baseLevel = GetTree().CurrentScene as BaseLevel;
			if (baseLevel != null && Type == PortalType.NormalLevelEntrance)
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
		if (Type == PortalType.ExtraLevelEntrance)
		{
			Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
			if (player != null)
				player.GlobalPosition = GlobalPosition;
		}
	}

	public override void _Process(double delta)
	{
		if (!_isPlayerNearby || _isTeleporting || !Input.IsActionJustPressed("Interact") || !IsInteractable)
			return;
		_isTeleporting = true;
		if (Type == PortalType.NormalLevelEntrance)
		{
			MapManager.Instance.RecordReturnPosition(GlobalPosition);
			if (_targetScene == null)
			{
				Godot.Collections.Array<PackedScene> pool = MapManager.Instance.ExtraMapPool;
				_targetScene = pool[Convert.ToInt32(GD.Randi() % pool.Count)];
			}
			SceneManager.Instance.ChangeScene(_targetScene);
			_isEntered = true;
		}
		else
		{
			MapManager.Instance.ReturnToMainMap();
		}

	}
	public GDDictionary SaveState()
	{
		return new()
		{
			["TargetScenePath"] = _targetScene,
			["IsInteractable"] = IsInteractable,
			["IsEntered"] = _isEntered
		};
	}
	public void LoadState(GDDictionary state)
	{
		if (state?.TryGetValue("TargetScenePath", out var targetScenePath) ?? false)
			_targetScene = (PackedScene)targetScenePath;
		if (state?.TryGetValue("IsInteractable", out var isInteractable) ?? false)
			IsInteractable = (bool)isInteractable;
		if (state?.TryGetValue("IsEntered", out var isEntered) ?? false)
			_isEntered = (bool)isEntered;
	}
}
