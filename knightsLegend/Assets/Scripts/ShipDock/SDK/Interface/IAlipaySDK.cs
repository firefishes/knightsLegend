using UnityEngine.Events;

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// 
    /// 
    /// Created by 田亚宗 on 2019/05/23.
    ///
    /// </summary>
    public interface IAlipaySDK 
	{
        bool IsAppInstalled();
        void SendPayRequest(string payOrder, UnityAction<string> callback);
	}	
}

