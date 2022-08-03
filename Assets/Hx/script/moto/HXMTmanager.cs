using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HXMTmanager : MonoBehaviour
{
    public static HXMTmanager instance;
    public IOSDemo iOSDemo;
    public GameObject modelUV, mt_old, mt_new, mt_new2, shiti, shiti2, modelAni, gif, chaijie, btnParent, errorModel;

    List<MachineEffectCtrl> EffectList;
    Dictionary<MeshRenderer, Material[]> modelsMatsDic;
    public Material ModelsShaderMat, errorMt, errorModelOldMt;
    public GameObject gifloGo;
    public AudioSource audio0;
    public Camera cam;
    public GameObject TipsBg;
    public Text tip;

    bool isFirstFound = true;
    bool isShan = false;

    private void Awake()
    {
        instance = this;

    }
    private void Start()
    {
        modelsMatsDic = new Dictionary<MeshRenderer, Material[]>();
        EffectList = new List<MachineEffectCtrl>();

        MeshRenderer[] renders = modelUV.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer item in renders)
        {
            Material[] mats = item.materials;
            modelsMatsDic.Add(item, mats);
            MachineEffectCtrl eff = item.gameObject.AddComponent<MachineEffectCtrl>();
            EffectList.Add(eff);
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                //Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到
                GameObject gameObj = hitInfo.collider.gameObject;
                //Debug.Log("click object name is " + gameObj.name);
                if (gameObj.tag == "point")
                {
                    SemicircleMenuManager.instance.Panel2_4_hot.GetComponent<SemicircleMenuRoot>().BtnSelect(gameObj.transform.parent.GetComponent<HotEvent>().id);
                }
                if (gameObj.name == "KTM2")
                {
                    audio0.Play();
                    gameObj.GetComponent<EffectGO>().enabled = true;
                   

                }
                else if (gameObj.name == "KTM")
                {
                    audio0.Play();
                    gameObj.GetComponent<EffectGO>().enabled = true;
                   
                }
                if (gameObj.name == "errorTip")
                {
                    audio0.Play();
                    gameObj.transform.parent.gameObject.SetActive(false);
                }
                if (gameObj.name == "switch1")
                {
                    SemicircleMenuManager.instance.Panel2_2_color.GetComponent<SemicircleMenuRoot>().BtnSelect(0);
                }
                else if (gameObj.name == "switch2")
                {
                    SemicircleMenuManager.instance.Panel2_2_color.GetComponent<SemicircleMenuRoot>().BtnSelect(1);
                }
            }
        }
    }
    public void Btn_Home()
    {
        
        modelUV.SetActive(true);
        modelUV.GetComponent<EffectGO>().enabled = true;
        modelUV.GetComponent<Collider>().enabled = true;
        modelUV.GetComponent<EffectGO>().ShowHide2();
        mt_new2.GetComponent<EffectGO>().enabled = true;
        mt_new2.GetComponent<Collider>().enabled = true;
        mt_new2.GetComponent<EffectGO>().ShowHide2();
        gif.SetActive(true);
    }
    public void Btn_point(GameObject go)
    {
        audio0.Play();
        go.transform.parent.GetComponent<HotEvent>().SetObjActive();
        go.transform.parent.GetComponent<HotEvent>().SetOtherActive();
    }

    public void Btn_switch1(int n = 0)
    {
        //print("****111111111");
        audio0.Play();
        modelUV.SetActive(false);
        modelUV = mt_new;
        modelUV.SetActive(true);
        iOSDemo.shitiMT = shiti;
        mt_new2.GetComponent<EffectGO>().ShowHide2();

    }
    public void Btn_switch2(int n = 0)
    {
        //print("****22222222");
        audio0.Play();
        modelUV.SetActive(false);
        modelUV = mt_new2;
        modelUV.SetActive(true);
        iOSDemo.shitiMT = shiti2;
        mt_new.GetComponent<EffectGO>().ShowHide2();
    }
   
    public void Found()
    {
        if (isFirstFound)
        {
            isFirstFound = false;
            audio0.Play();
            modelUV = mt_old;
            mt_old.transform.localPosition = Vector3.zero;
            //scenelight.SetActive(false);
            modelAni.SetActive(false);
            modelUV.SetActive(true);
            StartCoroutine(UVAni());
        }
        else
        {
            if (isShan)
            {
                isShan = false;
                modelUV.SetActive(false);
                modelUV = mt_new;
                modelUV.SetActive(true);
                iOSDemo.shitiMT = shiti;
                modelUV.GetComponent<EffectGO>().enabled = true;
                modelUV.GetComponent<Collider>().enabled = true;
                modelUV.GetComponent<EffectGO>().ShowHide();
                mt_new2.GetComponent<EffectGO>().enabled = true;
                mt_new2.GetComponent<Collider>().enabled = true;
                mt_new2.GetComponent<EffectGO>().ShowHide();

                gifloGo.SetActive(true);
                gif.SetActive(true);
                btnParent.SetActive(true);
            }

        }
    }
    IEnumerator UVAni()
    {
        isShan = true;
        //扫描效果
        modelUV.GetComponent<Collider>().enabled = false;
        modelUV.GetComponent<EffectGO>().enabled = true;
        modelUV.GetComponent<EffectGO>().Flash();
        yield return new WaitForSeconds(5f);

        //黄色透明效果
        modelUV.GetComponent<EffectGO>().enabled = false;
        yield return StartCoroutine(SetModelMats());
        isShan = false;
        
        modelUV.SetActive(false);
        modelUV = mt_new;
        modelUV.SetActive(true);
        iOSDemo.shitiMT = shiti;
        modelUV.GetComponent<EffectGO>().enabled = true;
        modelUV.GetComponent<Collider>().enabled = true;
        modelUV.GetComponent<EffectGO>().ShowHide();
        mt_new2.GetComponent<EffectGO>().enabled = true;
        mt_new2.GetComponent<Collider>().enabled = true;
        mt_new2.GetComponent<EffectGO>().ShowHide();

        gifloGo.SetActive(true);
        gif.SetActive(true);
        btnParent.SetActive(true);

    }

    
    IEnumerator SetModelMats()
    {
        foreach (var item in modelsMatsDic.Keys)
        {
            Material[] mats = new Material[item.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = ModelsShaderMat;
            }
            item.materials = mats;
        }
        foreach (var item in EffectList)
        {
            item.Init();
        }
        yield return new WaitForSeconds(3.5f);
        foreach (var item in EffectList)
        {
            item.StopMachineEffect();
        }
        foreach (var item in modelsMatsDic.Keys)
        {
            item.sharedMaterials = modelsMatsDic[item];
        }
    }

    /// <summary>
    /// 消隐效果结束，动图
    /// </summary>
    public void ShowOver()
    {
        modelUV.GetComponent<Collider>().enabled = false;
        modelAni.SetActive(true);
        //chaijie.SetActive(true);
        gifloGo.SetActive(true);
        gif.SetActive(true);
        btnParent.SetActive(true);
    }

    public void ErrorShow()
    {
        errorModel.SetActive(!errorModel.activeSelf);
    }
    public void ShowTip(string info)
    {
        if (TipsBg)
        {
            tip.text = info;
            TipsBg.SetActive(true);
        }
    }
}
