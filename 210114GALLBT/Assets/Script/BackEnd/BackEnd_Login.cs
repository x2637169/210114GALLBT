using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CFPGADrv;
using BYTE = System.Byte;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;

public class BackEnd_Login : MonoBehaviour
{
    #region IPC變數

    #region GS變數
#if GS

    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS(); //賽菲硬體初始化
    byte DataByte = 1;//賽飛訊號
    public static bool[] ButtonClickLong = new bool[32];//賽菲按鈕

#endif
    #endregion

    #region QXT變數
#if QXT

    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readdword(UInt32 offset);

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_readword(UInt32 offset);

    [DllImport("libqxt")]
    public static extern Byte qxt_dio_writeword(UInt32 offset, UInt32 Value);

    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readword_fb(UInt32 offset);

    volatile System.UInt32 read_long_result1, read_long_result2, read_long_result_stored1, read_long_result_stored2;

#endif
    #endregion

    #region AGP變數

#if AGP

    byte DataByte = 1;  //艾訊訊號
    public static bool[] ButtonClickLong = new bool[32];    //艾訊按鈕

#endif
    #endregion

    #endregion

    [SerializeField] InputField passwordInput;
    [SerializeField] GameObject passwordPanal;
    [SerializeField] BackEnd_Data backEnd_Data;
    [SerializeField] Text errorText, titleText, usertypeText, openclearSetText;
    [SerializeField] MonthlyReport monthlyReport;
    [SerializeField] Mod_UIController mod_UIController;
    public static bool LoginPass = false;
    bool changePassword = false, comfirmPassword = false;
    string tmp, changePasswordstring;
    float InitializeSettingButtonHoldTime = 0;
    public GameObject InitializeSettingPopWindows;
    public string button_bit;

    void Start()
    {
#if GS
        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FpgaPic_Init(); //賽菲
#endif
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, false);
        tmp = BackEnd_Data.loginPassword;

        //Debug.Log("Pass:" + tmp);
    }

    // Update is called once per frame
    void Update()
    {
        Mod_Data.IOLock = true;
        if (Mod_Data.IOLock)
        {
            if (Mod_Data.hasMonthCheck)
            {
                usertypeText.text = "現在是外用";
            }
            else
            {
                usertypeText.text = "現在是自用";
            }

            if (Mod_Data.openclearSet)
            {
                openclearSetText.text = "開洗鍵開啟";
            }
            else
            {
                openclearSetText.text = "開洗鍵關閉";
            }

            #region KeyBoard_Login
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     passwordInput.text = "";
            //     passwordPanal.SetActive(false);
            //     changePassword = false;
            //     comfirmPassword = false;
            // }
            // if (Input.GetKeyDown(KeyCode.Q))
            // {  // 停1
            //     passwordInput.text += 1;
            // }
            // if (Input.GetKeyDown(KeyCode.W))
            // {  // 停2
            //     passwordInput.text += 2;
            // }
            // if (Input.GetKeyDown(KeyCode.E))
            // {  // 停3
            //     passwordInput.text += 3;
            // }
            // if (Input.GetKeyDown(KeyCode.R))
            // {  // 停4 押注-
            //     passwordInput.text += 4;
            // }
            // if (Input.GetKeyDown(KeyCode.T))
            // {  // 停5 押注+
            //     backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, false);
            //     tmp = BackEnd_Data.loginPassword;
            //     if (comfirmPassword)
            //     {
            //         if (changePasswordstring == passwordInput.text)
            //         {
            //             tmp = changePasswordstring;
            //             passwordPanal.SetActive(false);
            //             BackEnd_Data.loginPassword = tmp;
            //             backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, true);

            //             backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, false);
            //             //Debug.Log(BackEnd_Data.loginPassword);
            //             //Debug.Log(tmp);
            //         }
            //         else
            //         {
            //             changePasswordstring = "";
            //             titleText.text = "新密碼";
            //             errorText.text = "輸入錯誤";
            //         }
            //         passwordInput.text = "";
            //         comfirmPassword = false;
            //         goto afterComfirm;
            //     }

            //     if (!changePassword)
            //     {
            //         if (passwordInput.text == tmp)
            //         {
            //             LoginPass = true;
            //             passwordInput.text = "";
            //             passwordPanal.SetActive(false);
            //             //Debug.Log("Pass");
            //         }
            //         else
            //         {
            //             errorText.text = "密碼錯誤";
            //             passwordInput.text = "";
            //             //Debug.Log("error");
            //         }
            //     }
            //     else
            //     {
            //         titleText.text = "確認新密碼";
            //         changePasswordstring = passwordInput.text;
            //         passwordInput.text = "";
            //         comfirmPassword = true;
            //     }
            // afterComfirm:;
            // }
            #endregion

            #region 偵測按鈕輸入

            #region GS偵測按鈕
#if GS

            for (int i = 0; i < 32; i++)
            {
                Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)i, ref DataByte);
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

#endif
            #endregion

            #region QXT偵測按鈕
#if QXT

            QXT_Button();

#endif
            #endregion

            #region AGP偵測按鈕
#if AGP

            for (int i = 0; i < 32; i++)
            {
                int result = 0;
                result = AGP_Func.AXGMB_DIO_DiReadBit((BYTE)i, ref DataByte);
                //Debug.Log("result: " + (AGP_Func.AGP_ReturnValue)result + " pins: " + i + " ref: " + DataByte);
                if (DataByte == 0 && !ButtonClickLong[i])
                {
                    ButtonClickLong[i] = true;
                    AGPButton(i);
                }
                else if (DataByte != 0)
                {
                    ButtonClickLong[i] = false;
                }
            }

#endif
            #endregion

            #endregion

            if (LoginPass && InitializeSettingPopWindows.activeInHierarchy)
            {
                InitialMachine();
            }
        }
    }

    #region IPC按鈕登入

    #region GS按鈕登入
