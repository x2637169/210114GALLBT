using MPOST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

#if Server

#region Server
public class MEI_Bill_Acceptor : MonoBehaviour
{
    //public Text _Text, _Text2, T3;
    Acceptor BillAcceptor = new Acceptor();
    PowerUp PupMode = PowerUp.A;
    public string ComPortName = BillAcceptorSettingData.COMPort; //連接COM號
    InvokePump _Invoke = new InvokePump();

    private CalibrateFinishEventHandler CalibrateFinishDelegate;
    private CalibrateProgressEventHandler CalibrateProgressDelegate;
    private CalibrateStartEventHandler CalibrateStartDelegate;
    private CashBoxCleanlinessEventHandler CashBoxCleanlinessDelegate;
    private CashBoxAttachedEventHandler CashBoxAttachedDelegate;
    private CashBoxRemovedEventHandler CashBoxRemovedDelegate;
    private CheatedEventHandler CheatedDelegate;
    private ClearAuditEventHandler ClearAuditDelegate;
    private ConnectedEventHandler ConnectedDelegate;
    private DisconnectedEventHandler DisconnectedDelegate;
    private DownloadFinishEventHandler DownloadFinishDelegate;
    private DownloadProgressEventHandler DownloadProgressDelegate;
    private DownloadRestartEventHandler DownloadRestartDelegate;
    private DownloadStartEventHandler DownloadStartDelegate;
    private ErrorOnSendMessageEventHandler ErrorOnSendMessageDelegate;
    private EscrowEventHandler EscrowedDelegate;
    private FailureClearedEventHandler FailureClearedDelegate;
    private FailureDetectedEventHandler FailureDetectedDelegate;
    private InvalidCommandEventHandler InvalidCommandDelegate;
    private JamClearedEventHandler JamClearedDelegate;
    private JamDetectedEventHandler JamDetectedDelegate;
    private NoteRetrievedEventHandler NoteRetrievedDelegate;
    private PauseClearedEventHandler PauseClearedDelegate;
    private PauseDetectedEventHandler PauseDetectedDelegate;
    private PowerUpCompleteEventHandler PowerUpCompleteDelegate;
    private PowerUpEventHandler PowerUpDelegate;
    private PUPEscrowEventHandler PUPEscrowDelegate;
    private RejectedEventHandler RejectedDelegate;
    private ReturnedEventHandler ReturnedDelegate;
    private StackedEventHandler StackedDelegate;
    // A new stacked event with document information has been added. Recommanded to be used.
    private StackedWithDocInfoEventHandler StackedWithDocInfoDelegate;
    private StackerFullClearedEventHandler StackerFullClearedDelegate;
    private StackerFullEventHandler StackerFullDelegate;
    private StallClearedEventHandler StallClearedDelegate;
    private StallDetectedEventHandler StallDetectedDelegate;
    // Start is called before the first frame update
    public void Start()
    {
        CalibrateFinishDelegate = new CalibrateFinishEventHandler(HandleCalibrateFinishEvent);
        CalibrateProgressDelegate = new CalibrateProgressEventHandler(HandleCalibrateProgressEvent);
        CalibrateStartDelegate = new CalibrateStartEventHandler(HandleCalibrateStartEvent);
        CashBoxCleanlinessDelegate = new CashBoxCleanlinessEventHandler(HandleCashBoxCleanlinessEvent);
        CashBoxAttachedDelegate = new CashBoxAttachedEventHandler(HandleCashBoxAttachedEvent);
        CashBoxRemovedDelegate = new CashBoxRemovedEventHandler(HandleCashBoxRemovedEvent);
        CheatedDelegate = new CheatedEventHandler(HandleCheatedEvent);
        ClearAuditDelegate = new ClearAuditEventHandler(HandleClearAuditEvent);
        ConnectedDelegate = new ConnectedEventHandler(HandleConnectedEvent);
        DisconnectedDelegate = new DisconnectedEventHandler(HandleDisconnectedEvent);
        DownloadFinishDelegate = new DownloadFinishEventHandler(HandleDownloadFinishEvent);
        DownloadProgressDelegate = new DownloadProgressEventHandler(HandleDownloadProgressEvent);
        DownloadRestartDelegate = new DownloadRestartEventHandler(HandleDownloadRestartEvent);
        DownloadStartDelegate = new DownloadStartEventHandler(HandleDownloadStartEvent);
        ErrorOnSendMessageDelegate = new ErrorOnSendMessageEventHandler(HandleSendMessageErrorEvent);
        EscrowedDelegate = new EscrowEventHandler(HandleEscrowedEvent);
        FailureClearedDelegate = new FailureClearedEventHandler(HandleFailureClearedEvent);
        FailureDetectedDelegate = new FailureDetectedEventHandler(HandleFailureDetectedEvent);
        InvalidCommandDelegate = new InvalidCommandEventHandler(HandleInvalidCommandEvent);
        JamClearedDelegate = new JamClearedEventHandler(HandleJamClearedEvent);
        JamDetectedDelegate = new JamDetectedEventHandler(HandleJamDetectedEvent);
        NoteRetrievedDelegate = new NoteRetrievedEventHandler(HandleNoteRetrievedEvent);
        PauseClearedDelegate = new PauseClearedEventHandler(HandlePauseClearedEvent);
        PauseDetectedDelegate = new PauseDetectedEventHandler(HandlePauseDetectedEvent);
        PowerUpCompleteDelegate = new PowerUpCompleteEventHandler(HandlePowerUpCompleteEvent);
        PowerUpDelegate = new PowerUpEventHandler(HandlePowerUpEvent);
        PUPEscrowDelegate = new PUPEscrowEventHandler(HandlePUPEscrowEvent);
        RejectedDelegate = new RejectedEventHandler(HandleRejectedEvent);
        ReturnedDelegate = new ReturnedEventHandler(HandleReturnedEvent);
        StackedDelegate = new StackedEventHandler(HandleStackedEvent);
        // A new stacked event with document information has been added. Recommanded to be used.
        StackedWithDocInfoDelegate = new StackedWithDocInfoEventHandler(HandleStackedWithDocInfoEvent);
        StackerFullClearedDelegate = new StackerFullClearedEventHandler(HandleStackerFullClearedEvent);
        StackerFullDelegate = new StackerFullEventHandler(HandleStackerFullEvent);
        StallClearedDelegate = new StallClearedEventHandler(HandleStallClearedEvent);
        StallDetectedDelegate = new StallDetectedEventHandler(HandleStallDetectedEvent);

        // Connect to the events.
        BillAcceptor.OnCalibrateFinish += CalibrateFinishDelegate;
        BillAcceptor.OnCalibrateProgress += CalibrateProgressDelegate;
        BillAcceptor.OnCalibrateStart += CalibrateStartDelegate;
        BillAcceptor.OnCashBoxCleanlinessDetected += CashBoxCleanlinessDelegate;
        BillAcceptor.OnCashBoxAttached += CashBoxAttachedDelegate;
        BillAcceptor.OnCashBoxRemoved += CashBoxRemovedDelegate;
        BillAcceptor.OnCheated += CheatedDelegate;
        BillAcceptor.OnClearAuditComplete += ClearAuditDelegate;
        BillAcceptor.OnConnected += ConnectedDelegate;
        BillAcceptor.OnDisconnected += DisconnectedDelegate;
        BillAcceptor.OnDownloadFinish += DownloadFinishDelegate;
        BillAcceptor.OnDownloadProgress += DownloadProgressDelegate;
        BillAcceptor.OnDownloadRestart += DownloadRestartDelegate;
        BillAcceptor.OnDownloadStart += DownloadStartDelegate;
        BillAcceptor.OnSendMessageFailure += ErrorOnSendMessageDelegate;
        BillAcceptor.OnEscrow += EscrowedDelegate;
        BillAcceptor.OnFailureCleared += FailureClearedDelegate;
        BillAcceptor.OnFailureDetected += FailureDetectedDelegate;
        BillAcceptor.OnInvalidCommand += InvalidCommandDelegate;
        BillAcceptor.OnJamCleared += JamClearedDelegate;
        BillAcceptor.OnJamDetected += JamDetectedDelegate;
        BillAcceptor.OnNoteRetrieved += NoteRetrievedDelegate;
        BillAcceptor.OnPauseCleared += PauseClearedDelegate;
        BillAcceptor.OnPauseDetected += PauseDetectedDelegate;
        BillAcceptor.OnPowerUpComplete += PowerUpCompleteDelegate;
        BillAcceptor.OnPowerUp += PowerUpDelegate;
        BillAcceptor.OnPUPEscrow += PUPEscrowDelegate;
        BillAcceptor.OnRejected += RejectedDelegate;
        BillAcceptor.OnReturned += ReturnedDelegate;
        BillAcceptor.OnStacked += StackedDelegate;
        //A new STACKED event with document information has been added. Recommended to be used.
        BillAcceptor.OnStackedWithDocInfo += StackedWithDocInfoDelegate;
        BillAcceptor.OnStackerFullCleared += StackerFullClearedDelegate;
        BillAcceptor.OnStackerFull += StackerFullDelegate;
        BillAcceptor.OnStallCleared += StallClearedDelegate;
        BillAcceptor.OnStallDetected += StallDetectedDelegate;
    }

