using CFPGADrv;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quixant;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using Quixant.LibNvram.NvramComponent;
using System.Text;
using System.Reflection;

public class NewSramManager : MonoBehaviour
{
    #region GSSram
#if GS

    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS();

    StatisticalData initData = new StatisticalData();
    RTP_SettingData initRtp = new RTP_SettingData();

    private void Awake()
    {
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FpgaPic_Init();

    }

    #region 存讀總紀錄
    //儲存總紀錄
    public void SaveTotalStatistcalData(int TotalOpenPoint, int TotalClearPoint, int TotalCoinIn, double TotalBet, double TotalWin, int TotalGamePlay, int TotalWinGamePlay)//開分 洗分 總押注 總贏分 遊戲場次 贏場次
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalBetInteger = 0, TotalWinInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0;
        if (TotalBet % 1 != 0)
        {
            TotalBetDecimal = (int)SaveDecimalPointHandle(TotalBet);
            TotalBetInteger = (int)TotalBet;
        }
        else
        {
            TotalBetInteger = (int)TotalBet;
        }

        if (TotalWin % 1 != 0)
        {
            TotalWinDecimal = (int)SaveDecimalPointHandle(TotalWin);
            TotalWinInteger = (int)TotalWin;
        }
        else
        {
            TotalWinInteger = (int)TotalWin;
        }

