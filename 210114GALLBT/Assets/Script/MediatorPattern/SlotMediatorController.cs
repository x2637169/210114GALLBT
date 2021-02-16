using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace DesignPattern_HSMediator
{
    public abstract class AbstractSlotMediator
    {
        protected Mod_Animation m_AnimationController = null;
        protected Mod_GameMath m_GameMath = null;
        protected Mod_MusicController m_MusicController = null;
        protected Mod_UIController m_UiContorller = null;
        protected Slots m_slots = null;
        protected Mod_State m_state = null;
        protected Mod_BonusScript m_BonusScript = null;
        protected BackEnd_Data m_BackEndData = null;
        protected Mod_Client m_client = null;
        protected NewSramManager m_newSramManager = null;
        protected Mod_GameButton m_gameButton = null;
        protected Mod_Animation_BTRule m_Animation_BTRule = null;

        public AbstractSlotMediator(Mod_Animation animationController, Mod_GameMath gameMath, Mod_MusicController musicController, Mod_UIController uiContorller,
        Slots slots, Mod_State state, Mod_BonusScript bonusScript, BackEnd_Data backEndData, Mod_Client clientScript, NewSramManager newSramManager, Mod_GameButton gameButton, Mod_Animation_BTRule animation_BTRule)
        {
            this.m_AnimationController = animationController;
            this.m_GameMath = gameMath;
            this.m_MusicController = musicController;
            this.m_UiContorller = uiContorller;
            this.m_slots = slots;
            this.m_state = state;
            this.m_BonusScript = bonusScript;
            this.m_BackEndData = backEndData;
            this.m_client = clientScript;
            this.m_newSramManager = newSramManager;
            this.m_gameButton = gameButton;
            this.m_Animation_BTRule = animation_BTRule;
        }

        // public abstract void SendMessage(IGameSystem theColleague, string Message);
        public abstract void SendMessage(string theColleague, string Message);
        public abstract void SendMessage(string theColleague, string Message, int inputValue);
    }
    public class ConcreteSlotMediator : AbstractSlotMediator
    {
        public ConcreteSlotMediator(Mod_Animation animationController, Mod_GameMath gameMath, Mod_MusicController musicController, Mod_UIController uiContorller, Slots slots, Mod_State state, Mod_BonusScript bonusScript, BackEnd_Data backEndData, Mod_Client clientScript, NewSramManager newSramManager, Mod_GameButton gameButton, Mod_Animation_BTRule animation_BTRule)
        : base(animationController, gameMath, musicController, uiContorller, slots, state, bonusScript, backEndData, clientScript, newSramManager, gameButton, animation_BTRule)
        {
        }
        #region SetScripts
        public void SetAnimationController(Mod_Animation AnimCtrller)
        {
            m_AnimationController = AnimCtrller;
        }
        public void SetGameMath(Mod_GameMath gameMath)
        {
            m_GameMath = gameMath;
        }
        public void SetMusicController(Mod_MusicController musicController)
        {
            m_MusicController = musicController;
        }
        public void SetUIController(Mod_UIController uiContorller)
        {
            m_UiContorller = uiContorller;
        }
        public void SetSlots(Slots slots)
        {
            m_slots = slots;
        }
        public void SetState(Mod_State state)
        {
            m_state = state;
        }
        public void SetBonusScript(Mod_BonusScript bonusScript)
        {
            m_BonusScript = bonusScript;
        }
        #endregion
        //public override void SendMessage(IGameSystem theColleague, string Message)
        //{
        //    if (theColleague == m_state && Message == "StartRunSlots")
        //    {
        //        m_AnimationController.StopAllGameFrameOutline();
        //        m_AnimationController.PlayBonusSpecialTimesAnim();
        //        m_MusicController.SoundCountClear();
        //        m_MusicController.RollingBGM();
        //        m_slots.SlotStart();

        //    }
        //    else if (theColleague == m_state && Message == "StopRunSlots")
        //    {
        //        m_slots.StopAllReel();
        //        m_MusicController.RollingBGMStop();

        //    }
        //    else if (theColleague == m_state && Message == "UpdateUIscore")
        //    {

        //        m_UiContorller.UpdateScore();

        //    }
        //    else if (theColleague == m_state && Message == "GameMathCount")
        //    {
        //        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        //        {
        //            m_GameMath.WayGame_MathClear();
        //        }
        //        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        //        {
        //            m_GameMath.LineGame_MathClear();
        //        }

        //        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        //        {
        //            m_GameMath.WayGame_CountMath();
        //            if (!Mod_Data.BonusSwitch) m_GameMath.WinScoreCount(Mod_Data.GameMode.BaseGame);
        //            else m_GameMath.WinScoreCount(Mod_Data.GameMode.BonusGame);

        //        }
        //        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        //        {
        //            m_GameMath.LineGame_CountMath();
        //            if (!Mod_Data.BonusSwitch) m_GameMath.WinScoreCount(Mod_Data.GameMode.BaseGame);
        //            else m_GameMath.WinScoreCount(Mod_Data.GameMode.BonusGame);
        //        }

        //    }

        //    else if (theColleague == m_state && Message == "PlayAnimation")
        //    {
        //        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        //        { 
        //            if (Mod_Data.allgame_WinLines > 0)
        //            {
        //                m_AnimationController.PlayWayGameFrameOutline();
        //                m_AnimationController.PlayBonusSpecialTimesAnim();

        //                //m_MusicController.BonusSpecialTimesSound();
        //                m_UiContorller.BannerRightShow(true);
        //            }
        //            else
        //            {
        //                m_UiContorller.BannerRightShow(false);
        //            }
        //        }
        //        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        //        {
        //             m_AnimationController.EndPlayBonusSpecialTimesAnim();//楚河
        //            if (Mod_Data.getBonus)
        //            {
        //                m_AnimationController.PlayLineGameFrameOutline();
        //            }
        //            else
        //            {
        //                if (Mod_Data.allgame_WinLines > 0)
        //                {
        //                    m_AnimationController.PlayLineGameFrameOutline();

        //                   // m_MusicController.BonusSpecialTimesSound();
        //                    m_UiContorller.BannerRightShow(true);
        //                }
        //                else
        //                {
        //                    m_UiContorller.BannerRightShow(false);
        //                }
        //            }

        //        }
        //        for (int i = 0; i < Mod_Data.slotReelNum; i++)
        //        {
        //            for (int k = 0; k < Mod_Data.slotRowNum; k++)
        //            {
        //                if (Mod_Data.allgame_AllCombineIconBool[i, k]) m_AnimationController.TriggerIconOpenAnimator(i, k, true);
        //            }
        //        }
        //        for (int i = 0; i < Mod_Data.slotReelNum; i++)
        //        {
        //            for (int k = 0; k < Mod_Data.slotRowNum; k++)
        //            {
        //                if (Mod_Data.allgame_AllCombineIconBool[i, k]) m_AnimationController.TriggerIconPlayAnimator(i, k, true);
        //            }
        //            //Debug.Log("A");
        //        }

        //        m_MusicController.IconSoundPlay();

        //        //m_UiContorller.StartRollScore(0, Mod_Data.Win);
        //    }//撥放框線動畫

        //    else if (theColleague == m_state && Message == "StartRollScore")//開始跑分
        //    {
        //        if (!Mod_Data.BonusSwitch)
        //        {
        //            m_UiContorller.StartRollScore(0, Mod_Data.Win);
        //        }
        //        else
        //        {
        //            m_UiContorller.StartRollScore(Mod_Data.Win - Mod_Data.Pay, Mod_Data.Win);
        //        }
        //        m_MusicController.s_GoldCoinRolling.Play();

        //    }
        //    else if (theColleague == m_state && Message == "StopRollScore")//停止跑分
        //    {
        //        m_UiContorller.StartRollScore(Mod_Data.Win, Mod_Data.Win);
        //        m_MusicController.s_GoldCoinRolling.Stop();
        //        m_MusicController.s_GoldCoinEnd.Play();
        //    }
        //    else if (theColleague == m_state && Message == "PlayBonusTransAnim")//播放進入Bonus動畫
        //    {
        //        m_AnimationController.BonusTransIn(true);
        //    }
        //    else if (theColleague == m_state && Message == "ShowChoosePanel")//進入Bonus顯示選擇面板
        //    {
        //        m_UiContorller.OpenBonusChoosePanel();
        //    }
        //    else if (theColleague == m_state && Message == "ShowTriggerPanel")//進入Bonus顯示贏得幾場面板
        //    {
        //        m_UiContorller.ShowTriggerPanel(true);
        //    }
        //    else if (theColleague == m_state && Message == "CloseTriggerPanel")//進入Bonus顯示贏得幾場面板
        //    {
        //        m_UiContorller.ShowTriggerPanel(false);
        //    }
        //    else if (theColleague == m_state && Message == "ShowBonusScorePanel")//離開Bonus顯示分數
        //    {
        //        m_UiContorller.OpenBonusScorePanel(true);
        //        m_UiContorller.text_BonusEndScore.text = Mod_Data.Win.ToString();
        //        m_UiContorller.text_CHBonusEndScore_Before.text=(Mod_Data.Win/Mod_Data.CH_BonusSpecialTime).ToString();
        //        m_UiContorller.text_CHSpecialTimes.text = Mod_Data.CH_BonusSpecialTime.ToString();
        //    }
        //    else if ((theColleague == m_BonusScript|| theColleague == m_state) && Message == "BonustransEnd")
        //    //選擇選項後 BonusTransOut  如果需要進Bonus前按按鈕就由BonusScript調用,若直接進入就由state直接調用
        //    {
        //        Mod_Data.startBonus = true;
        //        m_AnimationController.BonusTransIn(false);
        //    }
        //    else if ( theColleague == m_state && Message == "BonusEndtransOut")
        //    //選擇選項後 BonusTransOut  如果需要進Bonus前按按鈕就由BonusScript調用,若直接進入就由state直接調用
        //    {
        //        m_UiContorller.OpenBonusScorePanel(false);
        //        m_AnimationController.BonusTransIn(false);
        //    }
        //    else if (theColleague == m_state && Message == "CheckBonus")//檢查Bonus
        //    {
        //        m_BonusScript.CheckBonus();
        //    }
        //    else if (theColleague == m_BonusScript && Message == "Test")//檢查Bonus
        //    {
        //        m_state.SpecialOrder();
        //    }
        //    else if (theColleague == m_state && Message == "ReregisterState")//檢查Bonus
        //    {
        //        Mod_Data.registerMediator.ReregisterState();
        //    }
        //}
        bool playone;
        public override void SendMessage(string theColleague, string Message)
        {

            if (theColleague == "m_state" && Message == "SetReel")
            {
                m_slots.SlotSetReel();
            }
            if (theColleague == "m_state" && Message == "StartRunSlots")
            {
                m_AnimationController.StopAllGameFrameOutline();
                //m_AnimationController.PlayBonusSpecialTimesAnim();
                //if(Mod_Data.BonusSwitch) m_MusicController.BonusSpecialTimesSound();//楚河
                m_MusicController.SoundCountClear();
                if (!Mod_Data.BonusSwitch) m_MusicController.RollingBGM();
                m_UiContorller.closeDenom();
                m_slots.SlotStart();
            }
            if (theColleague == "m_state" && Message == "StopAllGameFrame")
            {
                if (Mod_Data.inBaseSpin) Mod_Data.Win = 0;
                m_AnimationController.StopAllGameFrameOutline();
                // m_AnimationController.PlayBonusSpecialTimesAnim();
                m_MusicController.SoundCountClear();
                // m_MusicController.RollingBGM();
            }
            else if (theColleague == "m_state" && Message == "StopAllGameFrame_BTRule")
            {
                m_AnimationController.StopAllGameFrameOutline();
                // m_AnimationController.PlayBonusSpecialTimesAnim();
                m_MusicController.SoundCountClear();
                // m_MusicController.RollingBGM();
            }
            if (theColleague == "m_state" && Message == "SaveData")
            {
                // GameObject.Find("BackEndManager").GetComponent<BackEnd_History>().SaveGameValue();
                m_BackEndData.SaveHistoryData();
            }

            if (theColleague == "m_state" && Message == "ComparisonMaxWin")
            {
                m_BackEndData.CompareWinScore();

            }
            if (theColleague == "m_state" && Message == "ComparisonMaxWinInBonus")
            {
                // GameObject.Find("GameController").GetComponent<GameDataManager>().ComparisonMaxWin((Mod_Data.Win), ((Mod_Data.Win) * Mod_Data.Denom), true);
            }
            if (theColleague == "m_state" && Message == "OpenPoint")
            {
                GameObject.Find("GameController").GetComponent<Mod_OpenClearPoint>().OpenPointFunction();
            }
            if (theColleague == "m_state" && Message == "CheckClearPoint")
            {
                GameObject.Find("GameController").GetComponent<Mod_OpenClearPoint>().ClearPointFunction();
            }
            if (theColleague == "m_state" && Message == "StartRemainder")
            {
                m_UiContorller.RemainderShow();
            }
            if (theColleague == "m_state" && Message == "EndBonusShow")
            {
                m_UiContorller.BannerRightShowAfterBonus();
            }
            if (theColleague == "m_state" && Message == "SaveBonusInforamtion")
            {  //儲存bonus倍率與場數資訊
                m_newSramManager.SaveSpeicalTime(Mod_Data.BonusSpecialTimes); //紀錄sram內 斷電用
                m_newSramManager.SaveBonusCount(Mod_Data.BonusCount); //紀錄sram內 斷電用
            }
            else if (theColleague == "m_state" && Message == "StopRunSlots")
            {
                //Debug.Log("call");
                m_slots.StopAllReel();
                m_MusicController.RollingBGMStop();

            }
            else if (theColleague == "m_state" && Message == "UpdateUIscore")
            {

                m_UiContorller.UpdateScore();

            }
            else if (theColleague == "m_state" && Message == "GameMathCount")
            {
                if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
                {
                    m_GameMath.WayGame_MathClear();
                }
                else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
                {
                    m_GameMath.LineGame_MathClear();
                }

                if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
                {
                    m_GameMath.WayGame_CountMath();
                    if (!Mod_Data.BonusSwitch) m_GameMath.WinScoreCount(Mod_Data.GameMode.BaseGame);
                    else m_GameMath.WinScoreCount(Mod_Data.GameMode.BonusGame);

                }
                else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
                {
                    m_GameMath.LineGame_CountMath();
                    if (!Mod_Data.BonusSwitch) m_GameMath.WinScoreCount(Mod_Data.GameMode.BaseGame);
                    else m_GameMath.WinScoreCount(Mod_Data.GameMode.BonusGame);
                }

            }
            #region 播放動畫 "PlayAnimation"

            #region 正常播放動畫
            else if (theColleague == "m_state" && Message == "PlayAnimation")
            {
                for (int p = 0; p < Mod_Data.waygame_combineIconBool.GetLength(0); p++)
                {
                    if (!Mod_Data.isFishChange_BTRule[p])
                    {
                        for (int i = 0; i < Mod_Data.waygame_combineIconBool.GetLength(1); i++)
                        {
                            for (int k = 0; k < Mod_Data.waygame_combineIconBool.GetLength(2); k++)
                            {
                                if (Mod_Data.waygame_combineIconBool[p, i, k])
                                {
                                    m_AnimationController.TriggerIconOpenAnimator(i, k, true);
                                    m_AnimationController.TriggerIconPlayAnimator(i, k, true);
                                }
                            }
                        }
                    }
                }

                // for (int i = 0; i < Mod_Data.slotReelNum; i++)
                // {
                //     for (int k = 0; k < Mod_Data.slotRowNum; k++)
                //     {
                //         if (Mod_Data.allgame_AllCombineIconBool[i, k])
                //         {
                //             m_AnimationController.TriggerIconOpenAnimator(i, k, true);
                //             m_AnimationController.TriggerIconPlayAnimator(i, k, true);
                //         }
                //     }
                // }

                // for (int i = 0; i < Mod_Data.slotReelNum; i++)
                // {
                //     for (int k = 0; k < Mod_Data.slotRowNum; k++)
                //     {
                //         if (Mod_Data.allgame_AllCombineIconBool[i, k]) m_AnimationController.TriggerIconPlayAnimator(i, k, true);
                //     }
                //     //    //Debug.Log("A");
                // }

                if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
                {
                    if (Mod_Data.getBonus)
                    {
                        m_AnimationController.PlayWayGameFrameOutline(false);
                        m_UiContorller.ShowBonusMask(true);
                    }
                    else
                    {
                        if (Mod_Data.allgame_WinLines > 0)
                        {
                            m_AnimationController.PlayWayGameFrameOutline(false);
                            // m_AnimationController.PlayBonusSpecialTimesAnim();

                            //m_MusicController.BonusSpecialTimesSound();
                            m_UiContorller.BannerRightShow(true, Mod_Data.Pay - Mod_Data.BT_Rule_Pay);
                        }
                        else
                        {
                            m_UiContorller.BannerRightShow(false, 0);
                        }
                    }
                }
                else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
                {
                    // m_AnimationController.EndPlayBonusSpecialTimesAnim();//楚河
                    if (Mod_Data.getBonus)
                    {
                        m_AnimationController.PlayLineGameFrameOutline();
                        m_UiContorller.ShowBonusMask(true);
                    }
                    else
                    {
                        if (Mod_Data.allgame_WinLines > 0)
                        {
                            m_AnimationController.PlayLineGameFrameOutline();

                            // m_MusicController.BonusSpecialTimesSound();
                            m_UiContorller.BannerRightShow(true, Mod_Data.Pay);
                        }
                        else
                        {
                            m_UiContorller.BannerRightShow(false, 0);
                        }
                    }

                }


                m_MusicController.IconSoundPlay();

            }//撥放框線動畫
            #endregion

            #region 泡泡缸特殊規則用跑動畫

            else if (theColleague == "m_state" && Message == "FishChangeAnim_BTRule") //換魚動畫
            {
                m_Animation_BTRule.PlayFishChangeAnimtion();
            }
            else if (theColleague == "m_state" && Message == "CloseFishChangeAnim_BTRule") //換魚動畫
            {
                m_Animation_BTRule.EnableFishChangeImages(false);
            }

            else if (theColleague == "m_state" && Message == "PlayAnimation_BTRule")
            {

                for (int p = 0; p < Mod_Data.waygame_combineIconBool.GetLength(0); p++)
                {
                    if (Mod_Data.isFishChange_BTRule[p])
                    {
                        for (int i = 0; i < Mod_Data.waygame_combineIconBool.GetLength(1); i++)
                        {
                            for (int k = 0; k < Mod_Data.waygame_combineIconBool.GetLength(2); k++)
                            {
                                if (Mod_Data.waygame_combineIconBool[p, i, k])
                                {
                                    m_AnimationController.TriggerIconOpenAnimator(i, k, true);
                                    m_AnimationController.TriggerIconPlayAnimator(i, k, true);
                                }
                            }
                        }
                    }
                }

                // for (int i = 0; i < Mod_Data.slotReelNum; i++)
                // {
                //     for (int k = 0; k < Mod_Data.slotRowNum; k++)
                //     {
                //         if (Mod_Data.allgame_AllCombineIconBool[i, k]) m_AnimationController.TriggerIconOpenAnimator(i, k, true);
                //     }
                // }

                // for (int i = 0; i < Mod_Data.slotReelNum; i++)
                // {
                //     for (int k = 0; k < Mod_Data.slotRowNum; k++)
                //     {
                //         if (Mod_Data.allgame_AllCombineIconBool[i, k]) m_AnimationController.TriggerIconPlayAnimator(i, k, true);
                //     }
                // }

                if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
                {
                    if (Mod_Data.getBonus)
                    {
                        m_AnimationController.PlayWayGameFrameOutline(true);
                        m_UiContorller.ShowBonusMask(true);
                    }
                    else
                    {
                        if (Mod_Data.allgame_WinLines > 0)
                        {
                            m_AnimationController.PlayWayGameFrameOutline(true);
                            m_UiContorller.BannerRightShow(true, Mod_Data.BT_Rule_Pay);
                        }
                        else
                        {
                            m_UiContorller.BannerRightShow(false, 0);
                        }
                    }
                }
                m_MusicController.IconSoundPlay();

            }//撥放框線動畫
            #endregion

            #endregion

            #region 跑分

            #region 正常跑分、停止跑分
            else if (theColleague == "m_state" && Message == "StartRollScore")//開始跑分
            {
                if (!Mod_Data.BonusSwitch)
                {
                    m_UiContorller.StartRollScore(0, Mod_Data.Win - Mod_Data.BT_Rule_Pay);
                    if (Mod_Data.Win - Mod_Data.BT_Rule_Pay > 10000) m_UiContorller.BonusCycleEndGarlicCreate(true);
                }
                else
                {
                    m_UiContorller.StartRollScore(Mod_Data.Win - Mathf.CeilToInt((float)Mod_Data.Pay * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))), Mod_Data.Win);
                    if (Mod_Data.Pay > 10000) m_UiContorller.BonusCycleEndGarlicCreate(true);
                }
                m_MusicController.s_GoldCoinRolling.Play();

            }
            else if (theColleague == "m_state" && Message == "StartFastRollScore")//開始跑分
            {
                if (!Mod_Data.BonusSwitch)
                {
                    m_UiContorller.StartFastRollScore(0, Mod_Data.Win - Mod_Data.BT_Rule_Pay);
                    if (Mod_Data.Win - Mod_Data.BT_Rule_Pay > 10000) m_UiContorller.BonusCycleEndGarlicCreate(true);
                }
                else
                {
                    m_UiContorller.StartFastRollScore(Mod_Data.Win - Mathf.CeilToInt((float)Mod_Data.Pay * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))), Mod_Data.Win);
                    if (Mod_Data.Pay > 10000) m_UiContorller.BonusCycleEndGarlicCreate(true);
                }
                m_MusicController.s_GoldCoinRolling.Play();

            }
            else if (theColleague == "m_state" && Message == "StopRollScore")//停止跑分
            {
                m_UiContorller.StartRollScore(Mod_Data.Win - Mod_Data.BT_Rule_Pay, Mod_Data.Win - Mod_Data.BT_Rule_Pay);
                m_UiContorller.BonusCycleEndGarlicCreate(false);
                m_MusicController.s_GoldCoinRolling.Stop();
                m_MusicController.s_GoldCoinEnd.Play();
            }
            #endregion

            #region 泡泡缸特殊規則跑分、停止跑分
            else if (theColleague == "m_state" && Message == "StartRollScore_BTRule")//開始跑分
            {
                if (!Mod_Data.BonusSwitch)
                {
                    m_UiContorller.StartRollScore(Mod_Data.Win - Mod_Data.BT_Rule_Pay, Mod_Data.Win);
                    if (Mod_Data.Win > 10000) m_UiContorller.BonusCycleEndGarlicCreate(true);
                }
                else
                {
                    m_UiContorller.StartRollScore(Mod_Data.Win - Mathf.CeilToInt((float)Mod_Data.Pay * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))), Mod_Data.Win);
                    if (Mod_Data.Pay > 10000) m_UiContorller.BonusCycleEndGarlicCreate(true);
                }
                m_MusicController.s_GoldCoinRolling.Play();

            }
            else if (theColleague == "m_state" && Message == "StartFastRollScore_BTRule")//開始跑分
            {
                if (!Mod_Data.BonusSwitch)
                {
                    m_UiContorller.StartFastRollScore(Mod_Data.Win - Mod_Data.BT_Rule_Pay, Mod_Data.Win);
                    if (Mod_Data.Win > 10000) m_UiContorller.BonusCycleEndGarlicCreate(true);
                }
                else
                {
                    m_UiContorller.StartFastRollScore(Mod_Data.Win - Mathf.CeilToInt((float)Mod_Data.Pay * ((float)((float)Mod_Data.Bet / (float)Mod_Data.BetOri))), Mod_Data.Win);
                    if (Mod_Data.Pay > 10000) m_UiContorller.BonusCycleEndGarlicCreate(true);
                }
                m_MusicController.s_GoldCoinRolling.Play();

            }
            else if (theColleague == "m_state" && Message == "StopRollScore_BTRule")//停止跑分
            {
                m_UiContorller.StartRollScore(Mod_Data.Win, Mod_Data.Win);
                m_UiContorller.BonusCycleEndGarlicCreate(false);
                m_MusicController.s_GoldCoinRolling.Stop();
                m_MusicController.s_GoldCoinEnd.Play();
            }
            #endregion

            #endregion
            else if (theColleague == "m_state" && Message == "PlayBonusTransAnim")//播放進入Bonus動畫
            {
                m_AnimationController.BonusTransIn(true);
                m_MusicController.PlayBonusBackGroundSound();

            }
            else if (theColleague == "m_state" && Message == "PlayBonusBackGroundSound")//播放進入Bonus動畫
            {
                m_MusicController.PlayBonusBackGroundSound();
            }
            else if (theColleague == "m_state" && Message == "PlayBonusTransOutAnim")//播放出來Bonus動畫
            {
                //m_AnimationController.BonusTransIn(true);
                // m_MusicController.PlayBonusBackGroundSound();
            }
            else if (theColleague == "m_state" && Message == "ShowChoosePanel")//進入Bonus顯示選擇面板
            {
                m_UiContorller.OpenBonusChoosePanel();
            }

            else if (theColleague == "m_state" && Message == "ShowStartTriggerPanel")//進入Bonus顯示贏得幾場面板
            {
                m_UiContorller.ShowBonusMask(false);
                //Debug.Log(Mod_Data.BonusCount);
                m_UiContorller.ShowTriggerPanel(Mod_UIController.triggerType.Trigger, true);

            }
            else if (theColleague == "m_state" && Message == "ShowReTriggerPanel")//進入Bonus顯示贏得幾場面板
            {
                m_UiContorller.ShowBonusMask(false);

                if (Mod_Data.BonusCount == Mod_Data.BonusLimit)
                {
                    m_UiContorller.ShowTriggerPanel(Mod_UIController.triggerType.MostRetrigger, true);
                    if (!playone) { m_MusicController.PlayBonusInBonusSound(); playone = true; }
                }
                else
                {
                    m_UiContorller.ShowTriggerPanel(Mod_UIController.triggerType.Retrigger, true);
                    if (!playone) { m_MusicController.PlayBonusInBonusSound(); playone = true; }
                }


            }
            else if (theColleague == "m_state" && Message == "CloseTriggerPanel")//進入Bonus顯示贏得幾場面板
            {
                playone = false;
                m_UiContorller.ShowTriggerPanel(Mod_UIController.triggerType.Trigger, false);
                m_UiContorller.ShowTriggerPanel(Mod_UIController.triggerType.Retrigger, false);
                m_UiContorller.ShowTriggerPanel(Mod_UIController.triggerType.MostRetrigger, false);
                m_UiContorller.ShowBonusMask(false);
            }
            else if (theColleague == "m_state" && Message == "ShowBonusScorePanel")//離開Bonus顯示分數
            {
                m_UiContorller.OpenBonusScorePanel(true);
                m_UiContorller.ShowBonusMask(false);
                m_UiContorller.text_BonusEndScore.text = Mod_Data.Win.ToString();

                m_UiContorller.BonusOutSetWinScoreToZero();
                m_MusicController.PlayBonusEndShowScoreSound();
            }
            else if (theColleague == "m_state" && Message == "BonustransEnd")
            //選擇選項後 BonusTransOut  如果需要進Bonus前按按鈕就由BonusScript調用,若直接進入就由state直接調用
            {
                Mod_Data.startBonus = true;
                m_AnimationController.BonusTransIn(false);
            }
            else if (theColleague == "m_state" && Message == "BonusEndtransOut")
            //選擇選項後 BonusTransOut  如果需要進Bonus前按按鈕就由BonusScript調用,若直接進入就由state直接調用
            {
                m_UiContorller.OpenBonusScorePanel(false);
                m_AnimationController.BonusTransIn(false);
            }
            else if (theColleague == "m_state" && Message == "CheckBonus")//檢查Bonus
            {
                m_BonusScript.CheckBonus();
            }
            else if (theColleague == "m_state" && Message == "PlayDKSpecialAnim")//播放驢子辣椒動畫
            {
                m_AnimationController.PlayBonusSpecialTimesAnim();
                if (Mod_Animation.isPlayingDKSpecialAnim) m_MusicController.BonusSpecialTimesSound();
            }
            else if (theColleague == "m_state" && Message == "StopDKSpecialAnim")//停止驢子辣椒動畫
            {
                m_AnimationController.StopBonusSpecialTimesAnim();
            }




            else if (theColleague == "m_state" && Message == "ReregisterState")//註冊協作者
            {
                Mod_Data.registerMediator.ReregisterState();
            }
            else if (theColleague == "m_state" && Message == "OpenBlankButton")//開啟blank按鈕
            {
                m_UiContorller.OpenBlankButton(true);
            }
            else if (theColleague == "m_state" && Message == "CloseBlankButton")//關閉blank按鈕
            {
                m_UiContorller.OpenBlankButton(false);
            }
            else if (theColleague == "m_state" && Message == "OpenAccount")//開啟帳務系統
            {
                m_UiContorller.OpenBackEnd("Account");
            }
            else if (theColleague == "m_state" && Message == "OpenLogin")//開啟登入介面
            {
                m_UiContorller.OpenBackEnd("Login");
            }
            else if (theColleague == "m_state" && Message == "ErrorWinOpen")
            {
                m_UiContorller.ErrorImageShow("贏分過大(" + Mod_Data.maxWin + ")系統鎖定>設定(" + Mod_Data.maxWin + ")", true);
            }
            else if (theColleague == "m_state" && Message == "ErrorWinClose")
            {
                m_UiContorller.ErrorImageShow("贏分過大(" + Mod_Data.maxWin + ")系統鎖定>設定(" + Mod_Data.maxWin + ")", false);
            }

            else if (theColleague == "m_state" && Message == "ErrorCreditOpen")
            {
                m_UiContorller.ErrorImageShow("彩分過大(" + Mod_Data.maxCredit + ")系統鎖定>設定(" + Mod_Data.maxCredit + ")", true);
            }
            else if (theColleague == "m_state" && Message == "ErrorCreditClose")
            {
                m_UiContorller.ErrorImageShow("彩分過大(" + Mod_Data.maxCredit + ")系統鎖定>設定(" + Mod_Data.maxCredit + ")", false);
            }

            else if (theColleague == "m_state" && Message == "ErrorMonthLockOpen")
            {
                m_UiContorller.ErrorImageShow("Month Error Lock", true);
            }
            else if (theColleague == "m_state" && Message == "ErrorMonthLockClose")
            {
                m_UiContorller.ErrorImageShow("Month Error Lock", false);
            }

            else if (theColleague == "m_state" && Message == "EnableAllLine")
            {
                //m_AnimationController.EnableOrDisabllAllLine(true);
            }
            else if (theColleague == "m_state" && Message == "DisableAllLine")
            {
                //不再餘分時重製得分
                Mod_DataInit.IconPaysLoad();
                m_UiContorller.RemainderUnShown();
                //m_AnimationController.EnableOrDisabllAllLine(false);
            }
            else if (theColleague == "m_slots" && Message == "PlayReelSpeedUp")
            {
                m_MusicController.PlaySpeedUpSound(true);
            }
            else if (theColleague == "m_slots" && Message == "StopReelSpeedUp")
            {
                m_MusicController.PlaySpeedUpSound(false);
            }
            else if (theColleague == "m_state" && Message == "OpenQrcodePanal")
            {
                m_UiContorller.OpenQrcodePanal();
            }