    public void Open_MEI_Bill(bool TicketEnable)
    {
        try
        {
            //ComPortName = T3.text;
            //ComPortName = "COM3";
            //_Text.text = ComPortName;
            BillAcceptor.Open(BillAcceptorSettingData.COMPort, PupMode);
            //BillAcceptor.EnableAcceptance = true;
            ////BillAcceptor.AutoStack = true;
            //BillAcceptor.AutoStack = false;
            //BillAcceptor.EnableBarCodes = true;
        }
        catch (Exception err)
        {
            BillAcceptorSettingData.ErrorBool = true;
            BillAcceptorSettingData.ErrorMessage = "Unable to open the bill acceptor on com port <" + ComPortName + "> : " + err.Message + "Open Bill Acceptor Error";
            Debug.Log("Unable to open the bill acceptor on com port <" + ComPortName + "> : " + err.Message + "Open Bill Acceptor Error");
            return;
        }

        //EnableBanknote(0);
        //EnableBanknote(2);
        //EnableBanknote(3);
        //if (TicketEnable)
        //{
        //    EnableBanknote(5);
        //}
        //else
        //{
        //    DisnableBanknote(5);
        //}

        //DisnableBanknote(1);
        //DisnableBanknote(4);
    }

    public void Close_MEI_Bill()
    {
        BillAcceptor.Close();
    }

    void EnableBanknote(int BanknoteCode) //TWD  0:100  1:200  2:500  3:1000  4:2000
    {
        if (BanknoteCode < 5)
        {
            Boolean[] enables = BillAcceptor.GetBillTypeEnables();
            enables[BanknoteCode] = true;
            BillAcceptor.SetBillTypeEnables(ref enables);
        }
        if (BanknoteCode == 5)
        {
            try
            {
                BillAcceptor.EnableBarCodes = true;
            }
            catch (Exception err)
            {
                Debug.Log("Unable to set enable barscodes : " + err.Message +
                                "Barcodes enable set error");
            }
        }
    }

    void DisnableBanknote(int BanknoteCode) //TWD  0:100  1:200  2:500  3:1000  4:2000
    {
        if (BanknoteCode < 5)
        {
            Boolean[] enables = BillAcceptor.GetBillTypeEnables();
            enables[BanknoteCode] = false;
            BillAcceptor.SetBillTypeEnables(ref enables);
        }
        if (BanknoteCode == 5)
        {
            try
            {
                BillAcceptor.EnableBarCodes = false;
            }
            catch (Exception err)
            {
                Debug.Log("Unable to set enable barscodes : " + err.Message +
                                "Barcodes enable set error");
            }
        }
    }

    void TicketIn(string TicketValue)
    {
        BillAcceptorSettingData.TicketIn = true;
        BillAcceptorSettingData.TicketType = "Ticket";
        BillAcceptorSettingData.TicketValue = TicketValue;
    }

