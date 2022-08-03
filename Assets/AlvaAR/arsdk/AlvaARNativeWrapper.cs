using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static arsdk.AlvaARUnity;

namespace arsdk
{
    internal class AlvaARNativeWrapper : IAlvaARWrapper
    {
        #region C# interface.........

        #region IR Interface............
        public int GetTrackVirtualMarkSize(IntPtr trackable, float[] width, float[] height)
        {
            return getTrackVirtualMarkSize(trackable, width, height);
        }
        public int InitIR(AlvaARUnity.AlvaDataType dataType, string companyName, string authString)
        {
            return IR_init((int)dataType, companyName, authString);
        }

        public int UnitIR()
        {
            return IR_unit();
        }

        public int SetDataInfoIRMemory(int wid, int hei, int fmt)
        {
            return IR_setDataInfoMemory(wid, hei, fmt);
        }

        public int IRMemory(byte[] data, float[] coord, float[] dataMatrix, float[] viewMatrix)
        {
            return IR_Memory(data, coord, dataMatrix, viewMatrix);
        }

        public int GetBackGroundData(byte[] data)
        {
            return IR_GetBackGroundData(data);
        }
        public int GetFoundNumIndexs(int[] oFoundNum, int[] oFoundIndexs)
        {
            return getFoundNumIndexs(oFoundNum, oFoundIndexs);
        }
        /*
        public int GetTrackerRTS(long trackable, float[] rts)
        {
            return getTrackerRTS(trackable, rts);
        }
        */
        public int GetTrackerPoseMatrix(long trackable, float[] matrix)
        {
            return getTrackerPoseMatrix(trackable, matrix);
        }
        public void InitTrackerManager()
        {
            AlvaARNativeWrapper.initTrackerManager();
        }

        public int UnitTrackerManager()
        {
            return AlvaARNativeWrapper.unitTrackerManager();
        }

        public void TrackerManagerOnSearchDone(SearchDoneClient.SearchDoneListener callbackPointer)
        {
            AlvaARNativeWrapper.setOnSearchDone(callbackPointer);
        }

        public int TrackerManagerCleanTracker()
        {
            return AlvaARNativeWrapper.cleanTrackable();
        }

        public int TrackerManagerAddXML(string FileNameOfXML, AlvaARUnity.AlvaFileType type)
        {
            return AlvaARNativeWrapper.addTrackable(FileNameOfXML, (int)type);
        }

        public int TrackerManagerGetNumber()
        {
            return AlvaARNativeWrapper.getTrackableNumber();
        }

        public IntPtr TrackerManagerGetByName(StringBuilder nameofTracker)
        {
            return AlvaARNativeWrapper.getTrackableByName(nameofTracker);
        }

        public IntPtr TrackerManagerGetByIndex(int index)
        {
            return AlvaARNativeWrapper.getTrackableByIndex(index);
        }

        public int TrackerManagerOfFoundTrackerID()
        {
            return AlvaARNativeWrapper.foundTracker();
        }

        public int AddTrackable(string FileNameOfXML, int type)
        {
            return addTrackable(FileNameOfXML, type);
        }

        public int SetTrackableStatus(long trackable, int status)
        {
            return setTrackableStatus(trackable, status);
        }

        public void YuvScaleI420(byte[] i420Src, int width, int height, byte[] i420Dst, int dstWidth, int dstHeight, int mode)
        {
            //scaleI420(i420Src, width, height, i420Dst, dstWidth, dstHeight, mode);
        }

        public void YuvRotateI420(byte[] i420Src, int width, int height, byte[] i420Dst, int degree)
        {
            //rotateI420(i420Src, width, height, i420Dst, degree);
        }
        #endregion IR Interface............

        #region MT Interface............

        public int InitMT(string companyName, string authString)
        {
            return MT_init(companyName, authString);
        }

