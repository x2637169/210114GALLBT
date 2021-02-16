using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO.Ports;
using System.Text;
using System;
using System.Runtime.InteropServices;

public static class Mod_Data
{
    public static languageList language;
    public enum languageList
    {
        CHT,
        EN
    }

    public static bool TestMode = false;//測試模式開關
    public static bool autoPlay = false;//自動玩開關
    public static bool BlankClick = false;//畫面按鈕
    public static bool inBaseSpin = false;
    public static Mod_State.STATE state;//目前狀態
    public static bool StartNotNormal = true;//是否斷電重開
    public static int StartNotNormalRTP = 99;//斷電重開上場舊RTP
    public static bool hasMonthCheck = false;
    public static bool openclearSet = false;

    public static JArray IconPays;//存Paytable
    public static VersionSelect Version;//硬體版本設定 PC 賽非 勝圖
    public static SlotVer slotVer;//滾輪介面設定 4*5 3*5
    public static SlotGameRule currentGameRule;
    public static LineRule lineRule;
    public static GameMode gameMode;
    public static BonusRule bonusRule;

    public static string projectName;
    public const int slotReelNum = 5;//滾輪數
    public const int slotRowNum = 3;//滾輪列數
    public const int linegame_LineCountOri = 50; //
    public static int linegame_LineCount = 50;//LineGame線數,新增規則也要新增

    public static double credit = 0;//彩分
    public static double bonusPoints = 0;//贈分

    public static double odds = 1;//壓注率
    public static double Denom = 1;//倍率
    public static double[] denomArray = new double[9] { 2.5, 1, 0.5, 0.25, 0.1, 0.05, 0.025, 0.02, 0.01 };//倍率選項
    public static bool[] denomOpenArray = new bool[9];
    public static int BetOri = 25;//遊戲規定的最低單場押注金額  更變遊戲這個要修改
    public static int Bet = 25;//單場押注金額
    public static double Win = 0;//總贏分(bonus內會累計)
    public static double Pay = 0;//該場贏得的總分(bonus內也是只計算該場)
    public static double BT_Rule_Pay = 0;//泡泡缸用跑換魚的分數
    public static double beforeGetBonusScore = 0;//紀錄獲得Bonus的分數
    public static int HistoryCount = 99;

    public static bool IOLock = false;
    public static bool winErrorLock = false;
    public static bool creditErrorLock = false;
    public static bool monthLock = false;
    public static bool billLock = false;
    public static bool memberLcok = false;
    public static bool serverLock = true;
    public static bool severHistoryLock = false;
    public static bool doorError = false;



    public static bool afterBonus = false;
    public static bool betLowCreditShowOnce = false;
    //----後臺設定值
    public static int maxOdds = 25;//最大押注倍數
    public static double maxCredit = 100000000;//最大彩分
    public static double maxWin = 100000000;//最大贏分
    public static int takeOutScoreNum;//洗分(分/次)
    public static int takeInScoreNum;//開分(分/次)

    //遊戲變數
    public static int BonusCount = 0;                           //Bonus總共場數
    public static int BonusIsPlayedCount = 0;                  //Bonus目前進行場數
    public static int BonusLimit = 99;                         //Bonus場數上限
    public static int getBonusCount = 0;                        //Bonus獲得的場數
    public static int BonusTicket = 0;//Bonus次數
    public static float BonusDelayTimes = 0;                         //Bonus等待停留時間 依照中線數判斷
    public static int BonusType = 0;//Bonus遊戲種類 選擇數學json
    public static int BonusSpecialTimes = 1;//Bonus特殊倍數
    public static int CH_BonusSpecialTime = 1;//楚河bonus特殊規則 進去中線數
    public static bool BonusSwitch = false;//是否在bonus內
    public static bool getBonus = false;//是否得到bonus
    public static int ScatterCount = 0; //Scatter中幾個
    public static bool getScatter = false; //是否有得分(3個以上得分)
    public static string Errortext;                              //錯誤訊息
    public static string Messagetext;                              //訊息

    //Sram變數

    //鈔機變數
    public static bool Cashin;
    public static int cash = 0;  //進入鈔機的錢
    public static int UBA_Enable = 0;  //1為開啟鈔機 0為關閉鈔機
    public static bool UBA_OpenSetting = true;  //鈔機 true開放設定 false關閉設定
    public static string UBAPortNumber = "COM2";
    public static bool ErrorrUBA;
    public static bool Ticketin = false;

