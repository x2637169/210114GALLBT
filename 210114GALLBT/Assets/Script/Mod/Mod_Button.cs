using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_Button : MonoBehaviour
{
    [SerializeField] bool isBonus;
    private void Update()
    {
        //Data.BonusSwitch = isBonus;
    }
    public void Btn_ClickInfomButton()
    {

    }

    public void ImgLanguageChange()
    {

        int LanguageNumber;
        if (Mod_Data.language == Mod_Data.languageList.CHT) LanguageNumber = (int)Mod_Data.languageList.EN;
        else LanguageNumber = (int)Mod_Data.languageList.CHT;
        switch (LanguageNumber)
        {
            case 0:
                Mod_Data.language = Mod_Data.languageList.CHT;

                break;
            case 1:
                Mod_Data.language = Mod_Data.languageList.EN;

                break;
            default:
                break;
        }
    }

    public void Information_Button(bool open)  //改經中介者
    {

        if (!Mod_Data.autoPlay) { 
        Mod_Data.IOLock = open;

        GetComponent<Mod_UIController>().Information_UI(open);
        }
    }
    public void InformationChange_Button(int page) //改經中介者
    {
        GetComponent<Mod_UIController>().Information_Page(page);
    }
}