        public int SetMTCameraInfo(float iw, float ih, float fx, float fy, float cx, float cy, int screenW, int screenH, AlvaRotation rotation)
        {
            return MT_SetCameraInfo(iw, ih, fx, fy, cx, cy, screenW, screenH, (int)rotation);
        }

        public int SetMTDataInfoMemory(int wid, int hei, AlvaARUnity.AlvaFormat fmt)
        {
            return MT_setDataInfoMemory(wid, hei, (int)fmt);
        }

        public int MTMemory(byte[] data,  float[] viewMatrix_CMO, float[] anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus)
        {
            return MT_Memory(data,  viewMatrix_CMO, anchorMatrix_CMO, camStatus, anchorIndex, anchorStatus);
        }

       // MT_MemoryChannel(IntPtr[] data, float[] viewMatrix_CMO, float[] anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus)
        public int MTMemoryChannel(IntPtr[] data, float[] viewMatrix_CMO, float[] anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus)
        {
            return MT_MemoryChannel(data, viewMatrix_CMO, anchorMatrix_CMO, camStatus, anchorIndex, anchorStatus);
        }


        public int GetMTBackGroundData(IntPtr data)
        {
            return MT_GetBackGroundData(data);
        }

        public int GetMTBackGroundDataChannel(IntPtr[] data)
        {
            return MT_GetBackGroundDataChannel(data);
        }

        public int UnitMT()
        {
            return MT_unit();
        }

        public void CleanMTFrameData()
        {
            MT_CleanFrameData();
        }

        public int InitModelManager()
        {
            return initModelManager();
        }

        public int AddModel(string fileNameOfXML)
        {
            //IntPtr model = Marshal.StringToHGlobalAuto(modelPath);
            // IntPtr config= Marshal.StringToHGlobalAuto(configPath);

            // Create two different encodings.

            return addModel(fileNameOfXML, 1);
        }

        public int RemoveModel(int modelIndex)
        {
            return removeModel(modelIndex);
        }


        public int CleanModel()
        {
            return cleanModel();
        }


        public int GetModelNumber()
        {
            return getModelNumber();
        }


        public IntPtr GetModelByName(string nameOfModel)
        {
            return getModelByName(nameOfModel);
        }


        public IntPtr GetModelByIndex(int index)
        {
            return getModelByIndex(index);
        }

        public int UnitModelManager()
        {
            return unitModelManager();
        }


        public int GetModelIndex(IntPtr model)
        {
            return getModelIndex(model);
        }


        public int GetModelName(IntPtr model, IntPtr name, int nameSize)
        {
            return getModelName(model, name, nameSize);
        }


        public int GetModelStatus(IntPtr model)
        {
            return getModelStatus(model);
        }

        public int SetModelStatus(IntPtr model, int status)
        {
            return setModelStatus(model, status);
        }


        public int GetModelRTS(IntPtr model, float[] rts)
        {
            return getModelRTS(model, rts);
        }


        public int GetModelPoseMatrix(IntPtr model, float[] matrix)
        {
            return getModelPoseMatrix(model, matrix);
        }

        public int IsModelFound(IntPtr model)
        {
            return isModelFound(model);
        }

        public int ComputeProjectMatrix(int wid, int hei, float[] viewMatrix, float[] projectMatrix)
        {
            return computeProjectMatrix(wid, hei, viewMatrix, projectMatrix);
        }


        public void ResetModelResult()
        {
            resetModelResult();
        }
        public int GetAnchorModelQT(IntPtr model, float[] anchorQT, int[] isDelete)
        {
           return  getAnchorModelQT( model, anchorQT, isDelete);
        }
        public void SetOnMTSearchDone(OnMTSearchDone onMTSearchDone)
        {
            //setOnMTSearchDone(onMTSearchDone);
        }

        public int ComputeProjectMatrixFixRange(int wid, int hei, float nearV, float farV, float[] projectMatrix)
        {
            return computeProjectMatrixFixRange(wid, hei, nearV, farV, projectMatrix);
        }


