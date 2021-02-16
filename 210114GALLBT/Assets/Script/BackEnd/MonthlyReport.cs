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

public class MonthlyReport : MonoBehaviour
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

    [SerializeField] NewSramManager newSramManager;
    [SerializeField] GameObject passwordPanel;
    public BackEnd_IniFile iniFile;
    public InputField passwordInput;
    public Text placeholder;

    public string[] account_Text;
    public string[] time_Text;
    public string[] time_Text2;
    public string serial_Text;
    string Password;
    string Password2;
    string PasswordRTP;
    string PasswordRTP2;

    string bitString;
    string account_String;
    int account_Int;
    string time_String;
    int time_Int;
    string serial_String;

    string lastPassword;
    float last_account;
    float last_time;
    private string[] split;
    BackEnd_Data backEnd_Data;

    bool SencondEncrypt = false;

    void OnEnable()
    {
        backEnd_Data = FindObjectOfType<BackEnd_Data>();

        account_Text = new string[3];
        time_Text = new string[4];
        time_Text2 = new string[4];
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, false);
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.AccountData, false);
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.AccountData_Class, false);
        //帳務
        account_Text[0] = BackEnd_Data.takeInScore.ToString();
        account_Text[1] = BackEnd_Data.takeOutScore.ToString();
        account_Text[2] = BackEnd_Data.coinIn.ToString();
        //時間
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

        time_Text[0] = datetime[0].ToString();
        time_Text[1] = datetime[1].ToString();
        time_Text[2] = datetime[2].ToString();
        time_Text[3] = datetime[3].ToString();
        //機台序號
        serial_Text = iniFile.ReadIniContent("Serial", "serial");

        Debug.Log("Encrypt One");
        SencondEncrypt = false;
        Encrypt();

        Debug.Log("Encrypt Two");

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

        time_Text2[0] = datetime[0].ToString();
        time_Text2[1] = datetime[1].ToString();
        time_Text2[2] = datetime[2].ToString();
        time_Text2[3] = datetime[3].ToString();
        time_Text2[3] = ((int.Parse(time_Text2[3]) - 1) == -1 ? 23 : (int.Parse(time_Text2[3]) - 1)).ToString();
        Debug.Log("time_Text2[3] : " + time_Text2[3]);

        SencondEncrypt = true;
        Encrypt();
    }
    int month;
    bool PasswordWrong = false;
    public void PasswordButton(string button)
    {
        switch (button)
        {
            case "Confirm":
                PasswordWrong = true;
                try
                {
                    if (!string.IsNullOrWhiteSpace(passwordInput.text))
                    {
                        PasswordWrong = false;

                        string checkNum = passwordInput.text.Substring(passwordInput.text.Length - 1, 1);
                        passwordInput.text = passwordInput.text.Remove(passwordInput.text.Length - 1, 1);

                        double tmpRtp = 0;
                        double tmpRtp2 = 0;
                        tmpRtp = (double.Parse(passwordInput.text) - double.Parse(Password));
                        tmpRtp2 = (double.Parse(passwordInput.text) - double.Parse(Password2));
                        Debug.Log("tmpRtp: " + tmpRtp);
                        Debug.Log("tmpRtp2: " + tmpRtp2);

                        string tmpSRTP = tmpRtp.ToString();
                        string tmpSRTP2 = tmpRtp2.ToString();
                        int rtp_int = 1;

                        if (tmpRtp > 0)
                        {
                            for (int i = 0; i < tmpSRTP.Length; i++)
                            {
                                rtp_int *= int.Parse(tmpSRTP[i].ToString());
                            }

                            rtp_int += int.Parse(tmpSRTP[tmpSRTP.Length - 1].ToString());
                            tmpSRTP = rtp_int.ToString();
                            tmpSRTP = tmpSRTP.Substring(tmpSRTP.Length - 1, 1);
                        }
                        else if (tmpRtp == 0)
                        {
                            tmpSRTP = "0";
                        }

                        Debug.Log("checkNum: " + checkNum);
                        Debug.Log("tmpSRTP: " + tmpSRTP);
                        bool tmpRTPisWrong = false;
                        if (tmpRtp < 0 || tmpSRTP != checkNum)
                        {
                            tmpRTPisWrong = true;
                            if (tmpRtp2 > 0)
                            {
                                rtp_int = 1;
                                for (int i = 0; i < tmpSRTP2.Length; i++)
                                {
                                    rtp_int *= int.Parse(tmpSRTP2[i].ToString());
                                }

                                rtp_int += int.Parse(tmpSRTP2[tmpSRTP2.Length - 1].ToString());
                                tmpSRTP2 = rtp_int.ToString();
                                tmpSRTP2 = tmpSRTP2.Substring(tmpSRTP2.Length - 1, 1);
                            }
                            else if (tmpRtp2 == 0)
                            {
                                tmpSRTP2 = "0";
                            }
                            Debug.Log("checkNum: " + checkNum);
                            Debug.Log("tmpSRTP2: " + tmpSRTP2);

                            if (tmpRtp2 < 0 || tmpSRTP2 != checkNum)
                            {
                                Debug.Log("tmpRtp Worng");
                                PasswordWrong = true;
                                goto PasswordisWorng;
                            }
                        }

                        if (tmpRtp >= 0) PasswordRTP = Bit3_TransToDec(tmpRtp, 9);
                        if (tmpRtp2 >= 0) PasswordRTP2 = Bit3_TransToDec(tmpRtp2, 9);
                        Debug.Log("PasswordRTP: " + PasswordRTP);
                        Debug.Log("PasswordRTP2: " + PasswordRTP2);
                        bool PasswordRTP_Wrong = false;
                        if (!string.IsNullOrWhiteSpace(PasswordRTP))
                        {
                            if (PasswordRTP.Length == 9)
                            {
                                for (int i = 0; i < PasswordRTP.Length; i++)
                                {
                                    if (int.Parse(PasswordRTP[i].ToString()) > 2 || int.Parse(PasswordRTP[i].ToString()) < 0)
                                    {
                                        Debug.Log("PasswordRTP[i]: " + PasswordRTP[i]);
                                        PasswordRTP_Wrong = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("PasswordRTP Worng");
                                PasswordRTP_Wrong = true;
                            }

                            if (PasswordRTP_Wrong)
                            {
                                if (!string.IsNullOrWhiteSpace(PasswordRTP2))
                                {
                                    if (PasswordRTP2.Length == 9)
                                    {
                                        for (int i = 0; i < PasswordRTP2.Length; i++)
                                        {
                                            if (int.Parse(PasswordRTP2[i].ToString()) > 2 || int.Parse(PasswordRTP2[i].ToString()) < 0)
                                            {
                                                Debug.Log("PasswordRTP2[i]: " + PasswordRTP2[i]);
                                                PasswordWrong = true;
                                                goto PasswordisWorng;
                                            }
                                        }

                                        PasswordWrong = false;
                                    }
                                    else
                                    {
                                        Debug.Log("PasswordRTP2 Worng");
                                        PasswordWrong = true;
                                        goto PasswordisWorng;
                                    }
                                }
                                else
                                {
                                    Debug.Log("PasswordRTP2 Worng");
                                    PasswordWrong = true;
                                    goto PasswordisWorng;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("PasswordRTP Worng");
                            PasswordWrong = true;
                            goto PasswordisWorng;
                        }

                        string tmpPassword = null;
                        string tmpPassword2 = null;
                        if (!PasswordRTP_Wrong && !tmpRTPisWrong)
                        {
                            tmpPassword = (double.Parse(passwordInput.text) - tmpRtp).ToString().PadLeft(8, '0');
                            tmpPassword2 = (double.Parse(passwordInput.text) - tmpRtp).ToString().PadLeft(8, '0');
                            Debug.Log("1 tmpPassword: " + tmpPassword);
                            Debug.Log("1 tmpPassword2: " + tmpPassword2);
                        }
                        else
                        {
                            tmpPassword = (double.Parse(passwordInput.text) - tmpRtp2).ToString().PadLeft(8, '0');
                            tmpPassword2 = (double.Parse(passwordInput.text) - tmpRtp2).ToString().PadLeft(8, '0');
                            Debug.Log("2 tmpPassword: " + tmpPassword);
                            Debug.Log("2 tmpPassword2: " + tmpPassword2);
                        }

                        if (tmpPassword != Password && tmpPassword2 != Password2)
                        {
                            Debug.Log("tmpPassword: " + tmpPassword + " Password: " + Password);
                            Debug.Log("tmpPassword2: " + tmpPassword2 + " Password: " + Password2);
                            Debug.Log("tmpPassword Worng");
                            PasswordWrong = true;
                            goto PasswordisWorng;
                        }

                        if (!PasswordWrong)
                        {
                            passwordInput.text = "";
                            placeholder.text = "";
                            passwordPanel.SetActive(false);
                            monthReportCheck();
                            if (tmpPassword == Password) SetRTP(0);
                            else if (tmpPassword2 == Password2) SetRTP(1);

                            Debug.Log("Login");
                            Debug.Log("Mod_Data.monthLock:" + Mod_Data.monthLock);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Password Exception: " + ex);
                    goto PasswordisWorng;
                }

            PasswordisWorng:
                if (PasswordWrong)
                {
                    SetText("PasswordWrong");
                }
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
        placeholder.text = "";
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
                Debug.Log("Case不存在");
                break;
        }
    }
    public void monthReportCheck()
    {
        //month = newSramManager.LoadMonthCheckData();
        if (datetime[1] == 2 && datetime[2] > 25)
        {
            newSramManager.SaveMonthCheckDate(3);
        }
        else if (datetime[2] > 27)
        {
            if (datetime[1] == 12)
            {
                month = 1;
            }
            else
            {
                month = (int)datetime[1] + 1;
            }
            newSramManager.SaveMonthCheckDate(month);
        }
        else
        {
            month = (int)datetime[1];
            newSramManager.SaveMonthCheckDate(month);
        }


        month = newSramManager.LoadMonthCheckData();
        Debug.Log("month: " + month);
        if (datetime[1] == 12 && (month == 12 || month == 1))
        {
            Mod_Data.monthLock = false;
        }
        else if (month - datetime[1] >= 0 && month - datetime[1] <= 1)
        {
            Mod_Data.monthLock = false;
        }
    }
    public void Encrypt()
    {
        account_String = null;
        time_String = null;
        serial_String = null;
        lastPassword = null;
        split = null;
        account_Int = 0;
        time_Int = 0;
        int lastPassword_Num = 0;

        //帳務
        for (int i = 0; i < account_Text.Length; i++)
        {
            Bit16_Trans(account_Text[i]);
            if (account_Int == 0) account_Int = int.Parse(bitString);
            else account_Int = account_Int + int.Parse(bitString);
        }
        account_String = account_Int.ToString();
        Bit16_Trans(account_String);
        account_String = bitString;

        //時間
        if (!SencondEncrypt)
        {
            for (int i = 0; i < time_Text.Length; i++)
            {
                Bit16_Trans(time_Text[i]);
                if (time_Int == 0) time_Int = int.Parse(bitString);
                else time_Int = time_Int * int.Parse(bitString);
            }
        }
        else
        {
            for (int i = 0; i < time_Text2.Length; i++)
            {
                Bit16_Trans(time_Text2[i]);
                if (time_Int == 0) time_Int = int.Parse(bitString);
                else time_Int = time_Int * int.Parse(bitString);
            }
        }
        time_String = time_Int.ToString();

        //機台序號
        Bit16_Trans(serial_Text);
        Bit16_Trans(bitString);
        serial_String = long.Parse(bitString).ToString();

        if (!SencondEncrypt)
        {
            Password = (int.Parse(account_String) + int.Parse(time_String) + long.Parse(serial_String) + 1).ToString();
        }
        else
        {
            Password2 = (int.Parse(account_String) + int.Parse(time_String) + long.Parse(serial_String) + 1).ToString();
        }

        if (int.Parse(account_Text[0]) != 0 && int.Parse(account_Text[1]) != 0)
            if (int.Parse(account_Text[0]) > int.Parse(account_Text[1]))
                last_account = float.Parse(account_Text[1]) / float.Parse(account_Text[0]);
            else
                last_account = float.Parse(account_Text[0]) / float.Parse(account_Text[1]);
        else
            last_account = 0;

        if (!SencondEncrypt)
        {
            float tmpTime = ((float.Parse(time_Text[1]) + 10) * (float.Parse(time_Text[2]) + 10) * (float.Parse(time_Text[3]) + 10));
            if (float.Parse(time_Text[0]) != 0 && tmpTime != 0)
                if (float.Parse(time_Text[0]) > tmpTime)
                    last_time = tmpTime / float.Parse(time_Text[0]);
                else
                    last_time = float.Parse(time_Text[0]) / tmpTime;
            else
                last_time = 0;
        }
        else
        {
            float tmpTime = ((float.Parse(time_Text2[1]) + 10) * (float.Parse(time_Text2[2]) + 10) * (float.Parse(time_Text2[3]) + 10));
            if (float.Parse(time_Text2[0]) != 0 && tmpTime != 0)
                if (float.Parse(time_Text2[0]) > tmpTime)
                    last_time = tmpTime / float.Parse(time_Text2[0]);
                else
                    last_time = float.Parse(time_Text2[0]) / tmpTime;
            else
                last_time = 0;
        }
        lastPassword = (last_account + last_time).ToString();

        Regex regex = new Regex(@"[.]");
        if (regex.IsMatch(lastPassword))
        {
            split = Regex.Split(lastPassword, @"[.]");
            lastPassword = split[1];
        }
        lastPassword = int.Parse(lastPassword).ToString();

        if (!SencondEncrypt)
        {
            while (Password.Length < 8)
            {
                if (lastPassword_Num >= lastPassword.Length) lastPassword_Num = 0;
                Password = Password.Insert(0, lastPassword[lastPassword_Num].ToString());
                lastPassword_Num++;
            }
        }
        else
        {
            while (Password2.Length < 8)
            {
                if (lastPassword_Num >= lastPassword.Length) lastPassword_Num = 0;
                Password2 = Password2.Insert(0, lastPassword[lastPassword_Num].ToString());
                lastPassword_Num++;
            }
        }

        Debug.Log("Password 1 : " + Password);
        Debug.Log("Password 2 : " + Password2);
    }

    void Bit16_Trans(string trans_string)
    {
        Regex regex = new Regex(@"[A-Z,a-z]");
        if (regex.IsMatch(trans_string)) trans_string = Regex.Replace(trans_string, @"[A-Z,a-z]", String.Empty);

        trans_string = System.Convert.ToString(long.Parse(trans_string), 16).ToUpper();
        string tmpString = null;
        bitString = null;

        for (int i = 0; i < trans_string.Length; i++)
        {
            switch (trans_string[i])
            {
                case 'A':
                    tmpString = "1";
                    break;
                case 'B':
                    tmpString = "2";
                    break;
                case 'C':
                    tmpString = "3";
                    break;
                case 'D':
                    tmpString = "4";
                    break;
                case 'E':
                    tmpString = "5";
                    break;
                case 'F':
                    tmpString = "6";
                    break;
                default:
                    tmpString = trans_string[i].ToString();
                    break;
            }
            bitString += tmpString;
        }
    }

    string Bit3_TransToDec(double tmpBit, int length)
    {
        string i = null;
        i += tmpBit % 3;

        while (true)
        {
            tmpBit = Math.Floor(tmpBit / 3);
            if (tmpBit <= 0) break;
            i = i.Insert(0, (tmpBit % 3).ToString());
        }

        i = i.PadLeft(length, '0');
        return i;
    }

    void SetRTP(int firstOrSencond)
    {
        Debug.Log("firstOrSencond: " + firstOrSencond);
        int y = 0;

        if (firstOrSencond == 0 && PasswordRTP.Length >= 9)
        {
            for (int i = 8; i >= 0; i--)
            {
                BackEnd_Data.RTPwinRate[i] = int.Parse(PasswordRTP[y].ToString());
                Debug.Log("i: " + i + " : " + BackEnd_Data.RTPwinRate[i] + " : " + int.Parse(PasswordRTP[y].ToString()));
                y++;
            }
        }
        else if (firstOrSencond == 1 && PasswordRTP2.Length >= 9)
        {
            for (int i = 8; i >= 0; i--)
            {
                BackEnd_Data.RTPwinRate[i] = int.Parse(PasswordRTP2[y].ToString());
                Debug.Log("i: " + i + " : " + BackEnd_Data.RTPwinRate[i] + " : " + int.Parse(PasswordRTP2[y].ToString()));
                y++;
            }
        }

        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, true);
    }
}
