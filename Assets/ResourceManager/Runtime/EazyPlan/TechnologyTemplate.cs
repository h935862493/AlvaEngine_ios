using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 工艺模板
/// </summary>
public class TechnologyTemplate : MonoBehaviour
{
    public GameObject UI;
    public string Name;
    public List<TechnologyProcess> technologyProcesses;

    private void Awake()
    {
        InitActive();
    }
   

    public void InitActive()
    {
        for (int i = 0; i < technologyProcesses[0].processSteps.Count; i++)
        {
            for (int j = 0; j < technologyProcesses[0].processSteps[i].stepResources.Count; j++)
            {
                technologyProcesses[0].processSteps[i].stepResources[j].Target.SetActive(true);
            }
        }
    }

    public void Up(int index)
    {
        if (index < 1)
        {
            return;
        }
        ProcessStep current = technologyProcesses[0].processSteps[index];
        ProcessStep up = technologyProcesses[0].processSteps[index - 1];
        int item = current.number;
        current.number = up.number;
        up.number = item;
        technologyProcesses[0].processSteps[index] = up;
        technologyProcesses[0].processSteps[index - 1] = current;
    }

    public void Down(int index)
    {
        //0 1 2...   === 2
        if (index == technologyProcesses[0].processSteps.Count - 1)
        {
            return;
        }
        ProcessStep current = technologyProcesses[0].processSteps[index];
        ProcessStep down = technologyProcesses[0].processSteps[index + 1];
        int item = current.number;
        current.number = down.number;
        down.number = item;
        technologyProcesses[0].processSteps[index] = down;
        technologyProcesses[0].processSteps[index + 1] = current;
    }

    public void Top(int index)
    {
        if (index == 0)
        {
            return;
        }
        ProcessStep current = technologyProcesses[0].processSteps[index];
        current.number = technologyProcesses[0].processSteps[0].number;
        technologyProcesses[0].processSteps.RemoveAt(index);
        for (int i = 0; i < index; i++)
        {
            technologyProcesses[0].processSteps[i].number = technologyProcesses[0].processSteps[i].number + 1;
        }
        technologyProcesses[0].processSteps.Insert(0,current);
    }

    public void Bottom(int index)
    {
        if (index == technologyProcesses[0].processSteps.Count - 1)
        {
            return;
        }
        ProcessStep current = technologyProcesses[0].processSteps[index];
        current.number = technologyProcesses[0].processSteps[technologyProcesses[0].processSteps.Count - 1].number;
        technologyProcesses[0].processSteps.RemoveAt(index);
        for (int i = index; i < technologyProcesses[0].processSteps.Count; i++)
        {
            technologyProcesses[0].processSteps[i].number = technologyProcesses[0].processSteps[i].number - 1;
        }
        technologyProcesses[0].processSteps.Insert(technologyProcesses[0].processSteps.Count, current);
    }

    public void AddStep()
    {
        ProcessStep step = new ProcessStep();
        step.number = technologyProcesses[0].processSteps.Count + 1;
        step.stepID = technologyProcesses[0].processSteps[technologyProcesses[0].processSteps.Count - 1].stepID + "1";
        step.stepResources = new List<StepResource>();
        technologyProcesses[0].processSteps.Add(step);
    }

    public void DeleteStep(List<bool> IsDeletes)
    {
        int index = 0;
        for (int i = 0; i < IsDeletes.Count; i++)
        {
            if (IsDeletes[i])
            {
                technologyProcesses[0].processSteps.RemoveAt(i + index);
                index--;
            }
        }
    }

    public void SelectStep(int index)
    {
        Debug.Log(index);
        for (int i = 0; i < technologyProcesses[0].processSteps.Count; i++)
        {
            //Debug.Log("i-----  " + i);
            for (int j = 0; j < technologyProcesses[0].processSteps[i].stepResources.Count; j++)
            {
                //Debug.Log("j=====  " + j);
                if (i == index)
                {
                    Debug.Log("true");
                    technologyProcesses[0].processSteps[i].stepResources[j].Target.SetActive(true);
                }
                else
                {
                    technologyProcesses[0].processSteps[i].stepResources[j].Target.SetActive(false);
                }
            }
        }
    }

    public void SelectTechnique()
    {
        InitActive();
    }

    public void DeleteRes(GameObject obj,int index)
    {
        for (int i = 0; i < technologyProcesses[0].processSteps.Count; i++)
        {
            for (int j = 0; j < technologyProcesses[0].processSteps[i].stepResources.Count; j++)
            {
                //Delete Select Resource
                if (i == index && obj == technologyProcesses[0].processSteps[i].stepResources[j].Target)
                {
                    technologyProcesses[0].processSteps[i].stepResources.RemoveAt(j);
                    return;
                }
            }
        }
    }
}
/// <summary>
/// 工艺流程
/// </summary>
[System.Serializable]
public class TechnologyProcess
{
    public string processID;
    public string Name;
    public bool extend;
    public List<ProcessStep> processSteps;
}
/// <summary>
/// 工艺步骤
/// </summary>
[System.Serializable]
public class ProcessStep
{
    public string stepID;
    public bool extend;
    public int number;
    public string text;
    public string describe;
    public GameObject StepObject;
    public List<StepResource> stepResources;
}
/// <summary>
/// 步骤中资源
/// </summary>
[System.Serializable]
public class StepResource
{
    public ResourceType Type;
    public GameObject Target;

    public enum ResourceType
    {
        IMAGE,
        MODEL,
        VIDEO,
        TEXT
    }
}