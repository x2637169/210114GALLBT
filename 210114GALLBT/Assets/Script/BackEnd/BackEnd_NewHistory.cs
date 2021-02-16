using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System;
using static NewSramManager;

public class BackEnd_NewHistory : MonoBehaviour
{
    // Start is called before the first frame update

    public int historytype = 0;
    [SerializeField]NewSramManager newSramManager;
    [SerializeField] Mod_GameMath m_GameMath;
    [SerializeField] Mod_Animation m_Animation;
    HistoryData historyData = new HistoryData();
    [SerializeField] UIItemDatabase BaseiconDatabase, BonusiconDatabase;
    [SerializeField] GameObject[] EndReel1, EndReel2, EndReel3, EndReel4, EndReel5;
    [SerializeField] Image[] reelFrame_0, reelFrame_1, reelFrame_2, reelFrame_3, reelFrame_4, LineImg;
    JArray ReelMath;
    int ReelRNG, bottomIndex;
    int historyDataIndex = 0, latestGameNumber = 0;
    int SaveWayID = 0;
    public Image[][] frameImages;

    //----------------UI
    [SerializeField] Image backGround, reelOutLineBackGround, reelBackGround,bonusLightImg, bonusSlashImg,autobtn,helpbtn,soundbtn;//基本UI
    [SerializeField] Image dkHead, specialTimesImage;//驢子專屬
    [SerializeField] Image image_Reel1, image_Reel2, image_Reel3, image_Reel4, image_Reel5;
    [SerializeField] Text creditText, realCreditText, betText, winText, denomText,bonusConutText,bonusIsPlayedCountText;//遊戲畫面文字
    [SerializeField] Text totalGameCount,nowGameCount,totalWinText, nowCreditText, timeText, openScoreText, clearScoreText;//歷史紀錄文字
    [SerializeField] GameObject Remainder1_0, Remainder1_2, Remainder2_0, Remainder2_2, Remainder3_0, Remainder3_2, Remainder4_0, Remainder4_2;

    [SerializeField] Sprite[] specialTimesSprite,dkHeads;
    [SerializeField] GameObject FishSpecialTime;

