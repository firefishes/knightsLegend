namespace ShipDock.UI
{
    public interface IUIStack
    {
        void Init();
        void Enter();
        void Interrupt();
        void StackAdvance();
        void ResetAdvance();
        void Renew();
        void Exit();
        bool IsExited { get; }
        bool IsStackAdvanced { get; }
        string UIName { get; }
        string Name { get; }
    }
}