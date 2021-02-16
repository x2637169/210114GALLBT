using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CFPGADrv;
using DWORD = System.UInt32;
using Quixant.LibQxt.LPComponent;
using System.Runtime.InteropServices;

public class MonthCheck : MonoBehaviour
{
    #region 不同IPC變數

    #region GS
#if GS
 
    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS();

#endif
    #endregion

    #region QXT
#if QXT

    [DllImport("libqxt")]
    public static extern bool qxt_rtc_getdate(out sEventRTC inevent, ClockType clockType);
    public enum ClockType : byte
    {
        RealtimeClock = 0x01,
        AlarmClock = 0x04,
        SystemClock = 0x02
    }
    bool result;
    sEventRTC doorEvent;

#endif
    #endregion

    #region AGP
#if AGP

    AGP_Func.RTC_TIME rtc_Time = new AGP_Func.RTC_TIME();

#endif
    #endregion

    #endregion

    DWORD[] datetime = new uint[6];
    int sramMonth, time, sramHour, sramMin;
    NewSramManager newSramManager;
    //datatime[0]: Year
    //datetime[1]: Month
    //datetime[2]: Date
    //datetime[3]: Hour
    //datetime[4]: Minute
    //datetime[5]: Second
    // Start is called before the first frame update
    void Start()
    {
        newSramManager = GetComponent<NewSramManager>();

        #region 獲得IPC時間

        #region GS
#if GS
        
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.PIC_ReadRTC(datetime);

#endif
        #endregion

        #region QXT
#if QXT
        
        Mod_Data.Qxt_Device_Init();
        result = qxt_rtc_getdate(out doorEvent, ClockType.RealtimeClock);
        datetime[0] = uint.Parse("20" + doorEvent.year.ToString());
        datetime[1] = doorEvent.month;
        datetime[2] = doorEvent.day;
        datetime[3] = doorEvent.hour;
        datetime[4] = doorEvent.min;
        datetime[5] = doorEvent.sec;
        Debug.Log(datetime[0] + " : " + datetime[1] + " : " + datetime[2] + " : " + datetime[3] + " : " + datetime[4] + " : " + datetime[5]);

#endif
        #endregion

        #region AGP
#if AGP

        AGP_Func.AXGMB_Intr_GetRtc(ref rtc_Time);
        datetime[0] = rtc_Time.year;
        datetime[1] = rtc_Time.month;
        datetime[2] = rtc_Time.day;
        datetime[3] = rtc_Time.hour;
        datetime[4] = rtc_Time.minute;
        datetime[5] = rtc_Time.second;

#endif
        #endregion

        #endregion

        sramMonth = newSramManager.LoadMonthCheckData();
        Mod_Data.hasMonthCheck = newSramManager.LoadIsMonthCheck();
        if (Mod_Data.hasMonthCheck)
        {
            if (datetime[1] == 12 && (sramMonth == 12 || sramMonth == 1))
            {
                Mod_Data.monthLock = false;
            }
            else if (sramMonth - datetime[1] >= 0 && sramMonth - datetime[1] <= 1)
            {
                Mod_Data.monthLock = false;
            }
            else
            {
                Mod_Data.monthLock = true;
            }
        }

        sramHour = int.Parse(datetime[3].ToString());
        sramMin = int.Parse(datetime[4].ToString());
        time = (23 - sramHour) * 3600 + (60 - sramMin) * 60;

        //Debug.Log("sramHour:" + sramHour + "sramMin:" + sramMin + "time:" + time);
        StartCoroutine(MonthCheckClock(time));
    }


    IEnumerator MonthCheckClock(int time)
    {
        yield return new WaitUntil(() => Mod_Data.hasMonthCheck == true);
        yield return new WaitForSeconds(time);

        #region 獲得IPC時間

        #region GS
#if GS
        
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.PIC_ReadRTC(datetime);

#endif
        #endregion

        #region QXT
#if QXT
        
        Mod_Data.Qxt_Device_Init();
        result = qxt_rtc_getdate(out doorEvent, ClockType.RealtimeClock);
        datetime[0] = uint.Parse("20" + doorEvent.year.ToString());
        datetime[1] = doorEvent.month;
        datetime[2] = doorEvent.day;
        datetime[3] = doorEvent.hour;
        datetime[4] = doorEvent.min;
        datetime[5] = doorEvent.sec;
        Debug.Log(datetime[0] + " : " + datetime[1] + " : " + datetime[2] + " : " + datetime[3] + " : " + datetime[4] + " : " + datetime[5]);

#endif
        #endregion

        #region AGP
#if AGP

        AGP_Func.AXGMB_Intr_GetRtc(ref rtc_Time);
        datetime[0] = rtc_Time.year;
        datetime[1] = rtc_Time.month;
        datetime[2] = rtc_Time.day;
        datetime[3] = rtc_Time.hour;
        datetime[4] = rtc_Time.minute;
        datetime[5] = rtc_Time.second;

#endif
        #endregion

        #endregion

        sramMonth = newSramManager.LoadMonthCheckData();
        Mod_Data.hasMonthCheck = newSramManager.LoadIsMonthCheck();
        if (Mod_Data.hasMonthCheck)
        {
            if (datetime[1] == 12 && (sramMonth == 12 || sramMonth == 1))
            {
                Mod_Data.monthLock = false;
            }
            else if (sramMonth - datetime[1] >= 0 && sramMonth - datetime[1] <= 1)
            {
                Mod_Data.monthLock = false;
            }
            else
            {
                Mod_Data.monthLock = true;
            }
        }
        StartCoroutine(MonthCheckClock(86400));
    }
}
