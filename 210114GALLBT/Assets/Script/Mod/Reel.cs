using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System;
using Random = System.Random;
using RNGUtility;

public class Reel : MonoBehaviour {

    //This Variable Will Be Changed By The "Slots" Class To Control When The Reel Spins 
    public bool spin;
    JArray ReelMath;
    public int ReelRng;
   // public UIItemDatabase itemDatabase;
    public GameObject[] EndReelIcon = new GameObject[8];
    public SlotSymbol[] reelSlotSymbol = new SlotSymbol[8];
    Vector3[] EndPosition = new Vector3[8];//原位置
    Vector3[] ReboundPosition = new Vector3[8];//回彈位置
    int IconNum = 0;
    //Speed That Reel Will Spin
    public int speed;
    List<int> parts = new List<int>();


    //---new
    public int reelIndex;
    [SerializeField] float startDelayTime;//開始延遲時間
    [SerializeField] float scrollTime;//滾動時間
    public float scrollSpeed = 6000;//滾動速度
    [SerializeField] float slowScrollSpeed = 1000;//緩速滾動速度
    [SerializeField] float normalSpeed = 3000;
    [SerializeField] float fastScrollSpeed = 5000;
    [SerializeField] float fastreboundSpeed = 30;
    [SerializeField] float normalreboundSpeed = 20;
    [SerializeField] float reboundSpeed = 500;//回彈速度
    [SerializeField] float stopTopPos = 5;//回彈頂位置


    [SerializeField] Transform bottomIconTrans;
    [SerializeField] bool isRebound = false;//是否回彈
    bool isTouchStop = false;
    [SerializeField] bool orderStop = false;
    public bool startSlowSpeed = false;//開始緩速
    public bool blankButtonDown = false;
    public int bottomIconPos = 0;

