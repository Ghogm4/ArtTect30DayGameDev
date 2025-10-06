using Godot;
using System;

public partial class AudioPlayer : Node
{
	public override void _Ready()
	{
		AudioManager.Instance.LoadBGM("opening", "res://Assets/BGM/suspicious opening.mp3");
		AudioManager.Instance.LoadSFX("confirm", "res://Assets/SFX/confirm.wav");
		AudioManager.Instance.LoadSFX("close", "res://Assets/SFX/close.wav");

		var startBGM = GetNode<Button>("../startBGM");
		startBGM.Pressed += OnStartBGMPressed;
		var endBGM = GetNode<Button>("../endBGM");
		endBGM.Pressed += OnEndBGMPressed;
		var confirmSFX = GetNode<Button>("../confirmSFX");
		confirmSFX.Pressed += OnConfirmSFXPressed;
		var cancelSFX = GetNode<Button>("../cancelSFX");
		cancelSFX.Pressed += OnCancelSFXPressed;

		var bgmSlider = GetNode<HSlider>("../BGMSlider");
		bgmSlider.ValueChanged += OnBGMVolumeChanged;
		var sfxSlider = GetNode<HSlider>("../SFXSlider");
		sfxSlider.ValueChanged += OnSFXVolumeChanged;
	}

	public void OnStartBGMPressed()
	{
		AudioManager.Instance.PlayBGM("opening", 57.78f, 86.22f);
	}
	public void OnEndBGMPressed()
	{
		AudioManager.Instance.StopBGM();
	}
	public void OnConfirmSFXPressed()
	{
		AudioManager.Instance.PlaySFX("confirm");
	}
	public void OnCancelSFXPressed()
	{
		AudioManager.Instance.PlaySFX("close");
	}
	public void OnBGMVolumeChanged(double value)
	{
		float db = Mathf.Lerp(-40, 0, (float)value);
		AudioManager.Instance.setBGMVolume(db);
	}
	public void OnSFXVolumeChanged(double value)
	{
		float db = Mathf.Lerp(-40, 0, (float)value);
		AudioManager.Instance.setSFXVolume(db);
	}
}
