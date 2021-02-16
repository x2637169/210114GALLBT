using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackEnd_SettingTest : MonoBehaviour
{
    [SerializeField] GameObject[] buttons;
    [SerializeField] BackendManager backendManager;

#if !Server
    #region !Server
    
    private void OnEnable()
    {
        refreshButton();
        backendManager.FinalExit();
    }
    
    public void refreshButton()
    {
        if (BackEnd_Login.LoginPass)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetActive(true);
            }
        }
        else
        {
            buttons[1].SetActive(false);//開分
            buttons[2].SetActive(false);//功能設定
            buttons[4].SetActive(false);//帳務系統設定
            buttons[5].SetActive(false);//按鈕測試
            //buttons[6].SetActive(false);//總事件紀錄
        }
    }

    #endregion
#endif
}
