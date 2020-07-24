namespace ShipDock.UI
{
    public class UIStack : IUIStack
    {
        public UIStack()
        {
        }

        public virtual void Init()
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit(bool isDestroy)
        {
            IsExited = true;
        }

        public virtual void Interrupt()
        {
        }

        public virtual void Renew()
        {
        }

        public virtual void ResetAdvance()
        {
            IsStackAdvanced = false;
        }

        public virtual void StackAdvance()
        {
            IsStackAdvanced = true;
        }

        public bool IsExited { get; private set; }
        public bool IsStackAdvanced { get; private set; }
        public virtual string UIName { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual bool IsStackable { get; } = true;
    }
}