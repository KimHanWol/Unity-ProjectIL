using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex;
    public GameObject[] questObject;

    Dictionary<int, QuestData> questList;

    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }
    void GenerateData()
    {
        questList.Add(10, new QuestData("첫 마을 방문", new int[] { 1000, 1000 }));
        questList.Add(20, new QuestData("동전 찾기", new int[] { 1000, 5000, 1000 }));
        questList.Add(30, new QuestData("퀘스트 클리어", new int[] { 0 }));
    }
    public int GetQuestTalkIndex(int id)
    {
        return questId + questActionIndex;
    }

    public string CheckQuest(int id)
    {
        //퀘스트 오브젝트 관리
        ControlObject();

        //다음 액션
        if (id == questList[questId].npcId[questActionIndex])
        {
            questActionIndex++;
        }

        //퀘스트 완료
        if (questActionIndex == questList[questId].npcId.Length)
        {
            NextQuest();
        }

        return questList[questId].questName;
    }

    public string CheckQuest()
    {
        return questList[questId].questName;
    }

    void NextQuest()
    {
        questId += 10;
        questActionIndex = 0;
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
