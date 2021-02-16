using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CFPGADrv;
using DWORD = System.UInt32;
using System.Text.RegularExpressions;
using Quixant.LibQxt.LPComponent;
using System.Runtime.InteropServices;

public class BackEnd_InitMachine_Login : MonoBehaviour
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
    //datatime[0]: Year
    //datetime[1]: Month
    //datetime[2]: Date
    //datetime[3]: Hour
    //datetime[4]: Minute
    //datetime[5]: Second

    [SerializeField] GameObject passwordPanel;
    [SerializeField] Text placeholder;
    [SerializeField] InputField passwordInput;
    string password;

    string tmp_year, tmp_month, tmp_day, tmp_hour;
    string tmp_Login_Password;

    //public GameObject ininMachine_SucessText;

    public void SetPassword()
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

        LoginPassword();
    }

    public void LoginPassword()
    {
        password = null;
        int year = (int)datetime[0];
        int month = (int)datetime[1];
        int day = (int)datetime[2];
        int hour = (int)datetime[3];

        tmp_year = System.Convert.ToString(datetime[0], 16).ToUpper();
        BitTrans(tmp_year);
        tmp_year = tmp_Login_Password;

        tmp_month = System.Convert.ToString(datetime[1] + 20, 16).ToUpper();
        BitTrans(tmp_month);
        tmp_month = tmp_Login_Password;

        tmp_day = System.Convert.ToString(datetime[2] + 20, 16).ToUpper();
        BitTrans(tmp_day);
        tmp_day = tmp_Login_Password;

        tmp_hour = System.Convert.ToString(datetime[3] + 20, 16).ToUpper();
        BitTrans(tmp_hour);
        tmp_hour = tmp_Login_Password;

        password = (int.Parse(tmp_year) + (int.Parse(tmp_month) * int.Parse(tmp_day) * int.Parse(tmp_hour))).ToString();
        password = System.Convert.ToString(int.Parse(password), 16).ToUpper();
        BitTrans(password);
        password = tmp_Login_Password;

        if (password.Length < 8)
        {
            string tmpTimeInser = null;
            int tmpTimeNum = 0;

            tmpTimeInser = (year - (month + day + hour + 37 * 3)).ToString();
            tmpTimeInser = tmpTimeInser += tmpTimeInser;
            Regex regex = new Regex(@"[-]"); //去除"-"符號
            while (password.Length < 8)
            {
                if (regex.IsMatch(tmpTimeInser))
                    break;
                password = password.Insert(0, tmpTimeInser[tmpTimeNum].ToString());
                tmpTimeNum++;
            }
        }

        //Debug.Log(password);
    }

    public void BitTrans(string trans_string) //16進位轉換
    {
        string tmpString = null;
        for (int i = 0; i < trans_string.Length; i++)
        {
            switch (trans_string[i])
            {
                case 'A':
                    tmpString += "10";
                    break;
                case 'B':
                    tmpString += "11";
                    break;
                case 'C':
                    tmpString += "12";
                    break;
                case 'D':
                    tmpString += "13";
                    break;
                case 'E':
                    tmpString += "14";
                    break;
                case 'F':
                    tmpString += "15";
                    break;
                default:
                    tmpString += trans_string[i].ToString();
                    break;
            }
        }

        tmp_Login_Password = tmpString;
    }

    public void PasswordButton(string button)
    {
        switch (button)
        {
            case "Confirm":
                if (passwordInput.text == password)
                {
                    passwordInput.text = "";
                    BackEnd_Login initializeSetting = this.gameObject.GetComponent<BackEnd_Login>();
                    initializeSetting.InitializeSetting();
                    placeholder.text = "";
                    passwordPanel.SetActive(false);
                    //ininMachine_SucessText.SetActive(true); 
                    ////Debug.Log("Pass");
                }
                else
                {
                    SetText("PasswordWrong");
                    ////Debug.Log("error");
                }
                break;
            case "Cancel":
                passwordPanel.SetActive(false);
                SetText("Clean");
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
        }
    }
}
