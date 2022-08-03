#ifndef _MODEL_MANAGER_H_
#define _MODEL_MANAGER_H_

#include "DynamicExports.h"

#ifdef __cplusplus
extern "C" {
#endif

    /**
     * @brief ��ʼ��ģ�������,
     *        ����Ѿ���ʼ����, �������ϴγ�ʼ��
     *
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int initModelManager(void);

    /**
     * @brief ����@FileNameOfXML�е���������FileNameOfXML.dat�м��������Ϣ
     *
     * @param FileNameOfXML   xml�ļ�����·��
     * @param type            �ļ����ͣ���Format.h��alvaft
     *
     * @return 0: �ɹ�, ����: ʧ��
     *
     * @note �˺������ʱ������������������߳������
     *       ��������߳�����ӣ���Ҫ���̵߳ȴ��˺������н�����سɹ��ټ�������
     * @note ���ִ��������£�
     *       0x8007A80E: ������߰汾������
     *       0x8009A80E: ���ݰ��汾������
     */
    ARSDK_API int addModel(char* FileNameOfXML, int type);

    /**
     * @brief ɾ�����Ϊ@modelIndex��model
     * @param modelIndex model�����
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int removeModel(int modelIndex);

    /**
     * @brief ɾ��������Model
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int cleanModel(void);

    /**
     * @brief ��ѯmodel�ܸ���
     * @return model�ܸ���
     */
    ARSDK_API int getModelNumber(void);

    /**
     * @brief ��ȡ��Ϊ@nameOfModel��Model
     * @param nameOfModel Ҫ��ѯ��model������, ��'\0'��β
     * @return NULL: δ�ҵ���Ӧ���ֵ�model
     *         ����: ptr of Model
     */
    ARSDK_API void* getModelByName(char* nameOfModel);

    /**
     * @brief ��ȡ���Ϊ@index��Model
     * @param index Ҫ��ѯ��model�����
     * @return NULL: δ�ҵ���Ӧ���ֵ�model
     *         ����: ptr of Model
     */
    ARSDK_API void* getModelByIndex(int index);

    /**
     * @brief �ͷ�ģ�����������
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int unitModelManager(void);

    /**
     * @brief ��ȡ@model�����
     * @param model Modelָ��
     *              ��@getModelByIndex��@getModelByName�õ�
     * @return >=0 @model�����
     *          <0 @modelΪ��ָ��
     */
    ARSDK_API int getModelIndex(void* model);

    /**
     * @brief ��ȡ@model������
     * @param model    Modelָ��
     *                 ��@getModelByIndex��@getModelByName�õ�
     * @param name     ��СΪ@nameSize���������ռ�, model������
     *                 �ڴ��ɵ����߹���
     * @param nameSize name�Ŀռ��С, ��λ: byte
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int getModelName(void* model, void* name, int nameSize);

    /**
     * @brief ��ȡ@model��״̬
     * @param model Modelָ��
     *              ��@getModelByIndex��@getModelByName�õ�
     * @return @model��״̬
     *          0: @modelδ����
     *          1: @model�Ѽ���
     *       ����: ʧ��
     */
    ARSDK_API int getModelStatus(void* model);

    /**
     * @brief ����@model��״̬
     * @param model  Modelָ��
     *               ��@getModelByIndex��@getModelByName�õ�
     * @param status Ŀ��״̬. 0: δ����, 1: ����
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int setModelStatus(void* model, int status);

    /**
     * @brief ��ѯ@model����̬@rts
     * @param model Modelָ��
     * @param rts   ���, @model����̬
     *              ��ʽ:
     *                  q1 q2 q3 q4 t1 t2 t3 s1 s2 s3
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int getModelRTS(void* model, float* rts);

    /**
     * @brief ��ѯ@model����̬����@viewMatrix_CMO
     * @param model          Modelָ��
     * @param viewMatrix_CMO ���, @model���ӽ���̬����,������
     *                       ��ʽ: m11 m12 m13 m14
     *                             m21 m22 m23 m24
     *                             m31 m32 m33 m34
     *                             m41 m42 m43 m44
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int getModelPoseMatrix(void* model, float* viewMatrix_CMO);

    /**
     * @brief ��ѯ@model��ê��ģ��λ��
     *        ê��ֻ�������һ��
     *        �ڵ�һ�����ê��֮ǰ��ѡ�񣺿���ʾ�û����һζ��ֻ�ʹ���ȶ���
     *        ��ê��ʧЧ����£���������ê�㣬����ɾ��ê�㣬������ӣ�ѡ��Ҳ����ê��ʧЧʱ����ʾ�û��ζ��ֻ���
     *        ��ê����Ч����£������Ҫɾ��ê�㣬�ڿ����ê������£��������
     *        ���ڵ�һ����ʾ�������м���̿���ѡ����ʾ���߲���ʾ
     * @param model          Modelָ��
     * @param anchorQT       ���, @model��ê��ģ��λ��
     *                       ��ʽ: q1 q2 q3 q4 t1 t2 t3
     * @param isDelete       �����intֵ��1��ɾ��ê�㣬�������ê��
     *                       ɾ�����ê����Ҫ��isAddΪ1ʱ����
     * @param isAdd          �����intֵ  1�������ê�� 0: �������ê���ô�qt
     *
     * @note                 �������ɾ��ê���������
     *                       1. ê��ʧЧ�����ؿ����ê��
     *                       2. ê����Ч�����ؿ����ê�㣬����isDelete����1
     *
     * @return 0: �ɹ�  ����: ʧ��
     */
    ARSDK_API int getAnchorModelQT(void* model, float* anchorQT, int* isDelete, int* isAdd);

    /**
     * @brief ��ѯ@model�Ƿ�ʶ��
     * @param model Modelָ��
     * @return 0: δ��ʶ��
     *         1: ��ʶ��
     *      ����: ��ѯʧ��
     */
    ARSDK_API int isModelFound(void* model);

    /**
     * @brief ���Model�Ĳ��ҽ��, ����Model������Ϊδ�ҵ�״̬
     *
     * @note�˺���ֻ������ȷ��Ҫ����֮ǰ���ҽ���������ʹ��
     * ����
     *      ���в��ҽ������ȷ��һ��ʱ��δ��������ͼ����ң�
     *      ����δ���³�ʼ�����������Ҫ��������ͼ����в��ң�
     *      ��ʱ����������ͼ��ǰ��Ҫ���ô˺���������ҽ��
     */
    ARSDK_API void resetModelResult(void);

    /**
     * @brief ����ͶӰ����
     * @param wid                ��ͼ��
     * @param hei                ��ͼ��
     * @param viewMatrix_LMO     ���룬�ӽǾ���16 * sizeof(float)��С�ռ�, ������
     * @param projectMatrix_CMO  �����ͶӰ����16 * sizeof(float)��С�ռ�, ������
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int computeProjectMatrix(int wid, int hei, float* viewMatrix_LMO, float* projectMatrix_CMO);

    /**
     * @brief ����̶��ü���Χ��ͶӰ����
     * @param wid                ��ͼ��
     * @param hei                ��ͼ��
     * @param nearV              ���ü���
     * @param farV               Զ�ü���
     * @param projectMatrix_CMO  �����ͶӰ����16 * sizeof(float)��С�ռ�, ������
     * @return 0: �ɹ�, ����: ʧ��
     *
     * @note ���@wid > @hei, ��Ϊ��ʾΪ����
     * @note ���@wid < @hei, ��Ϊ��ʾΪ����
     */
    ARSDK_API int computeProjectMatrixFixRange(int wid, int hei, float nearV, float farV, float* projectMatrix_CMO);

    /**
     * @brief ��ȡģ�ͳ�ʼλ���µ�͸��ͼRGBA��ʽ����800 ��480
     *        �ں���addModel֮�����
     * @param model   Modelָ��
     * @param picData ���buff �ɵ����߹���ռ䣨��*��*4��
     * @return 0: �ɹ�, ����: ʧ��
     */
    ARSDK_API int getModelGuideImage(void* model, unsigned char* picData);

#ifdef __cplusplus
}
#endif

#endif
