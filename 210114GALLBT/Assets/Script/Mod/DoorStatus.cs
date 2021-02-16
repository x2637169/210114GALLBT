using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quixant.LibQxt.LPComponent;
using System.Runtime.InteropServices;
using System;
using DWORD = System.UInt32;
using CFPGADrv;
using System.Text;

public class DoorStatus : MonoBehaviour
{
#if Server
    #region Server
    Mod_TextController Mod_TextController;
    NewSramManager newSramManager;

#if QXT
    [DllImport("libqxt")]
    public static extern byte qxt_int_getevent(ref sEventRTC inevent);  //抓開啟上下門的訊號
    [DllImport("libqxt")]
    public static extern byte qxt_int_readevents();  //抓開啟上下門的訊號
    public static bool DoorError = false;
    sEventRTC doorEvent;
    byte result;
    int readEventsNum = 0;
#else
    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS(); //賽菲硬體初始化
    DWORD Door = 255; //初始數值
    string doorStatus = null;
    uint tmpNum = 0; //紀錄數字
    bool[] lastDoorStatue;
    string doorInterput = "0";

    MyIni ini;
    string path = Application.streamingAssetsPath + "/Serial.ini";//路徑
    [DllImport("kernel32")]
    public static extern int GetPrivateProfileString(string section, string key, string deval, StringBuilder stringBuilder, int size, string path);
#endif


    void Awake()
    {
#if QXT
        newSramManager = FindObjectOfType<NewSramManager>();
        Mod_TextController = FindObjectOfType<Mod_TextController>();
#endif

#if GS
        Mod_TextController = FindObjectOfType<Mod_TextController>();
        if (path == null) path = Application.streamingAssetsPath + "/Serial.ini";
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
#if QXT
        Mod_Data.Qxt_Device_Init();
        FirstReadEvents();
        StartCoroutine("ReadEvents");
#endif

#if GS
        ini = new MyIni(Application.streamingAssetsPath + "/Door.ini");
        doorInterput = ini.ReadIniContent("Door", "Interput");
        lastDoorStatue = new bool[4];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FpgaPic_Init(); //賽菲
        StartCoroutine("DoorSwitch");
#endif
    }

    #region QXT
#if QXT
    public void FirstReadEvents()
    {
        bool error = false;
        //讀取IPC記憶體的機門狀態
        newSramManager.LoadLogicDoorStatus(out error);
        if (error) Mod_TextController.allTextBool[0] = true; //設置true(顯示)、false(不顯示)控制是否UI顯示錯誤訊息

        newSramManager.LoadMainDoorStatus(out error);
        if (error) Mod_TextController.allTextBool[1] = true;

        newSramManager.LoadBellyDoorStatus(out error);
        if (error) Mod_TextController.allTextBool[2] = true;

        newSramManager.LoadCashDoorStatus(out error);
        if (error) Mod_TextController.allTextBool[3] = true;


        //讀取IPC機門事件覆蓋記憶體機門狀態，以機門事件為優修
        readEventsNum = qxt_int_readevents();
        if (readEventsNum > 0)
        {
            while (readEventsNum > 0)
            {
                readEventsNum--;
                result = qxt_int_getevent(ref doorEvent);
                switch (doorEvent.eventcode)
                {
                    case 1:
                        newSramManager.SaveLogicDoorStatus(false); //儲存機門狀態False為關門狀態、True為開門狀態
                        Mod_TextController.allTextBool[0] = false;
                        break;
                    case 3:
                        newSramManager.SaveMainDoorStatus(false); //儲存機門狀態False為關門狀態、True為開門狀態
                        Mod_TextController.allTextBool[1] = false;
                        break;
                    case 4:
                        newSramManager.SaveBellyDoorStatus(false);
                        Mod_TextController.allTextBool[2] = false;
                        break;
                    case 5:
                        newSramManager.SaveCashDoorStatus(false);
                        Mod_TextController.allTextBool[3] = false;
                        break;
                    case 9:
                        newSramManager.SaveLogicDoorStatus(true);
                        Mod_TextController.allTextBool[0] = true;
                        break;
                    case 11:
                        newSramManager.SaveMainDoorStatus(true);
                        Mod_TextController.allTextBool[1] = true;
                        break;
                    case 12:
                        newSramManager.SaveBellyDoorStatus(true);
                        Mod_TextController.allTextBool[2] = true;
                        break;
                    case 13:
                        newSramManager.SaveCashDoorStatus(true);
                        Mod_TextController.allTextBool[3] = true;
                        break;
                }
            }
        }

        if (Mod_TextController.allTextBool[0] || Mod_TextController.allTextBool[1] || Mod_TextController.allTextBool[2] || Mod_TextController.allTextBool[3]) //只要任意一個門Bool為True，DoorError(鎖機台用)就為True
        {
            if (!Mod_TextController.ErrorTextRunning) Mod_TextController.RunErrorTextBool = true; //如果沒再跑錯誤訊息，開始跑錯誤訊息
            DoorError = true;
            Mod_Data.doorError = true;
        }
        else
        {
            DoorError = false;
            Mod_Data.doorError = false;
        }
    }

