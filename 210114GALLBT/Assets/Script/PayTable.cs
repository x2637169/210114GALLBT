using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class PayTable : MonoBehaviour
{

    //public GameObject  Wild, Bonus, P1, P2, P3, P4, P5, A, K, Q, J, TEN;
    public GameObject[] payTableObj, loadedObj;//payTableObj 根據ItemDatabase動態載入,loadedObj被載入的物件(載入Itembase有的物件)


    // Use this for initialization
    void Start()
    {
        PayTableUpData();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
    public void PayTableUpData()
    {
        for (int i = 0; i < payTableObj.Length; i++)
        {
            LoadPayToObject(i);
        }
    }

    JArray iconPay;
    void LoadPayToObject(int IconID)
    {
        iconPay = (JArray)Mod_Data.JsonObj["Icons"]["Icon" + (IconID + 1)]["IconPays"];//暫存對應iconID Paytable 值

        for (int i = 0; i < 5; i++)
        {

            if (int.Parse(iconPay[4 - i].ToString()) != 0)
                payTableObj[IconID].gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(iconPay[4 - i].ToString()) * Mod_Data.odds).ToString();
            else
                payTableObj[IconID].gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    //public void PayTableUpData()
    //{
    //    IconPaysLoad(0);
    //    Wild.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4].ToString()) * Mod_Data.odds).ToString();

    //    for (int i = 0; i < 4; i++)
    //    {
    //        IconPaysLoad(0);
    //        if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //            Wild.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        else
    //            Wild.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        IconPaysLoad(1);
    //        if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //            Bonus.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        else
    //            Bonus.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        IconPaysLoad(2);
    //        if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //            P1.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        else
    //            P1.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        IconPaysLoad(3);
    //        if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //            P2.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        else
    //            P2.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        IconPaysLoad(4);
    //        if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //            P3.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        else
    //            P3.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        IconPaysLoad(5);
    //        if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //            P4.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        else
    //            P4.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        //IconPaysLoad(6);
    //        //if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //        //    P5.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        //else
    //        //    P5.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        IconPaysLoad(6);
    //        if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //            A.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        else
    //            A.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        IconPaysLoad(8);
    //        if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //            Q.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        else
    //            Q.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        IconPaysLoad(9);
    //        if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //            J.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        else
    //            J.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //        //IconPaysLoad(9);
    //        //if (int.Parse(Mod_Data.IconPays[4 - i].ToString()) != 0)
    //        //    TEN.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>().text = (int.Parse(Mod_Data.IconPays[4 - i].ToString()) * Mod_Data.odds).ToString();
    //        //else
    //        //    TEN.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //    }
    //}

    //public void PayTableOff()
    //{
    //    for (int i = 0; i < 9; i++)
    //    {
    //        gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //    }
    //}

    //public void PayTableOn()
    //{
    //    for (int i = 0; i < 9; i++)
    //    {
    //        gameObject.transform.GetChild(i).gameObject.SetActive(true);
    //    }
    //}

    //void IconPaysLoad(int IconID)
    //{
    //    switch (IconID)
    //    {
    //        case 0:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon1"]["IconPays"];
    //            break;
    //        case 1:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon2"]["IconPays"];
    //            break;
    //        case 2:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon3"]["IconPays"];
    //            break;
    //        case 3:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon4"]["IconPays"];
    //            break;
    //        case 4:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon5"]["IconPays"];
    //            break;
    //        case 5:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon6"]["IconPays"];
    //            break;
    //        case 6:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon7"]["IconPays"];
    //            break;
    //        case 7:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon8"]["IconPays"];
    //            break;
    //        case 8:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon9"]["IconPays"];
    //            break;
    //        case 9:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon10"]["IconPays"];
    //            break;
    //        case 10:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon11"]["IconPays"];
    //            break;
    //        case 11:
    //            Mod_Data.IconPays = (JArray)Mod_Data.JsonObj["Icons"]["Icon12"]["IconPays"];
    //            break;
    //    }

    //}
}
