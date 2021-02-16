using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern_HSMediator;
using CFPGADrv;
using BYTE = System.Byte;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;

public class Mod_State : IGameSystem
{
#if Server
    #region Server

    public static bool InUpdate = false;
    public static bool hardSpaceButtonDown = false;

    public enum STATE
    {
        BaseSpin, BaseScrolling, BaseEnd, BaseRollScore,BaseFishChange_BTRule, BonustransIn, BonusSpin, BonusScrolling, BonusEnd, BonusRollScore, BonusTransOut, GetBonusInBonus, AfterBonusRollScore
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE stateName;
    protected EVENT stage;
    protected Mod_State nextState;

    public Mod_State()
    {
        stage = EVENT.ENTER;

    }

    public virtual void Enter() { Mod_Data.state = this.stateName; Mod_Data.BlankClick = false; m_SlotMediatorController.SendMessage("m_state", "ReregisterState"); stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }
    public Mod_State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    public virtual void SpecialOrder()
    {
        //  //Debug.Log("Testtest123");
    }
}

    #region BaseSpin

//開始滾輪前
public class BaseSpin : Mod_State
{
    int BetInInteger = 0;
    int BetDecimal = 0;

    public BaseSpin() : base()//初始化
    {
        stateName = STATE.BaseSpin;
    }

    public override void Enter()//Enter階段
    {
        Mod_Data.BonusCount = 0;
        billEnableTimer = 0;

        if (Mod_Data.Win > 0)//有贏分跑線時  開起來
        {
            m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        }
        else
        {
            m_SlotMediatorController.SendMessage("m_state", "CloseBlankButton");
        }

        if (Mod_Data.credit - Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom < 0)//彩分低於壓住分數時,停止自動遊戲
        {
            Mod_Data.autoPlay = false;
        }

        Mod_Data.inBaseSpin = true; //偵測是否在BaseSpin
        Mod_Data.StartNotNormal = false;//是否正常遊戲判斷,預設Mod_Data.StartNotNormal = true
        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 0);//設定狀態=BaseSpin
        base.Enter();
    }

