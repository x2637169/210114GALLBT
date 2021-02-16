using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Mod_Animation : IGameSystem
{
    [SerializeField] int rayCasterReelnum = Mod_Data.slotReelNum, rayCasterRownum = Mod_Data.slotRowNum;
    [SerializeField] Image lineImage, DKSpecialTimesImage, Line50Img;
    [SerializeField] RayCaster[] reelRayCaster_0, reelRayCaster_1, reelRayCaster_2, reelRayCaster_3, reelRayCaster_4;
    [SerializeField] Image[] reelFrame_0, reelFrame_1, reelFrame_2, reelFrame_3, reelFrame_4, LineImg;
    [SerializeField] public Color[] lineColors;
    [SerializeField] public Sprite[] lineSprite, DKSpecialTimesSprite;
    [SerializeField] bool playFrame, stopFrame;
    [SerializeField] Animator animatior_BonusTrans, animator_DKSpecialAnimReel2, animator_DKSpecialAnimReel4;
    [SerializeField] GameObject DKhead;
    [SerializeField] NewSramManager newSramManager;
    public Image[][] frameImages;
    RayCaster[][] rayCasters;
    Mod_UIController mod_UIController;
    public GameObject BonusMap;
    //int[] lingame_WinIconQuantity = new int[25];//LineGame記錄每連線icon個數
    //int[][] lingame_WinLine = new int[25][];//LineGame記錄每連線位置
    int lingame_lineCount;//LineGame記錄幾條線
    int lingame_lineIndex = 0;//LineGame記錄跳線序號

    private void Start()
    {
        initAnimation();

        mod_UIController = GetComponent<Mod_UIController>();

        lingame_lineCount = 25;
        // TriggerIconAnimator();

    }
    private void Update()
    {
        //if (playFrame && frameAnimationRunning)
        //{
        //    StopCoroutine(LineGame_PlayFrameOutlineAnimation());
        //    frameAnimationRunning = false;
        //    playFrame = false;
        //}
        //else if (playFrame)
        //{
        //    StartCoroutine(LineGame_PlayFrameOutlineAnimation());
        //    frameAnimationRunning = true;
        //    playFrame = false;
        //    lingame_lineIndex = 0;
        //}
        //if (playFrame)
        //{


        //    if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame) PlayWayGameFrameOutline();
        //    else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame) PlayLineGameFrameOutline();

        //    playFrame = false;
        //}

    }
    void initAnimation()
    {
        rayCasters = new RayCaster[Mod_Data.slotReelNum][];
        rayCasters[0] = reelRayCaster_0;
        rayCasters[1] = reelRayCaster_1;
        rayCasters[2] = reelRayCaster_2;
        rayCasters[3] = reelRayCaster_3;
        rayCasters[4] = reelRayCaster_4;

        frameImages = new Image[Mod_Data.slotReelNum][];
        frameImages[0] = reelFrame_0;
        frameImages[1] = reelFrame_1;
        frameImages[2] = reelFrame_2;
        frameImages[3] = reelFrame_3;
        frameImages[4] = reelFrame_4;

        if (Mod_Data.slotVer == Mod_Data.SlotVer.reel4x5)
        {
            List<int> parts = new List<int>();
            //紀錄位置
            parts.Add(280 - 52);
            parts.Add(457 - 52);
            parts.Add(634 - 52);
            parts.Add(811 - 52);
            //初始化frame及raycasters位置
            for (int i = 0; i < Mod_Data.slotReelNum; i++)
            {
                for (int j = 0; j < Mod_Data.slotRowNum; j++)
                {
                    frameImages[i][j].GetComponent<RectTransform>().position = new Vector2(frameImages[i][j].transform.position.x, parts[j]);
                    rayCasters[i][j].GetComponent<RectTransform>().position = new Vector2(rayCasters[i][j].transform.position.x, parts[j]);
                }
            }
            //初始化frame大小
            for (int i = 0; i < Mod_Data.slotReelNum; i++)
            {
                for (int j = 0; j < Mod_Data.slotRowNum; j++)
                {
                    frameImages[i][j].GetComponent<RectTransform>().sizeDelta =
                        new Vector2(frameImages[i][j].GetComponent<RectTransform>().sizeDelta.x, frameImages[i][j].GetComponent<RectTransform>().sizeDelta.y - 43);
                }
            }
        }
    }

    public void BonusTransIn(bool transIn)
    {
        if (transIn)
        {
            //Mod_Data.BonusSwitch = true;
            BonusMap.SetActive(true);
            BonusMap.GetComponent<Animator>().enabled = true;
            if (Mod_Data.StartNotNormal)
            {
                //Debug.Log("2222Mod_Data.BonusCount" + Mod_Data.BonusCount);
                //判斷fish跟boat是否有值,有值帶入沒值重啟
                if (Mod_Data.BonusSpecialTimes == 1 && Mod_Data.BonusCount == 0)
                {
                    //Debug.Log("重開bonus");
                }
                else if (Mod_Data.BonusSpecialTimes != 1 && Mod_Data.BonusCount == 0)
                {
                    int FishID = newSramManager.LoadUserSelectedSpeicalTime();
                    BonusMap.GetComponent<Mod_BonusCameraMove>().BonusFishSelect(FishID);
                }
                else if (Mod_Data.BonusSpecialTimes != 1 && Mod_Data.BonusCount != 0)
                {
                    int FishID = newSramManager.LoadUserSelectedSpeicalTime();
                    BonusMap.GetComponent<Mod_BonusCameraMove>().BonusFishSelect(FishID);
                    int BoatID = newSramManager.LoadUserSelectedBonusCount();
                    BonusMap.GetComponent<Mod_BonusCameraMove>().BonusBoatSelect(BoatID);
                    //Debug.Log("過場結束");
                }
            }
            //animatior_BonusTrans.SetTrigger("transIn");
        }

        else
        {

            animatior_BonusTrans.SetTrigger("transOut");
        }
    }

    public void TriggerIconPlayAnimator(int reel, int row, bool playAnim)//觸發動畫(排,列,開/關)
    {

        rayCasters[reel][row].GetSymbolPlayAnimation(playAnim);
        if (playAnim && Mod_Data.ReelMath[reel, row] == 0)
        {
            Mod_Data.hasWildPlaySound = true;

        }
        if (playAnim && Mod_Data.ReelMath[reel, row] == 12)
        {
            Mod_Data.hasSWildPlaySound = true;
        }
        if (!playAnim)
        {
            Mod_Data.hasWildPlaySound = false;
            Mod_Data.hasSWildPlaySound = false;
        }

    }
    public void TriggerIconOpenAnimator(int reel, int row, bool playAnim)//觸發開啟動畫器(排,列,開/關)
    {

        rayCasters[reel][row].GetSymbolOpenAnimator(playAnim);

    }
    public void TriggerIconChange(int reel, int row, int iconID)
    {
        //rayCasters[reel][row].GetSymbolChangeIconSprite(iconID);


    } //改變Icon圖 (測試用)


    public void TriggerFrameOutline(int reel, int row, bool showOutline, int colorIndex)//開啟框 設定框顏色(多載)
    {

        frameImages[reel][row].gameObject.SetActive(showOutline);
        frameImages[reel][row].color = lineColors[colorIndex];
        lineImage.color = lineColors[colorIndex];
    }
    public void TriggerFrameOutline(int reel, int row, bool showOutline)
    {

        frameImages[reel][row].gameObject.SetActive(showOutline);

    }//關閉框(多載)


    public void PlayWayGameFrameOutline(bool isBTRule)
    {
        if (!Mod_Data.getBonus)
        {
            int index_BTRule = Array.FindIndex(Mod_Data.isFishChange_BTRule, x => x == true);
            int lineLimit = index_BTRule <= -1 ? Mod_Data.allgame_WinLines : index_BTRule;
            index_BTRule = index_BTRule <= -1 ? 0 : index_BTRule;
            StartCoroutine(WayGame_PlayFrameOutlineAnimation((!isBTRule ? 0 : index_BTRule), (!isBTRule ? lineLimit : Mod_Data.allgame_WinLines), isBTRule));
        }
        else
        {
            for (int i = 0; i < Mod_Data.slotReelNum; i++)
            {
                for (int k = 0; k < Mod_Data.slotRowNum; k++)
                {
                    if (Mod_Data.ReelMath[i, k] == 1)
                    {
                        TriggerFrameOutline(i, k, true, 0);
                    }
                }
            }
            for (int i = 0; i < Mod_Data.allgame_WinLines; i++)
            {
                if (Mod_Data.waygame_WinIconID[i] == 1)
                {
                    mod_UIController.BannerLeftShow(i, lineColors[0]);
                }
            }
        }
    }
    public void PlayLineGameFrameOutline()
    {

        if (Mod_Data.getBonus)
        {
            /*** bonus 不在線上 動畫顯示***/
            for (int i = 0; i < Mod_Data.slotReelNum; i++)
            {
                for (int k = 0; k < Mod_Data.slotRowNum; k++)
                {
                    if (Mod_Data.linegame_BonusPos[i, k])
                    {
                        TriggerFrameOutline(i, k, true, 0);
                    }

                }
            }
            /*** bonus 在線上 動畫顯示***/
            //for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
            //{
            //    if (Mod_Data.linegame_HasWinline[i] == true&&Mod_Data.linegame_WinIconID[i]==1)
            //    {
            //        LineImg[i].gameObject.SetActive(true);
            //        LineImg[i].color = lineColors[i];
            //        for (int k = 0; k < Mod_Data.slotReelNum; k++)
            //        {
            //            if (Mod_Data.ReelMath[k, Mod_Data.linegame_Rule[i][k]] == 1)
            //            {
            //                TriggerFrameOutline(k, Mod_Data.linegame_Rule[i][k], true, i);
            //            }
            //        }
            //    }

            //}
        }
        else
        {
            //Debug.Log("PlayLINE");
            StartComparisons(Mod_Data.linegame_Rule, Mod_Data.linegame_WinIconQuantity, Mod_Data.linegame_LineCount, 0, true);
        }

    }

    public void StopAllGameFrameOutline()
    {
        StopAllCoroutines();
        EndComparisons();
        Mod_Data.hasWildPlaySound = false;
        for (int i = 0; i < Mod_Data.slotReelNum; i++)
        {
            for (int k = 0; k < Mod_Data.slotRowNum; k++)
            {
                TriggerFrameOutline(i, k, false); //清除外框
                TriggerIconPlayAnimator(i, k, false);
                //TriggerIconOpenAnimator(i, k, false);
            }
        }
        for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
        {
            LineImg[i].gameObject.SetActive(false);
        }
        lineImage.gameObject.SetActive(false);
        mod_UIController.BannerLeftHide();
        mod_UIController.BannerRightHide();
    }

    public static bool isPlayingDKSpecialAnim = false;//播放辣椒動畫
    public void PlayBonusSpecialTimesAnim()
    {
        if (Mod_Data.BonusSwitch)
        {
            for (int k = 0; k < Mod_Data.slotRowNum; k++)
            {

                if (Mod_Data.ReelMath[1, k] == 12)
                {
                    animator_DKSpecialAnimReel2.gameObject.SetActive(true);
                    DKhead.SetActive(false);
                    animator_DKSpecialAnimReel2.SetInteger("RowIndex", k);
                    animator_DKSpecialAnimReel2.SetTrigger("PlayOnce");
                    isPlayingDKSpecialAnim = true;
                }
                if (Mod_Data.ReelMath[3, k] == 12)
                {
                    animator_DKSpecialAnimReel4.gameObject.SetActive(true);
                    DKhead.SetActive(false);
                    animator_DKSpecialAnimReel4.SetInteger("RowIndex", k);
                    animator_DKSpecialAnimReel4.SetTrigger("PlayOnce");
                    isPlayingDKSpecialAnim = true;
                }
            }
            if (isPlayingDKSpecialAnim) Mod_Data.BonusDelayTimes += 3f;
        }
    }

    public void StopBonusSpecialTimesAnim()
    {
        animator_DKSpecialAnimReel2.gameObject.SetActive(false);
        animator_DKSpecialAnimReel4.gameObject.SetActive(false);
        EndPlayBonusSpecialTimesAnim(Mod_Data.BonusSpecialTimes - 1);
        DKhead.SetActive(true);
        if (Mod_Data.BonusSpecialTimes >= 8) DKhead.GetComponent<Animator>().SetBool("Playhot", true);
        else DKhead.GetComponent<Animator>().SetBool("Playhot", false);
        isPlayingDKSpecialAnim = false;
    }

    public void EndPlayBonusSpecialTimesAnim(int spriteIndex)
    {

        DKSpecialTimesImage.sprite = DKSpecialTimesSprite[spriteIndex];
    }

    public void EnableOrDisabllAllLine(bool open)
    {
        if (open)
        {
            Line50Img.gameObject.SetActive(true);
        }
        else
        {
            Line50Img.gameObject.SetActive(false);
        }
    }

    private Coroutine comparisonRoutine;
    public void StartComparisons(int[][] lingame_WinLine, int[] lingame_WinIconQuantity, int linegame_lineCount, int linegame_lineIndex, bool firstTime)
    {
        // No sense starting it again if its already running.
        if (comparisonRoutine == null)
        {
            comparisonRoutine = StartCoroutine(LineGame_PlayFrameOutlineAnimation(lingame_WinLine, lingame_WinIconQuantity, linegame_lineCount, linegame_lineIndex, firstTime));
        }
    }
    public void EndComparisons()
    {
        if (comparisonRoutine != null)
        {
            StopCoroutine(comparisonRoutine);
            comparisonRoutine = null;
        }
    }
    IEnumerator LineGame_PlayFrameOutlineAnimation(int[][] lingame_WinLine, int[] lingame_WinIconQuantity, int linegame_lineCount, int linegame_lineIndex, bool firstTime)
    //LineGame 跳線動畫   (N線規則,每連線Icon個數,共贏幾條線,初始跳線序)
    {

        if (firstTime)
        {
            if (isPlayingDKSpecialAnim)
            {
                yield return new WaitForSeconds(2.5f);
                StopBonusSpecialTimesAnim();
            }
            if (Mod_Data.hasWildPlaySound) yield return new WaitForSeconds(1f);
            else yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
            {
                if (Mod_Data.linegame_HasWinline[i] == true)
                {
                    //Mod_Data.BonusDelayTimes += 0.05f;
                    LineImg[i].gameObject.SetActive(true);
                    LineImg[i].color = lineColors[i];
                    yield return new WaitForSeconds(0.05f);
                }
            }
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < Mod_Data.linegame_LineCount; i++)
            {
                LineImg[i].gameObject.SetActive(false);
            }
        }
        ////Debug.Log(linegame_lineIndex);


        if (Mod_Data.linegame_HasWinline[linegame_lineIndex] == true)
        {
            for (int i = 0; i <= lingame_WinIconQuantity[linegame_lineIndex]; i++)
            {
                TriggerFrameOutline(i, lingame_WinLine[linegame_lineIndex][i], true, linegame_lineIndex);
            }

            lineImage.gameObject.SetActive(true);
            lineImage.sprite = lineSprite[linegame_lineIndex];

            mod_UIController.BannerLeftShow(linegame_lineIndex, lineColors[linegame_lineIndex]);
            //   //Debug.Log(Mod_Data.linegame_EachLinePay[linegame_lineIndex]);
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < Mod_Data.slotReelNum; i++)
            {
                for (int k = 0; k < Mod_Data.slotRowNum; k++)
                {
                    TriggerFrameOutline(i, k, false);
                }
            }
            lineImage.gameObject.SetActive(false);

            mod_UIController.BannerLeftHide();
        }

        linegame_lineIndex++;
        if (linegame_lineIndex >= linegame_lineCount) linegame_lineIndex = 0;

        EndComparisons();
        StartComparisons(Mod_Data.linegame_Rule, Mod_Data.linegame_WinIconQuantity, Mod_Data.linegame_LineCount, linegame_lineIndex, false);
        //Debug.Log("A" + linegame_lineIndex);
    }

    IEnumerator WayGame_PlayFrameOutlineAnimation(int winlinecount, int winLineLimit, bool isBTRule)  //LineGame 跳線動畫
    {
        int origWinLineCount = winlinecount;
        ////Debug.Log(winlinecount);
        while (true)
        {
            if (winlinecount >= 0)
            {
                if ((!isBTRule && !Mod_Data.isFishChange_BTRule[winlinecount]) || (isBTRule && Mod_Data.isFishChange_BTRule[winlinecount]))
                {
                    int SaveWinLines = winlinecount;  //第幾條線 贏
                    int WinningIconQuantity = Mod_Data.waygame_WinIconQuantity[SaveWinLines]; //贏幾個icon

                    for (int reel = 0; reel < Mod_Data.slotReelNum; reel++)
                    {
                        for (int row = 0; row < Mod_Data.slotRowNum; row++)
                        {
                            if (Mod_Data.waygame_combineIconBool[SaveWinLines, reel, row])//有連線的格子 符合目前指定的ID的格子或=wild(ID=0)
                            {
                                TriggerFrameOutline(reel, row, true, SaveWinLines);//開啟連線外框
                            }
                        }
                    }

                    mod_UIController.BannerLeftShow(SaveWinLines, lineColors[SaveWinLines]);
                }
            }

            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < Mod_Data.slotReelNum; i++)
            {
                for (int k = 0; k < Mod_Data.slotRowNum; k++)
                {
                    TriggerFrameOutline(i, k, false); //清除外框
                }
            }

            mod_UIController.BannerLeftHide();

            yield return new WaitForSeconds(0.15f);

            winlinecount++;
            if (winlinecount >= winLineLimit) winlinecount = origWinLineCount;
        }

    }

}
