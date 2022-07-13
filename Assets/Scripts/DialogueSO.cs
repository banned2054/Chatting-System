using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new Dialogue",menuName = "Data/new Dialogue")]
public class DialogueSO : ScriptableObject
{
    [Header("��ʾЧ��")]
    [Tooltip("false:ֱ����ʾ\ntrue:������ʾ")]
    public bool IsJump;

    [Tooltip("������ʾ���ٶ�")] public float JumpTime = 0;

    [Header("�Զ���������")]
    [Tooltip("�����Զ����ţ�����ѡ�����ͣ")]
    public bool IsAuto = false;

    [Tooltip("�Զ�����ʱ��ÿ������֮���ͣ��ʱ��")] public float AutoWaitTime;

    [Header("��Ƶ����")]
    [Tooltip("�����Զ����ţ�����ѡ�����ͣ")]
    public bool IsPlayBackroundAudio;

    [Tooltip("��������")] public AudioClip BackgroundAudioClip;

    [Header("�Ի��б�")] public List<Dialogue> DialogueList;


    [Serializable]
    public class Dialogue
    {
        [Tooltip("�Ƿ���ѡ��")] public bool IsElect = false;
        [Tooltip("�Ի�")] public MessageItem MessageItem;
        [Tooltip("ѡ��")] public List<ElectItem> ElectItems;

        [Tooltip("����")] public AudioItem AudioItem;
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
        [Tooltip("ѡ������")] public string ElectMessage;
        [Tooltip("ѡ����ת����һ���Ի�λ��")] public int NextDialogue;
    }

    [Serializable]
    public class AudioItem
    {
        [Tooltip("�Ƿ��������")] public bool IsPlayAudio;
        public AudioClip Audio;
    }
}