    float autoTimer = 0;
    float billEnableTimer = 0f;
    public override void Update()//Update階段
    {
        InUpdate = true;

        if (Mod_Client_Data.MemberCardOut && Mod_Data.cardneeded == 1)
        {
            Mod_Client_Data.MemberCardOut = false;
            Mod_Data.memberLcok = true;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        if (BillAcceptorSettingData.BillOpenClose && !BillAcceptorSettingData.BillAcceptorEnable && Mod_Data.Win == 0)
        {
            if (billEnableTimer > 1f)
            {
                BillAcceptorSettingData.BillAcceptorEnable = true;
                BillAcceptorSettingData.GetOrderType = "BillEnableDisable";
            }
            else
            {
                billEnableTimer += Time.unscaledDeltaTime;
            }
        }

        if (Mod_Data.odds > Mod_Data.maxOdds) Mod_Data.odds = Mod_Data.maxOdds;

        //彩分大於最大彩分,鎖定並跳警告
        if (Mod_Data.credit > Mod_Data.maxCredit)
        {
            if (!Mod_Data.creditErrorLock && !Mod_Data.monthLock)
            {
                //Debug.Log(Mod_Data.credit + "," + Mod_Data.maxCredit);
                m_SlotMediatorController.SendMessage("m_state", "ErrorCreditOpen");
                Mod_Data.creditErrorLock = true;
                Mod_Data.autoPlay = false;
            }

        }
        else
        {
            if (!Mod_Data.winErrorLock && !Mod_Data.monthLock)
            {
                m_SlotMediatorController.SendMessage("m_state", "ErrorCreditClose");
                Mod_Data.creditErrorLock = false;
            }


        }

        //贏分大於最大贏分時,鎖定並警告
        if (Mod_Data.Win > Mod_Data.maxWin)
        {
            if (!Mod_Data.winErrorLock && !Mod_Data.monthLock)
            {
                m_SlotMediatorController.SendMessage("m_state", "ErrorWinOpen");
                Mod_Data.winErrorLock = true;
                Mod_Data.autoPlay = false;
            }

        }
        else
        {
            if (!Mod_Data.creditErrorLock && !Mod_Data.monthLock)
            {
                m_SlotMediatorController.SendMessage("m_state", "ErrorWinClose");
                Mod_Data.winErrorLock = false;
            }
        }

        if (Mod_Data.monthLock && !Mod_Data.creditErrorLock && !Mod_Data.winErrorLock)
        {
            m_SlotMediatorController.SendMessage("m_state", "ErrorMonthLockOpen");
        }

        if (!Mod_Data.monthLock && !Mod_Data.creditErrorLock && !Mod_Data.winErrorLock)
        {
            m_SlotMediatorController.SendMessage("m_state", "ErrorMonthLockClose");
        }

        if (Mod_Data.severHistoryLock) return;
        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_Data.MachineError && !Mod_TimeController.GamePasue && !Mod_Data.memberLcok)
        {
            if (Mod_Data.credit < 0.01f) Mod_Data.credit = 0;
            //如果點擊螢幕,停止跑線&TakeWin
            if (Mod_Data.BlankClick)
            {
                Mod_Data.BlankClick = false;
                m_SlotMediatorController.SendMessage("m_state", "CloseBlankButton");
                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                else Mod_Data.Win = 0;
                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                autoTimer++;
            }

            //自動遊戲計時
            if (Mod_Data.autoPlay)
            {
                autoTimer += Time.deltaTime;
                if (autoTimer > 4)
                    autoTimer = 0;
            }

            if (((hardSpaceButtonDown) || (Mod_Data.autoPlay && autoTimer > 2) || (Mod_Data.autoPlay && Mod_Data.Win <= 0)) && Mod_Data.odds >= 1 && !Mod_Data.MachineError && !Mod_TimeController.GamePasue && !Mod_Data.PrinterTicket && !Mod_Data.memberLcok && ((BillAcceptorSettingData.BillOpenClose && BillAcceptorSettingData.GameCanPlay) || !BillAcceptorSettingData.BillOpenClose && !Mod_Data.changePointLock))
            {
                Mod_Data.linegame_LineCount = Mod_Data.linegame_LineCountOri;//初始化線數
                Mod_Data.Bet = Mod_Data.BetOri;//初始化押注
                //彩分高於目前設定壓注率及倍率(正常押注)
                m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                if (Mod_Data.credit - Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom >= 0)
                {
                    //先消費贈分再消費彩分
                    if (Mod_Data.bonusPoints > 0)
                    {
                        BetInInteger = (int)(Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom / 2);
                        BetDecimal = (int)(((Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom / 2) % 1) * 100 * 2);
                        Mod_Data.bonusPoints -= BetInInteger;
                        Mod_Data.credit -= (BetInInteger + BetDecimal * 0.01);
                        if (Mod_Data.bonusPoints < 0)
                        {
                            Mod_Data.credit += Mod_Data.bonusPoints;
                            Mod_Data.bonusPoints = 0;
                        }
                    }
                    else
                    {
                        Mod_Data.credit -= Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom;
                    }

                    Mod_Data.betLowCreditShowOnce = false;

                    //遊戲計算
                    m_SlotMediatorController.SendMessage("m_state", "DisableAllLine");//消除餘分線
                    m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");//更新分數
                    m_SlotMediatorController.SendMessage("m_state", "SetReel");//更新分數
                    m_SlotMediatorController.SendMessage("m_state", "CheckBonus");//偵測Bonus
                    m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                    Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//遊戲要儲存加贏分之後的彩分
                    m_SlotMediatorController.SendMessage("m_state", "SaveData");//儲存資料
                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 1);
                    if (!Mod_Data.getBonus) m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");//儲存最大贏分
                    Mod_Data.credit -= Mod_Data.Win * Mod_Data.Denom;//儲存完成後先扣掉贏分
                    Mod_Data.severHistoryLock = true;
                    m_SlotMediatorController.SendMessage("m_state", "GetLocalGameRound");
                    m_SlotMediatorController.SendMessage("m_state", "ServerWork", (int)Mod_Client_Data.messagetype.gamehistory);
                    m_SlotMediatorController.SendMessage("m_state", "SaveLocalGameRound");

                    /*紀錄帳務資訊*/
                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet) + Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom);
                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet_Class) + Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom);
                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount) + 1);
                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount_Class) + 1);
                    if (Mod_Data.Win > 0 && !Mod_Data.getBonus)
                    {
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                    }
                    //轉換狀態
                    nextState = new BaseScrolling();
                    nextState.setMediator(m_SlotMediatorController);
                    stage = EVENT.EXIT;
                }

                //餘分計算(餘分變化規則 降低押注率odds,押注降至最高且低於彩分之押注率,若押注率=1時仍無法低於彩分,則降低倍率Denom,降低倍率時,押注率升至最高且可負擔制押注率)
                //Ex1.1:1 彩分23分 最低押注25, odd=1,倍率(Denom降至0.5)彩分46分 最低押注25, odd=1,開始遊戲
                //Ex2.1:1 彩分20分  最低押注25, odd=1,倍率(Denom降至0.1)彩分200分, odds提高至可負擔押住率 odds=8 ,押注=200, 開始遊戲

                else if (Mod_Data.credit > 0)
                {
                    double tmpodds = Mod_Data.odds, tmpDenom = Mod_Data.Denom;
                    bool denomLeast = true, oddLeast = true;

                    //降至可負擔之最小倍率,還無法負擔時,降賭注賠率
                    for (int i = (int)Mod_Data.odds; i >= 1; i--)
                    {
                        if (Mod_Data.credit - Mod_Data.Bet * i * Mod_Data.Denom >= 0)
                        {
                            Mod_Data.odds = i;
                            //Debug.Log("Odds" + i);
                            break;
                        }
                    }

                    //Mod_Data.betLowCreditShowOnce 為顯示餘分線之判斷,若odds有更動時 如Ex1,則顯示一次餘分線後開始遊戲
                    if (!Mod_Data.betLowCreditShowOnce && tmpodds != Mod_Data.odds)//彩分低於最低賭注 顯示一次
                    {
                        Mod_Data.betLowCreditShowOnce = true;
                        Mod_Data.autoPlay = false;
                        m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                        m_SlotMediatorController.SendMessage("m_state", "EnableAllLine");  //顯示餘分線
                        m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                    }
                    else
                    {
                        for (int i = (int)Mod_Data.odds; i >= 1; i--)
                        {
                            if (Mod_Data.credit - Mod_Data.Bet * i * Mod_Data.Denom >= 0)//降至可負擔之最小倍率,還無法負擔時,降賭注賠率
                            {
                                Mod_Data.odds = i;
                                //先消費贈分再消費彩分
                                if (Mod_Data.bonusPoints > 0)
                                {
                                    BetInInteger = (int)(Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom / 2);
                                    BetDecimal = (int)(((Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom / 2) % 1) * 100 * 2);
                                    Mod_Data.bonusPoints -= BetInInteger;
                                    Mod_Data.credit -= (BetInInteger + BetDecimal * 0.01);
                                    if (Mod_Data.bonusPoints < 0)
                                    {
                                        Mod_Data.credit += Mod_Data.bonusPoints;
                                        Mod_Data.bonusPoints = 0;
                                    }
                                }
                                else
                                {
                                    Mod_Data.credit -= Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom;
                                }
                                Mod_Data.betLowCreditShowOnce = false;

                                //遊戲計算
                                m_SlotMediatorController.SendMessage("m_state", "DisableAllLine");//消除餘分線
                                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");//更新分數
                                m_SlotMediatorController.SendMessage("m_state", "SetReel");//更新分數
                                m_SlotMediatorController.SendMessage("m_state", "CheckBonus");//偵測Bonus
                                m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                                Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//遊戲要儲存加贏分之後的彩分
                                m_SlotMediatorController.SendMessage("m_state", "SaveData");//儲存資料
                                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 1);
                                if (!Mod_Data.getBonus) m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");//儲存最大贏分
                                Mod_Data.credit -= Mod_Data.Win * Mod_Data.Denom;//儲存完成後先扣掉贏分
                                Mod_Data.severHistoryLock = true;
                                m_SlotMediatorController.SendMessage("m_state", "GetLocalGameRound");
                                m_SlotMediatorController.SendMessage("m_state", "ServerWork", (int)Mod_Client_Data.messagetype.gamehistory);
                                m_SlotMediatorController.SendMessage("m_state", "SaveLocalGameRound");


                                /*紀錄帳務資訊*/
                                BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet) + Mod_Data.Bet * i * Mod_Data.Denom);
                                BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet_Class) + Mod_Data.Bet * i * Mod_Data.Denom);
                                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount) + 1);
                                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount_Class) + 1);
                                if (Mod_Data.Win > 0 && !Mod_Data.getBonus)
                                {
                                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                                }
                                //轉換狀態
                                nextState = new BaseScrolling();
                                nextState.setMediator(m_SlotMediatorController);
                                oddLeast = false;
                                stage = EVENT.EXIT;
                                break;
                            }
                        }
                    }

                    //若odds無更動時,表示需調整倍率 如Ex2
                    if (oddLeast && (tmpodds == Mod_Data.odds))
                    {
                        Mod_Data.odds = 1;
                        //降至可負擔之最大倍率
                        for (int i = 0; i < Mod_Data.denomArray.Length; i++)
                        {
                            if (Mod_Data.denomOpenArray[i])
                            {
                                if (Mod_Data.credit - Mod_Data.Bet * Mod_Data.odds * Mod_Data.denomArray[i] >= 0)
                                {
                                    Mod_Data.Denom = Mod_Data.denomArray[i];
                                    break;
                                }
                            }
                        }
                        //降至可負擔之最大倍率,押注率升至最高且可負擔之押注率
                        for (int i = (int)Mod_Data.maxOdds; i >= 1; i--)
                        {
                            if (Mod_Data.credit - Mod_Data.Bet * i * Mod_Data.Denom >= 0)
                            {
                                Mod_Data.odds = i;
                                break;
                            }
                        }
                        if (!Mod_Data.betLowCreditShowOnce && tmpDenom != Mod_Data.Denom)//彩分低於最低賭注 顯示一次
                        {
                            Mod_Data.betLowCreditShowOnce = true;
                            Mod_Data.autoPlay = false;
                            m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            m_SlotMediatorController.SendMessage("m_state", "EnableAllLine");
                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                        }
                        else
                        {
                            for (int i = 0; i < Mod_Data.denomArray.Length; i++)
                            {
                                if (Mod_Data.denomOpenArray[i])
                                {
                                    if (Mod_Data.credit - Mod_Data.Bet * Mod_Data.odds * Mod_Data.denomArray[i] >= 0)//降至可負擔之最大倍率
                                    {

                                        Mod_Data.Denom = Mod_Data.denomArray[i];
                                        //先消費贈分再消費彩分
                                        if (Mod_Data.bonusPoints > 0)
                                        {
                                            BetInInteger = (int)(Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom / 2);
                                            BetDecimal = (int)(((Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom / 2) % 1) * 100 * 2);
                                            Mod_Data.bonusPoints -= BetInInteger;
                                            Mod_Data.credit -= (BetInInteger + BetDecimal * 0.01);
                                            if (Mod_Data.bonusPoints < 0)
                                            {
                                                Mod_Data.credit += Mod_Data.bonusPoints;
                                                Mod_Data.bonusPoints = 0;
                                            }
                                        }
                                        else
                                        {
                                            Mod_Data.credit -= Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom;
                                        }
                                        Mod_Data.betLowCreditShowOnce = false;



                                        //遊戲計算
                                        m_SlotMediatorController.SendMessage("m_state", "DisableAllLine");//消除餘分線
                                        m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");//更新分數
                                        m_SlotMediatorController.SendMessage("m_state", "SetReel");//更新分數
                                        m_SlotMediatorController.SendMessage("m_state", "CheckBonus");//偵測Bonus
                                        m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                                        Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//遊戲要儲存加贏分之後的彩分
                                        m_SlotMediatorController.SendMessage("m_state", "SaveData");//儲存資料
                                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 1);
                                        if (!Mod_Data.getBonus) m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");//儲存最大贏分
                                        Mod_Data.credit -= Mod_Data.Win * Mod_Data.Denom;//儲存完成後先扣掉贏分
                                        Mod_Data.severHistoryLock = true;
                                        m_SlotMediatorController.SendMessage("m_state", "GetLocalGameRound");
                                        m_SlotMediatorController.SendMessage("m_state", "ServerWork", (int)Mod_Client_Data.messagetype.gamehistory);
                                        m_SlotMediatorController.SendMessage("m_state", "SaveLocalGameRound");


                                        /*紀錄帳務資訊*/
                                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet) + Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom);
                                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet_Class) + Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom);
                                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount) + 1);
                                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount_Class) + 1);
                                        if (Mod_Data.Win > 0 && !Mod_Data.getBonus)
                                        {
                                            BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                                            BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                                            BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                                            BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                                        }
                                        //轉換狀態
                                        nextState = new BaseScrolling();
                                        nextState.setMediator(m_SlotMediatorController);
                                        denomLeast = false;
                                        stage = EVENT.EXIT;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //若倍率及押注率都最低的情況下還無法負擔時,將押注分數降低至目前彩分
                    if (denomLeast && oddLeast && Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame && (tmpodds == Mod_Data.odds) && (tmpDenom == Mod_Data.Denom))
                    {
                        Mod_Data.odds = 1;
                        for (int i = Mod_Data.denomArray.Length - 1; i >= 0; i--)
                        {
                            if (Mod_Data.denomOpenArray[i])
                            {
                                Mod_Data.Denom = Mod_Data.denomArray[i];
                                //Debug.Log("Denom" + Mod_Data.Denom);
                                break;
                            }
                        }
                        if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 25 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 14)
                        {
                            Mod_Data.Bet = 15;

                        }
                        else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 8 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 15)
                        {
                            Mod_Data.Bet = 9;
                        }
                        else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 2 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 9)
                        {
                            Mod_Data.Bet = 3;
                        }
                        else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) >= 0 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 3)
                        {
                            Mod_Data.Bet = 1;
                        }
                        m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                        //Mod_Data.Bet = Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom));
                        if (!Mod_Data.betLowCreditShowOnce)//彩分低於最低賭注 顯示一次
                        {
                            Mod_Data.betLowCreditShowOnce = true;
                            m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            m_SlotMediatorController.SendMessage("m_state", "EnableAllLine");
                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                        }
                        else
                        {
                            //Mod_Data.odds = 1;
                            //Mod_Data.Bet = Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom));
                            //if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 25 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 14) {
                            //    Mod_Data.Bet = 15;
                            //}
                            //else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 8 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 15) {
                            //    Mod_Data.Bet = 9;
                            //}
                            //else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 2 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 9) {
                            //    Mod_Data.Bet = 3;
                            //}
                            //else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) >= 0 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 3) {
                            //    Mod_Data.Bet = 1;
                            //}
                            //m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                            if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) == 1)
                            {
                                Mod_Data.credit = 0;
                                Mod_Data.betLowCreditShowOnce = false;
                            }
                            else
                            {
                                double tmp = Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom;
                                Mod_Data.credit = (double)(((decimal)Mod_Data.credit) - (decimal)tmp);
                                if (Mod_Data.credit < 0.001)
                                {
                                    Mod_Data.credit = 0;
                                    Mod_Data.betLowCreditShowOnce = false;

                                    //遊戲計算
                                    m_SlotMediatorController.SendMessage("m_state", "DisableAllLine");//消除餘分線
                                    m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");//更新分數
                                    m_SlotMediatorController.SendMessage("m_state", "SetReel");//更新分數
                                    m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                                    m_SlotMediatorController.SendMessage("m_state", "CheckBonus");//偵測Bonus
                                    m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                                    Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//遊戲要儲存加贏分之後的彩分
                                    m_SlotMediatorController.SendMessage("m_state", "SaveData");//儲存資料
                                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 1);
                                    if (!Mod_Data.getBonus) m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");//儲存最大贏分
                                    Mod_Data.credit -= Mod_Data.Win * Mod_Data.Denom;//儲存完成後先扣掉贏分
                                    Mod_Data.severHistoryLock = true;
                                    m_SlotMediatorController.SendMessage("m_state", "GetLocalGameRound");
                                    m_SlotMediatorController.SendMessage("m_state", "ServerWork", (int)Mod_Client_Data.messagetype.gamehistory);
                                    m_SlotMediatorController.SendMessage("m_state", "SaveLocalGameRound");


                                    /*紀錄帳務資訊*/
                                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet) + Mod_Data.Bet * Mod_Data.Denom);
                                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet_Class) + Mod_Data.Bet * Mod_Data.Denom);
                                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount) + 1);
                                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount_Class) + 1);
                                    if (Mod_Data.Win > 0 && !Mod_Data.getBonus)
                                    {
                                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                                    }

                                    nextState = new BaseScrolling();
                                    nextState.setMediator(m_SlotMediatorController);
                                    oddLeast = false;

                                    stage = EVENT.EXIT;
                                }
                            }
                        }
                    }//餘分計算
                    else//如果沒錢
                    {
                        Mod_Data.autoPlay = false;
                    }
                }

            }
    #region Keyboard_Control
            //-----鍵盤控制
            //// 押注-
            // if (Input.GetKeyDown(KeyCode.R))
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock)
            //     {
            //         Mod_Data.Bet = Mod_Data.BetOri;
            //         if (Mod_Data.odds > 1)
            //         {
            //             Mod_Data.odds--;
            //             //if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //             //else Mod_Data.Win = 0;
            //             m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //             Mod_Data.Win = 0;
            //             m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
            //         }
            //     }
            // }
            // //押注+
            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock)
            //     {
            //         Mod_Data.Bet = Mod_Data.BetOri;
            //         if (Mod_Data.odds < Mod_Data.maxOdds)
            //         {
            //             Mod_Data.odds++;
            //             //if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //             //else Mod_Data.Win = 0;
            //             m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //             Mod_Data.Win = 0;
            //             m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
            //         }

            //     }
            // }
            // //自動
            // if (Input.GetKeyDown(KeyCode.A))
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
            // }
            // //最大押注
            // if (Input.GetKeyDown(KeyCode.S) && !Mod_Data.autoPlay)
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock)
            //     {
            //         Mod_Data.Bet = Mod_Data.BetOri;
            //         Mod_Data.odds = Mod_Data.maxOdds;
            //         //if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //         //else Mod_Data.Win = 0;
            //         m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //         Mod_Data.Win = 0;
            //         m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");

            //     }
            // }
            // //+注
            // if (Input.GetKeyDown(KeyCode.Z) && !Mod_Data.autoPlay)
            // {
            //     //Debug.Log("+D");
            //     Mod_Data.credit += 1000;
            //     m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
            // }
            // //統計資料
            // if (Input.GetKeyDown(KeyCode.O) && !Mod_Data.autoPlay)
            // {
            //     if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
            //     {

            //         Mod_Data.IOLock = true;
            //         if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //         m_SlotMediatorController.SendMessage("m_state", "OpenAccount");
            //     }
            // }
            // //後台
            // if (Input.GetKeyDown(KeyCode.P) && !Mod_Data.autoPlay)
            // {
            //     if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
            //     {
            //         Mod_Data.IOLock = true;
            //         if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //         m_SlotMediatorController.SendMessage("m_state", "OpenLogin");
            //     }
            // }
            // //洗分
            // if (Input.GetKeyDown(KeyCode.E) && !Mod_Data.autoPlay)
            // {
            //     m_SlotMediatorController.SendMessage("m_state", "CheckClearPoint");
            //     Mod_Data.Win = 0;
            //     m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
            //     //Debug.Log("clear");
            // }
            // //開分
            // if (Input.GetKeyDown(KeyCode.W) && !Mod_Data.autoPlay)
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && Mod_Data.Win <= 0)
            //     {
            //         m_SlotMediatorController.SendMessage("m_state", "OpenPoint");
            //     }
            // }
    #endregion

        }
        hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        Mod_Data.afterBonus = false;
        Mod_Data.inBaseSpin = false;
        if (BillAcceptorSettingData.BillOpenClose && BillAcceptorSettingData.BillAcceptorEnable)
        {
            BillAcceptorSettingData.BillAcceptorEnable = false;
            BillAcceptorSettingData.GetOrderType = "BillEnableDisable";
        }
        //Debug.Log("BaseSpinExit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
    }
}

    #endregion

    #region BaseScrolling

