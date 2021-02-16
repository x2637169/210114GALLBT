using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System.Globalization;

#region 客戶端主要程式處理

public class Mod_Client : IGameSystem
{
#if Server
    #region Server

    #region Mod_Client變數

    #region 連接伺服器組件、變數

    Thread severThread; //連接Server的執行緒
    Socket socketClient = null; //客戶端物件，接收收發送訊號
    string serverIp;  //伺服器端IP
    int serverPort; //伺服器端Port
    MyIni serverIni; //連接伺服器用INI檔

    #endregion

    #region 機台錯誤變數

    //要傳送的錯誤
    bool[] sendMachineError = new bool[8];
    //已傳送過的錯誤
    bool[] sendedSendMachineError = new bool[8];
    //正在傳送機台錯誤
    bool isSendingMachineError = false;
    //機台錯誤等待時間-長
    WaitForSecondsRealtime SendMachineErrorLongTime = new WaitForSecondsRealtime(1f);

    #endregion

    #region 機台事件變數

    //傳送機台事件
    bool isSendMachineEvent = false;
    //正在傳送機台事件
    bool isSendingMachineEvent = false;
    //機台事件等待時間-短
    WaitForSecondsRealtime WaitMachineEventSortTime = new WaitForSecondsRealtime(0.1f);
    //機台事件等待時間-長
    WaitForSecondsRealtime WaitMachineEventLongTime = new WaitForSecondsRealtime(1f);
    //機台事件等待直到伺服器沒有斷線並且機台初始化
    WaitUntil WaitMachineEvent_Until = new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
    //強制停止機台事件協程用變數
    Coroutine SendMachineEvent_Coroutine;

    #endregion

    #region 清鈔變數

    //傳送清鈔訊息
    bool isSendBillClear = false;
    //正在傳送清鈔訊息
    bool isSendingBillClear = false;
    //清鈔協程等待時間-短
    WaitForSecondsRealtime WaitBillClearShortTime = new WaitForSecondsRealtime(0.1f);
    //清鈔協程等待時間-長
    WaitForSecondsRealtime WaitBillClearLongTime = new WaitForSecondsRealtime(1f);
    //清鈔協程等待直到伺服器沒有斷線並且機台初始化
    WaitUntil WaitBillClear_Until = new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
    //強制停止清鈔協程用變數
    Coroutine SendBillClear_Coroutine;

    #endregion

    #region 會員登出變數

    //遊戲待機正常會員登入
    public static bool isMemberLogIn = false;
    //遊戲中斷電重啟會員強制登入
    public static bool isMemberLogInHard = false;
    //會員登出
    bool isMemberLogOut = false;
    //會員正在登出
    bool isMemberLoggingOut = false;
    //會員登出協程等待時間-短
    WaitForSecondsRealtime WaitMemberLogOutShortTime = new WaitForSecondsRealtime(0.1f);
    //會員登出協程等待時間-長
    WaitForSecondsRealtime WaitMemberLogOutLongTime = new WaitForSecondsRealtime(1f);
    //會員登出協程等待直到(伺服器沒有斷線並且機台初始化)、(非會員登入、非會員卡拔出或是斷電強制會員登入)、(非手付狀態或是會員ID為空白、空值)
    WaitUntil WaitMemberLogOut_Until = new WaitUntil(() => (!Mod_Client_Data.serverdisconnect && Mod_Data.machineInit
     && (!isMemberLogIn && !Mod_Client_Data.MemberCardOut || isMemberLogInHard) && ((!Mod_Data.handpay && !isWaitServerHandPay) || string.IsNullOrWhiteSpace(Mod_Data.memberID))));
    //強制停止會員登出協程用變數    
    Coroutine isMemberLogOut_Coroutine;

    #endregion

    #region 手付變數

    //需要手付
    public static bool isCallHandPay = false;
    //正在處理手付
    public static bool isHandPaying = false;
    //等待伺服器收到手付訊息
    public static bool isWaitServerHandPay = false;
    //等待工作站處理完手付
    public bool isWaitStationHandPaid = false;
    //手付等待時間-短
    WaitForSecondsRealtime WaitHandPayShortTime = new WaitForSecondsRealtime(0.1f);
    //手付等待時間-長
    WaitForSecondsRealtime WaitHandPayLongTime = new WaitForSecondsRealtime(1f);

    #endregion

    #region 遊戲紀錄變數

