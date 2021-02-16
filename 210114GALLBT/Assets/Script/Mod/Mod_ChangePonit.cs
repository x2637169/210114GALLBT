using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Mod_ChangePonit : MonoBehaviour
{
    [SerializeField] Text txt_point, txt_passwordhold;
    [SerializeField] GameObject obj_page1, obj_page2, obj_page3, btn_all, btn_100, btn_500, btn_1000, obj_changePanal;
    [SerializeField] InputField ifd_showPassword;
    [SerializeField] Mod_Client mod_Client;

#if Server
    #region Server
    string string_password;
    public static bool CloseMemberPanel = false;

    // Update is called once per frame
    void Update()
    {
        if (!Mod_Data.inBaseSpin || Mod_Data.autoPlay || Mod_Data.memberLcok)
        {
            obj_changePanal.SetActive(false);
            string_password = "";
            txt_passwordhold.text = "Enter Password...";
        }

        if (Mod_Data.memberRakebackPoint >= 1000)
        {
            btn_1000.SetActive(true);
            btn_500.SetActive(true);
            btn_100.SetActive(true);
            btn_all.SetActive(true);
        }
        else if (Mod_Data.memberRakebackPoint >= 500)
        {
            btn_1000.SetActive(false);
            btn_500.SetActive(true);
            btn_100.SetActive(true);
            btn_all.SetActive(true);
        }
        else if (Mod_Data.memberRakebackPoint >= 100)
        {
            btn_1000.SetActive(false);
            btn_500.SetActive(false);
            btn_100.SetActive(true);
            btn_all.SetActive(true);
        }
        else if (Mod_Data.memberRakebackPoint >= 1)
        {
            btn_1000.SetActive(false);
            btn_500.SetActive(false);
            btn_100.SetActive(false);
            btn_all.SetActive(true);
        }
        else
        {
            btn_1000.SetActive(false);
            btn_500.SetActive(false);
            btn_100.SetActive(false);
            btn_all.SetActive(false);
        }

        if (CloseMemberPanel)
        {
            obj_changePanal.SetActive(false);
            obj_page1.SetActive(false);
            obj_page2.SetActive(false);
            obj_page3.SetActive(false);
            CloseMemberPanel = false;
        }
    }
    public void OnEnable()
    {
        txt_point.text = Mathf.FloorToInt(Mod_Data.memberRakebackPoint).ToString("N0");
        string_password = "";
        ifd_showPassword.text = string_password;
        txt_passwordhold.text = "Enter Password...";

    }
    public void ClosePanal()
    {
        if (!Mod_Data.changePointLock)
        {

            obj_page1.SetActive(false);
            obj_page2.SetActive(false);
            obj_page3.SetActive(false);
        }
    }
    public void ReturnPage(int page)
    {

        switch (page)
        {
            case 0:
                obj_page1.SetActive(false);
                break;
            case 1:
                obj_page2.SetActive(false);
                txt_point.text = Mathf.FloorToInt(Mod_Data.memberRakebackPoint).ToString("N0");
                break;
            case 2:
                obj_page3.SetActive(false);
                break;
            case 99:
                obj_page1.SetActive(false);
                obj_page2.SetActive(false);
                obj_page3.SetActive(false);
                obj_changePanal.SetActive(false);
                break;
            default:
                break;
        }
        string_password = "";
        ifd_showPassword.text = string_password;
        txt_passwordhold.text = "Enter Password...";

    }
    public void KeyinPassword(int password)
    {
        string_password += password;
        ifd_showPassword.text = string_password;
    }
    public void ClearPassword()
    {
        string_password = "";
        ifd_showPassword.text = string_password;
        txt_passwordhold.text = "Enter Password...";
    }
    public void BackPassword()
    {
        if (string_password != null && string_password != "")
        {
            string_password = string_password.Remove(string_password.Length - 1);
        }
        ifd_showPassword.text = string_password;

    }
    public void ComfirmPassword()
    {
        if (StringEncrypt.ComputeSha256Hash(string_password) == Mod_Data.cardPassword)
        {
            obj_page3.SetActive(true);
            btn_all.SetActive(true);
            if (Mod_Data.memberRakebackPoint >= 1000)
            {
                btn_1000.SetActive(true);
                btn_500.SetActive(true);
                btn_100.SetActive(true);
                btn_all.SetActive(true);
            }
            else if (Mod_Data.memberRakebackPoint >= 500)
            {
                btn_1000.SetActive(false);
                btn_500.SetActive(true);
                btn_100.SetActive(true);
                btn_all.SetActive(true);
            }
            else if (Mod_Data.memberRakebackPoint >= 100)
            {
                btn_1000.SetActive(false);
                btn_500.SetActive(false);
                btn_100.SetActive(true);
                btn_all.SetActive(true);
            }
            else if (Mod_Data.memberRakebackPoint >= 1)
            {
                btn_1000.SetActive(false);
                btn_500.SetActive(false);
                btn_100.SetActive(false);
                btn_all.SetActive(true);
            }
            else
            {
                btn_1000.SetActive(false);
                btn_500.SetActive(false);
                btn_100.SetActive(false);
                btn_all.SetActive(false);
            }
        }
        else
        {
            string_password = "";
            ifd_showPassword.text = string_password;
            txt_passwordhold.text = "Wrong Password";
            Debug.Log("Wrong Password");
        }
    }

    public void changePonitButton(string point)
    {
        if (Mod_Data.changePointLock) return;

        switch (point)
        {
            case "100":
                Mod_Data.changePoint = 100;
                StartCoroutine("ExchangePointsCoroutine");
                break;
            case "500":
                Mod_Data.changePoint = 500;
                StartCoroutine("ExchangePointsCoroutine");
                break;
            case "1000":
                Mod_Data.changePoint = 1000;
                StartCoroutine("ExchangePointsCoroutine");
                break;
            case "all":
                Mod_Data.changePoint = Mathf.FloorToInt(Mod_Data.memberRakebackPoint);
                StartCoroutine("ExchangePointsCoroutine");
                break;
            default:
                break;
        }
    }

    WaitForSecondsRealtime WaitExchangePointsCoroutineShortTime = new WaitForSecondsRealtime(0.1f);
    WaitForSecondsRealtime WaitExchangePointsCoroutineLongTime = new WaitForSecondsRealtime(1f);
    IEnumerator ExchangePointsCoroutine()
    {
        Mod_Data.changePointLock = true;

        mod_Client.SendToSever(Mod_Client_Data.messagetype.exchangePoints);

        while (true)
        {
            if (!Mod_Data.changePointLock) break;
            if (Mod_Client_Data.serverdisconnect)
            {
                yield return new WaitUntil(() => Mod_Client_Data.serverdisconnect == false && Mod_Data.machineInit == true);
                yield return WaitExchangePointsCoroutineLongTime;
                mod_Client.SendToSever(Mod_Client_Data.messagetype.exchangePoints);
            }
            yield return WaitExchangePointsCoroutineShortTime;
        }
    }

    public void OpenChangePanal()
    {
        if (Mod_Data.inBaseSpin && !Mod_Data.autoPlay && !Mod_Data.memberLcok && !string.IsNullOrWhiteSpace(Mod_Data.memberID) && Mod_Data.cardneeded == 1)
        {
            obj_page1.SetActive(true);
            obj_changePanal.SetActive(true);
        }
    }
    #endregion
#endif
}
