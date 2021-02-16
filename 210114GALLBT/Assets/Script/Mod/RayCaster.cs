using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    public void GetSymbolPlayAnimation(bool playAnim)//
    {
        Collider[] hit = Physics.OverlapSphere(this.transform.position, 2f);
        if (hit.Length > 0)
        {
            hit[0].GetComponent<SlotSymbol>().PlayIconAnimation(playAnim);
        }
    }

   public void GetSymbolOpenAnimator(bool openAnim)//開啟動畫,動畫結尾會自動關掉(程式在animator裡)
    {
        Collider[] hit = Physics.OverlapSphere(this.transform.position, 2f);
        if (hit.Length > 0)
        {
            hit[0].GetComponent<Animator>().enabled = openAnim;
        }
    }

}