    [SerializeField] Image image_Bet1, image_Bet2, image_Bet5, image_Bet10, image_Bet25;
    [SerializeField] Text text_Bet2, text_Bet3, text_Bet6, text_Bet11;
    [SerializeField] Sprite sprite_BaseBet1, sprite_BaseBet2, sprite_BaseBet5, sprite_BaseBet10, sprite_BaseBet25, sprite_BonusBet1, sprite_BonusBet2, sprite_BonusBet5, sprite_BonusBet10, sprite_BonusBet25;
    [SerializeField] Sprite sprite_BaseReel1, sprite_BaseReel2, sprite_BaseReel3, sprite_BaseReel4, sprite_BaseReel5, sprite_BonusReel1, sprite_BonusReel2, sprite_BonusReel3, sprite_BonusReel4, sprite_BonusReel5;
    [SerializeField] Font basefont, bonusfont;
    void Start()
    {
        for(int i=0;i< LineImg.Length; i++)
        {
            LineImg[i].sprite = m_Animation.lineSprite[i];
            LineImg[i].color = m_Animation.lineColors[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    double[] WinScoreArray = new double[10];
        int[] sortArray=new int[10];
    int[] numberArray = new int[10];
    int tmpSort = 10;
    private void OnEnable()
    {

        MaxCount = 0;
        WinScoreArray = new double[10];
        sortArray = new int[10];
        numberArray = new int[10];
        newSramManager.LoadGameHistoryLog(historytype, out latestGameNumber);
        historyDataIndex = latestGameNumber;

        frameImages = new Image[Mod_Data.slotReelNum][];
        frameImages[0] = reelFrame_0;
        frameImages[1] = reelFrame_1;
        frameImages[2] = reelFrame_2;
        frameImages[3] = reelFrame_3;
        frameImages[4] = reelFrame_4;


        if (historytype == 0){
            newSramManager.LoadHistory(historyDataIndex, out historyData);
        }
        else if (historytype == 1)
        {
            for (int i = 1; i <= latestGameNumber; i++)
            {
                newSramManager.LoadMaxGameHistory(i, out historyData);
                WinScoreArray[i - 1] = historyData.Win * historyData.Demon;
                numberArray[i - 1] = i;
            }
            for (int i = 0; i < latestGameNumber; i++)
            {
                tmpSort = latestGameNumber;
                for (int j = 0; j < latestGameNumber; j++)
                {
                    if (i != j)
                    {
                        if(WinScoreArray[i] > WinScoreArray[j])
                        {
                            tmpSort--;
                        }
                        if (WinScoreArray[i] == WinScoreArray[j]&&i>j)
                        {
                            tmpSort--;
                        }
                    }
                }
                sortArray[tmpSort-1] = numberArray[i];
            }

            newSramManager.LoadMaxGameHistory(sortArray[0], out historyData);
            //Debug.Log("sortArray[0]"+sortArray[0]);
        }
        else if (historytype == 2)
        {
            for (int i = 1; i <= latestGameNumber; i++)
            {
                newSramManager.LoadMaxGameHistorySecond(i, out historyData);
                WinScoreArray[i - 1] = historyData.Win;
                numberArray[i - 1] = historyData.Number;
            }
            for (int i = 0; i < latestGameNumber; i++)
            {
                tmpSort = latestGameNumber;
                for (int j = 0; j < latestGameNumber; j++)
                {
                    if (i != j)
                    {
                        if (WinScoreArray[i] > WinScoreArray[j])
                        {
                            tmpSort--;
                        }
                        if (WinScoreArray[i] == WinScoreArray[j] && i > j)
                        {
                            tmpSort--;
                        }
                    }
                }
                sortArray[tmpSort - 1] = numberArray[i];
            }
            newSramManager.LoadMaxGameHistorySecond(sortArray[0], out historyData);
        }
       
        SetHistoryUI(historyData);
    }

    void SetHistoryUI(HistoryData historyData)  //控制所有UI
    {
        if (historyData.Bonus)
        {
            backGround.sprite = LoadSprite.DownBonusBackgroundPicture;
            reelOutLineBackGround.sprite = LoadSprite.BonusReelBack;
            reelBackGround.sprite = LoadSprite.iconBonusBackgroundSprite;
            bonusLightImg.enabled = true;
            //---驢子專用
            //dkHead.enabled = true;
            //if (historyData.SpecialTime == 8) dkHead.sprite = dkHeads[1];
            //else dkHead.sprite = dkHeads[0];
            //specialTimesImage.enabled = true;
            //if(specialTimesSprite.Length>0) specialTimesImage.sprite = specialTimesSprite[historyData.SpecialTime - 1];
            //Fish用
            FishSpecialTime.SetActive(true);
            FishSpecialTime.GetComponentInChildren<Text>().text = historyData.SpecialTime.ToString();
            bonusConutText.enabled = true;
            bonusConutText.text = historyData.BonusCount.ToString();
            bonusIsPlayedCountText.enabled = true;
            bonusIsPlayedCountText.text = historyData.BonusIsPlayedCount.ToString();
            bonusSlashImg.enabled = true;
            autobtn.enabled = false;
            helpbtn.enabled = false;
            soundbtn.enabled = false;
        }
        else
        {
            backGround.sprite = LoadSprite.DownBaseBackgroundPicture;
            reelOutLineBackGround.sprite = LoadSprite.BaseReelBack;
            reelBackGround.sprite = LoadSprite.iconBaseBackgroundSprite;
            bonusLightImg.enabled = false;
            //---驢子專用
            //dkHead.enabled = false;
            //specialTimesImage.enabled = false;
            //Fish用
            FishSpecialTime.SetActive(false);
            bonusConutText.enabled = false;
            bonusIsPlayedCountText.enabled = false;
            bonusSlashImg.enabled = false;
            autobtn.enabled = true;
            helpbtn.enabled = true;
            soundbtn.enabled = true;
        }
        //Debug.Log("RNG"+historyData.RNG[0]+"," + historyData.RNG[1] + "," + historyData.RNG[2] + "," + historyData.RNG[3] + "," + historyData.RNG[4] + "," + "historyData.Credit" + (float)(historyData.Credit / ((float)historyData.Demon / 1000f)) + "historyData.Demon" + historyData.Demon + "historyData.Bet" + historyData.Bet + " historyData.Odds" + historyData.Odds);

        if (historyData.Bonus)
        {
            creditText.text = Mathf.CeilToInt((float)((historyData.Credit) / ((float)historyData.Demon / 1000f))).ToString("N0");
            realCreditText.text = (historyData.Credit).ToString("N", CultureInfo.InvariantCulture);
        }
        else
        {
            creditText.text = Mathf.CeilToInt((float)((historyData.Credit - (historyData.Win * ((float)historyData.Demon / 1000f))) / ((float)historyData.Demon / 1000f))).ToString("N0");
            realCreditText.text = (historyData.Credit - (historyData.Win * ((float)historyData.Demon / 1000f))).ToString("N", CultureInfo.InvariantCulture);
        }
        denomText.text = (((float)historyData.Demon / 1000f)).ToString();
        betText.text = (historyData.Bet * historyData.Odds).ToString();
        winText.text = historyData.Win.ToString();
        totalWinText.text = "0";//historyData.Win.ToString();
        nowCreditText.text = historyData.Credit.ToString("N", CultureInfo.InvariantCulture);
        totalGameCount.text = latestGameNumber.ToString();

        if (historytype == 0)
        {
            nowGameCount.text = (latestGameNumber - historyDataIndex + 1).ToString();
        }
        else if (historytype == 1)
        {
            nowGameCount.text = (MaxCount + 1).ToString();
        }
        else if (historytype == 2)
        {
            nowGameCount.text = (MaxCount + 1).ToString();
        }
       


        timeText.text = historyData.Time.ToString("yyyy/MM/dd HH:mm:ss");
        openScoreText.text = historyData.OpenPoint.ToString();
        clearScoreText.text = historyData.ClearPoint.ToString();

        ChangeOddShow(historyData.Odds,historyData.Bonus);
        ChangeReelShow(historyData.Bet, historyData.Bonus);
        startLoadReel(historyData.RNG, historyData.Bonus);
        RemainderUnShown();
        if (historyData.Bet < 25) {
            RemainderShow();
        }
        else {
            Mod_DataInit.IconPaysLoad();
        }
        GameMath();
        SaveWayID = 0;
        ShowNextLine();
    }
    int MaxCount = 0;
    public void Btn_ChangeHistoryData(int count)
    {
       
        if (historytype == 0)
        {
            if (historyDataIndex + count <= latestGameNumber && historyDataIndex + count > 0)
            {
                historyDataIndex += count;
            }
            newSramManager.LoadHistory(historyDataIndex, out historyData);
        }
        else if (historytype == 1)
        {
            if(MaxCount + count < latestGameNumber && MaxCount + count >= 0)
            {
                MaxCount += count;
            }
            newSramManager.LoadMaxGameHistory(sortArray[MaxCount], out historyData);
        }
        else if (historytype == 2)
        {
            if (MaxCount + count < latestGameNumber && MaxCount + count >= 0)
            {
                MaxCount += count;
            }
            newSramManager.LoadMaxGameHistorySecond(sortArray[MaxCount], out historyData);
        }
        //Debug.Log("sortArray[MaxCount]" + sortArray[MaxCount]);
        SetHistoryUI(historyData);
    }
    void startLoadReel(int[] beforeBonusRNG, bool isBonus)
    {
        Mod_Data.ChangeMathFile(historyData.RTP);
        switch (isBonus)
        {
            case true:
                if (Mod_Data.BonusType == 0) Mod_Data.JsonObj = Mod_Data.JsonObjBonus[0];
                else if (Mod_Data. BonusType == 1) Mod_Data.JsonObj = Mod_Data.JsonObjBonus[1];
                break;
            case false:
                Mod_Data.JsonObj = Mod_Data.JsonObjBase;
                break;
        }
        ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_1"]["Reel"];
        ReelRNG = beforeBonusRNG[0];
        bottomIndex = EndReel1.Length - 1;
        for (int k = 0; k < Mod_Data.slotRowNum; k++)
        {
            if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
            Mod_Data.ReelMath[0, k] = int.Parse(ReelMath[ReelRNG].ToString());
            //Debug.Log(Mod_Data.ReelMath[0, k]);
            if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
            if (isBonus)
            {
                EndReel1[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BonusiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }
            else
            {
                EndReel1[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }



            ReelRNG -= 1;
            bottomIndex -= 1;
        }

        ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_2"]["Reel"];
        ReelRNG = beforeBonusRNG[1];
        bottomIndex = EndReel1.Length - 1;
        for (int k = 0; k < Mod_Data.slotRowNum; k++)
        {
            if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
            Mod_Data.ReelMath[1, k] = int.Parse(ReelMath[ReelRNG].ToString());
            if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
            if (isBonus)
            {
                EndReel2[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BonusiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }
            else
            {
                EndReel2[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }

            ReelRNG -= 1;
            bottomIndex -= 1;
        }

        ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_3"]["Reel"];
        ReelRNG = beforeBonusRNG[2];
        bottomIndex = EndReel1.Length - 1;
        for (int k = 0; k < Mod_Data.slotRowNum; k++)
        {
            if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
            Mod_Data.ReelMath[2, k] = int.Parse(ReelMath[ReelRNG].ToString());
            if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
            if (isBonus)
            {
                EndReel3[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BonusiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }
            else
            {
                EndReel3[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }


            ReelRNG -= 1;
            bottomIndex -= 1;
        }

        ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_4"]["Reel"];
        ReelRNG = beforeBonusRNG[3];
        bottomIndex = EndReel1.Length - 1;
        for (int k = 0; k < Mod_Data.slotRowNum; k++)
        {
            if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
            Mod_Data.ReelMath[3, k] = int.Parse(ReelMath[ReelRNG].ToString());
            if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
            if (isBonus)
            {
                EndReel4[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BonusiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }
            else
            {
                EndReel4[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }

            ReelRNG -= 1;
            bottomIndex -= 1;
        }

        ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_5"]["Reel"];
        ReelRNG = beforeBonusRNG[4];
        bottomIndex = EndReel1.Length - 1;
        for (int k = 0; k < Mod_Data.slotRowNum; k++)
        {
            if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
            Mod_Data.ReelMath[4, k] = int.Parse(ReelMath[ReelRNG].ToString());
            if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
            if (isBonus)
            {
                EndReel5[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BonusiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }
            else
            {
                EndReel5[bottomIndex].gameObject.GetComponent<Image>().sprite =
            BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
            }
            ReelRNG -= 1;
            bottomIndex -= 1;
        }

    }

    void ChangeOddShow(double tempOdd, bool isBonus) {
        if (isBonus) {
            image_Bet1.sprite = sprite_BonusBet1;
            image_Bet2.sprite = sprite_BonusBet2;
            image_Bet5.sprite = sprite_BonusBet5;
            image_Bet10.sprite = sprite_BonusBet10;
            image_Bet25.sprite = sprite_BonusBet25;
            text_Bet3.font = bonusfont;
            text_Bet6.font = bonusfont;
            text_Bet11.font = bonusfont;
        }
        else {
            image_Bet1.sprite = sprite_BaseBet1;
            image_Bet2.sprite = sprite_BaseBet2;
            image_Bet5.sprite = sprite_BaseBet5;
            image_Bet10.sprite = sprite_BaseBet10;
            image_Bet25.sprite = sprite_BaseBet25;
            text_Bet3.font = basefont;
            text_Bet6.font = basefont;
            text_Bet11.font = basefont;
        }
        if (tempOdd == 1) {
            image_Bet1.enabled = true;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
        }
        else if (tempOdd == 2) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = true;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
            //text_Bet2.text = tempOdd.ToString();
        }
        else if (tempOdd == 3 || tempOdd == 4) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = true;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
            text_Bet3.text = tempOdd.ToString();
        }
        else if (tempOdd == 5) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = true;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
            //text_Bet3.text = tempOdd.ToString();
        }
        else if (tempOdd >= 6 && tempOdd <= 9) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = true;
            text_Bet11.enabled = false;
            text_Bet6.text = tempOdd.ToString();
        }
        else if (tempOdd == 10) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = true;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
            //text_Bet6.text = tempOdd.ToString();
        }
        else if (tempOdd >= 11 && tempOdd <= 24) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = true;
            text_Bet11.text = tempOdd.ToString();
        }
        else if (tempOdd == 25) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = true;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
        }
    }
    //左側"滾輪"顯示
    void ChangeReelShow(int tempBet, bool isBonus) {
        if (isBonus) {
            image_Reel1.sprite = sprite_BonusReel1;
            image_Reel2.sprite = sprite_BonusReel2;
            image_Reel3.sprite = sprite_BonusReel3;
            image_Reel4.sprite = sprite_BonusReel4;
            image_Reel5.sprite = sprite_BonusReel5;
        }
        else {
            image_Reel1.sprite = sprite_BaseReel1;
            image_Reel2.sprite = sprite_BaseReel2;
            image_Reel3.sprite = sprite_BaseReel3;
            image_Reel4.sprite = sprite_BaseReel4;
            image_Reel5.sprite = sprite_BaseReel5;
        }

        if (tempBet >= 25) {
            image_Reel1.enabled = false;
            image_Reel2.enabled = false;
            image_Reel3.enabled = false;
            image_Reel4.enabled = false;
            image_Reel5.enabled = true;
        }
        else if (tempBet == 15) {
            image_Reel1.enabled = false;
            image_Reel2.enabled = false;
            image_Reel3.enabled = false;
            image_Reel4.enabled = true;
            image_Reel5.enabled = false;
        }
        else if (tempBet == 9) {
            image_Reel1.enabled = false;
            image_Reel2.enabled = false;
            image_Reel3.enabled = true;
            image_Reel4.enabled = false;
            image_Reel5.enabled = false;
        }
        else if (tempBet == 3) {
            image_Reel1.enabled = false;
            image_Reel2.enabled = true;
            image_Reel3.enabled = false;
            image_Reel4.enabled = false;
            image_Reel5.enabled = false;
        }
        else if (tempBet == 1) {
            image_Reel1.enabled = true;
            image_Reel2.enabled = false;
            image_Reel3.enabled = false;
            image_Reel4.enabled = false;
            image_Reel5.enabled = false;
        }
    }
    void GameMath()
    {
        GameMathClear();

        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        {
            m_GameMath.WayGame_CountMath();
        }
        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        {
            m_GameMath.LineGame_CountMath();
        }
    }
    public void GameMathClear()
    {
        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        {
            m_GameMath.WayGame_MathClear();
        }
        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        {
            m_GameMath.LineGame_MathClear();
        }

    }

    //歷史紀錄的餘分
    public void RemainderUnShown() {
        Remainder1_0.SetActive(false);
        Remainder1_2.SetActive(false);
        Remainder2_0.SetActive(false);
        Remainder2_2.SetActive(false);
        Remainder3_0.SetActive(false);
        Remainder3_2.SetActive(false);
        Remainder4_0.SetActive(false);
        Remainder4_2.SetActive(false);
    }
    public void RemainderShow() {
        //顯示黑格子
        //Debug.Log("Black");
        //Debug.Log(historyData.Bet);
        //特例排除  bonus和scatter
        switch (historyData.Bet) {
            case 1:
                for (int i = 1; i < 5; i++) {
                    if (Mod_Data.ReelMath[i, 0] != 1 && Mod_Data.ReelMath[i, 0] != 2) {
                        Mod_Data.ReelMath[i, 0] = 99;
                    }
                    if (Mod_Data.ReelMath[i, 2] != 1 && Mod_Data.ReelMath[i, 2] != 2) {
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
                for (int i = 2; i < 5; i++) {
                    if (Mod_Data.ReelMath[i, 0] != 1 && Mod_Data.ReelMath[i, 0] != 2) {
                        Mod_Data.ReelMath[i, 0] = 99;
                    }
                    if (Mod_Data.ReelMath[i, 2] != 1 && Mod_Data.ReelMath[i, 2] != 2) {
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
                for (int i = 3; i < 5; i++) {
                    if (Mod_Data.ReelMath[i, 0] != 1 && Mod_Data.ReelMath[i, 0] != 2) {
                        Mod_Data.ReelMath[i, 0] = 99;
                    }
                    if (Mod_Data.ReelMath[i, 2] != 1 && Mod_Data.ReelMath[i, 2] != 2) {
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
                for (int i = 4; i < 5; i++) {
                    if (Mod_Data.ReelMath[i, 0] != 1 && Mod_Data.ReelMath[i, 0] != 2) {
                        Mod_Data.ReelMath[i, 0] = 99;
                    }
                    if (Mod_Data.ReelMath[i, 2] != 1 && Mod_Data.ReelMath[i, 2] != 2) {
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

        Mod_Data.JsonReload();
        JArray IconPays;
        for (int i = 0; i < Mod_Data.iconDatabase.items.Length; i++) {
            IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon" + (i + 1)]["IconPays"];
            //特殊處理 當餘分降至最低會變動bet時 bonus與scatter分數會變動
            if (i == 1 || i == 2) {
                if (historyData.Bet < 25) {
                    for (int j = 0; j < 5; j++) {
                        if (int.Parse(IconPays[4 - j].ToString()) != 0) {
                            IconPays[4 - j] = int.Parse(IconPays[4 - j].ToString()) * historyData.Bet / 25;
                        }
                    }
                }
            }
            Mod_Data.iconTablePay[i] = new int[IconPays.Count];
            for (int k = 0; k < IconPays.Count; k++) {
                Mod_Data.iconTablePay[i][k] = int.Parse(IconPays[k].ToString());
            }
        }
    }



    int showLineIndex = 0;
    bool clearAllLine = false;
    int pay = 0;
    int Bonustimes = 0;
    public void ShowAllLine()
    {
        //Bonustimes = 0;
        //pay = 0;
        //for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
        //{
        //    LineImg[i].gameObject.SetActive(false);
        //}
        //for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
        //{
        //    if (Mod_Data.linegame_HasWinline[i] == true)
        //    {
        //        //Mod_Data.BonusDelayTimes += 0.05f;
        //        LineImg[i].gameObject.SetActive(true);
        //    }
        //}
        //for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
        //{
        //    if (Mod_Data.linegame_HasWinline[i])
        //    {
               
        //        pay += Mod_Data.iconTablePay[Mod_Data.linegame_WinIconID[i]][Mod_Data.linegame_WinIconQuantity[i]] * historyData.SpecialTime * (int)historyData.Odds;
        //    }
        //}

        //for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
        //{
        //    for (int row = 0; row < Mod_Data.slotRowNum; row++)
        //    {
        //        if (Mod_Data.ReelMath[reel, row] == 1)
        //        {
        //            Bonustimes++;
        //        }
        //    }
        //}

        //if(Bonustimes>=3) pay += Mod_Data.iconTablePay[1][Bonustimes - 1] * historyData.BonusSpecialTime * (int)historyData.Odds;
        //showLineIndex = -1;
        
        //if (historytype == 0)
        //{
        //    totalWinText.text = pay.ToString();
        //}
        //else if (historytype == 1)
        //{
        //    totalWinText.text = historyData.Win.ToString();
        //}
        //else if (historytype == 2)
        //{
        //    totalWinText.text = historyData.Win.ToString();
        //}
    }
    public void ShowNextLine() {
        clearAllLine = false;
        if (clearAllLine == false) {
            if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame) {
                for (int i = 0; i < Mod_Data.linegame_LineCount; i++) {
                    LineImg[i].gameObject.SetActive(false);
                }
            }
            else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame) {
                for (int i = 0; i < Mod_Data.slotRowNum; i++) {
                    reelFrame_0[i].gameObject.SetActive(false);
                    reelFrame_1[i].gameObject.SetActive(false);
                    reelFrame_2[i].gameObject.SetActive(false);
                    reelFrame_3[i].gameObject.SetActive(false);
                    reelFrame_4[i].gameObject.SetActive(false);
                }
            }
            clearAllLine = true;
        }

        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame) {
            for (int i = 0; i < Mod_Data.linegame_LineCount; i++) {
                if (Mod_Data.linegame_HasWinline[i] == true && i > showLineIndex) {
                    //Mod_Data.BonusDelayTimes += 0.05f;
                    LineImg[i].gameObject.SetActive(true);
                    LineImg[showLineIndex].gameObject.SetActive(false);
                    showLineIndex = i;
                    totalWinText.text = Mod_Data.iconTablePay[Mod_Data.linegame_WinIconID[i]][Mod_Data.linegame_WinIconQuantity[i]].ToString();
                    break;
                }
            }
        }
        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame) {
            if (historytype == 0) {
                if (SaveWayID < Mod_Data.allgame_WinLines - 1) {
                    SaveWayID++;
                }
            }
            else {
                SaveWayID = 0;
            }
            //for(int SaveWinLines = 0; SaveWinLines< Mod_Data.slotRowNum; SaveWinLines++) {
            for (int reel = 0; reel < Mod_Data.slotReelNum; reel++) {
                for (int row = 0; row < Mod_Data.slotRowNum; row++) {
                    //if(SaveWayID <= 3) { 
                    if (Mod_Data.waygame_combineIconBool[SaveWayID, reel, row])//有連線的格子 符合目前指定的ID的格子或=wild(ID=0)
                    {
                        //TriggerFrameOutline(reel, row, true, SaveWinLines);//開啟連線外框
                        frameImages[reel][row].gameObject.SetActive(true);
                        if (reel == 1 && row == 0) {
                            Remainder1_0.SetActive(false);
                        }
                        if (reel == 1 && row == 2) {
                            Remainder1_2.SetActive(false);
                        }
                        if (reel == 2 && row == 0) {
                            Remainder2_0.SetActive(false);
                        }
                        if (reel == 2 && row == 2) {
                            Remainder2_2.SetActive(false);
                        }
                        if (reel == 3 && row == 0) {
                            Remainder3_0.SetActive(false);
                        }
                        if (reel == 3 && row == 2) {
                            Remainder3_2.SetActive(false);
                        }
                        if (reel == 4 && row == 0) {
                            Remainder4_0.SetActive(false);
                        }
                        if (reel == 4 && row == 2) {
                            Remainder4_2.SetActive(false);
                        }
                        if (historytype == 0) {
                            totalWinText.text = (Mod_Data.waygmae_EachIconWinLineNum[SaveWayID] * Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[SaveWayID]][Mod_Data.waygame_WinIconQuantity[SaveWayID]] * historyData.Odds * historyData.SpecialTime).ToString();
                        }
                        else {
                            totalWinText.text = historyData.Win.ToString();
                        }
                    }
                }
            }
        }

    }
    public void ShowPreviousLine() {
        //if (clearAllLine == false)
        //{
        //    for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
        //    {
        //        LineImg[i].gameObject.SetActive(false);
        //    }
        //    clearAllLine = true;
        //}
        clearAllLine = false;
        if (clearAllLine == false) {
            if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame) {
                for (int i = 0; i < Mod_Data.linegame_LineCount; i++) {
                    LineImg[i].gameObject.SetActive(false);
                }
            }
            else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame) {
                for (int i = 0; i < Mod_Data.slotRowNum; i++) {
                    reelFrame_0[i].gameObject.SetActive(false);
                    reelFrame_1[i].gameObject.SetActive(false);
                    reelFrame_2[i].gameObject.SetActive(false);
                    reelFrame_3[i].gameObject.SetActive(false);
                    reelFrame_4[i].gameObject.SetActive(false);
                }
            }
            clearAllLine = true;
        }
        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame) {
            for (int i = Mod_Data.linegame_LineCount - 1; i >= 0; i--) {
                if (Mod_Data.linegame_HasWinline[i] == true && i < showLineIndex) {
                    //Mod_Data.BonusDelayTimes += 0.05f;
                    LineImg[i].gameObject.SetActive(true);
                    LineImg[showLineIndex].gameObject.SetActive(false);
                    showLineIndex = i;
                    totalWinText.text = Mod_Data.iconTablePay[Mod_Data.linegame_WinIconID[i]][Mod_Data.linegame_WinIconQuantity[i]].ToString();
                    break;
                }
            }
        }
        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame) {


            if (SaveWayID > 0) {
                SaveWayID--;
            }
            //for(int SaveWinLines = 0; SaveWinLines< Mod_Data.slotRowNum; SaveWinLines++) {
            for (int reel = 0; reel < Mod_Data.slotReelNum; reel++) {
                for (int row = 0; row < Mod_Data.slotRowNum; row++) {
                    //if(SaveWayID <= 3) { 
                    if (Mod_Data.waygame_combineIconBool[SaveWayID, reel, row])//有連線的格子 符合目前指定的ID的格子或=wild(ID=0)
                    {
                        //TriggerFrameOutline(reel, row, true, SaveWinLines);//開啟連線外框
                        frameImages[reel][row].gameObject.SetActive(true);
                        if (reel == 1 && row == 0) {
                            Remainder1_0.SetActive(false);
                        }
                        if (reel == 1 && row == 2) {
                            Remainder1_2.SetActive(false);
                        }
                        if (reel == 2 && row == 0) {
                            Remainder2_0.SetActive(false);
                        }
                        if (reel == 2 && row == 2) {
                            Remainder2_2.SetActive(false);
                        }
                        if (reel == 3 && row == 0) {
                            Remainder3_0.SetActive(false);
                        }
                        if (reel == 3 && row == 2) {
                            Remainder3_2.SetActive(false);
                        }
                        if (reel == 4 && row == 0) {
                            Remainder4_0.SetActive(false);
                        }
                        if (reel == 4 && row == 2) {
                            Remainder4_2.SetActive(false);
                        }
                        if (historytype == 0) {

                            totalWinText.text = (Mod_Data.waygmae_EachIconWinLineNum[SaveWayID] * Mod_Data.iconTablePay[Mod_Data.waygame_WinIconID[SaveWayID]][Mod_Data.waygame_WinIconQuantity[SaveWayID]] * historyData.Odds * historyData.SpecialTime).ToString();
                        }
                        else {
                            totalWinText.text = historyData.Win.ToString();
                        }

                    }
                }
            }
        }
    }
}
