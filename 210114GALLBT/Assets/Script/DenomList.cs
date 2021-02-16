using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DenomList : MonoBehaviour {

    public GameObject[] DenomListObj;
    public RectTransform[] DenomVec;
    public Text DenomButton;
    public GameObject DenomBG;
    [SerializeField] Mod_UIController mod_UIController;
    [SerializeField] BackEnd_SetFunction backEnd_SetFunction;
    [SerializeField] BackEnd_Data backEnd_Data;
    //public float[] DenomNumberList = new float[5];
    public bool ListOpen = false;
    // Use this for initialization
    void Start() {
        DenomVec = new RectTransform[DenomListObj.Length];
        for (int i = 0; i < DenomListObj.Length; i++) {
            DenomVec[i] = DenomListObj[i].GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update() {
       
        if (Mod_Data.Denom == 0.1) {
            DenomButton.text = "0.10";
        }
        else {
            DenomButton.text = Mod_Data.Denom.ToString();
        }
        for (int i = 0; i < DenomVec.Length; i++) {
            if (DenomVec[i].anchoredPosition.y < 52 && !ListOpen) {
                if (DenomListObj[i].activeInHierarchy == true) {
                    DenomListObj[i].SetActive(false);
                }
            }
        }

        if (!Mod_Data.inBaseSpin && ListOpen) {
            CloseDenomList();
        }
    }

    public void DenomButtonOnClick() {
        if (Mod_Data.inBaseSpin && !Mod_Data.autoPlay) {
            //Debug.Log(Mod_Data.BonusSwitch);
            if (!Mod_Data.BonusSwitch) {
                if (!ListOpen) {
                    OpenDenomList();
                    ListOpen = true;
                }
                else {
                    CloseDenomList();

                }
            }
        }
    }

    void ChangeDenom(int denomIndex) {
        //Debug.Log(denomIndex);
        Mod_Data.Denom = Mod_Data.denomArray[denomIndex];
        backEnd_SetFunction.RTPChoose();
        CloseDenomList();
        mod_UIController.UpdateScore();
        ListOpen = false;
    }

    public void OpenDenomList() {
        DenomBG.SetActive(true);
        int openIndex = 0;
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.denomArraySelect, false);
        for (int i = 0; i < Mod_Data.denomArray.Length; i++) {
            Mod_Data.denomOpenArray[i] = BackEnd_Data.denomArray[i];// GameDataManager._AllStatisticalData.DenomSelect[i];
            if (Mod_Data.denomOpenArray[i]) {
                int tmp = i;
                DenomListObj[i].SetActive(true);
                // DOTween.To(() => DenomListObj[tmp].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[tmp].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * openIndex), 0.5f);
                if (Mod_Data.denomArray[i] == 0.1) {
                    DenomListObj[i].GetComponentInChildren<Text>().text = "0.10";
                }
                else {
                    DenomListObj[i].GetComponentInChildren<Text>().text = Mod_Data.denomArray[i].ToString();
                }                
                DenomListObj[i].GetComponent<Button>().onClick.AddListener(delegate { ChangeDenom(tmp); });
                //  DoTweenMove(i, openIndex);
                openIndex++;
                //Debug.Log(i);
            }
            else {
                DenomListObj[i].SetActive(false);
            }
        }

    }

    public void CloseDenomList() {
        //for (int i = 0; i < Mod_Data.denomArray.Length; i++)
        //{
        //    int tmp = i;
        //    DOTween.To(() => DenomListObj[tmp].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[tmp].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 50.1f), 0.5f);

        //}
        DenomBG.SetActive(false);
        ListOpen = false;

    }

    void DoTweenMove(int i, int movePos) {
        switch (i) {
            case 0:
                DOTween.To(() => DenomListObj[0].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[0].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            case 1:
                DOTween.To(() => DenomListObj[1].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[1].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            case 2:
                DOTween.To(() => DenomListObj[2].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[2].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            case 3:
                DOTween.To(() => DenomListObj[3].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[3].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            case 4:
                DOTween.To(() => DenomListObj[4].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[4].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            case 5:
                DOTween.To(() => DenomListObj[5].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[5].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            case 6:
                DOTween.To(() => DenomListObj[6].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[6].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            case 7:
                DOTween.To(() => DenomListObj[7].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[7].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            case 8:
                DOTween.To(() => DenomListObj[8].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[8].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            case 9:
                DOTween.To(() => DenomListObj[9].GetComponent<RectTransform>().anchoredPosition, x => DenomListObj[9].GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1571, 145 + 100 * movePos), 0.5f);
                break;
            default:
                break;
        }

    }
}
