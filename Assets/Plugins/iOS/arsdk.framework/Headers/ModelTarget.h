#ifndef _MODEL_TARGET_H_
#define _MODEL_TARGET_H_

#include "DynamicExports.h"
#include "Format.h"

#ifdef __cplusplus
extern "C" {
#endif

    /**
     * @brief ModelTarget
     *        ������������1���£�
     *        ��ʼ��
     *            MT_init
     *            MT_EnhanceVerify����ѡ��
     *            MT_SetSimpleModel����ѡ��
     *            MT_EnableSLAM����ѡ��
     *            MT_setDataInfoMemory
     *            MT_SetCameraInfo
     *            initModelManager
     *            addModel
     *            getModelByIndex(0)
     *        ������
     *            MT_Memory / MT_MemoryChannel
     *        �����ݽ��
     *            isModelFound
     *            MT_GetBackGroundData / MT_GetBackGroundDataChannel
     *            getModelPoseMatrix
     *            getAnchorModelQT
     *            computeProjectMatrixFixRange
     *        ����ʼ��
     *            MT_unit
     *
     *        ������������2���£�(������)
     *        ��ʼ��
     *            MT_init
     *            MT_SetDisplayInfo
     *            MT_GetCameraIntrinsics
     *            initModelManager
     *            addModel
     *            getModelByIndex(0)
     *        ������
     *            MT_Start(ֻ��ȫ����ʼ��֮�����һ��)
     *        �����ݽ��
     *            isModelFound
     *            MT_GetBackGroundData / MT_GetBackGroundDataChannel
     *            getModelPoseMatrix
     *            computeProjectMatrixFixRange
     *        ����ʼ��
     *            MT_unit
     *
     * @note ��ʼ��  ��Androidƽ̨��ʼ�����̲��ܷŵ�OnDraw����
     * @note addModel������ģ��ʱ����������Ե����߳�ȥ���أ���������ټ�����������
     * @note �������幦�ܼ�˵��
     */

    /**
     * @brief ��ʼ������ͼ�����ݱ仯���仯�Ĺ���
     * @param companyName ��Ȩ�Ĺ�˾��
     * @param authString  ��Ȩ��
     * @return 0: �ɹ���������ʧ��
     *
     * @note ��Androidƽ̨�����ô˺���ʱ��Ҫ����android.permission.READ_PHONE_STATEȨ��
     *
     * @note ���ִ�����:
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
     * Android:(����ΪȨ�޴���)
     *            0x8EFF2001: ��android.permission.READ_PHONE_STATEȨ��
     *
     * Android:(���´�����Ϊ��ȡIMEI����)
     *            0x8E012001:
     *            0x8E022001:
     *            ...
     *            0x8E1F2001:
     * Windows:(���´�����Ϊ��ȡMAC����)
     *            0x80E12001:
     *            0x80E22001:
     *            0x80E32001:
     * Linux:(���´�����Ϊ��ȡMAC����)
     *            0x80E12001:
     *            0x80E22001:
     *
     * @note ��Linuxƽ̨����ȷ����Ȩ�õ�MAC��ַ��Ӧ��������Ϊ"eth0"
     */
    ARSDK_API int MT_init(char* companyName, char* authString);

    /**
     * @brief ���������Ϣ
     * @param iw            ͼ��Ŀ��, ͬ@MT_setDataInfoMemory��@wid
     * @param ih            ͼ��ĸ߶�, ͬ@MT_setDataInfoMemory��@hei
     * @param fx            ����x
     * @param fy            ����y
     * @param cx            ͶӰ����x
     * @param cy            ͶӰ����y
     * @param screenW       ��Ļ���
     * @param screenH       ��Ļ�߶�
     * @param rotation      ����������@alvaro
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_SetCameraInfo(float iw, float ih, float fx, float fy, float cx, float cy, int screenW, int screenH, int rotation);

    /**
     * @brief ����ͼ����Ϣ
     * @param wid ͼ��Ŀ��, ͬ@MT_SetCameraInfo��@iw
     * @param hei ͼ��ĸ߶�, ͬ@MT_SetCameraInfo��@ih
     * @param fmt ͼ���ʽ, ��@alvafmt, Ŀǰ��֧��Alva_FMT_GRAY,Alva_FMT_YUV_420p,Alva_FMT_NV21,Alva_FMT_NV12
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_setDataInfoMemory(int wid, int hei, int fmt);

    /**
     * @brief �����ֻ���Ϣ
     * @param display_rotation ����������@alvaro
     * @param display_width    ��Ļ��
     * @param display_height   ��Ļ��
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_SetDisplayInfo(int display_rotation, int display_width, int display_height);

    /**
     * @brief ��ȡ�ڲΣ�����ӿ�
     * @param img_width  ͼ���
     * @param img_height ͼ���
     * @param fx         ����fx
     * @param fy         ����fx
     * @param cx         ͶӰ����x
     * @param cy         ͶӰ����y
     * @param fmt        ͼ���ʽ
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_GetCameraIntrinsics(int *img_width, int *img_height, float *fx, float *fy, float *cx, float *cy, int* fmt);

    /**
     * @brief ��ʼ�����֮����ã��������
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_Start(void);

    /**
     * @brief Ϊʶ����ĸ���һ֡ͼ������
     * @param data              ��������ָ��
     * @param viewMatrix_CMO    ����ͷ���ӽǾ���, ��ʽ����:
     *                          m11 m12 m13 m14
     *                          m21 m22 m23 m24
     *                          m31 m32 m33 m34
     *                          m41 m42 m43 m44
     * @param anchorMatrix_CMO  ê���ģ�;���, ��ʽ����:
     *                          m11 m12 m13 m14
     *                          m21 m22 m23 m24
     *                          m31 m32 m33 m34
     *                          m41 m42 m43 m44
     * @param camStatus         ���״̬, ��@alvacs
     * @param anchorIndex       ê������, (��ǰһ��ê����������������1����ʼ����Ϊ0��û��ê��ʱΪ-1)
     * @param anchorStatus      ê��״̬, ��@alvacs
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_Memory(unsigned char*  data, float* viewMatrix_CMO, float* anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus);
    ARSDK_API int MT_MemoryChannel(unsigned char** data, float* viewMatrix_CMO, float* anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus);

    /**
     * @brief ��ȡһ֡��������Ⱦ��ͼ������
     * @param data ���ͼ�񻺳���ָ��, �ڴ��ɵ����߹���
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_GetBackGroundData(unsigned char* data);
    ARSDK_API int MT_GetBackGroundDataChannel(unsigned char** data);

    /**
     * @brief �ͷ���Դ
     *
     * @note �ڵ��ô˺���ǰ�󣬽�ֹ���������ӿڽ��л�ȡ����
     *       �˺������ú�, ��Ҫ���µ���MT_init���г�ʼ����,�ſɵ�����������
     *
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_unit(void);

    /**
     * @brief �������ͣ������ʱ����ڲ���������, ��������Ӱ
     *        Ӧ��������������һ�ε���MT_Memory֮ǰ����
     */
    ARSDK_API void MT_CleanFrameData(void);

    /**
     * @brief ��ǿ��֤
     *
     * @note Ĭ�Ͽ�����ģ���Ĳ������ɹرտ���
     *
     * @param switchValue ����ֵ, 1: ����, 0: �ر�
     *
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_EnhanceVerify(int switchValue);

    /**
     * @brief ���ü�ģ��
     *
     * @note Ĭ��Alva_NoSimple
     *
     * @param simpleModelType  ��ģ������, ��@alvamt
     *
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_SetSimpleModel(int simpleModelType);

    /**
     * @brief ʹ��slam
     *
     * @note Ĭ�Ͽ���(MT_init֮�����)
     *
     * @param switchValue ����ֵ, 1: ����, 0: �ر�
     *
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int MT_EnableSLAM(int switchValue);

#ifdef __cplusplus
};
#endif

#endif