    public static bool startBonus = false;
    public static bool transInAnimEnd = false;

    public static UIItemDatabase iconDatabase;//ScriptObject物件

    public static int[,] ReelMath = new int[slotReelNum, slotRowNum]; //讀取json 後的iconID位置陣列

    //票機
    public static bool printerError;//票機異常
    public static bool PrinterTicket;//出票成功
    public static bool MachineError = false;//機台異常
    public static bool billStackOpenerror = false;//清鈔 "false-Stack Close" "true-Stack Open"
    public static bool serviceLighterror = false;//"false-Light Off" "true-Light On"
    public static bool handpay = false;
    public static string printModel = "0"; //模板
    public static string count = "1"; //數量-更改無效
    public static string serial = "00-1234-5678-9123-4567"; //序號-1
    public static string ESTABLISHMENT = "WINLAND"; //機關-2
    public static string LOCATION = "Taiwan"; //地區-3
    public static string CITY_STATE_ZIP = "Taoyuan"; //縣市-4
    public static string serial2 = "00-1234-5678-9123-4567"; //序號2-右側-5
    public static string YYMMDD = "2020/01/20"; //年月日-6
    public static string HHMMSS = "08:31:15"; //時分秒-7
    public static string ticketNumber = "0111"; //票數-8
    public static string currency = ""; //金錢單位-9
    public static string noidea = " "; //目前不知道-10
    public static string voidDay = "60"; //幾天後票無效-11
    public static string machineNumbear = "0003-031"; //票機號碼-12
    public static string barcode = "432143214321432143"; //條碼-13
    public static string ticketExpirationDate = "2020/01/20 08:31:15";

    public static bool ReelSpeedChange = false;//聽牌改變滾輪速度
    public static int[] RNG = new int[5];//紀錄RNG
    public static bool reelAllStop = true;//滾輪全停
    public static bool runScore = false;//是否在跑分

    public enum VersionSelect
    {
        PC,
        Viewstd,
        Sephiroth
    }

    public enum SlotVer
    {
        reel3x5,
        reel4x5
    }
    public enum SlotGameRule
    {
        LineGame,
        WayGame
    }
    public enum LineRule
    {
        Line_25,
        Line_40,
        Line_50
    }
    public enum GameMode
    {
        BaseGame,
        BonusGame
    }
    public enum BonusRule
    {
        Reel1_2_3,
        Reel2_3_4,
        Reel1_2_3_4_5,
        ConsecutiveReel1_2_3_4_5
    }


    //控制LineGame變數

    public static int[][] linegame_Rule = new int[linegame_LineCount][];//LineGame 遊戲規則
    public static int[] linegame_WinIconQuantity = new int[linegame_LineCount];//每條線贏線數量
    public static bool[] linegame_HasWinline = new bool[linegame_LineCount];//第幾條線有連線
    public static int[] linegame_WinIconID = new int[linegame_LineCount];//每條線贏線的IconID
    public static int[] linegame_EachLinePay = new int[linegame_LineCount];//每條線贏線的贏分
    public static bool[,] linegame_BonusPos = new bool[slotReelNum, slotRowNum];//Bonus位置
                                                                                //LineGame贏分

    //勝圖通訊變數 X0X0V0Q802b214886a8071a6bf6194b2189dda104d9b424nePFjH&iwk5eZR*@neJl
    public static string memberID = " ", cardID = "1", cardPassword = " ";
    public static string memberlevel = " ";//會員等級
    public static string memberBirthday = " ";//會員生日
    public static string severTime = " ";//伺服器時間
    public static float memberRakebackPoint;//消補點點數
    public static float getRakebackPoint;//單場消補點
    public static int changePoint;
    public static bool changePointLock = false;
    //機台設定
    public static bool machineInit = false;//是否初始化
    public static int gameIndex = 0;//遊戲場次
    public static bool machineIDLock = true;//無機台編號鎖住
    public static int bonusGameIndex = 0;//Bonus遊戲場次
    public static int localRound = 0;
    public static int localBonusRound = 0;
    public static string machineID;//機台編號
    public static string gameID = " ";//遊戲編號
    public static string denomsetting;//denom設定值
    public static string billmachineenable;
    public static float rakeback; //返水比例
    public static int cardneeded = 1; //是否需要會員卡才可遊玩
                                      //machine staut
                                      //gen2 setting
                                      //public static string TicketNum = "";           //票號
                                      //public static string Ticketdate = "";

