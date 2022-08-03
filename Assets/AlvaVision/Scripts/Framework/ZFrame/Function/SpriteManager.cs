using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;

    private void Awake()
    {
        Instance = this;

    }

    public Sprite 登录点击;
    public Sprite 登录未点击;
    public Sprite 下载点击;
    public Sprite 下载未点击;
    public Sprite 展厅点击;
    public Sprite 展厅未点击;
    public Sprite 云项目点击;
    public Sprite 云项目未点击;

    public Sprite 置顶;
    public Sprite 未置顶;

    public Sprite 二维识别;
    public Sprite 模型识别;
    public Sprite 平面识别;
    public Sprite 空识别描述;

    public Sprite 已下载项目;
    public Sprite 需要更新项目;

    public Sprite 展开Table;
    public Sprite 折叠Table;

    public Sprite 二维Table;
    public Sprite 平面Table;
    public Sprite 三维Table;

}
