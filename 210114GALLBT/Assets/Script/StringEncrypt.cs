using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;


public static class StringEncrypt
{
    public static string strKey = "1470Afj1qRu2zvM3";
    public static string strIV = "1zSxeD4rB5hyUjKi";
    public static string salt = "X0X0V0Q";
  
    //strKey 金鑰(16位)
    //strIV IV(16位)
    public static string Encryptor(string TextToEncrypt)
    {
        //Turn the plaintext into a byte array. 
        byte[] PlainTextBytes = System.Text.UTF8Encoding.UTF8.GetBytes(TextToEncrypt);

        //Setup the AES providor for our purposes. 
        AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();

        aesProvider.BlockSize = 128;
        aesProvider.KeySize = 256;
        //My key and iv that i have used in openssl 
        aesProvider.Key = System.Text.Encoding.UTF8.GetBytes(strKey);
        aesProvider.IV = System.Text.Encoding.UTF8.GetBytes(strIV);
        aesProvider.Padding = PaddingMode.PKCS7;
        aesProvider.Mode = CipherMode.CBC;

        ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor(aesProvider.Key, aesProvider.IV);
        byte[] EncryptedBytes = cryptoTransform.TransformFinalBlock(PlainTextBytes, 0, PlainTextBytes.Length);
        return Convert.ToBase64String(EncryptedBytes);
    }

    //strKey 金鑰(16位)
    //strIV IV (16位)
    public static string Decryptor(string TextToDecrypt)
    {
        byte[] EncryptedBytes = Convert.FromBase64String(TextToDecrypt);

        //Setup the AES provider for decrypting.    
        AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
        //aesProvider.Key = System.Text.Encoding.UTF8.GetBytes(strKey); 
        //aesProvider.IV = System.Text.Encoding.UTF8.GetBytes(strIV); 
        aesProvider.BlockSize = 128;
        aesProvider.KeySize = 256;
        //My key and iv that i have used in openssl 
        aesProvider.Key = System.Text.Encoding.UTF8.GetBytes(strKey);
        aesProvider.IV = System.Text.Encoding.UTF8.GetBytes(strIV);
        aesProvider.Padding = PaddingMode.PKCS7;
        aesProvider.Mode = CipherMode.CBC;


        ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
        byte[] DecryptedBytes = cryptoTransform.TransformFinalBlock(EncryptedBytes, 0, EncryptedBytes.Length);
        return System.Text.Encoding.UTF8.GetString(DecryptedBytes);
    }

    public static string ComputeSha256Hash(string rawData)
    {

        string returnString;
        if (rawData.Length < 7)
        {
            rawData = (int.Parse(rawData) * 16791).ToString();

            // Create a SHA1   
            string data = "QV0WWE!d6wyRfaPTC47O" + rawData + salt;
            using (SHA1 sha256Hash = SHA1.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                returnString = salt + builder.ToString() + "nePFjH&iwk5eZR*@neJl";
                return returnString;
            }
        }
        else
        {
            if (rawData.Substring(0, 7) != salt)
            {
                rawData = (int.Parse(rawData) * 16791).ToString();

                // Create a SHA1   
                string data = "QV0WWE!d6wyRfaPTC47O" + rawData + salt;
                using (SHA1 sha256Hash = SHA1.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                    // Convert byte array to a string   
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    returnString = salt + builder.ToString() + "nePFjH&iwk5eZR*@neJl";
                    return returnString;
                }
            }
            else
            {
                returnString = rawData;
            }
        }

        return returnString;
        //return rawData;
    }
   
}

