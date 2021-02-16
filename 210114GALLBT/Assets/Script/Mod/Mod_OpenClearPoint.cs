using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CFPGADrv;
using BYTE = System.Byte;
using System;
using System.Text;
using System.IO;
using System.Threading;
using UnityEngine.UI;
using Quixant.LibQxt.LPComponent;
using System.Runtime.InteropServices;

public class Mod_OpenClearPoint : MonoBehaviour
{
    #region IPC變數

    #region GS變數
#if GS

    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS();

#endif
    #endregion

    #region QXT變數
#if QXT

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_setbit(UInt32 offset, UInt32 bitmask);

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_clearbit(UInt32 offset, UInt32 bitmask);

#endif
    #endregion

    #region AGP變數
#if AGP



#endif
    #endregion

    #endregion

    public Text TotalCreditText, CreditText;
    public int ClearLessCount = 0;
    int PlusCount = 0;
    bool CreditPlus = false;
    bool PlusThreadRuning = false;
    static public bool ClearLessRuning = false;
    [SerializeField] NewSramManager newSramManager;
    [SerializeField] Mod_Client mod_Client;


    // Start is called before the first frame update
    void Start()
    {
        // newSramManager = GetComponent<NewSramManager>();

        #region GS初始化
#if GS

        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FpgaPic_Init();

#endif
        #endregion

        ClearLessCount = newSramManager.LoadClearCount() - 1;
        if (ClearLessCount > 0)
        {
            Thread PlusLess = new Thread(CountPlusLess);
            PlusLess.IsBackground = true;
            PlusLess.Start();
        }
    }

    #region 開分函數
    public void OpenPointFunctionButton(int point)
    {
        //if ((Mod_Data.credit + point) <= newSramManager.LoadMaxCredit() && !CreditPlus)
        //{
        CreditPlus = true;
        //AutoPlayStopCheck();
        Mod_Data.credit += point;
        StartCoroutine(CounterPlus(0, 0));
        newSramManager.SaveTotalOpenPoint(newSramManager.LoadTotalOpenPoint() + point);
        newSramManager.SaveClassOpenPoint(newSramManager.LoadClassOpenPoint() + point);

        newSramManager.saveEventRecoredByEventName(2, point);

        TotalCreditText.text = RoundUp(Mod_Data.credit, 2).ToString("N2");
        CreditText.text = RoundUp((Mod_Data.credit / Mod_Data.Denom), 0).ToString("N0");

        newSramManager.SaveOpenClearPoint(true, newSramManager.LoadOpenClearPoint(true) + point);
        //CreditReset(Mod_Data.credit.ToString("N2"), GameDataManager._AllStatisticalData.OpenPointOnce, 0);
        //}
    }

#if Server
    #region  Server

    public void OpenPointFunction()
    {
        if ((Mod_Data.credit + newSramManager.LoadOpenPointSetting()) <= newSramManager.LoadMaxCredit() && !CreditPlus)
        {
            CreditPlus = true;
            //AutoPlayStopCheck();
            Mod_Data.credit += newSramManager.LoadOpenPointSetting();
            StartCoroutine(CounterPlus(0, 0));
            newSramManager.SaveTotalOpenPoint(newSramManager.LoadTotalOpenPoint() + newSramManager.LoadOpenPointSetting());
            newSramManager.SaveClassOpenPoint(newSramManager.LoadClassOpenPoint() + newSramManager.LoadOpenPointSetting());

            newSramManager.saveEventRecoredByEventName(2, newSramManager.LoadOpenPointSetting());

            TotalCreditText.text = RoundUp(Mod_Data.credit, 2).ToString("N2");
            CreditText.text = RoundUp((Mod_Data.credit / Mod_Data.Denom), 0).ToString("N0");

            newSramManager.SaveOpenClearPoint(true, newSramManager.LoadOpenClearPoint(true) + newSramManager.LoadOpenPointSetting());
            
            if (!Mod_Data.StartNotNormal) {
                mod_Client.SendToSever(Mod_Client_Data.messagetype.pointIn, newSramManager.LoadOpenPointSetting());
            }

            //CreditReset(Mod_Data.credit.ToString("N2"), GameDataManager._AllStatisticalData.OpenPointOnce, 0);
        }
    }

    #endregion
#else
    #region !Server

