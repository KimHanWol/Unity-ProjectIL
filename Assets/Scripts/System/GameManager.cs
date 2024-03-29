using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static TalkManager;

public class GameManager : MonoBehaviour
{
    //Manager
    private TalkManager talkManager;
    private QuestManager questManager;
    private UIManager uiManager;
    private SoundManager soundManager;

    public GameObject scanObject;
    public GameObject player;
    public bool isTalking;
    public int talkIndex;

    public bool CanAction;
    private bool IsPreTalkEventStarted;
    private bool IsPlayingTalkEventStarted;
    private bool IsPostTalkEventStarted;

    private bool IsTalkEventEnded;

    private bool IsTalkDelaying;
    LFGameObject ScanLFObject;
    private LFGameObject CurrentlyInteractedObject;

    void Awake()
    {
        talkManager = GetComponentInChildren<TalkManager>();
        questManager = GetComponentInChildren<QuestManager>();
        uiManager = GetComponentInChildren<UIManager>();
        soundManager = GetComponentInChildren<SoundManager>();
    }

    void Start()
    {
        GameLoad(0);

        uiManager.SetQuestText(questManager.CheckQuest());
        talkManager.InitializeTalkManager(SceneIndex);
        CanAction = true;

        soundManager.PlayBGMLoopBySceneIndex(SceneIndex);
    }

    void Update()
    {
        //Sub Menu
        if(Input.GetButtonDown("Cancel"))
        {
            if(uiManager != null)
            {
                uiManager.ToggleMenu();
            }
        }
    }

    public bool Action()
    {
        if(CanAction == false)
        {
            return false;
        }

        if (isTalking == true && CurrentlyInteractedObject != null)
        {
            ScanLFObject = CurrentlyInteractedObject;
        }
        else
        {
            if(scanObject != null)
            {
                ScanLFObject = scanObject.GetComponent<LFGameObject>();
            }
            else
            {
                ScanLFObject = null;
            }
        }

        CurrentlyInteractedObject = ScanLFObject;

        if (ScanLFObject == null)
        {
            return false;
        }

        if(scanObject != null && scanObject.activeInHierarchy == false)
        {
            return false;
        }

        bool IsActionComplete = false;

        switch (ScanLFObject.lFGameObjectType)
        {
            case LFGameObjectType.None:
                UnityEngine.Debug.Log("the Scan Object Has None Type, Please Select Object's Type");
                IsActionComplete = false;
                break;
            case LFGameObjectType.Object:
                IsActionComplete = Talk(ScanLFObject.id, false);
                break;
            case LFGameObjectType.NPC:
                IsActionComplete = Talk(ScanLFObject.id, true);
                break;
            case LFGameObjectType.Portal:
                IsActionComplete = MoveMap(ScanLFObject);
                break;
            case LFGameObjectType.AutoDialog:
                AutoDialogObject autoDialogObject = ScanLFObject.GetComponent<AutoDialogObject>();
                if (autoDialogObject != null)
                {
                    IsActionComplete = Talk(autoDialogObject.id, autoDialogObject.IsNPC);
                }
                break;
            default:
                break;
        }

        return IsActionComplete;
    }

    public bool CheckCanAction()
    {
        CanAction =
            //During Talk Event Play
            !((IsPreTalkEventStarted || IsPlayingTalkEventStarted || IsPostTalkEventStarted) && !IsTalkEventEnded) &&
            !IsTalkDelaying;

        return CanAction;
    }

