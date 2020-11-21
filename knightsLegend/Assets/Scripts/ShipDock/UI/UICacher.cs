#define _G_LOG

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
        
        public T GetUICache<T>(string stackName) where T : IUIStack
        {
            return mUICached.ContainsKey(stackName) ? (T)mUICached[stackName] : default;
        }

        public T CreateOrGetUICache<T>(string stackName) where T : IUIStack, new()
        {
            T result = default;
            if(mUICached.ContainsKey(stackName))
            {
                result = (T)mUICached[stackName];
                if(!result.IsExited && result.IsStackable)
                {
                    result.StackAdvance();//标记为栈置顶
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

                if (result.IsStackable)
                {
                    AddStack(result);
                }
            }
            return result;
        }

        public T RemoveAndCheckUICached<T>(string name, out bool isCurrentStack, out T removedStack, bool isDestroy = false) where T : IUIStack
        {
            T result = default;
            removedStack = default;
            isCurrentStack = false;
            if (mUICached.ContainsKey(name))
            {
                result = (T)mUICached[name];
                if (isDestroy)
                {
                    mUICached.Remove(name);
                }
                isCurrentStack = RemoveStack(result, out removedStack);
            }
            return result;
        }

        private void AddStack(IUIStack target)
        {
            StackCurrent = target;
            mUIStacks.Push(target);
        }

        private bool RemoveStack<T>(T target, out T removed) where T : IUIStack
        {
            removed = default;
            bool result = IsCurrentStack(target);
            if (result)
            {
                removed = (T)mUIStacks.Pop();
                "debug".Log("UIStacks.Count " + mUIStacks.Count);
                StackCurrent = mUIStacks.Count > 0 ? mUIStacks.Peek() : StackCurrent;
            }
            return result;
        }

        public bool IsCurrentStack(IUIStack target)
        {
            IUIStack item = default;

            CheckStackCurrentValid(ref item);
            
            return (item != default) && (target != default) && item.Name.Equals(target.Name);
        }

        private void CheckStackCurrentValid(ref IUIStack item)
        {
            if (mUIStacks.Count > 0)
            {
                item = mUIStacks.Peek();
                if (item.IsExited || item.IsStackAdvanced)
                {
                    //略过所有被标记为栈提前的界面
                    if (item.IsStackAdvanced)
                    {
                        item.ResetAdvance();
                    }
                    item = default;
                }
            }
        }

        public IUIStack StackCurrent { get; private set; }
    }
}