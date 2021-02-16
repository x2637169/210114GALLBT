using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Globalization;

public class Mod_UIController : IGameSystem
{
    [SerializeField] GameObject information, bonusChoosePanel, bonusScorePanel, blankButton, bonusMask, backEnd_Login, backEnd_Account, ErrorImage, specialTimes, memberLockImage, QrcodePanal;
    [SerializeField] Image information_Pics, windowTrigger_C, windowTrigger_E, windowReTrigger_C, windowReTrigger_E, windowMostReTrigger_C, windowMostReTrigger_E, autoPlayImage, specialTimesImage, audioSetImage, HistoryImage, MaxHistoryImage;
    [SerializeField] Image[] image_BannerIcon;
    [SerializeField] Text text_RealCredit, text_Bet, text_Win, text_Credit;
    public Text text_BonusPoints;

    [SerializeField] Text text_BannerRight, text_BannerCenter, text_BannerLeft;
    [SerializeField] Text text_bonusTicket, text_BonusIsPlayCount, text_BonusCount;
    [SerializeField] PayTable payTable;
    [SerializeField] Sprite autoPlayOn, autoPlayOff, specialTimesSprite;
    [SerializeField] Sprite[] audioSprite;
    [SerializeField] Button soundButton, infoButton;
    [SerializeField] DenomList denomlist;
    [SerializeField] GameObject Remainder1_0, Remainder1_2, Remainder2_0, Remainder2_2, Remainder3_0, Remainder3_2, Remainder4_0, Remainder4_2;

    [SerializeField] Font normalFont, highlightFont;
    [SerializeField] Image[] highlightFrame;
    [SerializeField] Text[] paytableFontGroup;

    [SerializeField] GameObject coinCamera;
    public Text text_BonusEndScore, text_CHBonusEndScore_Before, text_CHSpecialTimes;
    public bool coinCameraIsActive;
    // Start is called before the first frame update
    void Start()
    {
        //BonusCycleEndGarlicCreate(true);
        UpdateScore();

        //Camera.main.GetComponent<AudioListener>().

    }

    // Update is called once per frame
    void Update()
    {
        text_bonusTicket.text = Mod_Data.BonusTicket.ToString();
        text_BonusIsPlayCount.text = Mod_Data.BonusIsPlayedCount.ToString();

        //text_BonusCount.text = Mod_Data.BonusCount.ToString();
        //if (Mod_Data.autoPlay || !Mod_Data.reelAllStop)
        //{
        //    soundButton.enabled = false;
        //    infoButton.enabled = false;
        //}
        //else
        //{
        //    soundButton.enabled = true;
        //    infoButton.enabled = false;
        //}
    }
    public void AutoPaly()//自動玩按鈕
    {
        Mod_Data.autoPlay = !Mod_Data.autoPlay;
    }
    public void OpenBlankButton(bool open)//開啟Blank按鈕
    {
        blankButton.SetActive(open);
    }
    public void BlankButtonDown()//Blank按鈕
    {
        Mod_Data.BlankClick = true;
    }
    public void OpenBonusChoosePanel()//進bonus選擇面板
    {

        bonusChoosePanel.SetActive(true);

    }
    public enum triggerType  //贏得N場  再次贏得N場  贏得最多場
    {
        Trigger,
        Retrigger,
        MostRetrigger
    }
    public void ShowTriggerPanel(triggerType type, bool open)//開啟贏得N次圖/再次贏得/贏得最多
    {
        if (type == triggerType.Trigger)
        {
            specialTimes.SetActive(true);
            specialTimes.GetComponentInChildren<Text>().text = Mod_Data.BonusSpecialTimes.ToString();
            windowTrigger_C.gameObject.SetActive(open);
            windowTrigger_C.GetComponentInChildren<Text>().text = Mod_Data.BonusCount.ToString();
        }
        else if (type == triggerType.Retrigger)
        {
            windowReTrigger_C.gameObject.SetActive(open);
            windowReTrigger_C.GetComponentInChildren<Text>().text = Mod_Data.getBonusCount.ToString();
        }
        else if (type == triggerType.MostRetrigger)
        {
            windowMostReTrigger_C.gameObject.SetActive(open);
        }
        text_BonusCount.text = Mod_Data.BonusCount.ToString();
    }
    public void OpenBonusScorePanel(bool open)//Bonus贏分顯示
    {
        specialTimes.SetActive(false);
        bonusScorePanel.SetActive(open);
    }

