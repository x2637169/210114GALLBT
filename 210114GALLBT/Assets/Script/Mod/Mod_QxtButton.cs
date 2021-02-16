using System;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;

public class Mod_QxtButton : MonoBehaviour
{
    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readdword(UInt32 offset);

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_readword(UInt32 offset);

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_writeword(UInt32 offset, UInt32 Value);

    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readword_fb(UInt32 offset);

    volatile System.UInt32 read_long_result1, read_long_result2, read_long_result_stored1, read_long_result_stored2;

    public static bool ServiceLight = false;
    WaitForSecondsRealtime WaitTime = new WaitForSecondsRealtime(0.2f);

    void Start()
    {
        StartCoroutine("Button");
    }

    IEnumerator Button()
    {
        while (true)
        {
            #region 勝圖硬體<-------------------
            read_long_result1 = qxt_dio_readdword(0);  //讀取輸入訊號
            read_long_result2 = qxt_dio_readdword(4);
            if (qxt_dio_readword(1) != 255) { print(qxt_dio_readword(1)); }
            if (read_long_result1 != read_long_result_stored1)
            {
                //qxt_dio_readword_fb(3) == 252 qxt_dio_writeword(3, 0);關全燈
                //qxt_dio_readword_fb(3) == 253 qxt_dio_writeword(3, 1);開服務燈
                //qxt_dio_readword_fb(3) == 254 qxt_dio_writeword(3, 2);開故障燈
                //qxt_dio_readword_fb(3) == 255 qxt_dio_writeword(3, 3);開全燈
                switch (read_long_result1)
                {   //按鈕順序由左至右
                    case 0xFFFFFFFD:
                        print("服務鈴");//停二
                                     //print(qxt_dio_readword_fb(3) + "   開燈");
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)//白燈及藍燈亮起時 取消服務鈴
                        {
                            qxt_dio_writeword(3, 0);
                        }
                        else
                        {
                            qxt_dio_writeword(3, 1);
                        }
                        break;
                }
                read_long_result_stored1 = qxt_dio_readdword(0);
            }

            if (Mod_Data.MachineError || Mod_Data.memberLcok || Mod_Data.machineIDLock)
            {
                if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                {
                    qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                    qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    if (Mod_Data.MachineError) qxt_dio_writeword(3, 3);
                    else if (Mod_Data.memberLcok) qxt_dio_writeword(3, 1);
                }
                else if (qxt_dio_readword_fb(3) == 252 || qxt_dio_readword_fb(3) == 254)
                {
                    qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                    qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    if (Mod_Data.MachineError) qxt_dio_writeword(3, 2);
                    else if (Mod_Data.memberLcok) qxt_dio_writeword(3, 0);
                }
            }
            else if (!Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.machineIDLock)
            {
                switch (Mod_Data.state)
                {
                    case Mod_State.STATE.BaseSpin:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            if (Mod_Data.credit > 0 && !Mod_Data.PrinterTicket)
                            {
                                qxt_dio_writeword(0, Convert.ToUInt32("111", 2));
                                qxt_dio_writeword(2, Convert.ToUInt32("110000", 2));
                            }
                            else if (Mod_Data.credit <= 0 || Mod_Data.PrinterTicket)
                            {
                                qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                                qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            }
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            if (Mod_Data.credit > 0 && !Mod_Data.PrinterTicket)
                            {
                                qxt_dio_writeword(0, Convert.ToUInt32("101", 2));
                                qxt_dio_writeword(2, Convert.ToUInt32("110000", 2));
                            }
                            else if (Mod_Data.credit <= 0 || Mod_Data.PrinterTicket)
                            {
                                qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                                qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            }
                        }
                        break;
                    case Mod_State.STATE.BaseScrolling:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("111110", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("111110", 2));
                        }
                        break;
                    case Mod_State.STATE.BaseEnd:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("111110", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("111110", 2));
                        }
                        break;
                    case Mod_State.STATE.BaseRollScore:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        break;
                    case Mod_State.STATE.BonustransIn:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        break;
                    case Mod_State.STATE.BonusSpin:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        break;
                    case Mod_State.STATE.BonusScrolling:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        break;
                    case Mod_State.STATE.BonusEnd:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        break;
                    case Mod_State.STATE.BonusRollScore:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        break;
                    case Mod_State.STATE.BonusTransOut:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        break;
                    case Mod_State.STATE.GetBonusInBonus:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        break;
                    case Mod_State.STATE.AfterBonusRollScore:
                        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                            qxt_dio_writeword(3, 1);
                        }
                        else
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        break;
                }
            }

            if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
            {
                ServiceLight = true;
                Mod_Data.serviceLighterror = true;
            }
            else
            {
                ServiceLight = false;
                Mod_Data.serviceLighterror = false;
            }
            #endregion
            yield return WaitTime;
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        #region 勝圖硬體<-------------------
        read_long_result1 = qxt_dio_readdword(0);  //讀取輸入訊號
        read_long_result2 = qxt_dio_readdword(4);
        if (qxt_dio_readword(1) != 255) { print(qxt_dio_readword(1)); }
        if (read_long_result1 != read_long_result_stored1)
        {
            //qxt_dio_readword_fb(3) == 252 qxt_dio_writeword(3, 0);關全燈
            //qxt_dio_readword_fb(3) == 253 qxt_dio_writeword(3, 1);開服務燈
            //qxt_dio_readword_fb(3) == 254 qxt_dio_writeword(3, 2);開故障燈
            //qxt_dio_readword_fb(3) == 255 qxt_dio_writeword(3, 3);開全燈
            switch (read_long_result1)
            {   //按鈕順序由左至右
                case 0xFFFFFFFD:
                    print("服務鈴");//停二
                    //print(qxt_dio_readword_fb(3) + "   開燈");
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)//白燈及藍燈亮起時 取消服務鈴
                    {
                        qxt_dio_writeword(3, 0);
                    }
                    else
                    {
                        qxt_dio_writeword(3, 1);
                    }
                    break;
            }
            read_long_result_stored1 = qxt_dio_readdword(0);
        }

        if (Mod_Data.MachineError || Mod_Data.memberLcok || Mod_Data.machineIDLock)
        {
            if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
            {
                qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                if (Mod_Data.MachineError) qxt_dio_writeword(3, 3);
                else if (Mod_Data.memberLcok) qxt_dio_writeword(3, 1);
            }
            else if (qxt_dio_readword_fb(3) == 252 || qxt_dio_readword_fb(3) == 254)
            {
                qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                if (Mod_Data.MachineError) qxt_dio_writeword(3, 2);
                else if (Mod_Data.memberLcok) qxt_dio_writeword(3, 0);
            }
        }
        else if (!Mod_Data.MachineError && !Mod_Data.memberLcok && !Mod_Data.machineIDLock)
        {
            switch (Mod_Data.state)
            {
                case Mod_State.STATE.BaseSpin:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        if (Mod_Data.credit > 0 && !Mod_Data.PrinterTicket)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("111", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("110000", 2));
                        }
                        else if (Mod_Data.credit <= 0 || Mod_Data.PrinterTicket)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        if (Mod_Data.credit > 0 && !Mod_Data.PrinterTicket)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("101", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("110000", 2));
                        }
                        else if (Mod_Data.credit <= 0 || Mod_Data.PrinterTicket)
                        {
                            qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                            qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        }
                    }
                    break;
                case Mod_State.STATE.BaseScrolling:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("111110", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("111110", 2));
                    }
                    break;
                case Mod_State.STATE.BaseEnd:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("111110", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("111110", 2));
                    }
                    break;
                case Mod_State.STATE.BaseRollScore:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    }
                    break;
                case Mod_State.STATE.BonustransIn:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    }
                    break;
                case Mod_State.STATE.BonusSpin:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    }
                    break;
                case Mod_State.STATE.BonusScrolling:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    }
                    break;
                case Mod_State.STATE.BonusEnd:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    }
                    break;
                case Mod_State.STATE.BonusRollScore:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    }
                    break;
                case Mod_State.STATE.BonusTransOut:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    }
                    break;
                case Mod_State.STATE.GetBonusInBonus:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("010", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("000", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    }
                    break;
                case Mod_State.STATE.AfterBonusRollScore:
                    if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("011", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                        qxt_dio_writeword(3, 1);
                    }
                    else
                    {
                        qxt_dio_writeword(0, Convert.ToUInt32("001", 2));
                        qxt_dio_writeword(2, Convert.ToUInt32("000000", 2));
                    }
                    break;
            }
        }

        if (qxt_dio_readword_fb(3) == 253 || qxt_dio_readword_fb(3) == 255)
        {
            ServiceLight = true;
            Mod_Data.serviceLighterror = true;
        }
        else
        {
            ServiceLight = false;
            Mod_Data.serviceLighterror = false;
        }
        #endregion
    }*/
}
