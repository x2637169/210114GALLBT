using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Slots : IGameSystem {

    public Reel[] reel;
    public GameObject[] StopLine, ChangeLine;
    Mod_IconChangeTrigger[] iconChangeTrigger;
    Mod_ReelStopTrigger[] iconStopTrigger;
    int BonusCount = 0;
    public GameObject[] BlankButton;
    public bool[] reelStop = new bool[5];
    [SerializeField] bool[] slowReelOrder = new bool[5];
    [SerializeField] bool[] bonusOnReel = new bool[5];
    [SerializeField] float scrollTime = 0.4f;
    // Use this for initialization
    void Start() {
        iconChangeTrigger = new Mod_IconChangeTrigger[ChangeLine.Length];
        iconStopTrigger = new Mod_ReelStopTrigger[StopLine.Length];
        for (int i = 0; i < ChangeLine.Length; i++)
        {
            iconChangeTrigger[i] = ChangeLine[i].GetComponent<Mod_IconChangeTrigger>();
            iconStopTrigger[i] = StopLine[i].GetComponent<Mod_ReelStopTrigger>();
        }
    }

    // Update is called once per frame
    float StopTimer = 0;
    void Update() {

        if (reelAllStop)
        {
            StopTimer += Time.deltaTime;
            if (StopTimer > 0.2f)
            {
                Mod_Data.reelAllStop = reelAllStop;
                for (int i = 0; i < reel.Length; i++)
                {
                    BlankButton[i].SetActive(false);
                }
                StopTimer = 0;
                reelAllStop = false;
                if (m_ReelStoper != null) StopCoroutine(m_ReelStoper);
            }
        }
        for (int i = 0; i < slowReelOrder.Length; i++)
        {
            if (slowReelOrder[i]) reel[i].startSlowSpeed = true;
            else reel[i].startSlowSpeed = false;
        }


    }

    public void SlotSetReel()
    {
        for (int i = 0; i < reel.Length; i++)
        {
           reel[i].ReelSetRng();
        }
        // int i = 0;
        // while (true)
        // {
        //     for (int aai = 0; aai < reel.Length; aai++)
        //     {
        //         reel[aai].ReelSetRng();
        //     }
        //     Debug.Log("ReSlot:" + i);
        //     GameObject.Find("GameController").GetComponent<Mod_Check>().Check();
        //     if (!Mod_Data.BonusSwitch)
        //     {
        //         if (GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Win < GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Limit)
        //         {
        //             Debug.Log("base Check_Win: " + GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Win);
        //             Debug.Log("Limit: " + GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Limit);
        //             break;
        //         }
        //     }
        //     else
        //     {
        //         if (GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Win < GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Limit)
        //         {
        //             Debug.Log("bonus Check_Win: " + GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Win);
        //             Debug.Log("Limit: " + GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Limit);
        //             break;
        //         }
        //         //  Debug.Log("bonus Check_Win: " + GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Win );
        //         //  Debug.Log("Limit: " + GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Limit);
        //         //if (GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Win <= GameObject.Find("GameController").GetComponent<Mod_Check>().Check_Limit)
        //         //{
        //         //    break;
        //         //}
        //     }
        //     i++;
        //     if (i > 3) Mod_Data.TestMode = false;
        //     if (i > 10) { Debug.Log("force to roll"); break; }
        // }
    }

    public void SlotStart()
    {
        Mod_Data.reelAllStop = false;
       
        for (int i = 0; i < reel.Length; i++)
        {
            iconChangeTrigger[i].gameObject.SetActive(true);
            iconChangeTrigger[i].stopReel = false;
            iconStopTrigger[i].gameObject.SetActive(true);
            iconStopTrigger[i].stopReel = false;
            ChangeLine[i].SetActive(true);
            reelStop[i] = false;
            slowReelOrder[i] = false;
            bonusOnReel[i] = false;
            reel[i].blankButtonDown = false;
            if (!Mod_Data.BonusSwitch) BlankButton[i].SetActive(true);
           // reel[i].ReelSetRng();
            reel[i].SpinCtrl(true);
            bonusAndStopCount = 0;
        }

        m_ReelStoper = StartCoroutine(ReelStoper());
        DetectBonusOnReel();
    }

    //轉動滾輪
    public void StopAllReel()
    {
        for (int i = 0; i < Mod_Data.slotReelNum; i++)
        {
            reel[i].SpinCtrl(false);
        }
    }

    private Coroutine m_ReelStoper = null;
    public IEnumerator ReelStoper()
    {
        
        yield return new WaitForSeconds(scrollTime);
        if (slowReelOrder[0]) yield return new WaitForSeconds(2f);
        reel[0].SpinCtrl(false);

        yield return new WaitUntil(() => reel[0].spin == false);
        yield return new WaitForSeconds(scrollTime);
        if (slowReelOrder[1]) yield return new WaitForSeconds(2f);
        if (reel[1].spin&& bonusOnReel[0]&& bonusOnReel[1])
        {
            iconChangeTrigger[2].specialChange = true;//驢子閃牌
            //Debug.Log("A");
        }
        reel[1].SpinCtrl(false);
        yield return new WaitUntil(() => reel[0].spin == false);
        yield return new WaitUntil(() => reel[1].spin == false);
        yield return new WaitForSeconds(scrollTime);
        if (slowReelOrder[2]) yield return new WaitForSeconds(2f);
        
        reel[2].SpinCtrl(false);
        yield return new WaitUntil(() => reel[0].spin == false);
        yield return new WaitUntil(() => reel[1].spin == false);
        yield return new WaitUntil(() => reel[2].spin == false);
        yield return new WaitForSeconds(scrollTime);
        if (slowReelOrder[3]) yield return new WaitForSeconds(2f);
       
        reel[3].SpinCtrl(false);

        yield return new WaitUntil(() => reel[0].spin == false);
        yield return new WaitUntil(() => reel[1].spin == false);
        yield return new WaitUntil(() => reel[2].spin == false);
        yield return new WaitUntil(() => reel[3].spin == false);
        yield return new WaitForSeconds(scrollTime);
        if (slowReelOrder[4]) yield return new WaitForSeconds(2f);
        reel[4].SpinCtrl(false);
        //Debug.Log("44444");

    }

    bool reelAllStop = false;
    void DetectBonusOnReel()
    {
        for (int i = 0; i < Mod_Data.slotReelNum; i++)
        {
            for (int j = 0; j < Mod_Data.slotRowNum; j++)
            {
                if (Mod_Data.ReelMath[i, j] == 1)
                {
                    bonusOnReel[i] = true;
                    break;
                }
            }

        }
    }
    int bonusAndStopCount = 0;
    public void DetectAllReelStop(int reelIndex, bool isStop)
    {
        reelStop[reelIndex] = true;
        reelAllStop = false;
        for (int i = 0; i < reelStop.Length; i++)
        {
            reelAllStop = true;
            if (!reelStop[i])
            {
                reelAllStop = false;
                break;
            }
        }




        if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3)
        {
            m_SlotMediatorController.SendMessage("m_slots", "StopReelSpeedUp");
            if ((!reelStop[1] && !reelStop[2]) || ((!reelStop[2] && reelStop[1] && bonusOnReel[1]) || (!reelStop[1] && reelStop[2] && bonusOnReel[2])) || ((reelStop[1] && bonusOnReel[1]) && (reelStop[2] && bonusOnReel[2]))) {
                if ((reelStop[0] && bonusOnReel[0] && reelIndex == 0)) {
                    m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
                }
                else {
                    m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
                }
            }
            else {
                m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
            }
            if ((!reelStop[0] && !reelStop[2]) || ((!reelStop[2] && reelStop[0] && bonusOnReel[0]) || (!reelStop[0] && reelStop[2] && bonusOnReel[2])) || ((reelStop[0] && bonusOnReel[0]) && (reelStop[2] && bonusOnReel[2]))) {
                if ((reelStop[1] && bonusOnReel[1] && reelIndex == 1)) {
                    m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
                }
                else {
                    m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
                }
            }
            else {
                m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
            }
            if ((!reelStop[1] && !reelStop[0]) || ((!reelStop[0] && reelStop[1] && bonusOnReel[1]) || (!reelStop[1] && reelStop[0] && bonusOnReel[0])) || ((reelStop[1] && bonusOnReel[1]) && (reelStop[0] && bonusOnReel[0]))) {
                if ((reelStop[2] && bonusOnReel[2] && reelIndex == 2)) {
                    m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
                }
                else {
                    m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
                }
            }
            else {
                m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
            }
            ////Debug.Log(bonusAndStopCount);
            //if (bonusAndStopCount == 2) {
            //    if (!reelAllStop) m_SlotMediatorController.SendMessage("m_slots", "PlayReelSpeedUp");
            //    if (!reelStop[0]) {
            //        reel[0].startSlowSpeed = true;
            //        slowReelOrder[0] = true;
            //    }
            //    else if (!reelStop[1]) {
            //        reel[1].startSlowSpeed = true;
            //        slowReelOrder[1] = true;
            //    }
            //    else if (!reelStop[2]) {
            //        reel[2].startSlowSpeed = true;
            //        slowReelOrder[2] = true;
            //    }
            //}

            if (!reelStop[0] && (reelStop[1] && bonusOnReel[1]) && (reelStop[2] && bonusOnReel[2])) {
                m_SlotMediatorController.SendMessage("m_slots", "PlayReelSpeedUp");
                reel[0].startSlowSpeed = true;
                slowReelOrder[0] = true;
            }
            else if ((reelStop[0] && bonusOnReel[0]) && !reelStop[1] && (reelStop[2] && bonusOnReel[2])) {
                m_SlotMediatorController.SendMessage("m_slots", "PlayReelSpeedUp");
                reel[1].startSlowSpeed = true;
                slowReelOrder[1] = true;
            }
            else if ((reelStop[0] && bonusOnReel[0]) && (reelStop[1] && bonusOnReel[1]) && !reelStop[2]) {
                m_SlotMediatorController.SendMessage("m_slots", "PlayReelSpeedUp");
                reel[2].startSlowSpeed = true;
                slowReelOrder[2] = true;
            }
        }
        else if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4)
        {
            if (!reelStop[1] && (reelStop[2] && bonusOnReel[2]) && (reelStop[3] && bonusOnReel[3]))
            {
                reel[1].startSlowSpeed = true;
                slowReelOrder[1] = true;
            }
            else if ((reelStop[1] && bonusOnReel[1]) && !reelStop[2] && (reelStop[3] && bonusOnReel[3]))
            {
                reel[2].startSlowSpeed = true;
                slowReelOrder[2] = true;
            }
            else if ((reelStop[1] && bonusOnReel[1]) && (reelStop[2] && bonusOnReel[2]) && !reelStop[3])
            {
                reel[3].startSlowSpeed = true;
                slowReelOrder[3] = true;
            }
        }
        else if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5)
        {
           
            m_SlotMediatorController.SendMessage("m_slots", "StopReelSpeedUp");
            if ((reelStop[0] && bonusOnReel[0] && reelIndex == 0))
            {
                bonusAndStopCount++;
                m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
            }
            else if ((reelStop[1] && bonusOnReel[1] && reelIndex == 1))
            {
                bonusAndStopCount++;
                m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
            }
            else if ((reelStop[2] && bonusOnReel[2] && reelIndex == 2))
            {
                bonusAndStopCount++;
                m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
            }
            else if ((reelStop[3] && bonusOnReel[3] && reelIndex == 3))
            {
                bonusAndStopCount++;
                m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
            }
            else if ((reelStop[4] && bonusOnReel[4] && reelIndex == 4))
            {
                bonusAndStopCount++;
                m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
            }
            else
            {
                m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
            }

            ////Debug.Log(bonusAndStopCount);
            if (bonusAndStopCount >= 2)
            {
               if(!reelAllStop) m_SlotMediatorController.SendMessage("m_slots", "PlayReelSpeedUp");
                if (!reelStop[0])
                {
                    reel[0].startSlowSpeed = true;
                    slowReelOrder[0] = true;
                }
                else if (!reelStop[1])
                {
                    reel[1].startSlowSpeed = true;
                    slowReelOrder[1] = true;
                }
                else if (!reelStop[2])
                {
                    reel[2].startSlowSpeed = true;
                    slowReelOrder[2] = true;
                }
                else if (!reelStop[3])
                {
                    reel[3].startSlowSpeed = true;
                    slowReelOrder[3] = true;
                }
                else if (!reelStop[4])
                {
                    reel[4].startSlowSpeed = true;
                    slowReelOrder[4] = true;
                }
            }
        }
        else if (Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5)
        {
            bonusAndStopCount = 0;
            if ((reelStop[0] && bonusOnReel[0]))
            {
                bonusAndStopCount++;
            }
            if ((reelStop[1] && bonusOnReel[1]))
            {
                bonusAndStopCount++;
            }
            if ((reelStop[2] && bonusOnReel[2]))
            {
                bonusAndStopCount++;
            }
            if ((reelStop[3] && bonusOnReel[3]))
            {
                bonusAndStopCount++;
            }
            if ((reelStop[4] && bonusOnReel[4]))
            {
                bonusAndStopCount++;
            }
            ////Debug.Log(bonusAndStopCount);
            if (bonusAndStopCount >= 2)
            {
                if (!reelStop[0])
                {
                    reel[0].startSlowSpeed = true;
                    slowReelOrder[0] = true;
                }
                else if (!reelStop[1] && (reelStop[0] && bonusOnReel[0]))
                {
                    reel[1].startSlowSpeed = true;
                    slowReelOrder[1] = true;
                }
                else if ((reelStop[0] && bonusOnReel[0]) && (reelStop[1] && bonusOnReel[1]) && !reelStop[2])
                {
                    reel[2].startSlowSpeed = true;
                    slowReelOrder[2] = true;
                }
                else if ((reelStop[0] && bonusOnReel[0]) && (reelStop[1] && bonusOnReel[1]) && (reelStop[2] && bonusOnReel[2]) && !reelStop[3])
                {
                    reel[3].startSlowSpeed = true;
                    slowReelOrder[3] = true;
                }
                else if ((reelStop[0] && bonusOnReel[0]) && (reelStop[1] && bonusOnReel[1]) && (reelStop[2] && bonusOnReel[2]) && (reelStop[3] && bonusOnReel[3]) && !reelStop[4])
                {
                    reel[4].startSlowSpeed = true;
                    slowReelOrder[4] = true;
                }
            }




        }
        #region 楚河
        //else if (Mod_Data.bonusRule == Mod_Data.BonusRule.CH)
        //{
        //    if (!bonusOnReel[reelIndex]) m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
        //    else
        //    {



        //        if (reelAllStop)
        //        {
        //            if ((!(Mod_Data.ReelMath[2, 0] == 1 && Mod_Data.ReelMath[3, 2] == 1) && !(Mod_Data.ReelMath[2, 2] == 1 && Mod_Data.ReelMath[3, 0] == 1)
        //                && !(Mod_Data.ReelMath[3, 0] == 1 && Mod_Data.ReelMath[4, 2] == 1) && !(Mod_Data.ReelMath[3, 2] == 1 && Mod_Data.ReelMath[4, 0] == 1))
        //                && (bonusOnReel[2] && bonusOnReel[3] && bonusOnReel[4]))
        //            {
        //                m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
        //            }
        //            else
        //            {
        //                m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
        //            }
        //        }
        //        else
        //        {
        //            if (!bonusOnReel[2] && reelStop[2] && reelIndex == 3)
        //            {
        //                m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
        //            }
        //            else if ((!bonusOnReel[2] && reelStop[2]) || (!bonusOnReel[3] && reelStop[3]) && reelIndex == 4)
        //            {
        //                m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
        //            }
        //            else
        //            {
        //                if (((Mod_Data.ReelMath[2, 0] == 1 && Mod_Data.ReelMath[3, 2] == 1) || (Mod_Data.ReelMath[2, 2] == 1 && Mod_Data.ReelMath[3, 0] == 1))
        //                    && ((reelStop[2] && reelIndex == 3) || (reelIndex == 2 && reelStop[3])))
        //                {
        //                    m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
        //                }
        //                else if (((Mod_Data.ReelMath[3, 0] == 1 && Mod_Data.ReelMath[4, 2] == 1) || (Mod_Data.ReelMath[3, 2] == 1 && Mod_Data.ReelMath[4, 0] == 1))
        //                    && ((reelStop[3] && reelIndex == 4) || (reelIndex == 3 && reelStop[4])))
        //                {
        //                    m_SlotMediatorController.SendMessage("m_slots", "PlayReelStop", reelIndex);
        //                }
        //                else
        //                {
        //                    m_SlotMediatorController.SendMessage("m_slots", "PlayBonusReelStop", reelIndex);
        //                }

        //            }


        //        }


        //    }
        //    m_SlotMediatorController.SendMessage("m_slots", "StopReelSpeedUp");

        //    if (!reelStop[2] && (reelStop[3] && bonusOnReel[3]) && (reelStop[4] && bonusOnReel[4]))
        //    {
        //        if (!(Mod_Data.ReelMath[3, 0] == 1 && Mod_Data.ReelMath[4, 2] == 1) && !(Mod_Data.ReelMath[3, 2] == 1 && Mod_Data.ReelMath[4, 0] == 1))
        //        {
        //            reel[2].startSlowSpeed = true;
        //            slowReelOrder[2] = true;
        //            m_SlotMediatorController.SendMessage("m_slots", "PlayReelSpeedUp");
        //        }
        //    }
        //    else if ((reelStop[2] && bonusOnReel[2]) && !reelStop[3] && (reelStop[4] && bonusOnReel[4]))
        //    {

        //        reel[3].startSlowSpeed = true;
        //        slowReelOrder[3] = true;
        //        m_SlotMediatorController.SendMessage("m_slots", "PlayReelSpeedUp");

        //    }
        //    else if ((reelStop[2] && bonusOnReel[2]) && (reelStop[3] && bonusOnReel[3]) && !reelStop[4])
        //    {
        //        if (!(Mod_Data.ReelMath[2, 0] == 1 && Mod_Data.ReelMath[3, 2] == 1) && !(Mod_Data.ReelMath[2, 2] == 1 && Mod_Data.ReelMath[3, 0] == 1))
        //        {
        //            reel[4].startSlowSpeed = true;
        //            slowReelOrder[4] = true;
        //            m_SlotMediatorController.SendMessage("m_slots", "PlayReelSpeedUp");
        //        }
        //    }
        //}
        #endregion

        //Mod_Data.reelAllStop = reelAllStop;
    }
    public void BlankButtonClick(int ReelNumber)
    {
        if (!Mod_Data.BonusSwitch) {
            if (reel[ReelNumber].spin == true) {
                reel[ReelNumber].blankButtonDown = true;
                reel[ReelNumber].SpinCtrl(false);
                reel[ReelNumber].fastScroll();
            }

        }
    }
}