    [SerializeField] Mod_IconChangeTrigger iconChangeTrigger;
    [SerializeField] Mod_ReelStopTrigger iconStopTrigger;
    [SerializeField] Slots slots;
    // Use this for initialization
    void Start()
    {
        spin = false;
        speed = 6000;

        //定位初始ICON位置
        int ReelNum = 0;

        if (Mod_Data.slotVer == Mod_Data.SlotVer.reel3x5)
        {
            parts.Add(1956 - 52);
            parts.Add(1720 - 52);
            parts.Add(1484 - 52);
            parts.Add(1248 - 52);
            parts.Add(1012 - 52);
            parts.Add(776 - 52);
            parts.Add(540 - 52);
            parts.Add(304 - 52);


        }
        else if (Mod_Data.slotVer == Mod_Data.SlotVer.reel4x5)
        {
            parts.Add(1519 - 52);
            parts.Add(1342 - 52);
            parts.Add(1165 - 52);
            parts.Add(988 - 52);
            parts.Add(811 - 52);
            parts.Add(634 - 52);
            parts.Add(457 - 52);
            parts.Add(280 - 52);

            for (int i = 0; i < EndReelIcon.Length; i++)
            {
                EndReelIcon[i].GetComponent<RectTransform>().sizeDelta =
                    new Vector2(EndReelIcon[i].GetComponent<RectTransform>().sizeDelta.x, EndReelIcon[i].GetComponent<RectTransform>().sizeDelta.y - 43);

                EndReelIcon[i].GetComponent<SphereCollider>().radius = 75;
            }
        }
        foreach (Transform image in transform)
        {
            if (ReelNum >= 8)
            {
                ReelNum = 0;
            }
            EndPosition[ReelNum] = new Vector3(image.transform.position.x, parts[ReelNum], image.transform.position.z);
            ReboundPosition[ReelNum] = new Vector3(image.transform.position.x, parts[ReelNum] + stopTopPos, image.transform.position.z);
            reelSlotSymbol[ReelNum] = EndReelIcon[ReelNum].GetComponent<SlotSymbol>();
            ReelNum++;
        }

    }
    int stopPos = 0;
    // Update is called once per frame
    void Update()
    {
        if (spin)
        {
            isRebound = false;
            isTouchStop = false;
            foreach (Transform image in transform)
            {
                if (startSlowSpeed)
                {
                    reboundSpeed = normalreboundSpeed;
                    scrollSpeed = normalSpeed;
                    //Direction And Speed Of Movement
                    image.transform.Translate(Vector3.down * Time.smoothDeltaTime * slowScrollSpeed, Space.World);

                    //Once The Image Moves Below A Certain Point, Reset Its Position To The Top
                    if (Mod_Data.slotVer == Mod_Data.SlotVer.reel3x5)
                    {
                        if (image.transform.position.y <= 0) { image.transform.position = new Vector3(image.transform.position.x, image.transform.position.y + 1888, image.transform.position.z); }
                    }
                    else if (Mod_Data.slotVer == Mod_Data.SlotVer.reel4x5)
                    {
                        if (image.transform.position.y <= 0) { image.transform.position = new Vector3(image.transform.position.x, image.transform.position.y + 1416, image.transform.position.z); }
                    }
                }
                else
                {
                    //Direction And Speed Of Movement
                    image.transform.Translate(Vector3.down * Time.smoothDeltaTime * scrollSpeed, Space.World);

                    //Once The Image Moves Below A Certain Point, Reset Its Position To The Top
                    if (Mod_Data.slotVer == Mod_Data.SlotVer.reel3x5)
                    {
                        if (image.transform.position.y <= 0) { image.transform.position = new Vector3(image.transform.position.x, image.transform.position.y + 1888, image.transform.position.z); }
                    }
                    else if (Mod_Data.slotVer == Mod_Data.SlotVer.reel4x5)
                    {
                        if (image.transform.position.y <= 0) { image.transform.position = new Vector3(image.transform.position.x, image.transform.position.y + 1416, image.transform.position.z); }
                    }
                }

            }
            //ReelSetRng();
        }
        else
        {
            if (!isTouchStop)
            {
                slots.DetectAllReelStop(reelIndex, true);
                isTouchStop = true;
            }
            for (int i = 0; i < EndReelIcon.Length; i++)
            {
                if (reelSlotSymbol[i].bottomIcon)
                {
                    bottomIconTrans = reelSlotSymbol[i].transform;
                    // //Debug.Log("Bottom"+i);
                }
            }
            ////Debug.Log(bottomIconTrans.position.y - ReboundPosition[7].y);
            if ((bottomIconTrans.position.y - ReboundPosition[7].y) > -10 && !isRebound)
            {
                isRebound = true;
            }
            if (isRebound)
            {
                bottomIconTrans.transform.position = Vector3.Lerp(bottomIconTrans.transform.position, EndPosition[7], reboundSpeed * Time.deltaTime);
            }
            else
            {
                bottomIconTrans.transform.position = Vector3.Lerp(bottomIconTrans.transform.position, ReboundPosition[7], reboundSpeed * Time.deltaTime);
            }

            for (int i = 0; i < EndReelIcon.Length; i++)
            {

                if (i >= bottomIconPos)
                {
                    stopPos = i - bottomIconPos;

                }
                else
                {
                    stopPos = 7 - bottomIconPos + i + 1;
                }
                ////Debug.Log(i + " " + stopPos + " " + bottomIconPos);
                if (Mod_Data.slotVer == Mod_Data.SlotVer.reel3x5)
                {
                    if (i != bottomIconPos) EndReelIcon[i].transform.position = bottomIconTrans.position + new Vector3(0, stopPos * 236, 0);
                }
                else if (Mod_Data.slotVer == Mod_Data.SlotVer.reel4x5)
                {
                    if (i != bottomIconPos) EndReelIcon[i].transform.position = bottomIconTrans.position + new Vector3(0, stopPos * 177, 0);
                }
            }
        }
    }

    public void ReelMathReload()
    {
        ////Debug.Log(gameObject.name);
        Mod_Data.JsonReload();
        switch (gameObject.name)
        {
            case "Reel1":
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_1"]["Reel"];
                break;
            case "Reel2":
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_2"]["Reel"];
                break;
            case "Reel3":
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_3"]["Reel"];
                break;
            case "Reel4":
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_4"]["Reel"];
                break;
            case "Reel5":
                ReelMath = (JArray)Mod_Data.JsonObj["StripTable"]["Strip_5"]["Reel"];
                break;
        }
    }

/// <summary>
    /// 依據傳送最大值，判斷亂數演算使用方式，並取得一個亂數值。
    /// </summary>
    /// <returns>亂數值</returns>
    public int RandomRng(int maximum)
    {
        if (maximum <= 255)
        {
            RNGInt8 randomRng = new RNGInt8((uint)maximum);
            return (int)randomRng.Next();
        }
        else if (maximum <= 65535)
        {
            RNGInt16 randomRng = new RNGInt16((uint)maximum);
            return (int)randomRng.Next();
        }
        else if (maximum <= 16777215)
        {
            RNGInt24 randomRng = new RNGInt24((uint)maximum);
            return (int)randomRng.Next();
        }
        else if ((uint)maximum <= 4294967295)
        {
            RNGInt32 randomRng = new RNGInt32((uint)maximum);
            return (int)randomRng.Next();
        }

        return 0;
    }

