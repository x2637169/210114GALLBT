using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BYTE = System.Byte;
using CFPGADrv;
using System;
using System.Runtime.InteropServices;

public class BackEnd_TestWindow : MonoBehaviour
{
    public GameObject[] ButtonLightObject;
    public GameObject TestWindow;

    #region 不同IPC變數

    #region QXT
#if QXT

    [DllImport("libqxt")]
    public static extern UInt32 qxt_dio_readdword(UInt32 offset);
    volatile System.UInt32 read_long_result1;
    public string button_bit;

#endif
    #endregion

    #region GS
#if GS

    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS();
    byte DataByte = 1;

#endif
    #endregion

    #region AGP
#if AGP

    byte DataByte = 1;

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

#endif
    #endregion

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (TestWindow.activeInHierarchy)
        {
            #region DIO_Read

            #region QXT_DIO_Read
#if QXT

            read_long_result1 = qxt_dio_readdword(0);
            button_bit = Convert.ToString(read_long_result1, 2).PadLeft(32, '0');
            if (button_bit.Length >= 32)
            {
                for (int i = 0; i < button_bit.Length; i++)
                {
                    if (button_bit[i] == '0') ButtonTestLight(i, new Color32(0, 255, 0, 255));
                    else ButtonTestLight(i, new Color32(255, 255, 255, 255));
                }
            }

#endif
            #endregion

            #region GS_DIO_Read
#if GS

            for (int i = 0; i < 32; i++)
            {
                Status = (CFPGADrvBridge.STATUS)CFPGADrvBridge.FD_GetGPIByIndex(0, (BYTE)i, ref DataByte);
                if (DataByte == 0)
                {
                    ButtonTestLight(i, new Color32(0, 255, 0, 255));
                }
                else if (DataByte != 0)
                {
                    ButtonTestLight(i, new Color32(255, 255, 255, 255));
                }
            }
            
#endif
            #endregion

            #region AGP_DIO_Read
#if AGP

            for (int i = 0; i < 32; i++)
            {
                AGP_Func.AXGMB_DIO_DiReadBit((Byte)i, ref DataByte);

                if (DataByte == 0)
                {
                    ButtonTestLight(i, new Color32(0, 255, 0, 255));
                }
                else if (DataByte != 0)
                {
                    ButtonTestLight(i, new Color32(255, 255, 255, 255));
                }
            }

#endif
            #endregion

            #endregion
        }
    }

    void ButtonTestLight(int ButtonNumber, Color32 LightColor)
    {

#if Server

        #region Server

        #region QXT_Light
#if QXT

        switch (ButtonNumber)
        {
            case 0:
                ButtonLightObject[13].GetComponent<Image>().color = LightColor;
                break;
            case 1:
                ButtonLightObject[9].GetComponent<Image>().color = LightColor;
                break;
            case 6:
                ButtonLightObject[2].GetComponent<Image>().color = LightColor;
                break;
            case 7:
                ButtonLightObject[0].GetComponent<Image>().color = LightColor;
                break;
            case 8:
                ButtonLightObject[7].GetComponent<Image>().color = LightColor;
                break;
            case 9:
                ButtonLightObject[6].GetComponent<Image>().color = LightColor;
                break;
            case 10:
                ButtonLightObject[5].GetComponent<Image>().color = LightColor;
                break;
            case 26:
                ButtonLightObject[14].GetComponent<Image>().color = LightColor;
                break;
            case 27:
                ButtonLightObject[12].GetComponent<Image>().color = LightColor;
                break;
            case 28:
                ButtonLightObject[1].GetComponent<Image>().color = LightColor;
                break;
            case 29:
                ButtonLightObject[3].GetComponent<Image>().color = LightColor;
                break;
            case 30:
                ButtonLightObject[4].GetComponent<Image>().color = LightColor;
                break;
            case 31:
                ButtonLightObject[8].GetComponent<Image>().color = LightColor;
                break;
            default:
                break;
        }
#endif
        #endregion

        #region GS_Light
#if GS

        switch (ButtonNumber)
        {
            case 0:
                ButtonLightObject[0].GetComponent<Image>().color = LightColor;
                break;
            case 1:
                ButtonLightObject[9].GetComponent<Image>().color = LightColor;
                break;
            case 2:
                ButtonLightObject[2].GetComponent<Image>().color = LightColor;
                break;
            case 3:
                ButtonLightObject[3].GetComponent<Image>().color = LightColor;
                break;
            case 4:
                ButtonLightObject[4].GetComponent<Image>().color = LightColor;
                break;
            case 5:
                ButtonLightObject[5].GetComponent<Image>().color = LightColor;
                break;
            case 6:
                ButtonLightObject[6].GetComponent<Image>().color = LightColor;
                break;
            case 7:
                ButtonLightObject[7].GetComponent<Image>().color = LightColor;
                break;
            case 8:
                ButtonLightObject[8].GetComponent<Image>().color = LightColor;
                break;
            case 9:
                ButtonLightObject[1].GetComponent<Image>().color = LightColor;
                break;
            case 10:
                ButtonLightObject[14].GetComponent<Image>().color = LightColor;
                break;
            case 11:
                ButtonLightObject[11].GetComponent<Image>().color = LightColor;
                break;
            case 12:
                ButtonLightObject[12].GetComponent<Image>().color = LightColor;
                break;
            case 13:
                ButtonLightObject[13].GetComponent<Image>().color = LightColor;
                break;
            default:
                break;
        }

#endif
        #endregion

        #region AGP_Light
#if AGP

        switch (ButtonNumber)
        {
            case 0:
                ButtonLightObject[0].GetComponent<Image>().color = LightColor;
                break;
            case 1:
                ButtonLightObject[1].GetComponent<Image>().color = LightColor;
                break;
            case 2:
                ButtonLightObject[2].GetComponent<Image>().color = LightColor;
                break;
            case 3:
                ButtonLightObject[3].GetComponent<Image>().color = LightColor;
                break;
            case 4:
                ButtonLightObject[4].GetComponent<Image>().color = LightColor;
                break;
            case 5:
                ButtonLightObject[5].GetComponent<Image>().color = LightColor;
                break;
            case 6:
                ButtonLightObject[6].GetComponent<Image>().color = LightColor;
                break;
            case 7:
                ButtonLightObject[7].GetComponent<Image>().color = LightColor;
                break;
            case 8:
                ButtonLightObject[8].GetComponent<Image>().color = LightColor;
                break;
            case 9:
                ButtonLightObject[9].GetComponent<Image>().color = LightColor;
                break;
            case 10:
                ButtonLightObject[14].GetComponent<Image>().color = LightColor;
                break;
            case 11:
                ButtonLightObject[11].GetComponent<Image>().color = LightColor;
                break;
            case 12:
                ButtonLightObject[12].GetComponent<Image>().color = LightColor;
                break;
            case 13:
                ButtonLightObject[13].GetComponent<Image>().color = LightColor;
                break;
            default:
                break;
        }

#endif
        #endregion

        #endregion

#else

        #region !Server

        #region QXT_Light
#if QXT
        switch (ButtonNumber)
        {
            case 0:
                ButtonLightObject[13].GetComponent<Image>().color = LightColor;
                break;
            case 6:
                ButtonLightObject[2].GetComponent<Image>().color = LightColor;
                break;
            case 7:
                ButtonLightObject[0].GetComponent<Image>().color = LightColor;
                break;
            case 8:
                ButtonLightObject[7].GetComponent<Image>().color = LightColor;
                break;
            case 9:
                ButtonLightObject[6].GetComponent<Image>().color = LightColor;
                break;
            case 10:
                ButtonLightObject[5].GetComponent<Image>().color = LightColor;
                break;
            case 26:
                ButtonLightObject[10].GetComponent<Image>().color = LightColor;
                break;
            case 27:
                ButtonLightObject[12].GetComponent<Image>().color = LightColor;
                break;
            case 28:
                ButtonLightObject[9].GetComponent<Image>().color = LightColor;
                break;
            case 29:
                ButtonLightObject[3].GetComponent<Image>().color = LightColor;
                break;
            case 30:
                ButtonLightObject[4].GetComponent<Image>().color = LightColor;
                break;
            case 31:
                ButtonLightObject[8].GetComponent<Image>().color = LightColor;
                break;
            default:
                break;
        }

#endif
        #endregion

        #region GS_Light
#if GS

        switch (ButtonNumber)
        {
            case 0:
                ButtonLightObject[0].GetComponent<Image>().color = LightColor;
                break;
            case 1:
                ButtonLightObject[9].GetComponent<Image>().color = LightColor;
                break;
            case 2:
                ButtonLightObject[2].GetComponent<Image>().color = LightColor;
                break;
            case 3:
                ButtonLightObject[3].GetComponent<Image>().color = LightColor;
                break;
            case 4:
                ButtonLightObject[4].GetComponent<Image>().color = LightColor;
                break;
            case 5:
                ButtonLightObject[5].GetComponent<Image>().color = LightColor;
                break;
            case 6:
                ButtonLightObject[6].GetComponent<Image>().color = LightColor;
                break;
            case 7:
                ButtonLightObject[7].GetComponent<Image>().color = LightColor;
                break;
            case 8:
                ButtonLightObject[8].GetComponent<Image>().color = LightColor;
                break;
            case 9:
                ButtonLightObject[9].GetComponent<Image>().color = LightColor;
                break;
            case 10:
                ButtonLightObject[10].GetComponent<Image>().color = LightColor;
                break;
            case 11:
                ButtonLightObject[11].GetComponent<Image>().color = LightColor;
                break;
            case 12:
                ButtonLightObject[12].GetComponent<Image>().color = LightColor;
                break;
            case 13:
                ButtonLightObject[13].GetComponent<Image>().color = LightColor;
                break;
            default:
                break;
        }

#endif
        #endregion

        #region AGP_Light
#if AGP

        switch (ButtonNumber)
        {
            case 0:
                ButtonLightObject[3].GetComponent<Image>().color = LightColor;
                break;
            case 1:
                ButtonLightObject[4].GetComponent<Image>().color = LightColor;
                break;
            case 2:
                ButtonLightObject[5].GetComponent<Image>().color = LightColor;
                break;
            case 3:
                ButtonLightObject[6].GetComponent<Image>().color = LightColor;
                break;
            case 4:
                ButtonLightObject[7].GetComponent<Image>().color = LightColor;
                break;
            case 5:
                ButtonLightObject[8].GetComponent<Image>().color = LightColor;
                break;
            case 6:
                ButtonLightObject[0].GetComponent<Image>().color = LightColor;
                break;
            case 7:
                ButtonLightObject[2].GetComponent<Image>().color = LightColor;
                break;
            case 8:
                ButtonLightObject[9].GetComponent<Image>().color = LightColor;
                break;
            case 9:
                ButtonLightObject[10].GetComponent<Image>().color = LightColor;
                break;
            case 10:
                ButtonLightObject[12].GetComponent<Image>().color = LightColor;
                break;
            case 11:
                ButtonLightObject[13].GetComponent<Image>().color = LightColor;
                break;
            // case 12:
            //     ButtonLightObject[12].GetComponent<Image>().color = LightColor;
            //     break;
            // case 13:
            //     ButtonLightObject[13].GetComponent<Image>().color = LightColor;
            //     break;
            default:
                break;
        }

#endif
        #endregion

        #endregion

#endif
    }

    public void CloseWindow()
    {
        TestWindow.SetActive(false);
    }

    public void OpenWindow()
    {
        TestWindow.SetActive(true);
    }
}
