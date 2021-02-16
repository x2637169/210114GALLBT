using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Newtonsoft.Json.Linq;
using Random = System.Random;


public class EditorCtrl : MonoBehaviour
{
    public bool[,] slotpos = new bool[5, 3];
    Mod_Animation mod_Animation;
    Mod_UIController mod_UIController;
    public bool playBool,playReel; 
    private void Start()
    {
        mod_Animation = GetComponent<Mod_Animation>();
        mod_UIController = GetComponent<Mod_UIController>();
        //Debug.Log(Mod_Data.currentGameRule);

  
    }
    private void Update()
    {

        //    if (Mod_Data.reelAllStop)
        //    {

        //        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        //        {
        //            GetComponent<Mod_GameMath>().WayGame_MathClear();
        //        }
        //        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        //        {
        //            GetComponent<Mod_GameMath>().LineGame_MathClear();
        //        }

        //        if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.WayGame)
        //        {
        //            GetComponent<Mod_GameMath>().WayGame_CountMath();
        //            GetComponent<Mod_GameMath>().WinScoreCount(Mod_Data.GameMode.BaseGame);
        //            if (Mod_Data.allgame_WinLines > 0)
        //            {
        //                mod_Animation.PlayWayGameFrameOutline();
        //                mod_UIController.BannerRightShow(true);
        //            }
        //            else
        //            {
        //                mod_UIController.BannerRightShow(false);
        //            }
        //        }
        //        else if (Mod_Data.currentGameRule == Mod_Data.SlotGameRule.LineGame)
        //        {
        //            GetComponent<Mod_GameMath>().LineGame_CountMath();
        //            GetComponent<Mod_GameMath>().WinScoreCount(Mod_Data.GameMode.BaseGame);
        //            if (Mod_Data.allgame_WinLines > 0)
        //            {
        //                mod_Animation.PlayLineGameFrameOutline();
        //                mod_UIController.BannerRightShow(true);
        //            }
        //            else
        //            {
        //                mod_UIController.BannerRightShow(false);
        //            }
        //        }



        //        for (int i = 0; i < 5; i++)
        //        {
        //            for (int k = 0; k < 3; k++)
        //            {

        //                if (Mod_Data.allgame_AllCombineIconBool[i, k]) mod_Animation.TriggerIconOpenAnimator(i, k, true);

        //            }
        //        }
        //        for (int i = 0; i < 5; i++)
        //        {
        //            for (int k = 0; k < 3; k++)
        //            {

        //                if (Mod_Data.allgame_AllCombineIconBool[i, k]) mod_Animation.TriggerIconPlayAnimator(i, k, true);

        //            }
        //            //Debug.Log("A");
        //        }

        //        Mod_Data.reelAllStop = false;
        //    }
    }


}