//滾輪啟動
public class BaseScrolling : Mod_State
{
    public BaseScrolling() : base()
    {
        stateName = STATE.BaseScrolling;
        // base.Enter();
    }

    float detectButtonDownTime = 0;
    public override void Enter()
    {
        if (BillAcceptorSettingData.BillOpenClose) BillAcceptorSettingData.CheckIsInBaseSpin = false;

        //若不正常啟動時 斷電恢復
        if (Mod_Data.StartNotNormal)
        {
            detectButtonDownTime += Time.deltaTime;
            if (detectButtonDownTime > 2f)
            {
                m_SlotMediatorController.SendMessage("m_state", "SetReel");
                m_SlotMediatorController.SendMessage("m_state", "CheckBonus");
                if (Mod_Data.Bet < 25)
                {
                    m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                }
                m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                Mod_Data.severHistoryLock = true;
                m_SlotMediatorController.SendMessage("m_state", "GetLocalGameRound");
                m_SlotMediatorController.SendMessage("m_state", "ServerWork", (int)Mod_Client_Data.messagetype.gamehistory);
                m_SlotMediatorController.SendMessage("m_state", "SaveLocalGameRound");
                m_SlotMediatorController.SendMessage("m_state", "StartRunSlots");
                detectButtonDownTime = 0;
                base.Enter();
            }
        }
        else
        {
            m_SlotMediatorController.SendMessage("m_state", "StartRunSlots");
            base.Enter();
        }
    }

    public override void Update()
    {
        detectButtonDownTime += Time.deltaTime;

        if (detectButtonDownTime > 0.2f)
        {
            nextState = new BaseEnd();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //Debug.Log("BaseSpin Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBaseEnd");
    }
}

    #endregion

    #region BaseEnd

public class BaseEnd : Mod_State
{
    public BaseEnd() : base()
    {
        stateName = STATE.BaseEnd;
        // base.Enter();
    }

    bool isSpaceOnce = false;
    public override void Enter()
    {
        m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        isSpaceOnce = false;
        base.Enter();
    }

    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        if ((hardSpaceButtonDown) && !Mod_Data.MachineError)
        {
            Mod_Data.BlankClick = false;
            if (!isSpaceOnce) m_SlotMediatorController.SendMessage("m_state", "StopRunSlots");
            isSpaceOnce = true;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A) && !Mod_TimeController.GamePasue && !Mod_Data.MachineError)
        {  //自動
            Mod_Data.autoPlay = !Mod_Data.autoPlay;
        }
#endif

        if (hardSpaceButtonDown)
        {
            Mod_Data.BlankClick = false;
            if (!isSpaceOnce) m_SlotMediatorController.SendMessage("m_state", "StopRunSlots");
            isSpaceOnce = true;
        }

        if (Mod_Data.reelAllStop)
        {
            //Debug.Log("StopAll");
            m_SlotMediatorController.SendMessage("m_state", "StopRunSlots");
            m_SlotMediatorController.SendMessage("m_state", "PlayAnimation");
            nextState = new BaseRollScore();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }

        hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        //Debug.Log("BaseEnd Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBaseEnd");
    }
}

    #endregion

    #region BaseRollScore

public class BaseRollScore : Mod_State
{
    public BaseRollScore() : base()
    {
        stateName = STATE.BaseRollScore;
    }

    float timer = 0;
    public override void Enter()
    {
        m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");

        timer += Time.deltaTime;
        if (timer > 0.2f)
        {
            if (Mod_Data.getBonus)
            {
                nextState = new BonusTransIn();
                Mod_Data.getBonus = false;
            }
            else
            {
                nextState = new BaseSpin();
                Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//獲得bonus先不加入彩分
            }

            nextState.setMediator(m_SlotMediatorController);

            if (Mod_Data.Pay > 0)
            {
                Mod_Data.runScore = true;
                m_SlotMediatorController.SendMessage("m_state", "StartRollScore");
            }
            timer = 0;
            base.Enter();
        }
    }

    bool stopScore = false;
    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        timer += Time.deltaTime;

        if (Mod_Data.Win > 0)
        {
            if (Mod_Data.runScore)
            {
                if ((hardSpaceButtonDown || Mod_Data.BlankClick) && !Mod_TimeController.GamePasue && !Mod_Data.MachineError)
                {
                    if (timer > Mod_Data.BonusDelayTimes)
                    {
                        Mod_Data.BlankClick = false;
                        m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                        stage = EVENT.EXIT;
                    }
                }
            }
            else
            {
                if (!stopScore)
                {
                    m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                    stopScore = true;
                    stage = EVENT.EXIT;
                }
            }
        }
        else
        {
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        InUpdate = false;
        Mod_Data.runScore = false;
        Mod_Data.StartNotNormal = false;
        //Debug.Log("BaseRollScore Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {

    }
}

    #endregion

    #region BonusTransIn

public class BonusTransIn : Mod_State
{
    float timer = 0;
    public BonusTransIn() : base()
    {
        stateName = STATE.BonustransIn;
    }

    public override void Enter()
    {
        if (Mod_Data.StartNotNormal)
        {
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                m_SlotMediatorController.SendMessage("m_state", "PlayBonusTransAnim");
                Mod_Data.BlankClick = false;
                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 2);
                base.Enter();
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > 2f)
            {
                m_SlotMediatorController.SendMessage("m_state", "PlayBonusTransAnim");
                Mod_Data.BlankClick = false;
                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 2);
                base.Enter();
            }
        }
    }

    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif
        if (Mod_Data.transInAnimEnd)
        {
            // m_SlotMediatorController.SendMessage(this, "ShowChoosePanel");//如果還有可玩場數 就跳選擇畫面
            //Mod_Data.startBonus = true;
            if (timer < 5) timer += Time.deltaTime;

            if (!Mod_Data.BonusSwitch)
            {
                m_SlotMediatorController.SendMessage("m_state", "ShowStartTriggerPanel"); //顯示贏得幾場
                m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame"); //停止所有框線
                Mod_Data.BonusSwitch = true;
                m_SlotMediatorController.SendMessage("m_state", "ChangeScene", 0);//轉換場景至Bonus
            }

            if (timer > 2)
            {
                m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
            }

            if ((hardSpaceButtonDown || Mod_Data.BlankClick) && timer > 3 && !Mod_Data.MachineError)
            {
                Mod_Data.BlankClick = false;
                m_SlotMediatorController.SendMessage("m_state", "CloseTriggerPanel");
                m_SlotMediatorController.SendMessage("m_state", "BonustransEnd");
                Mod_Data.transInAnimEnd = false;
                // Mod_Data.startBonus = true;
                timer = 0;
            }
        }

        if (Mod_Data.startBonus)
        {
            timer += Time.deltaTime;

            if (timer > 1)
            {
                Mod_Data.BonusIsPlayedCount = 0;
                Mod_Data.startBonus = false;
                nextState = new BonusSpin();
                nextState.setMediator(m_SlotMediatorController);
                Mod_Data.BonusSwitch = true;
                stage = EVENT.EXIT;
            }
        }
        //hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        //m_SlotMediatorController.SendMessage(this, "BonustransEnd");
        //Debug.Log("BonusTransExit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        //  //Debug.Log("TestBonusTrans");
    }
}

    #endregion

    #region BonusSpin

public class BonusSpin : Mod_State
{
    float timer = 0;
    public BonusSpin() : base()
    {
        stateName = STATE.BonusSpin;
    }

