# DropTable

一个灵活的掉落物管理系统，支持自动加载、稀有度筛选和类别筛选。

## 基础设置

1. 添加 DropTable 节点到场景
2. 配置掉落物来源（可选其一或同时使用）：
   ```
   自动加载:
   - 启用 "Auto Load From Directories"
   - 设置要扫描的目录（默认：Combat/Movement/General文件夹）

   手动加载:
   - 在 "Manual Boost List" 添加物品
   - 如果使用Manual模式，在 "Manual Probabilities" 设置概率
   ```

## 掉落模式

在Inspector中选择掉落模式：

- **Manual**: 使用自定义概率（从Manual Probabilities中获取）
- **UniformRarity**: 选中稀有度的物品等概率掉落
- **UniformCategory**: 选中类别的物品等概率掉落

## 掉落物过滤

### 稀有度过滤
启用/禁用特定稀有度：
- Enable Common Rarity（普通）
- Enable Uncommon Rarity（非普通）
- Enable Rare Rarity（稀有）
- Enable Epic Rarity（史诗）
- Enable Legendary Rarity（传奇）

### 类别过滤
启用/禁用特定类别：
- Enable Combat Items（战斗物品）
- Enable Movement Items（移动物品）
- Enable General Items（通用物品）

## 掉落设置

- **Times To Run**: 掉落物数量（默认：1）

## 使用示例

1. 掉落3个普通物品：
   ```
   - Times To Run = 3
   - Drop Mode = UniformRarity
   - 只启用 "Common Rarity"
   ```

2. 按自定义概率掉落稀有战斗物品：
   ```
   - Drop Mode = Manual
   - 启用 "Rare Rarity" 和 "Combat Items"
   - 在 Manual Probabilities 中设置概率
   ```

3. 自动加载所有移动类增益：
   ```
   - 启用 Auto Load From Directories
   - Drop Mode = UniformCategory
   - 只启用 "Movement Items"
   ```

## 代码使用

```csharp
// 手动触发掉落
dropTable.Drop();

// 添加自定义概率
dropTable.ManualProbabilities["HealthBoost"] = 0.5f;
dropTable.ManualProbabilities["SpeedBoost"] = 0.3f;
```

## 提示

- 物品必须有正确设置稀有度和类别的BoostInfo组件
- 过滤器使用AND逻辑 - 物品必须同时通过稀有度和类别过滤
- 自动加载会扫描指定目录中的.tscn文件
- 可以同时使用手动和自动加载的物品
