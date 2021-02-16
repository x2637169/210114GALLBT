using System.Collections;
using UnityEngine;
using System.Threading;
using System.IO.Ports;
using System;
using System.Text.RegularExpressions;

public class JCM_Bill_Acceptor : MonoBehaviour
{
#if Server
    #region Server
    Mod_Client mod_Client;
    Mod_TextController Mod_TextController;
    NewSramManager newSramManager;
    SerialPort mySerialPort = new SerialPort(BillAcceptorSettingData.COMPort, 9600);
    public Thread RequestThread, InputCommand;
    public bool Enable = true;
    public int ReStart = 0;
    public bool ThreadStop = false;
    public byte[] vals = { 0xFC, 0x05, 0x11, 0x27, 0x56 };
    byte[] valsRe = { 0xFC, 0x05, 0x40, 0x2B, 0x15 };
    byte[] buffera;
    byte[][] StartVals =
        {
        new byte[]{ 0xFC ,0x07 ,0xC1 ,0x00 ,0x00 ,0xF1 ,0xEF },
        new byte[]{ 0xFC ,0x07 ,0xC0 ,0x00 ,0x00 ,0x2D ,0xB5 },
        new byte[]{ 0xFC ,0x07 ,0xC5 ,0x03 ,0x00 ,0xF8 ,0xA6 },
        new byte[]{ 0xFC ,0x05 ,0x88 ,0x6F ,0x5F },
        new byte[]{ 0xFC ,0x05 ,0x8A ,0x7D ,0x7C },
        new byte[]{ 0xFC ,0x06 ,0xC2 ,0x00 ,0xDC ,0xCF },
        new byte[]{ 0xFC ,0x07 ,0xC6 ,0x01 ,0x12 ,0xBF ,0x49 },
        new byte[]{ 0xFC ,0x06 ,0xC7 ,0xFC ,0x87 ,0x8C },
        new byte[]{ 0xFC ,0x05 ,0x70 ,0xA8 ,0x24 },
        new byte[]{ 0xFC ,0x07 ,0xF0 ,0x20 ,0x93 ,0xA2 ,0xB6 },
        new byte[]{ 0xFC ,0x08 ,0xF0 ,0x20 ,0xD1 ,0x01 ,0x0D ,0x88 },
        new byte[]{ 0xFC ,0x07 ,0xF0 ,0x20 ,0xA2, 0xA8 ,0x96 },
        new byte[]{ 0xFC ,0x07 ,0xF0 ,0x20 ,0x90 ,0x39 ,0x84 },
        new byte[]{ 0xFC ,0x08 ,0xF0 ,0x20 ,0xD2 ,0x1E ,0x13 ,0x4A }
    };
    byte[] Inhibit = { 0xFC, 0x06, 0xC3, 0x00, 0x04, 0xD6 };
    byte[] hold = { 0xFC, 0x06, 0x44, 0xFF, 0xB8, 0x18 };
    byte[] Stack2 = { 0xFC, 0x05, 0x42, 0x39, 0x36 };
    byte[] ack = { 0xFC, 0x05, 0x50, 0xAA, 0x05 };
    byte[] acc = { 0xFC, 0x06, 0x44, 0xFF, 0xB8, 0x18 };
    byte[] Return = { 0xFC, 0x05, 0x43, 0xB0, 0x27 };
    byte[] RE = { 0xFC, 0x05, 0x12, 0xBC, 0x64 };
    byte[] InhibitClose = { 0xFC, 0x06, 0xC3, 0x01, 0x8D, 0xC7 };
    // Use this for initialization
    void Start()
    {
        Mod_TextController = FindObjectOfType<Mod_TextController>();
        mod_Client = FindObjectOfType<Mod_Client>();
        newSramManager = FindObjectOfType<NewSramManager>();
    }

    public void Open_JCM_Bill()
    {

        mySerialPort.PortName = BillAcceptorSettingData.COMPort;
        mySerialPort.BaudRate = int.Parse("9600");
        mySerialPort.Parity = Parity.Even;
        mySerialPort.StopBits = StopBits.One;
        mySerialPort.DataBits = 8;
        mySerialPort.Handshake = Handshake.None;
        mySerialPort.RtsEnable = true;
        try
        {
            mySerialPort.Open();
            RequestThread = new Thread(StatusRequest);
            RequestThread.IsBackground = true;
            InputCommand = new Thread(InputcThread);
            InputCommand.IsBackground = true;
            //RequestThread.Start();
            ////Debug.Log(vals[0] + " " + vals[1] + " " + vals[2] + " " + vals[3] + " " + vals[4]);
            //mySerialPort.Write(valsRe, 0, 5);
            if (!RequestThread.IsAlive)
            {
                RequestThread.Start();
            }
            if (!InputCommand.IsAlive)
            {
                InputCommand.Start();
            }
        }
        catch (Exception ex)
        {
            //Debug.Log("Port has already been used or not exist");
        }
    }

