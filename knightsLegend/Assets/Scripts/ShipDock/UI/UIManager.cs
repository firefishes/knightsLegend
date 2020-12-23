using System;

namespace ShipDock.UI
{
    /// <summary>
    /// 
    /// UI管理器
    /// 
    /// </summary>
    public class UIManager
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

        public T GetUI<T>(string stackName) where T : IUIStack
        {
            return mUICacher.GetUICache<T>(stackName);
        }

        public T Open<T>(string stackName, Func<object> creater = default) where T : IUIStack, new()
        {
            T result = mUICacher.CreateOrGetUICache<T>(stackName, creater);

            if(!result.IsExited)
            {
                if(result.IsStackable)
                {
                    if ((mPrevious != default) && (mPrevious.Name != result.Name))
                    {
                        mPrevious.Interrupt();
                    }
                    mPrevious = mCurrent;
                    mCurrent = mUICacher.StackCurrent;
                    if (mCurrent.IsStackAdvanced)
                    {
                        mCurrent.Renew();//界面栈被提前后重新唤醒
                    }
                    else
                    {
                        mCurrent.Enter();//界面栈没被提前，说明此界面刚刚打开，已位于栈顶
                    }
                }
                else
                {
                    result.Enter();//非栈管理方式的界面，直接开启
                    //TODO 非栈管理方式的界面需要做层级管理
                }
            }
            return result;
        }

        public void Close(string name, bool isDestroy = false)
        {
            bool isCurrentStack;
            IUIStack result = mUICacher.RemoveAndCheckUICached(name, out isCurrentStack, out IUIStack removed);
            if (isCurrentStack)
            {
                mPrevious = mCurrent;
                mCurrent = mUICacher.StackCurrent;
                mCurrent.Renew();
            }
            else { }//非栈方式管理的界面的额外处理

            if(result != default)
            {
                result.Exit(isDestroy);//退出界面
            }
        }

        public IUIRoot UIRoot { get; private set; }

    }
}