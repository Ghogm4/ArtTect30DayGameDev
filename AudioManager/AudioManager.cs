using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    public AudioStreamPlayer BGMPlayer;
    public List<AudioStreamPlayer> SFXPlayers = new List<AudioStreamPlayer>();

    public Dictionary<string, AudioStream> BGMDict = new Dictionary<string, AudioStream>();
    public Dictionary<string, AudioStream> SFXDict = new Dictionary<string, AudioStream>();

    public float loopStart = 0f;
    public float loopEnd = 0f;
    public bool isLooping = false;

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
        BGMPlayer = new AudioStreamPlayer();
        BGMPlayer.Bus = "BGM";
        AddChild(BGMPlayer);

        for (int i = 0; i < 10; i++)
        {
            var SFXPlayer = new AudioStreamPlayer();
            SFXPlayer.Bus = "SFX";
            SFXPlayers.Add(SFXPlayer);
            AddChild(SFXPlayer);
            SFXPlayer.Autoplay = false;
        }
    }
    public void LoadBGM(string name, string path)
    {
        var stream = GD.Load<AudioStream>(path);
        if (stream != null)
        {
            BGMDict[name] = stream;
        }
    }
    public void PlayBGM(string name, float startLoop = 0f, float endLoop = 0f)
    {
        if (BGMDict.ContainsKey(name))
        {
            BGMPlayer.Stream = BGMDict[name];
            BGMPlayer.Play();
            if (endLoop > startLoop && endLoop <= BGMPlayer.Stream.GetLength())
            {
                loopStart = startLoop;
                loopEnd = endLoop;
                isLooping = true;
            }
            else
            {
                isLooping = false;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (isLooping && BGMPlayer.Playing)
        {
            if (BGMPlayer.GetPlaybackPosition() >= loopEnd)
            {
                BGMPlayer.Seek(loopStart);
            }
        }
    }

    public void StopBGM()
    {
        BGMPlayer.Stop();
        isLooping = false;
    }

    public void LoadSFX(string name, string path)
    {
        var stream = GD.Load<AudioStream>(path);
        if (stream != null)
        {
            SFXDict[name] = stream;
        }
    }
    public void PlaySFX(string name)
    {
        if (SFXDict.ContainsKey(name))
        {
            foreach (var sfx in SFXPlayers)
            {
                if (!sfx.Playing)
                {
                    sfx.Stream = SFXDict[name];
                    sfx.Play();
                    return;
                }
            }
        }
    }

    public void setBGMVolume(float volume)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("BGM"), volume);
    }

    public void setSFXVolume(float volume)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), volume);
    }
}
