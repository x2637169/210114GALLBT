using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DWORD = System.UInt32;
using CFPGADrv;
using System.Text;
using System.Runtime.InteropServices;

public class DoorStatus_GS : MonoBehaviour
{
    MyIni ini;
    string path = Application.streamingAssetsPath + "/Serial.ini";//路徑
    string doorInterput = "0";

    [DllImport("kernel32")]
    public static extern int GetPrivateProfileString(string section, string key, string deval, StringBuilder stringBuilder, int size, string path);

    void Awake()
    {
        if (path == null) path = Application.streamingAssetsPath + "/Serial.ini";
    }

    //讀取
    public string ReadIniContent(string section, string key)
    {
        StringBuilder temp = new StringBuilder(255);
        int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
        return temp.ToString();
    }

    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS(); //賽菲硬體初始化
    DWORD Door = 255; //初始數值
    string doorStatus = null;
    bool[] lastDoorStatue;
    Mod_TextController Mod_TextController;

    void Start()
    {
        ini = new MyIni(Application.streamingAssetsPath + "/Door.ini");
        doorInterput = ini.ReadIniContent("Door", "Interput");
        Debug.Log("doorInterput: " + doorInterput);
        lastDoorStatue = new bool[4];
        Mod_TextController = FindObjectOfType<Mod_TextController>();
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FpgaPic_Init(); //賽菲
        StartCoroutine("DoorSwitch");
    }

    WaitForSecondsRealtime waitDoorSwitchTime = new WaitForSecondsRealtime(0.1f);
    IEnumerator DoorSwitch()
    {
        while (true)
        {
            CFPGADrvBridge.PIC_GetDoorStatus(ref Door); //回傳門的值

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

            if (!Mod_TextController.ErrorTextRunning && (Mod_TextController.allTextBool[1] || Mod_TextController.allTextBool[2] || Mod_TextController.allTextBool[3])) Mod_TextController.RunErrorTextBool = true;

            if (!Mod_TextController.allTextBool[1] && lastDoorStatue[1]) Mod_TextController.doorCloseBool[1] = true;
            else Mod_TextController.doorCloseBool[1] = false;
            if (!Mod_TextController.allTextBool[2] && lastDoorStatue[2]) Mod_TextController.doorCloseBool[2] = true;
            else Mod_TextController.doorCloseBool[2] = false;
            if (!Mod_TextController.allTextBool[3] && lastDoorStatue[3]) Mod_TextController.doorCloseBool[3] = true;
            else Mod_TextController.doorCloseBool[3] = false;

            if (!Mod_TextController.DoorCloseTextRunning && (Mod_TextController.doorCloseBool[1] || Mod_TextController.doorCloseBool[2] || Mod_TextController.doorCloseBool[3])) Mod_TextController.RunDoorCloseTextBool = true; //如果沒再跑機門關閉訊息，開始跑訊息

            if (Mod_TextController.allTextBool[1]) lastDoorStatue[1] = true;  //最後存上次機門狀態
            else if (!Mod_TextController.allTextBool[1] && lastDoorStatue[1]) lastDoorStatue[1] = false;
            if (Mod_TextController.allTextBool[2]) lastDoorStatue[2] = true;
            else if (!Mod_TextController.allTextBool[2] && lastDoorStatue[2]) lastDoorStatue[2] = false;
            if (Mod_TextController.allTextBool[3]) lastDoorStatue[3] = true;
            else if (!Mod_TextController.allTextBool[3] && lastDoorStatue[3]) lastDoorStatue[3] = false;

            if (Mod_TextController.allTextBool[1] || Mod_TextController.allTextBool[2] || Mod_TextController.allTextBool[3]) //只要任意一個門Bool為True，DoorError(鎖機台用)就為True
            {
                Mod_Data.doorError = true;
            }
            else
            {
                Mod_Data.doorError = false;
            }

            yield return waitDoorSwitchTime;
        }
    }
}