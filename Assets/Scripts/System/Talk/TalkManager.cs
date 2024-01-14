using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

public class TalkManager : MonoBehaviour
{
    const int CSVDataSplitIndex = 10;

    Dictionary<int, TalkData> talkData;
    Dictionary<int, Sprite> portraitData;

    public Sprite[] portraitArr;

    public void InitializeTalkManager(int SceneIndex)
    {
        GenerateData(SceneIndex);
    }

    void Awake()
    {
        talkData = new Dictionary<int, TalkData>();
        portraitData = new Dictionary<int, Sprite>();
    }

    void GenerateData(int SceneIndex)
    {
        List<List<string>> CSVTalkData = CSVReader.GetSCVData("Day" + (SceneIndex + 1));
        int CurrentQuestIndex = 0;
        int QuestActionIndex = 0;
        for (int i = 0; i < CSVTalkData.Count; i++)
        {
            List<string> LineData = CSVTalkData[i];
            List<DialogData> TalkDialogDataList = new List<DialogData>();

            if (LineData.Count < CSVDataSplitIndex)
            {
                continue;
            }

            float TalkDelay = 0;
            float.TryParse(LineData[4], out TalkDelay);

            int PortraitIndex = -1;
            int.TryParse(LineData[5], out PortraitIndex);

            int CutsceneKey = -1;
            int.TryParse(LineData[9], out CutsceneKey);

            TalkDialogDataList.Add(new DialogData(LineData[3].Replace('/', ','), PortraitIndex, CutsceneKey, TalkDelay));

            //Check Dialog Data Loop
            while (i + 1 < CSVTalkData.Count) 
            {
                List<string> NextLineData = CSVTalkData[i + 1];
                if (NextLineData.Count < CSVDataSplitIndex)
                {
                    break;
                }

                int NextTalkKey = -1;
                int.TryParse(NextLineData[2], out NextTalkKey);

                //Dialog Data Not Continued
                if(NextTalkKey > 0)
                {
                    break;
                }

                float NextTalkDelay = 0;
                float.TryParse(NextLineData[4], out NextTalkDelay);

                int PortraitNum = -1;
                int.TryParse(NextLineData[5], out PortraitNum);

                int NextCutsceneKey = -1;
                int.TryParse(NextLineData[9], out NextCutsceneKey);

                //Additional Dialong Data When Key Is Not Valid
                TalkDialogDataList.Add(new DialogData(NextLineData[3].Replace('/', ','), PortraitNum, NextCutsceneKey, NextTalkDelay));
                i++;
            }

            int QuestIndex = -1;
            int ObjectIndex = -1;
            int SelectionKey = 0;
            int AcquiredItemKey = 0;
            int DisplayEnableItemKey = 0;
            int Endingkey = 0;
            int EffectSoundKey = 0;

            bool bIsValidData =
            int.TryParse(LineData[0], out QuestIndex) &&
            int.TryParse(LineData[1], out ObjectIndex);

            if(CurrentQuestIndex != QuestIndex)
            {
                CurrentQuestIndex = QuestIndex;
                QuestActionIndex = 0;
            }
            else
            {
                QuestActionIndex++;
            }

            if (bIsValidData == false)
            {
                continue;
            }

            int.TryParse(LineData[6], out SelectionKey);
            int.TryParse(LineData[7], out AcquiredItemKey);
            int.TryParse(LineData[8], out DisplayEnableItemKey);
            int.TryParse(LineData[10], out Endingkey);
            int.TryParse(LineData[11], out EffectSoundKey);

            int TalkKey = QuestIndex + ObjectIndex;

            talkData.Add(TalkKey, new TalkData(
                QuestIndex,
                QuestActionIndex,
                ObjectIndex,
                TalkDialogDataList,
                SelectionKey,
                AcquiredItemKey,
                DisplayEnableItemKey,
                Endingkey,
                EffectSoundKey
                ));
        }

        portraitData.Add(1000 + 0, portraitArr[0]);
        portraitData.Add(1000 + 1, portraitArr[1]);
        portraitData.Add(1000 + 2, portraitArr[2]);
        portraitData.Add(1000 + 3, portraitArr[3]);
    }

    public bool IsTalkDataForCurrentQuest(int talkDataId, int questId)
    {
        if (talkData.ContainsKey(talkDataId) == false)
        {
            return false;
        }

        return talkData[talkDataId].QuestIndex == questId;
    }

    public string GetTalkText(int id, int talkIndex, int questTalkIndex)
    {
        if(id == 0)
        {
            return null;
        }

        if(talkData.ContainsKey(id) == false)
        {
            return null;

            //if (talkData.ContainsKey(id - id % 10) == false)
            //{
            //    return GetTalk(id - id % 100, talkIndex); // Get First Talk
            //}
            //else
            //{
            //    return GetTalk(id - id % 10, talkIndex); // Get First Quest Talk
            //}
        }

        if (talkIndex >= talkData[id].DialogData.Count)
        {
            return null;
        }

        if (talkData[id].QuestActionIndex + talkData[id].QuestIndex == questTalkIndex)
        {
            return talkData[id].DialogData[talkIndex].DialogText;
        }

        return null;
    }

    public float GetTalkDelay(int id, int talkIndex)
    {
        if(talkData.ContainsKey(id) == true)
        {
            return talkData[id].DialogData[talkIndex].TalkDelay;
        }

        return 0;
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }
}