    WaitForSecondsRealtime waitReadEventsTime = new WaitForSecondsRealtime(0.1f);
    IEnumerator ReadEvents()
    {
        while (true)
        {
            result = qxt_int_getevent(ref doorEvent);
            Debug.Log("doorEvent.eventcode: " + doorEvent.eventcode);
            switch (doorEvent.eventcode)
            {
                case 1:
                    newSramManager.SaveLogicDoorStatus(false);

                    Mod_TextController.doorCloseBool[0] = true; //設置true(顯示)、false(不顯示)控制是否UI顯示機門關閉訊息
                    if (Mod_TextController.allTextBool[0] && !Mod_TextController.DoorCloseTextRunning) Mod_TextController.RunDoorCloseTextBool = true; //如果沒再跑機門關閉訊息，開始跑訊息
                    Mod_TextController.allTextBool[0] = false;
                    break;
                case 3:
                    newSramManager.SaveMainDoorStatus(false);

                    Mod_TextController.doorCloseBool[1] = true; //設置true(顯示)、false(不顯示)控制是否UI顯示機門關閉訊息
                    if (Mod_TextController.allTextBool[1] && !Mod_TextController.DoorCloseTextRunning) Mod_TextController.RunDoorCloseTextBool = true;//如果沒再跑機門關閉訊息，開始跑訊息
                    Mod_TextController.allTextBool[1] = false;
                    break;
                case 4:
                    newSramManager.SaveBellyDoorStatus(false);

                    Mod_TextController.doorCloseBool[2] = true;
                    if (Mod_TextController.allTextBool[2] && !Mod_TextController.DoorCloseTextRunning) Mod_TextController.RunDoorCloseTextBool = true;
                    Mod_TextController.allTextBool[2] = false;
                    break;
                case 5:
                    newSramManager.SaveCashDoorStatus(false);

                    Mod_TextController.doorCloseBool[3] = true;
                    if (Mod_TextController.allTextBool[3] && !Mod_TextController.DoorCloseTextRunning) Mod_TextController.RunDoorCloseTextBool = true;
                    Mod_TextController.allTextBool[3] = false;
                    break;

                case 9:
                    newSramManager.SaveLogicDoorStatus(true);

                    Mod_TextController.allTextBool[0] = true;
                    Mod_TextController.doorCloseBool[0] = false;
                    break;
                case 11:
                    newSramManager.SaveMainDoorStatus(true);

                    Mod_TextController.allTextBool[1] = true;
                    Mod_TextController.doorCloseBool[1] = false;
                    break;
                case 12:
                    newSramManager.SaveBellyDoorStatus(true);

                    Mod_TextController.allTextBool[2] = true;
                    Mod_TextController.doorCloseBool[2] = false;
                    break;
                case 13:
                    newSramManager.SaveCashDoorStatus(true);

                    Mod_TextController.allTextBool[3] = true;
                    Mod_TextController.doorCloseBool[3] = false;
                    break;
            }

            if (Mod_TextController.allTextBool[0] || Mod_TextController.allTextBool[1] || Mod_TextController.allTextBool[2] || Mod_TextController.allTextBool[3]) //只要任意一個門Bool為True，DoorError(鎖機台用)就為True
            {
                if (!Mod_TextController.ErrorTextRunning) Mod_TextController.RunErrorTextBool = true; //如果沒再跑錯誤訊息，開始跑錯誤訊息

                DoorError = true;
                Mod_Data.doorError = true;
            }
            else
            {
                DoorError = false;
                Mod_Data.doorError = false;
            }
            yield return waitReadEventsTime;
        }
    }
#endif
    #endregion

