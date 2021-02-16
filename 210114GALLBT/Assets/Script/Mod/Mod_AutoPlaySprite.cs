using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Mod_AutoPlaySprite : MonoBehaviour
{
    [SerializeField] Sprite autoPlay_C, autoPlay_E, stopPlay_C, stopPlay_E;
    bool tmpAuto;
    int nowLanguageNum;
    Image thisImg;
    // Start is called before the first frame update
    void Start()
    {
        tmpAuto = false;
        thisImg = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nowLanguageNum != (int)Mod_Data.language)
        {
            nowLanguageNum = (int)Mod_Data.language;

            if (nowLanguageNum == 0)
            {
                if (Mod_Data.autoPlay)
                    thisImg.sprite = stopPlay_C;
                else
                    thisImg.sprite = autoPlay_C;
            }
            else if(nowLanguageNum == 1){
                if (Mod_Data.autoPlay)
                    thisImg.sprite = stopPlay_E;
                else
                    thisImg.sprite = autoPlay_E;
            }
        }
        if (tmpAuto != Mod_Data.autoPlay)
        {
            tmpAuto = Mod_Data.autoPlay;

            if (nowLanguageNum == 0)
            {
                if (Mod_Data.autoPlay)
                    thisImg.sprite = stopPlay_C;
                else
                    thisImg.sprite = autoPlay_C;
            }
            else if (nowLanguageNum == 1)
            {
                if (Mod_Data.autoPlay)
                    thisImg.sprite = stopPlay_E;
                else
                    thisImg.sprite = autoPlay_E;
            }
        }
    }
}
