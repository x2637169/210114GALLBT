using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CFPGADrv;
using DWORD = System.UInt32;
using Quixant.LibQxt.LPComponent;
using System.Runtime.InteropServices;

public class BackEnd_Data : IGameSystem
{
    static NewSramManager newSramManager;
    public Mod_NewLoadGame mod_NewLoadGame;

    #region 不同IPC變數

    #region GS
#if GS

    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS();

#endif
    #endregion

    #region QXT
#if QXT

    [DllImport("libqxt")]
    public static extern bool qxt_rtc_getdate(out sEventRTC inevent, ClockType clockType);
    public enum ClockType : byte
    {
        RealtimeClock = 0x01,
        AlarmClock = 0x04,
        SystemClock = 0x02
    }
    bool result;
    sEventRTC doorEvent;

#endif
    #endregion

    #region AGP
#if AGP

    AGP_Func.RTC_TIME rtc_Time = new AGP_Func.RTC_TIME();

#endif
    #endregion

    #endregion

    DWORD[] datetime = new uint[6];

    public static string loginPassword, eventName, eventTime;//loginPassword登入密碼
    public static bool moneyChannelOn, moneyMachineOn, coinOutBtnOn, coinOutSwitch, RTPOn;//錢道開關 鈔機開關  退幣鍵開關  退幣開關  RTP中獎率開關(是否共通)
    public static int takeInScoreNum, takeOutScoreNum, maxOdds, maxCredit, maxWin, eventdata;//開分鍵(分/次),洗分鍵(分/次),最大押注倍數,最大籌碼,最大贏籌碼,事件資訊(開(洗)幾分)
    public static bool[] denomArray = new bool[9];//籌碼比率  
    public static int[] RTPwinRate = new int[10];//RTP中獎率(2.5~共通)(0~9)
    HistoryData historyData = new HistoryData();

    //---------------AccountData---------------//
    public static int takeInScore, takeOutScore, coinIn, coinOut, gameCount, winCount;
    //開分        //洗分       //投幣   //退幣  //遊戲次數  //贏分次數
    public static double totalBet, totalWin, winScoreRate, winCountRate, ticketIn, ticketOut;
    //押注分數  //贏分   //得分率        //贏分率      //比倍 //比倍贏分數
    //---------------AccountData_Class---------------//
    public static int takeInScore_Class, takeOutScore_Class, coinIn_Class, coinOut_Class, gameCount_Class, winCount_Class;
    //開分                 //洗分              //投幣        //退幣        //遊戲次數        //贏分次數
    public static double totalBet_Class, totalWin_Class, winScoreRate_Class, winCountRate_Class, ticketIn_Class, ticketOut_Class;
    //押注分數            //贏分          //得分率             //贏分率           //比倍      //比倍贏分數
    //---------------AccountData_UI---------------//

    //static SramManager sramManager;
    int log;
    private void Awake()
    {
        newSramManager = GetComponent<NewSramManager>();
        //sramManager = GameObject.Find("GameController").GetComponent<SramManager>();
    }
    private void Start()
    {
        if (!newSramManager.LoadIsSramTrue())
        {
            newSramManager.InitializeSetting();

            #region 獲得IPC時間

            #region GS
#if GS

            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.PIC_ReadRTC(datetime);

#endif
            #endregion

            #region QXT
#if QXT
        
        Mod_Data.Qxt_Device_Init();
        result = qxt_rtc_getdate(out doorEvent, ClockType.RealtimeClock);
        datetime[0] = uint.Parse("20" + doorEvent.year.ToString());
        datetime[1] = doorEvent.month;
        datetime[2] = doorEvent.day;
        datetime[3] = doorEvent.hour;
        datetime[4] = doorEvent.min;
        datetime[5] = doorEvent.sec;
        Debug.Log(datetime[0] + " : " + datetime[1] + " : " + datetime[2] + " : " + datetime[3] + " : " + datetime[4] + " : " + datetime[5]);

#endif
            #endregion

            #region AGP
#if AGP

            AGP_Func.AXGMB_Intr_GetRtc(ref rtc_Time);
            datetime[0] = rtc_Time.year;
            datetime[1] = rtc_Time.month;
            datetime[2] = rtc_Time.day;
            datetime[3] = rtc_Time.hour;
            datetime[4] = rtc_Time.minute;
            datetime[5] = rtc_Time.second;

#endif
            #endregion

            #endregion

            newSramManager.SaveIsSramTrue(true);
            //Debug.Log("DoStart"+ datetime[1]);
        }
        mod_NewLoadGame = GetComponent<Mod_NewLoadGame>();
        SaveLoadData(SramMultiData.denomArraySelect, false);
        Mod_Data.denomOpenArray = denomArray;
        Mod_Data.openclearSet = newSramManager.LoadOpenScoreButtonSet();
    }
    int status = 0;
    int historyIndex = 0;
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    //Debug.Log("historyIndex" + historyIndex);
        //    newSramManager.LoadHistory(historyIndex, out historyData);

