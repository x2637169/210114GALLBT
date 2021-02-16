using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

[ExecuteInEditMode]
public class LoadSprite : MonoBehaviour
{
    [SerializeField] string projectName;
    [SerializeField] UIItemDatabase iconScriptableObject;
    [SerializeField] Image UI_UpBackgroundPicture, UI_UpBackgroundWord, UI_DownBackgroundPicture, UI_ReelBack, UI_RoureL, UI_RouteR, iconBackgrounds;
    [SerializeField] Image[] lineImg;
    [SerializeField] LanguageAndBonusUICtrl upPayTableInfoImage, reelBack, route_L, route_R;


    //Icon圖檔
    static public Sprite icon_9, icon_10, icon_J, icon_Q, icon_K, icon_A, icon_P1, icon_P2, icon_P3, icon_P4, icon_P5, icon_P6, icon_Wild, icon_Bonus, icon_scatter, icon_Swild;
    //UI圖檔
    static public Sprite BaseReelBack, BonusReelBack, Route_C, Route_E, CountOF, WindowTrigger_C, WindowTrigger_E, WindowRetrigger01_C, WindowRetrigger01_E, WindowRetrigger02_C, WindowRetrigger02_E,
        WindowAccount_C, WindowAccount_E, WindowButton01, UpBaseBackground_C, UpBaseBackground_E, UpBonusBackground_C, UpBonusBackground_E, UpBackgroundPicture, DownBaseBackgroundPicture, DownBonusBackgroundPicture, iconBaseBackgroundSprite, iconBonusBackgroundSprite;
   //動畫或系列圖(規則圖)圖檔
    static public Sprite[] Base_under, Base_upper, Bonus_under, Bonus_upper, InfoBackground_C, InfoBackground_E, iconArray_9, iconArray_10, iconArray_J, iconArray_Q, iconArray_K, iconArray_A,
        iconArray_P1, iconArray_P2, iconArray_P3, iconArray_P4, iconArray_P5, iconArray_P6, iconArray_Wild, iconArray_Bonus, iconArray_scatter, iconArray_Swild,iconArray_BonusTransIn,iconArray_BonusTransOut;
    
    
    //動畫
    [SerializeField] AnimationClip Anim_DownBaseBackground, Anim_UpBaseBackground, Anim_DownBonusBackground, Anim_UpBonusBackground,
        Animicon_9, Animicon_10, Animicon_J, Animicon_Q, Animicon_K, Animicon_A, Animicon_P1, Animicon_P2, Animicon_P3, Animicon_P4, Animicon_P5, Animicon_P6, Animicon_Wild, Animicon_Bonus,
        Animicon_scatter, Animicon_Swild,Anim_BonusTransIn,Anim_BonusTransOut;

    [SerializeField] GameObject[] objUI;


    bool setSprite = false;
    Sprite tempSprite;
    Text playerName;




    void Awake()
    {
        Mod_Data.projectName = projectName;
        LoadIconSprite();//載入Icon圖
        LoadUISprite();//載入UI圖
        LoadSpriteArray();//載入相同系列圖ex動畫圖
        MakeAnimationClip();//製作動畫
        //LoadUILocation();//載入UI位置
        LoadSpriteToScriptableObject();//讀寫Icon的ScriptableObject
    }
    private void Start()
    {
        initUISprite();
    }
    // Update is called once per frame
    void Update()
    {
      
    }

    void initUISprite()
    {
        upPayTableInfoImage.changeSprite = new Sprite[4] { UpBaseBackground_C, UpBaseBackground_E, UpBonusBackground_C, UpBonusBackground_E };
        reelBack.changeSprite = new Sprite[2] { BaseReelBack, BonusReelBack };
        route_L.changeSprite = new Sprite[2] { Route_C, Route_E };
        route_R.changeSprite = new Sprite[2] { Route_C, Route_E };
        UI_UpBackgroundPicture.sprite = UpBackgroundPicture;
        UI_UpBackgroundWord.sprite = UpBaseBackground_C;
        UI_DownBackgroundPicture.sprite = DownBaseBackgroundPicture;
        UI_ReelBack.sprite = BaseReelBack;
        UI_RoureL.sprite = Route_C;
        UI_RouteR.sprite = Route_C;
        iconBackgrounds.sprite = iconBaseBackgroundSprite;


    }


