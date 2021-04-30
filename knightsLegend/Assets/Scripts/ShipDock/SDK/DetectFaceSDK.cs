using ShipDock.Notices;
using System;
using System.Threading;
using UnityEngine;

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// <-请描述该类的功能->
    /// 
    /// Created by 田亚宗 on 2019/12/06.
    ///
    /// </summary>
    public sealed class DetectFaceSDK : IDetectFaceSDK, IPlatformSDK
    {
        private bool mIsDone;
        private bool mIsWaiting;
        private string mServerResult;
#if TENCENT
        private Credential mCredential;
#endif
        private IDFServerResult mResult;
        private Action<IDFServerResult> onGetResult;
        void IPlatformSDK.Init()
        {
#if TENCENT
            mCredential = new Credential
            {
                SecretId = "AKID1pNgkXqgspVxA7jOnwsX8SWJ4nKbaLp2",
                SecretKey = "9brqY0C7mBSWNyINDn9OQCiz0mIYikaA"
            };
#endif
        }

        void IDetectFaceSDK.DetectFace(byte[] texData, Action<IDFServerResult> getResult, bool isNeedFaceAttribute, bool isNeedQualityDetect)
        {
            if (texData == null)
            {
                return;
            }
            IDFRequestParam param = DFRequestParam.Default;
            param.Image = texData;
            param.NeedFaceAttributes = isNeedFaceAttribute;
            param.NeedQualityDetection = isNeedQualityDetect;
            (this as IDetectFaceSDK).DetectFace(param, getResult);
        }

        void IDetectFaceSDK.DetectFace(Texture2D texture, Action<IDFServerResult> getResult, bool isNeedFaceAttribute = false, bool isNeedQualityDetect = false)
        {
            if (texture == null)
            {
                return;
            }
           (this as IDetectFaceSDK).DetectFace(texture.EncodeToJPG(), getResult, isNeedFaceAttribute, isNeedQualityDetect);
        }

        void IDetectFaceSDK.DetectFace(IDFRequestParam parm, Action<IDFServerResult> getResult)
        {
            if (mIsWaiting)
            {
                return;
            }
#if SHIP_DOCK_SDK
            JSONObject json = JSONObject.Create();
            json.AddField("MaxFaceNum", parm.MaxFaceNumber);
            json.AddField("MinFaceSize", parm.MinFaceSize);
            if (parm.Image != null)
            {
                json.AddField("Image", Convert.ToBase64String(parm.Image));
            }
            if (!string.IsNullOrEmpty(parm.Url))
            {
                json.AddField("Url", parm.Url);
            }
            json.AddField("NeedFaceAttributes", parm.NeedFaceAttributes ? 1 : 0);
            json.AddField("NeedQualityDetection", parm.NeedQualityDetection ? 1 : 0);
            json.AddField("FaceModelVersion", parm.FaceModelVersion);
            onGetResult = getResult;
            mIsDone = false;

            UpdaterNotice.SceneCallLater(WaitServerResult);

            if (!ThreadPool.QueueUserWorkItem(SendRequest, json))
            {
                mIsWaiting = false;
                throw new Exception("IDetectFaceSDK--> request server failed!");
            }
            else
            {
                mIsWaiting = true;
            }
#endif
        }

        private void SendRequest(object state)
        {
            mServerResult = string.Empty;
            try
            {
#if TENCENT
                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("iai.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;
                IaiClient client = new IaiClient(mCredential, "", clientProfile);
                DetectFaceRequest req = DetectFaceRequest.FromJsonString<DetectFaceRequest>(state.ToString());
                DetectFaceResponse resp = client.DetectFace(req).ConfigureAwait(false).GetAwaiter().GetResult();
                mServerResult = AbstractModel.ToJsonString(resp);
                mResult = JsonUtility.FromJson<DFServerResult>(mServerResult);
#endif
                mIsDone = true;
            }
            catch (System.Exception ex)
            {
                mServerResult = ex.Message;
                string[] sf = mServerResult.Split(' ');
                DFServerResult rs = new DFServerResult();

                rs.Error = new DFError();
                rs.Error.Code = sf[0].Split(':')[1];
                rs.Error.Message = sf[1].Split(':')[1];
                mResult = rs;
                mIsDone = true;
            }
        }

        private void WaitServerResult(int t)
        {
            if (mIsDone)
            {
                onGetResult?.Invoke(mResult);
                onGetResult = null;
                mResult = null;
                mIsWaiting = false;
            }
            else
            {
                UpdaterNotice.SceneCallLater(WaitServerResult);
            }
        }
    }

    public class DFRequestParam : IDFRequestParam
    {
        int IDFRequestParam.MaxFaceNumber { get; set; } = 1;
        int IDFRequestParam.MinFaceSize { get; set; } = 40;
        byte[] IDFRequestParam.Image { get; set; } 
        string IDFRequestParam.Url { get; set; }
        bool IDFRequestParam.NeedFaceAttributes { get; set; } = false;
        bool IDFRequestParam.NeedQualityDetection { get; set; } = false;
        string IDFRequestParam.FaceModelVersion => "3.0";

        public static DFRequestParam Default
        {
            get
            {
                return new DFRequestParam();
            }
        }
    }

    [Serializable]
    public class DFServerResult : IDFServerResult
    {
        public int ImageWidth;
        public int ImageHeight;
        public string RequestId;
        public string FaceModelVersion;
        public FaceInfos[] FaceInfos;
        public DFError Error;
        int IDFServerResult.ImageWidth => ImageWidth;

        int IDFServerResult.ImageHeight => ImageHeight;

        string IDFServerResult.FaceModelVersion => FaceModelVersion;

        string IDFServerResult.RequestId => RequestId;

        IFaceInfo[] IDFServerResult.FaceInfos => FaceInfos;

        IDFError IDFServerResult.Error => Error;
    }

    [Serializable]
    public class FaceInfos : IFaceInfo
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public FaceAttributesInfo FaceAttributesInfo;
        public FaceQualityInfo FaceQualityInfo;

        int IFaceInfo.X => X;

        int IFaceInfo.Y => Y;

        int IFaceInfo.Width => Width;

        int IFaceInfo.Height => Height;

        IFaceAttributesInfo IFaceInfo.FaceAttributesInfo => FaceAttributesInfo;

        IFaceQualityInfo IFaceInfo.FaceQualityInfo => FaceQualityInfo;
    }

    [Serializable]
    public class DFError : IDFError
    {
        public string Code;
        public string Message;

        string IDFError.Code => Code;

        string IDFError.Message => Message;
    }

    [Serializable]
    public class FaceAttributesInfo : IFaceAttributesInfo
    {
        public int Gender;
        public int Age;
        public int Expression;
        public bool Hat;
        public bool Glass;
        public bool Mask;
        public Hair Hair;
        public int Pitch;
        public int Yaw;
        public int Roll;
        public int Beauty;
        public bool EyeOpen;

        int IFaceAttributesInfo.Expression => Expression;

        bool IFaceAttributesInfo.Hat => Hat;

        bool IFaceAttributesInfo.Glass => Glass;

        bool IFaceAttributesInfo.Mask => Mask;

        IHair IFaceAttributesInfo.Hair => Hair;

        int IFaceAttributesInfo.Pitch => Pitch;

        int IFaceAttributesInfo.Yaw => Yaw;

        int IFaceAttributesInfo.Roll => Roll;

        int IFaceAttributesInfo.Beauty => Beauty;

        bool IFaceAttributesInfo.EyeOpen => EyeOpen;

        int IFaceAttributesInfo.Gender => Gender;

        int IFaceAttributesInfo.Age => Age;
    }

    [Serializable]
    public class Hair : IHair
    {
        public int Length;
        public int Bang;
        public int Color;

        int IHair.Length => Length;

        int IHair.Bang => Bang;

        int IHair.Color => Color;
    }

    [Serializable]
    public class FaceQualityInfo : IFaceQualityInfo
    {
        public int Score;
        public int Sharpness;
        public int Brightness;
        public Completeness Completeness;

        int IFaceQualityInfo.Score => Score;

        int IFaceQualityInfo.Sharpness => Sharpness;

        int IFaceQualityInfo.Brightness => Brightness;

        ICompleteness IFaceQualityInfo.Completeness => Completeness;
    }

    [Serializable]
    public class Completeness : ICompleteness
    {
        public int Eyebrow;
        public int Eye;
        public int Nose;
        public int Cheek;
        public int Mouth;
        public int Chin;

        int ICompleteness.Eyebrow => Eyebrow;

        int ICompleteness.Eye => Eye;

        int ICompleteness.Nose => Nose;

        int ICompleteness.Cheek => Cheek;

        int ICompleteness.Mouth => Mouth;

        int ICompleteness.Chin => Chin;
    }
}