    public static bool TicketCertification = false;  //入票認證
    public static float ticketcredit = 0;
    public static int gen2enable;//開關票機
    public static int gen2ticketinlimit;//可入票上限
    public static int gen2ticketoutlimit;//可出票上限
    //bill setting
    public static string Billcurrency = " "; //幣別
    public static string Billamountsetting = " "; //鈔額設定
    public static string _machineerror;
    //machine staut
    public static string machineerror
    {
        get { return _machineerror; }
        set
        {
            _machineerror = value;
            if (_machineerror == "3") { }//Debug.Log("ˇ23wer43"); 
        }
    }



    //控制WayGame變數
    public static int[] waygame_WinIconQuantity = new int[slotRowNum + 2];      //紀錄每種Icon排數(ex X3 X4 X5)
    public static int[] waygame_WinIconID = new int[slotRowNum + 2];     //記錄每種連線IconID
    public static bool[,,] waygame_combineIconBool = new bool[slotRowNum + 2, slotReelNum, slotRowNum];//WayGame個別連線icon位置 跑框線用
    public static int[] waygmae_EachIconWinLineNum = new int[slotRowNum + 2];
    public static bool[] isFishChange_BTRule = new bool[slotRowNum + 2];
    public static int[] scoreMultiple = new int[slotRowNum + 2];

    public static int allgame_WinLines = 0;//贏線類型數(幾種Icon連線)
    public static bool[,] allgame_AllCombineIconBool = new bool[slotReelNum, slotRowNum];//WayGame全部連線icon位置 啟動動畫用
    public static int[][] iconTablePay;
    public static bool hasWildPlaySound = false;
    public static bool hasSWildPlaySound = false;

    public static RegisterMediator registerMediator;
    public static int bonusTimes = 0;

    public static int RTPsetting = 2;
    #region json檔設定
    public static JObject JsonObj;                                                    //生成JSON物件
    public static JObject JsonObjBase;
    public static JObject[] JsonObjBonus;
    public static TextAsset sLowRTP_Base;
    public static TextAsset[] sLowRTP_Bonus;
    public static TextAsset lowRTP_Base;
    public static TextAsset[] lowRTP_Bonus;
    public static TextAsset midRTP_Base;
    public static TextAsset[] midRTP_Bonus;
    public static TextAsset highRTP_Base;
    public static TextAsset[] highRTP_Bonus;
    public static TextAsset sHighRTP_Base;
    public static TextAsset[] sHighRTP_Bonus;
    #endregion

    public static void ChangeMathFile(int RTP) //更換JObject物件
    {
#if Server
        #region Server
        switch (RTP)
        {
            case 0:
                JsonObjBase = JObject.Parse(sLowRTP_Base.text);
                JsonObjBonus = new JObject[sLowRTP_Bonus.Length];
                JsonObjBonus[0] = JObject.Parse(sLowRTP_Bonus[0].text);
                JsonObjBonus[1] = JObject.Parse(sLowRTP_Bonus[1].text);
                break;
            case 1:
                JsonObjBase = JObject.Parse(lowRTP_Base.text);
                JsonObjBonus = new JObject[lowRTP_Bonus.Length];
                JsonObjBonus[0] = JObject.Parse(lowRTP_Bonus[0].text);
                JsonObjBonus[1] = JObject.Parse(lowRTP_Bonus[1].text);
                break;
            case 2:
                JsonObjBase = JObject.Parse(midRTP_Base.text);
                JsonObjBonus = new JObject[midRTP_Bonus.Length];
                JsonObjBonus[0] = JObject.Parse(midRTP_Bonus[0].text);
                JsonObjBonus[1] = JObject.Parse(midRTP_Bonus[1].text);
                break;
            case 3:
                JsonObjBase = JObject.Parse(highRTP_Base.text);
                JsonObjBonus = new JObject[highRTP_Bonus.Length];
                JsonObjBonus[0] = JObject.Parse(highRTP_Bonus[0].text);
                JsonObjBonus[1] = JObject.Parse(highRTP_Bonus[1].text);
                break;
            case 4:
                JsonObjBase = JObject.Parse(sHighRTP_Base.text);
                JsonObjBonus = new JObject[sHighRTP_Bonus.Length];
                JsonObjBonus[0] = JObject.Parse(sHighRTP_Bonus[0].text);
                JsonObjBonus[1] = JObject.Parse(sHighRTP_Bonus[1].text);
                break;
        }
        #endregion
#else
        #region  !Server
        switch (RTP)
        {
            case 0:
                JsonObjBase = JObject.Parse(lowRTP_Base.text);
                JsonObjBonus = new JObject[lowRTP_Bonus.Length];
                JsonObjBonus[0] = JObject.Parse(lowRTP_Bonus[0].text);
                JsonObjBonus[1] = JObject.Parse(lowRTP_Bonus[1].text);
                break;
            case 1:
                JsonObjBase = JObject.Parse(midRTP_Base.text);
                JsonObjBonus = new JObject[midRTP_Bonus.Length];
                JsonObjBonus[0] = JObject.Parse(midRTP_Bonus[0].text);
                JsonObjBonus[1] = JObject.Parse(midRTP_Bonus[1].text);
                break;
            case 2:
                JsonObjBase = JObject.Parse(highRTP_Base.text);
                JsonObjBonus = new JObject[highRTP_Bonus.Length];
                JsonObjBonus[0] = JObject.Parse(highRTP_Bonus[0].text);
                JsonObjBonus[1] = JObject.Parse(highRTP_Bonus[1].text);
                break;
        }
        #endregion
#endif
        // JsonReload();
    }

