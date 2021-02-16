using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = System.Random;
using static NewSramManager;

public class Mod_NewLoadGame : MonoBehaviour
{
    // Start is called before the first frame update
    NewSramManager newSramManager;
    HistoryData historyData = new HistoryData();
    int latestGameNumber, status;
    [SerializeField] UIItemDatabase BaseiconDatabase, BonusiconDatabase;
    [SerializeField] GameObject[] EndReel1, EndReel2, EndReel3, EndReel4, EndReel5;
    [SerializeField] Mod_UIController mod_UIController;
    [SerializeField] GameObject BonusSpecialRuleTimesIcon;
    [SerializeField] Sprite[] DKTimesSprite;
    [SerializeField] Text WinText, text_BonusCount, text_SpecialTime;
    void Start()
    {
        newSramManager = GetComponent<NewSramManager>();

#if !UNITY_EDITOR
        Mod_Data.maxOdds = newSramManager.LoadMaxOdd();
        Mod_Data.maxCredit = newSramManager.LoadMaxCredit();
        Mod_Data.maxWin = newSramManager.LoadMaxWin();
#endif


        newSramManager.LoadGameHistoryLog(0, out latestGameNumber);
        status = newSramManager.LoadStatus();
        if (status == 0)//BaseSpin 
        {
            if (latestGameNumber > 0) newSramManager.LoadHistory(latestGameNumber, out historyData);
            Mod_Data.RTPsetting = historyData.RTP;
            startLoadReel(historyData.RNG, historyData.Bonus);
            Mod_Data.credit = historyData.Credit + newSramManager.LoadCoinInPoint() + newSramManager.LoadTicketInPoint() - newSramManager.LoadTicketOutPoint() + newSramManager.LoadOpenClearPoint(true) - newSramManager.LoadOpenClearPoint(false);
            Mod_Data.BonusSpecialTimes = historyData.SpecialTime;
            //Debug.Log("status0");
        }
        else if (status == 1)//BaseScrolling
        {
            if (latestGameNumber > 1) newSramManager.LoadHistory(latestGameNumber - 1, out historyData);
            Mod_Data.RTPsetting = historyData.RTP;
            startLoadReel(historyData.RNG, historyData.Bonus);
            if (latestGameNumber > 0) newSramManager.LoadHistory(latestGameNumber, out historyData);
            Mod_Data.credit = historyData.Credit + newSramManager.LoadCoinInPoint() + newSramManager.LoadTicketInPoint() - newSramManager.LoadTicketOutPoint() + newSramManager.LoadOpenClearPoint(true) - newSramManager.LoadOpenClearPoint(false) - historyData.Win * ((float)historyData.Demon / 1000);
            Mod_Data.BonusSpecialTimes = historyData.SpecialTime;
            Mod_Data.StartNotNormalRTP = historyData.RTP;
            //Debug.Log("status1");
        }
        else if (status == 2)//transIn
        {
            if (latestGameNumber > 0) newSramManager.LoadHistory(latestGameNumber, out historyData);
            Mod_Data.RTPsetting = historyData.RTP;
            startLoadReel(historyData.RNG, historyData.Bonus);
            Mod_Data.credit = historyData.Credit + newSramManager.LoadCoinInPoint() + newSramManager.LoadTicketInPoint() - newSramManager.LoadTicketOutPoint() + newSramManager.LoadOpenClearPoint(true) - newSramManager.LoadOpenClearPoint(false) - historyData.Win * ((float)historyData.Demon / 1000);
            Mod_Data.Win = historyData.Win;
            Mod_Data.BonusSpecialTimes = newSramManager.LoadSpeicalTime();
            Mod_Data.BonusCount = newSramManager.LoadBonusCount();
            if (Mod_Data.BonusSpecialTimes == 0)
            {
                Mod_Data.BonusSpecialTimes = 1;
            }
            //Debug.Log("BonusSpecialTimes" + Mod_Data.BonusSpecialTimes);
            //Debug.Log("BonusCount" + Mod_Data.BonusCount);
            //Debug.Log("status2");

        }
        else if (status == 3)//BonusScrolling
        {
            if (latestGameNumber > 1) newSramManager.LoadHistory(latestGameNumber - 1, out historyData);
            Mod_Data.RTPsetting = historyData.RTP;
            Mod_Data.BonusSwitch = historyData.Bonus;
            startLoadReel(historyData.RNG, historyData.Bonus);
            Mod_Data.BonusSpecialTimes = newSramManager.LoadSpeicalTime();
            //DKTimesImg.sprite = DKTimesSprite[Mod_Data.BonusSpecialTimes - 1];
            text_SpecialTime.text = Mod_Data.BonusSpecialTimes.ToString();
            Mod_Data.getBonusCount = newSramManager.LoadBonusCount();
            BonusSpecialRuleTimesIcon.SetActive(true);
            Mod_Data.Win = historyData.Win;
            if (historyData.BonusCount == 0)
            {
                text_BonusCount.text = newSramManager.LoadBonusCount().ToString();
            }
            else
            {
                text_BonusCount.text = historyData.BonusCount.ToString();
            }
            if (latestGameNumber > 0) newSramManager.LoadHistory(latestGameNumber, out historyData);
            Mod_Data.credit = historyData.Credit + newSramManager.LoadCoinInPoint() + newSramManager.LoadTicketInPoint() - newSramManager.LoadTicketOutPoint() + newSramManager.LoadOpenClearPoint(true) - newSramManager.LoadOpenClearPoint(false);
            Mod_Data.RNG = historyData.RNG;
            Mod_Data.BonusCount = historyData.BonusCount;
            WinText.text = Mod_Data.Win.ToString();
            Mod_Data.StartNotNormalRTP = historyData.RTP;
            //Debug.Log("status3");
        }
        else
        {

        }

        //Debug.Log("latestGameNumber" + latestGameNumber);


        Mod_Data.RNG = historyData.RNG;
        Mod_Data.BonusSwitch = historyData.Bonus;
        Mod_Data.Denom = (float)historyData.Demon / 1000;
        Mod_Data.Bet = historyData.Bet;
        Mod_Data.odds = historyData.Odds;
        // Mod_Data.Win = historyData.Win;
        Mod_Data.RTPsetting = historyData.RTP;

        //Mod_Data.CH_BonusSpecialTime = historyData.BonusSpecialTime;
        Mod_Data.BonusIsPlayedCount = historyData.BonusIsPlayedCount;
        //Mod_Data.BonusCount = historyData.BonusCount;
        mod_UIController.UpdateScore();

    }