    //需要傳送遊戲紀錄
    public bool isCallSendGameHisotry = false;
    //正在傳送遊戲紀錄
    bool isSendingGameHisotry = false;
    //遊戲紀錄等待時間-短
    WaitForSecondsRealtime WaitSendGameHistoryShortTime = new WaitForSecondsRealtime(0.1f);
    //遊戲紀錄等待時間-長
    WaitForSecondsRealtime WaitSendGameHistoryLongTime = new WaitForSecondsRealtime(1f);
    //遊戲紀錄協程等待直到伺服器沒有斷線並且機台初始化
    WaitUntil WaitGameHisotry_Until = new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);

    #endregion

    #region 機台初始化計時器變數

    private float _machineInitTimer = 0;
    ///<summary>
    ///機台初始化計時器，當時間大於3秒會像伺服器傳送訊息，會先向伺服器傳送連線訊息請求機台編號，之後會再傳機台初始值訊息到伺服器，請求機台初數值設定
    ///</summary>
    float machineInitTimer
    {
        get { return _machineInitTimer; }
        set
        {
            _machineInitTimer = value;
            //當時間大於3秒，向伺服器傳送訊息
            if (_machineInitTimer > 3)
            {
                if (string.IsNullOrWhiteSpace(Mod_Data.machineID)) SendToSever(Mod_Client_Data.messagetype.connect); //沒有機台編號，向伺服器傳送連線訊號，請求機台編號
                else if (!Mod_Data.machineInit) SendToSever(Mod_Client_Data.messagetype.gameset); //有機台編號，但機台還未初始化，向伺服器傳送初始化訊息，請求機台初始值設定
                _machineInitTimer = 0;
            }
        }
    }

    #endregion

    #region 伺服器延遲變數

    private float _severDelay_Time = 0;
    ///<summary>
    ///伺服器一段時間未傳送任何訊息到客戶端(機台)，就會開始計時，只要接收到伺服器任何訊息，將計時歸零並停止顯示延遲UI，如果計時大於等於15秒並且延遲UI還未顯示，顯示延遲UI
    ///</summary>
    float severDelay_Time
    {
        get { return _severDelay_Time; }
        set
        {
            _severDelay_Time = value;
            if (_severDelay_Time == 0) Mod_TextController.WaitServerTextRunning = false; //計時為零停止顯示延遲UI
            if (!Mod_TextController.WaitServerTextRunning && _severDelay_Time >= 15) Mod_TextController.RunWaitServerTextBool = true; //計時大於等於15秒並且延遲UI還未顯示，顯示延遲UI
        }
    }

    #endregion

    #region 開洗分變數

    int openClearScore = 0; //開洗分分數
    bool _isOpenClear = false; //是否開洗分

    /// <summary>
    /// 如果開分狀態為True，並且開分分數大於或小於0，增加彩分並更新分數UI
    /// </summary>
    bool isOpenClear
    {
        get { return _isOpenClear; }
        set
        {
            _isOpenClear = value;
            //開分狀態為True
            if (_isOpenClear)
            {
                if (openClearScore > 0)     //開分大於零，呼叫仲介增加彩分
                {
                    m_SlotMediatorController.SendMessage("m_client", "OpenPoint", openClearScore);
                }
                else if (openClearScore < 0)    //開分小於零，呼叫仲介減少彩分
                {
                    m_SlotMediatorController.SendMessage("m_client", "ClearPoint", openClearScore);
                }
                //更新分數UI
                mod_UIController.UpdateScore();
            }
        }
    }

    #endregion

    //讀卡機讀取字串
    string cardReader_Str;
    //清除暫時用手付會員資料
    bool isClearHandPayMemberID = false;
    //清除暫時用手付計時器
    float clearHandPayIDTimer = 0;

    #region 其他組件變數

    BackEnd_Data backEnd_Data;
    BillAcceptorController billAcceptorController;
    Mod_UIController mod_UIController;
    NewSramManager newSramManager;
    Mod_Gen2_Status gen2_Status;
    Mod_OpenClearPoint mod_OpenClearPoint;
    Mod_TextController mod_TextController;

    #endregion

    #endregion

    #region 開起程式初始化  

    void Start()
    {
    #region  抓取組件、更新部分變數

        backEnd_Data = FindObjectOfType<BackEnd_Data>();
        mod_UIController = FindObjectOfType<Mod_UIController>();
        newSramManager = FindObjectOfType<NewSramManager>();
        gen2_Status = FindObjectOfType<Mod_Gen2_Status>();
        mod_OpenClearPoint = FindObjectOfType<Mod_OpenClearPoint>();
        mod_TextController = FindObjectOfType<Mod_TextController>();
        billAcceptorController = FindObjectOfType<BillAcceptorController>();
        Mod_Data.memberLcok = true; //開啟鎖會員
        Mod_Data.machineerror = "0"; //變更回報給伺服器的機台狀態0為正常
        Mod_Data.cardID = " ";
        Mod_Data.barcode = " ";
        Mod_Data.serial = " ";
        Mod_Data.serial2 = " ";

    #endregion

        if (newSramManager.LoadHandPayStatus() != 0) isCallHandPay = true; //讀取記憶體手付狀態不為零則開啟手付處理

    #region 讀取ini設置伺服器變數，連接伺服器

        serverIni = new MyIni(Application.streamingAssetsPath + "/IPsetting.ini"); //設置ini位置
        serverIp = serverIni.ReadIniContent("IP", "serverIp"); //讀取ini檔IP
        serverPort = int.Parse(serverIni.ReadIniContent("Port", "serverPort")); //讀取ini檔Port
        ConnectSeverStarting(); //連接伺服器

    #endregion
    }

    #endregion

    #region Update每偵執行

    void Update()
    {
    #region 卡機

        //讀卡機輸入中
        if (Input.inputString.Length > 0)
        {
            //紀錄讀卡機輸入的字串、元
            cardReader_Str += Input.inputString;
        }
        else
        {
            //讀卡機停止輸入時讀卡機字串不為空白或空值，傳送字串到會員登入檢查，並清空字串
            if (!string.IsNullOrWhiteSpace(cardReader_Str)) MemberLogInCheck(cardReader_Str);
            cardReader_Str = null;
        }

    #endregion

    #region 機台錯誤處理

        //另外使用布林值紀錄是否錯誤，並依照錯誤等級排序
        sendMachineError[0] = Mod_Data.handpay == true ? true : false;
        sendMachineError[1] = Mod_Data.billStackOpenerror == true ? true : false;
        sendMachineError[2] = Mod_Data.printerError == true ? true : false;
        sendMachineError[3] = BillAcceptorSettingData.ErrorBool == true ? true : false;
        sendMachineError[4] = Mod_Data.doorError == true ? true : false;
        sendMachineError[5] = mod_TextController.printerTextBool[26] == true ? true : false;
        sendMachineError[6] = Mod_Data.MachineError == true ? true : false;
        sendMachineError[7] = Mod_Data.serviceLighterror == true ? true : false;

        //如果機台有任何錯誤，使用傳送錯誤訊息協程給伺服器
        if (Mod_Data.billStackOpenerror || Mod_Data.doorError || Mod_Data.serviceLighterror || Mod_Data.MachineError || Mod_Data.printerError || Mod_Data.handpay || BillAcceptorSettingData.ErrorBool || mod_TextController.printerTextBool[26])
        {
            //錯誤訊息協程不再跑的情況下才啟用傳送錯誤訊息協程
            if (!isSendingMachineError)
            {
                //開啟錯誤訊息協
                StartCoroutine("SendMachineError");
            }
        }
        //機台沒有任何錯誤，並且錯誤訊息協程再跑，停止錯誤訊息協程
        else if (isSendingMachineError)
        {
            //停止錯誤訊息協
            StopCoroutine("SendMachineError");
            isSendingMachineError = false;
            //"0"為機台正常
            Mod_Data.machineerror = "0";
            //傳送機台事件
            isSendMachineEvent = true;
        }

    #endregion

    #region 傳送機台事件

        if (isSendMachineEvent)
        {
            //如果傳送機台事件協程正在執行，強制停止協程
            if (SendMachineEvent_Coroutine != null) StopCoroutine(SendMachineEvent_Coroutine);
            //並重新開始一個協程
            SendMachineEvent_Coroutine = StartCoroutine(SendMachineEvent_IE());
            isSendMachineEvent = false;
        }

    #endregion

    #region UI錯誤顯示處理

        //文字控制器對應陣列為True，UI會顯示其錯誤文字(伺服器斷線、伺服器鎖住、機台ID錯誤、機台初始化錯誤)
        Mod_TextController.allTextBool[9] = Mod_Client_Data.serverdisconnect == true ? true : false;
        Mod_TextController.allTextBool[11] = Mod_Data.serverLock == true ? true : false;
        Mod_TextController.allTextBool[12] = Mod_Data.machineIDLock == true ? true : false;
        Mod_TextController.allTextBool[14] = Mod_Data.machineInit == false ? true : false;

        //(伺服器斷線、伺服器鎖住、機台ID錯誤、機台初始化錯誤)和文字控制器錯誤提示協程沒有在執行時，執行錯誤提示協程
        if (!Mod_TextController.ErrorTextRunning && (Mod_Client_Data.serverdisconnect || Mod_Data.serverLock || Mod_Data.machineIDLock || !Mod_Data.machineInit)) Mod_TextController.RunErrorTextBool = true;

    #endregion

    #region 手付

        //呼叫手付、分數贏分錯誤執行手付協程
        if (isCallHandPay || Mod_Data.creditErrorLock || Mod_Data.winErrorLock)
        {
            //如果手付協程沒有執行則開始手付協程
            if (!isHandPaying) StartCoroutine("HandPayCoroutine");
            isCallHandPay = false;
        }

    #endregion

    #region 鎖定、解除會員鎖

        if (Mod_Data.memberLcok)
        {
            //通過仲介者開始鎖定會員鎖
            m_SlotMediatorController.SendMessage("m_state", "memberLock", 1);
            //取消機台自動玩狀態
            Mod_Data.autoPlay = false;
        }
        else
        {
            //通過仲介者開始解除會員鎖
            m_SlotMediatorController.SendMessage("m_state", "memberLock", 0);
        }

    #endregion

    #region 機台錯誤強制會員登出

        //機台需要會員卡，並且已解除會員鎖、無伺服器斷線，機台有錯誤狀態會強制登出
        if (Mod_Data.cardneeded == 1 && !Mod_Client_Data.serverdisconnect && !Mod_Data.memberLcok && !isMemberLoggingOut && !String.IsNullOrWhiteSpace(Mod_Data.memberID) && (Mod_Data.billStackOpenerror || Mod_Data.doorError || Mod_Data.MachineError || Mod_Data.printerError || Mod_Data.handpay))
        {
            //開啟會員鎖
            Mod_Data.memberLcok = true;
            //傳送會員登出
            isMemberLogOut = true;
            //會員卡拔出
            Mod_Client_Data.MemberCardOut = false;
        }

    #endregion

    #region 會員登出

        //會員登出，關閉會員換點UI物件，並執行登出協程
        if (isMemberLogOut)
        {
            isMemberLogOut = false;
            //關閉會員換點UI物件
            Mod_ChangePonit.CloseMemberPanel = true;
            //如果登出協程執行中，強制停止
            if (isMemberLogOut_Coroutine != null) StopCoroutine(isMemberLogOut_Coroutine);
            //開始登出協程
            isMemberLogOut_Coroutine = StartCoroutine(isMemberLogOut_IE());
        }
        else
        {
            //停止等待伺服器回傳登出
            isMemberLoggingOut = false;
        }

    #endregion

        //在沒有伺服器斷線或伺服器鎖住狀態下，一段時間未收到伺服器訊號會透過文字控制器顯示延遲字串在UI上"...."
        if (!Mod_Data.serverLock && !Mod_Client_Data.serverdisconnect) severDelay_Time += Time.unscaledDeltaTime;

        //在沒有伺服器斷線狀態下並且未收到機台初始值或機台ID，一段時間會向伺服器請求機台初始值和機台ID
        if (!Mod_Client_Data.serverdisconnect && (string.IsNullOrWhiteSpace(Mod_Data.machineID) || !Mod_Data.machineInit)) machineInitTimer += Time.unscaledDeltaTime;

    #region 遊戲紀錄

        //需要傳送遊戲紀錄並且傳送遊戲紀錄協程並未執行，執行傳送遊戲紀錄協程
        if (isCallSendGameHisotry && !isSendingGameHisotry)
        {
            StartCoroutine("SendGameHistory_IE");
            isCallSendGameHisotry = false;
        }

    #endregion

    #region 清鈔

        //需要清鈔
        if (isSendBillClear)
        {
            //清鈔協程物件不為空值強制停止協程
            if (SendBillClear_Coroutine != null) StopCoroutine(SendBillClear_Coroutine);
            //啟用清鈔協程
            SendBillClear_Coroutine = StartCoroutine(SendBillClear_IE());
            isSendBillClear = false;
        }

    #endregion

    #region 清理手付用會員ID

        //如果需要清除手付用會員ID，不再手付狀態下經過一段時間清除ID
        if (isClearHandPayMemberID)
        {
            //如果手付用會員ID已經為空白或空值則取消清除
            if (String.IsNullOrWhiteSpace(Mod_Client_Data.handPaymemberID)) isClearHandPayMemberID = false;

            if (!Mod_Data.handpay)
            {
                clearHandPayIDTimer += Time.unscaledDeltaTime;
                if (clearHandPayIDTimer >= 10)
                {
                    Mod_Client_Data.handPaymemberID = " ";
                }
            }
        }

        //不需要清除手付用會員ID或是需要清除手付用會員ID但在手付狀態下將時間重置
        if (!isClearHandPayMemberID || (isClearHandPayMemberID && Mod_Data.handpay))
        {
            clearHandPayIDTimer = 0;
        }

    #endregion
    }

    #endregion

    #region 客戶端、伺服器、接收、發送處理

    /// <summary>
    /// 與伺服器連線，並且使用執行續持續檢測是否和伺服器失去連線，失去連線會嘗試和伺服器再次連線
    /// </summary>
    public void ConnectSeverStarting()
    {
        //使用SOCKET和伺服器嘗試連線
        StaticLink();
        //建立一個新的執行續，並先設置好程序
        severThread = new Thread(() =>
        {
            //持續執行檢測是否和伺服器失去連線，並嘗試重新連線
            while (true)
            {
                //與伺服器失去連線
                if (Mod_Client_Data.serverdisconnect)
                {
                    //丟棄原本客戶端連線
                    socketClient = null;
                    //將機台ID設為空值
                    Mod_Data.machineID = "";
                    //將機台已初始化設為關閉
                    Mod_Data.machineInit = false;
                    //通知機台與伺服器斷線
                    Strhandle("713");
                    //Debug.Log(" Reconnect in 3 secs");
                    Thread.Sleep(3000);
                    //重新與伺服器嘗試連線
                    StaticLink();
                    //Debug.Log("----Reconnecting...----");
                }
                Thread.Sleep(1);
            }
        });
        //開始執行續
        severThread.Start();
    }

    /// <summary>
    /// 與伺服器連線、同步，並生成客戶端和伺服器接收、發送訊息
    /// </summary>
    public void StaticLink()
    {
        try
        {
            //建立客戶端
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //非同步處理
            AsyncCallback asynccallback = new AsyncCallback(ReceiveServerMsg);
            IAsyncResult result = socketClient.BeginConnect(serverIp, serverPort, asynccallback, null);
            socketClient.EndConnect(result);
            //連線成功關閉與伺服器失去連線狀態
            Mod_Client_Data.serverdisconnect = false;
            Debug.Log("connecting");
            Debug.Log("send something to server");
        }
        catch (Exception e) //例外處理
        {
            Debug.Log(e.ToString());
            Debug.Log("client to server failed");
            //將機台ID設為空值
            Mod_Data.machineID = "";
            //將機台已初始化設為關閉
            Mod_Data.machineInit = false;
            //連線失敗開啟與伺服器失去連線狀態
            Mod_Client_Data.serverdisconnect = true;
            //通知機台與伺服器失去連線
            Strhandle("713");
            return;
        }
    }

    /// <summary>
    /// 客戶端接收伺服器端訊息，並通知機台處理對應訊息事件
    /// </summary>
    public void ReceiveServerMsg(IAsyncResult result)
    {
        byte[] receiveData = new byte[1024]; //接收數據
        int receiveLen = 0; //數據長度
        do
        {
            try
            {
                receiveLen = socketClient.Receive(receiveData); //獲取數據長度
                //數據長度大於零，轉換數據為可讀字串，並通知處理對應事件
                if (receiveLen > 0)
                {
                    //轉換數據為可讀字串
                    string receiveStr = System.Text.Encoding.UTF8.GetString(receiveData, 0, receiveLen);
                    Debug.Log("Msg 客戶端：" + receiveStr);
                    //通知處理對應事件
                    Strhandle(receiveStr);
                }
            }
            catch (Exception ex) //例外處理
            {
                //例外錯誤開啟與伺服器失去連線狀態
                Mod_Client_Data.serverdisconnect = true;
                Debug.Log("斷線: " + ex);
                return;
            }
        } while (receiveLen > 0);
    }

    /// <summary>
    /// 客戶端發送訊息到伺服器端
    /// </summary>
    public void SendMessages(string message)
    {
        //如果客戶端與伺服器端連線不存在返回
        if (socketClient == null)
        {
            return;
        }

        //訊息去除逗號如果為空白、空值則返回，不傳訊息到伺服器端
        if (String.IsNullOrWhiteSpace(message.Replace(',', ' '))) return;

        try
        {
            //寫入、傳送訊息到伺服器
            NetworkStream stream = new NetworkStream(socketClient);
            if (stream.CanWrite)
            {
                string clientMessage = message;
                byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client Send:" + clientMessage);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    #endregion

    #region 客戶端訊息處理

    ///<summary>
    ///此功能接收到伺服器訊息根據通訊格式做處理
    ///</summary>
    public void Strhandle(string message)
    {
        //Debug.Log(message);
        //每次收到伺服器的訊息重置延遲時間
        severDelay_Time = 0;
        try
        {
            //伺服器回傳的訊息為空白或空值則返回
            if (String.IsNullOrWhiteSpace(message)) return;
            //使用分號來分割訊息，避免黏包造成訊息錯誤
            string[] words = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            //根據分割出來的訊息長度用迴圈來依序處理
            for (int i = 0; i < words.Length; i++)
            {
                //使用逗號分割出有用的訊息，並做出相對應判斷
                string[] singlesign = words[i].Split(',');
                //If singlesign[1] get "0" then Fail  
                switch (singlesign[0])
                {
                    case "701":  //會員登入-插入會員卡成功與否
                                 //範例:701,1287,X0X0V0Q802b214886a8071a6bf6194b2189dda104d9b424nePFjH&iwk5eZR*@neJl,2020-05-27 上午 12:00:00,1,0,10000.00,1000.99,2020-07-28;
                                 //0:會員登入 1:會員編號 2:密碼 3:生日 4:會員等級 1:一般玩家 2:VIP 5:黑名單 6:累積點數 7:贈分點數 8:系統時間
                        if (singlesign.Length > 2 && !String.IsNullOrWhiteSpace(singlesign[1]) && singlesign[1] != "0")
                        {
                            Mod_Data.memberID = singlesign[1].ToString(); //會員編號
                            Mod_Client_Data.handPaymemberID = Mod_Data.memberID; //手付用會員編號
                            Mod_Data.cardPassword = singlesign[2]; //會員卡密碼
                            Mod_Data.memberBirthday = singlesign[3]; //會員生日
                            Mod_Data.memberlevel = singlesign[4]; //會員等級
                            Mod_Data.memberRakebackPoint = float.Parse(singlesign[6].ToString()); //會員累積點數
                            Mod_Data.bonusPoints = double.Parse(singlesign[7]); //會員贈分點數
                            Mod_Data.severTime = singlesign[8]; //伺服器時間
                            //檢測會員生日月份是否與伺服器月份相同，相同的話更新會員狀態為壽星
                            //壽星:會員等級原本為1更改為2，會員等級原本為2改為4
                            //非壽星:會員等級原本1更改為1，會員等級原本為2改為3
                            if (Mod_Data.memberBirthday.Substring(5, 2).Contains(Mod_Data.severTime.Substring(5, 2))) Mod_Data.memberlevel = Mod_Data.memberlevel == "1" ? "2" : Mod_Data.memberlevel == "2" ? "4" : Mod_Data.memberlevel;
                            else Mod_Data.memberlevel = Mod_Data.memberlevel == "1" ? "1" : Mod_Data.memberlevel == "2" ? "3" : Mod_Data.memberlevel;
                            //取消會員鎖
                            if (Mod_Data.cardneeded == 1) Mod_Data.memberLcok = false;
                            //傳送機台事件更新機台狀態
                            isSendMachineEvent = true;
                            //會員贈分點數小於等於0不顯示贈分UI
                            mod_UIController.text_BonusPoints.text = Mod_Data.bonusPoints <= 0 ? null : Mod_Data.bonusPoints.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty);
                            isMemberLogIn = false;
                            isMemberLogInHard = false;
                        }
                        else
                        {
                            //清空卡片資料
                            Mod_Data.cardID = " ";
                            isMemberLogIn = false;
                            isMemberLogInHard = false;
                        }
                        isClearHandPayMemberID = false;
                        break;
                    case "702":  //入票-入票成功與否
                                 //範例: 702,1,4,1000,6602-8500-5117-0481
                                 //0:入票 1: 0無效票、不存在資料庫 1有效票 2:金額 3:票號
                        BillAcceptorSettingData.TicketValue = "0";
                        if (singlesign[1] == "1")
                        {
                            //存取金額
                            BillAcceptorSettingData.TicketValue = singlesign[3];
                            Mod_Client_Data.ticketInAmount = BillAcceptorSettingData.TicketValue;
                            //入票成功
                            BillAcceptorSettingData.TicketInWaitSever = 1;
                        }
                        else
                        {
                            //入票失敗
                            BillAcceptorSettingData.TicketInWaitSever = 0;
                        }
                        break;
                    case "703": //出票-出票成功與否
                                //範例: 703,1,1000,04-5335-4174-1171-6325,2020-06-16 14:26:07,2020-09-16 14:26:07
                                //0:出票 1: 0成功 1失敗 2:金額 3:票號 4:有效日期 5:印票日期
                        Gen2_Data.TicketOutValue = 0;
                        if (singlesign[1] == "1")
                        {
                            Gen2_Data.TicketOutValue = double.Parse(singlesign[2]); //票額
                            Mod_Data.serial = singlesign[3];    //彩票編號
                            Mod_Data.serial2 = singlesign[3];   //彩票編號
                            Mod_Data.barcode = Mod_Data.serial.Replace("-", string.Empty);  //條碼
                            Mod_Data.ticketExpirationDate = singlesign[4];  //票有效期限
                            string[] ticketOutTime = singlesign[5].Split(' ');  //分割出票年月份、時日分
                            Mod_Data.YYMMDD = ticketOutTime[0]; //出票年月份
                            Mod_Data.HHMMSS = ticketOutTime[1]; //出票時日分
                            Gen2_Data.TicketOutWaitSever = 1;   //出票成功
                        }
                        else
                        {
                            Gen2_Data.TicketOutWaitSever = 0; //出票失敗
                        }
                        break;
                    case "704":  //遊戲紀錄-遊戲紀錄儲存成功與否
                                 // 0:失敗 1:成功
                        if (singlesign[1] == "1")
                        {
                            isSendingGameHisotry = false;   //遊戲紀錄儲存成功
                        }
                        break;
                    case "705":  //機台初始值
                                 //範例:705,2,105,0,021210012,011110011,1,700,NTD,1,1000,2000,3000,4000,10,0.22,1,0,1,171,2,3
                                 //0:機台初始值 1:遊戲編號 2:機台編號 3:預設語言 4:設定的RTP(9位數 0超低 1低 2中 3高 4超高) 5:DENOM設定(9位數 0關閉 1開啟) 6:鈔機開啟(0關閉 1開啟) 7:可入鈔額設定
                                 //8:可入鈔幣別 9:票機開啟(0關閉 1開啟) 10:可入票上限 11:可出票上限 12:贏分上限 13:機台分數上限 14:押注倍數 15:兌分比 16:是否需要會員卡(0關閉 1開啟)
                                 //17:是否吃鈔(0關閉 1開啟) 18:是否吃票(0關閉 1開啟) 19:最後一筆遊戲場次編號 20:最後一筆bonus次數 21:遊戲最大倍數
                        if (singlesign[1] != "0" && singlesign[2] != null)
                        {
                            Mod_Data.machineInit = true; //機台已初始化
                            Mod_Data.gameID = singlesign[1]; //遊戲編號
                            Mod_Data.machineID = singlesign[2]; //機台編號
                            Mod_Data.language = (Mod_Data.languageList)Convert.ToInt32(singlesign[3]); //預設語言
                            RTPSstting = singlesign[4].ToString(); //RTP設定
                            DenomSetting = singlesign[5].ToString(); //DENOM設定
                            Mod_Data.denomsetting = DenomSetting; //DENOM設定
                            BillAcceptorSettingData.BillOpen = singlesign[6] == "1" ? true : false; //鈔機需要開啟或關閉
                            BillAcceptorSettingData.BillEnableWaitServer = singlesign[6] == "1" ? 1 : 0; //開啟或關閉鈔機
                            Mod_Data.Billamountsetting = singlesign[7].ToString(); //可入超額設定
                            Mod_Data.Billcurrency = singlesign[8].ToString(); //幣別設定
                            Gen2_Data.GenOpen = singlesign[9] == "1" ? true : false; //票機需要開啟或關閉
                            Gen2_Data.GenEnableWaitServer = singlesign[9] == "1" ? 1 : 0; //開啟或關閉票機
                            BillAcceptorSettingData.CashOrTicketInMaxLimit = int.Parse(singlesign[10].ToString()); //可入票、鈔上限設定
                            Gen2_Data.TicketOutMaxLimit = int.Parse(singlesign[11].ToString()); //可出票上限設定

                            //機台贏分上限
                            Mod_Data.maxWin = int.Parse(singlesign[12].ToString());
                            BackEnd_Data.maxWin = (int)Mod_Data.maxWin;
                            backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxWin, true); //記錄到記憶體

                            //機台彩分上限
                            Mod_Data.maxCredit = int.Parse(singlesign[13].ToString());
                            BackEnd_Data.maxCredit = (int)Mod_Data.maxCredit;
                            backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxCredit, true); //記錄到記憶體

                            Mod_Data.maxOdds = int.Parse(singlesign[14].ToString()); //機台最大押注
                            Mod_Data.rakeback = float.Parse(singlesign[15].ToString()); //兌分比

                            //是否需要會員卡
                            CardNeed = int.Parse(singlesign[16].ToString());
                            Mod_Data.cardneeded = CardNeed;
                            Mod_Data.memberLcok = Mod_Data.cardneeded == 1 && string.IsNullOrWhiteSpace(Mod_Data.cardID) ? true : false; //會員ID存在是否需要會員鎖

                            billAcceptorController.BillOpenClose = true;
                            //鈔機吸鈔或吸票設定 TicketCashOpenClose需要吸鈔為0，需要吸票為1，兩者都需要為2
                            BillAcceptorSettingData.TicketCashOpenClose = singlesign[17] == "1" ? singlesign[18] == "1" ? 2 : 0 : singlesign[18] == "1" ? 1 : BillAcceptorSettingData.TicketCashOpenClose;
                            Mod_Data.gameIndex = int.Parse(singlesign[19].ToString()); //最後一筆遊戲紀錄
                            Mod_Data.bonusGameIndex = int.Parse(singlesign[20].ToString()); //最後一筆Bonus遊戲紀錄
                            Mod_ChangePonit.CloseMemberPanel = true; //關閉會員兌點UI物件
                        }
                        break;
                    case "706":  //收到入鈔成功與否 0:失敗	1:成功
                        if (singlesign[1] == "1")
                        {
                            //通知鈔機入鈔成功
                            JCM_Bill_Acceptor.cashin_waitServer = false;
                            BillAcceptorSettingData.TicketInWaitSever = 1;
                        }
                        break;
                    case "707":  //收到清鈔成功與否 0:失敗	1:成功
                        if (singlesign[1] == "1") isSendingBillClear = false; //清鈔成功通知協程可以結束
                        break;
                    case "708":  //收到機台事件與否 0:失敗	1:成功
                        if (singlesign[1] == "1") isSendingMachineEvent = false; //收到機台事件通知協程可以結束
                        break;
                    case "709":  //心跳包，收到伺服器訊息確認機台是否存活
                        SendToSever(Mod_Client_Data.messagetype.machinelive); //傳送訊息告知伺服器機台還在運作
                        Mod_Data.serverLock = false; //取消伺服器鎖
                        break;
                    case "710":  //連線 成功: 機台編號	失敗:0
                        if (singlesign[1] != "0" && singlesign[1] != "格式錯誤")
                        {
                            //更新機台編號
                            Mod_Data.machineID = singlesign[1];
                            Mod_Data.ticketNumber = singlesign[1];
                            Mod_Data.machineNumbear = singlesign[1];
                            Mod_Data.machineIDLock = false; //解除機台編號鎖
                            Mod_Data.serverLock = false; //解除伺服器連線鎖
                            isSendMachineEvent = true; //傳送機台事件更新機台狀態
                        }
                        break;
                    case "711":  //手付通知-通知機台伺服器已收到手付訊息 0:失敗  1:成功
                        if (singlesign[1] == "1") isWaitServerHandPay = false; //伺服器收到手付
                        break;
                    case "712":  //伺服器通知機台工作站手付處理完成可以reset
                        isWaitStationHandPaid = false; //工作站完成手付
                        break;
                    case "713":  //通知機台被伺服器斷線
                        Mod_Client_Data.serverdisconnect = true; //與伺服器失去現
                        Mod_Data.machineIDLock = true; //開啟機台編號鎖
                        Mod_Data.serverLock = true; //開啟伺服器連線鎖
                        break;
                    case "715":  //會員登出
                        if (singlesign[1] == "1" || singlesign[1] == "0")
                        {
                            if (Mod_Data.cardneeded == 1 && !Mod_Client_Data.MemberCardOut) Mod_Data.memberLcok = true; //開啟會員鎖
                            //清空會員資料
                            Mod_Client_Data.cardID = " ";
                            Mod_Data.cardID = " ";
                            Mod_Data.memberID = " ";
                            Mod_Data.cardPassword = " ";
                            Mod_Data.memberlevel = " ";
                            Mod_Data.bonusPoints = 0;
                            mod_UIController.text_BonusPoints.text = null; //清空UI文字
                            isMemberLoggingOut = false; //取消正在登出
                            isClearHandPayMemberID = true; //清空手付用會員ID
                        }
                        break;
                    case "716":  //兌分
                                 //範例:716,1,1000
                                 //0:兌分 1:0失敗 1 成功 2:會員剩餘點數
                        if (singlesign[1] == "1")
                        {
                            mod_OpenClearPoint.OpenPointFunction(Mathf.FloorToInt(Mod_Data.changePoint)); //開分
                            Mod_Data.changePointLock = false; //取消會員兌分鎖
                            Mod_Data.changePoint = 0; //開完分將兌分歸零
                            Mod_Data.memberRakebackPoint = float.Parse(singlesign[2].ToString()); //會員剩餘點數
                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore"); //更新彩分UI
                        }
                        else
                        {
                            Mod_Data.changePointLock = false; //取消會員兌分鎖
                            Mod_Data.changePoint = 0; //兌分歸零
                        }
                        break;
                    case "717":  //出票成功
                                 //範例:717,1
                                 //0:兌分 1:0失敗 1 成功
                        if (singlesign[1] == "1") Gen2_Data.TicketOutWaitSever = 1; //伺服器收到出票成功訊息，票機繼續執行後續動作
                        break;
                    case "721":  //將票卷改為無效票
                                 //範例:721,1
                                 //0:兌分 1:0失敗 1 成功
                        if (singlesign[1] == "1") BillAcceptorSettingData.TicketInWaitSever = 1; //伺服器收到將票卷改為無效票訊息，鈔機繼續執行後續動作
                        //Debug.Log("721: " + singlesign[1]);
                        break;
                    case "802":  //收到開分指令
                                 //範例:802,1000,2
                                 //0:開分 1:金額 2:機台編號
                                 //機台編號和開分指令的機台編號相同才能開分
                        if (singlesign[2] == Mod_Data.machineID && int.Parse(singlesign[1]) != 0)
                        {
                            openClearScore = int.Parse(singlesign[1]); //開分金額
                            isOpenClear = true; //開分
                        }
                        break;
                    case "804": //通知機台修改設定值
                                //範例:804,2,105,0,021210012,011110011,1,700,NTD,1,1000,2000,3000,4000,10,0.22,1,0,1
                                //0:機台初始值 1:遊戲編號 2:機台編號 3:預設語言 4:設定的RTP(9位數 0超低 1低 2中 3高 4超高) 5:DENOM設定(9位數 0關閉 1開啟) 6:鈔機開啟(0關閉 1開啟) 7:可入鈔額設定
                                //8:可入鈔幣別 9:票機開啟(0關閉 1開啟) 10:可入票上限 11:可出票上限 12:贏分上限 13:機台分數上限 14:押注倍數 15:兌分比 16:是否需要會員卡(0關閉 1開啟)
                                //17:是否吃鈔(0關閉 1開啟) 18:是否吃票(0關閉 1開啟)
                        if (singlesign[1] != "0" && singlesign[2] != null)
                        {
                            Mod_Data.machineInit = true; //機台已初始化
                            Mod_Data.gameID = singlesign[1]; //遊戲編號
                            Mod_Data.machineID = singlesign[2]; //機台編號
                            Mod_Data.language = (Mod_Data.languageList)Convert.ToInt32(singlesign[3]); //預設語言
                            RTPSstting = singlesign[4].ToString(); //RTP設定
                            DenomSetting = singlesign[5].ToString(); //DENOM設定
                            Mod_Data.denomsetting = DenomSetting; //DENOM設定
                            BillAcceptorSettingData.BillOpen = singlesign[6] == "1" ? true : false; //鈔機需要開啟或關閉
                            //判斷鈔機是否已開起決定開再次執行鈔機開關功能
                            if (BillAcceptorSettingData.BillOpen == true ? !BillAcceptorSettingData.BillOpenClose : BillAcceptorSettingData.BillOpenClose) billAcceptorController.BillOpenCloseButton();
                            Mod_Data.Billamountsetting = singlesign[7].ToString(); //可入超額設定
                            Mod_Data.Billcurrency = singlesign[8].ToString(); //幣別設定
                            Gen2_Data.GenOpen = singlesign[9] == "1" ? true : false; //票機需要開啟或關閉
                            //判斷票機是否已開起決定開再次執行票機開關功能
                            if (Gen2_Data.GenOpen == true ? !gen2_Status.keepConvert : gen2_Status.keepConvert) gen2_Status.OpenOrCloserGen2();
                            BillAcceptorSettingData.CashOrTicketInMaxLimit = int.Parse(singlesign[10].ToString()); //可入票、鈔上限設定
                            Gen2_Data.TicketOutMaxLimit = int.Parse(singlesign[11].ToString()); //可出票上限設定

                            //機台贏分上限
                            Mod_Data.maxWin = int.Parse(singlesign[12].ToString());
                            BackEnd_Data.maxWin = (int)Mod_Data.maxWin;
                            backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxWin, true); //記錄最大贏分記憶體

                            //機台彩分上限
                            Mod_Data.maxCredit = int.Parse(singlesign[13].ToString());
                            BackEnd_Data.maxCredit = (int)Mod_Data.maxCredit;
                            backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxCredit, true); //記錄最大彩分記憶體

                            Mod_Data.maxOdds = int.Parse(singlesign[14].ToString()); //機台最大押注
                            Mod_Data.rakeback = float.Parse(singlesign[15].ToString()); //兌分比

                            //是否需要會員卡
                            CardNeed = int.Parse(singlesign[16].ToString());
                            Mod_Data.cardneeded = CardNeed;//是否需要會員卡
                            Mod_Data.memberLcok = Mod_Data.cardneeded == 1 && string.IsNullOrWhiteSpace(Mod_Data.cardID) ? true : false; //會員ID存在是否需要會員鎖

                            billAcceptorController.BillOpenClose = true;
                            //鈔機吸鈔或吸票設定 TicketCashOpenClose需要吸鈔為0，需要吸票為1，兩者都需要為2
                            BillAcceptorSettingData.TicketCashOpenClose = singlesign[17] == "1" ? singlesign[18] == "1" ? 2 : 0 : singlesign[18] == "1" ? 1 : BillAcceptorSettingData.TicketCashOpenClose;
                            Mod_ChangePonit.CloseMemberPanel = true; //關閉會員兌點UI物件
                            if (Mod_Data.cardneeded == 0 && !string.IsNullOrWhiteSpace(Mod_Data.cardID) && !isMemberLoggingOut && !isMemberLogOut) isMemberLogOut = true;     //不需要會員卡，但機台目前是會員登入狀態，登出會員
                        }
                        break;
                    default:
                        //沒有上面的通訊格式到此處
                        Debug.Log("msg error");
                        break;
                }
            }
        }
        catch (Exception ex) //例外錯誤
        {
            Debug.Log("Exception: " + ex);
        }
    }

    #endregion

    #region 傳送訊息到伺服器

    ///<summary>
    ///此功能根據通訊格式整理出正確訊息，並傳送訊息到伺服器端
    ///</summary>
    public void SendToSever(Mod_Client_Data.messagetype messagetype)
    {
        //如果伺服器連線鎖住下，傳送訊號非連線則返回
        if (Mod_Data.serverLock && messagetype != Mod_Client_Data.messagetype.connect) return;
        UpdateGameData(); //更新客戶端遊戲資料

        try
        {
            //根據通訊格式做出不同訊息的傳送
            switch (messagetype)
            {
                case Mod_Client_Data.messagetype.memberlogin:   //701會員登入
                                                                //範例: 701,TXC-67,X0X0V0Q802b214886a8071a6bf6194b2189dda104d9b424nePFjH&iwk5eZR*@neJl ,1;
                                                                //0:會員登入 1:卡號 2:密碼 3:機台編號
                    SendMessages((int)Mod_Client_Data.messagetype.memberlogin + "," + Mod_Client_Data.cardID
                                                                              + "," + " "
                                                                              + "," + Mod_Data.machineID
                                                                              + ";"
                                                                             );
                    break;
                case Mod_Client_Data.messagetype.ticketin:  //702入票
                                                            //範例: 702,1,4,1000,6602-8500-5117-0481;
                                                            //0:入票 1:(0.無效票、不存在資料庫 1.有效票) 2:金額 3:票號
                    SendMessages((int)Mod_Client_Data.messagetype.ticketin + "," + Mod_Client_Data.ticketNum
                                                                           + "," + Mod_Data.machineID
                                                                           + ";"
                                                                          );
                    break;
                case Mod_Client_Data.messagetype.ticketout: //703出票
                                                            //範例: 703,04,1000,5,1;
                                                            //0:出票 1:票種 2:金額 3:會員編號 4:機台編號
                    SendMessages((int)Mod_Client_Data.messagetype.ticketout + "," + Mod_Client_Data.ticketType
                                                                           + "," + Mod_Client_Data.ticketAmount
                                                                           + "," + Mod_Client_Data.memberID
                                                                           + "," + Mod_Data.machineID
                                                                           + ";"
                                                                           );
                    break;
                case Mod_Client_Data.messagetype.gamehistory: //704遊戲紀錄
                                                              //範例: 704,10,10,1287,10,100,0,1,0.1,11111,0,0,95,10,10,TXC-1,1000,100,10000,31,1000;
                                                              //0:遊戲紀錄 1:場次編號 2:押注 3:會員編號 4:贏分 5:彩分 6:是否bonus 7:遊戲編號 8:押注比例 9:滾軸排面值 10:第幾場BONUS 11:BONUS總場數
                                                              //12:設定的rtp 13:denom 14:特定倍率 15:卡號 16:原始點數 17:修改點數 18:修改後餘點 19:機台編號 20:剩餘贈分點數
                    Mod_Client_Data.bonus = Mod_Data.getBonus == true ? "1" : "0";
                    SendMessages((int)Mod_Client_Data.messagetype.gamehistory + "," + Mod_Client_Data.gameIndex
                                                                            + "," + Mod_Client_Data.bet
                                                                            + "," + Mod_Client_Data.memberID
                                                                            + "," + Mod_Client_Data.winscroe
                                                                            + "," + Mod_Client_Data.credit
                                                                            + "," + Mod_Client_Data.bonus
                                                                            + "," + Mod_Client_Data.gameID
                                                                            + "," + Mod_Client_Data.betOdds
                                                                            + "," + Mod_Client_Data.gameRNG
                                                                            + "," + Mod_Client_Data.bonusCurrent
                                                                            + "," + Mod_Client_Data.bonusTotalgmae
                                                                            + "," + Mod_Client_Data.rtpset
                                                                            + "," + Mod_Client_Data.denom
                                                                            + "," + Mod_Client_Data.specialrules
                                                                             + "," + Mod_Client_Data.cardID
                                                                             + "," + Mod_Client_Data.memberRakebackPointOri
                                                                             + "," + Mod_Client_Data.getRakebackPoint
                                                                             + "," + Mod_Client_Data.memberRakebackPointAfter
                                                                             + "," + Mod_Data.machineID
                                                                             + "," + Mod_Client_Data.bonusPoints
                                                                             + ";"
                                                                            );
                    break;
                case Mod_Client_Data.messagetype.gameset:   //705機台初始值
                                                            //範例: 705,105;
                                                            //0:機台初始值 1:機台編號
                    SendMessages((int)Mod_Client_Data.messagetype.gameset + "," + Mod_Data.machineID
                                                                          + ";"
                                                                          );
                    break;
                case Mod_Client_Data.messagetype.billin:    //706入鈔
                                                            //範例: 706,6000,1;
                                                            //0:機台初始值 1:金額 1:機台編號
                    SendMessages((int)Mod_Client_Data.messagetype.billin + "," + Mod_Client_Data.billAmount
                                                                         + "," + Mod_Data.machineID
                                                                         + ";"
                                                                         );
                    break;
                case Mod_Client_Data.messagetype.billclear: //707清鈔
                                                            //範例: 707,B1;
                                                            //0:機台初始值 1:機台編號
                    SendMessages((int)Mod_Client_Data.messagetype.billclear + "," + Mod_Data.machineID
                                                                            + ";"
                                                                            );
                    break;
                case Mod_Client_Data.messagetype.machineevent:  //708機台事件
                                                                //範例: 708,3,0,1,1;
                                                                //0:機台事件 1:會員編號 2:機台狀態 3:機台編號 4:會員狀態
                    SendMessages((int)Mod_Client_Data.messagetype.machineevent + "," + Mod_Client_Data.memberID
                                                                               + "," + Mod_Client_Data.machineerror
                                                                               + "," + Mod_Data.machineID
                                                                               + "," + Mod_Data.memberlevel
                                                                               + ";"
                                                                               );
                    break;
                case Mod_Client_Data.messagetype.machinelive:   //709心跳包
                                                                //範例: 709;
                                                                //0:心跳包
                    SendMessages((int)Mod_Client_Data.messagetype.machinelive + ""
                                                                              + ";"
                                                                              );

                    break;
                case Mod_Client_Data.messagetype.connect:   //710連線
                                                            //範例: 710;
                                                            //0:連線
                    SendMessages((int)Mod_Client_Data.messagetype.connect + ""
                                                                          + ";"
                                                                          );
                    break;
                case Mod_Client_Data.messagetype.handpay:   //711手付
                                                            //範例: 711,3,1000,4,0000-1430-6035-4757,1;
                                                            //0:手付 1:會員編號 2:金額 3:機台編號 4:票號 5:手付型態(1,2)
                                                            //newSramManager.LoadHandPayStatus() == 1出票手付,newSramManager.LoadHandPayStatus() == (2 || 3)入鈔、票手付
                    SendMessages((int)Mod_Client_Data.messagetype.handpay + "," + Mod_Client_Data.handPaymemberID
                                                                          + "," + (newSramManager.LoadHandPayStatus() == 1 ? Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty)) : String.IsNullOrWhiteSpace(Mod_Client_Data.ticketInAmount) ? "0" : Mod_Client_Data.ticketInAmount)
                                                                          + "," + Mod_Data.machineID
                                                                          + "," + (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3 ? newSramManager.LoadTicketSerial() : String.IsNullOrWhiteSpace(Mod_Client_Data.ticketNum) ? " " : Mod_Client_Data.ticketNum)
                                                                          + "," + (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3 ? "1" : "0")
                                                                          + ";"
                                                                          );
                    break;
                case Mod_Client_Data.messagetype.handpayreset:  //712手付處理完成
                                                                //範例: 712;
                                                                //0:手付處理完成
                    SendMessages((int)Mod_Client_Data.messagetype.handpayreset + ""
                                                                               + ";"
                                                                               );
                    break;
                case Mod_Client_Data.messagetype.memberlogout:  //715會員登出
                                                                //範例: 715,TXC-21,3
                                                                //0:會員登出 1:卡號 2:機台編號
                    SendMessages((int)Mod_Client_Data.messagetype.memberlogout + "," + Mod_Client_Data.cardID
                                                                               + "," + Mod_Data.machineID
                                                                               + ";"
                                                                               );
                    if (Mod_Data.cardneeded == 1 && !Mod_Client_Data.MemberCardOut) Mod_Data.memberLcok = true; //開啟會員鎖
                    break;
                case Mod_Client_Data.messagetype.exchangePoints:    //716兌分
                                                                    //範例: 716,3,TXC-6,2000,1000,1000,X0X0V0Q802b214886a8071a6bf6194b2189dda104d9b424nePFjH&iwk5eZR*@neJl,1
                                                                    //0:兌分 1:卡號 2:兌分原始點數 3:兌分變更點數 4:兌分後剩餘點數 5:密碼 6:機台編號
                    SendMessages((int)Mod_Client_Data.messagetype.exchangePoints + "," + Mod_Client_Data.memberID
                                                                                 + "," + Mod_Client_Data.cardID
                                                                                 + "," + Mod_Data.memberRakebackPoint
                                                                                 + "," + Mod_Data.changePoint
                                                                                 + "," + (Mod_Data.memberRakebackPoint - (float)Mod_Data.changePoint)
                                                                                 + "," + Mod_Data.cardPassword
                                                                                 + "," + Mod_Data.machineID
                                                                                 + ";"
                                                                          );
                    break;
                case Mod_Client_Data.messagetype.ticketoutsuccess:  //717出票成功
                                                                    //範例: 721,6602-8500-5117-0481,1
                                                                    //0:出票成功 1:票號 2:機台編號
                    SendMessages((int)Mod_Client_Data.messagetype.ticketoutsuccess + "," + Mod_Client_Data.ticketNum.Substring(3) //04-6602-8500-5117-0481 -> 6602-8500-5117-0481
                                                                                   + "," + Mod_Data.machineID
                                                                                   + "," + Mod_Client_Data.ticketAmount
                                                                                   + ";"
                                                                          );
                    break;
                case Mod_Client_Data.messagetype.ticketChangeToInvalid: //721將票券改為無效票
                                                                        //範例: 717,7174-6261-1745-2822,1,1000
                                                                        //0:出票成功 1:票號 2:機台編號 3:金額
                    SendMessages((int)Mod_Client_Data.messagetype.ticketChangeToInvalid + "," + Mod_Client_Data.ticketNum
                                                                                        + "," + Mod_Data.machineID
                                                                                        + ";"
                                                                          );
                    break;
                default:
                    Debug.Log("cant get funtion");
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("SendToSever Exception: " + ex);
        }
    }

    ///<summary>
    ///此功能根據通訊格式(帶數字參數)整理出正確訊息，並傳送訊息到伺服器端
    ///</summary>
    public void SendToSever(Mod_Client_Data.messagetype messagetype, int value)
    {
        //如果伺服器連線鎖住下，傳送訊號非連線則返回
        if (Mod_Data.serverLock && messagetype != Mod_Client_Data.messagetype.connect) return;
        UpdateGameData(); //更新客戶端遊戲資料
        try
        {
            //根據通訊格式做出不同訊息的傳送
            switch (messagetype)
            {

                case Mod_Client_Data.messagetype.pointIn:   //722機台開分
                                                            //範例: 722,1,3,1000
                                                            //0:機台開分 1:機台編號 2:會員編號 3:金額
                    SendMessages((int)Mod_Client_Data.messagetype.pointIn + "," + Mod_Data.machineID
                                                                          + "," + Mod_Client_Data.memberID
                                                                          + "," + value
                                                                          );
                    break;
                case Mod_Client_Data.messagetype.pointOut:  //723機台洗分
                                                            //範例: 723,1,3,1000
                                                            //0:機台洗分 1:機台編號 2:會員編號 3:金額
                    SendMessages((int)Mod_Client_Data.messagetype.pointOut + "," + Mod_Data.machineID
                                                                           + "," + Mod_Client_Data.memberID
                                                                           + "," + value
                                                                           );
                    break;
                default:
                    //沒有以上通訊格式到此處
                    Debug.Log("cant get funtion");
                    break;
            }
        }
        catch (Exception ex) //例外錯誤
        {
            Debug.Log("SendToSever Exception: " + ex);
        }
    }

    #endregion

    #region 會員登入檢查

    public void MemberLogInCheck(string inputStr)
    {
        string cardstore = "";
        string cardnum = "";
        string cardcrynum = "";
        string S2 = "";
        string S3 = "";

        //正常插卡字串:%TXC-3236? Card Seated
        //正常拔卡字串:Media DetectedCard Removed
        //讀不到資料:No Data Card Seated
        if (inputStr.Contains("Card Seated"))
        {
            Debug.Log(inputStr);
            try
            {
                //ex:1.%TXC-3236? Card Seated, ex:2.%TXC-3236? Card Seated Test
                S3 = inputStr.Substring(inputStr.IndexOf("?") + 1); //-> ex1: Card Seated, ex2: Card Seated Test
                S3 = S3.Substring(S3.IndexOf("C")); //-> ex1:Card Seated, ex2:Card Seated Test
                if (!string.Equals(S3, "Card Seated")) //ex1:Card Seated相等"Card Seated"繼續執行, ex2:Card Seated Test不相等"Card Seated"返回
                {
                    Debug.Log(S3);
                    Debug.Log("Not Equals!");
                    return;
                }

                //ex:%TXC-3236? Card Seated
                S2 = inputStr.Substring(inputStr.IndexOf("%") + 1, inputStr.IndexOf("?") - (inputStr.IndexOf("%") + 1));//-> ex:TXC-3236
                if (S2.Contains("-"))
                {
                    cardstore = S2.Substring(0, S2.IndexOf("-") + 1);//-> ex:TXC
                    cardnum = S2.Substring(S2.IndexOf("-") + 1);//-> ex:3236
                }

                if (((cardnum.Length % 2) == 0) && (cardnum[0].ToString() == "3")) //擷取會員數字，cardnum餘數運算完數字為0，且開頭為3 ex:3236
                {
                    for (int iac = 1; iac <= cardnum.Length; iac++) //依照長度做迴圈
                    {
                        if (iac % 2 == 1) //當iac餘數運算完為1，抓取cardnum此長度字元 ex:3236 iac=1,2,3 (1 % 2== 1(cardnum[1]= '2'),3 % 2== 1(cardnum[3]= '6')) -> cardcrynum=26
                        {
                            cardcrynum += cardnum[iac];
                        }
                    }
                }

                //會員登入，與伺服器沒有失去連、機台以初始化、機台需要會員卡、機台沒有錯誤、機台沒有傳送機台事件
                if (!Mod_Client_Data.serverdisconnect && Mod_Data.machineInit && Mod_Data.cardneeded == 1 && !Mod_Data.MachineError && !isSendingMachineEvent)
                {
                    //在BaseSpin狀態、非正在登出或登出，登入會員
                    if (Mod_Data.inBaseSpin && !isMemberLoggingOut && !isMemberLogOut)
                    {
                        Mod_Client_Data.MemberCardOut = false; //取消會員卡拔出狀態
                        isMemberLogIn = true; //會員登入
                        Mod_Data.cardID = cardstore + cardcrynum; //賦予cardID值
                        SendToSever(Mod_Client_Data.messagetype.memberlogin); //傳送登入訊息到伺服器
                    }
                    else if (!Mod_Data.inBaseSpin) //非BaseSpin狀態登入
                    {
                        isMemberLogInHard = true; //開啟強制登入
                        Mod_Client_Data.MemberCardOut = false; //取消會員卡拔出狀態
                        isMemberLogOut = false; //取消會員登出
                        isMemberLogIn = true; //會員登入
                        Mod_Data.cardID = cardstore + cardcrynum; //賦予cardID值
                        SendToSever(Mod_Client_Data.messagetype.memberlogin); //傳送登入訊息到伺服器
                    }
                }
            }
            catch (Exception ex) //例外錯誤
            {
                Debug.Log("Exception: " + "ID Card Length Wrong");
            }
        }

        //字串符合"Media Detected" ex:Media Detected Card Removed
        if (inputStr.Contains("Media Detected"))
        {
            Debug.Log(inputStr);
            inputStr = inputStr.Replace("Media Detected", ""); //-> ex: Card Removed
        }

        //會員登出，字串符合"Card Removed" ex: Card Removed
        if (inputStr.Contains("Card Removed"))
        {
            Debug.Log(inputStr);
            //cardID不為空白、空值、非正在登出或登出，會員登出
            if (!string.IsNullOrWhiteSpace(Mod_Data.cardID) && !isMemberLoggingOut && !isMemberLogOut)
            {
                if (Mod_Data.cardneeded == 1 && !Mod_Data.inBaseSpin) Mod_Client_Data.MemberCardOut = true; //需要會員卡且不是在BaseSpin狀態，開啟會員卡拔除狀態(Bonus用，滾完Bonus直到BaseSpin才登出)
                else Mod_Data.memberLcok = true; //直接開啟會員鎖
                isMemberLogOut = true; //會員登出
            }
            Mod_ChangePonit.CloseMemberPanel = true; //關閉會員換點UI
        }

        if (inputStr.Contains("No Data"))
        {
            Debug.Log(inputStr);
            if (!Mod_Client_Data.serverdisconnect && !string.IsNullOrWhiteSpace(Mod_Data.cardID) && Mod_Data.cardneeded == 1 && !Mod_Data.MachineError && !isSendingMachineEvent && !isMemberLoggingOut && !isMemberLogOut && !isMemberLogIn) Mod_Data.cardID = " ";
        }
    }

    #endregion

    #region 協程處理

    #region 協程處理-傳送機台錯誤到伺服器

    /// <summary>
    /// 協程處理-機台錯誤依照嚴重等級排序，並傳送正確等級的機台事件到伺服器
    /// </summary>
    IEnumerator SendMachineError()
    {
        isSendingMachineError = true;

        //等待伺服器非斷線、機台已初始化
        yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);

        //找到為True的錯誤變數
        int findTrue = 0;
        //找不到錯誤變數
        int notFoundCount = 0;

        //重置已傳送過錯誤的布林
        for (int i = 0; i < sendMachineError.Length; i++)
        {
            sendedSendMachineError[i] = false;
        }

        //直到機台沒有錯誤才結束迴圈
        while (true)
        {
            //從零開始找到為True的錯誤並且沒有傳送過給伺服器
            if (sendMachineError[findTrue] && !sendedSendMachineError[findTrue])
            {
                //找到錯誤將找不到錯誤變數歸零重新計算
                notFoundCount = 0;

                //將找到的錯誤設為已傳送過，避免重複傳送
                sendedSendMachineError[findTrue] = true;

                //根據找到的錯誤設定傳送的值，0為正常
                switch (findTrue)
                {
                    case 0: Mod_Data.machineerror = "5"; break; //手付 handpay
                    case 1: Mod_Data.machineerror = "1"; break; //清鈔 billclear
                    case 2: Mod_Data.machineerror = "6"; break; //票機錯誤 printererror
                    case 3: Mod_Data.machineerror = "9"; break; //鈔機錯誤 billerror
                    case 4: Mod_Data.machineerror = "3"; break; //機門錯誤 doorerror
                    case 5: Mod_Data.machineerror = "8"; break; //票紙過少 paperLow
                    case 6: Mod_Data.machineerror = "4"; break; //機台錯誤 machineerror
                    case 7: Mod_Data.machineerror = "2"; break; //服務燈 servicelight
                }

                //等待(非機台錯誤)傳送機台事件結束
                yield return new WaitUntil(() => isSendingMachineEvent == false);
                //傳送機台事件
                isSendMachineEvent = true;
                //再次等待(機台錯誤)傳送機台事件結束
                yield return new WaitUntil(() => isSendingMachineEvent == false);

                //如果機台事件為1(清鈔)則傳送清鈔訊息給伺服器
                if (Mod_Data.machineerror == "1" && !isSendingBillClear) isSendBillClear = true;
                //等待伺服器回傳訊息，並結束清鈔
                yield return new WaitUntil(() => isSendingBillClear == false && isSendBillClear == false);

                int i = 0;
                //找到比前一次錯誤等級排序還高的或是此次錯誤狀態已解決則跳出迴圈
                while (true)
                {
                    //比前一次錯誤等級排序還高或是錯誤已解決跳出迴圈
                    if ((sendMachineError[i] && i < findTrue) || !sendMachineError[findTrue]) break;
                    i++;
                    //超出錯誤長度從零開始
                    if (i > sendMachineError.Length - 1) i = 0;
                    yield return null;
                }

                //重置所有已傳送錯誤狀態
                for (int p = 0; p < sendMachineError.Length; p++)
                {
                    if (!sendMachineError[findTrue]) sendedSendMachineError[p] = false;
                }

                //將錯誤值歸零重找
                findTrue = 0;

                //等待一段時間，繼續執行
                yield return SendMachineErrorLongTime;
            }

            findTrue++;
            //錯誤值超過錯誤長度歸零，如果已經超過重置次數直接結束協程
            if (findTrue > sendMachineError.Length - 1)
            {
                findTrue = 0;
                //找不到錯誤超過次數結束協程
                if (notFoundCount >= 3) yield break;
                notFoundCount++;
            }
            yield return null;
        }
    }

    #endregion

    #region 協程處理-傳送機台事件到伺服器

    /// <summary>
    /// 協程處理-傳送機台事件到伺服器
    /// </summary>
    IEnumerator SendMachineEvent_IE()
    {
        isSendingMachineEvent = true;

        //等待直到伺服器沒有斷線並且機台初始化
        yield return WaitMachineEvent_Until;
        //透過此功能整理機台事件資料並傳送到伺服器
        SendToSever(Mod_Client_Data.messagetype.machineevent);

        //傳送機台事件訊息到伺服器，直到伺服器回傳訊息才結束迴圈
        while (true)
        {
            //伺服器回傳訊息，跳出迴圈
            if (!isSendingMachineEvent) break;
            //如果再未收到伺服器回傳的訊息與伺服器失去連線，將再重新傳送機台事件訊息
            if (Mod_Client_Data.serverdisconnect)
            {
                //等待直到伺服器沒有斷線並且機台初始化
                yield return WaitMachineEvent_Until;
                //等待一段時間
                yield return WaitMachineEventLongTime;
                //透過此功能整理機台事件資料並傳送到伺服器
                SendToSever(Mod_Client_Data.messagetype.machineevent);
            }
            //等待一段時間
            yield return WaitMachineEventSortTime;
        }
    }

    #endregion

    #region 協程處理-傳送清鈔訊息到伺服器

    /// <summary>
    /// 協程處理-傳送清鈔訊息到伺服器
    /// </summary>
    IEnumerator SendBillClear_IE()
    {
        isSendingBillClear = true;
        //等待直到伺服器沒有斷線並且機台初始化
        yield return WaitBillClear_Until;
        //透過此功能整理清鈔資料並傳送到伺服器
        SendToSever(Mod_Client_Data.messagetype.billclear);

        //傳送清鈔訊息到伺服器，直到伺服器回傳訊息才結束迴圈
        while (true)
        {
            //伺服器回傳訊息，跳出迴圈
            if (!isSendingBillClear) break;
            //如果再未收到伺服器回傳的訊息與伺服器失去連線，將再重新傳送機台事件訊息
            if (Mod_Client_Data.serverdisconnect)
            {
                //等待直到伺服器沒有斷線並且機台初始化
                yield return WaitBillClear_Until;
                //等待一段時間
                yield return WaitBillClearLongTime;
                //透過此功能整理清鈔資料並傳送到伺服器
                SendToSever(Mod_Client_Data.messagetype.billclear);
            }
            //等待一段時間
            yield return WaitBillClearShortTime;
        }
    }

    #endregion

    #region 協程處理-傳送會員登出訊息到伺服器

    /// <summary>
    /// 協程處理-傳送會員登出訊息到伺服器
    /// </summary>
    IEnumerator isMemberLogOut_IE()
    {
        isMemberLoggingOut = true;
        //會員登出協程等待直到(伺服器沒有斷線並且機台初始化)、(非會員登入、非會員卡拔出或是斷電強制會員登入)、(非手付狀態或是會員ID為空白、空值)
        yield return WaitMemberLogOut_Until;

        //如果例外機台錯誤等造成會員登出，需強制登入或是卡片ID為空白、空值，取消會員登出並傳送機台事件更新機台狀態並結束協程
        if (isMemberLogInHard || string.IsNullOrWhiteSpace(Mod_Data.cardID))
        {
            //取消會員登出
            isMemberLoggingOut = false;
            //等待傳送其他機台事件結束
            yield return new WaitUntil(() => isSendingMachineEvent == false);
            //傳送現在機台事件(更新會員登入、登出狀態)
            isSendMachineEvent = true;
            yield break;
        }

        //登出中途不需要會員卡將部分資料清除
        if (Mod_Data.cardneeded == 0)
        {
            Mod_Data.memberID = " ";
            Mod_Client_Data.memberID = " ";
            Mod_Client_Data.cardID = " ";
            Mod_Client_Data.memberlevel = " ";
            Mod_Client_Data.memberRakebackPointOri = 0;
            Mod_Client_Data.getRakebackPoint = 0;
            Mod_Client_Data.memberRakebackPointAfter = 0;
        }

        //透過此功能整理會員登出資料並傳送到伺服器
        SendToSever(Mod_Client_Data.messagetype.memberlogout);

        //傳送會員登出訊息到伺服器，直到伺服器回傳訊息或是卡片ID為空白、空值才結束迴圈
        while (true)
        {
            //伺服器回傳訊息或是卡片ID為空白、空值結束迴圈
            if (!isMemberLoggingOut || string.IsNullOrWhiteSpace(Mod_Data.cardID)) break;
            //如果再未收到伺服器回傳的訊息與伺服器失去連線，將再重新傳送會員登出訊息
            if (Mod_Client_Data.serverdisconnect)
            {
                //等待直到(伺服器沒有斷線並且機台初始化)、(非會員登入、非會員卡拔出或是斷電強制會員登入)、(非手付狀態或是會員ID為空白、空值)
                yield return WaitMemberLogOut_Until;
                //如果例外機台錯誤等造成會員登出，需強制登入或是卡片ID為空白、空值，取消會員登出並傳送機台事件更新機台狀態並結束協程
                if (isMemberLogInHard || string.IsNullOrWhiteSpace(Mod_Data.cardID))
                {
                    isMemberLoggingOut = false;
                    yield return new WaitUntil(() => isSendingMachineEvent == false);
                    isSendMachineEvent = true;
                    yield break;
                }
                //等待一段時間
                yield return WaitMemberLogOutLongTime;
                //登出中途不需要會員卡將部分資料清除
                if (Mod_Data.cardneeded == 0)
                {
                    Mod_Client_Data.memberID = " ";
                    Mod_Client_Data.cardID = " ";
                    Mod_Client_Data.memberlevel = " ";
                    Mod_Client_Data.memberRakebackPointOri = 0;
                    Mod_Client_Data.getRakebackPoint = 0;
                    Mod_Client_Data.memberRakebackPointAfter = 0;
                }
                //透過此功能整理會員登出資料並傳送到伺服器
                SendToSever(Mod_Client_Data.messagetype.memberlogout);
            }
            //等待一段時間
            yield return WaitMemberLogOutShortTime;
        }

        //等待傳送其他機台事件結束
        yield return new WaitUntil(() => isSendingMachineEvent == false);
        //傳送現在機台事件(更新會員登入狀態)
        isSendMachineEvent = true;
    }

    #endregion

    #region 協程處理-手付處理、傳送手付訊息到伺服器

    /// <summary>
    /// 協程處理-手付狀態傳送給伺服器，並由工作站完成手付，工作站傳送訊息到伺服器，再由伺服器通知機台解除手付狀態
    /// </summary>
    IEnumerator HandPayCoroutine()
    {
        //Sram-HandPayStatus 記憶體儲存手付狀態
        //0為正常
        //1為票機錯誤手付
        //2為鈔機錯誤手付
        //3為鈔、票機錯誤手付

        Mod_Data.handpay = true;
        isHandPaying = true;
        isWaitServerHandPay = true;
        isWaitStationHandPaid = true;

        //開起、顯示手付用物件UI顯示手付資訊
        Mod_TextController.printerTicketOutValueInfo_Obj.SetActive(true);
        if (Mod_TextController.printerTicketOutValueInfo_Image.enabled == false) Mod_TextController.printerTicketOutValueInfo_Image.enabled = true;

        //如果在出票時，錯誤造成手付顯示此字串到UI遊戲畫面
        if (gen2_Status.ticketPirintDone)
        {
            Mod_TextController.printerTicketOutValue_Text.text = "HAND PAY : " + Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty)) + "<color=red>" + "\n" + Mod_Data.serial + "\n" + "此票為無效票" + "</color>";
        }
        //鈔機錯誤造成手付顯示此字串到UI遊戲畫面
        else if (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3)
        {
            //如果有記錄入鈔、票金額顯示金額
            if (string.IsNullOrWhiteSpace(Mod_Client_Data.ticketInAmount)) Mod_TextController.printerTicketOutValue_Text.text = "HAND PAY : " + "<color=red>" + "\n" + newSramManager.LoadTicketSerial() + "\n" + "此票資料有誤" + "</color>";
            else Mod_TextController.printerTicketOutValue_Text.text = "HAND PAY : " + Mod_Client_Data.ticketInAmount + "<color=red>" + "\n" + newSramManager.LoadTicketSerial() + "\n" + "此票資料有誤" + "</color>";
        }
        //其他錯誤造成手付顯示此字串到UI遊戲畫面
        else
        {
            Mod_TextController.printerTicketOutValue_Text.text = "HAND PAY : " + Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty));
        }

        //等待直到與伺服器沒有斷線並且機台已初始化
        yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);

        //如果還未傳過手付錯誤而且機台錯誤協程沒有在執行，執行機台錯誤協程
        if (!sendedSendMachineError[0] && !isSendingMachineError)
        {
            StopCoroutine("SendMachineError");
            StartCoroutine("SendMachineError");
        }

        //等待一段時間，並且已傳送完機台事件結束機台事件協程
        yield return WaitHandPayLongTime;
        yield return WaitHandPayLongTime;
        yield return new WaitUntil(() => isSendingMachineEvent == false);

        //透過此功能整理手付資料並傳送到伺服器
        SendToSever(Mod_Client_Data.messagetype.handpay);

        //傳送手付訊息到伺服器，直到伺服器回傳訊息才跳出迴圈
        while (true)
        {
            //伺服器回傳訊息跳出迴圈
            if (!isWaitServerHandPay) break;
            //如果和伺服器失去連線，等待直到與伺服器沒有斷線並且機台已初始化，並重新傳送手付訊息到伺服器
            if (Mod_Client_Data.serverdisconnect)
            {
                yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
                yield return WaitHandPayLongTime;
                //透過此功能整理手付資料並傳送到伺服器
                SendToSever(Mod_Client_Data.messagetype.handpay);
            }
            yield return WaitHandPayShortTime;
        }

        //等待伺服器通知機台工作站已完成手付
        yield return new WaitUntil(() => isWaitStationHandPaid == false);

        //鈔機手付錯誤或是鈔、票機手付錯誤優先處理鈔機手付錯誤
        if (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3)
        {
            //同時鈔、票手付錯誤，將鈔機手付錯誤處理完後，更改為票機手付錯誤，否則只有鈔機手付錯誤，處理完後更改為正常無手付錯誤狀態
            if (newSramManager.LoadHandPayStatus() == 1 || newSramManager.LoadHandPayStatus() == 3) newSramManager.SaveHandPayStatus(1);
            else newSramManager.SaveHandPayStatus(0);

            //清空記憶體所記錄的票號
            newSramManager.SaveTicketSerial(null);

            //關閉UI物件和清空UI文字
            Mod_TextController.printerTicketOutValueInfo_Image.enabled = false;
            Mod_TextController.printerTicketOutValue_Text.text = null;
            Mod_TextController.billAndTciketInfo_Text.text = null;
        }
        else //出票手付錯誤
        {
            //在帳務紀錄出票金額
            BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.ticketOut, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.ticketOut) + Mod_Data.credit);
            BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.ticketOut_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.ticketOut_Class) + Mod_Data.credit);
            //紀錄出票事件
            newSramManager.saveEventRecoredByEventName(6, (int)(Mod_Data.credit * 100));
            //儲存出票金額
            newSramManager.SaveTicketOutPoint(newSramManager.LoadTicketOutPoint() + Mod_Data.credit);

            //同時鈔、票手付錯誤，將鈔機手付錯誤處理完後，更改為入鈔、票手付錯誤，否則只有出票手付錯誤，處理完後更改為正常無手付錯誤狀態
            if (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3) newSramManager.SaveHandPayStatus(2);
            else newSramManager.SaveHandPayStatus(0);

            //關閉UI物件、清空UI文字、顯示手付處理掉的分數
            Mod_TextController.printerTicketOutValueInfo_Image.enabled = false;
            Mod_TextController.printerTicketOutValue_Text.text = null;
            Mod_TextController.billAndTciketInfo_Text.text = "HAND PAY Done: " + Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty));
            //將贏分和彩分歸零
            Mod_Data.credit = 0;
            Mod_Data.Win = 0;
            //更新UI分數
            mod_UIController.UpdateScore();
        }

        yield return WaitHandPayLongTime;

        //取消手付狀態
        Mod_Data.handpay = false;

        //傳送機台事件協程沒有在執行，將機台錯誤改為正常並執行機台事件協程
        if (!isSendingMachineEvent)
        {
            Mod_Data.machineerror = "0";
            isSendMachineEvent = true;
        }

        //取消正在手付狀態
        isHandPaying = false;
        //如果還未恢復成正常無手付狀態，再次執行手付
        if (newSramManager.LoadHandPayStatus() != 0) Mod_Client.isCallHandPay = true;
    }

    #endregion

    #region 協程處理-傳送遊戲紀錄訊息到伺服器

    /// <summary>
    /// 協程處理-傳送遊戲紀錄訊息到伺服器，伺服器回傳訊息，傳送遊戲紀錄協程結束才能繼續下一輪遊戲
    /// </summary>
    IEnumerator SendGameHistory_IE()
    {
        isSendingGameHisotry = true;
        //等待直到伺服器沒有斷線並且機台初始化
        yield return WaitGameHisotry_Until;
        //透過此功能整理遊戲紀錄資料並傳送到伺服器
        SendToSever(Mod_Client_Data.messagetype.gamehistory);

        //傳送遊戲紀錄訊息到伺服器，直到伺服器回傳訊息才跳出迴圈
        while (true)
        {
            //伺服器回傳訊息才跳出迴圈
            if (!isSendingGameHisotry) break;

            //如果與伺服器失去連線，等待直到伺服器沒有斷線並且機台初始化，取得機台遊戲場次，和伺服器給的遊戲場次比對，如果伺服器給的場次小於機台場次則重新傳送遊戲紀錄，否則結束遊戲紀錄協程
            if (Mod_Client_Data.serverdisconnect)
            {
                //等待直到伺服器沒有斷線並且機台初始化
                yield return WaitGameHisotry_Until;
                //取得機台本地遊戲場次
                GetLocalGameRound();
                yield return WaitSendGameHistoryLongTime;

                //BaseGame用BaseGame場次做比對，Bonus用Bonus場次做比對，只要伺服器紀錄的場次小於機台紀錄的場次重新傳送遊戲紀錄
                if (!Mod_Data.BonusSwitch)
                {
                    if (Mod_Data.gameIndex < Mod_Data.localRound)
                    {
                        //伺服器給的場次小於機台場次，要加一場，傳送遊戲紀錄才不會是伺服器給的場次，則是跟本地機台紀錄的場次一樣
                        Mod_Data.gameIndex++;
                        //透過此功能整理遊戲紀錄資料並傳送到伺服器
                        SendToSever(Mod_Client_Data.messagetype.gamehistory);
                    }
                    else isSendingGameHisotry = false;
                }
                else
                {
                    //伺服器給的Bonus場次小於機台Bonus場次，傳送遊戲紀錄
                    if (Mod_Data.bonusGameIndex < Mod_Data.localBonusRound) SendToSever(Mod_Client_Data.messagetype.gamehistory);
                    else isSendingGameHisotry = false;
                }
            }
            yield return WaitSendGameHistoryShortTime;
        }

        //解除遊戲紀錄鎖
        Mod_Data.severHistoryLock = false;
    }

    //取得本地遊戲紀錄
    public void GetLocalGameRound()
    {
        //取得記憶體本地BaseGame遊戲紀錄
        newSramManager.LoadLocalGameRound(out Mod_Data.localRound);
        //取得記憶體本地BonusGame遊戲紀錄
        newSramManager.LoadLocalBonusGameRound(out Mod_Data.localBonusRound);
    }

    //儲存本地遊戲紀錄
    public void SaveLocalGameRound()
    {
        //儲存記憶體本地BaseGame遊戲紀錄
        newSramManager.SaveLocalGameRound(Mod_Data.gameIndex);
        //儲存記憶體本地BonusGame遊戲紀錄
        newSramManager.SaveLocalBonusGameRound(Mod_Data.bonusGameIndex);
    }

    #endregion

    #endregion

    #region 更新傳送給伺服器的資料

    void UpdateGameData()
    {
        //遊戲相關
        Mod_Client_Data.gameIndex = Mod_Data.gameIndex;
        Mod_Client_Data.bet = (Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom).ToString();                  //押注  
        if (!Mod_Data.BonusSwitch) Mod_Client_Data.winscroe = (Mod_Data.Pay * Mod_Data.Denom).ToString();          //贏分
        else Mod_Client_Data.winscroe = (Mod_Data.Pay * Mod_Data.Denom * Mod_Data.CH_BonusSpecialTime).ToString();          //贏分
        if (!Mod_Data.BonusSwitch) Mod_Client_Data.credit = Mod_Data.credit.ToString();                     //彩分
        else Mod_Client_Data.credit = (Mod_Data.credit + (Mod_Data.Win - Mod_Data.Pay) * Mod_Data.CH_BonusSpecialTime).ToString();
        Mod_Client_Data.bonusPoints = Mod_Data.bonusPoints.ToString(); //贈分
        Mod_Client_Data.bonus = Mod_Data.BonusSwitch.ToString();                     //遊戲是否bonus
        Mod_Client_Data.gameID = Mod_Data.gameID;                  //遊戲編號
        Mod_Client_Data.betOdds = Mod_Data.odds.ToString();                    //押注比例 
        Mod_Client_Data.gameRNG = Mod_Data.RNG[0].ToString() + "." + Mod_Data.RNG[1].ToString() + "." + Mod_Data.RNG[2].ToString() + "." + Mod_Data.RNG[3].ToString() + "." + Mod_Data.RNG[4].ToString();                //遊戲滾輪值
        Mod_Client_Data.bonusCurrent = Mod_Data.BonusIsPlayedCount.ToString();            //當前bonus場次(第幾場)
        Mod_Client_Data.bonusTotalgmae = Mod_Data.BonusCount.ToString();          //Bonus遊戲總次數
        Mod_Client_Data.rtpset = Mod_Data.RTPsetting.ToString();                  //設置的rtp
        Mod_Client_Data.denom = Mod_Data.Denom.ToString();                      //比率
        Mod_Client_Data.specialrules = Mod_Data.BonusSpecialTimes.ToString();               //特定倍率
                                                                                            //會員相關
                                                                                            // Mod_Client_Data.cardPassword = Mod_Data.cardPassword;               //卡片密碼
                                                                                            //票相關
        Mod_Client_Data.ticketNum = Mod_Data.serial;                  //票號
        Mod_Client_Data.ticketType = "04";          //票種  01:正式票 02:試玩票
        Mod_Client_Data.ticketAmount = Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty));              //票額  
        Mod_Client_Data.ticketValidate = "2000/01/01";             //票有效期限 
                                                                   //鈔相關
        Mod_Client_Data.billAmount = Mod_Data.cash.ToString();                 //鈔票面額
        Mod_Client_Data.machineerror = Mod_Data.machineerror;

        if (Mod_Data.cardneeded == 1)
        {
            Mod_Client_Data.memberID = Mod_Data.memberID;                   //會員編號
            Mod_Client_Data.cardID = Mod_Data.cardID;                     //卡片編號
            Mod_Client_Data.memberlevel = Mod_Data.memberlevel;
            Mod_Client_Data.memberRakebackPointOri = (Mod_Data.memberRakebackPoint - Mod_Data.getRakebackPoint) > 0 ? Mod_Data.memberRakebackPoint - Mod_Data.getRakebackPoint : 0;
            Mod_Client_Data.getRakebackPoint = Mod_Data.getRakebackPoint;
            Mod_Client_Data.memberRakebackPointAfter = Mod_Data.memberRakebackPoint;
        }
    }

    #endregion

    #region 遊戲關閉客戶端處理

    //應用程式關閉
    private void OnApplicationQuit()
    {
        close();
    }

    //物件遭到銷毀、刪除
    private void OnDestroy()
    {
        close();
    }

    //關閉執行緒、客戶端與伺服器的連接
    void close()
    {
        //如果執行續物件存在
        if (severThread != null)
        {
            severThread.Interrupt(); //停止執行緒動作
            severThread.Abort(); //強制終止執行緒
        }

        socketClient.Close(); //將客戶端關閉
        // try
        // {
        //     socketClient.Shutdown(SocketShutdown.Both); //嘗試關閉與伺服器的連接
        // }
        // finally
        // {
        //     socketClient.Close(); //將客戶端關閉
        // }
    }

    #endregion

    #region 需要卡片變數

    int _cardNeed;
    ///<summary>
    ///此變數只有在只有在收到0或1才會變更數值
    ///</summary>
    int CardNeed
    {
        get { return _cardNeed; }
        set
        {
            if (_cardNeed == 1 || _cardNeed == 0)
            {
                _cardNeed = value;
            }
            else
            {
                _cardNeed = Mod_Data.cardneeded;
            }
        }
    }

    #endregion

    #region RTP變數

    string _RTPsetting;
    ///<Summary>
    ///改變RTP
    ///</Summary>
    string RTPSstting
    {
        get { return _RTPsetting; }
        set
        {
            _RTPsetting = value;
            Debug.Log(_RTPsetting);
            bool dataError = false;

            //RTP有九個Demon故長度為九，2.5, 1, 0.5, 0.25, 0.1, 0.05, 0.025, 0.02, 0.01
            //依照字串長度做迴圈並依字元做判斷，0:超低 1:低 2:中 3:高 4:超高
            //ex:111123321. 低,低,低,低,中,高,高,中,低.
            if (_RTPsetting.Length == 9)
            {
                for (int i = 0; i < 9; i++)
                {
                    switch (_RTPsetting[i])
                    {
                        case '0':
                            BackEnd_Data.RTPwinRate[i] = 0;
                            break;
                        case '1':
                            BackEnd_Data.RTPwinRate[i] = 1;
                            break;
                        case '2':
                            BackEnd_Data.RTPwinRate[i] = 2;
                            break;
                        case '3':
                            BackEnd_Data.RTPwinRate[i] = 3;
                            break;
                        case '4':
                            BackEnd_Data.RTPwinRate[i] = 4;
                            break;
                        default:
                            //有任何一個不符合上面條件則錯誤
                            dataError = true;
                            break;
                    }
                }

                //如果資料無誤將rtp設定改變並記錄到記憶體
                if (!dataError)
                {
                    backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, true);

                    if (Mod_Data.Denom == 0.01) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[8]; }
                    else if (Mod_Data.Denom == 0.02) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[7]; }
                    else if (Mod_Data.Denom == 0.025) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[6]; }
                    else if (Mod_Data.Denom == 0.05) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[5]; }
                    else if (Mod_Data.Denom == 0.1) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[4]; }
                    else if (Mod_Data.Denom == 0.25) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[3]; }
                    else if (Mod_Data.Denom == 0.5) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[2]; }
                    else if (Mod_Data.Denom == 1) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[1]; }
                    else if (Mod_Data.Denom == 2.5) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[0]; }
                }
            }
        }
    }

    #endregion

    #region 押注倍率變數

    string _denomSetting = "";
    ///<Summary>
    ///改變押注倍率設定
    ///</Summary>
    string DenomSetting
    {
        get { return _denomSetting; }
        set
        {
            _denomSetting = value;
            Debug.Log(_denomSetting.Length);
            //預設Demon為九個，2.5, 1, 0.5, 0.25, 0.1, 0.05, 0.025, 0.02, 0.01
            //依照字串長度做迴圈並依字元做判斷，0為關閉、1為開啟
            //ex:111100000. 機台將會開啟2.5, 1, 0.5, 0.25倍率
            if (_denomSetting.Length == 9)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (_denomSetting[i] == '0')
                    {
                        Mod_Data.denomOpenArray[i] = false;
                        BackEnd_Data.denomArray[i] = false;
                    }
                    else if (_denomSetting[i] == '1')
                    {
                        Mod_Data.denomOpenArray[i] = true;
                        BackEnd_Data.denomArray[i] = true;
                    }
                }
                //押注倍率設定記錄到記憶體
                backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.denomArraySelect, true);
            }
        }
    }

    #endregion
    
    #endregion