    void BanknoteIn(int BanknoteCode) //TWD  0:100  1:200  2:500  3:1000  4:2000
    {
        if (!BillAcceptorSettingData.StopCashIn)
        {
            switch (BanknoteCode)
            {
                case 0:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "100";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣壹佰";
                    Debug.Log("新台幣壹佰");
                    break;
                case 1:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "200";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣貳佰";
                    Debug.Log("新台幣貳佰");
                    break;
                case 2:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "500";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣伍佰";
                    Debug.Log("新台幣伍佰");
                    break;
                case 3:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "1000";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣壹仟";
                    Debug.Log("新台幣壹仟");
                    break;
                case 4:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "2000";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣貳仟";
                    Debug.Log("新台幣貳仟");
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    Open_MEI_Bill();
        //}
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    Close_MEI_Bill();
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    DisnableBanknote(0);
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    EnableBanknote(0);
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    DisnableBanknote(1);
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    EnableBanknote(1);
        //}

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    DisnableBanknote(2);
        //}

        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    EnableBanknote(2);
        //}

        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    DisnableBanknote(3);
        //}

        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    EnableBanknote(3);
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    DisnableBanknote(4);
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    //BillAcceptor.EnableAcceptance = true;
        //    //Debug.Log("2");
        //    //BillAcceptor.AutoStack = true;
        //    ////BillAcceptor.AutoStack = false;
        //    //BillAcceptor.EnableBarCodes = true;
        //    EnableBanknote(4);
        //}

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    DisnableBanknote(5);
        //}

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    EnableBanknote(5);
        //}

        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    ListEvent("Command: BillAccepter.EscrowStack()");
        //    try
        //    {
        //        BillAcceptor.EscrowStack();
        //    }
        //    catch (Exception err)
        //    {
        //        Debug.Log("Unable to stack : " + err.Message+
        //                        "Stack error");
        //        return;
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    ListEvent("Command: BillAccepter.EscrowReturn()");
        //    try
        //    {
        //        Debug.Log(BillAcceptor.BarCode);
        //        BillAcceptor.EscrowReturn();
        //    }
        //    catch (Exception err)
        //    {
        //        Debug.Log("Unable to return : " + err.Message +
        //                        "Return error");
        //        return;
        //    }
        //}
    }

    public void StackReturn(bool StackOrReturn) //true:Stack  false:Return
    {

        if (StackOrReturn)
        {
            try
            {
                BillAcceptor.EscrowStack();
            }
            catch (Exception err)
            {
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Unable to stack : " + err.Message + "Stack error";
            }
        }
        else
        {
            try
            {
                BillAcceptor.EscrowReturn();
            }
            catch (Exception err)
            {
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Unable to return : " + err.Message + "Return error";
            }
        }
    }

    public void BillAcceptorEnable(bool enable)
    {
        if (enable)
        {
            BillAcceptor.EnableAcceptance = true;
        }
        else
        {
            BillAcceptor.EnableAcceptance = false;
        }
    }

    #region Event Handlers

    private void HandleCalibrateFinishEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Calibrate Finish.", false);
    }
    private void HandleCalibrateProgressEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Calibrate Progress.", false);
    }
    private void HandleCalibrateStartEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Calibrate Start.", false);
    }

    private void HandleCashBoxCleanlinessEvent(object sender, CashBoxCleanlinessEventArgs e)
    {
        ListEvent(string.Format("Event: Cashbox Cleanliness - {0}", e.Value.ToString()), false);
    }
    private void HandleCashBoxAttachedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cassette Installed.", false);
    }
    private void HandleCashBoxRemovedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cassette Removed.", true);
    }
    private void HandleCheatedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cheat Detected.", true);
    }
    private void HandleClearAuditEvent(object sender, ClearAuditEventArgs e)
    {
        if (e.Success)
        {
            ListEvent("Event: Clear Audit Complete: Success", false);
        }
        else
        {
            ListEvent("Event: Clear Audit Complete: FAILED", true);
        }
    }
    private void HandleConnectedEvent(object sender, EventArgs e)
    {
        //PopulateCapabilities();
        PopulateBillSet();
        PopulateBillValue();
        //PopulateProperties();
        //PopulateInfo();
        //CloseBtn.Enabled = true;
        //CalibrateBtn.Enabled = false;
        //DownloadBtn.Enabled = BillAcceptor.CapFlashDownload;
        BillAcceptor.EnableAcceptance = true;
        //BillAcceptor.AutoStack = true;
        BillAcceptor.AutoStack = false;

        BillAcceptor.EnableAcceptance = true;
        //BillAcceptor.AutoStack = true;
        BillAcceptor.AutoStack = false;
        BillAcceptor.EnableBarCodes = true;

        EnableBanknote(0);
        EnableBanknote(2);
        EnableBanknote(3);
        if (BillAcceptorSettingData.TicketEnable)
        {
            EnableBanknote(5);
        }
        else
        {
            DisnableBanknote(5);
        }

        DisnableBanknote(1);
        DisnableBanknote(4);
        ListEvent("Event: Connected.", false);
    }
    private void HandleDisconnectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Disconnected.", true);
    }
    private void HandleDownloadFinishEvent(object sender, AcceptorDownloadFinishEventArgs e)
    {
        if (e.Success)
        {
            ListEvent("Event: Download Finished: OK", false);
        }
        else
        {
            ListEvent("Event: Download Finished: FAILED", true);
        }
    }
    private void HandleDownloadProgressEvent(object sender, AcceptorDownloadEventArgs e)
    {
        if (e.SectorCount % 100 == 0)
        {
            ListEvent("Event: Download Progress:" + e.SectorCount.ToString(), false);
        }
    }
    private void HandleDownloadRestartEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Download Restart.", false);
        //DoDownload();
    }
    private void HandleDownloadStartEvent(object sender, AcceptorDownloadEventArgs e)
    {
        ListEvent("Event: Download Start. Total Sectors: " + e.SectorCount.ToString(), false);
    }
    private void HandleSendMessageErrorEvent(object sender, AcceptorMessageEventArgs e)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Event: Error in send message. ");
        sb.Append(e.Msg.Description);
        sb.Append("  ");
        foreach (byte b in e.Msg.Payload)
        {
            sb.Append(b.ToString("X2") + " ");
        }
        ListEvent(sb.ToString(), true);
        if (BillAcceptor.DeviceState == State.Escrow)
        {
            //StackBtn.Enabled = true;
            //ReturnBtn.Enabled = true;
        }
    }

    //入鈔或入票時執行
    private void HandleEscrowedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Escrowed: " + DocInfoToString(BillAcceptor.DocType, BillAcceptor.getDocument()), false);
        //StackBtn.Enabled = true;
        //ReturnBtn.Enabled = true;
    }
    private void HandleFailureClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Device Failure Cleared. ", false);
    }
    private void HandleFailureDetectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Device Failure Detected. ", true);
    }
    private void HandleInvalidCommandEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Invalid Command.", false);
    }
    private void HandleJamClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Jam Cleared.", false);
    }
    private void HandleJamDetectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Jam Detected.", true);
    }
    private void HandleNoteRetrievedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Note Retrieved.", false);
    }
    private void HandlePauseClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Pause Cleared.", false);
    }
    private void HandlePauseDetectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Pause Detected.", true);
    }
    private void HandlePowerUpCompleteEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Power Up Complete.", false);
    }
    private void HandlePowerUpEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Power Up.", false);
    }
    private void HandlePUPEscrowEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Power Up with Escrow: " + DocInfoToString(BillAcceptor.DocType, BillAcceptor.getDocument()), false);
        //StackBtn.Enabled = true;
        //ReturnBtn.Enabled = true;
    }
    private void HandleRejectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Rejected.", false);
    }
    private void HandleReturnedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Returned.", false);
        //StackBtn.Enabled = false;
        //ReturnBtn.Enabled = false;
    }
    private void HandleStackedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Stacked", false);
        //StackBtn.Enabled = false;
        //ReturnBtn.Enabled = false;
        if (BillAcceptor.CapCashBoxTotal)
        {
            //CashInBox.Text = BillAcceptor.CashBoxTotal.ToString();
            Debug.Log(BillAcceptor.CashBoxTotal.ToString());
        }
    }

    //已確認鈔票或票券 放入錢箱時執行
    private void HandleStackedWithDocInfoEvent(object sender, StackedEventArgs e)
    {
        //ListEvent("Event: StackedWithDocInfo: " + DocInfoToString(e.DocType, e.Document), false);
        //StackBtn.Enabled = false;
        //ReturnBtn.Enabled = false;
        if (BillAcceptor.CapCashBoxTotal)
        {
            //CashInBox.Text = BillAcceptor.CashBoxTotal.ToString();
            Debug.Log(BillAcceptor.CashBoxTotal.ToString());
        }
    }
    private void HandleStackerFullClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cassette Full Cleared.", false);
    }
    private void HandleStackerFullEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cassette Full.", true);
    }
    private void HandleStallClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Stall Cleared.", false);
    }
    private void HandleStallDetectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Stall Detected.", true);
    }

    #endregion

    private void ListEvent(String e, bool ErrorOrNormal)
    {
        //_Text2.text = e;
        if (ErrorOrNormal)
        {
            BillAcceptorSettingData.ErrorBool = true;
            BillAcceptorSettingData.ErrorMessage = e;
            BillAcceptorSettingData.NormalMessage = "";
            BillAcceptorSettingData.NormalBool = false;
        }
        else
        {
            BillAcceptorSettingData.NormalBool = true;
            BillAcceptorSettingData.NormalMessage = e;
            BillAcceptorSettingData.ErrorMessage = "";
            BillAcceptorSettingData.ErrorBool = false;
        }
        Debug.Log(e);
    }

    private void PopulateBillSet()
    {
        MPOST.Bill[] Bills = BillAcceptor.BillTypes;
        Boolean[] Enables;
        try
        {
            Enables = BillAcceptor.GetBillTypeEnables();
        }
        catch (Exception err)
        {
            Debug.Log("Unable to populate bill set : " + err.Message + "Populate bill set error");
            return;
        }
    }

    private void PopulateBillValue()
    {
        MPOST.Bill[] Bills = BillAcceptor.BillValues;
        Boolean[] Enables;
        try
        {
            Enables = BillAcceptor.GetBillValueEnables();
        }
        catch (Exception err)
        {
            Debug.Log("Unable to populate bill values : " + err.Message + "Populate bill values error");
            return;
        }
    }

    private String DocInfoToString(DocumentType docType, IDocument doc)
    {
        if (docType == DocumentType.None)
            return "Doc Type: None";
        else if (docType == DocumentType.NoValue)
            return "Doc Type: No Value";
        else if (docType == DocumentType.Bill)
        {
            if (doc == null)
                return "Doc Type Bill = null";
            else if (!BillAcceptor.CapOrientationExt)
            {
                if (doc.ToString() == "TWD 100 B A B A") { BanknoteIn(0); }
                if (doc.ToString() == "TWD 200 A A B A") { BanknoteIn(1); }
                if (doc.ToString() == "TWD 500 C A B B") { BanknoteIn(2); }
                if (doc.ToString() == "TWD 1000 C A B B") { BanknoteIn(3); }
                if (doc.ToString() == "TWD 2000 A A B A") { BanknoteIn(4); }
                return "Doc Type Bill = " + doc.ToString() + " (Classification: " + BillAcceptor.EscrowClassification.ToString() + ")";
            }
            else
                return "Doc Type Bill = " + doc.ToString() +
                       " (" + BillAcceptor.EscrowOrientation.ToString() + ", Classification: " + BillAcceptor.EscrowClassification.ToString() + ")";
        }
        else if (docType == DocumentType.Barcode)
        {
            if (doc == null)
                return "Doc Type Bar Code = null";
            else
            {
                TicketIn(doc.ToString());
                return "Doc Type Bar Code = " + doc.ToString();
            }
        }
        else if (docType == DocumentType.Coupon)
        {
            if (doc == null)
                return "Doc Type Coupon = null";
            else
                return "Doc Type Coupon = " + doc.ToString();
        }
        else
            return "Unknown Doc Type Error";
    }

}





