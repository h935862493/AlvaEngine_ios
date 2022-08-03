using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.EazyPlan
{
    [System.Serializable]
    public class WorkProceduresItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string id;
        /// <summary>
        /// 
        /// </summary>
        public int sequenceNumber;
        /// <summary>
        /// 
        /// </summary>
        public string text;
        /// <summary>
        /// 
        /// </summary>
        public string description;
        /// <summary>
        /// 
        /// </summary>
        public List<WorkResourcesItem> workResources;
    }
    [System.Serializable]
    public class WorkResourcesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string id;
        /// <summary>
        /// 
        /// </summary>
        public int wrType;
        /// <summary>
        /// 
        /// </summary>
        public string localPath;
        /// <summary>
        /// 
        /// </summary>
        public int sequenceNumber;
    }
    [System.Serializable]
    public class WorkProcedureRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public string id;
        /// <summary>
        /// 
        /// </summary>
        public List<WorkProceduresItem> workProcedures;
    }
    public class WorkProcedureResources
    {
        public int totalSteps;
        public List<WorkProceduresItem> workProcedures;
        public Dictionary<string, List<string>> modelNameStepIDMap;
    }


    public class OperationProcedureConfigDecoder : MonoBehaviour
    {
        public static OperationProcedureConfigDecoder Instance;
        public TextAsset configJsonAsset;
        [SerializeField]
        public WorkProcedureRoot workProcedureRoot;
        public string resourceFolderName = "PLMXML";
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
            DecoderocedureConfig();
        }
        private void Start()
        {

        }
        void DecoderocedureConfig()
        {
            if (configJsonAsset)
            {
                workProcedureRoot = JsonUtility.FromJson<WorkProcedureRoot>(configJsonAsset.text);
               // Debug.LogError("workProcedureRoot--" + workProcedureRoot.id);
            }
        }
        public WorkProcedureResources GetWorkProcedureResources()
        {
            WorkProcedureResources workProcedureResources = new WorkProcedureResources();
            workProcedureResources.workProcedures = new List<WorkProceduresItem>();
            workProcedureResources.modelNameStepIDMap = new Dictionary<string,List< string>>();//多步骤用同一模型问题 模型名字 步骤ID列表
            //if (workProcedureRoot == null)
            //{
                DecoderocedureConfig();
            //}
            if (workProcedureRoot == null)
            {
                return null;
            }
            else
            {
                if (workProcedureRoot.workProcedures == null)
                {
                    return null;
                }
                for (int i = 0; i < workProcedureRoot.workProcedures.Count; i++)
                {
                    if (workProcedureRoot.workProcedures[i].workResources != null && workProcedureRoot.workProcedures[i].workResources.Count > 0)
                    {
                        workProcedureResources.workProcedures.Add(workProcedureRoot.workProcedures[i]);
                        for (int j = 0; j < workProcedureRoot.workProcedures[i].workResources.Count; j++)
                        {
                            if (workProcedureRoot.workProcedures[i].workResources[j].wrType == 1)
                            {
                                string modelURL = workProcedureRoot.workProcedures[i].workResources[j].localPath.Trim().Replace("\\", "/").Replace("%20", " ").Split('.')[0];
                                string[] modelStrs = modelURL.Split('/');
                                string modelName = modelStrs[modelStrs.Length - 1];
                                if (!workProcedureResources.modelNameStepIDMap.ContainsKey(modelName))
                                {
                                    List<string> newModelStepList = new List<string>();
                                    newModelStepList.Add(workProcedureRoot.workProcedures[i].id);
                                    workProcedureResources.modelNameStepIDMap.Add(modelName, newModelStepList);
                                }
                                else
                                {
                                    workProcedureResources.modelNameStepIDMap[modelName].Add(workProcedureRoot.workProcedures[i].id);
                                }                             
                            }
                        }
                    }
                }
                workProcedureResources.totalSteps = workProcedureResources.workProcedures.Count;
              //  Debug.LogError("workProcedureResources.totalSteps--" + workProcedureResources.totalSteps);
            }
            return workProcedureResources;
        }
        // Update is called once per frame
        void Update()
        {

        }
        public List<GameObject> GetAllNeedToCleanedUpModels()
        {
            List<GameObject> models = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                models.Add(transform.GetChild(i).gameObject);
            }
            return models;
        }


    }
}
