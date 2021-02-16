using System;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using CFPGADrv;
using BYTE = System.Byte;

public class Mod_GameButton : IGameSystem
{
    WaitForSecondsRealtime WaitTime = new WaitForSecondsRealtime(0.03f);

    void Start()
    {
#if GS
        StartCoroutine("GS_DIO");
#endif

#if QXT
        StartCoroutine("QXT_DIO");
#endif

#if AGP
        StartCoroutine("AGP_DIO");
#endif
    }

#if Server
    #region Server
    
    #region QXT_DIO
#if QXT

    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readdword(UInt32 offset);

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_readword(UInt32 offset);

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_writeword(UInt32 offset, UInt32 Value);

    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readword_fb(UInt32 offset);

    volatile System.UInt32 read_long_result1, read_long_result2, read_long_result3, read_long_result_stored1 = 0, read_long_result_stored2 = 0, read_long_result_stored3 = 0;
    public string button_bit;
    public string lightOne, lightTwo;
    public uint lightThree;
    float backEndTime = 0;
    float ClearButtonHoldTime = 0;

    IEnumerator QXT_DIO()
    {
        string tmpLightOne = "0", tmpLightTwo = "0";
        uint tmpLightThree = 0;

        while (true)
        {
    #region 勝圖硬體<-------------------
            read_long_result1 = qxt_dio_readdword(0);  //讀取輸入訊號
                                                       //read_long_result2 = qxt_dio_readdword(4);
                                                       //if (qxt_dio_readword(1) != 255) { //Debug.Log(qxt_dio_readword(1)); }
            if (read_long_result1 != read_long_result_stored3)
            {
                //qxt_dio_readword_fb(3) == 252 qxt_dio_writeword(3, 0);關全燈
                //qxt_dio_readword_fb(3) == 253 qxt_dio_writeword(3, 1);開服務燈
                //qxt_dio_readword_fb(3) == 254 qxt_dio_writeword(3, 2);開故障燈
                //qxt_dio_readword_fb(3) == 255 qxt_dio_writeword(3, 3);開全燈
                button_bit = Convert.ToString(read_long_result1, 2).PadLeft(32, '0');
                if (button_bit.Length >= 32)
                {
                    if (button_bit[26] == '0')
                    {
                        //Debug.Log("服務鈴");//停二
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)//白燈及藍燈亮起時 取消服務鈴
                        {
                            qxt_dio_writeword(3, 0);
                            Mod_Data.serviceLighterror = false;
                        }
                        else
                        {
                            qxt_dio_writeword(3, 1);
                            Mod_Data.serviceLighterror = true;
                        }
                    }
                }
                read_long_result_stored3 = qxt_dio_readdword(0);
            }

            if (Mod_State.InUpdate)
            {
                if (Mod_Data.state == Mod_State.STATE.BaseSpin && backEndTime < 0.5f) backEndTime += 0.03f;

                if (read_long_result1 != read_long_result_stored1)
                {
                    button_bit = Convert.ToString(read_long_result1, 2).PadLeft(32, '0');
                    if (button_bit.Length >= 32)
                    {
                        if (button_bit[0] == '0' && backEndTime > 0.5f)
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("後台");
                                    if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock && !Mod_State.hardSpaceButtonDown)
                                    {
                                        Mod_Data.IOLock = true;
                                        if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                        ////Debug.Log("Get BackEnd_Data.SramAccountData.totalWin: " + BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin));
                                        m_SlotMediatorController.SendMessage("m_state", "OpenLogin");
                                    }
                                    break;
                            }
                        }

                        if (button_bit[6] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("自動"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                                    break;
                                case Mod_State.STATE.BaseScrolling:
                                    //Debug.Log("自動"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                                    break;
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("自動"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                                    break;
                                case Mod_State.STATE.BaseRollScore:
                                    //Debug.Log("自動"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                                    break;
                            }
                        }

                        if (button_bit[7] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("最大押注"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay)
                                    {
                                        Mod_Data.Bet = Mod_Data.BetOri;
                                        Mod_Data.odds = Mod_Data.maxOdds;
                                        m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                        Mod_Data.Win = 0;
                                        m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");

                                    }
                                    break;
                            }
                        }

                        if (button_bit[8] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("押注5");//auto
                                    if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay)
                                    {
                                        Mod_Data.Bet = Mod_Data.BetOri;
                                        if (Mod_Data.odds < Mod_Data.maxOdds)
                                        {
                                            Mod_Data.odds++;
                                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                            else Mod_Data.Win = 0;
                                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                                        }
                                    }
                                    break;
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停5");//停3
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(4);
                                    break;
                            }
                        }


                        if (button_bit[9] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("押注4"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay)
                                    {
                                        Mod_Data.Bet = Mod_Data.BetOri;
                                        if (Mod_Data.odds > 1)
                                        {
                                            Mod_Data.odds--;
                                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                            else Mod_Data.Win = 0;
                                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                                        }
                                    }
                                    break;
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停4");//停4
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(3);
                                    break;
                            }
                        }

                        if (button_bit[10] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停3");//停3
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(2);
                                    break;
                            }
                        }


                        if (button_bit[28] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("出票");//停一
                                    if (Mod_Data.credit > 0 && !Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_Data.MachineError && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !BillAcceptorSettingData.StopCashIn && !BillAcceptorSettingData.CashOrTicketIn && !Mod_Data.handpay)
                                    {
                                        Mod_Data.PrinterTicket = true;
                                        Mod_Data.BlankClick = true;
                                        GameObject.Find("Printer").GetComponent<Mod_Gen2_Status>().PrintTikcet();
                                    }
                                    break;
                            }
                        }

                        if (button_bit[29] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停1");//停1
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(0);
                                    break;
                            }
                        }

                        if (button_bit[30] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停2");//停2
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(1);
                                    break;
                            }
                        }

                        if (button_bit[31] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("開始");
                                    if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay)
                                    {
                                        Mod_State.hardSpaceButtonDown = true;
                                    }
                                    break;
                                default:
                                    //Debug.Log("default 開始");
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) Mod_State.hardSpaceButtonDown = true;
                                    break;
                            }
                        }
                    }
                    read_long_result_stored1 = qxt_dio_readdword(0);
                }
            }
            else
            {
                backEndTime = 0;
                read_long_result_stored1 = 0;
                Mod_State.hardSpaceButtonDown = false;
            }

            if (Mod_Data.MachineError || Mod_Data.memberLcok || Mod_Data.machineIDLock || Mod_Data.handpay)
            {
                if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                {
                    if (Mod_Data.MachineError || Mod_Data.machineIDLock)
                    {
                        lightOne = "000"; lightTwo = "000000"; lightThree = 3;
                    }
                    else
                    {
                        lightOne = "000"; lightTwo = "000000"; lightThree = 1;
                    }
                }
                else
                {
                    if (Mod_Data.MachineError || Mod_Data.machineIDLock)
                    {
                        lightOne = "000"; lightTwo = "000000"; lightThree = 2;
                    }
                    else
                    {
                        lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                    }
                }
            }
            else if (!Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.machineIDLock && !Mod_Data.handpay)
            {
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            if (Mod_Data.credit > 0 && !Mod_Data.PrinterTicket)
                            {
                                lightOne = "001"; lightTwo = "111100"; lightThree = 1;
                            }
                            else if (Mod_Data.credit <= 0 || Mod_Data.PrinterTicket)
                            {
                                lightOne = "000"; lightTwo = "011100"; lightThree = 1;
                            }
                        }
                        else
                        {
                            if (Mod_Data.credit > 0 && !Mod_Data.PrinterTicket)
                            {
                                lightOne = "001"; lightTwo = "111100"; lightThree = 0;
                            }
                            else if (Mod_Data.credit <= 0 || Mod_Data.PrinterTicket)
                            {
                                lightOne = "000"; lightTwo = "011100"; lightThree = 0;
                            }
                        }
                        break;
                    case Mod_State.STATE.BaseScrolling:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "111"; lightTwo = "101110"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "111"; lightTwo = "101110"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "111"; lightTwo = "101110"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "111"; lightTwo = "101110"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.BaseRollScore:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "001"; lightTwo = "100000"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "001"; lightTwo = "100000"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.BonustransIn:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "001"; lightTwo = "000000"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "001"; lightTwo = "000000"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.BonusSpin:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.BonusScrolling:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.BonusEnd:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.BonusRollScore:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "001"; lightTwo = "000000"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "001"; lightTwo = "000000"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.BonusTransOut:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.GetBonusInBonus:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                        }
                        break;
                    case Mod_State.STATE.AfterBonusRollScore:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            lightOne = "001"; lightTwo = "000000"; lightThree = 1;
                        }
                        else
                        {
                            lightOne = "001"; lightTwo = "000000"; lightThree = 0;
                        }
                        break;
                }
            }

            if (tmpLightOne != lightOne || tmpLightTwo != lightTwo || tmpLightThree != lightThree)
            {
                Qxt_Light(lightOne, lightTwo, lightThree);
            }

            tmpLightOne = lightOne; tmpLightTwo = lightTwo; tmpLightThree = lightThree;

    #endregion
            yield return WaitTime;
        }
    }

    public void Qxt_Light(string lightOne, string lightTwo, uint lightThree)
    {
        //return;
        qxt_dio_writeword(0, Convert.ToUInt32(lightOne, 2));
        qxt_dio_writeword(2, Convert.ToUInt32(lightTwo, 2));
        qxt_dio_writeword(3, lightThree);
    }

