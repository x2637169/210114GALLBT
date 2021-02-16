using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_IconChangeTrigger : MonoBehaviour
{
    public Reel reel;
    int IconCount = 0;
    public GameObject ChangeLine;
    public bool stopReel, specialChange;
    public void OnTriggerEnter(Collider other)
    {
        // reel.IconSpriteReload(other.GetComponent<SlotSymbol>().indexPos);

        //if (IconCount > Mod_Data.slotRowNum)
        //{
        //    if (!other.GetComponent<SlotSymbol>().changeSprite) reel.IconSpriteReload(other.GetComponent<SlotSymbol>().indexPos);
        //    else other.GetComponent<SlotSymbol>().changeSprite = false;
        //}
        //IconCount++;
        if (specialChange)
        {
            reel.IconSpriteSpecialLoad(other.GetComponent<SlotSymbol>().indexPos);
            specialChange = false;
        }
        if (stopReel)
        {
            other.GetComponent<SlotSymbol>().bottomIcon = true;
            reel.bottomIconPos = other.GetComponent<SlotSymbol>().indexPos;
            reel.IconSpriteFinalLoad(other.GetComponent<SlotSymbol>().indexPos);
            stopReel = false;
            this.gameObject.SetActive(false);
            //IconCount = 0;
            // other.GetComponent<SlotSymbol>().changeSprite = true;
        }
        else if ((reel.name == "Reel1" || reel.name == "Reel2" || reel.name == "Reel3")) {
            if (other.GetComponent<SlotSymbol>().iconID == 1 || other.GetComponent<SlotSymbol>().iconID == 2) {
                if (!other.GetComponent<SlotSymbol>().changeSprite) reel.IconSpriteLoadWithoutBonus(other.GetComponent<SlotSymbol>().indexPos);
                else other.GetComponent<SlotSymbol>().changeSprite = false;
            }
        }


    }
}
