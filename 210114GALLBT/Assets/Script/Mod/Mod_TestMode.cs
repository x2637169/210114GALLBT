using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using UnityEngine.UI;
using System;
using Random = System.Random;


public class Mod_TestMode : MonoBehaviour
{
    Mod_GameMath m_GameMath;
    Mod_BonusScript m_BonusScript;
    double winTotal, betTotal,baseTotal,bonusTotal;
    int BonusTimes, BonusInBonusTimes;


    //FishGaGa用
    int[] BonusTime = new int[11];
    int[] BonusTimeWeight = new int[11];
    int[] BonusMulti = new int[6];
    int[] BonusMultiWeight = new int[6];
    int[] TimeArray = new int[100];   //random用
    int[] MultiArray = new int[100];   //random用
    int[,] BonusMultiContent = new int[,] { { 2, 2, 3, 4, 4 }, { 2, 2, 3, 3, 4 }, { 2, 2, 2, 3, 4 }, { 2, 2, 3, 4, 4 }, { 2, 3, 3, 4, 4 }, { 3, 3, 3, 4, 4 } };

    void Start()
    {
        Mod_Data.JsonReload();
        m_GameMath = GetComponent<Mod_GameMath>();
        m_BonusScript = GetComponent<Mod_BonusScript>();
        Mod_Data.StartNotNormal = false;

    }


    void Update()
    {

    }
    public void Test()
    {
        for (int i = 0; i < 10000000; i++)
        {
           
            ReelSetRng();
            Mod_Data.BonusCount = 0;
            m_BonusScript.CheckBonus();
            m_GameMath.WayGame_MathClear();
            m_GameMath.WayGame_CountMath();        
            m_GameMath.WinScoreCount(Mod_Data.GameMode.BaseGame);    
            if (Mod_Data.getBonus)
            {
                Mod_Data.getBonus = false;


                //抓取倍率與場次的json數學 FishGaGa用
                int k = 0;

                for (int j = 0; j < 11; j++) {
                    JArray Session = (JArray)Mod_Data.JsonObj["Icons"]["Icon2"]["PickBonusType2"];
                    JArray SessionWeight = (JArray)Mod_Data.JsonObj["Icons"]["Icon2"]["PickBonusType2Weight"];
                    BonusTime[j] = int.Parse(Session[j].ToString());
                    BonusTimeWeight[j] = int.Parse(SessionWeight[j].ToString());

                    for (int a = 0; a < BonusTimeWeight[j]; a++) {
                        TimeArray[k] = BonusTime[j];
                        k++;
                    }
                }

                k = 0;
                for (int j = 0; j < 6; j++) {
                    JArray Multi = (JArray)Mod_Data.JsonObj["Icons"]["Icon2"]["PickMultiplier"];
                    JArray MultiWeight = (JArray)Mod_Data.JsonObj["Icons"]["Icon2"]["PickMultiplierWeight"];
                    BonusMulti[j] = int.Parse(Multi[j].ToString());
                    BonusMultiWeight[j] = int.Parse(MultiWeight[j].ToString());

                    for (int a = 0; a < BonusMultiWeight[j]; a++) {
                        MultiArray[k] = BonusMulti[j];
                        k++;
                    }
                }
                Mod_Data.BonusIsPlayedCount = 1;
                Mod_Data.BonusCount = TimeArray[UnityEngine.Random.Range(0, 100)];
                Mod_Data.BonusSpecialTimes = BonusMultiContent[MultiArray[UnityEngine.Random.Range(0, 100)], UnityEngine.Random.Range(0, 5)];
                Mod_Data.getBonusCount = Mod_Data.BonusCount;
                Mod_Data.BonusSwitch = true;
            }
            else {
                baseTotal += Mod_Data.Win;
            }
            if (Mod_Data.BonusSwitch)
            {
                BonusTimes++;
                Mod_Data.JsonReload();
                while (Mod_Data.BonusIsPlayedCount < Mod_Data.BonusCount)
                {
                    ReelSetRng();
                    Mod_Data.BonusIsPlayedCount++;
                    m_BonusScript.CheckBonus();
                    m_GameMath.WayGame_MathClear();
                    m_GameMath.WayGame_CountMath();               
                    m_GameMath.WinScoreCount(Mod_Data.GameMode.BonusGame);                 
                    if (Mod_Data.getBonus)
                    {
                        Mod_Data.getBonus = false;
                        BonusInBonusTimes++;
                    }
                }
                //Debug.Log(Mod_Data.BonusCount);
              
                Mod_Data.BonusSwitch = false;
                
                Mod_Data.JsonReload();
                bonusTotal += Mod_Data.Win;
            }
            winTotal += Mod_Data.Win;
            betTotal += Mod_Data.Bet;
            ////Debug.Log(winTotal+"+"+ betTotal);
        }
        //Debug.Log(winTotal / betTotal + " " + baseTotal / betTotal + " " + bonusTotal / betTotal + " " + Mod_Data.bonusTimes+ " winTotal:" + winTotal + " betTotal:" + betTotal+ " BonusTimes:" + BonusTimes+ " BonusInBonusTimes:" + BonusInBonusTimes+ " baseTotal:" + baseTotal+ " bonusTotal:"+ bonusTotal);
    }

    JArray ReelMath;
    public void ReelMathReload(int i)
    {
        ////Debug.Log(gameObject.name);
        
        switch (i)
        {
            case 0:
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_1"]["Reel"];
                break;
            case 1:
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_2"]["Reel"];
                break;
            case 2:
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_3"]["Reel"];
                break;
            case 3:
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_4"]["Reel"];
                break;
            case 4:
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_5"]["Reel"];
                break;
        }
    }

    int ReelRng;
    string stringRNG;
    public void ReelSetRng()
    {
       
        //stringRNG = "";
        for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
        {
            ReelMathReload(reel);
            int ReelRngConut;
            Random Rng = new Random(Guid.NewGuid().GetHashCode());
            ReelRng = Rng.Next(0, ReelMath.Count);
            ReelRngConut = ReelRng;
            //stringRNG += ReelRngConut + " ";
            for (int row = 0; row < Mod_Data.slotRowNum; row++)
            {
                if (ReelRngConut < 0) { ReelRngConut = ReelMath.Count - 1; }
                Mod_Data.ReelMath[reel, row] = int.Parse(ReelMath[ReelRngConut].ToString());
                ReelRngConut -= 1;
            }
        }

        ////Debug.Log(stringRNG);
    }

    void GameMathCount()
    {
        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        {
            m_GameMath.WayGame_MathClear();
        }
        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        {
            m_GameMath.LineGame_MathClear();
        }

        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        {
            m_GameMath.WayGame_CountMath();
            if (!Mod_Data.BonusSwitch) m_GameMath.WinScoreCount(Mod_Data.GameMode.BaseGame);
            else m_GameMath.WinScoreCount(Mod_Data.GameMode.BonusGame);

        }
        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        {
            m_GameMath.LineGame_CountMath();
            if (!Mod_Data.BonusSwitch) m_GameMath.WinScoreCount(Mod_Data.GameMode.BaseGame);
            else m_GameMath.WinScoreCount(Mod_Data.GameMode.BonusGame);
        }

    }
}
