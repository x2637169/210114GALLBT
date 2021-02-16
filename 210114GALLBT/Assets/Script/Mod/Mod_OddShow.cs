using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Mod_OddShow : MonoBehaviour
{

    [SerializeField] Image image_Bet1, image_Bet2, image_Bet5, image_Bet10, image_Bet25;
    [SerializeField] Image image_Reel1, image_Reel2, image_Reel3, image_Reel4, image_Reel5;
    [SerializeField] Text text_Bet2, text_Bet3, text_Bet6, text_Bet11;
    [SerializeField] Font basefont, bonusfont;
    // Start is called before the first frame update
    double tempOdd = 99;
    int tempBet = 99;

    void Start()
    {
        
    }

    // Update is called once per frame

    bool nowBonusSwitch = true;
    void Update()
    {
        if (tempOdd != Mod_Data.odds)
        {
            tempOdd = Mod_Data.odds;
            ChangeOddShow(tempOdd);
        }

        if(tempBet != Mod_Data.Bet) {
            tempBet = Mod_Data.Bet;
            ChangeReelShow(tempBet);
        }
        if(Mod_Data.BonusSwitch!= nowBonusSwitch)
        {
            nowBonusSwitch = Mod_Data.BonusSwitch;

            if (Mod_Data.BonusSwitch)
            {
                text_Bet3.font = bonusfont;
                text_Bet6.font = bonusfont;
                text_Bet11.font = bonusfont;
            }
            else
            {
                text_Bet3.font = basefont;
                text_Bet6.font = basefont;
                text_Bet11.font = basefont;
            }
        }
    }

    void ChangeOddShow(double tempOdd) {

        if (tempOdd == 1) {
            image_Bet1.enabled = true;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
        }
        else if (tempOdd == 2) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = true;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
            //text_Bet2.text = tempOdd.ToString();
        }
        else if (tempOdd == 3 || tempOdd == 4) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = true;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
            text_Bet3.text = tempOdd.ToString();
        }
        else if (tempOdd == 5) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = true;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
            //text_Bet3.text = tempOdd.ToString();
        }
        else if (tempOdd >= 6 && tempOdd <= 9) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = true;
            text_Bet11.enabled = false;
            text_Bet6.text = tempOdd.ToString();
        }
        else if (tempOdd == 10) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = true;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
            //text_Bet6.text = tempOdd.ToString();
        }
        else if (tempOdd >= 11 && tempOdd <= 24) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = false;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = true;
            text_Bet11.text = tempOdd.ToString();
        }
        else if (tempOdd == 25) {
            image_Bet1.enabled = false;
            image_Bet2.enabled = false;
            image_Bet5.enabled = false;
            image_Bet10.enabled = false;
            image_Bet25.enabled = true;
            text_Bet2.enabled = false;
            text_Bet3.enabled = false;
            text_Bet6.enabled = false;
            text_Bet11.enabled = false;
        }
    }
    //左側"滾輪"顯示
    void ChangeReelShow(int tempBet) {
        if(tempBet >= 25) {
            image_Reel1.enabled = false;
            image_Reel2.enabled = false;
            image_Reel3.enabled = false;
            image_Reel4.enabled = false;
            image_Reel5.enabled = true;
        }
        else if(tempBet == 15) {
            image_Reel1.enabled = false;
            image_Reel2.enabled = false;
            image_Reel3.enabled = false;
            image_Reel4.enabled = true;
            image_Reel5.enabled = false;
        }
        else if(tempBet == 9) {
            image_Reel1.enabled = false;
            image_Reel2.enabled = false;
            image_Reel3.enabled = true;
            image_Reel4.enabled = false;
            image_Reel5.enabled = false;
        }
        else if(tempBet == 3) {
            image_Reel1.enabled = false;
            image_Reel2.enabled = true;
            image_Reel3.enabled = false;
            image_Reel4.enabled = false;
            image_Reel5.enabled = false;
        }
        else if(tempBet == 1) {
            image_Reel1.enabled = true;
            image_Reel2.enabled = false;
            image_Reel3.enabled = false;
            image_Reel4.enabled = false;
            image_Reel5.enabled = false;
        }
    }
}
