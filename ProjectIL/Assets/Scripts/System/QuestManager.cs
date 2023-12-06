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
        questList.Add(10, new QuestData("ù ���� �湮", new int[] { 1000, 1000 }));
        questList.Add(20, new QuestData("���� ã��", new int[] { 1000, 5000, 1000 }));
        questList.Add(30, new QuestData("����Ʈ Ŭ����", new int[] { 0 }));
    }
    public int GetQuestTalkIndex(int id)
    {
        return questId + questActionIndex;
    }

    public string CheckQuest(int id)
    {
        //����Ʈ ������Ʈ ����
        ControlObject();

        //���� �׼�
        if (id == questList[questId].npcId[questActionIndex])
        {
            questActionIndex++;
        }

        //����Ʈ �Ϸ�
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
