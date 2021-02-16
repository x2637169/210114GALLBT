using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RNGUtility;
using Newtonsoft.Json.Linq;

public class Mod_ChangeFish : MonoBehaviour
{
    public int GetChangeFishID()//取得換魚的答案
    {
        JArray changeFishMath;
        int[] changeFishMathArray;
        int changeFishMathTotal = 0;//隨機總數

        RouletteMathReload(out changeFishMath);//讀取JSON檔
        MakeRandomBox(changeFishMath, out changeFishMathArray, out changeFishMathTotal);//製作抽取池
        return RandomNumber(changeFishMathArray, changeFishMathTotal);//抽選答案並存起來;
    }

    void RouletteMathReload(out JArray fishMath)//讀取JSON檔
    {
        Mod_Data.JsonReload();
        fishMath = (JArray)Mod_Data.JsonObj["SpecialRule"]["ChangeCandidate"];
    }

    void MakeRandomBox(JArray fishMath, out int[] mathArray, out int mathTotal)//製作隨機抽取池
    {
        mathTotal = 0;
        mathArray = new int[fishMath.Count+1];
        mathArray[0] =0;
        for (int i = 0; i < fishMath.Count; i++)
        {
            mathTotal += int.Parse(fishMath[i].ToString());
            mathArray[i] = mathTotal;
        }
    }

    int RandomNumber(int[] mathArray, int mathTotal)//隨機抽取數字並存起來
    {
        int randomNum = RandomRng(mathTotal);
        for (int i = 0; i < mathArray.Length; i++)
        {
            if (i < mathArray.Length - 1)
            {
                if ((randomNum >= mathArray[i]) && (randomNum <= mathArray[i + 1]))
                {
                    return i + 2; //從p1開始icon數字2
                }
            }
        }

        return 2;
    }

    /// <summary>
    /// 依據傳送最大值，判斷亂數演算使用方式，並取得一個亂數值。
    /// </summary>
    /// <returns>亂數值</returns>
    int RandomRng(int maximum)
    {
        try
        {
            if (maximum <= 255)
            {
                RNGInt8 randomRng = new RNGInt8((uint)maximum);
                return (int)randomRng.Next();
            }
            else if (maximum <= 65535)
            {
                RNGInt16 randomRng = new RNGInt16((uint)maximum);
                return (int)randomRng.Next();
            }
            else if (maximum <= 16777215)
            {
                RNGInt24 randomRng = new RNGInt24((uint)maximum);
                return (int)randomRng.Next();
            }
            else if ((uint)maximum <= 4294967295)
            {
                RNGInt32 randomRng = new RNGInt32((uint)maximum);
                return (int)randomRng.Next();
            }
        }
        catch
        {
            return 0;
        }
        
        return 0;
    }
}
