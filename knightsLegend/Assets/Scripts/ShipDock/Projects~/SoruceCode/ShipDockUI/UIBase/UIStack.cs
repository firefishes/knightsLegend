namespace ShipDock.UI
{
    public static class UILayerType
    {
        public const int NONE = 0;
        public const int WINDOW = 1;
        public const int POPUPS = 2;
        public const int WIDGET = 3;
    }

    public enum UILayerTypeEnum
    {
        None,
        Window,
        Widget,
        Popup,
    }

    public class UIStack : object, IUIStack
    {

        public virtual bool IsExited { get; private set; }
        public virtual bool IsStackAdvanced { get; private set; }
        /// <summary>是否栈式UI模块</summary>
        public virtual bool IsStackable { get; } = true;
        /// <summary>UI资源名（预制体名）</summary>
        public virtual string UIAssetName { get; protected set; }
        /// <summary>UI名，用于UI管理器识别此UI</summary>
        public virtual string Name { get; protected set; }

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
            else { }
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
    }
}