    public override void Enter()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f && !Mod_Data.severHistoryLock)
        {
            Mod_Data.BonusIsPlayedCount++;
            Mod_Data.StartNotNormal = false;
            base.Enter();
        }
    }

    public override void Update()
    {
        //Debug.Log(Mod_Data.BonusSwitch);
        //Debug.Log("BonusSpinUpdate");
        m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
        m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
        m_SlotMediatorController.SendMessage("m_state", "SetReel");
        if (Mod_Data.Bet < 25)
        {
            m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
        }
        m_SlotMediatorController.SendMessage("m_state", "CheckBonus");
        m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
        Mod_Data.severHistoryLock = true;
        m_SlotMediatorController.SendMessage("m_state", "GetLocalGameRound");
        m_SlotMediatorController.SendMessage("m_state", "ServerWork", (int)Mod_Client_Data.messagetype.gamehistory);
        m_SlotMediatorController.SendMessage("m_state", "SaveLocalGameRound");
        m_SlotMediatorController.SendMessage("m_state", "SaveData");
        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 3);

        nextState = new BonusScrolling();
        nextState.setMediator(m_SlotMediatorController);
        stage = EVENT.EXIT;

    }

    public override void Exit()
    {
        //Debug.Log("BonusSpinExit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBonusSpin");
    }
}

    #endregion

    #region BonusScrolling

public class BonusScrolling : Mod_State
{
    public BonusScrolling() : base()
    {
        stateName = STATE.BonusScrolling;
        // base.Enter();
    }

    float detectButtonDownTime = 0;
    public override void Enter()
    {
        if (!Mod_Data.severHistoryLock)
        {

            if (Mod_Data.StartNotNormal)
            {
                detectButtonDownTime += Time.deltaTime;

                if (detectButtonDownTime > 2f)
                {
                    m_SlotMediatorController.SendMessage("m_state", "SetReel");
                    if (Mod_Data.Bet < 25)
                    {
                        m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                    }
                    m_SlotMediatorController.SendMessage("m_state", "CheckBonus");
                    m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                    Mod_Data.severHistoryLock = true;
                    m_SlotMediatorController.SendMessage("m_state", "GetLocalGameRound");
                    m_SlotMediatorController.SendMessage("m_state", "ServerWork", (int)Mod_Client_Data.messagetype.gamehistory);
                    m_SlotMediatorController.SendMessage("m_state", "SaveLocalGameRound");
                    m_SlotMediatorController.SendMessage("m_state", "StartRunSlots");
                    m_SlotMediatorController.SendMessage("m_state", "PlayBonusBackGroundSound");
                    detectButtonDownTime = 0;
                    base.Enter();
                }
            }
            else
            {
                m_SlotMediatorController.SendMessage("m_state", "StartRunSlots");
                base.Enter();
            }
        }
    }

    public override void Update()
    {
        detectButtonDownTime += Time.deltaTime;

        if (detectButtonDownTime > 0.2f)
        {
            nextState = new BonusEnd();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //Debug.Log("BonusRoll Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBaseEnd");
    }
}

    #endregion

    #region BonusEnd

public class BonusEnd : Mod_State
{
    public BonusEnd() : base()
    {
        stateName = STATE.BonusEnd;
    }

    public override void Enter()
    {
        //Debug.Log("BonusEndEnter");
        // m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        base.Enter();
    }

    public override void Update()
    {
        //Debug.Log("BonusEndUpdate");
        if (Mod_Data.reelAllStop)
        {
            m_SlotMediatorController.SendMessage("m_state", "StopRunSlots");
            //m_SlotMediatorController.SendMessage("m_state", "CheckBonus");
            //m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
            Mod_Data.BonusDelayTimes = 0;

            //m_SlotMediatorController.SendMessage("m_state", "PlayDKSpecialAnim");
            m_SlotMediatorController.SendMessage("m_state", "PlayAnimation");

            nextState = new BonusRollScore();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //Debug.Log("BonusEndExit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBonusEnd");
    }
}

    #endregion

    #region BonusRollScore

public class BonusRollScore : Mod_State
{
    public BonusRollScore() : base()
    {
        stateName = STATE.BonusRollScore;
    }

    float timer = 0;
    public override void Enter()
    {
        m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");

        timer += Time.deltaTime;
        if (timer > 0.7)
        {
            if (Mod_Data.BonusIsPlayedCount >= Mod_Data.BonusCount)
            {
                nextState = new BonusTransOut();
                //Debug.Log("A");
            }
            else
            {
                if (Mod_Data.getBonus)
                {
                    nextState = new GetBonusInBonus();
                    Mod_Data.getBonus = false;
                }
                else
                {
                    nextState = new BonusSpin();
                }
            }
            nextState.setMediator(m_SlotMediatorController);
            if (Mod_Data.Pay > 0)
            {
                Mod_Data.runScore = true;
                m_SlotMediatorController.SendMessage("m_state", "StartFastRollScore");
                //Debug.Log("RollScoreEnter" + Mod_Data.runScore);
                //Debug.Log("Mod_Data.Win" + Mod_Data.Win);
            }
            // m_SlotMediatorController.SendMessage("m_state", "SaveData");

            timer = 0;
            base.Enter();
        }
    }

    bool stopScore = false;
    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        timer += Time.deltaTime;

        if ((Mod_Data.BonusSwitch && Mod_Data.Pay > 0) || (!Mod_Data.BonusSwitch && Mod_Data.Win > 0))
        {
            if (Mod_Data.runScore)
            {
                if ((hardSpaceButtonDown || Mod_Data.BlankClick) && !Mod_Data.MachineError)
                {
                    if (timer > Mod_Data.BonusDelayTimes)
                    {
                        Mod_Data.BlankClick = false;
                        m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                        if (Mod_Animation.isPlayingDKSpecialAnim) m_SlotMediatorController.SendMessage("m_state", "StopDKSpecialAnim");
                        stage = EVENT.EXIT;
                    }
                }
            }
            else
            {
                if (!stopScore)
                {
                    m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                    stopScore = true;
                }
                if (timer > Mod_Data.BonusDelayTimes)
                {
                    if (!stopScore) m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                    if (Mod_Animation.isPlayingDKSpecialAnim) m_SlotMediatorController.SendMessage("m_state", "StopDKSpecialAnim");
                    stage = EVENT.EXIT;
                }
            }
        }
        else
        {
            if (timer > Mod_Data.BonusDelayTimes)
            {
                if (Mod_Animation.isPlayingDKSpecialAnim) m_SlotMediatorController.SendMessage("m_state", "StopDKSpecialAnim");
                stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        InUpdate = false;
        Mod_Data.runScore = false;
        base.Exit();

    }
    public override void SpecialOrder()
    {

    }
}

    #endregion

    #region BonusTransOut

public class BonusTransOut : Mod_State
{
    public BonusTransOut() : base()
    {
        stateName = STATE.BonusTransOut;
    }

    public override void Enter()
    {
        //  m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        base.Enter();
        timer += Time.deltaTime;
        Mod_Data.transInAnimEnd = true;
        //if (timer > 2)
        //{
        //    m_SlotMediatorController.SendMessage("m_state", "PlayBonusTransOutAnim");
        //    timer = 0;

        //    base.Enter();
        //}



    }
    float timer = 0;
    public override void Update()
    {
        if (Mod_Data.transInAnimEnd)
        {
            m_SlotMediatorController.SendMessage("m_state", "ShowBonusScorePanel");//如果可玩場次結束 就跳分數
            //Mod_Data.BonusSpecialTimes = 1;
            //Mod_Data.BonusCount = 0;
            //m_SlotMediatorController.SendMessage("m_state", "SaveBonusInforamtion");//儲存bonus倍率與場數資訊
            Mod_Data.BonusDelayTimes = 0;
            nextState = new AfterBonusRollScore();
            nextState.setMediator(m_SlotMediatorController);
            Mod_Data.transInAnimEnd = false;
            timer = 10;
        }

        timer -= Time.deltaTime;
        if (timer > 0 && timer < 5)
        {
            stage = EVENT.EXIT;
        }

    }
    public override void Exit()
    {
        m_SlotMediatorController.SendMessage("m_state", "BonusEndtransOut");
        Mod_Data.BonusSwitch = false;
        m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
        m_SlotMediatorController.SendMessage("m_state", "ChangeScene", 1);
        Mod_Data.afterBonus = true;
        //Debug.Log("BonusTransOutExit");
        base.Exit();
        //m_SlotMediatorController.SendMessage("m_state", "ChangeScene");
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBonusEnd");
    }
}

    #endregion

    #region AfterBonusRollScore

public class AfterBonusRollScore : Mod_State
{
    public AfterBonusRollScore() : base()
    {
        stateName = STATE.AfterBonusRollScore;
    }

    float timer = 0;
    public override void Enter()
    {
        m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");

        timer += Time.deltaTime;
        if (timer > 0.7)
        {
            m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWinInBonus");
            nextState = new BaseSpin();
            nextState.setMediator(m_SlotMediatorController);
            //Debug.Log("Mod_Data.Win" + Mod_Data.Win);
            Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;
            if (Mod_Data.Win > 0)
            {
                Mod_Data.runScore = true;
                m_SlotMediatorController.SendMessage("m_state", "StartRollScore");
            }
            base.Enter();
        }
    }

    bool stopScore = false;
    bool saveSrame = false;
    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        if (Mod_Data.Win > 0)
        {
            m_SlotMediatorController.SendMessage("m_state", "EndBonusShow");
            if (Mod_Data.runScore)
            {
                if ((hardSpaceButtonDown || Mod_Data.BlankClick) && !Mod_Data.MachineError)
                {
                    if (timer > Mod_Data.BonusDelayTimes)
                    {
                        saveSrame = true;
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);

                        Mod_Data.BlankClick = false;
                        m_SlotMediatorController.SendMessage("m_state", "StopRollScore");

                        stage = EVENT.EXIT;
                    }
                }
            }
            else
            {
                if (!stopScore)
                {
                    if (!saveSrame)
                    {
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                    }
                    m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                    stopScore = true;
                    stage = EVENT.EXIT;
                }
            }
        }
        else
        {
            stage = EVENT.EXIT;
        }
        Mod_Data.BonusIsPlayedCount = 0;
        Mod_Data.BonusCount = 0;
    }

    public override void Exit()
    {
        InUpdate = false;
        Mod_Data.BonusSpecialTimes = 1;
        Mod_Data.BonusCount = 0;
        m_SlotMediatorController.SendMessage("m_state", "SaveBonusInforamtion");//儲存bonus倍率與場數資訊
        m_SlotMediatorController.SendMessage("m_state", "SaveData");
        m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");
        Mod_Data.runScore = false;
        base.Exit();
    }

