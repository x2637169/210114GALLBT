using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class BillAcceptorController : MonoBehaviour
{
    public Dropdown BillNameDropDown, COMPortDropDown, TicketCashDropDown;
    public Button TicketEnableButton;
    public Text ErrorText, NormalText, insertText, BillOpenCloseText, TicketOpenCloseText, TestText;
    public MEI_Bill_Acceptor MEI;
    public ICT_Bill_Acceptor ICT;
    public JCM_Bill_Acceptor JCM;
    [SerializeField] NewSramManager newSramManager;
    [SerializeField] Mod_OpenClearPoint mod_OpenClearPoint;
    [SerializeField] Mod_UIController mod_UIController;
    [SerializeField] Mod_Client mod_Client;
    Mod_TextController mod_TextController;

#if Server
    #region Server
    float TimeCount = 0;
    public static bool cashIn = false;
    public bool BillOpenClose = false;
    float normalMessageTimer = 0;

    /* void Awake()
     {
         MEI = FindObjectOfType<MEI_Bill_Acceptor>();
         ICT = FindObjectOfType<ICT_Bill_Acceptor>();
         JCM = FindObjectOfType<JCM_Bill_Acceptor>();
         newSramManager = FindObjectOfType<NewSramManager>();
         mod_OpenClearPoint = FindObjectOfType<Mod_OpenClearPoint>();
         mod_UIController = FindObjectOfType<Mod_UIController>();
         BillNameDropDown = GameObject.Find("BillName").GetComponent<Dropdown>();
         COMPortDropDown = GameObject.Find("BillCOMPort").GetComponent<Dropdown>();
         TicketEnableButton = GameObject.Find("TicketEnable").GetComponent<Button>();
         NormalText = GameObject.Find("BillNormalText").GetComponent<Text>();
         BillOpenCloseText = GameObject.Find("BillOpenCloseText").GetComponent<Text>();
         TicketOpenCloseText = GameObject.Find("TicketOpenCloseText").GetComponent<Text>();
         TestText = GameObject.Find("BillTestText").GetComponent<Text>();
     }*/

    void Start()
    {
                TicketCashDropDown.gameObject.SetActive(true);
        TicketEnableButton.gameObject.SetActive(false);
        mod_TextController = FindObjectOfType<Mod_TextController>();
        mod_Client = FindObjectOfType<Mod_Client>();
        StartCoroutine(OpenBiil());
    }

    IEnumerator OpenBiil()
    {
        newSramManager.LoadBanknoteMachineName(out BillAcceptorSettingData.BillName);
        newSramManager.LoadBanknoteMachineCOMPort(out BillAcceptorSettingData.COMPort);
        newSramManager.LoadBillMachineCashOrTicketEnable(out BillAcceptorSettingData.TicketCashOpenClose);
        TicketCashOpenClose();
        COMPortDropDown.value = int.Parse(BillAcceptorSettingData.COMPort.Substring(3, 1)) - 1;

        bool waitServer = true;
        while (waitServer)  //等待伺服器回傳
        {
            if (BillAcceptorSettingData.BillEnableWaitServer != 2) waitServer = false; //伺服器回傳值判斷失敗還是成功
            //if (Mod_Client_Data.serverdisconnect) waitServer = false; //伺服器斷線
            yield return new WaitForSecondsRealtime(1f);
        }

        if (BillAcceptorSettingData.BillEnableWaitServer == 1 && BillAcceptorSettingData.BillOpen)
        {
            Debug.Log("Bill Open");
            BillOpenCloseButton();
        }
        else
        {
            Debug.Log("Bill Not Open");
        }

        BillAcceptorSettingData.BillEnableWaitServer = 2;
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.U))
        {
            BillAcceptorSettingData.TicketInWaitSever = 0;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            BillAcceptorSettingData.TicketInWaitSever = 1;
        }*/

        if (BillOpenClose)
        {
            TicketCashOpenClose();
            BillOpenClose = false;
        }

        if (BillAcceptorSettingData.ErrorBool)
        {
            TestText.text = "Error";
        }
        else if (BillAcceptorSettingData.NormalBool)
        {
            TestText.text = "NormalBool";
        }
        else
        {
            TestText.text = "NoData";
        }

        if (BillAcceptorSettingData.ErrorBool)
        {
            if (!string.IsNullOrEmpty(BillAcceptorSettingData.ErrorMessage))    //接收錯誤訊息
            {
                //ErrorText.text = BillAcceptorSettingData.ErrorMessage;
                BillAcceptorSettingData.ErrorMessage = "";
                Mod_Data.billLock = true;
            }
        }
        else
        {
            BillAcceptorSettingData.ErrorMessage = "";
            //ErrorText.text = BillAcceptorSettingData.ErrorMessage;
            Mod_Data.billLock = false;
        }

        if (BillAcceptorSettingData.NormalBool)
        {
            if (!string.IsNullOrEmpty(BillAcceptorSettingData.NormalMessage))   //接收一般訊息
            {

                NormalText.text = BillAcceptorSettingData.NormalMessage;
                BillAcceptorSettingData.NormalMessage = "";
                BillAcceptorSettingData.NormalBool = false;

            }
        }

        if (BillAcceptorSettingData.TicketIn && !cashIn)   //接收鈔額票額
        {
            BillAcceptorSettingData.TicketIn = false;
            if (BillAcceptorSettingData.TicketType == "Cash")
            {
                switch (BillAcceptorSettingData.BillName)
                {
                    case "JCM":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得鈔額
                        Debug.Log("JCash  " + BillAcceptorSettingData.TicketValue);
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        BillAcceptorSettingData.JCM_WaitRespond = false;
                        //
                        break;
                    case "MEI":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得鈔額
                        Debug.Log("MCash  " + BillAcceptorSettingData.TicketValue);
                        //傳送資料  決定吸入還是退出
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        //MEI.StackReturn(true);
                        //BillAcceptorSettingData.TicketValue = "";
                        //BillAcceptorSettingData.TicketType = "";
                        break;
                    case "ICT":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得鈔額
                        Debug.Log("ICash  " + BillAcceptorSettingData.TicketValue);
                        //傳送資料  決定吸入還是退出
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        break;
                    default:
                        break;
                }
            }
            else if (BillAcceptorSettingData.TicketType == "Ticket")
            {
                switch (BillAcceptorSettingData.BillName)
                {
                    case "JCM":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得鈔額
                        Debug.Log("Ticket  " + BillAcceptorSettingData.TicketValue);
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        BillAcceptorSettingData.JCM_WaitRespond = false;
                        break;
                    case "MEI":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得票值
                        Debug.Log("Ticket  " + BillAcceptorSettingData.TicketValue);
                        //傳送資料  決定吸入還是退出
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        //MEI.StackReturn(true);  //true:Stack  false:Return
                        //BillAcceptorSettingData.TicketValue = "";
                        //BillAcceptorSettingData.TicketType = "";
                        break;
                    case "ICT":
                        Debug.Log("Error ICT不吸票 不可能進來這裡");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                BillAcceptorSettingData.StackReturnBool = false;
                BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
                BillAcceptorSettingData.TicketType = null;
                BillAcceptorSettingData.TicketValue = null;
                BillAcceptorSettingData.JCM_WaitRespond = false;
            }
        }

        if (!string.IsNullOrEmpty(BillAcceptorSettingData.GetOrderType))    //指令設定完成 傳送指令
        {
            switch (BillAcceptorSettingData.GetOrderType)
            {
                case "StackReturnTicket":
                    GetOrderStackReturn(BillAcceptorSettingData.StackReturnBool);
                    break;
                case "BillEnableDisable":
                    GetOrderBillEnableDisable(BillAcceptorSettingData.BillAcceptorEnable);
                    break;
                default:
                    break;
            }

        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    BillAcceptorSettingData.GetOrderType = "BillEnableDisable";//吸鈔功能開關  遊戲過程中可能要開啟或關閉
        //}
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    BillAcceptorSettingData.BillAcceptorEnable = true;
        //}
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    BillAcceptorSettingData.BillAcceptorEnable = false;
        //}
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    BillAcceptorSettingData.StackReturnBool = true;
        //}
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    BillAcceptorSettingData.StackReturnBool = false;
        //}
    }
    public void ValueSend(string Value)
    {
        //BillAcceptorSettingData.ValueSendBool = true;
        Debug.Log("ValueSend: " + " IOLock: " + Mod_Data.IOLock + " autoPlay: " + Mod_Data.autoPlay + " winErrorLock: " + Mod_Data.winErrorLock + " monthLock: " + Mod_Data.monthLock
                + " billLock: " + Mod_Data.billLock + " Win: " + Mod_Data.Win + " CashOrTicketInMaxLimit: " + BillAcceptorSettingData.CashOrTicketInMaxLimit + 1);
        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && Mod_Data.Win <= 0 && double.Parse(Value) < BillAcceptorSettingData.CashOrTicketInMaxLimit + 1)
        {
            double addCredit = 0;
            addCredit = double.Parse(Value);
            if (BillAcceptorSettingData.TicketType == "Cash" && BillAcceptorSettingData.CashEnable)
            {
                BillAcceptorSettingData.StackReturnBool = true;
                BillAcceptorSettingData.GetOrderType = "StackReturnTicket";

                switch (Value)
                {
                    case "100":
                        addCredit = 100;
                        break;
                    case "500":
                        addCredit = 500;
                        break;
                    case "1000":
                        addCredit = 1000;
                        break;
                    default:
                        BillAcceptorSettingData.StackReturnBool = false;
                        BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
                        break;
                }

                if (BillAcceptorSettingData.StackReturnBool)
                {
                    StartCoroutine(AddCredit(0, addCredit));
                }
            }
            else if (BillAcceptorSettingData.TicketType == "Ticket" && BillAcceptorSettingData.TicketEnable)
            {
                BillAcceptorSettingData.StackReturnBool = true;
                BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
                StartCoroutine(AddCredit(1, addCredit));
            }
            else
            {
                BillAcceptorSettingData.StackReturnBool = false;
                BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
                BillAcceptorSettingData.billreturn = false;
                BillAcceptorSettingData.billstack = false;
                BillAcceptorSettingData.StopCashIn = false;
            }
        }
        else
        {
            BillAcceptorSettingData.StackReturnBool = false;
            BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
            BillAcceptorSettingData.billreturn = false;
            BillAcceptorSettingData.billstack = false;
            BillAcceptorSettingData.StopCashIn = false;
        }
    }

    WaitForSecondsRealtime waitLongTime = new WaitForSecondsRealtime(1f);
    WaitForSecondsRealtime waitShotrTime = new WaitForSecondsRealtime(0.01f);
    IEnumerator AddCredit(int cashOrTicket, double addCredit)
    {
        yield return new WaitUntil(() => (BillAcceptorSettingData.billstack && (JCM.Buffer == 0x14 || JCM.Buffer == 0x15 || JCM.Buffer == 0x16)) || BillAcceptorSettingData.billreturn || Mod_Data.handpay || mod_TextController.noticeTextBool[1] == true || mod_TextController.noticeTextBool[2] == true);
        //Debug.Log("billstack: " + BillAcceptorSettingData.billstack + " billreturn: " + BillAcceptorSettingData.billreturn + " mod_TextController.noticeTextBool[1]: " + mod_TextController.noticeTextBool[1] + " mod_TextController.noticeTextBool[2]: " + mod_TextController.noticeTextBool[2]);
        yield return waitLongTime;
        yield return waitLongTime;

        if (!BillAcceptorSettingData.billreturn && !mod_TextController.noticeTextBool[1] && !mod_TextController.noticeTextBool[2] && !Mod_Data.handpay)
        {
            if (cashOrTicket == 0)
            {
                Mod_TextController.billAndTciketInfo_Text.text = "ACCEPTOR INSERT : " + (addCredit.ToString("N2"));
                Mod_Data.credit += addCredit;
                mod_OpenClearPoint.CashInFunction((int)addCredit);
                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.coinIn, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.coinIn) + (int)addCredit);
                BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.coinIn_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.coinIn_Class) + (int)addCredit);
                newSramManager.saveEventRecoredByEventName(4, (int)addCredit);
                mod_UIController.UpdateScore();
                newSramManager.SaveCoinInPoint(newSramManager.LoadCoinInPoint() + (int)addCredit);

                mod_Client.SendToSever(Mod_Client_Data.messagetype.billin);
                while (true)  //等待伺服器回傳             //通訊-鈔機入鈔訊號
                {
                    if (!JCM_Bill_Acceptor.cashin_waitServer) break;
                    if (Mod_Client_Data.serverdisconnect)
                    {
                        while (true)
                        {
                            if (!Mod_Client_Data.serverdisconnect && Mod_Data.machineInit)
                            {
                                mod_Client.SendToSever(Mod_Client_Data.messagetype.billin);
                                break;
                            }
                            yield return waitLongTime;
                        }
                    }
                    yield return waitShotrTime;
                }
            }
            else if (cashOrTicket == 1)
            {
                if (BillAcceptorSettingData.TicketInWaitSever == 1)
                {
                    BillAcceptorSettingData.TicketInWaitSever = 2; //預設值
                    mod_Client.SendToSever(Mod_Client_Data.messagetype.ticketChangeToInvalid);
                    while (true)  //等待伺服器回傳
                    {
                        if (BillAcceptorSettingData.TicketInWaitSever != 2) break;
                        if (Mod_Client_Data.serverdisconnect)
                        {
                            if (newSramManager.LoadHandPayStatus() == 1 || newSramManager.LoadHandPayStatus() == 3) newSramManager.SaveHandPayStatus(3);
                            else newSramManager.SaveHandPayStatus(2);
                            newSramManager.SaveTicketSerial(Mod_Data.serial);
                            Mod_Client.isCallHandPay = true;
                            yield return waitLongTime;
                            break;
                        }
                        yield return waitShotrTime;
                    }
                }

                if (!Mod_Data.handpay && newSramManager.LoadHandPayStatus() == 0)
                {
                    Mod_TextController.billAndTciketInfo_Text.text = "ACCEPTOR INSERT : " + (addCredit.ToString("N2"));
                    Mod_Data.credit += addCredit;
                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.ticketIn, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.ticketIn) + addCredit);
                    BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.ticketIn_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.ticketIn_Class) + addCredit);

                    newSramManager.saveEventRecoredByEventName(5, (int)(addCredit * 100));
                    newSramManager.SaveTicketInPoint(newSramManager.LoadTicketInPoint() + addCredit);
                    mod_UIController.UpdateScore();
                }
            }
        }
        else
        {
            if (cashOrTicket == 1 && (BillAcceptorSettingData.billstack || JCM.Buffer == 0x14 || JCM.Buffer == 0x15 || JCM.Buffer == 0x16))
            {
                if (newSramManager.LoadHandPayStatus() == 1 || newSramManager.LoadHandPayStatus() == 3) newSramManager.SaveHandPayStatus(3);
                else newSramManager.SaveHandPayStatus(2);
                newSramManager.SaveTicketSerial(Mod_Data.serial);
                Mod_Client.isCallHandPay = true;
            }
        }

        BillAcceptorSettingData.TicketValue = null;
        BillAcceptorSettingData.TicketType = null;
        BillAcceptorSettingData.billreturn = false;
        BillAcceptorSettingData.billstack = false;
        BillAcceptorSettingData.StopCashIn = false;
        cashIn = false;
        Debug.Log("AddCredit Done!");
    }

    void GetOrderBillEnableDisable(bool EnableDisable) //true:Enable  false:Disable
    {
        BillAcceptorSettingData.GetOrderType = "";
        BillAcceptorSettingData.TicketIn = false;
        BillAcceptorSettingData.StackReturnBool = false;
        JCM_Bill_Acceptor.TicketCertification = false;
        BillAcceptorSettingData.JCM_WaitRespond = false;
        if (BillAcceptorSettingData.TicketInWaitSever != 1)
        {
            BillAcceptorSettingData.TicketType = null;
            BillAcceptorSettingData.TicketValue = null;
            BillAcceptorSettingData.TicketInWaitSever = 0;
        }

        Debug.Log("BillAcceptorEnable Done!");
        switch (BillAcceptorSettingData.BillName)
        {
            case "JCM":
                JCM.BillAcceptorEnable(EnableDisable);
                break;
            case "MEI":
                MEI.BillAcceptorEnable(EnableDisable);
                break;
            case "ICT":
                ICT.BillAcceptorEnable(EnableDisable);
                break;
            default:
                break;
        }
        //BillAcceptorSettingData.ValueSendBool = false;
    }

    void GetOrderStackReturn(bool StackReturn) //true:Stack   false:Return
    {
        BillAcceptorSettingData.GetOrderType = "";
        switch (BillAcceptorSettingData.BillName)
        {
            case "JCM":
                JCM.StackReturn(StackReturn);
                break;
            case "MEI":
                MEI.StackReturn(StackReturn);
                break;
            case "ICT":
                ICT.StackReturn(StackReturn);
                break;
            default:
                break;
        }
        //BillAcceptorSettingData.ValueSendBool = false;
        BillAcceptorSettingData.TicketValue = "";
        BillAcceptorSettingData.TicketType = "";
        cashIn = false;
        BillAcceptorSettingData.StopCashIn = false;
    }

    public void ChangeBillName()
    {
        if (!BillAcceptorSettingData.BillOpenClose)
        {
            switch (BillNameDropDown.value)
            {
                case 0:
                    BillAcceptorSettingData.BillName = "JCM";
                    Debug.Log("JCM");
                    break;
                case 1:
                    BillAcceptorSettingData.BillName = "MEI";
                    Debug.Log("MEI");
                    break;
                case 2:
                    BillAcceptorSettingData.BillName = "ICT";
                    if (BillAcceptorSettingData.TicketEnable)
                    {
                        TicketCashOpenClose();
                    }
                    Debug.Log("ICT");
                    break;
            }
        }
    }

    public void ChangeCOMPort()
    {
        if (!BillAcceptorSettingData.BillOpenClose)
        {
            BillAcceptorSettingData.COMPort = "COM" + (COMPortDropDown.value + 1);
            Debug.Log("COM" + (COMPortDropDown.value + 1));
        }
    }

    public void BillOpenCloseButton()
    {
        if (!BillAcceptorSettingData.BillOpenClose)
        {
            BillAcceptorSettingData.BillOpenClose = true;
            if (BillAcceptorSettingData.BillName == "ICT" && BillAcceptorSettingData.TicketEnable)
            {
                TicketCashOpenClose();
            }
            BillNameDropDown.interactable = false;
            COMPortDropDown.interactable = false;
            OpenBill();
            BillOpenCloseText.text = BillAcceptorSettingData.BillName + " BillAcceptor NowOpen";
            newSramManager.SaveBanknoteMachineCOMPort(BillAcceptorSettingData.COMPort);
            newSramManager.SaveBanknoteMachineName(BillAcceptorSettingData.BillName);
            newSramManager.SaveBanknoteMachineTicketEnable(BillAcceptorSettingData.BillOpenClose);
            newSramManager.SaveBillMachineCashOrTicketEnable(BillAcceptorSettingData.TicketCashOpenClose);
        }
        else
        {
            BillAcceptorSettingData.BillOpenClose = false;
            CloseBill();
            BillNameDropDown.interactable = true;
            COMPortDropDown.interactable = true;
            BillOpenCloseText.text = BillAcceptorSettingData.BillName + " BillAcceptor NowClose";
            newSramManager.SaveBanknoteMachineCOMPort(BillAcceptorSettingData.COMPort);
            newSramManager.SaveBanknoteMachineName(BillAcceptorSettingData.BillName);
            newSramManager.SaveBanknoteMachineTicketEnable(BillAcceptorSettingData.BillOpenClose);
            newSramManager.SaveBillMachineCashOrTicketEnable(BillAcceptorSettingData.TicketCashOpenClose);
        }
        if (BillAcceptorSettingData.BillOpenClose)
        {
            BillAcceptorSettingData.BillAcceptorEnable = true;
            BillAcceptorSettingData.GetOrderType = "BillEnableDisable";
        }
    }

    public void TicketCashOpenCloseDropDown()
    {
        BillAcceptorSettingData.TicketCashOpenClose = TicketCashDropDown.value;
        TicketCashOpenClose();
    }

    public void TicketCashOpenClose()
    {
        Debug.Log("TicketCashOpenClose: " + BillAcceptorSettingData.TicketCashOpenClose);
        TicketCashDropDown.value = BillAcceptorSettingData.TicketCashOpenClose;

        switch (BillAcceptorSettingData.TicketCashOpenClose)
        {
            case 0:
                BillAcceptorSettingData.CashEnable = true;
                BillAcceptorSettingData.TicketEnable = false;
                TicketOpenCloseText.text = "CashEnable " + BillAcceptorSettingData.CashEnable;
                break;
            case 1:
                BillAcceptorSettingData.CashEnable = false;
                BillAcceptorSettingData.TicketEnable = true;
                TicketOpenCloseText.text = "TicketEnable " + BillAcceptorSettingData.TicketEnable;
                break;
            case 2:
                BillAcceptorSettingData.CashEnable = true;
                BillAcceptorSettingData.TicketEnable = true;
                TicketOpenCloseText.text = "TicketEnable " + BillAcceptorSettingData.TicketEnable + "\nCashEnable " + BillAcceptorSettingData.CashEnable;
                break;
        }

        if (BillAcceptorSettingData.TicketEnable && BillAcceptorSettingData.BillName == "ICT")
        {
            BillAcceptorSettingData.TicketEnable = false;
            BillAcceptorSettingData.CashEnable = true;
            TicketCashDropDown.value = 0;
            BillAcceptorSettingData.TicketCashOpenClose = 0;
            TicketOpenCloseText.text = "TicketEnable " + BillAcceptorSettingData.TicketEnable + "\nCashEnable " + BillAcceptorSettingData.CashEnable;
        }
        newSramManager.SaveBillMachineCashOrTicketEnable(BillAcceptorSettingData.TicketCashOpenClose);
    }

    public void ResetBill()
    {
        Debug.Log("ResetBill!");
        JCM.ResetBill();
    }

    void OpenBill()
    {
        switch (BillAcceptorSettingData.BillName)
        {
            case "JCM":
                JCM.Open_JCM_Bill();
                break;
            case "MEI":
                MEI.ComPortName = BillAcceptorSettingData.COMPort;
                MEI.Open_MEI_Bill(BillAcceptorSettingData.TicketEnable);
                break;
            case "ICT":
                ICT.ComPortName = BillAcceptorSettingData.COMPort;
                ICT.Open_ICT_Bill(true, false, true, true, false);
                break;
            default:
                break;
        }
    }

    void CloseBill()
    {
        switch (BillAcceptorSettingData.BillName)
        {
            case "JCM":
                JCM.JCM_Bill_Close();
                break;
            case "MEI":
                MEI.Close_MEI_Bill();
                break;
            case "ICT":
                ICT.Close_ICT_Bill();
                break;
            default:
                break;
        }

        BillAcceptorSettingData.StopCashIn = false;
        BillAcceptorSettingData.StackReturnBool = false;
        BillAcceptorSettingData.JCM_WaitRespond = false;
        BillAcceptorSettingData.CheckIsInBaseSpin = false;
        BillAcceptorSettingData.GameCanPlay = true;
        BillAcceptorSettingData.CashOrTicketIn = false;
        BillAcceptorSettingData.BillOpen = false;
        BillAcceptorSettingData.billreturn = false;
        BillAcceptorSettingData.billstack = false;
    }

    #endregion