    public void ReelSetRng()
    {
        ReelMathReload();
        int ReelRngConut;
        if (!Mod_Data.StartNotNormal) ReelRng = RandomRng(ReelMath.Count);
        else ReelRng = Mod_Data.RNG[reelIndex];
        switch (gameObject.name)
        {

            case "Reel1":
                if (Mod_Data.TestMode)
                {
                    if (Mod_Data.BonusSwitch) ReelRng = 28;
                    else ReelRng = 2;
                }
                Mod_Data.RNG[0] = ReelRng;
                ReelRngConut = ReelRng;
                for (int i = 0; i < Mod_Data.slotRowNum; i++)
                {
                    if (ReelRngConut < 0) { ReelRngConut = ReelMath.Count - 1; }
                    Mod_Data.ReelMath[0, i] = int.Parse(ReelMath[ReelRngConut].ToString());
                    ReelRngConut -= 1;
                }
                break;
            case "Reel2":
                if (Mod_Data.TestMode)
                {
                    if (Mod_Data.BonusSwitch) ReelRng = 1;
                    else ReelRng = 2;
                }
                Mod_Data.RNG[1] = ReelRng;
                ReelRngConut = ReelRng;
                for (int i = 0; i < Mod_Data.slotRowNum; i++)
                {
                    if (ReelRngConut < 0) { ReelRngConut = ReelMath.Count - 1; }
                    Mod_Data.ReelMath[1, i] = int.Parse(ReelMath[ReelRngConut].ToString());
                    ReelRngConut -= 1;
                }
                break;
            case "Reel3":
                if (Mod_Data.TestMode)
                {
                    if (Mod_Data.BonusSwitch) ReelRng = 10;
                    else ReelRng = 2;
                }
                Mod_Data.RNG[2] = ReelRng;
                ReelRngConut = ReelRng;
                for (int i = 0; i < Mod_Data.slotRowNum; i++)
                {
                    if (ReelRngConut < 0) { ReelRngConut = ReelMath.Count - 1; }
                    Mod_Data.ReelMath[2, i] = int.Parse(ReelMath[ReelRngConut].ToString());
                    ReelRngConut -= 1;
                }
                break;
            case "Reel4":
                if (Mod_Data.TestMode)
                {
                    if (Mod_Data.BonusSwitch) ReelRng = 28;
                    else ReelRng = 2;
                }
                Mod_Data.RNG[3] = ReelRng;
                ReelRngConut = ReelRng;
                for (int i = 0; i < Mod_Data.slotRowNum; i++)
                {
                    if (ReelRngConut < 0) { ReelRngConut = ReelMath.Count - 1; }
                    Mod_Data.ReelMath[3, i] = int.Parse(ReelMath[ReelRngConut].ToString());
                    ReelRngConut -= 1;
                }
                break;
            case "Reel5":
                if (Mod_Data.TestMode)
                {
                    if (Mod_Data.BonusSwitch) ReelRng = 3;
                    else ReelRng = 2;
                }
                Mod_Data.RNG[4] = ReelRng;
                ReelRngConut = ReelRng;
                for (int i = 0; i < Mod_Data.slotRowNum; i++)
                {
                    if (ReelRngConut < 0) { ReelRngConut = ReelMath.Count - 1; }
                    Mod_Data.ReelMath[4, i] = int.Parse(ReelMath[ReelRngConut].ToString());
                    ReelRngConut -= 1;
                }
                break;
        }
    }