    public override void SpecialOrder()
    {

    }
}

    #endregion

    #region GetBonusInBonus

public class GetBonusInBonus : Mod_State
{
    public GetBonusInBonus() : base()
    {
        stateName = STATE.GetBonusInBonus;
    }

    float timer = 0;
    public override void Enter()
    {
        //m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        //Debug.Log("BonusInBonusEnter");
        timer = 0;
        base.Enter();
    }

    public override void Update()
    {
        m_SlotMediatorController.SendMessage("m_state", "ShowReTriggerPanel");
        timer += Time.deltaTime;
        if (timer > 1.5f)
        {
            Mod_Data.BlankClick = false;
            m_SlotMediatorController.SendMessage("m_state", "CloseTriggerPanel");
            nextState = new BonusSpin();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //Debug.Log("BonusInBonus Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBonusEnd");
    }
}

    #endregion

    #endregion
#else
    #region !Server

    public static bool InUpdate = false;

    public static bool hardSpaceButtonDown = false;

    public enum STATE
    {
        BaseSpin, BaseScrolling, BaseEnd, BaseRollScore, BaseFishChange_BTRule, BonustransIn, BonusSpin, BonusScrolling, BonusEnd, BonusRollScore, BonusTransOut, GetBonusInBonus, AfterBonusRollScore
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE stateName;
    protected EVENT stage;
    protected Mod_State nextState;

    public Mod_State()
    {
        stage = EVENT.ENTER;

    }

    public virtual void Enter() { Mod_Data.state = this.stateName; Mod_Data.BlankClick = false; m_SlotMediatorController.SendMessage("m_state", "ReregisterState"); stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }
    public Mod_State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    public virtual void SpecialOrder()
    {
        //  //Debug.Log("Testtest123");
    }
}

#region BaseSpin

//開始滾輪前
public class BaseSpin : Mod_State
{
    public BaseSpin() : base()//初始化
    {

        stateName = STATE.BaseSpin;
    }

    public override void Enter()//Enter階段
    {
        if (BillAcceptorSettingData.BillOpenClose)
        {
            BillAcceptorSettingData.BillAcceptorEnable = true;
            BillAcceptorSettingData.GetOrderType = "BillEnableDisable";
        }

        Mod_Data.BonusCount = 0;

        if (Mod_Data.Win > 0)//有贏分跑線時  開起來
        {
            m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        }
        else
        {
            m_SlotMediatorController.SendMessage("m_state", "CloseBlankButton");
        }

        if (Mod_Data.credit - Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom < 0)//彩分低於壓住分數時,停止自動遊戲
        {
            Mod_Data.autoPlay = false;
        }

        Mod_Data.inBaseSpin = true; //偵測是否在BaseSpin
        Mod_Data.StartNotNormal = false;//是否正常遊戲判斷,預設Mod_Data.StartNotNormal = true
        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 0);//設定狀態=BaseSpin
        base.Enter();

    }

    float autoTimer = 0;
    public override void Update()//Update階段
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        if (Mod_Data.odds > Mod_Data.maxOdds) Mod_Data.odds = Mod_Data.maxOdds;

        //彩分大於最大彩分,鎖定並跳警告
        if (Mod_Data.credit > Mod_Data.maxCredit)
        {
            if (!Mod_Data.creditErrorLock && !Mod_Data.monthLock)
            {
                //Debug.Log(Mod_Data.credit + "," + Mod_Data.maxCredit);
                m_SlotMediatorController.SendMessage("m_state", "ErrorCreditOpen");
                Mod_Data.creditErrorLock = true;
                Mod_Data.autoPlay = false;
            }

        }
        else
        {
            if (!Mod_Data.winErrorLock && !Mod_Data.monthLock)
            {
                m_SlotMediatorController.SendMessage("m_state", "ErrorCreditClose");
                Mod_Data.creditErrorLock = false;
            }


        }

        //贏分大於最大贏分時,鎖定並警告
        if (Mod_Data.Win > Mod_Data.maxWin)
        {
            if (!Mod_Data.winErrorLock && !Mod_Data.monthLock)
            {
                m_SlotMediatorController.SendMessage("m_state", "ErrorWinOpen");
                Mod_Data.winErrorLock = true;
                Mod_Data.autoPlay = false;
            }

        }
        else
        {
            if (!Mod_Data.creditErrorLock && !Mod_Data.monthLock)
            {
                m_SlotMediatorController.SendMessage("m_state", "ErrorWinClose");
                Mod_Data.winErrorLock = false;
            }
        }

        if (Mod_Data.monthLock && !Mod_Data.creditErrorLock && !Mod_Data.winErrorLock)
        {
            m_SlotMediatorController.SendMessage("m_state", "ErrorMonthLockOpen");
        }