        public void GetLogBuffData(string dataBuff)
        {
            //GetLogBuffData(dataBuff);
        }


        #endregion MT Interface............

        #endregion C# interface.........

        #region external native interface.........

        #region IR Interface-------
        /**
         * @brief 初始化不因图像数据变化而变化的功能
         * @param dataType    后续输入数据的类型，见Format.h中alvadt，目前仅支持Alva_Memory
         * @param companyName 授权的公司名
         * @param authString  授权码
         * @return 0: 成功，其他：失败
         * 部分错误码: 0x80011101:
         *            0x80011101: Authorize code is not correct.
         *            0x80031101: Authorize code is not for @companyName.
         *            0x80041101: Authorize code is not for current package.
         *            0x80051101:
         *            0x80061101: Authorize code is not for this machine.
         *            0x80071101: System time is not correct.
         *            0x80081101: Trying period is over.
         *            0x80091101: Authorize code is not for this app.
         *            0x800A1101: Authorize code is not for this app's module.
         * Android:(以下错误码为获取IMEI错误)
         *            0x80E11101:
         *            0x80E21101:
         *            0x80E31101:
         *            0x80E41101:
         *            0x80E51101:
         *            0x80E61101:
         *            0x80E71101:
         * Windows:(以下错误码为获取MAC错误)
         *            0x80E11101:
         *            0x80E21101:
         *            0x80E31101:
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int IR_init(int dataType, string companyName, string authString);

        /**
         * @brief 设置图像信息
         * @param wid 图像的宽度
         * @param hei 图像的高度
         * @param fmt 图像格式, 见@alvafmt, 目前仅支持Alva_FMT_YUV_420p
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int IR_setDataInfoMemory(int wid, int hei, int fmt);

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
            * @return 0: 成功, 其他: 失败
            */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int IR_Memory(byte[] data, float[] coord, float[] dataMatrix, float[] viewMatrix);



        /**
     * @brief 设置@trackable的状态
     * @param trackable Trackable指针
     *                  由getTrackableByIndex或getTrackableByName得到
     * @param status    目标状态. 0: 未激活, 1: 激活
     * @return 0: 成功, 其他: 失败
     */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int setTrackableStatus(long trackable, int status);

        /**
         * @brief 获取一帧将用于渲染的图像数据
         * @param data 输出图像缓冲区指针, 内存由调用者管理
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int IR_GetBackGroundData(byte[] data);


        /**
     * @brief 获取跟踪到模板的数量@oFoundNum和序号列表@oFoundIndexs
     * @param oFoundNum    输出, 跟踪到的模板数量
     * @param oFoundIndexs 输出, 跟踪到的模板序号列表
     *                     空间不小于getTrackableNumber() * sizeof(int)
     */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getFoundNumIndexs(int[] oFoundNum, int[] oFoundIndexs);

        /**
     * @brief 查询@trackable的姿态@rts
     * @param trackable Trackable指针
     * @param rts       输出, @trackable的姿态
     *                  格式:
     *                      q1 q2 q3 q4 t1 t2 t3 s1 s2 s3
     * @return 0: 成功, 其他: 失败
     
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getTrackerRTS(long trackable, float[] rts);
        */

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
     */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getTrackerPoseMatrix(long trackable, float[] matrix);

