using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Globalization;

//[ExecuteInEditMode]
public class Mod_Gen2 : MonoBehaviour
{
#if Server
    #region Server
    Mod_Gen2_Status gen2Status;
    Mod_TextController mod_AllErrorTextController;
    //建立GEN2 RS232連線
    public SerialPort Gen2Port = new SerialPort(Gen2_Data.Gen2Comport, 38400, Parity.None, 8, StopBits.One);
    private Thread receiveThread; //接收
    private Thread sendThread;    //發送
    int Count;
    public bool ReciveLoop; //持續接收迴圈
    public static bool PrinterWorking = false;

    void Awake()
    {
        gen2Status = FindObjectOfType<Mod_Gen2_Status>();
        mod_AllErrorTextController = FindObjectOfType<Mod_TextController>();
    }

    public void StartGen2Thread()
    {
        if (Gen2Port.IsOpen) return;
        //PortSet
        Debug.Log("Printer Try Open");
        Gen2Port.PortName = Gen2_Data.Gen2Comport;
        Gen2Port.BaudRate = 38400;
        Gen2Port.Parity = Parity.None;
        Gen2Port.StopBits = StopBits.One;
        Gen2Port.DataBits = 8;
        Gen2Port.Handshake = Handshake.None;
        Gen2Port.DtrEnable = true;
        Gen2Port.RtsEnable = true;
        Debug.Log(Gen2Port + "(" + Gen2Port.BaudRate + ")");

        Gen2Port.ReadTimeout = 1000;
        Gen2Port.WriteTimeout = 1000;

        try
        {
            //StartTheard
            Gen2Port.Open();
            ReciveLoop = true;
            receiveThread = new Thread(ReceiveThread);
            receiveThread.IsBackground = true;
            sendThread = new Thread(SendThread);
            sendThread.IsBackground = true;
            receiveThread.Start();
            sendThread.Start();
            gen2Status.SettingInfo.text = "票機開啟中...";
            StartCoroutine(gen2Status.PrinterOpen());
        }
        catch (Exception ex)
        {
            gen2Status.Gen2_OC_Info.text = "開啟票機";
            gen2Status.SettingInfo.text = "Port已被使用或者不存在";
            Debug.Log("Port has already been used or not exist");
        }
    }

    float autoRePrinterTime = 0;
    void Update()
    {
        if (Gen2Port.IsOpen)
        {
            PrinterWorking = true;
            autoRePrinterTime = 0f;
        }
        else
        {
            if (!Gen2_Data.FirstTryOpenPrinter && Gen2_Data.GenOpen && !Mod_Data.IOLock)
            {
                if (autoRePrinterTime >= 5f)
                {
                    Debug.Log("TryOpen");
                    gen2Status.OpenOrCloserGen2();
                    autoRePrinterTime = 0f;
                }
                else
                {
                    autoRePrinterTime += Time.unscaledDeltaTime;
                }
            }

            PrinterWorking = false;
        }
    }

    private void SendThread()
    {
        while (ReciveLoop)
        {
            try
            {
                if (Gen2Port.IsOpen)
                {
                    Thread.Sleep(1000);
                    Gen2Port.DiscardInBuffer();
                    Gen2Port.WriteLine("^Se|^"); //查詢機器狀態
                                                 //Debug.Log("Open");
                }
            }
            catch (Exception ex)
            {
                Mod_Gen2_Status.PrinterDisconnect = true;
                Debug.Log("SendThread Exception: " + ex.GetType().Name);
            }
        }
    }

    int reTimes = 0;
    int disconnectTimes = 0;