        offset = 0x00;
        byteuint = new uint[2];
        byteuint[0] = (uint)TotalOpenPoint;
        byteuint[1] = (uint)TotalClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x70;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalCoinIn;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x14;
        byteuint = new uint[2];
        byteuint[0] = (uint)TotalGamePlay;
        byteuint[1] = (uint)TotalWinGamePlay;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x08;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalBetInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalBetDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x0E;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalWinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

    }

    public void SaveTotalOpenPoint(int TotalOpenPoint)//總開分
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x00;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalOpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

    }
    public void SaveTotalClearPoint(int TotalClearPoint)//總洗分
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x04;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

    }
    public void SaveTotalCoinIn(int TotalCoinIn)//總入鈔
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x70;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalCoinIn;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    public void SaveTotalBet(double TotalBet)//總押注
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalBetInteger = 0;
        int TotalBetDecimal = 0;
        if (TotalBet % 1 != 0)
        {
            TotalBetDecimal = (int)SaveDecimalPointHandle(TotalBet);
            TotalBetInteger = (int)TotalBet;
        }
        else
        {
            TotalBetInteger = (int)TotalBet;
        }

        offset = 0x08;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalBetInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalBetDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

    }
    public void SaveTotalWin(double TotalWin)//總贏分
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalWinInteger = 0;
        int TotalWinDecimal = 0;
        if (TotalWin % 1 != 0)
        {
            TotalWinDecimal = (int)SaveDecimalPointHandle(TotalWin);
            TotalWinInteger = (int)TotalWin;
        }
        else
        {
            TotalWinInteger = (int)TotalWin;
        }
        offset = 0x0E;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalWinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }

    public void SaveTotalGamePlay(double TotalGamePlay)//總遊戲場次
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x14;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalGamePlay;

        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    public void SaveTotalWinGamePlay(double TotalWinGamePlay)//總贏場次
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x18;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinGamePlay;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }

    //讀取總紀錄
    public void LoadTotalStatistcalData(out int TotalOpenPoint, out int TotalClearPoint, out int TotalCoinIn, out double TotalBet, out double TotalWin, out int TotalGamePlay, out int TotalWinGamePlay)//開分 洗分 總押注 總贏分 遊戲場次 贏場次
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        int TotalBetInteger = 0, TotalWinInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0;
        offset = 0x00;
        byteuint = new uint[2];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalOpenPoint = (int)byteuint[0];
        TotalClearPoint = (int)byteuint[1];

        offset = 0x70;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalCoinIn = (int)byteuint[0];


        offset = 0x14;
        byteuint = new uint[2];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalGamePlay = (int)byteuint[0];
        TotalWinGamePlay = (int)byteuint[1];

        offset = 0x08;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalBetDecimal = (int)ushortbuf[0];

        offset = 0x0E;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalWinDecimal = (int)ushortbuf[0];

        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }

        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
    }

    public int LoadTotalOpenPoint()//讀取總開分
    {
        int TotalOpenPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x00;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalOpenPoint = (int)byteuint[0];


        return TotalOpenPoint;
    }
    public int LoadTotalClearPoint()//讀取總洗分
    {
        int TotalClearPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x04;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalClearPoint = (int)byteuint[0];


        return TotalClearPoint;
    }
    public int LoadTotalCoinIn()//讀取總入鈔
    {
        int TotalCoinIn;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x70;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalCoinIn = (int)byteuint[0];


        return TotalCoinIn;
    }
    public double LoadTotalBet()//讀取總押注
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalBet;
        int TotalBetInteger = 0;
        int TotalBetDecimal = 0;

        offset = 0x08;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalBetDecimal = (int)ushortbuf[0];
        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }
        return TotalBet;
    }
    public double LoadTotalWin()//讀取總贏分
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalWin;
        int TotalWinInteger = 0;
        int TotalWinDecimal = 0;

        offset = 0x0E;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalWinDecimal = (int)ushortbuf[0];
        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
        return TotalWin;
    }
    public int LoadTotalGamePlay()//讀取總場次
    {
        int TotalGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x14;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalGamePlay = (int)byteuint[0];


        return TotalGamePlay;
    }
    public int LoadTotalWinGamePlay()//讀取總贏場次
    {
        int TotalWinGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x18;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalWinGamePlay = (int)byteuint[0];


        return TotalWinGamePlay;
    }
    public float LoadTotalWinScoreRate()//讀取總得分率
    {
        return (float)(LoadTotalWin() / LoadTotalBet());
    }
    public float LoadTotalWinCountRate()//讀取總贏分率
    {
        return (float)((float)LoadTotalWinGamePlay() / (float)LoadTotalGamePlay());
    }
    #endregion

    #region 存讀班紀錄
    //儲存班別紀錄
    public void SaveClassStatistcalData(int ClassOpenPoint, int ClassClearPoint, int ClassCoinIn, double ClassBet, double ClassWin, int ClassGamePlay, int ClassWinGamePlay)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassBetInteger = 0, ClassWinInteger = 0;
        int ClassBetDecimal = 0, ClassWinDecimal = 0;
        if (ClassBet % 1 != 0)
        {
            ClassBetDecimal = (int)SaveDecimalPointHandle(ClassBet);
            ClassBetInteger = (int)ClassBet;
        }
        else
        {
            ClassBetInteger = (int)ClassBet;
        }

        if (ClassWin % 1 != 0)
        {
            ClassWinDecimal = (int)SaveDecimalPointHandle(ClassWin);
            ClassWinInteger = (int)ClassWin;
        }
        else
        {
            ClassWinInteger = (int)ClassWin;
        }

        offset = 0x1C;
        byteuint = new uint[2];
        byteuint[0] = (uint)ClassOpenPoint;
        byteuint[1] = (uint)ClassClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x74;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassCoinIn;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x30;
        byteuint = new uint[2];
        byteuint[0] = (uint)ClassGamePlay;
        byteuint[1] = (uint)ClassWinGamePlay;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x24;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassBetInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassBetDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x2A;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassWinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassWinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }

    public void SaveClassOpenPoint(int ClassOpenPoint)//總開分
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x1C;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassOpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

    }
    public void SaveClassClearPoint(int ClassClearPoint)//總洗分
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x20;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

    }
    public void SaveClassCoinIn(int ClassCoinIn)//總入鈔
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x74;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassCoinIn;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    public void SaveClassBet(double ClassBet)//總押注
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassBetInteger = 0;
        int ClassBetDecimal = 0;
        if (ClassBet % 1 != 0)
        {
            ClassBetDecimal = (int)SaveDecimalPointHandle(ClassBet);
            ClassBetInteger = (int)ClassBet;
        }
        else
        {
            ClassBetInteger = (int)ClassBet;
        }

        offset = 0x24;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassBetInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassBetDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

    }
    public void SaveClassWin(double ClassWin)//總贏分
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassWinInteger = 0;
        int ClassWinDecimal = 0;
        if (ClassWin % 1 != 0)
        {
            ClassWinDecimal = (int)SaveDecimalPointHandle(ClassWin);
            ClassWinInteger = (int)ClassWin;
        }
        else
        {
            ClassWinInteger = (int)ClassWin;
        }
        offset = 0x2A;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassWinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassWinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }
    public void SaveClassGamePlay(double ClassGamePlay)//總遊戲場次
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x30;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassGamePlay;

        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    public void SaveClassWinGamePlay(double ClassWinGamePlay)//總贏場次
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x34;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassWinGamePlay;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }

    //讀取班別紀錄
    public void LoadClassStatistcalData(out int ClassOpenPoint, out int ClassClearPoint, out int ClassCoinIn, out double ClassBet, out double ClassWin, out int ClassGamePlay, out int ClassWinGamePlay)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        int ClassBetInteger = 0, ClassWinInteger = 0;
        int ClassBetDecimal = 0, ClassWinDecimal = 0;
        offset = 0x1C;
        byteuint = new uint[2];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassOpenPoint = (int)byteuint[0];
        ClassClearPoint = (int)byteuint[1];

        offset = 0x74;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassCoinIn = (int)byteuint[0];

        offset = 0x30;
        byteuint = new uint[2];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassGamePlay = (int)byteuint[0];
        ClassWinGamePlay = (int)byteuint[1];

        offset = 0x24;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        ClassBetDecimal = (int)ushortbuf[0];

        offset = 0x2A;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        ClassWinDecimal = (int)ushortbuf[0];

        if (ClassBetDecimal == 0)
        {
            ClassBet = ClassBetInteger;
        }
        else
        {
            ClassBet = ClassBetInteger + (ClassBetDecimal * 0.01);
        }

        if (ClassWinDecimal == 0)
        {
            ClassWin = ClassWinInteger;
        }
        else
        {
            ClassWin = ClassWinInteger + (ClassWinDecimal * 0.01);
        }
    }

    public int LoadClassOpenPoint()//讀取總開分
    {
        int ClassOpenPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1C;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassOpenPoint = (int)byteuint[0];


        return ClassOpenPoint;
    }
    public int LoadClassClearPoint()//讀取總洗分
    {
        int ClassClearPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x20;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassClearPoint = (int)byteuint[0];


        return ClassClearPoint;
    }
    public int LoadClassCoinIn()//讀取總入鈔
    {
        int ClassCoinIn;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x74;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassCoinIn = (int)byteuint[0];


        return ClassCoinIn;
    }
    public double LoadClassBet()//讀取總押注
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalBet;
        int ClassBetInteger = 0;
        int ClassBetDecimal = 0;

        offset = 0x24;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        ClassBetDecimal = (int)ushortbuf[0];
        if (ClassBetDecimal == 0)
        {
            TotalBet = ClassBetInteger;
        }
        else
        {
            TotalBet = ClassBetInteger + (ClassBetDecimal * 0.01);
        }
        return TotalBet;
    }
    public double LoadClassWin()//讀取總贏分
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double ClassWin;
        int ClassWinInteger = 0;
        int ClassWinDecimal = 0;

        offset = 0x2A;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        ClassWinDecimal = (int)ushortbuf[0];
        if (ClassWinDecimal == 0)
        {
            ClassWin = ClassWinInteger;
        }
        else
        {
            ClassWin = ClassWinInteger + (ClassWinDecimal * 0.01);
        }
        return ClassWin;
    }
    public int LoadClassGamePlay()//讀取總場次
    {
        int ClassGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x30;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassGamePlay = (int)byteuint[0];


        return ClassGamePlay;
    }
    public int LoadClassWinGamePlay()//讀取總贏場次
    {
        int ClassWinGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x34;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassWinGamePlay = (int)byteuint[0];


        return ClassWinGamePlay;
    }

    public float LoadClassWinScoreRate()//讀取班得分率
    {
        return (float)(LoadClassWin() / LoadClassBet());
    }
    public float LoadClassWinCountRate()//讀取班贏分率
    {
        return (float)((float)LoadClassWinGamePlay() / (float)LoadClassGamePlay());
    }
    #endregion

    #region 存讀備份紀錄
    //儲存備份紀錄
    public void SaveBackupStatistcalData(int TotalOpenPoint, int TotalClearPoint, int TotalCoinIn, double TotalBet, double TotalWin, int TotalGamePlay, int TotalWinGamePlay)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalBetInteger = 0, TotalWinInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0;
        if (TotalBet % 1 != 0)
        {
            TotalBetDecimal = (int)SaveDecimalPointHandle(TotalBet);
            TotalBetInteger = (int)TotalBet;
        }
        else
        {
            TotalBetInteger = (int)TotalBet;
        }

        if (TotalWin % 1 != 0)
        {
            TotalWinDecimal = (int)SaveDecimalPointHandle(TotalWin);
            TotalWinInteger = (int)TotalWin;
        }
        else
        {
            TotalWinInteger = (int)TotalWin;
        }

        offset = 0x1000;
        byteuint = new uint[2];
        byteuint[0] = (uint)TotalOpenPoint;
        byteuint[1] = (uint)TotalClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x1022;
        byteuint = new uint[2];
        byteuint[0] = (uint)TotalCoinIn;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x1014;
        byteuint = new uint[2];
        byteuint[0] = (uint)TotalGamePlay;
        byteuint[1] = (uint)TotalWinGamePlay;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x1008;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalBetInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalBetDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x100E;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalWinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }
    //讀取備份紀錄
    public void LoadBackupStatistcalData(out int TotalOpenPoint, out int TotalClearPoint, out double TotalBet, out double TotalWin, out int TotalGamePlay, out int TotalWinGamePlay)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        int TotalBetInteger = 0, TotalWinInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0;
        offset = 0x1000;
        byteuint = new uint[2];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalOpenPoint = (int)byteuint[0];
        TotalClearPoint = (int)byteuint[1];

        offset = 0x1014;
        byteuint = new uint[2];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalGamePlay = (int)byteuint[0];
        TotalWinGamePlay = (int)byteuint[1];

        offset = 0x1008;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalBetDecimal = (int)ushortbuf[0];

        offset = 0x100E;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalWinDecimal = (int)ushortbuf[0];

        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }

        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
    }

    public int LoadBackupTotalOpenPoint()//讀取總開分
    {
        int TotalOpenPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1000;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalOpenPoint = (int)byteuint[0];


        return TotalOpenPoint;
    }
    public int LoadBackupTotalClearPoint()//讀取總洗分
    {
        int TotalClearPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1004;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalClearPoint = (int)byteuint[0];


        return TotalClearPoint;
    }
    public int LoadBackupTotalCoinIn()//讀取總入鈔
    {
        int TotalCoinIn;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1022;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalCoinIn = (int)byteuint[0];


        return TotalCoinIn;
    }
    public double LoadBackupTotalBet()//讀取總押注
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalBet;
        int TotalBetInteger = 0;
        int TotalBetDecimal = 0;

        offset = 0x1008;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalBetDecimal = (int)ushortbuf[0];
        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }
        return TotalBet;
    }
    public double LoadBackupTotalWin()//讀取總贏分
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalWin;
        int TotalWinInteger = 0;
        int TotalWinDecimal = 0;

        offset = 0x100E;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalWinDecimal = (int)ushortbuf[0];
        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
        return TotalWin;
    }
    public int LoadBackupTotalGamePlay()//讀取總場次
    {
        int TotalGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1014;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalGamePlay = (int)byteuint[0];


        return TotalGamePlay;
    }
    public int LoadBackupTotalWinGamePlay()//讀取總贏場次
    {
        int TotalWinGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1018;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalWinGamePlay = (int)byteuint[0];


        return TotalWinGamePlay;
    }

    #endregion

    //儲存密碼
    public void SavePassword(int password)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x38;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)password;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }
    //讀取密碼
    public int LoadPassword()
    {
        int password;
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x38;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        password = (int)ushortbuf[0];
        return password;
    }

    //儲存鈔機設定
    public void SaveBanknoteMachineSetting(bool Working)
    {
        UInt32 offset = 0x3A;
        byte[] bytebuf = new byte[1];
        if (Working)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取鈔機設定
    public void LoadBanknoteMachineSetting(out bool Working)
    {
        UInt32 offset = 0x3A;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        if (bytebuf[0] == 0)
        {
            Working = false;
        }
        else
        {
            Working = true;
        }
    }
    //儲存單次開分鍵與單次洗分鍵的數值
    public void SaveOpenPointSetting(int OpenPoint)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x3B;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }
    public void SaveClearPointSetting(int ClearPoint)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x3D;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }
    //讀取單次開分鍵與單次洗分鍵的數值
    public int LoadOpenPointSetting()
    {
        int OpenPoint;
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x3B;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        OpenPoint = (int)ushortbuf[0];
        return OpenPoint;
    }
    public int LoadClearPointSetting()
    {
        int ClearPoint;
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x3D;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        ClearPoint = (int)ushortbuf[0];
        return ClearPoint;
    }
    //儲存最大押注倍數
    public void SaveMaxOdd(int Odd)
    {
        UInt32 offset = 0x47;
        byte[] bytebuf = new byte[1];
        bytebuf[0] = (byte)Odd;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取最大押注倍數
    public int LoadMaxOdd()
    {
        int Odd;
        UInt32 offset = 0x47;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        Odd = (int)bytebuf[0];
        return Odd;
    }
    //儲存最大籌碼與最大贏籌碼
    public void SaveMaxCredit(int MaxCredit)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x48;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxCredit;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    public void SaveMaxWin(int MaxWin)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x4C;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxWin;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    //讀取最大籌碼與最大贏籌碼
    public int LoadMaxCredit()
    {
        int MaxCredit;
        UInt32 offset;
        uint[] byteuint;

        offset = 0x48;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        MaxCredit = (int)byteuint[0];
        return MaxCredit;

    }
    public int LoadMaxWin()
    {
        int MaxWin;
        UInt32 offset;
        uint[] byteuint;

        offset = 0x4C;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        MaxWin = (int)byteuint[0];
        return MaxWin;
    }
    //儲存顯示的籌碼比率
    public void SaveDenomOption(bool[] DenomOption)
    {
        UInt32 offset = 0x50;
        byte[] bytebuf = new byte[9];
        for (int i = 0; i < 9; i++)
        {
            if (DenomOption[i]) { bytebuf[i] = 1; }
            else { bytebuf[i] = 0; }
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取顯示的籌碼比率
    public void LoadDenomOption(out bool[] DenomOption)
    {
        UInt32 offset = 0x50;
        byte[] bytebuf = new byte[9];
        bool[] tmp = new bool[9];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        for (int i = 0; i < 9; i++)
        {
            if (bytebuf[i] == 0) { tmp[i] = false; }
            else { tmp[i] = true; }
        }
        DenomOption = tmp;
    }
    //儲存事件紀錄
    public void SaveEventRecored(EventRecoredData[] EventRecored)
    {
        UInt32 offset;
        byte[] Eventbytebuf;
        uint[] Eventbyteuint;

        offset = 0x0100;
        Eventbytebuf = new byte[100];
        for (int i = 0; i < 100; i++)
        {
            Eventbytebuf[i] = (byte)EventRecored[i].EventCode;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(Eventbytebuf, offset, (uint)Eventbytebuf.Length);

        offset = 0x0164;
        Eventbyteuint = new uint[100];
        for (int i = 0; i < 100; i++)
        {
            Eventbyteuint[i] = (uint)EventRecored[i].EventData;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(Eventbyteuint, offset, (uint)Eventbyteuint.Length);

        offset = 0x02F4;
        byte[] EventTime;
        for (int i = 0; i < 100; i++)
        {
            EventTime = new byte[8];
            EventTime = BitConverter.GetBytes(EventRecored[i].EventTime.Ticks);
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);
            offset += 0x0008;
        }
    }
    //讀取事件紀錄
    public void LoadEventRecored(out EventRecoredData[] EventRecored)
    {
        UInt32 offset;
        byte[] Eventbytebuf;
        uint[] Eventbyteuint;
        EventRecoredData[] tmp = new EventRecoredData[100];
        for (int i = 0; i < 100; i++)
        {
            tmp[i] = new EventRecoredData();
        }

        offset = 0x0100;
        Eventbytebuf = new byte[100];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(Eventbytebuf, offset, (uint)Eventbytebuf.Length);
        for (int i = 0; i < 100; i++)
        {
            tmp[i].EventCode = (int)Eventbytebuf[i];
        }

        offset = 0x0164;
        Eventbyteuint = new uint[100];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(Eventbyteuint, offset, (uint)Eventbyteuint.Length);
        for (int i = 0; i < 100; i++)
        {
            tmp[i].EventData = (int)Eventbyteuint[i];
        }

        offset = 0x02F4;
        byte[] EventTime;
        for (int i = 0; i < 100; i++)
        {
            EventTime = new byte[8];
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(EventTime, offset, (uint)EventTime.Length);
            tmp[i].EventTime = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));
            offset += 0x0008;
        }

        EventRecored = tmp;
    }


    int EventRecoredSaveQuantity = 100;
    public static EventRecoredDataList eventRecoredDataList = new EventRecoredDataList();

    public void saveEventRecoredByEventName(int _EventCode, int _EventData)
    {
        for (int i = EventRecoredSaveQuantity - 1; i >= 0; i--)//0:無資料  1以後的數字(包含1)請自行運用  0:無資料 1:啟動遊戲 2:開分 3:洗分 4:入鈔  0x0100 (1) * 200
        {
            if (eventRecoredDataList._EventRecoredData[i].EventCode != 0)
            {
                if (i < EventRecoredSaveQuantity - 1)
                {
                    eventRecoredDataList._EventRecoredData[i + 1].EventCode = eventRecoredDataList._EventRecoredData[i].EventCode;
                    eventRecoredDataList._EventRecoredData[i + 1].EventData = eventRecoredDataList._EventRecoredData[i].EventData;
                    eventRecoredDataList._EventRecoredData[i + 1].EventTime = eventRecoredDataList._EventRecoredData[i].EventTime;
                }
                else
                {
                    eventRecoredDataList._EventRecoredData[i] = new EventRecoredData();
                }
            }
        }
        eventRecoredDataList._EventRecoredData[0].EventCode = _EventCode;
        eventRecoredDataList._EventRecoredData[0].EventData = _EventData;
        eventRecoredDataList._EventRecoredData[0].EventTime = DateTime.Now;
        SaveEventRecored(eventRecoredDataList._EventRecoredData);
        //saveEventRecoredData(EventRecoredDataFileName, eventRecoredDataList);
        //GetComponent<SramManager>().EventRecoredSave();
    }


    //儲存個別RTP設定與是否使用共通RTP
    public void SaveRTPSetting(int[] RTPValue, bool RTPuse)
    {
        UInt32 offset = 0x624;
        byte[] bytebuf = new byte[11];
        for (int i = 0; i < 10; i++)
        {
            bytebuf[i] = (byte)RTPValue[i];
        }
        if (RTPuse) { bytebuf[10] = 1; }
        else { bytebuf[10] = 0; }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取個別RTP設定與是否使用共通RTP
    public void LoadRTPSetting(out int[] RTPValue, out bool RTPuse)
    {
        UInt32 offset = 0x624;
        int[] tmp = new int[10];
        byte[] bytebuf = new byte[11];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        for (int i = 0; i < 10; i++)
        {
            tmp[i] = (int)bytebuf[i];
        }
        RTPValue = tmp;

        if (bytebuf[10] == 0)
        {
            RTPuse = false;
        }
        else
        {
            RTPuse = true;
        }
    }


    //儲存狀態
    public void SaveStatus(int NowStatus)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x59;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)NowStatus;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }
    //讀取狀態
    public int LoadStatus()
    {
        int NowStatus;
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x59;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        NowStatus = (int)ushortbuf[0];
        return NowStatus;
    }
    //儲存Bouns前牌面 //原16 改為8
    public void SaveBeforeBonusRNG(int[] beforeBonusRNG)
    {
        UInt32 offset;
        offset = 0x4000;
        ushort[] ushortbuf;
        ushortbuf = new ushort[5];
        ushortbuf[0] = (ushort)beforeBonusRNG[0];
        ushortbuf[1] = (ushort)beforeBonusRNG[1];
        ushortbuf[2] = (ushort)beforeBonusRNG[2];
        ushortbuf[3] = (ushort)beforeBonusRNG[3];
        ushortbuf[4] = (ushort)beforeBonusRNG[4];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }
    //讀起Bonus前牌面  //原16 改為8
    public int[] LoadBeforeBonusRNG()
    {
        int[] beforeBonusRNG = new int[5];
        UInt32 offset;
        offset = 0x4000;
        ushort[] ushortbuf;
        ushortbuf = new ushort[5];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        beforeBonusRNG[0] = (int)ushortbuf[0];
        beforeBonusRNG[1] = (int)ushortbuf[1];
        beforeBonusRNG[2] = (int)ushortbuf[2];
        beforeBonusRNG[3] = (int)ushortbuf[3];
        beforeBonusRNG[4] = (int)ushortbuf[4];
        return beforeBonusRNG;
    }

    //儲存Sram是否有紀錄
    public void SaveIsSramTrue(bool isSram)
    {
        UInt32 offset = 0x5B;
        byte[] bytebuf = new byte[1];
        if (isSram) bytebuf[0] = (byte)1;
        else bytebuf[0] = (byte)0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取Sram是否有紀錄
    public bool LoadIsSramTrue()
    {
        UInt32 offset = 0x5B;
        byte[] bytebuf = new byte[1];
        int tmp;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        tmp = (int)bytebuf[0];
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    //讀取記錄編號 輸入Type(0:一般紀錄 1:最大獎 2:最大獎Second 預設:一般記錄)  輸出 最大的編號
    public void LoadGameHistoryLog(int Type, out int MaxLogNumber)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        int[] NumberArray;
        int HistoryLength = 100;
        MaxLogNumber = 0;
        switch (Type)
        {
            case 0:
                offset = 0x1500;
                HistoryLength = 100;
                break;
            case 1:
                offset = 0x3000;
                HistoryLength = 10;
                break;
            case 2:
                offset = 0x3300;
                HistoryLength = 10;
                break;
            default:
                offset = 0x1500;
                HistoryLength = 100;
                break;
        }
        ushortbuf = new ushort[HistoryLength];
        NumberArray = new int[HistoryLength];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (int i = 0; i < HistoryLength; i++)
        {
            NumberArray[i] = (int)ushortbuf[i];
            if (MaxLogNumber < NumberArray[i])
            {
                MaxLogNumber = NumberArray[i];
            }
        }
    }

    public void ClearHistory()
    {
        HistoryData SaveData = new HistoryData();
        for (int i = 0; i < 100; i++)
        {
            SaveHistory(SaveData);
        }


        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x1500;
        ushortbuf = new ushort[100];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        offset = 0x4100;
        ushortbuf = new ushort[100];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        for (int i = 1; i <= 10; i++)
        {
            SaveMaxGameHistory(i, SaveData);
            SaveMaxGameHistorySecond(i, SaveData);
        }
        offset = 0x3000;
        ushortbuf = new ushort[10];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x3300;
        ushortbuf = new ushort[10];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
    }
    //儲存歷史紀錄 會清空目前開洗分 輸入時必須已是完整記錄(包含開洗分)
    public void SaveHistory(HistoryData SaveData)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        int CreditInteger = 0, CreditDecimal = 0;
        int WinInteger = 0, WinDecimal = 0;
        byte[] EventTime;
        offset = 0x1500;
        ushortbuf = new ushort[100];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (Count = 0; Count < 100; Count++)
        {
            if (ushortbuf[Count] == 0 && Count == 0)
            {
                goto SaveStart;
            }
            else if (ushortbuf[Count] == 0)
            {
                goto SaveNew;
            }
            else if (Count == 99 && ushortbuf[Count] != 0)
            {
                goto SaveOverWrite;
            }
        }
        return;

    SaveStart:
    #region SaveStart
        offset = 0x1500;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)1;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (SaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(SaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)SaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }
        }
        offset = 0x15C8;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x4100;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);


        offset = 0x17BC;
        bytebuf = new byte[1];
        if (SaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (SaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(SaveData.Credit);
            CreditInteger = (int)SaveData.Credit;
        }
        else
        {
            CreditInteger = (int)SaveData.Credit;
        }

        offset = 0x1820;
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x1A78;
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.Demon;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x1C08;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.Bet;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x1CD0;
        bytebuf = new byte[1];
        bytebuf[0] = (byte)SaveData.Odds;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        WinInteger = 0;
        WinDecimal = 0;
        if (SaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(SaveData.Win);
            WinInteger = (int)SaveData.Win;
        }
        else
        {
            WinInteger = (int)SaveData.Win;
        }
        offset = 0x1D34;
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x1F8C;
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(SaveData.Time.Ticks);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);

        offset = 0x22AC;
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x243C;
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x25CC;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.RTP;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x2694;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.SpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x275C;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusSpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x2824;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusIsPlayedCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x28EC;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        goto End;
    #endregion

    SaveNew:
    #region SaveNew
        offset = 0x1500;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)(Count + 1);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (SaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(SaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)SaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }
        }

        offset = 0x15C8;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x4100;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x17BC;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (SaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (SaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(SaveData.Credit);
            CreditInteger = (int)SaveData.Credit;
        }
        else
        {
            CreditInteger = (int)SaveData.Credit;
        }

        offset = 0x1820;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x1A78;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.Demon;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x1C08;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.Bet;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x1CD0;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)SaveData.Odds;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        WinInteger = 0;
        WinDecimal = 0;
        if (SaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(SaveData.Win);
            WinInteger = (int)SaveData.Win;
        }
        else
        {
            WinInteger = (int)SaveData.Win;
        }
        offset = 0x1D34;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x1F8C;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(SaveData.Time.Ticks);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);

        offset = 0x22AC;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x243C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x25CC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.RTP;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x2694;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.SpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x275C;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusSpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x2824;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusIsPlayedCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x28EC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        goto End;
    #endregion

    SaveOverWrite:
    #region SaveOverWrite
        int[] _Number = new int[100];
        for (int i = 0; i < 100; i++)
        {
            _Number[i] = (int)ushortbuf[i];
            if (_Number[i] == 1)
            {
                Count = i;
            }
        }

        ushortbuf = new ushort[100];
        // int tmp = _Number[0];
        for (int i = 0; i < 100; i++)
        {
            //if (i < 99)
            //{
            //    _Number[i] = _Number[i + 1];
            //}
            //else
            //{
            //    _Number[i] = tmp;
            //}
            //ushortbuf[i] = (ushort)_Number[i];
            if (i <= Count)
            {
                _Number[i] = 100 - Count + i;
            }
            else
            {
                _Number[i] = i - Count;
            }
            ushortbuf[i] = (ushort)_Number[i];
        }

        offset = 0x1500;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (SaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(SaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)SaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }
        }

        offset = 0x15C8;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x4100;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x17BC;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (SaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (SaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(SaveData.Credit);
            CreditInteger = (int)SaveData.Credit;
        }
        else
        {
            CreditInteger = (int)SaveData.Credit;
        }

        offset = 0x1820;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x1A78;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.Demon;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x1C08;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.Bet;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x1CD0;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)SaveData.Odds;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        WinInteger = 0;
        WinDecimal = 0;
        if (SaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(SaveData.Win);
            WinInteger = (int)SaveData.Win;
        }
        else
        {
            WinInteger = (int)SaveData.Win;
        }
        offset = 0x1D34;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x1F8C;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(SaveData.Time.Ticks);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);

        offset = 0x22AC;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x243C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x25CC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.RTP;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x2694;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.SpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x275C;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusSpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x2824;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusIsPlayedCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x28EC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        goto End;
    #endregion

    End:
        ClearOpenClearPoint();
        ClearCoinInPoint();
        ClearTicketInPoint();
        ClearTicketOutPoint();
    }
    //讀取紀錄 輸入讀取的編號 1~100
    public void LoadHistory(int Number, out HistoryData LoadData)
    {
        UInt32 offset;
        HistoryData tmpData = new HistoryData();
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        offset = 0x1500;
        ushortbuf = new ushort[100];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (Count = 0; Count < 100; Count++)
        {
            if ((int)ushortbuf[Count] == Number)
            {
                tmpData.Number = (int)ushortbuf[Count];
                break;
            }
            if (Count == 99 && (int)ushortbuf[Count] != Number)
            {
                LoadData = tmpData;
                return;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        offset = 0x15C8;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x4100;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (int i = 0; i < 5; i++)
        {
            tmpData.RNG[i] = (int)bytebuf[i] + (int)ushortbuf[i];
        }

        offset = 0x17BC;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        if ((int)bytebuf[0] == 0)
        {
            tmpData.Bonus = false;
        }
        else
        {
            tmpData.Bonus = true;
        }

        offset = 0x1820;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        int CreditInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        int CreditDecimal = (int)ushortbuf[0];

        if (CreditDecimal == 0)
        {
            tmpData.Credit = CreditInteger;
        }
        else
        {
            tmpData.Credit = CreditInteger + ((double)CreditDecimal * 0.01);
        }

        offset = 0x1A78;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        tmpData.Demon = (int)byteuint[0];

        offset = 0x1C08;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.Bet = (int)ushortbuf[0];

        offset = 0x1CD0;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        tmpData.Odds = (int)bytebuf[0];

        offset = 0x1D34;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        int WinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        int WinDecimal = (int)ushortbuf[0];

        if (WinDecimal == 0)
        {
            tmpData.Win = WinInteger;
        }
        else
        {
            tmpData.Win = WinInteger + ((double)WinDecimal * 0.01);
        }

        offset = 0x1F8C;
        offset += (uint)((uint)0x08 * Count);
        byte[] EventTime;
        EventTime = new byte[8];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(EventTime, offset, (uint)EventTime.Length);
        tmpData.Time = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));

        offset = 0x22AC;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        tmpData.OpenPoint = (int)byteuint[0];

        offset = 0x243C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        tmpData.ClearPoint = (int)byteuint[0];

        offset = 0x25CC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.RTP = (int)ushortbuf[0];

        offset = 0x2694;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.SpecialTime = (int)ushortbuf[0];

        offset = 0x275C;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.BonusSpecialTime = (int)ushortbuf[0];

        offset = 0x2824;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.BonusIsPlayedCount = (int)ushortbuf[0];

        offset = 0x28EC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.BonusCount = (int)ushortbuf[0];

        LoadData = tmpData;
    }

    //儲存最大獎歷史紀錄 輸入要覆寫哪個編號(如果還有空間則不會覆寫 會直接使用新空間) 1~10
    public void SaveMaxGameHistory(int Number, HistoryData MaxGameSaveData)
    {

        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        int CreditInteger = 0, CreditDecimal = 0;
        int WinInteger = 0, WinDecimal = 0;
        byte[] EventTime;
        offset = 0x3000;
        ushortbuf = new ushort[10];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (Count = 0; Count < 10; Count++)
        {
            if (ushortbuf[Count] == 0 && Count == 0)
            {
                goto SaveStart;
            }
            else if (ushortbuf[Count] == 0)
            {
                goto SaveNew;
            }
            else if (ushortbuf[Count] == Number)
            {
                goto SaveOverWrite;
            }
        }
        return;

    SaveStart:

        offset = 0x3000;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)1;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }
        }
        offset = 0x3014;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x44E8;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x3046;
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3050;
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x308C;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x30B4;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x30C8;
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x30D2;
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x310E;
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);

        offset = 0x315E;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x3186;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x31AE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31C2;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31D6;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31EA;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31FE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        goto End;

    SaveNew:

        offset = 0x3000;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)(Count + 1);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }
        }
        offset = 0x3014;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x44E8;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x3046;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3050;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x308C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x30B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x30C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x30D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x310E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);

        offset = 0x315E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x3186;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x31AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        goto End;

    SaveOverWrite:

        int[] _Number = new int[10];
        for (int i = 0; i < 10; i++)
        {
            _Number[i] = (int)ushortbuf[i];
            if (_Number[i] == Number)
            {
                Count = i;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }
        }
        offset = 0x3014;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x44E8;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x3046;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3050;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x308C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x30B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x30C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x30D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x310E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);

        offset = 0x315E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x3186;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x31AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x31FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        goto End;

    End:
        return;
    }
    //讀取最大獎歷史紀錄 輸入讀取編號 1~100
    public void LoadMaxGameHistory(int Number, out HistoryData MaxGameLoadData)
    {
        UInt32 offset;
        HistoryData tmpData = new HistoryData();
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        offset = 0x3000;
        ushortbuf = new ushort[10];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (Count = 0; Count < 10; Count++)
        {
            if ((int)ushortbuf[Count] == Number)
            {
                tmpData.Number = (int)ushortbuf[Count];
                break;
            }
            if (Count == 9 && (int)ushortbuf[Count] != Number)
            {
                MaxGameLoadData = tmpData;
                return;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        offset = 0x3014;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x44E8;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (int i = 0; i < 5; i++)
        {
            tmpData.RNG[i] = (int)bytebuf[i] + (int)ushortbuf[i];
        }

        offset = 0x3046;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        if ((int)bytebuf[0] == 0)
        {
            tmpData.Bonus = false;
        }
        else
        {
            tmpData.Bonus = true;
        }

        offset = 0x3050;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        int CreditInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        int CreditDecimal = (int)ushortbuf[0];

        if (CreditDecimal == 0)
        {
            tmpData.Credit = CreditInteger;
        }
        else
        {
            tmpData.Credit = CreditInteger + ((double)CreditDecimal * 0.01);
        }

        offset = 0x308C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        tmpData.Demon = (int)byteuint[0];

        offset = 0x30B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.Bet = (int)ushortbuf[0];

        offset = 0x30C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        tmpData.Odds = (int)bytebuf[0];

        offset = 0x30D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        int WinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        int WinDecimal = (int)ushortbuf[0];

        if (WinDecimal == 0)
        {
            tmpData.Win = WinInteger;
        }
        else
        {
            tmpData.Win = WinInteger + ((double)WinDecimal * 0.01);
        }

        offset = 0x310E;
        offset += (uint)((uint)0x08 * Count);
        byte[] EventTime;
        EventTime = new byte[8];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(EventTime, offset, (uint)EventTime.Length);
        tmpData.Time = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));

        offset = 0x315E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        tmpData.OpenPoint = (int)byteuint[0];

        offset = 0x3186;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        tmpData.ClearPoint = (int)byteuint[0];

        offset = 0x31AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.RTP = (int)ushortbuf[0];

        offset = 0x31C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.SpecialTime = (int)ushortbuf[0];

        offset = 0x31D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.BonusSpecialTime = (int)ushortbuf[0];

        offset = 0x31EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.BonusIsPlayedCount = (int)ushortbuf[0];

        offset = 0x31FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.BonusCount = (int)ushortbuf[0];
        MaxGameLoadData = tmpData;
    }

    //儲存最大獎歷史紀錄Second 輸入要覆寫哪個編號(如果還有空間則不會覆寫 會直接使用新空間) 1~10
    public void SaveMaxGameHistorySecond(int Number, HistoryData MaxGameSaveData)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        int CreditInteger = 0, CreditDecimal = 0;
        int WinInteger = 0, WinDecimal = 0;
        byte[] EventTime;
        offset = 0x3300;
        ushortbuf = new ushort[10];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (Count = 0; Count < 10; Count++)
        {
            if (ushortbuf[Count] == 0 && Count == 0)
            {
                goto SaveStart;
            }
            else if (ushortbuf[Count] == 0)
            {
                goto SaveNew;
            }
            else if (ushortbuf[Count] == Number)
            {
                goto SaveOverWrite;
            }
        }
        return;

    SaveStart:

        offset = 0x3300;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)1;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }
        }
        offset = 0x3314;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x454C;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x3346;
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3350;
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x338C;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x33B4;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x33C8;
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x33D2;
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x340E;
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);

        offset = 0x345E;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x3486;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x34AE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34C2;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34D6;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34EA;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34FE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        goto End;

    SaveNew:

        offset = 0x3300;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)(Count + 1);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }
        }
        offset = 0x3314;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x454C;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x3346;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3350;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x338C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x33B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x33C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x33D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x340E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);

        offset = 0x345E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x3486;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x34AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        goto End;

    SaveOverWrite:

        int[] _Number = new int[10];
        for (int i = 0; i < 10; i++)
        {
            _Number[i] = (int)ushortbuf[i];
            if (_Number[i] == Number)
            {
                Count = i;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }
        }
        offset = 0x3314;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x454C;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x3346;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3350;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x338C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x33B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x33C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x33D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x340E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(EventTime, offset, (uint)EventTime.Length);

        offset = 0x345E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x3486;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

        offset = 0x34AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        offset = 0x34FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);

        goto End;

    End:
        return;
    }
    //讀取最大獎歷史紀錄Second 輸入讀取編號 1~10
    public void LoadMaxGameHistorySecond(int Number, out HistoryData MaxGameLoadData)
    {
        UInt32 offset;
        HistoryData tmpData = new HistoryData();
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        offset = 0x3300;
        ushortbuf = new ushort[10];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (Count = 0; Count < 10; Count++)
        {
            if ((int)ushortbuf[Count] == Number)
            {
                tmpData.Number = (int)ushortbuf[Count];
                break;
            }
            if (Count == 9 && (int)ushortbuf[Count] != Number)
            {
                MaxGameLoadData = tmpData;
                return;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        offset = 0x3314;
        offset += (uint)((uint)0x05 * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        offset = 0x454C;
        offset += (uint)((uint)0x0A * Count);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        for (int i = 0; i < 5; i++)
        {
            tmpData.RNG[i] = (int)bytebuf[i] + (int)ushortbuf[i];
        }

        offset = 0x3346;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        if ((int)bytebuf[0] == 0)
        {
            tmpData.Bonus = false;
        }
        else
        {
            tmpData.Bonus = true;
        }

        offset = 0x3350;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        int CreditInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        int CreditDecimal = (int)ushortbuf[0];

        if (CreditDecimal == 0)
        {
            tmpData.Credit = CreditInteger;
        }
        else
        {
            tmpData.Credit = CreditInteger + ((double)CreditDecimal * 0.01);
        }

        offset = 0x338C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        tmpData.Demon = (int)byteuint[0];

        offset = 0x33B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.Bet = (int)ushortbuf[0];

        offset = 0x33C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        tmpData.Odds = (int)bytebuf[0];

        offset = 0x33D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        int WinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        int WinDecimal = (int)ushortbuf[0];

        if (WinDecimal == 0)
        {
            tmpData.Win = WinInteger;
        }
        else
        {
            tmpData.Win = WinInteger + ((double)WinDecimal * 0.01);
        }

        offset = 0x340E;
        offset += (uint)((uint)0x08 * Count);
        byte[] EventTime;
        EventTime = new byte[8];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(EventTime, offset, (uint)EventTime.Length);
        tmpData.Time = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));

        offset = 0x345E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        tmpData.OpenPoint = (int)byteuint[0];

        offset = 0x3486;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        tmpData.ClearPoint = (int)byteuint[0];

        offset = 0x34AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.RTP = (int)ushortbuf[0];

        offset = 0x34C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.SpecialTime = (int)ushortbuf[0];

        offset = 0x34D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.BonusSpecialTime = (int)ushortbuf[0];

        offset = 0x34EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.BonusIsPlayedCount = (int)ushortbuf[0];

        offset = 0x34FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        tmpData.BonusCount = (int)ushortbuf[0];
        MaxGameLoadData = tmpData;
    }





    //儲存目前開洗分 輸入開分(true) 洗分(false) + 值
    public void SaveOpenClearPoint(bool OpenClear, int Value)
    {
        UInt32 offset;
        uint[] byteuint;
        if (OpenClear)
        {
            offset = 0x60;
            byteuint = new uint[1];
            byteuint[0] = (uint)Value;
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        }
        else
        {
            offset = 0x64;
            byteuint = new uint[1];
            byteuint[0] = (uint)Value;
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        }
    }
    //讀取目前開洗分 輸入開分(true) 洗分(false) + 值
    public int LoadOpenClearPoint(bool OpenClear)
    {
        int Value;
        UInt32 offset;
        uint[] byteuint;
        if (OpenClear)
        {
            offset = 0x60;
            byteuint = new uint[1];
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
            Value = (int)byteuint[0];
        }
        else
        {
            offset = 0x64;
            byteuint = new uint[1];
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
            Value = (int)byteuint[0];
        }
        return Value;
    }

    public void SaveCoinInPoint(int Value)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x80;
        byteuint = new uint[1];
        byteuint[0] = (uint)Value;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

    }

    public int LoadCoinInPoint()
    {

        int Value;
        UInt32 offset;
        uint[] byteuint;
        offset = 0x80;

        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        Value = (int)byteuint[0];

        return Value;
    }

    //清空目前開洗分
    public void ClearOpenClearPoint()
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x60;
        byteuint = new uint[2];
        byteuint[0] = 0;
        byteuint[1] = 0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

    }

    public void ClearCoinInPoint()
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x80;
        byteuint = new uint[1];
        byteuint[0] = 0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);

    }
    public void ClearTicketInPoint()
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x4636;
        uint[] ushortbuf;
        byteuint = new uint[1];
        ushortbuf = new uint[1];
        byteuint[0] = 0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        offset = 0x4640;
        ushortbuf[0] = 0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(ushortbuf, offset, (uint)ushortbuf.Length);
    }
    public void ClearTicketOutPoint()
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x4642;
        uint[] ushortbuf;
        byteuint = new uint[1];
        ushortbuf = new uint[1];
        byteuint[0] = 0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        offset = 0x4646;
        ushortbuf[0] = 0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(ushortbuf, offset, (uint)ushortbuf.Length);
    }



    public void SaveClearCount(int clearCount)//紀錄剩餘洗分次數
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x40;
        byteuint = new uint[1];
        byteuint[0] = (uint)clearCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    public int LoadClearCount()//讀取剩餘洗分次數
    {
        int clearCount;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x40;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        clearCount = (int)byteuint[0];
        return clearCount;
    }

    public void SaveMonthCheckDate(int month)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x68;
        byteuint = new uint[1];
        byteuint[0] = (uint)month;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    public int LoadMonthCheckData()
    {
        int monthData;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x68;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        monthData = (int)byteuint[0];
        return monthData;
    }
    //儲存是否要月報
    public void SaveIsMonthCheck(bool isMonthcheck)
    {
        UInt32 offset = 0x6C;
        byte[] bytebuf = new byte[1];
        if (isMonthcheck) bytebuf[0] = (byte)1;
        else bytebuf[0] = (byte)0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取是否要月報
    public bool LoadIsMonthCheck()
    {
        UInt32 offset = 0x6C;
        byte[] bytebuf = new byte[1];
        int tmp;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        tmp = (int)bytebuf[0];
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    //儲存票紙序號
    public void SaveTicketSerial(string serial)
    {
        UInt32 offset = 0x4656;
        if (String.IsNullOrWhiteSpace(serial))
        {
            ushort[] ushortbuf = new ushort[1];
            ushortbuf[0] = 0;
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        }
        else
        {
            string[] saveSerial = serial.Split('-');
            for (int i = 0; i < 4; i++)
            {
                ushort[] ushortbuf = new ushort[1];
                ushortbuf[0] = (ushort)ushort.Parse(saveSerial[i]);
                Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
                offset += 0x02;
            }
        }
    }
    //讀取票紙序號
    public string LoadTicketSerial()
    {
        UInt32 offset = 0x4656;
        ushort[] ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        string loadSerial = null;
        if (ushortbuf[0] == 0)
        {
            return "";
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
                loadSerial += ushortbuf[0].ToString().PadLeft(4, '0');
                if (i < 3) loadSerial += "-";
                offset += 0x02;
            }
            return loadSerial;
        }
    }
    //儲存是否要開啟開分洗分鍵
    public void SaveOpenScoreButtonSet(bool isOpenSet)
    {
        UInt32 offset = 0x8A;
        byte[] bytebuf = new byte[1];
        if (isOpenSet) bytebuf[0] = (byte)1;
        else bytebuf[0] = (byte)0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取是否要要開啟開分洗分鍵
    public bool LoadOpenScoreButtonSet()
    {
        UInt32 offset = 0x8A;
        byte[] bytebuf = new byte[1];
        int tmp;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        tmp = (int)bytebuf[0];
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    int[] rtpArray;
    int nowRtp;
    bool rtpAll;
    public void InitializeSetting()//初始化設定
    {

        //儲存備份紀錄
        SaveBackupStatistcalData(LoadBackupTotalOpenPoint() + LoadTotalOpenPoint(), LoadBackupTotalClearPoint() + LoadTotalClearPoint(), LoadBackupTotalCoinIn() + LoadTotalCoinIn(),
            LoadBackupTotalBet() + LoadTotalBet(), LoadBackupTotalWin() + LoadTotalWin(), LoadBackupTotalGamePlay() + LoadTotalGamePlay(), LoadBackupTotalWinGamePlay() + LoadTotalWinGamePlay());
        //初始化總帳戶
        SaveTotalStatistcalData(initData.OpenPointTotal, initData.ClearPointTotal, initData.CashIn, initData.BetCredit, initData.WinScore, initData.GameCount, initData.WinCount);
        //初始化班帳戶
        SaveClassStatistcalData(initData.OpenPointTotal, initData.ClearPointTotal, initData.CashIn, initData.BetCredit, initData.WinScore, initData.GameCount, initData.WinCount);
        //初始化密碼
        SavePassword(initData.AdminPassword);

        //初始化鈔機設定
        SaveBanknoteMachineSetting(initData.UBA_Enable == 1 ? true : false);
        //初始化開分設定
        SaveOpenPointSetting(initData.OpenPointOnce);
        //初始化洗分設定
        SaveClearPointSetting(initData.ClearPointOnce);
        //初始化最大押注設定
        SaveMaxOdd(initData.MaxOdds);

        //初始化最大彩分設定
        SaveMaxCredit(initData.MaxCredit);

        //初始化最大贏分設定
        SaveMaxWin(initData.MaxWinScore);
        ClearHistory();
#if !UNITY_EDITOR
        Mod_Data.maxOdds = LoadMaxOdd();
        Mod_Data.maxCredit = LoadMaxCredit();
        Mod_Data.maxWin = LoadMaxWin();
#endif
        //初始化倍率開啟設定
        SaveDenomOption(initData.DenomSelect);
        LoadDenomOption(out Mod_Data.denomOpenArray);
        //初始化RTP設定
        SaveRTPSetting(initRtp.RTP_Array, initRtp.RTP_All);

        GameObject.Find("BackEndManager").GetComponent<NewSramManager>().LoadRTPSetting(out rtpArray, out rtpAll);

        if (Mod_Data.Denom == 0.01) { nowRtp = rtpArray[8]; }
        else if (Mod_Data.Denom == 0.02) { nowRtp = rtpArray[7]; }
        else if (Mod_Data.Denom == 0.025) { nowRtp = rtpArray[6]; }
        else if (Mod_Data.Denom == 0.05) { nowRtp = rtpArray[5]; }
        else if (Mod_Data.Denom == 0.1) { nowRtp = rtpArray[4]; }
        else if (Mod_Data.Denom == 0.25) { nowRtp = rtpArray[3]; }
        else if (Mod_Data.Denom == 0.5) { nowRtp = rtpArray[2]; }
        else if (Mod_Data.Denom == 1) { nowRtp = rtpArray[1]; }
        else if (Mod_Data.Denom == 2.5) { nowRtp = rtpArray[0]; }

        Mod_Data.RTPsetting = nowRtp;
        GameObject.Find("GameController").GetComponent<Mod_MathController>().ChangeMathFile(nowRtp);
        //初始化是否有存值設定
        Mod_Data.credit = 0;
        Mod_Data.Win = 0;
        Mod_Data.Pay = 0;
        Mod_Data.odds = 1;
        Mod_Data.Denom = 1;
        Mod_Data.Bet = Mod_Data.BetOri;
        //SaveIsSramTrue(false);
        //SaveEventRecored();
    }
    public void DebugSram()
    {
        //Debug.Log("LoadTotalOpenPoint:" + LoadTotalOpenPoint());
        //Debug.Log("LoadTotalClearPoint:" + LoadTotalClearPoint());
        //Debug.Log("LoadTotalCoinIn:" + LoadTotalCoinIn());
        //Debug.Log("LoadTotalBet:" + LoadTotalBet());
        //Debug.Log("LoadTotalWin:" + LoadTotalWin());
        //Debug.Log("LoadTotalGamePlay:" + LoadTotalGamePlay());
        //Debug.Log("LoadTotalWinGamePlay:" + LoadTotalWinGamePlay());
        //Debug.Log("LoadPassword:" + LoadPassword());
        //Debug.Log("LoadOpenPointSetting:" + LoadOpenPointSetting());
        //Debug.Log("LoadClearPointSetting:" + LoadClearPointSetting());
        //Debug.Log("LoadMaxOdd:" + LoadMaxOdd());
        //Debug.Log("LoadMaxCredit:" + LoadMaxCredit());
        //Debug.Log("LoadMaxWin:" + LoadMaxWin());

        //Debug.Log("SaveIsSramTrue:" + LoadIsSramTrue());
    }

    public void InitializeClaseAccount()
    {
        StatisticalData initData = new StatisticalData();
        SaveClassStatistcalData(initData.OpenPointTotal, initData.ClearPointTotal, initData.CashIn, initData.BetCredit, initData.WinScore, initData.GameCount, initData.WinCount);
    }

    //COMPort				int		1	0x68 6D
    //BillName				int		1	0x69 6E
    //TicketEnable			bool	1	0x6A 6F

    //儲存鈔機COMPort
    public void SaveBanknoteMachineCOMPort(string COMPort)
    {
        UInt32 offset = 0x6D;
        byte[] bytebuf = new byte[1];
        switch (COMPort)
        {
            case "COM1":
                bytebuf[0] = 1;
                break;
            case "COM2":
                bytebuf[0] = 2;
                break;
            case "COM3":
                bytebuf[0] = 3;
                break;
            case "COM4":
                bytebuf[0] = 4;
                break;
            case "COM5":
                bytebuf[0] = 5;
                break;
            case "COM6":
                bytebuf[0] = 6;
                break;
            default:
                return;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取鈔機COMPort
    public void LoadBanknoteMachineCOMPort(out string COMPort)
    {
        UInt32 offset = 0x6D;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        switch (bytebuf[0])
        {
            case 1:
                COMPort = "COM1";
                break;
            case 2:
                COMPort = "COM2";
                break;
            case 3:
                COMPort = "COM3";
                break;
            case 4:
                COMPort = "COM4";
                break;
            case 5:
                COMPort = "COM5";
                break;
            case 6:
                COMPort = "COM6";
                break;
            default:
                COMPort = "COM1";
                break;
        }
    }
    //儲存鈔機型號
    public void SaveBanknoteMachineName(string BillName)
    {
        UInt32 offset = 0x6E;
        byte[] bytebuf = new byte[1];
        switch (BillName)
        {
            case "JCM":
                bytebuf[0] = 1;
                break;
            case "MEI":
                bytebuf[0] = 2;
                break;
            case "ICT":
                bytebuf[0] = 3;
                break;
            default:
                return;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取鈔機型號
    public void LoadBanknoteMachineName(out string BillName)
    {
        UInt32 offset = 0x6E;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        switch (bytebuf[0])
        {
            case 1:
                BillName = "JCM";
                break;
            case 2:
                BillName = "MEI";
                break;
            case 3:
                BillName = "ICT";
                break;
            default:
                BillName = "JCM";
                break;
        }
    }
    //儲存吸票開關
    public void SaveBanknoteMachineTicketEnable(bool Enable)
    {
        UInt32 offset = 0x6F;
        byte[] bytebuf = new byte[1];
        if (Enable)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取吸票開關
    public void LoadBanknoteMachineTicketEnable(out bool Enable)
    {
        UInt32 offset = 0x6F;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        if (bytebuf[0] == 1)
        {
            Enable = true;
        }
        else
        {
            Enable = false;
        }
    }

    //newok
    public void SaveTotalTicketIn(double TotalTicketIn)//存總入票
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;

        if (TotalTicketIn % 1 != 0)
        {
            TotalTicketInDecimal = (int)SaveDecimalPointHandle(TotalTicketIn);
            TotalTicketInInteger = (int)TotalTicketIn;
        }
        else
        {
            TotalTicketInInteger = (int)TotalTicketIn;
        }

        offset = 0x4600;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalTicketInInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        // result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4604;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalTicketInDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        // result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public void SaveTotalTicketOut(double TotalTicketOut)//存總出票
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalTicketOutInteger = 0;
        int TotalTicketOutDecimal = 0;

        if (TotalTicketOut % 1 != 0)
        {
            TotalTicketOutDecimal = (int)SaveDecimalPointHandle(TotalTicketOut);
            TotalTicketOutInteger = (int)TotalTicketOut;
        }
        else
        {
            TotalTicketOutInteger = (int)TotalTicketOut;
        }

        offset = 0x4618;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalTicketOutInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        // result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4622;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalTicketOutDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        // result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    //newok
    public double LoadTotalTicketIn()//讀取總入票
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalTicketIn;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;

        offset = 0x4600;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalTicketInInteger = (int)byteuint[0];
        //result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        //TotalTicketInInteger = (int)byteuint;

        offset = 0x4604;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalTicketInDecimal = (int)ushortbuf[0];

        //result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        // TotalTicketInDecimal = (int)ushortbuf;
        if (TotalTicketInDecimal == 0)
        {
            TotalTicketIn = TotalTicketInInteger;
        }
        else
        {
            TotalTicketIn = TotalTicketInInteger + (TotalTicketInDecimal * 0.01);
        }
        return TotalTicketIn;
    }
    public double LoadTotalTicketOut()//讀取總出票
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalTickeOut;
        int TotalTickeOutInteger = 0;
        int TotalTickeOutDecimal = 0;

        offset = 0x4618;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalTickeOutInteger = (int)byteuint[0];
        //result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        //TotalTickeOutInteger = (int)byteuint;

        offset = 0x4622;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalTickeOutDecimal = (int)ushortbuf[0];
        // result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        // TotalTickeOutDecimal = (int)ushortbuf;
        if (TotalTickeOutDecimal == 0)
        {
            TotalTickeOut = TotalTickeOutInteger;
        }
        else
        {
            TotalTickeOut = TotalTickeOutInteger + (TotalTickeOutDecimal * 0.01);
        }
        return TotalTickeOut;
    }

    //newok
    public void SaveClassTicketIn(double ClassTicketIn)//存班入票
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassTicketInInteger = 0;
        int ClassTicketInDecimal = 0;

        if (ClassTicketIn % 1 != 0)
        {
            ClassTicketInDecimal = (int)SaveDecimalPointHandle(ClassTicketIn);
            ClassTicketInInteger = (int)ClassTicketIn;
        }
        else
        {
            ClassTicketInInteger = (int)ClassTicketIn;
        }

        offset = 0x4606;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassTicketInInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        //result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4610;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassTicketInDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        //  result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public void SaveClassTicketOut(double ClassTicketOut)//存班出票
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassTicketOutInteger = 0;
        int ClassTicketOutDecimal = 0;

        if (ClassTicketOut % 1 != 0)
        {
            ClassTicketOutDecimal = (int)SaveDecimalPointHandle(ClassTicketOut);
            ClassTicketOutInteger = (int)ClassTicketOut;
        }
        else
        {
            ClassTicketOutInteger = (int)ClassTicketOut;
        }

        offset = 0x4624;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassTicketOutInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        // result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4628;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassTicketOutDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        //  result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }

    //newok
    public double LoadClassTicketIn()//讀取班入票
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalTicketIn;
        int ClassTicketInInteger = 0;
        int ClassTicketInDecimal = 0;

        offset = 0x4606;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassTicketInInteger = (int)byteuint[0];
        //result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        // ClassTicketInInteger = (int)byteuint;

        offset = 0x4610;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        ClassTicketInDecimal = (int)ushortbuf[0];
        //result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        // ClassTicketInDecimal = (int)ushortbuf;
        if (ClassTicketInDecimal == 0)
        {
            TotalTicketIn = ClassTicketInInteger;
        }
        else
        {
            TotalTicketIn = ClassTicketInInteger + (ClassTicketInDecimal * 0.01);
        }
        return TotalTicketIn;
    }
    public double LoadClassTicketOut()//讀取班出票
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalTickeOut;
        int ClassTickeOutInteger = 0;
        int ClassTickeOutDecimal = 0;

        offset = 0x4624;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        ClassTickeOutInteger = (int)byteuint[0];
        // result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        //ClassTickeOutInteger = (int)byteuint;

        offset = 0x4628;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        ClassTickeOutDecimal = (int)ushortbuf[0];
        // result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        //  ClassTickeOutDecimal = (int)ushortbuf;
        if (ClassTickeOutDecimal == 0)
        {
            TotalTickeOut = ClassTickeOutInteger;
        }
        else
        {
            TotalTickeOut = ClassTickeOutInteger + (ClassTickeOutDecimal * 0.01);
        }
        return TotalTickeOut;
    }
    //newok
    public void SaveTicketInPoint(double TicketIn)//存入票金額
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;

        if (TicketIn % 1 != 0)
        {
            TotalTicketInDecimal = (int)SaveDecimalPointHandle(TicketIn);
            TotalTicketInInteger = (int)TicketIn;
        }
        else
        {
            TotalTicketInInteger = (int)TicketIn;
        }

        offset = 0x4636;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalTicketInInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        //  result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4640;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalTicketInDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        // result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public double LoadTicketInPoint()//讀取入票金額
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TicketIn;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;

        offset = 0x4636;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalTicketInInteger = (int)byteuint[0];
        //result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        // TotalTicketInInteger = (int)byteuint;

        offset = 0x4640;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalTicketInDecimal = (int)ushortbuf[0];
        // result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        // TotalTicketInDecimal = (int)ushortbuf;
        if (TotalTicketInDecimal == 0)
        {
            TicketIn = TotalTicketInInteger;
        }
        else
        {
            TicketIn = TotalTicketInInteger + (TotalTicketInDecimal * 0.01);
        }
        return TicketIn;
    }
    public void SaveTicketOutPoint(double TicketOut)//存出票金額
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalTicketOutInteger = 0;
        int TotalTicketOutDecimal = 0;

        if (TicketOut % 1 != 0)
        {
            TotalTicketOutDecimal = (int)SaveDecimalPointHandle(TicketOut);
            TotalTicketOutInteger = (int)TicketOut;
        }
        else
        {
            TotalTicketOutInteger = (int)TicketOut;
        }

        offset = 0x4642;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalTicketOutInteger;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
        // result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        //  result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4646;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalTicketOutDecimal;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        //  result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public double LoadTicketOutPoint()//讀取出票金額
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TicketOut;
        int TotalTicketOutInteger = 0;
        int TotalTicketOutDecimal = 0;

        offset = 0x4642;
        // result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        //  TotalTicketOutInteger = (int)byteuint;
        byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        TotalTicketOutInteger = (int)byteuint[0];
        offset = 0x4646;
        ushortbuf = new ushort[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram16(ushortbuf, offset, (uint)ushortbuf.Length);
        TotalTicketOutDecimal = (int)ushortbuf[0];
        // result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        //TotalTicketOutDecimal = (int)ushortbuf;
        if (TotalTicketOutDecimal == 0)
        {
            TicketOut = TotalTicketOutInteger;
        }
        else
        {
            TicketOut = TotalTicketOutInteger + (TotalTicketOutDecimal * 0.01);
        }
        return TicketOut;
    }
    //newok
    //儲存吸鈔還是吸票
    public void SaveBillMachineCashOrTicketEnable(int select)
    {
        UInt32 offset = 0x78;
        byte[] bytebuf = new byte[1];
        switch (select)
        {
            case 0: bytebuf[0] = 0; break;
            case 1: bytebuf[0] = 1; break;
            case 2: bytebuf[0] = 2; break;
            default: bytebuf[0] = 0; break;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        // result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取吸鈔還是吸票
    public void LoadBillMachineCashOrTicketEnable(out int select)
    {
        UInt32 offset = 0x78;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        //  result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        switch (bytebuf[0])
        {
            case 0: select = 0; break;
            case 1: select = 1; break;
            case 2: select = 2; break;
            default: select = 0; break;
        }
    }
    //儲存手付狀態
    public void SaveHandPayStatus(int HandPay)
    {
        UInt32 offset = 0x79;
        byte[] bytebuf = new byte[1];
        bytebuf[0] = (byte)HandPay;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取手付狀態
    public int LoadHandPayStatus()
    {
        UInt32 offset = 0x79;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        return bytebuf[0];
    }
    //儲存票機開關狀態
    public void SavePrinterEnable(bool Enable)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        if (Enable)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        // result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取票機開關狀態
    public void LoadPrinterEnable(out bool Enable)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        // result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        if (bytebuf[0] == 1)
        {
            Enable = true;
        }
        else
        {
            Enable = false;
        }
    }
    //儲存票機COMPort
    public void SavePrinterCOMPort(int COMPort)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        bytebuf[0] = (byte)COMPort;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        //  result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取票機COMPort
    public void LoadPrinterCOMPort(out string COMPort)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        // result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        COMPort = "COM" + bytebuf[0];
        if (COMPort == "COM0") COMPort = "COM1";
    }
    //儲存邏輯機門狀態
    public void SaveLogicDoorStatus(bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
        //  result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取邏輯機門狀態
    public void LoadLogicDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        //  result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
    }

    //儲存上機門狀態
    public void SaveMainDoorStatus(bool Error)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        //Debug.Log("SaveMainDoorStatus: " + bytebuf[0]);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取上機門狀態
    public void LoadMainDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
        //Debug.Log("LoadMainDoorStatus: " + bytebuf[0]);
    }
    //儲存下機門狀態
    public void SaveBellyDoorStatus(bool Error)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        //Debug.Log("SaveBellyDoorStatus: " + bytebuf[0]);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取下機門狀態
    public void LoadBellyDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
        //Debug.Log("LoadBellyDoorStatus: " + bytebuf[0]);
    }
    //儲存鈔機門狀態
    public void SaveCashDoorStatus(bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        //Debug.Log("SaveCashDoorStatus: " + bytebuf[0]);
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram8(bytebuf, offset, (uint)bytebuf.Length);
    }
    //讀取鈔機門狀態
    public void LoadCashDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram8(bytebuf, offset, (uint)bytebuf.Length);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
        //Debug.Log("LoadCashDoorStatus: " + bytebuf[0]);
    }
    //儲存過場動畫 魚的倍率
    public void SaveBonusFish(int fishNum, int Muitl)
    {
        UInt32 offset = 0x5000;
        uint[] byteuint;
        offset = offset + (uint)(fishNum * 4);
        byteuint = new uint[1];
        byteuint[0] = (uint)Muitl;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    //讀取過場動畫 魚的倍率
    public int LoadBonusFish(int fishNum)
    {
        UInt32 offset = 0x5000;
        uint[] byteuint;
        offset = offset + (uint)(fishNum * 4);
        byteuint = new uint[1];

        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        return (int)byteuint[0];
    }
    //儲存過場動畫 船的場次
    public void SaveBonusBoat(int BoatNum, int time)
    {
        UInt32 offset = 0x5014;
        uint[] byteuint;
        offset = offset + (uint)(BoatNum * 4);
        byteuint = new uint[1];
        byteuint[0] = (uint)time;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    //讀取過場動畫 船的場次
    public int LoadBonusBoat(int BoatNum)
    {
        UInt32 offset = 0x5014;
        uint[] byteuint;
        offset = offset + (uint)(BoatNum * 4);
        byteuint = new uint[1];

        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        return (int)byteuint[0];
    }
    //儲存過場動畫 已選擇寶箱倍率
    public void SaveSpeicalTime(int SpeicalTime)
    {
        UInt32 offset = 0x5028;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)SpeicalTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    //讀取過場動畫 已選擇寶箱倍率
    public int LoadSpeicalTime()
    {
        UInt32 offset = 0x5028;
        uint[] byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        return (int)byteuint[0];
    }
    //儲存過場動畫 已選擇船的場次
    public void SaveBonusCount(int BonusCount)
    {
        UInt32 offset = 0x502C;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    //讀取過場動畫 已選擇船的場次
    public int LoadBonusCount()
    {
        UInt32 offset = 0x502C;
        uint[] byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        //Debug.Log("byteuint" + byteuint);
        return (int)byteuint[0];
    }
    //儲存過場動畫 使用者選擇的寶箱路線
    public void SaveUserSelectedSpeicalTime(int SpeicalTime)
    {
        UInt32 offset = 0x5030;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)SpeicalTime;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    //讀取過場動畫 使用者選擇的寶箱路線
    public int LoadUserSelectedSpeicalTime()
    {
        UInt32 offset = 0x5030;
        uint[] byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        return (int)byteuint[0];
    }
    //儲存過場動畫 使用者選擇的船路線
    public void SaveUserSelectedBonusCount(int BonusCount)
    {
        UInt32 offset = 0x5034;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)BonusCount;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    //讀取過場動畫 使用者選擇的船路線
    public int LoadUserSelectedBonusCount()
    {
        UInt32 offset = 0x5034;
        uint[] byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        return (int)byteuint[0];
    }
    //儲存本機場次編號
    public void SaveLocalGameRound(int Round)
    {
        UInt32 offset = 0x4648;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)Round;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    //讀取本機場次編號
    public void LoadLocalGameRound(out int Round)
    {
        UInt32 offset = 0x4648;
        uint[] byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        Round = (int)byteuint[0];
    }
    //儲存本機Bonus場次編號
    public void SaveLocalBonusGameRound(int BonusRound)
    {
        UInt32 offset = 0x4652;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)BonusRound;
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_WriteSram32(byteuint, offset, (uint)byteuint.Length);
    }
    //讀取本Bonus機場次編號
    public void LoadLocalBonusGameRound(out int BonusRound)
    {
        UInt32 offset = 0x4652;
        uint[] byteuint = new uint[1];
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_ReadSram32(byteuint, offset, (uint)byteuint.Length);
        BonusRound = (int)byteuint[0];
    }
#endif

    #endregion

    #region QXTSram
#if QXT

    StatisticalData initData = new StatisticalData();
    RTP_SettingData initRtp = new RTP_SettingData();

    public bool IsInitialized { get; set; }
    public bool HasInterrupts { get; set; }
    public OPMODE_IDS OpMode { get; set; }
    public NVRAMNotificationEventArgs InterruptData { get; set; }

    public DateTime InterruptTimestamp { get; set; }

    public NewSramManager()
    {
        IsInitialized = false;
        HasInterrupts = false;
        OpMode = OPMODE_IDS.OPMODE_NORMAL;
        InterruptData = null;
        InterruptTimestamp = DateTime.MinValue;
    }
    int result;
    NVRAMDevice dev = new NVRAMDevice();
    static NewSramManager ms = new NewSramManager();
    // Start is called before the first frame update
    private void Awake()
    {
        result = NVRAM.qxt_nvram_initEx(dev);
        //NVRAM_Connect();
    }

    void Start()
    {
        //result = NVRAM.qxt_nvram_initEx(dev);
    }
    //uint SaveIn;
    //uint SaveOffset;
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    result = NVRAM.qxt_nvram_get_size(out Size);
        //    T1.text = Size.ToString();
        //}
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    result = NVRAM.qxt_nvram_initEx(dev);
        //}
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    SaveOffset = uint.Parse(T2.text);
        //    SaveIn = uint.Parse(T3.text);
        //    result = NVRAM.qxt_nvram_writedword(SaveOffset, SaveIn);
        //    T1.text = (int)SaveOffset + "  " + (int)SaveIn;
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    SaveOffset = uint.Parse(T2.text);
        //    SaveIn = 0;
        //    result = NVRAM.qxt_nvram_readdword(SaveOffset, out SaveIn);
        //    T1.text = ((int)SaveOffset) + "  " + SaveIn;
        //}
    }
    //連接硬體 若未連接控制NVRAM
    public void NVRAM_Connect()
    {
        if (!ms.IsInitialized)
        {
            result = NVRAM.qxt_nvram_initEx(dev);
            ms.IsInitialized = true;
        }
    }
    //中斷硬體連接 若未中斷則可能發生硬體遭占用的問題
    public void NVRAM_Disconnect()
    {
        if (ms.IsInitialized)
        {
            NVRAM.qxt_nvram_terminate();
            ms.IsInitialized = false;
        }
    }

    #region 存讀總紀錄
    //儲存總紀錄
    public void SaveTotalStatistcalData(int TotalOpenPoint, int TotalClearPoint, int TotalCoinIn, double TotalBet, double TotalWin, int TotalGamePlay, int TotalWinGamePlay, double TicketIn, double TicketOut)//開分 洗分 總押注 總贏分 遊戲場次 贏場次
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int TotalBetInteger = 0, TotalWinInteger = 0, TotalTicketInInteger = 0, TotalTicketOutInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0, TotalTicketInDecimal = 0, TotalTicketOutDecimal = 0;
        if (TotalBet % 1 != 0)
        {
            TotalBetDecimal = (int)SaveDecimalPointHandle(TotalBet);
            TotalBetInteger = (int)TotalBet;
        }
        else
        {
            TotalBetInteger = (int)TotalBet;
        }

        if (TotalWin % 1 != 0)
        {
            TotalWinDecimal = (int)SaveDecimalPointHandle(TotalWin);
            TotalWinInteger = (int)TotalWin;
        }
        else
        {
            TotalWinInteger = (int)TotalWin;
        }
        if (TicketIn % 1 != 0)
        {
            TotalTicketInDecimal = (int)SaveDecimalPointHandle(TicketIn);
            TotalTicketInInteger = (int)TicketIn;
        }
        else
        {
            TotalTicketInInteger = (int)TicketIn;
        }
        if (TicketOut % 1 != 0)
        {
            TotalTicketOutDecimal = (int)SaveDecimalPointHandle(TicketOut);
            TotalTicketOutInteger = (int)TicketOut;
        }
        else
        {
            TotalTicketOutInteger = (int)TicketOut;
        }

        offset = 0x00;
        byteuint = (uint)TotalOpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x04;
        byteuint = (uint)TotalClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);


        offset = 0x70;
        byteuint = (uint)TotalCoinIn;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x14;
        byteuint = (uint)TotalGamePlay;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x18;
        byteuint = (uint)TotalWinGamePlay;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x08;
        byteuint = (uint)TotalBetInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset += 0x04;
        ushortbuf = (ushort)TotalBetDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);

        offset = 0x0E;
        byteuint = (uint)TotalWinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset += 0x04;
        ushortbuf = (ushort)TotalWinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);


        offset = 0x4600;
        byteuint = (uint)TotalTicketInInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4604;
        ushortbuf = (ushort)TotalTicketInDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
        offset = 0x4618;
        byteuint = (uint)TotalTicketOutInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4622;
        ushortbuf = (ushort)TotalTicketOutDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);


    }

    public void SaveTotalOpenPoint(int TotalOpenPoint)//總開分
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x00;
        byteuint = (uint)TotalOpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

    }
    public void SaveTotalClearPoint(int TotalClearPoint)//總洗分
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x04;
        byteuint = (uint)TotalClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

    }
    public void SaveTotalCoinIn(int TotalCoinIn)//總入鈔
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x70;
        byteuint = (uint)TotalCoinIn;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
    }
    public void SaveTotalTicketIn(double TotalTicketIn)//總入票
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;
        if (TotalTicketIn % 1 != 0)
        {
            TotalTicketInDecimal = (int)SaveDecimalPointHandle(TotalTicketIn);
            TotalTicketInInteger = (int)TotalTicketIn;
        }
        else
        {
            TotalTicketInInteger = (int)TotalTicketIn;
        }
        offset = 0x4600;
        byteuint = (uint)TotalTicketInInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4604;
        ushortbuf = (ushort)TotalTicketInDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public void SaveTotalTicketOut(double TotalTicketOut)//總出票
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int TotalTicketOutInteger = 0;
        int TotalTicketOutDecimal = 0;
        if (TotalTicketOut % 1 != 0)
        {
            TotalTicketOutDecimal = (int)SaveDecimalPointHandle(TotalTicketOut);
            TotalTicketOutInteger = (int)TotalTicketOut;
        }
        else
        {
            TotalTicketOutInteger = (int)TotalTicketOut;
        }
        offset = 0x4618;
        byteuint = (uint)TotalTicketOutInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4622;
        ushortbuf = (ushort)TotalTicketOutDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }

    public void SaveTotalBet(double TotalBet)//總押注
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int TotalBetInteger = 0;
        int TotalBetDecimal = 0;
        if (TotalBet % 1 != 0)
        {
            TotalBetDecimal = (int)SaveDecimalPointHandle(TotalBet);
            TotalBetInteger = (int)TotalBet;
        }
        else
        {
            TotalBetInteger = (int)TotalBet;
        }

        offset = 0x08;
        byteuint = (uint)TotalBetInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset += 0x04;
        ushortbuf = (ushort)TotalBetDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);

    }
    public void SaveTotalWin(double TotalWin)//總贏分
    {
        //Debug.Log("SaveTotalWin: " + TotalWin);
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int TotalWinInteger = 0;
        int TotalWinDecimal = 0;
        if (TotalWin % 1 != 0)
        {
            TotalWinDecimal = (int)SaveDecimalPointHandle(TotalWin);
            TotalWinInteger = (int)TotalWin;
        }
        else
        {
            TotalWinInteger = (int)TotalWin;
        }
        //Debug.Log("Save TotalWin: " + " TotalWinInteger: " + TotalWinInteger + " TotalWinDecimal: " + TotalWinDecimal + " 2: " + (decimal_TotalWin % 1) * 100);
        offset = 0x0E;
        byteuint = (uint)TotalWinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset += 0x04;
        ushortbuf = (ushort)TotalWinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }

    public void SaveTotalGamePlay(double TotalGamePlay)//總遊戲場次
    {
        UInt32 offset;
        uint byteuint;
        offset = 0x14;
        byteuint = (uint)TotalGamePlay;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
    }
    public void SaveTotalWinGamePlay(double TotalWinGamePlay)//總贏場次
    {
        UInt32 offset;
        uint byteuint;
        offset = 0x18;
        byteuint = (uint)TotalWinGamePlay;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
    }

    //讀取總紀錄
    public void LoadTotalStatistcalData(out int TotalOpenPoint, out int TotalClearPoint, out int TotalCoinIn, out double TotalBet, out double TotalWin, out int TotalGamePlay, out int TotalWinGamePlay, out double TicketIn, out double TicketOut)//開分 洗分 總押注 總贏分 遊戲場次 贏場次
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        int TotalBetInteger = 0, TotalWinInteger = 0, TotalTicketInInteger = 0, TotalTicketOutInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0, TotalTicketInDecimal = 0, TotalTicketOutDecimal = 0;

        offset = 0x00;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalOpenPoint = (int)byteuint;

        offset = 0x04;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalClearPoint = (int)byteuint;

        offset = 0x70;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalCoinIn = (int)byteuint;


        offset = 0x14;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalGamePlay = (int)byteuint;

        offset = 0x18;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalWinGamePlay = (int)byteuint;

        offset = 0x08;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalBetInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalBetDecimal = (int)ushortbuf;

        offset = 0x0E;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalWinInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalWinDecimal = (int)ushortbuf;

        offset = 0x4600;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalTicketInInteger = (int)byteuint;
        offset = 0x4604;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalTicketInDecimal = (int)ushortbuf;
        offset = 0x4618;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalTicketOutInteger = (int)byteuint;
        offset = 0x4622;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalTicketOutDecimal = (int)ushortbuf;

        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }

        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
        if (TotalTicketInDecimal == 0)
        {
            TicketIn = TotalTicketInInteger;
        }
        else
        {
            TicketIn = TotalTicketInInteger + (TotalTicketInDecimal * 0.01);
        }
        if (TotalTicketOutDecimal == 0)
        {
            TicketOut = TotalTicketOutInteger;
        }
        else
        {
            TicketOut = TotalTicketOutInteger + (TotalTicketOutDecimal * 0.01);
        }
    }

    public int LoadTotalOpenPoint()//讀取總開分
    {
        int TotalOpenPoint;

        UInt32 offset;
        uint byteuint;

        offset = 0x00;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalOpenPoint = (int)byteuint;


        return TotalOpenPoint;
    }
    public int LoadTotalClearPoint()//讀取總洗分
    {
        int TotalClearPoint;

        UInt32 offset;
        uint byteuint;

        offset = 0x04;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalClearPoint = (int)byteuint;


        return TotalClearPoint;
    }
    public int LoadTotalCoinIn()//讀取總入鈔
    {
        int TotalCoinIn;

        UInt32 offset;
        uint byteuint;

        offset = 0x70;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalCoinIn = (int)byteuint;


        return TotalCoinIn;
    }
    public double LoadTotalTicketIn()//讀取總入票
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TotalTicketIn;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;
        offset = 0x4600;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalTicketInInteger = (int)byteuint;
        offset = 0x4604;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalTicketInDecimal = (int)ushortbuf;
        if (TotalTicketInDecimal == 0)
        {
            TotalTicketIn = TotalTicketInInteger;
        }
        else
        {
            TotalTicketIn = TotalTicketInInteger + (TotalTicketInDecimal * 0.01);
        }
        return TotalTicketIn;
    }
    public double LoadTotalTicketOut()//讀取總出票
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TotalTickeOut;
        int TotalTickeOutInteger = 0;
        int TotalTickeOutDecimal = 0;
        offset = 0x4618;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalTickeOutInteger = (int)byteuint;
        offset = 0x4622;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalTickeOutDecimal = (int)ushortbuf;
        if (TotalTickeOutDecimal == 0)
        {
            TotalTickeOut = TotalTickeOutInteger;
        }
        else
        {
            TotalTickeOut = TotalTickeOutInteger + (TotalTickeOutDecimal * 0.01);
        }
        return TotalTickeOut;
    }


    public double LoadTotalBet()//讀取總押注
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TotalBet;
        int TotalBetInteger = 0;
        int TotalBetDecimal = 0;

        offset = 0x08;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalBetInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalBetDecimal = (int)ushortbuf;
        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }
        return TotalBet;
    }
    public double LoadTotalWin()//讀取總贏分
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TotalWin;
        int TotalWinInteger = 0;
        int TotalWinDecimal = 0;

        offset = 0x0E;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalWinInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalWinDecimal = (int)ushortbuf;
        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
        return TotalWin;
    }
    public int LoadTotalGamePlay()//讀取總場次
    {
        int TotalGamePlay;

        UInt32 offset;
        uint byteuint;

        offset = 0x14;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalGamePlay = (int)byteuint;


        return TotalGamePlay;
    }
    public int LoadTotalWinGamePlay()//讀取總贏場次
    {
        int TotalWinGamePlay;

        UInt32 offset;
        uint byteuint;

        offset = 0x18;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalWinGamePlay = (int)byteuint;


        return TotalWinGamePlay;
    }
    public float LoadTotalWinScoreRate()//讀取總得分率
    {
        return (float)(LoadTotalWin() / LoadTotalBet());
    }
    public float LoadTotalWinCountRate()//讀取總贏分率
    {
        return (float)((float)LoadTotalWinGamePlay() / (float)LoadTotalGamePlay());
    }

    #endregion

    #region 存讀班紀錄
    //儲存班別紀錄
    public void SaveClassStatistcalData(int ClassOpenPoint, int ClassClearPoint, int ClassCoinIn, double ClassBet, double ClassWin, int ClassGamePlay, int ClassWinGamePlay, double TicketIn, double TicketOut)
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int ClassBetInteger = 0, ClassWinInteger = 0, ClassTicketInInteger = 0, ClassTicketOutInteger = 0;
        ;
        int ClassBetDecimal = 0, ClassWinDecimal = 0, ClassTicketInDecimal = 0, ClassTicketOutDecimal = 0;
        if (ClassBet % 1 != 0)
        {
            ClassBetDecimal = (int)SaveDecimalPointHandle(ClassBet);
            ClassBetInteger = (int)ClassBet;
        }
        else
        {
            ClassBetInteger = (int)ClassBet;
        }

        if (ClassWin % 1 != 0)
        {
            ClassWinDecimal = (int)SaveDecimalPointHandle(ClassWin);
            ClassWinInteger = (int)ClassWin;
        }
        else
        {
            ClassWinInteger = (int)ClassWin;
        }
        if (TicketIn % 1 != 0)
        {
            ClassTicketInDecimal = (int)SaveDecimalPointHandle(TicketIn);
            ClassTicketInInteger = (int)TicketIn;
        }
        else
        {
            ClassTicketInInteger = (int)TicketIn;
        }
        if (TicketOut % 1 != 0)
        {
            ClassTicketOutDecimal = (int)SaveDecimalPointHandle(TicketOut);
            ClassTicketOutInteger = (int)TicketOut;
        }
        else
        {
            ClassTicketOutInteger = (int)TicketOut;
        }

        offset = 0x1C;
        byteuint = (uint)ClassOpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x20;
        byteuint = (uint)ClassClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x74;
        byteuint = (uint)ClassCoinIn;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x30;
        byteuint = (uint)ClassGamePlay;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x34;
        byteuint = (uint)ClassWinGamePlay;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x24;
        byteuint = (uint)ClassBetInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset += 0x04;
        ushortbuf = (ushort)ClassBetDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);

        offset = 0x2A;
        byteuint = (uint)ClassWinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset += 0x04;
        ushortbuf = (ushort)ClassWinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);

        offset = 0x4600;
        byteuint = (uint)ClassTicketInInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4604;
        ushortbuf = (ushort)ClassTicketInDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
        offset = 0x4618;
        byteuint = (uint)ClassTicketOutInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4621;
        ushortbuf = (ushort)ClassTicketOutDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);

    }

    public void SaveClassOpenPoint(int ClassOpenPoint)//總開分
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x1C;
        byteuint = (uint)ClassOpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

    }
    public void SaveClassClearPoint(int ClassClearPoint)//總洗分
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x20;
        byteuint = (uint)ClassClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

    }
    public void SaveClassCoinIn(int ClassCoinIn)//總入鈔
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x74;
        byteuint = (uint)ClassCoinIn;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
    }
    public void SaveClassTicketIn(double ClassTicketIn)//總入票
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int ClassTicketInInteger = 0;
        int ClassTicketInDecimal = 0;
        if (ClassTicketIn % 1 != 0)
        {
            ClassTicketInDecimal = (int)SaveDecimalPointHandle(ClassTicketIn);
            ClassTicketInInteger = (int)ClassTicketIn;
        }
        else
        {
            ClassTicketInInteger = (int)ClassTicketIn;
        }
        offset = 0x4606;
        byteuint = (uint)ClassTicketInInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4610;
        ushortbuf = (ushort)ClassTicketInDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public void SaveClassTicketOut(double ClassTicketOut)//總出票
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int ClassTicketOutInteger = 0;
        int ClassTicketOutDecimal = 0;
        if (ClassTicketOut % 1 != 0)
        {
            ClassTicketOutDecimal = (int)SaveDecimalPointHandle(ClassTicketOut);
            ClassTicketOutInteger = (int)ClassTicketOut;
        }
        else
        {
            ClassTicketOutInteger = (int)ClassTicketOut;
        }
        offset = 0x4624;
        byteuint = (uint)ClassTicketOutInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4628;
        ushortbuf = (ushort)ClassTicketOutDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }


    public void SaveClassBet(double ClassBet)//總押注
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int ClassBetInteger = 0;
        int ClassBetDecimal = 0;
        if (ClassBet % 1 != 0)
        {
            ClassBetDecimal = (int)SaveDecimalPointHandle(ClassBet);
            ClassBetInteger = (int)ClassBet;
        }
        else
        {
            ClassBetInteger = (int)ClassBet;
        }

        offset = 0x24;
        byteuint = (uint)ClassBetInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset += 0x04;
        ushortbuf = (ushort)ClassBetDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);

    }
    public void SaveClassWin(double ClassWin)//總贏分
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int ClassWinInteger = 0;
        int ClassWinDecimal = 0;
        if (ClassWin % 1 != 0)
        {
            ClassWinDecimal = (int)SaveDecimalPointHandle(ClassWin);
            ClassWinInteger = (int)ClassWin;
        }
        else
        {
            ClassWinInteger = (int)ClassWin;
        }
        offset = 0x2A;
        byteuint = (uint)ClassWinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset += 0x04;
        ushortbuf = (ushort)ClassWinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public void SaveClassGamePlay(double ClassGamePlay)//總遊戲場次
    {
        UInt32 offset;
        uint byteuint;
        offset = 0x30;
        byteuint = (uint)ClassGamePlay;

        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
    }
    public void SaveClassWinGamePlay(double ClassWinGamePlay)//總贏場次
    {
        UInt32 offset;
        uint byteuint;
        offset = 0x34;
        byteuint = (uint)ClassWinGamePlay;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
    }

    //讀取班別紀錄
    public void LoadClassStatistcalData(out int ClassOpenPoint, out int ClassClearPoint, out int ClassCoinIn, out double ClassBet, out double ClassWin, out int ClassGamePlay, out int ClassWinGamePlay, out double TicketIn, out double TicketOut)
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        int ClassBetInteger = 0, ClassWinInteger = 0, ClassTicketInInteger = 0, ClassTicketOutInteger = 0;
        int ClassBetDecimal = 0, ClassWinDecimal = 0, ClassTicketInDecimal = 0, ClassTicketOutDecimal = 0;
        offset = 0x1C;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassOpenPoint = (int)byteuint;
        offset = 0x20;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassClearPoint = (int)byteuint;

        offset = 0x74;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassCoinIn = (int)byteuint;

        offset = 0x30;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassGamePlay = (int)byteuint;
        offset = 0x34;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassWinGamePlay = (int)byteuint;

        offset = 0x24;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassBetInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        ClassBetDecimal = (int)ushortbuf;

        offset = 0x2A;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassWinInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        ClassWinDecimal = (int)ushortbuf;

        offset = 0x4600;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassTicketInInteger = (int)byteuint;
        offset = 0x4604;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        ClassTicketInDecimal = (int)ushortbuf;
        offset = 0x4618;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassTicketOutInteger = (int)byteuint;
        offset = 0x4621;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        ClassTicketOutDecimal = (int)ushortbuf;

        if (ClassBetDecimal == 0)
        {
            ClassBet = ClassBetInteger;
        }
        else
        {
            ClassBet = ClassBetInteger + (ClassBetDecimal * 0.01);
        }

        if (ClassWinDecimal == 0)
        {
            ClassWin = ClassWinInteger;
        }
        else
        {
            ClassWin = ClassWinInteger + (ClassWinDecimal * 0.01);
        }
        if (ClassTicketInDecimal == 0)
        {
            TicketIn = ClassTicketInInteger;
        }
        else
        {
            TicketIn = ClassTicketInInteger + (ClassTicketInDecimal * 0.01);
        }
        if (ClassTicketOutDecimal == 0)
        {
            TicketOut = ClassTicketOutInteger;
        }
        else
        {
            TicketOut = ClassTicketOutInteger + (ClassTicketOutDecimal * 0.01);
        }


    }

    public int LoadClassOpenPoint()//讀取總開分
    {
        int ClassOpenPoint;

        UInt32 offset;
        uint byteuint;

        offset = 0x1C;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassOpenPoint = (int)byteuint;


        return ClassOpenPoint;
    }
    public int LoadClassClearPoint()//讀取總洗分
    {
        int ClassClearPoint;

        UInt32 offset;
        uint byteuint;

        offset = 0x20;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassClearPoint = (int)byteuint;


        return ClassClearPoint;
    }
    public int LoadClassCoinIn()//讀取總入鈔
    {
        int ClassCoinIn;

        UInt32 offset;
        uint byteuint;

        offset = 0x74;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassCoinIn = (int)byteuint;


        return ClassCoinIn;
    }
    public double LoadClassTicketIn()//讀取總入票
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TotalTicketIn;
        int ClassTicketInInteger = 0;
        int ClassTicketInDecimal = 0;
        offset = 0x4606;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassTicketInInteger = (int)byteuint;
        offset = 0x4610;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        ClassTicketInDecimal = (int)ushortbuf;
        if (ClassTicketInDecimal == 0)
        {
            TotalTicketIn = ClassTicketInInteger;
        }
        else
        {
            TotalTicketIn = ClassTicketInInteger + (ClassTicketInDecimal * 0.01);
        }
        return TotalTicketIn;
    }
    public double LoadClassTicketOut()//讀取總出票
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TotalTickeOut;
        int ClassTickeOutInteger = 0;
        int ClassTickeOutDecimal = 0;
        offset = 0x4624;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassTickeOutInteger = (int)byteuint;
        offset = 0x4628;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        ClassTickeOutDecimal = (int)ushortbuf;
        if (ClassTickeOutDecimal == 0)
        {
            TotalTickeOut = ClassTickeOutInteger;
        }
        else
        {
            TotalTickeOut = ClassTickeOutInteger + (ClassTickeOutDecimal * 0.01);
        }
        return TotalTickeOut;
    }

    public double LoadClassBet()//讀取總押注
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TotalBet;
        int ClassBetInteger = 0;
        int ClassBetDecimal = 0;

        offset = 0x24;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassBetInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        ClassBetDecimal = (int)ushortbuf;
        if (ClassBetDecimal == 0)
        {
            TotalBet = ClassBetInteger;
        }
        else
        {
            TotalBet = ClassBetInteger + (ClassBetDecimal * 0.01);
        }
        return TotalBet;
    }
    public double LoadClassWin()//讀取總贏分
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double ClassWin;
        int ClassWinInteger = 0;
        int ClassWinDecimal = 0;

        offset = 0x2A;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassWinInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        ClassWinDecimal = (int)ushortbuf;
        if (ClassWinDecimal == 0)
        {
            ClassWin = ClassWinInteger;
        }
        else
        {
            ClassWin = ClassWinInteger + (ClassWinDecimal * 0.01);
        }
        return ClassWin;
    }
    public int LoadClassGamePlay()//讀取總場次
    {
        int ClassGamePlay;

        UInt32 offset;
        uint byteuint;

        offset = 0x30;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassGamePlay = (int)byteuint;


        return ClassGamePlay;
    }
    public int LoadClassWinGamePlay()//讀取總贏場次
    {
        int ClassWinGamePlay;

        UInt32 offset;
        uint byteuint;

        offset = 0x34;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        ClassWinGamePlay = (int)byteuint;


        return ClassWinGamePlay;
    }

    public float LoadClassWinScoreRate()//讀取班得分率
    {
        return (float)(LoadClassWin() / LoadClassBet());
    }
    public float LoadClassWinCountRate()//讀取班贏分率
    {
        return (float)((float)LoadClassWinGamePlay() / (float)LoadClassGamePlay());
    }

    #endregion

    #region 存讀備份紀錄
    //儲存備份紀錄
    public void SaveBackupStatistcalData(int TotalOpenPoint, int TotalClearPoint, int TotalCoinIn, double TotalBet, double TotalWin, int TotalGamePlay, int TotalWinGamePlay, double TotalTicketIn, double TotalTicketOut)
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int TotalBetInteger = 0, TotalWinInteger = 0, TotalTicketInInteger = 0, TotalTicketOutInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0, TotalTicketInDecimal = 0, TotalTicketOutDecimal = 0
;
        if (TotalBet % 1 != 0)
        {
            TotalBetDecimal = (int)SaveDecimalPointHandle(TotalBet);
            TotalBetInteger = (int)TotalBet;
        }
        else
        {
            TotalBetInteger = (int)TotalBet;
        }

        if (TotalWin % 1 != 0)
        {
            TotalWinDecimal = (int)SaveDecimalPointHandle(TotalWin);
            TotalWinInteger = (int)TotalWin;
        }
        else
        {
            TotalWinInteger = (int)TotalWin;
        }
        if (TotalTicketIn % 1 != 0)
        {
            TotalTicketInDecimal = (int)SaveDecimalPointHandle(TotalTicketIn);
            TotalTicketInInteger = (int)TotalTicketIn;
        }
        else
        {
            TotalTicketInInteger = (int)TotalTicketIn;
        }
        if (TotalTicketOut % 1 != 0)
        {
            TotalTicketOutDecimal = (int)SaveDecimalPointHandle(TotalTicketOut);
            TotalTicketOutInteger = (int)TotalTicketOut;
        }
        else
        {
            TotalTicketOutInteger = (int)TotalTicketOut;
        }

        offset = 0x1000;
        byteuint = (uint)TotalOpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x1004;
        byteuint = (uint)TotalClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x1022;
        byteuint = (uint)TotalCoinIn;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x1014;
        byteuint = (uint)TotalGamePlay;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x1018;
        byteuint = (uint)TotalWinGamePlay;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x1008;
        byteuint = (uint)TotalBetInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset += 0x04;
        ushortbuf = (ushort)TotalBetDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);

        offset = 0x100E;
        byteuint = (uint)TotalWinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset += 0x04;
        ushortbuf = (ushort)TotalWinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);

        offset = 0x4612;
        byteuint = (uint)TotalTicketInInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4616;
        ushortbuf = (ushort)TotalTicketInDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
        offset = 0x4630;
        byteuint = (uint)TotalTicketOutInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4634;
        ushortbuf = (ushort)TotalTicketOutDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);

    }
    //讀取備份紀錄
    public void LoadBackupStatistcalData(out int TotalOpenPoint, out int TotalClearPoint, out double TotalBet, out double TotalWin, out int TotalGamePlay, out int TotalWinGamePlay, out double TotalTicketIn, out double TotalTicketOut)
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        int TotalBetInteger = 0, TotalWinInteger = 0, TotalTicketInInteger = 0, TotalTicketOutInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0, TotalTicketInDecimal = 0, TotalTicketOutDecimal = 0;
        offset = 0x1000;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalOpenPoint = (int)byteuint;

        offset = 0x1004;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalClearPoint = (int)byteuint;

        offset = 0x1014;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalGamePlay = (int)byteuint;

        offset = 0x1018;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalWinGamePlay = (int)byteuint;

        offset = 0x1008;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalBetInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalBetDecimal = (int)ushortbuf;

        offset = 0x100E;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalWinInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalWinDecimal = (int)ushortbuf;

        offset = 0x4612;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalTicketInInteger = (int)byteuint;
        offset = 0x4616;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalTicketInDecimal = (int)ushortbuf;
        offset = 0x4630;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalTicketOutInteger = (int)byteuint;
        offset = 0x4634;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalTicketOutDecimal = (int)ushortbuf;

        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }

        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
        if (TotalTicketInDecimal == 0)
        {
            TotalTicketIn = TotalTicketInInteger;
        }
        else
        {
            TotalTicketIn = TotalTicketInInteger + (TotalTicketInDecimal * 0.01);
        }
        if (TotalTicketOutDecimal == 0)
        {
            TotalTicketOut = TotalTicketOutInteger;
        }
        else
        {
            TotalTicketOut = TotalTicketOutInteger + (TotalTicketOutDecimal * 0.01);
        }

    }

    public int LoadBackupTotalOpenPoint()//讀取總開分
    {
        int TotalOpenPoint;

        UInt32 offset;
        uint byteuint;

        offset = 0x1000;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalOpenPoint = (int)byteuint;


        return TotalOpenPoint;
    }
    public int LoadBackupTotalClearPoint()//讀取總洗分
    {
        int TotalClearPoint;

        UInt32 offset;
        uint byteuint;

        offset = 0x1004;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalClearPoint = (int)byteuint;


        return TotalClearPoint;
    }
    public int LoadBackupTotalCoinIn()//讀取總入鈔
    {
        int TotalCoinIn;

        UInt32 offset;
        uint byteuint;

        offset = 0x1022;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalCoinIn = (int)byteuint;


        return TotalCoinIn;
    }
    public double LoadBackupTotalBet()//讀取總押注
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TotalBet;
        int TotalBetInteger = 0;
        int TotalBetDecimal = 0;

        offset = 0x1008;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalBetInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalBetDecimal = (int)ushortbuf;
        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }
        return TotalBet;
    }
    public double LoadBackupTotalWin()//讀取總贏分
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TotalWin;
        int TotalWinInteger = 0;
        int TotalWinDecimal = 0;

        offset = 0x100E;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalWinInteger = (int)byteuint;

        offset += 0x04;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalWinDecimal = (int)ushortbuf;
        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
        return TotalWin;
    }
    public int LoadBackupTotalGamePlay()//讀取總場次
    {
        int TotalGamePlay;

        UInt32 offset;
        uint byteuint;

        offset = 0x1014;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalGamePlay = (int)byteuint;


        return TotalGamePlay;
    }
    public int LoadBackupTotalWinGamePlay()//讀取總贏場次
    {
        int TotalWinGamePlay;

        UInt32 offset;
        uint byteuint;

        offset = 0x1018;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalWinGamePlay = (int)byteuint;


        return TotalWinGamePlay;
    }


    #endregion
    //儲存密碼
    public void SavePassword(int password)
    {
        UInt32 offset;
        ushort ushortbuf;
        offset = 0x38;
        ushortbuf = (ushort)password;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    //讀取密碼
    public int LoadPassword()
    {
        int password;
        UInt32 offset;
        ushort ushortbuf;
        offset = 0x38;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        password = (int)ushortbuf;
        return password;
    }
    //儲存鈔機設定
    public void SaveBanknoteMachineSetting(bool Working)
    {
        UInt32 offset = 0x3A;
        byte bytebuf;
        if (Working)
        {
            bytebuf = 1;
        }
        else
        {
            bytebuf = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf);
    }
    //讀取鈔機設定
    public void LoadBanknoteMachineSetting(out bool Working)
    {
        UInt32 offset = 0x3A;
        byte bytebuf;
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf);
        if (bytebuf == 0)
        {
            Working = false;
        }
        else
        {
            Working = true;
        }
    }

    //儲存單次開分鍵與單次洗分鍵的數值
    public void SaveOpenPointSetting(int OpenPoint)
    {
        UInt32 offset;
        ushort ushortbuf;
        offset = 0x3B;
        ushortbuf = (ushort)OpenPoint;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public void SaveClearPointSetting(int ClearPoint)
    {
        UInt32 offset;
        ushort ushortbuf;
        offset = 0x3D;
        ushortbuf = (ushort)ClearPoint;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }

    //讀取單次開分鍵與單次洗分鍵的數值
    public int LoadOpenPointSetting()
    {
        int OpenPoint;
        UInt32 offset;
        ushort ushortbuf;
        offset = 0x3B;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        OpenPoint = (int)ushortbuf;
        return OpenPoint;
    }
    public int LoadClearPointSetting()
    {
        int ClearPoint;
        UInt32 offset;
        ushort ushortbuf;
        offset = 0x3D;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        ClearPoint = (int)ushortbuf;
        return ClearPoint;
    }

    //儲存最大押注倍數
    public void SaveMaxOdd(int Odd)
    {
        UInt32 offset = 0x47;
        byte bytebuf;
        bytebuf = (byte)Odd;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf);
    }
    //讀取最大押注倍數
    public int LoadMaxOdd()
    {
        int Odd;
        UInt32 offset = 0x47;
        byte bytebuf;
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf);
        Odd = (int)bytebuf;
        return Odd;
    }

    //儲存最大籌碼與最大贏籌碼
    public void SaveMaxCredit(int MaxCredit)
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x48;
        byteuint = (uint)MaxCredit;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
    }
    public void SaveMaxWin(int MaxWin)
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x4C;
        byteuint = (uint)MaxWin;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
    }

    //讀取最大籌碼與最大贏籌碼
    public int LoadMaxCredit()
    {
        int MaxCredit;
        UInt32 offset;
        uint byteuint;

        offset = 0x48;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        MaxCredit = (int)byteuint;
        return MaxCredit;

    }
    public int LoadMaxWin()
    {
        int MaxWin;
        UInt32 offset;
        uint byteuint;

        offset = 0x4C;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        MaxWin = (int)byteuint;
        return MaxWin;
    }
    //儲存顯示的籌碼比率
    public void SaveDenomOption(bool[] DenomOption)
    {
        UInt32 offset = 0x50;
        byte[] bytebuf = new byte[9];
        for (int i = 0; i < 9; i++)
        {
            if (DenomOption[i]) { bytebuf[i] = 1; }
            else { bytebuf[i] = 0; }

            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
        }
    }
    //讀取顯示的籌碼比率
    public void LoadDenomOption(out bool[] DenomOption)
    {
        UInt32 offset = 0x50;
        byte[] bytebuf = new byte[9];
        bool[] tmp = new bool[9];
        for (int i = 0; i < 9; i++)
        {
            result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * i)), out bytebuf[i]);
            if (bytebuf[i] == 0) { tmp[i] = false; }
            else { tmp[i] = true; }
        }
        DenomOption = tmp;
    }

    //儲存事件紀錄
    public void SaveEventRecored(EventRecoredData[] EventRecored)
    {
        UInt32 offset;
        byte[] Eventbytebuf;
        uint[] Eventbyteuint;

        offset = 0x0100;
        Eventbytebuf = new byte[100];
        for (int i = 0; i < 100; i++)
        {
            Eventbytebuf[i] = (byte)EventRecored[i].EventCode;
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), Eventbytebuf[i]);
        }

        offset = 0x0164;
        Eventbyteuint = new uint[100];
        for (int i = 0; i < 100; i++)
        {
            Eventbyteuint[i] = (uint)EventRecored[i].EventData;
            result = NVRAM.qxt_nvram_writedword(offset + ((uint)(0x04 * i)), Eventbyteuint[i]);
        }

        offset = 0x02F4;
        byte[] EventTime;
        for (int i = 0; i < 100; i++)
        {
            EventTime = new byte[8];
            EventTime = BitConverter.GetBytes(EventRecored[i].EventTime.Ticks);
            for (int k = 0; k < 8; k++)
            {
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * k)), EventTime[k]);
            }
            offset += 0x0008;
        }
    }
    //讀取事件紀錄
    public void LoadEventRecored(out EventRecoredData[] EventRecored)
    {
        UInt32 offset;
        byte[] Eventbytebuf;
        uint[] Eventbyteuint;
        EventRecoredData[] tmp = new EventRecoredData[100];
        for (int i = 0; i < 100; i++)
        {
            tmp[i] = new EventRecoredData();
        }

        offset = 0x0100;
        Eventbytebuf = new byte[100];
        for (int i = 0; i < 100; i++)
        {
            result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * i)), out Eventbytebuf[i]);
            tmp[i].EventCode = (int)Eventbytebuf[i];
        }

        offset = 0x0164;
        Eventbyteuint = new uint[100];
        for (int i = 0; i < 100; i++)
        {
            result = NVRAM.qxt_nvram_readdword(offset + ((uint)(0x04 * i)), out Eventbyteuint[i]);
            tmp[i].EventData = (int)Eventbyteuint[i];
        }

        offset = 0x02F4;
        byte[] EventTime;
        for (int i = 0; i < 100; i++)
        {
            EventTime = new byte[8];
            for (int k = 0; k < 8; k++)
            {
                result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * k)), out Eventbytebuf[k]);
            }
            tmp[i].EventTime = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));
            offset += 0x0008;
        }

        EventRecored = tmp;
    }

    int EventRecoredSaveQuantity = 100;
    public static EventRecoredDataList eventRecoredDataList = new EventRecoredDataList();
    public void saveEventRecoredByEventName(int _EventCode, int _EventData)
    {
        for (int i = EventRecoredSaveQuantity - 1; i >= 0; i--)//0:無資料  1以後的數字(包含1)請自行運用  0:無資料 1:啟動遊戲 2:開分 3:洗分 4:入鈔  0x0100 (1) * 200
        {
            if (eventRecoredDataList._EventRecoredData[i].EventCode != 0)
            {
                if (i < EventRecoredSaveQuantity - 1)
                {
                    eventRecoredDataList._EventRecoredData[i + 1].EventCode = eventRecoredDataList._EventRecoredData[i].EventCode;
                    eventRecoredDataList._EventRecoredData[i + 1].EventData = eventRecoredDataList._EventRecoredData[i].EventData;
                    eventRecoredDataList._EventRecoredData[i + 1].EventTime = eventRecoredDataList._EventRecoredData[i].EventTime;
                }
                else
                {
                    eventRecoredDataList._EventRecoredData[i] = new EventRecoredData();
                }
            }
        }
        eventRecoredDataList._EventRecoredData[0].EventCode = _EventCode;
        eventRecoredDataList._EventRecoredData[0].EventData = _EventData;
        eventRecoredDataList._EventRecoredData[0].EventTime = DateTime.Now;
        SaveEventRecored(eventRecoredDataList._EventRecoredData);
        //saveEventRecoredData(EventRecoredDataFileName, eventRecoredDataList);
        //GetComponent<SramManager>().EventRecoredSave();
    }

    //儲存個別RTP設定與是否使用共通RTP
    public void SaveRTPSetting(int[] RTPValue, bool RTPuse)
    {
        UInt32 offset = 0x624;
        byte[] bytebuf = new byte[11];
        for (int i = 0; i < 10; i++)
        {
            bytebuf[i] = (byte)RTPValue[i];
        }
        if (RTPuse) { bytebuf[10] = 1; }
        else { bytebuf[10] = 0; }

        for (int i = 0; i < 11; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
        }

    }
    //讀取個別RTP設定與是否使用共通RTP
    public void LoadRTPSetting(out int[] RTPValue, out bool RTPuse)
    {
        UInt32 offset = 0x624;
        int[] tmp = new int[10];
        byte[] bytebuf = new byte[11];
        for (int i = 0; i < 10; i++)
        {
            result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * i)), out bytebuf[i]);
            tmp[i] = (int)bytebuf[i];
        }
        RTPValue = tmp;

        result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * 10)), out bytebuf[10]);
        if (bytebuf[10] == 0)
        {
            RTPuse = false;
        }
        else
        {
            RTPuse = true;
        }
    }

    //儲存狀態
    public void SaveStatus(int NowStatus)
    {
        UInt32 offset;
        ushort ushortbuf;
        offset = 0x59;
        ushortbuf = (ushort)NowStatus;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    //讀取狀態
    public int LoadStatus()
    {
        int NowStatus;
        UInt32 offset;
        ushort ushortbuf;
        offset = 0x59;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        NowStatus = (int)ushortbuf;
        return NowStatus;
    }
    //儲存Bouns前牌面 //原16 改為8
    public void SaveBeforeBonusRNG(int[] beforeBonusRNG)
    {
        UInt32 offset;
        offset = 0x4000;
        ushort[] ushortbuf;
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            ushortbuf[i] = (ushort)beforeBonusRNG[i];
            result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf[i]);
        }
    }
    //讀起Bonus前牌面  //原16 改為8
    public int[] LoadBeforeBonusRNG()
    {
        int[] beforeBonusRNG = new int[5];
        UInt32 offset;
        offset = 0x4000;
        ushort[] ushortbuf;
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf[i]);
            beforeBonusRNG[i] = (int)ushortbuf[i];
        }
        return beforeBonusRNG;
    }
    //儲存Sram是否有紀錄
    public void SaveIsSramTrue(bool isSram)
    {
        UInt32 offset = 0x5B;
        byte bytebuf;
        if (isSram) bytebuf = (byte)1;
        else bytebuf = (byte)0;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf);
    }
    //讀取Sram是否有紀錄
    public bool LoadIsSramTrue()
    {
        UInt32 offset = 0x5B;
        byte bytebuf;
        int tmp;
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf);
        tmp = (int)bytebuf;
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //讀取記錄編號 輸入Type(0:一般紀錄 1:最大獎 2:最大獎Second 預設:一般記錄)  輸出 最大的編號
    public void LoadGameHistoryLog(int Type, out int MaxLogNumber)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        int[] NumberArray;
        int HistoryLength = 100;
        MaxLogNumber = 0;
        switch (Type)
        {
            case 0:
                offset = 0x1500;
                HistoryLength = 100;
                break;
            case 1:
                offset = 0x3000;
                HistoryLength = 10;
                break;
            case 2:
                offset = 0x3300;
                HistoryLength = 10;
                break;
            default:
                offset = 0x1500;
                HistoryLength = 100;
                break;
        }
        ushortbuf = new ushort[HistoryLength];
        NumberArray = new int[HistoryLength];
        for (int i = 0; i < HistoryLength; i++)
        {
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf[i]);
            NumberArray[i] = (int)ushortbuf[i];
            if (MaxLogNumber < NumberArray[i])
            {
                MaxLogNumber = NumberArray[i];
            }
        }
    }

    public void ClearHistory()
    {
        HistoryData SaveData = new HistoryData();
        for (int i = 0; i < 100; i++)
        {
            SaveHistory(SaveData);
        }


        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x1500;
        ushortbuf = new ushort[100];
        for (int i = 0; i < 100; i++)
        {
            result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf[i]);
        }
        offset = 0x4100;
        ushortbuf = new ushort[100];
        for (int i = 0; i < 100; i++)
        {
            result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf[i]);
        }

        for (int i = 1; i <= 10; i++)
        {
            SaveMaxGameHistory(i, SaveData);
            SaveMaxGameHistorySecond(i, SaveData);
        }
        offset = 0x3000;
        ushortbuf = new ushort[10];
        for (int i = 0; i < 10; i++)
        {
            result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf[i]);
        }

        offset = 0x3300;
        ushortbuf = new ushort[10];
        for (int i = 0; i < 10; i++)
        {
            result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf[i]);
        }
    }


    //儲存歷史紀錄 會清空目前開洗分 輸入時必須已是完整記錄(包含開洗分)
    public void SaveHistory(HistoryData SaveData)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        ushort[] ushortbuf_2;
        ushort ushortbuff;
        byte bytebuff;
        uint byteuintbuff;
        int Count = 0;
        int CreditInteger = 0, CreditDecimal = 0;
        int WinInteger = 0, WinDecimal = 0;
        byte[] EventTime;
        offset = 0x1500;
        ushortbuf = new ushort[100];
        for (int i = 0; i < 100; i++)
        {
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf[i]);
        }

        for (Count = 0; Count < 100; Count++)
        {
            if (ushortbuf[Count] == 0 && Count == 0)
            {
                goto SaveStart;
            }
            else if (ushortbuf[Count] == 0)
            {
                goto SaveNew;
            }
            else if (Count == 99 && ushortbuf[Count] != 0)
            {
                goto SaveOverWrite;
            }
        }
        return;

    SaveStart:
    #region SaveStart
        offset = 0x1500;
        ushortbuff = (ushort)1;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (SaveData.RNG[i] - 255 > 0)
            {
                offset = 0x15C8;
                bytebuf[i] = 0xFF;
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x4100;
                ushortbuf_2[i] = (ushort)(SaveData.RNG[i] - 255);
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
            else
            {
                offset = 0x15C8;
                bytebuf[i] = (byte)SaveData.RNG[i];
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x4100;
                ushortbuf_2[i] = 0x00;
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
        }

        offset = 0x17BC;
        if (SaveData.Bonus)
        {
            bytebuff = 1;
        }
        else
        {
            bytebuff = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuff);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (SaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(SaveData.Credit);
            CreditInteger = (int)SaveData.Credit;
        }
        else
        {
            CreditInteger = (int)SaveData.Credit;
        }

        offset = 0x1820;
        byteuintbuff = (uint)CreditInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset += 0x04;
        ushortbuff = (ushort)CreditDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x1A78;
        byteuintbuff = (uint)SaveData.Demon;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset = 0x1C08;
        ushortbuff = (ushort)SaveData.Bet;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x1CD0;
        bytebuff = (byte)SaveData.Odds;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuff);

        WinInteger = 0;
        WinDecimal = 0;
        if (SaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(SaveData.Win);
            WinInteger = (int)SaveData.Win;
        }
        else
        {
            WinInteger = (int)SaveData.Win;
        }
        offset = 0x1D34;
        byteuintbuff = (uint)WinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset += 0x04;
        ushortbuff = (ushort)WinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x1F8C;
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(SaveData.Time.Ticks);
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), EventTime[i]);
        }

        offset = 0x22AC;
        byteuintbuff = (uint)SaveData.OpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset = 0x243C;
        byteuintbuff = (uint)SaveData.ClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset = 0x25CC;
        ushortbuff = (ushort)SaveData.RTP;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x2694;
        ushortbuff = (ushort)SaveData.SpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x275C;
        ushortbuff = (ushort)SaveData.BonusSpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x2824;
        ushortbuff = (ushort)SaveData.BonusIsPlayedCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x28EC;
        ushortbuff = (ushort)SaveData.BonusCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        goto End;
    #endregion

    SaveNew:
    #region SaveNew
        offset = 0x1500;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)(Count + 1);
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (SaveData.RNG[i] - 255 > 0)
            {
                offset = 0x15C8;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = 0xFF;
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x4100;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = (ushort)(SaveData.RNG[i] - 255);
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
            else
            {
                offset = 0x15C8;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = (byte)SaveData.RNG[i];
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x4100;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = 0x00;
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
        }



        offset = 0x17BC;
        offset += (uint)((uint)0x01 * Count);
        bytebuff = new byte();
        if (SaveData.Bonus)
        {
            bytebuff = 1;
        }
        else
        {
            bytebuff = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuff);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (SaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(SaveData.Credit);
            CreditInteger = (int)SaveData.Credit;
        }
        else
        {
            CreditInteger = (int)SaveData.Credit;
        }

        offset = 0x1820;
        offset += (uint)((uint)0x06 * Count);
        byteuintbuff = (uint)CreditInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset += 0x04;
        ushortbuff = (ushort)CreditDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x1A78;
        offset += (uint)((uint)0x04 * Count);
        byteuintbuff = (uint)SaveData.Demon;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset = 0x1C08;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.Bet;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x1CD0;
        offset += (uint)((uint)0x01 * Count);
        bytebuff = (byte)SaveData.Odds;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuff);

        WinInteger = 0;
        WinDecimal = 0;
        if (SaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(SaveData.Win);
            WinInteger = (int)SaveData.Win;
        }
        else
        {
            WinInteger = (int)SaveData.Win;
        }
        offset = 0x1D34;
        offset += (uint)((uint)0x06 * Count);
        byteuintbuff = (uint)WinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset += 0x04;
        ushortbuff = (ushort)WinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x1F8C;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(SaveData.Time.Ticks);
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), EventTime[i]);
        }

        offset = 0x22AC;
        offset += (uint)((uint)0x04 * Count);
        byteuintbuff = (uint)SaveData.OpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset = 0x243C;
        offset += (uint)((uint)0x04 * Count);
        byteuintbuff = (uint)SaveData.ClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset = 0x25CC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.RTP;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x2694;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.SpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x275C;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.BonusSpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x2824;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.BonusIsPlayedCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x28EC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.BonusCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        goto End;
    #endregion

    SaveOverWrite:
    #region SaveOverWrite
        int[] _Number = new int[100];
        for (int i = 0; i < 100; i++)
        {
            _Number[i] = (int)ushortbuf[i];
            if (_Number[i] == 1)
            {
                Count = i;
            }
        }
        offset = 0x1500;
        ushortbuf = new ushort[100];
        // int tmp = _Number[0];
        for (int i = 0; i < 100; i++)
        {
            if (i <= Count)
            {
                _Number[i] = 100 - Count + i;
            }
            else
            {
                _Number[i] = i - Count;
            }
            ushortbuf[i] = (ushort)_Number[i];
            result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf[i]);
        }

        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (SaveData.RNG[i] - 255 > 0)
            {
                offset = 0x15C8;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = 0xFF;
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x4100;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = (ushort)(SaveData.RNG[i] - 255);
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
            else
            {
                offset = 0x15C8;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = (byte)SaveData.RNG[i];
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x4100;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = 0x00;
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
        }

        offset = 0x17BC;
        offset += (uint)((uint)0x01 * Count);
        if (SaveData.Bonus)
        {
            bytebuff = 1;
        }
        else
        {
            bytebuff = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuff);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (SaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(SaveData.Credit);
            CreditInteger = (int)SaveData.Credit;
        }
        else
        {
            CreditInteger = (int)SaveData.Credit;
        }

        offset = 0x1820;
        offset += (uint)((uint)0x06 * Count);
        byteuintbuff = (uint)CreditInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset += 0x04;
        ushortbuff = (ushort)CreditDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x1A78;
        offset += (uint)((uint)0x04 * Count);
        byteuintbuff = (uint)SaveData.Demon;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset = 0x1C08;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.Bet;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x1CD0;
        offset += (uint)((uint)0x01 * Count);
        bytebuff = (byte)SaveData.Odds;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuff);

        WinInteger = 0;
        WinDecimal = 0;
        if (SaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(SaveData.Win);
            WinInteger = (int)SaveData.Win;
        }
        else
        {
            WinInteger = (int)SaveData.Win;
        }
        offset = 0x1D34;
        offset += (uint)((uint)0x06 * Count);
        byteuintbuff = (uint)WinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset += 0x04;
        ushortbuff = (ushort)WinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x1F8C;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(SaveData.Time.Ticks);
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), EventTime[i]);
        }

        offset = 0x22AC;
        offset += (uint)((uint)0x04 * Count);
        byteuintbuff = (uint)SaveData.OpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset = 0x243C;
        offset += (uint)((uint)0x04 * Count);
        byteuintbuff = (uint)SaveData.ClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuintbuff);

        offset = 0x25CC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.RTP;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x2694;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.SpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x275C;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.BonusSpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x2824;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.BonusIsPlayedCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        offset = 0x28EC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuff = (ushort)SaveData.BonusCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuff);

        goto End;
    #endregion

    End:
        ClearOpenClearPoint();
        ClearCoinInPoint();
        ClearTicketInPoint();
        ClearTicketOutPoint();
    }

    //讀取紀錄 輸入讀取的編號 1~100
    public void LoadHistory(int Number, out HistoryData LoadData)
    {
        UInt32 offset;
        HistoryData tmpData = new HistoryData();
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        ushort[] ushortbuf_2;

        int Count = 0;
        offset = 0x1500;
        ushortbuf = new ushort[100];
        for (int i = 0; i < 100; i++)
        {
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf[i]);
        }
        for (Count = 0; Count < 100; Count++)
        {
            if ((int)ushortbuf[Count] == Number)
            {
                tmpData.Number = (int)ushortbuf[Count];
                break;
            }
            if (Count == 99 && (int)ushortbuf[Count] != Number)
            {
                LoadData = tmpData;
                return;
            }
        }

        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            offset = 0x15C8;
            offset += (uint)((uint)0x05 * (uint)Count);
            result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * i)), out bytebuf[i]);
            tmpData.RNG[i] = (int)bytebuf[i];
            offset = 0x4100;
            offset += (uint)((uint)0x0A * (uint)Count);
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf_2[i]);
            tmpData.RNG[i] += (int)ushortbuf_2[i];
        }

        offset = 0x17BC;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if ((int)bytebuf[0] == 0)
        {
            tmpData.Bonus = false;
        }
        else
        {
            tmpData.Bonus = true;
        }

        offset = 0x1820;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        int CreditInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        int CreditDecimal = (int)ushortbuf[0];

        if (CreditDecimal == 0)
        {
            tmpData.Credit = CreditInteger;
        }
        else
        {
            tmpData.Credit = CreditInteger + ((double)CreditDecimal * 0.01);
        }

        offset = 0x1A78;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        tmpData.Demon = (int)byteuint[0];

        offset = 0x1C08;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.Bet = (int)ushortbuf[0];

        offset = 0x1CD0;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        tmpData.Odds = (int)bytebuf[0];

        offset = 0x1D34;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        int WinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        int WinDecimal = (int)ushortbuf[0];

        if (WinDecimal == 0)
        {
            tmpData.Win = WinInteger;
        }
        else
        {
            tmpData.Win = WinInteger + ((double)WinDecimal * 0.01);
        }

        offset = 0x1F8C;
        offset += (uint)((uint)0x08 * Count);
        byte[] EventTime;
        EventTime = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * i)), out EventTime[i]);
        }
        tmpData.Time = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));

        offset = 0x22AC;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        tmpData.OpenPoint = (int)byteuint[0];

        offset = 0x243C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        tmpData.ClearPoint = (int)byteuint[0];

        offset = 0x25CC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.RTP = (int)ushortbuf[0];

        offset = 0x2694;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.SpecialTime = (int)ushortbuf[0];

        offset = 0x275C;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.BonusSpecialTime = (int)ushortbuf[0];

        offset = 0x2824;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.BonusIsPlayedCount = (int)ushortbuf[0];

        offset = 0x28EC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.BonusCount = (int)ushortbuf[0];

        LoadData = tmpData;
    }

    //儲存最大獎歷史紀錄 輸入要覆寫哪個編號(如果還有空間則不會覆寫 會直接使用新空間) 1~10
    public void SaveMaxGameHistory(int Number, HistoryData MaxGameSaveData)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        ushort[] ushortbuf_2;
        int Count = 0;
        int CreditInteger = 0, CreditDecimal = 0;
        int WinInteger = 0, WinDecimal = 0;
        byte[] EventTime;
        offset = 0x3000;
        ushortbuf = new ushort[10];
        for (int i = 0; i < 10; i++)
        {
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf[i]);
        }
        for (Count = 0; Count < 10; Count++)
        {
            if (ushortbuf[Count] == 0 && Count == 0)
            {
                goto SaveStart;
            }
            else if (ushortbuf[Count] == 0)
            {
                goto SaveNew;
            }
            else if (ushortbuf[Count] == Number)
            {
                goto SaveOverWrite;
            }
        }
        return;

    SaveStart:

        offset = 0x3000;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)1;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);


        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                offset = 0x3014;
                bytebuf[i] = 0xFF;
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x44E8;
                ushortbuf_2[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
            else
            {
                offset = 0x3014;
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x44E8;
                ushortbuf_2[i] = 0x00;
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
        }

        offset = 0x3046;
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3050;
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x308C;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x30B4;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x30C8;
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x30D2;
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x310E;
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), EventTime[i]);
        }

        offset = 0x315E;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x3186;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x31AE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31C2;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31D6;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31EA;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31FE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        goto End;

    SaveNew:

        offset = 0x3000;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)(Count + 1);
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);


        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                offset = 0x3014;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = 0xFF;
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x44E8;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
            else
            {
                offset = 0x3014;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x44E8;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = 0x00;
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
        }

        offset = 0x3046;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3050;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x308C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x30B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x30C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x30D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x310E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), EventTime[i]);
        }

        offset = 0x315E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x3186;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x31AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        goto End;

    SaveOverWrite:

        int[] _Number = new int[10];
        for (int i = 0; i < 10; i++)
        {
            _Number[i] = (int)ushortbuf[i];
            if (_Number[i] == Number)
            {
                Count = i;
            }
        }


        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                offset = 0x3014;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = 0xFF;
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x44E8;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
            else
            {
                offset = 0x3014;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x44E8;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = 0x00;
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
        }

        offset = 0x3046;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3050;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x308C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x30B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x30C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x30D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x310E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), EventTime[i]);
        }

        offset = 0x315E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x3186;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x31AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        goto End;

    End:
        return;
    }

    //讀取最大獎歷史紀錄 輸入讀取編號 1~10
    public void LoadMaxGameHistory(int Number, out HistoryData MaxGameLoadData)
    {
        UInt32 offset;
        HistoryData tmpData = new HistoryData();
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        ushort[] ushortbuf_2;
        int Count = 0;
        offset = 0x3000;
        ushortbuf = new ushort[10];
        for (int i = 0; i < 10; i++)
        {
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf[i]);
        }
        for (Count = 0; Count < 10; Count++)
        {
            if ((int)ushortbuf[Count] == Number)
            {
                tmpData.Number = (int)ushortbuf[Count];
                break;
            }
            if (Count == 9 && (int)ushortbuf[Count] != Number)
            {
                MaxGameLoadData = tmpData;
                return;
            }
        }

        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            offset = 0x3014;
            offset += (uint)((uint)0x05 * (uint)Count);
            result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * i)), out bytebuf[i]);
            tmpData.RNG[i] = (int)bytebuf[i];
            offset = 0x44E8;
            offset += (uint)((uint)0x0A * (uint)Count);
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf_2[i]);
            tmpData.RNG[i] += (int)ushortbuf_2[i];
        }

        offset = 0x3046;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if ((int)bytebuf[0] == 0)
        {
            tmpData.Bonus = false;
        }
        else
        {
            tmpData.Bonus = true;
        }

        offset = 0x3050;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        int CreditInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        int CreditDecimal = (int)ushortbuf[0];

        if (CreditDecimal == 0)
        {
            tmpData.Credit = CreditInteger;
        }
        else
        {
            tmpData.Credit = CreditInteger + ((double)CreditDecimal * 0.01);
        }

        offset = 0x308C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        tmpData.Demon = (int)byteuint[0];

        offset = 0x30B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.Bet = (int)ushortbuf[0];

        offset = 0x30C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        tmpData.Odds = (int)bytebuf[0];

        offset = 0x30D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        int WinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        int WinDecimal = (int)ushortbuf[0];

        if (WinDecimal == 0)
        {
            tmpData.Win = WinInteger;
        }
        else
        {
            tmpData.Win = WinInteger + ((double)WinDecimal * 0.01);
        }

        offset = 0x310E;
        offset += (uint)((uint)0x08 * Count);
        byte[] EventTime;
        EventTime = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * i)), out EventTime[i]);
        }
        tmpData.Time = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));

        offset = 0x315E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        tmpData.OpenPoint = (int)byteuint[0];

        offset = 0x3186;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        tmpData.ClearPoint = (int)byteuint[0];

        offset = 0x31AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.RTP = (int)ushortbuf[0];

        offset = 0x31C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.SpecialTime = (int)ushortbuf[0];

        offset = 0x31D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.BonusSpecialTime = (int)ushortbuf[0];

        offset = 0x31EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.BonusIsPlayedCount = (int)ushortbuf[0];

        offset = 0x31FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.BonusCount = (int)ushortbuf[0];
        MaxGameLoadData = tmpData;
    }

    //儲存最大獎歷史紀錄Second 輸入要覆寫哪個編號(如果還有空間則不會覆寫 會直接使用新空間) 1~10
    public void SaveMaxGameHistorySecond(int Number, HistoryData MaxGameSaveData)
    {

        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        ushort[] ushortbuf_2;
        byte[] bytebuf;
        int Count = 0;
        int CreditInteger = 0, CreditDecimal = 0;
        int WinInteger = 0, WinDecimal = 0;
        byte[] EventTime;
        offset = 0x3300;
        ushortbuf = new ushort[10];
        for (int i = 0; i < 10; i++)
        {
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf[i]);
        }
        for (Count = 0; Count < 10; Count++)
        {
            if (ushortbuf[Count] == 0 && Count == 0)
            {
                goto SaveStart;
            }
            else if (ushortbuf[Count] == 0)
            {
                goto SaveNew;
            }
            else if (ushortbuf[Count] == Number)
            {
                goto SaveOverWrite;
            }
        }
        return;

    SaveStart:

        offset = 0x3300;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)1;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                offset = 0x3314;
                bytebuf[i] = 0xFF;
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x454C;
                ushortbuf_2[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
            else
            {
                offset = 0x3314;
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x454C;
                ushortbuf_2[i] = 0x00;
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
        }


        offset = 0x3346;
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3350;
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x338C;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x33B4;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x33C8;
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x33D2;
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x340E;
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), EventTime[i]);
        }

        offset = 0x345E;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x3486;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x34AE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34C2;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x31D6;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34EA;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34FE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        goto End;

    SaveNew:

        offset = 0x3300;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)(Count + 1);
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                offset = 0x3314;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = 0xFF;
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x454C;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
            else
            {
                offset = 0x3314;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x454C;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = 0x00;
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
        }

        offset = 0x3346;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3350;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x338C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x33B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x33C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x33D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x340E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), EventTime[i]);
        }

        offset = 0x345E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x3486;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x34AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        goto End;

    SaveOverWrite:

        int[] _Number = new int[10];
        for (int i = 0; i < 10; i++)
        {
            _Number[i] = (int)ushortbuf[i];
            if (_Number[i] == Number)
            {
                Count = i;
            }
        }

        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                offset = 0x3314;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = 0xFF;
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x454C;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
            else
            {
                offset = 0x3314;
                offset += (uint)((uint)0x05 * Count);
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), bytebuf[i]);
                offset = 0x454C;
                offset += (uint)((uint)0x0A * Count);
                ushortbuf_2[i] = 0x00;
                result = NVRAM.qxt_nvram_writeword(offset + ((uint)(0x02 * i)), ushortbuf_2[i]);
            }
        }


        offset = 0x3346;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3350;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x338C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x33B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x33C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x33D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x340E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_writebyte(offset + ((uint)(0x01 * i)), EventTime[i]);
        }

        offset = 0x345E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x3486;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

        offset = 0x34AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        offset = 0x34FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf[0]);

        goto End;

    End:
        return;
    }

    //讀取最大獎歷史紀錄Second 輸入讀取編號 1~10
    public void LoadMaxGameHistorySecond(int Number, out HistoryData MaxGameLoadData)
    {
        UInt32 offset;
        HistoryData tmpData = new HistoryData();
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        ushort[] ushortbuf_2;
        int Count = 0;
        offset = 0x3300;
        ushortbuf = new ushort[10];
        for (int i = 0; i < 10; i++)
        {
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf[i]);
        }
        for (Count = 0; Count < 10; Count++)
        {
            if ((int)ushortbuf[Count] == Number)
            {
                tmpData.Number = (int)ushortbuf[Count];
                break;
            }
            if (Count == 9 && (int)ushortbuf[Count] != Number)
            {
                MaxGameLoadData = tmpData;
                return;
            }
        }

        bytebuf = new byte[5];
        ushortbuf_2 = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            offset = 0x3314;
            offset += (uint)((uint)0x05 * (uint)Count);
            result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * i)), out bytebuf[i]);
            tmpData.RNG[i] = (int)bytebuf[i];
            offset = 0x454C;
            offset += (uint)((uint)0x0A * (uint)Count);
            result = NVRAM.qxt_nvram_readword(offset + ((uint)(0x02 * i)), out ushortbuf_2[i]);
            tmpData.RNG[i] += (int)ushortbuf_2[i];
        }

        offset = 0x3346;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if ((int)bytebuf[0] == 0)
        {
            tmpData.Bonus = false;
        }
        else
        {
            tmpData.Bonus = true;
        }

        offset = 0x3350;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        int CreditInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        int CreditDecimal = (int)ushortbuf[0];

        if (CreditDecimal == 0)
        {
            tmpData.Credit = CreditInteger;
        }
        else
        {
            tmpData.Credit = CreditInteger + ((double)CreditDecimal * 0.01);
        }

        offset = 0x338C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        tmpData.Demon = (int)byteuint[0];

        offset = 0x33B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.Bet = (int)ushortbuf[0];

        offset = 0x33C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        tmpData.Odds = (int)bytebuf[0];

        offset = 0x33D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        int WinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        int WinDecimal = (int)ushortbuf[0];

        if (WinDecimal == 0)
        {
            tmpData.Win = WinInteger;
        }
        else
        {
            tmpData.Win = WinInteger + ((double)WinDecimal * 0.01);
        }

        offset = 0x340E;
        offset += (uint)((uint)0x08 * Count);
        byte[] EventTime;
        EventTime = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            result = NVRAM.qxt_nvram_readbyte(offset + ((uint)(0x01 * i)), out EventTime[i]);
        }
        tmpData.Time = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));

        offset = 0x345E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        tmpData.OpenPoint = (int)byteuint[0];

        offset = 0x3486;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        tmpData.ClearPoint = (int)byteuint[0];

        offset = 0x34AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.RTP = (int)ushortbuf[0];

        offset = 0x34C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.SpecialTime = (int)ushortbuf[0];

        offset = 0x34D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.BonusSpecialTime = (int)ushortbuf[0];

        offset = 0x34EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.BonusIsPlayedCount = (int)ushortbuf[0];

        offset = 0x34FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        tmpData.BonusCount = (int)ushortbuf[0];
        MaxGameLoadData = tmpData;
    }

    //儲存目前開洗分 輸入開分(true) 洗分(false) + 值
    public void SaveOpenClearPoint(bool OpenClear, int Value)
    {
        UInt32 offset;
        uint[] byteuint;
        if (OpenClear)
        {
            offset = 0x60;
            byteuint = new uint[1];
            byteuint[0] = (uint)Value;
            result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);
        }
        else
        {
            offset = 0x64;
            byteuint = new uint[1];
            byteuint[0] = (uint)Value;
            result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);
        }
    }
    //讀取目前開洗分 輸入開分(true) 洗分(false) + 值
    public int LoadOpenClearPoint(bool OpenClear)
    {
        int Value;
        UInt32 offset;
        uint[] byteuint;
        if (OpenClear)
        {
            offset = 0x60;
            byteuint = new uint[1];
            result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
            Value = (int)byteuint[0];
        }
        else
        {
            offset = 0x64;
            byteuint = new uint[1];
            result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
            Value = (int)byteuint[0];
        }
        return Value;
    }

    public void SaveCoinInPoint(int Value)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x80;
        byteuint = new uint[1];
        byteuint[0] = (uint)Value;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);

    }

    public int LoadCoinInPoint()
    {

        int Value;
        UInt32 offset;
        uint[] byteuint;
        offset = 0x80;

        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        Value = (int)byteuint[0];

        return Value;
    }
    public void SaveTicketInPoint(double TicketIn)
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;
        if (TicketIn % 1 != 0)
        {
            TotalTicketInDecimal = (int)SaveDecimalPointHandle(TicketIn);
            TotalTicketInInteger = (int)TicketIn;
        }
        else
        {
            TotalTicketInInteger = (int)TicketIn;
        }
        offset = 0x4636;
        byteuint = (uint)TotalTicketInInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4640;
        ushortbuf = (ushort)TotalTicketInDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public double LoadTicketInPoint()//讀取總入票
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TicketIn;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;
        offset = 0x4636;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalTicketInInteger = (int)byteuint;
        offset = 0x4640;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalTicketInDecimal = (int)ushortbuf;
        if (TotalTicketInDecimal == 0)
        {
            TicketIn = TotalTicketInInteger;
        }
        else
        {
            TicketIn = TotalTicketInInteger + (TotalTicketInDecimal * 0.01);
        }
        return TicketIn;
    }
    public void SaveTicketOutPoint(double TicketOut)
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        int TotalTicketOutInteger = 0;
        int TotalTicketOutDecimal = 0;
        if (TicketOut % 1 != 0)
        {
            TotalTicketOutDecimal = (int)SaveDecimalPointHandle(TicketOut);
            TotalTicketOutInteger = (int)TicketOut;
        }
        else
        {
            TotalTicketOutInteger = (int)TicketOut;
        }
        offset = 0x4642;
        byteuint = (uint)TotalTicketOutInteger;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4646;
        ushortbuf = (ushort)TotalTicketOutDecimal;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public double LoadTicketOutPoint()//讀取總入票
    {
        UInt32 offset;
        uint byteuint;
        ushort ushortbuf;
        double TicketOut;
        int TotalTicketOutInteger = 0;
        int TotalTicketOutDecimal = 0;
        offset = 0x4642;
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        TotalTicketOutInteger = (int)byteuint;
        offset = 0x4646;
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        TotalTicketOutDecimal = (int)ushortbuf;
        if (TotalTicketOutDecimal == 0)
        {
            TicketOut = TotalTicketOutInteger;
        }
        else
        {
            TicketOut = TotalTicketOutInteger + (TotalTicketOutDecimal * 0.01);
        }
        return TicketOut;
    }

    //清空目前開洗分
    public void ClearOpenClearPoint()
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x60;
        byteuint = 0;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        result = NVRAM.qxt_nvram_writedword(offset + (uint)0x04, byteuint);
    }
    public void ClearCoinInPoint()
    {
        UInt32 offset;
        uint byteuint;

        offset = 0x80;
        byteuint = 0;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);

    }
    public void ClearTicketInPoint()
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        offset = 0x4636;
        byteuint = 0;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4640;
        ushortbuf = 0;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public void ClearTicketOutPoint()
    {
        UInt32 offset;
        ushort ushortbuf;
        uint byteuint;
        offset = 0x4642;
        byteuint = 0;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        offset = 0x4646;
        ushortbuf = 0;
        result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }

    public void SaveClearCount(int clearCount)//紀錄剩餘洗分次數
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x40;
        byteuint = new uint[1];
        byteuint[0] = (uint)clearCount;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);
    }
    public int LoadClearCount()//讀取剩餘洗分次數
    {
        int clearCount;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x40;
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        clearCount = (int)byteuint[0];
        return clearCount;
    }

    public void SaveMonthCheckDate(int month)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x68;
        byteuint = new uint[1];
        byteuint[0] = (uint)month;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);
    }
    public int LoadMonthCheckData()
    {
        int monthData;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x68;
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        monthData = (int)byteuint[0];
        return monthData;
    }

    //儲存是否要月報
    public void SaveIsMonthCheck(bool isMonthcheck)
    {
        UInt32 offset = 0x6C;
        byte[] bytebuf = new byte[1];
        if (isMonthcheck) bytebuf[0] = (byte)1;
        else bytebuf[0] = (byte)0;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取是否要月報
    public bool LoadIsMonthCheck()
    {
        UInt32 offset = 0x6C;
        byte[] bytebuf = new byte[1];
        int tmp;
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        tmp = (int)bytebuf[0];
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    //儲存票紙序號
    public void SaveTicketSerial(string serial)
    {
        UInt32 offset = 0x4656;
        if (String.IsNullOrWhiteSpace(serial))
        {
            ushort ushortbuf;
            ushortbuf = 0;
            result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
        }
        else
        {
            string[] saveSerial = serial.Split('-');
            for (int i = 0; i < 4; i++)
            {
                ushort ushortbuf;
                ushortbuf = (ushort)ushort.Parse(saveSerial[i]);
                result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
                offset += 0x02;
            }
        }
    }
    //讀取是否要月報
    public string LoadTicketSerial()
    {
        UInt32 offset = 0x4656;
        ushort[] ushortbuf = new ushort[1];
        result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
        string loadSerial = null;
        if (ushortbuf[0] == 0)
        {
            return "";
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                result = NVRAM.qxt_nvram_readword(offset, out ushortbuf[0]);
                loadSerial += ushortbuf[0].ToString().PadLeft(4, '0');
                if (i < 3) loadSerial += "-";
                offset += 0x02;
            }
            return loadSerial;
        }
    }
    //儲存是否要開啟開分洗分鍵
    public void SaveOpenScoreButtonSet(bool isOpenSet)
    {
        UInt32 offset = 0x8A;
        byte[] bytebuf = new byte[1];
        if (isOpenSet) bytebuf[0] = (byte)1;
        else bytebuf[0] = (byte)0;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取是否要要開啟開分洗分鍵
    public bool LoadOpenScoreButtonSet()
    {
        UInt32 offset = 0x8A;
        byte[] bytebuf = new byte[1];
        int tmp;
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        tmp = (int)bytebuf[0];
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    int[] rtpArray;
    int nowRtp;
    bool rtpAll;
    public void InitializeSetting()//初始化設定
    {

        //儲存備份紀錄
        SaveBackupStatistcalData(LoadBackupTotalOpenPoint() + LoadTotalOpenPoint(), LoadBackupTotalClearPoint() + LoadTotalClearPoint(), LoadBackupTotalCoinIn() + LoadTotalCoinIn(),
            LoadBackupTotalBet() + LoadTotalBet(), LoadBackupTotalWin() + LoadTotalWin(), LoadBackupTotalGamePlay() + LoadTotalGamePlay(), LoadBackupTotalWinGamePlay() + LoadTotalWinGamePlay(), LoadTotalTicketIn(), LoadTotalTicketOut());
        //初始化總帳戶
        SaveTotalStatistcalData(initData.OpenPointTotal, initData.ClearPointTotal, initData.CashIn, initData.BetCredit, initData.WinScore, initData.GameCount, initData.WinCount, initData.TikcetIn, initData.TicketOut);
        //初始化班帳戶
        SaveClassStatistcalData(initData.OpenPointTotal, initData.ClearPointTotal, initData.CashIn, initData.BetCredit, initData.WinScore, initData.GameCount, initData.WinCount, initData.TikcetIn, initData.TicketOut);
        //初始化密碼
        SavePassword(initData.AdminPassword);

        //初始化鈔機設定
        SaveBanknoteMachineSetting(initData.UBA_Enable == 1 ? true : false);
        //初始化開分設定
        SaveOpenPointSetting(initData.OpenPointOnce);
        //初始化洗分設定
        SaveClearPointSetting(initData.ClearPointOnce);
        //初始化最大押注設定
        SaveMaxOdd(initData.MaxOdds);

        //初始化最大彩分設定
        SaveMaxCredit(initData.MaxCredit);

        //初始化最大贏分設定
        SaveMaxWin(initData.MaxWinScore);
        ClearHistory();
#if !UNITY_EDITOR
        Mod_Data.maxOdds = LoadMaxOdd();
        Mod_Data.maxCredit = LoadMaxCredit();
        Mod_Data.maxWin = LoadMaxWin();
#endif
        //初始化倍率開啟設定
        SaveDenomOption(initData.DenomSelect);
        LoadDenomOption(out Mod_Data.denomOpenArray);
        //初始化RTP設定
        SaveRTPSetting(initRtp.RTP_Array, initRtp.RTP_All);

        GameObject.Find("BackEndManager").GetComponent<NewSramManager>().LoadRTPSetting(out rtpArray, out rtpAll);

        if (Mod_Data.Denom == 0.01) { nowRtp = rtpArray[8]; }
        else if (Mod_Data.Denom == 0.02) { nowRtp = rtpArray[7]; }
        else if (Mod_Data.Denom == 0.025) { nowRtp = rtpArray[6]; }
        else if (Mod_Data.Denom == 0.05) { nowRtp = rtpArray[5]; }
        else if (Mod_Data.Denom == 0.1) { nowRtp = rtpArray[4]; }
        else if (Mod_Data.Denom == 0.25) { nowRtp = rtpArray[3]; }
        else if (Mod_Data.Denom == 0.5) { nowRtp = rtpArray[2]; }
        else if (Mod_Data.Denom == 1) { nowRtp = rtpArray[1]; }
        else if (Mod_Data.Denom == 2.5) { nowRtp = rtpArray[0]; }

        Mod_Data.RTPsetting = nowRtp;
        GameObject.Find("GameController").GetComponent<Mod_MathController>().ChangeMathFile(nowRtp);
        //初始化是否有存值設定
        Mod_Data.credit = 0;
        Mod_Data.Win = 0;
        Mod_Data.Pay = 0;
        Mod_Data.odds = 1;
        Mod_Data.Denom = 1;
        Mod_Data.Bet = Mod_Data.BetOri;
        //SaveIsSramTrue(false);
        //SaveEventRecored();
    }
    public void DebugSram()
    {
        //Debug.Log("LoadTotalOpenPoint:" + LoadTotalOpenPoint());
        //Debug.Log("LoadTotalClearPoint:" + LoadTotalClearPoint());
        //Debug.Log("LoadTotalCoinIn:" + LoadTotalCoinIn());
        //Debug.Log("LoadTotalBet:" + LoadTotalBet());
        //Debug.Log("LoadTotalWin:" + LoadTotalWin());
        //Debug.Log("LoadTotalGamePlay:" + LoadTotalGamePlay());
        //Debug.Log("LoadTotalWinGamePlay:" + LoadTotalWinGamePlay());
        //Debug.Log("LoadPassword:" + LoadPassword());
        //Debug.Log("LoadOpenPointSetting:" + LoadOpenPointSetting());
        //Debug.Log("LoadClearPointSetting:" + LoadClearPointSetting());
        //Debug.Log("LoadMaxOdd:" + LoadMaxOdd());
        //Debug.Log("LoadMaxCredit:" + LoadMaxCredit());
        //Debug.Log("LoadMaxWin:" + LoadMaxWin());

        //Debug.Log("SaveIsSramTrue:" + LoadIsSramTrue());
    }

    public void InitializeClaseAccount()
    {
        StatisticalData initData = new StatisticalData();
        SaveClassStatistcalData(initData.OpenPointTotal, initData.ClearPointTotal, initData.CashIn, initData.BetCredit, initData.WinScore, initData.GameCount, initData.WinCount, initData.TikcetIn, initData.TicketOut);
    }


    //COMPort               int     1   0x68 6D
    //BillName              int     1   0x69 6E
    //TicketEnable          bool    1   0x6A 6F
    //儲存鈔機COMPort
    public void SaveBanknoteMachineCOMPort(string COMPort)
    {
        UInt32 offset = 0x6D;
        byte[] bytebuf = new byte[1];
        bytebuf[0] = System.Convert.ToByte(COMPort.Substring(COMPort.Length - 1, 1));
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取鈔機COMPort
    public void LoadBanknoteMachineCOMPort(out string COMPort)
    {
        UInt32 offset = 0x6D;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        COMPort = "COM" + bytebuf[0];
        if (COMPort == "COM0") COMPort = "COM1";
    }
    //儲存鈔機型號
    public void SaveBanknoteMachineName(string BillName)
    {
        UInt32 offset = 0x6E;
        byte[] bytebuf = new byte[1];
        switch (BillName)
        {
            case "JCM":
                bytebuf[0] = 1;
                break;
            case "MEI":
                bytebuf[0] = 2;
                break;
            case "ICT":
                bytebuf[0] = 3;
                break;
            default:
                return;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取鈔機型號
    public void LoadBanknoteMachineName(out string BillName)
    {
        UInt32 offset = 0x6E;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        switch (bytebuf[0])
        {
            case 1:
                BillName = "JCM";
                break;
            case 2:
                BillName = "MEI";
                break;
            case 3:
                BillName = "ICT";
                break;
            default:
                BillName = "JCM";
                break;
        }
    }
    //儲存吸票開關
    public void SaveBanknoteMachineTicketEnable(bool Enable)
    {
        UInt32 offset = 0x6F;
        byte[] bytebuf = new byte[1];
        if (Enable)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取吸票開關
    public void LoadBanknoteMachineTicketEnable(out bool Enable)
    {
        UInt32 offset = 0x6F;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if (bytebuf[0] == 1)
        {
            Enable = true;
        }
        else
        {
            Enable = false;
        }
    }
    //儲存吸鈔還是吸票
    public void SaveBillMachineCashOrTicketEnable(int select)
    {
        UInt32 offset = 0x78;
        byte[] bytebuf = new byte[1];
        switch (select)
        {
            case 0: bytebuf[0] = 0; break;
            case 1: bytebuf[0] = 1; break;
            case 2: bytebuf[0] = 2; break;
            default: bytebuf[0] = 0; break;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取吸鈔還是吸票
    public void LoadBillMachineCashOrTicketEnable(out int select)
    {
        UInt32 offset = 0x78;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        switch (bytebuf[0])
        {
            case 0: select = 0; break;
            case 1: select = 1; break;
            case 2: select = 2; break;
            default: select = 0; break;
        }
    }
    //儲存手付狀態
    public void SaveHandPayStatus(int HandPay)
    {
        //HandPay 0 取消手付狀態
        //HandPay 1 出票金額異常狀態
        //HandPay 2 入票異常狀態
        //HandPay 3 出票金額異常、入票異常狀態
        UInt32 offset = 0x79;
        byte[] bytebuf = new byte[1];
        bytebuf[0] = (byte)HandPay;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取手付狀態
    public int LoadHandPayStatus()
    {
        UInt32 offset = 0x79;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        return bytebuf[0];
    }
    //儲存票機開關狀態
    public void SavePrinterEnable(bool Enable)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        if (Enable)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取票機開關狀態
    public void LoadPrinterEnable(out bool Enable)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if (bytebuf[0] == 1)
        {
            Enable = true;
        }
        else
        {
            Enable = false;
        }
    }
    //儲存票機COMPort
    public void SavePrinterCOMPort(int COMPort)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        bytebuf[0] = (byte)COMPort;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取票機COMPort
    public void LoadPrinterCOMPort(out string COMPort)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        COMPort = "COM" + bytebuf[0];
        if (COMPort == "COM0") COMPort = "COM1";
    }
    //儲存邏輯機門狀態
    public void SaveLogicDoorStatus(bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取邏輯機門狀態
    public void LoadLogicDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
    }
    //儲存上機門狀態
    public void SaveMainDoorStatus(bool Error)
    {
        UInt32 offset = 0x7D;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取上機門狀態
    public void LoadMainDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7D;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
    }
    //儲存下機門狀態
    public void SaveBellyDoorStatus(bool Error)
    {
        UInt32 offset = 0x7E;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取下機門狀態
    public void LoadBellyDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7E;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
    }
    //儲存鈔機門狀態
    public void SaveCashDoorStatus(bool Error)
    {
        UInt32 offset = 0x7F;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取鈔機門狀態
    public void LoadCashDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7F;
        byte[] bytebuf = new byte[1];
        result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
    }
    //儲存過場動畫 魚的倍率
    public void SaveBonusFish(int fishNum, int Muitl)
    {
        UInt32 offset = 0x5000;
        uint[] byteuint;
        offset = offset + (uint)(fishNum * 4);
        byteuint = new uint[1];
        byteuint[0] = (uint)Muitl;
        result = NVRAM.qxt_nvram_writedword(offset, (byte)byteuint[0]);
    }
    //讀取過場動畫 魚的倍率
    public int LoadBonusFish(int fishNum)
    {
        UInt32 offset = 0x5000;
        uint[] byteuint;
        offset = offset + (uint)(fishNum * 4);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        return (int)byteuint[0];
    }
    //儲存過場動畫 船的場次
    public void SaveBonusBoat(int BoatNum, int time)
    {
        UInt32 offset = 0x5014;
        uint[] byteuint;
        offset = offset + (uint)(BoatNum * 4);
        byteuint = new uint[1];
        byteuint[0] = (uint)time;
        result = NVRAM.qxt_nvram_writedword(offset, (byte)byteuint[0]);
    }
    //讀取過場動畫 船的場次
    public int LoadBonusBoat(int BoatNum)
    {
        UInt32 offset = 0x5014;
        uint[] byteuint;
        offset = offset + (uint)(BoatNum * 4);
        byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        return (int)byteuint[0];
    }
    //儲存過場動畫 已選擇寶箱倍率
    public void SaveSpeicalTime(int SpeicalTime)
    {
        UInt32 offset = 0x5028;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)SpeicalTime;
        result = NVRAM.qxt_nvram_writedword(offset, (byte)byteuint[0]);
    }
    //讀取過場動畫 已選擇寶箱倍率
    public int LoadSpeicalTime()
    {
        UInt32 offset = 0x5028;
        uint[] byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        return (int)byteuint[0];
    }
    //儲存過場動畫 已選擇船的場次
    public void SaveBonusCount(int BonusCount)
    {
        UInt32 offset = 0x502C;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)BonusCount;
        result = NVRAM.qxt_nvram_writedword(offset, (byte)byteuint[0]);
    }
    //讀取過場動畫 已選擇船的場次
    public int LoadBonusCount()
    {
        UInt32 offset = 0x502C;
        uint[] byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        return (int)byteuint[0];
    }
    //儲存過場動畫 使用者選擇的寶箱路線
    public void SaveUserSelectedSpeicalTime(int SpeicalTime)
    {
        UInt32 offset = 0x5030;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)SpeicalTime;
        result = NVRAM.qxt_nvram_writedword(offset, (byte)byteuint[0]);
    }
    //讀取過場動畫 使用者選擇的寶箱路線
    public int LoadUserSelectedSpeicalTime()
    {
        UInt32 offset = 0x5030;
        uint[] byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        return (int)byteuint[0];
    }
    //儲存過場動畫 使用者選擇的船路線
    public void SaveUserSelectedBonusCount(int BonusCount)
    {
        UInt32 offset = 0x5034;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)BonusCount;
        result = NVRAM.qxt_nvram_writedword(offset, (byte)byteuint[0]);
    }
    //讀取過場動畫 使用者選擇的船路線
    public int LoadUserSelectedBonusCount()
    {
        UInt32 offset = 0x5034;
        uint[] byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        return (int)byteuint[0];
    }
    //儲存本機場次編號
    public void SaveLocalGameRound(int Round)
    {
        UInt32 offset = 0x4648;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)Round;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);
    }
    //讀取本機場次編號
    public void LoadLocalGameRound(out int Round)
    {
        UInt32 offset = 0x4648;
        uint[] byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        Round = (int)byteuint[0];
    }
    //儲存本機Bonus場次編號
    public void SaveLocalBonusGameRound(int BonusRound)
    {
        UInt32 offset = 0x4652;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)BonusRound;
        result = NVRAM.qxt_nvram_writedword(offset, byteuint[0]);
    }
    //讀取本Bonus機場次編號
    public void LoadLocalBonusGameRound(out int BonusRound)
    {
        UInt32 offset = 0x4652;
        uint[] byteuint = new uint[1];
        result = NVRAM.qxt_nvram_readdword(offset, out byteuint[0]);
        BonusRound = (int)byteuint[0];
    }

    #region 應該沒用但是不敢刪

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        int mods = 0;
        if (!IsInitialized)
            sb.Append("???");
        else
        {
            if ((OpMode & OPMODE_IDS.OPMODE_NORMAL) > 0)
                sb.Append("No");
            if ((OpMode & OPMODE_IDS.OPMODE_BANK) > 0)
                sb.Append("Ba");
            if ((OpMode & OPMODE_IDS.OPMODE_MIRROR) > 0)
                sb.Append("Mi");
            if ((OpMode & OPMODE_IDS.OPMODE_AUTOVERIFY) > 0)
            {
                if (mods == 0)
                    sb.Append("(V");
                else
                    sb.Append("|V");
                mods++;
            }
            if ((OpMode & OPMODE_IDS.OPMODE_CONTIGUOUS_BANKS) > 0)
            {
                if (mods == 0)
                    sb.Append("(C");
                else
                    sb.Append("|C");
                mods++;
            }
            if ((OpMode & OPMODE_IDS.OPMODE_RW_SYNC) > 0)
            {
                if (mods == 0)
                    sb.Append("(S");
                else
                    sb.Append("|S");
                mods++;
            }

            if (mods > 0)
                sb.Append(")");

            if (HasInterrupts)
                sb.Append("!");
        }

        return sb.ToString();
    }

    private static void PrintResultData(NVRAMResultCode code, OperationRequested op, nvramResultData data)
    {
        switch (op)
        {
            case OperationRequested.CRC:
                {
                    if (!NVRAMResult.IsError(code))
                        Console.WriteLine("Resulting CRC is 0x{0:X8}", data.value);
                }
                break;
            case OperationRequested.BlockCompare:
                {
                    if (NVRAMResult.IsError(code))
                    {
                        Console.WriteLine("First difference at offset +{0:X8}", data.address);
                        Console.WriteLine("Bank0:\t+{0:X8}\nBank1:\t+{1:X8}\nBank2:\t+{2:X8}\nBank3:\t+{3:X8}", data.bank0, data.bank1, data.bank2, data.bank3);
                    }
                }
                break;
            case OperationRequested.BlockCopy:
                {
                    if (NVRAMResult.IsError(code))
                        Console.WriteLine("First error at offset +{0:X8}", data.address);
                }
                break;
        }
    }

    private static void Dev_OnDeviceNotification(object sender, NVRAMNotificationEventArgs e)
    {
        lock (ms)
        {
            ms.HasInterrupts = true;
            ms.InterruptData = new NVRAMNotificationEventArgs(e.MessageFlags, e.Payload, e.CRC, e.StorageAddress);
            ms.InterruptTimestamp = DateTime.Now;
        }
    }

    private static void ShowHexBuffer(uint startOffset, byte[] buffer)
    {
        StringBuilder sb = new StringBuilder();
        int remnants = 0;

        sb.Append("\t");

        for (uint i = 0; i < buffer.Length; i++)
        {
            if (i % 8 == 0)
                Console.Write("+{0:X8} \t", startOffset + i);

            Console.Write("{0:X2} ", buffer[i]);

            if (buffer[i] < 0x20 || buffer[i] > 0x7F)
                sb.Append(".");
            else
                sb.Append(Encoding.ASCII.GetString(buffer, (int)i, 1));

            if (i % 8 == 7)
            {
                Console.WriteLine(sb.ToString());
                sb.Remove(0, sb.Length);
                sb.Append("\t");
            }
        }
        remnants = buffer.Length % 8 != 0 ? 8 - (buffer.Length % 8) : 0;

        for (int i = 0; i < remnants; i++)
            Console.Write("   ");

        if (remnants != 0)
        {
            Console.WriteLine(sb.ToString());
        }
    }

    private static bool ParseBankMask(string input, out int mask)
    {
        int outMask = 0;

        if (input.Length < 4 || input.Length > 8)
        {
            mask = outMask;
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            switch (input[input.Length - 1 - i])
            {
                case '0': break;
                case '1': outMask |= (1 << i); break;
                default: mask = outMask; return false;
            }
        }

        mask = outMask;
        return true;
    }

    private static bool ParseBankMode(string input, out int mode)
    {
        string[] modes = input.Split('|');
        int outMode = 0;

        foreach (string s in modes)
            switch (s.ToLower())
            {
                case "normal": outMode |= (int)OPMODE_IDS.OPMODE_NORMAL; break;
                case "bank": outMode |= (int)OPMODE_IDS.OPMODE_BANK; break;
                case "mirror": outMode |= (int)OPMODE_IDS.OPMODE_MIRROR; break;
                case "verify": outMode |= (int)OPMODE_IDS.OPMODE_AUTOVERIFY; break;
                case "contiguous": outMode |= (int)OPMODE_IDS.OPMODE_CONTIGUOUS_BANKS; break;
                case "sync": outMode |= (int)OPMODE_IDS.OPMODE_RW_SYNC; break;
                default: mode = 0; return false;
            }

        mode = outMode;
        return true;
    }

    private static void ShowHelp()
    {
        Console.WriteLine("\nh, help, ?\t\tShows this help");
        Console.WriteLine("i,init\t\t\tOpens NVRAM device");
        Console.WriteLine("o,opmode x [m]\t\tSets NVRAM opmode to x, with an optional bank mask m");
        Console.WriteLine("w,write [+x] <data>\tWrites <data> ASCII bytes to NVRAM");
        Console.WriteLine("r,read [+x] n\t\tReads n bytes from NVRAM");
        Console.WriteLine("bankcopy [+x] n t\tCopies n bytes to t targets");
        Console.WriteLine("bankcmp [+x] n t\tCompares n bytes among t targets");
        Console.WriteLine("crc [+x] n t\t\tPerforms CRC check of stored data");
        Console.WriteLine("info\t\t\tGets NVRAM info");
        Console.WriteLine("s,shutdown\t\tCloses NVRAM device");
        Console.WriteLine("int\t\t\tShows pending device notification");
        Console.WriteLine("x, exit\t\t\tQuits NVRAM example\n");
    }

    private static string[] showPrompt(NewSramManager ms)
    {
        string prompt;

        lock (ms)
        {
            prompt = ms.ToString();
        }

        Console.Write("[{0}]# ", prompt);
        return Console.ReadLine().Split(' ');
    }

    public enum OperationRequested
    {
        CRC,
        BlockCopy,
        BlockCompare
    }

    #endregion


    private void OnApplicationQuit()
    {
        NVRAM_Disconnect();
    }