    //讀取Icon圖檔
    void LoadIconSprite()
    {
        icon_9 = LoadSpritebyResource("GameGraphic", "icon_9");
        icon_10 = LoadSpritebyResource("GameGraphic", "icon_10");
        icon_J = LoadSpritebyResource("GameGraphic", "icon_J");
        icon_Q = LoadSpritebyResource("GameGraphic", "icon_Q");
        icon_K = LoadSpritebyResource("GameGraphic", "icon_K");
        icon_A = LoadSpritebyResource("GameGraphic", "icon_A");
        icon_P1 = LoadSpritebyResource("GameGraphic", "icon_P1");
        icon_P2 = LoadSpritebyResource("GameGraphic", "icon_P2");
        icon_P3 = LoadSpritebyResource("GameGraphic", "icon_P3");
        icon_P4 = LoadSpritebyResource("GameGraphic", "icon_P4");
        icon_P5 = LoadSpritebyResource("GameGraphic", "icon_P5");
        icon_P6 = LoadSpritebyResource("GameGraphic", "icon_P6");
        icon_Wild = LoadSpritebyResource("GameGraphic", "icon_Wild");
        icon_Bonus = LoadSpritebyResource("GameGraphic", "icon_Bonus");
        icon_scatter = LoadSpritebyResource("GameGraphic", "icon_scatter");
        icon_Swild = LoadSpritebyResource("GameGraphic", "icon_Swild");
    }
    //讀取UI圖檔
    void LoadUISprite()
    {
        BaseReelBack = LoadSpritebyResource("GameGraphic", "BaseReelBack");
        BonusReelBack = LoadSpritebyResource("GameGraphic", "BonusReelBack");
        Route_C = LoadSpritebyResource("GameGraphic", "Route_C");
        Route_E = LoadSpritebyResource("GameGraphic", "Route_E");
        WindowAccount_C = LoadSpritebyResource("GameGraphic", "WindowAccount_C");
        WindowAccount_E = LoadSpritebyResource("GameGraphic", "WindowAccount_E");
        UpBaseBackground_C = LoadSpritebyResource("GameGraphic", "UpBaseBackground_C");
        UpBaseBackground_E = LoadSpritebyResource("GameGraphic", "UpBaseBackground_E");
        UpBonusBackground_C = LoadSpritebyResource("GameGraphic", "UpBonusBackground_C");
        UpBonusBackground_E = LoadSpritebyResource("GameGraphic", "UpBonusBackground_E");
        UpBackgroundPicture = LoadSpritebyResource("GameGraphic", "UpBackgroundPicture");
        DownBaseBackgroundPicture = LoadSpritebyResource("GameGraphic", "DownBackgroundPicture");
        DownBonusBackgroundPicture = LoadSpritebyResource("GameGraphic", "DownBonusBackgroundPicture");
        iconBaseBackgroundSprite = LoadSpritebyResource("GameGraphic", "BasegameCover");
        iconBonusBackgroundSprite = LoadSpritebyResource("GameGraphic", "BonusgameCover");
        for (int i=0;i< Mod_Data.linegame_LineCount; i++)
        {
            if (LoadSpritebyResource("Line", "Line" + i) != null)
                lineImg[i].sprite = LoadSpritebyResource("Line", "Line" + i);
            else
                break;
        }
    }

