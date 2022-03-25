using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        DialogueList dialogueList = new DialogueList();
        DialogueList.Dialogue dialogue1 = new DialogueList.Dialogue();
        DialogueList.Dialogue dialogue2 = new DialogueList.Dialogue();
        dialogue1.DialogueType = "Chat";
        dialogue1.Name = "asd";
        dialogue1.Conversation = "���";

        dialogue2.DialogueType = "Choose";
        dialogue2.ChoiceIndexList.Add(1);
        dialogue2.ChoiceIndexList.Add(2);
        dialogue2.ChoiceTextList.Add("������");
        dialogue2.ChoiceTextList.Add("����");

        dialogueList.Dialogues.Add(dialogue1);
        dialogueList.Dialogues.Add(dialogue2);

        string ans = JsonUtility.ToJson(dialogueList);

        Debug.Log(ans);
    }

    // Update is called once per frame
    void Update()
    {
    }
}