#endif
    #endregion

    #region GS_DIO
#if GS

    byte DataByte = 1;//賽飛訊號
    byte DataByteServiceLight = 1;//賽飛訊號
    float ClearButtonHoldTime = 0;
    public bool[] ButtonClickLong = new bool[32];//賽菲按鈕
    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS(); //賽菲硬體初始化
    public string gs_light;
    float backEndTime = 0;

    IEnumerator GS_DIO()
    {
        string tmpLight = "0";

        while (true)
        {
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)10, ref DataByteServiceLight);
            if (DataByteServiceLight == 0 && !ButtonClickLong[10])
            {
                //Debug.Log("服務鈴");
                ButtonClickLong[10] = true;
                Mod_Data.serviceLighterror = !Mod_Data.serviceLighterror;
            }
            else if (DataByteServiceLight != 0)
            {
                ButtonClickLong[10] = false;
            }

            if (Mod_State.InUpdate)
            {
                if (Mod_Data.state == Mod_State.STATE.BaseSpin)
                {
                    if (backEndTime <= 0.5f) backEndTime += 0.03f;
                    //CheckClearPoint();
                }

                for (int i = 0; i < 32; i++)
                {
                    Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)i, ref DataByte);
                    if (i != 10)
                    {
                        if (DataByte == 0 && !ButtonClickLong[i])
                        {

                            ButtonClickLong[i] = true;
                            SephirothButton(i);
                        }
                        else if (DataByte != 0)
                        {
                            ButtonClickLong[i] = false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 32; i++)
                {
                    if (i != 10) ButtonClickLong[i] = false;
                }

                backEndTime = 0;
                Mod_State.hardSpaceButtonDown = false;
            }

            /*SephirothOneButtonLed(0, 1);
            SephirothOneButtonLed(2, 1);
            SephirothOneButtonLed(3, 1); //最大壓住
            SephirothOneButtonLed(4, 1); //自動
            SephirothOneButtonLed(5, 0);
            SephirothOneButtonLed(6, 0);
            SephirothOneButtonLed(7, 0);
            SephirothOneButtonLed(8, 1);*/

            if (Mod_Data.MachineError || Mod_Data.memberLcok || Mod_Data.machineIDLock || Mod_Data.handpay)
            {
                if (Mod_Data.serviceLighterror)
                {
                    if (Mod_Data.MachineError || Mod_Data.machineIDLock)
                    {
                        gs_light = "0,0,0,0,0,0,0,0,1,1,1,1";
                    }
                    else if (Mod_Data.memberLcok)
                    {
                        gs_light = "0,0,0,0,0,0,0,0,1,1,0,1";
                    }
                    else
                    {
                        gs_light = "0,0,0,0,0,0,0,0,1,1,0,1";
                    }
                }
                else
                {
                    if (Mod_Data.MachineError || Mod_Data.machineIDLock)
                    {
                        gs_light = "0,0,0,0,0,0,0,0,1,1,1,0";
                    }
                    else if (Mod_Data.memberLcok)
                    {
                        gs_light = "0,0,0,0,0,0,0,0,1,1,0,0";
                    }
                    else
                    {
                        gs_light = "0,0,0,0,0,0,0,0,1,1,0,0";
                    }
                }
            }
            else if (!Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.machineIDLock && !Mod_Data.handpay)
            {
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (Mod_Data.serviceLighterror)
                        {
                            if (Mod_Data.credit > 0 && !Mod_Data.PrinterTicket)
                            {
                                gs_light = "1,1,0,0,0,1,1,1,1,1,0,1";
                                //6.7.1.2.3.4.5.8.up:9.10
                            }
                            else if (Mod_Data.credit <= 0 || Mod_Data.PrinterTicket)
                            {
                                gs_light = "1,1,0,0,0,1,1,0,1,1,0,1";
                            }
                        }
                        else
                        {
                            if (Mod_Data.credit > 0 && !Mod_Data.PrinterTicket)
                            {
                                gs_light = "1,1,0,0,0,1,1,1,1,1,0,0";

                            }
                            else if (Mod_Data.credit <= 0 || Mod_Data.PrinterTicket)
                            {
                                gs_light = "1,1,0,0,0,1,1,0,1,1,0,0";
                            }
                        }
                        break;
                    case Mod_State.STATE.BaseScrolling:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,1,1,1,1,1,1,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,1,1,1,1,1,1,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,1,1,1,1,1,1,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,1,1,1,1,1,1,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.BaseRollScore:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,0,0,0,0,0,1,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.BonustransIn:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,0,0,0,0,0,1,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.BonusSpin:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.BonusScrolling:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.BonusEnd:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.BonusRollScore:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,0,0,0,0,0,1,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.BonusTransOut:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.GetBonusInBonus:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,0,0,0,0,0,0,0,1,0,0";
                        }
                        break;
                    case Mod_State.STATE.AfterBonusRollScore:
                        if (Mod_Data.serviceLighterror)
                        {
                            gs_light = "0,0,0,0,0,0,0,1,0,1,0,1";
                        }
                        else
                        {
                            gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                        }
                        break;
                }
            }

            if (tmpLight != gs_light && !string.IsNullOrWhiteSpace(gs_light)) GS_Light(gs_light);
            tmpLight = gs_light;

            yield return WaitTime;
        }
    }


    public void SephirothButton(int ButtonNumber)
    {
        switch (ButtonNumber)
        {
            case 0:  //押注-
                //Debug.Log("最大押注"); // most
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay)
                        {
                            Mod_Data.Bet = Mod_Data.BetOri;
                            Mod_Data.odds = Mod_Data.maxOdds;
                            m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            Mod_Data.Win = 0;
                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                        }
                        break;
                }
                break;
            case 2: //押注+
                //Debug.Log("自動"); // most
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                        break;
                }
                break;
            case 3: //停1
                //Debug.Log("停1");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(0);
                        break;
                }
                break;
            case 4: //停2
                //Debug.Log("停2");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(1);
                        break;
                }

                break;
            case 5: //停3
                //Debug.Log("停3");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(2);
                        break;
                }
                break;
            case 6: //停4 押注-
                //Debug.Log("停4,+押注");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay)
                        {
                            Mod_Data.Bet = Mod_Data.BetOri;
                            if (Mod_Data.odds > 1)
                            {
                                Mod_Data.odds--;
                                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                else Mod_Data.Win = 0;
                                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                            }
                        }
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(3);
                        break;
                }
                break;
            case 7: //停5 押注+
                //Debug.Log("停5,+押注");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay)
                        {
                            Mod_Data.Bet = Mod_Data.BetOri;
                            if (Mod_Data.odds < Mod_Data.maxOdds)
                            {
                                Mod_Data.odds++;
                                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                else Mod_Data.Win = 0;
                                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                            }
                        }
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(4);
                        break;
                }
                break;
            case 8: //開始 全停 得分
                //Debug.Log("開始");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay)
                        {
                            Mod_State.hardSpaceButtonDown = true;
                        }
                        break;
                    default:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_TimeController.GamePasue && !Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay) Mod_State.hardSpaceButtonDown = true;
                        break;
                }
                break;
            case 9: //側面前方按鈕 開分
                //Debug.Log("出票");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        //Debug.Log("Mod_Data.IOLock: " + Mod_Data.IOLock + " Mod_Data.MachineError: " + Mod_Data.MachineError + " Mod_Data.PrinterTicket: " + Mod_Data.PrinterTicket + " Mod_Data.memberLcok: " + Mod_Data.memberLcok + "  Mod_Data.credit: " + Mod_Data.credit);
                        if (Mod_Data.credit > 0 && !Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_Data.MachineError && !Mod_Data.PrinterTicket && !Mod_Data.printerError && !Mod_Data.handpay && !BillAcceptorSettingData.StopCashIn && !BillAcceptorSettingData.CashOrTicketIn && !Mod_Data.handpay)
                        {
                            Mod_Data.PrinterTicket = true;
                            Mod_Data.BlankClick = true;
                            GameObject.Find("Printer").GetComponent<Mod_Gen2_Status>().PrintTikcet();
                        }
                        break;
                }
                break;
            case 10: //側面後方按鈕 洗分
                /*//Debug.Log("服務燈");
                Mod_Data.serviceLighterror = !Mod_Data.serviceLighterror;
                ServiceLight = !ServiceLight;*/
                break;
            case 12: //統計資料
                //Debug.Log("統計資料");
                if (backEndTime > 0.5f)
                {
                    switch (Mod_Data.state)
                    {
                        case Mod_State.STATE.BaseSpin:
                            if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
                            {
                                Mod_Data.IOLock = true;
                                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                m_SlotMediatorController.SendMessage("m_state", "OpenAccount");
                            }
                            break;
                    }
                }
                break;
            case 13: //設定
                //Debug.Log("設定");
                if (backEndTime > 0.5f)
                {
                    switch (Mod_Data.state)
                    {
                        case Mod_State.STATE.BaseSpin:
                            if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
                            {
                                Mod_Data.IOLock = true;
                                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                m_SlotMediatorController.SendMessage("m_state", "OpenLogin");
                            }
                            break;
                    }
                }
                break;
        }
    }

    public void GS_Light(string SwitchLed)
    {
        string[] Led = SwitchLed.Split(',');
        /*SephirothOneButtonLed(0, 1); //6
        SephirothOneButtonLed(2, 1); //7
        SephirothOneButtonLed(3, 1); //1
        SephirothOneButtonLed(4, 1); //2
        SephirothOneButtonLed(5, 0); //3
        SephirothOneButtonLed(6, 0); //4
        SephirothOneButtonLed(7, 0); //5
        SephirothOneButtonLed(8, 1); //8 */

        for (int i = 0; i < Led.Length; i++)
        {
            switch (i)
            {
                case 0:
                    SephirothOneButtonLed(0, byte.Parse(Led[i]));
                    break;
                case 1:
                    SephirothOneButtonLed(2, byte.Parse(Led[i]));
                    break;
                case 2:
                    SephirothOneButtonLed(3, byte.Parse(Led[i]));
                    break;
                case 3:
                    SephirothOneButtonLed(4, byte.Parse(Led[i]));
                    break;
                case 4:
                    SephirothOneButtonLed(5, byte.Parse(Led[i]));
                    break;
                case 5:
                    SephirothOneButtonLed(6, byte.Parse(Led[i]));
                    break;
                case 6:
                    SephirothOneButtonLed(7, byte.Parse(Led[i]));
                    break;
                case 7:
                    SephirothOneButtonLed(8, byte.Parse(Led[i]));
                    break;
                case 8:
                    SephirothOneButtonLed(9, byte.Parse(Led[i]));
                    break;
                case 9:
                    SephirothOneButtonLed(10, byte.Parse(Led[i]));
                    break;
                case 10:
                    SephirothOneButtonLed(16, byte.Parse(Led[i]));
                    break;
                case 11:
                    SephirothOneButtonLed(17, byte.Parse(Led[i]));
                    break;
            }
        }
    }

    public void SephirothOneButtonLed(int ButtonNumber, byte SwitchLed)
    {
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_SetGPOByIndex(0, (BYTE)ButtonNumber, SwitchLed);
    }

    public void CheckClearPoint()
    {

        if (!Mod_Data.autoPlay && !Mod_Data.IOLock && (Mod_Data.Win <= 0 || Mod_Data.creditErrorLock || Mod_Data.winErrorLock || Mod_Data.monthLock) && !Mod_OpenClearPoint.ClearLessRuning)
        {
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)10, ref DataByte);
            if (DataByte == 0)
            {
                ClearButtonHoldTime += Time.deltaTime;
                if (ClearButtonHoldTime >= 3f)
                {
                    m_SlotMediatorController.SendMessage("m_state", "CheckClearPoint");
                    ClearButtonHoldTime = 0f;
                    Mod_Data.Win = 0;
                    m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                }
            }
            else
            {
                ClearButtonHoldTime = 0;
            }
        }
    }
    
