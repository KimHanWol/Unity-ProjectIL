using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CSVReader
{
    static public List<List<string>> GetSCVData(string path)
    {
        List<List<string>> ResultList = new List<List<string>>();

        TextAsset textFile = (TextAsset)Resources.Load(path);
        string testText = textFile.text;
        string[] lineTextList = testText.Split("\n");
        for(int i = 1; i < lineTextList.Length; i++)
        {
            ResultList.Add(lineTextList[i].Split(",").ToList());
        }

        return ResultList;
    }
}