#if GS

    public void SephirothButton(int ButtonNumber)
    {
        switch (ButtonNumber)
        {
            case 0:  //最大押注

                break;
            case 2: //自動

                break;
            case 3: //停1
                passwordInput.text += 1;
                break;
            case 4: //停2
                passwordInput.text += 2;
                break;
            case 5: //停3
                passwordInput.text += 3;
                break;
            case 6: //停4 押注-
                passwordInput.text += 4;
                break;
            case 7: //停5 押注+
                backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, false);
                tmp = BackEnd_Data.loginPassword;
                if (comfirmPassword)
                {
                    if (changePasswordstring == passwordInput.text)
                    {
                        tmp = changePasswordstring;
                        passwordPanal.SetActive(false);
                        BackEnd_Data.loginPassword = tmp;
                        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, true);
                    }
                    else
                    {
                        changePasswordstring = "";
                        titleText.text = "新密碼";
                        errorText.text = "輸入錯誤";
                    }
                    passwordInput.text = "";
                    comfirmPassword = false;
                    goto afterComfirm;
                }

                if (!changePassword)
                {
                    if (passwordInput.text == tmp)
                    {
                        LoginPass = true;
                        passwordInput.text = "";
                        passwordPanal.SetActive(false);
                        //Debug.Log("Pass");
                    }
                    else
                    {
                        errorText.text = "密碼錯誤";
                        passwordInput.text = "";
                        //Debug.Log("error");
                    }
                }
                else
                {
                    titleText.text = "確認新密碼";
                    changePasswordstring = passwordInput.text;
                    passwordInput.text = "";
                    comfirmPassword = true;
                }
            afterComfirm:;
                break;
            case 8: //開始 全停 得分
                passwordInput.text = "";
                passwordPanal.SetActive(false);
                changePassword = false;
                comfirmPassword = false;
                break;
            case 9: //側面前方按鈕

                break;
            case 10: //側面後方按鈕

                break;
            case 12: //統計資料

                break;
            case 13: //設定

                break;

        }
    }

#endif
    #endregion

    #region QXT按鈕登入
