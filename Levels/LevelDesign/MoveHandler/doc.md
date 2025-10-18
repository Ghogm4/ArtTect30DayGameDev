# MoveHandler
### 用法：
确定好物体初始位置后，在场景根节点下放置一个MoveHandler节点,并将其拖到你想要的位置。  
  
然后将你想要运动的物体设置为MoveHandler的子节点。在MoveHandler的检查器界面可以对动画进行设置。

### 参数表
- `Xoffset`、`Xoffset`：物体的坐标位移  
- `Rotation Offset`：物体的旋转角度，请设置成正数。  
- `RotationClockwise`：是否顺时针旋转。勾选为顺时针，取消为逆时针。
- `Duration`：动画持续的时间（秒）。
- `TweenType`：动画过渡类型（例如 Linear、Sine、Cubic 等）。
- `EaseType`：动画缓动类型（例如 In、Out、InOut）。
- `Loop`：是否循环播放动画。勾选后动画无限重复。
- `Reverse`：是否反转动画（仅在 Loop 启用时有效）。勾选后动画往复运动。
- `AutoStart`：是否在场景加载时自动开始动画。勾选为自动开始，取消需手动调用 `StartMove()`。

### 示例
1. 设置 `Xoffset = 100`, `Yoffset = 0`, `Duration = 2`，物体将向右移动 100 单位，耗时 2 秒。
2. 设置 `Loop = true`, `Reverse = true`，物体将往复移动。
3. 设置 `RotationOffset = 360`, `RotationClockwise = true`，`Loop = true`, `Reverse = true`, 物体将顺时针不停旋转。

### 调用示例
```csharp
public MoveHandler movingGround = null;
// ...

public override void _Ready()
{
    movingGround = GetTree().GetRoot().FindChild("节点名称", true, false) as MoveHandler;
    // ...
}
// ...

public void some_thing_happened()
{
    movingGround.StartMove();
}
// ...

```