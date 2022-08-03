#ifndef _MODEL_TARGET_H_
#define _MODEL_TARGET_H_

#include "DynamicExports.h"
#include "Format.h"

#ifdef __cplusplus
extern "C" {
#endif

    /**
     * @brief ModelTarget
     *        基本调用流程1如下：
     *        初始化
     *            MT_init
     *            MT_EnhanceVerify（可选）
     *            MT_SetSimpleModel（可选）
     *            MT_EnableSLAM（可选）
     *            MT_setDataInfoMemory
     *            MT_SetCameraInfo
     *            initModelManager
     *            addModel
     *            getModelByIndex(0)
     *        加数据
     *            MT_Memory / MT_MemoryChannel
     *        拿数据结果
     *            isModelFound
     *            MT_GetBackGroundData / MT_GetBackGroundDataChannel
     *            getModelPoseMatrix
     *            getAnchorModelQT
     *            computeProjectMatrixFixRange
     *        反初始化
     *            MT_unit
     *
     *        基本调用流程2如下：(不启用)
     *        初始化
     *            MT_init
     *            MT_SetDisplayInfo
     *            MT_GetCameraIntrinsics
     *            initModelManager
     *            addModel
     *            getModelByIndex(0)
     *        加数据
     *            MT_Start(只用全部初始化之后调用一次)
     *        拿数据结果
     *            isModelFound
     *            MT_GetBackGroundData / MT_GetBackGroundDataChannel
     *            getModelPoseMatrix
     *            computeProjectMatrixFixRange
     *        反初始化
     *            MT_unit
     *
     * @note 初始化  ：Android平台初始化流程不能放到OnDraw里面
     * @note addModel：加载模型时间过长，可以单独线程去加载，加载完成再继续下面流程
     * @note 函数具体功能见说明
     */

    /**
     * @brief 初始化不因图像数据变化而变化的功能
     * @param companyName 授权的公司名
     * @param authString  授权码
     * @return 0: 成功，其他：失败
     *
     * @note 在Android平台，调用此函数时需要具有android.permission.READ_PHONE_STATE权限
     *
     * @note 部分错误码:
     *            0x80012001:
     *            0x80012001: Authorize code is not correct.
     *            0x80032001: Authorize code is not for @companyName.
     *            0x80042001: Authorize code is not for current package.
     *            0x80052001:
     *            0x80062001: Authorize code is not for this machine.
     *            0x80072001: System time is not correct.
     *            0x80082001: Trying period is over.
     *            0x80092001: Authorize code is not for this app.
     *            0x800A2001: Authorize code is not for this app's module.
     *
     * Android:(以下为权限错误)
     *            0x8EFF2001: 无android.permission.READ_PHONE_STATE权限
     *
     * Android:(以下错误码为获取IMEI错误)
     *            0x8E012001:
     *            0x8E022001:
     *            ...
     *            0x8E1F2001:
     * Windows:(以下错误码为获取MAC错误)
     *            0x80E12001:
     *            0x80E22001:
     *            0x80E32001:
     * Linux:(以下错误码为获取MAC错误)
     *            0x80E12001:
     *            0x80E22001:
     *
     * @note 在Linux平台上请确保授权用的MAC地址对应的网卡名为"eth0"
     */
    ARSDK_API int MT_init(char* companyName, char* authString);

    /**
     * @brief 设置相机信息
     * @param iw            图像的宽度, 同@MT_setDataInfoMemory的@wid
     * @param ih            图像的高度, 同@MT_setDataInfoMemory的@hei
     * @param fx            焦距x
     * @param fy            焦距y
     * @param cx            投影中心x
     * @param cy            投影中心y
     * @param screenW       屏幕宽度
     * @param screenH       屏幕高度
     * @param rotation      横竖屏，见@alvaro
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_SetCameraInfo(float iw, float ih, float fx, float fy, float cx, float cy, int screenW, int screenH, int rotation);

    /**
     * @brief 设置图像信息
     * @param wid 图像的宽度, 同@MT_SetCameraInfo的@iw
     * @param hei 图像的高度, 同@MT_SetCameraInfo的@ih
     * @param fmt 图像格式, 见@alvafmt, 目前仅支持Alva_FMT_GRAY,Alva_FMT_YUV_420p,Alva_FMT_NV21,Alva_FMT_NV12
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_setDataInfoMemory(int wid, int hei, int fmt);

    /**
     * @brief 设置手机信息
     * @param display_rotation 横竖屏，见@alvaro
     * @param display_width    屏幕宽
     * @param display_height   屏幕高
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_SetDisplayInfo(int display_rotation, int display_width, int display_height);

    /**
     * @brief 获取内参，输出接口
     * @param img_width  图像宽
     * @param img_height 图像高
     * @param fx         焦距fx
     * @param fy         焦距fx
     * @param cx         投影中心x
     * @param cy         投影中心y
     * @param fmt        图像格式
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_GetCameraIntrinsics(int *img_width, int *img_height, float *fx, float *fy, float *cx, float *cy, int* fmt);

    /**
     * @brief 初始化完成之后调用，添加数据
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_Start(void);

    /**
     * @brief 为识别核心更新一帧图像及数据
     * @param data              输入数据指针
     * @param viewMatrix_CMO    摄像头的视角矩阵, 格式如下:
     *                          m11 m12 m13 m14
     *                          m21 m22 m23 m24
     *                          m31 m32 m33 m34
     *                          m41 m42 m43 m44
     * @param anchorMatrix_CMO  锚点的模型矩阵, 格式如下:
     *                          m11 m12 m13 m14
     *                          m21 m22 m23 m24
     *                          m31 m32 m33 m34
     *                          m41 m42 m43 m44
     * @param camStatus         相机状态, 见@alvacs
     * @param anchorIndex       锚点索引, (在前一个锚点索引基础上增加1，初始索引为0，没有锚点时为-1)
     * @param anchorStatus      锚点状态, 见@alvacs
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_Memory(unsigned char*  data, float* viewMatrix_CMO, float* anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus);
    ARSDK_API int MT_MemoryChannel(unsigned char** data, float* viewMatrix_CMO, float* anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus);

    /**
     * @brief 获取一帧将用于渲染的图像数据
     * @param data 输出图像缓冲区指针, 内存由调用者管理
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_GetBackGroundData(unsigned char* data);
    ARSDK_API int MT_GetBackGroundDataChannel(unsigned char** data);

    /**
     * @brief 释放资源
     *
     * @note 在调用此函数前后，禁止调用其他接口进行获取数据
     *       此函数调用后, 需要重新调用MT_init进行初始化后,才可调用其他函数
     *
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_unit(void);

    /**
     * @brief 在相机暂停、重启时清除内部缓存数据, 以消除残影
     *        应该在相机重启后第一次调用MT_Memory之前调用
     */
    ARSDK_API void MT_CleanFrameData(void);

    /**
     * @brief 增强验证
     *
     * @note 默认开启，模型拍不完整可关闭开关
     *
     * @param switchValue 开关值, 1: 开启, 0: 关闭
     *
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_EnhanceVerify(int switchValue);

    /**
     * @brief 设置简单模型
     *
     * @note 默认Alva_NoSimple
     *
     * @param simpleModelType  简单模型类型, 见@alvamt
     *
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_SetSimpleModel(int simpleModelType);

    /**
     * @brief 使能slam
     *
     * @note 默认开启(MT_init之后调用)
     *
     * @param switchValue 开关值, 1: 开启, 0: 关闭
     *
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int MT_EnableSLAM(int switchValue);

#ifdef __cplusplus
};
#endif

#endif
