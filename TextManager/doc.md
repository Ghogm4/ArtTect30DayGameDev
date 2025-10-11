# TextManager
使在需要播放对话的时候, 提前调用
```
TextManager.Instance.LoadLines("PathToJson", "SceneName");
TextManager.Instance.LoadTextScene();
```

然后调用
```
SignalBus.Instance.EmitSignal(SignalBus.SignalName.ShowText);
```
从第一条剧情开始播放。
可以支持鼠标和ui_accept的跳过动画
播放完成后场景会queue

# 剧情文本
剧情格式如下：
```json
{
    "scene1":
        [
            {
                "Id": 1
                "Side": "Left / Right"
                "Profile": 
                "PathToProfile"
                "Text": "剧情文本。。。"
            }
            {
                "Id" : "2"
                //...
            }
        ]
    "scene2":
        []
        //...
}
```
建议先在textscene中设置好初始样式
理论上richtextlabel可以用bbcode实现变化样式，但是我不会，理论上如果想要别的变化，可以直接在json中加字段，比如`"font": "Courier"`这种，只要我在解析的时候加一行就行了。