    public void BillAcceptorEnable(bool enable)
    {
        BillAcceptorSettingData.GetOrderType = "";
        if (mySerialPort.IsOpen)
        {
            if (enable)
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(Inhibit, 0, 6);
                Enable = true;
            }
            else
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(InhibitClose, 0, 6);
                Enable = false;
            }
        }
    }

    public void StackReturn(bool StackOrReturn) //true:Stack    false:Return
    {
        if (mySerialPort.IsOpen)
        {
            if (StackOrReturn)
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(Stack2, 0, 5);
            }
            else
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(Return, 0, 5);
            }
        }
        BillAcceptorSettingData.JCM_WaitRespond = false;
    }

    bool atleastDoOnce = false;
    float inhbitTime = 0f;
    float holdTime = 0f;
    int inhbitCount = 0;
    int inhbitCloseCount = 0;
    //Update is called once per frame
    void Update()
    {
        if (BillAcceptorSettingData.BillOpen)
        {
            if (!BillAcceptorSettingData.BillOpenClose)
            {
                Mod_TextController.allTextBool[10] = true;
                if (!Mod_TextController.ErrorTextRunning) Mod_TextController.RunErrorTextBool = true;
            }
            else
            {
                Mod_TextController.allTextBool[10] = false;
            }
        }
        else
        {
            Mod_TextController.allTextBool[10] = false;
        }

        if (Buffer == 0x1A && Mod_Data.inBaseSpin && !Mod_TimeController.GamePasue && !Mod_Data.handpay && !Mod_Data.PrinterTicket && Mod_Data.Win == 0 && (!Enable || inhbitCount == 0) && mySerialPort.IsOpen)
        {
            if (inhbitTime > 1f)
            {
                Debug.Log("Inhibit!");
                if (mySerialPort.IsOpen) mySerialPort.Write(Inhibit, 0, 6);
                Enable = true;
                atleastDoOnce = true;
                inhbitTime = 0f;
                inhbitCount++;
                inhbitCloseCount = 0;
            }
            else
            {
                inhbitTime += Time.unscaledDeltaTime;
            }
        }
        else if (Buffer == 0x11 && (!Mod_Data.inBaseSpin || Mod_TimeController.GamePasue || Mod_Data.handpay || Mod_Data.PrinterTicket || Mod_Data.Win != 0) && (Enable || inhbitCloseCount == 0) && mySerialPort.IsOpen)
        {
            Debug.Log("InhibitClose!");
            if (mySerialPort.IsOpen) mySerialPort.Write(InhibitClose, 0, 6);
            Enable = false;
            atleastDoOnce = false;
            inhbitCloseCount++;
            inhbitCount = 0;
        }

        if (Buffer == 0x19)
        {
            if (holdTime >= 3)
            {
                Debug.Log("Retrun!");
                if (mySerialPort.IsOpen) mySerialPort.Write(Return, 0, 5);
                holdTime = 0;
            }
            else
            {
                holdTime += Time.unscaledDeltaTime;
            }
        }
    }

    void InputcThread()
    {
        try
        {
            while (!ThreadStop)
            {
                mySerialPort.Write(vals, 0, 5);
                Thread.Sleep(1000);
                if (ReStart > 3)
                {
                    ReStart = 0;
                    mySerialPort.DiscardInBuffer();
                }
                while (BillAcceptorSettingData.JCM_WaitRespond && !ThreadStop)
                {
                    Thread.Sleep(5);
                }
            }
        }
        catch (Exception ex)
        {
            if (ex.GetType() != typeof(ThreadAbortException))
            {
                Debug.Log("BV Disconnect");
            }
            else
            {
                Debug.Log(ex);
            }
        }
    }

    byte[] buffer;
    byte[] bufferTwo;
    public static bool TicketCertification = false;
    void StatusRequest()
    {

        mySerialPort.DiscardInBuffer();
        mySerialPort.Write(valsRe, 0, 5);
        Thread.Sleep(500);

        for (int i = 0; i < StartVals.Length; i++)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.DiscardInBuffer();
                mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
                ReQuest();
            }
        }
        while (!ThreadStop)
        {
            mySerialPort.DiscardInBuffer();
            if (this.mySerialPort != null && this.mySerialPort.IsOpen)
            {
                try
                {
                    mySerialPort.Write(vals, 0, 5);
                    ReQuest();
                    ReQuestTrans(bufferTwo);
                }
                catch (Exception ex)
                {
                    BillAcceptorSettingData.GameCanPlay = true;
                    BillAcceptorSettingData.CheckIsInBaseSpin = false;
                    if (ex.GetType() != typeof(ThreadAbortException))
                    {
                        Debug.Log("StatusRequest BV Disconnect");
                    }
                    else
                    {
                        Debug.Log("StatusRequest " + ex);
                    }
                }

            }
            Thread.Sleep(300);
        }
    }

    int DisconnectCount = 0;
    public void ReQuest()
    {
    ReadAgain:
        try
        {
            bufferTwo = null;
            Thread.Sleep(300);
            if (mySerialPort.BytesToRead > 0)
            {
                DisconnectCount = 0;
                if (!string.IsNullOrEmpty(BillAcceptorSettingData.ErrorMessage)) BillAcceptorSettingData.ErrorMessage = null;
                buffer = new byte[mySerialPort.ReadBufferSize + 1];
                mySerialPort.Read(buffer, 0, buffer.Length);
                Thread.Sleep(300);
                if (buffer[0] == 0xFC)
                {
                    bufferTwo = new byte[buffer[1]];
                    for (int i = 0; i < bufferTwo.Length; i++)
                    {
                        bufferTwo[i] = buffer[i];
                    }
                    if (bufferTwo[1] == 0x18 && bufferTwo[2] == 0x13)
                    {
                        BillAcceptorSettingData.CashOrTicketIn = true;
                        //Debug.Log("TicketIn");
                        string bufferString = ByteArrayToString(bufferTwo);
                        if (bufferString.Length >= 44) bufferString = bufferString.Substring(8, 36);
                        //Debug.Log(bufferString);
                        Regex regex = new Regex(@"[A-Z,a-z]");  //除去機台序號英文字母
                        if (regex.IsMatch(bufferString) || (bufferString.Length >= 37 || bufferString.Length <= 35))
                        {
                            //Debug.Log("BytesToRead OG : " + ByteArrayToString(bufferTwo));
                            mySerialPort.DiscardOutBuffer();
                            mySerialPort.DiscardInBuffer();
                            //Debug.Log("ReadAgain" + " bufferString: " + bufferString.Length + " regex: " + regex.IsMatch(bufferString));
                            goto ReadAgain;
                        }
                        else
                        {
                            TestTicketReQuest();
                        }
                    }
                    else if (bufferTwo[1] == 0x06 && bufferTwo[2] == 0x13)
                    {
                        BillAcceptorSettingData.CashOrTicketIn = true;
                        //Debug.Log("CashIn");
                        CashIn();
                    }
                    //Debug.Log(ByteArrayToString(bufferTwo));
                }
                mySerialPort.DiscardOutBuffer();
                mySerialPort.DiscardInBuffer();
            }
            else
            {
                DisconnectCount++;
            }

            if (DisconnectCount > 10)
            {
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Disconnect";
            }
        }
        catch (Exception ex)
        {
            if (ex.GetType() != typeof(ThreadAbortException))
            {
                Debug.Log("ReQuest BV Disconnect");
            }
            else
            {
                Debug.Log("ReQuest " + ex);
            }
        }
    }

    public static bool cashin_waitServer = false;
    public void CashIn()
    {
        if (mySerialPort.IsOpen) mySerialPort.Write(hold, 0, 6);

        Thread.Sleep(500);

        if (!Enable || !BillAcceptorSettingData.CheckIsInBaseSpin)
        {
            if (mySerialPort.IsOpen) mySerialPort.Write(Return, 0, 5);
            Debug.Log("CheckIsInBaseSpin: " + BillAcceptorSettingData.CheckIsInBaseSpin + " StopCashIn: " + BillAcceptorSettingData.StopCashIn);
            return;
        }

        BillAcceptorSettingData.billstack = false;
        BillAcceptorSettingData.billreturn = false;
        BillAcceptorSettingData.StopCashIn = true;
        Mod_TextController.CleanText = true;
        TicketCertification = false;

        if (!cashin_waitServer)
        {
            switch (bufferTwo[3])
            {
                case 0x64:
                    BillAcceptorSettingData.StopCashIn = true;
                    //Debug.Log("TWD 100 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "100";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
                case 0x65:
                    BillAcceptorSettingData.StopCashIn = true;
                    //Debug.Log("TWD 200 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "200";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
                case 0x66:
                    BillAcceptorSettingData.StopCashIn = true;
                    //Debug.Log("TWD 500 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "500";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
                case 0x67:
                    BillAcceptorSettingData.StopCashIn = true;
                    //Debug.Log("TWD 1000 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "1000";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
                case 0x68:
                    BillAcceptorSettingData.StopCashIn = true;
                    //Debug.Log("TWD 2000 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "2000";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
            }

            TicketCertification = false;

            if (BillAcceptorSettingData.TicketIn) Mod_Data.cash = int.Parse(BillAcceptorSettingData.TicketValue);

            //Debug.Log("While Start");
            while (BillAcceptorSettingData.JCM_WaitRespond && !ThreadStop)
            {
                Thread.Sleep(5);
            }
            //Debug.Log("While End");

            if (BillAcceptorSettingData.StackReturnBool) TicketCertification = true;
            else TicketCertification = false;

            if (TicketCertification)
            {
                //Debug.Log("Stack2");
                cashin_waitServer = true;
                if (mySerialPort.IsOpen) mySerialPort.Write(Stack2, 0, 5);
                BillAcceptorSettingData.billstack = true;
            }
            else
            {
                //Debug.Log("Return");
                if (mySerialPort.IsOpen) mySerialPort.Write(Return, 0, 5);
                BillAcceptorSettingData.billreturn = true;
                BillAcceptorSettingData.TicketValue = null;
                BillAcceptorSettingData.TicketType = null;
            }
        }


        cashin_waitServer = false;
        //if (TicketCertification)
        //{
        //    mySerialPort.Write(Stack2, 0, 5);
        //    ReQuest();
        //}
        //else
        //{
        //    mySerialPort.Write(Return, 0, 5);
        //    ReQuest();
        //}
    }

    int bufferNullCount = 0;
    int invalidCommandCount = 0;
    public byte Buffer;
    public void ReQuestTrans(byte[] TransBuffer)
    {
        if (TransBuffer == null)
        {
            //Debug.Log("Buffer Is Null");

            bufferNullCount++;
            if (bufferNullCount >= 3)
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(RE, 0, 5);
                BillAcceptorSettingData.ErrorBool = true;
                Mod_TextController.allTextBool[13] = true;
                if (!Mod_TextController.ErrorTextRunning) Mod_TextController.RunErrorTextBool = true;
            }
        }

        if (TransBuffer == null || TransBuffer.Length < 2) return;
        Buffer = TransBuffer[2];
        bufferNullCount = 0;
        Mod_TextController.allTextBool[13] = false;

        switch (TransBuffer[2])
        {
            case 0x05:
                //Debug.Log("ENQ [0x05]");
                break;
            case 0x11:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "ENABLE";
                Mod_TextController.noticeTextBool[1] = false;
                Mod_TextController.noticeTextBool[2] = false;
                Mod_TextController.allTextBool[4] = false;
                Mod_TextController.allTextBool[5] = false;
                Mod_TextController.allTextBool[6] = false;
                Mod_TextController.allTextBool[7] = false;
                Mod_TextController.allTextBool[8] = false;
                Mod_Data.billStackOpenerror = false;
                BillAcceptorSettingData.CashOrTicketIn = false;
                BillAcceptorSettingData.GameCanPlay = true;
                atleastDoOnce = true;
                if (Mod_Data.inBaseSpin) BillAcceptorSettingData.CheckIsInBaseSpin = true;
                //Debug.Log("ENABLE (IDLING) [0x11]");
                break;
            case 0x12:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "Accepting";
                BillAcceptorSettingData.CashOrTicketIn = true;
                BillAcceptorSettingData.GameCanPlay = false;
                if (Mod_Data.inBaseSpin) BillAcceptorSettingData.CheckIsInBaseSpin = true;
                //Debug.Log("Accepting [0x12]");
                break;
            case 0x13:
                //Debug.Log("ESCROW [0x13]: " + ByteArrayToString(TransBuffer));
                BillAcceptorSettingData.CashOrTicketIn = true;
                BillAcceptorSettingData.GameCanPlay = false;
                if (Mod_Data.inBaseSpin) BillAcceptorSettingData.CheckIsInBaseSpin = true;
                break;
            case 0x14:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "STACKING";
                BillAcceptorSettingData.CashOrTicketIn = true;
                Debug.Log("STACKING [0x14]");
                break;
            case 0x15:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "VEND VALID";
                BillAcceptorSettingData.CashOrTicketIn = true;
                Debug.Log("VEND VALID [0x15]");
                Thread.Sleep(500);
                if (mySerialPort.IsOpen) mySerialPort.Write(ack, 0, 5);
                break;
            case 0x16:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "STACKED";
                BillAcceptorSettingData.CashOrTicketIn = true;
                Debug.Log("STACKED [0x16]");
                break;
            case 0x17:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "REJECTING";
                Mod_TextController.noticeTextBool[1] = true;
                //Debug.Log("REJECTING [0x17]: " + ByteArrayToString(TransBuffer));
                //BillAcceptorSettingData.JCM_WaitRespond = false;
                break;
            case 0x18:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                Mod_TextController.noticeTextBool[2] = true;
                BillAcceptorSettingData.NormalMessage = "RETURNING";
                //Debug.Log("RETURNING [0x18]");
                break;
            case 0x19:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "HOLDING";
                //Debug.Log("HOLDING [0x19]");
                break;
            case 0x1A:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "DISABLE (INHIBIT)";
                Mod_TextController.noticeTextBool[1] = false;
                Mod_TextController.noticeTextBool[2] = false;
                Mod_TextController.allTextBool[4] = false;
                Mod_TextController.allTextBool[5] = false;
                Mod_TextController.allTextBool[6] = false;
                Mod_TextController.allTextBool[7] = false;
                Mod_TextController.allTextBool[8] = false;
                Mod_Data.billStackOpenerror = false;
                BillAcceptorSettingData.CashOrTicketIn = false;
                BillAcceptorSettingData.GameCanPlay = true;
                atleastDoOnce = false;
                //Debug.Log("DISABLE (INHIBIT) [0x1A]");
                Thread.Sleep(500);
                if (Enable)
                {
                    if (mySerialPort.IsOpen) mySerialPort.Write(Inhibit, 0, 6);
                    ReQuest();
                }
                break;
            case 0x1B:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "INITIALIZE";
                Mod_TextController.allTextBool[4] = false;
                Mod_TextController.allTextBool[5] = false;
                Mod_TextController.allTextBool[6] = false;
                Mod_TextController.allTextBool[7] = false;
                Mod_TextController.allTextBool[8] = false;
                Mod_Data.billStackOpenerror = false;
                //Debug.Log("INITIALIZE [0x1B]");
                break;
            case 0x40:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "POWER UP";
                mySerialPort.DiscardInBuffer();
                if (mySerialPort.IsOpen) mySerialPort.Write(valsRe, 0, 5);
                Thread.Sleep(500);

                for (int i = 0; i < StartVals.Length; i++)
                {
                    if (mySerialPort.IsOpen)
                    {
                        mySerialPort.DiscardInBuffer();
                        mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
                        ReQuest();
                    }
                }
                //Debug.Log("POWER UP [0x40]");
                break;
            case 0x41:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "POWER UP WITH BILL IN ACCEPTOR";
                mySerialPort.DiscardInBuffer();
                if (mySerialPort.IsOpen) mySerialPort.Write(valsRe, 0, 5);
                Thread.Sleep(500);

                for (int i = 0; i < StartVals.Length; i++)
                {
                    if (mySerialPort.IsOpen)
                    {
                        mySerialPort.DiscardInBuffer();
                        mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
                        ReQuest();
                    }
                }
                //Debug.Log("POWER UP WITH BILL IN ACCEPTOR [0x41]");
                break;
            case 0x42:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "POWER UP WITH BILL IN STACKER";
                mySerialPort.DiscardInBuffer();
                if (mySerialPort.IsOpen) mySerialPort.Write(valsRe, 0, 5);
                Thread.Sleep(500);

                for (int i = 0; i < StartVals.Length; i++)
                {
                    if (mySerialPort.IsOpen)
                    {
                        mySerialPort.DiscardInBuffer();
                        mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
                        ReQuest();
                    }
                }
                //Debug.Log("POWER UP WITH BILL IN STACKER [0x42]");
                break;
            case 0x43:
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Stacker Full";
                BillAcceptorSettingData.NormalBool = false;
                BillAcceptorSettingData.NormalMessage = "";
                Mod_TextController.allTextBool[4] = true;
                //Debug.Log("Stacker Full [0x43]");
                break;
            case 0x44:
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Stacker Open";
                BillAcceptorSettingData.NormalBool = false;
                BillAcceptorSettingData.NormalMessage = "";
                Mod_TextController.allTextBool[5] = true;
                Mod_Data.billStackOpenerror = true;
                //Debug.Log("Stacker Open [0x44]");
                break;
            case 0x45:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "JAM IN Acceptor";
                //Debug.Log("JAM IN Acceptor [0x45]");
                break;
            case 0x46:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "JAM IN Stacker";
                //Debug.Log("JAM IN Stacker [0x46]");
                break;
            case 0x47:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "Pause";
                //Debug.Log("Pause [0x47]");
                break;
            case 0x48:

                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "CHEATED";
                //Debug.Log("CHEATED [0x48]");
                break;
            case 0x49:
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "FAILURE [0x49]: " + ByteArrayToString(TransBuffer);
                BillAcceptorSettingData.NormalBool = false;
                BillAcceptorSettingData.NormalMessage = "";
                Mod_TextController.allTextBool[6] = true;
                //Debug.Log("FAILURE [0x49]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x4A:
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Communication Error";
                BillAcceptorSettingData.NormalBool = false;
                BillAcceptorSettingData.NormalMessage = "";
                Mod_TextController.allTextBool[7] = true;
                //Debug.Log("Communication Error [0x4A]");
                break;
            case 0x4B:
                if (invalidCommandCount >= 5)
                {
                    BillAcceptorSettingData.ErrorBool = true;
                    BillAcceptorSettingData.ErrorMessage = "INVALID COMMAND";
                    BillAcceptorSettingData.NormalBool = false;
                    BillAcceptorSettingData.NormalMessage = "";
                    Mod_TextController.allTextBool[8] = true;
                }
                //Debug.Log("INVALID COMMAND [0x4B]");
                break;
            case 0x50:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "ACK";
                //Debug.Log("ACK [0x50]");
                break;

            case 0x80:
                //Debug.Log("ENABLE / DISABLE (DENOMI) [0x80]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x81:
                ////Debug.Log("SECURTY (DENOMI) [0x81]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x82:
                ////Debug.Log("COMMUNICATION MODE [0x82]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x83:
                ////Debug.Log("INHIBIT (ACCEPTOR) [0x83]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x84:
                ////Debug.Log("DIRECTION [0x84]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x85:
                ////Debug.Log("OPTIONAL FUNCTION [0x85]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x88:
                ////Debug.Log("VERSION INFOMATION [0x88]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x89:
                ////Debug.Log("BOOT VERSION INFOMATION [0x89]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x8A:
                ////Debug.Log("DENOMINATION [0x8A]: " + ByteArrayToString(TransBuffer));
                break;


            case 0xC0:
                ////Debug.Log("ENABLE / DISABLE (DENOMI) [0xC0]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC1:
                ////Debug.Log("SECURTY (DENOMI) [0xC1]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC2:
                ////Debug.Log("COMMUNICATION MODE [0xC2]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC3:
                ////Debug.Log("INHIBIT (ACCEPTOR) [0xC3]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC4:
                ////Debug.Log("DIRECTION [0xC4]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC5:
                ////Debug.Log("OPTIONAL FUNCTION [0xC5]: " + ByteArrayToString(TransBuffer));
                break;

            default:
                break;
        }

        if (TransBuffer[2] == 0x4B)
        {
            invalidCommandCount++;
        }
        else
        {
            invalidCommandCount = 0;
        }

        if ((TransBuffer[2] == 0x15 || TransBuffer[2] == 0x16 || TransBuffer[2] == 0x17) && Mod_TimeController.GamePasue)
        {
            ////Debug.Log("Hard Pasue In Buffer");
            BillAcceptorSettingData.BillAcceptorEnable = false;
            BillAcceptorSettingData.GetOrderType = "BillEnableDisable";
        }

        if (BillAcceptorSettingData.ErrorBool && !Mod_TextController.ErrorTextRunning) Mod_TextController.RunErrorTextBool = true;
    }

    public void TestTicketReQuest()
    {
        if (!Enable || !BillAcceptorSettingData.CheckIsInBaseSpin || Mod_Data.PrinterTicket)
        {
            if (mySerialPort.IsOpen) mySerialPort.Write(Return, 0, 5);
            Debug.Log("CheckIsInBaseSpin: " + BillAcceptorSettingData.CheckIsInBaseSpin + " StopCashIn: " + BillAcceptorSettingData.StopCashIn);
            return;
        }
        
        BillAcceptorSettingData.TicketInWaitSever = 2; //預設值
        BillAcceptorSettingData.billstack = false;
        BillAcceptorSettingData.billreturn = false;
        BillAcceptorSettingData.StopCashIn = true;
        Mod_TextController.CleanText = true;
        TicketCertification = false;
        byte[] TicketBuffer = new byte[18];
        for (int i = 0; i < 18; i++)
        {
            TicketBuffer[i] = bufferTwo[i + 4];
        }
        string TicketString = ByteArrayToString(TicketBuffer);
        string TicketStringRE = "";
        int a = 0;

        for (int i = 1; i < 36; i += 2)
        {
            TicketStringRE += TicketString.Substring(i, 1);

            if (i >= 5) a++;

            if (a >= 4)
            {
                if (i < 35) TicketStringRE += "-";
                a = 0;
            }
        }

        //Debug.Log("TicketString: " + TicketString);
        //Debug.Log("Ticket: " + TicketStringRE);
        //Debug.Log("OG : " + ByteArrayToString(bufferTwo));

        if (mySerialPort.IsOpen) mySerialPort.Write(hold, 0, 6);
        Thread.Sleep(500);

        if (TicketStringRE.Length >= 18)
        {
            TicketStringRE = TicketStringRE.Substring(2);
            Mod_Data.serial = TicketStringRE;
            //Debug.Log("Mod_Data Ticket: " + Mod_Data.serial);

            int ReadTicketTime = 0;
            //確保讀到票
            while (true)
            {
                if (Buffer == 0x12 || Buffer == 0x13 || BillAcceptorSettingData.CashOrTicketIn || ReadTicketTime >= 500 || !Enable)
                {
                    //Debug.Log("Buffer: " + Buffer + " BillAcceptorSettingData.CashOrTicketIn: " + BillAcceptorSettingData.CashOrTicketIn + " ReadTicketTime: " + ReadTicketTime + " Enable: " + Enable);
                    break;
                }
                Thread.Sleep(10);
                ReadTicketTime += 1;
            }
            //超出讀票時間不傳資料，直接退票
            if (ReadTicketTime < 500 && Enable)
            {
                //傳送資料給SERVER
                BillAcceptorSettingData.TicketInWaitSever = 2;
                Mod_Client_Data.ticketInAmount = null;
                mod_Client.SendToSever(Mod_Client_Data.messagetype.ticketin);

                while (true)  //等待伺服器回傳
                {
                    if (BillAcceptorSettingData.TicketInWaitSever != 2) break;

                    if (Mod_Client_Data.serverdisconnect)
                    {
                        while (true)
                        {
                            if (!Mod_Client_Data.serverdisconnect && Mod_Data.machineInit)
                            {
                                Thread.Sleep(1000);
                                if (BillAcceptorSettingData.TicketInWaitSever != 2) break;
                                mod_Client.SendToSever(Mod_Client_Data.messagetype.ticketin);
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            else
            {
                BillAcceptorSettingData.TicketInWaitSever = 0;
            }
        }

        //Debug.Log("BillAcceptorSettingData.TicketInWaitSever: " + BillAcceptorSettingData.TicketInWaitSever);

        // 0或其他數字 伺服器回傳票認證失敗
        // 1 伺服器回傳票認證成功
        if (BillAcceptorSettingData.TicketInWaitSever == 1)
        {
            BillAcceptorSettingData.TicketType = "Ticket";
            BillAcceptorSettingData.TicketIn = true;
            BillAcceptorSettingData.JCM_WaitRespond = true;
        }
        else
        {
            BillAcceptorSettingData.TicketIn = false;
            BillAcceptorSettingData.StackReturnBool = false;
            BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
            BillAcceptorSettingData.JCM_WaitRespond = false;
        }

        //Debug.Log("Ticket While Start");
        while (BillAcceptorSettingData.JCM_WaitRespond && !ThreadStop)
        {
            Thread.Sleep(5);
        }
        //Debug.Log("Ticket While End");

        if (BillAcceptorSettingData.StackReturnBool) TicketCertification = true;
        else TicketCertification = false;

        if (TicketCertification && !BillAcceptorSettingData.ErrorBool && !Mod_TextController.noticeTextBool[1] && !Mod_TextController.noticeTextBool[2] && Enable && mySerialPort.IsOpen)
        {
            Debug.Log("Stack2");
            if (mySerialPort.IsOpen) mySerialPort.Write(Stack2, 0, 5);
            BillAcceptorSettingData.billstack = true;
        }
        else
        {
            Debug.Log("Return");
            if (mySerialPort.IsOpen) mySerialPort.Write(Return, 0, 5);
            BillAcceptorSettingData.billreturn = true;
            BillAcceptorSettingData.TicketValue = null;
            BillAcceptorSettingData.TicketType = null;
        }
    }

    public static string ByteArrayToString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", "");

        /*StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}",b);
        return hex.ToString();*/
    }

    public void ResetBill()
    {
        if (mySerialPort.IsOpen) mySerialPort.Write(RE, 0, 5);
    }

    public void JCM_Bill_Close()
    {
        if (!mySerialPort.IsOpen) return;

        if (mySerialPort.IsOpen)
        {
            mySerialPort.Write(InhibitClose, 0, 6);
            mySerialPort.Close();
        }
        ThreadStop = true;
        if (RequestThread.IsAlive)
        {
            RequestThread.Abort();
        }
        if (InputCommand.IsAlive)
        {
            InputCommand.Abort();
        }

        ThreadStop = false;
    }

    void OnApplicationQuit()
    {
        if (mySerialPort.IsOpen)
        {
            mySerialPort.Write(InhibitClose, 0, 6);
            mySerialPort.Close();
        }
        ThreadStop = true;
        if (RequestThread.IsAlive)
        {
            RequestThread.Abort();
        }
        if (InputCommand.IsAlive)
        {
            InputCommand.Abort();
        }
    }
    #endregion
#else
    #region !Server
    SerialPort mySerialPort = new SerialPort(BillAcceptorSettingData.COMPort, 9600);
    public Thread RequestThread, InputCommand;
    bool Enable = true;
    public int ReStart = 0;
    public bool ThreadStop = false;
    public byte[] vals = { 0xFC, 0x05, 0x11, 0x27, 0x56 };
    byte[] valsRe = { 0xFC, 0x05, 0x40, 0x2B, 0x15 };
    byte[] buffera;
    byte[][] StartVals =
        {
        new byte[]{ 0xFC ,0x07 ,0xC1 ,0x00 ,0x00 ,0xF1 ,0xEF },
        new byte[]{ 0xFC ,0x07 ,0xC0 ,0x00 ,0x00 ,0x2D ,0xB5 },
        new byte[]{ 0xFC ,0x07 ,0xC5 ,0x03 ,0x00 ,0xF8 ,0xA6 },
        new byte[]{ 0xFC ,0x05 ,0x88 ,0x6F ,0x5F },
        new byte[]{ 0xFC ,0x05 ,0x8A ,0x7D ,0x7C },
        new byte[]{ 0xFC ,0x06 ,0xC2 ,0x00 ,0xDC ,0xCF },
        new byte[]{ 0xFC ,0x07 ,0xC6 ,0x01 ,0x12 ,0xBF ,0x49 },
        new byte[]{ 0xFC ,0x06 ,0xC7 ,0xFC ,0x87 ,0x8C },
        new byte[]{ 0xFC ,0x05 ,0x70 ,0xA8 ,0x24 },
        new byte[]{ 0xFC ,0x07 ,0xF0 ,0x20 ,0x93 ,0xA2 ,0xB6 },
        new byte[]{ 0xFC ,0x08 ,0xF0 ,0x20 ,0xD1 ,0x01 ,0x0D ,0x88 },
        new byte[]{ 0xFC ,0x07 ,0xF0 ,0x20 ,0xA2, 0xA8 ,0x96 },
        new byte[]{ 0xFC ,0x07 ,0xF0 ,0x20 ,0x90 ,0x39 ,0x84 },
        new byte[]{ 0xFC ,0x08 ,0xF0 ,0x20 ,0xD2 ,0x1E ,0x13 ,0x4A }
    };
    byte[] Inhibit = { 0xFC, 0x06, 0xC3, 0x00, 0x04, 0xD6 };
    byte[] hold = { 0xFC, 0x06, 0x44, 0xFF, 0xB8, 0x18 };
    byte[] Stack2 = { 0xFC, 0x05, 0x42, 0x39, 0x36 };
    byte[] ack = { 0xFC, 0x05, 0x50, 0xAA, 0x05 };
    byte[] acc = { 0xFC, 0x06, 0x44, 0xFF, 0xB8, 0x18 };
    byte[] Return = { 0xFC, 0x05, 0x43, 0xB0, 0x27 };
    byte[] RE = { 0xFC, 0x05, 0x12, 0xBC, 0x64 };
    byte[] InhibitClose = { 0xFC, 0x06, 0xC3, 0x01, 0x8D, 0xC7 };
    // Use this for initialization
    void Start()
    {

        //mySerialPort.PortName = "COM4";
        //mySerialPort.BaudRate = int.Parse("9600");
        //mySerialPort.Parity = Parity.Even;
        //mySerialPort.StopBits = StopBits.One;
        //mySerialPort.DataBits = 8;
        //mySerialPort.Handshake = Handshake.None;
        //mySerialPort.RtsEnable = true;
        //mySerialPort.Open();
        //RequestThread = new Thread(StatusRequest);
        //RequestThread.IsBackground = true;
        //InputCommand = new Thread(InputcThread);
        //InputCommand.IsBackground = true;
        ////RequestThread.Start();
        //print(vals[0] + " " + vals[1] + " " + vals[2] + " " + vals[3] + " " + vals[4]);
        ////mySerialPort.Write(valsRe, 0, 5);
        //if (!RequestThread.IsAlive)
        //{
        //    RequestThread.Start();
        //}
        //if (!InputCommand.IsAlive)
        //{
        //    InputCommand.Start();
        //}
    }

    public void Open_JCM_Bill()
    {
        mySerialPort.PortName = BillAcceptorSettingData.COMPort;
        mySerialPort.BaudRate = int.Parse("9600");
        mySerialPort.Parity = Parity.Even;
        mySerialPort.StopBits = StopBits.One;
        mySerialPort.DataBits = 8;
        mySerialPort.Handshake = Handshake.None;
        mySerialPort.RtsEnable = true;

        Debug.Log("Open_JCM_Bill Comport: " + mySerialPort.PortName);
        try
        {
            mySerialPort.Open();
            RequestThread = new Thread(StatusRequest);
            RequestThread.IsBackground = true;
            InputCommand = new Thread(InputcThread);
            InputCommand.IsBackground = true;
            //RequestThread.Start();
            //Debug.Log(vals[0] + " " + vals[1] + " " + vals[2] + " " + vals[3] + " " + vals[4]);
            mySerialPort.Write(valsRe, 0, 5);
            if (!RequestThread.IsAlive)
            {
                RequestThread.Start();
            }
            if (!InputCommand.IsAlive)
            {
                InputCommand.Start();
            }
        }
        catch (Exception ex)
        {
            BillAcceptorSettingData.ErrorBool = true;
            BillAcceptorSettingData.ErrorMessage = "Disconnect";
            Debug.Log(ex);
        }
    }

    public void BillAcceptorEnable(bool enable)
    {
        if (mySerialPort.IsOpen)
        {
            BillAcceptorSettingData.GetOrderType = "";
            if (enable)
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(Inhibit, 0, 6);
                Enable = true;
            }
            else
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(InhibitClose, 0, 6);
                Enable = false;
            }
        }
    }

    public void StackReturn(bool StackOrReturn) //true:Stack    false:Return
    {
        if (mySerialPort.IsOpen)
        {
            if (StackOrReturn)
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(Stack2, 0, 5);
            }
            else
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(Return, 0, 5);
            }
        }
        BillAcceptorSettingData.JCM_WaitRespond = false;
    }

    bool atleastDoOnce = false;
    // Update is called once per frame
    void Update()
    {
        if (mySerialPort.IsOpen)
        {
            if (Buffer == 0x1A && Mod_Data.inBaseSpin && (!Enable || !atleastDoOnce))
            {
                //Debug.Log("Inhibit!");
                if (mySerialPort.IsOpen) mySerialPort.Write(Inhibit, 0, 6);
                Enable = true;
                atleastDoOnce = true;
            }
            else if (Buffer == 0x11 && !Mod_Data.inBaseSpin && (Enable || atleastDoOnce))
            {
                //Debug.Log("InhibitClose!");
                if (mySerialPort.IsOpen) mySerialPort.Write(InhibitClose, 0, 6);
                Enable = false;
                atleastDoOnce = false;
            }
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    mySerialPort.Write(InhibitClose, 0, 6);
        //    mySerialPort.Close();
        //    ThreadStop = true;
        //}

        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    mySerialPort.Open();
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    if (!InputCommand.IsAlive)
        //    {
        //        InputCommand.Start();
        //    }
        //    if (!RequestThread.IsAlive)
        //    {
        //        RequestThread.Start();
        //    }
        //}

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     mySerialPort.Write(vals, 0, 5);
        //     byte[] buffer = new byte[mySerialPort.ReadBufferSize + 1];
        //     mySerialPort.Read(buffer, 0, buffer.Length);
        // }

        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     mySerialPort.Write(Inhibit, 0, 6);
        //     MainReQuest();
        // }

        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     mySerialPort.Write(valsRe, 0, 5);
        //     byte[] buffer = new byte[mySerialPort.ReadBufferSize + 1];
        //     mySerialPort.Read(buffer, 0, buffer.Length);
        // }
        // if (Input.GetKeyDown(KeyCode.H))
        // {
        //     mySerialPort.Write(hold, 0, 6);
        // }

        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     mySerialPort.Write(Stack2, 0, 5);
        // }
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     mySerialPort.Write(ack, 0, 5);
        //     MainReQuest();
        // }

        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     mySerialPort.Write(Return, 0, 5);
        //     MainReQuest();
        // }
    }

    int disconnectCount_InputcThread = 0;
    void InputcThread()
    {
        try
        {
            while (!ThreadStop)
            {
                mySerialPort.Write(vals, 0, 5);
                Thread.Sleep(1000);
                if (ReStart > 3)
                {
                    ReStart = 0;
                    mySerialPort.DiscardInBuffer();
                }
                while (BillAcceptorSettingData.JCM_WaitRespond && !ThreadStop)
                {
                    Thread.Sleep(5);
                }
                disconnectCount_InputcThread = 0;
            }
        }
        catch (Exception ex)
        {
            disconnectCount_InputcThread++;

            if (disconnectCount_InputcThread >= 10)
            {
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Disconnect";
            }

            Debug.Log(ex);
        }
    }

    //IEnumerator StartBV()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    for (int i = 0; i < StartVals.Length; i++)
    //    {
    //        if (mySerialPort.IsOpen)
    //        {
    //            mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
    //            MainReQuest();
    //        }
    //        yield return new WaitForSeconds(0.75f);

    //    }
    //}

    //public void readw()
    //{
    //    byte[] buffer = new byte[mySerialPort.ReadBufferSize + 1];
    //    int bytes = 0;
    //    try
    //    {
    //        int count1 = mySerialPort.Read(buffer, 0, mySerialPort.ReadBufferSize);
    //        bytes = mySerialPort.Read(buffer, 0, buffer.Length);

    //    }
    //    catch (Exception ex)
    //    {
    //        if (ex.GetType() != typeof(ThreadAbortException))
    //        {
    //            //Debug.Log("Printer Disconnect");
    //        }
    //    }
    //}

    byte[] buffer;
    byte[] bufferTwo;
    bool TicketCertification = false;
    int disconnectCount_StatusRequest = 0;
    void StatusRequest()
    {
        mySerialPort.DiscardInBuffer();
        mySerialPort.Write(valsRe, 0, 5);
        Thread.Sleep(500);

        for (int i = 0; i < StartVals.Length; i++)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.DiscardInBuffer();
                mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
                ReQuest();
            }
        }
        while (!ThreadStop)
        {
            mySerialPort.DiscardInBuffer();
            if (this.mySerialPort != null && this.mySerialPort.IsOpen)
            {
                try
                {
                    mySerialPort.Write(vals, 0, 5);
                    ReQuest();
                    ReQuestTrans(bufferTwo);
                    disconnectCount_StatusRequest = 0;
                }
                catch (Exception ex)
                {
                    BillAcceptorSettingData.GameCanPlay = true;
                    BillAcceptorSettingData.CheckIsInBaseSpin = false;

                    disconnectCount_StatusRequest++;

                    if (disconnectCount_StatusRequest >= 10)
                    {

                        BillAcceptorSettingData.ErrorBool = true;
                        BillAcceptorSettingData.ErrorMessage = "Disconnect";
                    }

                    if (ex.GetType() != typeof(ThreadAbortException))
                    {
                        Debug.Log("BV Disconnect");
                    }
                    else
                    {
                        print(ex);
                    }
                }

            }
            Thread.Sleep(300);
        }
    }

    int DisconnectCount_ReQuest = 0;
    public void ReQuest()
    {
        try
        {
            bufferTwo = null;
            Thread.Sleep(300);
            if (mySerialPort.BytesToRead > 0)
            {
                DisconnectCount_ReQuest = 0;
                buffer = new byte[mySerialPort.ReadBufferSize + 1];
                if (mySerialPort.IsOpen) mySerialPort.Read(buffer, 0, buffer.Length);
                Thread.Sleep(300);
                if (buffer[0] == 0xFC)
                {
                    bufferTwo = new byte[buffer[1]];
                    for (int i = 0; i < bufferTwo.Length; i++)
                    {
                        bufferTwo[i] = buffer[i];
                    }
                    if (bufferTwo[1] == 0x18 && bufferTwo[2] == 0x13)
                    {
                        TestTicketReQuest();
                    }
                    else if (bufferTwo[1] == 0x06 && bufferTwo[2] == 0x13)
                    {
                        CashIn();
                    }
                    //print(ByteArrayToString(bufferTwo));
                }
                mySerialPort.DiscardOutBuffer();
                mySerialPort.DiscardInBuffer();
            }
            else
            {
                DisconnectCount_ReQuest++;
            }
            if (DisconnectCount_ReQuest > 10)
            {
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Disconnect";
            }
        }
        catch (Exception ex)
        {
            BillAcceptorSettingData.ErrorBool = true;
            BillAcceptorSettingData.ErrorMessage = "Disconnect";
            Debug.Log(ex);
        }
    }

    public void CashIn()
    {
        if (!Enable || !BillAcceptorSettingData.CheckIsInBaseSpin)
        {
            if (mySerialPort.IsOpen) mySerialPort.Write(Return, 0, 5);
            return;
        }

        BillAcceptorSettingData.Stacked = false;
        BillAcceptorSettingData.Return = false;
        TicketCertification = false;

        if (!BillAcceptorSettingData.StopCashIn && Enable && BillAcceptorSettingData.CheckIsInBaseSpin)
        {
            switch (bufferTwo[3])
            {
                case 0x64:
                    BillAcceptorSettingData.StopCashIn = true;
                    print("TWD 100 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "100";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
                case 0x65:
                    BillAcceptorSettingData.StopCashIn = true;
                    print("TWD 200 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "200";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
                case 0x66:
                    BillAcceptorSettingData.StopCashIn = true;
                    print("TWD 500 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "500";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
                case 0x67:
                    BillAcceptorSettingData.StopCashIn = true;
                    print("TWD 1000 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "1000";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
                case 0x68:
                    BillAcceptorSettingData.StopCashIn = true;
                    print("TWD 2000 : OG : " + ByteArrayToString(bufferTwo));
                    BillAcceptorSettingData.JCM_WaitRespond = true;
                    TicketCertification = true;
                    BillAcceptorSettingData.TicketValue = "2000";
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketIn = true;
                    break;
            }
            print("While Start");
            while (BillAcceptorSettingData.JCM_WaitRespond && !ThreadStop)
            {
                Thread.Sleep(5);
            }
            print("While End");
            if (BillAcceptorSettingData.StackReturnBool)
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(Stack2, 0, 5);
                BillAcceptorSettingData.Stacked = true;
                //Debug.Log("Stack");
            }
            else
            {
                if (mySerialPort.IsOpen) mySerialPort.Write(Return, 0, 5);
                BillAcceptorSettingData.Return = true;
                //Debug.Log("Return");
            }
        }
    }

    byte Buffer;
    public void ReQuestTrans(byte[] TransBuffer)
    {
        Buffer = TransBuffer[2];
        switch (TransBuffer[2])
        {
            case 0x05:
                ////print("ENQ [0x05]");
                break;
            case 0x11:
                BillAcceptorSettingData.GameCanPlay = true;
                if (Mod_Data.inBaseSpin) BillAcceptorSettingData.CheckIsInBaseSpin = true;
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "ENABLE";
                atleastDoOnce = true;
                //Debug.Log("ENABLE (IDLING) [0x11]");
                break;
            case 0x12:
                BillAcceptorSettingData.GameCanPlay = false;
                if (!Mod_Data.inBaseSpin) BillAcceptorSettingData.CheckIsInBaseSpin = false;
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "Accepting";
                //Debug.Log("Accepting [0x12]");
                break;
            case 0x13:
                BillAcceptorSettingData.GameCanPlay = false;
                if (!Mod_Data.inBaseSpin) BillAcceptorSettingData.CheckIsInBaseSpin = false;
                //Debug.Log("ESCROW [0x13]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x14:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "STACKING";
                BillAcceptorSettingData.Jcm_Buffer = TransBuffer[2];
                //Debug.Log("STACKING [0x14]");
                break;
            case 0x15:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "VEND VALID";
                BillAcceptorSettingData.Jcm_Buffer = TransBuffer[2];
                //Debug.Log("VEND VALID [0x15]");
                Thread.Sleep(500);
                if (mySerialPort.IsOpen) mySerialPort.Write(ack, 0, 5);
                break;
            case 0x16:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "STACKED";
                BillAcceptorSettingData.Jcm_Buffer = TransBuffer[2];
                //Debug.Log("STACKED [0x16]");
                break;
            case 0x17:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "REJECTING";
                BillAcceptorSettingData.Jcm_Buffer = TransBuffer[2];
                //Debug.Log("REJECTING [0x17]: " + ByteArrayToString(TransBuffer));
                //BillAcceptorSettingData.JCM_WaitRespond = false;
                break;
            case 0x18:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "RETURNING";
                BillAcceptorSettingData.Jcm_Buffer = TransBuffer[2];
                //Debug.Log("RETURNING [0x18]");
                break;
            case 0x19:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "HOLDING";
                //Debug.Log("HOLDING [0x19]");
                break;
            case 0x1A:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "DISABLE (INHIBIT)";
                atleastDoOnce = false;
                //Debug.Log("DISABLE (INHIBIT) [0x1A]");
                Thread.Sleep(500);
                if (Enable)
                {
                    if (mySerialPort.IsOpen) mySerialPort.Write(Inhibit, 0, 6);
                    ReQuest();
                }
                break;
            case 0x1B:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "INITIALIZE";
                //Debug.Log("INITIALIZE [0x1B]");
                break;
            case 0x40:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "POWER UP";
                if (mySerialPort.IsOpen) mySerialPort.Write(valsRe, 0, 5);
                //Debug.Log("POWER UP [0x40]");
                break;
            case 0x41:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "POWER UP WITH BILL IN ACCEPTOR";
                if (mySerialPort.IsOpen) mySerialPort.Write(valsRe, 0, 5);
                //Debug.Log("POWER UP WITH BILL IN ACCEPTOR [0x41]");
                break;
            case 0x42:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "POWER UP WITH BILL IN STACKER";
                if (mySerialPort.IsOpen) mySerialPort.Write(valsRe, 0, 5);
                //Debug.Log("POWER UP WITH BILL IN STACKER [0x42]");
                break;
            case 0x43:
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Stacker Full";
                BillAcceptorSettingData.NormalBool = false;
                BillAcceptorSettingData.NormalMessage = "";
                //Debug.Log("Stacker Full [0x43]");
                break;
            case 0x44:
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Stacker Open";
                BillAcceptorSettingData.NormalBool = false;
                BillAcceptorSettingData.NormalMessage = "";
                //Debug.Log("Stacker Open [0x44]");
                break;
            case 0x45:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "JAM IN Acceptor";
                //Debug.Log("JAM IN Acceptor [0x45]");
                break;
            case 0x46:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "JAM IN Stacker";
                //Debug.Log("JAM IN Stacker [0x46]");
                break;
            case 0x47:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "Pause";
                //Debug.Log("Pause [0x47]");
                break;
            case 0x48:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "CHEATED";
                //Debug.Log("CHEATED [0x48]");
                break;
            case 0x49:
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "FAILURE [0x49]: " + ByteArrayToString(TransBuffer);
                BillAcceptorSettingData.NormalBool = false;
                BillAcceptorSettingData.NormalMessage = "";
                //Debug.Log("FAILURE [0x49]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x4A:
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Communication Error";
                BillAcceptorSettingData.NormalBool = false;
                BillAcceptorSettingData.NormalMessage = "";
                //Debug.Log("Communication Error [0x4A]");
                break;
            case 0x4B:
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "INVALID COMMAND";
                BillAcceptorSettingData.NormalBool = false;
                BillAcceptorSettingData.NormalMessage = "";
                //Debug.Log("INVALID COMMAND [0x4B]");
                break;
            case 0x50:
                BillAcceptorSettingData.ErrorBool = false;
                BillAcceptorSettingData.ErrorMessage = "";
                BillAcceptorSettingData.NormalBool = true;
                BillAcceptorSettingData.NormalMessage = "ACK";
                //Debug.Log("ACK [0x50]");
                break;

            case 0x80:
                //Debug.Log("ENABLE / DISABLE (DENOMI) [0x80]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x81:
                //Debug.Log("SECURTY (DENOMI) [0x81]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x82:
                //Debug.Log("COMMUNICATION MODE [0x82]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x83:
                //Debug.Log("INHIBIT (ACCEPTOR) [0x83]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x84:
                //Debug.Log("DIRECTION [0x84]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x85:
                //Debug.Log("OPTIONAL FUNCTION [0x85]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x88:
                //Debug.Log("VERSION INFOMATION [0x88]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x89:
                //Debug.Log("BOOT VERSION INFOMATION [0x89]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x8A:
                //Debug.Log("DENOMINATION [0x8A]: " + ByteArrayToString(TransBuffer));
                break;


            case 0xC0:
                //Debug.Log("ENABLE / DISABLE (DENOMI) [0xC0]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC1:
                //Debug.Log("SECURTY (DENOMI) [0xC1]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC2:
                //Debug.Log("COMMUNICATION MODE [0xC2]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC3:
                //Debug.Log("INHIBIT (ACCEPTOR) [0xC3]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC4:
                //Debug.Log("DIRECTION [0xC4]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC5:
                //Debug.Log("OPTIONAL FUNCTION [0xC5]: " + ByteArrayToString(TransBuffer));
                break;

            default:
                break;
        }
    }


    public void TestTicketReQuest()
    {

        TicketCertification = false;
        byte[] TicketBuffer = new byte[18];
        for (int i = 0; i < 18; i++)
        {
            TicketBuffer[i] = bufferTwo[i + 4];
        }
        string TicketString = ByteArrayToString(TicketBuffer);
        string TicketStringRE = "";
        for (int i = 1; i < 36; i += 2)
        {
            TicketStringRE += TicketString.Substring(i, 1);
        }
        //print("Ticket: " + TicketStringRE);
        //print("OG : " + ByteArrayToString(bufferTwo));

        if (BillAcceptorSettingData.TicketEnable)
        {
            //傳送資料給SERVER 取得票券資料
            BillAcceptorSettingData.TicketValue = TicketStringRE;
            BillAcceptorSettingData.TicketType = "Ticket";
            BillAcceptorSettingData.TicketIn = true;
            BillAcceptorSettingData.JCM_WaitRespond = true;
            //print("Ticket While Start");
            while (BillAcceptorSettingData.JCM_WaitRespond && !ThreadStop)
            {
                Thread.Sleep(5);
            }
            //print("Ticket While End");
            if (BillAcceptorSettingData.StackReturnBool)
            {
                TicketCertification = true;
            }
            else
            {
                TicketCertification = false;
            }
            //switch (TicketStringRE)
            //{
            //    case "014514268461590265":
            //        TicketCertification = true;
            //        //Data.Credit += 2000;
            //        //Data.Ticketin = true;
            //        break;
            //    default:
            //        TicketCertification = false;
            //        break;
            //}
        }

        if (TicketCertification)
        {
            mySerialPort.Write(Stack2, 0, 5);
            //ReQuest();
        }
        else
        {
            mySerialPort.Write(Return, 0, 5);
            //ReQuest();
        }
    }

    IEnumerator MainReQuest()
    {
        mySerialPort.Read(buffer, 0, buffer.Length);
        yield return new WaitForSeconds(0.25f);
        bufferTwo = new byte[buffer[1]];

        for (int i = 0; i < bufferTwo.Length; i++)
        {
            bufferTwo[i] = buffer[i];
        }
        print(ByteArrayToString(bufferTwo));
    }
    //void OnApplicationQuit()
    //{
    //    mySerialPort.Write(InhibitClose, 0, 6);
    //    mySerialPort.Close();
    //    ThreadStop = true;
    //    RequestThread.Abort();
    //}
    public static string ByteArrayToString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", "");

        /*StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}",b);
        return hex.ToString();*/
    }

    public void JCM_Bill_Close()
    {
        if (!mySerialPort.IsOpen) return;

        if (mySerialPort.IsOpen)
        {
            mySerialPort.Write(InhibitClose, 0, 6);
            mySerialPort.Close();
        }
        ThreadStop = true;
        if (RequestThread.IsAlive)
        {
            RequestThread.Abort();
        }
        if (InputCommand.IsAlive)
        {
            InputCommand.Abort();
        }

        ThreadStop = false;
    }

    void OnApplicationQuit()
    {
        if (mySerialPort.IsOpen)
        {
            mySerialPort.Write(InhibitClose, 0, 6);
            mySerialPort.Close();
        }
        ThreadStop = true;
        if (RequestThread.IsAlive)
        {
            RequestThread.Abort();
        }
        if (InputCommand.IsAlive)
        {
            InputCommand.Abort();
        }
    }
    #endregion
#endif
}
