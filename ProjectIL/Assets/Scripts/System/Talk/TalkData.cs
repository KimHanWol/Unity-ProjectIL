using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TalkData
{
    public int QuestIndex;
    public int ObjectId;
    public TalkContentsData[] TextData;

    public TalkData(int QuestIndex, int ObjectId, string[] TextDataStringList)
    {
        this.QuestIndex = QuestIndex;
        this.ObjectId = ObjectId;

        List<TalkContentsData> TalkContentsDataList = new List<TalkContentsData>();
        foreach(string TextDataString in TextDataStringList)
        {
            string TextData = "";
            int PortraitIndex = 0;
            int TalkObjectId = 0;

            string[] SplitTextDataList = TextDataString.Split(':');
            if (SplitTextDataList.Length > 0) TextData = SplitTextDataList[0];
            if (SplitTextDataList.Length > 1) PortraitIndex = int.Parse(SplitTextDataList[1]);
            if (SplitTextDataList.Length > 2) TalkObjectId = int.Parse(SplitTextDataList[2]);

            TalkContentsDataList.Add(new TalkContentsData(TalkObjectId, TextData, PortraitIndex));
        }

        this.TextData = TalkContentsDataList.ToArray();
    }
}

public class TalkContentsData
{
    public int TalkObjectId;
    public string TextData;
    public int PortraitIndex;

    public TalkContentsData(int TalkObjectId, string TextData, int PortraitIndex = 0)
    {
        this.TalkObjectId = TalkObjectId;
        this.TextData = TextData;
        this.PortraitIndex = PortraitIndex;
    }
}

