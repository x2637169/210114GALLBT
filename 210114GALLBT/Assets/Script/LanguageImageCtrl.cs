using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LanguageImageCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    int nowLanguageNum, nowBonusNum;
    bool nowBonusSwitch;
    [SerializeField] Sprite[] languageSprite,BonusSprite;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (nowLanguageNum != (int)Mod_Data.language)
        {
            nowLanguageNum = (int)Mod_Data.language;

            if (GetComponent<Animator>() != null) GetComponent<Animator>().SetBool("isEnglishLanguage", nowLanguageNum == 1?true:false);
            if (languageSprite.Length != 0)
            { 
                if (GetComponent<Image>() != null) GetComponent<Image>().sprite = languageSprite[nowLanguageNum];
                else if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().sprite = languageSprite[nowLanguageNum];
            }
        }
        if (nowBonusSwitch != Mod_Data.BonusSwitch)
        {
            nowBonusSwitch = Mod_Data.BonusSwitch;
            nowBonusNum = nowBonusSwitch == false ? 0 : 1;
            
            if (GetComponent<Animator>() != null) GetComponent<Animator>().SetBool("isBonusMode", nowBonusSwitch);

            if (BonusSprite.Length != 0)
            {
                if (GetComponent<Image>() != null)
                {
                    GetComponent<Image>().sprite = BonusSprite[nowBonusNum];
                    //Debug.Log(this.name + nowBonusNum);
                }
                else if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().sprite = BonusSprite[nowBonusNum];
                
            }
        }
        
    }
}
