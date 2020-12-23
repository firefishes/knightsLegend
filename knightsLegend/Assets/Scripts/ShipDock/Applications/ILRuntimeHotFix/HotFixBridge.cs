using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 使用热更类库的抽象类实现的桥接器组件，可以直接使用
    /// </summary>
    public class HotFixBridge : HotFixer
    {
        [SerializeField]
        protected HotFixerSubgroup m_DataSource;

        public HotFixerSubgroup DataSource
        {
            get
            {
                return m_DataSource;
            }
        }

        protected override void Purge() { }

        protected override void RunWithinFramework() { }

        protected override void InitILRuntime()
        {
            base.InitILRuntime();

            //此处启动已封装的 ILRuntimeHotFix 实例，例如： ShipDockApp.Instance.ILRuntimeHotFix.Start();

        }
    }

}