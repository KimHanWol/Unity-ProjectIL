using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Manager
    public TalkManager talkManager;
    public QuestManager questManager;
    public ItemManager itemManager;

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
    public bool isAction;
    public int talkIndex;

    void Start()
    {
        GameLoad(0);
        questText.text = questManager.CheckQuest();
    }

    void Update()
    {
        //Sub Menu
        if(Input.GetButtonDown("Cancel"))
        {
            menuUI.SetActive(!menuUI.activeSelf);
        }
    }

    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        JFGameObject JFGameObject = scanObject.GetComponent<JFGameObject>();
        Talk(JFGameObject.id, JFGameObject.isNpc);

        talkPanel.SetBool("isShow", isAction);
    }

    public void OpenItemUI()
    {
        if (ItemUI.activeSelf == true) return;

        ItemUI.SetActive(true);
        itemManager.UpdateItemBags();
    }

    void Talk(int id, bool isNpc)
    {
        int questTalkIndex = 0;
        string talkData = "";

        if(TypingAnimation.bIsPlaying)
        {
            TypingAnimation.SetMsg("");
            return;
        }
        else
        {
            questTalkIndex = questManager.GetQuestTalkIndex(id);
            talkData = talkManager.GetTalk(id + questTalkIndex, talkIndex);
        }
        //Set Talk Data
        questTalkIndex = questManager.GetQuestTalkIndex(id);
        talkData = talkManager.GetTalk(id + questTalkIndex, talkIndex);

        //End Talk
        if(talkData == null) 
        {
            isAction = false;
            talkIndex = 0;
            questText.text = questManager.CheckQuest(id);
            cutSceneUI.SetActive(false);
            return;
        }

        if (talkData.Split('&').Length > 1)
        {
            string path = talkData.Split('&')[1].Split('\r')[0];
            Sprite cutSceneSprite = (Sprite)Resources.Load<Sprite>(path);
            cutSceneUI.SetActive(true);
            cutSceneImage.sprite = cutSceneSprite;
        }
        else
        {
            cutSceneUI.SetActive(false);

            if (isNpc)
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
            else
            {
                TypingAnimation.SetMsg(talkData);

                portraitImg.color = new Color(1, 1, 1, 0);
            }
        }

        isAction = true;
        talkIndex++;
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
}
