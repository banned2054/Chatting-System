using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMgr : MonoBehaviour
{
    [Tooltip("对话列表")] public List<DialogueSO> DialogueSoList;

    [SerializeField] private GameObject _electItemPrefab; //选项按钮
    [SerializeField] private Transform _electItemsParent; //选项的父节点
    [SerializeField] private Button _nextButton; //点击下一个对话
    [SerializeField] private Text _dialogueNameText; //对话者的名字
    [SerializeField] private Text _dialogueMessageText; //对话内容
    [SerializeField] private AudioSource _backgroundAudioSource;
    [SerializeField] private AudioSource _speakerAudioSource;

    private List<DialogueSO.Dialogue> _dialogueList; //当前的所有对话列表
    private int _currentIndex; //当前的对话序号
    private float _currentJumpTime; //当前的滚动速度
    private float _autoWaitTime; //自动的等待时间
    private bool _isSpell; //是否在滚动显示中
    private bool _isBreak; //是否停止滚动显示（当前句子）
    private bool _isJump; //是否开启滚动显示（全局）
    private bool _isAuto; //是否开启自动
    private AudioClip _backgroundAudio;
    private int _currentTimes = 0;

    public void ShowDialogue(int number)
    {
        {
            if (number >= DialogueSoList.Count || number < 0) return;
            if (DialogueSoList[number].DialogueList.Count == 0) return;
        } //越界判断

        {
            {
                _dialogueList = DialogueSoList[number].DialogueList;
                _currentIndex = 0;
            } //保存对话到dialogueList中

            {
                _isJump = DialogueSoList[number].IsJump;
                _currentJumpTime = DialogueSoList[number].JumpTime;
                _isAuto = DialogueSoList[number].IsAuto;
                _autoWaitTime = DialogueSoList[number].AutoWaitTime;
            } //滚动和auto的设置


            if (DialogueSoList[number].IsPlayBackroundAudio)
            {
                _backgroundAudioSource.Stop();
                _backgroundAudio = DialogueSoList[number].BackgroundAudioClip;
                _backgroundAudioSource.PlayOneShot(_backgroundAudio, 0.5f);
            } //背景音乐的设置

            _currentTimes++;
        } //读取该段对话的所有内容

        if (!_dialogueList[_currentIndex].IsElect)
        {
            ShowMessage();
        } //对话
        else
        {
            ShowElect();
        } //选项
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
    } //按下next按钮

    void ShowMessage()
    {
        if (_currentIndex >= _dialogueList.Count || _currentIndex < 0) return;

        _nextButton.gameObject.SetActive(true);
        _isSpell = true; //开始拼写句子

        string speaker = _dialogueList[_currentIndex].MessageItem.CharacterName;
        string message = _dialogueList[_currentIndex].MessageItem.Message;
        float audioTime = 0;
        if (_dialogueList[_currentIndex].AudioItem.IsPlayAudio)
        {
            audioTime = _dialogueList[_currentIndex].AudioItem.Audio.length;
            _speakerAudioSource.clip = _dialogueList[_currentIndex].AudioItem.Audio;
            _speakerAudioSource.Play();
        } //单个句子的音频播放

        _currentIndex = _dialogueList[_currentIndex].MessageItem.MessageNext; //更新index
        StartCoroutine(NextDialogueWaitor(_currentJumpTime, speaker, message, audioTime)); //开始展示对话
    } //展示对话

    void ShowElect()
    {
        _nextButton.gameObject.SetActive(false);
        if (_currentIndex >= _dialogueList.Count || _currentIndex < 0) return;
        if (_dialogueList[_currentIndex].ElectItems.Count == 0) return;

        _electItemsParent.gameObject.SetActive(true);

        foreach (Transform child in _electItemsParent)
        {
            GameObject.Destroy(child.gameObject);
        } //清空旧的选项


        for (int i = 0; i < _dialogueList[_currentIndex].ElectItems.Count; i++)
        {
            int x = _dialogueList[_currentIndex].ElectItems[i].NextDialogue;
            var newElectButton = Instantiate(_electItemPrefab, _electItemsParent).GetComponent<Button>();
            newElectButton.gameObject.SetActive(true);
            newElectButton.transform.GetChild(0).GetComponent<Text>().text =
                _dialogueList[_currentIndex].ElectItems[i].ElectMessage;
            newElectButton.onClick.AddListener(delegate { ElectButtonDown(x); });
        } //添加新的按钮
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
    } //点击按钮后的跳转

    IEnumerator NextDialogueWaitor(float jumpTime, string name, string message, float audioTime)
    {
        bool flag = false;
        float sumTime = 0f;
        if (audioTime > 0)
        {
            flag = true;
        }//是否开启语音

        int myTime = _currentTimes;
        if (!_isJump)
        {
            _dialogueMessageText.text = message;
            if (flag)
            {
                yield return new WaitForSeconds(audioTime);
                flag = false;
            }
        }//无滚动，等语音结束就结束
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
                }//重新开启对话，所以currentTimes更新了，直接结束当前输出

                if (_isBreak)
                {
                    _dialogueMessageText.text = message;
                    _isBreak = false;
                    _speakerAudioSource.Stop();
                    flag = false;
                    break;
                }//滚动过程中再次点击next按钮，直接显示整段对话

                _dialogueMessageText.text += message[i];
                yield return new WaitForSeconds(jumpTime);
                sumTime += jumpTime;
                if (sumTime >= audioTime) flag = false;//如果滚动所用时间已超过音频，flag=false
            }
        }

        if (myTime == _currentTimes)
        {
            if (flag)
            {
                yield return new WaitForSeconds(audioTime - sumTime);
            }//如果音频还没放完，等待音频放完

            _isSpell = false;//结束拼写状态
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
            }//自动模式下直接跳转下一对话
        }
    }//滚动显示
}