    public bool Talk(int id,  bool isNpc)
    {
        if (ScanLFObject == null)
        {
            return false;
        }

        CheckCanAction();

        //When Talk Event Playing
        if (CanAction == false)
        {
            return false;
        }

        int questTalkIndex = 0;

        int talkDataId = id + questManager.questId;

        //Set Talk Data
        questTalkIndex = questManager.GetQuestTalkIndex(id);

        string talkData = "";
        //ScanLFObject.id == id
        talkData = talkManager.GetTalkText(talkDataId, talkIndex, questTalkIndex);


        //If There Is Talk Delay
        float TalkDelay = talkManager.GetTalkDelay(id + questTalkIndex, talkIndex);
        if (TalkDelay > 0 && IsTalkDelaying == false)
        {
            IsTalkDelaying = true;
            isTalking = false;
            uiManager.EnableDialogUIWithAnimation(false);
            StartCoroutine(WaitForSecondsToTalk(TalkDelay, id, isNpc));
            return true;
        }
        else
        {
            IsTalkDelaying = false;
        }

        //It's Not For Current Quest
        if (talkManager.IsTalkDataForCurrentQuest(talkDataId, questManager.questId) == false)
        {
            //return false;
        }

        if (uiManager == null)
        {
            return false;
        }

        if(soundManager == null) 
        {
            return false;    
        }

        isTalking = true;

        if (uiManager.IsTyping())
        {
            uiManager.SetTypingText("");
            return true;
        }

        //Pre Play UI Animation
        {
            string[] TalkAnimationKeyStringList = talkManager.GetAnimationKeyList(talkDataId, talkIndex, AnimationTiming.Pre);
            if (IsPreTalkEventStarted == false)
            {
                float AnimationLength = 0.0f;
                foreach (string TalkAnimationKeyString in TalkAnimationKeyStringList)
                {
                    float InAnimationLength = uiManager.PlayAnimation(TalkAnimationKeyString);
                    AnimationLength = AnimationLength > InAnimationLength ? AnimationLength : InAnimationLength;
                }
                if (AnimationLength > 0)
                {
                    IsPreTalkEventStarted = true;
                    isTalking = false;
                    uiManager.EnableDialogUIWithAnimation(false);
                    StartCoroutine(WaitForSecondsToTalk(AnimationLength, id, isNpc));
                    return true;
                }
            }

            IsPreTalkEventStarted = false;
            uiManager.EnableDialogUIWithAnimation(true);
        }

        //Pre Talk Event
        string[] PreTalkEventKeyList = talkManager.GetTalkEventKeyList(talkDataId, talkIndex, AnimationTiming.Pre);
        foreach (string PreTalkEventKey in PreTalkEventKeyList)
        {
            CheckTalkEvent(PreTalkEventKey);
        }

        //Playing Play UI Animation
        {
            string[] TalkAnimationKeyStringList = talkManager.GetAnimationKeyList(talkDataId, talkIndex, AnimationTiming.Playing);
            if (IsPlayingTalkEventStarted == false)
            {
                float AnimationLength = 0.0f;
                foreach (string TalkAnimationKeyString in TalkAnimationKeyStringList)
                {
                    float InAnimationLength = uiManager.PlayAnimation(TalkAnimationKeyString);
                    AnimationLength = AnimationLength > InAnimationLength ? AnimationLength : InAnimationLength;
                }
                if (AnimationLength > 0)
                {
                    IsPlayingTalkEventStarted = true;
                    isTalking = false;
                    uiManager.EnableDialogUIWithAnimation(false);
                    StartCoroutine(WaitForSecondsToTalk(AnimationLength, id, isNpc));
                    return true;
                }
            }

            IsPlayingTalkEventStarted = false;
            uiManager.EnableDialogUIWithAnimation(true);
        }

        //Playing Talk Event
        string[] PlayingTalkEventKeyList = talkManager.GetTalkEventKeyList(talkDataId, talkIndex, AnimationTiming.Playing);
        foreach (string PlayingTalkEventKey in PlayingTalkEventKeyList)
        {
            CheckTalkEvent(PlayingTalkEventKey);
        }

        //Post Play UI Animation
        {
            if(talkIndex > 0)
            {
                string[] TalkAnimationKeyStringList = talkManager.GetAnimationKeyList(talkDataId, talkIndex - 1, AnimationTiming.Post);
                if (IsPostTalkEventStarted == false)
                {
                    float AnimationLength = 0.0f;
                    foreach (string TalkAnimationKeyString in TalkAnimationKeyStringList)
                    {
                        float InAnimationLength = uiManager.PlayAnimation(TalkAnimationKeyString);
                        AnimationLength = AnimationLength > InAnimationLength ? AnimationLength : InAnimationLength;
                    }
                    if (AnimationLength > 0)
                    {
                        IsPostTalkEventStarted = true;
                        isTalking = false;
                        uiManager.EnableDialogUIWithAnimation(false);
                        StartCoroutine(WaitForSecondsToTalk(AnimationLength, id, isNpc));
                        return true;
                    }
                }

                IsPostTalkEventStarted = false;
                uiManager.EnableDialogUIWithAnimation(true);
            }
        }

        //Post Talk Event
        if(talkIndex > 0)
        {
            string[] PostTalkEventKeyList = talkManager.GetTalkEventKeyList(talkDataId, talkIndex - 1, AnimationTiming.Post);
            foreach(string PostTalkEventKey in PostTalkEventKeyList)
            {
                CheckTalkEvent(PostTalkEventKey);
            }
        }

        //Sound Event
        {
            string EffectSoundKey = talkManager.GetSoundKey(talkDataId, talkIndex);
            soundManager.PlayAudioSound(EffectSoundKey);
        }

        //Cutcene Event
        string CutSceneKey = talkManager.GetCutSceneKey(talkDataId, talkIndex);
        if(CutSceneKey != null && CutSceneKey != "")
        {
            uiManager.ShowCutScene(CutSceneKey);
        }
        else
        {
            uiManager.HideCutScene();
        }

        //End Talk
        if (talkData == null) 
        {
            //대사 없는 사물
            if (talkIndex == 0)
            {
                AutoDialogObject autoDialogObject = ScanLFObject.GetComponent<AutoDialogObject>();
                if (autoDialogObject != null)
                {
                    if (autoDialogObject.QuestIndex != questManager.questId)
                    {
                        EndTalk(id, true);
                        return false;
                    }
                }
                else
                {
                    if (ScanLFObject != null && ScanLFObject.DisplayName.Length > 0)
                    {
                        if (ScanLFObject.DisplayName.Length > 0)
                        {
                            talkData = "평범한 " + ScanLFObject.DisplayName + "(이)다.";
                        }

                        List<string> interactionTalkDataList = new List<string>();
                        foreach (InteractionTalkData scanTalkData in ScanLFObject.InteractionTalkData)
                        {
                            if (scanTalkData.TalkKey == questTalkIndex)
                            {
                                interactionTalkDataList = scanTalkData.TalkDataList;
                                break;
                            }
                        }

                        if (interactionTalkDataList.Count <= 0)
                        {
                            interactionTalkDataList = ScanLFObject.DefaultInteractionTalkData;
                        }

                        if (interactionTalkDataList.Count > 0 &&
                            interactionTalkDataList.Count > ScanLFObject.TalkIndex)
                        {
                            talkData = interactionTalkDataList[ScanLFObject.TalkIndex];
                            ScanLFObject.TalkIndex++;
                        }
                        else
                        {
                            ScanLFObject.TalkIndex = 0;
                        }
                    }
                    else
                    {
                        EndTalk(id, true);
                        return false;
                    }
                }
            }
            else
            {
                AutoDialogObject autoDialogObject = ScanLFObject.GetComponent<AutoDialogObject>();
                if(autoDialogObject != null)
                {
                    if (autoDialogObject.QuestIndex != questManager.questId)
                    {
                        return false;
                    }
                }

                EndTalk(id, true);
                return true;
            }
        }

        {
            uiManager.SetTypingText(talkData);
            uiManager.SetPortraitColor(new Color(1, 1, 1, 0));
        }


        isTalking = true;
        uiManager.EnableDialogUIWithAnimation(isTalking);
        talkIndex++;

        return true;
    }