#endif
}

#endregion

#region 客戶端資料變數

public static class Mod_Client_Data
{
#if Server
    #region Server

    #region 錯誤

    ///<summary>
    ///機台錯誤 0正常 1清鈔 2服務燈 3機門錯誤 4機台錯誤 5手付 6票機錯誤 8票紙過少 9鈔機錯誤
    ///</summary>
    public static string machineerror = "0"; //機台錯誤 0正常 1清鈔 2服務燈 3機門錯誤 4機台錯誤 5手付 6票機錯誤 8票紙過少 9鈔機錯誤 
    public static bool serverdisconnect = true;   //連線中斷

    #endregion

    #region 通訊編號

    public enum messagetype
    {
        ///<summary>
        ///<para>701 會員登入</para>
        ///<para>範例: 701,TXC-67,X0X0V0Q802b2148a6bf6194b2189dda104d9b424nePFjHiwk5eZR*@neJl ,1;</para>
        ///<para>0:會員登入 1:卡號 2:密碼 3:機台編號</para>
        ///</summary>
        memberlogin = 701,
        ///<summary>
        ///<para>702 入票</para>
        ///<para>範例: 702,1,4,1000,6602-8500-5117-0481;</para>
        ///<para>0:入票 1: 0無效票、不存在資料庫 1有效票 2:金額 3:票號</para>
        ///</summary>
        ticketin = 702,
        ///<summary>
        ///<para>703 出票</para>
        ///<para>範例: 703,04,1000,5,1;</para>
        ///<para>0:出票 1:票種 2:金額 3:會員編號 4:機台編號</para>
        ///</summary>
        ticketout = 703,
        ///<summary>
        ///<para>704 遊戲紀錄</para>
        ///<para>範例: 704,10,10,1287,10,100,0,1,0.1,11111,0,0,95,10,10,TXC-1,1000,100,10000,31,1000;</para>
        ///<para>0:遊戲紀錄 1:場次編號 2:押注 3:會員編號 4:贏分 5:彩分 6:是否bonus 7:遊戲編號 8:押注比例 9:滾軸排面值 10:第幾場BONUS 11:BONUS總場數
        ///12:設定的rtp 13:denom 14:特定倍率 15:卡號 16:原始點數 17:修改點數 18:修改後餘點 19:機台編號 20:剩餘贈分點數</para>
        ///</summary>
        gamehistory = 704,
        ///<summary>
        ///<para>705 機台初始值</para>
        ///<para>範例: 705,105;</para>
        ///<para>0:機台初始值 1:機台編號</para>
        ///</summary>
        gameset = 705,
        ///<summary>
        ///<para>706 入鈔</para>
        ///<para>範例: 706,6000,1;</para>
        ///<para>0:機台初始值 1:金額 1:機台編號</para>
        ///</summary>
        billin = 706,
        ///<summary>
        ///<para>707 清鈔</para>
        ///<para>範例: 707,B1;</para>
        ///<para>0:機台初始值 1:機台編號</para>
        ///</summary>
        billclear = 707,
        ///<summary>
        ///<para>708 機台事件</para>
        ///<para>範例: 708,3,0,1,1;</para>
        ///<para>0:機台事件 1:會員編號 2:機台狀態 3:機台編號 4:會員狀態</para>
        ///</summary>
        machineevent = 708,
        ///<summary>
        ///<para>709 心跳包</para>
        ///<para>範例: 709;</para>
        ///<para>0:心跳包</para>
        ///</summary>
        machinelive = 709,
        ///<summary>
        ///<para>710 連線</para>
        ///<para>範例: 710;</para>
        ///<para>0:連線</para>
        ///</summary>
        connect = 710,
        ///<summary>
        ///<para>711 手付</para>
        ///<para>範例: 711,3,1000,4,0000-1430-6035-4757,1;</para>
        ///<para>0:手付 1:會員編號 2:金額 3:機台編號 4:票號 5:手付型態</para>
        ///</summary>
        handpay = 711,
        ///<summary>
        ///<para>712 手付處理完成</para>
        ///<para>範例: 712;</para>
        ///<para>0:手付處理完成</para>
        ///</summary>
        handpayreset = 712,
        reconnent = 713,
        rewardpoint = 714,
        ///<summary>
        ///<para>715 會員登出</para>
        ///<para>範例: 715,TXC-21,3</para>
        ///<para>0:會員登出 1:卡號 2:機台編號</para>
        ///</summary>
        memberlogout = 715,
        ///<summary>
        ///<para>716 兌分</para>
        ///<para>範例: 716,3,TXC-6,2000,1000,1000,X0X086a8071a6bf6194b2189dda104d9k5eZR*@neJl,1</para>
        ///<para>0:兌分 1:卡號 2:兌分原始點數 3:兌分變更點數 4:兌分後剩餘點數 5:密碼 6:機台編號</para>
        ///</summary>
        exchangePoints = 716,
        ///<summary>
        ///<para>717 出票成功</para>
        ///<para>範例: 721,6602-8500-5117-0481,1</para>
        ///<para>0:出票成功 1:票號 2:機台編號</para>
        ///</summary>
        ticketoutsuccess = 717,
        ///<summary>
        ///<para>721 將票券改為無效票</para>
        ///<para>範例: 717,7174-6261-1745-2822,1,1000</para>
        ///<para>0:出票成功 1:票號 2:機台編號 3:金額</para>
        ///</summary>
        ticketChangeToInvalid = 721,
        ///<summary>
        ///<para>722 機台開分</para>
        ///<para>範例: 722,1,3,1000</para>
        ///<para>0:機台開分 1:機台編號 2:會員編號 3:金額</para>
        ///</summary>
        pointIn = 722,
        ///<summary>
        ///<para>723 將票券改為無效票</para>
        ///<para>範例: 723,1,3,1000</para>
        ///<para>0:機台開分 1:機台編號 2:會員編號 3:金額</para>
        ///</summary>
        pointOut = 723,
        creditin = 802
    }

