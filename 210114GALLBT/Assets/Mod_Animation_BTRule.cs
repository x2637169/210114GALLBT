using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mod_Animation_BTRule : IGameSystem
{
    public AnimationClip FishChangeClip;
    public static float fishChangeTIme;
    public GameObject[] FishChangeReels;
    private Image[,] FishCangerIconImages = new Image[Mod_Data.slotReelNum, Mod_Data.slotRowNum];
    private Animator[,] FishCangerIconAnims = new Animator[Mod_Data.slotReelNum, Mod_Data.slotRowNum];

    void Start()
    {
        fishChangeTIme = FishChangeClip.length;

        for (int i = 0; i < FishCangerIconAnims.GetLength(0); i++)
        {
            for (int y = 0; y < FishCangerIconAnims.GetLength(1); y++)
            {
                FishCangerIconImages[i, y] = FishChangeReels[i].transform.GetChild(y).GetComponent<Image>();
                FishCangerIconAnims[i, y] = FishChangeReels[i].transform.GetChild(y).GetComponent<Animator>();
                FishCangerIconImages[i, y].enabled = false;
            }
        }

    }

    public void EnableFishChangeImages(bool isEnable)
    {
        for (int i = 0; i < FishCangerIconImages.GetLength(0); i++)
        {
            for (int y = 0; y < FishCangerIconImages.GetLength(1); y++)
            {
                    FishCangerIconImages[i, y].enabled = isEnable;
            }
        }
    }

    public void PlayFishChangeAnimtion()
    {
        for (int g = 0; g < Mod_Data.waygame_combineIconBool.GetLength(0); g++)
        {
            if (Mod_Data.isFishChange_BTRule[g])
            {
                for (int i = 0; i < Mod_Data.waygame_combineIconBool.GetLength(1); i++)
                {
                    for (int y = 0; y < Mod_Data.waygame_combineIconBool.GetLength(2); y++)
                    {
                        if (Mod_Data.waygame_combineIconBool[g, i, y])
                        {
                            FishCangerIconImages[i, y].enabled = true;
                            FishCangerIconAnims[i, y].SetTrigger("FishChangeAnim");
                        }
                    }
                }
            }
        }
    }
}
