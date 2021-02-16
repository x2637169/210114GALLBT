using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Mod_GameMath : IGameSystem
{
    int[] matchFishNums = new int[] { 2, 3, 4, 5, 6 };

    #region WayGame數學

    #region WayGame算數處理

    #region 算數主控

    #region 正常WayGame處理

    ///<summary>
    ///WayGame算數
    ///</summary>
    public void WayGame_CountMath()
    {
        bool isNeedRightToLeft = true;
        bool isNeedBT_Rule = true;

        WayGameMathData wayGameMathData_L = new WayGameMathData(Mod_Data.slotReelNum, Mod_Data.slotRowNum, Mod_Data.ReelMath);
        CheckWinConnect(true, false, matchFishNums, false, ref wayGameMathData_L);
        CheckWinIcon(true, ref wayGameMathData_L);

        if (isNeedRightToLeft)
        {
            WayGameMathData wayGameMathData_R = new WayGameMathData(Mod_Data.slotReelNum, Mod_Data.slotRowNum, Mod_Data.ReelMath);
            CheckWinConnect(false, true, matchFishNums, false, ref wayGameMathData_R);
            CheckWinIcon(false, ref wayGameMathData_R);

            if (wayGameMathData_L.connectCount != 0 && wayGameMathData_R.connectCount != 0)
            {
                CombineGameResult(ref wayGameMathData_L, wayGameMathData_R, true);
            }
            else
            {
                if (wayGameMathData_R.connectCount != 0) wayGameMathData_L = wayGameMathData_R;
            }
        }

        if (isNeedBT_Rule)
        {
            WayGame_CountMath_BT_Rule(ref wayGameMathData_L, matchFishNums);
        }

        if (wayGameMathData_L.connectCount != 0)
        {
            SetGameResult(wayGameMathData_L);
        }
    }

    #endregion

    #region 泡泡缸特殊規則處理

    ///<summary>
    ///泡泡缸特殊規則(換魚)算數
    ///</summary>
    public void WayGame_CountMath_BT_Rule(ref WayGameMathData OrigMathData, int[] matchFishNums)
    {
        bool isNeedRightToLeft = true;
        WayGameMathData BTRuleMathData_L = new WayGameMathData(OrigMathData.slotReelNum, OrigMathData.slotRowNum, OrigMathData.reelMath);
        BTRuleMathData_L.reelMath = SetBTRuleMathData(BTRuleMathData_L.reelMath, matchFishNums);
        CheckWinConnect(true, true, matchFishNums, true, ref BTRuleMathData_L);
        CheckWinIcon(true, ref BTRuleMathData_L);

        if (isNeedRightToLeft)
        {
            WayGameMathData BTRuleMathData_R = new WayGameMathData(BTRuleMathData_L.slotReelNum, BTRuleMathData_L.slotRowNum, BTRuleMathData_L.reelMath);
            CheckWinConnect(false, true, matchFishNums, true, ref BTRuleMathData_R);
            CheckWinIcon(false, ref BTRuleMathData_R);

            if (BTRuleMathData_L.connectCount != 0 && BTRuleMathData_R.connectCount != 0)
            {
                CombineGameResult(ref BTRuleMathData_L, BTRuleMathData_R, true);
            }
            else
            {
                if (BTRuleMathData_R.connectCount != 0) BTRuleMathData_L = BTRuleMathData_R;
            }
        }

        if (BTRuleMathData_L.connectCount != 0)
        {
            CombineGameResult(ref OrigMathData, BTRuleMathData_L, false);
        }
    }

    #region 檢查是否需要泡泡缸特殊規則

    ///<summary>
    ///檢查是否需要泡泡缸特殊規則
    ///</summary>
    bool CheckIsNeedBT_Rule(int[] winIcon, int[] matchFishNums)
    {
        for (int i = 0; i < winIcon.Length; i++)
        {
            if (MatchNumbers(winIcon[i], matchFishNums)) return true;
        }

        return false;
    }

    #endregion

    #region 泡泡缸特殊規則-獲取更換魚的ID並更換滾輪數學

    ///<summary>
    ///泡泡缸特殊規則-獲取更換魚的ID並更換滾輪數學
    ///</summary>
    int[,] SetBTRuleMathData(int[,] reelMath, int[] matchFishNums)
    {
        Mod_ChangeFish mod_ChangeFish = FindObjectOfType<Mod_ChangeFish>();
        int changeIconID = mod_ChangeFish.GetChangeFishID();

        for (int y = 0; y < reelMath.GetLength(0); y++)
        {
            for (int p = 0; p < reelMath.GetLength(1); p++)
            {
                if (MatchNumbers(reelMath[y, p], matchFishNums))
                {
                    reelMath[y, p] = changeIconID;
                }
            }
        }

        return reelMath;
    }

    #endregion

    #endregion

    #endregion

    #region 算數處理

    #region 計算贏得的連線數量和ICON

    ///<summary>
    ///計算贏得的連線數量和ICON
    ///</summary>
    ///<param name="isLeftStart">True:從左計算到右 False:從右計算到左</param>
    ///<param name="isNeedMathFishID">泡泡缸特殊規則 True:只需比對魚IconID False:正常運算</param>
    ///<param name="matchFishNums">比對魚IconID</param>
    public void CheckWinConnect(bool isLeftStart, bool isOnlyNeedFishID, int[] matchFishNums, bool isBTRuleFishChange, ref WayGameMathData MathData)
    {
        int tmpIconID;
        bool isSameIcon = false;//連線icon中是否有重複
        MathData.winIconID = new int[0]; //紀錄連線的IconID
        MathData.connectCount = 0; //紀錄連線數量

        //滾輪從左開始或從右開始
        int reel1 = isLeftStart ? 0 : MathData.slotReelNum - 1;
        int reel2 = isLeftStart ? 1 : MathData.slotReelNum - 2;
        int reel3 = isLeftStart ? 2 : MathData.slotReelNum - 3;

        //驗證是否有連線
        for (int rowInReeL1 = 0; rowInReeL1 < MathData.slotRowNum; rowInReeL1++)
        {
            tmpIconID = MathData.reelMath[reel1, rowInReeL1];//存取要比對的ICON

            if (tmpIconID != 0 && tmpIconID != 99 && (!isOnlyNeedFishID || (isOnlyNeedFishID && MatchNumbers(tmpIconID, matchFishNums))))
            {
                //驗證要比對的ICON是否跟贏得ICON相同
                isSameIcon = false;
                for (int i = 0; i < MathData.connectCount; i++)
                {
                    if (MathData.winIconID[i] == tmpIconID)
                    {
                        isSameIcon = true;
                        break;
                    }
                }

                if (!isSameIcon)//是否為相同ICON、是否為Bonus(個別判斷)
                {
                    for (int rowInReeL2 = 0; rowInReeL2 < MathData.slotRowNum; rowInReeL2++)
                    {
                        if (tmpIconID == MathData.reelMath[reel2, rowInReeL2] || (MathData.reelMath[reel2, rowInReeL2] == 0 && tmpIconID != 1))//依序比對 目前指定的Reel1中的1個icon和Reel2的三個icon是否相同
                        {
                            for (int rowInReeL3 = 0; rowInReeL3 < MathData.slotRowNum; rowInReeL3++)
                            {
                                if (tmpIconID == MathData.reelMath[reel3, rowInReeL3] || (MathData.reelMath[reel3, rowInReeL3] == 0 && tmpIconID != 1))//依序比對 目前指定的Reel1中的1個icon和Reel3的三個icon是否相同
                                {
                                    if (MathData.connectCount >= MathData.winIconID.Length) Array.Resize(ref MathData.winIconID, MathData.winIconID.Length + 1);
                                    MathData.winIconID[MathData.connectCount] = tmpIconID;
                                    MathData.connectCount++;
                                    goto hasSame;
                                }
                            }
                        }
                    }
                }
            }
        hasSame:;
        }

        MathData.isMathFishChange = new bool[MathData.connectCount];
        MathData.scoreMultiple = Enumerable.Repeat(1, MathData.connectCount).ToArray();

        if (isBTRuleFishChange)
        {
            for (int i = 0; i < MathData.isMathFishChange.Length; i++)
            {
                MathData.isMathFishChange[i] = true;
            }
        }
    }

    #endregion

    #region 計算贏得的ICON數量和次數

    ///<summary>
    ///依照贏得連線數和ICON，計算ICON數量和次數，並設置動畫和外框線
    ///</summary>
    ///<param name="isLeftStart">True:從左計算到右 False:從右計算到左</param>
    void CheckWinIcon(bool isLeftStart, ref WayGameMathData MathData)
    {
        int times = 0;
        MathData.sameIconWinTimes = Enumerable.Repeat(1, MathData.connectCount).ToArray();
        MathData.waygame_WinIconQuantity = new int[MathData.connectCount];
        MathData.waygame_combineIconBool = new bool[MathData.connectCount, MathData.slotReelNum, MathData.slotRowNum];
        MathData.allgame_AllCombineIconBool = new bool[MathData.slotReelNum, MathData.slotRowNum];

        //tmpcount > 0表示有連線//算分數
        if (MathData.connectCount > 0)
        {
            for (int a = 0; a < MathData.connectCount; a++)//計算不同Icon連線分數
            {
                MathData.waygame_WinIconQuantity[a] = MathData.slotReelNum - 1;//每線輪數預設5個

                if (MathData.winIconID[a] != 1)
                {
                    for (int reel = (isLeftStart ? 0 : MathData.slotReelNum - 1); (isLeftStart ? reel < MathData.slotReelNum : reel >= 0); reel = (isLeftStart ? reel + 1 : reel - 1))
                    {
                        for (int row = 0; row < MathData.slotRowNum; row++)
                        {
                            if (MathData.winIconID[a] == MathData.reelMath[reel, row] || MathData.reelMath[reel, row] == 0)
                            {
                                times++;
                                MathData.waygame_combineIconBool[a, reel, row] = true;
                                MathData.allgame_AllCombineIconBool[reel, row] = true;
                            }
                        }

                        if (times != 0)
                        {
                            MathData.sameIconWinTimes[a] *= times;//每排個數相乘等於連線數
                            times = 0;
                        }
                        else
                        {
                            MathData.waygame_WinIconQuantity[a] = (isLeftStart ? reel - 1 : (MathData.slotReelNum - 1) - (reel + 1));//若該牌無相同icon則輪數為前一輪
                            break;
                        }
                    }
                }
                else if (MathData.winIconID[a] == 1)
                {
                    int BonusCountTime = 0;

                    for (int reel = 0; reel < MathData.slotReelNum; reel++)
                    {
                        for (int row = 0; row < MathData.slotRowNum; row++)
                        {
                            if (MathData.winIconID[a] == MathData.reelMath[reel, row])
                            {
                                BonusCountTime++;
                                MathData.waygame_combineIconBool[a, reel, row] = true;
                                MathData.allgame_AllCombineIconBool[reel, row] = true;
                                MathData.waygame_WinIconQuantity[a] = BonusCountTime - 1;//若該牌無相同icon則輪數為前一輪
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion

    #endregion

    #endregion

    #region 合併兩筆遊戲資料

    ///<summary>
    ///設置計算完畢結果並將兩筆資料合併至靜態變數
    ///</summary>
    ///<param name="isNeedRemoveSameConnect">True:需要刪除重複元素 False:不需要刪除重複元素</param>
    void CombineGameResult(ref WayGameMathData data1, WayGameMathData data2, bool isNeedRemoveSameConnect)
    {
        //Debug.Log("SetGameResult 2");
        if (isNeedRemoveSameConnect) RemoveSameConnect(ref data1, ref data2);
        data1.waygame_WinIconQuantity = data1.waygame_WinIconQuantity.Concat(data2.waygame_WinIconQuantity).ToArray();
        data1.waygame_combineIconBool = CombineIconBool(data1.waygame_combineIconBool, data2.waygame_combineIconBool, data1.waygame_WinIconQuantity, data2.waygame_WinIconQuantity);
        data1.allgame_AllCombineIconBool = AllCombineIconBool(data1.allgame_AllCombineIconBool, data2.allgame_AllCombineIconBool);
        data1.sameIconWinTimes = data1.sameIconWinTimes.Concat(data2.sameIconWinTimes).ToArray();
        data1.winIconID = data1.winIconID.Concat(data2.winIconID).ToArray();
        data1.isMathFishChange = data1.isMathFishChange.Concat(data2.isMathFishChange).ToArray();
        data1.scoreMultiple = data1.scoreMultiple.Concat(data2.scoreMultiple).ToArray();
        data1.connectCount = data1.connectCount + data2.connectCount;
    }

    #region 刪除重複連線的元素

    ///<summary>
    ///檢查兩筆資料的贏分連線是否重計算，並刪除重複計算元素
    ///</summary>
    void RemoveSameConnect(ref WayGameMathData data1, ref WayGameMathData data2)
    {
        //左到右、右到左相同贏得ICON只能算一次，以左到右為基準，刪除右到左相同ICON資料
        for (int i = 0; i < data1.connectCount; i++)
        {
            if (data1.waygame_WinIconQuantity[i] >= data1.slotReelNum - 1)
            {
                for (int y = 0; y < data2.connectCount; y++)
                {
                    if (data1.winIconID[i] == data2.winIconID[y] && data2.waygame_WinIconQuantity[y] >= data2.slotReelNum - 1)
                    {
                        if (MatchNumbers(data1.winIconID[i], matchFishNums)) data1.scoreMultiple[i] += 1;
                        ArrayDeleteAt(ref data2.waygame_WinIconQuantity, y);
                        ArrayDeleteAt(ref data2.waygame_combineIconBool, y);
                        ArrayDeleteAt(ref data2.sameIconWinTimes, y);
                        ArrayDeleteAt(ref data2.winIconID, y);
                        ArrayDeleteAt(ref data2.isMathFishChange, y);
                        ArrayDeleteAt(ref data2.scoreMultiple, y);
                        data2.connectCount--;
                    }
                }
            }
        }
    }

    #endregion

    #region 刪除指定陣列元素

    ///<summary>
    ///刪除指定陣列元素
    ///</summary>
    void ArrayDeleteAt<T>(ref T[] array, int index)
    {
        T[] tmpArray = new T[array.GetLength(0) - 1];

        int y = 0;
        for (int i = 0; i < tmpArray.GetLength(0); i++)
        {
            if (i == index) y++;
            tmpArray[i] = array[y];
            y++;
        }

        array = tmpArray;
    }

    ///<summary>
    ///刪除指定陣列元素
    ///</summary>
    void ArrayDeleteAt<T>(ref T[,,] array, int index)
    {
        T[,,] tmpArray = new T[array.GetLength(0) - 1, array.GetLength(1), array.GetLength(2)];

        int p = 0;
        for (int i = 0; i < tmpArray.GetLength(0); i++)
        {
            if (i == index) p++;
            for (int y = 0; y < tmpArray.GetLength(1); y++)
            {
                for (int q = 0; q < tmpArray.GetLength(2); q++)
                {
                    tmpArray[i, y, q] = array[p, y, q];
                }
            }
            p++;
        }

        array = tmpArray;
    }

    #endregion

    #region 陣列合併

    ///<summary>
    ///合併左到右、右到左跑分框線陣列
    ///</summary>
    ///<returns>返回合併後陣列</returns>
    bool[,,] CombineIconBool(bool[,,] bool1, bool[,,] bool2, int[] WinIconQuantity1, int[] WinIconQuantity2)
    {
        //Debug.Log("----------------------CombineIconBool-----------------");
        //Debug.Log("bool1: " + " length 0: " + bool1.GetLength(0) + " length 1: " + bool1.GetLength(1) + " length 1: " + bool1.GetLength(1));
        //Debug.Log("bool2: " + " length 0: " + bool2.GetLength(0) + " length 1: " + bool2.GetLength(1) + " length 1: " + bool2.GetLength(1));
        bool[,,] tmpArray = new bool[bool1.GetLength(0) + bool2.GetLength(0), bool1.GetLength(1), bool1.GetLength(2)];
        //Debug.Log("tmpArray: " + " length 0: " + tmpArray.GetLength(0) + " length 1: " + tmpArray.GetLength(1) + " length 1: " + tmpArray.GetLength(1));

        for (int i = 0; i < tmpArray.GetLength(0); i++)
        {
            for (int y = 0; y < tmpArray.GetLength(1); y++)
            {
                for (int p = 0; p < tmpArray.GetLength(2); p++)
                {
                    if (i < bool1.GetLength(0))
                    {
                        if (bool1[i, y, p]) tmpArray[i, y, p] = true;
                    }
                    else
                    {
                        int length = i - bool1.GetLength(0);
                        if (bool2[length, (bool2.GetLength(1) - 1) - y, p]) tmpArray[i, (bool2.GetLength(1) - 1) - y, p] = true;
                    }
                }
            }
        }

        return tmpArray;
    }

    ///<summary>
    ///合併左到右、右到左開啟動畫陣列
    ///</summary>
    ///<returns>返回合併後陣列</returns>
    bool[,] AllCombineIconBool(bool[,] bool1, bool[,] bool2)
    {
        bool[,] tmpArray = new bool[bool1.GetLength(0), bool1.GetLength(1)];

        for (int i = 0; i < bool1.GetLength(0); i++)
        {
            for (int y = 0; y < bool1.GetLength(1); y++)
            {
                if (bool1[i, y] || bool2[i, y]) tmpArray[i, y] = true;
            }
        }

        return tmpArray;
    }

    #endregion

    #endregion

    #region 設置遊戲結果(動畫框限、贏分連線數、贏分Icon....)

    ///<summary>
    ///設置計算完畢結果至靜態變數
    ///</summary>
    void SetGameResult(WayGameMathData _data)
    {
        Mod_Data.waygame_WinIconQuantity = _data.waygame_WinIconQuantity;
        Mod_Data.waygame_combineIconBool = _data.waygame_combineIconBool;
        Mod_Data.allgame_AllCombineIconBool = _data.allgame_AllCombineIconBool;
        Mod_Data.waygmae_EachIconWinLineNum = _data.sameIconWinTimes;
        Mod_Data.waygame_WinIconID = _data.winIconID;
        Mod_Data.allgame_WinLines = _data.connectCount;
        Mod_Data.isFishChange_BTRule = _data.isMathFishChange;
        Mod_Data.scoreMultiple = _data.scoreMultiple;
    }

    #endregion

    #region 比對目標數字是否在陣列中

    ///<summary>
    ///比對目標數字是否在陣列中
    ///</summary>
    bool MatchNumbers(int target, int[] nums)
    {
        for (int i = 0; i < nums.Length; i++)
        {
            if (target == nums[i]) return true;
        }

        return false;
    }

    #endregion

    #region WayGame數學資料清除

    public void WayGame_MathClear()
    {
        Mod_Data.waygame_WinIconQuantity = Enumerable.Repeat(Mod_Data.slotReelNum - 1, Mod_Data.slotRowNum + 2).ToArray();
        Mod_Data.waygmae_EachIconWinLineNum = Enumerable.Repeat(1, Mod_Data.slotRowNum + 2).ToArray();
        Array.Clear(Mod_Data.waygame_combineIconBool, 0, Mod_Data.waygame_combineIconBool.Length);
        Array.Clear(Mod_Data.allgame_AllCombineIconBool, 0, Mod_Data.allgame_AllCombineIconBool.Length);
        Array.Clear(Mod_Data.isFishChange_BTRule, 0, Mod_Data.isFishChange_BTRule.Length);
        Mod_Data.scoreMultiple = Enumerable.Repeat(1, Mod_Data.slotRowNum + 2).ToArray();
        Mod_Data.allgame_WinLines = 0;
        Mod_Data.Pay = 0;
        Mod_Data.BT_Rule_Pay = 0;
        if (!Mod_Data.BonusSwitch) Mod_Data.Win = 0;
        Mod_Data.getScatter = false;
        Mod_Data.ScatterCount = 0;
    }

    #endregion

    #endregion

    #region LineGame數學

    int Bonustimes = 0;
    public void LineGame_CountMath()
    {
        int tmpcount = 0, recordIndex = 0, tmpIconID = 0;
        int tmp;
        for (int lineIndex = 0; lineIndex < Mod_Data.linegame_LineCount; lineIndex++)
        {

            tmp = Mod_Data.ReelMath[0, Mod_Data.linegame_Rule[lineIndex][0]];

            if (tmp == 0)//Reel1 = wild情況
            {
                if (Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]] == 0 || Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]] == 12)//Reel2 = wild情況
                {
                    if (Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] == 0 || Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] == 12)//Reel3 = wild情況
                    {
                        if (Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == 0 || Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == 12)//Reel4 = wild情況
                        {
                            if (Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 0 || Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 12)//Reel5 = wild情況
                            {
                                tmp = 0;
                            }
                            else//Reel5 != wild情況
                            {
                                if ((Mod_Data.iconTablePay[Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]]][4]
                                    > Mod_Data.iconTablePay[0][3]) && Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] != 1)
                                {
                                    tmp = Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]];
                                }
                            }
                        }
                        else//Reel4 != wild情況
                        {
                            if ((Mod_Data.iconTablePay[Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]]][3]
                                   > Mod_Data.iconTablePay[0][2]) && Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] != 1)
                            {
                                tmp = Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]];
                            }
                            else if ((Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]]
                                || Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 0) && Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] != 1)
                            {
                                if (Mod_Data.iconTablePay[Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]]][4]
                                    > Mod_Data.iconTablePay[0][2])
                                {
                                    tmp = Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]];
                                }
                            }
                        }
                    }
                    else//Reel3 != wild情況
                    {
                        if ((Mod_Data.iconTablePay[Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]]][2]
                                   > Mod_Data.iconTablePay[0][1]) && Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] != 1)
                        {
                            ////Debug.Log(tmp + "," + Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]]);
                            tmp = Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]];

                        }
                        else if ((Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]]
                                || Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == 0) && Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] != 1)
                        {
                            if ((Mod_Data.iconTablePay[Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]]][3]
                                   > Mod_Data.iconTablePay[0][1]) && Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] != 1)
                            {
                                tmp = Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]];
                            }
                            else if ((Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]]
                                || Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 0) && Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] != 1)
                            {
                                if ((Mod_Data.iconTablePay[Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]]][4]
                                    > Mod_Data.iconTablePay[0][1]) && Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] != 1)
                                {
                                    tmp = Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]];
                                }
                            }

                        }
                    }

                }
                else//Reel2 != wild情況
                {
                    tmp = Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]];
                }
                ////Debug.Log(tmp+","+ Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]]);

            }

            if (tmp != 1 && tmp != 12)
            {
                if (tmp == Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]] || Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]] == 0 || Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]] == 12)
                {
                    if (tmp == Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] || Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] == 0 || Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]] == 12)
                    {
                        Mod_Data.linegame_WinIconQuantity[lineIndex] = 2;
                        Mod_Data.linegame_HasWinline[lineIndex] = true;
                        Mod_Data.linegame_WinIconID[lineIndex] = tmp;
                        recordIndex++;
                        ////Debug.Log(lineIndex);
                        Mod_Data.allgame_AllCombineIconBool[0, Mod_Data.linegame_Rule[lineIndex][0]] = true;
                        Mod_Data.allgame_AllCombineIconBool[1, Mod_Data.linegame_Rule[lineIndex][1]] = true;
                        Mod_Data.allgame_AllCombineIconBool[2, Mod_Data.linegame_Rule[lineIndex][2]] = true;
                        if (tmp == Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] || Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == 0 || Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == 12)
                        {
                            Mod_Data.linegame_WinIconQuantity[lineIndex] = 3;
                            Mod_Data.allgame_AllCombineIconBool[3, Mod_Data.linegame_Rule[lineIndex][3]] = true;
                            if (tmp == Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] || Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 0 || Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 12)
                            {
                                Mod_Data.linegame_WinIconQuantity[lineIndex] = 4;
                                Mod_Data.allgame_AllCombineIconBool[4, Mod_Data.linegame_Rule[lineIndex][4]] = true;
                            }
                        }

                    }
                    //else
                    //{
                    //    if (tmp == 2 || tmp == 3 || tmp == 0)
                    //    {
                    //        Mod_Data.linegame_WinIconQuantity[lineIndex] = 1;
                    //        Mod_Data.linegame_HasWinline[lineIndex] = true;
                    //        Mod_Data.linegame_WinIconID[lineIndex] = tmp;
                    //        recordIndex++;
                    //        ////Debug.Log(lineIndex);
                    //        Mod_Data.allgame_AllCombineIconBool[0, Mod_Data.linegame_Rule[lineIndex][0]] = true;
                    //        Mod_Data.allgame_AllCombineIconBool[1, Mod_Data.linegame_Rule[lineIndex][1]] = true;
                    //    }
                    //}
                }

            }
            ////Debug.Log(lineIndex + "," + tmp);


            #region 須在線上的Bonus判斷 需要時打開註解
            //if (Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5)
            //{   //在線上且1~5連續
            //    if (tmp == Mod_Data.ReelMath[1, Mod_Data.linegame_Rule[lineIndex][1]])
            //    {
            //        if (tmp == Mod_Data.ReelMath[2, Mod_Data.linegame_Rule[lineIndex][2]])
            //        {
            //            Mod_Data.linegame_WinIconQuantity[lineIndex] = 2;
            //            Mod_Data.linegame_HasWinline[lineIndex] = true;
            //            Mod_Data.linegame_WinIconID[lineIndex] = tmp;
            //            recordIndex++;
            //            ////Debug.Log(lineIndex);
            //            Mod_Data.allgame_AllCombineIconBool[0, Mod_Data.linegame_Rule[lineIndex][0]] = true;
            //            Mod_Data.allgame_AllCombineIconBool[1, Mod_Data.linegame_Rule[lineIndex][1]] = true;
            //            Mod_Data.allgame_AllCombineIconBool[2, Mod_Data.linegame_Rule[lineIndex][2]] = true;
            //            if (tmp == Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] || Mod_Data.ReelMath[3, Mod_Data.linegame_Rule[lineIndex][3]] == 0)
            //            {
            //                Mod_Data.linegame_WinIconQuantity[lineIndex] = 3;
            //                Mod_Data.allgame_AllCombineIconBool[3, Mod_Data.linegame_Rule[lineIndex][3]] = true;
            //                if (tmp == Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] || Mod_Data.ReelMath[4, Mod_Data.linegame_Rule[lineIndex][4]] == 0)
            //                {
            //                    Mod_Data.linegame_WinIconQuantity[lineIndex] = 4;
            //                    Mod_Data.allgame_AllCombineIconBool[4, Mod_Data.linegame_Rule[lineIndex][4]] = true;
            //                }
            //            }

            //        }
            //    }
            //}
            //else if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3 || Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4 || Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5)
            //{   //在線上且不連續3個以上
            //    int BonusNum = 0;
            //    for (int i = 0; i < Mod_Data.slotReelNum; i++)
            //    {
            //        if (Mod_Data.ReelMath[i, Mod_Data.linegame_Rule[lineIndex][i]] == 1)
            //        {
            //            BonusNum++;
            //        }
            //    }
            //    if (BonusNum > 2)
            //    {
            //        Mod_Data.linegame_WinIconQuantity[lineIndex] = BonusNum - 1;
            //        Mod_Data.linegame_HasWinline[lineIndex] = true;
            //        Mod_Data.linegame_WinIconID[lineIndex] = 1;
            //        recordIndex++;
            //        ////Debug.Log(lineIndex);
            //        for (int i = 0; i < Mod_Data.slotReelNum; i++)
            //        {
            //            if (Mod_Data.ReelMath[i, Mod_Data.linegame_Rule[lineIndex][i]] == 1)
            //            {
            //                Mod_Data.allgame_AllCombineIconBool[i, Mod_Data.linegame_Rule[lineIndex][i]] = true;
            //            }
            //        }

            //    }
            //}
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
        for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
        {
            Mod_Data.linegame_WinIconQuantity[i] = 0;
            Mod_Data.linegame_HasWinline[i] = false;
            Mod_Data.linegame_WinIconID[i] = 0;
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
        Bonustimes = 0;
    }

    #endregion

    public void WinScoreCount(Mod_Data.GameMode mode)
    {
        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        {
            if (mode == Mod_Data.GameMode.BaseGame)
            {
                for (int i = 0; i < Mod_Data.allgame_WinLines; i++)
                {
                    if (Mod_Data.isFishChange_BTRule[i]) Mod_Data.BT_Rule_Pay += Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[i]][Mod_Data.waygame_WinIconQuantity[i]] * Mod_Data.scoreMultiple[i] * Mod_Data.waygmae_EachIconWinLineNum[i] * Mod_Data.odds;
                    Mod_Data.Pay += Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[i]][Mod_Data.waygame_WinIconQuantity[i]] * Mod_Data.scoreMultiple[i] * Mod_Data.waygmae_EachIconWinLineNum[i] * Mod_Data.odds;

                    if (Mod_Data.waygame_WinIconID[i] == 1) Mod_Data.bonusTimes++;
                }
                Mod_Data.Win += Mathf.CeilToInt((float)Mod_Data.Pay);
            }
            else if (mode == Mod_Data.GameMode.BonusGame)
            {
                GameSpecialBonusRule();
            }
        }
        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        {
            if (mode == Mod_Data.GameMode.BaseGame)
            {
                //Debug.Log(Bonustimes);
                for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
                {
                    if (Mod_Data.linegame_HasWinline[i])
                    {
                        Mod_Data.linegame_EachLinePay[i] = Mod_Data.iconTablePay[Mod_Data.linegame_WinIconID[i]][Mod_Data.linegame_WinIconQuantity[i]] * (int)Mod_Data.odds;
                        Mod_Data.Pay += Mod_Data.linegame_EachLinePay[i];
                    }
                }
                if (Bonustimes > 0)
                {
                    Mod_Data.Pay += Mod_Data.iconTablePay[1][Bonustimes - 1] * Mod_Data.odds;
                    //Debug.Log(Mod_Data.iconTablePay[1][Bonustimes - 1]);
                }
                Mod_Data.Win += Mathf.CeilToInt((float)Mod_Data.Pay * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri)));
            }
            else if (mode == Mod_Data.GameMode.BonusGame)
            {
                GameSpecialBonusRule();
            }
        }
    }

    public void GameSpecialBonusRule()
    {  //每個遊戲特殊規則處理 這個是FishGaGa

        for (int i = 0; i < Mod_Data.allgame_WinLines; i++)
        {
            if (Mod_Data.waygame_WinIconID[i] == 3 && Mod_Data.waygame_WinIconQuantity[i] == 4)
            {
                Mod_Data.Pay += Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[i]][Mod_Data.waygame_WinIconQuantity[i]] * Mod_Data.waygmae_EachIconWinLineNum[i] * Mod_Data.BonusSpecialTimes * Mod_Data.odds;
            }
            else
            {
                Mod_Data.Pay += Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[i]][Mod_Data.waygame_WinIconQuantity[i]] * Mod_Data.waygmae_EachIconWinLineNum[i] * Mod_Data.BonusSpecialTimes * Mod_Data.odds;
            }
        }

        Mod_Data.Win += Mathf.CeilToInt((float)Mod_Data.Pay);

    }

}

public class WayGameMathData
{
    public WayGameMathData(int _SlotReelNum, int _SlotRowNum, int[,] _ReelMath)
    {
        this.slotReelNum = _SlotReelNum;
        this.slotRowNum = _SlotRowNum;
        this.reelMath = _ReelMath;
    }

    public int[] winIconID;
    public int connectCount = 0;
    public int[] sameIconWinTimes;
    public int slotReelNum;
    public int slotRowNum;
    public int[,] reelMath;
    public int[] waygame_WinIconQuantity;
    public bool[,,] waygame_combineIconBool;
    public bool[,] allgame_AllCombineIconBool;
    public bool[] isMathFishChange;
    public int[] scoreMultiple;
}