    #endregion

    #region 遊戲相關

    public static int gameIndex = Mod_Data.gameIndex; //遊戲場次
    public static string bet = Mod_Data.Bet.ToString();                  //押注
    public static string winscroe = Mod_Data.Win.ToString();                 //贏分
    public static string credit = Mod_Data.credit.ToString();                     //彩分    
    public static string bonusPoints = Mod_Data.bonusPoints.ToString();                     //彩分    
    public static string bonus = Mod_Data.getBonus.ToString();                     //遊戲是否bonus
    public static string gameID = Mod_Data.gameID;//Mod_Data.projectName;                  //遊戲編號
    public static string betOdds = Mod_Data.odds.ToString();                    //押注比例 
    public static string gameRNG = Mod_Data.RNG[0].ToString() + "." + Mod_Data.RNG[1].ToString() + "." + Mod_Data.RNG[2].ToString() + "." + Mod_Data.RNG[3].ToString() + "." + Mod_Data.RNG[4].ToString();                //遊戲滾輪值
    public static string bonusCurrent = Mod_Data.BonusIsPlayedCount.ToString();            //當前bonus場次(第幾場)
    public static string bonusTotalgmae = Mod_Data.BonusCount.ToString();          //Bonus遊戲總次數
    public static string rtpset = Mod_Data.RTPsetting.ToString();                  //設置的rtp
    public static string denom = Mod_Data.Denom.ToString();                      //比率
    public static string specialrules = Mod_Data.BonusSpecialTimes.ToString();               //特定倍率

