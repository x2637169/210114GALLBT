using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Globalization;

//[ExecuteInEditMode]
public class Mod_Gen2_Status : MonoBehaviour
{
    public static bool status1_Error, status2_Error, status3_Error, status4_Error, status5_Error;
    public static bool status1_Notice, status2_Notice, status3_Notice, status4_Notice, status5_Notice;
    [SerializeField] Mod_Gen2 mod_Gen2;
    [SerializeField] Mod_UIController mod_UIController;
    [SerializeField] NewSramManager newSramManager;
    [SerializeField] Mod_TextController mod_AllErrorTextController;
    [SerializeField] Mod_Client mod_Client;
    public Dropdown comPortDropDown;
    public Button printer_PanelExit, printer_PanelOpenClose;
    public Text SettingInfo, Gen2_OC_Info;
    public string strbytes;
    //public double tmpCredit;
    //bool recoverCredit = false;
    public bool keepConvert = false;
    public static bool PrinterDisconnect = false;

#if Server
    #region Server
    //Regex regexGen2 = new Regex(@"GURCH2GE0"); //尋找有否此字串
    //Regex regexGen5 = new Regex(@"5RUSAGE27"); //尋找有否此字串

    /*void Awake()
    {
        mod_Gen2 = FindObjectOfType<Mod_Gen2>();
        mod_UIController = FindObjectOfType<Mod_UIController>();
        newSramManager = FindObjectOfType<NewSramManager>();
        mod_AllErrorTextController = FindObjectOfType<Mod_TextController>();
        comPortDropDown = GameObject.Find("Pirnter_COMPort").GetComponent<Dropdown>();
        printer_PanelExit = GameObject.Find("Printer_Exit_Button").GetComponent<Button>();
        printer_PanelOpenClose = GameObject.Find("Printer_OpenClose").GetComponent<Button>();
        SettingInfo = GameObject.Find("Printer_SettingInfo").GetComponent<Text>();
        Gen2_OC_Info = GameObject.Find("Printer_OpenCloseInfo").GetComponent<Text>();
    }*/

    // Start is called before the first frame update
    void Start()
    {

        newSramManager.LoadPrinterCOMPort(out Gen2_Data.Gen2Comport);
        comPortDropDown.value = int.Parse(Gen2_Data.Gen2Comport.Substring(Gen2_Data.Gen2Comport.Length - 1, 1)) - 1;
        StartCoroutine(OpenGen());
    }

