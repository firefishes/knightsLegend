namespace ShipDock.UI
{
    public class UIStack : IUIStack
    {
        public virtual void Init()
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit(bool isDestroy)
        {
            if (isDestroy)
            {
                IsExited = true;
            }
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
        /// <summary>是否栈式UI模块</summary>
        public virtual bool IsStackable { get; } = true;
        /// <summary>UI资源名（预制体名）</summary>
        public virtual string UIAssetName { get; protected set; }
        /// <summary>UI名，用于UI管理器识别此UI</summary>
        public virtual string Name { get; protected set; }
    }
}