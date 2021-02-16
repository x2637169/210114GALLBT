using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class Mod_TextController : MonoBehaviour
{
    #region 詳細錯誤訊息設定
    //大部分錯誤訊息、控制變數設定
    public static bool[] allTextBool;
    public string[] allTextString;
    public Text allErrorText;
    public static bool ErrorTextRunning = false, RunErrorTextBool = false;

    //機門錯誤訊息、控制變數設定
    public bool[] doorCloseBool;
    public string[] doorCloseString;
    public Text doorCloseText;
    public static bool DoorCloseTextRunning = false, RunDoorCloseTextBool = false, StopRunningDoorCloseText = false;

    //票機錯誤訊息、控制變數設定
    public bool[] printerTextBool;
    public string[] printerTextString;
    public Text printerErrorText;
    public static bool printerTextRunning = false, RunPrinterTextBool = false;

    //部分提示訊息、控制變數設定
    public bool[] noticeTextBool;
    public string[] noticeTextString;
    public Text noticeText;
    public bool noticeTextRunning = false;

    //伺服器延遲提示、控制變數設定
    public Text waitServerText;
    public static bool WaitServerTextRunning = false, RunWaitServerTextBool = false;
    public static bool CleanText = false;
    #endregion

    #region 錯誤訊息UI
    public Text machineErrorText;

    public static Text billAndTciketInfo_Text, printerTicketOutValue_Text;
    public static GameObject printerTicketOutValueInfo_Obj;
    public static Image printerTicketOutValueInfo_Image;
    #endregion

#if Server
    #region Server
    WaitForSecondsRealtime waitRunErrorTime = new WaitForSecondsRealtime(0.1f);
    WaitForSecondsRealtime waitRunDoorCloseTime = new WaitForSecondsRealtime(0.1f);
    WaitForSecondsRealtime waitRunPrinterTextTime = new WaitForSecondsRealtime(0.1f);
    WaitForSecondsRealtime waitRunNoticeTime = new WaitForSecondsRealtime(0.1f);

    void Awake()
    {
        //allErrorText = GameObject.Find("All_Error_Text").GetComponent<Text>();
        //doorCloseText = GameObject.Find("All_DoorClose_Text").GetComponent<Text>();
        //printerErrorText = GameObject.Find("All_Printer_Text").GetComponent<Text>();
        //noticeText = GameObject.Find("All_Notice_Text").GetComponent<Text>();
        //machineErrorText = GameObject.Find("All_MachineLock_Text").GetComponent<Text>();

        allTextBool = new bool[15];
        allTextString = new string[15];
        doorCloseBool = new bool[4];
        doorCloseString = new string[4];
        printerTextBool = new bool[28];
        printerTextString = new string[28];
        noticeTextBool = new bool[3];
        noticeTextString = new string[3];

        allErrorText.text = null;
        doorCloseText.text = null;
        printerErrorText.text = null;
        machineErrorText.text = null;
        noticeText.text = null;

        printerTicketOutValueInfo_Obj = GameObject.Find("All_PrinterTicketOutValueInfo");
        billAndTciketInfo_Text = GameObject.Find("All_BillAndTciketInfo_Text").GetComponent<Text>();
        printerTicketOutValue_Text = printerTicketOutValueInfo_Obj.GetComponentInChildren<Text>(true);
        printerTicketOutValueInfo_Image = printerTicketOutValueInfo_Obj.GetComponentInChildren<Image>(true);
        if (!printerTicketOutValueInfo_Image.isActiveAndEnabled) printerTicketOutValueInfo_Image.gameObject.SetActive(true); printerTicketOutValueInfo_Image.enabled = true;
        printerTicketOutValueInfo_Obj.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        //設置陣列內容
        SetAllText();
        SetDoorCloseText();
        SetPrinterText();
        SetNoticeText();
    }

    bool MachineTextFirstTime = true;
    void Update()
    {
        //提示狀態為True，並且提示協程不是運行狀態、執行協程
        if ((noticeTextBool[0] || noticeTextBool[1] || noticeTextBool[2]) && !noticeTextRunning)
        {
            RunNoticeText();
        }

        //取消錯誤狀態
        if (!ErrorTextRunning && !printerTextRunning && Mod_Data.MachineError)
        {
            Mod_Data.MachineError = false;
        }

        //初次設置機台直到機台沒任何錯誤，顯示Machine Ready
        if (Mod_Data.MachineError && (string.IsNullOrEmpty(machineErrorText.text) || machineErrorText.text == "Machine Ready"))
        {
            machineErrorText.text = "Machine Lock";
        }
        else if (!Mod_Data.MachineError)
        {
            if (MachineTextFirstTime)
            {
                machineErrorText.text = "Machine Ready";
                MachineTextFirstTime = false;
            }
            else if (!string.IsNullOrEmpty(machineErrorText.text))
            {
                if (!Mod_Data.inBaseSpin || machineErrorText.text != "Machine Ready") machineErrorText.text = null;
            }
        }

        //錯誤狀態為True，並且錯誤協程不是運行狀態、執行協程
        if (RunErrorTextBool && !ErrorTextRunning)
        {
            ErrorTextRunning = true;
            RunErrorTextBool = false;
            RunErrorText();
        }

        //機門關閉狀態為True，並且機門協程不是運行狀態、執行協程
        if (RunDoorCloseTextBool && !DoorCloseTextRunning)
        {
            DoorCloseTextRunning = true;
            RunDoorCloseTextBool = false;
            RunDoorCloseText();
        }

        //機門協程運行狀態，停止機門關閉狀態為True，執行協程
        if (StopRunningDoorCloseText && DoorCloseTextRunning)
        {
            StopRunningDoorCloseText = false;
            StopRunDoorCloseText();
        }

        //票機狀態為True，並且票機協程不是運行狀態、執行協程
        if ((RunPrinterTextBool && !printerTextRunning) || (!printerTextRunning && (printerTextBool[0] || printerTextBool[27])))
        {
            printerTextRunning = true;
            RunPrinterTextBool = false;
            RunPrinterText();
        }

        //伺服器延遲狀態為True，並且伺服器延遲協程不是運行狀態、執行協程
        if (RunWaitServerTextBool && !WaitServerTextRunning)
        {
            WaitServerTextRunning = true;
            RunWaitServerTextBool = false;
            RunWaitServerText();
        }

        //清除文字狀態為True，清除UI的Text內容
        if (CleanText)
        {
            CleanText = false;
            cleanPrinterAndBillInfo();
        }

        if (Mod_TextController.DoorCloseTextRunning && !Mod_Data.inBaseSpin && !Mod_Data.MachineError) StopRunningDoorCloseText = true;
        if (!string.IsNullOrEmpty(Mod_TextController.billAndTciketInfo_Text.text) && !Mod_Data.inBaseSpin && !Mod_Data.handpay) Mod_TextController.billAndTciketInfo_Text.text = null;
        if (!string.IsNullOrEmpty(Mod_TextController.printerTicketOutValue_Text.text) && !Mod_Data.inBaseSpin && !Mod_Data.handpay) Mod_TextController.printerTicketOutValue_Text.text = null;
        if (Mod_TextController.printerTicketOutValueInfo_Obj.activeInHierarchy && !Mod_Data.inBaseSpin && !Mod_Data.handpay) Mod_TextController.printerTicketOutValueInfo_Obj.SetActive(false);
    }

    public void cleanPrinterAndBillInfo()
    {
        if (!string.IsNullOrEmpty(Mod_TextController.billAndTciketInfo_Text.text)) Mod_TextController.billAndTciketInfo_Text.text = null;
        if (!string.IsNullOrEmpty(Mod_TextController.printerTicketOutValue_Text.text)) Mod_TextController.printerTicketOutValue_Text.text = null;
        if (Mod_TextController.printerTicketOutValueInfo_Obj.activeInHierarchy) Mod_TextController.printerTicketOutValueInfo_Obj.SetActive(false);
    }

    public void RunErrorText() //開始跑錯誤訊息
    {
        StartCoroutine("RunErrorText_IE");
        //Debug.Log("RunErrorText_IE");
    }

    public void StopRunErrorText() //停止跑錯誤訊息
    {
        StopCoroutine("RunErrorText_IE");
        ErrorTextRunning = false;
        allErrorText.text = null;
        //Debug.Log("RunErrorText_IE Stop");
    }

    IEnumerator RunErrorText_IE()
    {
        int findTrue = 0;
        int count = 0;
        int noError = 0;

        while (true)
        {
            Mod_Data.MachineError = true;

            while (allTextBool[findTrue] != true)
            {
                findTrue++;
                if (findTrue > allTextBool.Length - 1) //超出範圍初始化
                {
                    findTrue = 0;
                    if (noError >= 1) StopRunErrorText(); //如果跑一輪沒找到錯誤停止迴圈
                    noError++;
                }
                yield return null;
            }

            noError = 0;

            if (!string.IsNullOrEmpty(allTextString[findTrue]))
            {
                allErrorText.text = allTextString[findTrue]; //UI顯示錯誤訊息
                while (allTextBool[findTrue] == true && count < 10)
                {
                    count++;
                    yield return waitRunErrorTime; //等待N秒繼續執行
                }
            }

            count = 0;
            findTrue++;
            if (findTrue > allTextBool.Length - 1) findTrue = 0;
        }
    }

    public void RunDoorCloseText() //開始跑錯誤訊息
    {
        StartCoroutine("RunDoorCloseText_IE");
        //Debug.Log("RunDoorCloseText_IE");
    }

    public void StopRunDoorCloseText() //停止跑錯誤訊息
    {
        StopCoroutine("RunDoorCloseText_IE");
        doorCloseText.text = null;
        DoorCloseTextRunning = false;
        //Debug.Log("RunDoorCloseText_IE Stop");
    }

    IEnumerator RunDoorCloseText_IE()
    {
        int findTrue = 0;
        int count = 0;
        int noError = 0;

        while (true)
        {
            while (doorCloseBool[findTrue] != true)
            {
                findTrue++;
                if (findTrue > doorCloseBool.Length - 1)
                {
                    findTrue = 0;
                    if (noError >= 1) StopRunDoorCloseText();
                    noError++;
                }
                yield return null;
            }

            noError = 0;

            if (!string.IsNullOrEmpty(doorCloseString[findTrue]))
            {
                doorCloseText.text = doorCloseString[findTrue];
                while (doorCloseBool[findTrue] == true && count < 10)
                {
                    count++;
                    yield return waitRunDoorCloseTime; //等待N秒繼續執行
                }
            }

            count = 0;
            findTrue++;
            if (findTrue > doorCloseBool.Length - 1) findTrue = 0;
        }
    }

    public void RunPrinterText() //開始跑錯誤訊息
    {
        StartCoroutine("RunPrinterText_IE");
        //Debug.Log("RunPrinterText_IE");
    }

    public void StopPrinterText() //停止跑錯誤訊息
    {
        StopCoroutine("RunPrinterText_IE");
        printerErrorText.text = null;
        printerTextRunning = false;
        //Debug.Log("RunPrinterText_IE Stop");
    }

    IEnumerator RunPrinterText_IE()
    {
        int findTrue = 0;
        int count = 0;
        int noError = 0;

        while (true)
        {

            while (printerTextBool[findTrue] != true)
            {
                findTrue++;
                if (findTrue > printerTextBool.Length - 1)
                {
                    findTrue = 0;
                    if (noError >= 1) StopPrinterText();
                    noError++;
                }
                yield return null;
            }

            noError = 0;

            if (!string.IsNullOrEmpty(printerTextString[findTrue]) && findTrue != 20 && findTrue != 21 && findTrue != 24)
            {
                if (!Mod_Data.MachineError)
                {
                    if (findTrue != 20 || findTrue != 21 || findTrue != 24 || findTrue != 26) Mod_Data.MachineError = true;
                }
                printerErrorText.text = printerTextString[findTrue];
                while (printerTextBool[findTrue] == true && count < 10)
                {
                    count++;
                    yield return waitRunPrinterTextTime; //等待N秒繼續執行
                }
            }

            count = 0;
            findTrue++;
            if (findTrue > printerTextBool.Length - 1) findTrue = 0;
        }
    }

    public void RunNoticeText() //開始跑錯誤訊息
    {
        noticeTextRunning = true;
        StartCoroutine("RunNoticerText_IE");
        //Debug.Log("RunNoticerText_IE");
    }

    public void StopNoticeText() //停止跑錯誤訊息
    {
        StopCoroutine("RunNoticerText_IE");
        noticeText.text = null;
        noticeTextRunning = false;
        //Debug.Log("RunNoticerText_IE Stop");
    }

    IEnumerator RunNoticerText_IE()
    {
        int findTrue = 0;
        int count = 0;
        int noError = 0;

        while (true)
        {
            while (noticeTextBool[findTrue] != true)
            {
                findTrue++;
                if (findTrue > noticeTextBool.Length - 1)
                {
                    findTrue = 0;
                    if (noError >= 1) StopNoticeText();
                    noError++;
                }
                yield return null;
            }

            noError = 0;

            if (!string.IsNullOrEmpty(noticeTextString[findTrue]))
            {
                noticeText.text = noticeTextString[findTrue];
                while (noticeTextBool[findTrue] == true && count < 10)
                {
                    count++;
                    yield return waitRunNoticeTime;
                }
            }

            count = 0;
            findTrue++;
            if (findTrue > noticeTextBool.Length - 1) findTrue = 0;
        }
    }


    public void RunWaitServerText() //開始跑錯誤訊息
    {
        StartCoroutine("RunWaitServerText_IE");
        //Debug.Log("RunNoticerText_IE");
    }

    public void StopWaitServerText() //停止跑錯誤訊息
    {
        StopCoroutine("RunWaitServerText_IE");
        WaitServerTextRunning = false;
        //Debug.Log("RunNoticerText_IE Stop");
    }
    WaitForSecondsRealtime WaitServerTime = new WaitForSecondsRealtime(0.5f);
    IEnumerator RunWaitServerText_IE()
    {
        waitServerText.text = null;

        while (WaitServerTextRunning)
        {
            waitServerText.text += ".";
            if (waitServerText.text.Length >= 10) waitServerText.text = null;
            yield return WaitServerTime;
        }

        waitServerText.text = null;
    }

    #region 設置array訊息
    void SetAllText()
    {
        allTextString[0] = "Logic Door Open";
        allTextString[1] = "Main Door Open";
        allTextString[2] = "Belly Door Open";
        allTextString[3] = "Cash Door Open";
        allTextString[4] = "Stacker Full";
        allTextString[5] = "Stacker Open";
        allTextString[6] = "Failure [0x49]: ";
        allTextString[7] = "Communication Error";
        allTextString[8] = "Invalid Command";
        allTextString[9] = "Server Disconnect";
        allTextString[10] = "BillMachine Not Open";
        allTextString[11] = "Server Lock";
        allTextString[12] = "MachineID Lock";
        allTextString[13] = "Bill Buffer Error";
        allTextString[14] = "GameSet Lock";
    }

    void SetDoorCloseText()
    {
        doorCloseString[0] = "Logic Door Close";
        doorCloseString[1] = "Main Door Close";
        doorCloseString[2] = "Belly Door Close";
        doorCloseString[3] = "CASH Door Close";
    }

    void SetPrinterText()
    {
        printerTextString[0] = "Printer Disconnect!";
        printerTextString[1] = "PrinterBusy";
        printerTextString[2] = "PrinterSystemError";
        printerTextString[3] = "PlatenUp";
        printerTextString[4] = "PrinterPaperOut";
        printerTextString[5] = "HeadError";
        printerTextString[6] = "VoltageError";
        printerTextString[7] = "TemperatureError";
        printerTextString[8] = "LibraryRefError";
        printerTextString[9] = "PRDataError";
        printerTextString[10] = "LibraryLoadError";
        printerTextString[11] = "BufferOverflow";
        printerTextString[12] = "JobMemoryOverflow";
        printerTextString[13] = "CommandError";
        printerTextString[14] = "PrintLibrariesCorrupted";
        printerTextString[15] = "FlashProgError";
        printerTextString[16] = "PrinterOffLine";
        printerTextString[17] = "MissingSupplyIndex";
        printerTextString[18] = "JournalPrinting";
        printerTextString[19] = "PaperJamMayExist";
        printerTextString[20] = "ValidationDone";
        printerTextString[21] = "AtTopOfFromJam";
        printerTextString[22] = "XOff";
        printerTextString[23] = "PrinterOpen";
        printerTextString[24] = "BarcodeDataIsAccessed";
        printerTextString[25] = "ResetOrPowerUp";
        printerTextString[26] = "PaperLow";
        printerTextString[27] = "Printer Not Open";
    }

    void SetNoticeText()
    {
        noticeTextString[0] = "Please Take Your Ticket";
        noticeTextString[1] = "Acceptor Reject";
        noticeTextString[2] = "Acceptor RETURNING";
    }
    #endregion
    #endregion
#endif
}