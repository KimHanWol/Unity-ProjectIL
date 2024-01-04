using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex;
    public GameObject[] questObject;

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
        foreach (QuestData data in questList)
        {
            questDictionary.Add(data.questId, data);
        }
    }

    public int GetQuestTalkIndex(int id)
    {
        return questId + questActionIndex;
    }

    public string CheckQuest(int id)
    {
        //퀘스트 오브젝트 관리
        ControlObject();

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
        questId += 10;
        questActionIndex = 0;

        //All Clear Quest On This Scene 
        return questDictionary.ContainsKey(questId);
    }

    private void NextScene()
    {
        
    }

    public void ControlObject()
    {
        switch (questId + questActionIndex)
        {
            case 10:
                break;
            case 20:
                questObject[0].SetActive(true);
                break;
            case 21:
                questObject[0].SetActive(false);
                break;
        }
    }
}