    public IEnumerator WaitForSecondsToTalk(float AnimationLength, int id, bool isNpc)
    {
        IsTalkEventEnded = false;

        yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(AnimationLength);

        IsTalkEventEnded = true;
        Talk(id, isNpc);
    }

    void EndTalk(int id, bool bSucceed)
    {
        if(bSucceed == true)
        {
            string QuestText = questManager.CheckQuest(id);
            uiManager.SetQuestText(QuestText);
            if(QuestText == "SceneEnd")
            {
                LoadNextScene();
                return;
            }

            if (ScanLFObject != null)
            {
                if (ScanLFObject != null && ScanLFObject.lFGameObjectType == LFGameObjectType.AutoDialog)
                {
                    ScanLFObject.gameObject.SetActive(false);
                }
            }
        }

        isTalking = false;
        talkIndex = 0;
        uiManager.EnableDialogUIWithAnimation(false);

        /*        else
                {
                    isTalking = false;
                    talkIndex = 0;
                    cutSceneUI.SetActive(false);
                    talkPanel.SetBool("isShow", isTalking);
                }*/
    }

    IEnumerator WaitTalkDelay(float duration)
    {
        uiManager.EnableDialogUIWithAnimation(false);
        yield return new WaitForSeconds(duration);
        uiManager.EnableDialogUIWithAnimation(true);
    }

