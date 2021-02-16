using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class QRcode : MonoBehaviour
{

    //用于显示生成的二维码RawImage
    public RawImage m_QRCode;

    //申请一个写二维码的变量

    string message = "2020,7,28,8,3400,1800,0,0,1846B00983,111111111";

    void Start()
    {
        //启动方法
        /////////////////////////////////
        //ShowQRCode("http://ipcuse.azurewebsites.net/QRcode_OpenScore.aspx?psw=" + web.StringEncrypt.aesEncryptBase64(message, "322782").Replace("+", "%2B"), 512, 512);
        /////////////////////////////////
    }

    /// <summary>
    /// 显示绘制的二维码
    /// </summary>
    /// <param name="s_formatStr">扫码信息</param>
    /// <param name="s_width">码宽</param>
    /// <param name="s_height">码高</param>
    public static Texture2D ShowQRCode(string s_str, int s_width, int s_height)
    {
        //定义Texture2D并且填充
        Texture2D tTexture = new Texture2D(s_width, s_height);

        //绘制相对应的贴图纹理
        tTexture.SetPixels32(GeneQRCode(s_str, s_width, s_height));

        tTexture.Apply();

        //赋值贴图
        //m_QRCode.texture = tTexture;
        return tTexture;
    }

    /// <summary>
    /// 返回对应颜色数组
    /// </summary>
    /// <param name="s_formatStr">扫码信息</param>
    /// <param name="s_width">码宽</param>
    /// <param name="s_height">码高</param>
    static Color32[] GeneQRCode(string s_formatStr, int s_width, int s_height)
    {
        //设置中文编码格式，否则中文不支持
        QrCodeEncodingOptions tOptions = new QrCodeEncodingOptions();
        tOptions.CharacterSet = "UTF-8";
        //设置宽高
        tOptions.Width = s_width;
        tOptions.Height = s_height;
        //设置二维码距离边缘的空白距离
        tOptions.Margin = 0;
        
        BarcodeWriter m_barcodeWriter;
        //重置申请写二维码变量类       (参数为：码格式（二维码、条形码...）    编码格式（支持的编码格式）    )
        m_barcodeWriter = new BarcodeWriter { Format = BarcodeFormat.QR_CODE, Options = tOptions };

        //将咱们需要隐藏在码后面的信息赋值上
        return m_barcodeWriter.Write(s_formatStr);
    }
}