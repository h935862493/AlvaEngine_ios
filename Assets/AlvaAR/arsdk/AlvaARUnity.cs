using System.Text;
using UnityEngine;

namespace arsdk
{
    /****************************************************************
     *Copyright(C) 2017 by #AlvaSystems@alva.com.cn# All rights reserved. 
     *FileName:     #ARUnity.cs# 
     *Author:       #AUTHOR# 
     *Version:      #VERSION# 
     *UnityVersion：#UNITYVERSION# 
     *Date:         #2017-12-25# 
     *Description:   AR Unity常量
     *History: 
    *****************************************************************/
    public class AlvaARUnity
    {
        /// <summary>
        /// 720 * 1280的分辨率
        /// </summary>
        public const int ScreenWidth = 720;
        public const int ScreenHeight = 1280;


        /// <summary>
        /// 输入数据类型
        /// </summary>
        public enum AlvaDataType
        {
            Alva_Memory = 0, //后续输入为内存
            Alva_Texture = 1, //后续输入为OpenGL纹理
        }


        /// <summary>
        /// 输入路径的指向类型
        /// </summary>
        public enum AlvaFileType
        {
            Alva_Asset = 0, //Android的Asset文件（仅限Android平台）
            Alva_File = 1, //存储器中的文件
        }


        public enum AlvaRotation
        {
            Alva_ROTATION_0 = 0, //  0度, 竖屏
            Alva_ROTATION_90 = 1, // 90度, 左横屏
            Alva_ROTATION_180 = 2, //180度, 反向竖屏
            Alva_ROTATION_270 = 3, //270度, 右横屏
        }


        // 摘要:
        //     初始化错误
        public enum ErrorCode
        {
            INIT_SUCCESS = 0,
            INIT_ERROR,
            INIT_LICENSE_ERROR_KEY_INVALID = 0x8001,
            INIT_LICENSE_ERROR_COMPANYNAME_NOT_CORRECT = 0x8003,
            INIT_LICENSE_ERROR_PACKAGENAME_NOT_CORRECT = 0x8004,
            INIT_LICENSE_ERROR_MACHINE_CODE_NOT_CORRECT = 0x8005,
            INIT_LICENSE_ERROR_MACHINECODE_NOT_CORRECT2 = 0x8006,
            INIT_LICENSE_ERROR_SYSTEMTIME_NOT_CORRECT = 0x8007,
            INIT_LICENSE_ERROR_KEY_OVERDUE = 0x8008,
            INIT_LICENSE_ERROR_NOT_FOR_APP = 0x8009,
            INIT_LICENSE_ERROR_NOT_FOR_APP_MODULE = 0x80A1,
            INIT_LICENSE_ERROR_Android_IMEI_Error1 = 0x80E1,
            INIT_LICENSE_ERROR_Android_IMEI_Error2 = 0x80E2,
            INIT_LICENSE_ERROR_Android_IMEI_Error3 = 0x80E3,
            INIT_LICENSE_ERROR_Android_IMEI_Error4 = 0x80E4,
            INIT_LICENSE_ERROR_Android_IMEI_Error5 = 0x80E5,
            INIT_LICENSE_ERROR_Android_IMEI_Error6 = 0x80E6,
            INIT_LICENSE_ERROR_Android_IMEI_Error7 = 0x80E7,

        }

        /// <summary>
        /// 图像格式
        /// 
        /// I420: YYYYYYYY UU VV    =>YUV420P

        /// YV12: YYYYYYYY VV UU    =>YUV420P

        /// NV12: YYYYYYYY UVUV     => YUV420SP

        /// NV21: YYYYYYYY VUVU     => YUV420SP


        /// </summary>
        public enum AlvaFormat
        {
            Alva_FMT_Start = -1,
            Alva_FMT_RGBA_4444,//RGBARGBARGBA(4444)
            Alva_FMT_BGRA_4444,//BGRABGRABGRA(4444)
            Alva_FMT_RGBA_8888,//RGBARGBARGBA(8888)
            Alva_FMT_BGRA_8888,//BGRABGRABGRA(8888)
            Alva_FMT_GRAY,//GGGG(gray), (R + G + B) / 3
            Alva_FMT_RG_88,//RGRGRG(88)
            Alva_FMT_RGB_332,//RGBRGB(332)
            Alva_FMT_BGR_233,//BGRBGR(332)
            Alva_FMT_RGB_565,//RGBRGB(565)
            Alva_FMT_BGR_565,//BGRBGR(565)
            Alva_FMT_RGB_555,//RGBRGB(555)
            Alva_FMT_BGR_555,//BGRBGR(555)
            Alva_FMT_RGB_888,//RGBRGBRGB(888)
            Alva_FMT_BGR_888,//BGRBGRBGR(888)
            Alva_FMT_YUV_420p,//Y(w*h) + U(w*h/4) + V(w*h/4) 
            Alva_FMT_I420 = Alva_FMT_YUV_420p,
            Alva_FMT_YU12 = Alva_FMT_YUV_420p,
            Alva_FMT_YV12,//Y(w*h) + V(w*h/4) + U(w*h/4)
            Alva_FMT_YUV_422p,//Y(w*h) + U(w*h/2) + V(w*h/2)
            Alva_FMT_YUV_420sp,//Y(w*h) + UV(w*h/4)
            Alva_FMT_NV12 = Alva_FMT_YUV_420sp,
            Alva_FMT_YUV_422sp,//Y(w*h) + UV(w*h/2)
            Alva_FMT_YVU_420sp,//Y(w*h) + VU(w*h/4)
            Alva_FMT_NV21 = Alva_FMT_YVU_420sp,
            Alva_FMT_YVU_422sp,//Y(w*h) + VU(w*h/2)

            Alva_FMT_YUVY,//YUV422, YUYVYUYV
            Alva_FMT_UYVY,//YUV422, YVYUYVYU

            Alva_FMT_Y,//YYYY(单独Y通道，可作为另一种GRAY)

            Alva_FMT_MultiDim,//P1P2P3...PNP1P2P3...PNP1P2P3...PN

            Alva_FMT_NUMBER
        }



        /// <summary>
        /// 输入数据的Camera朝向
        /// </summary>
        public enum AlvaCameraFacing
        {
            Alva_BACK = 0, //后置摄像头
            Alva_FRONT = 1, //前置摄像头
        }

        /// <summary>
        /// 输入相机数据的ar状态
        /// </summary>
        public enum AlvaCamARStatus_
        {
            Alva_ARTRACKING = 0, //tracking
            Alva_ARPAUSED = 1, //paused tracking
            Alva_ARSTOPPED = 2, //stopped tracking
        }

        public static string GetCString(string str)
        {
            //Debug.Log("zjh 111");
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.UTF8;
            //Debug.Log("zjh 222");
            // Convert the string into a byte[].
            byte[] unicodeBytes = unicode.GetBytes(str);
           // Debug.Log("zjh 333");
            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
           // Debug.Log("zjh 444");
            // Convert the new byte[] into a char[] and then into a string.
            // This is a slightly different approach to converting to illustrate
            // the use of GetCharCount/GetChars.
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
           // Debug.Log("zjh 555");
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
           // Debug.Log("zjh 666");
            string newString = new string(asciiChars);
           // Debug.Log("zjh 777");
            return newString;
        }
    }
}