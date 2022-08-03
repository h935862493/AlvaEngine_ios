using UnityEngine;
using UnityEngine.UI;
using System;

public class HxUI_DownLoadPanel : MonoBehaviour
{
    public Slider slider;

    int index = 1, total = 1;
    public Text text;

    public Text ProcessText;

    // Start is called before the first frame update
    void Start()
    {
        text.text = index + "/" + total;
        slider.onValueChanged.AddListener(SliderValueChanged);
    }

    public void DownLoadResourcesFromServer(string url, string fileName, Action<string> onfinished = null)
    {
        total = 1;
        this.url = url;
        this.fileName = fileName;
        _onfinished = onfinished;
        System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(DownLoadFile));
        thread.Start();
    }
    private Action<string> _onfinished;
    private string url, fileName;


    private void DownLoadFile()
    {
        lock (ZManager.instnace.zServer)
        {
            ZManager.instnace.zServer.HxDownLoadFile_HTTP(url, fileName, true,
                (path) =>
                {
                    index++;
                    if (index > total)
                    {
                        ZManager.instnace.zServer.QueueOnMainThread((new_y) =>
                        {
                            //Debug.Log("hxxxxxx00000资源下载完毕new_y:" + new_y);
                            if (string.IsNullOrEmpty(new_y.ToString()))
                            {
                                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                                box.SetTipData("您异地登录，您的登录信息已过期，请重新登录！");
                                DestroyImmediate(gameObject);
                            }
                            _onfinished?.Invoke(new_y.ToString());
                        }, path);
                    }
                    else
                    {
                        text.text = index + "/" + total;
                        //Debug.Log(index + "=====" + total);
                    }
                }, (f) =>
                {
                    ZManager.instnace.zServer.QueueOnMainThread((new_f) =>
                    {
                        ProcessSlider(float.Parse(new_f.ToString()));
                    }, f);
                }
            );


        }

    }

    private void SliderValueChanged(float arg0)
    {
        ProcessText.text = "下载:" + arg0.ToString("P");
    }

    void ProcessSlider(float g)
    {
        slider.value = g;
    }
}
