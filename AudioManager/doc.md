# Audio Manager 使用方法
建议参照AudioPlayer的API调用方式

## 创建于加载音频资源
首先建议在场景中创建一个AudioPlayer节点用于脚本管理

通过如下方式加载 BGM 和 SFX 资源（建议在场景初始化时调用）：

```csharp
AudioManager.Instance.LoadBGM("bgm名称", "res://路径");
AudioManager.Instance.LoadSFX("音效名称", "res://路径");
```

## 播放与停止

播放 BGM（可选设置循环区间，单位为秒）：

```csharp
AudioManager.Instance.PlayBGM("bgm名称", startLoop, endLoop);
```
- `startLoop` 和 `endLoop` 可选，设置后将在指定区间无缝循环。

停止 BGM：

```csharp
AudioManager.Instance.StopBGM();
```

播放音效（支持多个音效同时播放）：

```csharp
AudioManager.Instance.PlaySFX("音效名称");
```

## 音量调节

设置 BGM 或 SFX 音量（单位为分贝，推荐范围 -40 ~ 0）：

```csharp
AudioManager.Instance.setBGMVolume(db);
AudioManager.Instance.setSFXVolume(db);
```

建议使用Slider，其中Slider的value应当被转写成db值，例如：
```csharp
float db = Mathf.Lerp(-40, 0, (float)value);
```