/// <summary>
/// Queue actions up to be called during some other threads update pump.
/// </summary>
public class InvokePump : WaitHandle
{

    #region Fields

    private int _threadId;

    //private Action _invoking;
    private Queue<Action> _invoking;
    private object _invokeLock = new object();
    private EventWaitHandle _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
    //private EventWaitHandle _waitHandleAlt = new EventWaitHandle(false, EventResetMode.AutoReset);

    #endregion

    #region CONSTRUCTOR

    public InvokePump()
        : this(null)
    {

    }

    public InvokePump(Thread ownerThread)
    {
        _threadId = (ownerThread != null) ? ownerThread.ManagedThreadId : System.Threading.Thread.CurrentThread.ManagedThreadId;

        _invoking = new Queue<Action>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns true if on a thread other than the one that owns this pump.
    /// </summary>
    public bool InvokeRequired
    {
        get { return _threadId != 0 && Thread.CurrentThread.ManagedThreadId != _threadId; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Queues an action to be invoked next time Update is called. This method will block until that occurs.
    /// </summary>
    /// <param name="action"></param>
    public void Invoke(Action action)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        if (action == null) throw new System.ArgumentNullException("action");

        lock (_invokeLock)
        {
            //_invoking += action;
            _invoking.Enqueue(action);
        }
        _waitHandle.WaitOne(); //block until it's called
    }

    /// <summary>
    /// Queues an action to be invoked next time Update is called. This method does not block.
    /// </summary>
    /// <param name="action"></param>
    public void BeginInvoke(Action action)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (action == null) throw new System.ArgumentNullException("action");

        lock (_invokeLock)
        {
            //_invoking += action;
            _invoking.Enqueue(action);
        }
    }

    /// <summary>
    /// Can only be called by the thread that owns this InvokePump, this will run all queued actions.
    /// </summary>
    public void Update()
    {
        if (_threadId == 0) return; //we're destroyed
        if (this.InvokeRequired) throw new System.InvalidOperationException("InvokePump.Update can only be updated on the thread that was designated its owner.");

        //record the current length so we only activate those actions added at this point
        //any newly added actions should wait until NEXT update
        int cnt = _invoking.Count;
        for (int i = 0; i < cnt; i++)
        {
            Action act;
            lock (_invokeLock)
            {
                act = _invoking.Dequeue();
            }

            if (act != null) act();
        }

        //release waits
        _waitHandle.Set();
    }

    #endregion

    #region Overrides

    public override void Close()
    {
        base.Close();

        if (_threadId == 0) return; //already was destroyed
                                    //_invoking = null;
        _invoking.Clear();
        _waitHandle.Close();
        //_waitHandleAlt.Close();
        _threadId = 0;
    }

    protected override void Dispose(bool explicitDisposing)
    {
        base.Dispose(explicitDisposing);

        if (_threadId == 0) return; //already was destroyed
                                    //_invoking = null;
        _invoking.Clear();
        (_waitHandle as IDisposable).Dispose();
        //(_waitHandleAlt as IDisposable).Dispose();
        _threadId = 0;
    }

    public override bool WaitOne()
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne();
    }

    public override bool WaitOne(int millisecondsTimeout)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne(millisecondsTimeout);
    }

    public override bool WaitOne(int millisecondsTimeout, bool exitContext)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne(millisecondsTimeout, exitContext);
    }

    public override bool WaitOne(TimeSpan timeout)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne(timeout);
    }

    public override bool WaitOne(TimeSpan timeout, bool exitContext)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne(timeout, exitContext);
    }


    #endregion

}
#endregion

#else

#region !Server
public class MEI_Bill_Acceptor : MonoBehaviour
{
 
    //public Text _Text, _Text2, T3;
    Acceptor BillAcceptor = new Acceptor();
    PowerUp PupMode = PowerUp.A;
    public string ComPortName = BillAcceptorSettingData.COMPort; //連接COM號
    InvokePump _Invoke = new InvokePump();

    private CalibrateFinishEventHandler CalibrateFinishDelegate;
    private CalibrateProgressEventHandler CalibrateProgressDelegate;
    private CalibrateStartEventHandler CalibrateStartDelegate;
    private CashBoxCleanlinessEventHandler CashBoxCleanlinessDelegate;
    private CashBoxAttachedEventHandler CashBoxAttachedDelegate;
    private CashBoxRemovedEventHandler CashBoxRemovedDelegate;
    private CheatedEventHandler CheatedDelegate;
    private ClearAuditEventHandler ClearAuditDelegate;
    private ConnectedEventHandler ConnectedDelegate;
    private DisconnectedEventHandler DisconnectedDelegate;
    private DownloadFinishEventHandler DownloadFinishDelegate;
    private DownloadProgressEventHandler DownloadProgressDelegate;
    private DownloadRestartEventHandler DownloadRestartDelegate;
    private DownloadStartEventHandler DownloadStartDelegate;
    private ErrorOnSendMessageEventHandler ErrorOnSendMessageDelegate;
    private EscrowEventHandler EscrowedDelegate;
    private FailureClearedEventHandler FailureClearedDelegate;
    private FailureDetectedEventHandler FailureDetectedDelegate;
    private InvalidCommandEventHandler InvalidCommandDelegate;
    private JamClearedEventHandler JamClearedDelegate;
    private JamDetectedEventHandler JamDetectedDelegate;
    private NoteRetrievedEventHandler NoteRetrievedDelegate;
    private PauseClearedEventHandler PauseClearedDelegate;
    private PauseDetectedEventHandler PauseDetectedDelegate;
    private PowerUpCompleteEventHandler PowerUpCompleteDelegate;
    private PowerUpEventHandler PowerUpDelegate;
    private PUPEscrowEventHandler PUPEscrowDelegate;
    private RejectedEventHandler RejectedDelegate;
    private ReturnedEventHandler ReturnedDelegate;
    private StackedEventHandler StackedDelegate;
    // A new stacked event with document information has been added. Recommanded to be used.
    private StackedWithDocInfoEventHandler StackedWithDocInfoDelegate;
    private StackerFullClearedEventHandler StackerFullClearedDelegate;
    private StackerFullEventHandler StackerFullDelegate;
    private StallClearedEventHandler StallClearedDelegate;
    private StallDetectedEventHandler StallDetectedDelegate;
    bool isEscrowedError = false;
    float EscrowedError_f = 0;

