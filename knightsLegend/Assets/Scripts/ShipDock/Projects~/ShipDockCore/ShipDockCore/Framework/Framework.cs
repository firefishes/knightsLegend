using ShipDock.Tools;
using System;

namespace ShipDock
{
    public class Framework : Singletons<Framework>
    {
        public const int UNIT_DATA = 0;
        public const int UNIT_IOC = 1;
        public const int UNIT_MODULARS = 2;
        public const int UNIT_ECS = 3;
        public const int UNIT_AB = 4;
        public const int UNIT_CONFIG = 5;
        public const int UNIT_UI = 6;
        public const int UNIT_ASSET_POOL = 7;
        public const int UNIT_FSM = 8;

        private Action mOnStart;

        public ICustomFramework App { get; private set; }
        public IUpdatesComponent Updates { get; set; }

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
            }
        }

        private KeyValueList<int, IFrameworkUnit> mUnits;

        public Framework()
        {
            mUnits = new KeyValueList<int, IFrameworkUnit>();
        }

        public void Clean()
        {
            Utils.Reclaim(ref mUnits);
            App?.Clean();
        }

        public IFrameworkUnit CreateUnitByBridge<T>(int name, T target)
        {
            return new FrameworkUnitBrige<T>(name, target);
        }

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
            }
        }

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

        public void InitCustomFramework(ICustomFramework app, int ticks, Action onStartUp = default)
        {
            if (App == default)
            {
                App = app;
                App.SetUpdatesComp(Updates);
                Updates = default;

                if (mOnStart != default)
                {
                    App.AddStart(mOnStart);
                    mOnStart = default;
                }
                App.AddStart(onStartUp);
                App.Start(ticks);
                LoadUnit(App.FrameworkUnits);
            }
        }

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
