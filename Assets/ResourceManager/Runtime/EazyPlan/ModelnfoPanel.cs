using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alva.EazyPlan
{
    public class ModelnfoPanel : MonoBehaviour
    {
        public static ModelnfoPanel Instance;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Button playAnimationButton;
        public Transform bg;
        WorkProceduresItem workProceduresItem;
        OperationStepModels operationStepModels;
        public string id;
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;         
        }
        private void Start()
        {
            playAnimationButton.onClick.AddListener(OnClickPlayAnimation);
        }
        private void OnClickPlayAnimation()
        {
            OperationProcedurePanel.Instance.PlayOperationStepModelsAnimation();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void Show(WorkProceduresItem _workProceduresItem )
        {
            if (bg)
            {
                bg.gameObject.SetActive(true);
            }
            workProceduresItem = _workProceduresItem;
            if (workProceduresItem!=null)
            {
                titleText.text = workProceduresItem.text;
                descriptionText.text = workProceduresItem.description;
                id = workProceduresItem.id;
            }
        }
        public void Show(OperationStepModels _operationStepModels)
        {
            if (bg)
            {
                bg.gameObject.SetActive(true);
            }
            operationStepModels = _operationStepModels;
            if (operationStepModels != null)
            {
                titleText.text = operationStepModels.title;
                descriptionText.text = operationStepModels.description;
                id = operationStepModels.id;
            }
        }
        public void Hide()
        {
            if (bg)
            {
                bg.gameObject.SetActive(false);
            }
        }
        public void SetTitle(string title)
        {
            titleText.text = title;
        }
        public void SetDescription(string description)
        {
            descriptionText.text = description;
        }
        public void SetTitleAndeDescription(string title, string description)
        {
            titleText.text = title;
            descriptionText.text = description;
        }
    }
}