    // Start is called before the first frame update
    public void Start()
    {
        CalibrateFinishDelegate = new CalibrateFinishEventHandler(HandleCalibrateFinishEvent);
        CalibrateProgressDelegate = new CalibrateProgressEventHandler(HandleCalibrateProgressEvent);
        CalibrateStartDelegate = new CalibrateStartEventHandler(HandleCalibrateStartEvent);
        CashBoxCleanlinessDelegate = new CashBoxCleanlinessEventHandler(HandleCashBoxCleanlinessEvent);
        CashBoxAttachedDelegate = new CashBoxAttachedEventHandler(HandleCashBoxAttachedEvent);
        CashBoxRemovedDelegate = new CashBoxRemovedEventHandler(HandleCashBoxRemovedEvent);
        CheatedDelegate = new CheatedEventHandler(HandleCheatedEvent);
        ClearAuditDelegate = new ClearAuditEventHandler(HandleClearAuditEvent);
        ConnectedDelegate = new ConnectedEventHandler(HandleConnectedEvent);
        DisconnectedDelegate = new DisconnectedEventHandler(HandleDisconnectedEvent);
        DownloadFinishDelegate = new DownloadFinishEventHandler(HandleDownloadFinishEvent);
        DownloadProgressDelegate = new DownloadProgressEventHandler(HandleDownloadProgressEvent);
        DownloadRestartDelegate = new DownloadRestartEventHandler(HandleDownloadRestartEvent);
        DownloadStartDelegate = new DownloadStartEventHandler(HandleDownloadStartEvent);
        ErrorOnSendMessageDelegate = new ErrorOnSendMessageEventHandler(HandleSendMessageErrorEvent);
        EscrowedDelegate = new EscrowEventHandler(HandleEscrowedEvent);
        FailureClearedDelegate = new FailureClearedEventHandler(HandleFailureClearedEvent);
        FailureDetectedDelegate = new FailureDetectedEventHandler(HandleFailureDetectedEvent);
        InvalidCommandDelegate = new InvalidCommandEventHandler(HandleInvalidCommandEvent);
        JamClearedDelegate = new JamClearedEventHandler(HandleJamClearedEvent);
        JamDetectedDelegate = new JamDetectedEventHandler(HandleJamDetectedEvent);
        NoteRetrievedDelegate = new NoteRetrievedEventHandler(HandleNoteRetrievedEvent);
        PauseClearedDelegate = new PauseClearedEventHandler(HandlePauseClearedEvent);
        PauseDetectedDelegate = new PauseDetectedEventHandler(HandlePauseDetectedEvent);
        PowerUpCompleteDelegate = new PowerUpCompleteEventHandler(HandlePowerUpCompleteEvent);
        PowerUpDelegate = new PowerUpEventHandler(HandlePowerUpEvent);
        PUPEscrowDelegate = new PUPEscrowEventHandler(HandlePUPEscrowEvent);
        RejectedDelegate = new RejectedEventHandler(HandleRejectedEvent);
        ReturnedDelegate = new ReturnedEventHandler(HandleReturnedEvent);
        StackedDelegate = new StackedEventHandler(HandleStackedEvent);
        // A new stacked event with document information has been added. Recommanded to be used.
        StackedWithDocInfoDelegate = new StackedWithDocInfoEventHandler(HandleStackedWithDocInfoEvent);
        StackerFullClearedDelegate = new StackerFullClearedEventHandler(HandleStackerFullClearedEvent);
        StackerFullDelegate = new StackerFullEventHandler(HandleStackerFullEvent);
        StallClearedDelegate = new StallClearedEventHandler(HandleStallClearedEvent);
        StallDetectedDelegate = new StallDetectedEventHandler(HandleStallDetectedEvent);

        // Connect to the events.
        BillAcceptor.OnCalibrateFinish += CalibrateFinishDelegate;
        BillAcceptor.OnCalibrateProgress += CalibrateProgressDelegate;
        BillAcceptor.OnCalibrateStart += CalibrateStartDelegate;
        BillAcceptor.OnCashBoxCleanlinessDetected += CashBoxCleanlinessDelegate;
        BillAcceptor.OnCashBoxAttached += CashBoxAttachedDelegate;
        BillAcceptor.OnCashBoxRemoved += CashBoxRemovedDelegate;
        BillAcceptor.OnCheated += CheatedDelegate;
        BillAcceptor.OnClearAuditComplete += ClearAuditDelegate;
        BillAcceptor.OnConnected += ConnectedDelegate;
        BillAcceptor.OnDisconnected += DisconnectedDelegate;
        BillAcceptor.OnDownloadFinish += DownloadFinishDelegate;
        BillAcceptor.OnDownloadProgress += DownloadProgressDelegate;
        BillAcceptor.OnDownloadRestart += DownloadRestartDelegate;
        BillAcceptor.OnDownloadStart += DownloadStartDelegate;
        BillAcceptor.OnSendMessageFailure += ErrorOnSendMessageDelegate;
        BillAcceptor.OnEscrow += EscrowedDelegate;
        BillAcceptor.OnFailureCleared += FailureClearedDelegate;
        BillAcceptor.OnFailureDetected += FailureDetectedDelegate;
        BillAcceptor.OnInvalidCommand += InvalidCommandDelegate;
        BillAcceptor.OnJamCleared += JamClearedDelegate;
        BillAcceptor.OnJamDetected += JamDetectedDelegate;
        BillAcceptor.OnNoteRetrieved += NoteRetrievedDelegate;
        BillAcceptor.OnPauseCleared += PauseClearedDelegate;
        BillAcceptor.OnPauseDetected += PauseDetectedDelegate;
        BillAcceptor.OnPowerUpComplete += PowerUpCompleteDelegate;
        BillAcceptor.OnPowerUp += PowerUpDelegate;
        BillAcceptor.OnPUPEscrow += PUPEscrowDelegate;
        BillAcceptor.OnRejected += RejectedDelegate;
        BillAcceptor.OnReturned += ReturnedDelegate;
        BillAcceptor.OnStacked += StackedDelegate;
        //A new STACKED event with document information has been added. Recommended to be used.
        BillAcceptor.OnStackedWithDocInfo += StackedWithDocInfoDelegate;
        BillAcceptor.OnStackerFullCleared += StackerFullClearedDelegate;
        BillAcceptor.OnStackerFull += StackerFullDelegate;
        BillAcceptor.OnStallCleared += StallClearedDelegate;
        BillAcceptor.OnStallDetected += StallDetectedDelegate;
        //StartCoroutine(CustomrLog());
    }

