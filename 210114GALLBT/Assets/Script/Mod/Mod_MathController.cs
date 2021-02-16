using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Mod_MathController : MonoBehaviour
{
    [SerializeField] TextAsset sLowRTP_Base;
    [SerializeField] TextAsset[] sLowRTP_Bonus;
    [SerializeField] TextAsset lowRTP_Base;
    [SerializeField] TextAsset[] lowRTP_Bonus;
    [SerializeField] TextAsset midRTP_Base;
    [SerializeField] TextAsset[] midRTP_Bonus;
    [SerializeField] TextAsset highRTP_Base;
    [SerializeField] TextAsset[] highRTP_Bonus;
    [SerializeField] TextAsset sHighRTP_Base;
    [SerializeField] TextAsset[] sHighRTP_Bonus;

    JObject sLowRTP_Base_J;
    JObject[] sLowRTP_Bonus_J;
    JObject lowRTP_Base_J;
    JObject[] lowRTP_Bonus_J;
    JObject midRTP_Base_J;
    JObject[] midRTP_Bonus_J;
    JObject highRTP_Base_J;
    JObject[] highRTP_Bonus_J;
    JObject sHighRTP_Base_J;
    JObject[] sHighRTP_Bonus_J;

    private void Awake()
    {
        Cursor.visible = false; //關閉滑鼠
        LoadJobjects();
    }

    void LoadJobjects()
    {
        sLowRTP_Base_J = JObject.Parse(sLowRTP_Base.text);
        sLowRTP_Bonus_J = new JObject[sLowRTP_Bonus.Length];
        for (int i = 0; i < sLowRTP_Bonus_J.Length; i++)
        {
            sLowRTP_Bonus_J[i] = JObject.Parse(sLowRTP_Bonus[i].text);
        }

        lowRTP_Base_J = JObject.Parse(lowRTP_Base.text);
        lowRTP_Bonus_J = new JObject[lowRTP_Bonus.Length];
        for (int i = 0; i < lowRTP_Bonus_J.Length; i++)
        {
            lowRTP_Bonus_J[i] = JObject.Parse(lowRTP_Bonus[i].text);
        }

        midRTP_Base_J = JObject.Parse(midRTP_Base.text);
        midRTP_Bonus_J = new JObject[midRTP_Bonus.Length];
        for (int i = 0; i < midRTP_Bonus_J.Length; i++)
        {
            midRTP_Bonus_J[i] = JObject.Parse(midRTP_Bonus[i].text);
        }

        highRTP_Base_J = JObject.Parse(highRTP_Base.text);
        highRTP_Bonus_J = new JObject[highRTP_Bonus.Length];
        for (int i = 0; i < highRTP_Bonus_J.Length; i++)
        {
            highRTP_Bonus_J[i] = JObject.Parse(highRTP_Bonus[i].text);
        }

        sHighRTP_Base_J = JObject.Parse(sHighRTP_Base.text);
        sHighRTP_Bonus_J = new JObject[sHighRTP_Bonus.Length];
        for (int i = 0; i < sHighRTP_Bonus_J.Length; i++)
        {
            sHighRTP_Bonus_J[i] = JObject.Parse(sHighRTP_Bonus[i].text);
        }
    }

    public void ChangeMathFile(int RTP)
    {
#if Server
        #region Server
        switch (RTP)
        {
            case 0:
                Mod_Data.JsonObjBase = sLowRTP_Base_J;
                Mod_Data.JsonObjBonus = sLowRTP_Bonus_J;
                break;
            case 1:
                Mod_Data.JsonObjBase = lowRTP_Base_J;
                Mod_Data.JsonObjBonus = lowRTP_Bonus_J;
                break;
            case 2:
                Mod_Data.JsonObjBase = midRTP_Base_J;
                Mod_Data.JsonObjBonus = midRTP_Bonus_J;
                break;
            case 3:
                Mod_Data.JsonObjBase = highRTP_Base_J;
                Mod_Data.JsonObjBonus = highRTP_Bonus_J;
                break;
            case 4:
                Mod_Data.JsonObjBase = sHighRTP_Base_J;
                Mod_Data.JsonObjBonus = sHighRTP_Bonus_J;
                break;
        }
        #endregion
#else
        #region  !Server
        switch (RTP)
        {
            case 0:
                Mod_Data.JsonObjBase = lowRTP_Base_J;
                Mod_Data.JsonObjBonus = lowRTP_Bonus_J;
                break;
            case 1:
                Mod_Data.JsonObjBase = midRTP_Base_J;
                Mod_Data.JsonObjBonus = midRTP_Bonus_J;
                break;
            case 2:
                Mod_Data.JsonObjBase = highRTP_Base_J;
                Mod_Data.JsonObjBonus = highRTP_Bonus_J;
                break;
        }
        #endregion
#endif
        Mod_Data.JsonReload();
    }
}
