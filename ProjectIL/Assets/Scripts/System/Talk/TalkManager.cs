using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    Dictionary<int, Sprite> portraitData;



    public Sprite[] portraitArr;

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    void GenerateData()
    {
        List<TalkData> TalkDataList = new List<TalkData>();
        List<List<string>> CSVTalkData = CSVReader.GetSCVData("TalkDataCSV");
        foreach(List<string> ListTalkData in CSVTalkData)
        {
            int ListTalkCount = ListTalkData.Count;
            List<string> TalkContentsDataList = new List<string>();
            for(int i = 2; i < ListTalkCount; i++) 
            {
                TalkContentsDataList.Add(ListTalkData[i]);
            }

            TalkDataList.Add(new TalkData(int.Parse(ListTalkData[0]), int.Parse(ListTalkData[1]), TalkContentsDataList.ToArray())); 
        }

        foreach(TalkData InTalkData in TalkDataList)
        {
            List<string> TextDataList = new List<string>();
            foreach(TalkContentsData TalkDataString in InTalkData.TextData)
            {
                TextDataList.Add(TalkDataString.TextData + ":" + TalkDataString.PortraitIndex);
            }

            talkData.Add(InTalkData.ObjectId + InTalkData.QuestIndex, TextDataList.ToArray());
        }

        portraitData.Add(1000 + 0, portraitArr[0]);
        portraitData.Add(1000 + 1, portraitArr[1]);
        portraitData.Add(1000 + 2, portraitArr[2]);
        portraitData.Add(1000 + 3, portraitArr[3]);
    }

    public string GetTalk(int id, int talkIndex)
    {
        if(talkData.ContainsKey(id) == false)
        {
            if (talkData.ContainsKey(id - id % 10) == false)
            {
                return GetTalk(id - id % 100, talkIndex); // Get First Talk
            }
            else
            {
                return GetTalk(id - id % 10, talkIndex); // Get First Quest Talk
            }
        }

        if(talkIndex == talkData[id].Length)
        {
            return null;
        }
        else
        {
            return talkData[id][talkIndex];
        }
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }
}