    public void OpenPointFunction()
    {
        if ((Mod_Data.credit + newSramManager.LoadOpenPointSetting()) <= newSramManager.LoadMaxCredit() && !CreditPlus)
        {
            CreditPlus = true;
            Mod_Data.credit += newSramManager.LoadOpenPointSetting();
            StartCoroutine(CounterPlus(0, 0));
            newSramManager.SaveTotalOpenPoint(newSramManager.LoadTotalOpenPoint() + newSramManager.LoadOpenPointSetting());
            newSramManager.SaveClassOpenPoint(newSramManager.LoadClassOpenPoint() + newSramManager.LoadOpenPointSetting());

            newSramManager.saveEventRecoredByEventName(2, newSramManager.LoadOpenPointSetting());

            TotalCreditText.text = RoundUp(Mod_Data.credit, 2).ToString("N2");
            CreditText.text = RoundUp((Mod_Data.credit / Mod_Data.Denom), 0).ToString("N0");

            newSramManager.SaveOpenClearPoint(true, newSramManager.LoadOpenClearPoint(true) + newSramManager.LoadOpenPointSetting());
        }
    }

    #endregion
#endif

    public void OpenPointFunction(int openPointNum)//網路後臺用 開洗分
    {
        //if ((Mod_Data.credit + openPointNum) <= newSramManager.LoadMaxCredit())
        //{

        //AutoPlayStopCheck();
        Mod_Data.credit += openPointNum;

        newSramManager.SaveTotalOpenPoint(newSramManager.LoadTotalOpenPoint() + openPointNum);
        newSramManager.SaveClassOpenPoint(newSramManager.LoadClassOpenPoint() + openPointNum);

        newSramManager.saveEventRecoredByEventName(2, openPointNum);

        TotalCreditText.text = RoundUp(Mod_Data.credit, 2).ToString("N2");
        CreditText.text = RoundUp((Mod_Data.credit / Mod_Data.Denom), 0).ToString("N0");

        newSramManager.SaveOpenClearPoint(true, newSramManager.LoadOpenClearPoint(true) + openPointNum);
        //CreditReset(Mod_Data.credit.ToString("N2"), GameDataManager._AllStatisticalData.OpenPointOnce, 0);
        //}
    }

    #endregion

    #region 洗分函數

#if Server
    #region Server

    public void ClearPointFunction()
    {
        if (newSramManager.LoadClearCount() == 0)
        {
            int ClearCount = 0;
            Mod_Data.credit = RoundUp(Mod_Data.credit, 3);
            if ((Mod_Data.credit >= newSramManager.LoadClearPointSetting()))
            {
                ClearCount = (int)(Math.Floor(Mod_Data.credit / newSramManager.LoadClearPointSetting()));
                ClearLessCount = ClearCount;
                Mod_Data.credit -= (newSramManager.LoadClearPointSetting() * ClearCount);
                newSramManager.SaveTotalClearPoint(newSramManager.LoadTotalClearPoint() + newSramManager.LoadClearPointSetting() * ClearCount);
                newSramManager.SaveClassClearPoint(newSramManager.LoadClassClearPoint() + newSramManager.LoadClearPointSetting() * ClearCount);
                newSramManager.SaveOpenClearPoint(false, newSramManager.LoadOpenClearPoint(false) + newSramManager.LoadClearPointSetting() * ClearCount);
                newSramManager.saveEventRecoredByEventName(3, newSramManager.LoadClearPointSetting() * ClearCount);

                TotalCreditText.text = RoundUp(Mod_Data.credit, 2).ToString("N2");
                CreditText.text = RoundUp((Mod_Data.credit / Mod_Data.Denom), 0).ToString("N0");

                StartCoroutine(CounterPlus(1, 0));
                if (!Mod_Data.StartNotNormal)
                {
                    mod_Client.SendToSever(Mod_Client_Data.messagetype.pointOut, newSramManager.LoadClearPointSetting() * ClearCount);
                }


            }
        }
    }
    
    #endregion
#else
    #region !Server

    public void ClearPointFunction()
    {
        if (newSramManager.LoadClearCount() == 0)
        {
            int ClearCount = 0;
            Mod_Data.credit = RoundUp(Mod_Data.credit, 3);
            if ((Mod_Data.credit >= newSramManager.LoadClearPointSetting()))
            {
                ClearCount = (int)(Math.Floor(Mod_Data.credit / newSramManager.LoadClearPointSetting()));
                ClearLessCount = ClearCount;
                Mod_Data.credit -= (newSramManager.LoadClearPointSetting() * ClearCount);
                newSramManager.SaveTotalClearPoint(newSramManager.LoadTotalClearPoint() + newSramManager.LoadClearPointSetting() * ClearCount);
                newSramManager.SaveClassClearPoint(newSramManager.LoadClassClearPoint() + newSramManager.LoadClearPointSetting() * ClearCount);
                newSramManager.SaveOpenClearPoint(false, newSramManager.LoadOpenClearPoint(false) + newSramManager.LoadClearPointSetting() * ClearCount);
                newSramManager.saveEventRecoredByEventName(3, newSramManager.LoadClearPointSetting() * ClearCount);

                TotalCreditText.text = RoundUp(Mod_Data.credit, 2).ToString("N2");
                CreditText.text = RoundUp((Mod_Data.credit / Mod_Data.Denom), 0).ToString("N0");

                StartCoroutine(CounterPlus(1, 0));
            }
        }
    }

