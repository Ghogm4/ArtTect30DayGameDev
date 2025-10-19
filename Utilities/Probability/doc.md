# Probability 类使用文档

## 基本概念
`Probability` 是一个用于处理概率事件的工具类，它允许你注册多个带有权重的事件，并根据权重随机触发其中一个。

## 主要功能
1. 注册带权重的事件
2. 根据权重随机执行事件
3. 支持链式调用
4. 提供静态便捷方法

## 使用方法

### 1. 基础用法
```csharp
Probability probability = new();
probability
    .Register(0.1f, () => GD.Print("Rare"))
    .Register(0.3f, () => GD.Print("Uncommon"))
    .Register(0.6f, () => GD.Print("Common"));
probability.Run();
```
这段代码会：
- 10% 概率打印 "Rare"
- 30% 概率打印 "Uncommon"
- 60% 概率打印 "Common"

### 2. 静态便捷方法
```csharp
// 一次性使用
Probability.Run(
    new Tuple<float, Action>(0.1f, () => GD.Print("Static Rare")),
    new Tuple<float, Action>(0.3f, () => GD.Print("Static Uncommon")),
    new Tuple<float, Action>(0.6f, () => GD.Print("Static Common"))
);

// 二选一
Probability.RunIfElse(0.3f, 
    () => GD.Print("30% chance"), 
    () => GD.Print("70% chance")
);

// 单一概率事件
Probability.RunSingle(0.5f, () => GD.Print("50% chance to print"));
```

### 3. 基于权重的随机
你可以使用任意正数作为权重，系统会自动计算概率：
```csharp
Probability probability = new();
probability
    .Register(5, () => GD.Print("Weight 5"))    // 5/15 = 33.33%
    .Register(7, () => GD.Print("Weight 7"))    // 7/15 = 46.67%
    .Register(3, () => GD.Print("Weight 3"));   // 3/15 = 20%
probability.Run();
```

## 注意事项

### 1. 概率合法性
当使用概率模式时（0-1之间的数值），所有注册事件的概率之和不能超过1：
```csharp
// 错误示例：概率总和超过1
probability
    .Register(0.5f, () => GD.Print("50%"))
    .Register(0.6f, () => GD.Print("60%")); // 总和1.1，会报错
```

### 2. 使用建议
1. 对于需要多次使用的概率系统，创建一个实例并重复使用
```csharp
private Probability _dropProbability = new();

public void InitDrops()
{
    _dropProbability
        .Register(0.7f, DropCommonItem)
        .Register(0.3f, DropRareItem);
}

public void TryDrop()
{
    _dropProbability.Run();
}
```

2. 对于一次性使用，使用静态方法更方便
```csharp
// 简单的50/50判断
Probability.RunIfElse(0.5f, 
    () => GD.Print("Success"), 
    () => GD.Print("Fail")
);
```

### 3. 实际应用场景

#### 物品掉落系统
```csharp
var dropSystem = new Probability();
dropSystem
    .Register(60, () => DropItem("Common"))    // 60% 概率
    .Register(30, () => DropItem("Uncommon"))  // 30% 概率
    .Register(10, () => DropItem("Rare"));     // 10% 概率
```

#### 随机事件系统
```csharp
var eventSystem = new Probability();
eventSystem
    .Register(0.4f, SpawnEnemy)
    .Register(0.3f, SpawnTreasure)
    .Register(0.2f, SpawnHealth)
    .Register(0.1f, SpawnRareItem);
```

#### AI决策系统
```csharp
var aiDecision = new Probability();
aiDecision
    .Register(0.5f, () => enemy.Attack())
    .Register(0.3f, () => enemy.Defend())
    .Register(0.2f, () => enemy.UseSpecialAbility());
```

## 技术细节
- 使用 Godot 的 RandomNumberGenerator 进行随机数生成
- 支持链式调用 API
- 线程安全（每个实例使用独立的随机数生成器）
- 概率精度：使用浮点数，支持最多7位小数的精确度

## 错误处理
- 当概率总和超过1时会报错
- 当没有注册任何事件就调用 Run 时会报错
- 对于静态方法 RunIfElse，概率必须在0-1之间