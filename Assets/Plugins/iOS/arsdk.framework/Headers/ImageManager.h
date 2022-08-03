#ifndef _IMAGE_MANAGER_H_
#define _IMAGE_MANAGER_H_

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
    ARSDK_API int initTrackerManager(void);

#ifndef CreateFeature
    /**
     * @brief 根据FileNameOfXML中的描述，从FileNameOfXML.dat中加载tracker
     * @param FileNameOfXML xml文件完整路径
     * @param type          文件类型，见Format.h中alvaft
     * @return 0: 成功, 其他: 失败
     *
     * @note 部分错误码如下：
     *         0x8006A80E: This IS Not Own PackageFile
     *         0x8007A80E: PackageFile Version MisMatch
     *         0x8008A80E: PackageFile DataType MisMatch
     *         0x8009A80E: PackageFile DataVersion MisMatch
     */
    ARSDK_API int addTrackable(char* FileNameOfXML, int type);
#endif

    /**
     * @brief 删除库号为trackIndex的trackable
     * @param trackIndex trackable的序号
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int removeTrackable(int trackIndex);

    /**
     * @brief 删除最所有trackable
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int cleanTrackable(void);

    /**
     * @brief 查询trackable总个数
     * @return trackable总个数
     */
    ARSDK_API int getTrackableNumber(void);

    /**
     * @brief 获取名为@nameofTrackable的Trackable
     * @param nameofTrackable 要查询的trackable的名字, 以'\0'结尾
     * @return NULL: 未找到对应名字的trackable
     *         其他: ptr of Trackable
     */
    ARSDK_API void* getTrackableByName(char* nameofTrackable);

    /**
     * @brief 获取序号为@index的Trackable
     * @param index 要查询的trackable的序号
     * @return NULL: 未找到对应名字的trackable
     *         其他: ptr of Trackable
     */
    ARSDK_API void* getTrackableByIndex(int index);

    /**
     * @brief 释放模板管理器环境
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int unitTrackerManager(void);

    /**
     * @brief 获取trackable的序号
     * @param trackable Trackable指针
     *                  由getTrackableByIndex或getTrackableByName得到
     * @return >=0 @trackable的序号
     *          <0 @trackable为空指针
     */
    ARSDK_API int getTrackableIndex(void* trackable);

    /**
     * @brief 获取@trackable的名称
     * @param trackable Trackable指针
     *                  由getTrackableByIndex或getTrackableByName得到
     * @param name      大小为@nameSize的输出缓存空间, trackable的名字
     *                  内存由调用者管理
     * @param nameSize  name的空间大小, 单位: byte
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int getTrackableName(void* trackable, void* name, int nameSize);

    /**
     * @brief 获取@trackable的状态
     * @param trackable Trackable指针
     *                  由getTrackableByIndex或getTrackableByName得到
     * @return @trackable的状态
     *          0: @trackable未激活
     *          1: @trackable已激活
     *       其他: 失败
     */
    ARSDK_API int getTrackableStatus(void* trackable);

    /**
     * @brief 设置@trackable的状态
     * @param trackable Trackable指针
     *                  由getTrackableByIndex或getTrackableByName得到
     * @param status    目标状态. 0: 未激活, 1: 激活
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int setTrackableStatus(void* trackable, int status);

    /**
     * @brief 查询@trackable的模板图像宽度
     * @param trackable Trackable指针
     * @return >=0: @trackable对应模板图像宽度
     *          <0: 失败
     */
    ARSDK_API int getTrackerWidth(void* trackable);

    /**
     * @brief 查询@trackable的模板图像高度
     * @param trackable Trackable指针
     * @return >=0: @trackable对应模板图像高度
     *          <0: 失败
     */
    ARSDK_API int getTrackerHeight(void* trackable);

    /**
     * @brief 获取@trackable对应虚拟Mark的大小
     * @param trackable Trackable指针
     * @param width     输出, 虚拟模型宽
     * @param height    输出, 虚拟模型高
     * @return 0: 成功, 其他: 失败
     * @note 此函数必须在addTrackable及至少两次IR_Memory完成之后调用
	 * @note 虚拟模型定义：(-width / 2, -height / 2, -75.0f)
	 *                    (-width / 2,  height / 2, -75.0f)
	 *                    ( width / 2, -height / 2, -75.0f)
	 *                    ( width / 2,  height / 2, -75.0f)
     */
    ARSDK_API int getTrackVirtualMarkSize(void* trackable, float* width, float* height);

    /**
     * @brief 查询@trackable的姿态矩阵@matrix
     * @param trackable Trackable指针
     * @param matrix    输出, @trackable的姿态矩阵
     *                  格式:
     *                      m11 m12 m13 m14
     *                      m21 m22 m23 m24
     *                      m31 m32 m33 m34
     *                      m41 m42 m43 m44
     * @return 0: 成功, 其他: 失败
     * @note 此函数在模板是跟踪情况下调用
     */
    ARSDK_API int getTrackerPoseMatrix(void* trackable, float* matrix);

    /**
     * @brief 获取跟踪到模板的数量@oFoundNum和序号列表@oFoundIndexs
     * @param oFoundNum    输出, 跟踪到的模板数量
     * @param oFoundIndexs 输出, 跟踪到的模板序号列表
     *                     空间不小于getTrackableNumber() * sizeof(int)
     */
    ARSDK_API void getFoundNumIndexs(int* oFoundNum, int* oFoundIndexs);

    /**
     * @brief 查询@trackable是否被跟踪到
     * @param trackable Trackable指针
     * @return 0: 未被识别到
     *         1: 被识别到
     *      其他: 查询失败
     */
    ARSDK_API int isTrackerFound(void* trackable);

    /**
     * @brief 清除Tracker的查找结果, 所有Trackable都重置为未找到状态
     *
     * @note此函数只有在明确需要重置之前查找结果的情况下使用
     * 例如
     *      在有查找结果后，明确在一段时间未进行输入图像查找，
     *      又在未重新初始化的情况，需要重新输入图像进行查找，
     *      此时在重新输入图像前需要调用此函数清理查找结果
     */
    ARSDK_API void resetTrackerResult(void);

    /**
     * @brief 查询被识别到Trackable的序号, 仅在ImageSearch下可用
     * @return -1: 没有识别到Trackable
     *        >=0: 识别到Trackable的序号
     */
    ARSDK_API int foundTracker(void);

    /**
     * @brief 图像匹配完成回调接口, 仅在ImageSearch下可用
     * @param foundIndex 匹配到的序号
     *                   -1: 未匹配到
     *                   -2: 点击区域识别模式下，未检测到点击区域
     *                   其他: 匹配到的图像的序号
     * @param frameIndex 帧序号, 目前未实现, 请勿使用
     */
    typedef void(*OnSearchDone)(int foundIndex, int frameIndex);

    /**
     * @brief 设置搜索结果回调函数, 仅在ImageSearch下可用
     * @param pOnSearchDone 回调函数指针
     *                      如果传NULL,则清除之前回调函数设置
     *                      在程序结束前必须清空此接口
     * @note 此接口中不要加重负载计算
     */
    ARSDK_API void setOnSearchDone(OnSearchDone pOnSearchDone);

    /**
     * @brief 获取识别的H, 仅在ImageSearch下可用, 在回调函数触发后调用
     * @param H 生成的H
     * @note 此接口中不是线程安全
     */
    ARSDK_API void getSearchH(float* H);
#ifdef __cplusplus
}
#endif

#endif
