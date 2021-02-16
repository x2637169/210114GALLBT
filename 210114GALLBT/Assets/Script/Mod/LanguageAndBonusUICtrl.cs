using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LanguageAndBonusUICtrl : MonoBehaviour
{
    [SerializeField] bool changeByLanguage, changeByBonus, baseSetActive, bonusSetAcive;

    int nowLanguageNum = 3, nowBonusNum = 3;
    bool nowBonusSwitch = false;
    public Sprite[] changeSprite;
    [SerializeField] GameObject[] setActiveObject;
    [SerializeField] GameObject[] setServerDeActiveObject;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (changeByLanguage && changeByBonus) ChangeByBoth();
        else if (changeByLanguage) ChangeByLanguage();
        else if (changeByBonus)
        {
            ChangeByBonus();
            if (GetComponent<Animator>() != null && GetComponent<Animator>().GetBool("isBonusMode") != Mod_Data.BonusSwitch)
            {
                GetComponent<Animator>().SetBool("isBonusMode", Mod_Data.BonusSwitch);
            }
        }
    }

    void ChangeByLanguage()
    {
        if (nowLanguageNum != (int)Mod_Data.language)
        {
            nowLanguageNum = (int)Mod_Data.language;

            if (GetComponent<Animator>() != null) GetComponent<Animator>().SetBool("isEnglishLanguage", nowLanguageNum == 1 ? true : false);
            if (changeSprite.Length != 0)
            {
                if (GetComponent<Image>() != null) GetComponent<Image>().sprite = changeSprite[nowLanguageNum];
                else if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().sprite = changeSprite[nowLanguageNum];
            }
        }
    }
    void ChangeByBonus()
    {
        if (nowBonusSwitch != Mod_Data.BonusSwitch)
        {
            nowBonusSwitch = Mod_Data.BonusSwitch;
            nowBonusNum = nowBonusSwitch == false ? 0 : 1;

            if (GetComponent<Animator>() != null) GetComponent<Animator>().SetBool("isBonusMode", nowBonusSwitch);

            if (changeSprite.Length != 0)
            {
                if (GetComponent<Image>() != null) GetComponent<Image>().sprite = changeSprite[nowBonusNum];
                else if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().sprite = changeSprite[nowBonusNum];
            }

            if (baseSetActive)
            {
                if (!nowBonusSwitch) setObjectActive(true);
                else setObjectActive(false);
            }
            if (bonusSetAcive)
            {
                if (nowBonusSwitch) setObjectActive(true);
                else setObjectActive(false);
            }

#if Server

#else
            SetServerObjectDeActive(false);
#endif
        }
    }

    void ChangeByBoth()
    {
        if (nowLanguageNum != (int)Mod_Data.language || nowBonusSwitch != Mod_Data.BonusSwitch)
        {
            nowLanguageNum = (int)Mod_Data.language;
            nowBonusSwitch = Mod_Data.BonusSwitch;
            nowBonusNum = nowBonusSwitch == false ? 0 : 1;

            if (GetComponent<Animator>() != null)
            {
                GetComponent<Animator>().SetBool("isEnglishLanguage", nowLanguageNum == 1 ? true : false);
                GetComponent<Animator>().SetBool("isBonusMode", nowBonusSwitch);
            }
            else
            {
                if (GetComponent<Image>() != null)
                {
                    // //Debug.Log(nowLanguageNum + (nowBonusNum == 0 ? 0 : 2));
                    GetComponent<Image>().sprite = changeSprite[nowLanguageNum + (nowBonusNum == 0 ? 0 : 2)];
                }
                else if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().sprite = changeSprite[nowBonusNum];
            }

        }
    }

    void setObjectActive(bool setBool)
    {
        for (int i = 0; i < setActiveObject.Length; i++)
        {
            setActiveObject[i].SetActive(setBool);
        }
    }

    void SetServerObjectDeActive(bool setBool)
    {
        if (setServerDeActiveObject.Length <= 0) return;
        for (int i = 0; i < setServerDeActiveObject.Length; i++)
        {
            setServerDeActiveObject[i].SetActive(setBool);
        }
    }
}
