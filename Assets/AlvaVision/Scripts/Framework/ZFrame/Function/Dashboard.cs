using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour
{
    [Tooltip("仪表盘显示角度")]
    private float Angle = 105;

    [Space(10)]
    [Tooltip("背景图")]
    public Image ima_back;
    [Tooltip("指针图")]
    public Image ima_zhen;

    [Space(10)]
    [Tooltip("单位刻度")]
    private int BigSplite = 5;
    [Tooltip("单位分刻度")]
    private int SmallSplite = 1;
    [Tooltip("实际最小值")]
    private float RealMin = 30;
    [Tooltip("实际最大值")]
    private float RealMax = 270;

    float endPosMax, startPosMin;

    [Tooltip("显示精度")]
    private int Precision = 0;

    [Space(10)]
    [Tooltip("当前值")]
    private float Current = 1;

    private void OnEnable()
    {
        InitView();
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            return;
        }
        if (EventSystem.current.currentSelectedGameObject.layer == 8)
        {
            SelctDashboard();
        }
    }

    public delegate bool OnSelctDashboardHandler();
    public event OnSelctDashboardHandler SelctDashboard;

    /// <summary>
    /// 制作表盘
    /// </summary>
    private void InitView()
    {
        StartCoroutine(DesChild(transform.Find("BigParent"), CreateView_Real));
    }

    /// <summary>
    /// 仪表旋转的真实角度和数据
    /// </summary>
    private void CreateView_Real()
    {
        //Debug.Log(RealMin + "  " + RealMax + "  " + BigSplite + "  " + SmallSplite + "  " + Angle);

        transform.Find("BigParent").localEulerAngles = Vector3.zero;
        GameObject goBig = Resources.Load<GameObject>("Prefab/bigSp");
        GameObject goSmall = Resources.Load<GameObject>("Prefab/SmallSp");

        endPosMax = 360 - (360 - Angle) / 2;
        startPosMin = (360 - Angle) / 2;
        float adv = (endPosMax - startPosMin) / BigSplite;
        float advsm = adv / (SmallSplite + 1);

        for (int i = BigSplite; i >= 0; i--)
        {
            //1、计算起始位置realmin和结束位置realmax
            float currentValue = startPosMin + adv * i;
            GameObject goBigTemp = Instantiate(goBig, transform.Find("BigParent"));
            goBigTemp.transform.eulerAngles = new Vector3(0, 0, -currentValue - 90);

            float realCurrent = RealMin + (RealMax - RealMin)/BigSplite * i;
            goBigTemp.GetComponentInChildren<Text>().text = realCurrent.ToString("N" + Precision);

            if (currentValue + adv > endPosMax) continue;
            for (int j = SmallSplite - 1; j >= 0; j--)
            {
                GameObject goSamllTemp = Instantiate(goSmall, goBigTemp.transform);
                goSamllTemp.transform.eulerAngles = new Vector3(0, 0, -currentValue - 90 - (j+1) * advsm);

                //float realadvsm = (realMax - realMin) / bigSplite / (smallSplite + 1);
                //goSamllTemp.GetComponentInChildren<Text>().text = ((j + 1) * realadvsm).ToString(".0");
            }
        }

        transform.Find("BigParent").localEulerAngles = new Vector3(0, 0, 180f);

        SetZhenValue();
    }

    IEnumerator DesChild(Transform tfparent,Action overAction = null)
    {
        if (tfparent != null)
        {
            while (tfparent.childCount > 0)
            {
                Destroy(transform.Find("BigParent").GetChild(0).gameObject);
                yield return new WaitForEndOfFrame();
            }
        }
        overAction?.Invoke();
    }

    /// <summary>
    /// 仪表指针的真实位置
    /// </summary>
    private void SetZhenValue()
    {
        if (Mathf.Abs(Current - ima_zhen.transform.eulerAngles.z) > 0.2f)
        {
            Current = Mathf.Clamp(Current, RealMin, RealMax);
            float cur = (Current - RealMin) / (RealMax - RealMin) * (endPosMax - startPosMin) + startPosMin;
            float f = Mathf.LerpAngle(ima_zhen.transform.eulerAngles.z, -cur,1f);
            ima_zhen.transform.eulerAngles = new Vector3(0, 0,f);
        }
    }
    /// <summary>
    /// 设置显示初始值
    /// </summary>
    /// <param name="current"></param>
    public float SetCurrentValue(float current)
    {
        this.Current = current;
        Current = Mathf.Clamp(Current, RealMin, RealMax);

        SetZhenValue();

        return Current;
    }
    /// <summary>
    /// 设置显示的值域
    /// </summary>
    /// <param name="min">最小数值</param>
    /// <param name="max">最大数值</param>
    public void SetShowValue(float min,float max)
    {
        RealMin = min;
        RealMax = max;
        InitView();
    }

    /// <summary>
    /// 设置单位刻度
    /// </summary>
    /// <param name="bigSplite">外层刻度</param>
    /// <param name="smallSplit">内层分刻度</param>
    public void SetSpliteValue(int bigSplite,int smallSplit = 1)
    {
        BigSplite = bigSplite;
        SmallSplite = smallSplit;

        InitView();
    }
    /// <summary>
    /// 表盘显示的角度
    /// </summary>
    /// <param name="angle">角度值</param>
    public void SetAngleValue(float angle)
    {
        Angle = angle;
        InitView();
    }

    public void InitAllData(float min, float max,int bigSplite, int smallSplit, float angle)
    {
        RealMin = min;
        RealMax = max;
        BigSplite = bigSplite;
        SmallSplite = smallSplit;
        Angle = angle;

        //InitView();
    }
}