#endif
    #endregion
    
    #endregion
#else
    #region !Server

    #region QXT_DIO
#if QXT

    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readdword(UInt32 offset);

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_readword(UInt32 offset);

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_writeword(UInt32 offset, UInt32 Value);

    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readword_fb(UInt32 offset);

    volatile System.UInt32 read_long_result1, read_long_result2, read_long_result3, read_long_result_stored1 = 0, read_long_result_stored2 = 0, read_long_result_stored3 = 0;
    public string button_bit;
    public string lightOne, lightTwo;
    public uint lightThree;
    float backEndTime = 0;
    float ClearButtonHoldTime = 0;
    bool clearButtonDown = false;
    float qrCodeTime = 0f;
    int qrCount = 0;
    int betLowCreditShowOnce = 0;

    IEnumerator QXT_DIO()
    {
        string tmpLightOne = "0", tmpLightTwo = "0";
        uint tmpLightThree = 0;
        while (true)
        {
    #region 勝圖硬體<-------------------
            read_long_result1 = qxt_dio_readdword(0);  //讀取輸入訊號
            if (Mod_State.InUpdate)
            {
                if (Mod_Data.state == Mod_State.STATE.BaseSpin && backEndTime < 0.5f) backEndTime += 0.03f;

                button_bit = Convert.ToString(read_long_result1, 2).PadLeft(32, '0');
                if (button_bit.Length >= 32)
                {
                    if (backEndTime > 0.5f)
                    {
                        if (button_bit[26] == '0')
                        {
                            ClearButtonHoldTime += Time.unscaledDeltaTime;
                            if (ClearButtonHoldTime >= 3f) CheckClearPoint();
                            clearButtonDown = true;
                        }
                        else
                        {
                            if (qrCount > 0)
                            {
                                qrCodeTime += Time.unscaledDeltaTime;
                                if (qrCodeTime > 3f)
                                {
                                    qrCount = 0;
                                    qrCodeTime = 0f;
                                }
                            }

                            ClearButtonHoldTime = 0f;
                            if (clearButtonDown)
                            {
                                qrCount++;
                                if (qrCount > 5)
                                {
                                    qrCodeTime = 0f;
                                    qrCount = 0;
                                    m_SlotMediatorController.SendMessage("m_state", "OpenQrcodePanal");
                                }
                                clearButtonDown = false;
                            }
                        }
                    }

                    if (button_bit[31] == '0')
                    {
                        switch (Mod_Data.state)
                        {
                            case Mod_State.STATE.BaseSpin:
                                //Debug.Log("開始");
                                if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && betLowCreditShowOnce != 1)
                                {
                                    if (Mod_Data.betLowCreditShowOnce && betLowCreditShowOnce == 0)
                                    {
                                        betLowCreditShowOnce = 1;
                                        Mod_State.hardSpaceButtonDown = false;
                                    }
                                    else
                                    {
                                        Mod_State.hardSpaceButtonDown = true;
                                    }

                                    if (betLowCreditShowOnce == 2) betLowCreditShowOnce = 0;
                                }
                                break;
                            default:
                                //Debug.Log("default 開始");
                                if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_State.hardSpaceButtonDown = true;
                                break;
                        }
                    }
                    else
                    {
                        if (betLowCreditShowOnce == 1) betLowCreditShowOnce = 2;
                    }

                    if (read_long_result1 != read_long_result_stored1)
                    {
                        if (backEndTime > 0.5f)
                        {
                            if (button_bit[0] == '0')
                            {
                                //Debug.Log("後台");
                                switch (Mod_Data.state)
                                {
                                    case Mod_State.STATE.BaseSpin:
                                        if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock && !Mod_State.hardSpaceButtonDown)
                                        {
                                            Mod_Data.IOLock = true;
                                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                            ////Debug.Log("Get BackEnd_Data.SramAccountData.totalWin: " + BackEnd_Data.GetDouble(BackEnd_Data.SramAccountData.totalWin));
                                            m_SlotMediatorController.SendMessage("m_state", "OpenLogin");
                                        }
                                        break;
                                }
                            }

                            if (button_bit[27] == '0')
                            {
                                //Debug.Log("帳務");
                                switch (Mod_Data.state)
                                {
                                    case Mod_State.STATE.BaseSpin:
                                        if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
                                        {
                                            Mod_Data.IOLock = true;
                                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                            m_SlotMediatorController.SendMessage("m_state", "OpenAccount");
                                        }
                                        break;
                                }
                            }
                        }

                        if (button_bit[6] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("自動"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                                    break;
                                case Mod_State.STATE.BaseScrolling:
                                    //Debug.Log("自動"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                                    break;
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("自動"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                                    break;
                                case Mod_State.STATE.BaseRollScore:
                                    //Debug.Log("自動"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                                    break;
                            }
                        }

                        if (button_bit[7] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("最大押注"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                                    {
                                        Mod_Data.Bet = Mod_Data.BetOri;
                                        Mod_Data.odds = Mod_Data.maxOdds;
                                        m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                        Mod_Data.Win = 0;
                                        m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");

                                    }
                                    break;
                            }
                        }

                        if (button_bit[8] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("押注5");//auto
                                    if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                                    {
                                        Mod_Data.Bet = Mod_Data.BetOri;
                                        if (Mod_Data.odds < Mod_Data.maxOdds)
                                        {
                                            Mod_Data.odds++;
                                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                            else Mod_Data.Win = 0;
                                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                                        }
                                    }
                                    break;
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停5");//停3
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(4);
                                    break;
                            }
                        }


                        if (button_bit[9] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("押注4"); // most
                                    if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                                    {
                                        Mod_Data.Bet = Mod_Data.BetOri;
                                        if (Mod_Data.odds > 1)
                                        {
                                            Mod_Data.odds--;
                                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                            else Mod_Data.Win = 0;
                                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                                        }
                                    }
                                    break;
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停4");//停4
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(3);
                                    break;
                            }
                        }

                        if (button_bit[10] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停3");//停3
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(2);
                                    break;
                            }
                        }


                        if (button_bit[28] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseSpin:
                                    //Debug.Log("出票");
                                    // if (Mod_Data.credit > 0 && !Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock && !Mod_Data.MachineError && !Mod_Data.PrinterTicket && !Mod_Data.printerError)
                                    // {
                                    //     Mod_Data.PrinterTicket = true;
                                    //     Mod_Data.BlankClick = true;
                                    //     GameObject.Find("Printer").GetComponent<Mod_Gen2_Status>().PrintTikcet();
                                    // }
                                    //Debug.Log("開分");
                                    if (Mod_Data.Win <= 0 && !Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                                    {
                                        m_SlotMediatorController.SendMessage("m_state", "OpenPoint");
                                    }
                                    break;
                            }
                        }

                        if (button_bit[29] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停1");//停1
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(0);
                                    break;
                            }
                        }

                        if (button_bit[30] == '0')
                        {
                            switch (Mod_Data.state)
                            {
                                case Mod_State.STATE.BaseEnd:
                                    //Debug.Log("停2");//停2
                                    if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(1);
                                    break;
                            }
                        }
                    }
                    read_long_result_stored1 = qxt_dio_readdword(0);
                }
            }
            else
            {
                backEndTime = 0;
                if (betLowCreditShowOnce != 0)
                {
                    betLowCreditShowOnce = 0;
                    Debug.Log("betLowCreditShowOnce 4 : " + betLowCreditShowOnce);
                }
                read_long_result_stored1 = 0;
                Mod_State.hardSpaceButtonDown = false;
            }

            switch (Mod_Data.state)
            {
                case Mod_State.STATE.BaseSpin:
                    if (Mod_Data.credit > 0)
                    {
                        lightOne = "001"; lightTwo = "111100"; lightThree = 0;
                    }
                    else if (Mod_Data.credit <= 0)
                    {
                        lightOne = "000"; lightTwo = "011100"; lightThree = 0;
                    }
                    break;
                case Mod_State.STATE.BaseScrolling:
                    lightOne = "111"; lightTwo = "101110"; lightThree = 0;
                    break;
                case Mod_State.STATE.BaseEnd:
                    lightOne = "111"; lightTwo = "101110"; lightThree = 0;
                    break;
                case Mod_State.STATE.BaseRollScore:
                    lightOne = "001"; lightTwo = "100000"; lightThree = 0;
                    break;
                case Mod_State.STATE.BonustransIn:
                    lightOne = "001"; lightTwo = "000000"; lightThree = 0;
                    break;
                case Mod_State.STATE.BonusSpin:
                    lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                    break;
                case Mod_State.STATE.BonusScrolling:
                    lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                    break;
                case Mod_State.STATE.BonusEnd:
                    lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                    break;
                case Mod_State.STATE.BonusRollScore:
                    lightOne = "001"; lightTwo = "000000"; lightThree = 0;
                    break;
                case Mod_State.STATE.BonusTransOut:
                    lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                    break;
                case Mod_State.STATE.GetBonusInBonus:
                    lightOne = "000"; lightTwo = "000000"; lightThree = 0;
                    break;
                case Mod_State.STATE.AfterBonusRollScore:
                    lightOne = "001"; lightTwo = "000000"; lightThree = 0;
                    break;
            }

            if (tmpLightOne != lightOne || tmpLightTwo != lightTwo || tmpLightThree != lightThree)
            {
                Qxt_Light(lightOne, lightTwo, lightThree);
            }

            tmpLightOne = lightOne; tmpLightTwo = lightTwo; tmpLightThree = lightThree;

    #endregion
            yield return WaitTime;
        }
    }

    public void Qxt_Light(string lightOne, string lightTwo, uint lightThree)
    {
        //return;
        qxt_dio_writeword(0, Convert.ToUInt32(lightOne, 2));
        qxt_dio_writeword(2, Convert.ToUInt32(lightTwo, 2));
        qxt_dio_writeword(3, lightThree);
    }

    public void CheckClearPoint()
    {
        //Debug.Log("CheckClearPoint: " + " Mod_Data.autoPlay: " + Mod_Data.autoPlay + " Mod_Data.IOLock: " + Mod_Data.IOLock + " Mod_Data.Win: " + Mod_Data.Win + " Mod_Data.creditErrorLock: " + Mod_Data.creditErrorLock + "  Mod_Data.winErrorLock: " + Mod_Data.winErrorLock + "  Mod_Data.monthLock: " + Mod_Data.monthLock + " Mod_OpenClearPoint.ClearLessRuning: " + Mod_OpenClearPoint.ClearLessRuning);
        if (!Mod_Data.autoPlay && !Mod_Data.IOLock && (Mod_Data.Win <= 0 || Mod_Data.creditErrorLock || Mod_Data.winErrorLock || Mod_Data.monthLock) && !Mod_OpenClearPoint.ClearLessRuning)
        {
            m_SlotMediatorController.SendMessage("m_state", "CheckClearPoint");
            ClearButtonHoldTime = 0f;
            Mod_Data.Win = 0;
            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
            qrCount = 0;
        }
    }

