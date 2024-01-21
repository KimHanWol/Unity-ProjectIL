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
    public TalkManager talkManager;
    public QuestManager questManager;
    public ItemManager itemManager;
    public UIManager uiManager;

    //Animator
    public Animator talkPanel;
    public Animator PortraitAnim;

    public Image portraitImg;
    public Sprite prevSprite;
    public TypingAnimation TypingAnimation;

    public Text questText;
    public GameObject menuUI;
    public GameObject scanObject;
    public GameObject player;
    public GameObject cutSceneUI;
    public GameObject ItemUI;
    public Image cutSceneImage;
    public Text DebugUI;
    public bool isTalking;
    public int talkIndex;

    public bool CanAction;
    private bool IsPreAnimationPlaying;
    private bool IsPostAnimationPlaying;
    private bool IsTalkDelaying;

    void Start()
    {
        GameLoad(0);
        questText.text = questManager.CheckQuest();
        talkManager.InitializeTalkManager(SceneIndex);
        CanAction = true;
    }

    void Update()
    {
        //Sub Menu
        if(Input.GetButtonDown("Cancel"))
        {
            menuUI.SetActive(!menuUI.activeSelf);
        }

        CanAction = !IsPreAnimationPlaying && !IsPostAnimationPlaying && !IsTalkDelaying;
    }

    public void Action()
    {
        if(CanAction == false)
        {
            return;
        }

        if(scanObject == null)
        {
            return;
        }

        LFGameObject LFGameObject = scanObject.GetComponent<LFGameObject>();
        if(LFGameObject != null)
        {
            switch(LFGameObject.lFGameObjectType)
            {
                case LFGameObjectType.None:
                    UnityEngine.Debug.Log("the Scan Object Has None Type, Please Select Object's Type");
                    break;
                case LFGameObjectType.Object:
                    Talk(LFGameObject.id, false);
                    break;
                case LFGameObjectType.NPC:
                    Talk(LFGameObject.id, true);
                    break;
                case LFGameObjectType.Portal:
                    MoveMap(LFGameObject);
                    break;
                case LFGameObjectType.AutoDialog:
                    AutoDialogObject autoDialogObject = scanObject.GetComponent<AutoDialogObject>();
                    if (autoDialogObject != null)
                    {
                        Talk(autoDialogObject.DialogKey, autoDialogObject.IsNPC);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void OpenItemUI()
    {
        if (ItemUI.activeSelf == true) return;

        ItemUI.SetActive(true);
        itemManager.UpdateItemBags();
    }

    public bool Talk(int id,  bool isNpc)
    {
         LFGameObject ScanLFObject = scanObject.GetComponent<LFGameObject>();
        if(ScanLFObject == null)
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
            talkPanel.SetBool("isShow", false);
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

        isTalking = true;

        if (TypingAnimation.bIsPlaying)
        {
            TypingAnimation.SetMsg("");
            return true;
        }

        //Pre Play UI Animation
        if (uiManager != null)
        {
            string TalkAnimationKeyString = talkManager.GetAnimationKey(talkDataId, talkIndex, TalkAnimationTiming.Pre);
            if (IsPreAnimationPlaying == false)
            {
                float AnimationLength = uiManager.PlayAnimation(TalkAnimationKeyString);
                if (AnimationLength > 0)
                {
                    IsPreAnimationPlaying = true;
                    isTalking = false;
                    talkPanel.SetBool("isShow", false);
                    StartCoroutine(WaitForSecondsToTalk(AnimationLength, id, isNpc));
                    return true;
                }
            }
            IsPreAnimationPlaying = false;
            talkPanel.SetBool("isShow", true);
        }

        //Playing Play UI Animation
        if (uiManager != null)
        {
            string TalkAnimationKeyString = talkManager.GetAnimationKey(talkDataId, talkIndex, TalkAnimationTiming.Playing);
            uiManager.PlayAnimation(TalkAnimationKeyString);
        }

        //Post Play UI Animation
        if (uiManager != null)
        {
            if(talkIndex > 0)
            {
                string TalkAnimationKeyString = talkManager.GetAnimationKey(talkDataId, talkIndex - 1, TalkAnimationTiming.Post);
                if (IsPostAnimationPlaying == false)
                {
                     float AnimationLength = uiManager.PlayAnimation(TalkAnimationKeyString);
                    if (AnimationLength > 0)
                    {
                        IsPostAnimationPlaying = true;
                        isTalking = false;
                        talkPanel.SetBool("isShow", false);
                        StartCoroutine(WaitForSecondsToTalk(AnimationLength, id, false));
                        return true;
                    }
                }
                IsPostAnimationPlaying = false;
                talkPanel.SetBool("isShow", true);
            }
        }


        //End Talk
        if(talkData == null) 
        {
            //대사 없는 사물
            if (talkIndex == 0)
            {
                AutoDialogObject autoDialogObject = scanObject.GetComponent<AutoDialogObject>();
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
                }
            }
            else
            {
                AutoDialogObject autoDialogObject = scanObject.GetComponent<AutoDialogObject>();
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

/*        if (talkData.Split('&').Length > 1)
        {
            string path = talkData.Split('&')[1].Split('\r')[0];
            Sprite cutSceneSprite = (Sprite)Resources.Load<Sprite>(path);
            cutSceneUI.SetActive(true);
            cutSceneImage.sprite = cutSceneSprite;
        }
        else*/


        {
            cutSceneUI.SetActive(false);

/*            if (isNpc)
            {
                TypingAnimation.SetMsg(talkData.Split(':')[0]);

                portraitImg.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(":")[1]));
                portraitImg.color = new Color(1, 1, 1, 1);

                //Show Portrait Animation
                if (prevSprite != portraitImg.sprite)
                {
                    PortraitAnim.SetTrigger("doEffect");
                    prevSprite = portraitImg.sprite;
                }
            }
            else*/
            {
                TypingAnimation.SetMsg(talkData);

                portraitImg.color = new Color(1, 1, 1, 0);
            }
        }

/*        //Play UI Animation
        if (uiManager != null)
        {
            string TalkAnimationKeyString = talkManager.GetAnimationKey(talkDataId, talkIndex, true);
            if (IsAnimationPlaying == false)
            {
                float AnimationLength = uiManager.PlayAnimation(TalkAnimationKeyString);
                if (AnimationLength > 0)
                {
                    IsAnimationPlaying = true;
                    isTalking = false;
                    talkPanel.SetBool("isShow", false);
                    StartCoroutine(WaitForSecondsToTalk(AnimationLength, id, isNpc));
                    return true;
                }
            }
            IsAnimationPlaying = false;
            talkPanel.SetBool("isShow", true);
        }*/


        isTalking = true;
        talkPanel.SetBool("isShow", isTalking);
        talkIndex++;

        return true;
    }

    public IEnumerator WaitForSecondsToTalk(float AnimationLength, int id, bool isNpc)
    {
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(AnimationLength);

        Talk(id, isNpc);
    }

    void EndTalk(int id, bool bSucceed)
    {
        if(bSucceed == true)
        {
            questText.text = questManager.CheckQuest(id);
            if(questText.text == "SceneEnd")
            {
                LoadNextScene();
                return;
            }

            if (scanObject != null)
            {
                LFGameObject ScanLFObject = scanObject.GetComponent<LFGameObject>();
                if (ScanLFObject != null && ScanLFObject.lFGameObjectType == LFGameObjectType.AutoDialog)
                {
                    scanObject.gameObject.SetActive(false);
                }
            }
        }

        isTalking = false;
        talkIndex = 0;
        cutSceneUI.SetActive(false);
        talkPanel.SetBool("isShow", false);

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
        bool IsCutSceneUIActived = cutSceneUI.activeInHierarchy;
        cutSceneUI.SetActive(false);
        talkPanel.SetBool("isShow", false);

        yield return new WaitForSeconds(duration);

        cutSceneUI.SetActive(IsCutSceneUIActived);
        talkPanel.SetBool("isShow", true);
    }

    public void MoveMap(LFGameObject InScanObject)
    {
        if (InScanObject == null || InScanObject.lFGameObjectType != LFGameObjectType.Portal)
        {
            return;
        }

        Portal portalObject = (Portal)InScanObject;
        if (portalObject == null)
        {
            return;
        }

        if (portalObject.SpawnObject == null)
        {
            return;
        }

        //Z축은 그대로
        player.transform.position = new Vector3(
            portalObject.SpawnObject.transform.position.x,
            portalObject.SpawnObject.transform.position.y,
            player.transform.position.z);
    }

    public void GameSave(int index)
    {
        //빌드 세팅 > PlayerSetting > Company, Product Name 으로 레지스트리에 저장됨
        PlayerPrefs.SetFloat("PlayerX" + index.ToString(), player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY" + index.ToString(), player.transform.position.y);
        PlayerPrefs.SetFloat("QuestId" + index.ToString(), questManager.questId);
        PlayerPrefs.SetFloat("QuestActionIndex" + index.ToString(), questManager.questActionIndex);
        PlayerPrefs.Save();

        DebugUI.text += player.transform.position.ToString() + questManager.questId.ToString() + "\n";
    }

    public void GameLoad(int index)
    {
        return;

        DebugUI.text += "Load Start" + "\n";

        if (PlayerPrefs.HasKey("PlayerX" + index.ToString()))
        {
            DebugUI.text += "No Key" + "\n";
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

        DebugUI.text += "Load Complete" + "\n";
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
