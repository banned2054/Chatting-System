using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;

public class ChatMgr : MonoBehaviour
{
    [SerializeField, Header("文字"), Tooltip("默认字体")]
    private Font _defaultFont;

    [SerializeField, Header("图片"), Tooltip("聊天框图片")]
    private Sprite _chatBackgroundSprite;

    [SerializeField, Tooltip("按钮图片")] private Sprite _chatChooseButtonSprite;

    [SerializeField, Header("输入"), Tooltip("确认按键")]
    private string _submitButton;

    [SerializeField, Tooltip("上下按键")] private string _verticalAixs;

    [SerializeField] private List<TextAsset> _jsonList;

    class DialogueList
    {
        [System.Serializable]
        public class Dialogue
        {
            public string DialogueType;
            public string Name;
            public string Conversation;
            public int NextIndex;
            public List<string> ChoiceTextList = new List<string>();
            public List<int> ChoiceIndexList = new List<int>();
        }

        public List<Dialogue> Dialogues = new List<Dialogue>();
    }

    private GameObject _chatPanel;
    private Text _chatName;
    private Text _chatText;
    private Image _chatImage;
    private Button _chatChooseButtonPrefab;
    private GameObject _choosePanel;

    private bool _isbeginChat;
    private int _currentIndex;
    private DialogueList _currentDialogueList;

    void Awake()
    {
        _chatPanel = GameObject.Find("ChatPanel");
        _choosePanel = _chatPanel.transform.GetChild(1).gameObject;
        _chatName = _chatPanel.transform.GetChild(0).GetChild(2).GetComponent<Text>();
        _chatText = _chatPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>();
        _chatImage = _chatPanel.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        _chatChooseButtonPrefab = _chatPanel.transform.GetChild(2).GetChild(0).GetComponent<Button>();

        if (_defaultFont != null)
        {
            _chatName.font = _defaultFont;
            _chatText.font = _defaultFont;
        }

        if (_chatBackgroundSprite != null)
        {
            _chatImage.sprite = _chatBackgroundSprite;
        }

        if (_chatChooseButtonSprite != null)
        {
            _chatChooseButtonPrefab.GetComponent<SpriteRenderer>().sprite = _chatChooseButtonSprite;
        }
    }


    void FixedUpdate()
    {
        if (_isbeginChat)
        {
            if (Input.GetAxisRaw(_submitButton) != 0)
            {
                Debug.Log(1);
                UpdateChat();
            }
        }
    }

    public void ShowChat(int fileIndex)
    {
        if (_jsonList.Count < fileIndex)
        {
            return;
        }

        TextAsset currentTestAsset = _jsonList[fileIndex];
        _currentDialogueList = JsonUtility.FromJson<DialogueList>(currentTestAsset.text);
        if (_currentDialogueList.Dialogues.Count < 1)
        {
            _currentDialogueList = null;
            return;
        }

        _isbeginChat = true;
        _currentIndex = 0;
        _chatPanel.SetActive(true);
        _chatName.text = _currentDialogueList.Dialogues[_currentIndex].Name;
        _chatText.text = _currentDialogueList.Dialogues[_currentIndex].Conversation;
    }

    void UpdateChat()
    {
        _currentIndex++;
        if (_currentDialogueList.Dialogues.Count <= _currentIndex)
        {
            _isbeginChat = false;
            _chatPanel.SetActive(false);
            _currentDialogueList = null;
            return;
        }

        GameObject eventSystem = GameObject.Find("EventSystem");
        DialogueList.Dialogue dialogue = _currentDialogueList.Dialogues[_currentIndex];

        if (dialogue.DialogueType.CompareTo("Chat") == 0)
        {
            _chatName.text = dialogue.Name;
            _chatText.text = dialogue.Conversation;
            eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
        else
        {
            _isbeginChat = false;
            int len1 = dialogue.ChoiceTextList.Count;
            int len2 = dialogue.ChoiceIndexList.Count;

            if (len1 == 0 || len2 == 0)
            {
                UpdateChat();
                return;
            }


            int minLen = Mathf.Min(len1, len2);
            for (int i = 0; i < minLen; i++)
            {
                Button newButton = Object.Instantiate(_chatChooseButtonPrefab, _choosePanel.transform) as Button;
                newButton.transform.GetChild(0).GetComponent<Text>().text = dialogue.ChoiceTextList[i];
                newButton.gameObject.SetActive(true);
                int currentTarget = dialogue.ChoiceIndexList[i] - 1;
                newButton.onClick.AddListener(() => JumpConversation(currentTarget));
            }

            GameObject firstButton = _choosePanel.transform.GetChild(0).gameObject;
            eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(firstButton);
        }
    }

    void JumpConversation(int x)
    {
        _currentIndex = x;

        for (int i = _choosePanel.transform.childCount - 1; i > -1; i--)
        {
            GameObject currentButton = _choosePanel.transform.GetChild(i).gameObject;
            GameObject.Destroy(currentButton);
        }

        _isbeginChat = true;
    }
}