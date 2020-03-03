using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.UI
{
    public class UICacher
    {
        private Stack<IUIStack> mUIStacks;
        private KeyValueList<string, IUIStack> mUICached;

        public void Init()
        {
            mUIStacks = new Stack<IUIStack>();
            mUICached = new KeyValueList<string, IUIStack>();
        }

        public void Clear()
        {
            Utils.Reclaim(ref mUIStacks, false);
            Utils.Reclaim(ref mUICached, false);
        }

        public T CreateOrGetUICache<T>(string name) where T : IUIStack, new()
        {
            T result = default;
            if(mUICached.ContainsKey(name))
            {
                result = (T)mUICached[name];
                if(!result.IsExited)
                {
                    result.StackAdvance();
                    if(result.IsStackAdvanced)
                    {
                        AddStack(result);
                    }
                }
            }
            else
            {
                result = new T();
                result.Init();
                mUICached[result.Name] = result;
                AddStack(result);
            }
            return result;
        }

        public T RemoveAndCheckUICached<T>(string name, out bool isCurrentStack) where T : IUIStack
        {
            T result = default;
            isCurrentStack = false;
            if (mUICached.ContainsKey(name))
            {
                result = (T)mUICached[name];
                mUICached.Remove(name);
                isCurrentStack = RemoveStack(result);
            }
            return result;
        }

        private void AddStack(IUIStack target)
        {
            StackCurrent = target;
            mUIStacks.Push(target);
        }

        private bool RemoveStack(IUIStack target)
        {
            bool result = IsStackCurrent(target, true);
            if (result)
            {
                StackCurrent = mUIStacks.Pop();
            }
            return result;
        }

        public bool IsStackCurrent(IUIStack target, bool isCheckValid = false)
        {
            IUIStack item = default;
            if(isCheckValid)
            {
                CheckStackCurrentValid(ref item);
            }
            else
            {
                item = (isCheckValid && (mUIStacks.Count > 0)) ? mUIStacks.Peek() : default;
            }
            return (item != default) && (target != default) && item.Name.Equals(target.Name);
        }

        private void CheckStackCurrentValid(ref IUIStack item)
        {
            while (mUIStacks.Count > 0)
            {
                item = mUIStacks.Peek();
                if (item.IsExited || item.IsStackAdvanced)
                {
                    if (item.IsStackAdvanced)
                    {
                        item.ResetAdvance();
                    }
                    mUIStacks.Pop();
                    item = default;
                }
                else
                {
                    break;
                }
            }
        }

        public IUIStack StackCurrent { get; private set; }
    }
    

}