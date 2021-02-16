using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticalBackupData
{
    public int HaveData;                //(1)*11    0x1042        

    public double OpenPointTotal;       //(4)*11    0x1050          
    public double ClearPointTotal;      //(4)*11    0x1080          
    public double CashIn;               //(4)*11    0x10B0          
    public double Cashout;              //(4)*11    0x10E0          

    public double BetCredit;            //(4)*11    0x1110          
    public double WinScore;             //(4)*11    0x1140          
    public double ScoringRate;      
    public double GameCount;            //(4)*11    0x1170          
    public double WinCount;             //(4)*11    0x11A0          
    public double WinRate;

    public double RatioScore;           //(4)*11    0x11D0          
    public double RatioWinScore;        //(4)*11    0x1200          

    public StatisticalBackupData()
    {
        HaveData = 0;

        OpenPointTotal = 0;
        ClearPointTotal = 0;
        CashIn = 0;
        Cashout = 0;

        BetCredit = 0;
        WinScore = 0;
        ScoringRate = 0;
        GameCount = 0;
        WinCount = 0;
        WinRate = 0;

        RatioScore = 0;
        RatioWinScore = 0;
    }

    public StatisticalBackupData(int _HaveData, double _OpenPointTotal, double _ClearPointTotal, double _CashIn, double _Cashout, double _BetCredit, double _WinScore
        , double _GameCount, double _WinCount, double _RatioScore, double _RatioWinScore)
    {
        HaveData = _HaveData;

        OpenPointTotal = _OpenPointTotal;
        ClearPointTotal = _ClearPointTotal;
        CashIn = _CashIn;
        Cashout = _Cashout;

        BetCredit = _BetCredit;
        WinScore = _WinScore;
        ScoringRate = _WinScore / _BetCredit;
        GameCount = _GameCount;
        WinCount = _WinCount;
        WinRate = _WinCount / _GameCount;

        RatioScore = _RatioScore;
        RatioWinScore = _RatioWinScore;
    }
}