        /**
         * @brief 释放资源
         *
         * @note 此函数调用后, 需要重新调用IR_init进行初始化后,
         *       才可调用其他函数
         *
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int IR_unit();

        /**
         * TrackerManager
         * 初始化，如果已经初始化过，则应该清空
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int initTrackerManager();


        /**
         * add
         * 根据FileNameOfXML中的描述，从FileNameOfXML.dat中加载tracker
         * @FileNameOfXML: xml文件完整路径
         * @type: 见TrackSourceType
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int addTrackable(string FileNameOfXML, int type);

        /**
         * getNumber
         * 查询目前Tracker的数量
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getTrackableNumber();

        /**
         * getByName
         * 获取名为nameofTracker的Trackable
         * @return: ptr of Trackable
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern IntPtr getTrackableByName(StringBuilder nameofTracker);

        /**
         * getByIndex
         * 获取序号为index的Trackable
         * @return: ptr of Trackable
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern IntPtr getTrackableByIndex(int index);

        /**
         * getFoundTrackerID
         * 查找找到的Tracker的ID，仅在ImageSearch的时候可用
         * @return: -1:未找到任务Tracker, 其他：tracker的ID
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int foundTracker();

        //[DllImport("arsdk")]
        //private static extern void resetTrackerResult();

        /*
         * resetTrackResult
         * 清除Tracker的查找结果, 所有Tracker的都重置为未找到状态
         * 注：此函数只有在明确需要重置之前查找结果的情况下使用
         * 例如
         *      在有查找结果后，明确在一段时间未进行输入图像查找，
         *      又在未重新初始化的情况，需要重新输入图像进行查找，
         *      此时在重新输入图像前需要调用此函数清理查找结果
         */
        //public void resetTrackResult()
        //{
        //resetTrackerResult();
        //}

        /*
            * Clean
            * 清空模板数据
            * @return: 0
            */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int cleanTrackable();

        /**
         * unit
         * 释放环境
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int unitTrackerManager();

#if UNITY_IPHONE
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("arsdk", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern void setOnSearchDone(SearchDoneClient.SearchDoneListener listener);

        #endregion IR Interface---------

        #region MT interface---------
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
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int MT_init(string companyName, string authString);

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
             * @param projectMatrix 投影矩阵
             * @return 0: 成功, 其他: 失败
             */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int MT_SetCameraInfo(float iw, float ih, float fx, float fy, float cx, float cy, int screenW, int screenH, int rotation);

