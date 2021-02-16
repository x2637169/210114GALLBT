using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_ReelStopTrigger : MonoBehaviour
{
    public Reel reel;
    public bool stopReel;

    public void OnTriggerEnter(Collider other)
    {
        //    if (other.tag == "StopIcon")
        //    {
        //        if (other.GetComponent<SlotSymbol>().changeSprite) {
        //            //停止並定位滾輪
        //            reel.spin = false;
        //            reel.SetPosition();
        //            GameObject.Find("GameController").GetComponent<Mod_MusicController>().ReelStopSound(reel.name);
        //            other.GetComponent<SlotSymbol>().changeSprite = false;
        //        }
        //    }
        if (stopReel) {
            other.GetComponent<SlotSymbol>().bottomIcon = true;
            reel.bottomIconPos = other.GetComponent<SlotSymbol>().indexPos;
            reel.IconSpriteFinalLoad(other.GetComponent<SlotSymbol>().indexPos);
            stopReel = false;
            this.gameObject.SetActive(false);
            //  IconCount = 0;
            // other.GetComponent<SlotSymbol>().changeSprite = true;
        }

        if (other.GetComponent<SlotSymbol>().bottomIcon)
        {
            //停止並定位滾輪
            reel.spin = false;
            //reel.SetPosition();
            //GameObject.Find("GameController").GetComponent<Mod_MusicController>().ReelStopSound(reel.name);
            other.GetComponent<SlotSymbol>().changeSprite = false;
            //other.GetComponent<SlotSymbol>().bottomIcon = false;
        }
    }
  
}
