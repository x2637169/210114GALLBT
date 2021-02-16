using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using CFPGADrv;
using BYTE = System.Byte;
using DWORD = System.UInt32;
using Quixant.LibQxt.LPComponent;
using System.Runtime.InteropServices;

public class BackEnd_SetAccount : MonoBehaviour
{
    [SerializeField] NewSramManager newSramManager;
    ////---------------AccountData---------------//
    //int takeInScore, takeOutScore, coinIn, coinOut, gameCount, winCount;
    ////開分        //洗分       //投幣   //退幣  //遊戲次數  //贏分次數
    //double totalBet, totalWin, winScoreRate, winCountRate, biBet, biBetWin;
    ////押注分數  //贏分   //得分率        //贏分率      //比倍 //比倍贏分數
    ////---------------AccountData_Class---------------//
    //int takeInScore_Class, takeOutScore_Class, coinIn_Class, coinOut_Class, gameCount_Class, winCount_Class;
    ////開分                 //洗分              //投幣        //退幣        //遊戲次數        //贏分次數
    //double totalBet_Class, totalWin_Class, winScoreRate_Class, winCountRate_Class, biBet_Class, biBetWin_Class;
    ////押注分數            //贏分          //得分率             //贏分率           //比倍      //比倍贏分數

    //---------------AccountData_UI---------------//
    [SerializeField] BackEnd_Data backEnd_Data;
    [SerializeField]
    Text text_takeInScore, text_takeOutScore, text_coinIn, text_coinOut, text_totalBet, text_totalWin,
        text_winScoreRate, text_gameCount, text_winCount, text_winCountRate, text_TicketIn, text_TicketOut;
    [SerializeField]
    Text text_takeInScore_Class, text_takeOutScore_Class, text_coinIn_Class, text_coinOut_Class, text_totalBet_Class, text_totalWin_Class,
        text_winScoreRate_Class, text_gameCount_Class, text_winCount_Class, text_winCountRate_Class, text_TicketIn_Class, text_TicketOut_Class;
    [SerializeField]
    Text gameText, timeText, serialText;
    int a = 467, b = 1775;
    public GameObject StatisticalDataWindows;

    #region IPC變數

    #region GS變數
#if GS

    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS();
    byte DataByte = 1;

#endif
    #endregion

    #region QXT變數
#if QXT

    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readdword(UInt32 offset);
    volatile System.UInt32 read_long_result1, read_long_result2, read_long_result3, read_long_result_stored1 = 0, read_long_result_stored2 = 0, read_long_result_stored3 = 0;
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
    public string button_bit;

#endif
    #endregion

    #region AGP變數
#if AGP

    AGP_Func.RTC_TIME rtc_Time = new AGP_Func.RTC_TIME();

#endif
    #endregion

    #endregion

    float ClassChangeHoldTime = 0;
    DWORD[] datetime = new uint[6];
    [SerializeField] BackEnd_IniFile iniFile;
    //datatime[0]: Year
    //datetime[1]: Month
    //datetime[2]: Date
    //datetime[3]: Hour
    //datetime[4]: Minute
    //datetime[5]: Second

    void Update()
    {
        if (StatisticalDataWindows.activeInHierarchy)
        {
            ClassChangeButton();
            reFreshData();
        }
    }