        if (!Mod_Data.monthLock && !Mod_Data.creditErrorLock && !Mod_Data.winErrorLock)
        {
            m_SlotMediatorController.SendMessage("m_state", "ErrorMonthLockClose");
        }

        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && ((BillAcceptorSettingData.BillOpenClose && BillAcceptorSettingData.GameCanPlay) || !BillAcceptorSettingData.BillOpenClose))
        {
            if (Mod_Data.credit < 0.01f) Mod_Data.credit = 0;
            //如果點擊螢幕,停止跑線&TakeWin
            if (Mod_Data.BlankClick)
            {
                Mod_Data.BlankClick = false;
                m_SlotMediatorController.SendMessage("m_state", "CloseBlankButton");
                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                else Mod_Data.Win = 0;
                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                autoTimer++;
            }
            //自動遊戲計時
            if (Mod_Data.autoPlay)
            {
                autoTimer += Time.deltaTime;
                if (autoTimer > 4)
                    autoTimer = 0;
            }

            if (((hardSpaceButtonDown) || (Mod_Data.autoPlay && autoTimer > 2) || (Mod_Data.autoPlay && Mod_Data.Win <= 0)) && Mod_Data.odds >= 1)
            {
                //Debug.Log("Start!!");
                Mod_Data.linegame_LineCount = Mod_Data.linegame_LineCountOri;//初始化線數
                Mod_Data.Bet = Mod_Data.BetOri;//初始化押注
                //彩分高於目前設定壓注率及倍率(正常押注)
                m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");

                if (Mod_Data.credit - Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom >= 0)
                {
                    Mod_Data.credit -= Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom;
                    Mod_Data.betLowCreditShowOnce = false;

                    //遊戲計算
                    m_SlotMediatorController.SendMessage("m_state", "DisableAllLine");//消除餘分線
                    m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");//更新分數
                    m_SlotMediatorController.SendMessage("m_state", "SetReel");//更新分數
                    m_SlotMediatorController.SendMessage("m_state", "CheckBonus");//偵測Bonus
                    m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                    Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//遊戲要儲存加贏分之後的彩分
                    m_SlotMediatorController.SendMessage("m_state", "SaveData");//儲存資料
                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 1);
                    if (!Mod_Data.getBonus) m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");//儲存最大贏分
                    Mod_Data.credit -= Mod_Data.Win * Mod_Data.Denom;//儲存完成後先扣掉贏分

                    /*紀錄帳務資訊*/
                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet) + Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom);
                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet_Class) + Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom);
                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount) + 1);
                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount_Class) + 1);
                    if (Mod_Data.Win > 0 && !Mod_Data.getBonus)
                    {
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                    }
                    //轉換狀態
                    nextState = new BaseScrolling();
                    nextState.setMediator(m_SlotMediatorController);
                    stage = EVENT.EXIT;
                }

                //餘分計算(餘分變化規則 降低押注率odds,押注降至最高且低於彩分之押注率,若押注率=1時仍無法低於彩分,則降低倍率Denom,降低倍率時,押注率升至最高且可負擔制押注率)
                //Ex1.1:1 彩分23分 最低押注25, odd=1,倍率(Denom降至0.5)彩分46分 最低押注25, odd=1,開始遊戲
                //Ex2.1:1 彩分20分  最低押注25, odd=1,倍率(Denom降至0.1)彩分200分, odds提高至可負擔押住率 odds=8 ,押注=200, 開始遊戲

                else if (Mod_Data.credit > 0)
                {
                    Debug.Log("2");
                    double tmpodds = Mod_Data.odds, tmpDenom = Mod_Data.Denom;
                    bool denomLeast = true, oddLeast = true;

                    //降至可負擔之最小倍率,還無法負擔時,降賭注賠率
                    for (int i = (int)Mod_Data.odds; i >= 1; i--)
                    {
                        if (Mod_Data.credit - Mod_Data.Bet * i * Mod_Data.Denom >= 0)
                        {
                            Mod_Data.odds = i;
                            //Debug.Log("Odds" + i);
                            break;
                        }
                    }

                    //Mod_Data.betLowCreditShowOnce 為顯示餘分線之判斷,若odds有更動時 如Ex1,則顯示一次餘分線後開始遊戲
                    if (!Mod_Data.betLowCreditShowOnce && tmpodds != Mod_Data.odds)//彩分低於最低賭注 顯示一次
                    {
                        Debug.Log("2-1");
                        Mod_Data.betLowCreditShowOnce = true;
                        Mod_Data.autoPlay = false;
                        m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                        m_SlotMediatorController.SendMessage("m_state", "EnableAllLine");  //顯示餘分線
                        m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                    }
                    else
                    {
                        for (int i = (int)Mod_Data.odds; i >= 1; i--)
                        {
                            if (Mod_Data.credit - Mod_Data.Bet * i * Mod_Data.Denom >= 0)//降至可負擔之最小倍率,還無法負擔時,降賭注賠率
                            {
                                Debug.Log("3");
                                Mod_Data.odds = i;
                                Mod_Data.credit -= Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom;
                                Mod_Data.betLowCreditShowOnce = false;

                                //遊戲計算
                                m_SlotMediatorController.SendMessage("m_state", "DisableAllLine");//消除餘分線
                                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");//更新分數
                                m_SlotMediatorController.SendMessage("m_state", "SetReel");//更新分數
                                m_SlotMediatorController.SendMessage("m_state", "CheckBonus");//偵測Bonus
                                m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                                Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//遊戲要儲存加贏分之後的彩分
                                m_SlotMediatorController.SendMessage("m_state", "SaveData");//儲存資料
                                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 1);
                                if (!Mod_Data.getBonus) m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");//儲存最大贏分
                                Mod_Data.credit -= Mod_Data.Win * Mod_Data.Denom;//儲存完成後先扣掉贏分

                                /*紀錄帳務資訊*/
                                BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet) + Mod_Data.Bet * i * Mod_Data.Denom);
                                BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet_Class) + Mod_Data.Bet * i * Mod_Data.Denom);
                                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount) + 1);
                                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount_Class) + 1);
                                if (Mod_Data.Win > 0 && !Mod_Data.getBonus)
                                {
                                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                                    BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                                }

                                //轉換狀態
                                nextState = new BaseScrolling();
                                nextState.setMediator(m_SlotMediatorController);
                                oddLeast = false;
                                stage = EVENT.EXIT;
                                break;
                            }
                        }
                    }

                    //若odds無更動時,表示需調整倍率 如Ex2
                    if (oddLeast && (tmpodds == Mod_Data.odds))
                    {
                        Debug.Log("4");
                        Mod_Data.odds = 1;
                        //降至可負擔之最大倍率
                        for (int i = 0; i < Mod_Data.denomArray.Length; i++)
                        {
                            if (Mod_Data.denomOpenArray[i])
                            {
                                if (Mod_Data.credit - Mod_Data.Bet * Mod_Data.odds * Mod_Data.denomArray[i] >= 0)
                                {
                                    Mod_Data.Denom = Mod_Data.denomArray[i];
                                    break;
                                }
                            }
                        }
                        //降至可負擔之最大倍率,押注率升至最高且可負擔之押注率
                        for (int i = (int)Mod_Data.maxOdds; i >= 1; i--)
                        {
                            if (Mod_Data.credit - Mod_Data.Bet * i * Mod_Data.Denom >= 0)
                            {
                                Mod_Data.odds = i;
                                break;
                            }
                        }

                        if (!Mod_Data.betLowCreditShowOnce && tmpDenom != Mod_Data.Denom)//彩分低於最低賭注 顯示一次
                        {
                            Debug.Log("5");
                            Mod_Data.betLowCreditShowOnce = true;
                            Mod_Data.autoPlay = false;
                            m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            m_SlotMediatorController.SendMessage("m_state", "EnableAllLine");
                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                        }
                        else
                        {
                            for (int i = 0; i < Mod_Data.denomArray.Length; i++)
                            {
                                if (Mod_Data.denomOpenArray[i])
                                {
                                    if (Mod_Data.credit - Mod_Data.Bet * Mod_Data.odds * Mod_Data.denomArray[i] >= 0)//降至可負擔之最大倍率
                                    {
                                        Debug.Log("6");
                                        Mod_Data.Denom = Mod_Data.denomArray[i];
                                        Mod_Data.credit -= Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom;
                                        Mod_Data.betLowCreditShowOnce = false;

                                        //遊戲計算
                                        m_SlotMediatorController.SendMessage("m_state", "DisableAllLine");//消除餘分線
                                        m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");//更新分數
                                        m_SlotMediatorController.SendMessage("m_state", "SetReel");//更新分數
                                        m_SlotMediatorController.SendMessage("m_state", "CheckBonus");//偵測Bonus
                                        m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                                        Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//遊戲要儲存加贏分之後的彩分
                                        m_SlotMediatorController.SendMessage("m_state", "SaveData");//儲存資料
                                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 1);
                                        if (!Mod_Data.getBonus) m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");//儲存最大贏分
                                        Mod_Data.credit -= Mod_Data.Win * Mod_Data.Denom;//儲存完成後先扣掉贏分

                                        /*紀錄帳務資訊*/
                                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet) + Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom);
                                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet_Class) + Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom);
                                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount) + 1);
                                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount_Class) + 1);
                                        if (Mod_Data.Win > 0 && !Mod_Data.getBonus)
                                        {
                                            BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                                            BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                                            BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                                            BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                                        }
                                        //轉換狀態
                                        nextState = new BaseScrolling();
                                        nextState.setMediator(m_SlotMediatorController);
                                        denomLeast = false;
                                        stage = EVENT.EXIT;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //若倍率及押注率都最低的情況下還無法負擔時,將押注分數降低至目前彩分
                    if (denomLeast && oddLeast && Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame && (tmpodds == Mod_Data.odds) && (tmpDenom == Mod_Data.Denom))
                    {
                        Debug.Log("7");
                        Mod_Data.odds = 1;
                        for (int i = Mod_Data.denomArray.Length - 1; i >= 0; i--)
                        {
                            if (Mod_Data.denomOpenArray[i])
                            {
                                Mod_Data.Denom = Mod_Data.denomArray[i];
                                //Debug.Log("Denom" + Mod_Data.Denom);
                                break;
                            }
                        }
                        if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 25 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 14)
                        {
                            Mod_Data.Bet = 15;

                        }
                        else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 8 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 15)
                        {
                            Mod_Data.Bet = 9;
                        }
                        else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 2 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 9)
                        {
                            Mod_Data.Bet = 3;
                        }
                        else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) >= 0 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 3)
                        {
                            Mod_Data.Bet = 1;
                        }
                        Debug.Log("Mod_Data.Bet: " + Mod_Data.Bet);
                        m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                        //Mod_Data.Bet = Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom));
                        if (!Mod_Data.betLowCreditShowOnce)//彩分低於最低賭注 顯示一次
                        {
                            Mod_Data.betLowCreditShowOnce = true;
                            m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            m_SlotMediatorController.SendMessage("m_state", "EnableAllLine");
                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");

                        }
                        else
                        {
                            Debug.Log("8");
                            //Mod_Data.odds = 1;
                            //Mod_Data.Bet = Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom));
                            //if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 25 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 14) {
                            //    Mod_Data.Bet = 15;
                            //}
                            //else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 8 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 15) {
                            //    Mod_Data.Bet = 9;
                            //}
                            //else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) > 2 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 9) {
                            //    Mod_Data.Bet = 3;
                            //}
                            //else if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) >= 0 && Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) < 3) {
                            //    Mod_Data.Bet = 1;
                            //}
                            //m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                            if (Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)) == 1)
                            {
                                Mod_Data.credit = 0;
                                Mod_Data.betLowCreditShowOnce = false;
                            }
                            else
                            {
                                Debug.Log("9");
                                double tmp = Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom;
                                Debug.Log("tmp: " + tmp);
                                Debug.Log("9-1 Mod_Data.credit: " + Mod_Data.credit);
                                Mod_Data.credit = (double)(((decimal)Mod_Data.credit) - (decimal)tmp);
                                Debug.Log("9-2 Mod_Data.credit: " + Mod_Data.credit);

                                if (Mod_Data.credit < 0.001)
                                {
                                    Mod_Data.credit = 0;
                                }

                                Mod_Data.betLowCreditShowOnce = false;
                            }

                            //遊戲計算
                            m_SlotMediatorController.SendMessage("m_state", "DisableAllLine");//消除餘分線
                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");//更新分數
                            m_SlotMediatorController.SendMessage("m_state", "SetReel");//更新分數
                            m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                            m_SlotMediatorController.SendMessage("m_state", "CheckBonus");//偵測Bonus
                            m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                            Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//遊戲要儲存加贏分之後的彩分
                            m_SlotMediatorController.SendMessage("m_state", "SaveData");//儲存資料
                            BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 1);
                            if (!Mod_Data.getBonus) m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");//儲存最大贏分
                            Mod_Data.credit -= Mod_Data.Win * Mod_Data.Denom;//儲存完成後先扣掉贏分

                            /*紀錄帳務資訊*/
                            BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet) + Mod_Data.Bet * Mod_Data.Denom);
                            BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalBet_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalBet_Class) + Mod_Data.Bet * Mod_Data.Denom);
                            BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount) + 1);
                            BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.gameCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.gameCount_Class) + 1);
                            if (Mod_Data.Win > 0 && !Mod_Data.getBonus)
                            {
                                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                                BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                                BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                            }

                            nextState = new BaseScrolling();
                            nextState.setMediator(m_SlotMediatorController);
                            oddLeast = false;

                            stage = EVENT.EXIT;
                        }
                    }
                }//餘分計算
                else//如果沒錢
                {
                    Mod_Data.autoPlay = false;
                }
            }

            #region KeyBoard_Control
            // //-----鍵盤控制

            // //// 押注-
            // if (Input.GetKeyDown(KeyCode.R))
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock)
            //     {
            //         Mod_Data.Bet = Mod_Data.BetOri;
            //         if (Mod_Data.odds > 1)
            //         {
            //             Mod_Data.odds--;
            //             //if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //             //else Mod_Data.Win = 0;
            //             m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //             Mod_Data.Win = 0;
            //             m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
            //         }
            //     }
            // }
            // //押注+
            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock)
            //     {
            //         Mod_Data.Bet = Mod_Data.BetOri;
            //         if (Mod_Data.odds < Mod_Data.maxOdds)
            //         {
            //             Mod_Data.odds++;
            //             //if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //             //else Mod_Data.Win = 0;
            //             m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //             Mod_Data.Win = 0;
            //             m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
            //         }

            //     }
            // }
            // //自動
            // if (Input.GetKeyDown(KeyCode.A))
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
            // }
            // //最大押注
            // if (Input.GetKeyDown(KeyCode.S) && !Mod_Data.autoPlay)
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock)
            //     {
            //         Mod_Data.Bet = Mod_Data.BetOri;
            //         Mod_Data.odds = Mod_Data.maxOdds;
            //         //if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //         //else Mod_Data.Win = 0;
            //         m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //         Mod_Data.Win = 0;
            //         m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");

            //     }
            // }
            // //+注
            // if (Input.GetKeyDown(KeyCode.Z) && !Mod_Data.autoPlay)
            // {
            //     //Debug.Log("+D");
            //     Mod_Data.credit += 1000;
            //     m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
            // }
            // //統計資料
            // if (Input.GetKeyDown(KeyCode.O) && !Mod_Data.autoPlay)
            // {
            //     if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
            //     {

            //         Mod_Data.IOLock = true;
            //         if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //         m_SlotMediatorController.SendMessage("m_state", "OpenAccount");
            //     }
            // }
            // //後台
            // if (Input.GetKeyDown(KeyCode.P) && !Mod_Data.autoPlay)
            // {
            //     if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
            //     {
            //         Mod_Data.IOLock = true;
            //         if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
            //         m_SlotMediatorController.SendMessage("m_state", "OpenLogin");
            //     }
            // }
            // //洗分
            // if (Input.GetKeyDown(KeyCode.E) && !Mod_Data.autoPlay)
            // {
            //     m_SlotMediatorController.SendMessage("m_state", "CheckClearPoint");
            //     Mod_Data.Win = 0;
            //     m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
            //     //Debug.Log("clear");
            // }
            // //開分
            // if (Input.GetKeyDown(KeyCode.W) && !Mod_Data.autoPlay)
            // {
            //     if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && Mod_Data.Win <= 0)
            //     {
            //         m_SlotMediatorController.SendMessage("m_state", "OpenPoint");
            //     }
            // }
            #endregion

        }

        hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        Mod_Data.afterBonus = false;
        Mod_Data.inBaseSpin = false;
        if (BillAcceptorSettingData.BillOpenClose)
        {
            BillAcceptorSettingData.BillAcceptorEnable = false;
            BillAcceptorSettingData.GetOrderType = "BillEnableDisable";
        }
        //Debug.Log("BaseSpinExit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
    }
}

