using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System;
using System.Text;
using System.Runtime.InteropServices;
using CFPGADrv;
using UnityEngine.UI;

public class Mod_UBA : MonoBehaviour {

    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS();
    SerialPort mySerialPort = new SerialPort("COM2", 9600);
    public float TimeOutCount = 0;
    public GameObject ErrorPanal;
    public bool TimeOut = false;
    public Thread RequestThread, InputCommand;
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
    void Start() {
        if (Mod_Data.UBA_Enable == 1)
        {
            StartUBAThread();
        }
    }
    public void StartUBAThread()
    {
        mySerialPort.PortName = Mod_Data.UBAPortNumber;
        mySerialPort.BaudRate = int.Parse("9600");
        mySerialPort.Parity = Parity.Even;
        mySerialPort.StopBits = StopBits.One;
        mySerialPort.DataBits = 8;
        mySerialPort.Handshake = Handshake.None;
        mySerialPort.RtsEnable = true;
        mySerialPort.Open();
        RequestThread = new Thread(StatusRequest);
        RequestThread.IsBackground = true;
        InputCommand = new Thread(InputcThread);
        InputCommand.IsBackground = true;
        //RequestThread.Start();
        print(vals[0] + "" + vals[1] + "" + vals[2] + "" + vals[3] + "" + vals[4]);
        //mySerialPort.Write(valsRe, 0, 5);
        if (!RequestThread.IsAlive)
        {
            RequestThread.Start();
        }
        //if (!InputCommand.IsAlive)
        //{
        //    InputCommand.Start();
        //}
    }



    // Update is called once per frame
    void Update() {
        if (Mod_Data.UBA_Enable == 1)
        {
            TimeOutCount += Time.deltaTime;
            if (TimeOutCount > 3)
            {
                TimeOut = true;
                TimeOutCount = 0;
                mySerialPort.Write(vals, 0, 5);
                //Thread.Sleep(500);
                /*//Debug.Log("BV Disconnect");
                Data.Errortext = "<color=#FF0000>BV Disconnect</color>" + "\r\n ";
                Data.ErrorrUBA = false;*/
                if (ReStart > 3)
                {
                    ReStart = 0;
                    mySerialPort.DiscardInBuffer();

                    mySerialPort.Write(valsRe, 0, 5);
                    Thread.Sleep(100);  //500

                    for (int i = 0; i < StartVals.Length; i++)
                    {
                        if (mySerialPort.IsOpen)
                        {
                            mySerialPort.DiscardInBuffer();
                            mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
                            Thread.Sleep(100);  //200
                        }
                    }
                    TimeOut = false;
                    ErrorPanal.SetActive(false);
                }
            }

            if (TimeOut == true)
            {
                //Debug.Log("BV Disconnect");
                ErrorPanal.transform.GetChild(0).GetComponent<Text>().text = "<color=#FF0000>BV Disconnect</color>" + "\r\n ";
                ErrorPanal.SetActive(true);
                Mod_Data.ErrorrUBA = false;
            }
        }
    }

