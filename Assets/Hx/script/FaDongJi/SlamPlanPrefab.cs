using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamPlanPrefab : MonoBehaviour
{
    public GameObject child;
   
    private void Update()
    {
        //print(child.name + ":" +child. transform.localScale);
        child.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(child.transform.localScale.x, child.transform.localScale.z);
    }
 
}
