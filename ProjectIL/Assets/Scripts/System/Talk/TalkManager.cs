using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Rendering;

public class TalkManager : MonoBehaviour
{
    const int CSVDataSplitIndex = 10;

    Dictionary<int, TalkData> talkData;
    Dictionary<int, Sprite> portraitData;

    public Sprite[] portraitArr;

    void Awake()
    {
        talkData = new Dictionary<int, TalkData>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    void GenerateData()
    {
        List<List<string>> CSVTalkData = CSVReader.GetSCVData("TalkDataCSV");
        for(int i = 0; i < CSVTalkData.Count; i++)
        {
            List<string> LineData = CSVTalkData[i];
            List<Tuple<string, int>> TalkDialogDataList = new List<Tuple<string, int>>();


            if (LineData.Count < CSVDataSplitIndex)
            {
                continue;
            }

            int TalkKey = -1;
            int.TryParse(LineData[2], out TalkKey);
            if(TalkKey < 0)
            {
                continue;
            }

            int PortraitIndex = 0;
            int.TryParse(LineData[4], out PortraitIndex);

            Tuple<string, int> DialogData = Tuple.Create<string, int>(LineData[3], PortraitIndex);
            TalkDialogDataList.Add(DialogData);

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

                int PortraitNum = 0;
                int.TryParse(NextLineData[4], out PortraitNum);

                //Additional Dialong Data When Key Is Not Valid
                Tuple<string, int> NextDialogData = Tuple.Create<string, int>(NextLineData[3], PortraitNum);
                TalkDialogDataList.Add(NextDialogData);
                i++;
            }

            int QuestIndex = -1;
            int ObjectIndex = -1;
            float TalkDelay = 0;
            int SelectionKey = 0;
            int AcquiredItemKey = 0;
            int DisplayEnableItemKey = 0;
            int Endingkey = 0;
            int EffectSoundKey = 0;

            bool bIsValidData =
            int.TryParse(LineData[0], out QuestIndex) &&
            int.TryParse(LineData[1], out ObjectIndex);

            if (bIsValidData == false)
            {
                continue;
            }

            float.TryParse(LineData[4], out TalkDelay);
            int.TryParse(LineData[5], out SelectionKey);
            int.TryParse(LineData[6], out AcquiredItemKey);
            int.TryParse(LineData[7], out DisplayEnableItemKey);
            int.TryParse(LineData[8], out Endingkey);
            int.TryParse(LineData[8], out EffectSoundKey);

            talkData.Add(int.Parse(LineData[2]), new TalkData(
                QuestIndex,
                ObjectIndex,
                TalkDialogDataList,
                TalkDelay,
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

    public string GetTalk(int id, int talkIndex)
    {
        if(id == 0)
        {
            return null;
        }

        if(talkData.ContainsKey(id) == false)
        {
            return null;

            if (talkData.ContainsKey(id - id % 10) == false)
            {
                return GetTalk(id - id % 100, talkIndex); // Get First Talk
            }
            else
            {
                return GetTalk(id - id % 10, talkIndex); // Get First Quest Talk
            }
        }

        if(talkIndex == talkData[id].DialogData.Count)
        {
            return null;
        }
        else
        {
            return talkData[id].DialogData[talkIndex].Item1;
        }
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }
}