    // Update is called once per frame
    void Update()
    {
        ////Debug.Log(EndReel1[bottomIndex].gameObject.GetComponent<Image>().sprite.name);
    }
    int randomID = 0;
    Random randomIDspace = new Random(Guid.NewGuid().GetHashCode());
    JArray ReelMath;
    int[] beforeBonusRNG;
    int ReelRNG, bottomIndex;
    public void ChangeScene(int changeToBonus)//changeToBonus = 0 changeToBase = 1
    {
        if (changeToBonus == 0)
        {
            newSramManager.SaveBeforeBonusRNG(Mod_Data.RNG);
            for (int i = 0; i < EndReel1.Length; i++)
            {


                randomID = randomIDspace.Next(0, BonusiconDatabase.items.Length);
                if (randomID == 1) randomID += 2;
                if (randomID == 12) randomID--;
                EndReel1[i].gameObject.GetComponent<Image>().sprite =
                        BonusiconDatabase.GetByID(randomID).Sprite;
                EndReel1[i].GetComponent<SlotSymbol>().iconID = randomID;
                randomID = randomIDspace.Next(0, BonusiconDatabase.items.Length);
                if (randomID == 1) randomID += 2;
                if (randomID == 12) randomID--;
                EndReel2[i].gameObject.GetComponent<Image>().sprite =
                       BonusiconDatabase.GetByID(randomID).Sprite;
                EndReel2[i].GetComponent<SlotSymbol>().iconID = randomID;
                randomID = randomIDspace.Next(0, BonusiconDatabase.items.Length);
                if (randomID == 1) randomID += 3;
                if (randomID == 12) randomID--;
                EndReel3[i].gameObject.GetComponent<Image>().sprite =
                       BonusiconDatabase.GetByID(randomID).Sprite;
                EndReel3[i].GetComponent<SlotSymbol>().iconID = randomID;
                randomID = randomIDspace.Next(0, BonusiconDatabase.items.Length);
                if (randomID == 1) randomID += 4;
                if (randomID == 12) randomID--;
                EndReel4[i].gameObject.GetComponent<Image>().sprite =
                       BonusiconDatabase.GetByID(randomID).Sprite;
                EndReel4[i].GetComponent<SlotSymbol>().iconID = randomID;
                randomID = randomIDspace.Next(0, BonusiconDatabase.items.Length);
                if (randomID == 1) randomID += 2;
                if (randomID == 12) randomID--;
                EndReel5[i].gameObject.GetComponent<Image>().sprite =
                       BonusiconDatabase.GetByID(randomID).Sprite;
                EndReel5[i].GetComponent<SlotSymbol>().iconID = randomID;
            }
        }
        else//BonusToBase
        {

            beforeBonusRNG = newSramManager.LoadBeforeBonusRNG();
            Mod_Data.RNG = beforeBonusRNG;
            Mod_Data.JsonReload();
            for (int i = 0; i < EndReel1.Length; i++)
            {
                if (EndReel1[i].GetComponent<SlotSymbol>().bottomIcon)
                {
                    ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_1"]["Reel"];
                    ReelRNG = beforeBonusRNG[0];
                    bottomIndex = i;
                    for (int k = 0; k < 8; k++)
                    {
                        if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
                        if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
                        EndReel1[bottomIndex].gameObject.GetComponent<Image>().sprite =
                         BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
                        EndReel1[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());
                        ReelRNG -= 1;
                        bottomIndex -= 1;
                    }
                }
                if (EndReel2[i].GetComponent<SlotSymbol>().bottomIcon)
                {
                    ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_2"]["Reel"];
                    ReelRNG = beforeBonusRNG[1];
                    bottomIndex = i;
                    for (int k = 0; k < 8; k++)
                    {
                        if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
                        if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
                        EndReel2[bottomIndex].gameObject.GetComponent<Image>().sprite =
                    BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
                        EndReel2[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());

                        ReelRNG -= 1;
                        bottomIndex -= 1;
                    }
                }
                if (EndReel3[i].GetComponent<SlotSymbol>().bottomIcon)
                {
                    ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_3"]["Reel"];
                    ReelRNG = beforeBonusRNG[2];
                    bottomIndex = i;
                    for (int k = 0; k < 8; k++)
                    {
                        if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
                        if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
                        EndReel3[bottomIndex].gameObject.GetComponent<Image>().sprite =
                     BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
                        EndReel3[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());

                        ReelRNG -= 1;
                        bottomIndex -= 1;
                    }
                }
                if (EndReel4[i].GetComponent<SlotSymbol>().bottomIcon)
                {
                    ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_4"]["Reel"];
                    ReelRNG = beforeBonusRNG[3];
                    bottomIndex = i;
                    for (int k = 0; k < 8; k++)
                    {
                        if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
                        if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
                        EndReel4[bottomIndex].gameObject.GetComponent<Image>().sprite =
                     BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
                        EndReel4[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());

                        ReelRNG -= 1;
                        bottomIndex -= 1;
                    }
                }
                if (EndReel5[i].GetComponent<SlotSymbol>().bottomIcon)
                {
                    ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_5"]["Reel"];
                    ReelRNG = beforeBonusRNG[4];
                    bottomIndex = i;
                    for (int k = 0; k < 8; k++)
                    {
                        if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
                        if (bottomIndex < 0) { bottomIndex = EndReel1.Length - 1; }
                        EndReel5[bottomIndex].gameObject.GetComponent<Image>().sprite =
                     BaseiconDatabase.GetByID(int.Parse(ReelMath[ReelRNG].ToString())).Sprite;
                        EndReel5[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());

                        ReelRNG -= 1;
                        bottomIndex -= 1;
                    }
                }
            }
        }
    }

