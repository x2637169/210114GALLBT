using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class Mod_TimeController : MonoBehaviour
{
#if Server
    #region Server
    AudioSource[] AllGameAudio;
    public static bool GamePasue = false;

    // Start is called before the first frame update
    void Start()
    {
        AllGameAudio = FindObjectsOfType<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Mod_Data.MachineError || Mod_Client_Data.serverdisconnect || Mod_Data.memberLcok || Mod_Data.machineIDLock) && Time.timeScale != 0)
        {
            PasueGame();
        }
        else if (!Mod_Data.MachineError && !Mod_Client_Data.serverdisconnect && !Mod_Data.memberLcok && !Mod_Data.machineIDLock && Time.timeScale == 0)
        {
            ResumeGame();
        }
    }

    void PasueGame()
    {
        GamePasue = true;
        Time.timeScale = 0f;
        for (int i = 0; i < AllGameAudio.Length; i++)
        {
            AllGameAudio[i].pitch = 0;
        }

        if (BillAcceptorSettingData.BillOpenClose && !BillAcceptorSettingData.CashOrTicketIn)
        {
            //Debug.Log("Pasue");
            BillAcceptorSettingData.BillAcceptorEnable = false;
            BillAcceptorSettingData.GetOrderType = "BillEnableDisable";
        }
    }

    void ResumeGame()
    {
        GamePasue = false;
        Time.timeScale = 1f;
        for (int i = 0; i < AllGameAudio.Length; i++)
        {
            AllGameAudio[i].pitch = 1;
        }

        if (BillAcceptorSettingData.BillOpenClose && !BillAcceptorSettingData.CashOrTicketIn)
        {
            //Debug.Log("Resume");
            BillAcceptorSettingData.BillAcceptorEnable = true;
            BillAcceptorSettingData.GetOrderType = "BillEnableDisable";
        }
    }
    #endregion
#endif
}
