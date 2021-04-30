using System;
using UnityEngine;

#pragma warning disable
namespace ShipDock.SDK
{
	/// <summary>
    /// 
    /// <-请描述该类的功能->
    /// 
    /// Created by 田亚宗 on 2019/12/05.
    ///
    /// </summary>
	public interface IDetectFaceSDK
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texData">需要检测的图片数据</param>
        /// <param name="getResult">检测结果</param>
        /// <param name="isNeedFaceAttribute">是否需要人脸属性</param>
        /// <param name="isNeedQualityDetect">是否需要图片质量检测</param>
        void DetectFace(byte[] texData, Action<IDFServerResult> getResult, bool isNeedFaceAttribute = false, bool isNeedQualityDetect = false);
        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="texture">需要检测的图片</param>
        /// <param name="getResult">检测结果</param>
        /// <param name="isNeedFaceAttribute">是否需要人脸属性</param>
        /// <param name="isNeedQualityDetect">是否需要图片质量检测</param>
        void DetectFace(Texture2D texture,Action<IDFServerResult>getResult, bool isNeedFaceAttribute = false, bool isNeedQualityDetect = false);
        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="parm">人脸检测参数</param>
        /// <param name="getResult">检测结果</param>
        void DetectFace(IDFRequestParam parm,Action<IDFServerResult> getResult);
	}

    public interface IDFRequestParam
    {
        /// <summary>
        /// 最多处理的人脸数目。默认值为1（仅检测图片中面积最大的那张人脸），最大值为120。此参数用于控制处理待检测图片中的人脸个数，值越小，处理速度越快。
        /// </summary>
        int MaxFaceNumber { get; set; }
        /// <summary>
        /// 人脸长和宽的最小尺寸，单位为像素。默认为40。建议不低于34。低于MinFaceSize值的人脸不会被检测。
        /// </summary>
        int MinFaceSize { get; set; }
        /// <summary>
        /// 需要检测的图片数据。支持PNG、JPG、JPEG、BMP，不支持 GIF 图片。
        /// </summary>
        byte[] Image { get; set; }
        /// <summary>
        ///图片的 Url
        /// </summary>
        string Url { get; set; }
        /// <summary>
        /// 是否需要返回人脸属性信息,最多返回面积最大的 5 张人脸属性信息，超过 5 张人脸（第 6 张及以后的人脸）的 FaceAttributesInfo 不具备参考意义。
        /// </summary>
        bool NeedFaceAttributes { get; set; }
        /// <summary>
        /// 是否开启质量检测。最多返回面积最大的 30 张人脸质量分信息，超过 30 张人脸（第 31 张及以后的人脸）的 FaceQualityInfo不具备参考意义。
        /// </summary>
        bool NeedQualityDetection { get; set; }
        /// <summary>
        /// 人脸识别服务所用的算法模型版本
        /// </summary>
        string FaceModelVersion { get; }
    }

    public interface IDFServerResult
    {
        /// <summary>
        /// 请求的图片宽度
        /// </summary>
        int ImageWidth { get; }
        /// <summary>
        /// 请求的图片高度
        /// </summary>
        int ImageHeight { get; }
        /// <summary>
        /// 人脸识别所用的算法模型版本
        /// </summary>
        string FaceModelVersion { get; }
        /// <summary>
        /// 唯一请求 ID，每次请求都会返回。定位问题时需要提供该次请求的 RequestId
        /// </summary>
        string RequestId { get; }
        IDFError Error { get; }
        /// <summary>
        /// 人脸信息列表
        /// </summary>
        IFaceInfo[] FaceInfos { get; }
    }

    public interface IFaceInfo
    {
        /// <summary>
        /// 人脸框左上角横坐标
        /// </summary>
        int X { get; }
        /// <summary>
        /// 人脸框左上角纵坐标
        /// </summary>
        int Y { get; }
        /// <summary>
        /// 人脸框宽度
        /// </summary>
        int Width { get; }
        /// <summary>
        /// 人脸框高度
        /// </summary>
        int Height { get; }
        /// <summary>
        /// 人脸属性信息
        /// </summary>
        IFaceAttributesInfo FaceAttributesInfo { get; }
        /// <summary>
        /// 人脸质量信息
        /// </summary>
        IFaceQualityInfo FaceQualityInfo { get; }
    }

    public interface IFaceAttributesInfo
    {
        /// <summary>
        /// 性别[0~49]为女性，[50，100]为男性，越接近0和100表示置信度越高
        /// </summary>
        int Gender { get; }
        /// <summary>
        /// 年龄 [0~100]。NeedFaceAttributes 不为1 或检测超过 5 张人脸时，此参数仍返回，但不具备参考意义
        /// </summary>
        int Age { get; }
        /// <summary>
        /// 微笑[0(normal，正常)~50(smile，微笑)~100(laugh，大笑)]
        /// </summary>
        int Expression { get; }
        /// <summary>
        /// 是否有帽子 [true,false]
        /// </summary>
        bool Hat { get; }
        /// <summary>
        /// 是否有眼镜 [true,false]
        /// </summary>
        bool Glass { get; }
        /// <summary>
        /// 是否有口罩 [true,false]
        /// </summary>
        bool Mask { get; }
        /// <summary>
        /// 头发信息，包含头发长度（length）、有无刘海（bang）、头发颜色（color）
        /// </summary>
        IHair Hair { get; }
        /// <summary>
        /// 上下偏移[-30,30]，单位角度
        /// </summary>
        int Pitch { get; }
        /// <summary>
        /// 左右偏移[-30,30]，单位角度
        /// </summary>
        int Yaw { get; }
        /// <summary>
        /// 平面旋转[-180,180]，单位角度
        /// </summary>
        int Roll { get; }
        /// <summary>
        /// 魅力[0~100]
        /// </summary>
        int Beauty { get; }
        /// <summary>
        /// 双眼是否睁开 [true,false]。只要有超过一只眼睛闭眼，就返回false
        /// </summary>
        bool EyeOpen { get; }
    }

    public interface IHair
    {
        /// <summary>
        /// 0：光头，1：短发，2：中发，3：长发，4：绑发
        /// </summary>
        int Length { get; }
        /// <summary>
        /// 0：有刘海，1：无刘海
        /// </summary>
        int Bang { get; }
        /// <summary>
        /// 0：黑色，1：金色，2：棕色，3：灰白色
        /// </summary>
        int Color { get; }
    }

    public interface IFaceQualityInfo
    {
        /// <summary>
        /// 质量分: [0,100]，综合评价图像质量是否适合人脸识别，分数越高质量越好,参考范围：[0,40]较差，[40,60] 一般，[60,80]较好，[80,100]很好
        /// </summary>
        int Score { get; }
        /// <summary>
        /// 清晰分：[0,100]，评价图片清晰程度，分数越高越清晰。参考范围：[0,40]特别模糊，[40,60]模糊，[60,80]一般，[80,100]清晰
        /// </summary>
        int Sharpness { get; }
        /// <summary>
        /// 光照分：[0,100]，评价图片光照程度，分数越高越亮,参考范围： [0,30]偏暗，[30,70]光照正常，[70,100]偏亮
        /// </summary>
        int Brightness { get; }
        /// <summary>
        /// 五官遮挡分，评价眉毛（Eyebrow）、眼睛（Eye）、鼻子（Nose）、脸颊（Cheek）、嘴巴（Mouth）、下巴（Chin）的被遮挡程度
        /// </summary>
        ICompleteness Completeness { get;}
    }

    /// <summary>
    /// 五官遮挡分，评价眉毛（Eyebrow）、眼睛（Eye）、鼻子（Nose）、脸颊（Cheek）、嘴巴（Mouth）、下巴（Chin）的被遮挡程度。
    /// </summary>
    public interface ICompleteness
    {
        /// <summary>
        /// 眉毛的遮挡分数[0,100]，分数越高遮挡越少,参考范围：[0,80]表示发生遮挡
        /// </summary>
        int Eyebrow { get; }
        /// <summary>
        /// 眼睛的遮挡分数[0,100],分数越高遮挡越少。参考范围：[0,80]表示发生遮挡
        /// </summary>
        int Eye { get; }
        /// <summary>
        /// 鼻子的遮挡分数[0,100],分数越高遮挡越少。参考范围：[0,60]表示发生遮挡
        /// </summary>
        int Nose { get; }
        /// <summary>
        /// 脸颊的遮挡分数[0,100],分数越高遮挡越少。参考范围：[0,70]表示发生遮挡
        /// </summary>
        int Cheek { get; }
        /// <summary>
        /// 嘴巴的遮挡分数[0,100],分数越高遮挡越少,参考范围：[0,50]表示发生遮挡
        /// </summary>
        int Mouth { get; }
        /// <summary>
        /// 下巴的遮挡分数[0,100],分数越高遮挡越少,参考范围：[0,70]表示发生遮挡
        /// </summary>
        int Chin { get; }
    }

    public interface IDFError
    {
        string Code { get; }
        string Message { get; }
    }

}

