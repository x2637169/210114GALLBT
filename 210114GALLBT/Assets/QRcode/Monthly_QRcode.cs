using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CFPGADrv;
using DWORD = System.UInt32;
using System.Text;
using Quixant.LibQxt.LPComponent;
using System.Runtime.InteropServices;

public class Monthly_QRcode : MonoBehaviour
{
    string path = Application.streamingAssetsPath + "/Serial.ini";//路徑

    [DllImport("kernel32")]
    public static extern int GetPrivateProfileString(string section, string key, string deval, StringBuilder stringBuilder, int size, string path);

    [SerializeField] BackEnd_Data backEnd_Data;
    public RawImage m_QRCode;
    string message = "2020,7,28,8,3400,1800,0,0,1846B00983";

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
    string[] time_Text;
    string[] account_Text;
    string serial_Text;
    [SerializeField]
    Text gameText, timeText, serialText;
    [SerializeField] BackEnd_IniFile iniFile;
    void Awake()
    {
        if (path == null) path = Application.streamingAssetsPath + "/Serial.ini";
        Debug.Log(path);
    }

    void OnEnable()
    {

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

        // if (backEnd_Data = null) backEnd_Data = FindObjectOfType<BackEnd_Data>();
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.AccountData, false);
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.AccountData_Class, false);
        ShowQRcode();
    }

    public void ShowQRcode()
    {
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, false);
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.AccountData, false);
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.AccountData_Class, false);
        account_Text = new string[10];
        time_Text = new string[4];

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

        time_Text[0] = datetime[0].ToString();
        time_Text[1] = datetime[1].ToString();
        time_Text[2] = datetime[2].ToString();
        time_Text[3] = datetime[3].ToString();

        //帳務
        account_Text[0] = BackEnd_Data.takeInScore.ToString();
        account_Text[1] = BackEnd_Data.takeOutScore.ToString();
        account_Text[2] = BackEnd_Data.coinIn.ToString();
        account_Text[3] = BackEnd_Data.coinOut.ToString();
        account_Text[4] = BackEnd_Data.totalBet.ToString();
        account_Text[5] = BackEnd_Data.totalWin.ToString();
        account_Text[6] = ((double.IsNaN(BackEnd_Data.winScoreRate) ? 0 : BackEnd_Data.winScoreRate) * 100).ToString("F2");
        account_Text[7] = BackEnd_Data.gameCount.ToString();
        account_Text[8] = BackEnd_Data.winCount.ToString();
        account_Text[9] = ((double.IsNaN(BackEnd_Data.winCountRate) ? 0 : BackEnd_Data.winCountRate) * 100).ToString("F2");

        //機台序號
        serial_Text = ReadIniContent("Serial", "serial");

        string winrate = null;
        for (int i = 8; i >= 0; i--)
        {
            winrate += BackEnd_Data.RTPwinRate[i].ToString();
        }
        //QRCode加密繪製圖形
        Debug.Log(winrate);
        message = time_Text[0] + "," + time_Text[1] + "," + time_Text[2] + "," + time_Text[3] + "," +
                  account_Text[0] + "," + account_Text[1] + "," + account_Text[2] + "," + account_Text[3] + "," +
                  serial_Text + "," + winrate + "," + account_Text[4] + "," + account_Text[5] + "," + account_Text[6] + "," +
                  account_Text[7] + "," + account_Text[8] + "," + account_Text[9];
        Debug.Log(message);
        m_QRCode.texture = QRcode.ShowQRCode("http://ipcuse.azurewebsites.net/QRcode_OpenScore.aspx?psw=" + web.StringEncrypt.aesEncryptBase64(message, "322782").Replace("+", "%2B"), 512, 512);
    }
    //讀取ini檔
    public string ReadIniContent(string section, string key)
    {
        StringBuilder temp = new StringBuilder(255);
        int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
        return temp.ToString();
    }
}
