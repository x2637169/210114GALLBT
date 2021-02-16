using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackEnd_SetRTP : MonoBehaviour
{
    [SerializeField] Text[] rtpButtonText;
    [SerializeField] Text commonRtpText;
    [SerializeField] Button[] eachRtpButton;
    [SerializeField] BackEnd_Data backEnd_Data;

    void OnEnable()
    {
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, false);
        SetRtpUI();
    }

    public void UpdateShowData()
    {
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, false);
        SetRtpUI();
    }

    public void SetRTPButton(int index)
    {
#if Server
        #region Server

         BackEnd_Data.RTPwinRate[index] = BackEnd_Data.RTPwinRate[index] + 1;
        if (BackEnd_Data.RTPwinRate[index] > 4) BackEnd_Data.RTPwinRate[index] = 0;

        switch (BackEnd_Data.RTPwinRate[index])
        {
            case 0:
                rtpButtonText[index].text = "超低";
                break;
            case 1:
                rtpButtonText[index].text = "低";
                break;
            case 2:
                rtpButtonText[index].text = "中";
                break;
            case 3:
                rtpButtonText[index].text = "高";
                break;
            case 4:
                rtpButtonText[index].text = "超高";
                break;
            default:
                break;
        }
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, true);

        #endregion
#else
        #region !Server

        BackEnd_Data.RTPwinRate[index] = BackEnd_Data.RTPwinRate[index] + 1;
        if (BackEnd_Data.RTPwinRate[index] > 2) BackEnd_Data.RTPwinRate[index] = 0;

        switch (BackEnd_Data.RTPwinRate[index])
        {
            case 0:
                rtpButtonText[index].text = "低";
                break;
            case 1:
                rtpButtonText[index].text = "中";
                break;
            case 2:
                rtpButtonText[index].text = "高";
                break;
            default:
                break;
        }
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, true);

        #endregion
#endif
    }

    public void SetRTPwithCommon()
    {
        BackEnd_Data.RTPOn = !BackEnd_Data.RTPOn;
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.RTPOn, true);
        SetRtpUI();
    }

    public void SetRtpUI()
    {
        if (BackEnd_Data.RTPOn)
        {
            commonRtpText.text = "修改RTP按鈕";
            // for (int i = 0; i < eachRtpButton.Length - 1; i++)
            // {
            //     eachRtpButton[i].interactable = false;
            // }
            // eachRtpButton[eachRtpButton.Length - 1].interactable = true;
        }
        else
        {
            commonRtpText.text = "修改RTP按鈕";
            for (int i = 0; i < eachRtpButton.Length; i++)
            {
                //eachRtpButton[i].interactable = true;
#if Server
                #region Server

                 switch (BackEnd_Data.RTPwinRate[i])
                {
                    case 0:
                        rtpButtonText[i].text = "超低";
                        break;
                    case 1:
                        rtpButtonText[i].text = "低";
                        break;
                    case 2:
                        rtpButtonText[i].text = "中";
                        break;
                    case 3:
                        rtpButtonText[i].text = "高";
                        break;
                    case 4:
                        rtpButtonText[i].text = "超高";
                        break;
                    default:
                        break;
                }

                #endregion
#else
                #region !Server

                switch (BackEnd_Data.RTPwinRate[i])
                {
                    case 0:
                        rtpButtonText[i].text = "低";
                        break;
                    case 1:
                        rtpButtonText[i].text = "中";
                        break;
                    case 2:
                        rtpButtonText[i].text = "高";
                        break;
                    default:
                        break;
                }

                #endregion
#endif
            }

            //eachRtpButton[eachRtpButton.Length - 1].interactable = false;
        }
    }
}