#if Server
            #region Server
            else if (theColleague == "m_state" && Message == "GetLocalGameRound")
            {
                m_client.GetLocalGameRound();
            }
            else if (theColleague == "m_state" && Message == "SaveLocalGameRound")
            {
                m_client.SaveLocalGameRound();
            }
            #endregion
#endif

        }
        public override void SendMessage(string theColleague, string Message, int inputValue)
        {
            if (theColleague == "m_slots" && Message == "PlayBonusReelStop")
            {
                m_MusicController.PlayReelStopSound(inputValue, true);
            }
            else if (theColleague == "m_slots" && Message == "PlayReelStop")
            {
                m_MusicController.PlayReelStopSound(inputValue, false);
            }
            else if (theColleague == "m_state" && Message == "ChangeScene")
            {
                if (inputValue == 0)
                {
                    m_BackEndData.mod_NewLoadGame.ChangeScene(0);
                }
                else
                {
                    m_BackEndData.mod_NewLoadGame.ChangeScene(1);
                }
            }
            else if (theColleague == "m_client" && Message == "OpenPoint")
            {
                GameObject.Find("GameController").GetComponent<Mod_OpenClearPoint>().OpenPointFunction(inputValue);
            }
            else if (theColleague == "m_client" && Message == "ClearPoint")
            {
                GameObject.Find("GameController").GetComponent<Mod_OpenClearPoint>().ClearPointFunction(inputValue);
            }
            else if (theColleague == "m_state" && Message == "memberLock")
            {
                if (inputValue == 0)
                {
                    m_UiContorller.MemberLockImage(false);
                }
                else
                {
                    m_UiContorller.MemberLockImage(true);
                }
            }
#if Server
            #region Server
            else if (theColleague == "m_state" && Message == "ServerWork")
            {
                switch (inputValue)
                {
                    case (int)Mod_Client_Data.messagetype.memberlogin:             //                 701會員登入
                        break;
                    case (int)Mod_Client_Data.messagetype.ticketin:                //                 702入票           ***  
                        break;
                    case (int)Mod_Client_Data.messagetype.ticketout:                //                 703出票            *** 
                        break;
                    case (int)Mod_Client_Data.messagetype.gamehistory:             //                 704遊戲紀錄
                        bool SendSeverHistory = false;
                        Mod_Data.severHistoryLock = true;
                        //Debug.Log("gamehistory: " + " Mod_Data.localRound: " + Mod_Data.localRound + " Mod_Data.gameIndex: " + Mod_Data.gameIndex);
                        if (Mod_Data.StartNotNormal)
                        {
                            if (!Mod_Data.BonusSwitch)
                            {
                                if (Mod_Data.localRound > Mod_Data.gameIndex || Mod_Data.gameIndex == 0) SendSeverHistory = true;
                                else Mod_Data.severHistoryLock = false;
                            }
                            else
                            {
                                if (Mod_Data.localBonusRound > Mod_Data.bonusGameIndex || Mod_Data.bonusGameIndex == 0) SendSeverHistory = true;
                                else Mod_Data.severHistoryLock = false;
                            }
                        }
                        else
                        {
                            SendSeverHistory = true;
                        }
                        if (SendSeverHistory)
                        {
                            if (!Mod_Data.BonusSwitch)
                            {
                                Mod_Data.getRakebackPoint = (float)(Mod_Data.Bet * Mod_Data.odds * Mod_Data.rakeback * Mod_Data.Denom);//反水點數=押注*兌分比
                                Mod_Data.memberRakebackPoint += Mod_Data.getRakebackPoint;
                                Mod_Data.gameIndex++;//遊戲場次
                            }
                            m_client.isCallSendGameHisotry = true;
                        }
                        //m_client.SendToSever(Mod_Client_Data.messagetype.gamehistory);
                        break;
                    case (int)Mod_Client_Data.messagetype.gameset:                //                705遊戲初始值設定
                        break;
                    case (int)Mod_Client_Data.messagetype.billin:                 //                706入鈔
                        break;
                    case (int)Mod_Client_Data.messagetype.billclear:              //                707清鈔
                        break;
                    case (int)Mod_Client_Data.messagetype.machineevent:           //                708機台事件
                        break;
                    case (int)Mod_Client_Data.messagetype.machinelive:           //                 709 machine live
                        break;
                    case (int)Mod_Client_Data.messagetype.connect:                //                710連線
                        break;
                    case (int)Mod_Client_Data.messagetype.handpay:                //                711手付
                        break;
                    case (int)Mod_Client_Data.messagetype.handpayreset:           //                712手付重製reset
                        break;
                    case (int)Mod_Client_Data.messagetype.memberlogout:           //                715玩家登出
                        break;
                    case (int)Mod_Client_Data.messagetype.exchangePoints:           //                716兌分                                                     
                        break;
                    case (int)Mod_Client_Data.messagetype.ticketoutsuccess:           //                717出票成功                                                      
                        break;
                    default:
                        //Debug.Log("errorOrder");
                        break;
                }
            }
            #endregion
#endif

        }

    }
}