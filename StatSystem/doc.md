# 使用方法
主要组件包含`StatComponent`与`StatModifierComponent`
`StatComponent`挂载在需要拥有属性的生物下，可以编辑其中的`Stats`。`Stats`是一个`Dictionary<string, Stat>`，`string`对应要加入的属性名称，`Stat`对应基础值。

`StatModifierComponent`挂载在可拾取物品下面，可无限加入`Modifier Resource`，编辑`Value`(用于加成的值)，`Type`(属性的`FinalValue`公式为`(BaseValue+BaseAdd)*Mult+FinalAdd`，修改`Type`可以修改加成到哪个部分)，`ReferencedPercentage`(需要引用其他属性的值时，可以填写百分比，100代表100%)，`ReferencedStatName`(引用属性的名字)，`TargetStatName`(加成到哪个属性)

`Player`的拾取范围碰到处于`Pickups`组的东西，就会自动根据其所携带的`StatModifierComponent`进行加成（前提是记得挂，挂完将根节点加入`Pickups`组）
