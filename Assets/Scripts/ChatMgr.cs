using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;

public class ChatMgr : MonoBehaviour
{
    [SerializeField, Header("����"), Tooltip("Ĭ������")]
    private Font _defaultFont;

    [SerializeField, Header("ͼƬ"), Tooltip("�����ͼƬ")]
    private Sprite _chatBackgroundSprite;

    [SerializeField, Tooltip("��ťͼƬ")] private Sprite _chatChooseButtonSprite;

    [SerializeField, Header("����"), Tooltip("ȷ�ϰ���")]
    private string _submitButton;

    [SerializeField, Tooltip("���°���")] private string _verticalAixs;

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
    private Button _chatNextButtonPrefab;
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
        _chatNextButtonPrefab = _chatPanel.transform.GetChild(2).GetChild(1).GetComponent<Button>();

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
        UpdateChat();
    }

    void UpdateChat()
    {
        Debug.Log("current index" +_currentIndex.ToString());
        if (_currentDialogueList.Dialogues.Count <= _currentIndex || _currentIndex < 0)
        {
            _isbeginChat = false;
            _chatPanel.SetActive(false);
            _currentDialogueList = null;
            return;
        }

        GameObject eventSystem = GameObject.Find("EventSystem");
        DialogueList.Dialogue dialogue = _currentDialogueList.Dialogues[_currentIndex];
        ClearButton();

        if (dialogue.DialogueType.CompareTo("Chat") == 0)
        {
            Button newButton = Object.Instantiate(_chatNextButtonPrefab, _choosePanel.transform) as Button;
            newButton.gameObject.SetActive(true);
            _chatName.text = dialogue.Name;
            _chatText.text = dialogue.Conversation;
            int currentTarget = dialogue.NextIndex;
            newButton.onClick.AddListener(() => JumpConversation(currentTarget));

        }
        else
        {
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
                int currentTarget = dialogue.ChoiceIndexList[i];
                newButton.onClick.AddListener(() => JumpConversation(currentTarget));
            }
        }

        GameObject firstButton = _choosePanel.transform.GetChild(0).gameObject;
        eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().firstSelectedGameObject = firstButton;
        eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(firstButton);

        Debug.Log("1");
    }

    void JumpConversation(int x)
    {
        Debug.Log("first button down");
        _currentIndex = x;

        ClearButton();
        UpdateChat();
    }

    void ClearButton()
    {
        for (int i = _choosePanel.transform.childCount - 1; i > -1; i--)
        {
            GameObject currentButton = _choosePanel.transform.GetChild(i).gameObject;
            GameObject.Destroy(currentButton);
        }
    }
}