    #endregion

    #region 會員相關

    public static string memberID = Mod_Data.memberID;                   //會員編號
    public static string cardID = Mod_Data.cardID;                     //卡片編號
    public static string memberlevel = " ";  //會員等級
    public static string handPaymemberID = " ";                   //手付用暫時會員編號
    public static float memberRakebackPointOri = Mod_Data.memberRakebackPoint - Mod_Data.getRakebackPoint; //會員原始返點
    public static float getRakebackPoint = Mod_Data.getRakebackPoint; //會員返點修改值
    public static float memberRakebackPointAfter = Mod_Data.memberRakebackPoint; //會員修改後返點
    public static bool MemberCardOut = false; //會員卡拔出

    #endregion

    #region 鈔機相關

    public static string ticketNum;                  //票號
    public static string ticketType = "04";          //票種  01:正式票 02:試玩票
    public static string ticketAmount;               //票額  
    public static string ticketValidate;             //票有效期限 
    public static string ticketInAmount;                  //入票金額
    public static string billAmount = Mod_Data.cash.ToString();                 //鈔票面額

    #endregion

    #region 彩分檢查

    ///<summary>
    ///<para>此功能用來檢查彩分是否該顯示小數點後的數字，利用傳過了的彩分字串分割出小數點前彩分和小數點後彩分</para>
    ///<para><returns>小數點後大於零，返回原始字串</returns></para>
    ///<para><returns>小數點後小於等於零，返回小數點前字串</returns></para>
    ///</summary>
    public static string CreditToStrnigCheck(string credit)
    {
        string[] credits = credit.Split('.');
        if (decimal.Parse(credits[1]) > 0) return credit;
        else return credits[0];
    }

    #endregion

    #endregion
#endif
}

#endregion

#region Ini檔讀取和寫入

public class MyIni
{
    [DllImport("kernel32")]
    public static extern long WritePrivateProfileString(string section, string key, string value, string path);
    [DllImport("kernel32")]
    public static extern int GetPrivateProfileString(string section, string key, string deval, StringBuilder stringBuilder, int size, string path);

    public string path; //Ini文件路徑
    //設置Ini文件路徑
    public MyIni(string path)
    {
        this.path = path;
    }

    //寫入Ini文件
    public void WriteIniContent(string section, string key, string value)
    {
        WritePrivateProfileString(section, key, value, this.path);
    }

    //讀取Ini文件
    public string ReadIniContent(string section, string key)
    {
        StringBuilder temp = new StringBuilder(255);
        int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
        return temp.ToString();
    }

    //判斷入徑是否正確
    public bool IsIniPath()
    {
        return File.Exists(this.path);
    }
}

#endregion

#region 備份

// public class MyIni
// {
// #if Server
//     #region Server
//     public string path;//ini文件的路径
//     public MyIni(string path)
//     {
//         this.path = path;
//     }
//     [DllImport("kernel32")]
//     public static extern long WritePrivateProfileString(string section, string key, string value, string path);
//     [DllImport("kernel32")]
//     public static extern int GetPrivateProfileString(string section, string key, string deval, StringBuilder stringBuilder, int size, string path);

//     //写入ini文件
//     public void WriteIniContent(string section, string key, string value)
//     {
//         WritePrivateProfileString(section, key, value, this.path);
//     }

//     //读取Ini文件
//     public string ReadIniContent(string section, string key)
//     {
//         StringBuilder temp = new StringBuilder(255);
//         int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
//         return temp.ToString();
//     }

//     //判断路径是否正确
//     public bool IsIniPath()
//     {
//         return File.Exists(this.path);
//     }
//     #endregion
// #endif
// }

// public class Mod_Client : IGameSystem
// {
// #if Server
//     #region  Server
//     //  #region private members 	
//     public Text gt;
//     private TcpClient socketConnection;
//     private Thread clientReceiveThread;
//     public Thread thread, ServerTread;
//     Socket SocketClient = null;
//     public string ml;
//     public string serverIp;
//     public int serverPort;
//     MyIni ini;
//     public int position = 0;
//     public string S1, S2;
//     public string testSign = "706,1000";
//     public int randomSleepMin = 300;
//     public int randomSleepMax = 400;
//     public int sentTimes = 1000;
//     public int currentTimes = 0;
//     Random rnd;
//     public int sleeptime;
//     log debuglog;
//     bool _OpenClear = false;
//     bool OpenClear
//     {
//         get { return _OpenClear; }
//         set
//         {
//             _OpenClear = value;
//             if (_OpenClear)
//             {
//                 if (openClearScore > 0)
//                 {
//                     m_SlotMediatorController.SendMessage("m_client", "OpenPoint", openClearScore);
//                 }
//                 else if (openClearScore < 0)
//                 {
//                     m_SlotMediatorController.SendMessage("m_client", "ClearPoint", openClearScore);
//                 }
//             }
//         }
//     }
//     int openClearScore = 0;

//     BillAcceptorController billAcceptorController;
//     Mod_UIController mod_UIController;
//     NewSramManager newSramManager;
//     Mod_Gen2_Status gen2_Status;
//     Mod_OpenClearPoint mod_OpenClearPoint;
//     Mod_TextController mod_TextController;
//     BackEnd_Data backEnd_Data;

//     // #endregion
//     // Use this for initialization 	    
//     void Start()
//     {
//         Mod_Data.memberLcok = true;
//         Mod_Data.machineerror = "0";
//         Mod_Data.cardID = " ";
//         Mod_Data.barcode = " ";
//         Mod_Data.serial = " ";
//         Mod_Data.serial2 = " ";
//         mod_UIController = FindObjectOfType<Mod_UIController>();
//         newSramManager = FindObjectOfType<NewSramManager>();
//         gen2_Status = FindObjectOfType<Mod_Gen2_Status>();
//         mod_OpenClearPoint = FindObjectOfType<Mod_OpenClearPoint>();
//         mod_TextController = FindObjectOfType<Mod_TextController>();
//         billAcceptorController  = FindObjectOfType<BillAcceptorController>();
//         backEnd_Data= FindObjectOfType<BackEnd_Data>();

//         if (newSramManager.LoadHandPayStatus() != 0) CallHandPay = true;

//         ini = new MyIni(Application.streamingAssetsPath + "/IPsetting.ini");
//         serverIp = ini.ReadIniContent("IP", "serverIp");
//         serverPort = int.Parse(ini.ReadIniContent("Port", "serverPort"));
//         testSign = ini.ReadIniContent("SIGN", "testSign");
//         randomSleepMin = int.Parse(ini.ReadIniContent("SleepTimeMin", "randomSleepMin"));
//         randomSleepMax = int.Parse(ini.ReadIniContent("SleepTimeMax", "randomSleepMax"));
//         sentTimes = int.Parse(ini.ReadIniContent("SentTimes", "sentTimes"));
//         starting();
//         rnd = new Random(Guid.NewGuid().GetHashCode());
//         //TestServer();
//         debuglog = new log();
//         //Debug.Log(Mod_Data.barcode);
//         // SendToSever(Mod_Client_Data.messagetype.gameset);
//         //Debug.Log(StringEncrypt.ComputeSha256Hash("0123456789"));
//     }

//     public void TestServer()
//     {
//         ServerTread = new Thread(() =>
//         {
//             Thread.Sleep(5000);
//             while (currentTimes < sentTimes)
//             {

//                 currentTimes++;
//                 sleeptime = rnd.Next(randomSleepMin, randomSleepMax);
//                 SendMessages("704," + currentTimes + ",60,3,500,600,0,2,10,9,0,0,95,1,1,TXC-1,1000,100,1100;");
//                 Debug.Log("現在次數:" + currentTimes + "/" + sentTimes + "現在幾毫秒:" + sleeptime);
//                 //debuglog.create_log(testSign + " 現在次數:" + currentTimes + "/" + sentTimes + "現在幾毫秒:" + sleeptime);
//                 Thread.Sleep(sleeptime);

//             }
//         });
//         ServerTread.Start();
//     }

//     public void starting()
//     {
//         StaticLink();
//         thread = new Thread(() =>
//         {
//             while (true)
//             {
//                 if (Mod_Client_Data.serverdisconnect)
//                 {
//                     SocketClient = null;
//                     Mod_Data.machineID = "";
//                     Mod_Data.machineInit = false;
//                     Strhandle("713"); //Server斷線
//                     Debug.Log(" Reconnect in 3 secs");
//                     Thread.Sleep(3000);
//                     StaticLink();
//                     Debug.Log("----Reconnecting...----");
//                 }
//                 Thread.Sleep(1);
//             }
//         });
//         thread.Start();
//     }
//     public void StaticLink()
//     {
//         try
//         {
//             AsyncCallback asynccallback = new AsyncCallback(StaticSendMsg);
//             SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//             IAsyncResult result = SocketClient.BeginConnect(serverIp, serverPort, asynccallback, null);
//             SocketClient.EndConnect(result);
//             Mod_Client_Data.serverdisconnect = false;
//             Debug.Log("connecting");
//             Debug.Log("send something to server");
//         }
//         catch (Exception e)
//         {
//             Debug.Log(e.ToString());
//             Debug.Log("client to server failed");
//             Mod_Data.machineID = "";
//             Mod_Data.machineInit = false;
//             Mod_Client_Data.serverdisconnect = true;
//             Strhandle("713"); //Server斷線S
//             return;
//         }
//     }

//     public void StaticSendMsg(IAsyncResult result)
//     {
//         byte[] bytes = new byte[1024];
//         int a = 0;
//         do
//         {
//             try
//             {
//                 a = SocketClient.Receive(bytes);
//                 if (a > 0)
//                 {
//                     string b = System.Text.Encoding.UTF8.GetString(bytes, 0, a);
//                     Debug.Log("Msg 客戶端：" + b);
//                     Strhandle(b);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Mod_Client_Data.serverdisconnect = true;
//                 Debug.Log("斷線: " + ex);
//                 return;
//             }
//         } while (a > 0);
//     }

//     // Update is called once per frame
//     bool[] sendMachineError = new bool[8];
//     bool[] servetGetMachineError = new bool[8];
//     void Update()
//     {
//     #region 卡機
//         if (Input.inputString.Length > 0)
//         {
//             string cardstore = "";
//             string cardnum = "";
//             string cardcrynum = "";
//             string S3 = "";
//             S1 += Input.inputString;
//             if (S1.Contains("Card Seated"))
//             {
//                 try
//                 {
//                     //Debug.Log(S1);
//                     S3 = S1.Substring(S1.IndexOf("?") + 1);
//                     S3 = S3.Substring(S3.IndexOf("C"));
//                     S2 = S1.Substring(S1.IndexOf("%") + 1, S1.IndexOf("?") - (S1.IndexOf("%") + 1));
//                     //Debug.Log(" cardID: " + S2);
//                     //Debug.Log("S3: " + S3);
//                     if (!string.Equals(S3, "Card Seated"))
//                     {
//                         S1 = "";
//                         S2 = "";
//                         S3 = "";
//                         Debug.Log("Not Equals Return!");
//                         return;
//                     }
//                     if (S2.Contains("-"))
//                     {
//                         cardstore = S2.Substring(0, S2.IndexOf("-") + 1);
//                         cardnum = S2.Substring(S2.IndexOf("-") + 1);
//                     }

//                     if (((cardnum.Length % 2) == 0) && (cardnum[0].ToString() == "3"))
//                     {
//                         for (int iac = 1; iac <= cardnum.Length; iac++)
//                         {
//                             if (iac % 2 == 1)
//                             {
//                                 cardcrynum += cardnum[iac];
//                             }
//                         }
//                     }
//                     //Debug.Log(" serverdisconnect: " + Mod_Client_Data.serverdisconnect + " machineInit: " + Mod_Data.machineInit + " cardneeded: " + Mod_Data.cardneeded + " MachineError: " + Mod_Data.MachineError + " MachineEventWaitS: " + MachineEventWaitServer + " MemberLogOutWait: " + MemberLogOutWait + " SendMemberLogOut: " + SendMemberLogOut);
//                     if (!Mod_Client_Data.serverdisconnect && Mod_Data.machineInit && Mod_Data.cardneeded == 1 && !Mod_Data.MachineError && !MachineEventWaitServer)
//                     {
//                         //Debug.Log("LogIn!!");
//                        // Debug.Log("MemberLogIn: " + MemberLogIn + " MemberLogInHard: " + MemberLogInHard + " Mod_Client_Data.MemberCardOut: " + Mod_Client_Data.MemberCardOut);
//                         if (Mod_Data.inBaseSpin & !MemberLogOutWait && !SendMemberLogOut)
//                         {
//                             Mod_Client_Data.MemberCardOut = false;
//                             MemberLogIn = true;
//                             Mod_Data.cardID = cardstore + cardcrynum;
//                             SendToSever(Mod_Client_Data.messagetype.memberlogin);
//                         }
//                         else if (!Mod_Data.inBaseSpin)
//                         {
//                             MemberLogInHard = true;
//                             Mod_Client_Data.MemberCardOut = false;
//                             SendMemberLogOut = false;
//                             MemberLogIn = true;
//                             Mod_Data.cardID = cardstore + cardcrynum;
//                             SendToSever(Mod_Client_Data.messagetype.memberlogin);
//                         }
//                     }
//                     S1 = "";
//                 }
//                 catch (Exception ex)
//                 {
//                     Debug.Log("Exception: " + "ID Card Length Wrong");
//                     S1 = "";
//                 }
//             }
//             else if (S1.Contains("Media Detected"))
//             {
//                 Debug.Log(" ME " + S1);
//                 S1 = S1.Replace("Media Detected", "");
//             }
//             else if (S1.Contains("Card Removed"))
//             {
//                 Debug.Log(S1);
//                 //Debug.Log(" cardID: " + string.IsNullOrWhiteSpace(Mod_Data.cardID) + " MemberLogOutWait: " + MemberLogOutWait + " SendMemberLogOut: " + SendMemberLogOut);
//                 if (!string.IsNullOrWhiteSpace(Mod_Data.cardID) && !MemberLogOutWait && !SendMemberLogOut)
//                 {
//                     if (Mod_Data.cardneeded == 1 && !Mod_Data.inBaseSpin) Mod_Client_Data.MemberCardOut = true;
//                     else Mod_Data.memberLcok = true;
//                     SendMemberLogOut = true;
//                 }
//                 //Mod_Client_Data.cardID = "";
//                 S1 = "";
//                 Mod_ChangePonit.CloseMemberPanel = true;
//             }
//             else if (S1.Contains("No Data"))
//             {
//                 if (!Mod_Client_Data.serverdisconnect && !string.IsNullOrWhiteSpace(Mod_Data.cardID) && Mod_Data.cardneeded == 1 && !Mod_Data.MachineError && !MachineEventWaitServer && !MemberLogOutWait) Mod_Data.cardID = " ";
//                 S1 = "";
//             }
//         }
//     #endregion