#if QXT

    public void QXT_Button()
    {
    #region 勝圖硬體<-------------------
        read_long_result1 = qxt_dio_readdword(0);  //讀取輸入訊號
        read_long_result2 = qxt_dio_readdword(4);
        if (qxt_dio_readword(1) != 255) { print(qxt_dio_readword(1)); }
        if (read_long_result1 != read_long_result_stored1)
        {
            switch (read_long_result1)
            {   //按鈕順序由左至右
                case 0xFFFFFFFB://出票
                    passwordInput.text += 1;
                    break;
                case 0xFFFFFFFD://服務燈
                    passwordInput.text += 2;
                    break;
                case 0xFFDFFFFF://停1
                    passwordInput.text += 3;
                    break;
                case 0xFFBFFFFF://停2
                    passwordInput.text += 4;
                    break;
                case 0xFF7FFFFF://停3
                    backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, false);
                    tmp = BackEnd_Data.loginPassword;
                    if (comfirmPassword)
                    {
                        if (changePasswordstring == passwordInput.text)
                        {
                            tmp = changePasswordstring;
                            passwordPanal.SetActive(false);
                            BackEnd_Data.loginPassword = tmp;
                            backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, true);
                        }
                        else
                        {
                            changePasswordstring = "";
                            titleText.text = "新密碼";
                            errorText.text = "輸入錯誤";
                        }
                        passwordInput.text = "";
                        comfirmPassword = false;
                        goto afterComfirm;
                    }

                    if (!changePassword)
                    {
                        if (passwordInput.text == tmp)
                        {
                            LoginPass = true;
                            passwordInput.text = "";
                            passwordPanal.SetActive(false);
                            //Debug.Log("Pass");
                        }
                        else
                        {
                            errorText.text = "密碼錯誤";
                            passwordInput.text = "";
                            //Debug.Log("error");
                        }
                    }
                    else
                    {
                        titleText.text = "確認新密碼";
                        changePasswordstring = passwordInput.text;
                        passwordInput.text = "";
                        comfirmPassword = true;
                    }
                afterComfirm:;
                    break;
                case 0xFEFFFFFF://停4
                    passwordInput.text = "";
                    passwordPanal.SetActive(false);
                    changePassword = false;
                    comfirmPassword = false;
                    break;
                case 0xFDFFFFFF://停5
                    break;
                case 0xFFFFFFFE://開始
                    break;
                case 0x7FFFFFFF:     //轉鑰匙時的訊號 開啟後台用
                    break;
            }
            read_long_result_stored1 = qxt_dio_readdword(0);
        }
    #endregion
    }

#endif
    #endregion

    #region AGP按鈕登入
#if AGP

    public void AGPButton(int ButtonNumber)
    {
        switch (ButtonNumber)
        {
            case 0: //停1
                #region 按鈕:停1 功能:輸入密碼1
                passwordInput.text += 1;
                #endregion
                break;
            case 1: //停2
                #region 按鈕:停2 功能:輸入密碼2
                passwordInput.text += 2;
                #endregion
                break;
            case 2: //停3
                #region 按鈕:停3 功能:輸入密碼3
                passwordInput.text += 3;
                #endregion
                break;
            case 3: //停4 押注-
                #region 按鈕:停4、押注- 功能:輸入密碼4
                passwordInput.text += 4;
                #endregion
                break;
            case 4: //停5 押注+
                #region 按鈕:停5、押注+ 功能:登入
                backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, false);
                tmp = BackEnd_Data.loginPassword;
                if (comfirmPassword)
                {
                    if (changePasswordstring == passwordInput.text)
                    {
                        tmp = changePasswordstring;
                        passwordPanal.SetActive(false);
                        BackEnd_Data.loginPassword = tmp;
                        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.loginPassword, true);
                    }
                    else
                    {
                        changePasswordstring = "";
                        titleText.text = "新密碼";
                        errorText.text = "輸入錯誤";
                    }
                    passwordInput.text = "";
                    comfirmPassword = false;
                    goto afterComfirm;
                }

                if (!changePassword)
                {
                    if (passwordInput.text == tmp)
                    {
                        LoginPass = true;
                        passwordInput.text = "";
                        passwordPanal.SetActive(false);
                        //Debug.Log("Pass");
                    }
                    else
                    {
                        errorText.text = "密碼錯誤";
                        passwordInput.text = "";
                        //Debug.Log("error");
                    }
                }
                else
                {
                    titleText.text = "確認新密碼";
                    changePasswordstring = passwordInput.text;
                    passwordInput.text = "";
                    comfirmPassword = true;
                }
            afterComfirm:;
                #endregion
                break;
            case 5: //開始 全停 得分
                #region 按鈕:開始、全停、得分 功能:離開登入
                passwordInput.text = "";
                passwordPanal.SetActive(false);
                changePassword = false;
                comfirmPassword = false;
                #endregion
                break;
            case 6:  //最大押注
                break;
            case 7: //自動
                break;
            case 8: //側面前方按鈕
                break;
            case 9: //側面後方按鈕
                break;
            case 10: //統計資料
                break;
            case 11: //設定
                break;

        }
    }

