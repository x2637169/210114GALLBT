using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ICT_Bill_Acceptor : MonoBehaviour
{
    //public Text _Text, _Text2, T3;
    public string ComPortName = BillAcceptorSettingData.COMPort;
    SerialPort port = new SerialPort(BillAcceptorSettingData.COMPort, 9600, Parity.Even, 8, StopBits.One);
    public Thread RequestThread;
    byte[] code = { 0x02 };
    byte[][] Allcode = 
        {
        new byte[]{0x30},   //reset
        new byte[]{0x02},   //accept
        new byte[]{0x5E},   //disable
        new byte[]{0x3E},   //enable
        new byte[]{0x0F},   //reject
        new byte[]{0x0C}
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (!firstStart)
        {
            DisconnectCount += Time.deltaTime;
        }
        if (!firstStart)
        {
            if (DisconnectCount > 5)
            {
                port.DiscardInBuffer();
                port.Write(Allcode[5], 0, 1);
                DisconnectCount = 0;
                //Thread.Sleep(100);
            }
            if (DisconnectCount > 7)
            {
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Disconnect";
            }
        }
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    string tmp = "1,0,1,1,0," + BillAcceptorSettingData.COMPort;
        //    string[] words = tmp.Split(',');
        //    bool[] booltmp = new bool[5];
        //    for(int i = 0; i < 5; i++)
        //    {
        //        if (words[i] == "1")
        //        {
        //            booltmp[i] = true;
        //        }
        //        else
        //        {
        //            booltmp[i] = false;
        //        }
        //    }
        //    CashInSetting(booltmp[0], booltmp[1], booltmp[2], booltmp[3], booltmp[4]);
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    string tmp = "1,0,1,1,0," + BillAcceptorSettingData.COMPort;
        //    string[] words = tmp.Split(',');
        //    port = new SerialPort(BillAcceptorSettingData.COMPort, 9600, Parity.Even, 8, StopBits.One);
        //    port.Open();
        //    RequestThread = new Thread(ICTListenThread);
        //    RequestThread.IsBackground = true;
        //    isRun = true;
        //    if (!RequestThread.IsAlive)
        //    {
        //        RequestThread.Start();
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    isRun = false;
        //    port.Close();
        //    RequestThread.Abort();
        //}

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    RequestThread.Abort();
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    port.DiscardInBuffer();
        //    port.Write(Allcode[0], 0, 1);
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    port.DiscardInBuffer();
        //    port.Write(Allcode[1], 0, 1);
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    port.DiscardInBuffer();
        //    port.Write(Allcode[2], 0, 1);
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    port.DiscardInBuffer();
        //    port.Write(Allcode[3], 0, 1);
        //}

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    port.DiscardInBuffer();
        //    port.Write(Allcode[4], 0, 1);
        //}

        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    port.DiscardInBuffer();
        //    port.Write(Allcode[5], 0, 1);
        //}

        //if (message != "" && message != null)
        //{
        //    //_Text.text = message;
        //    message = "";
        //}
    }
    byte[] buffer;
    public static bool isRun = true;
    //public bool data = false;
    public static string message = "";
    bool[] CashInEnable = new bool[5];

    public void Open_ICT_Bill(bool OneHundred, bool TwoHundred, bool FiveHundred, bool OneThousand, bool TwoThousand)
    {
        firstStart = true;
        firstCheck[0] = false;
        firstCheck[1] = false;
        CashInSetting(OneHundred, TwoHundred, FiveHundred, OneThousand, TwoThousand);
        port = new SerialPort(BillAcceptorSettingData.COMPort, 9600, Parity.Even, 8, StopBits.One);
        port.Open();
        RequestThread = new Thread(ICTListenThread);
        RequestThread.IsBackground = true;
        isRun = true;
        if (!RequestThread.IsAlive)
        {
            RequestThread.Start();
        }
    }

    public void Close_ICT_Bill()
    {
        isRun = false;
        if (port.IsOpen)
        {
            port.DiscardInBuffer();
            port.Write(Allcode[2], 0, 1);
            port.Close();
        }
        // 結束 Unity 關閉執行緒
        if (RequestThread.IsAlive)
        {
            RequestThread.Abort();
        }
    }

    public void BillAcceptorEnable(bool enable)
    {
        if (enable)
        {
            port.DiscardInBuffer();
            port.Write(Allcode[3], 0, 1);
        }
        else
        {
            port.DiscardInBuffer();
            port.Write(Allcode[2], 0, 1);
        }
    }

    public void CashInSetting(bool OneHundred, bool TwoHundred, bool FiveHundred, bool OneThousand, bool TwoThousand)
    {
        CashInEnable[0] = OneHundred;
        CashInEnable[1] = TwoHundred;
        CashInEnable[2] = FiveHundred;
        CashInEnable[3] = OneThousand;
        CashInEnable[4] = TwoThousand;
    }

    //void BillIn(int billcode)
    //{
    //    switch (billcode)
    //    {
    //        case 100:
    //            port.DiscardInBuffer();
    //            if (CashInEnable[0])
    //            {
    //                port.Write(Allcode[1], 0, 1);
    //            }
    //            else
    //            {
    //                port.Write(Allcode[4], 0, 1);
    //            }
    //            break;

    //        case 200:
    //            port.DiscardInBuffer();
    //            if (CashInEnable[1])
    //            {
    //                port.Write(Allcode[1], 0, 1);
    //            }
    //            else
    //            {
    //                port.Write(Allcode[4], 0, 1);
    //            }
    //            break;
    //        case 500:
    //            port.DiscardInBuffer();
    //            if (CashInEnable[2])
    //            {
    //                port.Write(Allcode[1], 0, 1);
    //            }
    //            else
    //            {
    //                port.Write(Allcode[4], 0, 1);
    //            }
    //            break;
    //        case 1000:
    //            port.DiscardInBuffer();
    //            if (CashInEnable[3])
    //            {
    //                port.Write(Allcode[1], 0, 1);
    //            }
    //            else
    //            {
    //                port.Write(Allcode[4], 0, 1);
    //            }
    //            break;
    //        case 2000:
    //            port.DiscardInBuffer();
    //            if (CashInEnable[4])
    //            {
    //                port.Write(Allcode[1], 0, 1);
    //            }
    //            else
    //            {
    //                port.Write(Allcode[4], 0, 1);
    //            }
    //            break;
    //        default:
    //            port.DiscardInBuffer();
    //            port.Write(Allcode[4], 0, 1);
    //            break;
    //    }
    //}

    void BillIn(int billcode)
    {
        //if (!BillAcceptorSettingData.ValueSendBool)
        //{
        print(billcode);
            switch (billcode)
            {
                case 100:
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "100";
                    BillAcceptorSettingData.TicketIn = true;
                    //port.DiscardInBuffer();
                    //if (CashInEnable[0])
                    //{
                    //    port.Write(Allcode[1], 0, 1);
                    //}
                    //else
                    //{
                    //    port.Write(Allcode[4], 0, 1);
                    //}
                    break;

                case 200:
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "200";
                    BillAcceptorSettingData.TicketIn = true;
                    //port.DiscardInBuffer();
                    //if (CashInEnable[1])
                    //{
                    //    port.Write(Allcode[1], 0, 1);
                    //}
                    //else
                    //{
                    //    port.Write(Allcode[4], 0, 1);
                    //}
                    break;
                case 500:
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "500";
                    BillAcceptorSettingData.TicketIn = true;
                    //port.DiscardInBuffer();
                    //if (CashInEnable[2])
                    //{
                    //    port.Write(Allcode[1], 0, 1);
                    //}
                    //else
                    //{
                    //    port.Write(Allcode[4], 0, 1);
                    //}
                    break;
                case 1000:
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "1000";
                    BillAcceptorSettingData.TicketIn = true;
                    //port.DiscardInBuffer();
                    //if (CashInEnable[3])
                    //{
                    //    port.Write(Allcode[1], 0, 1);
                    //}
                    //else
                    //{
                    //    port.Write(Allcode[4], 0, 1);
                    //}
                    break;
                case 2000:
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "2000";
                    BillAcceptorSettingData.TicketIn = true;
                    //port.DiscardInBuffer();
                    //if (CashInEnable[4])
                    //{
                    //    port.Write(Allcode[1], 0, 1);
                    //}
                    //else
                    //{
                    //    port.Write(Allcode[4], 0, 1);
                    //}
                    break;
                default:
                    port.DiscardInBuffer();
                    port.Write(Allcode[4], 0, 1);
                    break;
            }
        //}
    }

    public void StackReturn(bool StackReturn)   //true:Stack    false:Return
    {
        port.DiscardInBuffer();
        if (StackReturn)
        {
            port.Write(Allcode[1], 0, 1);
        }
        else
        {
            port.Write(Allcode[4], 0, 1);
        }
    }
    float DisconnectCount = 0;
    bool[] firstCheck = new bool[2];
    bool firstStart = true;
    public void ICTListenThread()
    {
        try
        {
            port.DiscardInBuffer();
            port.Write(Allcode[0], 0, 1);
            // 持續讀取 RS232
            //message = "Open";
            //Thread.Sleep(1000);
            //BillAcceptorEnable(true);
            //Thread.Sleep(50);
            while (isRun)
            {
                //port.DiscardInBuffer();
                //port.Write(Allcode[5], 0, 1);
                //Thread.Sleep(100);
                DisconnectCount = 0;
                    buffer = null;
                    buffer = new byte[port.ReadBufferSize + 1];
                    port.Read(buffer, 0, buffer.Length);
                DisconnectCount = 0;
                switch (buffer[0])
                    {
                        case 0x20:
                            BillAcceptorSettingData.ErrorBool = true;
                            BillAcceptorSettingData.ErrorMessage = "Motor Failure";
                            BillAcceptorSettingData.NormalBool = false;
                            BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x20 Motor Failure";
                            break;
                        case 0x21:
                            BillAcceptorSettingData.ErrorBool = true;
                            BillAcceptorSettingData.ErrorMessage = "Checksum Error";
                            BillAcceptorSettingData.NormalBool = false;
                            BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x21 Checksum Error";
                            break;
                        case 0x22:
                            BillAcceptorSettingData.ErrorBool = true;
                            BillAcceptorSettingData.ErrorMessage = "Bill Jam";
                            BillAcceptorSettingData.NormalBool = false;
                            BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x22 Bill Jam";
                            break;
                        case 0x23:
                            BillAcceptorSettingData.ErrorBool = true;
                            BillAcceptorSettingData.ErrorMessage = "Bill Remove";
                            BillAcceptorSettingData.NormalBool = false;
                            BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x23 Bill Remove";
                            break;
                        case 0x24:
                            BillAcceptorSettingData.ErrorBool = true;
                            BillAcceptorSettingData.ErrorMessage = "Stacker Open";
                            BillAcceptorSettingData.NormalBool = false;
                            BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x24 Stacker Open";
                            break;
                        case 0x25:
                            BillAcceptorSettingData.ErrorBool = true;
                            BillAcceptorSettingData.ErrorMessage = "Sensor Problem";
                            BillAcceptorSettingData.NormalBool = false;
                            BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x25 Sensor Problem";
                            break;
                        case 0x27:
                            BillAcceptorSettingData.ErrorBool = true;
                            BillAcceptorSettingData.ErrorMessage = "Bill Fish";
                            BillAcceptorSettingData.NormalBool = false;
                            BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x27 Bill Fish";
                            break;
                        case 0x28:
                            BillAcceptorSettingData.ErrorBool = true;
                            BillAcceptorSettingData.ErrorMessage = "Stacker Problem";
                            BillAcceptorSettingData.NormalBool = false;
                            BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x28 Stacker Problem";
                            break;
                        case 0x2A:
                            BillAcceptorSettingData.ErrorBool = true;
                            BillAcceptorSettingData.ErrorMessage = "Invalid Command";
                            BillAcceptorSettingData.NormalBool = false;
                            BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x2A Invalid Command";
                            break;
                        case 0x2E:
                            //BillAcceptorSettingData.NormalBool = true;
                            //BillAcceptorSettingData.NormalMessage = "Reserved";
                            //BillAcceptorSettingData.ErrorBool = false;
                            //BillAcceptorSettingData.ErrorMessage = "";
                            //message = "0x2E Reserved";
                            break;
                        case 0x3E:
                            //BillAcceptorSettingData.NormalBool = true;
                            //BillAcceptorSettingData.NormalMessage = "Enable";
                            //BillAcceptorSettingData.ErrorBool = false;
                            //BillAcceptorSettingData.ErrorMessage = "";
                            //message = "0x3E Enable";
                            break;
                        case 0x5E:
                            //BillAcceptorSettingData.NormalBool = true;
                            //BillAcceptorSettingData.NormalMessage = "Disable";
                            //BillAcceptorSettingData.ErrorBool = false;
                            //BillAcceptorSettingData.ErrorMessage = "";
                            //message = "0x5E Disable";
                            break;
                        case 0x40:
                            if (!BillAcceptorSettingData.StopCashIn)
                            {
                                BillAcceptorSettingData.NormalBool = true;
                                BillAcceptorSettingData.NormalMessage = "TWD 100";
                                BillAcceptorSettingData.ErrorBool = false;
                                BillAcceptorSettingData.ErrorMessage = "";
                                //message = "0x40 TWD 100";
                                BillIn(100);
                            }
                            break;
                        case 0x41:
                            if (!BillAcceptorSettingData.StopCashIn)
                            {
                                BillAcceptorSettingData.NormalBool = true;
                                BillAcceptorSettingData.NormalMessage = "TWD 200";
                                BillAcceptorSettingData.ErrorBool = false;
                                BillAcceptorSettingData.ErrorMessage = "";
                                //message = "0x41 TWD 200";
                                BillIn(200);
                            }
                            break;
                        case 0x42:
                            if (!BillAcceptorSettingData.StopCashIn)
                            {
                                BillAcceptorSettingData.NormalBool = true;
                                BillAcceptorSettingData.NormalMessage = "TWD 500";
                                BillAcceptorSettingData.ErrorBool = false;
                                BillAcceptorSettingData.ErrorMessage = "";
                                //message = "0x42 TWD 500";
                                BillIn(500);
                            }
                            break;
                        case 0x43:
                            if (!BillAcceptorSettingData.StopCashIn)
                            {
                                BillAcceptorSettingData.NormalBool = true;
                                BillAcceptorSettingData.NormalMessage = "TWD 1000";
                                BillAcceptorSettingData.ErrorBool = false;
                                BillAcceptorSettingData.ErrorMessage = "";
                                //message = "0x43 TWD 1000";
                                BillIn(1000);
                            }
                            break;
                        case 0x44:
                            if (!BillAcceptorSettingData.StopCashIn)
                            {
                                BillAcceptorSettingData.NormalBool = true;
                                BillAcceptorSettingData.NormalMessage = "TWD 2000";
                                BillAcceptorSettingData.ErrorBool = false;
                                BillAcceptorSettingData.ErrorMessage = "";
                                //message = "0x44 TWD 2000";
                                BillIn(2000);
                            }
                            break;
                        case 0x80:
                            //message = "0x80";
                            firstCheck[1] = true;
                            BillAcceptorSettingData.ErrorBool = false;
                            BillAcceptorSettingData.ErrorMessage = "";
                            break;
                        case 0x8F:
                            //message = "0x8F";
                            //firstStart = false;
                            BillAcceptorSettingData.ErrorBool = false;
                            BillAcceptorSettingData.ErrorMessage = "";
                            port.DiscardInBuffer();
                            port.Write(Allcode[1], 0, 1);
                            firstCheck[0] = true;
                            break;
                        case 0x81:
                            //BillAcceptorSettingData.NormalBool = true;
                            //BillAcceptorSettingData.NormalMessage = "Bill validated";
                            //BillAcceptorSettingData.ErrorBool = false;
                            //BillAcceptorSettingData.ErrorMessage = "";
                            //message = "0x81 Bill validated";
                            break;
                        case 0x10:
                            //BillAcceptorSettingData.NormalBool = true;
                            //BillAcceptorSettingData.NormalMessage = "Stacking";
                            //BillAcceptorSettingData.ErrorBool = false;
                            //BillAcceptorSettingData.ErrorMessage = "";
                            //message = "0x10 Stacking";
                            break;
                        case 0x11:
                            BillAcceptorSettingData.NormalBool = true;
                            BillAcceptorSettingData.NormalMessage = "Reject";
                            BillAcceptorSettingData.ErrorBool = false;
                            BillAcceptorSettingData.ErrorMessage = "";
                            BillAcceptorSettingData.TicketValue = "";
                            BillAcceptorSettingData.TicketType = "";
                            //BillAcceptorSettingData.ValueSendBool = false;
                            //message = "0x11 Reject";
                            break;
                        case 0x29:
                            BillAcceptorSettingData.NormalBool = true;
                            BillAcceptorSettingData.NormalMessage = "Bill Reject";
                            BillAcceptorSettingData.ErrorBool = false;
                            BillAcceptorSettingData.ErrorMessage = "";
                            BillAcceptorSettingData.TicketValue = "";
                            BillAcceptorSettingData.TicketType = "";
                            //BillAcceptorSettingData.ValueSendBool = false;
                            //message = "0x29 Bill Reject";
                            break;
                        case 0x2F:
                            //BillAcceptorSettingData.ErrorBool = true;
                            //BillAcceptorSettingData.ErrorMessage = "Response when Error Status is Exclusion";
                            //BillAcceptorSettingData.NormalBool = false;
                            //BillAcceptorSettingData.NormalMessage = "";
                            //message = "0x2F Response when Error Status is Exclusion";
                            break;
                        default:
                            break;
                    }


                //string read = port.ReadLine();
                //message = read;
                //print(read);

                Thread.Sleep(100);
                if (firstStart)
                {
                    if (firstCheck[0] && firstCheck[1])
                    {
                        firstStart = false;
                    }
                }
            }
            //message = "close";
        }
        catch
        {
            
        }
    }

    void OnApplicationQuit()
    {
        isRun = false;
        if (port.IsOpen)
        {
            port.DiscardInBuffer();
            port.Write(Allcode[2], 0, 1);
            port.Close();
        }
        // 結束 Unity 關閉執行緒
        if (RequestThread.IsAlive)
        {
            RequestThread.Abort();
        }
    }
}
