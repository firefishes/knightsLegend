using System;

namespace ShipDock.Datas
{
    [Serializable]
    public class ClientLocalInfo
    {
        /// <summary>是否已初始化</summary>
        public bool isInited;
        /// <summary>是否已注册</summary>
        public bool isRegistered;
        /// <summary>用户id</summary>
        public string account_id;
        /// <summary>客户端id</summary>
        public string client_account_id;
        /// <summary>背景音量</summary>
        public float volumnBGM = 1f;
        /// <summary>音效音量</summary>
        public float volumnSound = 1f;

        public bool isNewUser;

        /// <summary>
        /// 检测后续增补的字段信息
        /// </summary>
        public virtual void CheckInfoPatch() { }
    }
}
