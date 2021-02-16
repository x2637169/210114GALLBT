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

public class BackEnd_RTP_Login : MonoBehaviour
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
    public BackEnd_IniFile iniFile;
    [SerializeField] BackEnd_Data backEnd_Data;
    [SerializeField] GameObject passwordPanel;
    [SerializeField] Text placeholder;
    [SerializeField] InputField passwordInput;
    string Time_password_login;
    string Time_password_rtp;

    [SerializeField] GameObject RTPPanel;

    public List<RTPList> RTP_List;

    [Serializable]
    public class RTPList
    {
        public int RTP_index;
        public int winrates;
    }

    string serialText;

    int[] RTPIndex = new int[9] { 8, 7, 6, 5, 4, 3, 2, 1, 0 };
    string tmp_year, tmp_month, tmp_day, tmp_hour;
    string tmp_Login_Password;

    public void PasswordButton(string button)
    {
        switch (button)
        {
            case "Confirm":
                if (passwordInput.text != "")
                {
                    if (passwordInput.text == Time_password_login)
                    {
                        passwordInput.text = "";
                        passwordPanel.SetActive(false);
                        RTPPanel.SetActive(true);
                        //Debug.Log("Login");
                        break;
                    }

                    if (passwordInput.text.Length >= 8)
                    {
                        string checkString = null;
                        checkString = serialText;
                        Regex regex = new Regex(@"[A-Z,a-z]");  //除去機台序號英文字母
                        if (regex.IsMatch(checkString)) checkString = Regex.Replace(checkString, @"[A-Z,a-z]", String.Empty);
                        if (checkString.Length >= 2) checkString = checkString.Substring(checkString.Length - 1, 1); //抓取驗證碼
                        else checkString = "0";

                        checkString = checkString += Time_password_rtp.Substring(Time_password_rtp.Length - 1, 1);

                        if (passwordInput.text.Length == 10 && passwordInput.text[0] == checkString[0] && passwordInput.text[passwordInput.text.Length - 1] == checkString[1])
                            RTP_Decrypt(passwordInput.text);
                        else
                            SetText("RTPFailed");

                        break;
                    }

                    SetText("RTPFailed");
                }
                else
                {
                    SetText("PasswordWrong");
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

    void RTP_Decrypt(string password)
    {
        RTP_List.Clear();

        string tmpRtp = null;
        string tmpRealPassword = null;
        string tmpSerial = null;
        bool RTP_Wrong = false;
        tmpRealPassword = password;

        tmpSerial = serialText;
        Regex regex = new Regex(@"[A-Z,a-z]");  //除去機台序號英文字母
        if (regex.IsMatch(tmpSerial)) tmpSerial = Regex.Replace(serialText, @"[A-Z,a-z]", String.Empty);

        tmpRealPassword = tmpRealPassword.Remove(0, 1);
        tmpRealPassword = tmpRealPassword.Remove(tmpRealPassword.Length - 1, 1);
        tmpSerial = tmpSerial.Length > 6 ? tmpSerial.Substring(0, 6) : tmpSerial.Length > 0 ? tmpSerial.Substring(0, tmpSerial.Length) : "0";
        tmpRealPassword = ((int.Parse(tmpRealPassword) - int.Parse(Time_password_rtp) - int.Parse(tmpSerial)).ToString()).PadLeft(8, '0');
        tmpRealPassword = System.Convert.ToString(int.Parse(tmpRealPassword), 8).PadLeft(9, '0');

        for (int i = 0; i < tmpRealPassword.Length; i++)
        {
            tmpRtp = tmpRealPassword.Substring(i, 1);
            if (int.Parse(tmpRtp) >= 4)
            {
                SetText("RTPFailed");
                RTP_Wrong = true;
                break;
            }
        }

        if (!RTP_Wrong)
        {
            for (int i = 0; i < 9; i++)
            {
                tmpRtp = tmpRealPassword.Substring(i, 1);
                if (int.Parse(tmpRtp) <= 3 && int.Parse(tmpRtp) >= 1)
                    RTP_List.Add(new RTPList() { RTP_index = int.Parse(RTPIndex[i].ToString()), winrates = int.Parse(tmpRtp) - 1 });
            }

            if (RTP_List.Count == 0)
            {
                SetText("RTPFailed");
                return;
            }

            RTP_List.Sort((x, y) => -x.RTP_index.CompareTo(y.RTP_index));

            SetRTP();
            SetText("RTPSucess");
            StartCoroutine(ClosePasswordPanal());
        }

        RTP_List.Clear();
    }

    void SetRTP()
    {
        bool common = false;
        for (int i = 0; i < RTP_List.Count; i++)
        {
            BackEnd_Data.RTPwinRate[RTP_List[i].RTP_index] = RTP_List[i].winrates;
            if (RTP_List[i].RTP_index == 9)
                common = true;
        }

        if (common)
            BackEnd_Data.RTPOn = true;
        else
            BackEnd_Data.RTPOn = false;

        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, true);
    }

    IEnumerator ClosePasswordPanal()
    {
        yield return new WaitForSeconds(1f);
        passwordPanel.SetActive(false);
    }

    public void SetPassword()//時間加密
    {
        SetText("Clean");
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, false);
        serialText = iniFile.ReadIniContent("Serial", "serial");

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

        LoginPassword(); //設置進入RTP用的時間
        Time_password_rtp = System.Convert.ToString(datetime[0] * (datetime[1] * (datetime[2] + datetime[3])) + 1, 8); //設置RTP用的時間
    }

    public void LoginPassword()
    {
        Time_password_login = null;

        int year = (int)datetime[0];
        int month = (int)datetime[1];
        int day = (int)datetime[2];
        int hour = (int)datetime[3];

        tmp_year = System.Convert.ToString(year, 16).ToUpper();
        BitTrans(tmp_year);
        tmp_year = tmp_Login_Password;

        tmp_month = System.Convert.ToString(month + 20, 16).ToUpper();
        BitTrans(tmp_month);
        tmp_month = tmp_Login_Password;

        tmp_day = System.Convert.ToString(day + 20, 16).ToUpper();
        BitTrans(tmp_day);
        tmp_day = tmp_Login_Password;

        tmp_hour = System.Convert.ToString(hour + 20, 16).ToUpper();
        BitTrans(tmp_hour);
        tmp_hour = tmp_Login_Password;

        Time_password_login = (int.Parse(tmp_year) + (int.Parse(tmp_month) * int.Parse(tmp_day) * int.Parse(tmp_hour))).ToString();
        Time_password_login = System.Convert.ToString(int.Parse(Time_password_login), 16).ToUpper();
        BitTrans(Time_password_login);
        Time_password_login = tmp_Login_Password;

        if (Time_password_login.Length < 8)
        {
            string tmpTimeInser = null;
            int tmpTimeNum = 0;

            tmpTimeInser = (year - (month + day + hour + 37 * 3)).ToString();
            tmpTimeInser = tmpTimeInser += tmpTimeInser;
            Regex regex = new Regex(@"[-]"); //去除"-"符號
            while (Time_password_login.Length < 8)
            {
                if (regex.IsMatch(tmpTimeInser))
                    break;
                Time_password_login = Time_password_login.Insert(0, tmpTimeInser[tmpTimeNum].ToString());
                tmpTimeNum++;
            }
        }

        Debug.Log(Time_password_login);
    }

    public void BitTrans(string trans_string)
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

    public void SetText(string text)
    {
        switch (text)
        {
            case "Clean":
                RTP_List.Clear();
                passwordInput.text = "";
                placeholder.text = "輸入密碼";
                placeholder.color = new Color32(255, 255, 255, 128);
                break;
            case "PasswordWrong":
                passwordInput.text = "";
                placeholder.text = "密碼錯誤";
                placeholder.color = new Color32(255, 0, 0, 255);
                break;
            case "RTPSucess":
                passwordInput.text = "";
                placeholder.text = "RTP設置成功";
                placeholder.color = new Color32(255, 0, 0, 255);
                break;
            case "RTPFailed":
                passwordInput.text = "";
                placeholder.text = "RTP設置失敗";
                placeholder.color = new Color32(255, 0, 0, 255);
                break;
            default:
                //Debug.Log("Case不存在");
                break;
        }
    }
}

