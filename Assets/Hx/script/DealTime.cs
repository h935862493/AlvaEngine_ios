using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealTime : MonoBehaviour
{

    float TT = 0;
    public float num = 2;
    private void OnEnable()
    {
        TT = num;
        StartCoroutine(CutTime());
    }
    private void OnDisable()
    {

        StopAllCoroutines();
        TT = num;
    }
    IEnumerator CutTime()
    {
        while (TT > 0)
        {
            TT -= 1;
            yield return new WaitForSeconds(1);
        }
        gameObject.SetActive(false);
    }


}