    public void Open_MEI_Bill(bool TicketEnable)
    {
        try
        {
            //ComPortName = T3.text;
            //ComPortName = "COM3";
            //_Text.text = ComPortName;
            BillAcceptor.Open(BillAcceptorSettingData.COMPort, PupMode);
            BillAcceptor.DisconnectTimeout = 3000;
            //ListEvent("Event: Power Up.", false);
            //BillAcceptor.EnableAcceptance = true;
            ////BillAcceptor.AutoStack = true;
            //BillAcceptor.AutoStack = false;
            //BillAcceptor.EnableBarCodes = true;
        }
        catch (Exception err)
        {
            BillAcceptorSettingData.ErrorBool = true;
            BillAcceptorSettingData.ErrorMessage = "Open Bill Acceptor Error";
            //BillAcceptorSettingData.ErrorMessage = "Unable to open the bill acceptor on com port <" + ComPortName + "> : " + err.Message + "Open Bill Acceptor Error";
            //Debug.Log("Unable to open the bill acceptor on com port <" + ComPortName + "> : " + err.Message + "Open Bill Acceptor Error");
        }

        //EnableBanknote(0);
        //EnableBanknote(2);
        //EnableBanknote(3);
        //if (TicketEnable)
        //{
        //    EnableBanknote(5);
        //}
        //else
        //{
        //    DisnableBanknote(5);
        //}

        //DisnableBanknote(1);
        //DisnableBanknote(4);
    }