#endregion

#region BaseScrolling

//滾輪啟動
public class BaseScrolling : Mod_State
{
    public BaseScrolling() : base()
    {
        stateName = STATE.BaseScrolling;
        // base.Enter();
    }

    float detectButtonDownTime = 0;
    public override void Enter()
    {
        Debug.Log("BaseScrolling In");
        if (BillAcceptorSettingData.BillOpenClose) BillAcceptorSettingData.CheckIsInBaseSpin = false;

        //若不正常啟動時 斷電恢復
        if (Mod_Data.StartNotNormal)
        {
            detectButtonDownTime += Time.deltaTime;
            if (detectButtonDownTime > 2f)
            {
                m_SlotMediatorController.SendMessage("m_state", "SetReel");
                m_SlotMediatorController.SendMessage("m_state", "CheckBonus");
                if (Mod_Data.Bet < 25)
                {
                    m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                }
                m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                detectButtonDownTime = 0;
                base.Enter();
            }
        }
        else
        {
            m_SlotMediatorController.SendMessage("m_state", "StartRunSlots");
            base.Enter();
        }
    }

    public override void Update()
    {
        detectButtonDownTime += Time.deltaTime;

        if (detectButtonDownTime > 0.2f)
        {
            nextState = new BaseEnd();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        Debug.Log("BaseScrolling Exit");
        base.Exit();

    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBaseEnd");
    }
}

#endregion

#region BaseEnd

public class BaseEnd : Mod_State
{
    public BaseEnd() : base()
    {
        stateName = STATE.BaseEnd;
        // base.Enter();
    }

    bool isSpaceOnce = false;
    public override void Enter()
    {
        Debug.Log("BaseEnd In");
        m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        isSpaceOnce = false;
        base.Enter();
    }

    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {  //自動
            Mod_Data.autoPlay = !Mod_Data.autoPlay;
        }
#endif

        if (!isSpaceOnce && hardSpaceButtonDown)
        {
            Mod_Data.BlankClick = false;
            if (!isSpaceOnce) m_SlotMediatorController.SendMessage("m_state", "StopRunSlots");
            isSpaceOnce = true;
        }

        if (Mod_Data.reelAllStop)
        {
            m_SlotMediatorController.SendMessage("m_state", "StopRunSlots");
            m_SlotMediatorController.SendMessage("m_state", "PlayAnimation");
            nextState = new BaseRollScore();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }

        hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        Debug.Log("BaseEnd Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBaseEnd");
    }
}

#endregion

#region BaseRollScore

public class BaseRollScore : Mod_State
{
    public BaseRollScore() : base()
    {
        stateName = STATE.BaseRollScore;
    }

    float timer = 0;
    public override void Enter()
    {
        if (timer <= 0)
        {
            m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        }

        timer += Time.deltaTime;

        if (timer > 0.2f)
        {
            Debug.Log("BaseRollScore IN");

            if (Mod_Data.BT_Rule_Pay > 0)
            {
                nextState = new BaseFishChange_BTRule();
            }
            else
            {
                if (Mod_Data.getBonus)
                {
                    nextState = new BonusTransIn();
                    Mod_Data.getBonus = false;
                }
                else
                {
                    nextState = new BaseSpin();
                    Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//獲得bonus先不加入彩分
                }
            }

            nextState.setMediator(m_SlotMediatorController);

            if (Mod_Data.Pay > 0)
            {
                Mod_Data.runScore = true;
                m_SlotMediatorController.SendMessage("m_state", "StartRollScore");
            }

            timer = 0;
            base.Enter();
        }
    }

    bool stopScore = false;
    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        timer += Time.deltaTime;

        if (Mod_Data.Win > 0)
        {
            if (Mod_Data.runScore)
            {
                if ((hardSpaceButtonDown || Mod_Data.BlankClick))
                {
                    if (timer > Mod_Data.BonusDelayTimes)
                    {
                        Mod_Data.BlankClick = false;
                        m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                        stage = EVENT.EXIT;
                    }
                }
            }
            else
            {
                if (!stopScore)
                {
                    m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                    stopScore = true;
                    stage = EVENT.EXIT;
                }
            }
        }
        else
        {
            stage = EVENT.EXIT;
        }

        hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        Mod_Data.runScore = false;
        Mod_Data.StartNotNormal = false;
        Debug.Log("BaseRollScore Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {

    }
}

#endregion

#region BaseFishChange_BTRule

public class BaseFishChange_BTRule : Mod_State
{
    public BaseFishChange_BTRule() : base()
    {
        stateName = STATE.BaseFishChange_BTRule;
        fishChangeTimer = Mod_Animation_BTRule.fishChangeTIme + 1f;
    }

    float timer = 0;
    float fishChangeTimer = 0;
    public override void Enter()
    {
        if (timer <= 0)
        {
            m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame_BTRule");
            m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        }

        timer += Time.deltaTime;
        if (timer > 1f)
        {
            m_SlotMediatorController.SendMessage("m_state", "FishChangeAnim_BTRule");

            if (timer > fishChangeTimer)
            {
                m_SlotMediatorController.SendMessage("m_state", "CloseFishChangeAnim_BTRule");
                m_SlotMediatorController.SendMessage("m_state", "PlayAnimation_BTRule");
                Debug.Log("BaseFishChange_BTRule IN");
                if (Mod_Data.getBonus)
                {
                    nextState = new BonusTransIn();
                    Mod_Data.getBonus = false;
                }
                else
                {
                    nextState = new BaseSpin();
                    Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;//獲得bonus先不加入彩分
                }

                nextState.setMediator(m_SlotMediatorController);

                if (Mod_Data.BT_Rule_Pay > 0)
                {
                    Mod_Data.runScore = true;
                    m_SlotMediatorController.SendMessage("m_state", "StartRollScore_BTRule");
                    //Debug.Log("RollScoreEnter" + Mod_Data.runScore);
                }

                timer = 0;
                base.Enter();
            }

        }
    }

    bool stopScore = false;
    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        timer += Time.deltaTime;

        if (Mod_Data.Win > 0)
        {
            if (Mod_Data.runScore)
            {
                if ((hardSpaceButtonDown || Mod_Data.BlankClick))
                {
                    if (timer > Mod_Data.BonusDelayTimes)
                    {
                        Mod_Data.BlankClick = false;
                        m_SlotMediatorController.SendMessage("m_state", "StopRollScore_BTRule");
                        stage = EVENT.EXIT;
                    }
                }
            }
            else
            {
                if (!stopScore)
                {
                    m_SlotMediatorController.SendMessage("m_state", "StopRollScore_BTRule");
                    stopScore = true;
                    stage = EVENT.EXIT;
                }
            }
        }
        else
        {
            stage = EVENT.EXIT;
        }

        hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        Mod_Data.runScore = false;
        Mod_Data.StartNotNormal = false;
        Debug.Log("BaseFishChange_BTRule Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {

    }
}

#endregion

#region BonusTransIn

public class BonusTransIn : Mod_State
{
    float timer = 0;
    public BonusTransIn() : base()
    {
        stateName = STATE.BonustransIn;
    }

    public override void Enter()
    {
        if (Mod_Data.StartNotNormal)
        {
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                m_SlotMediatorController.SendMessage("m_state", "PlayBonusTransAnim");
                Mod_Data.BlankClick = false;
                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 2);
                base.Enter();
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > 2f)
            {
                m_SlotMediatorController.SendMessage("m_state", "PlayBonusTransAnim");
                Mod_Data.BlankClick = false;
                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 2);
                base.Enter();
            }
        }
    }

    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        if (Mod_Data.transInAnimEnd)
        {
            // m_SlotMediatorController.SendMessage(this, "ShowChoosePanel");//如果還有可玩場數 就跳選擇畫面
            //
            //Mod_Data.startBonus = true;
            if (timer < 5) timer += Time.deltaTime;

            if (!Mod_Data.BonusSwitch)
            {
                m_SlotMediatorController.SendMessage("m_state", "ShowStartTriggerPanel"); //顯示贏得幾場
                m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame"); //停止所有框線
                Mod_Data.BonusSwitch = true;
                m_SlotMediatorController.SendMessage("m_state", "ChangeScene", 0);//轉換場景至Bonus
            }

            if (timer > 2)
            {
                m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
            }

            if ((hardSpaceButtonDown || Mod_Data.BlankClick) && timer > 3)
            {
                Mod_Data.BlankClick = false;
                m_SlotMediatorController.SendMessage("m_state", "CloseTriggerPanel");
                m_SlotMediatorController.SendMessage("m_state", "BonustransEnd");
                Mod_Data.transInAnimEnd = false;
                // Mod_Data.startBonus = true;
                timer = 0;
            }
        }

        if (Mod_Data.startBonus)
        {
            timer += Time.deltaTime;

            if (timer > 1)
            {
                Mod_Data.BonusIsPlayedCount = 0;
                Mod_Data.startBonus = false;
                nextState = new BonusSpin();
                nextState.setMediator(m_SlotMediatorController);
                Mod_Data.BonusSwitch = true;
                stage = EVENT.EXIT;
            }
        }
        hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        //m_SlotMediatorController.SendMessage(this, "BonustransEnd");
        //Debug.Log("BonusTransExit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        //  //Debug.Log("TestBonusTrans");
    }
}

#endregion

#region BonusSpin

public class BonusSpin : Mod_State
{
    float timer = 0;
    public BonusSpin() : base()
    {
        stateName = STATE.BonusSpin;
    }

    public override void Enter()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            Mod_Data.BonusIsPlayedCount++;
            Mod_Data.StartNotNormal = false;
            base.Enter();
        }
    }

    public override void Update()
    {
        //Debug.Log(Mod_Data.BonusSwitch);
        //Debug.Log("BonusSpinUpdate");
        m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
        m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
        m_SlotMediatorController.SendMessage("m_state", "SetReel");
        if (Mod_Data.Bet < 25)
        {
            m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
        }
        m_SlotMediatorController.SendMessage("m_state", "CheckBonus");
        m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
        m_SlotMediatorController.SendMessage("m_state", "SaveData");
        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.status, 3);

        nextState = new BonusScrolling();
        nextState.setMediator(m_SlotMediatorController);
        stage = EVENT.EXIT;

    }

    public override void Exit()
    {
        //Debug.Log("BonusSpinExit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBonusSpin");
    }
}

