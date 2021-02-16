using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DenomListClose : MonoBehaviour
{
    // Start is called before the first frame update
    Vector2 aPos;
    Image img;
    void Start()
    {
        aPos = GetComponent<RectTransform>().anchoredPosition;
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (aPos.y <= 45.5f)
        {
            if(img.enabled==true)
            img.enabled = false;
        }
        else
        {
            if (img.enabled == false)
                img.enabled = true;
        }
    }
}
