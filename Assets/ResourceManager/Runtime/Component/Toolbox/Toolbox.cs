using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Alva.Runtime.Toolbox
{
    /// <summary>
    /// 文件工具
    /// </summary>
    public static class FileTools
    {
        /// <summary>
        /// 获取文件Md5值
        /// </summary>
        /// <param name="fileName"> 文件路径 </param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string filePath)
        {
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                file.Dispose();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        /// <summary>
        /// 读取文件二进制数据
        /// </summary>
        /// <param name="filePath"> 文件路径 </param>
        /// <returns></returns>
        public static byte[] ReadFileByte(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    byte[] buffur = new byte[fs.Length];
                    fs.Read(buffur, 0, (int)fs.Length);
                    fs.Close();
                    return buffur;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
    public static class DirectoryTools
    {
        /// <summary>
        /// 清空当前文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void Clear(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            DirectoryInfo sDir = new DirectoryInfo(path);
            FileInfo[] fileArray = sDir.GetFiles();
            foreach (FileInfo file in fileArray)
            {
                file.Delete();
            }
            DirectoryInfo[] subDirArray = sDir.GetDirectories();
            foreach (DirectoryInfo subDir in subDirArray)
            {
                subDir.Delete(true);
            }
        }
    }
    public static class JsonTools
    {
        /// <summary>
        /// 反序列化Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_json"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string _json)
        {
            T json = LitJson.JsonMapper.ToObject<T>(_json);
            return json;
        }
        /// <summary>
        /// 序列化Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject(object obj)
        {
            string jsonData = LitJson.JsonMapper.ToJson(obj);
            return jsonData;
        }
    }
    public static class TimeTools
    {
        /// 格式化时间
        /// </summary>
        /// <param name="seconds">秒</param>
        /// <returns></returns>
        public static string FormatTime(float seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
            string str = "";

            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = "00:" + ts.Seconds.ToString("00");
            }

            return str;
        }

        /// <summary>
        /// DateTime --> long
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        [Obsolete]
        public static long ConvertDataTimeToLong(DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dt.Subtract(dtStart);
            long timeStamp = toNow.Ticks;
            timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
            return timeStamp;
        }

        /// <summary>
        /// long --> DateTime
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        [Obsolete]
        public static DateTime ConvertLongToDateTime(long d)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(d + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }
    }
    public static class ZipTools
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="_sourcePath"></param>
        /// <param name="_desPath"></param>
        public static string ZipFileTool(string _sourcePath, string version = null)
        {
            string startPath = _sourcePath;
            string zipPath = _sourcePath + version + ".zip";

            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(System.Text.Encoding.UTF8))
            {
                zip.UseZip64WhenSaving = Ionic.Zip.Zip64Option.AsNecessary;
                zip.StatusMessageTextWriter = Console.Out;
                zip.AddDirectory(startPath); // recurses subdirectories
                zip.Save(zipPath);
                zip.Dispose();
            }
            return zipPath;
        }
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="_zipPath">路径</param>
        /// <param name="_extractPath">提取路径</param>
        public static void ReZipFileTool(string _zipPath, string _extractPath, bool IsDestroy)
        {
            Debug.Log("解压路径：" + _zipPath);
            if (!Directory.Exists(_extractPath))
            {
                Directory.CreateDirectory(_extractPath);
            }
            else
            {
                Debug.Log("特征目录已存在：" + _extractPath);
            }
            Ionic.Zip.ReadOptions options = new Ionic.Zip.ReadOptions();
            options.Encoding = System.Text.Encoding.UTF8;
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(_zipPath, options))
            {
                foreach (Ionic.Zip.ZipEntry entry in zip)
                {
                    //Extract 解压 zip 文件包的方法，参数是保存解压后文件的路基  
                    entry.Extract(_extractPath);
                }
                zip.Dispose();
            }
            if (IsDestroy)
            {
                File.Delete(_zipPath);
            }
        }
    }
    public static class Texture2DTools
    {
        /// <summary>
        /// 加载图片纹理数据
        /// </summary>
        /// <param name="filePath"> 文件路径 </param>
        /// <returns></returns>
        public static Texture LoadTexture(byte[] fileBytes)
        {
            Texture2D texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(fileBytes);
            return texture2D;
        }
        /// <summary>
        /// 纹理转精灵图片
        /// </summary>
        /// <param name="texture"> 纹理数据 </param>
        /// <returns></returns>
        public static Sprite Texture2DToSprite(Texture2D texture)
        {
            Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            return sp;
        }

        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }

            result.Apply();
            return result;
        }

    }

    public static class MeshTools
    {
        /// <summary>
        /// 添加碰撞器
        /// </summary>
        /// <param name="model"></param>
        private static void AddCollider(GameObject model)
        {
            //添加网格碰撞器
            SkinnedMeshRenderer smr = model.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr)
            {
                smr.gameObject.AddComponent<MeshCollider>().sharedMesh = smr.sharedMesh;
                if (smr.material.color.a != 1)
                {
                    SetMaterialRenderingMode(smr.material, RenderingMode.Transparent);
                }
            }
            else
            {
                MeshRenderer[] mr = model.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer item in mr)
                {
                    if (item.material.color.a != 1)
                    {
                        SetMaterialRenderingMode(item.material, RenderingMode.Transparent);
                    }
                    item.gameObject.AddComponent<MeshCollider>().sharedMesh = item.GetComponent<MeshFilter>().mesh;
                }
            }
        }
        /// <summary>
        /// 修改材质渲染模式
        /// </summary>
        /// <param name="material"></param>
        /// <param name="renderingMode"></param>
        public static void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }
        /// <summary>
        /// 顶点界限计算
        /// </summary>
        /// <param name="model"></param>
        /// <param name="minBoundsSize"></param>
        /// <returns></returns>
        public static Bounds CalculateBounds(GameObject model, float minBoundsSize = 0.1f)
        {
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            Vector3 scale = model.transform.localScale;
            model.transform.localScale = Vector3.one;

            if (renderers.Length == 0)
            {
                return new Bounds(model.transform.position, Vector2.one * minBoundsSize);
            }
            Bounds bounds = renderers[0].bounds;
            foreach (Renderer r in renderers)
            {
                bounds.Encapsulate(r.bounds);
            }

            model.transform.localScale = scale;
            return bounds;
        }
        /// <summary>
        /// 重置着色器
        /// </summary>
        /// <param name="obj"></param>
        public static void ResetShader(UnityEngine.Object obj)
        {
            System.Collections.Generic.List<Material> listMat = new System.Collections.Generic.List<Material>();
            listMat.Clear();
            if (obj is Material)
            {
                Material m = obj as Material;
                listMat.Add(m);
            }
            else if (obj is GameObject)
            {
                GameObject go = obj as GameObject;
                Renderer[] rends = go.GetComponentsInChildren<Renderer>(true);
                if (null != rends)
                {
                    foreach (Renderer item in rends)
                    {
                        Material[] materialsArr = item.sharedMaterials;
                        foreach (Material m in materialsArr)
                            listMat.Add(m);
                    }
                }
            }
            for (int i = 0; i < listMat.Count; i++)
            {
                Material m = listMat[i];
                if (null == m)
                    continue;
                var shaderName = m.shader.name;
                var newShader = Shader.Find(shaderName);
                if (newShader != null)
                    m.shader = newShader;
            }
        }
    }

    /// <summary>
    /// 材质球渲染模式
    /// </summary>
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    public class ToolsHelper : MonoBehaviour
    {
        public static string FormatTime(float seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
            string str = "";

            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = "00:" + ts.Seconds.ToString("00");
            }

            return str;
        }
    }
    public class TimerTools
    {
        static Action callback;
        static int index;
        static int total;
        public static void TimerInvoke(float delayTime, Action _callback)
        {
#if UNITY_EDITOR
            index = 0;
            total = (int)(100 * delayTime);
            UnityEditor.EditorApplication.update += UpdateEvent;
            callback = _callback;
#endif
        }
        static void UpdateEvent()
        {
#if UNITY_EDITOR
            ++index;
            if (index >= total)
            {
                callback?.Invoke();
                callback = null;
                UnityEditor.EditorApplication.update -= UpdateEvent;
            }
#endif
        }
        public static void CancleTimerInvoke()
        {
#if UNITY_EDITOR
            callback = null;
            UnityEditor.EditorApplication.update -= UpdateEvent;
#endif
        }
    }

    public class ImageShowTool
    {
        static GameObject imageMask;
        static GameObject imageShow;
        public static void ImageShow(GameObject obj)
        {
            imageMask = obj.transform.GetChild(0).GetComponentInChildren<Image>().gameObject;
            imageShow = imageMask.transform.GetChild(0).GetComponentInChildren<Image>().gameObject;
            imageShow.GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;
            obj.GetComponent<Image>().enabled = false;
            if (obj.GetComponent<Alva.Runtime.Components.ImageAgent>().ImageShowType == Components.ImageShowType.Simple)
            {
                ShowImageBySimple(obj);
            }
            if (obj.GetComponent<Alva.Runtime.Components.ImageAgent>().ImageShowType == Components.ImageShowType.LongType)
            {
                ShowImageByLong(obj);
            }
            if (obj.GetComponent<Alva.Runtime.Components.ImageAgent>().ImageShowType == Components.ImageShowType.ShortType)
            {
                ShowImageByShort(obj);
            }
        }

        public static void ShowImageBySimple(GameObject obj)
        {
            obj.GetComponent<Image>().enabled = false;
            imageMask.GetComponent<Mask>().enabled = false;
            imageMask.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            imageShow.GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;

            float spriteHeight = obj.GetComponent<RectTransform>().rect.height;
            float spriteWidth = obj.GetComponent<RectTransform>().rect.width;
            float width = obj.GetComponent<Image>().sprite.texture.width;
            float height = obj.GetComponent<Image>().sprite.texture.height;
            imageShow.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteWidth, spriteHeight);
        }

        public static void ShowImageByLong(GameObject obj)
        {
            float spriteHeight = obj.GetComponent<RectTransform>().sizeDelta.y;
            float spriteWidth = obj.GetComponent<RectTransform>().sizeDelta.x;
            float width = obj.GetComponent<Image>().sprite.texture.width;
            float height = obj.GetComponent<Image>().sprite.texture.height;
            float radio = width / height;
            imageMask.GetComponent<Mask>().enabled = false;
            imageMask.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            if (width < height)
            {
                imageMask.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteWidth, spriteHeight);
                imageShow.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteHeight * radio, spriteHeight);
            }
            else
            {
                imageMask.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteWidth, spriteHeight);
                imageShow.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteWidth, spriteWidth / radio);
            }
        }

        public static void ShowImageByShort(GameObject obj)
        {
            float spriteHeight = obj.GetComponent<RectTransform>().rect.height;
            float spriteWidth = obj.GetComponent<RectTransform>().rect.width;
            float width = obj.GetComponent<Image>().sprite.texture.width;
            float height = obj.GetComponent<Image>().sprite.texture.height;
            float radio1 = width / height;
           
            imageMask.GetComponent<Mask>().enabled = true;
            imageMask.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            if (width < height)
            {
                imageMask.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteWidth, spriteHeight);
                imageShow.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteWidth, spriteHeight * radio1);
            }
            else
            {
                imageMask.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteWidth, spriteHeight);
                imageShow.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteWidth * radio1, spriteHeight);
            }
        }
    }
}