    #endregion
#endif

    public void ClearPointFunction(int clearPointNum)
    {
        Mod_Data.credit = RoundUp(Mod_Data.credit, 3);
        if (Mod_Data.credit >= clearPointNum)
        {

            Mod_Data.credit -= clearPointNum;
            newSramManager.SaveTotalClearPoint(newSramManager.LoadTotalClearPoint() + clearPointNum);
            newSramManager.SaveClassClearPoint(newSramManager.LoadClassClearPoint() + clearPointNum);
            newSramManager.SaveOpenClearPoint(false, newSramManager.LoadOpenClearPoint(false) + clearPointNum);
            newSramManager.saveEventRecoredByEventName(3, clearPointNum);

            TotalCreditText.text = RoundUp(Mod_Data.credit, 2).ToString("N2");
            CreditText.text = RoundUp((Mod_Data.credit / Mod_Data.Denom), 0).ToString("N0");

        }
    }

    #endregion

    #region 跳計數器函數

    #region 金錢單位換算跳計數器次數

    public void CashInFunction(int Cash)
    {
        PlusCount = Cash / 100;
        StartCoroutine(CounterPlus(2, 0));
    }

    #endregion

    #region 選擇開分、洗分、投幣、退幣

    public IEnumerator CounterPlus(int CounterNumber, int WaitTime)
    {
        switch (CounterNumber)
        {
            #region GS跳計數器方法
#if GS

            case 0:             //開分
                SephirothOneButtonLed(10, 1);
                yield return new WaitForSeconds(0.1f);
                SephirothOneButtonLed(10, 0);
                CreditPlus = false;
                break;
            case 1:             //洗分
                yield return new WaitForSeconds(0.1f);
                Thread PlusLess = new Thread(CountPlusLess);
                PlusLess.IsBackground = true;
                PlusLess.Start();
                break;
            case 2:             //投幣
                yield return new WaitForSeconds(0.1f);
                Thread PlusPlus = new Thread(CountPlusPlus);
                PlusPlus.IsBackground = true;
                PlusPlus.Start();
                yield return new WaitForSeconds(5);
                //GetComponent<UIController>().CashInText.gameObject.SetActive(false);
                break;
            case 3:             //退幣
                SephirothOneButtonLed(13, 1);
                yield return new WaitForSeconds(0.1f);
                SephirothOneButtonLed(13, 0);
                break;
            default:
                break;

#endif
            #endregion

            #region QXT跳計數器方法
#if QXT

            case 0:             //開分
                qxt_IO_Out_SetBits(27);
                yield return new WaitForSeconds(0.1f);
                qxt_IO_Out_ClearBits(27);
                CreditPlus = false;
                break;
            case 1:             //洗分
                yield return new WaitForSeconds(0.1f);
                Thread PlusLess = new Thread(CountPlusLess);
                PlusLess.IsBackground = true;
                PlusLess.Start();
                break;
            case 2:             //投幣
                yield return new WaitForSeconds(0.1f);
                Thread PlusPlus = new Thread(CountPlusPlus);
                PlusPlus.IsBackground = true;
                PlusPlus.Start();
                yield return new WaitForSeconds(5);
                //GetComponent<UIController>().CashInText.gameObject.SetActive(false);
                break;
            case 3:             //退幣
                qxt_IO_Out_SetBits(26);
                yield return new WaitForSeconds(0.1f);
                qxt_IO_Out_ClearBits(26);
                break;
            default:
                break;

#endif
            #endregion

            #region AGP跳計數器方法
#if AGP

            case 0:             //開分
                AGP_Func.AXGMB_DIO_WriteBit(8, 1);
                yield return new WaitForSeconds(0.1f);
                AGP_Func.AXGMB_DIO_WriteBit(8, 0);
                CreditPlus = false;
                break;
            case 1:             //洗分
                yield return new WaitForSeconds(0.1f);
                Thread PlusLess = new Thread(CountPlusLess);
                PlusLess.IsBackground = true;
                PlusLess.Start();
                break;
            case 2:             //投幣
                yield return new WaitForSeconds(0.1f);
                Thread PlusPlus = new Thread(CountPlusPlus);
                PlusPlus.IsBackground = true;
                PlusPlus.Start();
                yield return new WaitForSeconds(5);
                //GetComponent<UIController>().CashInText.gameObject.SetActive(false);
                break;
            case 3:             //退幣
                AGP_Func.AXGMB_DIO_WriteBit(11, 1);
                yield return new WaitForSeconds(0.1f);
                AGP_Func.AXGMB_DIO_WriteBit(11, 0);
                break;
            default:
                break;

#endif
                #endregion

        }
        yield return null;
    }