    int randomID = 0;
    Random randomIDspace = new Random(Guid.NewGuid().GetHashCode());
    public void IconSpriteReload(int IconNumber)
    {
        randomID = randomIDspace.Next(0, Mod_Data.iconDatabase.items.Length);
        //判斷該輪是否能出現Bonus
        if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3)
        {
            if (reelIndex == 3 || reelIndex == 4)
            {
                if (randomID == 1) randomID = randomID + 2;
            }
        }
        else if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel2_3_4)
        {
            if (reelIndex == 0 || reelIndex == 4)
            {
                if (randomID == 1) randomID++;
            }
        }
        else if (Mod_Data.bonusRule == Mod_Data.BonusRule.Reel1_2_3_4_5)
        {
            if(startSlowSpeed && randomID == 1) randomID++;
            //驢子
            if (Mod_Data.BonusSwitch && randomID == 12) randomID--;
        }
        else if (Mod_Data.bonusRule == Mod_Data.BonusRule.ConsecutiveReel1_2_3_4_5)
        {

        }
        //else if (Mod_Data.bonusRule == Mod_Data.BonusRule.CH)
        //{
        //    if (reelIndex == 0 || reelIndex == 1)
        //    {
        //        if (randomID == 1) randomID++;
        //    }
        //}

        //結果有Bonus的情況
        if (Mod_Data.ReelMath[reelIndex, 0] == 1 || Mod_Data.ReelMath[reelIndex, 1] == 1 || Mod_Data.ReelMath[reelIndex, 2] == 1)
        {
            if (randomID == 1) randomID = randomID + 2;
        }
        if ((Mod_Data.ReelMath[reelIndex, 0] != 1 || Mod_Data.ReelMath[reelIndex, 1] != 1 || Mod_Data.ReelMath[reelIndex, 2] != 1) && reelIndex == 2)
        {
            if (randomID == 1) randomID = randomID + 2;
        }

        //偵測鄰近是否有Bonus
        if (randomID == 1)
        {
            if (IconNumber == 0)
            {
                if (EndReelIcon[6].GetComponent<SlotSymbol>().iconID == 1 || EndReelIcon[7].GetComponent<SlotSymbol>().iconID == 1) randomID = randomID + 2;
            }
            else if (IconNumber == 1)
            {
                if (EndReelIcon[0].GetComponent<SlotSymbol>().iconID == 1 || EndReelIcon[7].GetComponent<SlotSymbol>().iconID == 1) randomID = randomID + 2;
            }
            else
            {
                if ((EndReelIcon[IconNumber - 1].GetComponent<SlotSymbol>().iconID == 1 || EndReelIcon[IconNumber - 2].GetComponent<SlotSymbol>().iconID == 1)) randomID = randomID + 2;
            }
        }

        EndReelIcon[IconNumber].gameObject.GetComponent<Image>().sprite =
             Mod_Data.iconDatabase.GetByID(randomID).Sprite;
        EndReelIcon[IconNumber].GetComponent<SlotSymbol>().iconID = randomID;
        //ReelRngConut -= 1;
        //}
    }
    //最後圖
    public void IconSpriteFinalLoad(int iconPos)
    {
        int ReelRngConut, endIconNum, changeIconPos;
        ReelRngConut = Mod_Data.RNG[reelIndex];
        
        for (int i = 0; i < Mod_Data.slotRowNum; i++)
        {
            //判斷是否會扣到滾輪底部 負數則個別處理
            if (ReelRngConut - i < 0)
            {
                endIconNum = ReelMath.Count + (ReelRngConut - i);
            }
            else
            {
                endIconNum = ReelRngConut - i;
            }
            changeIconPos = iconPos + i;
            if (changeIconPos > 7) changeIconPos = changeIconPos - 8;

           // //Debug.Log(ReelMath[endIconNum]+"Y");
            EndReelIcon[changeIconPos].gameObject.GetComponent<Image>().sprite =
                Mod_Data.iconDatabase.GetByID(int.Parse(ReelMath[endIconNum].ToString())).Sprite;
            EndReelIcon[changeIconPos].GetComponent<SlotSymbol>().iconID = int.Parse(ReelMath[endIconNum].ToString());
            ////Debug.Log("Reel"+reelIndex + ",iconPos:" + iconPos + "changeIconPos:" + changeIconPos + "ID:" + int.Parse(ReelMath[endIconNum].ToString()));
        }
    }
    //報牌
    int specialIconIndex = 0;
    public void IconSpriteSpecialLoad(int IconNumber)
    {
        specialIconIndex = 0;
        if (Mod_Data.ReelMath[reelIndex, 0] == 1)
        {
            specialIconIndex = IconNumber + 2;

            EndReelIcon[specialIconIndex].GetComponent<SlotSymbol>().changeSprite = true;
            EndReelIcon[specialIconIndex].gameObject.GetComponent<Image>().sprite =
                 Mod_Data.iconDatabase.GetByID(1).Sprite;
            EndReelIcon[specialIconIndex].GetComponent<SlotSymbol>().iconID = 1;
        }
        else if (Mod_Data.ReelMath[reelIndex, 1] == 1)
        {
            specialIconIndex = IconNumber + 3;
            if (specialIconIndex >= 8) specialIconIndex = specialIconIndex - 8;

            EndReelIcon[specialIconIndex].GetComponent<SlotSymbol>().changeSprite = true;
            EndReelIcon[specialIconIndex].gameObject.GetComponent<Image>().sprite =
                  Mod_Data.iconDatabase.GetByID(1).Sprite;
            EndReelIcon[specialIconIndex].GetComponent<SlotSymbol>().iconID = 1;
        }
        else if (Mod_Data.ReelMath[reelIndex, 2] == 1)
        {
            specialIconIndex = IconNumber + 4;
            if (specialIconIndex >= 8) specialIconIndex = specialIconIndex - 8;

            EndReelIcon[specialIconIndex].GetComponent<SlotSymbol>().changeSprite = true;
            EndReelIcon[specialIconIndex].gameObject.GetComponent<Image>().sprite =
                 Mod_Data.iconDatabase.GetByID(1).Sprite;
            EndReelIcon[specialIconIndex].GetComponent<SlotSymbol>().iconID = 1;
        }
        //else if (Mod_Data.ReelMath[reelIndex, 3] == 1)
        //{
        //    specialIconIndex = IconNumber +3;
        //    if (specialIconIndex >= 8) specialIconIndex = specialIconIndex - 8;

        //    EndReelIcon[specialIconIndex].GetComponent<SlotSymbol>().changeSprite = true;
        //    EndReelIcon[specialIconIndex].gameObject.GetComponent<Image>().sprite =
        //         Mod_Data.iconDatabase.GetByID(1).Sprite;
        //    EndReelIcon[specialIconIndex].GetComponent<SlotSymbol>().iconID = 1;
        //}

    }

    public void IconSpriteLoadWithoutBonus(int IconNumber) {
        EndReelIcon[IconNumber].GetComponent<SlotSymbol>().changeSprite = true;
        EndReelIcon[IconNumber].gameObject.GetComponent<Image>().sprite =
        Mod_Data.iconDatabase.GetByID(3).Sprite;
        EndReelIcon[IconNumber].GetComponent<SlotSymbol>().iconID = 3;
    }

    public void SetPosition()
    {  //滾輪停止後定位
        int ReelNum = 0;

        foreach (Transform image in transform)
        {
            if (ReelNum >= 8)
            {
                ReelNum = 0;
            }
            if (Mod_Data.ReelSpeedChange)
            {
                image.transform.position = new Vector3(image.transform.position.x, parts[ReelNum], image.transform.position.z);
            }
            else
            {
                image.transform.position = new Vector3(image.transform.position.x, parts[ReelNum] - 100, image.transform.position.z);
            }
            ReelNum++;
        }
    }
    public void SpinCtrl(bool start)
    {
        if (start)
        {
            m_Runner = StartCoroutine(StartSpin());
            orderStop = false;
        }
        else
        {
            if (!orderStop)
            {
                StopCoroutine(m_Runner);
                if (!blankButtonDown) {
                    iconChangeTrigger.stopReel = true;
                }
                else {
                    iconStopTrigger.stopReel = true;
                    //Debug.Log("Click");
                }
                //iconChangeTrigger.stopReel = true;
                ////Debug.Log("RunAgain" + reelIndex);
                orderStop = true;
            }
        }
    }

    private Coroutine m_Runner = null;

    public IEnumerator StartSpin()
    {
        yield return new WaitForSeconds(startDelayTime);
        spin = true;
        scrollSpeed = normalSpeed;
        reboundSpeed = normalreboundSpeed;
        for (int i = 0; i < reelSlotSymbol.Length; i++)
        {
            reelSlotSymbol[i].bottomIcon = false;
        }

        yield return new WaitUntil(() => orderStop == true);
        if (startSlowSpeed) yield return new WaitForSeconds(10f);
        //Debug.Log("NoStop" + reelIndex);
        iconChangeTrigger.stopReel = true;
        //if (!orderStop)
        //{
        //    iconChangeTrigger.stopReel = true;
        //    orderStop = true;
        //}
    }
    public void fastScroll() {
        scrollSpeed = fastScrollSpeed;
        reboundSpeed = fastreboundSpeed;
    }
}