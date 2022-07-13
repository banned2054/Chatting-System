using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new Dialogue",menuName = "Data/new Dialogue")]
public class DialogueSO : ScriptableObject
{
    [Header("显示效果")]
    [Tooltip("false:直接显示\ntrue:滚动显示")]
    public bool IsJump;

    [Tooltip("滚动显示的速度")] public float JumpTime = 0;

    [Header("自动播放设置")]
    [Tooltip("开启自动播放，遇到选项会暂停")]
    public bool IsAuto = false;

    [Tooltip("自动播放时，每个句子之间的停顿时间")] public float AutoWaitTime;

    [Header("音频设置")]
    [Tooltip("开启自动播放，遇到选项会暂停")]
    public bool IsPlayBackroundAudio;

    [Tooltip("背景音乐")] public AudioClip BackgroundAudioClip;

    [Header("对话列表")] public List<Dialogue> DialogueList;


    [Serializable]
    public class Dialogue
    {
        [Tooltip("是否是选项")] public bool IsElect = false;
        [Tooltip("对话")] public MessageItem MessageItem;
        [Tooltip("选择")] public List<ElectItem> ElectItems;

        [Tooltip("语音")] public AudioItem AudioItem;
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
        [Tooltip("选项内容")] public string ElectMessage;
        [Tooltip("选项跳转的下一个对话位置")] public int NextDialogue;
    }

    [Serializable]
    public class AudioItem
    {
        [Tooltip("是否添加语音")] public bool IsPlayAudio;
        public AudioClip Audio;
    }
}
