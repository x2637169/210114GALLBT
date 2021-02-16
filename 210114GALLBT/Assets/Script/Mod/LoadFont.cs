using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LoadFont : MonoBehaviour
{
    string projectName;

    [SerializeField] public Text Number_Credit, Number_Bet, Number_Win, Number_PlayerCent, Number_Denom, Number_BonusWindow, Number_BonusTimes, Number_BonusTimesCount, Number_ExtraFreeGame; // Number_GameMultiply; 土豪
    [SerializeField] public Text Number_Wild5x, Number_Wild4x, Number_Wild3x, Number_Bonus5x, Number_Bonus4x, Number_Bonus3x, Number_P15x, Number_P14x, Number_P13x, Number_P25x, Number_P24x, Number_P23x, Number_P35x, Number_P34x, Number_P33x, Number_P45x, Number_P44x, Number_P43x, Number_P55x, Number_P54x, Number_P53x, Number_P65x, Number_P64x, Number_P63x, Number_A5x, Number_A4x, Number_A3x, Number_K5x, Number_K4x, Number_K3x, Number_Q5x, Number_Q4x, Number_Q3x, Number_J5x, Number_J4x, Number_J3x, Number_105x, Number_104x, Number_103x, Number_95x, Number_94x, Number_93x;
    static public Font Number_CreditFT, Number_BetFT, Number_WinFT, Number_PlayerCentFT, Number_DenomFT, Number_BonusWindowFT, Number_BonusTimesFT, Number_BonusTimesCountFT, Number_ExtraFreeGameFT; //Number_GameMultiplyFT; 土豪
    static public Font Number_Wild5xFT, Number_Wild4xFT, Number_Wild3xFT, Number_Bonus5xFT, Number_Bonus4xFT, Number_Bonus3xFT, Number_P15xFT, Number_P14xFT, Number_P13xFT, Number_P25xFT, Number_P24xFT, Number_P23xFT, Number_P35xFT, Number_P34xFT, Number_P33xFT, Number_P45xFT, Number_P44xFT, Number_P43xFT, Number_P55xFT, Number_P54xFT, Number_P53xFT, Number_P65xFT, Number_P64xFT, Number_P63xFT, Number_A5xFT, Number_A4xFT, Number_A3xFT, Number_K5xFT, Number_K4xFT, Number_K3xFT, Number_Q5xFT, Number_Q4xFT, Number_Q3xFT, Number_J5xFT, Number_J4xFT, Number_J3xFT, Number_105xFT, Number_104xFT, Number_103xFT, Number_95xFT, Number_94xFT, Number_93xFT;
    static public Material Number_CreditMT, Number_BetMT, Number_WinMT, Number_PlayerCentMT, Number_DenomMT, Number_BonusWindowMT, Number_BonusTimesMT, Number_BonusTimesCountMT, Number_ExtraFreeGameMT;// Number_GameMultiplyMT; 土豪
    static public Material Number_Wild5xMT, Number_Wild4xMT, Number_Wild3xMT, Number_Bonus5xMT, Number_Bonus4xMT, Number_Bonus3xMT, Number_P15xMT, Number_P14xMT, Number_P13xMT, Number_P25xMT, Number_P24xMT, Number_P23xMT, Number_P35xMT, Number_P34xMT, Number_P33xMT, Number_P45xMT, Number_P44xMT, Number_P43xMT, Number_P55xMT, Number_P54xMT, Number_P53xMT, Number_P65xMT, Number_P64xMT, Number_P63xMT, Number_A5xMT, Number_A4xMT, Number_A3xMT, Number_K5xMT, Number_K4xMT, Number_K3xMT, Number_Q5xMT, Number_Q4xMT, Number_Q3xMT, Number_J5xMT, Number_J4xMT, Number_J3xMT, Number_105xMT, Number_104xMT, Number_103xMT, Number_95xMT, Number_94xMT, Number_93xMT;
    // Start is called before the first frame update
    void Start()
    {
        projectName = Mod_Data.projectName;
        LoadCommonFont();
        LoadCommonFontMaterial();
        iniCommonFont();
    }

    // Update is called once per frame
    void Update()
    {

    }
    //讀取common font
    void LoadCommonFont()
    {
        Number_CreditFT = LoadCommonFontbyResource("Common", "Number-Credit", "Number-Credit");
        Number_BetFT = LoadCommonFontbyResource("Common", "Number-Credit", "Number-Credit");
        Number_WinFT = LoadCommonFontbyResource("Common", "Number-Credit", "Number-Credit");

        Number_PlayerCentFT = LoadCommonFontbyResource("Common", "Number-Credit", "Number-Credit");

        Number_DenomFT = LoadCommonFontbyResource("Common", "Number-Denom", "Number-Denom");

        Number_BonusWindowFT = LoadProjectFontbyResource("Number", "Number-BonusWindow", "Number-BonusWindow");

        Number_BonusTimesFT = LoadProjectFontbyResource("Number", "Number-BonusTimes", "Number-BonusTimes");
        Number_BonusTimesCountFT = LoadProjectFontbyResource("Number", "Number-BonusTimes", "Number-BonusTimes");

        Number_ExtraFreeGameFT = LoadProjectFontbyResource("Number", "Number-ExtraFreeGame", "Number-ExtraFreeGame");

        Number_Wild5xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Wild4xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Wild3xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_Bonus5xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Bonus4xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Bonus3xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P15xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P14xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P13xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P25xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P24xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P23xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P35xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P34xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P33xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P45xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P44xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P43xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P55xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P54xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P53xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P65xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P64xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P63xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_A5xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_A4xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_A3xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_K5xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_K4xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_K3xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_Q5xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Q4xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Q3xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_J5xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_J4xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_J3xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_105xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_104xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_103xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_95xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_94xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_93xFT = LoadProjectFontbyResource("Number", "Number-UpScreen", "Number-UpScreen");
    }
    //讀取common font material
    void LoadCommonFontMaterial()
    {
        Number_CreditMT = LoadCommonFontMaterialbyResource("Common", "Number-Credit", "Number-Credit");
        Number_BetMT = LoadCommonFontMaterialbyResource("Common", "Number-Credit", "Number-Credit");
        Number_WinMT = LoadCommonFontMaterialbyResource("Common", "Number-Credit", "Number-Credit");

        Number_PlayerCentMT = LoadCommonFontMaterialbyResource("Common", "Number-Credit", "Number-Credit");

        Number_DenomMT = LoadCommonFontMaterialbyResource("Common", "Number-Denom", "Number-Denom");

        Number_BonusWindowMT = LoadProjectFontMaterialbyResource("Number", "Number-BonusWindow", "Number-BonusWindow");

        Number_BonusTimesMT = LoadProjectFontMaterialbyResource("Number", "Number-BonusTimes", "Number-BonusTimes");
        Number_BonusTimesCountMT = LoadProjectFontMaterialbyResource("Number", "Number-BonusTimes", "Number-BonusTimes");

        Number_ExtraFreeGameMT = LoadProjectFontMaterialbyResource("Number", "Number-ExtraFreeGame", "Number-ExtraFreeGame");

        Number_Wild5xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Wild4xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Wild3xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Bonus5xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Bonus4xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Bonus3xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P15xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P14xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P13xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P25xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P24xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P23xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P35xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P34xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P33xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P45xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P44xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P43xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P55xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P54xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P53xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_P65xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P64xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_P63xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_A5xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_A4xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_A3xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_K5xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_K4xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_K3xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_Q5xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Q4xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_Q3xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_J5xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_J4xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_J3xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_105xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_104xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_103xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");

        Number_95xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_94xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
        Number_93xMT = LoadProjectFontMaterialbyResource("Number", "Number-UpScreen", "Number-UpScreen");
    }
    //初始化font
    void iniCommonFont()
    {
        Number_Credit.font = Number_CreditFT;
        Number_Credit.material = Number_CreditMT;
        Number_Bet.font = Number_BetFT;
        Number_Bet.material = Number_BetMT;
        Number_Win.font = Number_WinFT;
        Number_Win.material = Number_WinMT;

        Number_PlayerCent.font = Number_PlayerCentFT;
        Number_PlayerCent.material = Number_PlayerCentMT;

        Number_Denom.font = Number_DenomFT;
        Number_Denom.material = Number_DenomMT;

        Number_BonusWindow.font = Number_BonusWindowFT;
        Number_BonusWindow.material = Number_BonusWindowMT;

        Number_BonusTimes.font = Number_BonusTimesFT;
        Number_BonusTimes.material = Number_BonusTimesMT;
        Number_BonusTimesCount.font = Number_BonusTimesCountFT;
        Number_BonusTimesCount.material = Number_BonusTimesCountMT;

        Number_ExtraFreeGame.font = Number_ExtraFreeGameFT;
        Number_ExtraFreeGame.material = Number_ExtraFreeGameMT;

        Number_Wild5x.font = Number_Wild5xFT;
        Number_Wild5x.material = Number_Wild5xMT;
        Number_Wild4x.font = Number_Wild4xFT;
        Number_Wild4x.material = Number_Wild4xMT;
        Number_Wild3x.font = Number_Wild3xFT;
        Number_Wild3x.material = Number_Wild3xMT;


        Number_Bonus5x.font = Number_Bonus5xFT;
        Number_Bonus5x.material = Number_Bonus5xMT;
        Number_Bonus4x.font = Number_Bonus4xFT;
        Number_Bonus4x.material = Number_Bonus4xMT;
        Number_Bonus3x.font = Number_Bonus3xFT;
        Number_Bonus3x.material = Number_Bonus3xMT;

        Number_P15x.font = Number_P15xFT;
        Number_P15x.material = Number_P15xMT;
        Number_P14x.font = Number_P14xFT;
        Number_P14x.material = Number_P14xMT;
        Number_P13x.font = Number_P13xFT;
        Number_P13x.material = Number_P13xMT;

        Number_P25x.font = Number_P25xFT;
        Number_P25x.material = Number_P25xMT;
        Number_P24x.font = Number_P24xFT;
        Number_P24x.material = Number_P24xMT;
        Number_P23x.font = Number_P23xFT;
        Number_P23x.material = Number_P23xMT;

        Number_P35x.font = Number_P35xFT;
        Number_P35x.material = Number_P35xMT;
        Number_P34x.font = Number_P34xFT;
        Number_P34x.material = Number_P34xMT;
        Number_P33x.font = Number_P33xFT;
        Number_P33x.material = Number_P33xMT;

        Number_P45x.font = Number_P45xFT;
        Number_P45x.material = Number_P45xMT;
        Number_P44x.font = Number_P44xFT;
        Number_P44x.material = Number_P44xMT;
        Number_P43x.font = Number_P43xFT;
        Number_P43x.material = Number_P43xMT;

        Number_P55x.font = Number_P55xFT;
        Number_P55x.material = Number_P55xMT;
        Number_P54x.font = Number_P54xFT;
        Number_P54x.material = Number_P54xMT;
        Number_P53x.font = Number_P53xFT;
        Number_P53x.material = Number_P53xMT;

        Number_P65x.font = Number_P65xFT;
        Number_P65x.material = Number_P65xMT;
        Number_P64x.font = Number_P64xFT;
        Number_P64x.material = Number_P64xMT;
        Number_P63x.font = Number_P63xFT;
        Number_P63x.material = Number_P63xMT;

        Number_A5x.font = Number_A5xFT;
        Number_A5x.material = Number_A5xMT;
        Number_A4x.font = Number_A4xFT;
        Number_A4x.material = Number_A4xMT;
        Number_A3x.font = Number_A3xFT;
        Number_A3x.material = Number_A3xMT;

        Number_K5x.font = Number_K5xFT;
        Number_K5x.material = Number_K5xMT;
        Number_K4x.font = Number_K4xFT;
        Number_K4x.material = Number_K4xMT;
        Number_K3x.font = Number_K3xFT;
        Number_K3x.material = Number_K3xMT;

        Number_Q5x.font = Number_Q5xFT;
        Number_Q5x.material = Number_Q5xMT;
        Number_Q4x.font = Number_Q4xFT;
        Number_Q4x.material = Number_Q4xMT;
        Number_Q3x.font = Number_Q3xFT;
        Number_Q3x.material = Number_Q3xMT;

        Number_J5x.font = Number_J5xFT;
        Number_J5x.material = Number_J5xMT;
        Number_J4x.font = Number_J4xFT;
        Number_J4x.material = Number_J4xMT;
        Number_J3x.font = Number_J3xFT;
        Number_J3x.material = Number_J3xMT;

        Number_105x.font = Number_105xFT;
        Number_105x.material = Number_105xMT;
        Number_104x.font = Number_104xFT;
        Number_104x.material = Number_104xMT;
        Number_103x.font = Number_103xFT;
        Number_103x.material = Number_103xMT;

        Number_95x.font = Number_95xFT;
        Number_95x.material = Number_95xMT;
        Number_94x.font = Number_94xFT;
        Number_94x.material = Number_94xMT;
        Number_93x.font = Number_93xFT;
        Number_93x.material = Number_93xMT;

    }

    //主程式-從Resource中讀取Common Font
    Font LoadCommonFontbyResource(string parentFoldname, string childFoldname, string CommonFontName)
    {
        Font CommonFont = Resources.Load<Font>(parentFoldname + "/" + childFoldname + "/" + CommonFontName);
        return CommonFont;
    }
    //主程式-從Resource中讀取Common Font Material
    Material LoadCommonFontMaterialbyResource(string parentFoldname, string childFoldname, string CommonFontMTName)
    {
        Material CommonFontMT = Resources.Load<Material>(parentFoldname + "/" + childFoldname + "/" + CommonFontMTName);
        return CommonFontMT;
    }
    //主程式-從Resource中讀取遊戲Font
    Font LoadProjectFontbyResource(string parentFoldname, string childFoldname, string GameFontName)
    {
        Font GameFont = Resources.Load<Font>(projectName + "/" + parentFoldname + "/" + childFoldname + "/" + GameFontName);
        return GameFont;
    }
    //主程式-從Resource中讀取遊戲Font Material
    Material LoadProjectFontMaterialbyResource(string parentFoldname, string childFoldname, string GameFontMTName)
    {
        Material GameFontMT = Resources.Load<Material>(projectName + "/" + parentFoldname + "/" + childFoldname + "/" + GameFontMTName);
        return GameFontMT;
    }
}