    public void Information_UI(bool open)//開關資訊UI
    {
        InformationPage = 0;
        information.SetActive(open);

        // //Debug.Log(LoadSprite.InfoBackground_C[InformationPage].name);

        if (Mod_Data.language == Mod_Data.languageList.CHT) information_Pics.GetComponent<Image>().sprite = LoadSprite.InfoBackground_C[InformationPage];
        else information_Pics.GetComponent<Image>().sprite = LoadSprite.InfoBackground_E[InformationPage];
    }
    int InformationPage = 0;
    public void Information_Page(int page)//資訊UI換頁
    {
        InformationPage += page;
        if (InformationPage > LoadSprite.InfoBackground_C.Length - 1)
        {
            InformationPage = 0;
        }
        else if (InformationPage < 0)
        {
            InformationPage = LoadSprite.InfoBackground_C.Length - 1;
        }
        if (Mod_Data.language == Mod_Data.languageList.CHT) information_Pics.GetComponent<Image>().sprite = LoadSprite.InfoBackground_C[InformationPage];
        else information_Pics.GetComponent<Image>().sprite = LoadSprite.InfoBackground_E[InformationPage];
    }

    public void BannerLeftHide()//關閉左邊跑馬燈
    {
        for (int i = 0; i < Mod_Data.slotReelNum; i++)
        {
            image_BannerIcon[i].enabled = false;
        }
        text_BannerLeft.enabled = false;
        for (int i = 0; i < paytableFontGroup.Length; i++)
        {
            if (paytableFontGroup[i].font == highlightFont) paytableFontGroup[i].font = normalFont;
        }
        for (int i = 0; i < highlightFrame.Length; i++)
        {
            if (highlightFrame[i].enabled == true) highlightFrame[i].enabled = false;
        }
    }
    public void BannerLeftShow(int showLineIndex, Color textColor)
    {
        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        {

            for (int i = 0; i <= Mod_Data.linegame_WinIconQuantity[showLineIndex]; i++)
            {
                image_BannerIcon[i].sprite = Mod_Data.iconDatabase.GetByID(Mod_Data.linegame_WinIconID[showLineIndex]).Sprite;
                image_BannerIcon[i].enabled = true;
            }
            text_BannerLeft.enabled = true;
            text_BannerLeft.color = textColor;
            if ((Mod_Data.linegame_EachLinePay[showLineIndex] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))) != Mathf.FloorToInt(Mod_Data.linegame_EachLinePay[showLineIndex] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))))
                text_BannerLeft.text = "WINS " + (Mod_Data.linegame_EachLinePay[showLineIndex] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))).ToString("N", CultureInfo.InvariantCulture);
            else
                text_BannerLeft.text = "WINS " + (Mod_Data.linegame_EachLinePay[showLineIndex] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))).ToString("N0");

            for (int i = 0; i < paytableFontGroup.Length; i++)
            {
                if (paytableFontGroup[i].font == highlightFont) paytableFontGroup[i].font = normalFont;
            }
            for (int i = 0; i < highlightFrame.Length; i++)
            {
                if (highlightFrame[i].enabled == true) highlightFrame[i].enabled = false;
            }
            highlightFrame[Mod_Data.linegame_WinIconID[showLineIndex]].enabled = true;
            if (Mod_Data.linegame_WinIconQuantity[showLineIndex] == 2)
            {
                paytableFontGroup[Mod_Data.linegame_WinIconID[showLineIndex] * 4 + 2].font = highlightFont;
                text_BannerLeft.transform.position = new Vector3(750.3f, 880.3f, 0.0f);
            }
            else if (Mod_Data.linegame_WinIconQuantity[showLineIndex] == 3)
            {
                paytableFontGroup[Mod_Data.linegame_WinIconID[showLineIndex] * 4 + 1].font = highlightFont;
                text_BannerLeft.transform.position = new Vector3(833.6f, 880.3f, 0.0f);
            }
            else if (Mod_Data.linegame_WinIconQuantity[showLineIndex] == 4)
            {
                paytableFontGroup[Mod_Data.linegame_WinIconID[showLineIndex] * 4].font = highlightFont;
                text_BannerLeft.transform.position = new Vector3(929, 880.3f, 0.0f);
            }
            else if (Mod_Data.linegame_WinIconQuantity[showLineIndex] == 1)
            {
                paytableFontGroup[Mod_Data.linegame_WinIconID[showLineIndex] * 4 + 3].font = highlightFont;
            }




        }
        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        {
            for (int i = 0; i <= Mod_Data.waygame_WinIconQuantity[showLineIndex]; i++)
            {
                image_BannerIcon[i].sprite = Mod_Data.iconDatabase.GetByID(Mod_Data.waygame_WinIconID[showLineIndex]).Sprite;
                image_BannerIcon[i].enabled = true;
            }
            text_BannerLeft.color = textColor;
            text_BannerLeft.enabled = true;

            //泡泡缸規則中ICON數量大於等於滾輪數量分數2倍計算
            if (Mod_Data.waygame_WinIconQuantity[showLineIndex] >= Mod_Data.slotReelNum - 1)
            {
                text_BannerLeft.text = "WINS " + Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[showLineIndex]][Mod_Data.waygame_WinIconQuantity[showLineIndex]] * Mod_Data.scoreMultiple[showLineIndex] * Mod_Data.odds + " X" + Mod_Data.waygmae_EachIconWinLineNum[showLineIndex];

            }
            else
            {
                text_BannerLeft.text = "WINS " + Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[showLineIndex]][Mod_Data.waygame_WinIconQuantity[showLineIndex]] * Mod_Data.odds + " X" + Mod_Data.waygmae_EachIconWinLineNum[showLineIndex];

            }

            for (int i = 0; i < paytableFontGroup.Length; i++)
            {
                if (paytableFontGroup[i].font == highlightFont) paytableFontGroup[i].font = normalFont;
            }
            for (int i = 0; i < highlightFrame.Length; i++)
            {
                if (highlightFrame[i].enabled == true) highlightFrame[i].enabled = false;
            }
            highlightFrame[Mod_Data.waygame_WinIconID[showLineIndex]].enabled = true;
            if (Mod_Data.waygame_WinIconQuantity[showLineIndex] == 2)
            {
                paytableFontGroup[Mod_Data.waygame_WinIconID[showLineIndex] * 4 + 2].font = highlightFont;
                text_BannerLeft.transform.position = new Vector3(750.3f, 885.3f, 0.0f);
            }
            else if (Mod_Data.waygame_WinIconQuantity[showLineIndex] == 3)
            {
                paytableFontGroup[Mod_Data.waygame_WinIconID[showLineIndex] * 4 + 1].font = highlightFont;
                text_BannerLeft.transform.position = new Vector3(833.6f, 885.3f, 0.0f);
            }
            else if (Mod_Data.waygame_WinIconQuantity[showLineIndex] == 4)
            {
                paytableFontGroup[Mod_Data.waygame_WinIconID[showLineIndex] * 4].font = highlightFont;
                text_BannerLeft.transform.position = new Vector3(929, 885.3f, 0.0f);
            }
        }
    }//開啟左邊跑馬燈 框線動畫中被調用

    public void BannerCenterShow()
    {

    }

    public void BannerRightShowAfterBonus()
    { //bonus結束後右邊跑馬燈
        text_BannerRight.enabled = true;
        text_BannerRight.text = "GAME PAYS " + Mod_Data.Win.ToString("N0");
    }

    public void BannerRightShow(bool isWin, double pay)//開啟右邊跑馬燈
    {
        text_BannerRight.enabled = true;

        if (isWin)
        {
            //if (Mod_Data.Pay * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri)) != Mathf.FloorToInt((float)Mod_Data.Pay * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))))
            //    text_BannerRight.text = "GAME PAYS " + (Mod_Data.Pay * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))).ToString("N", CultureInfo.InvariantCulture);
            //else
            text_BannerRight.text = "GAME PAYS " + pay.ToString("N0");
        }
        else
        {
            text_BannerRight.text = "GAME OVER";
        }

    }
    
    public void BannerRightHide()
    {
        text_BannerRight.enabled = false;
    }//關閉右邊跑馬燈

    public void ShowBonusMask(bool open)
    {
        bonusMask.SetActive(open);
        if (!Mod_Data.BonusSwitch) specialTimesImage.sprite = specialTimesSprite;

        if (open)
        {
            text_BannerLeft.enabled = true;
            text_BannerLeft.color = new Color32(0, 249, 255, 255);

            for (int i = 0; i < paytableFontGroup.Length; i++)
            {
                if (paytableFontGroup[i].font == highlightFont) paytableFontGroup[i].font = normalFont;
            }
            for (int i = 0; i < highlightFrame.Length; i++)
            {
                if (highlightFrame[i].enabled == true) highlightFrame[i].enabled = false;
            }

            highlightFrame[1].enabled = true;
            //if (Mod_Data.getBonusCount == 10)
            //{
            //    for (int i = 0; i <= 2; i++)
            //    {
            //        image_BannerIcon[i].sprite = Mod_Data.iconDatabase.GetByID(1).Sprite;
            //        image_BannerIcon[i].enabled = true;
            //    }
            //    //if ((Mod_Data.iconTablePay[1][2] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))) != Mathf.FloorToInt(Mod_Data.iconTablePay[1][2] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))))
            //    //    text_BannerLeft.text = "WINS " + (Mod_Data.iconTablePay[1][2] *Mod_Data.odds* ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))).ToString("N", CultureInfo.InvariantCulture);
            //    //else
            //        text_BannerLeft.text = "WINS " + (Mod_Data.iconTablePay[1][2] * Mod_Data.odds * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))).ToString("N0");
            //    paytableFontGroup[1 * 4 + 2].font = highlightFont;
            //    text_BannerLeft.transform.position = new Vector3(750.3f, 880.3f, 0.0f);
            //}
            //else if (Mod_Data.getBonusCount == 15)
            //{
            //    for (int i = 0; i <= 3; i++)
            //    {
            //        image_BannerIcon[i].sprite = Mod_Data.iconDatabase.GetByID(1).Sprite;
            //        image_BannerIcon[i].enabled = true;
            //    }
            //    if ((Mod_Data.iconTablePay[1][3] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))) != Mathf.FloorToInt(Mod_Data.iconTablePay[1][3] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))))
            //        text_BannerLeft.text = "WINS " + (Mod_Data.iconTablePay[1][3] * Mod_Data.odds * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))).ToString("N", CultureInfo.InvariantCulture);
            //    else
            //        text_BannerLeft.text = "WINS " + (Mod_Data.iconTablePay[1][3] * Mod_Data.odds * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))).ToString("N0");
            //    paytableFontGroup[1 * 4 + 1].font = highlightFont;
            //    text_BannerLeft.transform.position = new Vector3(833.6f, 880.3f, 0.0f);
            //}
            //else if (Mod_Data.getBonusCount == 25)
            //{
            //    for (int i = 0; i <= 4; i++)
            //    {
            //        image_BannerIcon[i].sprite = Mod_Data.iconDatabase.GetByID(1).Sprite;
            //        image_BannerIcon[i].enabled = true;
            //    }
            //    if ((Mod_Data.iconTablePay[1][4] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))) != Mathf.FloorToInt(Mod_Data.iconTablePay[1][4] * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))))
            //        text_BannerLeft.text = "WINS " + (Mod_Data.iconTablePay[1][4] * Mod_Data.odds * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))).ToString("N", CultureInfo.InvariantCulture);
            //    else
            //        text_BannerLeft.text = "WINS " + (Mod_Data.iconTablePay[1][4] * Mod_Data.odds * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))).ToString("N0");
            //    paytableFontGroup[1 * 4].font = highlightFont;
            //    text_BannerLeft.transform.position = new Vector3(929, 880.3f, 0.0f);
            //}
            //else if (Mod_Data.linegame_WinIconQuantity[showLineIndex] == 1)
            //{
            //    paytableFontGroup[1 * 4 + 3].font = highlightFont;
            //}

        }
        else
        {
            for (int i = 0; i < Mod_Data.slotReelNum; i++)
            {
                image_BannerIcon[i].enabled = false;
            }
            text_BannerLeft.enabled = false;
            for (int i = 0; i < paytableFontGroup.Length; i++)
            {
                if (paytableFontGroup[i].font == highlightFont) paytableFontGroup[i].font = normalFont;
            }
            for (int i = 0; i < highlightFrame.Length; i++)
            {
                if (highlightFrame[i].enabled == true) highlightFrame[i].enabled = false;
            }
        }
    }

    public void UpdateScore()//更新分數
    {
        text_BonusPoints.text = Mod_Data.bonusPoints <= 0 ? null : Mod_Data.bonusPoints.ToString("N", CultureInfo.InvariantCulture);
        text_Credit.text = Mathf.CeilToInt((float)(Mod_Data.credit / Mod_Data.Denom)).ToString("N0");
        text_RealCredit.text = Mod_Data.credit.ToString("N", CultureInfo.InvariantCulture);
        //if(Mod_Data.betLowCreditShowOnce)
        //{
        //    text_Bet.text = System.Math.Round((double)(Mod_Data.credit / Mod_Data.Denom)).ToString("N0");
        //}
        //else 
        text_Bet.text = (Mod_Data.Bet * Mod_Data.odds).ToString();
        if (!Mod_Data.BonusSwitch)
        {
            text_Win.text = "0";
            text_Win.enabled = false;
        }
        if (Mod_Data.StartNotNormal)
        {
            text_Win.text = Mod_Data.Win.ToString();
            text_Win.enabled = true;
        }
        payTable.PayTableUpData();
    }

    public void BonusOutSetWinScoreToZero()
    {
        text_Win.text = "";
    }

    float frameTime;
    float totalScore;
    float rollScoreTotalTime;
    float rollScore;
    public void StartFastRollScore(double startScore, double endScore)//開始跑分(起始分,結束分)
    {
        frameTime = Time.deltaTime;
        totalScore = (float)(endScore - startScore);
        rollScoreTotalTime = totalScore * frameTime;
        if (totalScore >= (5 / frameTime)) rollScore = rollScoreTotalTime / 5;
        else rollScore = 1;
        //Debug.Log(5 / frameTime);
        if (startScore == endScore)
        {
            StopAllCoroutines();
            text_Win.text = endScore.ToString();
            Mod_Data.runScore = false;
        }
        else
        {
            if (endScore > 0)
            {
                text_Win.enabled = true;
                StartCoroutine(RollFastScore(startScore, endScore, rollScore, frameTime));

            }
        }
    }
    public void StartRollScore(double startScore, double endScore)//開始跑分(起始分,結束分)
    {

        if (startScore >= endScore)
        {
            StopAllCoroutines();
            text_Win.text = endScore.ToString();
            Mod_Data.runScore = false;
        }
        else
        {
            if (endScore > 0)
            {
                text_Win.enabled = true;
                StartCoroutine(RollScore(startScore, endScore));
            }
        }

    }
    public void OpenBackEnd(string backEndPanal)
    {
        switch (backEndPanal)
        {
            case "Account":
                backEnd_Account.SetActive(true);
                break;
            case "Login":
                backEnd_Login.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void BonusCycleEndGarlicCreate(bool StartCreate)
    {
        if (StartCreate)
        {
            // InvokeRepeating("GarlicCreate", 0.01f, 0.02f);
            coinCameraIsActive = true;
            coinCamera.SetActive(true);
        }
        else
        {
            // CancelInvoke("GarlicCreate");
            coinCameraIsActive = false;
            coinCamera.SetActive(false);
        }
    }

    public void HelpBtnCoinCamera(bool open)
    {
        if (open)
        {
            if (!Mod_Data.autoPlay) coinCamera.SetActive(false);
        }
        else
        {
            if (coinCameraIsActive) coinCamera.SetActive(true);
        }
    }


    public GameObject Garlic;
    public Vector3 GarlicPos;
    public void GarlicCreate()
    {
        Instantiate(Garlic, GarlicPos, Quaternion.Euler(UnityEngine.Random.Range(-180f, -90f), UnityEngine.Random.Range(-90f, 0f), UnityEngine.Random.Range(-180f, 0f)));
    }

    IEnumerator RollScore(double rollScore, double endScore)//跑分
    {
        yield return new WaitForSeconds(0.005f);

        if (rollScore < endScore)
        {
            Mod_Data.runScore = true;
            rollScore++;
            text_Win.text = rollScore.ToString();
            if (rollScore >= endScore) Mod_Data.runScore = false;
            StartCoroutine(RollScore(rollScore, endScore));
        }
    }

    IEnumerator RollFastScore(double rollScore, double endScore, double frameScore, float frameTime)//跑分
    {
        while (rollScore < endScore)
        {
            Mod_Data.runScore = true;
            rollScore += frameScore;
            if (rollScore >= endScore) rollScore = endScore;
            text_Win.text = rollScore.ToString("F0");

            yield return null;
        }
        Mod_Data.runScore = false;
    }

    public void closeDenom()
    {
        if (denomlist.ListOpen) denomlist.CloseDenomList();
    }
    float volumeset = 1;
    public void SetVolume()
    {
        if (volumeset == 0)
        {
            volumeset = 0.3f;
            audioSetImage.sprite = audioSprite[1];
        }
        else if (volumeset == 0.3f)
        {
            volumeset = 0.6f;
            audioSetImage.sprite = audioSprite[2];
        }
        else if (volumeset == 0.6f)
        {
            volumeset = 1f;
            audioSetImage.sprite = audioSprite[3];
        }
        else if (volumeset == 1)
        {
            volumeset = 0f;
            audioSetImage.sprite = audioSprite[0];
        }
        AudioListener.volume = volumeset;
    }

    public void ErrorImageShow(string errortext, bool open)
    {
        ErrorImage.SetActive(open);
        ErrorImage.GetComponentInChildren<Text>().text = errortext;
    }
    public void RemainderUnShown()
    {
        Remainder1_0.SetActive(false);
        Remainder1_2.SetActive(false);
        Remainder2_0.SetActive(false);
        Remainder2_2.SetActive(false);
        Remainder3_0.SetActive(false);
        Remainder3_2.SetActive(false);
        Remainder4_0.SetActive(false);
        Remainder4_2.SetActive(false);
    }
    public void RemainderShow()
    {
        //顯示黑格子
        //Debug.Log("Black");
        //Debug.Log(Mod_Data.Bet);
        //特例排除  bonus和scatter
        switch (Mod_Data.Bet)
        {
            case 1:
                for (int i = 1; i < 5; i++)
                {
                    if (Mod_Data.ReelMath[i, 0] != 1 && Mod_Data.ReelMath[i, 0] != 2)
                    {
                        Mod_Data.ReelMath[i, 0] = 99;
                    }
                    if (Mod_Data.ReelMath[i, 2] != 1 && Mod_Data.ReelMath[i, 2] != 2)
                    {
                        Mod_Data.ReelMath[i, 2] = 99;
                    }
                }
                Remainder1_0.SetActive(true);
                Remainder1_2.SetActive(true);
                Remainder2_0.SetActive(true);
                Remainder2_2.SetActive(true);
                Remainder3_0.SetActive(true);
                Remainder3_2.SetActive(true);
                Remainder4_0.SetActive(true);
                Remainder4_2.SetActive(true);
                break;
            case 3:
                for (int i = 2; i < 5; i++)
                {
                    if (Mod_Data.ReelMath[i, 0] != 1 && Mod_Data.ReelMath[i, 0] != 2)
                    {
                        Mod_Data.ReelMath[i, 0] = 99;
                    }
                    if (Mod_Data.ReelMath[i, 2] != 1 && Mod_Data.ReelMath[i, 2] != 2)
                    {
                        Mod_Data.ReelMath[i, 2] = 99;
                    }
                }
                Remainder1_0.SetActive(false);
                Remainder1_2.SetActive(false);
                Remainder2_0.SetActive(true);
                Remainder2_2.SetActive(true);
                Remainder3_0.SetActive(true);
                Remainder3_2.SetActive(true);
                Remainder4_0.SetActive(true);
                Remainder4_2.SetActive(true);
                break;
            case 9:
                for (int i = 3; i < 5; i++)
                {
                    if (Mod_Data.ReelMath[i, 0] != 1 && Mod_Data.ReelMath[i, 0] != 2)
                    {
                        Mod_Data.ReelMath[i, 0] = 99;
                    }
                    if (Mod_Data.ReelMath[i, 2] != 1 && Mod_Data.ReelMath[i, 2] != 2)
                    {
                        Mod_Data.ReelMath[i, 2] = 99;
                    }
                }
                Remainder1_0.SetActive(false);
                Remainder1_2.SetActive(false);
                Remainder2_0.SetActive(false);
                Remainder2_2.SetActive(false);
                Remainder3_0.SetActive(true);
                Remainder3_2.SetActive(true);
                Remainder4_0.SetActive(true);
                Remainder4_2.SetActive(true);
                break;
            case 15:
                for (int i = 4; i < 5; i++)
                {
                    if (Mod_Data.ReelMath[i, 0] != 1 && Mod_Data.ReelMath[i, 0] != 2)
                    {
                        Mod_Data.ReelMath[i, 0] = 99;
                    }
                    if (Mod_Data.ReelMath[i, 2] != 1 && Mod_Data.ReelMath[i, 2] != 2)
                    {
                        Mod_Data.ReelMath[i, 2] = 99;
                    }
                }
                Remainder1_0.SetActive(false);
                Remainder1_2.SetActive(false);
                Remainder2_0.SetActive(false);
                Remainder2_2.SetActive(false);
                Remainder3_0.SetActive(false);
                Remainder3_2.SetActive(false);
                Remainder4_0.SetActive(true);
                Remainder4_2.SetActive(true);
                break;
        }
        Mod_DataInit.IconPaysLoad();
    }
    public void MemberLockImage(bool open)
    {
        memberLockImage.SetActive(open);
        ////Debug.Log(open);
    }

    public void OpenQrcodePanal()
    {
        QrcodePanal.SetActive(true);
    }

}
