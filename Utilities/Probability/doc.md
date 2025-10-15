# 使用方法
`Probability` 用于设定概率事件，用法示例：
```csharp
Probability probability = new();
probability
	.Register(0.1f, () => GD.Print("Rare"))
	.Register(0.3f, () => GD.Print("Uncommon"))
	.Register(0.6f, () => GD.Print("Common"));
probability.Run();
```
`Register(float probability, Action action)` 方法用于在这个 `Probability` 对象注册一个对应概率的事件。
随后调用 `probability.Run()` 即可按概率挑选出一个你注册的事件进行触发，上述代码即为 10% 概率打印 Rare，30% 概率打印 Uncommon，60% 概率打印 Common。
`Run` 函数也支持静态调用，即创建一个指定的对象然后立刻 `Run`，例如：
```csharp
Probability.Run(
	new Tuple<float, Action>(0.1f, () => GD.Print("Static Rare")),
	new Tuple<float, Action>(0.3f, () => GD.Print("Static Uncommon")),
	new Tuple<float, Action>(0.6f, () => GD.Print("Static Common"))
);
```
如果注册事件概率相加超过 1，则会报错，例如：
```csharp
probability
	.Register(0.1f, () => GD.Print("Rare"))
	.Register(0.3f, () => GD.Print("Uncommon"))
	.Register(0.6f, () => GD.Print("Common"))
	.Register(0.001f, () => GD.Print("SHIT"));
```