#endif
    #endregion

    #region GS_DIO
#if GS

    byte DataByte = 1;//賽飛訊號
    byte DataByteServiceLight = 1;//賽飛訊號
    float ClearButtonHoldTime = 0;
    public bool[] ButtonClickLong = new bool[32];//賽菲按鈕
    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS(); //賽菲硬體初始化
    public string gs_light;
    float backEndTime = 0;
    bool betLowCreditShowOnce = false;

    IEnumerator GS_DIO()
    {
        string tmpLight = "0";

        while (true)
        {
            if (Mod_State.InUpdate)
            {
                if (Mod_Data.state == Mod_State.STATE.BaseSpin)
                {
                    if (backEndTime <= 0.5f) backEndTime += 0.03f;
                    if (backEndTime >= 0.5f)
                    {
                        CheckClearPoint();
                    }
                }

                for (int i = 0; i < 32; i++)
                {
                    Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)i, ref DataByte);
                    if (i != 10)
                    {
                        if (DataByte == 0 && !ButtonClickLong[i])
                        {
                            if (i == 8)
                            {
                                if (Mod_Data.betLowCreditShowOnce && !betLowCreditShowOnce)
                                {
                                    Mod_State.hardSpaceButtonDown = false;
                                    ButtonClickLong[i] = true;
                                }

                                if (!ButtonClickLong[8]) SephirothButton(i);
                            }

                            if (i != 8)
                            {
                                ButtonClickLong[i] = true;
                                SephirothButton(i);
                            }
                        }
                        else if (DataByte != 0)
                        {
                            if (i == 8 && Mod_Data.betLowCreditShowOnce && ButtonClickLong[8]) betLowCreditShowOnce = true;
                            ButtonClickLong[i] = false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 32; i++)
                {
                    if (i != 10) ButtonClickLong[i] = false;
                }

                betLowCreditShowOnce = false;
                backEndTime = 0;
                Mod_State.hardSpaceButtonDown = false;
            }

            switch (Mod_Data.state)
            {
                case Mod_State.STATE.BaseSpin:
                    if (Mod_Data.credit > 0)
                    {
                        gs_light = "1,1,0,0,0,1,1,1,1,1,0,0";

                    }
                    else if (Mod_Data.credit <= 0)
                    {
                        gs_light = "1,1,0,0,0,1,1,0,1,1,0,0";
                    }
                    break;
                case Mod_State.STATE.BaseScrolling:
                    gs_light = "0,0,1,1,1,1,1,1,0,1,0,0";
                    break;
                case Mod_State.STATE.BaseEnd:
                    gs_light = "0,1,1,1,1,1,1,1,0,1,0,0";
                    break;
                case Mod_State.STATE.BaseRollScore:
                    gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                    break;
                case Mod_State.STATE.BonustransIn:
                    gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                    break;
                case Mod_State.STATE.BonusSpin:
                    gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                    break;
                case Mod_State.STATE.BonusScrolling:
                    gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                    break;
                case Mod_State.STATE.BonusEnd:
                    gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                    break;
                case Mod_State.STATE.BonusRollScore:
                    gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                    break;
                case Mod_State.STATE.BonusTransOut:
                    gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                    break;
                case Mod_State.STATE.GetBonusInBonus:
                    gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                    break;
                case Mod_State.STATE.AfterBonusRollScore:
                    gs_light = "0,0,0,0,0,0,0,1,0,1,0,0";
                    break;
            }

            if (tmpLight != gs_light && !string.IsNullOrWhiteSpace(gs_light)) GS_Light(gs_light);
            tmpLight = gs_light;

            yield return WaitTime;
        }
    }


    public void SephirothButton(int ButtonNumber)
    {
        switch (ButtonNumber)
        {
            case 0:  //押注-
                     //Debug.Log("最大押注"); // most
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            Mod_Data.Bet = Mod_Data.BetOri;
                            Mod_Data.odds = Mod_Data.maxOdds;
                            m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            Mod_Data.Win = 0;
                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                        }
                        break;
                }
                break;
            case 2: //押注+
                    //Debug.Log("自動"); // most
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                        break;
                }
                break;
            case 3: //停1
                    //Debug.Log("停1");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(0);
                        break;
                }
                break;
            case 4: //停2
                    //Debug.Log("停2");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(1);
                        break;
                }

                break;
            case 5: //停3
                    //Debug.Log("停3");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(2);
                        break;
                }
                break;
            case 6: //停4 押注-
                    //Debug.Log("停4,+押注");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            Mod_Data.Bet = Mod_Data.BetOri;
                            if (Mod_Data.odds > 1)
                            {
                                Mod_Data.odds--;
                                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                else Mod_Data.Win = 0;
                                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                            }
                        }
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(3);
                        break;
                }
                break;
            case 7: //停5 押注+
                    //Debug.Log("停5,+押注");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            Mod_Data.Bet = Mod_Data.BetOri;
                            if (Mod_Data.odds < Mod_Data.maxOdds)
                            {
                                Mod_Data.odds++;
                                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                else Mod_Data.Win = 0;
                                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                            }
                        }
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(4);
                        break;
                }
                break;
            case 8: //開始 全停 得分
                    //Debug.Log("開始");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            Mod_State.hardSpaceButtonDown = true;
                        }
                        break;
                    default:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_State.hardSpaceButtonDown = true;
                        break;
                }
                break;
            case 9: //側面前方按鈕 開分
                    //Debug.Log("出票");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (Mod_Data.Win <= 0 && !Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            m_SlotMediatorController.SendMessage("m_state", "OpenPoint");
                        }
                        break;
                }
                break;
            case 10: //側面後方按鈕 洗分
                /*//Debug.Log("服務燈");
                Mod_Data.serviceLighterror = !Mod_Data.serviceLighterror;
                ServiceLight = !ServiceLight;*/
                break;
            case 12: //統計資料
                     //Debug.Log("統計資料");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
                        {
                            Mod_Data.IOLock = true;
                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            m_SlotMediatorController.SendMessage("m_state", "OpenAccount");
                        }
                        break;
                }
                break;
            case 13: //設定
                     //Debug.Log("設定");
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
                        {
                            Mod_Data.IOLock = true;
                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            m_SlotMediatorController.SendMessage("m_state", "OpenLogin");
                        }
                        break;
                }
                break;
        }
    }

    public void GS_Light(string SwitchLed)
    {
        string[] Led = SwitchLed.Split(',');

        for (int i = 0; i < Led.Length; i++)
        {
            switch (i)
            {
                case 0://最大，左6
                    SephirothOneButtonLed(0, byte.Parse(Led[i]));
                    break;
                case 1://自動，左7
                    SephirothOneButtonLed(2, byte.Parse(Led[i]));
                    break;
                case 2://停1，左1
                    SephirothOneButtonLed(3, byte.Parse(Led[i]));
                    break;
                case 3://停2，左2
                    SephirothOneButtonLed(4, byte.Parse(Led[i]));
                    break;
                case 4://停3，左3
                    SephirothOneButtonLed(5, byte.Parse(Led[i]));
                    break;
                case 5://停4，左4
                    SephirothOneButtonLed(6, byte.Parse(Led[i]));
                    break;
                case 6://停5，左5
                    SephirothOneButtonLed(7, byte.Parse(Led[i]));
                    break;
                case 7://開始，左8
                    SephirothOneButtonLed(8, byte.Parse(Led[i]));
                    break;
                case 8:
                    SephirothOneButtonLed(9, byte.Parse(Led[i]));
                    break;
                case 9:
                    SephirothOneButtonLed(10, byte.Parse(Led[i]));
                    break;
                case 10:
                    SephirothOneButtonLed(16, byte.Parse(Led[i]));
                    break;
                case 11:
                    SephirothOneButtonLed(17, byte.Parse(Led[i]));
                    break;
            }
        }
    }

    public void SephirothOneButtonLed(int ButtonNumber, byte SwitchLed)
    {
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_SetGPOByIndex(0, (BYTE)ButtonNumber, SwitchLed);
    }

    bool clearButtonDown = false;
    int qrCount = 0;
    float qrCodeTime = 0f;
    public void CheckClearPoint()
    {
        if (!Mod_Data.autoPlay && !Mod_Data.IOLock && (Mod_Data.Win <= 0 || Mod_Data.creditErrorLock || Mod_Data.winErrorLock || Mod_Data.monthLock) && !Mod_OpenClearPoint.ClearLessRuning)
        {
            Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)10, ref DataByte);
            if (DataByte == 0)
            {
                ClearButtonHoldTime += Time.deltaTime;
                if (ClearButtonHoldTime >= 3f)
                {
                    m_SlotMediatorController.SendMessage("m_state", "CheckClearPoint");
                    ClearButtonHoldTime = 0f;
                    Mod_Data.Win = 0;
                    m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                    qrCount = 0;
                }
                clearButtonDown = true;
            }
            else
            {
                if (qrCount > 0)
                {
                    qrCodeTime += Time.unscaledDeltaTime;
                    if (qrCodeTime > 3f)
                    {
                        qrCount = 0;
                        qrCodeTime = 0f;
                    }
                }

                ClearButtonHoldTime = 0;

                if (clearButtonDown)
                {
                    qrCount++;
                    if (qrCount > 5)
                    {
                        qrCodeTime = 0f;
                        qrCount = 0;
                        m_SlotMediatorController.SendMessage("m_state", "OpenQrcodePanal");
                    }
                    clearButtonDown = false;
                }
            }
        }
    }