//         sendMachineError[0] = Mod_Data.handpay == true ? true : false;
//         sendMachineError[1] = Mod_Data.billStackOpenerror == true ? true : false;
//         sendMachineError[2] = Mod_Data.printerError == true ? true : false;
//         sendMachineError[3] = BillAcceptorSettingData.ErrorBool == true ? true : false;
//         sendMachineError[4] = Mod_Data.doorError == true ? true : false;
//         sendMachineError[5] = mod_TextController.printerTextBool[26] == true ? true : false;
//         sendMachineError[6] = Mod_Data.MachineError == true ? true : false;
//         sendMachineError[7] = Mod_Data.serviceLighterror == true ? true : false;

//         if (Mod_Data.billStackOpenerror || Mod_Data.doorError || Mod_Data.serviceLighterror || Mod_Data.MachineError || Mod_Data.printerError || Mod_Data.handpay || BillAcceptorSettingData.ErrorBool || mod_TextController.printerTextBool[26])
//         {
//             // Debug.Log("Mod_Data.billStackOpenerror: " + Mod_Data.billStackOpenerror + " Mod_Data.doorError: " + Mod_Data.doorError + " Mod_Data.serviceLighterror: " + Mod_Data.serviceLighterror + " Mod_Data.MachineError: " + Mod_Data.MachineError + " Mod_Data.printerError: " + Mod_Data.printerError + " Mod_Data.handpay: " + Mod_Data.handpay + " BillAcceptorSettingData.ErrorBool: " + BillAcceptorSettingData.ErrorBool);
//             if (!ErrorCoroutineRunning)
//             {
//                 StartCoroutine("SendMachineError");
//                 //Debug.Log("Start ErrorCoroutineRunning");
//             }
//         }
//         else if (ErrorCoroutineRunning)
//         {
//             StopCoroutine("SendMachineError");
//             //Debug.Log("Stop ErrorCoroutineRunning");
//             ErrorCoroutineRunning = false;
//             Mod_Data.machineerror = "0";
//             SendMachineEvent = true;
//         }

//         Mod_TextController.allTextBool[9] = Mod_Client_Data.serverdisconnect == true ? true : false;
//         Mod_TextController.allTextBool[11] = Mod_Data.serverLock == true ? true : false;
//         Mod_TextController.allTextBool[12] = Mod_Data.machineIDLock == true ? true : false;
//         Mod_TextController.allTextBool[14] = Mod_Data.machineInit == false ? true : false;
//         if (!Mod_TextController.ErrorTextRunning && (Mod_Client_Data.serverdisconnect || Mod_Data.serverLock || Mod_Data.machineIDLock || !Mod_Data.machineInit)) Mod_TextController.RunErrorTextBool = true;

//         if (CallHandPay || Mod_Data.creditErrorLock || Mod_Data.winErrorLock)
//         {
//             if (!HandPayCoroutineRun) StartCoroutine("HandPayCoroutine");
//             //Debug.Log("CallHandPay: " + CallHandPay + " Mod_Data.creditErrorLock: " + Mod_Data.creditErrorLock + " Mod_Data.winErrorLock: " + Mod_Data.winErrorLock);
//             CallHandPay = false;
//         }

//         if (Mod_Data.memberLcok)
//         {
//             m_SlotMediatorController.SendMessage("m_state", "memberLock", 1);
//             Mod_Data.autoPlay = false;
//         }
//         else
//         {
//             m_SlotMediatorController.SendMessage("m_state", "memberLock", 0);
//         }

//         if (Mod_Data.cardneeded == 1 && !Mod_Client_Data.serverdisconnect && !Mod_Data.memberLcok && !MemberLogOutWait && !String.IsNullOrWhiteSpace(Mod_Data.memberID) && (Mod_Data.billStackOpenerror || Mod_Data.doorError || Mod_Data.MachineError || Mod_Data.printerError || Mod_Data.handpay))
//         {
//             Mod_Data.memberLcok = true;
//             SendMemberLogOut = true;
//             Mod_Client_Data.MemberCardOut = false;
//         }

//         if (Input.GetKeyDown(KeyCode.K))
//         {
//             WaitServerHandPay = false;
//             WaitStationHandPay = false;
//         }

//         if (!Mod_Data.serverLock && !Mod_Client_Data.serverdisconnect) severDelay_Time += Time.unscaledDeltaTime;
//         if (!Mod_Client_Data.serverdisconnect && (string.IsNullOrWhiteSpace(Mod_Data.machineID) || !Mod_Data.machineInit)) machineInit_Time += Time.unscaledDeltaTime;
//         if (ClearHandPayMemberID)
//         {
//             if (String.IsNullOrWhiteSpace(Mod_Client_Data.handPaymemberID)) ClearHandPayMemberID = false;
//             if (!Mod_Data.handpay) clearHandPayIDTime += Time.unscaledDeltaTime;
//             else if (clearHandPayIDTime > 0) clearHandPayIDTime = 0;
//         }
//         else if (clearHandPayIDTime > 0) clearHandPayIDTime = 0;
//     }

//     //SendMachineInit(710)ToServer
//     private float _machineInit_Time = 0;
//     float machineInit_Time
//     {
//         get { return _machineInit_Time; }
//         set
//         {
//             _machineInit_Time = value;
//             if (_machineInit_Time > 3)
//             {
//                 if (string.IsNullOrWhiteSpace(Mod_Data.machineID)) SendToSever(Mod_Client_Data.messagetype.connect);
//                 else if (!Mod_Data.machineInit) SendToSever(Mod_Client_Data.messagetype.gameset);
//                 _machineInit_Time = 0;
//             }
//         }
//     }

//     //ServerDelay-Notices
//     private float _severDelay_Time = 0;
//     float severDelay_Time
//     {
//         get { return _severDelay_Time; }
//         set
//         {
//             _severDelay_Time = value;
//             if (_severDelay_Time == 0) Mod_TextController.WaitServerTextRunning = false;
//             if (!Mod_TextController.WaitServerTextRunning && _severDelay_Time >= 15) Mod_TextController.RunWaitServerTextBool = true;
//         }
//     }

//     //ChooseMachineEventSendToServer(708)
//     bool ErrorCoroutineRunning = false;
//     WaitForSecondsRealtime WaitSendMachineErrorTime = new WaitForSecondsRealtime(1f);
//     IEnumerator SendMachineError()
//     {
//         ErrorCoroutineRunning = true;

//         yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
//         int findTrue = 0;

//         for (int i = 0; i < sendMachineError.Length; i++)
//         {
//             servetGetMachineError[i] = false;
//         }

//         while (true)
//         {
//             if (sendMachineError[findTrue] && !servetGetMachineError[findTrue])
//             {
//                 servetGetMachineError[findTrue] = true;

//                 switch (findTrue)
//                 {
//                     case 0: Mod_Data.machineerror = "5"; break; //handpay
//                     case 1: Mod_Data.machineerror = "1"; break; //billclear
//                     case 2: Mod_Data.machineerror = "6"; break; //printererror
//                     case 3: Mod_Data.machineerror = "9"; break; //billerror
//                     case 4: Mod_Data.machineerror = "3"; break; //doorerror
//                     case 5: Mod_Data.machineerror = "8"; break; //paperLow
//                     case 6: Mod_Data.machineerror = "4"; break; //machineerror
//                     case 7: Mod_Data.machineerror = "2"; break; //servicelight
//                 }

//                 yield return new WaitUntil(() => MachineEventWaitServer == false);
//                 SendMachineEvent = true;
//                 yield return new WaitUntil(() => MachineEventWaitServer == false);

//                 if (Mod_Data.machineerror == "1" && !BillClearWaitSever) SendBillClear = true;
//                 yield return new WaitUntil(() => BillClearWaitSever == false && SendBillClear == false);

//                 int i = 0;

//                 while (true)
//                 {
//                     if ((sendMachineError[i] && i < findTrue) || !sendMachineError[findTrue]) break;
//                     i++;
//                     if (i > sendMachineError.Length - 1) i = 0;
//                     yield return null;
//                 }

//                 for (int p = 0; p < sendMachineError.Length; p++)
//                 {
//                     if (!sendMachineError[findTrue]) servetGetMachineError[p] = false;
//                 }

//                 findTrue = 0;

//                 yield return WaitSendMachineErrorTime;
//             }

//             findTrue++;
//             if (findTrue > sendMachineError.Length - 1) findTrue = 0;
//             yield return null;
//         }
//     }

//     //SendMachineEvent(708)ToServer
//     private bool _SendMachineEvent = false;
//     bool SendMachineEvent
//     {
//         get { return _SendMachineEvent; }
//         set
//         {
//             _SendMachineEvent = value;
//             if (_SendMachineEvent == true)
//             {
//                 //Debug.Log("SendMachineEvent");
//                 if (SendMachineEvent_Coroutine != null) StopCoroutine(SendMachineEvent_Coroutine);
//                 SendMachineEvent_Coroutine = StartCoroutine(SendMachineEvent_IE());
//                 _SendMachineEvent = false;
//             }
//         }
//     }
//     bool MachineEventWaitServer = false;
//     WaitForSecondsRealtime WaitMachineEventSortTime = new WaitForSecondsRealtime(0.1f);
//     WaitForSecondsRealtime WaitMachineEventLongTime = new WaitForSecondsRealtime(1f);
//     WaitUntil WaitMachineEvent_Until = new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
//     Coroutine SendMachineEvent_Coroutine;
//     IEnumerator SendMachineEvent_IE()
//     {
//         MachineEventWaitServer = true;

//         yield return WaitMachineEvent_Until;
//         SendToSever(Mod_Client_Data.messagetype.machineevent);

//         while (true)
//         {
//             if (!MachineEventWaitServer) break;
//             if (Mod_Client_Data.serverdisconnect)
//             {
//                 yield return WaitMachineEvent_Until;
//                 yield return WaitMachineEventLongTime;
//                 SendToSever(Mod_Client_Data.messagetype.machineevent);
//             }
//             yield return WaitMachineEventSortTime;
//         }
//     }

//     //SendBillClear(707)ToServer
//     private bool _SendBillClear = false;
//     bool SendBillClear
//     {
//         get { return _SendBillClear; }
//         set
//         {
//             _SendBillClear = value;
//             if (_SendBillClear == true)
//             {
//                 //Debug.Log("SendMachineEvent");
//                 if (WaitBillClear_Coroutine != null) StopCoroutine(WaitBillClear_Coroutine);
//                 WaitBillClear_Coroutine = StartCoroutine(WaitBillClear_IE());
//                 _SendBillClear = false;
//             }
//         }
//     }
//     bool BillClearWaitSever = false;
//     WaitForSecondsRealtime WaitBillClearShortTime = new WaitForSecondsRealtime(0.1f);
//     WaitForSecondsRealtime WaitBillClearLongTime = new WaitForSecondsRealtime(1f);
//     WaitUntil WaitBillClear_Until = new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
//     Coroutine WaitBillClear_Coroutine;
//     IEnumerator WaitBillClear_IE()
//     {
//         BillClearWaitSever = true;

//         yield return WaitBillClear_Until;
//         SendToSever(Mod_Client_Data.messagetype.billclear);

//         while (true)
//         {
//             if (!BillClearWaitSever) break;
//             if (Mod_Client_Data.serverdisconnect)
//             {
//                 yield return WaitBillClear_Until;
//                 yield return WaitBillClearLongTime;
//                 SendToSever(Mod_Client_Data.messagetype.billclear);
//             }
//             yield return WaitBillClearShortTime;
//         }
//     }

//     //SendMemberLogOut(715)ToServer
//     public static bool MemberLogIn = false;
//     public static bool MemberLogInHard = false;
//     bool MemberLogOutWait = false;
//     private bool _SendMemberLogOut = false;
//     bool SendMemberLogOut
//     {
//         get { return _SendMemberLogOut; }
//         set
//         {
//             _SendMemberLogOut = value;
//             Mod_ChangePonit.CloseMemberPanel = true;
//             if (SendMemberLogOut_Coroutine != null) StopCoroutine(SendMemberLogOut_Coroutine);
//             if (_SendMemberLogOut) SendMemberLogOut_Coroutine = StartCoroutine(SendMemberLogOut_IE());
//             if (!_SendMemberLogOut) MemberLogOutWait = false;
//             _SendMemberLogOut = false;
//         }
//     }
//     WaitForSecondsRealtime WaitMemberLogOutShortTime = new WaitForSecondsRealtime(0.1f);
//     WaitForSecondsRealtime WaitMemberLogOutLongTime = new WaitForSecondsRealtime(1f);
//     WaitUntil WaitMemberLogOut_Until = new WaitUntil(() => (!Mod_Client_Data.serverdisconnect && Mod_Data.machineInit
//      && (!MemberLogIn && !Mod_Client_Data.MemberCardOut || MemberLogInHard) && ((!Mod_Data.handpay && !WaitServerHandPay) || string.IsNullOrWhiteSpace(Mod_Data.memberID))));
//     Coroutine SendMemberLogOut_Coroutine;
//     IEnumerator SendMemberLogOut_IE()
//     {
//         MemberLogOutWait = true;
//         yield return WaitMemberLogOut_Until;
//         if (MemberLogInHard || string.IsNullOrWhiteSpace(Mod_Data.cardID))
//         {
//             MemberLogOutWait = false;
//             yield return new WaitUntil(() => MachineEventWaitServer == false);
//             SendMachineEvent = true;
//             yield break;
//         }
//         if (Mod_Data.cardneeded == 0)
//         {
//             Mod_Data.memberID = " ";
//             Mod_Client_Data.memberID = " ";
//             Mod_Client_Data.cardID = " ";
//             Mod_Client_Data.memberlevel = " ";
//             Mod_Client_Data.memberRakebackPointOri = 0;
//             Mod_Client_Data.getRakebackPoint = 0;
//             Mod_Client_Data.memberRakebackPointAfter = 0;
//         }

//         SendToSever(Mod_Client_Data.messagetype.memberlogout);

//         while (true)
//         {
//             if (!MemberLogOutWait || string.IsNullOrWhiteSpace(Mod_Data.cardID)) break;
//             if (Mod_Client_Data.serverdisconnect)
//             {
//                 yield return WaitMemberLogOut_Until;

//                 if (MemberLogInHard || string.IsNullOrWhiteSpace(Mod_Data.cardID))
//                 {
//                     MemberLogOutWait = false;
//                     yield return new WaitUntil(() => MachineEventWaitServer == false);
//                     SendMachineEvent = true;
//                     yield break;
//                 }

//                 yield return WaitMemberLogOutLongTime;
//                 if (Mod_Data.cardneeded == 0)
//                 {
//                     Mod_Client_Data.memberID = " ";
//                     Mod_Client_Data.cardID = " ";
//                     Mod_Client_Data.memberlevel = " ";
//                     Mod_Client_Data.memberRakebackPointOri = 0;
//                     Mod_Client_Data.getRakebackPoint = 0;
//                     Mod_Client_Data.memberRakebackPointAfter = 0;
//                 }
//                 SendToSever(Mod_Client_Data.messagetype.memberlogout);
//             }
//             yield return WaitMemberLogOutShortTime;
//         }

//         yield return new WaitUntil(() => MachineEventWaitServer == false);
//         SendMachineEvent = true;
//     }

//     //清除暫時用手付會員資料
//     bool ClearHandPayMemberID = false;
//     private float _clearHandPayIDTime = 0;
//     float clearHandPayIDTime
//     {
//         get { return _clearHandPayIDTime; }
//         set
//         {
//             _clearHandPayIDTime = value;
//             //Debug.Log("Add Time! : " + _clearHandPayIDTime);
//             if (_clearHandPayIDTime >= 10)
//             {
//                 //Debug.Log("Mod_Client_Data.handPaymemberID clearHandPayIDTime : " + Mod_Client_Data.handPaymemberID);
//                 Mod_Client_Data.handPaymemberID = " ";
//                 //Debug.Log("Mod_Client_Data.handPaymemberID clearHandPayIDTime : " + Mod_Client_Data.handPaymemberID);
//                 _clearHandPayIDTime = 0;
//             }
//         }
//     }

//     //711手付處理
//     WaitForSecondsRealtime WaitHandPayCoroutineShortTime = new WaitForSecondsRealtime(0.1f);
//     WaitForSecondsRealtime WaitHandPayCoroutineLongTime = new WaitForSecondsRealtime(1f);
//     public static bool CallHandPay = false;
//     public static bool HandPayCoroutineRun = false;
//     public static bool WaitServerHandPay = false;
//     public bool WaitStationHandPay = false;
//     IEnumerator HandPayCoroutine()
//     {
//         //Debug.Log("HandPay Running");
//         Mod_Data.handpay = true;
//         HandPayCoroutineRun = true;
//         WaitServerHandPay = true;
//         WaitStationHandPay = true;
//         Mod_TextController.printerTicketOutValueInfo_Obj.SetActive(true);
//         if (Mod_TextController.printerTicketOutValueInfo_Image.enabled == false) Mod_TextController.printerTicketOutValueInfo_Image.enabled = true;

//         if (gen2_Status.ticketPirintDone) Mod_TextController.printerTicketOutValue_Text.text = "HAND PAY : " + Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty)) + "<color=red>" + "\n" + Mod_Data.serial + "\n" + "此票為無效票" + "</color>";
//         else if (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3)
//         {
//             if (string.IsNullOrWhiteSpace(Mod_Client_Data.ticketInAmount)) Mod_TextController.printerTicketOutValue_Text.text = "HAND PAY : " + "<color=red>" + "\n" + newSramManager.LoadTicketSerial() + "\n" + "此票資料有誤" + "</color>";
//             else Mod_TextController.printerTicketOutValue_Text.text = "HAND PAY : " + Mod_Client_Data.ticketInAmount + "<color=red>" + "\n" + newSramManager.LoadTicketSerial() + "\n" + "此票資料有誤" + "</color>";
//         }
//         else Mod_TextController.printerTicketOutValue_Text.text = "HAND PAY : " + Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty));

//         yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);

//         if (!servetGetMachineError[0] && !ErrorCoroutineRunning)
//         {
//             StopCoroutine("SendMachineError");
//             StartCoroutine("SendMachineError");
//         }

//         yield return WaitHandPayCoroutineLongTime;
//         yield return WaitHandPayCoroutineLongTime;
//         yield return new WaitUntil(() => MachineEventWaitServer == false);

//         SendToSever(Mod_Client_Data.messagetype.handpay);

//         while (true)
//         {
//             if (!WaitServerHandPay) break;
//             if (Mod_Client_Data.serverdisconnect)
//             {
//                 //Debug.Log("Handpay serverdisconnect!");
//                 yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
//                 yield return WaitHandPayCoroutineLongTime;
//                 SendToSever(Mod_Client_Data.messagetype.handpay);
//             }
//             yield return WaitHandPayCoroutineShortTime;
//         }

//         yield return new WaitUntil(() => WaitStationHandPay == false);

//         if (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3)
//         {
//             if (newSramManager.LoadHandPayStatus() == 1 || newSramManager.LoadHandPayStatus() == 3) newSramManager.SaveHandPayStatus(1);
//             else newSramManager.SaveHandPayStatus(0);
//             newSramManager.SaveTicketSerial(null);
//             Mod_TextController.printerTicketOutValueInfo_Image.enabled = false;
//             Mod_TextController.printerTicketOutValue_Text.text = null;
//             Mod_TextController.billAndTciketInfo_Text.text = null;
//         }
//         else
//         {
//             BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.ticketOut, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.ticketOut) + Mod_Data.credit);
//             BackEnd_Data.SetDouble(BackEnd_Data.SramAccountData.ticketOut_Class, BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.ticketOut_Class) + Mod_Data.credit);
//             newSramManager.saveEventRecoredByEventName(6, (int)(Mod_Data.credit * 100));
//             newSramManager.SaveTicketOutPoint(newSramManager.LoadTicketOutPoint() + Mod_Data.credit);
//             if (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3) newSramManager.SaveHandPayStatus(2);
//             else newSramManager.SaveHandPayStatus(0);

//             Mod_TextController.printerTicketOutValueInfo_Image.enabled = false;
//             Mod_TextController.printerTicketOutValue_Text.text = null;
//             Mod_TextController.billAndTciketInfo_Text.text = "HAND PAY Done: " + Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty));
//             Mod_Data.credit = 0;
//             Mod_Data.Win = 0;
//             mod_UIController.UpdateScore();
//             //Debug.Log("HandPay: " + " Credit: " + Mod_Data.credit + " Win: " + Mod_Data.Win);
//         }

//         yield return WaitHandPayCoroutineLongTime;
//         Mod_Data.handpay = false;
//         if (!MachineEventWaitServer)
//         {
//             Mod_Data.machineerror = "0";
//             SendMachineEvent = true;
//         }
//         HandPayCoroutineRun = false;
//         if (newSramManager.LoadHandPayStatus() != 0) Mod_Client.CallHandPay = true;
//         //Debug.Log("Mod_Data.PrintTicket: " + Mod_Data.PrinterTicket + " HandPayCoroutineRun: " + HandPayCoroutineRun + " HandPay: " + Mod_Data.handpay);
//     }