    public void CheckTalkEvent(string TalkEventKey)
    {
        if(TalkEventKey == null || TalkEventKey == "" )
        {
            return;
        }

        SpriteRenderer PlayerSprite = player.GetComponent<SpriteRenderer>();
        if(PlayerSprite == null)
        {
            return;
        }

        Animator PlayerAnimator = player.GetComponent<Animator>();
        if(PlayerAnimator == null)
        {
            return;
        }

        Camera PlayerCamera = player.GetComponentInChildren<Camera>();
        if(PlayerCamera == null)
        {
            return;
        }

        PlayerAction PlayerActionComp = player.GetComponent<PlayerAction>();
        if(PlayerActionComp == null)
        {
            return;
        }

        switch (TalkEventKey)
        {
            case "Teleport_Bed":
                player.transform.position = new Vector3(-24.0f, -1.66f, -1.0f);
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerCamera.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerAnimator.SetInteger("vAxisRaw", -1);
                PlayerAnimator.SetBool("isChange", true);
                break;
            case "Teleport_Home":
                player.transform.position = new Vector3(-3.09f, -2.89f, -1.0f);
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerCamera.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerAnimator.SetInteger("vAxisRaw", -1);
                PlayerAnimator.SetBool("isChange", true);
                break;
            case "Teleport_VR_Home":
                player.transform.position = new Vector3(1.0f, 43.0f, -1.0f);
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerCamera.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerAnimator.SetInteger("vAxisRaw", -1);
                PlayerAnimator.SetBool("isChange", true);
                break;
            case "Teleport_VR_Vacuity_Black":
                player.transform.position = new Vector3(1.0f, -18.0f, -1.0f);
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerCamera.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerAnimator.SetInteger("vAxisRaw", -1);
                PlayerAnimator.SetBool("isChange", true);
                break;
            case "Teleport_VR_Vacuity_White":
                player.transform.position = new Vector3(1.0f, -53.0f, -1.0f);
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerCamera.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerAnimator.SetInteger("vAxisRaw", -1);
                PlayerAnimator.SetBool("isChange", true);
                break;
            case "Teleport_VR_Reality":
                player.transform.position = new Vector3(16.9f, -4.2f, -1.0f);
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerCamera.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerAnimator.SetInteger("vAxisRaw", -1);
                PlayerAnimator.SetBool("isChange", true);
                break;
            case "PassOut_Start":
                PlayerSprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                PlayerCamera.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerSprite.color = Color.gray;
                PlayerAnimator.SetInteger("vAxisRaw", 1);
                PlayerAnimator.SetBool("isChange", true);
                break;
            case "PassOut_End":
                PlayerSprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerCamera.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                PlayerSprite.color = Color.white;
                PlayerAnimator.SetInteger("vAxisRaw", -1);
                PlayerAnimator.SetBool("isChange", true);
                break;
            case "HighSpeed_Start":
                PlayerActionComp.Speed *= 2;
                break;
            case "HighSpeed_End":
                PlayerActionComp.Speed /= 2;
                break;
        }
    }

    public bool MoveMap(LFGameObject InScanObject)
    {
        if (InScanObject == null || InScanObject.lFGameObjectType != LFGameObjectType.Portal)
        {
            return false;
        }

        Portal portalObject = (Portal)InScanObject;
        if (portalObject == null)
        {
            return false;
        }

        if (portalObject.SpawnObject == null)
        {
            return false;
        }

        //Z축은 그대로
        player.transform.position = new Vector3(
            portalObject.SpawnObject.transform.position.x,
            portalObject.SpawnObject.transform.position.y,
            player.transform.position.z);

        return true;
    }

    public void GameSave(int index)
    {
        //빌드 세팅 > PlayerSetting > Company, Product Name 으로 레지스트리에 저장됨
        PlayerPrefs.SetFloat("PlayerX" + index.ToString(), player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY" + index.ToString(), player.transform.position.y);
        PlayerPrefs.SetFloat("QuestId" + index.ToString(), questManager.questId);
        PlayerPrefs.SetFloat("QuestActionIndex" + index.ToString(), questManager.questActionIndex);
        PlayerPrefs.Save();

        uiManager.AddDebugText(player.transform.position.ToString() + questManager.questId.ToString());
    }

    public void GameLoad(int index)
    {
        if(uiManager == null)
        {
            return;
        }

        uiManager.AddDebugText("Load Start");

        if (PlayerPrefs.HasKey("PlayerX" + index.ToString()))
        {
            uiManager.AddDebugText("No Key");
            return;
        }


        float x = PlayerPrefs.GetFloat("PlayerX" + index.ToString());
        float y = PlayerPrefs.GetFloat("PlayerY" + index.ToString());
        int questId = PlayerPrefs.GetInt("QuestId" + index.ToString());
        int questActionIndex = PlayerPrefs.GetInt("QuestActionIndex" + index.ToString());

        player.transform.position = new Vector3(x, y, 0);
        questManager.questId = questId;
        questManager.questActionIndex = questActionIndex;
        questManager.ControlObject();

        uiManager.AddDebugText("Load Complete");
    }

    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public int SceneIndex = 0;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(++SceneIndex);
    }

    public int CurrentSceneIndex()
    {
        return SceneIndex;
    }
}
