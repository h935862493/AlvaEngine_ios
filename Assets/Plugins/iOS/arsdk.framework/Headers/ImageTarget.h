#ifndef _IMAGETARGET_H_
#define _IMAGETARGET_H_

#include "DynamicExports.h"
#include "Format.h"

/**
 * 识别的模式选择
 */
typedef enum _SearchMode_ {
    SM_Full  = 0,
    SM_Half  = 1,
    SM_Click = 2
}SearchMode;

#ifdef __cplusplus
extern "C" {
#endif

    /**
     * @brief ImageTarget
     *        基本调用流程1如下：
     *        初始化
     *            IR_init
     *            IR_setDataInfoMemory
     *            IR_setCameraParamInfo
     *            [IR_EnableShadowEnhance], 可选
     *            [IR_SetSearchMode]      , 可选
     *            [IR_EnableServer]       , 可选
     *            [IR_EnableTrack]        , 可选
     *            initTrackerManager
     *            addTrackable
     *        激活模板
     *            getTrackableNumber
     *            getTrackableByIndex
     *            setTrackableStatus
     *        加数据
     *            [CleanFrameData]         , 可选
     *            IR_Memory / IR_MemoryChannel
     *        拿结果
     *            IR_GetBackGroundData / IR_GetBackGroundDataChannel
     *          拿跟踪结果
     *            getFoundNumIndexs
     *            getTrackerPoseMatrix
     *            getTrackVirtualMarkSize
     *            IR_computeProjectMatrixFixRange
     *          拿识别结果（和帧是异步关系）
     *            两种方式 回调函数和获取结果指针的值
     *                回调函数：OnSearchDone   setOnSearchDone   getSearchH
     *                获取结果指针的值： foundTracker getSearchH
     *        反初始化
     *            IR_unit
     *
     * @note 函数具体功能见说明
     */

    /**
     * @brief 初始化不因图像数据变化而变化的功能
     * @param dataType    后续输入数据的类型，见Format.h中alvadt，目前仅支持Alva_Memory
     * @param companyName 授权的公司名
     * @param authString  授权码
     * @return 0: 成功，其他：失败
     *
     * @note 在Android平台，调用此函数时需要具有android.permission.READ_PHONE_STATE权限
     *
     * @note 部分错误码:
     *            0x80011101:
     *            0x80011101: Authorize code is not correct.
     *            0x80031101: Authorize code is not for @companyName.
     *            0x80041101: Authorize code is not for current package.
     *            0x80051101:
     *            0x80061101: Authorize code is not for this machine.
     *            0x80071101: System time is not correct.
     *            0x80081101: Trying period is over.
     *            0x80091101: Authorize code is not for this app.
     *            0x800A1101: Authorize code is not for this app's module.
     *
     * Android:(以下为权限错误)
     *            0x8EFF1101: 无android.permission.READ_PHONE_STATE权限
     *
     * Android:(以下错误码为获取IMEI错误)
     *            0x8E011101:
     *            0x8E021101:
     *            ...
     *            0x8E1F1101:
     * Windows:(以下错误码为获取MAC错误)
     *            0x80E11101:
     *            0x80E21101:
     *            0x80E31101:
     * Linux:(以下错误码为获取MAC错误)
     *            0x80E11101:
     *            0x80E21101:
     *
     * @note 在Linux平台上请确保授权用的MAC地址对应的网卡名为"eth0"
     */
    ARSDK_API int IR_init(int dataType, char* companyName, char* authString);

    /**
     * @brief 设置图像信息
     * @param wid 图像的宽度
     * @param hei 图像的高度
     * @param fmt 图像格式, 见@alvafmt, 目前支持Alva_FMT_Y,Alva_FMT_GRAY,Alva_FMT_YUV_420p,Alva_FMT_NV21,Alva_FMT_NV12
     *                                      SM_Click模式仅支持Alva_FMT_YUV_420p
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int IR_setDataInfoMemory(int wid, int hei, int fmt);

    /**
     * @brief 设置相机信息
     * @param fx            焦距x
     * @param fy            焦距y
     * @param cx            投影中心x
     * @param cy            投影中心y
     * @param screenW       屏幕宽度
     * @param screenH       屏幕高度
     * @param rotation      横竖屏，见@alvaro
     * @return 0: 成功, 其他: 失败
     * @note 此函数必须在IR_setDataInfoMemory之后
     */
    ARSDK_API int IR_setCameraParamInfo(float fx, float fy, float cx, float cy, int screenW, int screenH, int rotation);

    /**
     * @brief 计算固定裁剪范围的投影矩阵
     * @param wid                视图宽
     * @param hei                视图高
     * @param nearV              近裁剪面
     * @param farV               远裁剪面
     * @param projectMatrix_CMO  输出，投影矩阵，16 * sizeof(float)大小空间, 列主序
     * @return 0: 成功, 其他: 失败
     *
     * @note 如果@wid > @hei, 认为显示为横屏
     * @note 如果@wid < @hei, 认为显示为竖屏
     */
    ARSDK_API int IR_computeProjectMatrixFixRange(int wid, int hei, float nearV, float farV, float* projectMatrix_CMO);

    /**
     * @brief 为识别核心更新一帧图像及数据
     * @param data          输入数据指针
     * @param coord         点击坐标, 格式：coordx coordy
     *                      仅在mode==2(“点击区域”模式)识别时使用
     * @param dataMatrix    摄像头变换矩阵, 格式如下:
     *                          m11 m12 m13 m14
     *                          m21 m22 m23 m24
     *                          m31 m32 m33 m34
     *                          m41 m42 m43 m44
     * @param viewMatrix    摄像头的视角矩阵(暂时未启用), 格式如下:
     *                          m11 m12 m13 m14
     *                          m21 m22 m23 m24
     *                          m31 m32 m33 m34
     *                          m41 m42 m43 m44
     *
     * @note dataMatrix可参考Android平台的数据，根据各自平台调整：
     *      0度（竖屏）：
     *             0.0   -1.0   0.0   0.0
     *            -1.0    0.0   0.0   0.0
     *             0.0    0.0   1.0   0.0
     *             0.0    0.0   0.0   1.0
     *      90度（左横屏）：
     *             1.0    0.0   0.0   0.0
     *             0.0   -1.0   0.0   0.0
     *             0.0    0.0   1.0   0.0
     *             0.0    0.0   0.0   1.0
     *      180度（倒竖屏）：
     *             0.0   -1.0   0.0   0.0
     *            -1.0    0.0   0.0   0.0
     *             0.0    0.0   1.0   0.0
     *             0.0    0.0   0.0   1.0
     *      270度（右横屏）：
     *            -1.0    0.0   0.0   0.0
     *             0.0    1.0   0.0   0.0
     *             0.0    0.0   1.0   0.0
     *             0.0    0.0   0.0   1.0
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int IR_Memory(unsigned char* data, float* coord, float* dataMatrix, float* viewMatrix);
    ARSDK_API int IR_MemoryChannel(unsigned char** data, float* coord, float* dataMatrix, float* viewMatrix);

    /**
     * @brief 获取一帧将用于渲染的图像数据
     * @param data 输出图像缓冲区指针, 内存由调用者管理
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int IR_GetBackGroundData(unsigned char* data);
    ARSDK_API int IR_GetBackGroundDataChannel(unsigned char** data);

#ifdef CreateFeature
    /**
     * @brief 对@data生成特征, 并将特征保存在用记指定的outputfolder/featurename文件中
     * @param data           输入图像
     * @param featurename    特征名称
     * @param outputfolder   特征输出路径, 必须以'\\'结尾
     * @param addFeatureName 要添加的特征文件.xml
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int IR_CreateFeature(unsigned char* data, char* featurename, char* outputfolder, char* addFeatureName);

    /**
     * @brief 使能相机位置增强
     * @param enable  使能开关, 0: 关闭, 1: 开启, 默认开启
     *
     * @note 相机位置增强是以一定的存储性能为代价来克服相机与对象的夹角对识别的影响，
     *       从而提高算法在不同相对夹角的条件下的稳健性
     */
    ARSDK_API void IR_PositionEnhance(int enhance);
#endif

    /**
     * @brief 释放资源
     *
     * @note 此函数调用后, 需要重新调用IR_init进行初始化后,
     *       才可调用其他函数
     *
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int IR_unit(void);

    /**
     * @brief 在相机暂停、重启时清除内部缓存数据, 以消除残影
     *        应该在相机重启后第一次调用IR_Memory之前调用
     */
    ARSDK_API void CleanFrameData(void);

#if OS_ALVA == OS_Windows || OS_ALVA == OS_Linux
    /**
     * @brief 使能光影增强
     * @param enable  使能开关, 0: 关闭, 1: 开启, 默认关闭
     *
     * @note 光影增强是以一定的计算性能为代价来克服光照条件对识别的影响，
     *       从而提高算法在不同光照条件下的稳健性
     *
     * @note 此设置将在下一次调用IR_Memory, IR_CreateFeature时生效
     *
     * @note 此函数必须在IR_setDataInfoMemory之后
     */
    ARSDK_API void IR_EnableShadowEnhance(int enable);

    /**
     * @brief 使能服务器版本（扩大模板容量为80000，默认20000）
     *
     * @note 此函数须在IR_setDataInfoMemory之后调用
     */
    ARSDK_API int IR_EnableServer(void);
#endif

    /**
     * @brief 设置识别模式
     * @param mode  模式选择 @SearchMode
     *              SM_Full: 全屏, SM_Half: 全屏加中心半屏, SM_Click: 点击区域  默认SM_Full
     *
     * @note 此函数须在IR_setDataInfoMemory之后,跟踪模式下必须为 SM_Full全屏
     */
    ARSDK_API void IR_SetSearchMode(int mode);

    /**
     * @brief 使能跟踪
     *
     * @note 此函数须在IR_setDataInfoMemory之后调用，调用addTrackable之前调用
     * @note 如果需要IR_EnableServer，则须在IR_EnableServer之后调用
     */
    ARSDK_API void IR_EnableTrack(void);

#ifdef __cplusplus
};
#endif

#endif
