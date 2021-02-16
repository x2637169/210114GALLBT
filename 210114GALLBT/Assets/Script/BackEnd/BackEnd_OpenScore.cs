using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BackEnd_OpenScore : MonoBehaviour
{
    [SerializeField] BackEnd_Data backEnd_Data;
    [SerializeField] Text moneyMachineText, takeInMoneyText, takeOutMoneyText;
    [SerializeField] Button openScoreBtn, clearScoreBtn;

    int[] takeInScoreRange = new int[] { 1, 2, 5, 10, 20, 50, 100, 500, 1000, 5000 };
    int[] takeOutScoreRange = new int[] { 1, 2, 5, 10, 20, 50, 100, 500, 1000, 5000 };

    private void OnEnable()
    {
        //backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.moneyMachineOn, false);
        if (BackEnd_Data.moneyMachineOn) moneyMachineText.text = "ON";
        else moneyMachineText.text = "OFF";

        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.takeOutScoreNum, false);
        takeOutMoneyText.text = BackEnd_Data.takeOutScoreNum.ToString("N0");

        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.takeInScoreNum, false);
        takeInMoneyText.text = BackEnd_Data.takeInScoreNum.ToString("N0");

        openScoreBtn.interactable = Mod_Data.openclearSet;
        clearScoreBtn.interactable = Mod_Data.openclearSet;
    }
    public void SetMoneyMachineOn()
    {
        //backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.moneyMachineOn, false);
        BackEnd_Data.moneyMachineOn = !BackEnd_Data.moneyMachineOn;

        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.moneyMachineOn, BackEnd_Data.moneyMachineOn);
        if (BackEnd_Data.moneyMachineOn) moneyMachineText.text = "ON";
        else moneyMachineText.text = "OFF";

    }
    public void TakeOutMoneyButton()
    {
        if (BackEnd_Data.takeOutScoreNum == takeOutScoreRange[takeOutScoreRange.Length - 1])
        {
            BackEnd_Data.takeOutScoreNum = takeOutScoreRange[0];
        }
        else
        {
            for (int i = 0; i < takeOutScoreRange.Length; i++)
            {
                if (takeOutScoreRange[i] > BackEnd_Data.takeOutScoreNum)
                {
                    BackEnd_Data.takeOutScoreNum = takeOutScoreRange[i];
                    break;
                }
            }
        }
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.takeOutScoreNum, true);
        //Debug.Log("BackEnd_Data.takeOutScoreNum"+BackEnd_Data.takeOutScoreNum);

        Mod_Data.takeOutScoreNum = BackEnd_Data.takeOutScoreNum;
        takeOutMoneyText.text = BackEnd_Data.takeOutScoreNum.ToString("N0");
    }
    public void TakeInMoneyButton()
    {
        if (BackEnd_Data.takeInScoreNum == takeInScoreRange[takeInScoreRange.Length - 1])
        {
            BackEnd_Data.takeInScoreNum = takeInScoreRange[0];
        }
        else
        {
            for (int i = 0; i < takeInScoreRange.Length; i++)
            {
                if (takeInScoreRange[i] > BackEnd_Data.takeInScoreNum)
                {
                    BackEnd_Data.takeInScoreNum = takeInScoreRange[i];
                    break;
                }
            }
        }
        backEnd_Data.SaveLoadData(BackEnd_Data.SramMultiData.takeInScoreNum, true);
        Mod_Data.takeInScoreNum = BackEnd_Data.takeInScoreNum;
        takeInMoneyText.text = BackEnd_Data.takeInScoreNum.ToString("N0");
    }
}
