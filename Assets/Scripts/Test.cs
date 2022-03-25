using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    class DialogueList
    {
        [System.Serializable]
        public class Dialogue
        {
            public string DialogueType;
            public string Name;
            public string Conversation;
            public List<string> ChoiceTextList = new List<string>();
            public List<int> ChoiceIndexList = new List<int>();
        }

        public List<Dialogue> Dialogues = new List<Dialogue>();
    }

    private static string TYPECHAT = "Chat";
    private static string TYPECHOOSE = "Choose";

    void Start()
    {
        DialogueList dialogueList = new DialogueList();
        DialogueList.Dialogue dialogue1 = new DialogueList.Dialogue();
        DialogueList.Dialogue dialogue2 = new DialogueList.Dialogue();
        DialogueList.Dialogue dialogue3 = new DialogueList.Dialogue();
        DialogueList.Dialogue dialogue4 = new DialogueList.Dialogue();
        dialogue1.DialogueType = TYPECHAT;
        dialogue1.Name = "asd";
        dialogue1.Conversation = "你好，你是犯人吗？";

        dialogue2.DialogueType = TYPECHOOSE;
        dialogue2.ChoiceIndexList.Add(2);
        dialogue2.ChoiceIndexList.Add(3);
        dialogue2.ChoiceTextList.Add("不是我");
        dialogue2.ChoiceTextList.Add("是我");

        dialogue3.DialogueType = TYPECHAT;
        dialogue3.Name = "asd";
        dialogue3.Conversation = "好，那我去找别人了";

        dialogue4.DialogueType = TYPECHAT;
        dialogue4.Name = "asd";
        dialogue4.Conversation = "那乖乖站好，别跑了";


        dialogueList.Dialogues.Add(dialogue1);
        dialogueList.Dialogues.Add(dialogue2);
        dialogueList.Dialogues.Add(dialogue3);
        dialogueList.Dialogues.Add(dialogue4);

        string ans = JsonUtility.ToJson(dialogueList);

        Debug.Log(ans);

    }
}