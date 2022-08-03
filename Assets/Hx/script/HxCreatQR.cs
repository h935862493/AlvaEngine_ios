using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

public class HxCreatQR : MonoBehaviour
{
    //在屏幕上显示二维码  
    public RawImage image;
    public GameObject login, panel_qr;
    public Text tt;
    //////存放二维码  
    ////Texture2D encoded;
    int totalTime = 180;
    int num = 180;

    void Start()
    {
        num = totalTime;
        //encoded = new Texture2D(256, 256);
    }

    /// <summary>定义方法生成二维码</summary>
    /// <param name="textForEncoding">需要生产二维码的字符串</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width,
                //设置二维码边缘留白宽度（值越大留白宽度大，二维码就减小）
                Margin = 1
            }
        };
        return writer.Write(textForEncoding);
    }

    public bool isClick = false;
    public void BtnGetData()
    {
        if (isClick)
        {
            return;
        }
        isClick = true;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络连接失败，请检查是否联网！");
            return;
        }

        if (num == totalTime)
        {
            StartCoroutine(ZManager.instnace.zServer.Get(GlobalData.BaseUrl + GlobalData.QRcodeUrl, new string[2] { "alva_author_token", GlobalData.getUserInfo.token }, null, Btn_CreatQr));
        }
        else
        {
            login.SetActive(false);
            panel_qr.SetActive(true);
            isClick = false;
        }
    }

    /// <summary>  
    /// 生成二维码  
    /// </summary>  
    void Btn_CreatQr(DownloadHandler handle)
    {
        isClick = false;
        //print("/////handle.text:" + handle.text);
        if (string.IsNullOrEmpty(handle.text))
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("返回值为空");
            return;
        }
        NewServerMessage<string> mgs = GlobalData.DeserializeObject<NewServerMessage<string>>(handle.text);
        if (mgs.code.Equals(0))
        {
            if (mgs.data.Length > 1)
            {
                GenerateQRImage(mgs.data, 1024, 1024);

                login.SetActive(false);
                panel_qr.SetActive(true);
                StartCoroutine("Countdown");
            }
            else
            {
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                box.SetTipData("生成二维码失败");
            }
        }
        else
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(mgs.message)));
        }

    }

    IEnumerator Countdown()
    {
        tt.text = "二维码有效期：" + num.ToString();
        while (num > 0)
        {
            yield return new WaitForSeconds(1);
            num--;
            tt.text = "二维码有效期：" + num.ToString();
        }
        num = totalTime;
        panel_qr.SetActive(false);
        login.SetActive(true);

    }
    /// <summary>
    /// 刷新二维码
    /// </summary>
    public void UpdateQR()
    {
        StopCoroutine("Countdown");
        num = totalTime;
        BtnGetData();
    }

    /// <summary>
    /// 生成2维码 方法二
    /// 经测试：能生成任意尺寸的正方形
    /// </summary>
    /// <param name="content"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    void GenerateQRImage(string content, int width, int height)
    {
        // 编码成color32
        MultiFormatWriter writer = new MultiFormatWriter();
        Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
        hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
        hints.Add(EncodeHintType.MARGIN, 1);
        hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.M);
        BitMatrix bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, width, height, hints);

        // 转成texture2d
        int w = bitMatrix.Width;
        int h = bitMatrix.Height;
        //print(string.Format("w={0},h={1}", w, h));
        Texture2D texture = new Texture2D(w, h);
        for (int x = 0; x < h; x++)
        {
            for (int y = 0; y < w; y++)
            {
                if (bitMatrix[x, y])
                {
                    texture.SetPixel(y, x, Color.black);
                }
                else
                {
                    texture.SetPixel(y, x, Color.white);
                }
            }
        }
        texture.Apply();
        image.texture = texture;
    }
}
