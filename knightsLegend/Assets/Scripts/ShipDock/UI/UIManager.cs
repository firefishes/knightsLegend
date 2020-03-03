using ShipDock.Interfaces;

namespace ShipDock.UI
{
    public class UIManager : IDispose
    {
        private IUIStack mCurrent;
        private IUIStack mPrevious;
        private UICacher mUICacher;

        public UIManager()
        {
            mUICacher = new UICacher();
            mUICacher.Init();
        }

        public void Dispose()
        {
            mUICacher.Clear();

            mCurrent = default;
            mPrevious = default;
        }

        public void SetRoot(IUIRoot root)
        {
            UIRoot = root;
        }

        public T Open<T>(string name) where T : IUIStack, new()
        {
            T result = mUICacher.CreateOrGetUICache<T>(name);

            if(!result.IsExited)
            {
                if(mPrevious != default)
                {
                    mPrevious.Interrupt();
                }
                mPrevious = mCurrent;
                mCurrent = mUICacher.StackCurrent;
                if (mCurrent.IsStackAdvanced)
                {
                    mCurrent.Renew();
                }
                else
                {
                    mCurrent.Enter();
                }
            }
            return result;
        }

        public void Close<T>(string name) where T : IUIStack, new()
        {
            bool isCurrentStack;
            T result = mUICacher.RemoveAndCheckUICached<T>(name, out isCurrentStack);
            if(isCurrentStack)
            {
                mPrevious = mCurrent;
                mCurrent = mUICacher.StackCurrent;
                mCurrent.Renew();
            }
            if(result != default)
            {
                result.Exit();
            }
        }

        public IUIRoot UIRoot { get; private set; }

    }
}