#endregion

#region BonusScrolling

public class BonusScrolling : Mod_State
{
    public BonusScrolling() : base()
    {
        stateName = STATE.BonusScrolling;
        // base.Enter();
    }

    float detectButtonDownTime = 0;
    public override void Enter()
    {
        if (Mod_Data.StartNotNormal)
        {
            detectButtonDownTime += Time.deltaTime;

            if (detectButtonDownTime > 2f)
            {
                m_SlotMediatorController.SendMessage("m_state", "SetReel");
                if (Mod_Data.Bet < 25)
                {
                    m_SlotMediatorController.SendMessage("m_state", "StartRemainder");//餘分黑格子
                }
                m_SlotMediatorController.SendMessage("m_state", "CheckBonus");
                m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
                m_SlotMediatorController.SendMessage("m_state", "StartRunSlots");
                m_SlotMediatorController.SendMessage("m_state", "PlayBonusBackGroundSound");
                detectButtonDownTime = 0;
                base.Enter();
            }
        }
        else
        {
            m_SlotMediatorController.SendMessage("m_state", "StartRunSlots");
            base.Enter();
        }
    }

    public override void Update()
    {
        detectButtonDownTime += Time.deltaTime;

        if (detectButtonDownTime > 0.2f)
        {
            nextState = new BonusEnd();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //Debug.Log("BonusRoll Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBaseEnd");
    }
}

#endregion

#region BonusEnd

public class BonusEnd : Mod_State
{
    public BonusEnd() : base()
    {
        stateName = STATE.BonusEnd;
    }

    public override void Enter()
    {
        //Debug.Log("BonusEndEnter");
        // m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        base.Enter();
    }

    public override void Update()
    {
        ////Debug.Log("BonusEndUpdate");
        if (Mod_Data.reelAllStop)
        {
            m_SlotMediatorController.SendMessage("m_state", "StopRunSlots");
            //m_SlotMediatorController.SendMessage("m_state", "CheckBonus");
            //m_SlotMediatorController.SendMessage("m_state", "GameMathCount");
            Mod_Data.BonusDelayTimes = 0;

            //m_SlotMediatorController.SendMessage("m_state", "PlayDKSpecialAnim");
            m_SlotMediatorController.SendMessage("m_state", "PlayAnimation");

            nextState = new BonusRollScore();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }

    }

    public override void Exit()
    {
        //Debug.Log("BonusEndExit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBonusEnd");
    }
}

#endregion

#region BonusRollScore

public class BonusRollScore : Mod_State
{
    public BonusRollScore() : base()
    {
        stateName = STATE.BonusRollScore;
    }

    float timer = 0;
    public override void Enter()
    {
        m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");

        timer += Time.deltaTime;
        if (timer > 0.7)
        {
            if (Mod_Data.BonusIsPlayedCount >= Mod_Data.BonusCount)
            {
                nextState = new BonusTransOut();
                //Debug.Log("A");
            }
            else
            {
                if (Mod_Data.getBonus)
                {
                    nextState = new GetBonusInBonus();
                    Mod_Data.getBonus = false;
                }
                else
                {
                    nextState = new BonusSpin();
                }
            }
            nextState.setMediator(m_SlotMediatorController);
            if (Mod_Data.Pay > 0)
            {
                Mod_Data.runScore = true;
                m_SlotMediatorController.SendMessage("m_state", "StartFastRollScore");
                //Debug.Log("RollScoreEnter" + Mod_Data.runScore);
                //Debug.Log("Mod_Data.Win" + Mod_Data.Win);
            }
            // m_SlotMediatorController.SendMessage("m_state", "SaveData");

            timer = 0;
            base.Enter();
        }
    }

    bool stopScore = false;
    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif
        timer += Time.deltaTime;

        if ((Mod_Data.BonusSwitch && Mod_Data.Pay > 0) || (!Mod_Data.BonusSwitch && Mod_Data.Win > 0))
        {
            if (Mod_Data.runScore)
            {
                if ((hardSpaceButtonDown || Mod_Data.BlankClick))
                {
                    if (timer > Mod_Data.BonusDelayTimes)
                    {
                        Mod_Data.BlankClick = false;
                        m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                        if (Mod_Animation.isPlayingDKSpecialAnim) m_SlotMediatorController.SendMessage("m_state", "StopDKSpecialAnim");
                        stage = EVENT.EXIT;
                    }
                }
            }
            else
            {
                if (!stopScore)
                {
                    m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                    stopScore = true;
                }

                if (timer > Mod_Data.BonusDelayTimes)
                {
                    if (!stopScore) m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                    if (Mod_Animation.isPlayingDKSpecialAnim) m_SlotMediatorController.SendMessage("m_state", "StopDKSpecialAnim");
                    stage = EVENT.EXIT;
                }
            }
        }
        else
        {
            if (timer > Mod_Data.BonusDelayTimes)
            {
                if (Mod_Animation.isPlayingDKSpecialAnim) m_SlotMediatorController.SendMessage("m_state", "StopDKSpecialAnim");
                stage = EVENT.EXIT;
            }
        }
        hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        Mod_Data.runScore = false;
        //Debug.Log("BonusRollScore Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {

    }
}

#endregion

#region BonusTransOut

public class BonusTransOut : Mod_State
{
    public BonusTransOut() : base()
    {
        stateName = STATE.BonusTransOut;
    }

    public override void Enter()
    {
        //  m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        base.Enter();
        timer += Time.deltaTime;
        Mod_Data.transInAnimEnd = true;
        //if (timer > 2)
        //{
        //    m_SlotMediatorController.SendMessage("m_state", "PlayBonusTransOutAnim");
        //    timer = 0;

        //    base.Enter();
        //}



    }
    float timer = 0;
    public override void Update()
    {
        if (Mod_Data.transInAnimEnd)
        {
            m_SlotMediatorController.SendMessage("m_state", "ShowBonusScorePanel");//如果可玩場次結束 就跳分數
            //Mod_Data.BonusSpecialTimes = 1;
            //Mod_Data.BonusCount = 0;
            //m_SlotMediatorController.SendMessage("m_state", "SaveBonusInforamtion");//儲存bonus倍率與場數資訊
            Mod_Data.BonusDelayTimes = 0;
            nextState = new AfterBonusRollScore();
            nextState.setMediator(m_SlotMediatorController);
            Mod_Data.transInAnimEnd = false;
            timer = 10;
        }

        timer -= Time.deltaTime;

        if (timer > 0 && timer < 5)
        {
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        m_SlotMediatorController.SendMessage("m_state", "BonusEndtransOut");
        Mod_Data.BonusSwitch = false;
        m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
        m_SlotMediatorController.SendMessage("m_state", "ChangeScene", 1);
        Mod_Data.afterBonus = true;
        //Debug.Log("BonusTransOutExit");
        base.Exit();
        //m_SlotMediatorController.SendMessage("m_state", "ChangeScene");
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBonusEnd");
    }
}

#endregion

#region AfterBonusRollScore

public class AfterBonusRollScore : Mod_State
{
    public AfterBonusRollScore() : base()
    {
        stateName = STATE.AfterBonusRollScore;
    }

    float timer = 0;
    public override void Enter()
    {
        m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");

        timer += Time.deltaTime;
        if (timer > 0.7)
        {
            m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWinInBonus");
            nextState = new BaseSpin();
            nextState.setMediator(m_SlotMediatorController);
            //Debug.Log("Mod_Data.Win" + Mod_Data.Win);
            Mod_Data.credit += Mod_Data.Win * Mod_Data.Denom;
            if (Mod_Data.Win > 0)
            {
                Mod_Data.runScore = true;
                m_SlotMediatorController.SendMessage("m_state", "StartRollScore");
            }
            base.Enter();
        }
    }

    bool stopScore = false;
    bool saveSrame = false;
    public override void Update()
    {
        InUpdate = true;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardSpaceButtonDown = true;
        }
#endif

        if (Mod_Data.Win > 0)
        {
            if (Mod_Data.runScore)
            {
                if ((hardSpaceButtonDown || Mod_Data.BlankClick))
                {
                    if (timer > Mod_Data.BonusDelayTimes)
                    {
                        saveSrame = true;
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);

                        Mod_Data.BlankClick = false;
                        m_SlotMediatorController.SendMessage("m_state", "StopRollScore");

                        stage = EVENT.EXIT;
                    }
                }
            }
            else
            {
                if (!stopScore)
                {
                    if (!saveSrame)
                    {
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.totalWin_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin_Class) + Mod_Data.Win * Mod_Data.Denom);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount) + 1);
                        BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.winCount_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.winCount_Class) + 1);
                    }
                    m_SlotMediatorController.SendMessage("m_state", "StopRollScore");
                    stopScore = true;
                    stage = EVENT.EXIT;
                }
            }
        }
        else
        {
            stage = EVENT.EXIT;
        }

        Mod_Data.BonusIsPlayedCount = 0;
        Mod_Data.BonusCount = 0;
        hardSpaceButtonDown = false;
    }

    public override void Exit()
    {
        InUpdate = false;
        Mod_Data.BonusSpecialTimes = 1;
        Mod_Data.BonusCount = 0;
        m_SlotMediatorController.SendMessage("m_state", "SaveBonusInforamtion");//儲存bonus倍率與場數資訊
        m_SlotMediatorController.SendMessage("m_state", "SaveData");
        m_SlotMediatorController.SendMessage("m_state", "ComparisonMaxWin");
        Mod_Data.runScore = false;
        base.Exit();
    }

    public override void SpecialOrder()
    {

    }
}

#endregion

#region GetBonusInBonus

public class GetBonusInBonus : Mod_State
{
    public GetBonusInBonus() : base()
    {
        stateName = STATE.GetBonusInBonus;
    }

    float timer = 0;
    public override void Enter()
    {
        //m_SlotMediatorController.SendMessage("m_state", "OpenBlankButton");
        //Debug.Log("BonusInBonusEnter");
        timer = 0;
        base.Enter();
    }

    public override void Update()
    {
        m_SlotMediatorController.SendMessage("m_state", "ShowReTriggerPanel");
        timer += Time.deltaTime;
        if (timer > 1.5f)
        {
            Mod_Data.BlankClick = false;
            m_SlotMediatorController.SendMessage("m_state", "CloseTriggerPanel");
            nextState = new BonusSpin();
            nextState.setMediator(m_SlotMediatorController);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //Debug.Log("BonusInBonus Exit");
        base.Exit();
    }

    public override void SpecialOrder()
    {
        // //Debug.Log("TestBonusEnd");
    }
}

#endregion

#endregion
#endif