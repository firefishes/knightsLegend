
//#define DEV_ANDROID
//#define DEV_IOS

#pragma warning disable
namespace Greencheng.Applications.SDK
{

    /// <summary>
    /// 
    /// 设备声音SDK
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
#if UNITY_ANDROID && !DEV_IOS

    using UnityEngine;

    public class SDKDeviceAudio
    {
        private const string currentVolume = "getStreamVolume";//当前音量
        private const string maxVolume = "getStreamMaxVolume";//最大音量

        private const int STREAM_VOICE_CALL = 0;// 通话音量
        private const int STREAM_SYSTEM = 1;// 系统音量
        private const int STREAM_RING = 2;// 铃声音量
        private const int STREAM_MUSIC = 3;// 媒体音量
        private const int STREAM_ALARM = 4;// 警报音量 
        private const int STREAM_NOTIFICATION = 5;// 窗口顶部状态栏 Notification
        private const int STREAM_DTMF = 8;// 双音多频
        private const int ADJUST_LOWER = 9;// 降低音量

        private AndroidJavaObject mSystemService;
        private AudioInfos mAudioInfos = new AudioInfos();

        public void InitSdk()
        {
            if(Application.platform == RuntimePlatform.Android)
            {
                //mSystemService = Call<AndroidJavaObject>("getSystemService", new AndroidJavaObject("java.lang.String", "audio"));
            }
        }

        //protected override void Purge()
        //{
        //    base.Purge();

        //    mSystemService = null;
        //}

        public void AsyncDeviceVoice()
        {
#if !UNITY_EDITOR || DEV_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                mAudioInfos.VoiceCall = mSystemService.Call<int>(currentVolume, STREAM_VOICE_CALL);
                mAudioInfos.VoiceSystem = mSystemService.Call<int>(currentVolume, STREAM_SYSTEM);
                mAudioInfos.VoiceRing = mSystemService.Call<int>(currentVolume, STREAM_RING);
                mAudioInfos.VoiceMusic = mSystemService.Call<int>(currentVolume, STREAM_MUSIC);
                mAudioInfos.VoiceAleam = mSystemService.Call<int>(currentVolume, STREAM_ALARM);
                mAudioInfos.VoiceNotification = mSystemService.Call<int>(currentVolume, STREAM_NOTIFICATION);
                mAudioInfos.VoiceDTMF = mSystemService.Call<int>(currentVolume, STREAM_DTMF);

                mAudioInfos.VoiceMaxCall = mSystemService.Call<int>(maxVolume, STREAM_VOICE_CALL);
                mAudioInfos.VoiceMaxSystem = mSystemService.Call<int>(maxVolume, STREAM_SYSTEM);
                mAudioInfos.VoiceMaxRing = mSystemService.Call<int>(maxVolume, STREAM_RING);
                mAudioInfos.VoiceMaxMusic = mSystemService.Call<int>(maxVolume, STREAM_MUSIC);
                mAudioInfos.VoiceMaxAleam = mSystemService.Call<int>(maxVolume, STREAM_ALARM);
                mAudioInfos.VoiceMaxNotification = mSystemService.Call<int>(maxVolume, STREAM_NOTIFICATION);
                mAudioInfos.VoiceMaxDTMF = mSystemService.Call<int>(maxVolume, STREAM_DTMF);
            }
#else
            mAudioInfos.VoiceCall = 3;
            mAudioInfos.VoiceMaxCall = 10;
#endif
        }

        //public override string SdkName
        //{
        //    get
        //    {
        //        return nameof(SDKDeviceAudio);
        //    }
        //}

        public bool IsVoiceTooLow
        {
            get
            {
                AsyncDeviceVoice();
                return (0.3f >= (mAudioInfos.VoiceCall / mAudioInfos.VoiceMaxCall));
            }
        }
    }
#endif

#if !UNITY_ANDROID || DEV_IOS
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class SDKDeviceAudio : IPlatformSDK
    {
        public const string NAME_DEVICE_AUDIO_SDK = "DeviceAudioSDK";

        //[DllImport("__Internal")]
        //static extern int GetVoiceCallFromIOS();

        private AudioInfos mAudioInfos = new AudioInfos();

        public SDKDeviceAudio()
        {
        }

        public void AsyncDeviceVoice()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //mAudioInfos.VoiceCall = GetVoiceCallFromIOS();
            }
            else
            {
                //mAudioInfos.VoiceCall = 3;
                //mAudioInfos.VoiceMaxCall = 10;
            }
        }

        public void Init()
        {

        }

        public string SdkName
        {
            get
            {
                return NAME_DEVICE_AUDIO_SDK;
            }
        }

        public bool IsVoiceTooLow
        {
            get
            {
                AsyncDeviceVoice();
                return (0.3f >= mAudioInfos.VoiceCall);
            }
        }
    }
#endif

    public struct AudioInfos
    {
        public int VoiceCall { get; set; }
        public int VoiceSystem { get; set; }
        public int VoiceRing { get; set; }
        public int VoiceMusic { get; set; }
        public int VoiceAleam { get; set; }
        public int VoiceNotification { get; set; }
        public int VoiceDTMF { get; set; }

        public int VoiceMaxCall { get; set; }
        public int VoiceMaxSystem { get; set; }
        public int VoiceMaxRing { get; set; }
        public int VoiceMaxMusic { get; set; }
        public int VoiceMaxAleam { get; set; }
        public int VoiceMaxNotification { get; set; }
        public int VoiceMaxDTMF { get; set; }
    }
}