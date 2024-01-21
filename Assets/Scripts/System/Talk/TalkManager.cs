using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

public class TalkManager : MonoBehaviour
{
    const int CSVDataSplitIndex = 8;

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

            string CutsceneKey = LineData[6];
            string AnimationKey = LineData[7];
            string EffectSoundKey = LineData[8];

            TalkDialogDataList.Add(
                new DialogData(
                    LineData[3].Replace('/', ','),
                    TalkDelay,
                    PortraitIndex,
                    CutsceneKey,
                    AnimationKey,
                    EffectSoundKey
                    )
                );

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

                int NextPortraitNum = -1;
                int.TryParse(NextLineData[5], out NextPortraitNum);

                string NextCutsceneKey = NextLineData[6];
                string NextAnimationKey = NextLineData[7];
                string NextEffectSoundKey = NextLineData[8];

                //Additional Dialong Data When Key Is Not Valid
                TalkDialogDataList.Add(
                    new DialogData(
                        NextLineData[3].Replace('/', ','),
                        NextTalkDelay,
                        NextPortraitNum, 
                        NextCutsceneKey, 
                        NextAnimationKey,
                        NextEffectSoundKey
                        )
                    );
                i++;
            }

            int QuestIndex = -1;
            int ObjectIndex = -1;

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

            int TalkKey = QuestIndex + ObjectIndex;

            talkData.Add(TalkKey, new TalkData(
                QuestIndex,
                QuestActionIndex,
                ObjectIndex,
                TalkDialogDataList,
                CutsceneKey,
                AnimationKey,
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

        //if (talkData[id].QuestActionIndex + talkData[id].QuestIndex == questTalkIndex)
        {
            return talkData[id].DialogData[talkIndex].DialogText;
        }

        //return null;
    }

    public enum TalkAnimationTiming
    { 
        Pre,
        Playing,
        Post
    }


    public string GetAnimationKey(int id, int talkIndex, TalkAnimationTiming Timing)
    {
        if (talkData.ContainsKey(id) != true)
        {
            return "";
        }

        if (talkData[id].DialogData.Count <= talkIndex)
        {
            return "";
        }

        if(talkData[id].DialogData[talkIndex].AnimationKey == "")
        {
            return "";
        }

        string[] SplitAnimationKeyArray = talkData[id].DialogData[talkIndex].AnimationKey.Split("-");
        if (SplitAnimationKeyArray.Length <= 0)
        {
            return "";
        }

        if ((Timing == TalkAnimationTiming.Pre && SplitAnimationKeyArray[0] == "Pre") ||
            (Timing == TalkAnimationTiming.Post && SplitAnimationKeyArray[0] == "Post") ||
            Timing == TalkAnimationTiming.Playing && SplitAnimationKeyArray.Length == 1)
        {
            return SplitAnimationKeyArray[1];
        }

        return "";
    }

    public float GetTalkDelay(int id, int talkIndex)
    {
        if (talkData.ContainsKey(id) != true)
        {
            return 0;
        }

        if (talkData[id].DialogData.Count <= talkIndex)
        {
            return 0;
        }

        return talkData[id].DialogData[talkIndex].TalkDelay;
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }
}
