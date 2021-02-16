using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NewSramManager;

public class BackendManager : MonoBehaviour {
    [SerializeField] GameObject[] backendPanal;
    [SerializeField] NewSramManager newSramManager;
    [SerializeField] BillAcceptorController billAcceptorController;
    [SerializeField] GameObject maxhistoryPanal, secondMaxhistoryPanal;
    public void FinalExit() {
        bool panalOpen = false;
        for (int i = 0; i < backendPanal.Length; i++) {
            if (backendPanal[i].activeInHierarchy) panalOpen = true;
        }
        if (!panalOpen) {
            Mod_Data.IOLock = false;
            BackEnd_Login.LoginPass = false;
        }
    }
    int historyNum;
    public void OpenHistoryPanal(GameObject historyPanal) {
        historyNum = 0;
        newSramManager.LoadGameHistoryLog(0, out historyNum);
        if (historyNum > 0) {
            historyPanal.SetActive(true);
        }
    }
    public void OpenMaxHistoryPanal(GameObject maxHistoryPanal) {
        historyNum = 0;
        newSramManager.LoadGameHistoryLog(1, out historyNum);
        if (historyNum > 0) {
            secondMaxhistoryPanal.SetActive(false);
            maxHistoryPanal.SetActive(true);
        }
    }
    public void OpensecondMaxHistoryPanal(GameObject secondMaxHistoryPanal) {
        historyNum = 0;
        newSramManager.LoadGameHistoryLog(2, out historyNum);
        if (historyNum > 0) {
            maxhistoryPanal.SetActive(false);
            secondMaxHistoryPanal.SetActive(true);
        }
    }
    // EventRecoredDataList tmpeventRecoredDataList = new EventRecoredDataList();
    EventRecoredData[] tmp = new EventRecoredData[100];
    private void Start() {
        newSramManager.LoadEventRecored(out tmp);

        //Debug.Log(tmp[0].EventCode);
        for (int i = 0; i < 100; i++) {
            NewSramManager.eventRecoredDataList._EventRecoredData[i].EventCode = tmp[i].EventCode;
            NewSramManager.eventRecoredDataList._EventRecoredData[i].EventData = tmp[i].EventData;
            NewSramManager.eventRecoredDataList._EventRecoredData[i].EventTime = tmp[i].EventTime;
        }
        newSramManager.saveEventRecoredByEventName(1, 0);

    }
}