        //    //Debug.Log("historyData.RNG" + historyData.RNG[0]+"," + historyData.RNG[1] + "," + historyData.RNG[2] + "," + historyData.RNG[3] + "," + historyData.RNG[4]);
        //    //Debug.Log("historyData.Bonus" + historyData.Bonus);
        //    //Debug.Log("historyData.Credit" + historyData.Credit);
        //    //Debug.Log("historyData.Demon" + historyData.Demon);
        //    //Debug.Log("historyData.Bet" + historyData.Bet);
        //    //Debug.Log("historyData.Odds" + historyData.Odds);
        //    //Debug.Log("historyData.Win" + historyData.Win);
        //    //Debug.Log("historyData.OpenPoint" + historyData.OpenPoint);
        //    //Debug.Log("historyData.ClearPoint" + historyData.ClearPoint);
        //    //Debug.Log("historyData.RTP" + historyData.RTP);
        //    //Debug.Log("historyData.SpecialTime" + historyData.SpecialTime);
        //    //Debug.Log("historyData.BonusSpecialTime" + historyData.BonusSpecialTime);
        //    //Debug.Log("historyData.BonusIsPlayedCount" + historyData.BonusIsPlayedCount);
        //    //Debug.Log("historyData.BonusCount" + historyData.BonusCount);

