using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Mod_BonusCameraMove : MonoBehaviour {
    [SerializeField] NewSramManager newSramManager;
    public GameObject FishButton, ChoiceFrame, BonusTrans, ChoiceFrameBoat, BonusInformation, BoatButton;
    public Animator BonusMap; //管理動畫
    int[] BonusTime = new int[11];
    int[] BonusTimeWeight = new int[11];
    int[] BonusMulti = new int[6];
    int[] BonusMultiWeight = new int[6];
    int[,] BonusMultiContent = new int[,] { { 2, 2, 3, 4, 4 }, { 2, 2, 3, 3, 4 }, { 2, 2, 2, 3, 4 }, { 2, 2, 3, 4, 4 }, { 2, 3, 3, 4, 4 }, { 3, 3, 3, 4, 4 } };
    int[] TimeArray = new int[100];   //random用
    int[] MultiArray = new int[100];   //random用
    int[] tmp = new int[5];
    public Text[] BonusTimeText, BonusMultiText = new Text[5]; //寶藏與船的字體
    public Text BonusTitleTime, BonusTitleMulti; //結尾標題的字體
    public GameObject[] BoatTimeText, BoatFreeGameText; //船 "開啟"圖案與自體
    public Sprite[] FreeGameImage;//切換免費場次圖案(亮與不亮)
    public Font[] FreeGameText; //切換字體(亮與不亮)

    // Start is called before the first frame update
    void Start() {
        //抓取倍率與場次的json數學
        Mod_Data.JsonReload();
        int k = 0;

        for (int i = 0; i < 11; i++) {
            JArray Session = (JArray)Mod_Data.JsonObj["Icons"]["Icon2"]["PickBonusType2"];
            JArray SessionWeight = (JArray)Mod_Data.JsonObj["Icons"]["Icon2"]["PickBonusType2Weight"];
            BonusTime[i] = int.Parse(Session[i].ToString());
            BonusTimeWeight[i] = int.Parse(SessionWeight[i].ToString());

            for (int j = 0; j < BonusTimeWeight[i]; j++) {
                TimeArray[k] = BonusTime[i];
                k++;
            }
        }

        k = 0;
        for (int i = 0; i < 6; i++) {
            JArray Multi = (JArray)Mod_Data.JsonObj["Icons"]["Icon2"]["PickMultiplier"];
            JArray MultiWeight = (JArray)Mod_Data.JsonObj["Icons"]["Icon2"]["PickMultiplierWeight"];
            BonusMulti[i] = int.Parse(Multi[i].ToString());
            BonusMultiWeight[i] = int.Parse(MultiWeight[i].ToString());

            for (int j = 0; j < BonusMultiWeight[i]; j++) {
                MultiArray[k] = BonusMulti[i];
                k++;
            }
        }

        //Debug.Log(TimeArray);
        //Debug.Log(MultiArray);
    }

    // Update is called once per frame
    void Update() {

    }

    public void BonusFirstSelectAni() {  //開啟選擇倍數物件
        //ChoiceFrame.SetActive(true);
        //FishButton.SetActive(true);
    }

    public void BonusFishSelect(int FishID) {  //bonus倍數處理 以及動畫開啟
        BonusMap.SetInteger("Fish", FishID);

        if (Mod_Data.BonusSpecialTimes == 1) { //沒有選過
            int FishNum = Random.Range(0, 100);
            FishNum = MultiArray[FishNum];
            //Debug.Log(FishNum);
            //隨機排列五個倍數且不重複
            for (int i = 0; i < 5; i++) {
                tmp[i] = Random.Range(0, 5);
                for (int j = 0; j < i; j++) {
                    while (tmp[j] == tmp[i]) {
                        j = 0;
                        tmp[i] = Random.Range(0, 5);
                    }
                }
            }
            for (int i = 0; i < 5; i++) {
                tmp[i] = BonusMultiContent[FishNum, tmp[i]];
                BonusTimeText[i].text = tmp[i].ToString();
                newSramManager.SaveBonusFish(i, tmp[i]);
                //選中的變色
                if (i == FishID) {
                    BonusTimeText[i].GetComponent<Text>().font = FreeGameText[0];
                }
                else {
                    BonusTimeText[i].GetComponent<Text>().font = FreeGameText[1];
                }
            }
        }
        else { //已選過 斷電重開
            for(int i = 0; i < 5; i++) {
                BonusTimeText[i].text = newSramManager.LoadBonusFish(i).ToString();
                tmp[i] = newSramManager.LoadBonusFish(i);
                //選中的變色
                if (i == FishID) {
                    BonusTimeText[i].GetComponent<Text>().font = FreeGameText[0];
                }
                else {
                    BonusTimeText[i].GetComponent<Text>().font = FreeGameText[1];
                }
            }
        }
        //給倍數
        Mod_Data.BonusSpecialTimes = tmp[FishID];
        newSramManager.SaveSpeicalTime(Mod_Data.BonusSpecialTimes); //紀錄sram內 斷電用
        newSramManager.SaveUserSelectedSpeicalTime(FishID); //紀錄sram 使用者選擇

        //開啟被關掉(船)的按鈕
        for (int i = 0; i < 5; i++) {
            BoatButton.transform.GetChild(i).GetComponent<Button>().enabled = true;
        }

    }

    public void BonusBoatSelect(int BoatID) {
        BonusMap.enabled = false;  //暫停動畫
        ChoiceFrameBoat.SetActive(false);
        BonusInformation.SetActive(true);
        if(Mod_Data.BonusCount == 0) {
            //隨機選出五個場次且不重複
            for (int i = 0; i < 5; i++) {
                tmp[i] = Random.Range(0, 100);
                for (int j = 0; j < i; j++) {
                    while (tmp[j] == tmp[i]) {
                        j = 0;
                        tmp[i] = Random.Range(0, 100);
                    }
                }
            }
            for (int i = 0; i < 5; i++) {
                BonusMultiText[i].text = TimeArray[tmp[i]].ToString();
                BoatTimeText[i].SetActive(true);
                BoatFreeGameText[i].SetActive(true);
                newSramManager.SaveBonusBoat(i, TimeArray[tmp[i]]);
                //選中的變色
                if (i == BoatID) {
                    BoatFreeGameText[i].GetComponent<Image>().sprite = FreeGameImage[0];
                    BoatTimeText[i].GetComponent<Text>().font = FreeGameText[0];
                }
                else {
                    BoatFreeGameText[i].GetComponent<Image>().sprite = FreeGameImage[1];
                    BoatTimeText[i].GetComponent<Text>().font = FreeGameText[1];
                }
                //關閉按鈕 避免重複點到
                BoatButton.transform.GetChild(i).GetComponent<Button>().enabled = false;
            }
        }
        else {
            for(int i = 0; i < 5; i++) {
                BonusMultiText[i].text = newSramManager.LoadBonusBoat(i).ToString();
                BoatTimeText[i].SetActive(true);
                BoatFreeGameText[i].SetActive(true);

                //選中的變色
                if (i == BoatID) {
                    BoatFreeGameText[i].GetComponent<Image>().sprite = FreeGameImage[0];
                    BoatTimeText[i].GetComponent<Text>().font = FreeGameText[0];
                }
                else {
                    BoatFreeGameText[i].GetComponent<Image>().sprite = FreeGameImage[1];
                    BoatTimeText[i].GetComponent<Text>().font = FreeGameText[1];
                }
                //關閉按鈕 避免重複點到
                BoatButton.transform.GetChild(i).GetComponent<Button>().enabled = false;
            }
        }
        //給場次
        Mod_Data.BonusCount = int.Parse(BonusMultiText[BoatID].text);
        Mod_Data.getBonusCount = int.Parse(BonusMultiText[BoatID].text);
        newSramManager.SaveBonusCount(Mod_Data.BonusCount); //紀錄sram內 斷電用
        newSramManager.SaveUserSelectedBonusCount(BoatID); //紀錄sram 使用者選擇

        //顯示場次與倍數
        BonusTitleTime.text = Mod_Data.BonusCount.ToString();
        BonusTitleMulti.text = Mod_Data.BonusSpecialTimes.ToString();
        StartCoroutine(BonusEnd());

    }

    IEnumerator BonusEnd() {
        for (float i = 0; i <= 3; i += Time.deltaTime) {
            yield return 0;
        }
        BonusTrans.SetActive(false);
        Mod_Data.transInAnimEnd = true;

    }

    public void showtest() {
        //Debug.Log("Mod_Data.BonusSpecialTimes" + Mod_Data.BonusSpecialTimes);
        //Debug.Log("Mod_Data.BonusCount" + Mod_Data.BonusCount);
    }
}
