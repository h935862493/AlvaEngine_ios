using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Alva.ITweens;

namespace Alva.EazyPlan
{
    public class OperationProcedurePanel : MonoBehaviour
    {
        public static OperationProcedurePanel Instance;
        [Header("运行开始是否显示工艺流程")]
        public bool InitShowOperationProcedure = true;
        public Transform bg;
        public Button previousButton;
        public Button nextButton;
        public Toggle modelButton;
        public Toggle imageButton;
        public Toggle videoButton;
        WorkProcedureResources workProcedureResources;
        int currentStepIndex = 0;
        int totalSteps = 0;
        public Transform InfoPanelBG;
        public Button downOrUpButton;
        bool isDown = true; //向下箭头按钮状态，此时展开
        public List<OperationStepModels> operationStepModelsList = new List<OperationStepModels>();
        OperationStepModels currentOperationStepModels;
        float  downInfoPanelBGPosY;
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
            previousButton.onClick.AddListener(OnClickPreviousButton);
            nextButton.onClick.AddListener(OnClickNextButton);
            modelButton.onValueChanged.AddListener(ClickModelButton);
            imageButton.onValueChanged.AddListener(ClickImageButton);
            videoButton.onValueChanged.AddListener(ClickVideoButton);
            if (downOrUpButton)
            {
                downOrUpButton.onClick.AddListener(OnClickDownOrUpButton);
            }
        
        }
 
        private void OnClickDownOrUpButton()
        {
            if (downOrUpButton)
            {
                if (isDown)
                {
                    isDown = false;
                    downOrUpButton.transform.localEulerAngles = new Vector3(0, 0, -90);
                    Hashtable args = new Hashtable();
                    args.Add("easeType", iTween.EaseType.easeInOutExpo);
                    args.Add("y", downInfoPanelBGPosY-616);
                    args.Add("time", 0.3f);
                    args.Add("islocal", true);
                    if (InfoPanelBG)
                    {
                        iTween.MoveTo(InfoPanelBG.gameObject, args);
                    }
                }
                else
                {
                    isDown = true;
                    downOrUpButton.transform.localEulerAngles = new Vector3(0, 0, 90);
                    Hashtable args = new Hashtable();
                    args.Add("easeType", iTween.EaseType.easeInOutExpo);
                    args.Add("y", downInfoPanelBGPosY);
                    args.Add("time", 0.3f);
                    args.Add("islocal", true);
                    if (InfoPanelBG)
                    {
                        iTween.MoveTo(InfoPanelBG.gameObject, args);
                    }
                }

            }
        }

        private void Start()
        {
            GetOperationStepModelsList();
            HideAllOperationStepModels();
            Hide();
            if (InitShowOperationProcedure)
            {
                Show();
            }
            if (InfoPanelBG)
            {
                downInfoPanelBGPosY = InfoPanelBG.transform.localPosition.y;
            }
        }

