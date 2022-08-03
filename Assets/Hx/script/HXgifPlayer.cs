using Gif2Textures;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HXgifPlayer : MonoBehaviour
{
    private bool m_bIsPlayer = false;//是否正在播放

    private Material mat;
    private RawImage raw;
    private float count;
    public string gifName;
    /// <summary>
    /// 播放状态
    /// </summary>
    public enum PLAYSTATE
    {
        ONCE = 0,
        LOOP
    }

    private void Start()
    {
        Gif2Textures.GifFrames mframes = LoadGif(Application.streamingAssetsPath + "/motoGIF/" + gifName);
        Material mat = GetComponentInChildren<MeshRenderer>().material;
       
        GetComponentInChildren<HXgifPlayer>().isPlayOnAwake = true;
        
        GetComponentInChildren<HXgifPlayer>().PlayGif(0, 50f, mframes,mat);
        gameObject.SetActive(false);
    }

    public bool isPlayOnAwake = false;

    public void PlayGif()
    {
        if (mat != null)
        {
            StartCoroutine(PlayUpdateMat(count, mat, m_state));
        }
        if (raw != null)
        {
            StartCoroutine(PlayUpdateRawImage(count, raw, m_state));
        }
    }

    private void OnEnable()
    {
        if (isPlayOnAwake)
        {
            PlayGif();
        }
    }

    private PLAYSTATE m_state = PLAYSTATE.ONCE;
    private int FrameIndex = 0;
    private GifFrames m_GifFrames = null;

    /// <summary>
    /// 加载Gif文件
    /// </summary>
    /// <param name="m_GifFilePath">文件地址</param>
    /// <param name="m_cacheTextures">是否缓存在播放</param>
    /// <returns>Gif帧文件</returns>
    public GifFrames LoadGif(string m_GifFilePath, bool m_cacheTextures = false)
    {
        byte[] ta = File.ReadAllBytes(m_GifFilePath);
        //获取
        MemoryStream ms = new MemoryStream(ta);

        GifFrames m_GifFrames = new GifFrames();
        if (!m_GifFrames.Load(ms, m_cacheTextures))
            m_GifFrames = null;
        return m_GifFrames;
    }
    /// <summary>
    /// 播放Gif
    /// </summary>
    /// <param name="FrameIndex">从哪帧开始</param>
    /// <param name="m_frameCount">播放速度帧数</param>
    /// <param name="m_GifFrames">Gif帧文件</param>
    /// <param name="m_RawImage">播放Gif的组件</param>
    /// <param name="m_state">循环模式</param>
    public void PlayGif(int FrameIndex, float m_frameCount, GifFrames m_GifFrames, RawImage m_RawImage = null, PLAYSTATE m_state = PLAYSTATE.ONCE)
    {
        m_bIsPlayer = true;//记录是否在播放
        this.m_state = m_state;
        this.FrameIndex = FrameIndex;
        this.m_GifFrames = m_GifFrames;
        this.raw = m_RawImage;
        this.count = m_frameCount;
        StartCoroutine(PlayUpdateRawImage(m_frameCount, m_RawImage, m_state));
    }

    /// <summary>
    /// 播放Gif
    /// </summary>
    /// <param name="FrameIndex">从哪帧开始</param>
    /// <param name="m_frameCount">播放速度帧数</param>
    /// <param name="m_GifFrames">Gif帧文件</param>
    /// <param name="m_Mat">播放Gif的组件</param>
    /// <param name="m_state">循环模式</param>
    public void PlayGif(int FrameIndex, float m_frameCount, GifFrames m_GifFrames, Material m_Mat, PLAYSTATE m_state = PLAYSTATE.ONCE)
    {
        m_bIsPlayer = true;//记录是否在播放
        this.m_state = m_state;
        this.FrameIndex = FrameIndex;
        this.m_GifFrames = m_GifFrames;
        this.mat = m_Mat;
        this.count = m_frameCount;
        //StartCoroutine(PlayUpdateMat(m_frameCount, m_Mat, m_state));
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    public void Pause()
    {
        m_bIsPlayer = false;//记录是否在播放
        StopAllCoroutines();//停止协程
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public void Stop()
    {
        m_bIsPlayer = false;//记录是否在播放
        FrameIndex = 0;//返回为第一帧
        m_GifFrames.Restart();
        StopAllCoroutines();//停止协程
    }

    /// <summary>
    /// 是否正在播放
    /// </summary>
    public bool IsPlayer()
    {
        return m_bIsPlayer;
    }

    public int GetCurrentFrameIndex()
    {
        return FrameIndex;
    }
    IEnumerator PlayUpdateRawImage(float m_frameCount, RawImage m_RawImage, PLAYSTATE m_state = PLAYSTATE.ONCE)
    {
        //总帧数
        int FrameCount = m_GifFrames.GetFrameCount();
        //获取每一张图片的持续时间
        float m_frameSecond = 1.0f / m_frameCount;
        //死循环运算
        while (true)
        {
            #region 替换为下一张
            ++FrameIndex;//索引+ addCount  相当于+1  如果是pingpong 则可能addCount == -1
            //如果索引超出范围
            if (FrameIndex >= FrameCount || FrameIndex < 0)
            {
                //不同状态有不同的处理方式
                if (m_state == PLAYSTATE.ONCE)
                {
                    //Stop();
                    break;
                }
                else if (m_state == PLAYSTATE.LOOP)
                {
                    FrameIndex = 0;//如果是循环则从0开始
                }
            }
            ChangeImage(m_GifFrames, m_RawImage);//替换图片
            #endregion
            yield return new WaitForSeconds(m_frameSecond);//暂停协程,开始其他运算
        }
        yield return null;
    }

    IEnumerator PlayUpdateMat(float m_frameCount, Material m_Mat, PLAYSTATE m_state = PLAYSTATE.ONCE)
    {
        FrameIndex = 0;
        //总帧数
        int FrameCount = m_GifFrames.GetFrameCount();
        //获取每一张图片的持续时间
        float m_frameSecond = 1.0f / m_frameCount;
        //死循环运算
        while (true)
        {
            #region 替换为下一张
            ++FrameIndex;//索引+ addCount  相当于+1  如果是pingpong 则可能addCount == -1
            //如果索引超出范围
            if (FrameIndex >= FrameCount || FrameIndex < 0)
            {
                //不同状态有不同的处理方式
                if (m_state == PLAYSTATE.ONCE)
                {
                    //Stop();
                    break;
                }
                else if (m_state == PLAYSTATE.LOOP)
                {
                    FrameIndex = 0;//如果是循环则从0开始
                }
            }
            ChangeImage(m_GifFrames, null, m_Mat);//替换图片
            #endregion
            yield return new WaitForSeconds(m_frameSecond);//暂停协程,开始其他运算
        }
        yield return null;
    }

    /// <summary>
    /// 切换图片
    /// </summary>
    void ChangeImage(GifFrames m_GifFrames, RawImage m_RawImage = null, Material m_Mat = null)
    {
        Texture2D texTemp;
        float delay;
        m_GifFrames.GetNextFrame(out texTemp, out delay);//从gif管理器获取纹理
        //RawImage 或者 模型贴图
        if (m_RawImage != null)
        {
            m_RawImage.texture = texTemp;
        }
        else
        {
            m_Mat.mainTexture = texTemp;
        }

    }
}
