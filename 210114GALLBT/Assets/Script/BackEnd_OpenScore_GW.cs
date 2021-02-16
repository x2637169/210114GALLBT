using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackEnd_OpenScore_GW : MonoBehaviour
{
    [SerializeField] Mod_UIController mod_UIController;
    [SerializeField] Mod_OpenClearPoint mod_OpenClearPoint;
    [SerializeField] Text tmpCreditText, tmpCreditInfoText;
    [SerializeField] double tmpCredit;

    int[] takeInScoreRange = new int[] { 1, 2, 5, 10, 20, 50, 100, 500, 1000, 5000 };
    int[] takeOutScoreRange = new int[] { 1, 2, 5, 10, 20, 50, 100, 500, 1000, 5000 };

    public void AddTmpCredit(string creditString)
    {
        tmpCredit += double.Parse(creditString);
        tmpCreditText.text = tmpCredit.ToString();
    }

    public void ApplyTmpCredit()
    {
        if (tmpCredit == 0) return;
        StopCoroutine("CleanText");
        tmpCreditText.text = null;
        tmpCreditInfoText.text = "TOTAL CREDITS ADDED : " + tmpCredit;
        mod_OpenClearPoint.OpenPointFunctionButton((int)tmpCredit);
        tmpCredit = 0;
        mod_UIController.UpdateScore();
        StartCoroutine("CleanText");
    }

    IEnumerator CleanText()
    {
        yield return new WaitForSeconds(3f);
        tmpCreditInfoText.text = null;
    }

    public void ResetTmpCredit()
    {
        if (tmpCredit == 0) return;
        tmpCredit = 0;
        tmpCreditText.text = null;
    }
}
