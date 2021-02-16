using CFPGADrv;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BYTE = System.Byte;

public class BackEnd_SetEvent : MonoBehaviour
{
    // Start is called before the first frame update
    CFPGADrvBridge.STATUS Status = new CFPGADrvBridge.STATUS();
    public GameObject EventRecoredWindows;
    int EventRecoredPage = 0;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenEvnetRecoredWindow() {
        EventRecoredPage = 0;
        EventRecoredDataUpdate(EventRecoredPage);
        EventRecoredWindows.SetActive(true);
    }
    public void EventRecoredDataUpdate(int Page) {
        int NullCount = 0;
        for (int i = 0; i < 6; i++) {
            if (NewSramManager.eventRecoredDataList._EventRecoredData[i + (6 * Page)].EventCode != 0) {
                EventRecoredWindows.transform.GetChild(1).transform.GetChild(i).GetComponent<Text>().text = EventRecoredDataEventCodeToName(NewSramManager.eventRecoredDataList._EventRecoredData[i + (6 * Page)].EventCode);
                EventRecoredWindows.transform.GetChild(1).transform.GetChild(i).transform.GetChild(1).GetComponent<Text>().text = NewSramManager.eventRecoredDataList._EventRecoredData[i + (6 * Page)].EventTime.ToString("yyyy/MM/dd HH:mm:ss");
                if (NewSramManager.eventRecoredDataList._EventRecoredData[i + (6 * Page)].EventData != 0) {
                    if (NewSramManager.eventRecoredDataList._EventRecoredData[i + (6 * Page)].EventCode == 5 || NewSramManager.eventRecoredDataList._EventRecoredData[i + (6 * Page)].EventCode == 6) {
                        EventRecoredWindows.transform.GetChild(1).transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text = ((double)NewSramManager.eventRecoredDataList._EventRecoredData[i + (6 * Page)].EventData / 100).ToString();
                    }
                    else
                        EventRecoredWindows.transform.GetChild(1).transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text = NewSramManager.eventRecoredDataList._EventRecoredData[i + (6 * Page)].EventData.ToString();
                }
                else {
                    EventRecoredWindows.transform.GetChild(1).transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text = "";
                }
            }
            else {
                EventRecoredWindows.transform.GetChild(1).transform.GetChild(i).GetComponent<Text>().text = "";
                EventRecoredWindows.transform.GetChild(1).transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text = "";
                EventRecoredWindows.transform.GetChild(1).transform.GetChild(i).transform.GetChild(1).GetComponent<Text>().text = "";
                NullCount++;
            }
            if (NullCount > 5) {
                if (EventRecoredPage > 0) {
                    EventRecoredPage--;
                    EventRecoredDataUpdate(EventRecoredPage);
                }
            }
        }
    }
    string EventRecoredDataEventCodeToName(int EventCode) {
        string EventName = "";
        switch (EventCode) {
            case 1:
                EventName = "啟動遊戲";
                break;
            case 2:
                EventName = "開分";
                break;
            case 3:
                EventName = "洗分";
                break;
            case 4:
                EventName = "投幣";
                break;
            case 5:
                EventName = "入票";
                break;
            case 6:
                EventName = "出票";
                break;
            default:
                break;
        }
        return EventName;
    }

    public void EventRecoredChangePage(bool NextorPrevious) {
        if (NextorPrevious) {
            EventRecoredPage++;
            EventRecoredDataUpdate(EventRecoredPage);
        }
        else {
            if (EventRecoredPage > 0) {
                EventRecoredPage--;
                EventRecoredDataUpdate(EventRecoredPage);
            }
        }
    }

}