    WaitForSecondsRealtime waitLongTime = new WaitForSecondsRealtime(1f);
    WaitForSecondsRealtime waitShotrTime = new WaitForSecondsRealtime(0.01f);
    IEnumerator OpenGen()
    {
        Gen2_Data.FirstTryOpenPrinter = true;
        while (true)  //等待伺服器回傳
        {
            if (Gen2_Data.GenEnableWaitServer != 2) break; //伺服器回傳值判斷失敗還是成功
            yield return waitLongTime;
        }

        yield return waitLongTime;

        if (Gen2_Data.GenEnableWaitServer == 1 && Gen2_Data.GenOpen)
        {
            Debug.Log("Gen Open");
            OpenOrCloserGen2();
        }
        else
        {
            Debug.Log("Gen Not Open");
        }

        Gen2_Data.FirstTryOpenPrinter = false;
        Gen2_Data.GenEnableWaitServer = 2;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //OpenOrCloserGen2();
        //PrintTikcet();
        //}

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    Mod_Client_Data.serverdisconnect = false;
        //    Debug.Log("serverdisconnect: " + Mod_Client_Data.serverdisconnect);
        //}

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    Mod_Client_Data.serverdisconnect = true;
        //    Debug.Log("serverdisconnect: " + Mod_Client_Data.serverdisconnect);
        //}

        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    mod_Client.Strhandle("703,1,9999,00-0668-5170-1417-2830,2020-02-06 15:15:18");
        //    Debug.Log("Print: ");
        //}

        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    mod_Client.Strhandle("703,0,9999,00-0668-5170-1417-2830,2020-02-06 15:15:18");
        //    Debug.Log("Not Print: ");
        //}


        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    mod_Client.Strhandle("705,1,1,2,95,1,1,200,1,1,100000,100000,100000,100000,10,0.5");
        //}

        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    mod_Client.Strhandle("705,1,1,2,95,0,0,200,0,0,100000,100000,100000,100000,10,0.5");
        //}
        if (Gen2_Data.GenOpen)
        {
            if (keepConvert)
            {
                mod_AllErrorTextController.printerTextBool[27] = false;
            }
            else
            {
                mod_AllErrorTextController.printerTextBool[27] = true;
                if (!Mod_TextController.ErrorTextRunning) Mod_TextController.RunErrorTextBool = true;
            }
        }
        else
        {
            mod_AllErrorTextController.printerTextBool[27] = false;
        }

        if (PrinterDisconnect)
        {
            Mod_Data.printerError = true;
            SettingInfo.text = "Port設置錯誤或者線脫落";
            if (keepConvert)
            {
                OpenOrCloserGen2();
            }
            else
            {
                mod_AllErrorTextController.printerTextBool[0] = true;
                if (!Mod_TextController.printerTextRunning) Mod_TextController.RunPrinterTextBool = true;
            }
            // if(Gen2_Data.GenOpen){
            //     mod_AllErrorTextController.printerTextBool[0] = true;
            //    if (!mod_AllErrorTextController.printerTextRunning) mod_AllErrorTextController.RunPrinterText();
            // }
            // else{
            //     mod_AllErrorTextController.printerTextBool[0] = false;
            // }
            //mod_AllErrorTextController.printerTextBool[0] = true;
            //if (!mod_AllErrorTextController.printerTextRunning) mod_AllErrorTextController.RunPrinterText();


        }
    }