    private void OnEnable()
    {
        reFreshData();
        //Debug.Log("refresh");
        newSramManager.DebugSram();

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

        timeText.text = datetime[0] + "/" + datetime[1] + "/" + datetime[2] + "  " + datetime[3] + ":" + datetime[4] + ":" + datetime[5];
        serialText.text = iniFile.ReadIniContent("Serial", "serial");
        gameText.text = Mod_Data.projectName;
    }
    public void reFreshData()
    {
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.AccountData, false);
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.AccountData_Class, false);
        text_takeInScore.text = BackEnd_Data.takeInScore.ToString();
        text_takeOutScore.text = BackEnd_Data.takeOutScore.ToString();
        text_coinIn.text = BackEnd_Data.coinIn.ToString();
        text_coinOut.text = BackEnd_Data.coinOut.ToString();
        text_totalBet.text = BackEnd_Data.totalBet.ToString();
        text_totalWin.text = BackEnd_Data.totalWin.ToString();

        if (BackEnd_Data.totalBet == 0) text_winScoreRate.text = "0%";
        else text_winScoreRate.text = Mathf.CeilToInt((float)(BackEnd_Data.totalWin / BackEnd_Data.totalBet) * 100) + "%";// BackEnd_Data.winScoreRate.ToString();

        text_gameCount.text = BackEnd_Data.gameCount.ToString();
        text_winCount.text = BackEnd_Data.winCount.ToString();

        if (BackEnd_Data.gameCount == 0) text_winCountRate.text = "0%";
        else text_winCountRate.text = Mathf.CeilToInt((float)((float)BackEnd_Data.winCount / (float)BackEnd_Data.gameCount) * 100) + "%";// BackEnd_Data.winCountRate.ToString();

        text_TicketIn.text = BackEnd_Data.ticketIn.ToString();
        text_TicketOut.text = BackEnd_Data.ticketOut.ToString();

        text_takeInScore_Class.text = BackEnd_Data.takeInScore_Class.ToString();
        text_takeOutScore_Class.text = BackEnd_Data.takeOutScore_Class.ToString();
        text_coinIn_Class.text = BackEnd_Data.coinIn_Class.ToString();
        text_coinOut_Class.text = BackEnd_Data.coinOut_Class.ToString();
        text_totalBet_Class.text = BackEnd_Data.totalBet_Class.ToString();
        text_totalWin_Class.text = BackEnd_Data.totalWin_Class.ToString();

        if (BackEnd_Data.totalBet_Class == 0) text_winScoreRate_Class.text = "0%";// BackEnd_Data.winScoreRate.ToString();
        else text_winScoreRate_Class.text = Mathf.CeilToInt((float)(BackEnd_Data.totalWin_Class / BackEnd_Data.totalBet_Class) * 100) + "%";

        text_gameCount_Class.text = BackEnd_Data.gameCount_Class.ToString();
        text_winCount_Class.text = BackEnd_Data.winCount_Class.ToString();

        if (BackEnd_Data.gameCount_Class == 0) text_winCountRate_Class.text = "0%";
        else text_winCountRate_Class.text = Mathf.CeilToInt((float)((float)BackEnd_Data.winCount_Class / (float)BackEnd_Data.gameCount_Class) * 100) + "%";//BackEnd_Data.winCountRate_Class.ToString();

        text_TicketIn_Class.text = BackEnd_Data.ticketIn_Class.ToString();
        text_TicketOut_Class.text = BackEnd_Data.ticketOut_Class.ToString();
    }

    #region 清除本班紀錄

    public void ClassChangeButton()
    {
        #region GS按鈕
#if GS

        byte dataByte7 = 1;
        byte dataByte2 = 1;

        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)7, ref dataByte7);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)2, ref dataByte2);

        if (dataByte7 == 0 && dataByte2 == 0)
        {
            ClassChangeHoldTime += Time.deltaTime;
        }
        else if (dataByte7 != 0 || dataByte2 != 0)
        {
            ClassChangeHoldTime = 0;
        }

        if (ClassChangeHoldTime > 6)
        {
            GameObject.Find("BackEndManager").GetComponent<NewSramManager>().InitializeClaseAccount();
            ClassChangeHoldTime = 0;
        }

#endif
        #endregion

        #region QXT按鈕
#if QXT

        read_long_result1 = qxt_dio_readdword(0);  //讀取輸入訊號
        button_bit = Convert.ToString(read_long_result1, 2).PadLeft(32, '0');
        if (button_bit.Length >= 32)
        {
            if (button_bit[8] == '0' || button_bit[6] == '0')
            {
                ClassChangeHoldTime += Time.deltaTime;
            }
            else
            {
                ClassChangeHoldTime = 0;
            }

            if (ClassChangeHoldTime > 6)
            {
                GameObject.Find("BackEndManager").GetComponent<NewSramManager>().InitializeClaseAccount();
                ClassChangeHoldTime = 0;
            }
        }

#endif
        #endregion

        #region AGP按鈕
#if AGP

        byte dataByte7 = 1;
        byte dataByte4 = 1;

        AGP_Func.AXGMB_DIO_DiReadBit((BYTE)7, ref dataByte7);
        AGP_Func.AXGMB_DIO_DiReadBit((BYTE)4, ref dataByte4);

        if (dataByte7 == 0 && dataByte4 == 0)
        {
            ClassChangeHoldTime += Time.deltaTime;
        }
        else if (dataByte7 != 0 || dataByte4 != 0)
        {
            ClassChangeHoldTime = 0;
        }

        if (ClassChangeHoldTime >= 3)
        {
            GameObject.Find("BackEndManager").GetComponent<NewSramManager>().InitializeClaseAccount();
            ClassChangeHoldTime = 0;
        }

#endif
        #endregion
    }

    #endregion
}