        /**
         * @brief 设置图像信息
         * @param wid 图像的宽度, 同@MT_SetCameraInfo的@iw
         * @param hei 图像的高度, 同@MT_SetCameraInfo的@ih
         * @param fmt 图像格式, 见@alvafmt, 目前仅支持Alva_FMT_YUV_420p
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int MT_setDataInfoMemory(int wid, int hei, int fmt);

        /**
     * @brief 为识别核心更新一帧图像及数据
     * @param data              输入数据指针
     * @param dataMatrix_CMO    数据的位姿, 格式如下:(横屏时为单位阵，竖屏m11和m22为-1)
     *                          m11 m12 m13 m14
     *                          m21 m22 m23 m24
     *                          m31 m32 m33 m34
     *                          m41 m42 m43 m44
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
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern  int MT_Memory(byte[] data,  float[] viewMatrix_CMO, float[] anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus);

#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int MT_MemoryChannel(IntPtr[] data, float[] viewMatrix_CMO, float[] anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus);

        /**
         * @brief 获取一帧将用于渲染的图像数据
         * @param data 输出图像缓冲区指针, 内存由调用者管理
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int MT_GetBackGroundData(IntPtr data);




#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int MT_GetBackGroundDataChannel(IntPtr[] data);





        /**
         * @brief 释放资源
         *
         * @note 此函数调用后, 需要重新调用MT_init进行初始化后,
         *       才可调用其他函数
         *
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int MT_unit();

        /**
         * @brief 在相机暂停、重启时清除内部缓存数据, 以消除残影
         *        应该在相机重启后第一次调用MT_Memory之前调用
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern void MT_CleanFrameData();


        /**
         * @brief 初始化模板管理器,
         *        如果已经初始化过, 则会清空上次初始化
         *
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int initModelManager();

        /**
         * @brief 从@modelPath加载模型
         * @param modelPath     gltf文件完整路径
         * @param configPath    config文件完整路径
         * @param type          文件类型，见Format.h中alvaft
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int addModel(string fileNameOfXML, int type);

        /**
         * @brief 删除库号为@modelIndex的model
         * @param modelIndex model的序号
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int removeModel(int modelIndex);

        /**
         * @brief 删除最所有Model
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int cleanModel();

        /**
         * @brief 查询model总个数
         * @return model总个数
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getModelNumber();

        /**
         * @brief 获取名为@nameOfModel的Model
         * @param nameOfModel 要查询的model的名字, 以'\0'结尾
         * @return NULL: 未找到对应名字的model
         *         其他: ptr of Model
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern IntPtr getModelByName(string nameOfModel);

        /**
         * @brief 获取序号为@index的Model
         * @param index 要查询的model的序号
         * @return NULL: 未找到对应名字的model
         *         其他: ptr of Model
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern IntPtr getModelByIndex(int index);

        /**
         * @brief 释放模板管理器环境
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int unitModelManager();

        /**
         * @brief 获取@model的序号
         * @param model Model指针
         *              由@getModelByIndex或@getModelByName得到
         * @return >=0 @model的序号
         *          <0 @model为空指针
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getModelIndex(IntPtr model);

        /**
         * @brief 获取@model的名称
         * @param model    Model指针
         *                 由@getModelByIndex或@getModelByName得到
         * @param name     大小为@nameSize的输出缓存空间, model的名字
         *                 内存由调用者管理
         * @param nameSize name的空间大小, 单位: byte
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getModelName(IntPtr model, IntPtr name, int nameSize);

        /**
         * @brief 获取@model的状态
         * @param model Model指针
         *              由@getModelByIndex或@getModelByName得到
         * @return @model的状态
         *          0: @model未激活
         *          1: @model已激活
         *       其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getModelStatus(IntPtr model);

        /**
         * @brief 设置@model的状态
         * @param model  Model指针
         *               由@getModelByIndex或@getModelByName得到
         * @param status 目标状态. 0: 未激活, 1: 激活
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int setModelStatus(IntPtr model, int status);

        /**
         * @brief 查询@model的姿态@rts
         * @param model Model指针
         * @param rts   输出, @model的姿态
         *              格式:
         *                  q1 q2 q3 q4 t1 t2 t3 s1 s2 s3
         * @return 0: 成功, 其他: 失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getModelRTS(IntPtr model, float[] rts);

        /**
 * @brief 查询@model的姿态矩阵@matrix
 * @param model  Model指针
 * @param matrix 输出, @model的姿态矩阵
 *               格式:
 *slamView           m11 m12 m13 m14
 *                   m21 m22 m23 m24
 *                   m31 m32 m33 m34
 *                   m41 m42 m43 m44
 *
 *slamModel          m11 m12 m13 m14
 *                   m21 m22 m23 m24
 *                   m31 m32 m33 m34
 *                   m41 m42 m43 m44
 *
 *cadView            m11 m12 m13 m14
 *                   m21 m22 m23 m24
 *                   m31 m32 m33 m34
 *                   m41 m42 m43 m44
 *
 *cadPredict         m11 m12 m13 m14
 *                   m21 m22 m23 m24
 *                   m31 m32 m33 m34
 *                   m41 m42 m43 m44
 *modelCenter        x,y
 * @return 0: 成功, 其他: 失败
 */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getModelPoseMatrix(IntPtr model, float[] matrix);

        /**
         * @brief 查询@model是否被识别到
         * @param model Model指针
         * @return 0: 未被识别到
         *         1: 被识别到
         *      其他: 查询失败
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int isModelFound(IntPtr model);

        /**
         * @brief 清除Model的查找结果, 所有Model都重置为未找到状态
         *
         * @note此函数只有在明确需要重置之前查找结果的情况下使用
         * 例如
         *      在有查找结果后，明确在一段时间未进行输入图像查找，
         *      又在未重新初始化的情况，需要重新输入图像进行查找，
         *      此时在重新输入图像前需要调用此函数清理查找结果
         */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern void resetModelResult();