    #endregion

    #region 投幣方法

    public void CountPlusPlus()
    {
        PlusThreadRuning = true;
        int _PlusCount = PlusCount;
        for (int i = 0; i < _PlusCount; i++)
        {

            #region GS跳計數器方法
#if GS

            SephirothOneButtonLed(12, 1);
            Thread.Sleep(25);
            SephirothOneButtonLed(12, 0);
            Thread.Sleep(25);
            
#endif
            #endregion

            #region QXT跳計數器方法
#if QXT

            qxt_IO_Out_SetBits(31);
            Thread.Sleep(25);
            qxt_IO_Out_ClearBits(31);
            Thread.Sleep(25);

#endif
            #endregion

            #region AGP跳計數器方法
#if AGP

            AGP_Func.AXGMB_DIO_WriteBit(10, 1);
            Thread.Sleep(25);
            AGP_Func.AXGMB_DIO_WriteBit(10, 0);
            Thread.Sleep(25);

#endif
            #endregion

        }

        Mod_Data.Cashin = false;
        Mod_Data.cash = 0;
        PlusCount = 0;
        PlusThreadRuning = false;
    }

    #endregion

    #region 洗分方法

    public void CountPlusLess()
    {
        ClearLessRuning = true;
        int LessCount = ClearLessCount;
        int noUseCount = LessCount;
        newSramManager.SaveClearCount(noUseCount);
        for (int i = 0; i < LessCount; i++)
        {
            noUseCount--;

            #region GS跳計數器方法
#if GS

            SephirothOneButtonLed(11, 1);
            Thread.Sleep(25);
            SephirothOneButtonLed(11, 0);
            Thread.Sleep(25);

#endif
            #endregion

            #region QXT跳計數器方法
#if QXT

            qxt_IO_Out_SetBits(29);
            Thread.Sleep(25);
            qxt_IO_Out_ClearBits(29);
            Thread.Sleep(25);

#endif
            #endregion

            #region AGP跳計數器方法
#if AGP

            AGP_Func.AXGMB_DIO_WriteBit(9, 1);
            Thread.Sleep(25);
            AGP_Func.AXGMB_DIO_WriteBit(9, 0);
            Thread.Sleep(25);

#endif
            #endregion

            newSramManager.SaveClearCount(noUseCount);
        }

        ClearLessCount = 0;
        ClearLessRuning = false;
    }

    #endregion

    #region IPC DO(GPO)寫入方法

    #region GS DO(GPO)寫入方法
#if GS

    public void SephirothOneButtonLed(int ButtonNumber, byte SwitchLed)
    {
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_SetGPOByIndex(0, (BYTE)ButtonNumber, SwitchLed);
    }

#endif
    #endregion

    #region QXT DO(GPO)寫入方法
#if QXT

    public void qxt_IO_Out_SetBits(int Bits)
    {
        UInt32 bitmask = (UInt32)(1 << (Bits % 8));
        UInt32 offset = (UInt32)(Bits / 8);
        qxt_dio_setbit(offset, bitmask);
    }

    public void qxt_IO_Out_ClearBits(int Bits)
    {
        UInt32 bitmask = (UInt32)(1 << (Bits % 8));
        UInt32 offset = (UInt32)(Bits / 8);
        qxt_dio_clearbit(offset, bitmask);
    }

#endif
    #endregion

    #endregion

    #endregion

    public static double RoundUp(double input, int num)
    {
        double tmpNum = Math.Floor(input);
        if ((input - tmpNum) > 0) //判斷input是否為整數
        {
            if ((input - Math.Round(input, num)) != 0) //判斷所要取的位數是否存在
            {
                //利用四捨五入的方法判斷是否要進位，若取的下一位數大於等於5則不用進位
                if (Convert.ToInt32(input * Math.Pow(10, num + 1) % 10) < 5)
                {
                    return Math.Round(input, num, MidpointRounding.AwayFromZero) + Math.Pow(0.1, num);
                }
                else
                {
                    return Math.Round(input, num, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                return input;
            }
        }
        else
        {
            return input;
        }
    }
}