    public static void JsonReload()                             //重新讀取JSON
    {
        if (StartNotNormal && StartNotNormalRTP != 99)
        {
            ChangeMathFile(StartNotNormalRTP);
        }
        else
        {
            ChangeMathFile(RTPsetting);
        }
        switch (BonusSwitch)
        {
            case true:
                if (BonusType == 0) JsonObj = JsonObjBonus[0];
                else if (BonusType == 1) JsonObj = JsonObjBonus[1];
                break;
            case false:
                JsonObj = JsonObjBase;
                break;
        }
    }
    //  public static  currentGameMod;

    [DllImport("libqxt")]
    public static extern void qxt_device_init();
    public static bool QxtIsInit = false;
    public static void Qxt_Device_Init()
    {
        if (QxtIsInit) return;
        qxt_device_init();
        QxtIsInit = true;
    }

}


public class Mod_DataInit : MonoBehaviour
{
    [SerializeField] bool testMode, English;
    [SerializeField] PayTable paytable;
    [SerializeField] UIItemDatabase BaseiconDatabase, BonusiconDatabase;
    [SerializeField] Mod_Data.SlotGameRule setCurrentGameMod;
    [SerializeField] Mod_Data.LineRule setLineRule;
    [SerializeField] Mod_Data.BonusRule bonusRule;
    [SerializeField] Mod_Data.VersionSelect version;
    [SerializeField] Mod_Data.SlotVer slotVer;
    [SerializeField] bool[] setDenomArray;
    [SerializeField] TextAsset sLowRTP_Base;
    [SerializeField] TextAsset[] sLowRTP_Bonus;
    [SerializeField] TextAsset lowRTP_Base;
    [SerializeField] TextAsset[] lowRTP_Bonus;
    [SerializeField] TextAsset midRTP_Base;
    [SerializeField] TextAsset[] midRTP_Bonus;
    [SerializeField] TextAsset highRTP_Base;
    [SerializeField] TextAsset[] highRTP_Bonus;
    [SerializeField] TextAsset sHighRTP_Base;
    [SerializeField] TextAsset[] sHighRTP_Bonus;