    float waitTime = 0.5f;
    public IEnumerator ConvertToBits()
    {
        while (keepConvert)
        {
            if (Mod_Gen2.PrinterWorking && !string.IsNullOrEmpty(strbytes) && !PrinterDisconnect)
            {
                string[] status = strbytes.Split('|');
                if (strbytes.Length == 28 && status.Length == 10)
                {
                    Flag1Showstatus(StringToBinary(status[3])); //flag1
                    Flag2Showstatus(StringToBinary(status[4])); //flag2
                    Flag3Showstatus(StringToBinary(status[5])); //flag3
                    Flag4Showstatus(StringToBinary(status[6])); //flag4
                    Flag5Showstatus(StringToBinary(status[7])); //flag5
                }
                else
                {
                    Debug.Log("PrinterError");
                }
                CheckPrinter();
            }

            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    public string StringToBinary(string data) //位元轉換
    {
        string i = null;
        foreach (char c in data.ToCharArray())
        {
            i = Convert.ToString(c, 2).PadLeft(8, '0');
        }
        return i.ToString();
    }

    public void Flag1Showstatus(string Flag1Binary)
    {
        status1_Error = false;
        status1_Notice = false;
        for (int i = 0; i < Flag1Binary.Length; i++)
        {
            switch (Flag1Binary[i])
            {
                case '0':
                    switch (i)
                    {
                        case 2:
                            //mod_AllErrorTextController.printerTextBool[1] = false;
                            break;
                        case 3:
                            //mod_AllErrorTextController.printerTextBool[2] = false;
                            break;
                        case 4:
                            mod_AllErrorTextController.printerTextBool[3] = false; break;
                        case 5:
                            mod_AllErrorTextController.printerTextBool[4] = false; break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[5] = false; break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[6] = false; break;
                    }
                    break;
                case '1':
                    switch (i)
                    {
                        case 2:
                            //mod_AllErrorTextController.printerTextBool[1] = true; //ErrorText.text += "<color=#FF0000>PrinterBusy</color>\r\n "; //忙碌-印票才會出現
                            //status1_Error = true;
                            break;
                        case 3:
                            //mod_AllErrorTextController.printerTextBool[2] = true; //ErrorText.text += "<color=#FF0000>PrinterSystemError</color>\r\n "; //票機錯誤
                            //status1_Error = true; 
                            break;
                        case 4:
                            mod_AllErrorTextController.printerTextBool[3] = true; //ErrorText.text += "<color=#FF0000>PlatenUp</color>\r\n "; //票機門打開狀態
                            status1_Error = true; break;
                        case 5:
                            mod_AllErrorTextController.printerTextBool[4] = true; //ErrorText.text += "<color=#FF0000>PrinterPaperOut</color>\r\n "; //沒票機紙
                            //status1_Error = true; 
                            break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[5] = true; //ErrorText.text += "<color=#FF0000>HeadError</color>\r\n "; //票機頭錯誤
                            status1_Error = true; break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[6] = true; //ErrorText.text += "<color=#FF0000>VoltageError</color>\r\n "; //電壓錯誤
                            status1_Error = true; break;
                    }
                    break;
            }
        }
    }

    public void Flag2Showstatus(string Flag2Binary)
    {
        status2_Error = false;
        status2_Notice = false;
        for (int i = 0; i < Flag2Binary.Length; i++)
        {
            switch (Flag2Binary[i])
            {
                case '0':
                    switch (i)
                    {
                        case 2:
                            mod_AllErrorTextController.printerTextBool[7] = false; break;
                        case 3:
                            mod_AllErrorTextController.printerTextBool[8] = false; break;
                        case 4:
                            mod_AllErrorTextController.printerTextBool[9] = false; break;
                        case 5:
                            mod_AllErrorTextController.printerTextBool[10] = false; break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[11] = false; break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[12] = false; break;
                    }
                    break;
                case '1':
                    switch (i)
                    {
                        case 2:
                            mod_AllErrorTextController.printerTextBool[7] = true; //ErrorText.text += "<color=#FF0000>TemperatureError</color>\r\n "; //票機溫度過高
                            status2_Error = true; break;
                        case 3:
                            mod_AllErrorTextController.printerTextBool[8] = true; //ErrorText.text += "<color=#FF0000>LibraryRefError</color>\r\n ";
                            status2_Error = true; break;
                        case 4:
                            mod_AllErrorTextController.printerTextBool[9] = true; //ErrorText.text += "<color=#FF0000>PRDataError</color>\r\n ";
                            status2_Error = true; break;
                        case 5:
                            mod_AllErrorTextController.printerTextBool[10] = true; //ErrorText.text += "<color=#FF0000>LibraryLoadError</color>\r\n ";
                            status2_Error = true; break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[11] = true; //ErrorText.text += "<color=#FF0000>BufferOverflow</color>\r\n "; //緩衝溢出
                            status2_Error = true; break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[12] = true; //ErrorText.text += "<color=#FF0000>JobMemoryOverflow</color>\r\n "; //向票機請求內存
                            status2_Error = true; break;
                    }
                    break;
            }
        }
    }

    public void Flag3Showstatus(string Flag3Binary)
    {
        status3_Error = false;
        status3_Notice = false;
        for (int i = 0; i < Flag3Binary.Length; i++)
        {
            switch (Flag3Binary[i])
            {
                case '0':
                    switch (i)
                    {
                        case 2:
                            mod_AllErrorTextController.printerTextBool[13] = false; break;
                        case 3:
                            mod_AllErrorTextController.printerTextBool[14] = false; break;
                        case 4:
                            mod_AllErrorTextController.noticeTextBool[0] = false; break;
                        case 5:
                            mod_AllErrorTextController.printerTextBool[15] = false; break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[16] = false; break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[17] = false; break;
                    }
                    break;
                case '1':
                    switch (i)
                    {
                        case 2:
                            mod_AllErrorTextController.printerTextBool[13] = true; //ErrorText.text += "<color=#FF0000>CommandError</color>\r\n "; //指令錯誤
                            status3_Error = true;
                            mod_Gen2.ClearError(); break;
                        case 3:
                            mod_AllErrorTextController.printerTextBool[14] = true; //ErrorText.text += "<color=#FF0000>PrintLibrariesCorrupted</color>\r\n "; //等待字體加載
                            status3_Error = true; break;
                        case 4:
                            mod_AllErrorTextController.noticeTextBool[0] = true;//NoticeText.text += "<color=#FF0000>PaperInChute</color>\r\n "; //票還留在出票口
                            status3_Notice = true; break;
                        case 5:
                            mod_AllErrorTextController.printerTextBool[15] = true; //ErrorText.text += "<color=#FF0000>FlashProgError</color>\r\n ";
                            status3_Error = true; break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[16] = true; //ErrorText.text += "<color=#FF0000>PrinterOffLine</color>\r\n "; //票機離線
                            status3_Error = true; break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[17] = true; //ErrorText.text += "<color=#FF0000>MissingSupplyIndex</color>\r\n "; //紙張位置錯誤、傳感器故障
                            status3_Error = true; break;
                    }
                    break;
            }
        }
    }

    public void Flag4Showstatus(string Flag4Binary)
    {
        status4_Error = false;
        status4_Notice = false;
        for (int i = 0; i < Flag4Binary.Length; i++)
        {
            switch (Flag4Binary[i])
            {
                case '0':
                    switch (i)
                    {
                        case 4:
                            mod_AllErrorTextController.printerTextBool[18] = false; break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[19] = false; break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[26] = false; break;
                    }
                    break;
                case '1':
                    switch (i)
                    {
                        case 4:
                            mod_AllErrorTextController.printerTextBool[18] = true; //ErrorText.text += "<color=#FF0000>JournalPrinting</color>\r\n ";
                            status4_Error = true; break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[19] = true; //ErrorText.text += "<color=#FF0000>PaperJamMayExist</color>\r\n "; //卡紙
                            status4_Error = true;
                            break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[26] = true; //NoticeText.text += "<color=#FF0000>PaperLow</color>\r\n "; //紙槽紙數量過低
                            //status4_Error = true; 
                            break;
                    }
                    break;
            }
        }
    }

    public void Flag5Showstatus(string Flag5Binary)
    {
        status5_Error = false;
        status5_Notice = false;
        for (int i = 0; i < Flag5Binary.Length; i++)
        {
            switch (Flag5Binary[i])
            {
                case '0':
                    switch (i)
                    {
                        case 2:
                            mod_AllErrorTextController.printerTextBool[20] = false;
                            break;
                        case 3:
                            mod_AllErrorTextController.printerTextBool[21] = false;
                            break;
                        case 4:
                            mod_AllErrorTextController.printerTextBool[22] = false; break;
                        case 5:
                            mod_AllErrorTextController.printerTextBool[23] = false; break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[24] = false;
                            break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[25] = false; break;
                    }
                    break;
                case '1':
                    switch (i)
                    {
                        case 2:
                            mod_AllErrorTextController.printerTextBool[20] = true; //ErrorText.text += "<color=#FF0000>ValidationDone</color>\r\n ";
                            //status5_Error = true;
                            break;
                        case 3:
                            mod_AllErrorTextController.printerTextBool[21] = true; //ErrorText.text += "<color=#FF0000>AtTopOfFromJam</color>\r\n ";
                            //status5_Error = true;
                            break;
                        case 4:
                            mod_AllErrorTextController.printerTextBool[22] = true; //ErrorText.text += "<color=#FF0000>XOff</color>\r\n "; //票機緩重快滿
                            status5_Error = true; break;
                        case 5:
                            mod_AllErrorTextController.printerTextBool[23] = true; //ErrorText.text += "<color=#FF0000>PrinterOpen</color>\r\n ";
                            status5_Error = true; break;
                        case 6:
                            mod_AllErrorTextController.printerTextBool[24] = true; //ErrorText.text += "<color=#FF0000>BarcodeDataIsAccessed</color>\r\n ";
                            //status5_Error = true;
                            mod_Gen2.ClearError(); break;
                        case 7:
                            mod_AllErrorTextController.printerTextBool[25] = true; //ErrorText.text += "<color=#FF0000>ResetOrPowerUp</color>\r\n "; //票機重啟、通電
                            status5_Error = true;
                            mod_Gen2.ClearError(); break;
                    }
                    //mod_Gen2.ClearError();
                    break;
            }
        }
    }

    public void PrintTikcet()
    {
        StartCoroutine("PrintTicket_IE");
    }

    public bool ticketPirintDone = false;
#if Server
    #region Server
    public IEnumerator PrintTicket_IE()
    {
        bool ticketOutFailed = true;
        ticketPirintDone = false;
        Gen2_Data.PinterHardStop = false;

        if (Mod_Gen2.PrinterWorking && !PrinterDisconnect && Mod_Data.state == Mod_State.STATE.BaseSpin && Mod_Data.credit > 0 && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.printerError && !Mod_Data.MachineError && Mod_Data.credit < Gen2_Data.TicketOutMaxLimit + 1)
        {
            Debug.Log("Print Ticket");
            Mod_TextController.CleanText = true;
            Mod_Data.serial = null;
            Mod_Data.serial2 = null;
            StopCoroutine("CleanText");

            Gen2_Data.TicketOutWaitSever = 2;
            mod_Client.SendToSever(Mod_Client_Data.messagetype.ticketout);   //傳送資料給SERVER獲取出票資料
            while (true)
            {
                if (Gen2_Data.TicketOutWaitSever != 2) break;//伺服器回傳值判斷認證失敗還是成功 伺服器斷線
                if (Mod_Client_Data.serverdisconnect)
                {
                    yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
                    yield return waitLongTime;
                    mod_Client.SendToSever(Mod_Client_Data.messagetype.ticketout);   //傳送資料給SERVER獲取出票資料
                }
                yield return waitShotrTime;
            }

            // 0或其他數字 伺服器回傳票失敗 1 伺服器回傳票成功
            if (Gen2_Data.TicketOutWaitSever == 1)
            {
                Mod_TextController.printerTicketOutValueInfo_Obj.SetActive(true);
                if (Mod_TextController.printerTicketOutValueInfo_Image.enabled == false) Mod_TextController.printerTicketOutValueInfo_Image.enabled = true;
                Mod_TextController.printerTicketOutValue_Text.text = "START CASH OUT : " + Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty));
                Mod_TextController.billAndTciketInfo_Text.text = "TICKET PRINT DONE : " + Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty));
                if (newSramManager.LoadHandPayStatus() == 3 || newSramManager.LoadHandPayStatus() == 2) newSramManager.SaveHandPayStatus(3);
                else newSramManager.SaveHandPayStatus(1);

                if (Mod_Gen2.PrinterWorking) mod_Gen2.PrintTicket();

                int i = 0;
                while (true)
                {
                    yield return waitLongTime;
                    if (Mod_Data.printerError || Gen2_Data.PinterHardStop || mod_AllErrorTextController.printerTextBool[0] || mod_AllErrorTextController.printerTextBool[27] || (mod_AllErrorTextController.noticeTextBool[0] && i >= 5) || i >= 10) break;
                    i++;
                }
                //yield return new WaitUntil(() => (mod_AllErrorTextController.printerTextBool[20] || mod_AllErrorTextController.noticeTextBool[0]) || Mod_Data.printerError || Gen2_Data.PinterHardStop); //等待票完全印完或票機斷線出錯
                ticketPirintDone = true;
                Debug.Log("Mod_Data.MachineError: " + Mod_Data.MachineError + " Mod_Data.printerError: " + Mod_Data.printerError + " Gen2_Data.PinterHardStop: " + Gen2_Data.PinterHardStop + " Printer Disconnect: " + mod_AllErrorTextController.printerTextBool[0] + " Printer Not Open: " + mod_AllErrorTextController.printerTextBool[27]);
                Debug.Log(" take ticket: " + mod_AllErrorTextController.printerTextBool[20] + " PrinterPaperOut: " + mod_AllErrorTextController.printerTextBool[4] + " AtTopOfFromJam: " + mod_AllErrorTextController.printerTextBool[21] + " PaperLow: " + mod_AllErrorTextController.printerTextBool[26] + " TickeInChuw: " + mod_AllErrorTextController.noticeTextBool[0]);
                if (!Gen2_Data.PinterHardStop && !Mod_Data.printerError && !mod_AllErrorTextController.printerTextBool[0] && !mod_AllErrorTextController.printerTextBool[27])
                {
                    Debug.Log("Ticket Out");
                    if (newSramManager.LoadHandPayStatus() == 3 || newSramManager.LoadHandPayStatus() == 2) newSramManager.SaveHandPayStatus(2);
                    else newSramManager.SaveHandPayStatus(0);
                    mod_Gen2.ClearError();
                    ticketOutFailed = false;
                    Gen2_Data.TicketOutWaitSever = 2;
                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.ticketOut, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.ticketOut) + Mod_Data.credit);
                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.ticketOut_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.ticketOut_Class) + Mod_Data.credit);
                    newSramManager.saveEventRecoredByEventName(6, (int)(Mod_Data.credit * 100));
                    newSramManager.SaveTicketOutPoint(newSramManager.LoadTicketOutPoint() + Mod_Data.credit);

                    mod_Client.SendToSever(Mod_Client_Data.messagetype.ticketoutsuccess); //傳送資料給SERVER確定出票成功
                    while (true)
                    {
                        if (Gen2_Data.TicketOutWaitSever == 1) break;//伺服器回傳值判斷認證失敗還是成功 伺服器斷線
                        if (Mod_Client_Data.serverdisconnect)
                        {
                            yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
                            yield return waitLongTime;
                            mod_Client.SendToSever(Mod_Client_Data.messagetype.ticketoutsuccess); //傳送資料給SERVER確定出票成功
                        }
                        yield return waitShotrTime;
                    }

                    Mod_Data.credit = 0;
                    Debug.Log("Ticket Out Credit: " + Mod_Data.credit);
                    mod_UIController.UpdateScore(); //更新畫面上的分數

                    StartCoroutine("CleanText");
                }
            }
        }

        if (ticketOutFailed)
        {
            Mod_Client.isCallHandPay = true;
            if (Mod_Gen2.PrinterWorking) mod_Gen2.ClearError();
            if (newSramManager.LoadHandPayStatus() == 3 || newSramManager.LoadHandPayStatus() == 2) newSramManager.SaveHandPayStatus(3);
            else newSramManager.SaveHandPayStatus(1);
            Debug.Log("Print Ticket But Printer Error or Ticket Jam");
            yield return new WaitUntil(() => Mod_Client.isHandPaying && Mod_Data.handpay);
            yield return new WaitUntil(() => newSramManager.LoadHandPayStatus() == 0);
            yield return new WaitUntil(() => !Mod_Client.isHandPaying && !Mod_Data.handpay);
            Debug.Log("Hanpay Done!");
        }

        ticketPirintDone = false;
        Mod_Data.PrinterTicket = false;
        Gen2_Data.TicketOutWaitSever = 2;
        yield return null;
    }
    #endregion