    void InputcThread()
    {
        while (!ThreadStop)
        {
            try
            {
                mySerialPort.Write(vals, 0, 5);
                Thread.Sleep(100); //500

                if (ReStart > 3)
                {
                    ReStart = 0;
                    mySerialPort.DiscardInBuffer();

                    mySerialPort.Write(valsRe, 0, 5);
                    Thread.Sleep(100);  //500

                    for (int i = 0; i < StartVals.Length; i++)
                    {
                        if (mySerialPort.IsOpen)
                        {
                            mySerialPort.DiscardInBuffer();
                            mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
                            Thread.Sleep(100);  //200
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                print("ReloadError");
            }
        }
    }

    IEnumerator StartBV()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < StartVals.Length; i++)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
                MainReQuest();
            }
            yield return new WaitForSeconds(0.75f);

        }
    }

    public void readw()
    {
        byte[] buffer = new byte[mySerialPort.ReadBufferSize + 1];
        int bytes = 0;
        try
        {
            int count1 = mySerialPort.Read(buffer, 0, mySerialPort.ReadBufferSize);
            bytes = mySerialPort.Read(buffer, 0, buffer.Length);

        }
        catch (Exception ex)
        {
            if (ex.GetType() != typeof(ThreadAbortException))
            {
                ////Debug.Log("Printer Disconnect");
            }
        }
    }
    byte[] buffer;
    byte[] bufferTwo;
    bool TicketCertification = false;
    void StatusRequest()
    {
        mySerialPort.DiscardInBuffer();
        mySerialPort.Write(valsRe, 0, 5);
        Thread.Sleep(100);  //500

        for (int i = 0; i < StartVals.Length; i++)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.DiscardInBuffer();
                mySerialPort.Write(StartVals[i], 0, StartVals[i].Length);
                ReQuest();
            }
        }
        /*if (!InputCommand.IsAlive)
        {
            InputCommand.Start();
        }*/
        //TimeOut = false;
        Mod_Data.UBA_OpenSetting = true;
        Thread.Sleep(100);
        while (!ThreadStop)
        {
            
            if (this.mySerialPort != null && this.mySerialPort.IsOpen)
            {
                
                try
                {
                    TimeOut = false;
                    ErrorPanal.SetActive(false);
                    TimeOutCount = 0;
                    mySerialPort.DiscardInBuffer();
                    mySerialPort.Write(vals, 0, 5);
                    ReQuest();
                    ReQuestTrans(bufferTwo);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ThreadAbortException))
                    {
                        //Debug.Log("BV Disconnect");
                        ErrorPanal.transform.GetChild(0).GetComponent<Text>().text = "<color=#FF0000>BV Disconnect</color>" + "\r\n ";
                        ErrorPanal.SetActive(true);
                    }
                    ReStart++;
                }

            }
            Thread.Sleep(100);  //500
        }
    }

    public void ReQuest()
    {
        bufferTwo = null;
        Thread.Sleep(100); //300
        buffer = null;
        buffer = new byte[mySerialPort.ReadBufferSize + 1];
        mySerialPort.Read(buffer, 0, buffer.Length);
        Thread.Sleep(100); //300
        if (buffer[0] == 0xFC)
        {
            bufferTwo = new byte[buffer[1]];
            for (int i = 0; i < bufferTwo.Length; i++)
            {
                bufferTwo[i] = buffer[i];
            }
            if((bufferTwo[1] == 0x18) && (bufferTwo[2] == 0x13))
            {
                mySerialPort.Write(hold, 0, 6);
                Thread.Sleep(100); //300
                TestTicketReQuest();
            }
            else if ((bufferTwo[1] == 0x06) && (bufferTwo[2] == 0x13))
            {
                CashIn();
            }
            //print(ByteArrayToString(bufferTwo));
        }
        mySerialPort.DiscardOutBuffer();
        mySerialPort.DiscardInBuffer();
    }

    public void CashIn()
    {
        mySerialPort.Write(hold, 0, 6);
        TicketCertification = false;
        int Cash = 0;
        Mod_Data.cash = 0;
        switch (bufferTwo[3])
        {
            case 0x64:
                print("TWD 100 : OG : " + ByteArrayToString(bufferTwo));
                TicketCertification = true;
                Cash = 100;
                Mod_Data.Ticketin = true;
                Mod_Data.Messagetext = "Cash In 100";
                break;
            //case 0x65:
            //    print("TWD 200 : OG : " + ByteArrayToString(bufferTwo));
            //    TicketCertification = true;
            //    Cash = 200;
            //    Data.Ticketin = true;
            //    Data.Messagetext = "Cash In 200";
            //    break;
            case 0x66:
                print("TWD 500 : OG : " + ByteArrayToString(bufferTwo));
                TicketCertification = true;
                Cash = 500;
                Mod_Data.Ticketin = true;
                Mod_Data.Messagetext = "Cash In 500";
                break;
            case 0x67:
                print("TWD 1000 : OG : " + ByteArrayToString(bufferTwo));
                TicketCertification = true;
                Cash = 1000;
                Mod_Data.Ticketin = true;
                Mod_Data.Messagetext = "Cash In 1000";
                break;
            //case 0x68:
            //    print("TWD 2000 : OG : " + ByteArrayToString(bufferTwo));
            //    TicketCertification = true;
            //    Cash = 2000;
            //    Data.Ticketin = true;
            //    Data.Messagetext = "Cash In 2000";
            //    break;
            default:
                break;
        }

        if (TicketCertification)
        {
            mySerialPort.Write(Stack2, 0, 5);
            Mod_Data.cash = Cash;
            Mod_Data.credit += Cash;
            GameObject.Find("GameController").GetComponent<Mod_UIController>().UpdateScore();
            //CreditReset(Mod_Data.credit.ToString("N2"), Cash, 0);
            Mod_Data.Ticketin = true;
            //Data.Cashin = true;
            ReQuest();
        }
        else
        {
            mySerialPort.Write(Return, 0, 5);
            ReQuest();
        }
    }

    public void ReQuestTrans(byte[] TransBuffer)
    {
        switch (TransBuffer[2])
        {
            case 0x05:
                print("ENQ [0x05]");
                break;
            case 0x11:
                //print("ENABLE (IDLING) [0x11]");
                Mod_Data.ErrorrUBA = true;
                if (Mod_Data.state != Mod_State.STATE.BaseSpin) {
                    mySerialPort.Write(InhibitClose,0,6);
                    ReQuest();
                }
                break;
            case 0x12:
                print("Accepting [0x12]");
                break;
            case 0x13:
                print("ESCROW [0x13]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x14:
                print("STACKING [0x14]");
                break;
            case 0x15:
                print("VEND VALID [0x15]");
                Thread.Sleep(500);
                mySerialPort.Write(ack, 0, 5);
                break;
            case 0x16:
                print("STACKED [0x16]");
                break;
            case 0x17:
                print("REJECTING [0x17]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x18:
                print("RETURNING [0x18]");
                break;
            case 0x19:
                print("HOLDING [0x19]");
                break;
            case 0x1A:

                Mod_Data.ErrorrUBA = true;
                Thread.Sleep(500);
                if (Mod_Data.state == Mod_State.STATE.BaseSpin)
                { mySerialPort.Write(Inhibit, 0, 6);
                    ReQuest();
                }
                //ReQuest();
                break;
            case 0x1B:
                print("INITIALIZE [0x1B]");
                break;
            case 0x40:
                print("POWER UP [0x40]");
                break;
            case 0x41:
                print("POWER UP WITH BILL IN ACCEPTOR [0x41]");
                break;
            case 0x42:
                print("POWER UP WITH BILL IN STACKER [0x42]");
                break;
            case 0x43:
                print("Stacker Full [0x43]");
                Mod_Data.Errortext = "<color=#FF0000>Stacker Full</color>" + "\r\n ";
                Mod_Data.ErrorrUBA = false;
                break;
            case 0x44:
                print("Stacker Open [0x44]");
                Mod_Data.Errortext = "<color=#FF0000>Stacker Open</color>" + "\r\n ";
                Mod_Data.ErrorrUBA = false;
                break;
            case 0x45:
                print("JAM IN Acceptor [0x45]");
                Mod_Data.Errortext = "<color=#FF0000>JAM IN Acceptor</color>" + "\r\n ";
                Mod_Data.ErrorrUBA = false;
                break;
            case 0x46:
                print("JAM IN Stacker [0x46]");
                Mod_Data.Errortext = "<color=#FF0000>JAM IN Stacker</color>" + "\r\n ";
                Mod_Data.ErrorrUBA = false;
                break;
            case 0x47:
                print("Pause [0x47]");
                Mod_Data.Errortext = "<color=#FF0000>Stacker Full</color>" + "\r\n ";
                Mod_Data.ErrorrUBA = false;
                break;
            case 0x48:
                print("CHEATED [0x48]");
                Mod_Data.Errortext = "<color=#FF0000>CHEATED</color>" + "\r\n ";
                Mod_Data.ErrorrUBA = false;
                break;
            case 0x49:
                print("FAILURE [0x49]: " + ByteArrayToString(TransBuffer));
                Mod_Data.Errortext = "<color=#FF0000>FAILURE</color>" + "\r\n ";
                Mod_Data.ErrorrUBA = false;
                break;
            case 0x4A:
                print("Communication Error [0x4A]");
                Mod_Data.Errortext = "<color=#FF0000>Communication Error</color>" + "\r\n ";
                Mod_Data.ErrorrUBA = false;
                break;
            case 0x4B:
                print("INVALID COMMAND [0x4B]");
                break;
            case 0x50:
                print("ACK [0x50]");
                break;

            case 0x80:
                print("ENABLE / DISABLE (DENOMI) [0x80]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x81:
                print("SECURTY (DENOMI) [0x81]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x82:
                print("COMMUNICATION MODE [0x82]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x83:
                print("INHIBIT (ACCEPTOR) [0x83]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x84:
                print("DIRECTION [0x84]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x85:
                print("OPTIONAL FUNCTION [0x85]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x88:
                print("VERSION INFOMATION [0x88]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x89:
                print("BOOT VERSION INFOMATION [0x89]: " + ByteArrayToString(TransBuffer));
                break;
            case 0x8A:
                print("DENOMINATION [0x8A]: " + ByteArrayToString(TransBuffer));
                break;


            case 0xC0:
                print("ENABLE / DISABLE (DENOMI) [0xC0]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC1:
                print("SECURTY (DENOMI) [0xC1]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC2:
                print("COMMUNICATION MODE [0xC2]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC3:
                print("INHIBIT (ACCEPTOR) [0xC3]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC4:
                print("DIRECTION [0xC4]: " + ByteArrayToString(TransBuffer));
                break;
            case 0xC5:
                print("OPTIONAL FUNCTION [0xC5]: " + ByteArrayToString(TransBuffer));
                break;

            default:
                break;
        }
    }
   



    public void TestTicketReQuest()
    {
        //mySerialPort.Write(hold,0,6);
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
        print("Ticket: " + TicketStringRE);
        print("OG : "+ ByteArrayToString(bufferTwo));

        switch (TicketStringRE)
        {
            //case "000000000044444444":
            //    TicketCertification = true;
            //    Data.Credit += 2000;
            //    Data.Ticketin = true;
            //    Data.Messagetext = "Ticket In" +"2000";
            //    break;
            default:
                TicketCertification = false;
                Mod_Data.Messagetext = "Ticket Reject";
                break;
        }

        if (TicketCertification)
        {
            mySerialPort.DiscardOutBuffer();
            mySerialPort.DiscardInBuffer();
            mySerialPort.Write(Stack2, 0, 5);
            Thread.Sleep(100); //200
            mySerialPort.DiscardOutBuffer();
            mySerialPort.DiscardInBuffer();
            mySerialPort.Write(vals,0,5);
            Thread.Sleep(100); //200
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
    void OnApplicationQuit()
    {
        mySerialPort.Write(InhibitClose, 0, 6);
        mySerialPort.Close();
        ThreadStop = true;
        RequestThread.Abort();
        InputCommand.Abort();
    }

    public void OpenUBA()
    {
        ThreadStop = false;
        StartUBAThread();
    }

    public void CloseUBA()
    {
        mySerialPort.Write(InhibitClose, 0, 6);
        mySerialPort.Close();
        ThreadStop = true;
        RequestThread.Abort();
        InputCommand.Abort();
        Mod_Data.UBA_OpenSetting = true;
        Mod_Data.ErrorrUBA = true;
        Mod_Data.Errortext = ""; 
    }


    public void CreditReset(string Cridit, int OpenPoint, int ClearPoint)
    {

        List<string> GameValuesTxt1 = new List<string>(100);
        GameValuesTxt1.Clear();


        if (GameValuesTxt1.Count < 1)
        {
            GameValuesTxt1.Add("3|18|26|32|26|0|0|1|60|0|1|0|0|" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "|0|0|" + /*GameDataManager._AllStatisticalData.RTP*/"0" + "|1|0|0");
        }
        string orignalvalue = GameValuesTxt1[GameValuesTxt1.Count - 1];
        string[] orignalvaluearray = orignalvalue.Split('|');

        //transfer 
        orignalvaluearray[6] = Cridit;
        orignalvaluearray[14] = (int.Parse(orignalvaluearray[14]) + OpenPoint).ToString();
        orignalvaluearray[15] = (int.Parse(orignalvaluearray[15]) + ClearPoint).ToString();
        string NewCridet = "";

        for (int i = 0; i < 20; i++)
        {
            NewCridet += orignalvaluearray[i] + "|";
        }
        //if (GameValuesTxt1.Count >= 100)
        //{
        GameValuesTxt1.Remove(GameValuesTxt1[GameValuesTxt1.Count - 1]);
        GameValuesTxt1.Add(NewCridet); //value gamedetail
                                       //}
                                       //else
                                       //{
                                       //    GameValuesTxt1.Add(NewCridet); //value
                                       //}


        UInt32 offset;
        offset = 0x1500;
        for (int i = 0; i < GameValuesTxt1.Count; i++)
        {
            byte[] UTF8bytes = Encoding.UTF8.GetBytes(GameValuesTxt1[i]);
            byte[] UTF8Length = new byte[1];
            UTF8Length[0] = (byte)UTF8bytes.Length;
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(UTF8Length, offset, (uint)UTF8Length.Length);
            offset += 0x0001;
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(UTF8bytes, offset, (uint)UTF8bytes.Length);
            offset += UTF8Length[0];
        }
    }


    public static string ByteArrayToString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-","");

        /*StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}",b);
        return hex.ToString();*/
    }
}