    //讀取動畫圖檔或系列圖檔
    void LoadSpriteArray()
    {
        Base_upper = LoadSpriteArraybyResource("Animation", "UpBaseBackground");
        Bonus_upper = LoadSpriteArraybyResource("Animation", "UpBonusBackground");
        Base_under = LoadSpriteArraybyResource("Animation", "DownBaseBackground");
        Bonus_under = LoadSpriteArraybyResource("Animation", "DownBonusBackground");

        InfoBackground_C = LoadSpriteArraybyResource("GameGraphic", "InfoBackground_C");
        InfoBackground_E = LoadSpriteArraybyResource("GameGraphic", "InfoBackground_E");

        iconArray_9 = LoadSpriteArraybyResource("Animation", "9");
        iconArray_10 = LoadSpriteArraybyResource("Animation", "10");
        iconArray_J = LoadSpriteArraybyResource("Animation", "J");
        iconArray_Q = LoadSpriteArraybyResource("Animation", "Q");
        iconArray_K = LoadSpriteArraybyResource("Animation", "K");
        iconArray_A = LoadSpriteArraybyResource("Animation", "A");
        iconArray_P1 = LoadSpriteArraybyResource("Animation", "P1");
        iconArray_P2 = LoadSpriteArraybyResource("Animation", "P2");
        iconArray_P3 = LoadSpriteArraybyResource("Animation", "P3");
        iconArray_P4 = LoadSpriteArraybyResource("Animation", "P4");
        iconArray_P5 = LoadSpriteArraybyResource("Animation", "P5");
        iconArray_P6 = LoadSpriteArraybyResource("Animation", "P6");
        iconArray_Wild = LoadSpriteArraybyResource("Animation", "Wild");
        iconArray_Bonus = LoadSpriteArraybyResource("Animation", "Bonus");
        iconArray_scatter = LoadSpriteArraybyResource("Animation", "Scatter");
        iconArray_Swild = LoadSpriteArraybyResource("Animation", "Swild");

        iconArray_BonusTransIn = LoadSpriteArraybyResource("Animation", "Transition_In");
        iconArray_BonusTransOut = LoadSpriteArraybyResource("Animation", "Transition_Out");

    }

    //製作動畫
    void MakeAnimationClip()
    {
        LoadSpriteMakeAnim(Base_under, Anim_DownBaseBackground);
        LoadSpriteMakeAnim(Base_upper, Anim_UpBaseBackground);
        LoadSpriteMakeAnim(Bonus_under, Anim_DownBonusBackground);
        LoadSpriteMakeAnim(Bonus_upper, Anim_UpBonusBackground);

        LoadSpriteMakeAnim(iconArray_9, Animicon_9);
        LoadSpriteMakeAnim(iconArray_10, Animicon_10);
        LoadSpriteMakeAnim(iconArray_J, Animicon_J);
        LoadSpriteMakeAnim(iconArray_Q, Animicon_Q);
        LoadSpriteMakeAnim(iconArray_K, Animicon_K);
        LoadSpriteMakeAnim(iconArray_A, Animicon_A);
        LoadSpriteMakeAnim(iconArray_P1, Animicon_P1);
        LoadSpriteMakeAnim(iconArray_P2, Animicon_P2);
        LoadSpriteMakeAnim(iconArray_P3, Animicon_P3);
        LoadSpriteMakeAnim(iconArray_P4, Animicon_P4);
        LoadSpriteMakeAnim(iconArray_P5, Animicon_P5);
        LoadSpriteMakeAnim(iconArray_P6, Animicon_P6);
        LoadSpriteMakeAnim(iconArray_Bonus, Animicon_Bonus);
        LoadSpriteMakeAnim(iconArray_Wild, Animicon_Wild);
        LoadSpriteMakeAnim(iconArray_scatter, Animicon_scatter);
        LoadSpriteMakeAnim(iconArray_Swild, Animicon_Swild);

        LoadSpriteMakeAnim(iconArray_BonusTransIn, Anim_BonusTransIn);
        LoadSpriteMakeAnim(iconArray_BonusTransOut, Anim_BonusTransOut);
    }

    //讀寫Icon的ScriptableObject
    void LoadSpriteToScriptableObject()
    {
        ////Debug.Log(iconScriptableObject.items.Length);
        for (int i = 0; i < iconScriptableObject.items.Length; i++)
            iconScriptableObject.GetByID(i).Sprite = LoadSpriteByName(iconScriptableObject.GetByID(i).Name);
    }
    //讀取UI位置
    void LoadUILocation()
    {
        TextAsset tmp;
        int UICount = 0;
        tmp = LoadJsonbyResource("Location");
        Location UILocation = JsonUtility.FromJson<Location>(tmp.text);
        foreach (var UI in UILocation.UIName)
        {
            if (objUI[UICount].name == UI.name)
            {
                if (UI.X != 0)
                {
                    objUI[UICount].transform.position = new Vector3(UI.X, UI.Y);
                }
                UICount++;
            }
        }
    }
    //Json檔序列化
    [Serializable]
    public class Location
    {
        public List<UI> UIName;
    }
    [Serializable]
    public class UI
    {
        public string name;
        public int X;
        public int Y;
    }


