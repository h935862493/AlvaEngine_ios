using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FaDongJiManager : MonoBehaviour
{
    public static FaDongJiManager instance;
    public AudioSource audio0;
    public Material saomiao_mt;
    public float snum = 0;
    public float sspeed = 1;
    bool ishide = false;
    bool isCut;
    public GameObject saomiao, chaijie, fire, btn;

    public AlvaType alvaType = AlvaType.ImageTarget;

    /// <summary>
    /// 扫描特效的依据父物体
    /// </summary>
    public Transform saoMiaoUVpos;
    public float waitUvTime = 4.5f;
    public GameObject ResetButton;
    //public MeshRenderer[] renderers;
    //List<Material> m = new List<Material>();
    private void Awake()
    {
        instance = this;
        //Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Update()
    {
        if (ishide)
        {
            //print("///////////_CapturePoint:" + saomiao_mt.GetVector("_CapturePoint"));
            switch (alvaType)
            {
                case AlvaType.SLAM:
                    saomiao_mt.SetFloat("_Max", 1);
                    saomiao_mt.SetVector("_CapturePoint", new Vector4(
            saoMiaoUVpos.position.x,
            saoMiaoUVpos.position.y,
            saoMiaoUVpos.position.z,
            0));
                    break;
                case AlvaType.ImageTarget:
                    //        saomiao_mt.SetFloat("_Max", 30);
                    //        saomiao_mt.SetVector("_CapturePoint", new Vector4(
                    //saoMiaoUVpos.position.x,
                    //saoMiaoUVpos.position.y,
                    //saoMiaoUVpos.position.z + 75,
                    //0));
                    break;
                case AlvaType.ModelTarget:
                    saomiao_mt.SetFloat("_Max", 6.5f);
                    saomiao_mt.SetVector("_CapturePoint", new Vector4(
            saoMiaoUVpos.position.x,
            saoMiaoUVpos.position.y,
            saoMiaoUVpos.position.z,
            0));
                    break;

            }
            //print("///////////_CapturePoint:" + saomiao_mt.GetVector("_CapturePoint"));
            if (isCut)
            {
                snum -= Time.deltaTime * sspeed;
                if (snum <= -0.1f)
                {
                    isCut = false;
                }
            }
            else
            {
                snum += Time.deltaTime * sspeed;
                if (snum >= 1.1f)
                {
                    isCut = true;
                }
            }
            saomiao_mt.SetFloat("_Threshold", snum);
        }


        if (ResetButton && Input.GetMouseButtonDown(0))//Input.touchCount > 0)
        {
            if (EventSystem.current.currentSelectedGameObject && (EventSystem.current.currentSelectedGameObject.name == "Button_menu" || EventSystem.current.currentSelectedGameObject.name == "ResetButton"))
            {
                return;
            }
            else
                ResetButton.gameObject.SetActive(false);
        }
    }

    bool isFirst = true;
    public void Found()
    {
        if (isFirst)
        {
            isFirst = false;
            StartCoroutine("Begin");
        }
        else
        {
            Begin2();
        }
    }

    public IEnumerator Begin()
    {
        audio0.Play();
        saomiao.SetActive(true);
        ishide = true;
        yield return new WaitForSeconds(waitUvTime);
        saomiao.SetActive(false);
        chaijie.SetActive(true);
        ishide = false;
        saomiao_mt.SetFloat("_Threshold", -0.1f);
        btn.SetActive(true);
        //saomiaoUI.SetActive(false);
    }
    public void Begin2()
    {
        saomiao.SetActive(false);
        if (!chaiStats && !fireStats)
            chaijie.SetActive(true);
        else if (chaiStats)
        {
            chaijie.SetActive(chaiStats);
            chaijie.GetComponent<Animator>().enabled = true;
            chaijie.GetComponent<Animator>().Play(aniName, 0, 0);
            chaijie.GetComponent<Animator>().speed = 1;

        }
        else
            chaijie.SetActive(chaiStats);
        fire.SetActive(fireStats);

        ishide = false;
        btn.SetActive(true);
    }
    bool chaiStats, fireStats;
    string aniName;
    public void Lost()
    {
        StopCoroutine("Begin");
        ishide = false;
        chaiStats = chaijie.activeSelf;
        fireStats = fire.activeSelf;

        saomiao.SetActive(false);
        chaijie.SetActive(false);
        fire.SetActive(false);
        btn.SetActive(false);
        snum = 0;
    }
    //爆炸动画
    public void Boom_btn()
    {
        audio0.Play();
        fire.SetActive(false);
        chaijie.SetActive(true);
        chaijie.GetComponent<Animator>().enabled = true;
        aniName = "baozha";
        chaijie.GetComponent<Animator>().Play("baozha", 0, 0);
        chaijie.GetComponent<Animator>().speed = 1;
    }

    public void Error_btn()
    {
        audio0.Play();
        fire.SetActive(false);
        chaijie.SetActive(true);
        chaijie.GetComponent<Animator>().enabled = true;
        aniName = "weixiu";
        chaijie.GetComponent<Animator>().Play("weixiu", 0, 0);
        chaijie.GetComponent<Animator>().speed = 1;
    }

    //集合动画
    public void Gather_btn()
    {
        audio0.Play();
        fire.SetActive(false);
        chaijie.SetActive(true);
        chaijie.GetComponent<Animator>().enabled = true;
        aniName = "gather";
        chaijie.GetComponent<Animator>().Play("gather", 0, 0);
        chaijie.GetComponent<Animator>().speed = 1;
    }
    //运行动画
    public void Fire_btn()
    {
        audio0.Play();
        chaijie.SetActive(false);
        fire.SetActive(true);
    }

    public void Return_btn()
    {
        //print("Return_btn()");
        if (MyMessageData.assetBundle_FDJ1 != null)
        {
            //print("Return_btn()1111111");
            MyMessageData._asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("FDJ1");
        }
        else
        {
            //print("Return_btn()2222222");
            //Screen.orientation = ScreenOrientation.Portrait;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainTemp");
        }
    }
    public void Rest_btn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Btn_Menu(GameObject g)
    {
        g.SetActive(!g.activeSelf);
    }

    private void OnDisable()
    {
        StopCoroutine("Begin");
        saomiao_mt.SetFloat("_Threshold", -0.1f);
        saomiao_mt.SetFloat("_Max", 1);
    }
}
