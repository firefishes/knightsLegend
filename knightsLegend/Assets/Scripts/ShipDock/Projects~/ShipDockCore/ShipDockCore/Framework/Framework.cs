using ShipDock.Tools;
using System;

namespace ShipDock
{
    public class Framework : Singletons<Framework>
    {
        public const int UNIT_IOC = 0;

        private Action mOnStart;

        public ICustomFramework App { get; private set; }

        public bool IsStarted
        {
            get
            {
                return App != default ? false : App.IsStarted;
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

        public T GetUnit<T>(int name)
        {
            return (T)mUnits[name];
        }

        public void InitCustomFramework(ICustomFramework app)
        {
            if (App == default)
            {
                App = app;
                if (mOnStart != default)
                {
                    App.AddStart(mOnStart);
                    mOnStart = default;
                }
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
