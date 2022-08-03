#ifndef _MODEL_MANAGER_H_
#define _MODEL_MANAGER_H_

#include "DynamicExports.h"

#ifdef __cplusplus
extern "C" {
#endif

    /**
     * @brief 初始化模板管理器,
     *        如果已经初始化过, 则会清空上次初始化
     *
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int initModelManager(void);

    /**
     * @brief 根据@FileNameOfXML中的描述，从FileNameOfXML.dat中加载相关信息
     *
     * @param FileNameOfXML   xml文件完整路径
     * @param type            文件类型，见Format.h中alvaft
     *
     * @return 0: 成功, 其他: 失败
     *
     * @note 此函数添加时间过长，不建议在主线程中添加
     *       如果在子线程中添加，需要主线程等待此函数运行结果返回成功再继续流程
     * @note 部分错误码如下：
     *       0x8007A80E: 打包工具版本不兼容
     *       0x8009A80E: 数据包版本不兼容
     */
    ARSDK_API int addModel(char* FileNameOfXML, int type);

    /**
     * @brief 删除库号为@modelIndex的model
     * @param modelIndex model的序号
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int removeModel(int modelIndex);

    /**
     * @brief 删除最所有Model
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int cleanModel(void);

    /**
     * @brief 查询model总个数
     * @return model总个数
     */
    ARSDK_API int getModelNumber(void);

    /**
     * @brief 获取名为@nameOfModel的Model
     * @param nameOfModel 要查询的model的名字, 以'\0'结尾
     * @return NULL: 未找到对应名字的model
     *         其他: ptr of Model
     */
    ARSDK_API void* getModelByName(char* nameOfModel);

    /**
     * @brief 获取序号为@index的Model
     * @param index 要查询的model的序号
     * @return NULL: 未找到对应名字的model
     *         其他: ptr of Model
     */
    ARSDK_API void* getModelByIndex(int index);

    /**
     * @brief 释放模板管理器环境
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int unitModelManager(void);

    /**
     * @brief 获取@model的序号
     * @param model Model指针
     *              由@getModelByIndex或@getModelByName得到
     * @return >=0 @model的序号
     *          <0 @model为空指针
     */
    ARSDK_API int getModelIndex(void* model);

    /**
     * @brief 获取@model的名称
     * @param model    Model指针
     *                 由@getModelByIndex或@getModelByName得到
     * @param name     大小为@nameSize的输出缓存空间, model的名字
     *                 内存由调用者管理
     * @param nameSize name的空间大小, 单位: byte
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int getModelName(void* model, void* name, int nameSize);

    /**
     * @brief 获取@model的状态
     * @param model Model指针
     *              由@getModelByIndex或@getModelByName得到
     * @return @model的状态
     *          0: @model未激活
     *          1: @model已激活
     *       其他: 失败
     */
    ARSDK_API int getModelStatus(void* model);

    /**
     * @brief 设置@model的状态
     * @param model  Model指针
     *               由@getModelByIndex或@getModelByName得到
     * @param status 目标状态. 0: 未激活, 1: 激活
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int setModelStatus(void* model, int status);

    /**
     * @brief 查询@model的姿态@rts
     * @param model Model指针
     * @param rts   输出, @model的姿态
     *              格式:
     *                  q1 q2 q3 q4 t1 t2 t3 s1 s2 s3
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int getModelRTS(void* model, float* rts);

    /**
     * @brief 查询@model的姿态矩阵@viewMatrix_CMO
     * @param model          Model指针
     * @param viewMatrix_CMO 输出, @model的视角姿态矩阵,列主序
     *                       格式: m11 m12 m13 m14
     *                             m21 m22 m23 m24
     *                             m31 m32 m33 m34
     *                             m41 m42 m43 m44
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int getModelPoseMatrix(void* model, float* viewMatrix_CMO);

    /**
     * @brief 查询@model的锚点模型位姿
     *        锚点只允许添加一个
     *        在第一次添加锚点之前（选择：可提示用户左右晃动手机使其稳定）
     *        在锚点失效情况下，如果可添加锚点，则需删除锚点，重新添加（选择：也可在锚点失效时，提示用户晃动手机）
     *        在锚点有效情况下，如果需要删除锚点，在可添加锚点情况下，重新添加
     *        可在第一次提示，后面中间过程可以选择提示或者不提示
     * @param model          Model指针
     * @param anchorQT       输出, @model的锚点模型位姿
     *                       格式: q1 q2 q3 q4 t1 t2 t3
     * @param isDelete       输出，int值，1：删除锚点，重新添加锚点
     *                       删除添加锚点需要在isAdd为1时进行
     * @param isAdd          输出，int值  1：可添加锚点 0: 不可添加锚点用此qt
     *
     * @note                 两种情况删除锚点重新添加
     *                       1. 锚点失效，返回可添加锚点
     *                       2. 锚点有效，返回可添加锚点，并且isDelete返回1
     *
     * @return 0: 成功  其他: 失败
     */
    ARSDK_API int getAnchorModelQT(void* model, float* anchorQT, int* isDelete, int* isAdd);

    /**
     * @brief 查询@model是否被识别到
     * @param model Model指针
     * @return 0: 未被识别到
     *         1: 被识别到
     *      其他: 查询失败
     */
    ARSDK_API int isModelFound(void* model);

    /**
     * @brief 清除Model的查找结果, 所有Model都重置为未找到状态
     *
     * @note此函数只有在明确需要重置之前查找结果的情况下使用
     * 例如
     *      在有查找结果后，明确在一段时间未进行输入图像查找，
     *      又在未重新初始化的情况，需要重新输入图像进行查找，
     *      此时在重新输入图像前需要调用此函数清理查找结果
     */
    ARSDK_API void resetModelResult(void);

    /**
     * @brief 计算投影矩阵
     * @param wid                视图宽
     * @param hei                视图高
     * @param viewMatrix_LMO     输入，视角矩阵，16 * sizeof(float)大小空间, 行主序
     * @param projectMatrix_CMO  输出，投影矩阵，16 * sizeof(float)大小空间, 列主序
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int computeProjectMatrix(int wid, int hei, float* viewMatrix_LMO, float* projectMatrix_CMO);

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
    ARSDK_API int computeProjectMatrixFixRange(int wid, int hei, float nearV, float farV, float* projectMatrix_CMO);

    /**
     * @brief 获取模型初始位姿下的透明图RGBA格式，宽800 高480
     *        在函数addModel之后调用
     * @param model   Model指针
     * @param picData 输出buff 由调用者管理空间（宽*高*4）
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int getModelGuideImage(void* model, unsigned char* picData);

#ifdef __cplusplus
}
#endif

#endif
