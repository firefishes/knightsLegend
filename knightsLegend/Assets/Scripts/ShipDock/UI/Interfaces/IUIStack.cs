namespace ShipDock.UI
{
    /// <summary>UI堆栈接口</summary>
    public interface IUIStack
    {
        void Init();//初始化
        void Enter();//开启
        void Interrupt();//中断
        void StackAdvance();//标记为栈提前
        void ResetAdvance();//重置栈提前标记
        void Renew();//唤醒
        void Exit(bool isDestroy);//退出
        bool IsExited { get; }//是否已退出
        bool IsStackAdvanced { get; }//是否已标记为栈提前
        string UIAssetName { get; }//在资源包中的名称
        string Name { get; }//模块名（栈名）
        bool IsStackable { get; }//是否用栈管理
    }
}