#endif

    IEnumerator CleanText()
    {
        yield return new WaitForSecondsRealtime(3f);
        Mod_TextController.printerTicketOutValueInfo_Obj.SetActive(false);
        //TicketOutValueText.text = null;
        //TicketOutText.text = null;
    }

    public void CheckPrinter()
    {
        if ((status1_Error) || (status2_Error) || (status3_Error) || (status4_Error) || (status5_Error) || (Gen2_Data.GenOpen && (!keepConvert || !Mod_Gen2.PrinterWorking))) //票機不正常
        {
            //ErrorText.text = ErrorText.text.Insert(0, "<color=#FF0000>Error</color>\r\n ");
            if (!Mod_TextController.printerTextRunning) Mod_TextController.RunPrinterTextBool = true;
            Mod_Data.printerError = true;
        }
        else if ((!status1_Error) && (!status2_Error) && (!status3_Error) && (!status4_Error) && (!status5_Error) && (!Gen2_Data.GenOpen || (Gen2_Data.GenOpen && keepConvert && Mod_Gen2.PrinterWorking)) && !PrinterDisconnect) //票機正常
        {
            Mod_Data.printerError = false;
        }

        if (mod_AllErrorTextController.printerTextBool[4] || mod_AllErrorTextController.printerTextBool[26])
        {
            if (!Mod_TextController.printerTextRunning) Mod_TextController.RunPrinterTextBool = true;
        }

        /*if ((status1_Notice) || (status2_Notice) || (status3_Notice) || (status4_Notice) || (status5_Notice))
        {
            //NoticeText.text = NoticeText.text.Insert(0, "<color=#FF0000>Notice</color>\r\n ");
        }
        else if ((!status1_Notice) && (!status2_Notice) && (!status3_Notice) && (!status4_Notice) && (!status5_Notice))
        {

        }*/
    }

    public void OpenOrCloserGen2()
    {
        mod_AllErrorTextController.printerTextBool[0] = false;
        if (!mod_Gen2.ReciveLoop)
        {
            Gen2_OC_Info.text = "關閉票機";
            Debug.Log("票機開啟");
            mod_Gen2.StartGen2Thread();
            newSramManager.SavePrinterCOMPort(comPortDropDown.value + 1);
            newSramManager.SavePrinterEnable(true);
        }
        else
        {
            Gen2_OC_Info.text = "開啟票機";
            SettingInfo.text = "票機關閉";
            Debug.Log("票機關閉");
            keepConvert = false;
            printer_PanelExit.interactable = false;
            printer_PanelOpenClose.interactable = false;
            comPortDropDown.interactable = false;
            mod_Gen2.CloseGen2();
            newSramManager.SavePrinterCOMPort(comPortDropDown.value + 1);
            newSramManager.SavePrinterEnable(false);
        }
    }
    float waitPrinterOpenTime = 3f;
    public IEnumerator PrinterOpen()
    {
        printer_PanelExit.interactable = false;
        printer_PanelOpenClose.interactable = false;
        comPortDropDown.interactable = false;

        yield return new WaitForSecondsRealtime(waitPrinterOpenTime);

        printer_PanelExit.interactable = true;
        printer_PanelOpenClose.interactable = true;
        comPortDropDown.interactable = true;

        if (PrinterDisconnect)
        {
            SettingInfo.text = "Port設置錯誤或者線脫落";
            keepConvert = false;
        }
        else
        {
            SettingInfo.text = "票機開啟成功";
            keepConvert = true;
            PrinterDisconnect = false;
            StartCoroutine("ConvertToBits");
        }
    }

    public void PrinterOpen_()
    {
        if (keepConvert) return;
        SettingInfo.text = "票機開啟成功";
        keepConvert = true;
        PrinterDisconnect = false;
        StartCoroutine("ConvertToBits");
    }

    public void SettingInfomation()
    {
        if (Mod_Gen2.PrinterWorking && mod_Gen2.ReciveLoop)
        {
            if (PrinterDisconnect) SettingInfo.text = "Port設置錯誤或者線脫落";
            else SettingInfo.text = "票機已啟動";
        }
        else
        {
            SettingInfo.text = "票機未啟動";
        }
    }

    public void ChangeCOMPort()
    {
        Gen2_Data.Gen2Comport = "COM" + (comPortDropDown.value + 1);
        Debug.Log("COM" + (comPortDropDown.value + 1));
    }
    #endregion
#endif
}

public static class Gen2_Data
{
#if Server
    #region Server
    public static string Gen2Comport = "COM1";
    public static double TicketOutMaxLimit = 5000000;
    public static int TicketOutWaitSever = 2;
    public static double TicketOutValue = 0;
    public static int GenEnableWaitServer = 2;
    public static bool GenOpen = false;
    public static bool PinterHardStop = false;
    public static bool FirstTryOpenPrinter = false;
    #endregion
#endif
}