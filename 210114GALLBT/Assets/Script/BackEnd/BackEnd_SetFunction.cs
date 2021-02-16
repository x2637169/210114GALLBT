using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BackEnd_SetFunction : MonoBehaviour
{

    [SerializeField] Text mostOdds, mostCredit, mostWin;
    [SerializeField] BackEnd_Data backEnd_Data;
    [SerializeField] GameObject denomSettingPannal;
    [SerializeField] Image[] denomSettingImg;
    int[] maxCreditRange = new int[] { 5000000, 10000000 };
    int[] maxWinRange = new int[] { 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000 };
    public static bool[] denomOpenArray = new bool[9];
    // Start is called before the first frame update

     
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSetPannal()
    {
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxOdds, false);//讀取maxodds
        mostOdds.text = BackEnd_Data.maxOdds.ToString();
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxCredit, false);//讀取maxCredit
        mostCredit.text = BackEnd_Data.maxCredit.ToString("N0");
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxWin, false);//讀取maxWin
        mostWin.text = BackEnd_Data.maxWin.ToString("N0");
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.denomArraySelect, false);//讀取maxWin
    }
    public void SetMostOdds()//設定odds
    {

        BackEnd_Data.maxOdds++;
        if (BackEnd_Data.maxOdds > 25) BackEnd_Data.maxOdds = 1;
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxOdds, true);
        Mod_Data.maxOdds = BackEnd_Data.maxOdds;
        mostOdds.text = BackEnd_Data.maxOdds.ToString();

    }
    public void SetMostCredit()
    {
        if(BackEnd_Data.maxCredit== maxCreditRange[maxCreditRange.Length - 1])
        {
            BackEnd_Data.maxCredit = maxCreditRange[0];
        }
        else
        {
            for (int i = 0; i < maxCreditRange.Length; i++)
            {
                if (maxCreditRange[i] > BackEnd_Data.maxCredit)
                {
                    BackEnd_Data.maxCredit = maxCreditRange[i];
                    break;
                }
            }
        }
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxCredit, true);
        Mod_Data.maxCredit = BackEnd_Data.maxCredit;
        mostCredit.text = BackEnd_Data.maxCredit.ToString("N0");
    }
    public void SetMostWin()
    {
        if (BackEnd_Data.maxWin == maxWinRange[maxWinRange.Length - 1])
        {
            BackEnd_Data.maxWin = maxWinRange[0];
        }
        else
        {
            for (int i = 0; i < maxWinRange.Length; i++)
            {
                if (maxWinRange[i] > BackEnd_Data.maxWin)
                {
                    BackEnd_Data.maxWin = maxWinRange[i];
                    break;
                }
            }
        }
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.maxWin, true);
        Mod_Data.maxWin = BackEnd_Data.maxWin;
        mostWin.text = BackEnd_Data.maxWin.ToString("N0");
    }

    public void SetDenom()
    {
        denomSettingPannal.SetActive(!denomSettingPannal.activeInHierarchy);
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.denomArraySelect, false);
        for (int denomIndex = 0; denomIndex < BackEnd_Data.denomArray.Length; denomIndex++)
        {
            if (BackEnd_Data.denomArray[denomIndex]) denomSettingImg[denomIndex].color = new Color32(46, 255, 0, 255);
            else denomSettingImg[denomIndex].color = new Color32(255, 255, 255, 255);
        }
    }

    public void DenomSettingButton(int denomIndex)
    {
        BackEnd_Data.denomArray[denomIndex] = !BackEnd_Data.denomArray[denomIndex];
        if(BackEnd_Data.denomArray[denomIndex])denomSettingImg[denomIndex].color= new Color32(46, 255, 0, 255);
        else denomSettingImg[denomIndex].color = new Color32(255, 255, 255, 255);
    }

    public void ComfirmDenomSetting()
    {
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.denomArraySelect, true);
        for (int i = 0; i < denomOpenArray.Length; i++)
        {
            Mod_Data.denomOpenArray[i] = BackEnd_Data.denomArray[i];
        }
        //RTPChoose();
        denomSettingPannal.SetActive(false);
    }
    int[] rtpArray;
    int nowRtp;
    bool rtpAll;
    public void RTPChoose() {

        GameObject.Find("BackEndManager").GetComponent<NewSramManager>().LoadRTPSetting(out rtpArray,out rtpAll);


       
            if (Mod_Data.Denom == 0.01) { nowRtp = rtpArray[8]; }
            else if (Mod_Data.Denom == 0.02) { nowRtp = rtpArray[7]; }
            else if (Mod_Data.Denom == 0.025) { nowRtp = rtpArray[6]; }
            else if (Mod_Data.Denom == 0.05) { nowRtp = rtpArray[5]; }
            else if (Mod_Data.Denom == 0.1) { nowRtp = rtpArray[4]; }
            else if (Mod_Data.Denom == 0.25) { nowRtp = rtpArray[3]; }
            else if (Mod_Data.Denom == 0.5) { nowRtp = rtpArray[2]; }
            else if (Mod_Data.Denom == 1) { nowRtp = rtpArray[1]; }
            else if (Mod_Data.Denom == 2.5) { nowRtp = rtpArray[0]; }
        
        Mod_Data.RTPsetting = nowRtp;
        GameObject.Find("GameController").GetComponent<Mod_MathController>().ChangeMathFile(nowRtp);
    }

}