    #region GS
#if GS
    //讀取
    public string ReadIniContent(string section, string key)
    {
        StringBuilder temp = new StringBuilder(255);
        int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
        return temp.ToString();
    }

    WaitForSecondsRealtime waitDoorSwitchTime = new WaitForSecondsRealtime(0.1f);
    IEnumerator DoorSwitch()
    {
        while (true)
        {
            CFPGADrvBridge.PIC_GetDoorStatus(ref Door); //回傳門的值


            if (tmpNum != Door)
            {
                doorStatus = Convert.ToString(Door, 2).Substring(5);

                for (int i = doorStatus.Length - 1; i >= 0; i--)
                {
                    if (doorStatus[i] == '1')
                    {
                        if (doorInterput == "0") Mod_TextController.allTextBool[3 - i] = false;
                        else if (doorInterput == "1") Mod_TextController.allTextBool[3 - i] = true;
                    }
                    else if (doorStatus[i] == '0')
                    {
                        if (doorInterput == "0") Mod_TextController.allTextBool[3 - i] = true;
                        else if (doorInterput == "1") Mod_TextController.allTextBool[3 - i] = false;
                    }
                }

                if (Mod_TextController.allTextBool[1] || Mod_TextController.allTextBool[2] || Mod_TextController.allTextBool[3])
                {
                    if (!Mod_TextController.ErrorTextRunning) Mod_TextController.RunErrorTextBool = true;
                    Mod_Data.doorError = true;
                }
                else
                {
                    Mod_Data.doorError = false;
                }

                if (!Mod_TextController.allTextBool[1] && lastDoorStatue[1]) Mod_TextController.doorCloseBool[1] = true;
                else Mod_TextController.doorCloseBool[1] = false;
                if (!Mod_TextController.allTextBool[2] && lastDoorStatue[2]) Mod_TextController.doorCloseBool[2] = true;
                else Mod_TextController.doorCloseBool[2] = false;
                if (!Mod_TextController.allTextBool[3] && lastDoorStatue[3]) Mod_TextController.doorCloseBool[3] = true;
                else Mod_TextController.doorCloseBool[3] = false;

                if (!Mod_TextController.DoorCloseTextRunning && (Mod_TextController.doorCloseBool[1] || Mod_TextController.doorCloseBool[2] || Mod_TextController.doorCloseBool[3])) Mod_TextController.RunDoorCloseTextBool = true; //如果沒再跑機門關閉訊息，開始跑訊息

                if (Mod_TextController.allTextBool[1]) lastDoorStatue[1] = true;  //最後存上次機門狀態
                else lastDoorStatue[1] = false;
                if (Mod_TextController.allTextBool[2]) lastDoorStatue[2] = true;
                else lastDoorStatue[2] = false;
                if (Mod_TextController.allTextBool[3]) lastDoorStatue[3] = true;
                else lastDoorStatue[3] = false;

                tmpNum = Door;
            }

            yield return waitDoorSwitchTime;
        }
    }
#endif
    #endregion
    #endregion
#endif
}