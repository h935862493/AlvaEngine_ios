using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StepMono : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Text WorkAreaText;

    public ToggleGroup Group;

    public Toggle TextToggle;
    public Toggle VideoToggle;
    public Toggle ImageToggle;

    public Button PreButton;
    public Button NextButton;

    public Button PlayAniButton;

    public Transform VideoScroll;
    public Transform ImageScroll;
    public Transform TextLabel;
    public GameObject BG;

    private TechnologyProcess TechnologyProcess;
    private ProcessStep CurrentStep;
    private int StepIndex = 0;
    void Start()
    {
        TechnologyProcess = GetComponentInChildren<TechnologyTemplate>().technologyProcesses[0];

        TextToggle.onValueChanged.AddListener(TextButtonClick);
        PreButton.onClick.AddListener(PreButtonClick);
        NextButton.onClick.AddListener(NextButtonClick);

        TextToggle.onValueChanged.AddListener(TextButtonClick);
        ImageToggle.onValueChanged.AddListener(ImageButtonClick);
        VideoToggle.onValueChanged.AddListener(VideoButtonClick);

        PlayAniButton.onClick.AddListener(PlayAniButtonClick);

        text.text = (StepIndex+1) + "/" + TechnologyProcess.processSteps.Count + " " + TechnologyProcess.processSteps[StepIndex].text;
        WorkAreaText.text = TechnologyProcess.processSteps[StepIndex].describe;
        for (int i = 0; i < TechnologyProcess.processSteps.Count; i++)
        {
            for (int j = 0; j < TechnologyProcess.processSteps[i].stepResources.Count; j++)
            {
                TechnologyProcess.processSteps[i].stepResources[j].Target.SetActive(false);
            }
        }
        InitStep(true);

        BG.SetActive(false);
    }

    private void TextButtonClick(bool isOn)
    {
        if (Group.AnyTogglesOn() && isOn)
        {
            BG.SetActive(true);
            VideoScroll.gameObject.SetActive(false);
            ImageScroll.gameObject.SetActive(false);
            TextLabel.gameObject.SetActive(true);
            WorkAreaText.text = TechnologyProcess.processSteps[StepIndex].describe;
        }
        else
        {
            BG.SetActive(false);
            TextLabel.gameObject.SetActive(false);
        }
    }

    private void ImageButtonClick(bool isOn)
    {
        if (Group.AnyTogglesOn() && isOn)
        {
            BG.SetActive(true);
            VideoScroll.gameObject.SetActive(false);
            ImageScroll.gameObject.SetActive(true);
            TextLabel.gameObject.SetActive(false);
        }
        else
        {
            BG.SetActive(false);
            ImageScroll.gameObject.SetActive(false);
        }
    }

    private void VideoButtonClick(bool isOn)
    {
        if (Group.AnyTogglesOn() && isOn)
        {
            BG.SetActive(true);
            VideoScroll.gameObject.SetActive(true);
            ImageScroll.gameObject.SetActive(false);
            TextLabel.gameObject.SetActive(false);
        }
        else
        {
            BG.SetActive(false);
            VideoScroll.gameObject.SetActive(false);
        }
    }

    private void InitStep(bool isNext)
    {
        ProcessStep OldStep = CurrentStep;
        
        CurrentStep = TechnologyProcess.processSteps[StepIndex];
        int textflag = 0;
        int imageFlag = 0;
        int videoFlag = 0;
        for (int i = 0; i < CurrentStep.stepResources.Count; i++)
        {
            CurrentStep.stepResources[i].Target.SetActive(true);
            if (TechnologyProcess.processSteps[StepIndex].describe != "")
                textflag++;
            if (CurrentStep.stepResources[i].Type == StepResource.ResourceType.IMAGE)
                imageFlag++;
            if (CurrentStep.stepResources[i].Type == StepResource.ResourceType.VIDEO)
                videoFlag++;
            if (StepIndex > 0)
            {
                if (isNext)
                {
                    if (CurrentStep.stepResources[i].Type != StepResource.ResourceType.MODEL)
                    {
                        OldStep.stepResources[i].Target.SetActive(false);
                    }
                }
                //else
                //{
                //    OldStep.stepResources[i].Target.SetActive(false);
                //}
            }
        }
        if (!isNext)
        {
            for (int i = 0; i < OldStep.stepResources.Count; i++)
            {
                OldStep.stepResources[i].Target.SetActive(false);
            }
        }
        if (textflag == 0)
            TextToggle.gameObject.transform.parent.gameObject.SetActive(false);
        if (imageFlag == 0)
            ImageToggle.gameObject.transform.parent.gameObject.SetActive(false);
        if (videoFlag == 0)
            VideoToggle.gameObject.transform.parent.gameObject.SetActive(false);
        TextToggle.isOn = false;
        ImageToggle.isOn = false;
        VideoToggle.isOn = false;
    }

    private void NextButtonClick()
    {
        StepIndex++;
        BG.SetActive(false);
        TextToggle.isOn = false;
        ImageToggle.isOn = false;
        VideoToggle.isOn = false;
        if (StepIndex == TechnologyProcess.processSteps.Count)
        {
            StepIndex--;
            UnityEngine.SceneManagement.SceneManager.LoadScene("CheckScene");
            return;
        }
        TextToggle.gameObject.transform.parent.gameObject.SetActive(true);
        ImageToggle.gameObject.transform.parent.gameObject.SetActive(true);
        VideoToggle.gameObject.transform.parent.gameObject.SetActive(true);
        text.text = (StepIndex + 1) + "/" + TechnologyProcess.processSteps.Count + " " + TechnologyProcess.processSteps[StepIndex].text;
        WorkAreaText.text = TechnologyProcess.processSteps[StepIndex].describe;
        InitStep(true);
    }

    private void PreButtonClick()
    {
        StepIndex--;
        BG.SetActive(false);
        TextToggle.isOn = false;
        ImageToggle.isOn = false;
        VideoToggle.isOn = false;
        if (StepIndex < 0)
        {
            StepIndex++;
            text.text = (StepIndex + 1) + "/" + TechnologyProcess.processSteps.Count + " " + TechnologyProcess.processSteps[StepIndex].text;
            WorkAreaText.text = TechnologyProcess.processSteps[StepIndex].describe;
            return;
        }
        TextToggle.gameObject.transform.parent.gameObject.SetActive(true);
        ImageToggle.gameObject.transform.parent.gameObject.SetActive(true);
        VideoToggle.gameObject.transform.parent.gameObject.SetActive(true);
        text.text = (StepIndex + 1) + "/" + TechnologyProcess.processSteps.Count + " " + TechnologyProcess.processSteps[StepIndex].text;
        WorkAreaText.text = TechnologyProcess.processSteps[StepIndex].describe;
        InitStep(false);
    }

    private void PlayAniButtonClick()
    {
        if (CurrentStep.StepObject != null && CurrentStep.StepObject.GetComponent<Animation>() != null)
        {
            CurrentStep.StepObject.GetComponent<Animation>().Play();
        }        
    }

}