        /**
         * @brief 图像匹配完成回调接口
         * @param foundIndex 匹配到的序号
         *                   -1: 未匹配到
         *                   其他: 匹配到的模型序号
         * @param frameIndex 帧序号, 目前未实现, 请勿使用
         */
        public delegate void OnMTSearchDone(int foundIndex, int frameIndex);

        /**
         * @brief 设置搜索结果回调函数, 仅在ImageSearch下可用
         * @param pOnMTSearchDone 回调函数指针
         *                        如果传NULL,则清除之前回调函数设置
         *                        在程序结束前必须清空此接口
         * @note 此接口中不要加重负载计算
        
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern void setOnMTSearchDone(OnMTSearchDone pOnMTSearchDone);
         */

        /**
         * @brief 获取必要的日志
         * @param dataBuff 输出空间1024
         
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern void getLogBuffData(string dataBuff);
        */


        /**
     * @brief 计算投影矩阵
     * @param wid                视图宽
     * @param hei                视图高
     * @param viewMatrix_LMO     输入，视角矩阵，16 * sizeof(float)大小空间, 行主序
     * @param projectMatrix_CMO  输出，投影矩阵，16 * sizeof(float)大小空间, 列主序
     * @return 0: 成功, 其他: 失败
     */

#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int computeProjectMatrix(int wid, int hei, float[] viewMatrix, float[] projectMatrix);



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

#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int computeProjectMatrixFixRange(int wid, int hei, float nearV, float farV, float[] projectMatrix_CMO);

        /*
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern void JniInit();
        */


        /**
     * @brief 获取模型初始位姿下的透明图RGBA格式，宽800 高480
     *        在函数addModel之后调用
     * @param model   Model指针
     * @param picData 输出buff 由调用者管理空间（宽*高*4）
     * @return 0: 成功, 其他: 失败
     */

#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getModelGuideImage(IntPtr model, IntPtr picData);

        /**
     * @brief 查询@model的锚点模型位姿
     *        锚点只允许添加一个
     *        在锚点失效情况下，如果可添加锚点，则需删除锚点，重新添加
     * @param model          Model指针
     * @param anchorQT       输出, @model的锚点模型位姿
     *                       格式: q1 q2 q3 q4 t1 t2 t3
     * @isDelete             输出，int值，不等于0时，删除锚点，重新添加锚点
     *
     * @return 0: 成功,可添加锚点  其他: 失败
     */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getAnchorModelQT(IntPtr model, float[] anchorQT, int[] isDelete);


        #endregion MT interface---------

        #endregion external interface..........

        /**
    * yuv数据的缩放操作
    *
    * @param i420Src   i420原始数据
    * @param width     原始宽度
    * @param height    原始高度
    * @param i420Dst   i420目标数据
    * @param dstWidth  目标宽度
    * @param dstHeight 目标高度
    * @param mode      压缩模式 ，0~3，质量由低到高，一般传入0
    
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("yuvutil")]
#endif
        private static extern void scaleI420(byte[] i420Src, int width, int height, byte[] i420Dst, int dstWidth, int dstHeight, int mode);
        */

        /**
     * yuv数据的旋转操作
     *
     * @param i420Src i420原始数据
     * @param width
     * @param height
     * @param i420Dst i420目标数据
     * @param degree  旋转角度
     
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("yuvutil")]
#endif
        private static extern void rotateI420(byte[] i420Src, int width, int height, byte[] i420Dst, int degree);
        */

        /*
    * @brief 获取@trackable对应虚拟Mark的大小
    * @param trackable Trackable指针
    * @param width     输出, 虚拟模型宽
    * @param height    输出, 虚拟模型高
    * @return 0: 成功, 其他: 失败
    * @note 此函数必须在addTrackable之后调用
    */
#if UNITY_IPHONE
        [DllImport("__Internal")]
#else
        [DllImport("arsdk")]
#endif
        private static extern int getTrackVirtualMarkSize(IntPtr trackable, float[] width, float[] height);
    }



}