#endif
    #endregion

    #region AGP_DIO
#if AGP

    #region AGP_GPI_Info
    public enum AGP_GPI_Info
    {
        ///<summary>
        ///按鈕:停1
        ///</summary>
        GPI0 = 0,

        ///<summary>
        ///按鈕:停2
        ///</summary>
        GPI1 = 1,

        ///<summary>
        ///按鈕:停3
        ///</summary>
        GPI2 = 2,

        ///<summary>
        ///按鈕:停4、-押
        ///</summary>
        GPI3 = 3,

        ///<summary>
        ///按鈕:停5、+押
        ///</summary>
        GPI4 = 4,

        ///<summary>
        ///按鈕:開始
        ///</summary>
        GPI5 = 5,

        ///<summary>
        ///按鈕:最大押注
        ///</summary>
        GPI6 = 6,

        ///<summary>
        ///按鈕:自動
        ///</summary>
        GPI7 = 7,

        ///<summary>
        ///按鈕:開分
        ///</summary>
        GPI8 = 8,

        ///<summary>
        ///按鈕:洗分
        ///</summary>
        GPI9 = 9,

        ///<summary>
        ///按鈕:統計資料
        ///</summary>
        GPI10 = 10,

        ///<summary>
        ///按鈕:後臺設定
        ///</summary>
        GPI11 = 11
    }
    #endregion

    byte DataByte = 1;//賽飛訊號
    float ClearButtonHoldTime = 0;
    public bool[] ButtonClickLong = new bool[32];//賽菲按鈕

    ///<summary>
    ///<para>存取要寫入GPO的16進位值</para>
    ///<para>使用計算機2進位算出要寫入的GPO，再轉16進位即為要寫入的值</para>
    ///<para>例如:要寫開始按鈕的GPO、開始按鈕GPO為"第5個位置"，二進位為0000 0000 0000 0000 0000 0000 0010 0000 (從右數到左(0~31共32個數字))，</para>
    ///<para>轉16進位為20，unit要輸入為0x00000020</para>
    ///</summary>
    uint AGP_DO_uint = 0;
    float backEndTime = 0;
    bool betLowCreditShowOnce = false;

    IEnumerator AGP_DIO()
    {
        uint tmpAGP_DO = 0;

        while (true)
        {
            #region DI GPI讀取、判斷

            if (Mod_State.InUpdate)
            {
                if (Mod_Data.state == Mod_State.STATE.BaseSpin)
                {
                    if (backEndTime <= 0.5f) backEndTime += 0.03f;
                    if (backEndTime >= 0.5f)
                    {
                        CheckClearPoint();
                    }
                }

                for (int i = 0; i < 32; i++)
                {
                    AGP_Func.AXGMB_DIO_DiReadBit((Byte)i, ref DataByte);

                    if (DataByte == 0 && !ButtonClickLong[i])
                    {
                        if (i == (int)AGP_GPI_Info.GPI5)
                        {
                            if (Mod_Data.betLowCreditShowOnce && !betLowCreditShowOnce)
                            {
                                Mod_State.hardSpaceButtonDown = false;
                                ButtonClickLong[5] = true;
                            }

                            if (!ButtonClickLong[5]) AGPButton((AGP_GPI_Info)i);
                        }

                        if (i != (int)AGP_GPI_Info.GPI5)
                        {
                            ButtonClickLong[i] = true;
                            AGPButton((AGP_GPI_Info)i);
                        }
                    }
                    else if (DataByte != 0)
                    {
                        if (i == (int)AGP_GPI_Info.GPI5 && Mod_Data.betLowCreditShowOnce && ButtonClickLong[5]) betLowCreditShowOnce = true;
                        ButtonClickLong[i] = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 32; i++)
                {
                    ButtonClickLong[i] = false;
                }

                betLowCreditShowOnce = false;
                backEndTime = 0;
                Mod_State.hardSpaceButtonDown = false;
            }

            #endregion

            #region DO GPO讀取寫入

            switch (Mod_Data.state)
            {
                case Mod_State.STATE.BaseSpin:
                    if (Mod_Data.credit > 0)
                    {
                        AGP_DO_uint = 0x000000F8;
                    }
                    else if (Mod_Data.credit <= 0)
                    {
                        AGP_DO_uint = 0x00000058;
                    }
                    break;
                case Mod_State.STATE.BaseScrolling:
                    AGP_DO_uint = 0x000000BF;
                    break;
                case Mod_State.STATE.BaseEnd:
                    AGP_DO_uint = 0x000000BF;
                    break;
                case Mod_State.STATE.BaseRollScore:
                    AGP_DO_uint = 0x000000A0;
                    break;
                case Mod_State.STATE.BonustransIn:
                    AGP_DO_uint = 0x00000020;
                    break;
                case Mod_State.STATE.BonusSpin:
                    AGP_DO_uint = 0x00;
                    break;
                case Mod_State.STATE.BonusScrolling:
                    AGP_DO_uint = 0x00;
                    break;
                case Mod_State.STATE.BonusEnd:
                    AGP_DO_uint = 0x00;
                    break;
                case Mod_State.STATE.BonusRollScore:
                    AGP_DO_uint = 0x00000020;
                    break;
                case Mod_State.STATE.BonusTransOut:
                    AGP_DO_uint = 0x00000020;
                    break;
                case Mod_State.STATE.GetBonusInBonus:
                    AGP_DO_uint = 0x00000020;
                    break;
                case Mod_State.STATE.AfterBonusRollScore:
                    AGP_DO_uint = 0x000000A0;
                    break;
            }

            if (tmpAGP_DO != AGP_DO_uint)
            {
                tmpAGP_DO = AGP_DO_uint;
                AGP_Func.AXGMB_DIO_Write(0xFFFFFFFF, AGP_DO_uint);
            }

            #endregion

            yield return WaitTime;
        }
    }

    #region 傳遞AGP DI(GPI)讀取值到函數做不同處理

    public void AGPButton(AGP_GPI_Info ButtonNumber)
    {
        switch (ButtonNumber)
        {
            case AGP_GPI_Info.GPI0:  //停1
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(0);
                        break;
                }
                break;
            case AGP_GPI_Info.GPI1: //停2
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(1);
                        break;
                }
                break;
            case AGP_GPI_Info.GPI2: //停3
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(2);
                        break;
                }
                break;
            case AGP_GPI_Info.GPI3: //停4、-押
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            Mod_Data.Bet = Mod_Data.BetOri;
                            if (Mod_Data.odds > 1)
                            {
                                Mod_Data.odds--;
                                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                else Mod_Data.Win = 0;
                                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                            }
                        }
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(3);
                        break;
                }
                break;
            case AGP_GPI_Info.GPI4: //停5、+押
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            Mod_Data.Bet = Mod_Data.BetOri;
                            if (Mod_Data.odds < Mod_Data.maxOdds)
                            {
                                Mod_Data.odds++;
                                if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                                else Mod_Data.Win = 0;
                                m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                            }
                        }
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) GameObject.Find("Slots").GetComponent<Slots>().BlankButtonClick(4);
                        break;
                }
                break;
            case AGP_GPI_Info.GPI5: //開始
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            Mod_State.hardSpaceButtonDown = true;
                        }
                        break;
                    default:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_State.hardSpaceButtonDown = true;
                        break;
                }
                break;
            case AGP_GPI_Info.GPI6: //最大押注
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            Mod_Data.Bet = Mod_Data.BetOri;
                            Mod_Data.odds = Mod_Data.maxOdds;
                            m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            Mod_Data.Win = 0;
                            m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                        }
                        break;
                }
                break;
            case AGP_GPI_Info.GPI7: //自動
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (!Mod_Data.IOLock && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock) Mod_Data.autoPlay = !Mod_Data.autoPlay;
                        break;
                }
                break;
            case AGP_GPI_Info.GPI8: //側面前方按鈕 開分
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (Mod_Data.Win <= 0 && !Mod_Data.IOLock && !Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.monthLock && !Mod_Data.billLock)
                        {
                            m_SlotMediatorController.SendMessage("m_state", "OpenPoint");
                        }
                        break;
                }
                break;
            case AGP_GPI_Info.GPI9: //側面後方按鈕 洗分
                break;
            case AGP_GPI_Info.GPI10: //統計資料
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
                        {
                            Mod_Data.IOLock = true;
                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            m_SlotMediatorController.SendMessage("m_state", "OpenAccount");
                        }
                        break;
                }
                break;
            case AGP_GPI_Info.GPI11: //後台設定
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (!Mod_Data.autoPlay && !Mod_Data.winErrorLock && !Mod_Data.creditErrorLock && !Mod_Data.IOLock)
                        {
                            Mod_Data.IOLock = true;
                            if (!Mod_Data.afterBonus) m_SlotMediatorController.SendMessage("m_state", "StopAllGameFrame");
                            m_SlotMediatorController.SendMessage("m_state", "OpenLogin");
                        }
                        break;
                }
                break;
        }
    }

    #endregion

    #region 洗分、QRcodePanel函數

    bool clearButtonDown = false;
    int qrCount = 0;
    float qrCodeTime = 0f;
    public void CheckClearPoint()
    {
        if (!Mod_Data.autoPlay && !Mod_Data.IOLock && (Mod_Data.Win <= 0 || Mod_Data.creditErrorLock || Mod_Data.winErrorLock || Mod_Data.monthLock) && !Mod_OpenClearPoint.ClearLessRuning)
        {
            AGP_Func.AXGMB_DIO_DiReadBit((BYTE)9, ref DataByte);

            if (DataByte == 0)
            {
                ClearButtonHoldTime += Time.deltaTime;
                if (ClearButtonHoldTime >= 3f)
                {
                    m_SlotMediatorController.SendMessage("m_state", "CheckClearPoint");
                    ClearButtonHoldTime = 0f;
                    Mod_Data.Win = 0;
                    m_SlotMediatorController.SendMessage("m_state", "UpdateUIscore");
                    qrCount = 0;
                }
                clearButtonDown = true;
            }
            else
            {
                if (qrCount > 0)
                {
                    qrCodeTime += Time.unscaledDeltaTime;
                    if (qrCodeTime > 3f)
                    {
                        qrCount = 0;
                        qrCodeTime = 0f;
                    }
                }

                ClearButtonHoldTime = 0;

                if (clearButtonDown)
                {
                    qrCount++;
                    if (qrCount > 5)
                    {
                        qrCodeTime = 0f;
                        qrCount = 0;
                        m_SlotMediatorController.SendMessage("m_state", "OpenQrcodePanal");
                    }
                    clearButtonDown = false;
                }
            }
        }
    }

    #endregion

#endif
    #endregion

    #endregion
#endif
}