using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_MusicController : IGameSystem
{
    LoadMusic loadMusic;
    public AudioSource[] ReelStop, RollingBackground, BnousIcon;
    public AudioSource S_SpeedUp, s_GoldCoinRolling, s_GoldCoinStart,s_GoldCoinEnd,s_Bonus,s_Wild,s_P1,s_P2,s_P3,s_P4,s_SBonus,s_SWild,bonusBackGround,bonusEndShowScore;
    public AudioSource s_SpecialAnimSound,s_one,s_button;
    int RollingBgmCount = 0;
    bool BonusIcon;
    int ReelBonusIconCount = 0;
    int ReelMusicCount = 0;
    
 
    //聲音大小
    public float[] VolumeList = new float[4];

    // Start is called before the first frame update
    void Start()
    {
        SoundCountClear();
        loadMusic = Camera.main.GetComponent<LoadMusic>();
        s_GoldCoinRolling = loadMusic.S_GoldCoinRolling;
        s_GoldCoinStart = loadMusic.S_GoldCoinStart;
        s_GoldCoinEnd = loadMusic.S_GoldCoinEnd;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlayButtonSound()
    {
        s_button.PlayOneShot(s_button.clip);
    }
    //背景音樂
    public void RollingBGM()
    {
        RollingBackground[RollingBgmCount].Play();
        RollingBgmCount++;
        if (RollingBgmCount > 2)
        {
            RollingBgmCount = 0;
        }
    }

    //背景音樂停止
    public void RollingBGMStop()
    {
        int StopNumber = RollingBgmCount - 1;
        if (StopNumber < 0) { StopNumber = 2; }
        RollingBackground[StopNumber].Stop();
    }

    //判斷滾輪停止音&Bonus音
    public void ReelStopSound(string ReelName)
    {
        if(Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3) { 
        switch (ReelName)
        {
            case "Reel1":
                if (Mod_Data.ReelMath[0, 0] == 1 || Mod_Data.ReelMath[0, 1] == 1 || Mod_Data.ReelMath[0, 2] == 1)
                {
                    BonusIcon = true;
                    ReelBonusIconSound();
                }
                else
                {
                    ReelStopPlay();
                }
                break;

            case "Reel2":
                if (Mod_Data.ReelMath[1, 0] == 1 || Mod_Data.ReelMath[1, 1] == 1 || Mod_Data.ReelMath[1, 2] == 1)
                {
                    if (BonusIcon)
                    {
                        ReelBonusIconSound();
                    }
                    else
                    {
                        BonusIcon = false;
                        ReelStopPlay();
                    }
                }
                else
                {
                    BonusIcon = false;
                    ReelStopPlay();
                }
                break;

            case "Reel3":
                if (Mod_Data.ReelMath[2, 0] == 1 || Mod_Data.ReelMath[2, 1] == 1 || Mod_Data.ReelMath[2, 2] == 1)
                {
                    if (BonusIcon)
                    {
                        ReelBonusIconSound();
                    }
                    else
                    {
                        BonusIcon = false;
                        ReelStopPlay();
                    }
                }
                else
                {
                    BonusIcon = false;
                    ReelStopPlay();
                }
                break;

            case "Reel4":
                BonusIcon = false;
                ReelStopPlay();
                break;

            case "Reel5":
                ReelStopPlay();
                break;
        }
        }
        if(Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5) {
            switch (ReelName) {
                case "Reel1":
                    if (Mod_Data.ReelMath[0, 0] == 1 || Mod_Data.ReelMath[0, 1] == 1 || Mod_Data.ReelMath[0, 2] == 1 || Mod_Data.ReelMath[0, 3] == 1) {
                        ReelBonusIconSound();
                    }
                    else {
                        ReelStopPlay();
                    }
                    break;

                case "Reel2":
                    if (Mod_Data.ReelMath[1, 0] == 1 || Mod_Data.ReelMath[1, 1] == 1 || Mod_Data.ReelMath[1, 2] == 1 || Mod_Data.ReelMath[1, 3] == 1) {
                        ReelBonusIconSound();
                    }
                    else {
                        ReelStopPlay();
                    }
                    break;

                case "Reel3":
                    if (Mod_Data.ReelMath[2, 0] == 1 || Mod_Data.ReelMath[2, 1] == 1 || Mod_Data.ReelMath[2, 2] == 1 || Mod_Data.ReelMath[2, 3] == 1) {
                        ReelBonusIconSound();
                    }
                    else {
                        ReelStopPlay();
                    }
                    break;

                case "Reel4":
                    if (Mod_Data.ReelMath[3, 0] == 1 || Mod_Data.ReelMath[3, 1] == 1 || Mod_Data.ReelMath[3, 2] == 1 || Mod_Data.ReelMath[3, 3] == 1) {
                        ReelBonusIconSound();
                    }
                    else {
                        ReelStopPlay();
                    }
                    break;

                case "Reel5":
                    if (Mod_Data.ReelMath[4, 0] == 1 || Mod_Data.ReelMath[4, 1] == 1 || Mod_Data.ReelMath[4, 2] == 1 || Mod_Data.ReelMath[4, 3] == 1) {
                        ReelBonusIconSound();
                    }
                    else {
                        ReelStopPlay();
                    }
                    break;
            }
        }
        if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4) {
            switch (ReelName) {
                case "Reel1":
                    ReelStopPlay();
                    break;
                case "Reel2":
                    if (Mod_Data.ReelMath[1, 0] == 1 || Mod_Data.ReelMath[1, 1] == 1 || Mod_Data.ReelMath[1, 2] == 1) {
                        BonusIcon = true;
                        ReelBonusIconSound();
                    }
                    else {
                        ReelStopPlay();
                    }
                    break;

                case "Reel3":
                    if (Mod_Data.ReelMath[2, 0] == 1 || Mod_Data.ReelMath[2, 1] == 1 || Mod_Data.ReelMath[2, 2] == 1) {
                        if (BonusIcon) {
                            ReelBonusIconSound();
                        }
                        else {
                            BonusIcon = false;
                            ReelStopPlay();
                        }
                    }
                    else {
                        BonusIcon = false;
                        ReelStopPlay();
                    }
                    break;

                case "Reel4":
                    if (Mod_Data.ReelMath[3, 0] == 1 || Mod_Data.ReelMath[3, 1] == 1 || Mod_Data.ReelMath[3, 2] == 1) {
                        if (BonusIcon) {
                            ReelBonusIconSound();
                        }
                        else {
                            BonusIcon = false;
                            ReelStopPlay();
                        }
                    }
                    else {
                        BonusIcon = false;
                        ReelStopPlay();
                    }
                    break;

                case "Reel5":
                    BonusIcon = false;
                    ReelStopPlay();
                    break;
            }
        }
        if (Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5) {
            switch (ReelName) {
                case "Reel1":
                    BonusIcon = false;
                    if (Mod_Data.ReelMath[0, 0] == 1 || Mod_Data.ReelMath[0, 1] == 1 || Mod_Data.ReelMath[0, 2] == 1) {
                        BonusIcon = true;
                        ReelBonusIconSound();
                    }
                    else {
                        ReelStopPlay();
                    }
                    break;

                case "Reel2":
                    if (Mod_Data.ReelMath[1, 0] == 1 || Mod_Data.ReelMath[1, 1] == 1 || Mod_Data.ReelMath[1, 2] == 1) {
                        if (BonusIcon) {
                            ReelBonusIconSound();
                        }
                        else {
                            BonusIcon = false;
                            ReelStopPlay();
                        }
                    }
                    else {
                        BonusIcon = false;
                        ReelStopPlay();
                    }
                    break;

                case "Reel3":
                    if (Mod_Data.ReelMath[2, 0] == 1 || Mod_Data.ReelMath[2, 1] == 1 || Mod_Data.ReelMath[2, 2] == 1) {
                        if (BonusIcon) {
                            ReelBonusIconSound();
                        }
                        else {
                            BonusIcon = false;
                            ReelStopPlay();
                        }
                    }
                    else {
                        BonusIcon = false;
                        ReelStopPlay();
                    }
                    break;

                case "Reel4":
                    if (Mod_Data.ReelMath[3, 0] == 1 || Mod_Data.ReelMath[3, 1] == 1 || Mod_Data.ReelMath[3, 2] == 1) {
                        if (BonusIcon) {
                            ReelBonusIconSound();
                        }
                        else {
                            BonusIcon = false;
                            ReelStopPlay();
                        }
                    }
                    else {
                        BonusIcon = false;
                        ReelStopPlay();
                    }
                    break;
                case "Reel5":
                    if (Mod_Data.ReelMath[4, 0] == 1 || Mod_Data.ReelMath[4, 1] == 1 || Mod_Data.ReelMath[4, 2] == 1) {
                        if (BonusIcon) {
                            ReelBonusIconSound();
                        }
                        else {
                            BonusIcon = false;
                            ReelStopPlay();
                        }
                    }
                    else {
                        BonusIcon = false;
                        ReelStopPlay();
                    }
                    break;
            }
        }
    }


    public void PlayReelStopSound(int ReelMusicCount, bool isbonus)
    {
        if (isbonus)
        {
            BnousIcon[ReelMusicCount].Play();
        }
        else
        {
            ReelStop[ReelMusicCount].Play();
        }
    }

    public void PlaySpeedUpSound(bool isPlay)
    {
        if (isPlay) S_SpeedUp.Play();
        else S_SpeedUp.Stop();
    }


    //滾輪停止音
    public void ReelStopPlay()
    {
        if(Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3) {
            S_SpeedUp.Stop();
            ReelStop[ReelMusicCount].Play();
            ReelMusicCount++;

            if (ReelBonusIconCount + ReelMusicCount == 3) {
                S_SpeedUp.Stop();
            }
        }
        if(Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5) {
            if (S_SpeedUp.isPlaying) {
                S_SpeedUp.Play();
                ReelStop[ReelMusicCount].Play();
                ReelMusicCount++;
            }
            else {
                ReelStop[ReelMusicCount].Play();
                ReelMusicCount++;

            }

            if (ReelBonusIconCount + ReelMusicCount >= 5) {
                S_SpeedUp.Stop();
            }
        }
        if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4) {
            ReelStop[ReelMusicCount].Play();
            //S_SpeedUp.Stop();
            ReelMusicCount++;

            if (ReelBonusIconCount + ReelMusicCount == 5) {
                S_SpeedUp.Stop();
            }
        }
        if(Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5) {
            ReelStop[ReelMusicCount].Play();
            //S_SpeedUp.Stop();
            ReelMusicCount++;

            if (ReelBonusIconCount + ReelMusicCount >= 5) {
                S_SpeedUp.Stop();
            }
        }
    }

    //Bonus延長音
    public void ReelBonusIconSound()
    {
        if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3) {
            BnousIcon[ReelBonusIconCount].Play();
            ReelBonusIconCount++;
            if (ReelBonusIconCount == 2 && (ReelBonusIconCount + ReelMusicCount < 3)) {
                S_SpeedUp.Play();
            }
        }
        if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5) {
            BnousIcon[ReelBonusIconCount].Play();
            ReelBonusIconCount++;
            if (ReelBonusIconCount >= 2 && (ReelBonusIconCount + ReelMusicCount < 5)) {
                Mod_Data.ReelSpeedChange = true;
                S_SpeedUp.Play();
            }
            else {
                S_SpeedUp.Stop();
            }
        }
        if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4) {
            BnousIcon[ReelBonusIconCount].Play();
            ReelBonusIconCount++;
            if (ReelBonusIconCount >= 2 && (ReelBonusIconCount + ReelMusicCount < 4)) {
                S_SpeedUp.Play();
            }
            else {
                S_SpeedUp.Stop();
            }
        }
        if(Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5) {
            BnousIcon[ReelBonusIconCount].Play();
            ReelBonusIconCount++;
            if (ReelBonusIconCount >= 2 && (ReelBonusIconCount + ReelMusicCount < 5)) {
                S_SpeedUp.Play();
            }
            else {
                S_SpeedUp.Stop();
            }
        }
    }

    //初始化
    public void SoundCountClear()
    {
        ReelMusicCount = 0;
        ReelBonusIconCount = 0;
    }

    //個別遊戲Icon音效調整

    public void IconSoundPlay()
    {
        for (int i = 0; i < Mod_Data.allgame_WinLines; i++)
        {
            if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
            {
                if (Mod_Data.hasWildPlaySound) s_Wild.Play();
                if (Mod_Data.hasSWildPlaySound) s_SWild.Play();

            }
            else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
            {
                if (Mod_Data.hasWildPlaySound) s_Wild.Play();
               // if (Mod_Data.waygame_WinIconID[i] == 1) s_Bonus.Play();
                if (Mod_Data.waygame_WinIconID[i] == 2) s_P1.Play();
                if (Mod_Data.waygame_WinIconID[i] == 3) s_P2.Play();
                if (Mod_Data.waygame_WinIconID[i] == 4) s_P3.Play();
                if (Mod_Data.waygame_WinIconID[i] == 5) s_P4.Play();
            }

        }
        if (Mod_Data.getBonus)
        {
            if(!Mod_Data.BonusSwitch)s_Bonus.Play();
            else s_SBonus.Play();
            ////Debug.Log("Music");
        }
    }

    public void BonusSpecialTimesSound()
    {
        //if (Mod_Data.BonusSpecialTimes == 2 || Mod_Data.BonusSpecialTimes == 10) s_SpecialTimesTwo.Play();
        //if (Mod_Data.BonusSpecialTimes == 3 || Mod_Data.BonusSpecialTimes == 15) s_SpecialTimesThree.Play();
        //if (Mod_Data.BonusSpecialTimes == 5 || Mod_Data.BonusSpecialTimes == 30) s_SpecialTimesFive.Play();
        s_SpecialAnimSound.Play();
        //Debug.Log("AAAAAAA00");
    }
    public void PlayBonusBackGroundSound()
    {
        bonusBackGround.Play();
    }
    public void PlayBonusEndShowScoreSound()
    {
        bonusBackGround.Stop();
        bonusEndShowScore.PlayOneShot(bonusEndShowScore.clip);
    }
    public void PlayBonusInBonusSound()
    {
        s_one.PlayOneShot(s_one.clip);
    }
}
