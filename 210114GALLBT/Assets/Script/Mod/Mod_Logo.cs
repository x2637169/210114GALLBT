using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mod_Logo : MonoBehaviour
{
    public GameObject Logo;
    public Sprite[] changeLogo;
    float ChangeTime = 0;
    int Picture = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Mod_Data.BonusSwitch) {
            Logo.SetActive(true);
            ChangeTime = ChangeTime + Time.deltaTime;

            if (ChangeTime > 3) {
                Picture++;
                if (Picture == changeLogo.Length) {
                    Picture = 0;
                }
                Logo.GetComponent<Image>().sprite = changeLogo[Picture];
                ChangeTime = 0;
            }

        }
        else {
            Logo.SetActive(false);
        }
    }
}
