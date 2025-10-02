# Scene Manager 使用方法

首先获取目标场景的相对路径ScenePath
然后调用
```
SceneManager.Instance.ChangeScene(ScenePath);
```
来转换场景。

目前只有黑幕，后面可以添加其他动画过渡。

## 关于跨场景存储数据
目前思路是建立一个全局数据类GameData，在里面定义所需的数据，然后再脚本中添加：
```
public GameData GameData => (GameData)GetNode("/root/GameData");
```
随后直接用`GameData.health  #（例）`来访问对应数据