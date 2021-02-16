using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using CFPGADrv;
using DWORD = System.UInt32;
using System.Text.RegularExpressions;
using Quixant.LibQxt.LPComponent;
using System.Runtime.InteropServices;

public class Engine_Password : MonoBehaviour
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
    [SerializeField] GameObject systemPanal, passwordPanel;
    //datatime[0]: Year
    //datetime[1]: Month
    //datetime[2]: Date
    //datetime[3]: Hour
    //datetime[4]: Minute
    //datetime[5]: Second
    public InputField passwordInput;
    public Text placeholder;

    public string[] Time_text;
    string password;

    void OnEnable()
    {
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

        Time_text = new string[2];
        Time_text[0] = datetime[1].ToString();
        Time_text[1] = datetime[2].ToString();

        SetPassword();
    }

    public void PasswordButton(string button)
    {
        switch (button)
        {
            case "Confirm":
                if (passwordInput.text != "")
                    if (passwordInput.text == password)
                    {
                        passwordInput.text = "";
                        //DO Something
                        systemPanal.SetActive(true);
                        placeholder.text = "";
                        passwordPanel.SetActive(false);
                        //Debug.Log("Login");
                        break;
                    }

                SetText("PasswordWrong");

                break;
            case "Cancel":
                SetText("Clean");
                this.gameObject.SetActive(false);
                break;
            case "Clear":
                SetText("Clean");
                break;
            case "Back":
                if (passwordInput.text != null && passwordInput.text != "")
                {
                    passwordInput.text = passwordInput.text.Remove(passwordInput.text.Length - 1);
                    if (passwordInput.text == "")
                        SetText("Clean");
                }
                else
                {
                    SetText("Clean");
                }
                break;
            default:
                passwordInput.text += button;
                break;
        }
    }

    public void SetText(string text)
    {
        switch (text)
        {
            case "Clean":
                passwordInput.text = "";
                placeholder.text = "輸入密碼";
                placeholder.color = new Color32(255, 255, 255, 128);
                break;
            case "PasswordWrong":
                passwordInput.text = "";
                placeholder.text = "密碼錯誤";
                placeholder.color = new Color32(255, 0, 0, 255);
                break;
            default:
                //Debug.Log("Case不存在");
                break;
        }
    }

    public void SetPassword()
    {
        float num1 = (int.Parse(Time_text[0]) + 20) + (int.Parse(Time_text[1]) + 20);
        float num2 = (int.Parse(Time_text[0]) + 20) * (int.Parse(Time_text[1]) + 20);
        string tmpPassword = (num1 + num2).ToString();
        string tmpPassword2 = (num1 / num2).ToString();
        string[] split;
        tmpPassword = System.Convert.ToString(int.Parse(tmpPassword), 8);
        password = tmpPassword;

        Regex regex = new Regex(@"[.]");
        if (regex.IsMatch(tmpPassword2))
        {
            split = Regex.Split(tmpPassword2, @"[.]");
            tmpPassword2 = split[1];
        }
        tmpPassword2 = int.Parse(tmpPassword2).ToString();

        if (password.Length < 8)
        {
            int tmpNum = 0;
            int tmpNum2 = 0;
            while (password.Length < 8)
            {
                if (tmpNum >= tmpPassword2.Length) tmpNum = 0;
                if (tmpNum2 >= password.Length) tmpNum2 = 0;
                password = password.Insert(tmpNum2, tmpPassword2[tmpNum].ToString());
                tmpNum++;
                tmpNum2 += 2;
            }
        }

        //Debug.Log("password:" + password);
    }
}
