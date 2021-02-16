using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Mod_Check : MonoBehaviour
{
    // Start is called before the first frame update
    int Bonustimes = 0;
    public double Check_Pay = 0;
    public double Check_Win = 0;
    double Win = 0;
    public double Check_Limit;
    public int CH_BonusSpecialTime = 0;
    int BonusSpecialTimes = 1;
    void Start()
    {
        Check_Limit = 2000000;
    }

    // Update is called once per frame

    void Update()
    {
        
    }
    public void Check()
    {
        //LineGame_MathClear();
        //LineGame_CountMath();
        WayGame_MathClear();
        WayGame_CountMath();
        if (!Mod_Data.BonusSwitch)
        {
            CheckWinScoreCount(Mod_Data.GameMode.BaseGame);
            //Debug.Log(Check_Pay);
        }
        else
        {
            CheckWinScoreCount(Mod_Data.GameMode.BonusGame);
            //Check_Pay = 0;
            //Bonustimes = 0;
            //BonusSpecialTimes = Mod_Data.BonusSpecialTimes;
            //WinScoreCount(Mod_Data.GameMode.BonusGame);
            ////Debug.Log(Check_Pay);            
        }
    }
  
    public void CheckWinScoreCount(Mod_Data.GameMode mode)
    {
        Bonustimes = 0;
        if (mode == Mod_Data.GameMode.BaseGame)
        {
          
            Debug.Log(Bonustimes);
            for (int i = 0; i < Mod_Data.allgame_WinLines; i++)
            {
                Check_Pay += Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[i]][Mod_Data.waygame_WinIconQuantity[i]] * Mod_Data.waygmae_EachIconWinLineNum[i] * Mod_Data.odds;
            }
           
            Check_Win += Mathf.CeilToInt((float)Check_Pay ) * Mod_Data.Denom;
            Debug.Log("Check_Win:" + Check_Win);
        }
        else if (mode == Mod_Data.GameMode.BonusGame)
        {
           GameSpecialBonusRule();
        }
    }
    /*
    public void LineGame_CountMath()
    {
        int tmpcount = 0, recordIndex = 0, tmpIconID = 0;
        int tmp;
        for (int lineIndex = 0; lineIndex < Mod_Data.linegame_LineCount; lineIndex++)
        {
            tmp = Mod_Data.ReelMath[0, Mod_Data.linegame_Rule[lineIndex][0]];

            if (tmp != 1)
            {
                if (tmp == Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]] || Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]] == 0)
                {
                    if (Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]] == 0) Mod_Data.linegame_SpecialWild[lineIndex]++;

                    if (tmp == Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] || Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] == 0)
                    {
                        Mod_Data.linegame_WinIconQuantity[lineIndex] = 2;
                        Mod_Data.linegame_HasWinline[lineIndex] = true;
                        Mod_Data.linegame_WinIconID[lineIndex] = tmp;
                        if (Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] == 0) Mod_Data.linegame_SpecialWild[lineIndex]++;

                        recordIndex++;
                        //Debug.Log(lineIndex);
                        Mod_Data.allgame_AllCombineIconBool[0, Mod_Data.linegame_Rule[lineIndex][0]] = true;
                        Mod_Data.allgame_AllCombineIconBool[1, Mod_Data.linegame_Rule[lineIndex][1]] = true;
                        Mod_Data.allgame_AllCombineIconBool[2, Mod_Data.linegame_Rule[lineIndex][2]] = true;
                        if (tmp == Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] || Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == 0)
                        {
                            Mod_Data.linegame_WinIconQuantity[lineIndex] = 3;
                            Mod_Data.allgame_AllCombineIconBool[3, Mod_Data.linegame_Rule[lineIndex][3]] = true;
                            if (Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == 0) Mod_Data.linegame_SpecialWild[lineIndex]++;

                            if (tmp == Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] || Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 0)
                            {
                                Mod_Data.linegame_WinIconQuantity[lineIndex] = 4;
                                Mod_Data.allgame_AllCombineIconBool[4, Mod_Data.linegame_Rule[lineIndex][4]] = true;
                                if (Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 0) Mod_Data.linegame_SpecialWild[lineIndex]++;
                            }
                        }

                    }
                }

            }
            //Debug.Log(lineIndex + "," + tmp);

            #region 須在線上的Bonus判斷 需要時打開註解
            /*if (Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5)
            {   //在線上且1~5連續
                if (tmp == Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]])
                {
                    if (tmp == Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]])
                    {
                        Mod_Data.linegame_WinIconQuantity[lineIndex] = 2;
                        Mod_Data.linegame_HasWinline[lineIndex] = true;
                        Mod_Data.linegame_WinIconID[lineIndex] = tmp;
                        recordIndex++;
                        //Debug.Log(lineIndex);
                        Mod_Data.allgame_AllCombineIconBool[0, Mod_Data.linegame_Rule[lineIndex][0]] = true;
                        Mod_Data.allgame_AllCombineIconBool[1, Mod_Data.linegame_Rule[lineIndex][1]] = true;
                        Mod_Data.allgame_AllCombineIconBool[2, Mod_Data.linegame_Rule[lineIndex][2]] = true;
                        if (tmp == Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] || Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == 0)
                        {
                            Mod_Data.linegame_WinIconQuantity[lineIndex] = 3;
                            Mod_Data.allgame_AllCombineIconBool[3, Mod_Data.linegame_Rule[lineIndex][3]] = true;
                            if (tmp == Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] || Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 0)
                            {
                                Mod_Data.linegame_WinIconQuantity[lineIndex] = 4;
                                Mod_Data.allgame_AllCombineIconBool[4, Mod_Data.linegame_Rule[lineIndex][4]] = true;
                            }
                        }

                    }
                }
            }
            else if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3 || Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4 || Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5 || Mod_Data.bonusRule == Mod_Data.BonusRule.CH)
            {   //在線上且不連續3個以上
                int BonusNum = 0;
                for (int i = 0; i < Mod_Data.slotReelNum; i++)
                {
                    if (Mod_Data.ReelMath[i, Mod_Data.linegame_Rule[lineIndex][i]] == 1)
                    {
                        BonusNum++;
                    }
                }

                if (BonusNum > 2)
                {
                    Mod_Data.linegame_WinIconQuantity[lineIndex] = BonusNum - 1;
                    Mod_Data.linegame_HasWinline[lineIndex] = true;
                    Mod_Data.linegame_WinIconID[lineIndex] = 1;
                    recordIndex++;
                    //Debug.Log("BonuslineIndex: " + lineIndex);
                    for (int i = 0; i < Mod_Data.slotReelNum; i++)
                    {
                        if (Mod_Data.ReelMath[i, Mod_Data.linegame_Rule[lineIndex][i]] == 1)
                        {
                            Mod_Data.allgame_AllCombineIconBool[i, Mod_Data.linegame_Rule[lineIndex][i]] = true;
                            Debug.Log(i + "," + Mod_Data.linegame_Rule[lineIndex][i]);
                        }
                    }
                }

            }
            #endregion
        }

        #region 隨意不須在線上的Bonus判斷 需要時打開註解
        if (Mod_Data.getBonus)//checkBonus後確定有Bonus  ****隨意不須在線上的Bonus判斷****
        {
            if (Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5)
            {
                for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
                {
                    for (int row = 0; row < Mod_Data.slotRowNum; row++)
                    {
                        if (Mod_Data.ReelMath[reel, row] == 1)
                        {
                            Bonustimes++;
                            Mod_Data.linegame_BonusPos[reel, row] = true;
                            Mod_Data.allgame_AllCombineIconBool[reel, row] = true;
                        }
                        else
                        {
                            goto hasnotBonus;
                        }
                    }
                }
                hasnotBonus:;
            }
            else if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3 || Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4 || Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5)
            {
                for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
                {
                    for (int row = 0; row < Mod_Data.slotRowNum; row++)
                    {
                        if (Mod_Data.ReelMath[reel, row] == 1)
                        {
                            Bonustimes++;
                            Mod_Data.linegame_BonusPos[reel, row] = true;
                            Mod_Data.allgame_AllCombineIconBool[reel, row] = true;
                            //Debug.Log("reel: " + reel + " row: " + row);
                        }
                    }
                }
            }
        }
        #endregion
        Mod_Data.allgame_WinLines = recordIndex;
    }

    public void LineGame_MathClear()
    {
        Check_Pay = 0;
        for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
        {
            Mod_Data.linegame_WinIconQuantity[i] = 0;
            Mod_Data.linegame_HasWinline[i] = false;
            Mod_Data.linegame_WinIconID[i] = 0;
            Mod_Data.linegame_SpecialWild[i] = 0;
        }

        for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
        {
            for (int row = 0; row < Mod_Data.slotRowNum; row++)
            {
                Mod_Data.linegame_BonusPos[reel, row] = false;
                Mod_Data.allgame_AllCombineIconBool[reel, row] = false;
            }

        }
        Mod_Data.allgame_WinLines = 0;
        Mod_Data.Pay = 0;
        if (!Mod_Data.BonusSwitch) Mod_Data.Win = 0;
        if (!Mod_Data.BonusSwitch) Check_Win = 0;
        Bonustimes = 0;
    }
*/
    public void WayGame_CountMath()
    {
        int tmpcount = 0, times = 0, BonusCountTime = 0, ScatterTimes = 0, combineTimes = 1;//有連線Icon數   每排個數   總線數(每排互乘)
        int tmpIconID;
        int[] tmpArray = new int[Mod_Data.slotRowNum + 2];//紀錄連線的IconID

        bool sameicon = false;//連線icon中是否有重複


        //驗證是否有連線
        for (int rowInReeL1 = 0; rowInReeL1 < Mod_Data.slotRowNum; rowInReeL1++)
        {
            tmpIconID = Mod_Data.ReelMath[0, rowInReeL1];//依序比對Reel1的3個icon
            if (tmpIconID != 1 && tmpIconID != 2)//是否為Bonus(個別判斷)
            {
                for (int rowInReeL2 = 0; rowInReeL2 < Mod_Data.slotRowNum; rowInReeL2++)
                {
                    if (tmpIconID == Mod_Data.ReelMath[1, rowInReeL2] || Mod_Data.ReelMath[1, rowInReeL2] == 0)//依序比對 目前指定的Reel1中的1個icon和Reel2的三個icon是否相同
                    {
                        for (int rowInReeL3 = 0; rowInReeL3 < Mod_Data.slotRowNum; rowInReeL3++)
                        {
                            if (tmpIconID == Mod_Data.ReelMath[2, rowInReeL3] || Mod_Data.ReelMath[2, rowInReeL3] == 0)//依序比對 目前指定的Reel1中的1個icon和Reel3的三個icon是否相同
                            {
                                if (tmpcount != 0)  //若有兩個以上確認是否相同符號,相同則不紀錄
                                {
                                    sameicon = false;
                                    for (int i = 0; i < tmpcount; i++)
                                    {
                                        if (tmpArray[i] == tmpIconID)
                                        {
                                            sameicon = true;
                                            break;
                                        }
                                    }
                                    if (!sameicon)
                                    {

                                        tmpArray[tmpcount] = tmpIconID;//紀錄連線的IconID
                                        tmpcount++;
                                    }
                                }
                                else
                                {
                                    tmpArray[tmpcount] = tmpIconID;
                                    tmpcount++;
                                }
                                goto hasSame;
                            }

                        }

                    }

                }
            }
            else if (tmpIconID == 1)//偵測Bonus不比對wild
            {
                for (int rowInReeL2 = 0; rowInReeL2 < Mod_Data.slotRowNum; rowInReeL2++)
                {
                    if (tmpIconID == Mod_Data.ReelMath[1, rowInReeL2])//依序比對 目前指定的Reel1中的1個icon和Reel2的三個icon是否相同
                    {
                        for (int rowInReeL3 = 0; rowInReeL3 < Mod_Data.slotRowNum; rowInReeL3++)
                        {
                            if (tmpIconID == Mod_Data.ReelMath[2, rowInReeL3])//依序比對 目前指定的Reel1中的1個icon和Reel3的三個icon是否相同
                            {
                                if (tmpcount != 0)  //若有兩個以上確認是否相同符號,相同則不紀錄
                                {
                                    sameicon = false;
                                    for (int i = 0; i < tmpcount; i++)
                                    {
                                        if (tmpArray[i] == tmpIconID)
                                        {
                                            sameicon = true;
                                            break;
                                        }
                                    }
                                    if (!sameicon)
                                    {

                                        tmpArray[tmpcount] = tmpIconID;//紀錄連線的IconID
                                        tmpcount++;
                                    }
                                }
                                else
                                {
                                    tmpArray[tmpcount] = tmpIconID;
                                    tmpcount++;
                                }
                                goto hasSame;
                            }

                        }

                    }

                }
            }
            hasSame:;

            //  Debug.Log("combinTimes:" + combinTimes);

            for (int LineInReeL1 = 0; LineInReeL1 < Mod_Data.slotReelNum; LineInReeL1++)
            {
                tmpIconID = Mod_Data.ReelMath[LineInReeL1, rowInReeL1];
                if (tmpIconID == 2)
                {
                    Mod_Data.ScatterCount++;
                }
            }
        }
        if (Mod_Data.ScatterCount >= 3)
        {
            Mod_Data.getScatter = true;
            tmpIconID = 2;
            tmpArray[tmpcount] = tmpIconID;
            tmpcount++;
        }
        else
        {
            Mod_Data.getScatter = false;
        }

        //tmpcount > 0表示有連線//算分數

        if (tmpcount > 0)
        {


            for (int a = 0; a < tmpcount; a++)//計算不同Icon連線分數
            {

                Mod_Data.waygame_WinIconQuantity[a] = Mod_Data.slotReelNum - 1;//每線輪數預設5個
                combineTimes = 1;//連線數
                if (tmpArray[a] != 1 && tmpArray[a] != 2)
                {
                    for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
                    {
                        for (int row = 0; row < Mod_Data.slotRowNum; row++)
                        {
                            if (tmpArray[a] == Mod_Data.ReelMath[reel, row] || Mod_Data.ReelMath[reel, row] == 0)
                            {
                                times++;
                                Mod_Data.waygame_combineIconBool[a, reel, row] = true;
                                Mod_Data.allgame_AllCombineIconBool[reel, row] = true;
                            }
                        }

                        if (times != 0)
                        {
                            combineTimes = combineTimes * times;//每排個數相乘等於連線數
                            times = 0;

                        }
                        else
                        {
                            Mod_Data.waygame_WinIconQuantity[a] = reel - 1;//若該牌無相同icon則輪數為前一輪
                            break;
                        }
                    }
                }
                else if (tmpArray[a] == 1)
                {
                    if (Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5)
                    {
                        for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
                        {
                            for (int row = 0; row < Mod_Data.slotRowNum; row++)
                            {
                                if (tmpArray[a] == Mod_Data.ReelMath[reel, row])
                                {
                                    times++;
                                    Mod_Data.waygame_combineIconBool[a, reel, row] = true;
                                    Mod_Data.allgame_AllCombineIconBool[reel, row] = true;
                                }
                            }

                            if (times != 0)
                            {
                                combineTimes = combineTimes * times;//每排個數相乘等於連線數
                                times = 0;
                            }
                            else
                            {
                                Mod_Data.waygame_WinIconQuantity[a] = 2;//若該牌無相同icon則輪數為前一輪
                                break;
                            }
                        }
                    }
                    else if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3 || Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4 || Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5)
                    {
                        for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
                        {
                            for (int row = 0; row < Mod_Data.slotRowNum; row++)
                            {
                                if (tmpArray[a] == Mod_Data.ReelMath[reel, row])
                                {
                                    BonusCountTime++;
                                    Mod_Data.waygame_combineIconBool[a, reel, row] = true;
                                    Mod_Data.allgame_AllCombineIconBool[reel, row] = true;
                                    Mod_Data.waygame_WinIconQuantity[a] = BonusCountTime - 1;//若該牌無相同icon則輪數為前一輪
                                }
                            }
                            //if (times != 0)
                            //{
                            //    combineTimes = combineTimes * times;//每排個數相乘等於連線數
                            //    times = 0;
                            //}

                        }
                    }
                }

                else if (tmpArray[a] == 2)
                {
                    for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
                    {
                        for (int row = 0; row < Mod_Data.slotRowNum; row++)
                        {
                            if (tmpArray[a] == Mod_Data.ReelMath[reel, row])
                            {
                                ScatterTimes++;
                                Mod_Data.waygame_combineIconBool[a, reel, row] = true;
                                Mod_Data.allgame_AllCombineIconBool[reel, row] = true;
                                Mod_Data.waygame_WinIconQuantity[a] = ScatterTimes - 1;//若該牌無相同icon則輪數為前一輪
                            }
                        }

                        //if (times != 0) {
                        //    combineTimes = combineTimes * times;//每排個數相乘等於連線數
                        //    times = 0;
                        //}
                        //else {
                        //    Mod_Data.waygame_WinIconQuantity[a] = reel - 1;//若該牌無相同icon則輪數為前一輪
                        //    break;
                        //}

                    }
                }
                Mod_Data.waygame_WinIconID[a] = tmpArray[a];
                Mod_Data.waygmae_EachIconWinLineNum[a] = combineTimes;
                // Debug.Log("tmpArray:" + tmpArray[a] + "combineTimes:" + combineTimes + "times:" + times + "Data.LineLinkPay:" + Data.LineLinkPay[a]);
            }

            Mod_Data.allgame_WinLines = tmpcount;
        }

        //Debug.Log("AAA");
    }//未加第一排wild情況
    public void WayGame_MathClear()
    {
        Check_Pay = 0;
        for (int i = 0; i < Mod_Data.slotRowNum+2; i++)
        {
            Mod_Data.waygame_WinIconQuantity[i] = Mod_Data.slotReelNum - 1;
            Mod_Data.waygmae_EachIconWinLineNum[i] = 0;
        }
        for (int i = 0; i < Mod_Data.slotRowNum+2; i++)
        {
            for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
            {
                for (int row = 0; row < Mod_Data.slotRowNum; row++)
                {
                    Mod_Data.waygame_combineIconBool[i, reel, row] = false;
                    Mod_Data.allgame_AllCombineIconBool[reel, row] = false;
                }

            }
            //Mod_Data.WinningIconArray[a]=
        }
        Mod_Data.allgame_WinLines = 0;
        Mod_Data.Pay = 0;
        if (!Mod_Data.BonusSwitch) Mod_Data.Win = 0;
        if (!Mod_Data.BonusSwitch) Check_Win = 0;
        Mod_Data.getScatter = false;
        Mod_Data.ScatterCount = 0;
        Bonustimes = 0;
    }

    /*
    public void GameSpecialBonusRule()//每個遊戲特殊規則處理 這個是楚河
    {
        for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
        {
            if (Mod_Data.linegame_HasWinline[i])
            {
                //Debug.Log(Mod_Data.linegame_SpecialWild[i]);
                int SpecialBounsWild = (int)Math.Pow(5, Mod_Data.linegame_SpecialWild[i]);
                if (SpecialBounsWild > 125) SpecialBounsWild = 125;
                Mod_Data.linegame_EachLinePay[i] = Mod_Data.iconTablePay[Mod_Data.linegame_WinIconID[i]][Mod_Data.linegame_WinIconQuantity[i]] * SpecialBounsWild * (int)Mod_Data.odds;
                Check_Pay += Mod_Data.linegame_EachLinePay[i];
                //Debug.Log("Mod_Data.linegame_HasWinline[i]: " + i + " Mod_Data.Pay," + Mod_Data.Pay + " EachLinePay," + Mod_Data.linegame_EachLinePay[i] + " iconTablePay," + Mod_Data.iconTablePay[Mod_Data.linegame_WinIconID[i]][Mod_Data.linegame_WinIconQuantity[i]] + " WinIconID," + Mod_Data.linegame_WinIconID[i] + " WinIconQuantity," + Mod_Data.linegame_WinIconQuantity[i] + " SpecialWild," + Mod_Data.linegame_SpecialWild[i]);
                //Debug.Log(Mod_Data.Pay + "," + Mod_Data.linegame_WinIconID[i] + "," + Mod_Data.linegame_EachLinePay[i] + "," + SpecialBounsWild);
            }
        }

        if (Bonustimes > 0)
        {
            Check_Pay += Mod_Data.iconTablePay[1][Bonustimes - 1] * Mod_Data.Bet / Mod_Data.BetOri * (int)Mod_Data.odds;
        }
        Check_Win = Mod_Data.Win * Mod_Data.Denom + Mathf.CeilToInt((float)Check_Pay) * Mod_Data.Denom;
    }
    */

    public void GameSpecialBonusRule()
    {  //每個遊戲特殊規則處理 這個是FishGaGa
        //Random rnd = new Random(Guid.NewGuid().GetHashCode());

        for (int i = 0; i < Mod_Data.allgame_WinLines; i++)
        {
            Check_Pay += Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[i]][Mod_Data.waygame_WinIconQuantity[i]] * Mod_Data.waygmae_EachIconWinLineNum[i] * Mod_Data.BonusSpecialTimes * Mod_Data.odds;
        }
       // Mod_Data.Win += Mathf.CeilToInt((float)Mod_Data.Pay);
        Check_Win = Mod_Data.Win * Mod_Data.Denom + Mathf.CeilToInt((float)Check_Pay) * Mod_Data.Denom;
    }
}