        //}
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    newSramManager.LoadGameHistoryLog(0, out historyIndex);
        //}
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //   if(historyIndex<=100) historyIndex++;
        //}
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    if (historyIndex >0) historyIndex--;
        //}
    }


    public enum SramMultiData
    {
        loginPassword,//密碼
        eventName,//事件名稱 包含 事件資訊(開(洗)幾分)
        moneyChannelOn,//錢道  目前無功能
        moneyMachineOn,//鈔機開關
        coinOutBtnOn, //退幣鍵開關 無此功能
        coinOutSwitch,  //退幣開關 無此功能
        takeInScoreNum,//開分鍵(分/次)
        takeOutScoreNum,//洗分鍵(分/次)
        maxOdds,//最大押注倍數
        maxCredit,//最大籌碼
        maxWin,//最大贏籌碼
        denomArraySelect,//籌碼比率開關  { 2.5,1,0.5,0.25,0.1,0.05,0.025,0.02,0.01}
        RTPOn,//RTP中獎率開關(共通~2.5)  RTPwinRate RTPOn=bool true=共通開  false =共通關  RTP_Array[i] 0(低) or 1(中) or 2(高) int[10]  
        AccountData, //所有統計資料
        AccountData_Class   //班別統計資料
    }
    public enum SramAccountData
    {
        takeInScore,
        takeOutScore,
        coinIn,
        coinOut,
        ticketIn,
        ticketOut,
        gameCount,
        winCount,
        totalBet,
        totalWin,
        biBet,
        biBetWin,
        takeInScore_Class,
        takeOutScore_Class,
        coinIn_Class,
        coinOut_Class,
        ticketIn_Class,
        ticketOut_Class,
        gameCount_Class,
        winCount_Class,
        totalBet_Class,
        totalWin_Class,
        biBet_Class,
        biBetWin_Class,
        status
    }


    public void SaveLoadData(SramMultiData dataName, bool SaveLoad)
    {         //dataName 變數名稱   SaveLoad true = 存, false = 讀
        if (dataName == SramMultiData.loginPassword)
        {  //密碼
            if (SaveLoad == true)
            {    //存
                newSramManager.SavePassword(int.Parse(loginPassword));
            }
            else
            {      //讀
                loginPassword = newSramManager.LoadPassword() + "";
            }
        }

        if (dataName == SramMultiData.eventName)
        { //事件名稱 包含 事件資訊(開(洗)幾分)
            //if (SaveLoad == true)
            //{  //存
            //    if (eventName == "NoData")
            //    {
            //        GameObject.Find("GameController").GetComponent<GameDataManager>().saveEventRecoredByEventName(0, 0);
            //    }
            //    else if (eventName == "Start")
            //    {
            //        GameObject.Find("GameController").GetComponent<GameDataManager>().saveEventRecoredByEventName(1, 0);
            //    }
            //}
            //else if (eventName == "takeInScore")
            //{
            //    //eventdata = 開分值;
            //    GameObject.Find("GameController").GetComponent<GameDataManager>().saveEventRecoredByEventName(2, eventdata);
            //}
            //else if (eventName == "takeOutScore")
            //{
            //    //eventdata = 洗分值;
            //    GameObject.Find("GameController").GetComponent<GameDataManager>().saveEventRecoredByEventName(3, eventdata);
            //}
            //else if (eventName == "coinIn")
            //{
            //    //eventdata = 入鈔值;
            //    GameObject.Find("GameController").GetComponent<GameDataManager>().saveEventRecoredByEventName(4, eventdata);
            //}
            //else
            //{          //讀
            //    GameObject.Find("GameController").GetComponent<SramManager>().EventRecoredLoad();
            //    //GameDataManager.eventRecoredDataList._EventRecoredData[i].EventCode  //事件碼
            //    //GameDataManager.eventRecoredDataList._EventRecoredData[i].EventData  //事件資料
            //    //GameDataManager.eventRecoredDataList._EventRecoredData[i].EventTime  //事件時間
            //}
        }

        if (dataName == SramMultiData.moneyChannelOn)
        {
            //錢道  目前無功能
        }

        if (dataName == SramMultiData.moneyMachineOn)
        {  //鈔機開關
            if (SaveLoad == true)
            {   //開啟鈔機
                newSramManager.SaveBanknoteMachineSetting(true);
                Mod_Data.UBA_Enable = 1;
                GameObject.Find("GameController").GetComponent<Mod_UBA>().OpenUBA();
            }
            else if (SaveLoad == false)
            {  //關閉鈔機
                newSramManager.SaveBanknoteMachineSetting(false);
                Mod_Data.UBA_Enable = 0;
                GameObject.Find("GameController").GetComponent<Mod_UBA>().CloseUBA();

            }
        }

        if (dataName == SramMultiData.coinOutBtnOn)
        {
            //退幣鍵開關 無此功能
        }

        if (dataName == SramMultiData.coinOutSwitch)
        {
            //退幣開關 無此功能
        }

        if (dataName == SramMultiData.takeInScoreNum)
        {  //開分鍵(分/次)
            if (SaveLoad == true)
            {   //存
                newSramManager.SaveOpenPointSetting(takeInScoreNum);
            }
            else if (SaveLoad == false)
            {   //讀
                takeInScoreNum = newSramManager.LoadOpenPointSetting();
            }
        }

        if (dataName == SramMultiData.takeOutScoreNum)
        {  //洗分鍵(分/次)
            if (SaveLoad == true)
            {  //存
                newSramManager.SaveClearPointSetting(takeOutScoreNum);
                //Debug.Log("BackEnd_Data.newSramManager.LoadClearPointSetting()" + newSramManager.LoadClearPointSetting());
            }
            else if (SaveLoad == false)
            { //讀
                takeOutScoreNum = newSramManager.LoadClearPointSetting();
            }
        }

        if (dataName == SramMultiData.maxOdds)
        {  //最大押注倍數
            if (SaveLoad == true)
            {  //存
                newSramManager.SaveMaxOdd(maxOdds);

            }
            else if (SaveLoad == false)
            {  //讀
                maxOdds = newSramManager.LoadMaxOdd();
            }
        }

        if (dataName == SramMultiData.maxCredit)
        { //最大籌碼
            if (SaveLoad == true)
            {  //存
                newSramManager.SaveMaxCredit(maxCredit);
            }
            else if (SaveLoad == false)
            {  //讀
                maxCredit = newSramManager.LoadMaxCredit();
            }
        }

        if (dataName == SramMultiData.maxWin)
        {  //最大贏籌碼
            if (SaveLoad == true)
            {  //存
                newSramManager.SaveMaxWin(maxWin);
            }
            else if (SaveLoad == false)
            {  //讀
                maxWin = newSramManager.LoadMaxWin();
            }
        }

        if (dataName == SramMultiData.denomArraySelect)
        {  //籌碼比率開關  { 2.5,1,0.5,0.25,0.1,0.05,0.025,0.02,0.01}
            if (SaveLoad == true)
            {  //存顯示比率
                newSramManager.SaveDenomOption(denomArray);
            }
            if (SaveLoad == false)
            { //讀顯示比率
                newSramManager.LoadDenomOption(out denomArray);
            }
        }

        if (dataName == SramMultiData.RTPOn)
        {  //RTP中獎率開關(共通~2.5)  RTPwinRate RTPOn=bool true=共通開  false =共通關  RTP_Array[i] 0(低) or 1(中) or 2(高) int[10]  
            if (SaveLoad == true)
            {  //存
                newSramManager.SaveRTPSetting(RTPwinRate, false);
            }
            else if (SaveLoad == false)
            {  //讀
                newSramManager.LoadRTPSetting(out RTPwinRate, out RTPOn);
            }
        }

        if (dataName == SramMultiData.AccountData)
        {  //所有統計資料
            if (SaveLoad == true)
            {  //存
                //GameDataManager._AllStatisticalData.OpenPointTotal = takeInScore;
                //GameDataManager._AllStatisticalData.ClearPointTotal = takeOutScore;
                //GameDataManager._AllStatisticalData.CashIn = coinIn;
                //GameDataManager._AllStatisticalData.CashOut = coinOut;
                //GameDataManager._AllStatisticalData.GameCount = gameCount;
                //GameDataManager._AllStatisticalData.WinCount = winCount;
                //GameDataManager._AllStatisticalData.BetCredit = totalBet;
                //GameDataManager._AllStatisticalData.WinScore = totalWin;
                //GameObject.Find("GameController").GetComponent<GameDataManager>().ScoringRateUpdate();
                //GameObject.Find("GameController").GetComponent<GameDataManager>().WinRateUpdate();
                //GameDataManager._AllStatisticalData.RatioScore = biBet;
                //GameDataManager._AllStatisticalData.RatioWinScore = biBetWin;
                //GameObject.Find("GameController").GetComponent<SramManager>().AllStatistcalDataSave();


            }
            if (SaveLoad == false)
            {  //讀
                takeInScore = newSramManager.LoadTotalOpenPoint();
                takeOutScore = newSramManager.LoadTotalClearPoint();
                coinIn = newSramManager.LoadTotalCoinIn();
                // coinOut = (int)GameDataManager._AllStatisticalData.CashOut;
                gameCount = newSramManager.LoadTotalGamePlay();
                winCount = newSramManager.LoadTotalWinGamePlay();
                totalBet = newSramManager.LoadTotalBet();
                totalWin = newSramManager.LoadTotalWin();
                winScoreRate = newSramManager.LoadTotalWinScoreRate();
                winCountRate = newSramManager.LoadTotalWinCountRate();
                ticketIn = newSramManager.LoadTotalTicketIn();
                ticketOut = newSramManager.LoadTotalTicketOut();
            }
        }

        if (dataName == SramMultiData.AccountData_Class)
        {  //班別統計資料
            if (SaveLoad == true)
            {  //存
                //GameDataManager._ClassStatisticsData.OpenPointTotal = takeInScore_Class;
                //GameDataManager._ClassStatisticsData.ClearPointTotal = takeOutScore_Class;
                //GameDataManager._ClassStatisticsData.CashIn = coinIn_Class;
                //GameDataManager._ClassStatisticsData.CashOut = coinOut_Class;
                //GameDataManager._ClassStatisticsData.GameCount = gameCount_Class;
                //GameDataManager._ClassStatisticsData.WinCount = winCount_Class;
                //GameDataManager._ClassStatisticsData.BetCredit = totalBet_Class;
                //GameDataManager._ClassStatisticsData.WinScore = totalWin_Class;
                //GameObject.Find("GameController").GetComponent<GameDataManager>().ScoringRateUpdate();
                //GameObject.Find("GameController").GetComponent<GameDataManager>().WinRateUpdate();
                //GameDataManager._ClassStatisticsData.RatioScore = biBet_Class;
                //GameDataManager._ClassStatisticsData.RatioWinScore = biBetWin_Class;
                //GameObject.Find("GameController").GetComponent<SramManager>().ClassStatistcalDataSave();
            }
            else if (SaveLoad == false)
            {  //讀
               // GameObject.Find("GameController").GetComponent<SramManager>().ClassStatistcalDataLoad();
                takeInScore_Class = newSramManager.LoadClassOpenPoint();
                takeOutScore_Class = newSramManager.LoadClassClearPoint();
                coinIn_Class = newSramManager.LoadClassCoinIn();
                // coinOut_Class = (int)GameDataManager._ClassStatisticsData.CashOut;
                gameCount_Class = newSramManager.LoadClassGamePlay();
                winCount_Class = newSramManager.LoadClassWinGamePlay();
                totalBet_Class = newSramManager.LoadClassBet();
                totalWin_Class = newSramManager.LoadClassWin();

                winScoreRate_Class = newSramManager.LoadClassWinScoreRate();

                winCountRate_Class = newSramManager.LoadClassWinCountRate();
                ticketIn_Class = newSramManager.LoadClassTicketIn();
                ticketOut_Class = newSramManager.LoadClassTicketOut();

                //biBet_Class = GameDataManager._ClassStatisticsData.RatioScore;
                //biBetWin_Class = GameDataManager._ClassStatisticsData.RatioWinScore;
            }
        }

    }

    public static void SetInt(SramAccountData accountData, int value)
    {
        switch (accountData)
        {
            case SramAccountData.takeInScore:
                newSramManager.SaveTotalOpenPoint(value);
                break;
            case SramAccountData.takeOutScore:
                newSramManager.SaveTotalClearPoint(value);
                break;
            case SramAccountData.coinIn:
                newSramManager.SaveTotalCoinIn(value);
                break;
            case SramAccountData.coinOut:
                // GameDataManager._AllStatisticalData.CashOut = value;
                break;
            case SramAccountData.gameCount:
                newSramManager.SaveTotalGamePlay(value);
                break;
            case SramAccountData.winCount:
                newSramManager.SaveTotalWinGamePlay(value);
                break;

            case SramAccountData.takeInScore_Class:
                newSramManager.SaveClassOpenPoint(value);
                break;
            case SramAccountData.takeOutScore_Class:
                newSramManager.SaveClassClearPoint(value);
                break;
            case SramAccountData.coinIn_Class:
                newSramManager.SaveClassCoinIn(value);
                break;
            case SramAccountData.coinOut_Class:
                //GameDataManager._ClassStatisticsData.CashOut = value;
                break;
            case SramAccountData.gameCount_Class:
                newSramManager.SaveClassGamePlay(value);
                break;
            case SramAccountData.winCount_Class:
                newSramManager.SaveClassWinGamePlay(value);
                break;
            case SramAccountData.status:
                newSramManager.SaveStatus(value);
                break;
            default:
                break;
        }
    }
    public static void SetDouble(SramAccountData accountData, double value)
    {

        switch (accountData)
        {
            case SramAccountData.totalBet:
                newSramManager.SaveTotalBet(value);
                break;
            case SramAccountData.totalWin:
                newSramManager.SaveTotalWin(value);
                break;
            case SramAccountData.biBet:
                break;
            case SramAccountData.biBetWin:
                break;

            case SramAccountData.totalBet_Class:
                newSramManager.SaveClassBet(value);
                break;
            case SramAccountData.totalWin_Class:
                newSramManager.SaveClassWin(value);
                break;
            case SramAccountData.biBet_Class:
                break;
            case SramAccountData.biBetWin_Class:
                break;
            case SramAccountData.ticketIn:
                newSramManager.SaveTotalTicketIn(value);
                break;
            case SramAccountData.ticketOut:
                newSramManager.SaveTotalTicketOut(value);
                break;
            case SramAccountData.ticketIn_Class:
                newSramManager.SaveClassTicketIn(value);
                break;
            case SramAccountData.ticketOut_Class:
                newSramManager.SaveClassTicketOut(value);
                break;

            default:
                break;
        }
        //sramManager.AllStatistcalDataSave();
        //sramManager.ClassStatistcalDataSave();
    }
    public static int GetInt(SramAccountData accountData)
    {
        switch (accountData)
        {

            case SramAccountData.takeInScore://開分
                return newSramManager.LoadTotalOpenPoint();
                break;
            case SramAccountData.takeOutScore://洗分
                return newSramManager.LoadTotalClearPoint();
                break;
            case SramAccountData.coinIn://入鈔
                return newSramManager.LoadTotalCoinIn();
                break;
            //case SramAccountData.coinOut:
            //    //return (int)GameDataManager._AllStatisticalData.CashOut;
            //    break;
            case SramAccountData.gameCount://遊戲場次
                return newSramManager.LoadTotalGamePlay();
                break;
            case SramAccountData.winCount://贏場次
                return newSramManager.LoadTotalWinGamePlay();
                break;

            case SramAccountData.takeInScore_Class://班開分
                return newSramManager.LoadClassOpenPoint();
                break;
            case SramAccountData.takeOutScore_Class://班洗分
                return newSramManager.LoadClassClearPoint();
                break;
            case SramAccountData.coinIn_Class://班入鈔
                return newSramManager.LoadClassCoinIn();
                break;
            //case SramAccountData.coinOut_Class:
            //    //return (int)GameDataManager._ClassStatisticsData.CashOut;
            //    break;
            case SramAccountData.gameCount_Class://班遊戲場次
                return newSramManager.LoadClassGamePlay();
                break;
            case SramAccountData.winCount_Class://班贏場次
                return newSramManager.LoadClassWinGamePlay();
                break;

            default:
                return 0;
                break;
        }
    }
    public static double GetDouble(SramAccountData accountData)
    {
        //sramManager.AllStatistcalDataLoad();
        //sramManager.ClassStatistcalDataLoad();
        switch (accountData)
        {
            case SramAccountData.totalBet://總押注
                return newSramManager.LoadTotalBet();
                break;
            case SramAccountData.totalWin://總贏分
                return newSramManager.LoadTotalWin();
                break;


            case SramAccountData.totalBet_Class://班總押注

                return newSramManager.LoadClassBet();
                //return totalBet;
                break;
            case SramAccountData.totalWin_Class://班總贏分
                return newSramManager.LoadClassWin();
                break;
            case SramAccountData.ticketIn:
                return newSramManager.LoadTotalTicketIn();
                break;
            case SramAccountData.ticketOut:
                return newSramManager.LoadTotalTicketOut();
                break;
            case SramAccountData.ticketIn_Class:
                return newSramManager.LoadClassTicketIn();
                break;
            case SramAccountData.ticketOut_Class:
                return newSramManager.LoadClassTicketOut();
                break;

            default:
                return 0;
                break;
        }
    }

    int sramMonth;
    public void SetUserType(bool outsideUser)
    {
        newSramManager.SaveIsMonthCheck(outsideUser);
        Mod_Data.hasMonthCheck = outsideUser;

        #region 獲得IPC時間

        #region GS
#if GS

        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.PIC_ReadRTC(datetime);

#endif
        #endregion

        #region QXT
#if QXT
        
        Mod_Data.Qxt_Device_Init();
        result = qxt_rtc_getdate(out doorEvent, ClockType.RealtimeClock);
        datetime[0] = uint.Parse("20" + doorEvent.year.ToString());
        datetime[1] = doorEvent.month;
        datetime[2] = doorEvent.day;
        datetime[3] = doorEvent.hour;
        datetime[4] = doorEvent.min;
        datetime[5] = doorEvent.sec;
        Debug.Log(datetime[0] + " : " + datetime[1] + " : " + datetime[2] + " : " + datetime[3] + " : " + datetime[4] + " : " + datetime[5]);

#endif
        #endregion

        #region AGP
#if AGP

        AGP_Func.AXGMB_Intr_GetRtc(ref rtc_Time);
        datetime[0] = rtc_Time.year;
        datetime[1] = rtc_Time.month;
        datetime[2] = rtc_Time.day;
        datetime[3] = rtc_Time.hour;
        datetime[4] = rtc_Time.minute;
        datetime[5] = rtc_Time.second;

#endif
        #endregion

        #endregion

        sramMonth = newSramManager.LoadMonthCheckData();
        Mod_Data.hasMonthCheck = newSramManager.LoadIsMonthCheck();
        if (Mod_Data.hasMonthCheck)
        {
            if (datetime[1] == 12 && sramMonth == 12 || sramMonth == 1)
            {
                Mod_Data.monthLock = false;
            }
            else if (sramMonth - datetime[1] >= 0)
            {
                Mod_Data.monthLock = false;
            }
            else
            {
                Mod_Data.monthLock = true;
            }
        }
        else
        {
            Mod_Data.monthLock = false;
        }
    }

    public void SetOpenClearButton(bool isOpen)
    {
        newSramManager.SaveOpenScoreButtonSet(isOpen);
        Mod_Data.openclearSet = isOpen;
    }

    public void SaveHistoryData()
    {
        historyData.RNG = Mod_Data.RNG;
        historyData.Bonus = Mod_Data.BonusSwitch;
        historyData.Credit = Mod_Data.credit;
        historyData.Demon = (int)(Mod_Data.Denom * 1000);
        historyData.Bet = Mod_Data.Bet;
        historyData.Odds = (int)Mod_Data.odds;
        historyData.Win = Mod_Data.Win;
        historyData.OpenPoint = newSramManager.LoadOpenClearPoint(true);
        historyData.ClearPoint = newSramManager.LoadOpenClearPoint(false);
        historyData.RTP = Mod_Data.RTPsetting;
        historyData.SpecialTime = Mod_Data.BonusSpecialTimes;
        historyData.BonusSpecialTime = Mod_Data.CH_BonusSpecialTime;
        historyData.BonusIsPlayedCount = Mod_Data.BonusIsPlayedCount;
        historyData.BonusCount = Mod_Data.BonusCount;
        historyData.Time = DateTime.Now;
        newSramManager.SaveHistory(historyData);
    }


    int searchIndex = 0;
    int lowestNumber = 0;
    double lowestWin = 0;
    public void CompareWinScore()
    {
        newSramManager.LoadGameHistoryLog(1, out searchIndex);
        if (Mod_Data.Win > 0)
        {
            if (searchIndex == 0)
            {
                saveHighScoreWithDenomHistoryData(1);
            }
            else if (searchIndex == 10)
            {
                lowestWin = Mod_Data.Win * Mod_Data.Denom * 1000;
                lowestNumber = 0;
                for (int i = 1; i <= 10; i++)
                {
                    newSramManager.LoadMaxGameHistory(i, out historyData);
                    if (lowestWin > historyData.Win * historyData.Demon)
                    {
                        lowestWin = historyData.Win * historyData.Demon;
                        lowestNumber = i;
                    }
                }
                if (lowestNumber != 0) saveHighScoreWithDenomHistoryData(lowestNumber);
            }
            else
            {
                saveHighScoreWithDenomHistoryData(searchIndex + 1);
                //Debug.Log("saveHighScoreWithDenomHistoryData"+searchIndex);
            }
        }


        newSramManager.LoadGameHistoryLog(2, out searchIndex);
        //Debug.Log("searchIndex" + searchIndex);
        if (Mod_Data.Win > 0)
        {
            if (searchIndex == 0)
            {
                saveHighScoreHistoryData(1);
            }
            else if (searchIndex == 10)
            {
                lowestWin = Mod_Data.Win;
                lowestNumber = 0;
                for (int i = 1; i <= 10; i++)
                {
                    newSramManager.LoadMaxGameHistory(i, out historyData);
                    if (lowestWin > historyData.Win)
                    {
                        lowestWin = historyData.Win;
                        lowestNumber = i;
                    }
                }
                if (lowestNumber != 0) saveHighScoreHistoryData(lowestNumber);
            }
            else
            {
                saveHighScoreHistoryData(searchIndex + 1);
                //Debug.Log("saveHighScoreHistoryData" + searchIndex);
            }
        }
        //Debug.Log("searchIndex" + searchIndex);

    }
    void saveHighScoreHistoryData(int index)
    {
        historyData.RNG = Mod_Data.RNG;
        historyData.Bonus = Mod_Data.BonusSwitch;
        historyData.Credit = Mod_Data.credit;
        historyData.Demon = (int)(Mod_Data.Denom * 1000);
        historyData.Bet = Mod_Data.Bet;
        historyData.Odds = (int)Mod_Data.odds;
        historyData.Win = Mod_Data.Win;
        historyData.OpenPoint = newSramManager.LoadOpenClearPoint(true);
        historyData.ClearPoint = newSramManager.LoadOpenClearPoint(false);
        historyData.RTP = Mod_Data.RTPsetting;
        historyData.SpecialTime = Mod_Data.BonusSpecialTimes;
        historyData.BonusSpecialTime = Mod_Data.CH_BonusSpecialTime;
        historyData.BonusIsPlayedCount = Mod_Data.BonusIsPlayedCount;
        historyData.BonusCount = Mod_Data.BonusCount;
        historyData.Time = DateTime.Now;
        newSramManager.SaveMaxGameHistorySecond(index, historyData);
    }
    void saveHighScoreWithDenomHistoryData(int index)
    {
        historyData.RNG = Mod_Data.RNG;
        historyData.Bonus = Mod_Data.BonusSwitch;
        historyData.Credit = Mod_Data.credit;
        historyData.Demon = (int)(Mod_Data.Denom * 1000);
        historyData.Bet = Mod_Data.Bet;
        historyData.Odds = (int)Mod_Data.odds;
        historyData.Win = Mod_Data.Win;
        historyData.OpenPoint = newSramManager.LoadOpenClearPoint(true);
        historyData.ClearPoint = newSramManager.LoadOpenClearPoint(false);
        historyData.RTP = Mod_Data.RTPsetting;
        historyData.SpecialTime = Mod_Data.BonusSpecialTimes;
        historyData.BonusSpecialTime = Mod_Data.CH_BonusSpecialTime;
        historyData.BonusIsPlayedCount = Mod_Data.BonusIsPlayedCount;
        historyData.BonusCount = Mod_Data.BonusCount;
        historyData.Time = DateTime.Now;
        newSramManager.SaveMaxGameHistory(index, historyData);
    }
}
