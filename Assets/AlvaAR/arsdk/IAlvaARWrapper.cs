using System;
using System.Text;
using System.Runtime.InteropServices;
using static arsdk.AlvaARUnity;

namespace arsdk
{
    public interface IAlvaARWrapper
    {
        int InitIR(AlvaARUnity.AlvaDataType dataType, string companyName, string authString);

        int UnitIR();

        int SetDataInfoIRMemory(int wid, int hei, int fmt);

        int IRMemory(byte[] data, float[] coord, float[] dataMatrix, float[] viewMatrix);

        int GetBackGroundData(byte[] data);

        void InitTrackerManager();

        int UnitTrackerManager();

        void TrackerManagerOnSearchDone(SearchDoneClient.SearchDoneListener callbackPointer);

        int TrackerManagerCleanTracker();

        int TrackerManagerAddXML(string FileNameOfXML, AlvaARUnity.AlvaFileType type);

        int TrackerManagerGetNumber();

        IntPtr TrackerManagerGetByName(StringBuilder nameofTracker);

        IntPtr TrackerManagerGetByIndex(int index);

        int TrackerManagerOfFoundTrackerID();

        int InitMT(string companyName, string authString);

        int SetMTCameraInfo(float iw, float ih, float fx, float fy, float cx, float cy, int screenW, int screenH, AlvaRotation rotation);

        int SetMTDataInfoMemory(int wid, int hei, AlvaARUnity.AlvaFormat fmt);

        int MTMemory(byte[] data, float[] viewMatrix_CMO, float[] anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus);

        //int MTMemoryChannel(ref byte[] data, float[] dataMatrix, float[] viewMatrix, int camStatus);
        int MTMemoryChannel(IntPtr[] data, float[] viewMatrix_CMO, float[] anchorMatrix_CMO, int camStatus, int anchorIndex, int anchorStatus);


        int GetMTBackGroundDataChannel(IntPtr[] data);

        int GetMTBackGroundData(IntPtr data);

        int UnitMT();

        void CleanMTFrameData();

        int InitModelManager();

        int AddModel(string fileNameOfXML);

        int RemoveModel(int modelIndex);

        int CleanModel();

        int GetModelNumber();

        IntPtr GetModelByName(string nameOfModel);

        IntPtr GetModelByIndex(int index);

        int UnitModelManager();

        int GetModelIndex(IntPtr model);

        int GetModelName(IntPtr model, IntPtr name, int nameSize);

        int GetModelStatus(IntPtr model);

        int SetModelStatus(IntPtr model, int status);

        int GetModelRTS(IntPtr model, float[] rts);

        int GetModelPoseMatrix(IntPtr model, float[] matrix);

        int IsModelFound(IntPtr model);

        void ResetModelResult();

        void GetLogBuffData(string dataBuff);

        int ComputeProjectMatrix(int wid, int hei, float[] viewMatrix, float[] projectMatrix);

        int ComputeProjectMatrixFixRange(int wid, int hei, float nearV, float farV, float[] projectMatrix_CMO);

        int AddTrackable(string FileNameOfXML, int type);

        int SetTrackableStatus(long trackable, int status);

        void YuvScaleI420(byte[] i420Src, int width, int height, byte[] i420Dst, int dstWidth, int dstHeight, int mode);

        int GetFoundNumIndexs(int[] oFoundNum, int[] oFoundIndexs);
        //int GetTrackerRTS(long trackable, float[] rts);
        int GetTrackerPoseMatrix(long trackable, float[] matrix);
        void YuvRotateI420(byte[] i420Src, int width, int height, byte[] i420Dst, int degree);
        int GetAnchorModelQT(IntPtr model, float[] anchorQT, int[] isDelete);

        int GetTrackVirtualMarkSize(IntPtr trackable, float[] width, float[] height);
    }
}