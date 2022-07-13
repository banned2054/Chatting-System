using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMgr : MonoBehaviour
{
    [Tooltip("�Ի��б�")] public List<DialogueSO> DialogueSoList;

    [SerializeField] private GameObject _electItemPrefab; //ѡ�ť
    [SerializeField] private Transform _electItemsParent; //ѡ��ĸ��ڵ�
    [SerializeField] private Button _nextButton; //�����һ���Ի�
    [SerializeField] private Text _dialogueNameText; //�Ի��ߵ�����
    [SerializeField] private Text _dialogueMessageText; //�Ի�����
    [SerializeField] private AudioSource _backgroundAudioSource;
    [SerializeField] private AudioSource _speakerAudioSource;

    private List<DialogueSO.Dialogue> _dialogueList; //��ǰ�����жԻ��б�
    private int _currentIndex; //��ǰ�ĶԻ����
    private float _currentJumpTime; //��ǰ�Ĺ����ٶ�
    private float _autoWaitTime; //�Զ��ĵȴ�ʱ��
    private bool _isSpell; //�Ƿ��ڹ�����ʾ��
    private bool _isBreak; //�Ƿ�ֹͣ������ʾ����ǰ���ӣ�
    private bool _isJump; //�Ƿ���������ʾ��ȫ�֣�
    private bool _isAuto; //�Ƿ����Զ�
    private AudioClip _backgroundAudio;
    private int _currentTimes = 0;

    public void ShowDialogue(int number)
    {
        {
            if (number >= DialogueSoList.Count || number < 0) return;
            if (DialogueSoList[number].DialogueList.Count == 0) return;
        } //Խ���ж�

        {
            {
                _dialogueList = DialogueSoList[number].DialogueList;
                _currentIndex = 0;
            } //����Ի���dialogueList��

            {
                _isJump = DialogueSoList[number].IsJump;
                _currentJumpTime = DialogueSoList[number].JumpTime;
                _isAuto = DialogueSoList[number].IsAuto;
                _autoWaitTime = DialogueSoList[number].AutoWaitTime;
            } //������auto������


            if (DialogueSoList[number].IsPlayBackroundAudio)
            {
                _backgroundAudioSource.Stop();
                _backgroundAudio = DialogueSoList[number].BackgroundAudioClip;
                _backgroundAudioSource.PlayOneShot(_backgroundAudio, 0.5f);
            } //�������ֵ�����

            _currentTimes++;
        } //��ȡ�öζԻ�����������

        if (!_dialogueList[_currentIndex].IsElect)
        {
            ShowMessage();
        } //�Ի�
        else
        {
            ShowElect();
        } //ѡ��
    }

    public void NextButtonDown()
    {
        if (_isSpell)
        {
            _isBreak = true;
        }
        else
        {
            if (_currentIndex >= _dialogueList.Count || _currentIndex < 0) return;
            if (!_dialogueList[_currentIndex].IsElect)
            {
                ShowMessage();
            }
            else
            {
                ShowElect();
            }
        }
    } //����next��ť

    void ShowMessage()
    {
        if (_currentIndex >= _dialogueList.Count || _currentIndex < 0) return;

        _nextButton.gameObject.SetActive(true);
        _isSpell = true; //��ʼƴд����

        string speaker = _dialogueList[_currentIndex].MessageItem.CharacterName;
        string message = _dialogueList[_currentIndex].MessageItem.Message;
        float audioTime = 0;
        if (_dialogueList[_currentIndex].AudioItem.IsPlayAudio)
        {
            audioTime = _dialogueList[_currentIndex].AudioItem.Audio.length;
            _speakerAudioSource.clip = _dialogueList[_currentIndex].AudioItem.Audio;
            _speakerAudioSource.Play();
        } //�������ӵ���Ƶ����

        _currentIndex = _dialogueList[_currentIndex].MessageItem.MessageNext; //����index
        StartCoroutine(NextDialogueWaitor(_currentJumpTime, speaker, message, audioTime)); //��ʼչʾ�Ի�
    } //չʾ�Ի�

    void ShowElect()
    {
        _nextButton.gameObject.SetActive(false);
        if (_currentIndex >= _dialogueList.Count || _currentIndex < 0) return;
        if (_dialogueList[_currentIndex].ElectItems.Count == 0) return;

        _electItemsParent.gameObject.SetActive(true);

        foreach (Transform child in _electItemsParent)
        {
            GameObject.Destroy(child.gameObject);
        } //��վɵ�ѡ��


        for (int i = 0; i < _dialogueList[_currentIndex].ElectItems.Count; i++)
        {
            int x = _dialogueList[_currentIndex].ElectItems[i].NextDialogue;
            var newElectButton = Instantiate(_electItemPrefab, _electItemsParent).GetComponent<Button>();
            newElectButton.gameObject.SetActive(true);
            newElectButton.transform.GetChild(0).GetComponent<Text>().text =
                _dialogueList[_currentIndex].ElectItems[i].ElectMessage;
            newElectButton.onClick.AddListener(delegate { ElectButtonDown(x); });
        } //����µİ�ť
    }

    void ElectButtonDown(int target)
    {
        _electItemsParent.gameObject.SetActive(false);
        _currentIndex = target;
        if (_currentIndex >= _dialogueList.Count || _currentIndex < 0) return;
        if (!_dialogueList[_currentIndex].IsElect)
        {
            ShowMessage();
        }
        else
        {
            ShowElect();
        }
    } //�����ť�����ת

    IEnumerator NextDialogueWaitor(float jumpTime, string name, string message, float audioTime)
    {
        bool flag = false;
        float sumTime = 0f;
        if (audioTime > 0)
        {
            flag = true;
        }//�Ƿ�������

        int myTime = _currentTimes;
        if (!_isJump)
        {
            _dialogueMessageText.text = message;
            if (flag)
            {
                yield return new WaitForSeconds(audioTime);
                flag = false;
            }
        }//�޹����������������ͽ���
        else
        {
            int i;
            _dialogueMessageText.text = "";
            _dialogueNameText.text = name;
            for (i = 0; i < message.Length; i++)
            {
                if (myTime != _currentTimes)
                {
                    break;
                }//���¿����Ի�������currentTimes�����ˣ�ֱ�ӽ�����ǰ���

                if (_isBreak)
                {
                    _dialogueMessageText.text = message;
                    _isBreak = false;
                    _speakerAudioSource.Stop();
                    flag = false;
                    break;
                }//�����������ٴε��next��ť��ֱ����ʾ���ζԻ�

                _dialogueMessageText.text += message[i];
                yield return new WaitForSeconds(jumpTime);
                sumTime += jumpTime;
                if (sumTime >= audioTime) flag = false;//�����������ʱ���ѳ�����Ƶ��flag=false
            }
        }

        if (myTime == _currentTimes)
        {
            if (flag)
            {
                yield return new WaitForSeconds(audioTime - sumTime);
            }//�����Ƶ��û���꣬�ȴ���Ƶ����

            _isSpell = false;//����ƴд״̬
            _nextButton.gameObject.SetActive(true);

            if (_isAuto)
            {
                yield return new WaitForSeconds(_autoWaitTime);
                if (_currentIndex < _dialogueList.Count)
                {
                    if (!_dialogueList[_currentIndex].IsElect)
                    {
                        ShowMessage();
                    }
                    else
                    {
                        ShowElect();
                    }
                }
            }//�Զ�ģʽ��ֱ����ת��һ�Ի�
        }
    }//������ʾ
}