        private void HideAllOperationStepModels()
        {
            if (operationStepModelsList.Count > 0)
            {
                for (int i = 0; i < operationStepModelsList.Count; i++)
                {
                    if (operationStepModelsList[i])
                    {
                        operationStepModelsList[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        private void ShowOperationStepModels(string _id)
        {
            if (operationStepModelsList.Count > 0)
            {
                for (int i = 0; i < operationStepModelsList.Count; i++)
                {
                    if (operationStepModelsList[i] && operationStepModelsList[i].id == _id)
                    {
                        operationStepModelsList[i].gameObject.SetActive(true);
                        currentOperationStepModels = operationStepModelsList[i];
                    }
                    else
                    {
                        operationStepModelsList[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        private List<OperationStepModels> GetOperationStepModelsList()
        {
            OperationStepModels[] stepModelsArray = GameObject.FindObjectsOfType<OperationStepModels>(true);
            operationStepModelsList = new List<OperationStepModels>();//过滤模型，图片，视频全不存在则为空步骤不显示
            for (int i = 0; i < stepModelsArray.Length; i++)
            {
                OperationStepModels operationStepModels = stepModelsArray[i];
                if (stepModelsArray[i])
                {
                    StepSingleModel[] models = stepModelsArray[i].transform.GetComponentsInChildren<StepSingleModel>(true);
                    if (models != null && models.Length > 0)
                    {
                        operationStepModelsList.Add(stepModelsArray[i]);
                        continue;
                    }
                    if (stepModelsArray[i].stepImages.Count > 0 || stepModelsArray[i].stepVideos.Count > 0)
                    {
                        operationStepModelsList.Add(stepModelsArray[i]);
                        continue;
                    }
                }
            }
            //if (stepModelsArray!=null)
            //{
            //    operationStepModelsList = new List<OperationStepModels>(stepModelsArray);
            //    operationStepModelsList= operationStepModelsList.OrderBy(x=>x.stepOrder).ToList();
            //}         
            operationStepModelsList = operationStepModelsList.OrderBy(x => x.stepOrder).ToList();
            return operationStepModelsList;
        }

        public void Show()
        {
            if (operationStepModelsList.Count < 1)
            {
                return;
            }
            if (bg)
            {
                bg.gameObject.SetActive(true);
            }
            //workProcedureResources = OperationProcedureConfigDecoder.Instance.GetWorkProcedureResources();
            //totalSteps = workProcedureResources.totalSteps;
            totalSteps = operationStepModelsList.Count;
            currentStepIndex = 0;
            // ShowOperationStepModels(workProcedureResources.workProcedures[currentStepIndex].id);
            ShowOperationStepModels(operationStepModelsList[currentStepIndex].id);
            PlayOperationStepModelsAnimation();
            ResetShowModelInfo();
        }

        public void PlayOperationStepModelsAnimation()
        {
            if (currentOperationStepModels != null)
            {
                currentOperationStepModels.PlayAnimation();
            }
        }

        private void OnClickNextButton()
        {
            if (currentStepIndex >= totalSteps - 1)
            {
                return;
            }
            currentStepIndex++;
            // ShowOperationStepModels(workProcedureResources.workProcedures[currentStepIndex].id);
            ShowOperationStepModels(operationStepModelsList[currentStepIndex].id);
            PlayOperationStepModelsAnimation();
            ResetShowModelInfo();
        }

        private void OnClickPreviousButton()
        {
            if (currentStepIndex <= 0)
            {
                return;
            }
            currentStepIndex--;
            // ShowOperationStepModels(workProcedureResources.workProcedures[currentStepIndex].id);
            ShowOperationStepModels(operationStepModelsList[currentStepIndex].id);
            PlayOperationStepModelsAnimation();
            ResetShowModelInfo();
        }
        void ResetShowModelInfo()
        {
            if (modelButton)
            {
                modelButton.SetIsOnWithoutNotify(true);
                ClickModelButton(true);
            }
        }
        void ClickModelButton(bool isFlag)
        {
            if (isFlag)
            {
                // ModelnfoPanel.Instance.Show(workProcedureResources.workProcedures[currentStepIndex]);
                ModelnfoPanel.Instance.Show(operationStepModelsList[currentStepIndex]);
                ProcedurePicturesPanel.Instance.Hide();
                ProcedureVideosPanel.Instance.Hide();
            }
        }
        void ClickImageButton(bool isFlag)
        {
            if (isFlag)
            {
                ProcedurePicturesPanel.Instance.Show(operationStepModelsList[currentStepIndex]);
                ModelnfoPanel.Instance.Hide();
                ProcedureVideosPanel.Instance.Hide();
            }

        }
        void ClickVideoButton(bool isFlag)
        {
            if (isFlag)
            {
                //ProcedureVideosPanel.Instance.Show(workProcedureResources.workProcedures[currentStepIndex]);
                ProcedureVideosPanel.Instance.Show(operationStepModelsList[currentStepIndex]);
                ModelnfoPanel.Instance.Hide();
                ProcedurePicturesPanel.Instance.Hide();
            }

        }

        public void Hide()
        {
            if (bg)
            {
                bg.gameObject.SetActive(false);
            }
        }
        // Update is called once per frame
        void Update()
        {
            //测试显示
            if (Input.GetKeyUp(KeyCode.A))
            {
                Show();
            }
        }


    }
}