    JArray IconPays;
    private void Awake()
    {
        Application.targetFrameRate = 60;
#if QXT
        Screen.SetResolution(1280, 720, true);
#else
        Screen.SetResolution(960, 540, true);
#endif

        #region AGP硬體初始化使用
#if AGP

        int comPortLen = 6;
        SerialPort[] mySerialPort = new SerialPort[comPortLen - 1];
        for (int i = 0; i < mySerialPort.Length; i++)
        {
            try
            {
                mySerialPort[i] = new SerialPort("COM" + (i + 1));
                mySerialPort[i].Open();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        int result = 0;
        result = AGP_Func.AXGMB_Nvram_Open();
        Debug.Log("AXGMB_Nvram_Open: " + (AGP_Func.AGP_ReturnValue)result);
        result = AGP_Func.AXGMB_DIO_Open();
        Debug.Log("AXGMB_DIO_Open: " + (AGP_Func.AGP_ReturnValue)result);
        result = AGP_Func.AXGMB_Intr_Open();
        Debug.Log("AXGMB_Intr_Open: " + (AGP_Func.AGP_ReturnValue)result);


        for (int i = 0; i < mySerialPort.Length; i++)
        {
            try
            {
                mySerialPort[i].Close();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

#endif
        #endregion

        LoadTextasset();
        Mod_Data.language = (Mod_Data.languageList)Convert.ToInt32(English);
        Mod_Data.Version = version;
        Mod_Data.slotVer = slotVer;
        Mod_Data.linegame_LineCount = Mod_Data.linegame_LineCountOri;
        Mod_Data.iconDatabase = BaseiconDatabase;
        Mod_Data.currentGameRule = setCurrentGameMod;
        Mod_Data.lineRule = setLineRule;
        Mod_Data.bonusRule = bonusRule;
        InitLineRule(Mod_Data.lineRule);
        Mod_Data.iconTablePay = new int[BonusiconDatabase.items.Length][];
        IconPaysLoad();
        Mod_Data.registerMediator = GetComponent<RegisterMediator>();
        ////Debug.Log(Mod_Data.iconTablePay[1][3] + "," + Mod_Data.iconTablePay[2][2] + "," + Mod_Data.iconTablePay[3][4]);
        if (Mod_Data.cardneeded == 1) Mod_Data.memberLcok = true;
    }

    private void Start()
    {

    }
    void InitLineRule(Mod_Data.LineRule getLineRule)
    {
        if (getLineRule == Mod_Data.LineRule.Line_25)
        {
            Mod_Data.linegame_Rule[0] = new int[5] { 1, 1, 1, 1, 1 };
            Mod_Data.linegame_Rule[1] = new int[5] { 2, 2, 2, 2, 2 };
            Mod_Data.linegame_Rule[2] = new int[5] { 0, 0, 0, 0, 0 };
            Mod_Data.linegame_Rule[3] = new int[5] { 2, 1, 0, 1, 2 };
            Mod_Data.linegame_Rule[4] = new int[5] { 0, 1, 2, 1, 0 };

            Mod_Data.linegame_Rule[5] = new int[5] { 2, 2, 1, 0, 0 };
            Mod_Data.linegame_Rule[6] = new int[5] { 0, 0, 1, 2, 2 };
            Mod_Data.linegame_Rule[7] = new int[5] { 1, 2, 1, 0, 1 };
            Mod_Data.linegame_Rule[8] = new int[5] { 1, 0, 1, 2, 1 };
            Mod_Data.linegame_Rule[9] = new int[5] { 2, 1, 1, 1, 0 };

            Mod_Data.linegame_Rule[10] = new int[5] { 0, 1, 1, 1, 2 };
            Mod_Data.linegame_Rule[11] = new int[5] { 1, 1, 2, 1, 1 };
            Mod_Data.linegame_Rule[12] = new int[5] { 1, 1, 0, 1, 1 };
            Mod_Data.linegame_Rule[13] = new int[5] { 1, 2, 2, 2, 1 };
            Mod_Data.linegame_Rule[14] = new int[5] { 1, 0, 0, 0, 1 };

            Mod_Data.linegame_Rule[15] = new int[5] { 2, 2, 1, 0, 1 };
            Mod_Data.linegame_Rule[16] = new int[5] { 0, 0, 1, 2, 1 };
            Mod_Data.linegame_Rule[17] = new int[5] { 1, 2, 1, 0, 0 };
            Mod_Data.linegame_Rule[18] = new int[5] { 1, 0, 1, 2, 2 };
            Mod_Data.linegame_Rule[19] = new int[5] { 2, 2, 2, 1, 0 };

            Mod_Data.linegame_Rule[20] = new int[5] { 0, 0, 0, 1, 2 };
            Mod_Data.linegame_Rule[21] = new int[5] { 0, 1, 2, 2, 2 };
            Mod_Data.linegame_Rule[22] = new int[5] { 2, 1, 0, 0, 0 };
            Mod_Data.linegame_Rule[23] = new int[5] { 2, 1, 0, 1, 1 };
            Mod_Data.linegame_Rule[24] = new int[5] { 0, 1, 2, 1, 1 };
        }
        else if (getLineRule == Mod_Data.LineRule.Line_40)
        {
            Mod_Data.linegame_Rule[0] = new int[5] { 2, 2, 2, 2, 2 };
            Mod_Data.linegame_Rule[1] = new int[5] { 1, 1, 1, 1, 1 };
            Mod_Data.linegame_Rule[2] = new int[5] { 3, 3, 3, 3, 3 };
            Mod_Data.linegame_Rule[3] = new int[5] { 0, 0, 0, 0, 0 };
            Mod_Data.linegame_Rule[4] = new int[5] { 2, 1, 0, 1, 2 };

            Mod_Data.linegame_Rule[5] = new int[5] { 1, 2, 3, 2, 1 };
            Mod_Data.linegame_Rule[6] = new int[5] { 3, 3, 2, 1, 0 };
            Mod_Data.linegame_Rule[7] = new int[5] { 0, 0, 1, 2, 3 };
            Mod_Data.linegame_Rule[8] = new int[5] { 2, 3, 3, 3, 2 };
            Mod_Data.linegame_Rule[9] = new int[5] { 1, 0, 0, 0, 1 };

            Mod_Data.linegame_Rule[10] = new int[5] { 3, 2, 1, 0, 0 };
            Mod_Data.linegame_Rule[11] = new int[5] { 0, 1, 2, 3, 3 };
            Mod_Data.linegame_Rule[12] = new int[5] { 2, 3, 2, 1, 2 };
            Mod_Data.linegame_Rule[13] = new int[5] { 1, 0, 1, 2, 1 };
            Mod_Data.linegame_Rule[14] = new int[5] { 3, 2, 3, 2, 3 };

            Mod_Data.linegame_Rule[15] = new int[5] { 0, 1, 0, 1, 0 };
            Mod_Data.linegame_Rule[16] = new int[5] { 2, 1, 2, 3, 2 };
            Mod_Data.linegame_Rule[17] = new int[5] { 1, 2, 1, 0, 1 };
            Mod_Data.linegame_Rule[18] = new int[5] { 3, 2, 2, 2, 3 };
            Mod_Data.linegame_Rule[19] = new int[5] { 0, 1, 1, 1, 0 };

            Mod_Data.linegame_Rule[20] = new int[5] { 2, 2, 1, 0, 0 };
            Mod_Data.linegame_Rule[21] = new int[5] { 1, 1, 0, 1, 1 };
            Mod_Data.linegame_Rule[22] = new int[5] { 2, 2, 3, 2, 2 };
            Mod_Data.linegame_Rule[23] = new int[5] { 1, 1, 2, 3, 3 };
            Mod_Data.linegame_Rule[24] = new int[5] { 2, 1, 1, 1, 0 };

            Mod_Data.linegame_Rule[25] = new int[5] { 1, 2, 2, 2, 3 };
            Mod_Data.linegame_Rule[26] = new int[5] { 3, 3, 2, 3, 3 };
            Mod_Data.linegame_Rule[27] = new int[5] { 0, 0, 1, 0, 0 };
            Mod_Data.linegame_Rule[28] = new int[5] { 3, 3, 3, 2, 1 };
            Mod_Data.linegame_Rule[29] = new int[5] { 3, 2, 1, 1, 0 };

            Mod_Data.linegame_Rule[30] = new int[5] { 0, 1, 2, 2, 3 };
            Mod_Data.linegame_Rule[31] = new int[5] { 0, 0, 0, 1, 2 };
            Mod_Data.linegame_Rule[32] = new int[5] { 2, 3, 3, 2, 1 };
            Mod_Data.linegame_Rule[33] = new int[5] { 1, 0, 0, 1, 2 };
            Mod_Data.linegame_Rule[34] = new int[5] { 3, 2, 2, 1, 0 };

            Mod_Data.linegame_Rule[35] = new int[5] { 0, 1, 1, 2, 3 };
            Mod_Data.linegame_Rule[36] = new int[5] { 2, 3, 2, 1, 0 };
            Mod_Data.linegame_Rule[37] = new int[5] { 1, 0, 1, 2, 3 };
            Mod_Data.linegame_Rule[38] = new int[5] { 3, 2, 1, 0, 1 };
            Mod_Data.linegame_Rule[39] = new int[5] { 0, 1, 2, 3, 2 };

        }
        else if (getLineRule == Mod_Data.LineRule.Line_50)
        {
            Mod_Data.linegame_Rule[0] = new int[5] { 3, 3, 3, 3, 3 };
            Mod_Data.linegame_Rule[1] = new int[5] { 0, 0, 0, 0, 0 };
            Mod_Data.linegame_Rule[2] = new int[5] { 2, 2, 2, 2, 2 };
            Mod_Data.linegame_Rule[3] = new int[5] { 1, 1, 1, 1, 1 };
            Mod_Data.linegame_Rule[4] = new int[5] { 3, 3, 2, 3, 2 };

            Mod_Data.linegame_Rule[5] = new int[5] { 0, 0, 1, 0, 1 };
            Mod_Data.linegame_Rule[6] = new int[5] { 2, 2, 3, 3, 3 };
            Mod_Data.linegame_Rule[7] = new int[5] { 1, 1, 0, 0, 0 };
            Mod_Data.linegame_Rule[8] = new int[5] { 3, 3, 2, 1, 0 };
            Mod_Data.linegame_Rule[9] = new int[5] { 0, 0, 1, 2, 3 };

            Mod_Data.linegame_Rule[10] = new int[5] { 2, 3, 3, 3, 2 };
            Mod_Data.linegame_Rule[11] = new int[5] { 1, 0, 0, 0, 1 };
            Mod_Data.linegame_Rule[12] = new int[5] { 3, 2, 3, 2, 3 };
            Mod_Data.linegame_Rule[13] = new int[5] { 0, 1, 0, 1, 0 };
            Mod_Data.linegame_Rule[14] = new int[5] { 2, 3, 2, 3, 2 };

            Mod_Data.linegame_Rule[15] = new int[5] { 1, 0, 1, 0, 1 };
            Mod_Data.linegame_Rule[16] = new int[5] { 3, 2, 2, 2, 2 };
            Mod_Data.linegame_Rule[17] = new int[5] { 0, 1, 1, 1, 1 };
            Mod_Data.linegame_Rule[18] = new int[5] { 2, 1, 2, 1, 2 };
            Mod_Data.linegame_Rule[19] = new int[5] { 1, 2, 1, 2, 1 };

            Mod_Data.linegame_Rule[20] = new int[5] { 3, 2, 1, 2, 3 };
            Mod_Data.linegame_Rule[21] = new int[5] { 0, 1, 2, 1, 0 };
            Mod_Data.linegame_Rule[22] = new int[5] { 2, 2, 2, 1, 0 };
            Mod_Data.linegame_Rule[23] = new int[5] { 1, 1, 1, 2, 3 };
            Mod_Data.linegame_Rule[24] = new int[5] { 3, 1, 3, 1, 3 };

            Mod_Data.linegame_Rule[25] = new int[5] { 0, 2, 0, 2, 0 };
            Mod_Data.linegame_Rule[26] = new int[5] { 2, 2, 1, 2, 2 };
            Mod_Data.linegame_Rule[27] = new int[5] { 1, 1, 2, 1, 1 };
            Mod_Data.linegame_Rule[28] = new int[5] { 3, 1, 2, 1, 3 };
            Mod_Data.linegame_Rule[29] = new int[5] { 0, 2, 1, 2, 0 };

            Mod_Data.linegame_Rule[30] = new int[5] { 2, 2, 3, 2, 2 };
            Mod_Data.linegame_Rule[31] = new int[5] { 1, 1, 0, 1, 1 };
            Mod_Data.linegame_Rule[32] = new int[5] { 3, 0, 3, 0, 3 };
            Mod_Data.linegame_Rule[33] = new int[5] { 0, 3, 0, 3, 0 };
            Mod_Data.linegame_Rule[34] = new int[5] { 2, 1, 1, 1, 1 };

            Mod_Data.linegame_Rule[35] = new int[5] { 1, 2, 2, 2, 2 };
            Mod_Data.linegame_Rule[36] = new int[5] { 3, 0, 0, 0, 3 };
            Mod_Data.linegame_Rule[37] = new int[5] { 0, 3, 3, 3, 0 };
            Mod_Data.linegame_Rule[38] = new int[5] { 2, 1, 0, 0, 0 };
            Mod_Data.linegame_Rule[39] = new int[5] { 1, 2, 3, 3, 3 };

            Mod_Data.linegame_Rule[40] = new int[5] { 2, 1, 0, 1, 2 };
            Mod_Data.linegame_Rule[41] = new int[5] { 1, 2, 3, 2, 1 };
            Mod_Data.linegame_Rule[42] = new int[5] { 3, 3, 1, 1, 1 };
            Mod_Data.linegame_Rule[43] = new int[5] { 0, 0, 2, 2, 2 };
            Mod_Data.linegame_Rule[44] = new int[5] { 2, 1, 0, 0, 1 };

            Mod_Data.linegame_Rule[45] = new int[5] { 1, 2, 3, 3, 2 };
            Mod_Data.linegame_Rule[46] = new int[5] { 2, 1, 1, 1, 0 };
            Mod_Data.linegame_Rule[47] = new int[5] { 1, 2, 2, 2, 3 };
            Mod_Data.linegame_Rule[48] = new int[5] { 3, 3, 2, 2, 2 };
            Mod_Data.linegame_Rule[49] = new int[5] { 0, 0, 1, 1, 1 };
        }
    }

    void InitIconTablePay()
    {

    }

    static public void IconPaysLoad()
    {
        Mod_Data.JsonReload();
        JArray IconPays;
        for (int i = 0; i < Mod_Data.iconDatabase.items.Length; i++)
        {
            IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon" + (i + 1)]["IconPays"];
            //特殊處理 當餘分降至最低會變動bet時 bonus與scatter分數會變動
            if (i == 1 || i == 2)
            {
                if (Mod_Data.Bet < 25)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (int.Parse(IconPays[4 - j].ToString()) != 0)
                        {
                            IconPays[4 - j] = int.Parse(IconPays[4 - j].ToString()) * Mod_Data.Bet / 25;
                        }
                    }
                }
            }
            Mod_Data.iconTablePay[i] = new int[IconPays.Count];
            for (int k = 0; k < IconPays.Count; k++)
            {
                Mod_Data.iconTablePay[i][k] = int.Parse(IconPays[k].ToString());
            }

        }
    }

    bool isBonus;//紀錄是否為Bonus 切換itembase用

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            testMode = !testMode;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Mod_Data.maxCredit += 10000;
            Mod_Data.credit += 100;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Mod_Data.maxCredit += 10000;
            Mod_Data.credit += 9;
        }


        Mod_Data.TestMode = testMode;
        Mod_Data.denomOpenArray = setDenomArray;
        ////Debug.Log(Mod_Data.BonusSwitch);

        if (isBonus != Mod_Data.BonusSwitch)
        {
            isBonus = Mod_Data.BonusSwitch;
            if (Mod_Data.BonusSwitch)
            {
                Mod_Data.iconDatabase = BonusiconDatabase;
                //Mod_Data.iconTablePay = new int[Mod_Data.iconDatabase.items.Length][];
                IconPaysLoad();
                paytable.PayTableUpData();
            }
            else
            {
                Mod_Data.iconDatabase = BaseiconDatabase;
                //Mod_Data.iconTablePay = new int[Mod_Data.iconDatabase.items.Length][];
                IconPaysLoad();
                paytable.PayTableUpData();
            }
        }
        // //Debug.Log(Mod_Data.state);
    }