    //主程式-根據名稱讀取圖檔 給 Icon的ScriptableObject使用 自動根據名稱放入圖檔
    static public Sprite LoadSpriteByName(string iconName)
    {
        switch (iconName)
        {
            case "Wild":
                return icon_Wild;
            case "Bonus":
                return icon_Bonus;
            case "Scatter":
                return icon_scatter;
            case "P1":
                return icon_P1;
            case "P2":
                return icon_P2;
            case "P3":
                return icon_P3;
            case "P4":
                return icon_P4;
            case "P5":
                return icon_P5;
            case "P6":
                return icon_P6;
            case "A":
                return icon_A;
            case "K":
                return icon_K;
            case "Q":
                return icon_Q;
            case "J":
                return icon_J;
            case "10":
                return icon_10;
            case "9":
                return icon_9;
            case "Swild":
                return icon_Swild;
            default:
                return null;

        }


    }
    //主程式-從Resource中讀取圖檔陣列
    Sprite[] LoadSpriteArraybyResource(string parentFoldname, string childfoldName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(projectName + "/" + parentFoldname + "/" + childfoldName);
        return sprites;
    }
    //主程式-從Resource中讀取圖檔
    Sprite LoadSpritebyResource(string parentFoldname, string spriteName)
    {
        Sprite sprites = Resources.Load<Sprite>(projectName + "/" + parentFoldname + "/" + spriteName);
        return sprites;
    }
    //主程式-從Resource中讀取Json檔
    TextAsset LoadJsonbyResource(string jsonName)
    {
        TextAsset Json = Resources.Load<TextAsset>(jsonName);//projectName + "/" + 
        return Json;
    }






    //主程式-圖檔製作動畫
    void LoadSpriteMakeAnim(Sprite[] sprites, AnimationClip animClip)
    {
        //此段程式只能在editor模式使用,輸出時須關閉
#if UNITY_EDITOR
        //AnimationClip animClip = new AnimationClip();
        animClip.frameRate = 30;

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(Image);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < (sprites.Length); i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe();
            spriteKeyFrames[i].time = i * (1 / 30f);
            spriteKeyFrames[i].value = sprites[i];
        }
        AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);

#endif

    }

    //Sprite[] LoadFile(string parentFolder, string Folder)
    //{
    //    string spritepath = Path.Combine(Application.streamingAssetsPath, parentFolder + "/" + Folder);
    //    DirectoryInfo directoryInfo = new DirectoryInfo(spritepath);
    //    FileInfo[] allfiles = directoryInfo.GetFiles("*.*");
    //    sprite = new Sprite[allfiles.Length / 2];
    //    int index = 0;
    //    foreach (FileInfo file in allfiles)
    //    {

    //        StartCoroutine(LoadPlayerUI(file, index));
    //        if (file.Name.Contains("meta"))
    //        {

    //            index++;
    //        }

    //    }

    //    return sprite;
    //}

    //IEnumerator LoadPlayerUI(FileInfo playerFile, int fileIndex)
    //{
    //    //1
    //    if (playerFile.Name.Contains("meta"))
    //    {

    //        yield break;
    //    }
    //    //2
    //    else
    //    {
    //        string playerFileWithoutExtension = Path.GetFileNameWithoutExtension(playerFile.ToString());
    //        string[] playerNameData = playerFileWithoutExtension.Split(" "[0]);
    //        //3
    //        string tempPlayerName = "";
    //        int i = 0;
    //        foreach (string stringFromFileName in playerNameData)
    //        {
    //            if (i != 0)
    //            {
    //                tempPlayerName = tempPlayerName + stringFromFileName + " ";
    //            }
    //            i++;
    //        }
    //        //Debug.Log(playerFileWithoutExtension);
    //        string wwwPlayerFilePath = "file://" + playerFile.FullName.ToString();
    //        WWW www = new WWW(wwwPlayerFilePath);

    //        yield return www;
    //        //5
    //        //Debug.Log("N" + playerFileWithoutExtension);
    //        tempSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
    //        tempSprite.name = playerFileWithoutExtension;
    //        sprite[fileIndex] = tempSprite;
    //    }
    //}
}