#endif
    #endregion

    #endregion

    public void LoginPassword()
    {
        InitializeSettingPopWindows.SetActive(false);
        passwordPanal.SetActive(true);
        passwordInput.text = "";
        titleText.text = "密碼";
        errorText.text = "";
        changePassword = false;
        comfirmPassword = false;
    }

    public void ChagePassword()
    {
        if (LoginPass)
        {
            InitializeSettingPopWindows.SetActive(false);
            passwordInput.text = "";
            changePassword = true;
            comfirmPassword = false;
            passwordPanal.SetActive(true);
            errorText.text = "";
            titleText.text = "新密碼";
        }
    }

    public void OpenIniMachine()
    {
        if (LoginPass)
        {
            InitializeSettingPopWindows.SetActive(true);
            passwordPanal.SetActive(false);
        }
    }

    public void InitialMachine()
    {
        #region IPC初始化按鈕

        #region GS初始化按鈕
#if GS

        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)7, ref DataByte);
        if (DataByte == 0)
        {
            InitializeSettingButtonHoldTime += Time.deltaTime;
            InitializeSettingPopWindows.transform.GetChild(1).gameObject.GetComponent<Text>().text = InitializeSettingButtonHoldTime.ToString("N0");
        }
        else if (DataByte != 0)
        {
            InitializeSettingButtonHoldTime = 0;
            InitializeSettingPopWindows.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
        }

        Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)2, ref DataByte);

        if (DataByte == 0)
        {
            InitializeSettingButtonHoldTime += Time.deltaTime;
            InitializeSettingPopWindows.transform.GetChild(1).gameObject.GetComponent<Text>().text = InitializeSettingButtonHoldTime.ToString("N0");
        }
        else if (DataByte != 0)
        {
            InitializeSettingButtonHoldTime = 0;
            InitializeSettingPopWindows.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
        }
        
#endif
        #endregion

        #region QXT初始化按鈕
#if QXT

        #region 勝圖硬體<-------------------
        read_long_result1 = qxt_dio_readdword(0);  //讀取輸入訊號
        button_bit = Convert.ToString(read_long_result1, 2).PadLeft(32, '0');
        if (button_bit.Length >= 32)
        {
            if (button_bit[6] == '0' && button_bit[8] == '0')//自動&&停5
            {
                InitializeSettingButtonHoldTime += Time.deltaTime;
                InitializeSettingPopWindows.transform.GetChild(1).gameObject.GetComponent<Text>().text = InitializeSettingButtonHoldTime.ToString("N0");
            }
            else
            {
                InitializeSettingButtonHoldTime = 0;
                InitializeSettingPopWindows.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
            }
        }
        else
        {
            InitializeSettingButtonHoldTime = 0;
            InitializeSettingPopWindows.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
        }

        #endregion
#endif
        #endregion

        #region AGP初始化按鈕
#if AGP

        byte dataByte7 = 0;
        byte dataByte4 = 0;

        AGP_Func.AXGMB_DIO_DiReadBit((BYTE)7, ref dataByte7);
        AGP_Func.AXGMB_DIO_DiReadBit((BYTE)4, ref dataByte4);

        if (dataByte7 == 0 && dataByte4 == 0)
        {
            InitializeSettingButtonHoldTime += Time.deltaTime;
            InitializeSettingPopWindows.transform.GetChild(1).gameObject.GetComponent<Text>().text = InitializeSettingButtonHoldTime.ToString("N0");
        }
        else if (dataByte7 != 0 || dataByte4 != 0)
        {
            InitializeSettingButtonHoldTime = 0;
            InitializeSettingPopWindows.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
        }

#endif
        #endregion

        #endregion

        if (InitializeSettingButtonHoldTime > 5.5)
        {
            InitializeSettingButtonHoldTime = 0;
        }
    }

    public void InitializeSetting()
    {
        GameObject.Find("BackEndManager").GetComponent<NewSramManager>().InitializeSetting();
        monthlyReport.monthReportCheck();
        mod_UIController.UpdateScore();
    }
    public void monitorSetting()
    {
        System.Diagnostics.Process.Start("C:\\Program Files (x86)\\eGalaxTouch\\eGalaxTouch.exe");
    }
}
