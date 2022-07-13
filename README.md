# Chatting System
 目前完成了基础的对话功能，功能如下

- [x] 对话
* [x] 选择
+ [x] 滚动对话
+ [x] 自动播放对话
+ [x] 每条对话添加语音

## 数据储存
使用`ScriptableObject`继承的`DialogueSO`文件来储存一段对话内容：
### 滚动播放设置
```c#
public bool IsJump; //true开始滚动对话，字符一个一个的跳动
public float JumpTIme = 0; //每个字符出现的时间，单位为秒
```
### 自动播放设置
```c#
public bool IsAuto = false; //true开启自动对话
public float AutoWaitTime; //每句话滚动显示完成后，停顿时间，再跳转到下一对话
```
### 背景音乐设置
```c#
public bool IsPlayBackroundAudio; //true开启背景音乐
public AudioClip BackgroundAudioClip; //背景音乐的音频
```
### 对话列表
因为会出现选项，对话使用数组储存的伪链表结构进行储存。
```c#
public List<Dialogue> DialogueList;


[Serializable]
public class Dialogue
{
	public bool IsElect = false; //本条对话是选项还是对话
    public MessageItem MessageItem; //对话的存储
    public List<ElectItem> ElectItems; //选项的储存

    public AudioItem AudioItem; //可添加音频，例如角色语音
}

[Serializable]
public class MessageItem
{
	public string CharacterName;
    public string Message;
    public int MessageNext;
}

[Serializable]
public class ElectItem
{
	public string ElectMessage;
    public int NextDialogue;
}

[Serializable]
public class AudioItem
{
	public bool IsPlayAudio;
    public AudioClip Audio;
}
```
## 对话的使用
设置好对话文件后，将其添加到`DialogueMgr`的`Dialogue So List`中，然后调用`DialogueMgr`的`ShowDialogue(number)`函数即可，number值为在上述列表`Dialogue So List`的序号。