    public void Close_MEI_Bill()
    {
        if (BillAcceptor.Connected)
        {
            BillAcceptor.EscrowReturn();
        }

        try
        {
            BillAcceptor.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    private void OnApplicationQuit()
    {
        if (BillAcceptor.Connected)
        {
            BillAcceptor.EscrowReturn();
        }

        try
        {
            BillAcceptor.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    void EnableBanknote(int BanknoteCode) //TWD  0:100  1:200  2:500  3:1000  4:2000
    {
        if (BanknoteCode < 5)
        {
            Boolean[] enables = BillAcceptor.GetBillTypeEnables();
            enables[BanknoteCode] = true;
            BillAcceptor.SetBillTypeEnables(ref enables);
        }
        if (BanknoteCode == 5)
        {
            try
            {
                BillAcceptor.EnableBarCodes = true;
            }
            catch (Exception err)
            {
               // Debug.Log("Unable to set enable barscodes : " + err.Message +
                                //"Barcodes enable set error");
            }
        }
    }

    void DisnableBanknote(int BanknoteCode) //TWD  0:100  1:200  2:500  3:1000  4:2000
    {
        if (BanknoteCode < 5)
        {
            Boolean[] enables = BillAcceptor.GetBillTypeEnables();
            enables[BanknoteCode] = false;
            BillAcceptor.SetBillTypeEnables(ref enables);
        }
        if (BanknoteCode == 5)
        {
            try
            {
                BillAcceptor.EnableBarCodes = false;
            }
            catch (Exception err)
            {
                //Debug.Log("Unable to set enable barscodes : " + err.Message +
                              // "Barcodes enable set error");
            }
        }
    }

    void TicketIn(string TicketValue)
    {
        BillAcceptorSettingData.TicketIn = true;
        BillAcceptorSettingData.TicketType = "Ticket";
        BillAcceptorSettingData.TicketValue = TicketValue;
    }

    void BanknoteIn(int BanknoteCode) //TWD  0:100  1:200  2:500  3:1000  4:2000
    {
        if (!BillAcceptorSettingData.StopCashIn)
        {
            switch (BanknoteCode)
            {
                case 0:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "100";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣壹佰";
                    //Debug.Log("新台幣壹佰");
                    break;
                case 1:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "200";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣貳佰";
                    //Debug.Log("新台幣貳佰");
                    break;
                case 2:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "500";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣伍佰";
                    //Debug.Log("新台幣伍佰");
                    break;
                case 3:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "1000";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣壹仟";
                    //Debug.Log("新台幣壹仟");
                    break;
                case 4:
                    BillAcceptorSettingData.StopCashIn = true;
                    BillAcceptorSettingData.TicketType = "Cash";
                    BillAcceptorSettingData.TicketValue = "2000";
                    BillAcceptorSettingData.TicketIn = true;
                    //_Text.text = "新台幣貳仟";
                    //Debug.Log("新台幣貳仟");
                    break;
            }
        }
    }

    bool isEnableAtleastOnce = false;
    // Update is called once per frame
    void Update()
    {
        if (BillAcceptor.Connected)
        {
            if (Mod_Data.inBaseSpin && !BillAcceptor.EnableAcceptance)
            {
                //Debug.Log("BillEnable!");
                BillAcceptor.EnableAcceptance = true;
            }
            else if (!Mod_Data.inBaseSpin && BillAcceptor.EnableAcceptance)
            {
                //Debug.Log("Billdsable!");
                BillAcceptor.EnableAcceptance = false;
            }
        }

        //Ticket or Cash In But Jam On the Escrowed.
        if (isEscrowedError)
        {
            EscrowedError_f += Time.unscaledDeltaTime;

            //awhile time show error hint on screen.
            if (EscrowedError_f >= 5)
            {
                if (!BillAcceptorSettingData.ErrorMessage.Contains("EscrowedError!"))
                {
                    BillAcceptorSettingData.ErrorBool = true;
                    BillAcceptorSettingData.ErrorMessage = "EscrowedError!";
                    BillAcceptorSettingData.NormalMessage = "";
                    BillAcceptorSettingData.NormalBool = false;
                }
            }

            //When timeout for ten seconds, try return ticker or cash.
            if (EscrowedError_f >= 5)
            {
                BillAcceptor.EscrowReturn();
                BillAcceptorSettingData.Return = true;
                EscrowedError_f = 0f;
            }
        }

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    Open_MEI_Bill();
        //}
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    Close_MEI_Bill();
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    DisnableBanknote(0);
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    EnableBanknote(0);
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    DisnableBanknote(1);
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    EnableBanknote(1);
        //}

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    DisnableBanknote(2);
        //}

        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    EnableBanknote(2);
        //}

        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    DisnableBanknote(3);
        //}

        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    EnableBanknote(3);
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    DisnableBanknote(4);
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    //BillAcceptor.EnableAcceptance = true;
        //    //Debug.Log("2");
        //    //BillAcceptor.AutoStack = true;
        //    ////BillAcceptor.AutoStack = false;
        //    //BillAcceptor.EnableBarCodes = true;
        //    EnableBanknote(4);
        //}

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    DisnableBanknote(5);
        //}

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    EnableBanknote(5);
        //}

        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    ListEvent("Command: BillAccepter.EscrowStack()");
        //    try
        //    {
        //        BillAcceptor.EscrowStack();
        //    }
        //    catch (Exception err)
        //    {
        //        Debug.Log("Unable to stack : " + err.Message+
        //                        "Stack error");
        //        return;
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    ListEvent("Command: BillAccepter.EscrowReturn()");
        //    try
        //    {
        //        Debug.Log(BillAcceptor.BarCode);
        //        BillAcceptor.EscrowReturn();
        //    }
        //    catch (Exception err)
        //    {
        //        Debug.Log("Unable to return : " + err.Message +
        //                        "Return error");
        //        return;
        //    }
        //}
    }

    public void StackReturn(bool StackOrReturn) //true:Stack  false:Return
    {

        if (StackOrReturn)
        {
            try
            {
                BillAcceptor.EscrowStack();
                BillAcceptorSettingData.Stacked = true;
            }
            catch (Exception err)
            {
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Unable to stack : " + err.Message + "Stack error";
            }
        }
        else
        {
            try
            {
                BillAcceptor.EscrowReturn();
                BillAcceptorSettingData.Return = true;
            }
            catch (Exception err)
            {
                BillAcceptorSettingData.ErrorBool = true;
                BillAcceptorSettingData.ErrorMessage = "Unable to return : " + err.Message + "Return error";
            }
        }
    }

    public void BillAcceptorEnable(bool enable)
    {
        try
        {
            if (enable)
            {
                BillAcceptor.EnableAcceptance = true;
            }
            else
            {
                BillAcceptor.EnableAcceptance = false;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("BillAcceptorEnable: " + ex);
        }
    }

#region Event Handlers

    private void HandleCalibrateFinishEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Calibrate Finish.", false);
    }
    private void HandleCalibrateProgressEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Calibrate Progress.", false);
    }
    private void HandleCalibrateStartEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Calibrate Start.", false);
    }

    private void HandleCashBoxCleanlinessEvent(object sender, CashBoxCleanlinessEventArgs e)
    {
        ListEvent(string.Format("Event: Cashbox Cleanliness - {0}", e.Value.ToString()), false);
    }
    private void HandleCashBoxAttachedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cassette Installed.", false);
    }
    private void HandleCashBoxRemovedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cassette Removed.", true);
    }
    private void HandleCheatedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cheat Detected.", true);
    }
    private void HandleClearAuditEvent(object sender, ClearAuditEventArgs e)
    {
        if (e.Success)
        {
            ListEvent("Event: Clear Audit Complete: Success", false);
        }
        else
        {
            ListEvent("Event: Clear Audit Complete: FAILED", true);
        }
    }
    private void HandleConnectedEvent(object sender, EventArgs e)
    {
        //PopulateCapabilities();
        PopulateBillSet();
        PopulateBillValue();
        //PopulateProperties();
        //PopulateInfo();
        //CloseBtn.Enabled = true;
        //CalibrateBtn.Enabled = false;
        //DownloadBtn.Enabled = BillAcceptor.CapFlashDownload;
        BillAcceptor.EnableAcceptance = true;
        BillAcceptor.AutoStack = false;
        BillAcceptor.EnableBarCodes = true;

        EnableBanknote(0);
        EnableBanknote(2);
        EnableBanknote(3);
        if (BillAcceptorSettingData.TicketEnable)
        {
            EnableBanknote(5);
        }
        else
        {
            DisnableBanknote(5);
        }

        DisnableBanknote(1);
        DisnableBanknote(4);

        BillAcceptorSettingData.Stacked = false;
        BillAcceptorSettingData.Return = false;
        ListEvent("Event: Connected.", false);
    }

    private void HandleDisconnectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Disconnected.", true);
    }

    private void HandleDownloadFinishEvent(object sender, AcceptorDownloadFinishEventArgs e)
    {
        if (e.Success)
        {
            ListEvent("Event: Download Finished: OK", false);
        }
        else
        {
            ListEvent("Event: Download Finished: FAILED", true);
        }
    }
    private void HandleDownloadProgressEvent(object sender, AcceptorDownloadEventArgs e)
    {
        if (e.SectorCount % 100 == 0)
        {
            ListEvent("Event: Download Progress:" + e.SectorCount.ToString(), false);
        }
    }
    private void HandleDownloadRestartEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Download Restart.", false);
        //DoDownload();
    }
    private void HandleDownloadStartEvent(object sender, AcceptorDownloadEventArgs e)
    {
        ListEvent("Event: Download Start. Total Sectors: " + e.SectorCount.ToString(), false);
    }
    private void HandleSendMessageErrorEvent(object sender, AcceptorMessageEventArgs e)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Event: Error in send message. ");
        sb.Append(e.Msg.Description);
        sb.Append("  ");
        foreach (byte b in e.Msg.Payload)
        {
            sb.Append(b.ToString("X2") + " ");
        }
        ListEvent(sb.ToString(), true);
        if (BillAcceptor.DeviceState == State.Escrow)
        {
            //StackBtn.Enabled = true;
            //ReturnBtn.Enabled = true;
        }
    }

    //入鈔或入票時執行
    private void HandleEscrowedEvent(object sender, EventArgs e)
    {
        BillAcceptorSettingData.Stacked = false;
        BillAcceptorSettingData.Return = false;
        ListEvent("Event: Escrowed: " + DocInfoToString(BillAcceptor.DocType, BillAcceptor.getDocument()), false); //Normal
        BillAcceptorSettingData.Stacked = true;
        BillAcceptorSettingData.GameCanPlay = false;
        //StackBtn.Enabled = true;
        //ReturnBtn.Enabled = true;
    }
    private void HandleFailureClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Device Failure Cleared. ", false);
    }
    private void HandleFailureDetectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Device Failure Detected. ", true);
    }
    private void HandleInvalidCommandEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Invalid Command.", false);
    }
    private void HandleJamClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Jam Cleared.", false);
    }
    private void HandleJamDetectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Jam Detected.", true);
    }
    private void HandleNoteRetrievedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Note Retrieved.", false);
    }
    private void HandlePauseClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Pause Cleared.", false);
    }
    private void HandlePauseDetectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Pause Detected.", true);
    }
    private void HandlePowerUpCompleteEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Power Up Complete.", false);
    }
    private void HandlePowerUpEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Power Up.", false);
    }
    private void HandlePUPEscrowEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Power Up with Escrow: " + DocInfoToString(BillAcceptor.DocType, BillAcceptor.getDocument()), false);
        //StackBtn.Enabled = true;
        //ReturnBtn.Enabled = true;
    }
    private void HandleRejectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Rejected.", false);
        BillAcceptorSettingData.GameCanPlay = true;
    }
    private void HandleReturnedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Returned.", false);
        BillAcceptorSettingData.Return = true;
        BillAcceptorSettingData.GameCanPlay = true;
        //StackBtn.Enabled = false;
        //ReturnBtn.Enabled = false;
    }
    private void HandleStackedEvent(object sender, EventArgs e)
    {
        //Debug.Log("Stacked");
        ListEvent("Event: Stacked", false);
        BillAcceptorSettingData.Stacked = false;
        BillAcceptorSettingData.Return = false;
        BillAcceptorSettingData.GameCanPlay = true;

        //StackBtn.Enabled = false;
        //ReturnBtn.Enabled = false;
        if (BillAcceptor.CapCashBoxTotal)
        {
            //CashInBox.Text = BillAcceptor.CashBoxTotal.ToString();
            //Debug.Log(BillAcceptor.CashBoxTotal.ToString());
        }
    }

    //已確認鈔票或票券 放入錢箱時執行
    private void HandleStackedWithDocInfoEvent(object sender, StackedEventArgs e)
    {
        //ListEvent("Event: StackedWithDocInfo: " + DocInfoToString(e.DocType, e.Document), false);
        //StackBtn.Enabled = false;
        //ReturnBtn.Enabled = false;
        if (BillAcceptor.CapCashBoxTotal)
        {
            //CashInBox.Text = BillAcceptor.CashBoxTotal.ToString();
            //Debug.Log(BillAcceptor.CashBoxTotal.ToString());
        }
    }
    private void HandleStackerFullClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cassette Full Cleared.", false);
    }
    private void HandleStackerFullEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Cassette Full.", true);
    }
    private void HandleStallClearedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Stall Cleared.", false);
    }
    private void HandleStallDetectedEvent(object sender, EventArgs e)
    {
        ListEvent("Event: Stall Detected.", true);
    }