    private void ReceiveThread()//接收迴圈
    {
        disconnectTimes = 0;
        while (ReciveLoop)
        {
            if (this.Gen2Port != null && this.Gen2Port.IsOpen)
            {
                byte[] buffer = new byte[Gen2Port.ReadBufferSize + 1];
                int Bytes = 0;
                try
                {
                    int count1 = Gen2Port.Read(buffer, 0, Gen2Port.ReadBufferSize);
                    Bytes = Gen2Port.Read(buffer, 0, buffer.Length);
                    if (Bytes == 0)
                    {
                        continue;
                    }
                    else
                    {
                        mod_AllErrorTextController.printerTextBool[0] = false; //斷線
                        Mod_Gen2_Status.PrinterDisconnect = false;
                        if (disconnectTimes > 0) gen2Status.PrinterOpen_();
                        disconnectTimes = 0;
                        reTimes = 0;
                        string strbytes = Encoding.Default.GetString(buffer, 0, count1);
                        //Debug.Log("strbytes: " + strbytes);
                        gen2Status.strbytes = strbytes;
                    }
                }
                catch (Exception ex)
                {
                    mod_AllErrorTextController.printerTextBool[0] = true; //斷線

                    if (disconnectTimes > 2 && reTimes < 1)
                    {
                        if (Gen2Port.IsOpen) Gen2Port.Write("^r|^");
                        Debug.Log("Printer Reset");
                        reTimes++;
                    }

                    if (disconnectTimes > 5)
                    {
                        gen2Status.keepConvert = true;
                        Mod_Gen2_Status.PrinterDisconnect = true;
                    }

                    if (ex.GetType() == typeof(ArgumentNullException) || ex.GetType() == typeof(InvalidOperationException))
                    {
                        Debug.Log("Printer Can't Read");
                    }

                    if (ex.GetType() == typeof(ThreadAbortException))
                    {
                        Debug.Log("Printer Abort");
                    }

                    if (ex.GetType() == typeof(TimeoutException))
                    {
                        disconnectTimes++;
                        //gen2Status.SettingInfo.text = "Port設置錯誤或者線脫落";
                        Debug.Log("Printer Disconnect or plug not right");
                    }
                }
            }
            Thread.Sleep(500);
        }
    }

    public void CloseGen2()
    {
        //.Join()等待執行續執行完並停止動作，可以避免在讀取資料突然中斷Port鎖死
        ReciveLoop = false;
        receiveThread.Join();
        Debug.Log("receiveThread Stop");

        sendThread.Join();
        Debug.Log("sendThread Stop");

        Gen2Port.Close();
        Debug.Log("Gen2Port Stop");
        Gen2Port.Dispose();
        GC.Collect();

        Mod_Data.printerError = false;
        gen2Status.printer_PanelExit.interactable = true;
        gen2Status.printer_PanelOpenClose.interactable = true;
        gen2Status.comPortDropDown.interactable = true;
        Debug.Log("All Close");
    }


    void OnApplicationQuit()
    {
        ReciveLoop = false;
        Gen2Port.Close();
        receiveThread.Abort();
        sendThread.Abort();
        Debug.Log("Application close");
    }

    public void PrintTicket()
    {
        try
        {
            //string command = "^P|0|1|00-5671-4085-4797-7327|On Casino Floor|9295 Prototype Drive|Reno NV89511|4|A|00-5671-4085-4797-7327|09/29/1999|09:09:23|TICKET # 0045|ZERO DOLLARS ANDFIFTY CENTS|G|$$$$$$$$0.50|J|30 days|MACHINE# 0003-031|005671408547977327|^";

            string command = "^P|" + Mod_Data.printModel + "|" + Mod_Data.count + "|  " + Mod_Data.ticketExpirationDate + "|" + Mod_Data.ESTABLISHMENT + "|" + Mod_Data.LOCATION + "|" + Mod_Data.CITY_STATE_ZIP +
           "|||" + Mod_Data.serial2 + "|" + Mod_Data.YYMMDD + "|" + Mod_Data.HHMMSS + "|TICKET # " + Mod_Data.ticketNumber + "|" + "|" +
            "|" + Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty)) + " POINTS" + "||60 Days" + "|" + "MACHINE# " + Mod_Data.machineNumbear + "|" + Mod_Data.barcode + "|^";
            
            if (Gen2Port.IsOpen)
            {
                Gen2Port.Write(command); //出票
            }
            else
            {
                Gen2_Data.PinterHardStop = true;
                Mod_Data.printerError = true;
            }

            ClearError();
        }
        catch (Exception ex)
        {
            Gen2_Data.PinterHardStop = true;
            Mod_Data.printerError = true;
            Debug.Log("PrintTicket Exception: " + ex);
        }
        //Count = 0;
        //InvokeRepeating("delayUnLock", 0.0001f, 1);
    }

    void delayUnLock()
    {
        Count += 1;
        if (Count == 10)
        {
            Mod_Data.PrinterTicket = false;
            Debug.Log("TicketButton OK");
            CancelInvoke();
        }
    }

    public void PrinterReset()
    {
        if (Gen2Port.IsOpen) Gen2Port.Write("^r|^");//reset printer
    }

    public void ClearError()
    {
        if (Gen2Port.IsOpen) Gen2Port.Write("^C|^");//Clear error status
    }

    public void FeedPaper()
    {
        if (Gen2Port.IsOpen) Gen2Port.Write("^f|I|^");//feed papaer
    }

    #endregion
#endif
}
