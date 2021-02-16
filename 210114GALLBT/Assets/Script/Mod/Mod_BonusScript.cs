using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
public class Mod_BonusScript : IGameSystem
{
    [SerializeField] GameObject bonusCountPanel;
    protected int BonusIconCount;//計算bonus數

    int[] BonusType = new int[3];
    int[] BonusTypeWeight = new int[3];
    public static int[][] bonusWeightArray = new int[2][];

    JArray sepcialTimes;
    JArray sepcialTimesWeight;
    public void Start()
    {
        //setBonusMultiWeight(0);
        //setBonusMultiWeight(1);
        ////Debug.Log("Set");
    }
    private void Update()
    {
       // m_SlotMediatorController.SendMessage(this, "Test");
    }
    public void CheckBonus()
    {
        if (Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5)
        {
            CheckBonusType(WinBonusType.ConsecutiveBonus);
        }
        else if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3|| Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4 || Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5)
        {
            CheckBonusType(WinBonusType.NotConsecutiveBonus);
            //CheckBonusType(WinBonusType.NotConsecutiveBonus);
        }
    }
    enum WinBonusType
    {
        ConsecutiveBonus,/*連續Bonus*/
        NotConsecutiveBonus,/*不連續Bonus*/
        OnlineBonus  /*需在贏分線上的Bonus*/
    }
    void CheckBonusType(WinBonusType winBonusType)
    {
        switch (winBonusType)
        {

            case WinBonusType.ConsecutiveBonus:
                BonusType_ConsecutiveBonus();
                break;

            case WinBonusType.NotConsecutiveBonus:
                BonuType_NotConsecutiveBonus();
                break;
            case WinBonusType.OnlineBonus:
                OnlineBonus();
                break;
            default:
                break;
        }
    }

    void BonusType_ConsecutiveBonus()//連續且Bonus出現在1~5
    {
        BonusIconCount = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < Mod_Data.slotRowNum; k++)
            {
                if (Mod_Data.ReelMath[i, k] == 1)
                {
                    BonusIconCount++;
                }
            }
        }

        if (BonusIconCount >= 3)
        {
            if (!Mod_Data.BonusSwitch)
            {
                Mod_Data.getBonus = true;
            }
            else
            {
                //Mod_Data.getBonus = true;
            }
           if(!Mod_Data.StartNotNormal) Mod_Data.BonusTicket++;
            bonusCountPanel.transform.GetChild(2).gameObject.GetComponent<Text>().text = Mod_Data.BonusTicket.ToString();
        }
    }
    void BonuType_NotConsecutiveBonus()  //目前是fish規則
    {
        BonusIconCount = 0;
        for (int i = 0; i < Mod_Data.slotReelNum; i++)
        {
            for (int k = 0; k < Mod_Data.slotRowNum; k++)
            {
                if (Mod_Data.ReelMath[i, k] == 1)
                {
                    BonusIconCount++;
                }
            }
        }

        if (BonusIconCount >= 3) {
            Mod_Data.getBonus = true;
            //Debug.Log("Mod_Data.getBonus=" + Mod_Data.getBonus);

            //if (BonusIconCount==3)
            //{
            //    Mod_Data.getBonusCount = 10;
            //}
            //else if (BonusIconCount == 4)
            //{
            //    Mod_Data.getBonusCount = 15;
            //}
            //else if (BonusIconCount == 5)
            //{
            //    Mod_Data.getBonusCount = 25;
            //}

            if (!Mod_Data.StartNotNormal && Mod_Data.BonusCount != 0) Mod_Data.BonusCount += Mod_Data.getBonusCount;
            ////Debug.Log("22222Mod_Data.BonusCount" + Mod_Data.BonusCount);
            //Mod_Data.BonusTicket++;
            if (Mod_Data.BonusCount > Mod_Data.BonusLimit) Mod_Data.BonusCount = Mod_Data.BonusLimit;
            // bonusCountPanel.transform.GetChild(2).gameObject.GetComponent<Text>().text = Mod_Data.BonusTicket.ToString();
        }
        else {
            Mod_Data.getBonus = false;
        }
    }
    void OnlineBonus()//目前是楚河遊戲規則
    {
        for(int i = 0; i < Mod_Data.linegame_LineCount; i++)
        {
            if (Mod_Data.linegame_WinIconID[i] == 1)
            {
                Mod_Data.getBonus = true;
                if (!Mod_Data.StartNotNormal) Mod_Data.BonusCount += 5;
                if (Mod_Data.BonusCount > Mod_Data.BonusLimit) Mod_Data.BonusCount = Mod_Data.BonusLimit;
                break;
            }
        }
        if (Mod_Data.BonusCount == 5) 
        {
            for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
            {
                if (Mod_Data.linegame_WinIconID[i] == 1)
                {
                  
                }
            }
        }
    }

    //public void CheckBonusTypes(int bonusType)
    //{
    //    int BonusAddCount = 0;
    //    switch (bonusType)
    //    {
    //        case 0:
    //            BonusAddCount = 7;
    //            Mod_Data.BonusTicket--;
    //            //BonusIconPay(BonusIconQuantity);
    //            break;

    //        case 1:
    //            BonusAddCount = 25;
    //            Mod_Data.BonusTicket--;
    //            // BonusIconPay(BonusIconQuantity);
    //            break;


    //    }
    //    Mod_Data.BonusType = bonusType;

    //    bonusCountPanel.transform.GetChild(1).gameObject.GetComponent<Text>().text = BonusAddCount.ToString();
    //    Mod_Data.BonusCount = BonusAddCount;

    //    bonusCountPanel.transform.GetChild(2).gameObject.GetComponent<Text>().text = Mod_Data.BonusTicket.ToString();

    //    m_SlotMediatorController.SendMessage(this, "BonustransEnd");
    //}//海神專用
    //public void setBonusMultiWeight(int bonusType)
    //{

    //    if (bonusType == 0)
    //    {
    //        sepcialTimes = (JArray)Mod_Data.JsonObjBonus2["Icons"]["Icon3"]["MultiplierWin"];
    //        sepcialTimesWeight = (JArray)Mod_Data.JsonObjBonus2["Icons"]["Icon3"]["MultiplierWeight"];
    //    }
    //    else if (bonusType == 1)
    //    {

    //        sepcialTimes = (JArray)Mod_Data.JsonObjBonus1["Icons"]["Icon3"]["MultiplierWin"];
    //        sepcialTimesWeight = (JArray)Mod_Data.JsonObjBonus1["Icons"]["Icon3"]["MultiplierWeight"];
    //    }
    //    int k = 0, multiSun = 0;



    //    for (int i = 0; i < 3; i++)
    //    {
    //        BonusType[i] = int.Parse(sepcialTimes[i].ToString());
    //        BonusTypeWeight[i] = int.Parse(sepcialTimesWeight[i].ToString());
    //        multiSun += BonusTypeWeight[i];
    //    }
    //    bonusWeightArray[bonusType] = new int[multiSun];
    //    //random用
    //    for (int i = 0; i < 3; i++)
    //    {


    //        for (int j = 0; j < BonusTypeWeight[i]; j++)
    //        {
    //            bonusWeightArray[bonusType][k] = BonusType[i];

    //            k++;

    //        }
    //    }
    //}//海神專用

}