#else
    #region !Server
    float TimeCount = 0;
    bool cashIn = false;
    bool isOpenBill = false;
    // Start is called before the first frame update
    void Start()
    {
        TicketCashDropDown.gameObject.SetActive(false);
        TicketEnableButton.gameObject.SetActive(true);
        //讀取記憶體鈔機紀錄，是否開啟鈔機
        newSramManager.LoadBanknoteMachineName(out BillAcceptorSettingData.BillName);
        newSramManager.LoadBanknoteMachineCOMPort(out BillAcceptorSettingData.COMPort);
        newSramManager.LoadBanknoteMachineTicketEnable(out isOpenBill);
        SetDropDownUI();
        if (isOpenBill) BillOpenCloseButton();
    }

    // Update is called once per frame
    float normalMessageTimer = 0;
    float portWrongTimer = 0;
    float errorMessageTimer = 0;
    void Update()
    {
        if (BillAcceptorSettingData.ErrorBool)
        {
            TestText.text = "Error";
        }
        else if (BillAcceptorSettingData.NormalBool)
        {
            TestText.text = "NormalBool";
        }
        else
        {
            TestText.text = "NoData";
        }

        if (BillAcceptorSettingData.BillOpenClose)
        {
            if (BillAcceptorSettingData.ErrorBool)
            {
                if (!string.IsNullOrEmpty(BillAcceptorSettingData.ErrorMessage))    //接收錯誤訊息
                {
                    ErrorText.text = BillAcceptorSettingData.ErrorMessage;
                    BillAcceptorSettingData.ErrorMessage = "";
                }
                Mod_Data.billLock = true;
            }
            else if (string.IsNullOrEmpty(ErrorText.text) && Mod_Data.billLock)
            {
                portWrongTimer += Time.unscaledDeltaTime;

                if (portWrongTimer >= 5)
                {
                    BillAcceptorSettingData.ErrorBool = true;
                    BillAcceptorSettingData.ErrorMessage = "Disconnect";
                    ErrorText.text = BillAcceptorSettingData.ErrorMessage;
                    BillAcceptorSettingData.ErrorMessage = "";
                    portWrongTimer = 0f;
                }

            }
            else
            {
                BillAcceptorSettingData.ErrorMessage = "";
                ErrorText.text = BillAcceptorSettingData.ErrorMessage;
                Mod_Data.billLock = false;
            }
        }
        else
        {
            if (!BillAcceptorSettingData.BillOpenClose && (!string.IsNullOrWhiteSpace(ErrorText.text) || !string.IsNullOrWhiteSpace(NormalText.text)))
            {
                if (errorMessageTimer <= 3) errorMessageTimer += Time.unscaledDeltaTime;
                if (errorMessageTimer >= 3)
                {
                    BillAcceptorSettingData.NormalBool = false;
                    BillAcceptorSettingData.NormalMessage = " ";
                    BillAcceptorSettingData.ErrorBool = false;
                    BillAcceptorSettingData.ErrorMessage = " ";
                    ErrorText.text = null;
                    NormalText.text = null;
                    Mod_Data.billLock = false;
                    errorMessageTimer = 0;
                }
            }
            portWrongTimer = 0f;
        }

        if (BillAcceptorSettingData.NormalBool)
        {
            if (!string.IsNullOrEmpty(BillAcceptorSettingData.NormalMessage))   //接收一般訊息
            {

                NormalText.text = BillAcceptorSettingData.NormalMessage;
                BillAcceptorSettingData.NormalMessage = "";
                BillAcceptorSettingData.NormalBool = false;

            }
        }

        if (BillAcceptorSettingData.TicketIn && !cashIn)   //接收鈔額票額
        {

            BillAcceptorSettingData.TicketIn = false;
            if (BillAcceptorSettingData.TicketType == "Cash")
            {
                switch (BillAcceptorSettingData.BillName)
                {
                    case "JCM":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得鈔額
                        //Debug.Log("JCash  " + BillAcceptorSettingData.TicketValue);
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        BillAcceptorSettingData.JCM_WaitRespond = false;
                        //
                        break;
                    case "MEI":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得鈔額
                        //Debug.Log("MCash  " + BillAcceptorSettingData.TicketValue);
                        //傳送資料  決定吸入還是退出
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        break;
                    case "ICT":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得鈔額
                        //Debug.Log("ICash  " + BillAcceptorSettingData.TicketValue);
                        //傳送資料  決定吸入還是退出
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        break;
                    default:
                        break;
                }
            }
            else if (BillAcceptorSettingData.TicketType == "Ticket")
            {
                switch (BillAcceptorSettingData.BillName)
                {
                    case "JCM":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得鈔額
                        //Debug.Log("Ticket  " + BillAcceptorSettingData.TicketValue);
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        BillAcceptorSettingData.JCM_WaitRespond = false;
                        break;
                    case "MEI":
                        NormalText.text = BillAcceptorSettingData.TicketValue;
                        //取得票值
                        //Debug.Log("Ticket  " + BillAcceptorSettingData.TicketValue);
                        //傳送資料  決定吸入還是退出
                        ValueSend(BillAcceptorSettingData.TicketValue);
                        //MEI.StackReturn(true);  //true:Stack  false:Return
                        //BillAcceptorSettingData.TicketValue = "";
                        //BillAcceptorSettingData.TicketType = "";
                        break;
                    case "ICT":
                        //Debug.Log("Error ICT不吸票 不可能進來這裡");
                        break;
                    default:
                        break;
                }
            }
        }

        if (!string.IsNullOrEmpty(BillAcceptorSettingData.GetOrderType))    //指令設定完成 傳送指令
        {
            switch (BillAcceptorSettingData.GetOrderType)
            {
                case "StackReturnTicket":
                    GetOrderStackReturn(BillAcceptorSettingData.StackReturnBool);
                    break;
                case "BillEnableDisable":
                    GetOrderBillEnableDisable(BillAcceptorSettingData.BillAcceptorEnable);
                    break;
                default:
                    break;
            }

        }

        if (insertText.text != "")
        {
            normalMessageTimer += Time.deltaTime;
            if (normalMessageTimer > 3)
            {
                insertText.text = "";
                normalMessageTimer = 0;
            }
        }
    }

    //收到票或鈔做遊戲加金額處理
    public void ValueSend(string Value)
    {
        //判斷是否退鈔、票
        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && Mod_Data.Win <= 0)
        {
            cashIn = true;
            if (Value == "100" || Value == "500" || Value == "1000")
            {
                //通過協程處理
                StartCoroutine(AddCredit(int.Parse(Value)));
            }
            else
            {
                //退鈔、票
                cashIn = false;
                BillAcceptorSettingData.GameCanPlay = true;
                BillAcceptorSettingData.StackReturnBool = false;
                BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
                BillAcceptorSettingData.StopCashIn = false;
            }
        }
        else
        {
            //退鈔、票
            cashIn = false;
            BillAcceptorSettingData.GameCanPlay = true;
            BillAcceptorSettingData.StackReturnBool = false;
            BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
            BillAcceptorSettingData.StopCashIn = false;
        }
    }

    WaitForSecondsRealtime AddCreditWaitShort = new WaitForSecondsRealtime(0.01f);
    IEnumerator AddCredit(int credit) //增加彩分處理
    {
        BillAcceptorSettingData.Jcm_Buffer = 0x00;
        BillAcceptorSettingData.StackReturnBool = true;
        BillAcceptorSettingData.GetOrderType = "StackReturnTicket";
        int CountWrong = 0;
        bool isAddCredit = false;
        //Debug.Log("AddCredit Wait!");

        if (BillAcceptorSettingData.BillName == "JCM")
        {
            //等待入鈔、票成功或失敗退票
            while (true)
            {
                if (CountWrong >= 500 || (BillAcceptorSettingData.Stacked && BillAcceptorSettingData.Jcm_Buffer == 0x14 || BillAcceptorSettingData.Jcm_Buffer == 0x15 || BillAcceptorSettingData.Jcm_Buffer == 0x16)
   || BillAcceptorSettingData.Return || (BillAcceptorSettingData.Jcm_Buffer == 0x17 || BillAcceptorSettingData.Jcm_Buffer == 0x18)) break;
                yield return AddCreditWaitShort;
                CountWrong++;
            }
            //判斷是否可以增加金額
            if ((BillAcceptorSettingData.Stacked || BillAcceptorSettingData.Jcm_Buffer == 0x14 || BillAcceptorSettingData.Jcm_Buffer == 0x15 || BillAcceptorSettingData.Jcm_Buffer == 0x16)
                        && !BillAcceptorSettingData.Return && BillAcceptorSettingData.Jcm_Buffer != 0x17 && BillAcceptorSettingData.Jcm_Buffer != 0x18 && CountWrong < 500)
            {
                isAddCredit = true;
            }
        }
        else if (BillAcceptorSettingData.BillName == "MEI")
        {
            //等待入鈔、票成功或失敗退票
            while (true)
            {
                if (BillAcceptorSettingData.Stacked || BillAcceptorSettingData.Return || CountWrong >= 500) break;
                yield return AddCreditWaitShort;
                CountWrong++;
            }
            //判斷是否可以增加金額
            if (BillAcceptorSettingData.Stacked && !BillAcceptorSettingData.Return && CountWrong < 500)
            {
                isAddCredit = true;
            }
        }

        if (isAddCredit)
        {
            //增加金額更新UI，並記錄事件、金額至記憶體
            Mod_Data.credit += credit;
            mod_OpenClearPoint.CashInFunction(credit);
            BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.coinIn, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.coinIn) + credit);
            BackEnd_Data.SetInt(BackEnd_Data.SramAccountData.coinIn_Class, BackEnd_Data.GetInt(BackEnd_Data.SramAccountData.coinIn_Class) + credit);
            newSramManager.saveEventRecoredByEventName(4, credit);
            newSramManager.SaveCoinInPoint(newSramManager.LoadCoinInPoint() + credit);
            mod_UIController.UpdateScore();
            insertText.text = "insert " + credit;
        }

        //重置部分設定，讓遊戲能繼續玩
        cashIn = false;
        BillAcceptorSettingData.GameCanPlay = true;
        BillAcceptorSettingData.Stacked = false;
        BillAcceptorSettingData.Return = false;
        BillAcceptorSettingData.StopCashIn = false;
        BillAcceptorSettingData.Jcm_Buffer = 0x00;
        CountWrong = 0;
        isAddCredit = false;
        //Debug.Log("AddCredit Done!");
    }

    void GetOrderBillEnableDisable(bool EnableDisable) //true:Enable  false:Disable
    {
        BillAcceptorSettingData.GetOrderType = "";
        switch (BillAcceptorSettingData.BillName)
        {
            case "JCM":
                JCM.BillAcceptorEnable(EnableDisable);
                break;
            case "MEI":
                MEI.BillAcceptorEnable(EnableDisable);
                break;
            case "ICT":
                ICT.BillAcceptorEnable(EnableDisable);
                break;
            default:
                break;
        }
        //BillAcceptorSettingData.ValueSendBool = false;
    }

    void GetOrderStackReturn(bool StackReturn) //true:Stack   false:Return
    {
        BillAcceptorSettingData.GetOrderType = "";
        switch (BillAcceptorSettingData.BillName)
        {
            case "JCM":
                JCM.StackReturn(StackReturn);
                break;
            case "MEI":
                MEI.StackReturn(StackReturn);
                break;
            case "ICT":
                ICT.StackReturn(StackReturn);
                break;
            default:
                break;
        }
        //BillAcceptorSettingData.ValueSendBool = false;
        BillAcceptorSettingData.TicketValue = "";
        BillAcceptorSettingData.TicketType = "";
        cashIn = false;
        BillAcceptorSettingData.StopCashIn = false;
    }

    public void ChangeBillName()
    {
        if (!BillAcceptorSettingData.BillOpenClose)
        {
            switch (BillNameDropDown.value)
            {
                case 0:
                    BillAcceptorSettingData.BillName = "JCM";
                    //Debug.Log("JCM");
                    break;
                case 1:
                    BillAcceptorSettingData.BillName = "MEI";
                    //Debug.Log("MEI");
                    break;
                case 2:
                    BillAcceptorSettingData.BillName = "ICT";
                    if (BillAcceptorSettingData.TicketEnable)
                    {
                        TicketOpenCloseButton();
                    }
                    //Debug.Log("ICT");
                    break;
            }
        }

        Debug.Log("BillName: " + BillAcceptorSettingData.BillName);
    }

    public void ChangeCOMPort()
    {
        if (!BillAcceptorSettingData.BillOpenClose)
        {
            BillAcceptorSettingData.COMPort = "COM" + (COMPortDropDown.value + 1);
            //Debug.Log("COM" + (COMPortDropDown.value + 1));
        }

        Debug.Log("COMPort: " + BillAcceptorSettingData.COMPort);
    }

    public void SetDropDownUI()
    {
        COMPortDropDown.value = int.Parse(BillAcceptorSettingData.COMPort.Substring(3, 1)) - 1;

        switch (BillAcceptorSettingData.BillName)
        {
            case "JCM":
                BillNameDropDown.value = 0;
                break;
            case "MEI":
                BillNameDropDown.value = 1;
                break;
            case "ICT":
                BillNameDropDown.value = 2;
                break;
            default:
                BillNameDropDown.value = 0;
                break;
        }
    }

    public void BillOpenCloseButton()
    {
        BillAcceptorSettingData.GameCanPlay = true;
        COMPortDropDown.value = int.Parse(BillAcceptorSettingData.COMPort.Substring(3)) - 1;
        if (!BillAcceptorSettingData.BillOpenClose)
        {
            BillAcceptorSettingData.BillOpenClose = true;
            if (BillAcceptorSettingData.BillName == "ICT" && BillAcceptorSettingData.TicketEnable)
            {
                TicketOpenCloseButton();
            }
            BillNameDropDown.enabled = false;
            COMPortDropDown.enabled = false;
            TicketEnableButton.enabled = false;
            BillOpenCloseText.text = BillAcceptorSettingData.BillName + " BillAcceptor NowOpen";
            newSramManager.SaveBanknoteMachineCOMPort(BillAcceptorSettingData.COMPort);
            newSramManager.SaveBanknoteMachineName(BillAcceptorSettingData.BillName);
            newSramManager.SaveBanknoteMachineTicketEnable(BillAcceptorSettingData.BillOpenClose);
            OpenBill();
        }
        else
        {
            BillAcceptorSettingData.BillOpenClose = false;
            BillNameDropDown.enabled = true;
            COMPortDropDown.enabled = true;
            TicketEnableButton.enabled = true;
            BillOpenCloseText.text = BillAcceptorSettingData.BillName + " BillAcceptor NowClose";
            newSramManager.SaveBanknoteMachineCOMPort(BillAcceptorSettingData.COMPort);
            newSramManager.SaveBanknoteMachineName(BillAcceptorSettingData.BillName);
            newSramManager.SaveBanknoteMachineTicketEnable(BillAcceptorSettingData.BillOpenClose);
            CloseBill();
        }
        if (BillAcceptorSettingData.BillOpenClose)
        {
            BillAcceptorSettingData.BillAcceptorEnable = true;
            BillAcceptorSettingData.GetOrderType = "BillEnableDisable";
        }
    }

    public void TicketOpenCloseButton()
    {
        //ICT無法吸票
        if (!BillAcceptorSettingData.TicketEnable && BillAcceptorSettingData.BillName != "ICT")
        {
            BillAcceptorSettingData.TicketEnable = true;
        }
        else
        {
            BillAcceptorSettingData.TicketEnable = false;
        }
        TicketOpenCloseText.text = "TicketEnable " + BillAcceptorSettingData.TicketEnable;
    }

    void OpenBill()
    {
        switch (BillAcceptorSettingData.BillName)
        {
            case "JCM":
                JCM.Open_JCM_Bill();
                break;
            case "MEI":
                MEI.ComPortName = BillAcceptorSettingData.COMPort;
                MEI.Open_MEI_Bill(BillAcceptorSettingData.TicketEnable);
                break;
            case "ICT":
                ICT.ComPortName = BillAcceptorSettingData.COMPort;
                ICT.Open_ICT_Bill(true, false, true, true, false);
                break;
            default:
                break;
        }
    }

    void CloseBill()
    {
        switch (BillAcceptorSettingData.BillName)
        {
            case "JCM":
                JCM.JCM_Bill_Close();
                break;
            case "MEI":
                MEI.Close_MEI_Bill();
                break;
            case "ICT":
                ICT.Close_ICT_Bill();
                break;
            default:
                break;
        }

        BillAcceptorSettingData.NormalBool = false;
        BillAcceptorSettingData.NormalMessage = " ";
        BillAcceptorSettingData.ErrorBool = false;
        BillAcceptorSettingData.ErrorMessage = " ";
        BillAcceptorSettingData.GameCanPlay = true;
        BillAcceptorSettingData.StopCashIn = false;
        BillAcceptorSettingData.Stacked = false;
        BillAcceptorSettingData.Return = false;
        cashIn = false;
        ErrorText.text = null;
        NormalText.text = null;
        Mod_Data.billLock = false;
        errorMessageTimer = 0;
    }
    #endregion
#endif
}