using System.Collections;
using System.Collections.Generic;

public static class BillAcceptorSettingData
{
#if Server
    #region Server
public static bool ErrorBool = false;
    public static bool NormalBool = false;
    public static bool TicketIn = false;
    public static string TicketType = "";
    public static string TicketValue = "";
    public static string ErrorMessage = "";
    public static string NormalMessage = "";
    public static string COMPort = "COM1";
    public static string BillName = "JCM"; //0:JCM 1:MEI 2:ICT
    public static bool BillOpenClose = false; //true:open  false:close
    public static bool TicketEnable = true;  //吸票
    public static bool CashEnable = false; //吸鈔
    public static int TicketCashOpenClose = 1;
    public static bool StopCashIn = false;
    public static bool BillAcceptorEnable = true;
    public static string GetOrderType = "";
    public static bool StackReturnBool = false;
    public static bool JCM_WaitRespond = false;
    public static bool CheckIsInBaseSpin = false;
    public static bool GameCanPlay = true;
    public static bool CashOrTicketIn = false;
    public static double CashOrTicketInMaxLimit = 10000000;
    public static int TicketInWaitSever = 2;
    public static int BillEnableWaitServer = 2;
    public static bool BillOpen = false;
    public static bool billreturn = false;
    public static bool billstack = false;
    //public static bool ValueSendBool = false;
    #endregion
#else
    #region !Server
    public static bool ErrorBool = false;
    public static bool NormalBool = false;
    public static bool TicketIn = false;
    public static string TicketType = "";
    public static string TicketValue = "";
    public static string ErrorMessage = "";
    public static string NormalMessage = "";
    public static string COMPort = "COM1";
    public static string BillName = "JCM"; //0:JCM 1:MEI 2:ICT
    public static bool BillOpenClose = false; //true:open  false:close
    public static bool TicketEnable = false;  //吸票
    public static bool StopCashIn = false;
    public static bool BillAcceptorEnable = true;
    public static string GetOrderType = "";
    public static bool StackReturnBool = false;
    public static bool JCM_WaitRespond = false;
    public static bool GameCanPlay = false;
    public static bool CheckIsInBaseSpin = false;
    public static bool Stacked = false;
    public static bool Return = false;
    public static byte Jcm_Buffer = 0x00;
    //public static bool ValueSendBool = false;
    #endregion
#endif
}
