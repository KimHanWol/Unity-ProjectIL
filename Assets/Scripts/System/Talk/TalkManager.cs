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
        List<List<string>> CSVTalkData = CSVReader.GetSCVData("GameScript\\Day" + (SceneIndex + 1));
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
            string TalkEventKey = LineData[7];
            string AnimationKey = LineData[8];
            string EffectSoundKey = LineData[9].Split("\r")[0];

            TalkDialogDataList.Add(
                new DialogData(
                    LineData[3].Replace('/', ','),
                    TalkDelay,
                    PortraitIndex,
                    CutsceneKey,
                    TalkEventKey,
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
                if (NextTalkKey > 0)
                {
                    break;
                }

                float NextTalkDelay = 0;
                float.TryParse(NextLineData[4], out NextTalkDelay);

                int NextPortraitNum = -1;
                int.TryParse(NextLineData[5], out NextPortraitNum);

                string NextCutsceneKey = NextLineData[6];
                string NextTalkEventKey = NextLineData[7];
                string NextAnimationKey = NextLineData[8];
                string NextEffectSoundKey = NextLineData[9].Split("\r")[0];

                //Additional Dialong Data When Key Is Not Valid
                TalkDialogDataList.Add(
                    new DialogData(
                        NextLineData[3].Replace('/', ','),
                        NextTalkDelay,
                        NextPortraitNum,
                        NextCutsceneKey,
                        NextTalkEventKey,
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

            if (CurrentQuestIndex != QuestIndex)
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

        //portraitData.Add(1000 + 0, portraitArr[0]);
        //portraitData.Add(1000 + 1, portraitArr[1]);
        //portraitData.Add(1000 + 2, portraitArr[2]);
        //portraitData.Add(1000 + 3, portraitArr[3]);
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
        if (id == 0)
        {
            return null;
        }

        if(talkData == null)
        {
            return null;
        }

        if (talkData.ContainsKey(id) == false)
        {
            return null;
        }

        if (talkIndex >= talkData[id].DialogData.Count)
        {
            return null;
        }

        return talkData[id].DialogData[talkIndex].DialogText;
    }

    public enum AnimationTiming
    {
        Pre,
        Playing,
        Post
    }


    public string[] GetAnimationKeyList(int id, int talkIndex, AnimationTiming Timing)
    {
        if(talkData == null)
        {
            return new string[] { };
        }

        if (talkData.ContainsKey(id) != true)
        {
            return new string[] { };
        }

        if (talkData[id].DialogData.Count <= talkIndex)
        {
            return new string[] { };
        }

        if (talkData[id].DialogData[talkIndex].AnimationKey == "")
        {
            return new string[] { };
        }

        string[] AnimationKeyList = talkData[id].DialogData[talkIndex].AnimationKey.Split('/');
        string[] ResultAnimationKeyList = new string[AnimationKeyList.Length];
        int ResultAnimationKeyIndex = 0;
        foreach (string AnimationKey in AnimationKeyList)
        {
            string SplitAnimationKey = "";
            string[] SplitAnimationKeyArray = AnimationKey.Split("-");
            if (SplitAnimationKeyArray.Length <= 0)
            {
                return new string[] { };
            }

            if ((Timing == AnimationTiming.Pre && SplitAnimationKeyArray[0] == "Pre") ||
            (Timing == AnimationTiming.Post && SplitAnimationKeyArray[0] == "Post"))
            {
                SplitAnimationKey = SplitAnimationKeyArray[1];
            }
            else if (Timing == AnimationTiming.Playing && SplitAnimationKeyArray.Length == 1)
            {
                SplitAnimationKey = SplitAnimationKeyArray[0];
            }

            ResultAnimationKeyList[ResultAnimationKeyIndex] = SplitAnimationKey;
            ResultAnimationKeyIndex++;
        }

        return ResultAnimationKeyList;
    }

    public string GetSoundKey(int id, int talkIndex)
    {
        if (talkData == null)
        {
            return "";
        }

        if (talkData.ContainsKey(id) != true)
        {
            return "";
        }

        if (talkData[id].DialogData.Count <= talkIndex)
        {
            return "";
        }

        return talkData[id].DialogData[talkIndex].EffectSoundKey;
    }

    public float GetTalkDelay(int id, int talkIndex)
    {
        if (talkData == null)
        {
            return 0;
        }

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


    public string GetCutSceneKey(int id, int talkIndex)
    {
        if (talkData == null)
        {
            return "";
        }

        if (talkData.ContainsKey(id) != true)
        {
            return "";
        }

        if (talkData[id].DialogData.Count <= talkIndex)
        {
            return "";
        }

        return talkData[id].DialogData[talkIndex].CutSceneKey;
    }

    public string[] GetTalkEventKeyList(int id, int talkIndex, AnimationTiming Timing)
    {
        if (talkData == null)
        {
            return new string[] { };
        }

        if (talkData.ContainsKey(id) != true)
        {
            return new string[] { };
        }

        if (talkData[id].DialogData.Count <= talkIndex)
        {
            return new string[] { };
        }

        string[] TalkEventKeyList = talkData[id].DialogData[talkIndex].TalkEventKey.Split('/');
        string[] ResultTalkEventKeyList = new string[TalkEventKeyList.Length];
        int ResultTalkEventKeyIndex = 0;
        foreach(string TalkEventKey in TalkEventKeyList)
        {
            string SplitTalkEventKey = "";
            string[] SplitTalkEventKeyArray = TalkEventKey.Split("-");
            if (SplitTalkEventKeyArray.Length <= 0)
            {
                return new string[]{ };
            }

            if ((Timing == AnimationTiming.Pre && SplitTalkEventKeyArray[0] == "Pre") ||
                (Timing == AnimationTiming.Post && SplitTalkEventKeyArray[0] == "Post"))
            {
                SplitTalkEventKey = SplitTalkEventKeyArray[1];
            }
            else if (Timing == AnimationTiming.Playing && SplitTalkEventKeyArray.Length == 1)
            {
                SplitTalkEventKey = SplitTalkEventKeyArray[0];
            }

            ResultTalkEventKeyList[ResultTalkEventKeyIndex] = SplitTalkEventKey;
            ResultTalkEventKeyIndex++;
        }

        return ResultTalkEventKeyList;
    }
}