    public void startLoadReel(int[] beforeBonusRNG, bool isBonus)
    {

        Mod_Data.JsonReload();
        for (int i = 0; i < EndReel1.Length; i++)
        {
            if (EndReel1[i].GetComponent<SlotSymbol>().bottomIcon)
            {
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_1"]["Reel"];
                ReelRNG = beforeBonusRNG[0];
                bottomIndex = i;
                for (int k = 0; k < Mod_Data.slotRowNum; k++)
                {
                    if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
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


                    EndReel1[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());
                    ReelRNG -= 1;
                    bottomIndex -= 1;
                }
            }
            if (EndReel2[i].GetComponent<SlotSymbol>().bottomIcon)
            {
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_2"]["Reel"];
                ReelRNG = beforeBonusRNG[1];
                bottomIndex = i;
                for (int k = 0; k < Mod_Data.slotRowNum; k++)
                {
                    if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
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
                    EndReel2[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());

                    ReelRNG -= 1;
                    bottomIndex -= 1;
                }
            }
            if (EndReel3[i].GetComponent<SlotSymbol>().bottomIcon)
            {
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_3"]["Reel"];
                ReelRNG = beforeBonusRNG[2];
                bottomIndex = i;
                for (int k = 0; k < Mod_Data.slotRowNum; k++)
                {
                    if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
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
                    EndReel3[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());

                    ReelRNG -= 1;
                    bottomIndex -= 1;
                }
            }
            if (EndReel4[i].GetComponent<SlotSymbol>().bottomIcon)
            {
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_4"]["Reel"];
                ReelRNG = beforeBonusRNG[3];
                bottomIndex = i;
                for (int k = 0; k < Mod_Data.slotRowNum; k++)
                {
                    if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
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
                    EndReel4[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());

                    ReelRNG -= 1;
                    bottomIndex -= 1;
                }
            }
            if (EndReel5[i].GetComponent<SlotSymbol>().bottomIcon)
            {
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_5"]["Reel"];
                ReelRNG = beforeBonusRNG[4];
                bottomIndex = i;
                for (int k = 0; k < Mod_Data.slotRowNum; k++)
                {
                    if (ReelRNG < 0) { ReelRNG = ReelMath.Count - 1; }
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
                    EndReel5[bottomIndex].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[ReelRNG].ToString());

                    ReelRNG -= 1;
                    bottomIndex -= 1;
                }
            }
        }

    }
}
