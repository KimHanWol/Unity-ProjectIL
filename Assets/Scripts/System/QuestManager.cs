using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex;
    public GameObject[] questObjects;

    [SerializeField]
    private List<QuestData> questList;

    private Dictionary<int, QuestData> questDictionary;

    void Awake()
    {
        questDictionary = new Dictionary<int, QuestData>();
        GenerateData();
    }

    void GenerateData()
    {
        int questIdIndex = 1;
        foreach (QuestData data in questList)
        {
            data.questId = 1000 * questIdIndex++;
            questDictionary.Add(data.questId, data);
        }
    }

    public int GetQuestTalkIndex(int id)
    {
        return questId + questActionIndex;
    }

    public string CheckQuest(int id)
    {
        if (questDictionary.ContainsKey(questId) == false)
        {
            return "SceneEnd";
        }

        //다음 액션
        if (id == questDictionary[questId].objectId[questActionIndex])
        {
            questActionIndex++;
        }

        //퀘스트 완료
        if (questActionIndex == questDictionary[questId].objectId.Length)
        {
            bool isThereNextQeust = NextQuest();
            if (isThereNextQeust == false) 
            {
                NextScene();
                return "Quest Complete";
            }
        }

        //퀘스트 오브젝트 관리
        ControlObject();

        return questDictionary[questId].questName;
    }

    public string CheckQuest()
    {
        if (questDictionary.ContainsKey (questId) == false)
        {
            Debug.Log("There's no quest : " + questId);
            return "";
        }

        return questDictionary[questId].questName;
    }

    bool NextQuest()
    {
        questId += 1000;
        questActionIndex = 0;

        //All Clear Quest On This Scene 
        return questDictionary.ContainsKey(questId);
    }

    private void NextScene()
    {
        
    }

    public void ControlObject()
    {
        foreach(GameObject questObject in questObjects)
        {
            AutoDialogObject AutoDialogObject = questObject.GetComponent<AutoDialogObject>();
            if (AutoDialogObject != null)
            {
                if (AutoDialogObject.QuestIndex + AutoDialogObject.DialogKey  == questId + questActionIndex + 1)
                {
                    questObject.SetActive(true);
                }
                else
                {
                    questObject.SetActive(false);
                }
            }
        }
    }
}