#endif
    #endregion

    #region AGPSram
#if AGP
    StatisticalData initData = new StatisticalData();
    RTP_SettingData initRtp = new RTP_SettingData();

    #region 存讀總紀錄
    //儲存總紀錄
    public void SaveTotalStatistcalData(int TotalOpenPoint, int TotalClearPoint, int TotalCoinIn, double TotalBet, double TotalWin, int TotalGamePlay, int TotalWinGamePlay)//開分 洗分 總押注 總贏分 遊戲場次 贏場次
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalBetInteger = 0, TotalWinInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0;
        if (TotalBet % 1 != 0)
        {
            TotalBetDecimal = (int)SaveDecimalPointHandle(TotalBet);
            TotalBetInteger = (int)TotalBet;
        }
        else
        {
            TotalBetInteger = (int)TotalBet;
        }

        if (TotalWin % 1 != 0)
        {
            TotalWinDecimal = (int)SaveDecimalPointHandle(TotalWin);
            TotalWinInteger = (int)TotalWin;
        }
        else
        {
            TotalWinInteger = (int)TotalWin;
        }

        offset = 0x00;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalOpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x04;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x70;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalCoinIn;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x14;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalGamePlay;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x18;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinGamePlay;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x08;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalBetInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalBetDecimal;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x0E;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalWinDecimal;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }

    public void SaveTotalOpenPoint(int TotalOpenPoint)//總開分
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x00;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalOpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }

    public void SaveTotalClearPoint(int TotalClearPoint)//總洗分
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x04;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }

    public void SaveTotalCoinIn(int TotalCoinIn)//總入鈔
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x70;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalCoinIn;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    public void SaveTotalBet(double TotalBet)//總押注
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalBetInteger = 0;
        int TotalBetDecimal = 0;
        if (TotalBet % 1 != 0)
        {
            TotalBetDecimal = (int)SaveDecimalPointHandle(TotalBet);
            TotalBetInteger = (int)TotalBet;
        }
        else
        {
            TotalBetInteger = (int)TotalBet;
        }

        offset = 0x08;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalBetInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalBetDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
    }

    public void SaveTotalWin(double TotalWin)//總贏分
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalWinInteger = 0;
        int TotalWinDecimal = 0;
        if (TotalWin % 1 != 0)
        {
            TotalWinDecimal = (int)SaveDecimalPointHandle(TotalWin);
            TotalWinInteger = (int)TotalWin;
        }
        else
        {
            TotalWinInteger = (int)TotalWin;
        }
        offset = 0x0E;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalWinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
    }

    public void SaveTotalGamePlay(double TotalGamePlay)//總遊戲場次
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x14;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalGamePlay;

        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    public void SaveTotalWinGamePlay(double TotalWinGamePlay)//總贏場次
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x18;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinGamePlay;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }

    //讀取總紀錄
    public void LoadTotalStatistcalData(out int TotalOpenPoint, out int TotalClearPoint, out int TotalCoinIn, out double TotalBet, out double TotalWin, out int TotalGamePlay, out int TotalWinGamePlay)//開分 洗分 總押注 總贏分 遊戲場次 贏場次
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        int TotalBetInteger = 0, TotalWinInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0;

        offset = 0x00;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalOpenPoint = (int)byteuint[0];

        offset = 0x04;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalClearPoint = (int)byteuint[0];

        offset = 0x70;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalCoinIn = (int)byteuint[0];

        offset = 0x14;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalGamePlay = (int)byteuint[0];

        offset = 0x18;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalWinGamePlay = (int)byteuint[0];

        offset = 0x08;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalBetDecimal = (int)ushortbuf[0];

        offset = 0x0E;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalWinDecimal = (int)ushortbuf[0];

        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }

        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
    }

    public int LoadTotalOpenPoint()//讀取總開分
    {
        int TotalOpenPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x00;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalOpenPoint = (int)byteuint[0];


        return TotalOpenPoint;
    }
    public int LoadTotalClearPoint()//讀取總洗分
    {
        int TotalClearPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x04;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalClearPoint = (int)byteuint[0];


        return TotalClearPoint;
    }
    public int LoadTotalCoinIn()//讀取總入鈔
    {
        int TotalCoinIn;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x70;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalCoinIn = (int)byteuint[0];


        return TotalCoinIn;
    }
    public double LoadTotalBet()//讀取總押注
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalBet;
        int TotalBetInteger = 0;
        int TotalBetDecimal = 0;

        offset = 0x08;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalBetDecimal = (int)ushortbuf[0];
        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }
        return TotalBet;
    }
    public double LoadTotalWin()//讀取總贏分
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalWin;
        int TotalWinInteger = 0;
        int TotalWinDecimal = 0;

        offset = 0x0E;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalWinDecimal = (int)ushortbuf[0];
        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
        return TotalWin;
    }
    public int LoadTotalGamePlay()//讀取總場次
    {
        int TotalGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x14;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalGamePlay = (int)byteuint[0];


        return TotalGamePlay;
    }
    public int LoadTotalWinGamePlay()//讀取總贏場次
    {
        int TotalWinGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x18;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalWinGamePlay = (int)byteuint[0];


        return TotalWinGamePlay;
    }
    public float LoadTotalWinScoreRate()//讀取總得分率
    {
        return (float)(LoadTotalWin() / LoadTotalBet());
    }
    public float LoadTotalWinCountRate()//讀取總贏分率
    {
        return (float)((float)LoadTotalWinGamePlay() / (float)LoadTotalGamePlay());
    }
    #endregion

    #region 存讀班紀錄
    //儲存班別紀錄
    public void SaveClassStatistcalData(int ClassOpenPoint, int ClassClearPoint, int ClassCoinIn, double ClassBet, double ClassWin, int ClassGamePlay, int ClassWinGamePlay)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassBetInteger = 0, ClassWinInteger = 0;
        int ClassBetDecimal = 0, ClassWinDecimal = 0;
        if (ClassBet % 1 != 0)
        {
            ClassBetDecimal = (int)SaveDecimalPointHandle(ClassBet);
            ClassBetInteger = (int)ClassBet;
        }
        else
        {
            ClassBetInteger = (int)ClassBet;
        }

        if (ClassWin % 1 != 0)
        {
            ClassWinDecimal = (int)SaveDecimalPointHandle(ClassWin);
            ClassWinInteger = (int)ClassWin;
        }
        else
        {
            ClassWinInteger = (int)ClassWin;
        }

        offset = 0x1C;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassOpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x20;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x74;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassCoinIn;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x30;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassGamePlay;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x34;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassWinGamePlay;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x24;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassBetInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassBetDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x2A;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassWinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassWinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
    }

    public void SaveClassOpenPoint(int ClassOpenPoint)//總開分
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x1C;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassOpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

    }
    public void SaveClassClearPoint(int ClassClearPoint)//總洗分
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x20;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

    }
    public void SaveClassCoinIn(int ClassCoinIn)//總入鈔
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x74;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassCoinIn;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    public void SaveClassBet(double ClassBet)//總押注
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassBetInteger = 0;
        int ClassBetDecimal = 0;
        if (ClassBet % 1 != 0)
        {
            ClassBetDecimal = (int)SaveDecimalPointHandle(ClassBet);
            ClassBetInteger = (int)ClassBet;
        }
        else
        {
            ClassBetInteger = (int)ClassBet;
        }

        offset = 0x24;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassBetInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassBetDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

    }
    public void SaveClassWin(double ClassWin)//總贏分
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassWinInteger = 0;
        int ClassWinDecimal = 0;
        if (ClassWin % 1 != 0)
        {
            ClassWinDecimal = (int)SaveDecimalPointHandle(ClassWin);
            ClassWinInteger = (int)ClassWin;
        }
        else
        {
            ClassWinInteger = (int)ClassWin;
        }
        offset = 0x2A;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassWinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassWinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
    }
    public void SaveClassGamePlay(double ClassGamePlay)//總遊戲場次
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x30;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassGamePlay;

        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    public void SaveClassWinGamePlay(double ClassWinGamePlay)//總贏場次
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x34;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassWinGamePlay;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }

    //讀取班別紀錄
    public void LoadClassStatistcalData(out int ClassOpenPoint, out int ClassClearPoint, out int ClassCoinIn, out double ClassBet, out double ClassWin, out int ClassGamePlay, out int ClassWinGamePlay)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        int ClassBetInteger = 0, ClassWinInteger = 0;
        int ClassBetDecimal = 0, ClassWinDecimal = 0;

        offset = 0x1C;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassOpenPoint = (int)byteuint[0];

        offset = 0x20;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassClearPoint = (int)byteuint[0];

        offset = 0x74;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassCoinIn = (int)byteuint[0];

        offset = 0x30;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassGamePlay = (int)byteuint[0];

        offset = 0x34;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassWinGamePlay = (int)byteuint[0];

        offset = 0x24;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        ClassBetDecimal = (int)ushortbuf[0];

        offset = 0x2A;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        ClassWinDecimal = (int)ushortbuf[0];

        if (ClassBetDecimal == 0)
        {
            ClassBet = ClassBetInteger;
        }
        else
        {
            ClassBet = ClassBetInteger + (ClassBetDecimal * 0.01);
        }

        if (ClassWinDecimal == 0)
        {
            ClassWin = ClassWinInteger;
        }
        else
        {
            ClassWin = ClassWinInteger + (ClassWinDecimal * 0.01);
        }
    }

    public int LoadClassOpenPoint()//讀取總開分
    {
        int ClassOpenPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1C;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassOpenPoint = (int)byteuint[0];


        return ClassOpenPoint;
    }
    public int LoadClassClearPoint()//讀取總洗分
    {
        int ClassClearPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x20;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassClearPoint = (int)byteuint[0];


        return ClassClearPoint;
    }
    public int LoadClassCoinIn()//讀取總入鈔
    {
        int ClassCoinIn;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x74;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassCoinIn = (int)byteuint[0];


        return ClassCoinIn;
    }
    public double LoadClassBet()//讀取總押注
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalBet;
        int ClassBetInteger = 0;
        int ClassBetDecimal = 0;

        offset = 0x24;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        ClassBetDecimal = (int)ushortbuf[0];
        if (ClassBetDecimal == 0)
        {
            TotalBet = ClassBetInteger;
        }
        else
        {
            TotalBet = ClassBetInteger + (ClassBetDecimal * 0.01);
        }
        return TotalBet;
    }
    public double LoadClassWin()//讀取總贏分
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double ClassWin;
        int ClassWinInteger = 0;
        int ClassWinDecimal = 0;

        offset = 0x2A;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        ClassWinDecimal = (int)ushortbuf[0];
        if (ClassWinDecimal == 0)
        {
            ClassWin = ClassWinInteger;
        }
        else
        {
            ClassWin = ClassWinInteger + (ClassWinDecimal * 0.01);
        }
        return ClassWin;
    }
    public int LoadClassGamePlay()//讀取總場次
    {
        int ClassGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x30;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassGamePlay = (int)byteuint[0];


        return ClassGamePlay;
    }
    public int LoadClassWinGamePlay()//讀取總贏場次
    {
        int ClassWinGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x34;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassWinGamePlay = (int)byteuint[0];


        return ClassWinGamePlay;
    }

    public float LoadClassWinScoreRate()//讀取班得分率
    {
        return (float)(LoadClassWin() / LoadClassBet());
    }
    public float LoadClassWinCountRate()//讀取班贏分率
    {
        return (float)((float)LoadClassWinGamePlay() / (float)LoadClassGamePlay());
    }
    #endregion

    #region 存讀備份紀錄
    //儲存備份紀錄
    public void SaveBackupStatistcalData(int TotalOpenPoint, int TotalClearPoint, int TotalCoinIn, double TotalBet, double TotalWin, int TotalGamePlay, int TotalWinGamePlay)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalBetInteger = 0, TotalWinInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0;
        if (TotalBet % 1 != 0)
        {
            TotalBetDecimal = (int)SaveDecimalPointHandle(TotalBet);
            TotalBetInteger = (int)TotalBet;
        }
        else
        {
            TotalBetInteger = (int)TotalBet;
        }

        if (TotalWin % 1 != 0)
        {
            TotalWinDecimal = (int)SaveDecimalPointHandle(TotalWin);
            TotalWinInteger = (int)TotalWin;
        }
        else
        {
            TotalWinInteger = (int)TotalWin;
        }

        offset = 0x1000;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalOpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x1004;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x1022;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalCoinIn;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x1014;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalGamePlay;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x1018;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinGamePlay;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x1008;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalBetInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalBetDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x100E;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalWinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalWinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
    }
    //讀取備份紀錄
    public void LoadBackupStatistcalData(out int TotalOpenPoint, out int TotalClearPoint, out double TotalBet, out double TotalWin, out int TotalGamePlay, out int TotalWinGamePlay)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        int TotalBetInteger = 0, TotalWinInteger = 0;
        int TotalBetDecimal = 0, TotalWinDecimal = 0;

        offset = 0x1000;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalOpenPoint = (int)byteuint[0];

        offset = 0x1004;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalClearPoint = (int)byteuint[0];

        offset = 0x1014;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalGamePlay = (int)byteuint[0];

        offset = 0x1018;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalWinGamePlay = (int)byteuint[0];

        offset = 0x1008;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalBetDecimal = (int)ushortbuf[0];

        offset = 0x100E;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalWinDecimal = (int)ushortbuf[0];

        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }

        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
    }

    public int LoadBackupTotalOpenPoint()//讀取總開分
    {
        int TotalOpenPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1000;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalOpenPoint = (int)byteuint[0];


        return TotalOpenPoint;
    }
    public int LoadBackupTotalClearPoint()//讀取總洗分
    {
        int TotalClearPoint;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1004;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalClearPoint = (int)byteuint[0];


        return TotalClearPoint;
    }
    public int LoadBackupTotalCoinIn()//讀取總入鈔
    {
        int TotalCoinIn;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1022;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalCoinIn = (int)byteuint[0];


        return TotalCoinIn;
    }
    public double LoadBackupTotalBet()//讀取總押注
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalBet;
        int TotalBetInteger = 0;
        int TotalBetDecimal = 0;

        offset = 0x1008;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalBetInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalBetDecimal = (int)ushortbuf[0];
        if (TotalBetDecimal == 0)
        {
            TotalBet = TotalBetInteger;
        }
        else
        {
            TotalBet = TotalBetInteger + (TotalBetDecimal * 0.01);
        }
        return TotalBet;
    }
    public double LoadBackupTotalWin()//讀取總贏分
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalWin;
        int TotalWinInteger = 0;
        int TotalWinDecimal = 0;

        offset = 0x100E;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalWinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalWinDecimal = (int)ushortbuf[0];
        if (TotalWinDecimal == 0)
        {
            TotalWin = TotalWinInteger;
        }
        else
        {
            TotalWin = TotalWinInteger + (TotalWinDecimal * 0.01);
        }
        return TotalWin;
    }
    public int LoadBackupTotalGamePlay()//讀取總場次
    {
        int TotalGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1014;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalGamePlay = (int)byteuint[0];

        return TotalGamePlay;
    }
    public int LoadBackupTotalWinGamePlay()//讀取總贏場次
    {
        int TotalWinGamePlay;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x1018;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalWinGamePlay = (int)byteuint[0];

        return TotalWinGamePlay;
    }

    #endregion

    //儲存密碼
    public void SavePassword(int password)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x38;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)password;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
    }
    //讀取密碼
    public int LoadPassword()
    {
        int password;
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x38;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        password = (int)ushortbuf[0];
        return password;
    }

    //儲存鈔機設定
    public void SaveBanknoteMachineSetting(bool Working)
    {
        UInt32 offset = 0x3A;
        byte[] bytebuf = new byte[1];
        if (Working)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取鈔機設定
    public void LoadBanknoteMachineSetting(out bool Working)
    {
        UInt32 offset = 0x3A;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        if (bytebuf[0] == 0)
        {
            Working = false;
        }
        else
        {
            Working = true;
        }
    }
    //儲存單次開分鍵與單次洗分鍵的數值
    public void SaveOpenPointSetting(int OpenPoint)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x3B;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)OpenPoint;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
    }
    public void SaveClearPointSetting(int ClearPoint)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x3D;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClearPoint;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
    }
    //讀取單次開分鍵與單次洗分鍵的數值
    public int LoadOpenPointSetting()
    {
        int OpenPoint;
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x3B;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        OpenPoint = (int)ushortbuf[0];
        return OpenPoint;
    }
    public int LoadClearPointSetting()
    {
        int ClearPoint;
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x3D;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        ClearPoint = (int)ushortbuf[0];
        return ClearPoint;
    }
    //儲存最大押注倍數
    public void SaveMaxOdd(int Odd)
    {
        UInt32 offset = 0x47;
        byte[] bytebuf = new byte[1];
        bytebuf[0] = (byte)Odd;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取最大押注倍數
    public int LoadMaxOdd()
    {
        int Odd;
        UInt32 offset = 0x47;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        Odd = (int)bytebuf[0];
        return Odd;
    }
    //儲存最大籌碼與最大贏籌碼
    public void SaveMaxCredit(int MaxCredit)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x48;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxCredit;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    public void SaveMaxWin(int MaxWin)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x4C;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxWin;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    //讀取最大籌碼與最大贏籌碼
    public int LoadMaxCredit()
    {
        int MaxCredit;
        UInt32 offset;
        uint[] byteuint;

        offset = 0x48;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        MaxCredit = (int)byteuint[0];
        return MaxCredit;

    }
    public int LoadMaxWin()
    {
        int MaxWin;
        UInt32 offset;
        uint[] byteuint;

        offset = 0x4C;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        MaxWin = (int)byteuint[0];
        return MaxWin;
    }
    //儲存顯示的籌碼比率
    public void SaveDenomOption(bool[] DenomOption)
    {
        UInt32 offset = 0x50;
        byte[] bytebuf = new byte[9];
        for (int i = 0; i < 9; i++)
        {
            if (DenomOption[i]) { bytebuf[i] = 1; }
            else { bytebuf[i] = 0; }

            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);
            offset += 0x01;
        }
    }
    //讀取顯示的籌碼比率
    public void LoadDenomOption(out bool[] DenomOption)
    {
        UInt32 offset = 0x50;
        byte[] bytebuf = new byte[9];
        bool[] tmp = new bool[9];
        for (int i = 0; i < 9; i++)
        {
            AGP_Func.AXGMB_Nvram_Read8(out bytebuf[i], offset, 1);
            offset += 0x01;

            if (bytebuf[i] == 0) { tmp[i] = false; }
            else { tmp[i] = true; }
        }
        DenomOption = tmp;
    }
    //儲存事件紀錄
    public void SaveEventRecored(EventRecoredData[] EventRecored)
    {
        UInt32 offset;
        byte[] Eventbytebuf;
        uint[] Eventbyteuint;

        offset = 0x0100;
        Eventbytebuf = new byte[100];
        for (int i = 0; i < 100; i++)
        {
            Eventbytebuf[i] = (byte)EventRecored[i].EventCode;
            AGP_Func.AXGMB_Nvram_Write8(Eventbytebuf[i], offset + (uint)(0x01 * i), 1);
        }

        offset = 0x0164;
        Eventbyteuint = new uint[100];
        for (int i = 0; i < 100; i++)
        {
            Eventbyteuint[i] = (uint)EventRecored[i].EventData;
            AGP_Func.AXGMB_Nvram_Write32(Eventbyteuint[i], offset + (uint)(0x04 * i), 4);
        }

        offset = 0x02F4;
        byte[] EventTime;
        for (int i = 0; i < 100; i++)
        {
            EventTime = new byte[8];
            EventTime = BitConverter.GetBytes(EventRecored[i].EventTime.Ticks);
            for (int y = 0; y < EventTime.Length; y++)
            {
                AGP_Func.AXGMB_Nvram_Write8(EventTime[y], offset + (uint)(0x01 * y), 1);
            }
            offset += 0x08;
        }
    }
    //讀取事件紀錄
    public void LoadEventRecored(out EventRecoredData[] EventRecored)
    {
        UInt32 offset;
        byte[] Eventbytebuf;
        uint[] Eventbyteuint;
        EventRecoredData[] tmp = new EventRecoredData[100];
        for (int i = 0; i < 100; i++)
        {
            tmp[i] = new EventRecoredData();
        }

        offset = 0x0100;
        Eventbytebuf = new byte[100];
        for (int i = 0; i < 100; i++)
        {
            AGP_Func.AXGMB_Nvram_Read8(out Eventbytebuf[i], offset + (uint)(0x01 * i), 1);
            tmp[i].EventCode = (int)Eventbytebuf[i];
        }

        offset = 0x0164;
        Eventbyteuint = new uint[100];
        for (int i = 0; i < 100; i++)
        {
            AGP_Func.AXGMB_Nvram_Read32(out Eventbyteuint[i], offset + (uint)(0x04 * i), 4);
            tmp[i].EventData = (int)Eventbyteuint[i];
        }

        offset = 0x02F4;
        byte[] EventTime;
        for (int i = 0; i < 100; i++)
        {
            EventTime = new byte[8];
            for (int y = 0; y < EventTime.Length; y++)
            {
                AGP_Func.AXGMB_Nvram_Read8(out EventTime[y], offset + (uint)(0x01 * y), 1);
            }

            tmp[i].EventTime = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));
            offset += 0x08;
        }

        EventRecored = tmp;
    }
    int EventRecoredSaveQuantity = 100;
    public static EventRecoredDataList eventRecoredDataList = new EventRecoredDataList();

    public void saveEventRecoredByEventName(int _EventCode, int _EventData)
    {
        for (int i = EventRecoredSaveQuantity - 1; i >= 0; i--)//0:無資料  1以後的數字(包含1)請自行運用  0:無資料 1:啟動遊戲 2:開分 3:洗分 4:入鈔  0x0100 (1) * 200
        {
            if (eventRecoredDataList._EventRecoredData[i].EventCode != 0)
            {
                if (i < EventRecoredSaveQuantity - 1)
                {
                    eventRecoredDataList._EventRecoredData[i + 1].EventCode = eventRecoredDataList._EventRecoredData[i].EventCode;
                    eventRecoredDataList._EventRecoredData[i + 1].EventData = eventRecoredDataList._EventRecoredData[i].EventData;
                    eventRecoredDataList._EventRecoredData[i + 1].EventTime = eventRecoredDataList._EventRecoredData[i].EventTime;
                }
                else
                {
                    eventRecoredDataList._EventRecoredData[i] = new EventRecoredData();
                }
            }
        }
        eventRecoredDataList._EventRecoredData[0].EventCode = _EventCode;
        eventRecoredDataList._EventRecoredData[0].EventData = _EventData;
        eventRecoredDataList._EventRecoredData[0].EventTime = DateTime.Now;
        SaveEventRecored(eventRecoredDataList._EventRecoredData);
        //saveEventRecoredData(EventRecoredDataFileName, eventRecoredDataList);
        //GetComponent<SramManager>().EventRecoredSave();
    }

 //儲存個別RTP設定與是否使用共通RTP
    public void SaveRTPSetting(int[] RTPValue, bool RTPuse)
    {
        UInt32 offset = 0x624;
        byte[] bytebuf = new byte[11];

        for (int i = 0; i < 10; i++)
        {
            bytebuf[i] = (byte)RTPValue[i];
        }

        if (RTPuse) { bytebuf[10] = 1; }
        else { bytebuf[10] = 0; }

        for (int i = 0; i < 11; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset + (uint)(0x01 * i), 1);
        }
    }

    //讀取個別RTP設定與是否使用共通RTP
    public void LoadRTPSetting(out int[] RTPValue, out bool RTPuse)
    {
        UInt32 offset = 0x624;
        int[] tmp = new int[10];
        byte[] bytebuf = new byte[11];

        for (int i = 0; i < 10; i++)
        {
            AGP_Func.AXGMB_Nvram_Read8(out bytebuf[i], offset + (uint)(0x01 * i), 1);
            tmp[i] = (int)bytebuf[i];
        }
        RTPValue = tmp;

        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[10], offset + (uint)(0x01 * 10), 1);
        if (bytebuf[10] == 0)
        {
            RTPuse = false;
        }
        else
        {
            RTPuse = true;
        }
    }

    //儲存狀態
    public void SaveStatus(int NowStatus)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x59;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)NowStatus;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
    }
    //讀取狀態
    public int LoadStatus()
    {
        int NowStatus;
        UInt32 offset;
        ushort[] ushortbuf;
        offset = 0x59;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        NowStatus = (int)ushortbuf[0];
        return NowStatus;
    }
    //儲存Bouns前牌面 //原16 改為8
    public void SaveBeforeBonusRNG(int[] beforeBonusRNG)
    {
        UInt32 offset;
        offset = 0x4000;
        ushort[] ushortbuf;
        ushortbuf = new ushort[5];
        ushortbuf[0] = (ushort)beforeBonusRNG[0];
        ushortbuf[1] = (ushort)beforeBonusRNG[1];
        ushortbuf[2] = (ushort)beforeBonusRNG[2];
        ushortbuf[3] = (ushort)beforeBonusRNG[3];
        ushortbuf[4] = (ushort)beforeBonusRNG[4];

        for (int i = 0; i < ushortbuf.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
            offset += 0x02;
        }
    }
    //讀起Bonus前牌面  //原16 改為8
    public int[] LoadBeforeBonusRNG()
    {
        int[] beforeBonusRNG = new int[5];
        UInt32 offset;
        offset = 0x4000;
        ushort[] ushortbuf;
        ushortbuf = new ushort[5];

        for (int i = 0; i < ushortbuf.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);
            offset += 0x02;
        }

        beforeBonusRNG[0] = (int)ushortbuf[0];
        beforeBonusRNG[1] = (int)ushortbuf[1];
        beforeBonusRNG[2] = (int)ushortbuf[2];
        beforeBonusRNG[3] = (int)ushortbuf[3];
        beforeBonusRNG[4] = (int)ushortbuf[4];

        return beforeBonusRNG;
    }

    //儲存Sram是否有紀錄
    public void SaveIsSramTrue(bool isSram)
    {
        UInt32 offset = 0x5B;
        byte[] bytebuf = new byte[1];
        if (isSram) bytebuf[0] = (byte)1;
        else bytebuf[0] = (byte)0;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取Sram是否有紀錄
    public bool LoadIsSramTrue()
    {
        UInt32 offset = 0x5B;
        byte[] bytebuf = new byte[1];
        int tmp;
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        tmp = (int)bytebuf[0];
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    //讀取記錄編號 輸入Type(0:一般紀錄 1:最大獎 2:最大獎Second 預設:一般記錄)  輸出 最大的編號
    public void LoadGameHistoryLog(int Type, out int MaxLogNumber)
    {
        UInt32 offset;
        ushort[] ushortbuf;
        int[] NumberArray;
        int HistoryLength = 100;
        MaxLogNumber = 0;
        switch (Type)
        {
            case 0:
                offset = 0x1500;
                HistoryLength = 100;
                break;
            case 1:
                offset = 0x3000;
                HistoryLength = 10;
                break;
            case 2:
                offset = 0x3300;
                HistoryLength = 10;
                break;
            default:
                offset = 0x1500;
                HistoryLength = 100;
                break;
        }

        ushortbuf = new ushort[HistoryLength];
        NumberArray = new int[HistoryLength];
        for (int i = 0; i < HistoryLength; i++)
        {
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);
            offset += 0x02;

            NumberArray[i] = (int)ushortbuf[i];
            if (MaxLogNumber < NumberArray[i])
            {
                MaxLogNumber = NumberArray[i];
            }
        }
    }

    public void ClearHistory()
    {
        HistoryData SaveData = new HistoryData();

        for (int i = 0; i < 100; i++)
        {
            SaveHistory(SaveData);
        }

        UInt32 offset;
        ushort[] ushortbuf;
        ushortbuf = new ushort[1];
        for (int i = 0; i < 100; i++)
        {
            ushortbuf[0] = 0;

            offset = 0x1500;
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

            offset = 0x4100;
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
        }

        for (int i = 1; i <= 10; i++)
        {
            SaveMaxGameHistory(i, SaveData);
            SaveMaxGameHistorySecond(i, SaveData);
        }

        ushortbuf = new ushort[1];
        for (int i = 0; i <= 10; i++)
        {
            ushortbuf[0] = 0;

            offset = 0x3000;
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

            offset = 0x3300;
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
        }

    }

    //儲存歷史紀錄 會清空目前開洗分 輸入時必須已是完整記錄(包含開洗分)
    public void SaveHistory(HistoryData SaveData)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        int CreditInteger = 0, CreditDecimal = 0;
        int WinInteger = 0, WinDecimal = 0;
        byte[] EventTime;
        offset = 0x1500;
        ushortbuf = new ushort[100];
        for (int i = 0; i < ushortbuf.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);
            offset += 0x02;
        }

        for (Count = 0; Count < 100; Count++)
        {
            if (ushortbuf[Count] == 0 && Count == 0)
            {
                goto SaveStart;
            }
            else if (ushortbuf[Count] == 0)
            {
                goto SaveNew;
            }
            else if (Count == 99 && ushortbuf[Count] != 0)
            {
                goto SaveOverWrite;
            }
        }
        return;

    SaveStart:
        #region SaveStart
        offset = 0x1500;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)1;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (SaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(SaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)SaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }

            offset = 0x15C8;
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);

            offset = 0x4100;
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
        }

        offset = 0x17BC;
        bytebuf = new byte[1];
        if (SaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (SaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(SaveData.Credit);
            CreditInteger = (int)SaveData.Credit;
        }
        else
        {
            CreditInteger = (int)SaveData.Credit;
        }

        offset = 0x1820;
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x1A78;
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.Demon;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x1C08;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.Bet;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x1CD0;
        bytebuf = new byte[1];
        bytebuf[0] = (byte)SaveData.Odds;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        WinInteger = 0;
        WinDecimal = 0;
        if (SaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(SaveData.Win);
            WinInteger = (int)SaveData.Win;
        }
        else
        {
            WinInteger = (int)SaveData.Win;
        }
        offset = 0x1D34;
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x1F8C;
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(SaveData.Time.Ticks);
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(EventTime[i], offset, 1);
            offset += 0x01;
        }

        offset = 0x22AC;
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.OpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x243C;
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.ClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x25CC;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.RTP;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x2694;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.SpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x275C;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusSpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x2824;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusIsPlayedCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x28EC;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        goto End;
    #endregion

    SaveNew:
        #region SaveNew
        offset = 0x1500;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)(Count + 1);
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (SaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(SaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)SaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }

            offset = 0x15C8;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);

            offset = 0x4100;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
        }

        offset = 0x17BC;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (SaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (SaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(SaveData.Credit);
            CreditInteger = (int)SaveData.Credit;
        }
        else
        {
            CreditInteger = (int)SaveData.Credit;
        }

        offset = 0x1820;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x1A78;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.Demon;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x1C08;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.Bet;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x1CD0;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)SaveData.Odds;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        WinInteger = 0;
        WinDecimal = 0;
        if (SaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(SaveData.Win);
            WinInteger = (int)SaveData.Win;
        }
        else
        {
            WinInteger = (int)SaveData.Win;
        }
        offset = 0x1D34;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x1F8C;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(SaveData.Time.Ticks);
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(EventTime[i], offset, 1);
            offset += 0x01;
        }

        offset = 0x22AC;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.OpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x243C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.ClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x25CC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.RTP;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x2694;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.SpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x275C;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusSpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x2824;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusIsPlayedCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x28EC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        goto End;
    #endregion

    SaveOverWrite:
        #region SaveOverWrite
        int[] _Number = new int[100];
        for (int i = 0; i < 100; i++)
        {
            _Number[i] = (int)ushortbuf[i];
            if (_Number[i] == 1)
            {
                Count = i;
            }
        }

        offset = 0x1500;
        ushortbuf = new ushort[100];
        // int tmp = _Number[0];
        for (int i = 0; i < 100; i++)
        {
            //if (i < 99)
            //{
            //    _Number[i] = _Number[i + 1];
            //}
            //else
            //{
            //    _Number[i] = tmp;
            //}
            //ushortbuf[i] = (ushort)_Number[i];
            if (i <= Count)
            {
                _Number[i] = 100 - Count + i;
            }
            else
            {
                _Number[i] = i - Count;
            }
            ushortbuf[i] = (ushort)_Number[i];

            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
            offset += 0x02;
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (SaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(SaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)SaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }

            offset = 0x15C8;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);

            offset = 0x4100;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
        }


        offset = 0x17BC;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (SaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (SaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(SaveData.Credit);
            CreditInteger = (int)SaveData.Credit;
        }
        else
        {
            CreditInteger = (int)SaveData.Credit;
        }

        offset = 0x1820;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x1A78;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.Demon;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x1C08;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.Bet;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x1CD0;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)SaveData.Odds;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        WinInteger = 0;
        WinDecimal = 0;
        if (SaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(SaveData.Win);
            WinInteger = (int)SaveData.Win;
        }
        else
        {
            WinInteger = (int)SaveData.Win;
        }
        offset = 0x1D34;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x1F8C;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(SaveData.Time.Ticks);
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(EventTime[i], offset, 1);
            offset += 0x01;
        }

        offset = 0x22AC;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.OpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x243C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)SaveData.ClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x25CC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.RTP;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x2694;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.SpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x275C;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusSpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x2824;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusIsPlayedCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x28EC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)SaveData.BonusCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        goto End;
    #endregion

    End:
        ClearOpenClearPoint();
        ClearCoinInPoint();
        ClearTicketInPoint();
        ClearTicketOutPoint();
    }
    //讀取紀錄 輸入讀取的編號 1~100
    public void LoadHistory(int Number, out HistoryData LoadData)
    {
        UInt32 offset;
        HistoryData tmpData = new HistoryData();
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        offset = 0x1500;
        ushortbuf = new ushort[100];
        for (int i = 0; i < ushortbuf.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);
            offset += 0x02;
        }

        for (Count = 0; Count < 100; Count++)
        {
            if ((int)ushortbuf[Count] == Number)
            {
                tmpData.Number = (int)ushortbuf[Count];
                break;
            }
            if (Count == 99 && (int)ushortbuf[Count] != Number)
            {
                LoadData = tmpData;
                return;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            offset = 0x15C8;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Read8(out bytebuf[i], offset, 1);

            offset = 0x4100;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);

            tmpData.RNG[i] = (int)bytebuf[i] + (int)ushortbuf[i];
        }

        offset = 0x17BC;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        if ((int)bytebuf[0] == 0)
        {
            tmpData.Bonus = false;
        }
        else
        {
            tmpData.Bonus = true;
        }

        offset = 0x1820;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        int CreditInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        int CreditDecimal = (int)ushortbuf[0];

        if (CreditDecimal == 0)
        {
            tmpData.Credit = CreditInteger;
        }
        else
        {
            tmpData.Credit = CreditInteger + ((double)CreditDecimal * 0.01);
        }

        offset = 0x1A78;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        tmpData.Demon = (int)byteuint[0];

        offset = 0x1C08;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.Bet = (int)ushortbuf[0];

        offset = 0x1CD0;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        tmpData.Odds = (int)bytebuf[0];

        offset = 0x1D34;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        int WinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        int WinDecimal = (int)ushortbuf[0];

        if (WinDecimal == 0)
        {
            tmpData.Win = WinInteger;
        }
        else
        {
            tmpData.Win = WinInteger + ((double)WinDecimal * 0.01);
        }

        offset = 0x1F8C;
        offset += (uint)((uint)0x08 * Count);
        byte[] EventTime;
        EventTime = new byte[8];
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read8(out EventTime[i], offset, 1);
            offset += 0x01;
        }
        tmpData.Time = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));

        offset = 0x22AC;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        tmpData.OpenPoint = (int)byteuint[0];

        offset = 0x243C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        tmpData.ClearPoint = (int)byteuint[0];

        offset = 0x25CC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.RTP = (int)ushortbuf[0];

        offset = 0x2694;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.SpecialTime = (int)ushortbuf[0];

        offset = 0x275C;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.BonusSpecialTime = (int)ushortbuf[0];

        offset = 0x2824;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.BonusIsPlayedCount = (int)ushortbuf[0];

        offset = 0x28EC;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.BonusCount = (int)ushortbuf[0];

        LoadData = tmpData;
    }

    //儲存最大獎歷史紀錄 輸入要覆寫哪個編號(如果還有空間則不會覆寫 會直接使用新空間) 1~10
    public void SaveMaxGameHistory(int Number, HistoryData MaxGameSaveData)
    {

        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        int CreditInteger = 0, CreditDecimal = 0;
        int WinInteger = 0, WinDecimal = 0;
        byte[] EventTime;
        offset = 0x3000;
        ushortbuf = new ushort[10];
        for (int i = 0; i < ushortbuf.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);
            offset += 0x02;
        }

        for (Count = 0; Count < 10; Count++)
        {
            if (ushortbuf[Count] == 0 && Count == 0)
            {
                goto SaveStart;
            }
            else if (ushortbuf[Count] == 0)
            {
                goto SaveNew;
            }
            else if (ushortbuf[Count] == Number)
            {
                goto SaveOverWrite;
            }
        }
        return;

    SaveStart:

        offset = 0x3000;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)1;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }

            offset = 0x3014;
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);

            offset = 0x44E8;
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
        }

        offset = 0x3046;
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3050;
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x308C;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x30B4;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x30C8;
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x30D2;
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x310E;
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(EventTime[i], offset, 1);
            offset += 0x01;
        }

        offset = 0x315E;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x3186;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x31AE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31C2;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31D6;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31EA;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31FE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        goto End;

    SaveNew:

        offset = 0x3000;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)(Count + 1);
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }

            offset = 0x3014;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);

            offset = 0x44E8;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
        }

        offset = 0x3046;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3050;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x308C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x30B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x30C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x30D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x310E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(EventTime[i], offset, 1);
            offset += 0x01;
        }

        offset = 0x315E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x3186;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x31AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        goto End;

    SaveOverWrite:

        int[] _Number = new int[10];
        for (int i = 0; i < 10; i++)
        {
            _Number[i] = (int)ushortbuf[i];
            if (_Number[i] == Number)
            {
                Count = i;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }

            offset = 0x3014;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);

            offset = 0x44E8;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
        }

        offset = 0x3046;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3050;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x308C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x30B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x30C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x30D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x310E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(EventTime[i], offset, 1);
            offset += 0x01;
        }

        offset = 0x315E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x3186;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x31AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x31FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        goto End;

    End:
        return;
    }

    //讀取最大獎歷史紀錄 輸入讀取編號 1~100
    public void LoadMaxGameHistory(int Number, out HistoryData MaxGameLoadData)
    {
        UInt32 offset;
        HistoryData tmpData = new HistoryData();
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        offset = 0x3000;
        ushortbuf = new ushort[10];
        for (int i = 0; i < ushortbuf.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);
            offset += 0x02;
        }

        for (Count = 0; Count < 10; Count++)
        {
            if ((int)ushortbuf[Count] == Number)
            {
                tmpData.Number = (int)ushortbuf[Count];
                break;
            }
            if (Count == 9 && (int)ushortbuf[Count] != Number)
            {
                MaxGameLoadData = tmpData;
                return;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            offset = 0x3014;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Read8(out bytebuf[i], offset, 1);

            offset = 0x44E8;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);

            tmpData.RNG[i] = (int)bytebuf[i] + (int)ushortbuf[i];
        }

        offset = 0x3046;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        if ((int)bytebuf[0] == 0)
        {
            tmpData.Bonus = false;
        }
        else
        {
            tmpData.Bonus = true;
        }

        offset = 0x3050;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        int CreditInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        int CreditDecimal = (int)ushortbuf[0];

        if (CreditDecimal == 0)
        {
            tmpData.Credit = CreditInteger;
        }
        else
        {
            tmpData.Credit = CreditInteger + ((double)CreditDecimal * 0.01);
        }

        offset = 0x308C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        tmpData.Demon = (int)byteuint[0];

        offset = 0x30B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.Bet = (int)ushortbuf[0];

        offset = 0x30C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        tmpData.Odds = (int)bytebuf[0];

        offset = 0x30D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        int WinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        int WinDecimal = (int)ushortbuf[0];

        if (WinDecimal == 0)
        {
            tmpData.Win = WinInteger;
        }
        else
        {
            tmpData.Win = WinInteger + ((double)WinDecimal * 0.01);
        }

        offset = 0x310E;
        offset += (uint)((uint)0x08 * Count);
        byte[] EventTime;
        EventTime = new byte[8];
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read8(out EventTime[i], offset, 1);
            offset += 0x01;
        }
        tmpData.Time = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));

        offset = 0x315E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        tmpData.OpenPoint = (int)byteuint[0];

        offset = 0x3186;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        tmpData.ClearPoint = (int)byteuint[0];

        offset = 0x31AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.RTP = (int)ushortbuf[0];

        offset = 0x31C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.SpecialTime = (int)ushortbuf[0];

        offset = 0x31D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.BonusSpecialTime = (int)ushortbuf[0];

        offset = 0x31EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.BonusIsPlayedCount = (int)ushortbuf[0];

        offset = 0x31FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.BonusCount = (int)ushortbuf[0];
        MaxGameLoadData = tmpData;
    }

    //儲存最大獎歷史紀錄Second 輸入要覆寫哪個編號(如果還有空間則不會覆寫 會直接使用新空間) 1~10
    public void SaveMaxGameHistorySecond(int Number, HistoryData MaxGameSaveData)
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        int CreditInteger = 0, CreditDecimal = 0;
        int WinInteger = 0, WinDecimal = 0;
        byte[] EventTime;
        offset = 0x3300;
        ushortbuf = new ushort[10];
        for (int i = 0; i < ushortbuf.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);
            offset += 0x02;
        }

        for (Count = 0; Count < 10; Count++)
        {
            if (ushortbuf[Count] == 0 && Count == 0)
            {
                goto SaveStart;
            }
            else if (ushortbuf[Count] == 0)
            {
                goto SaveNew;
            }
            else if (ushortbuf[Count] == Number)
            {
                goto SaveOverWrite;
            }
        }
        return;

    SaveStart:

        offset = 0x3300;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)1;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }

            offset = 0x3314;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);

            offset = 0x454C;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
        }

        offset = 0x3346;
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3350;
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x338C;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x33B4;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x33C8;
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x33D2;
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x340E;
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(EventTime[i], offset, 1);
            offset += 0x01;
        }

        offset = 0x345E;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x3486;
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x34AE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34C2;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34D6;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34EA;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34FE;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        goto End;

    SaveNew:

        offset = 0x3300;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)(Count + 1);
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }

            offset = 0x3314;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);

            offset = 0x454C;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
        }

        offset = 0x3346;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3350;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x338C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x33B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x33C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x33D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x340E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(EventTime[i], offset, 1);
            offset += 0x01;
        }

        offset = 0x345E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x3486;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x34AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        goto End;

    SaveOverWrite:

        int[] _Number = new int[10];
        for (int i = 0; i < 10; i++)
        {
            _Number[i] = (int)ushortbuf[i];
            if (_Number[i] == Number)
            {
                Count = i;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            if (MaxGameSaveData.RNG[i] - 255 > 0)
            {
                bytebuf[i] = 0xFF;
                ushortbuf[i] = (ushort)(MaxGameSaveData.RNG[i] - 255);
            }
            else
            {
                bytebuf[i] = (byte)MaxGameSaveData.RNG[i];
                ushortbuf[i] = 0x00;
            }

            offset = 0x3314;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Write8(bytebuf[i], offset, 1);

            offset = 0x454C;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[i], offset, 2);
        }

        offset = 0x3346;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        if (MaxGameSaveData.Bonus)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        CreditInteger = 0;
        CreditDecimal = 0;
        if (MaxGameSaveData.Credit % 1 != 0)
        {
            CreditDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Credit);
            CreditInteger = (int)MaxGameSaveData.Credit;
        }
        else
        {
            CreditInteger = (int)MaxGameSaveData.Credit;
        }

        offset = 0x3350;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)CreditInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)CreditDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x338C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.Demon;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x33B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.Bet;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x33C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        bytebuf[0] = (byte)MaxGameSaveData.Odds;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);

        WinInteger = 0;
        WinDecimal = 0;
        if (MaxGameSaveData.Win % 1 != 0)
        {
            WinDecimal = (int)SaveDecimalPointHandle(MaxGameSaveData.Win);
            WinInteger = (int)MaxGameSaveData.Win;
        }
        else
        {
            WinInteger = (int)MaxGameSaveData.Win;
        }
        offset = 0x33D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)WinInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset += 0x04;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)WinDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x340E;
        offset += (uint)((uint)0x08 * Count);
        EventTime = new byte[8];
        EventTime = BitConverter.GetBytes(MaxGameSaveData.Time.Ticks);
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Write8(EventTime[i], offset, 1);
            offset += 0x01;
        }

        offset = 0x345E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.OpenPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x3486;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        byteuint[0] = (uint)MaxGameSaveData.ClearPoint;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x34AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.RTP;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.SpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusSpecialTime;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusIsPlayedCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        offset = 0x34FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)MaxGameSaveData.BonusCount;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);

        goto End;

    End:
        return;
    }

    //讀取最大獎歷史紀錄Second 輸入讀取編號 1~10
    public void LoadMaxGameHistorySecond(int Number, out HistoryData MaxGameLoadData)
    {
        UInt32 offset;
        HistoryData tmpData = new HistoryData();
        uint[] byteuint;
        ushort[] ushortbuf;
        byte[] bytebuf;
        int Count = 0;
        offset = 0x3300;
        ushortbuf = new ushort[10];
        for (int i = 0; i < ushortbuf.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);
            offset += 0x02;
        }

        for (Count = 0; Count < 10; Count++)
        {
            if ((int)ushortbuf[Count] == Number)
            {
                tmpData.Number = (int)ushortbuf[Count];
                break;
            }
            if (Count == 9 && (int)ushortbuf[Count] != Number)
            {
                MaxGameLoadData = tmpData;
                return;
            }
        }

        bytebuf = new byte[5];
        ushortbuf = new ushort[5];
        for (int i = 0; i < 5; i++)
        {
            offset = 0x3314;
            offset += (uint)((uint)0x05 * Count);
            offset += (uint)(0x01 * i);
            AGP_Func.AXGMB_Nvram_Read8(out bytebuf[i], offset, 1);

            offset = 0x454C;
            offset += (uint)((uint)0x0A * Count);
            offset += (uint)(0x02 * i);
            AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[i], offset, 2);

            tmpData.RNG[i] = (int)bytebuf[i] + (int)ushortbuf[i];
        }

        offset = 0x3346;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        if ((int)bytebuf[0] == 0)
        {
            tmpData.Bonus = false;
        }
        else
        {
            tmpData.Bonus = true;
        }

        offset = 0x3350;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        int CreditInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        int CreditDecimal = (int)ushortbuf[0];

        if (CreditDecimal == 0)
        {
            tmpData.Credit = CreditInteger;
        }
        else
        {
            tmpData.Credit = CreditInteger + ((double)CreditDecimal * 0.01);
        }

        offset = 0x338C;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        tmpData.Demon = (int)byteuint[0];

        offset = 0x33B4;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.Bet = (int)ushortbuf[0];

        offset = 0x33C8;
        offset += (uint)((uint)0x01 * Count);
        bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        tmpData.Odds = (int)bytebuf[0];

        offset = 0x33D2;
        offset += (uint)((uint)0x06 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        int WinInteger = (int)byteuint[0];

        offset += 0x04;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        int WinDecimal = (int)ushortbuf[0];

        if (WinDecimal == 0)
        {
            tmpData.Win = WinInteger;
        }
        else
        {
            tmpData.Win = WinInteger + ((double)WinDecimal * 0.01);
        }

        offset = 0x340E;
        offset += (uint)((uint)0x08 * Count);
        byte[] EventTime;
        EventTime = new byte[8];
        for (int i = 0; i < EventTime.Length; i++)
        {
            AGP_Func.AXGMB_Nvram_Read8(out EventTime[i], offset, 1);
            offset += 0x01;
        }
        tmpData.Time = DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));

        offset = 0x345E;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        tmpData.OpenPoint = (int)byteuint[0];

        offset = 0x3486;
        offset += (uint)((uint)0x04 * Count);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        tmpData.ClearPoint = (int)byteuint[0];

        offset = 0x34AE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.RTP = (int)ushortbuf[0];

        offset = 0x34C2;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.SpecialTime = (int)ushortbuf[0];

        offset = 0x34D6;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.BonusSpecialTime = (int)ushortbuf[0];

        offset = 0x34EA;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.BonusIsPlayedCount = (int)ushortbuf[0];

        offset = 0x34FE;
        offset += (uint)((uint)0x02 * Count);
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        tmpData.BonusCount = (int)ushortbuf[0];
        MaxGameLoadData = tmpData;
    }





    //儲存目前開洗分 輸入開分(true) 洗分(false) + 值
    public void SaveOpenClearPoint(bool OpenClear, int Value)
    {
        UInt32 offset;
        uint[] byteuint;
        if (OpenClear)
        {
            offset = 0x60;
            byteuint = new uint[1];
            byteuint[0] = (uint)Value;
            AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        }
        else
        {
            offset = 0x64;
            byteuint = new uint[1];
            byteuint[0] = (uint)Value;
            AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        }
    }
    //讀取目前開洗分 輸入開分(true) 洗分(false) + 值
    public int LoadOpenClearPoint(bool OpenClear)
    {
        int Value;
        UInt32 offset;
        uint[] byteuint;
        if (OpenClear)
        {
            offset = 0x60;
            byteuint = new uint[1];
            AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
            Value = (int)byteuint[0];
        }
        else
        {
            offset = 0x64;
            byteuint = new uint[1];
            AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
            Value = (int)byteuint[0];
        }
        return Value;
    }

    public void SaveCoinInPoint(int Value)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x80;
        byteuint = new uint[1];
        byteuint[0] = (uint)Value;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

    }

    public int LoadCoinInPoint()
    {

        int Value;
        UInt32 offset;
        uint[] byteuint;
        offset = 0x80;

        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        Value = (int)byteuint[0];

        return Value;
    }

    //清空目前開洗分
    public void ClearOpenClearPoint()
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x60;
        byteuint = new uint[1];
        byteuint[0] = 0;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

        offset = 0x64;
        byteuint = new uint[1];
        byteuint[0] = 0;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }

    public void ClearCoinInPoint()
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x80;
        byteuint = new uint[1];
        byteuint[0] = 0;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);

    }
    public void ClearTicketInPoint()
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x4636;
        uint[] ushortbuf;
        byteuint = new uint[1];
        ushortbuf = new uint[1];
        byteuint[0] = 0;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        offset = 0x4640;
        ushortbuf[0] = 0;
        AGP_Func.AXGMB_Nvram_Write32(ushortbuf[0], offset, 4);
    }
    public void ClearTicketOutPoint()
    {
        UInt32 offset;
        uint[] byteuint;
        offset = 0x4642;
        uint[] ushortbuf;
        byteuint = new uint[1];
        ushortbuf = new uint[1];
        byteuint[0] = 0;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        offset = 0x4646;
        ushortbuf[0] = 0;
        AGP_Func.AXGMB_Nvram_Write32(ushortbuf[0], offset, 4);
    }

    public void SaveClearCount(int clearCount)//紀錄剩餘洗分次數
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x40;
        byteuint = new uint[1];
        byteuint[0] = (uint)clearCount;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    public int LoadClearCount()//讀取剩餘洗分次數
    {
        int clearCount;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x40;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        clearCount = (int)byteuint[0];
        return clearCount;
    }

    public void SaveMonthCheckDate(int month)
    {
        UInt32 offset;
        uint[] byteuint;

        offset = 0x68;
        byteuint = new uint[1];
        byteuint[0] = (uint)month;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    public int LoadMonthCheckData()
    {
        int monthData;

        UInt32 offset;
        uint[] byteuint;

        offset = 0x68;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        monthData = (int)byteuint[0];
        return monthData;
    }
    //儲存是否要月報
    public void SaveIsMonthCheck(bool isMonthcheck)
    {
        UInt32 offset = 0x6C;
        byte[] bytebuf = new byte[1];
        if (isMonthcheck) bytebuf[0] = (byte)1;
        else bytebuf[0] = (byte)0;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取是否要月報
    public bool LoadIsMonthCheck()
    {
        UInt32 offset = 0x6C;
        byte[] bytebuf = new byte[1];
        int tmp;
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        tmp = (int)bytebuf[0];
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    //儲存票紙序號
    public void SaveTicketSerial(string serial)
    {
        UInt32 offset = 0x4656;
        if (String.IsNullOrWhiteSpace(serial))
        {
            ushort[] ushortbuf = new ushort[1];
            ushortbuf[0] = 0;
            AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
        }
        else
        {
            string[] saveSerial = serial.Split('-');
            for (int i = 0; i < 4; i++)
            {
                ushort[] ushortbuf = new ushort[1];
                ushortbuf[0] = (ushort)ushort.Parse(saveSerial[i]);
                AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
                offset += 0x02;
            }
        }
    }
    //讀取票紙序號
    public string LoadTicketSerial()
    {
        UInt32 offset = 0x4656;
        ushort[] ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        string loadSerial = null;
        if (ushortbuf[0] == 0)
        {
            return "";
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
                loadSerial += ushortbuf[0].ToString().PadLeft(4, '0');
                if (i < 3) loadSerial += "-";
                offset += 0x02;
            }
            return loadSerial;
        }
    }
    //儲存是否要開啟開分洗分鍵
    public void SaveOpenScoreButtonSet(bool isOpenSet)
    {
        UInt32 offset = 0x8A;
        byte[] bytebuf = new byte[1];
        if (isOpenSet) bytebuf[0] = (byte)1;
        else bytebuf[0] = (byte)0;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取是否要要開啟開分洗分鍵
    public bool LoadOpenScoreButtonSet()
    {
        UInt32 offset = 0x8A;
        byte[] bytebuf = new byte[1];
        int tmp;
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        tmp = (int)bytebuf[0];
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    int[] rtpArray;
    int nowRtp;
    bool rtpAll;
    public void InitializeSetting()//初始化設定
    {

        //儲存備份紀錄
        SaveBackupStatistcalData(LoadBackupTotalOpenPoint() + LoadTotalOpenPoint(), LoadBackupTotalClearPoint() + LoadTotalClearPoint(), LoadBackupTotalCoinIn() + LoadTotalCoinIn(),
            LoadBackupTotalBet() + LoadTotalBet(), LoadBackupTotalWin() + LoadTotalWin(), LoadBackupTotalGamePlay() + LoadTotalGamePlay(), LoadBackupTotalWinGamePlay() + LoadTotalWinGamePlay());
        //初始化總帳戶
        SaveTotalStatistcalData(initData.OpenPointTotal, initData.ClearPointTotal, initData.CashIn, initData.BetCredit, initData.WinScore, initData.GameCount, initData.WinCount);
        //初始化班帳戶
        SaveClassStatistcalData(initData.OpenPointTotal, initData.ClearPointTotal, initData.CashIn, initData.BetCredit, initData.WinScore, initData.GameCount, initData.WinCount);
        //初始化密碼
        SavePassword(initData.AdminPassword);

        //初始化鈔機設定
        SaveBanknoteMachineSetting(initData.UBA_Enable == 1 ? true : false);
        //初始化開分設定
        SaveOpenPointSetting(initData.OpenPointOnce);
        //初始化洗分設定
        SaveClearPointSetting(initData.ClearPointOnce);
        //初始化最大押注設定
        SaveMaxOdd(initData.MaxOdds);

        //初始化最大彩分設定
        SaveMaxCredit(initData.MaxCredit);

        //初始化最大贏分設定
        SaveMaxWin(initData.MaxWinScore);
        ClearHistory();
#if !UNITY_EDITOR
        Mod_Data.maxOdds = LoadMaxOdd();
        Mod_Data.maxCredit = LoadMaxCredit();
        Mod_Data.maxWin = LoadMaxWin();
#endif
        //初始化倍率開啟設定
        SaveDenomOption(initData.DenomSelect);
        LoadDenomOption(out Mod_Data.denomOpenArray);
        //初始化RTP設定
        SaveRTPSetting(initRtp.RTP_Array, initRtp.RTP_All);

        GameObject.Find("BackEndManager").GetComponent<NewSramManager>().LoadRTPSetting(out rtpArray, out rtpAll);

        if (Mod_Data.Denom == 0.01) { nowRtp = rtpArray[8]; }
        else if (Mod_Data.Denom == 0.02) { nowRtp = rtpArray[7]; }
        else if (Mod_Data.Denom == 0.025) { nowRtp = rtpArray[6]; }
        else if (Mod_Data.Denom == 0.05) { nowRtp = rtpArray[5]; }
        else if (Mod_Data.Denom == 0.1) { nowRtp = rtpArray[4]; }
        else if (Mod_Data.Denom == 0.25) { nowRtp = rtpArray[3]; }
        else if (Mod_Data.Denom == 0.5) { nowRtp = rtpArray[2]; }
        else if (Mod_Data.Denom == 1) { nowRtp = rtpArray[1]; }
        else if (Mod_Data.Denom == 2.5) { nowRtp = rtpArray[0]; }

        Mod_Data.RTPsetting = nowRtp;
        GameObject.Find("GameController").GetComponent<Mod_MathController>().ChangeMathFile(nowRtp);
        //初始化是否有存值設定
        Mod_Data.credit = 0;
        Mod_Data.Win = 0;
        Mod_Data.Pay = 0;
        Mod_Data.odds = 1;
        Mod_Data.Denom = 1;
        Mod_Data.Bet = Mod_Data.BetOri;
        //SaveIsSramTrue(false);
        //SaveEventRecored();
    }
    public void DebugSram()
    {
        //Debug.Log("LoadTotalOpenPoint:" + LoadTotalOpenPoint());
        //Debug.Log("LoadTotalClearPoint:" + LoadTotalClearPoint());
        //Debug.Log("LoadTotalCoinIn:" + LoadTotalCoinIn());
        //Debug.Log("LoadTotalBet:" + LoadTotalBet());
        //Debug.Log("LoadTotalWin:" + LoadTotalWin());
        //Debug.Log("LoadTotalGamePlay:" + LoadTotalGamePlay());
        //Debug.Log("LoadTotalWinGamePlay:" + LoadTotalWinGamePlay());
        //Debug.Log("LoadPassword:" + LoadPassword());
        //Debug.Log("LoadOpenPointSetting:" + LoadOpenPointSetting());
        //Debug.Log("LoadClearPointSetting:" + LoadClearPointSetting());
        //Debug.Log("LoadMaxOdd:" + LoadMaxOdd());
        //Debug.Log("LoadMaxCredit:" + LoadMaxCredit());
        //Debug.Log("LoadMaxWin:" + LoadMaxWin());

        //Debug.Log("SaveIsSramTrue:" + LoadIsSramTrue());
    }

    public void InitializeClaseAccount()
    {
        StatisticalData initData = new StatisticalData();
        SaveClassStatistcalData(initData.OpenPointTotal, initData.ClearPointTotal, initData.CashIn, initData.BetCredit, initData.WinScore, initData.GameCount, initData.WinCount);
    }

    //COMPort				int		1	0x68 6D
    //BillName				int		1	0x69 6E
    //TicketEnable			bool	1	0x6A 6F

    //儲存鈔機COMPort
    public void SaveBanknoteMachineCOMPort(string COMPort)
    {
        UInt32 offset = 0x6D;
        byte[] bytebuf = new byte[1];
        switch (COMPort)
        {
            case "COM1":
                bytebuf[0] = 1;
                break;
            case "COM2":
                bytebuf[0] = 2;
                break;
            case "COM3":
                bytebuf[0] = 3;
                break;
            case "COM4":
                bytebuf[0] = 4;
                break;
            case "COM5":
                bytebuf[0] = 5;
                break;
            case "COM6":
                bytebuf[0] = 6;
                break;
            default:
                return;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取鈔機COMPort
    public void LoadBanknoteMachineCOMPort(out string COMPort)
    {
        UInt32 offset = 0x6D;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        switch (bytebuf[0])
        {
            case 1:
                COMPort = "COM1";
                break;
            case 2:
                COMPort = "COM2";
                break;
            case 3:
                COMPort = "COM3";
                break;
            case 4:
                COMPort = "COM4";
                break;
            case 5:
                COMPort = "COM5";
                break;
            case 6:
                COMPort = "COM6";
                break;
            default:
                COMPort = "COM1";
                break;
        }
    }
    //儲存鈔機型號
    public void SaveBanknoteMachineName(string BillName)
    {
        UInt32 offset = 0x6E;
        byte[] bytebuf = new byte[1];
        switch (BillName)
        {
            case "JCM":
                bytebuf[0] = 1;
                break;
            case "MEI":
                bytebuf[0] = 2;
                break;
            case "ICT":
                bytebuf[0] = 3;
                break;
            default:
                return;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取鈔機型號
    public void LoadBanknoteMachineName(out string BillName)
    {
        UInt32 offset = 0x6E;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        switch (bytebuf[0])
        {
            case 1:
                BillName = "JCM";
                break;
            case 2:
                BillName = "MEI";
                break;
            case 3:
                BillName = "ICT";
                break;
            default:
                BillName = "JCM";
                break;
        }
    }
    //儲存吸票開關
    public void SaveBanknoteMachineTicketEnable(bool Enable)
    {
        UInt32 offset = 0x6F;
        byte[] bytebuf = new byte[1];
        if (Enable)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取吸票開關
    public void LoadBanknoteMachineTicketEnable(out bool Enable)
    {
        UInt32 offset = 0x6F;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        if (bytebuf[0] == 1)
        {
            Enable = true;
        }
        else
        {
            Enable = false;
        }
    }

    //newok
    public void SaveTotalTicketIn(double TotalTicketIn)//存總入票
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;

        if (TotalTicketIn % 1 != 0)
        {
            TotalTicketInDecimal = (int)SaveDecimalPointHandle(TotalTicketIn);
            TotalTicketInInteger = (int)TotalTicketIn;
        }
        else
        {
            TotalTicketInInteger = (int)TotalTicketIn;
        }

        offset = 0x4600;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalTicketInInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        // result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4604;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalTicketInDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
        // result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public void SaveTotalTicketOut(double TotalTicketOut)//存總出票
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalTicketOutInteger = 0;
        int TotalTicketOutDecimal = 0;

        if (TotalTicketOut % 1 != 0)
        {
            TotalTicketOutDecimal = (int)SaveDecimalPointHandle(TotalTicketOut);
            TotalTicketOutInteger = (int)TotalTicketOut;
        }
        else
        {
            TotalTicketOutInteger = (int)TotalTicketOut;
        }

        offset = 0x4618;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalTicketOutInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        // result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4622;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalTicketOutDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
        // result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    //newok
    public double LoadTotalTicketIn()//讀取總入票
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalTicketIn;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;

        offset = 0x4600;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalTicketInInteger = (int)byteuint[0];
        //result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        //TotalTicketInInteger = (int)byteuint;

        offset = 0x4604;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalTicketInDecimal = (int)ushortbuf[0];

        //result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        // TotalTicketInDecimal = (int)ushortbuf;
        if (TotalTicketInDecimal == 0)
        {
            TotalTicketIn = TotalTicketInInteger;
        }
        else
        {
            TotalTicketIn = TotalTicketInInteger + (TotalTicketInDecimal * 0.01);
        }
        return TotalTicketIn;
    }
    public double LoadTotalTicketOut()//讀取總出票
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalTickeOut;
        int TotalTickeOutInteger = 0;
        int TotalTickeOutDecimal = 0;

        offset = 0x4618;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalTickeOutInteger = (int)byteuint[0];
        //result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        //TotalTickeOutInteger = (int)byteuint;

        offset = 0x4622;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalTickeOutDecimal = (int)ushortbuf[0];
        // result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        // TotalTickeOutDecimal = (int)ushortbuf;
        if (TotalTickeOutDecimal == 0)
        {
            TotalTickeOut = TotalTickeOutInteger;
        }
        else
        {
            TotalTickeOut = TotalTickeOutInteger + (TotalTickeOutDecimal * 0.01);
        }
        return TotalTickeOut;
    }

    //newok
    public void SaveClassTicketIn(double ClassTicketIn)//存班入票
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassTicketInInteger = 0;
        int ClassTicketInDecimal = 0;

        if (ClassTicketIn % 1 != 0)
        {
            ClassTicketInDecimal = (int)SaveDecimalPointHandle(ClassTicketIn);
            ClassTicketInInteger = (int)ClassTicketIn;
        }
        else
        {
            ClassTicketInInteger = (int)ClassTicketIn;
        }

        offset = 0x4606;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassTicketInInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        //result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4610;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassTicketInDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
        //  result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public void SaveClassTicketOut(double ClassTicketOut)//存班出票
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int ClassTicketOutInteger = 0;
        int ClassTicketOutDecimal = 0;

        if (ClassTicketOut % 1 != 0)
        {
            ClassTicketOutDecimal = (int)SaveDecimalPointHandle(ClassTicketOut);
            ClassTicketOutInteger = (int)ClassTicketOut;
        }
        else
        {
            ClassTicketOutInteger = (int)ClassTicketOut;
        }

        offset = 0x4624;
        byteuint = new uint[1];
        byteuint[0] = (uint)ClassTicketOutInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        // result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4628;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)ClassTicketOutDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
        //  result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }

    //newok
    public double LoadClassTicketIn()//讀取班入票
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalTicketIn;
        int ClassTicketInInteger = 0;
        int ClassTicketInDecimal = 0;

        offset = 0x4606;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassTicketInInteger = (int)byteuint[0];
        //result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        // ClassTicketInInteger = (int)byteuint;

        offset = 0x4610;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        ClassTicketInDecimal = (int)ushortbuf[0];
        //result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        // ClassTicketInDecimal = (int)ushortbuf;
        if (ClassTicketInDecimal == 0)
        {
            TotalTicketIn = ClassTicketInInteger;
        }
        else
        {
            TotalTicketIn = ClassTicketInInteger + (ClassTicketInDecimal * 0.01);
        }
        return TotalTicketIn;
    }
    public double LoadClassTicketOut()//讀取班出票
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TotalTickeOut;
        int ClassTickeOutInteger = 0;
        int ClassTickeOutDecimal = 0;

        offset = 0x4624;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        ClassTickeOutInteger = (int)byteuint[0];
        // result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        //ClassTickeOutInteger = (int)byteuint;

        offset = 0x4628;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        ClassTickeOutDecimal = (int)ushortbuf[0];
        // result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        //  ClassTickeOutDecimal = (int)ushortbuf;
        if (ClassTickeOutDecimal == 0)
        {
            TotalTickeOut = ClassTickeOutInteger;
        }
        else
        {
            TotalTickeOut = ClassTickeOutInteger + (ClassTickeOutDecimal * 0.01);
        }
        return TotalTickeOut;
    }
    //newok
    public void SaveTicketInPoint(double TicketIn)//存入票金額
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;

        if (TicketIn % 1 != 0)
        {
            TotalTicketInDecimal = (int)SaveDecimalPointHandle(TicketIn);
            TotalTicketInInteger = (int)TicketIn;
        }
        else
        {
            TotalTicketInInteger = (int)TicketIn;
        }

        offset = 0x4636;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalTicketInInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        //  result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4640;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalTicketInDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
        // result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public double LoadTicketInPoint()//讀取入票金額
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TicketIn;
        int TotalTicketInInteger = 0;
        int TotalTicketInDecimal = 0;

        offset = 0x4636;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalTicketInInteger = (int)byteuint[0];
        //result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        // TotalTicketInInteger = (int)byteuint;

        offset = 0x4640;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalTicketInDecimal = (int)ushortbuf[0];
        // result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        // TotalTicketInDecimal = (int)ushortbuf;
        if (TotalTicketInDecimal == 0)
        {
            TicketIn = TotalTicketInInteger;
        }
        else
        {
            TicketIn = TotalTicketInInteger + (TotalTicketInDecimal * 0.01);
        }
        return TicketIn;
    }
    public void SaveTicketOutPoint(double TicketOut)//存出票金額
    {
        UInt32 offset;
        ushort[] ushortbuf;
        uint[] byteuint;
        int TotalTicketOutInteger = 0;
        int TotalTicketOutDecimal = 0;

        if (TicketOut % 1 != 0)
        {
            TotalTicketOutDecimal = (int)SaveDecimalPointHandle(TicketOut);
            TotalTicketOutInteger = (int)TicketOut;
        }
        else
        {
            TotalTicketOutInteger = (int)TicketOut;
        }

        offset = 0x4642;
        byteuint = new uint[1];
        byteuint[0] = (uint)TotalTicketOutInteger;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
        // result = NVRAM.qxt_nvram_writedword(offset, byteuint);
        //  result = NVRAM.qxt_nvram_writedword(offset, byteuint);

        offset = 0x4646;
        ushortbuf = new ushort[1];
        ushortbuf[0] = (ushort)TotalTicketOutDecimal;
        AGP_Func.AXGMB_Nvram_Write16(ushortbuf[0], offset, 2);
        //  result = NVRAM.qxt_nvram_writeword(offset, ushortbuf);
    }
    public double LoadTicketOutPoint()//讀取出票金額
    {
        UInt32 offset;
        uint[] byteuint;
        ushort[] ushortbuf;
        double TicketOut;
        int TotalTicketOutInteger = 0;
        int TotalTicketOutDecimal = 0;

        offset = 0x4642;
        // result = NVRAM.qxt_nvram_readdword(offset, out byteuint);
        //  TotalTicketOutInteger = (int)byteuint;
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        TotalTicketOutInteger = (int)byteuint[0];
        offset = 0x4646;
        ushortbuf = new ushort[1];
        AGP_Func.AXGMB_Nvram_Read16(out ushortbuf[0], offset, 2);
        TotalTicketOutDecimal = (int)ushortbuf[0];
        // result = NVRAM.qxt_nvram_readword(offset, out ushortbuf);
        //TotalTicketOutDecimal = (int)ushortbuf;
        if (TotalTicketOutDecimal == 0)
        {
            TicketOut = TotalTicketOutInteger;
        }
        else
        {
            TicketOut = TotalTicketOutInteger + (TotalTicketOutDecimal * 0.01);
        }
        return TicketOut;
    }
    //newok
    //儲存吸鈔還是吸票
    public void SaveBillMachineCashOrTicketEnable(int select)
    {
        UInt32 offset = 0x78;
        byte[] bytebuf = new byte[1];
        switch (select)
        {
            case 0: bytebuf[0] = 0; break;
            case 1: bytebuf[0] = 1; break;
            case 2: bytebuf[0] = 2; break;
            default: bytebuf[0] = 0; break;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
        // result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取吸鈔還是吸票
    public void LoadBillMachineCashOrTicketEnable(out int select)
    {
        UInt32 offset = 0x78;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        //  result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        switch (bytebuf[0])
        {
            case 0: select = 0; break;
            case 1: select = 1; break;
            case 2: select = 2; break;
            default: select = 0; break;
        }
    }
    //儲存手付狀態
    public void SaveHandPayStatus(int HandPay)
    {
        UInt32 offset = 0x79;
        byte[] bytebuf = new byte[1];
        bytebuf[0] = (byte)HandPay;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取手付狀態
    public int LoadHandPayStatus()
    {
        UInt32 offset = 0x79;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        return bytebuf[0];
    }
    //儲存票機開關狀態
    public void SavePrinterEnable(bool Enable)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        if (Enable)
        {
            bytebuf[0] = 1;
        }
        else
        {
            bytebuf[0] = 0;
        }
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
        // result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取票機開關狀態
    public void LoadPrinterEnable(out bool Enable)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        // result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        if (bytebuf[0] == 1)
        {
            Enable = true;
        }
        else
        {
            Enable = false;
        }
    }
    //儲存票機COMPort
    public void SavePrinterCOMPort(int COMPort)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        bytebuf[0] = (byte)COMPort;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
        //  result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取票機COMPort
    public void LoadPrinterCOMPort(out string COMPort)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        // result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        COMPort = "COM" + bytebuf[0];
        if (COMPort == "COM0") COMPort = "COM1";
    }
    //儲存邏輯機門狀態
    public void SaveLogicDoorStatus(bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
        //  result = NVRAM.qxt_nvram_writebyte(offset, bytebuf[0]);
    }
    //讀取邏輯機門狀態
    public void LoadLogicDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        //  result = NVRAM.qxt_nvram_readbyte(offset, out bytebuf[0]);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
    }

    //儲存上機門狀態
    public void SaveMainDoorStatus(bool Error)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取上機門狀態
    public void LoadMainDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7A;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
    }
    //儲存下機門狀態
    public void SaveBellyDoorStatus(bool Error)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取下機門狀態
    public void LoadBellyDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7B;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
    }
    //儲存鈔機門狀態
    public void SaveCashDoorStatus(bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        if (Error) bytebuf[0] = 1;
        else bytebuf[0] = 0;
        AGP_Func.AXGMB_Nvram_Write8(bytebuf[0], offset, 1);
    }
    //讀取鈔機門狀態
    public void LoadCashDoorStatus(out bool Error)
    {
        UInt32 offset = 0x7C;
        byte[] bytebuf = new byte[1];
        AGP_Func.AXGMB_Nvram_Read8(out bytebuf[0], offset, 1);
        if (bytebuf[0] == 1) Error = true;
        else Error = false;
    }
    //儲存過場動畫 魚的倍率
    public void SaveBonusFish(int fishNum, int Muitl)
    {
        UInt32 offset = 0x5000;
        uint[] byteuint;
        offset = offset + (uint)(fishNum * 4);
        byteuint = new uint[1];
        byteuint[0] = (uint)Muitl;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    //讀取過場動畫 魚的倍率
    public int LoadBonusFish(int fishNum)
    {
        UInt32 offset = 0x5000;
        uint[] byteuint;
        offset = offset + (uint)(fishNum * 4);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        return (int)byteuint[0];
    }
    //儲存過場動畫 船的場次
    public void SaveBonusBoat(int BoatNum, int time)
    {
        UInt32 offset = 0x5014;
        uint[] byteuint;
        offset = offset + (uint)(BoatNum * 4);
        byteuint = new uint[1];
        byteuint[0] = (uint)time;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    //讀取過場動畫 船的場次
    public int LoadBonusBoat(int BoatNum)
    {
        UInt32 offset = 0x5014;
        uint[] byteuint;
        offset = offset + (uint)(BoatNum * 4);
        byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        return (int)byteuint[0];
    }
    //儲存過場動畫 已選擇寶箱倍率
    public void SaveSpeicalTime(int SpeicalTime)
    {
        UInt32 offset = 0x5028;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)SpeicalTime;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    //讀取過場動畫 已選擇寶箱倍率
    public int LoadSpeicalTime()
    {
        UInt32 offset = 0x5028;
        uint[] byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        return (int)byteuint[0];
    }
    //儲存過場動畫 已選擇船的場次
    public void SaveBonusCount(int BonusCount)
    {
        UInt32 offset = 0x502C;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)BonusCount;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    //讀取過場動畫 已選擇船的場次
    public int LoadBonusCount()
    {
        UInt32 offset = 0x502C;
        uint[] byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        return (int)byteuint[0];
    }
    //儲存過場動畫 使用者選擇的寶箱路線
    public void SaveUserSelectedSpeicalTime(int SpeicalTime)
    {
        UInt32 offset = 0x5030;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)SpeicalTime;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    //讀取過場動畫 使用者選擇的寶箱路線
    public int LoadUserSelectedSpeicalTime()
    {
        UInt32 offset = 0x5030;
        uint[] byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        return (int)byteuint[0];
    }
    //儲存過場動畫 使用者選擇的船路線
    public void SaveUserSelectedBonusCount(int BonusCount)
    {
        UInt32 offset = 0x5034;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)BonusCount;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    //讀取過場動畫 使用者選擇的船路線
    public int LoadUserSelectedBonusCount()
    {
        UInt32 offset = 0x5034;
        uint[] byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        return (int)byteuint[0];
    }
    //儲存本機場次編號
    public void SaveLocalGameRound(int Round)
    {
        UInt32 offset = 0x4648;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)Round;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    //讀取本機場次編號
    public void LoadLocalGameRound(out int Round)
    {
        UInt32 offset = 0x4648;
        uint[] byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        Round = (int)byteuint[0];
    }
    //儲存本機Bonus場次編號
    public void SaveLocalBonusGameRound(int BonusRound)
    {
        UInt32 offset = 0x4652;
        uint[] byteuint = new uint[1];
        byteuint[0] = (uint)BonusRound;
        AGP_Func.AXGMB_Nvram_Write32(byteuint[0], offset, 4);
    }
    //讀取本Bonus機場次編號
    public void LoadLocalBonusGameRound(out int BonusRound)
    {
        UInt32 offset = 0x4652;
        uint[] byteuint = new uint[1];
        AGP_Func.AXGMB_Nvram_Read32(out byteuint[0], offset, 4);
        BonusRound = (int)byteuint[0];
    }

#endif
    #endregion

    //彩分相關小數點數字處理
    public int SaveDecimalPointHandle(double saveDecimal)
    {
        int returnInt = (int)(Math.Round((decimal)saveDecimal % 1, 2, MidpointRounding.ToEven) * 100);
        return returnInt;
    }

}

public class EventRecoredDataList
{
    public EventRecoredData[] _EventRecoredData = new EventRecoredData[100];
    public EventRecoredData Temp;

    public EventRecoredDataList()
    {
        //Temp = new EventRecoredData(0, 0, DateTime.Now);
        for (int i = 0; i < 100; i++)
        {
            _EventRecoredData[i] = new EventRecoredData(1, 0, DateTime.Now); ;
        }
    }


}

public class EventRecoredData
{
    public int EventCode;       //0:無資料  1以後的數字(包含1)請自行運用  0:無資料 1:啟動遊戲 2:開分 3:洗分 4:入鈔  0x0100 (1) * 200
    public int EventData;
    public DateTime EventTime;

    public EventRecoredData()
    {
        EventCode = 0;
        EventData = 0;
        EventTime = DateTime.Now;
    }

    public EventRecoredData(int _EventCode, int _EventData, DateTime _EventTime)
    {
        EventCode = _EventCode;
        EventData = _EventData;
        EventTime = _EventTime;
    }
}

public class HistoryData
{
    public int[] RNG;
    public bool Bonus;
    public double Credit;
    public int Demon;
    public int Bet;
    public int Odds;
    public double Win;
    public DateTime Time;
    public int OpenPoint;
    public int ClearPoint;
    public int RTP;
    public int SpecialTime;
    public int BonusSpecialTime;
    public int BonusIsPlayedCount;
    public int BonusCount;
    public int HistoryCount;
    public int Number;

    public HistoryData()
    {
        RNG = new int[5] { 0, 10, 0, 0, 0 };
        Bonus = false;
        Credit = 0;
        Demon = 1000;
        Bet = 25;
        Odds = 1;
        Win = 0;
        Time = DateTime.MinValue;
        OpenPoint = 0;
        ClearPoint = 0;
        RTP = 2;
        SpecialTime = 1;
        BonusSpecialTime = 1;
        BonusIsPlayedCount = 0;
        BonusCount = 0;
        HistoryCount = 0;
        Number = 0;
    }
}

public class StatisticalData
{
    //Sram確認             0x55 (1)   0:無資料(return false)  1:有資料(return true)
    public int OpenPointTotal;       //開分            
    public int ClearPointTotal;      //洗分            
    public int CashIn;               //投幣            
    public int CashOut;              //退幣          
    public double TikcetIn;          //入票
    public double TicketOut;         //出票   


    public double BetCredit;            //押注分數         
    public double WinScore;             //贏分             
    public float ScoringRate;           //得分率
    public int GameCount;            //遊戲次數        
    public int WinCount;             //贏分次數        
    public float WinRate;               //贏分率

    public int OpenPointOnce;           //開分鍵          
    public int ClearPointOnce;          //洗分鍵          
    public double RatioScore;           //比倍分數        
    public double RatioWinScore;        //比倍贏分數      

    public float RTP;                   //RTP              
    public int MaxOdds;                 //最大押注倍數    
    public int MaxCredit;            //最大籌碼         
    public int MaxWinScore;          //最大贏籌碼       
    public int RatioTimes;              //比倍次數        
    public double[] DenomArray;          //籌碼比率 
    public bool[] DenomSelect;          //籌碼比率是否顯示 

    public int AdminPassword;        //密碼            

    public int UBA_Enable;              //鈔機               0:關閉    1:開啟

    public StatisticalData()
    {
        OpenPointTotal = 0;
        ClearPointTotal = 0;
        CashIn = 0;
        CashOut = 0;
        TikcetIn = 0;
        TicketOut = 0;
        BetCredit = 0;
        WinScore = 0;
        ScoringRate = 0;
        GameCount = 0;
        WinCount = 0;
        WinRate = 0;

        OpenPointOnce = 100;
        ClearPointOnce = 100;
        RatioScore = 0;
        RatioWinScore = 0;

        RTP = 2;
        MaxOdds = 25;
        MaxCredit = 10000000;
        MaxWinScore = 10000000;
        RatioTimes = 0;
        DenomArray = new double[9] { 2.5, 1, 0.5, 0.25, 0.1, 0.05, 0.025, 0.02, 0.01 };
        DenomSelect = new bool[9] { false, true, true, true, true, false, false, false, false };
        AdminPassword = 2222;

        UBA_Enable = 0;
    }
}

public class RTP_SettingData
{
    public bool RTP_All;        //0x062D    (1)
    public int[] RTP_Array;     //0x0624   (1)*10

    public RTP_SettingData()
    {
        RTP_All = false;
        RTP_Array = new int[10] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };     //0~8:個別RTP   9:全部共用RTP
    }
}