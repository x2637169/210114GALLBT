using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxWinData
{
    public double MaxOriginalWinScore;
    public double MaxWinScore;
    public string GameData;

    public MaxWinData()
    {
        MaxOriginalWinScore = 0;
        MaxWinScore = 0;
        GameData = "";
    }

    public MaxWinData(double _MaxOriginalWinScore,double _MaxWinScore,string _GameData)
    {
        MaxOriginalWinScore = _MaxOriginalWinScore;
        MaxWinScore = _MaxWinScore;
        GameData = _GameData;
    }

}
