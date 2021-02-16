using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class BackEnd_RTPpassword : MonoBehaviour
{
    string passwrod,realPassword, tempPassword;
    
    [SerializeField] InputField inputField;
    [SerializeField] GameObject RTPobj;
    void Start()
    {
        //Debug.Log(DateTime.Now.Day);
    }
    private void OnEnable()
    {
        realPassword = passwordmaker();
    }
    private void OnDisable()
    {
        passwrod = "";
        realPassword = "";
        tempPassword = "";
    }
    // Update is called once per frame
    void Update()
    {
        inputField.text = passwrod;
        //Debug.Log(realPassword);
    }
    string passwordmaker()
    {
        tempPassword = "";


        switch (DateTime.Now.Day)
        {
            case 1:
                tempPassword += "10";
                break;
            case 2:
                tempPassword += "20";
                break;
            case 3:
                tempPassword += "30";
                break;
            case 4:
                tempPassword += "40";
                break;
            case 5:
                tempPassword += "50";
                break;
            case 6:
                tempPassword += "60";
                break;
            case 7:
                tempPassword += "70";
                break;
            case 8:
                tempPassword += "80";
                break;
            case 9:
                tempPassword += "90";
                break;
            case 10:
                tempPassword += "01";
                break;
            case 11:
                tempPassword += "11";
                break;
            case 12:
                tempPassword += "21";
                break;
            case 13:
                tempPassword += "31";
                break;
            case 14:
                tempPassword += "41";
                break;
            case 15:
                tempPassword += "51";
                break;
            case 16:
                tempPassword += "61";
                break;
            case 17:
                tempPassword += "71";
                break;
            case 18:
                tempPassword += "81";
                break;
            case 19:
                tempPassword += "91";
                break;
            case 20:
                tempPassword += "02";
                break;
            case 21:
                tempPassword += "12";
                break;
            case 22:
                tempPassword += "22";
                break;
            case 23:
                tempPassword += "32";
                break;
            case 24:
                tempPassword += "42";
                break;
            case 25:
                tempPassword += "52";
                break;
            case 26:
                tempPassword += "62";
                break;
            case 27:
                tempPassword += "72";
                break;
            case 28:
                tempPassword += "82";
                break;
            case 29:
                tempPassword += "92";
                break;
            case 30:
                tempPassword += "03";
                break;
            case 31:
                tempPassword += "13";
                break;
            default:
                break;
        }
        switch (DateTime.Now.Month)
        {
            case 1:
                tempPassword += "10";
                break;
            case 2:
                tempPassword += "20";
                break;
            case 3:
                tempPassword += "30";
                break;
            case 4:
                tempPassword += "40";
                break;
            case 5:
                tempPassword += "50";
                break;
            case 6:
                tempPassword += "60";
                break;
            case 7:
                tempPassword += "70";
                break;
            case 8:
                tempPassword += "80";
                break;
            case 9:
                tempPassword += "90";
                break;
            case 10:
                tempPassword += "01";
                break;
            case 11:
                tempPassword += "11";
                break;
            case 12:
                tempPassword += "21";
                break;
            default:
                break;
        }
        tempPassword += "2892";
        return tempPassword;
    }
    public void inputPasswordButton(int inputPassword)
    {
        passwrod += inputPassword;
    }
    public void ComfirmButton()
    {
        if (passwrod== realPassword)
        {
            RTPobj.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
