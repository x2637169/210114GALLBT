using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingButton : MonoBehaviour
{
    public bool SettingList = false;
    public GameObject LanguageButton, HelpButton, HistoryButton;//, LanguageEngButton;
    private Tween HelpTween, LanguageTween, HistoryTween;//, LanguageEngTween;

    // Start is called before the first frame update
    void Start()
    {
        SettingButtonClose();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SettingButtonClick()
    {
        
            if (!SettingList)
            {
                SettingButtonOpen();
            }
            else if (SettingList)
            {
                SettingButtonClose();
            }
        
    }

    public void SettingButtonOpen() //打開設定選項(語言 Help 歷史)
    {
        DOTween.To(() => gameObject.GetComponent<RectTransform>().anchoredPosition, x => gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(130, 180), 0.5f);
        HelpTween = DOTween.To(() => HelpButton.gameObject.GetComponent<RectTransform>().anchoredPosition, x => HelpButton.gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(268, 45), 0.5f);
        HistoryTween = DOTween.To(() => HistoryButton.gameObject.GetComponent<RectTransform>().anchoredPosition, x => HistoryButton.gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(406, 45), 0.5f);
        LanguageTween = DOTween.To(() => LanguageButton.gameObject.GetComponent<RectTransform>().anchoredPosition, x => LanguageButton.gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(130, 45), 0.5f);
       // LanguageEngTween = DOTween.To(() => LanguageEngButton.gameObject.GetComponent<RectTransform>().anchoredPosition, x => LanguageEngButton.gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(130, 45), 0.5f);
        SettingList = true;
    }

    public void SettingButtonClose() //關閉設定選項(語言 Help 歷史)
    {
        DOTween.To(() => gameObject.GetComponent<RectTransform>().anchoredPosition, x => gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(130, 50), 0.5f);
        HelpTween = DOTween.To(() => HelpButton.gameObject.GetComponent<RectTransform>().anchoredPosition, x => HelpButton.gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(268, -87), 0.5f);
        HistoryTween = DOTween.To(() => HistoryButton.gameObject.GetComponent<RectTransform>().anchoredPosition, x => HistoryButton.gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(406, -87), 0.5f);
        LanguageTween = DOTween.To(() => LanguageButton.gameObject.GetComponent<RectTransform>().anchoredPosition, x => LanguageButton.gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(130, -87), 0.5f);
       // LanguageEngTween = DOTween.To(() => LanguageEngButton.gameObject.GetComponent<RectTransform>().anchoredPosition, x => LanguageEngButton.gameObject.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(130, -130), 0.5f);
        SettingList = false;
    }
}
