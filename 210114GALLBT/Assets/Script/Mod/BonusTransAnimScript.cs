using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusTransAnimScript : StateMachineBehaviour
{


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (stateInfo.IsName("Anim_BonusTransOut"))
        {
            Mod_Data.transInAnimEnd = true;
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (stateInfo.IsName("Anim_BonusTransIn"))
        //{
        //    GameObject.Find("GameController").GetComponent<Mod_UIController>().OpenBonusChoosePanel();
        //    Mod_Data.BonusSwitch = true;
        //}
        if (stateInfo.IsName("Anim_BonusTransIn"))
        {
            Mod_Data.transInAnimEnd = true;
        }

    }


}