using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SlotSymbol : MonoBehaviour
{
    public int iconID;
    public int indexPos;
    public bool changeSprite = false, bottomIcon = false, specialChangeIcon = false;
    Animator animator;
    Image image;
    private void Start()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
       // ChageIconImageByiconID(0);
    }
    private void Update()
    {
       
    }

    public void PlayIconAnimation(bool playAnim)
    {
        animator.SetInteger("IconID", iconID);
        animator.SetBool("IconAnimationPlay", playAnim);
        animator.SetBool("InBonus", Mod_Data.BonusSwitch);
        if (!playAnim) ChageIconImageByiconID(iconID);
      //  //Debug.Log(this.name);
    }
    public void ChageIconImageByiconID(int setIconID)
    {
        if(Mod_Data.iconDatabase.GetByID(setIconID)!=null)
        image.sprite = Mod_Data.iconDatabase.GetByID(setIconID).Sprite;
       // //Debug.Log(Mod_Data.iconDatabase.GetByID(setIconID).Sprite.name);
    }
}
