# 敌人生成器用法

## `EnemyMap`
首先在场景根节点下放一个`EnemyMap`节点。在检查器中的Array栏内拖入你希望加载的所有敌人的场景文件。

## `EnemyMarker`
需要在指定位置生成一个敌人时，添加一个`EnemyMarker`节点，并将`EnemyMap`设为其父节点。  
拖动`EnemyMarker`的位置即可指定敌人生成位置
在检查器中的Dictionary栏内，添加新的键值对以指定敌人和生成概率。例如：
![alt text](doc/image.png)
![alt text](doc/image2.png)
### **务必注意EnemyType中的敌人名称一定要和敌人场景名称完全一致**