#endregion

    private void ListEvent(String e, bool ErrorOrNormal)
    {
        isEscrowedError = e.Contains("Escrowed") ? true : false;
        if (!isEscrowedError) EscrowedError_f = 0;

        if (ErrorOrNormal)
        {
            BillAcceptorSettingData.ErrorBool = true;
            BillAcceptorSettingData.ErrorMessage = e;
            BillAcceptorSettingData.NormalMessage = "";
            BillAcceptorSettingData.NormalBool = false;
        }
        else
        {
            BillAcceptorSettingData.NormalBool = true;
            BillAcceptorSettingData.NormalMessage = e;
            BillAcceptorSettingData.ErrorMessage = "";
            BillAcceptorSettingData.ErrorBool = false;
        }
        //Debug.Log(e);
    }

    private void PopulateBillSet()
    {
        MPOST.Bill[] Bills = BillAcceptor.BillTypes;
        Boolean[] Enables;
        try
        {
            Enables = BillAcceptor.GetBillTypeEnables();
        }
        catch (Exception err)
        {
            Debug.Log("Unable to populate bill set : " + err.Message + "Populate bill set error");
            return;
        }
    }

    private void PopulateBillValue()
    {
        MPOST.Bill[] Bills = BillAcceptor.BillValues;
        Boolean[] Enables;
        try
        {
            Enables = BillAcceptor.GetBillValueEnables();
        }
        catch (Exception err)
        {
            Debug.Log("Unable to populate bill values : " + err.Message + "Populate bill values error");
            return;
        }
    }

    private String DocInfoToString(DocumentType docType, IDocument doc)
    {
        if (docType == DocumentType.None)
            return "Doc Type: None";
        else if (docType == DocumentType.NoValue)
            return "Doc Type: No Value";
        else if (docType == DocumentType.Bill)
        {
            if (doc == null)
                return "Doc Type Bill = null";
            else if (!BillAcceptor.CapOrientationExt)
            {
                if (doc.ToString() == "TWD 100 B A B B") { BanknoteIn(0); }
                //if (doc.ToString() == "TWD 200 A A B A") { BanknoteIn(1); }
                if (doc.ToString() == "TWD 500 C A B C") { BanknoteIn(2); }
                if (doc.ToString() == "TWD 1000 C A B C") { BanknoteIn(3); }
                //if (doc.ToString() == "TWD 2000 A A B A") { BanknoteIn(4); }
                return "Doc Type Bill = " + doc.ToString() + " (Classification: " + BillAcceptor.EscrowClassification.ToString() + ")";
            }
            else
                return "Doc Type Bill = " + doc.ToString() +
                       " (" + BillAcceptor.EscrowOrientation.ToString() + ", Classification: " + BillAcceptor.EscrowClassification.ToString() + ")";
        }
        else if (docType == DocumentType.Barcode)
        {
            if (doc == null)
                return "Doc Type Bar Code = null";
            else
            {
                TicketIn(doc.ToString());
                return "Doc Type Bar Code = " + doc.ToString();
            }
        }
        else if (docType == DocumentType.Coupon)
        {
            if (doc == null)
                return "Doc Type Coupon = null";
            else
                return "Doc Type Coupon = " + doc.ToString();
        }
        else
            return "Unknown Doc Type Error";
    }

}





/// <summary>
/// Queue actions up to be called during some other threads update pump.
/// </summary>
public class InvokePump : WaitHandle
{

#region Fields

    private int _threadId;

    //private Action _invoking;
    private Queue<Action> _invoking;
    private object _invokeLock = new object();
    private EventWaitHandle _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
    //private EventWaitHandle _waitHandleAlt = new EventWaitHandle(false, EventResetMode.AutoReset);

#endregion

#region CONSTRUCTOR

    public InvokePump()
        : this(null)
    {

    }

    public InvokePump(Thread ownerThread)
    {
        _threadId = (ownerThread != null) ? ownerThread.ManagedThreadId : System.Threading.Thread.CurrentThread.ManagedThreadId;

        _invoking = new Queue<Action>();
    }

#endregion

#region Properties

    /// <summary>
    /// Returns true if on a thread other than the one that owns this pump.
    /// </summary>
    public bool InvokeRequired
    {
        get { return _threadId != 0 && Thread.CurrentThread.ManagedThreadId != _threadId; }
    }

#endregion

#region Methods

    /// <summary>
    /// Queues an action to be invoked next time Update is called. This method will block until that occurs.
    /// </summary>
    /// <param name="action"></param>
    public void Invoke(Action action)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        if (action == null) throw new System.ArgumentNullException("action");

        lock (_invokeLock)
        {
            //_invoking += action;
            _invoking.Enqueue(action);
        }
        _waitHandle.WaitOne(); //block until it's called
    }

    /// <summary>
    /// Queues an action to be invoked next time Update is called. This method does not block.
    /// </summary>
    /// <param name="action"></param>
    public void BeginInvoke(Action action)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (action == null) throw new System.ArgumentNullException("action");

        lock (_invokeLock)
        {
            //_invoking += action;
            _invoking.Enqueue(action);
        }
    }

    /// <summary>
    /// Can only be called by the thread that owns this InvokePump, this will run all queued actions.
    /// </summary>
    public void Update()
    {
        if (_threadId == 0) return; //we're destroyed
        if (this.InvokeRequired) throw new System.InvalidOperationException("InvokePump.Update can only be updated on the thread that was designated its owner.");

        //record the current length so we only activate those actions added at this point
        //any newly added actions should wait until NEXT update
        int cnt = _invoking.Count;
        for (int i = 0; i < cnt; i++)
        {
            Action act;
            lock (_invokeLock)
            {
                act = _invoking.Dequeue();
            }

            if (act != null) act();
        }

        //release waits
        _waitHandle.Set();
    }

#endregion

#region Overrides

    public override void Close()
    {
        base.Close();

        if (_threadId == 0) return; //already was destroyed
                                    //_invoking = null;
        _invoking.Clear();
        _waitHandle.Close();
        //_waitHandleAlt.Close();
        _threadId = 0;
    }

    protected override void Dispose(bool explicitDisposing)
    {
        base.Dispose(explicitDisposing);

        if (_threadId == 0) return; //already was destroyed
                                    //_invoking = null;
        _invoking.Clear();
        (_waitHandle as IDisposable).Dispose();
        //(_waitHandleAlt as IDisposable).Dispose();
        _threadId = 0;
    }

    public override bool WaitOne()
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne();
    }

    public override bool WaitOne(int millisecondsTimeout)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne(millisecondsTimeout);
    }

    public override bool WaitOne(int millisecondsTimeout, bool exitContext)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne(millisecondsTimeout, exitContext);
    }

    public override bool WaitOne(TimeSpan timeout)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne(timeout);
    }

    public override bool WaitOne(TimeSpan timeout, bool exitContext)
    {
        if (_threadId == 0) throw new System.InvalidOperationException("InvokePump has been closed.");
        if (Thread.CurrentThread.ManagedThreadId == _threadId) throw new System.InvalidOperationException("Never call WaitOne on an InvokePump from the thread that owns it, this will freeze that thread indefinitely.");
        return _waitHandle.WaitOne(timeout, exitContext);
    }


#endregion

}
#endregion

#endif