using ShipDock.Tools;
using System;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 框架定制单例
    /// 
    /// 用于实现以各个管理单元为主构成的通用框架层
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class Framework : Singletons<Framework>
    {
        /// <summary>数据管理单元</summary>
        public const int UNIT_DATA = 0;
        /// <summary>IoC容器管理单元</summary>
        public const int UNIT_IOC = 1;
        /// <summary>业务大模块管理单元</summary>
        public const int UNIT_MODULARS = 2;
        /// <summary>ECS管理单元</summary>
        public const int UNIT_ECS = 3;
        /// <summary>资源包管理单元</summary>
        public const int UNIT_AB = 4;
        /// <summary>配置表管理单元</summary>
        public const int UNIT_CONFIG = 5;
        /// <summary>界面管理单元</summary>
        public const int UNIT_UI = 6;
        /// <summary>U资源对象池管理单元</summary>
        public const int UNIT_ASSET_POOL = 7;
        /// <summary>状态机管理单元</summary>
        public const int UNIT_FSM = 8;
        /// <summary>音效管理单元</summary>
        public const int UNIT_SOUND = 9;

        /// <summary>框架启动回调函数</summary>
        private Action mOnStart;

        /// <summary>框架定制应用对象</summary>
        public ICustomFramework App { get; private set; }
        /// <summary>框架定制对象</summary>
        public IUpdatesComponent Updates { get; set; }

        /// <summary>定制框架是否已启动</summary>
        public bool IsStarted
        {
            get
            {
                return App == default ? false : App.IsStarted;
            }
            set
            {
                if (App == default)
                {
                    App.SetStarted(value);
                }
                else { }
            }
        }

        /// <summary>各管理单元映射</summary>
        private KeyValueList<int, IFrameworkUnit> mUnits;

        public Framework()
        {
            mUnits = new KeyValueList<int, IFrameworkUnit>();
        }

        /// <summary>清除定制框架</summary>
        public void Clean()
        {
            App?.Clean();
            Utils.Reclaim(ref mUnits);
        }

        /// <summary>为定制框架创建桥接单元</summary>
        public IFrameworkUnit CreateUnitByBridge<T>(int name, T target)
        {
            return new FrameworkUnitBrige<T>(name, target);
        }

        /// <summary>
        /// 加载框架的管理单元
        /// </summary>
        /// <param name="units"></param>
        public void LoadUnit(params IFrameworkUnit[] units)
        {
            int max = units.Length;
            IFrameworkUnit unit;
            for (int i = 0; i < max; i++)
            {
                unit = units[i];
                if (!mUnits.ContainsKey(unit.Name))
                {
                    mUnits[unit.Name] = unit;
                }
                else { }
            }
        }

        /// <summary>
        /// 重新加载框架的管理单元
        /// </summary>
        /// <param name="units"></param>
        public void ReloadUnit(params IFrameworkUnit[] units)
        {
            int max = units.Length;
            IFrameworkUnit unit;
            for (int i = 0; i < max; i++)
            {
                unit = units[i];
                mUnits[unit.Name] = unit;
            }
        }

        public T GetUnit<T>(int name)
        {
            if (mUnits == default)
            {
                return default;
            }
            else { }

            T result = default;
            IFrameworkUnit unit = mUnits[name];
            if (unit is FrameworkUnitBrige<T> bridge)
            {
                result = bridge.Unit;
            }
            else
            {
                result = (T)unit;
            }
            return result;
        }

        /// <summary>
        /// 初始化一个定制框架
        /// </summary>
        /// <param name="app">定制框架的对象</param>
        /// <param name="ticks">子线程的运行帧率</param>
        /// <param name="onStartUp">定制框架启动后的回调函数</param>
        public void InitCustomFramework(ICustomFramework app, int ticks, Action onStartUp = default)
        {
            if (App == default)
            {
                App = app;
                App.SetUpdatesComp(Updates);
                Updates = default;//主线帧更新器填充后清除定制框架的引用

                if (mOnStart != default)
                {
                    App.AddStart(mOnStart);
                    mOnStart = default;
                }
                else { }

                App.AddStart(onStartUp);
                App.Start(ticks);
                LoadUnit(App.FrameworkUnits);//加载各个控制单元
            }
            else { }
        }

        /// <summary>
        /// 增加框架启动的回调函数
        /// </summary>
        /// <param name="method"></param>
        public void AddStart(Action method)
        {
            if (App == default)
            {
                mOnStart += method;
            }
            else
            {
                App.AddStart(method);
            }
        }
    }
}
