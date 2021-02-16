using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Runtime.InteropServices;

public class BackEnd_IniFile : MonoBehaviour
{
    string path = Application.streamingAssetsPath + "/Serial.ini";//路徑

    [DllImport("kernel32")]
    public static extern long WritePrivateProfileString(string section, string key, string value, string path);
    [DllImport("kernel32")]
    public static extern int GetPrivateProfileString(string section, string key, string deval, StringBuilder stringBuilder, int size, string path);

    void Awake()
    {
        if (path == null) path = Application.streamingAssetsPath + "/Serial.ini";
        //Debug.Log(path);
    }

    //寫入
    public void WriteIniContent(string section, string key, string value)
    {
        WritePrivateProfileString(section, key, value, this.path);
    }

    //讀取
    public string ReadIniContent(string section, string key)
    {
        StringBuilder temp = new StringBuilder(255);
        int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
        return temp.ToString();
    }
}