//     WaitForSecondsRealtime WaitSend704GameHistoryShortTime = new WaitForSecondsRealtime(0.1f);
//     WaitForSecondsRealtime WaitSend704GameHistoryLongTime = new WaitForSecondsRealtime(1f);
//     public bool _CallSendGameHisotry = false;
//     public bool CallSendGameHisotry
//     {
//         get { return _CallSendGameHisotry; }
//         set
//         {
//             _CallSendGameHisotry = value;
//             if (!send704Running) StartCoroutine("Send704GameHistory");
//             _CallSendGameHisotry = false;
//         }
//     }
//     bool send704Running = false;
//     IEnumerator Send704GameHistory()
//     {
//         send704Running = true;
//         yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
//         SendToSever(Mod_Client_Data.messagetype.gamehistory);

//         while (true)
//         {
//             if (!send704Running) break;
//             if (Mod_Client_Data.serverdisconnect)
//             {
//                 yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
//                 GetLocalGameRound();
//                 //Debug.Log("Send704GameHistory : " + " Mod_Data.localRound: " + Mod_Data.localRound + " Mod_Data.localBonusRound: " + Mod_Data.localBonusRound);
//                 yield return WaitSend704GameHistoryLongTime;

//                 if (!Mod_Data.BonusSwitch)
//                 {
//                     if (Mod_Data.gameIndex < Mod_Data.localRound)
//                     {
//                         //Debug.Log("Mod_Data.gameIndex 1: " + Mod_Data.gameIndex);
//                         Mod_Data.gameIndex++;
//                         //Debug.Log("Mod_Data.gameIndex 2: " + Mod_Data.gameIndex);
//                         SendToSever(Mod_Client_Data.messagetype.gamehistory);
//                     }
//                     else send704Running = false;
//                 }
//                 else
//                 {
//                     if (Mod_Data.bonusGameIndex < Mod_Data.localBonusRound) SendToSever(Mod_Client_Data.messagetype.gamehistory);
//                     else send704Running = false;
//                 }
//             }
//             yield return WaitSend704GameHistoryShortTime;
//         }

//         Mod_Data.severHistoryLock = false;
//     }

//     public void GetLocalGameRound()
//     {
//         newSramManager.LoadLocalGameRound(out Mod_Data.localRound);
//         newSramManager.LoadLocalBonusGameRound(out Mod_Data.localBonusRound);
//         //Debug.Log("Get Mod_Data.localRound: " + Mod_Data.localRound + " Mod_Data.localBonusRound: " + Mod_Data.localBonusRound);
//     }

//     public void SaveLocalGameRound()
//     {
//         //Debug.Log("Save Mod_Data.gameIndex: " + Mod_Data.gameIndex + "Mod_Data.bonusGameIndex: " + Mod_Data.bonusGameIndex);
//         newSramManager.SaveLocalGameRound(Mod_Data.gameIndex);
//         newSramManager.SaveLocalBonusGameRound(Mod_Data.bonusGameIndex);
//     }

//     public void SendMessages(string message)
//     {
//         if (SocketClient == null)
//         {
//             return;
//         }
//         if (String.IsNullOrWhiteSpace(message.Replace(',', ' '))) return;
//         try
//         {
//             NetworkStream stream = new NetworkStream(SocketClient);
//             if (stream.CanWrite)
//             {
//                 string clientMessage = message;
//                 byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage);
//                 stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
//                 Debug.Log("Client Send:" + clientMessage);
//             }
//         }
//         catch (SocketException socketException)
//         {
//             Debug.Log("Socket exception: " + socketException);
//         }
//     }


//     /*--------------------RETURN FROM SEVER-----------------------*/

//     public void Strhandle(string message)
//     {
//         //Debug.Log(message);
//         severDelay_Time = 0;
//         try
//         {
//             if (String.IsNullOrWhiteSpace(message)) return;
//             string[] words = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

//             for (int i = 0; i < words.Length; i++)
//             {
//                 string[] singlesign = words[i].Split(',');

//                 switch (singlesign[0]) // If singlesign[1] get "0" then Fail  
//                 {
//                     case "701":  //插入會員卡成功與否
//                                  //singlesign[1]; //會員編號  1:Fail  成功: 2會員編號,3會員姓名,4暱稱,5身分證,6密碼,7性別,8電話,9生日,10地址,11職業,12店家代號,13VIP,14備註,15黑名單,16員工編號,17會員狀態,18累積點數,19註冊時間,20刪除時間,21刪除標記,22卡片狀態
//                                  //範例: 701,5,小白,白,A456541254,asd1234,0,0915845211,2019-09-27 00:00:00,不告訴你地址,魔術師,apl,2,備註,0,photo,5566,1,2019-09-27 16:26:26,,0
//                         if (singlesign.Length > 2 && !String.IsNullOrWhiteSpace(singlesign[1]) && singlesign[1] != "0")
//                         {
//                             //Debug.Log("scceesful login   ID: " + Mod_Client_Data.cardID);
//                             Mod_Data.memberID = singlesign[1].ToString();
//                             Mod_Client_Data.handPaymemberID = Mod_Data.memberID;
//                             //Debug.Log("MemberID: " + singlesign[1].ToString());
//                             Mod_Data.cardPassword = singlesign[2];
//                             Mod_Data.memberBirthday = singlesign[3];
//                             Mod_Data.memberlevel = singlesign[4];
//                             Mod_Data.memberRakebackPoint = float.Parse(singlesign[6].ToString());
//                             Mod_Data.bonusPoints = double.Parse(singlesign[7]);
//                             Mod_Data.severTime = singlesign[8];
//                             //Debug.Log("memberBirthday: " + Mod_Data.memberBirthday.Substring(5, 2) + " severTime: " + Mod_Data.severTime.Substring(5, 2));
//                             if (Mod_Data.memberBirthday.Substring(5, 2).Contains(Mod_Data.severTime.Substring(5, 2))) Mod_Data.memberlevel = Mod_Data.memberlevel == "1" ? "2" : Mod_Data.memberlevel == "2" ? "4" : Mod_Data.memberlevel;
//                             else Mod_Data.memberlevel = Mod_Data.memberlevel == "1" ? "1" : Mod_Data.memberlevel == "2" ? "3" : Mod_Data.memberlevel;
//                             if (Mod_Data.cardneeded == 1) Mod_Data.memberLcok = false;
//                             SendMachineEvent = true;
//                             mod_UIController.text_BonusPoints.text = Mod_Data.bonusPoints <= 0 ? null : Mod_Data.bonusPoints.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty);
//                             MemberLogIn = false;
//                             MemberLogInHard = false;
//                         }
//                         else if (singlesign[1] == "0")
//                         {
//                             Mod_Data.cardID = " ";
//                             MemberLogIn = false;
//                             MemberLogInHard = false;
//                         }
//                         //Debug.Log("Mod_Client_Data.handPaymemberID 701: " + Mod_Client_Data.handPaymemberID);
//                         ClearHandPayMemberID = false;
//                         break;
//                     case "702":  //插入票紙驗證與否 1:Fail,成功 2票種,3金額,4票號
//                                  //singlesign[1]; //是否為合法票  0:Fail 1:Sucess
//                         //Debug.Log(singlesign[1]);
//                         BillAcceptorSettingData.TicketValue = "0";
//                         if (singlesign[1] == "1")
//                         {
//                             BillAcceptorSettingData.TicketValue = singlesign[3];
//                             Mod_Client_Data.ticketInAmount = BillAcceptorSettingData.TicketValue;
//                             BillAcceptorSettingData.TicketInWaitSever = 1;
//                         }
//                         else
//                         {
//                             BillAcceptorSettingData.TicketInWaitSever = 0;
//                         }
//                         break;
//                     case "703":  //出票成功與否 1:Fail,成功: 2票額,3彩票編號,4有效日期  範例 : 703,500,02-8514-0738-8500-2754,2019-12-03 13:54:16
//                         Gen2_Data.TicketOutValue = 0;
//                         if (singlesign[1] == "1")
//                         {
//                             Gen2_Data.TicketOutValue = double.Parse(singlesign[2]); //票額
//                             Mod_Data.serial = singlesign[3]; //彩票編號
//                             Mod_Data.serial2 = singlesign[3]; //彩票編號
//                             Mod_Data.barcode = Mod_Data.serial.Replace("-", string.Empty);//條碼
//                             Mod_Data.ticketExpirationDate = singlesign[4];//票有效期限
//                             string[] ticketOutTime = singlesign[5].Split(' ');
//                             Mod_Data.YYMMDD = ticketOutTime[0]; //出票時間
//                             Mod_Data.HHMMSS = ticketOutTime[1];
//                             Gen2_Data.TicketOutWaitSever = 1;
//                             //Debug.Log("Ticket Info: " + " Gen2_Data.TicketOutValue: " + Gen2_Data.TicketOutValue + " Mod_Data.serial: " + Mod_Data.serial + " Mod_Data.barcode: " + Mod_Data.barcode + " Mod_Data.YYMMDD: " + Mod_Data.YYMMDD + " Mod_Data.HHMMSS: " + Mod_Data.HHMMSS + " OrigTime: " + singlesign[4] + " ticketExpirationDate: " + Mod_Data.ticketExpirationDate);
//                         }
//                         else
//                         {
//                             Gen2_Data.TicketOutWaitSever = 0;
//                         }
//                         break;
//                     case "704":  //遊戲紀錄儲存成功與否 0:失敗 1:成功
//                         if (singlesign[1] == "1")
//                         {
//                             send704Running = false;
//                             //Debug.Log("Game saved");
//                         }
//                         break;
//                     case "705":  //機台初始值設置數值 *** 0:失敗/ 2遊戲編號,3機台編號,4遊戲語言,5設定RTP,6denom,7鈔機開啟0關1開,8可入鈔額設定,9可入鈔幣別,10票機開啟0關1開,11可入票上限,12可出票上限,13贏分上限,14機台分數上限,15押注倍數,16兌分比17是否需要會員卡18是否吃鈔19是否吃票20最後一筆遊戲場次編號
//                                  //範例:705,1,1,2,95,1,1,200,1,1,3000,2000,5000,6000,10,0.5
//                         //Debug.Log("705:" + singlesign[1]);
//                         if (singlesign[1] != "0" && singlesign[2] != null)
//                         {
//                             Mod_Data.machineInit = true;
//                             Mod_Data.gameID = singlesign[1];
//                             Mod_Data.machineID = singlesign[2];
//                             Mod_Data.language = (Mod_Data.languageList)Convert.ToInt32(singlesign[3]);
//                             RTPSstting = singlesign[4].ToString();
//                             //Mod_Data.RTPsetting = RTPSstting;
//                             DenomSetting = singlesign[5].ToString();
//                             Mod_Data.denomsetting = DenomSetting;
//                             BillAcceptorSettingData.BillOpen = singlesign[6] == "1" ? true : false;
//                             BillAcceptorSettingData.BillEnableWaitServer = singlesign[6] == "1" ? 1 : 0;
//                             Mod_Data.Billamountsetting = singlesign[7].ToString();
//                             Mod_Data.Billcurrency = singlesign[8].ToString();
//                             Gen2_Data.GenOpen = singlesign[9] == "1" ? true : false;
//                             Gen2_Data.GenEnableWaitServer = singlesign[9] == "1" ? 1 : 0;
//                             BillAcceptorSettingData.CashOrTicketInMaxLimit = int.Parse(singlesign[10].ToString());
//                             Gen2_Data.TicketOutMaxLimit = int.Parse(singlesign[11].ToString());
//                             Mod_Data.maxWin = int.Parse(singlesign[12].ToString());
//                             BackEnd_Data.maxWin = (int)Mod_Data.maxWin;
//                             backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxWin, true);
//                             Mod_Data.maxCredit = int.Parse(singlesign[13].ToString());
//                             BackEnd_Data.maxCredit = (int)Mod_Data.maxCredit;
//                             backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxCredit, true);
//                             Mod_Data.maxOdds = int.Parse(singlesign[14].ToString());
//                             Mod_Data.rakeback = float.Parse(singlesign[15].ToString());
//                             CardNeed = int.Parse(singlesign[16].ToString());
//                             Mod_Data.cardneeded = CardNeed;//是否需要會員卡
//                             Mod_Data.memberLcok = Mod_Data.cardneeded == 1 && string.IsNullOrWhiteSpace(Mod_Data.cardID) ? true : false;
//                             //Debug.Log((singlesign[17].ToString() == "0") + "," + (singlesign[18].ToString() == "1"));
//                             //Debug.Log("success------------------------------------------------------"
//                           //+ "Mod_Data.gameID:" + Mod_Data.gameID + "Mod_Data.machineID:" + Mod_Data.machineID + "Mod_Data.language:" + (int)Mod_Data.language
//                           //+ "RTP" + BackEnd_Data.RTPwinRate[0] + "." + BackEnd_Data.RTPwinRate[1] + "." + BackEnd_Data.RTPwinRate[2] + "." + BackEnd_Data.RTPwinRate[3] + "." + BackEnd_Data.RTPwinRate[4] + "." + BackEnd_Data.RTPwinRate[5] + "." + BackEnd_Data.RTPwinRate[6] + "." + BackEnd_Data.RTPwinRate[7] + "." + BackEnd_Data.RTPwinRate[8]);
//                             billAcceptorController.BillOpenClose = true;
//                             BillAcceptorSettingData.TicketCashOpenClose = singlesign[17] == "1" ? singlesign[18] == "1" ? 2 : 0 : singlesign[18] == "1" ? 1 : BillAcceptorSettingData.TicketCashOpenClose;
//                             Mod_Data.gameIndex = int.Parse(singlesign[19].ToString());
//                             Mod_Data.bonusGameIndex = int.Parse(singlesign[20].ToString());
//                             Mod_ChangePonit.CloseMemberPanel = true;
//                             //Debug.Log("Mod_Data.gameIndex: " + Mod_Data.gameIndex);
//                         }
//                         break;
//                     case "706":  //收到入鈔成功與否 0:失敗	1:成功
//                         if (singlesign[1] == "1")
//                         {
//                             JCM_Bill_Acceptor.cashin_waitServer = false;
//                             BillAcceptorSettingData.TicketInWaitSever = 1;
//                         }
//                         //Debug.Log("706: " + singlesign[1]);
//                         break;
//                     case "707":  //收到拔鈔成功與否 0:失敗	1:成功
//                         if (singlesign[1] == "1") BillClearWaitSever = false;
//                         //Debug.Log("707: " + singlesign[1]);
//                         break;
//                     case "708":  //收到機台事件與否 0:失敗	1:成功
//                         if (singlesign[1] == "1") MachineEventWaitServer = false;
//                         break;
//                     case "709":
//                         SendToSever(Mod_Client_Data.messagetype.machinelive);
//                         Mod_Data.serverLock = false;
//                         break;
//                     case "802":  //收到開分指令 ***
//                                  //singlesign[1]; //開分分數
//                         if (singlesign[2] == Mod_Data.machineID && int.Parse(singlesign[1]) != 0)
//                         {
//                             openClearScore = int.Parse(singlesign[1]);
//                             OpenClear = true;
//                             mod_UIController.UpdateScore();
//                         }
//                         //Debug.Log("Sever開分 " + singlesign[1] + singlesign[2]);
//                         break;
//                     case "710":  //連線 成功: 機台編號	失敗:0
//                         if (singlesign[1] != "0" && singlesign[1] != "格式錯誤")
//                         {
//                             Mod_Data.machineID = singlesign[1];
//                             Mod_Data.ticketNumber = singlesign[1];
//                             Mod_Data.machineNumbear = singlesign[1];
//                             Mod_Data.machineIDLock = false;
//                             Mod_Data.serverLock = false;
//                             SendMachineEvent = true;
//                             //Debug.Log("我的機台號碼 " + singlesign[1]);
//                         }
//                         break;
//                     case "711":  //手付-機台通知 0:失敗  1:成功
//                         if (singlesign[1] == "1") WaitServerHandPay = false;
//                         //Debug.Log(singlesign[1]);
//                         break;
//                     case "712":  //通知機台手付處理完成可以 reset 712 手付 reset 訊號
//                         WaitStationHandPay = false;
//                         break;
//                     case "713":  //通知機台被server斷線 713  重新連線回傳	710
//                         Mod_Client_Data.serverdisconnect = true;
//                         Mod_Data.machineIDLock = true;
//                         Mod_Data.serverLock = true;
//                         //Debug.Log("713: " + " Mod_Client_Data.serverdisconnect: " + Mod_Client_Data.serverdisconnect + " Mod_Data.machineIDLock: " + Mod_Data.machineIDLock + "  Mod_Data.serverLock: " + Mod_Data.serverLock);
//                         break;
//                     case "714":  //銷/補點 714
//                         //Debug.Log(singlesign[1]);
//                         break;
//                     case "715":  //登出 715
//                         if (singlesign[1] == "1" || singlesign[1] == "0")
//                         {
//                             //Debug.Log("ID: " + Mod_Client_Data.cardID + " 登出成功 " + " Mod_Client_Data.MemberCardOut: " + Mod_Client_Data.MemberCardOut);
//                             if (Mod_Data.cardneeded == 1 && !Mod_Client_Data.MemberCardOut) Mod_Data.memberLcok = true;
//                             Mod_Client_Data.cardID = " ";
//                             Mod_Data.cardID = " ";
//                             Mod_Data.memberID = " ";
//                             Mod_Data.cardPassword = " ";
//                             Mod_Data.memberlevel = " ";
//                             Mod_Data.bonusPoints = 0;
//                             mod_UIController.text_BonusPoints.text = null;
//                             MemberLogOutWait = false;
//                             ClearHandPayMemberID = true;
//                         }
//                         //Debug.Log("Mod_Client_Data.handPaymemberID 715: " + Mod_Client_Data.handPaymemberID);
//                         break;
//                     case "716":  //兌分
//                         if (singlesign[1] == "1")
//                         {
//                             //Debug.Log("Mod_Data.changePoint: " + Mod_Data.changePoint + " Mod_Data.memberRakebackPoint: " + Mod_Data.memberRakebackPoint);
//                             mod_OpenClearPoint.OpenPointFunction(Mathf.FloorToInt(Mod_Data.changePoint));
//                             Mod_Data.changePointLock = false;
//                             Mod_Data.changePoint = 0;
//                             Mod_Data.memberRakebackPoint = float.Parse(singlesign[2].ToString());
//                             m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
//                         }
//                         else
//                         {
//                             Mod_Data.changePointLock = false;
//                             Mod_Data.changePoint = 0;
//                         }
//                         break;
//                     case "717":  //出票成功
//                         if (singlesign[1] == "1") Gen2_Data.TicketOutWaitSever = 1;
//                         break;
//                     case "721":  //改為有效票成功
//                         if (singlesign[1] == "1") BillAcceptorSettingData.TicketInWaitSever = 1;
//                         //Debug.Log("721: " + singlesign[1]);
//                         break;
//                     case "804":
//                         if (singlesign[1] != "0" && singlesign[2] != null)
//                         {
//                             Mod_Data.machineInit = true;
//                             Mod_Data.gameID = singlesign[1];
//                             Mod_Data.machineID = singlesign[2];
//                             Mod_Data.language = (Mod_Data.languageList)Convert.ToInt32(singlesign[3]);
//                             RTPSstting = singlesign[4].ToString();
//                             //Mod_Data.RTPsetting = RTPSstting;
//                             DenomSetting = singlesign[5].ToString();
//                             Mod_Data.denomsetting = DenomSetting;
//                             BillAcceptorSettingData.BillOpen = singlesign[6] == "1" ? true : false;
//                             if (BillAcceptorSettingData.BillOpen == true ? !BillAcceptorSettingData.BillOpenClose : BillAcceptorSettingData.BillOpenClose) billAcceptorController.BillOpenCloseButton();
//                             Mod_Data.Billamountsetting = singlesign[7].ToString();
//                             Mod_Data.Billcurrency = singlesign[8].ToString();
//                             Gen2_Data.GenOpen = singlesign[9] == "1" ? true : false;
//                             if (Gen2_Data.GenOpen == true ? !gen2_Status.keepConvert : gen2_Status.keepConvert) gen2_Status.OpenOrCloserGen2();
//                             BillAcceptorSettingData.CashOrTicketInMaxLimit = int.Parse(singlesign[10].ToString());
//                             Gen2_Data.TicketOutMaxLimit = int.Parse(singlesign[11].ToString());
//                             Mod_Data.maxWin = int.Parse(singlesign[12].ToString());
//                             BackEnd_Data.maxWin = (int)Mod_Data.maxWin;
//                             backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxWin, true);
//                             Mod_Data.maxCredit = int.Parse(singlesign[13].ToString());
//                             BackEnd_Data.maxCredit = (int)Mod_Data.maxCredit;
//                             backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxCredit, true);
//                             Mod_Data.maxOdds = int.Parse(singlesign[14].ToString());
//                             Mod_Data.rakeback = float.Parse(singlesign[15].ToString());
//                             CardNeed = int.Parse(singlesign[16].ToString());
//                             Mod_Data.cardneeded = CardNeed;//是否需要會員卡
//                             Mod_Data.memberLcok = Mod_Data.cardneeded == 1 && string.IsNullOrWhiteSpace(Mod_Data.cardID) ? true : false;
//                             Mod_ChangePonit.CloseMemberPanel = true;
//                             billAcceptorController.BillOpenClose = true;
//                             BillAcceptorSettingData.TicketCashOpenClose = singlesign[17] == "1" ? singlesign[18] == "1" ? 2 : 0 : singlesign[18] == "1" ? 1 : BillAcceptorSettingData.TicketCashOpenClose;
//                             if (Mod_Data.cardneeded == 0 && !string.IsNullOrWhiteSpace(Mod_Data.cardID) && !MemberLogOutWait && !SendMemberLogOut) SendMemberLogOut = true;
//                         }
//                         break;
//                     default:
//                         Debug.Log("msg error");
//                         break;
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.Log("Exception: " + ex);
//         }
//     }