    void LoadTextasset()
    {
        Mod_Data.sLowRTP_Base = sLowRTP_Base;
        Mod_Data.sLowRTP_Bonus = new TextAsset[sLowRTP_Bonus.Length];
        for (int i = 0; i < Mod_Data.sLowRTP_Bonus.Length; i++)
        {
            Mod_Data.sLowRTP_Bonus[i] = sLowRTP_Bonus[i];
        }
        Mod_Data.lowRTP_Base = lowRTP_Base;
        Mod_Data.lowRTP_Bonus = new TextAsset[lowRTP_Bonus.Length];
        for (int i = 0; i < Mod_Data.lowRTP_Bonus.Length; i++)
        {
            Mod_Data.lowRTP_Bonus[i] = lowRTP_Bonus[i];
        }
        Mod_Data.midRTP_Base = midRTP_Base;
        Mod_Data.midRTP_Bonus = new TextAsset[midRTP_Bonus.Length];
        for (int i = 0; i < Mod_Data.midRTP_Bonus.Length; i++)
        {
            Mod_Data.midRTP_Bonus[i] = midRTP_Bonus[i];
        }
        Mod_Data.highRTP_Base = highRTP_Base;
        Mod_Data.highRTP_Bonus = new TextAsset[highRTP_Bonus.Length];
        for (int i = 0; i < Mod_Data.highRTP_Bonus.Length; i++)
        {
            Mod_Data.highRTP_Bonus[i] = highRTP_Bonus[i];
        }
        Mod_Data.sHighRTP_Base = sHighRTP_Base;
        Mod_Data.sHighRTP_Bonus = new TextAsset[sHighRTP_Bonus.Length];
        for (int i = 0; i < Mod_Data.sHighRTP_Bonus.Length; i++)
        {
            Mod_Data.sHighRTP_Bonus[i] = sHighRTP_Bonus[i];
        }
    }
    
}
