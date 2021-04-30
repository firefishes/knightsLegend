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

        protected override void Init()
        {
            //以下步骤须按注释的顺序调用（包括 base.Init 的执行）
            //在运行到此处以前，设置热更新器实例及热更配置，例如：ShipDockApp.Instance.SetHotFixSetting(new ILRuntimeHotFix(ShipDockApp.Instance), new AppHotFixConfig());
            //启动热更新器 ILRuntimeHotFix 实例，例如： ShipDockApp.Instance.ILRuntimeHotFix.Start();

            base.Init();

            //添加热更新脚本 StartHotfix(dll, pdb);
        }
    }

}