//     /*------------------------SEND TO SEVER------------------------*/
//     public void SendToSever(Mod_Client_Data.messagetype messagetype)
//     {
//         if (Mod_Data.serverLock && messagetype != Mod_Client_Data.messagetype.connect) return;
//         //Debug.Log("Send To Server: " + messagetype);
//         UpdateGameData();
//         try
//         {
//             switch (messagetype)
//             {
//                 case Mod_Client_Data.messagetype.memberlogin:             //                 701會員登入
//                     SendMessages((int)Mod_Client_Data.messagetype.memberlogin //+ "," + Mod_Client_Data.memberID 
//                                                                              + "," + Mod_Client_Data.cardID
//                                                                              + "," + " "
//                                                                              + "," + Mod_Data.machineID
//                                                                              + ";"
//                                                                              );
//                     break;
//                 case Mod_Client_Data.messagetype.ticketin:                //                 702入票           ***  
//                     SendMessages((int)Mod_Client_Data.messagetype.ticketin + "," + Mod_Client_Data.ticketNum
//                                                                            + "," + Mod_Data.machineID
//                                                                            + ";"
//                                                                           );
//                     break;
//                 case Mod_Client_Data.messagetype.ticketout:                //                 703出票            *** 
//                     SendMessages((int)Mod_Client_Data.messagetype.ticketout + "," + Mod_Client_Data.ticketType
//                                                                            + "," + Mod_Client_Data.ticketAmount
//                                                                            + "," + Mod_Client_Data.memberID
//                                                                            + "," + Mod_Data.machineID
//                                                                            + ";"
//                                                                            );
//                     break;
//                 case Mod_Client_Data.messagetype.gamehistory:             //                 704遊戲紀錄
//                     Mod_Client_Data.bonus = Mod_Data.getBonus == true ? "1" : "0";
//                     SendMessages((int)Mod_Client_Data.messagetype.gamehistory + "," + Mod_Client_Data.gameIndex
//                                                                             + "," + Mod_Client_Data.bet
//                                                                             + "," + Mod_Client_Data.memberID
//                                                                             + "," + Mod_Client_Data.winscroe
//                                                                             + "," + Mod_Client_Data.credit
//                                                                             + "," + Mod_Client_Data.bonus
//                                                                             + "," + Mod_Client_Data.gameID
//                                                                             + "," + Mod_Client_Data.betOdds
//                                                                             + "," + Mod_Client_Data.gameRNG
//                                                                             + "," + Mod_Client_Data.bonusCurrent
//                                                                             + "," + Mod_Client_Data.bonusTotalgmae
//                                                                             + "," + Mod_Client_Data.rtpset
//                                                                             + "," + Mod_Client_Data.denom
//                                                                             + "," + Mod_Client_Data.specialrules
//                                                                              + "," + Mod_Client_Data.cardID
//                                                                              + "," + Mod_Client_Data.memberRakebackPointOri
//                                                                              + "," + Mod_Client_Data.getRakebackPoint
//                                                                              + "," + Mod_Client_Data.memberRakebackPointAfter
//                                                                              + "," + Mod_Data.machineID
//                                                                              + "," + Mod_Client_Data.bonusPoints
//                                                                              + ";"
//                                                                             );
//                     break;
//                 case Mod_Client_Data.messagetype.gameset:                //                705遊戲初始值設定
//                     SendMessages((int)Mod_Client_Data.messagetype.gameset + "," + Mod_Data.machineID
//                                                                           + ";"
//                                                                           );
//                     break;
//                 case Mod_Client_Data.messagetype.billin:                 //                706入鈔
//                     SendMessages((int)Mod_Client_Data.messagetype.billin + "," + Mod_Client_Data.billAmount
//                                                                          + "," + Mod_Data.machineID
//                                                                          + ";"
//                                                                          );
//                     break;
//                 case Mod_Client_Data.messagetype.billclear:              //                707清鈔
//                     SendMessages((int)Mod_Client_Data.messagetype.billclear + "," + Mod_Data.machineID
//                                                                             + ";"
//                                                                             );
//                     break;
//                 case Mod_Client_Data.messagetype.machineevent:           //                708機台事件
//                     SendMessages((int)Mod_Client_Data.messagetype.machineevent + "," + Mod_Client_Data.memberID
//                                                                               + "," + Mod_Client_Data.machineerror
//                                                                               + "," + Mod_Data.machineID
//                                                                               + "," + Mod_Data.memberlevel
//                                                                               + ";"
//                                                                               );
//                     break;
//                 case Mod_Client_Data.messagetype.machinelive:           //                 709 machine live
//                     SendMessages((int)Mod_Client_Data.messagetype.machinelive + ""
//                                                                               + ";"
//                                                                               );

//                     break;
//                 case Mod_Client_Data.messagetype.connect:                //                710連線
//                     SendMessages((int)Mod_Client_Data.messagetype.connect + ""
//                                                                           + ";"
//                                                                           );
//                     break;
//                 case Mod_Client_Data.messagetype.handpay:                //                711手付 
//                     SendMessages((int)Mod_Client_Data.messagetype.handpay + "," + Mod_Client_Data.handPaymemberID
//                                                                           + "," + (newSramManager.LoadHandPayStatus() == 1 ? Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty)) : String.IsNullOrWhiteSpace(Mod_Client_Data.ticketInAmount) ? "0" : Mod_Client_Data.ticketInAmount)
//                                                                           + "," + Mod_Data.machineID
//                                                                           + "," + (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3 ? newSramManager.LoadTicketSerial() : String.IsNullOrWhiteSpace(Mod_Client_Data.ticketNum) ? " " : Mod_Client_Data.ticketNum)
//                                                                           + "," + (newSramManager.LoadHandPayStatus() == 2 || newSramManager.LoadHandPayStatus() == 3 ? "1" : "0")
//                                                                           + ";"
//                                                                           );
//                     break;
//                 case Mod_Client_Data.messagetype.handpayreset:           //                712手付重製reset
//                     SendMessages((int)Mod_Client_Data.messagetype.handpayreset + ""
//                                                                                + ";"
//                                                                                );
//                     break;
//                 case Mod_Client_Data.messagetype.memberlogout:           //                715玩家登出
//                     SendMessages((int)Mod_Client_Data.messagetype.memberlogout + "," + Mod_Client_Data.cardID
//                                                                                + "," + Mod_Data.machineID
//                                                                                + ";"
//                                                                                );
//                     if (Mod_Data.cardneeded == 1 && !Mod_Client_Data.MemberCardOut) Mod_Data.memberLcok = true;
//                     break;
//                 case Mod_Client_Data.messagetype.exchangePoints:           //                716兌分
//                     SendMessages((int)Mod_Client_Data.messagetype.exchangePoints + "," + Mod_Client_Data.memberID
//                                                                                  + "," + Mod_Client_Data.cardID
//                                                                                  + "," + Mod_Data.memberRakebackPoint
//                                                                                  + "," + Mod_Data.changePoint
//                                                                                  + "," + (Mod_Data.memberRakebackPoint - (float)Mod_Data.changePoint)
//                                                                                  + "," + Mod_Data.cardPassword
//                                                                                  + "," + Mod_Data.machineID
//                                                                                  + ";"
//                                                                           );
//                     //Debug.Log(" memberRakebackPoint: " + Mod_Data.memberRakebackPoint + " changePoint: " + Mod_Data.changePoint);
//                     break;
//                 case Mod_Client_Data.messagetype.ticketoutsuccess:           //                717出票成功
//                     SendMessages((int)Mod_Client_Data.messagetype.ticketoutsuccess + "," + Mod_Client_Data.ticketNum.Substring(3)
//                                                                                    + "," + Mod_Data.machineID
//                                                                                    + "," + Mod_Client_Data.ticketAmount
//                                                                                    + ";"
//                                                                           );
//                     break;
//                 case Mod_Client_Data.messagetype.ticketinsucess:           //                721入票有效票改為無效票
//                     SendMessages((int)Mod_Client_Data.messagetype.ticketinsucess + "," + Mod_Client_Data.ticketNum
//                                                                                         + "," + Mod_Data.machineID
//                                                                                         + ";"
//                                                                           );
//                     break;
//                 default:
//                     Debug.Log("cant get funtion");
//                     break;
//             }
//         }
//         catch (Exception ex)
//         {
//             //Mod_Client_Data.serverdisconnect = true;
//             Debug.Log("SendToSever Exception: " + ex);
//         }
//     }

//     public void SendToSever(Mod_Client_Data.messagetype messagetype, int value)
//     {
//         if (Mod_Data.serverLock && messagetype != Mod_Client_Data.messagetype.connect) return;
//         Debug.Log("Send To Server: " + messagetype);
//         UpdateGameData();
//         try
//         {
//             switch (messagetype)
//             {

//                 case Mod_Client_Data.messagetype.pointIn:
//                     SendMessages((int)Mod_Client_Data.messagetype.pointIn + "," + Mod_Data.machineID
//                                                                                + "," + Mod_Client_Data.memberID
//                                                                                 + "," + value
//                                                                                 );
//                     break;
//                 case Mod_Client_Data.messagetype.pointOut:
//                     SendMessages((int)Mod_Client_Data.messagetype.pointOut + "," + Mod_Data.machineID
//                                                                               + "," + Mod_Client_Data.memberID
//                                                                                + "," + value
//                                                                                );
//                     break;
//                 default:
//                     Debug.Log("cant get funtion");
//                     break;
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.Log("SendToSever Exception: " + ex);
//         }
//     }

//     private void OnApplicationQuit()
//     {
//         close();

//     }
//     private void OnDestroy()
//     {

//         close();
//     }

//     void close()
//     {
//         if (clientReceiveThread != null)
//         {
//             clientReceiveThread.Interrupt();
//             clientReceiveThread.Abort();
//         }
//         if (null != socketConnection)
//         {
//             socketConnection.Close();
//         }

//         if (thread != null)
//         {
//             thread.Interrupt();
//             thread.Abort();
//         }
//         if (ServerTread != null)
//         {
//             ServerTread.Interrupt();
//             ServerTread.Abort();
//         }
//         try
//         {
//             SocketClient.Shutdown(SocketShutdown.Both);
//         }
//         finally
//         {
//             SocketClient.Close();
//         }

//     }

//     void UpdateGameData()
//     {
//         //遊戲相關
//         Mod_Client_Data.gameIndex = Mod_Data.gameIndex;
//         Mod_Client_Data.bet = (Mod_Data.Bet * Mod_Data.odds * Mod_Data.Denom).ToString();                  //押注  
//         if (!Mod_Data.BonusSwitch) Mod_Client_Data.winscroe = (Mod_Data.Pay * Mod_Data.Denom).ToString();          //贏分
//         else Mod_Client_Data.winscroe = (Mod_Data.Pay * Mod_Data.Denom).ToString();          //贏分
//         if (!Mod_Data.BonusSwitch) Mod_Client_Data.credit = Mod_Data.credit.ToString();                     //彩分
//         else Mod_Client_Data.credit = (Mod_Data.credit + (Mod_Data.Win - Mod_Data.Pay)).ToString();
//         Mod_Client_Data.bonusPoints = Mod_Data.bonusPoints.ToString(); //贈分
//         Mod_Client_Data.bonus = Mod_Data.BonusSwitch.ToString();                     //遊戲是否bonus
//         Mod_Client_Data.gameID = Mod_Data.gameID;                  //遊戲編號
//         Mod_Client_Data.betOdds = Mod_Data.odds.ToString();                    //押注比例 
//         Mod_Client_Data.gameRNG = Mod_Data.RNG[0].ToString() + "." + Mod_Data.RNG[1].ToString() + "." + Mod_Data.RNG[2].ToString() + "." + Mod_Data.RNG[3].ToString() + "." + Mod_Data.RNG[4].ToString();                //遊戲滾輪值
//         Mod_Client_Data.bonusCurrent = Mod_Data.BonusIsPlayedCount.ToString();            //當前bonus場次(第幾場)
//         Mod_Client_Data.bonusTotalgmae = Mod_Data.BonusCount.ToString();          //Bonus遊戲總次數
//         Mod_Client_Data.rtpset = Mod_Data.RTPsetting.ToString();                  //設置的rtp
//         Mod_Client_Data.denom = Mod_Data.Denom.ToString();                      //比率
//         Mod_Client_Data.specialrules = Mod_Data.BonusSpecialTimes.ToString();               //特定倍率
//                                                                                             //會員相關
//                                                                                             // Mod_Client_Data.cardPassword = Mod_Data.cardPassword;               //卡片密碼
//                                                                                             //票相關
//         Mod_Client_Data.ticketNum = Mod_Data.serial;                  //票號
//         Mod_Client_Data.ticketType = "04";          //票種  01:正式票 02:試玩票
//         Mod_Client_Data.ticketAmount = Mod_Client_Data.CreditToStrnigCheck(Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture).Replace(",", string.Empty));              //票額  
//         Mod_Client_Data.ticketValidate = "2000/01/01";             //票有效期限 
//                                                                    //鈔相關
//         Mod_Client_Data.billAmount = Mod_Data.cash.ToString();                 //鈔票面額
//         Mod_Client_Data.machineerror = Mod_Data.machineerror;

//         if (Mod_Data.cardneeded == 1)
//         {
//             Mod_Client_Data.memberID = Mod_Data.memberID;                   //會員編號
//             Mod_Client_Data.cardID = Mod_Data.cardID;                     //卡片編號
//             Mod_Client_Data.memberlevel = Mod_Data.memberlevel;
//             Mod_Client_Data.memberRakebackPointOri = (Mod_Data.memberRakebackPoint - Mod_Data.getRakebackPoint) > 0 ? Mod_Data.memberRakebackPoint - Mod_Data.getRakebackPoint : 0;
//             Mod_Client_Data.getRakebackPoint = Mod_Data.getRakebackPoint;
//             Mod_Client_Data.memberRakebackPointAfter = Mod_Data.memberRakebackPoint;
//         }
//     }

//     //是否要會員卡,回傳值不對則不設定
//     int _cardNeed;
//     int CardNeed
//     {
//         get { return _cardNeed; }
//         set
//         {
//             if (_cardNeed == 1 || _cardNeed == 0)
//             {
//                 _cardNeed = value;
//             }
//             else
//             {
//                 _cardNeed = Mod_Data.cardneeded;
//             }
//         }
//     }

//     //改變RTP,如果有新增這裡要新增
//     string _RTPsetting;
//     string RTPSstting
//     {
//         get { return _RTPsetting; }
//         set
//         {
//             _RTPsetting = value;
//             Debug.Log(_RTPsetting);
//             bool dataError = false;
//             //if (_RTPsetting == 0|| _RTPsetting == 1 || _RTPsetting == 2)
//             //{
//             //    _RTPsetting = value;
//             //}
//             //else
//             //{
//             //    _RTPsetting = Mod_Data.RTPsetting;
//             //}
//             if (_RTPsetting.Length == 9)
//             {
//                 for (int i = 0; i < 9; i++)
//                 {
//                     if (_RTPsetting[i] == '0')
//                     {
//                         BackEnd_Data.RTPwinRate[i] = 0;
//                     }
//                     else if (_RTPsetting[i] == '1')
//                     {
//                         BackEnd_Data.RTPwinRate[i] = 1;
//                     }
//                     else if (_RTPsetting[i] == '2')
//                     {
//                         BackEnd_Data.RTPwinRate[i] = 2;
//                     }
//                     else
//                     {
//                         dataError = true;
//                     }
//                 }
//                 if (!dataError)
//                 {
//                     backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, true);

//                     if (Mod_Data.Denom == 0.01) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[8]; }
//                     else if (Mod_Data.Denom == 0.02) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[7]; }
//                     else if (Mod_Data.Denom == 0.025) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[6]; }
//                     else if (Mod_Data.Denom == 0.05) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[5]; }
//                     else if (Mod_Data.Denom == 0.1) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[4]; }
//                     else if (Mod_Data.Denom == 0.25) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[3]; }
//                     else if (Mod_Data.Denom == 0.5) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[2]; }
//                     else if (Mod_Data.Denom == 1) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[1]; }
//                     else if (Mod_Data.Denom == 2.5) { Mod_Data.RTPsetting = BackEnd_Data.RTPwinRate[0]; }
//                 }
//             }
//         }
//     }

//     string _denomSetting = "";
//     string DenomSetting
//     {
//         get { return _denomSetting; }
//         set
//         {
//             _denomSetting = value;
//             Debug.Log(_denomSetting.Length);
//             if (_denomSetting.Length == 9)
//             {
//                 for (int i = 0; i < 9; i++)
//                 {
//                     if (_denomSetting[i] == '0')
//                     {
//                         Mod_Data.denomOpenArray[i] = false;
//                         BackEnd_Data.denomArray[i] = false;
//                     }
//                     else if (_denomSetting[i] == '1')
//                     {
//                         Mod_Data.denomOpenArray[i] = true;
//                         BackEnd_Data.denomArray[i] = true;
//                     }
//                 }
//                 backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.denomArraySelect, true);
//             }

//         }
//     }
//     #endregion
// #endif
// }

// public static class Mod_Client_Data
// {
// #if Server
//     #region Server

//     public static machineerrorlist machineerrorlists;
//     public static memberlevellist memberlevellists;
//     public static string machineerror = "0";
//     public static string memberlevel;
//     public enum machineerrorlist
//     {
//         normal = 0,
//         clearbill = 1,
//         service = 2,
//         dooropen = 3,
//         machinerror = 4,
//         handpay = 5,
//         ticketerror = 6
//     }
//     public enum memberlevellist
//     {
//         normalvip = 0,
//         vipAA = 1,
//         vipAAbirthday = 2,
//     }
//     public enum messagetype
//     {
//         memberlogin = 701,
//         ticketin = 702,
//         ticketout = 703,
//         gamehistory = 704,
//         gameset = 705,
//         billin = 706,
//         billclear = 707,
//         machineevent = 708,
//         machinelive = 709,
//         connect = 710,
//         handpay = 711,
//         handpayreset = 712,
//         reconnent = 713,
//         rewardpoint = 714,
//         memberlogout = 715,
//         exchangePoints = 716,
//         ticketoutsuccess = 717,
//         ticketinsucess = 721,
//         pointIn = 722,
//         pointOut = 723,
//         creditin = 802
//     }

//     //遊戲相關
//     public static int gameIndex = Mod_Data.gameIndex; //遊戲場次
//     public static string bet = Mod_Data.Bet.ToString();                  //押注
//     public static string winscroe = Mod_Data.Win.ToString();                 //贏分
//     public static string credit = Mod_Data.credit.ToString();                     //彩分    
//     public static string bonusPoints = Mod_Data.bonusPoints.ToString();                     //彩分    
//     public static string bonus = Mod_Data.getBonus.ToString();                     //遊戲是否bonus
//     public static string gameID = Mod_Data.gameID;//Mod_Data.projectName;                  //遊戲編號
//     public static string betOdds = Mod_Data.odds.ToString();                    //押注比例 
//     public static string gameRNG = Mod_Data.RNG[0].ToString() + "." + Mod_Data.RNG[1].ToString() + "." + Mod_Data.RNG[2].ToString() + "." + Mod_Data.RNG[3].ToString() + "." + Mod_Data.RNG[4].ToString();                //遊戲滾輪值
//     public static string bonusCurrent = Mod_Data.BonusIsPlayedCount.ToString();            //當前bonus場次(第幾場)
//     public static string bonusTotalgmae = Mod_Data.BonusCount.ToString();          //Bonus遊戲總次數
//     public static string rtpset = Mod_Data.RTPsetting.ToString();                  //設置的rtp
//     public static string denom = Mod_Data.Denom.ToString();                      //比率
//     public static string specialrules = Mod_Data.BonusSpecialTimes.ToString();               //特定倍率
//     //會員相關
//     public static string memberID = Mod_Data.memberID;                   //會員編號
//     public static string cardID = Mod_Data.cardID;                     //卡片編號
//                                                                        // public static string cardPassword = Mod_Data.cardPassword;               //卡片密碼
//     public static string handPaymemberID = " ";                   //手付用暫時會員編號
//     public static float memberRakebackPointOri = Mod_Data.memberRakebackPoint - Mod_Data.getRakebackPoint;
//     public static float getRakebackPoint = Mod_Data.getRakebackPoint;
//     public static float memberRakebackPointAfter = Mod_Data.memberRakebackPoint;
//     public static bool MemberCardOut = false;
//     //票相關
//     public static string ticketNum;                  //票號
//     public static string ticketType = "04";          //票種  01:正式票 02:試玩票
//     public static string ticketAmount;               //票額  
//     public static string ticketValidate;             //票有效期限 
//     //入票相關
//     public static string ticketInAmount;                  //入票金額
//     //鈔相關
//     public static string billAmount = Mod_Data.cash.ToString();                 //鈔票面額

//     //錯誤報告  *error = true*
//     public static bool serverdisconnect = true;   //連線中斷

//     public static string CreditToStrnigCheck(string credit)
//     {
//         string[] credits = credit.Split('.');
//         if (decimal.Parse(credits[1]) > 0) return credit;
//         else return credits[0];
//     }
//     #endregion
// #endif
// }

#endregion