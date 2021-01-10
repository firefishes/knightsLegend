using ShipDock.Interfaces;
using System;
using System.Collections.Generic;

namespace ShipDock.Tools
{
    /// <summary>
    /// 队列执行器
    /// 
    /// add by Minghua.ji
    /// 
    /// 若干个队列单元以一个顺序组成队列依次执行
    /// 队列单元支持范围包括：函数、实现了IQueueExecuter接口、继承此类的子类实例
    /// 
    /// </summary>
    public class QueueExecuter : IQueueExecuter
    {
        /// <summary>执行下一个队列单元时触发的回调</summary>
        public QueueNextUnit OnNextUnit { set; get; }
        /// <summary>队列单元已被执行的回调</summary>
        public QueueUnitExecuted OnUnitExecuted { set; get; }
        /// <summary>全部队列执行完毕的回调</summary>
        public QueueUnitCompleted OnUnitCompleted { set; get; }
        
        /// <summary>当前执行到的索引</summary>
        private int mCurrentIndex;
        /// <summary>流程队列</summary>
        private Queue<IQueueExecuter> mQueue;
        private Queue<IQueueExecuter> mQueueExecuted;
        /// <summary>是否自动销毁，开启自动销毁功能会在队列完成时自动销毁各个队列单元</summary>
        private bool mAutoDispose;
        private IQueueExecuter mCurrent;

        /// <summary>构筑一个执行队列</summary>
        public QueueExecuter(bool autoDispose = true)
        {
            mQueue = new Queue<IQueueExecuter>();
            mQueueExecuted = new Queue<IQueueExecuter>();
            mAutoDispose = autoDispose;
            Init();
        }
        
        /// <summary>销毁</summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;//不重复销毁，否则会引起循环调用
            }

            IsDisposed = true;
            ClearQueue(true);

            Utils.Reclaim(ref mQueue, true, IsDisposQueueItem);
            Utils.Reclaim(ref mQueueExecuted, true, IsDisposQueueItem);

            mQueue = default;
            mQueueExecuted = default;
            OnNextUnit = default;
            OnUnitExecuted = default;
            OnUnitCompleted = default;
            ActionUnit = default;
        }

        public void ClearWithoutDispose()
        {
            isRunning = false;
            Utils.Reclaim(ref mQueue, false);
            Utils.Reclaim(ref mQueueExecuted, false);
        }
        
        /// <summary>清理队列</summary>
        public void ClearQueue(bool isDispose = false)
        {
            isRunning = false;
            Utils.Reclaim(ref mQueue, true, isDispose);
            Utils.Reclaim(ref mQueueExecuted, true, isDispose);
            Init();
        }

        /// <summary>初始化</summary>
        private void Init()
        {
            mCurrent = default;
            isRunning = false;
            IsDisposed = false;
            mCurrentIndex = 0;
        }

        /// <summary>重置队列</summary>
        public void ResetQueue(bool autoDispose = true)
        {
            Dispose();
            mQueue = new Queue<IQueueExecuter>();
            mQueueExecuted = new Queue<IQueueExecuter>();
            mAutoDispose = autoDispose;
            Init();
        }

        /// <summary>将一个流程执行器增加到队列末尾</summary>
        public void Add(IQueueExecuter target)
        {
            if (target == null)
            {
                return;
            }

            mQueue.Enqueue(target);
        }

        /// <summary>添加元素</summary>
        public void Add(Action args)
        {
            if (args == null)
            {
                return;
            }

            QueueExecuter unit = new QueueExecuter();
            unit.ActionUnit = args;
            mQueue.Enqueue(unit);
        }

        /// <summary>开始执行流程队列</summary>
        protected void Start()
        {
            if (isRunning)
            {
                return;
            }
            isRunning = true;
            IsDisposed = false;
            if (mAutoDispose || (QueueSize == 0))
            {
                mCurrentIndex = 0;
            }
            ExecuteUnit();
        }

        /// <summary>重置</summary>
        public virtual void Reset()
        {
            ClearQueue();
        }
        
        /// <summary>执行队列中的下一个执行器</summary>
        protected void ExecuteUnit()
        {
            IQueueExecuter unit = null;

            if (OnUnitExecuted != null)
            {
                OnUnitExecuted.Invoke(this);//本执行单元开始执行
            }

            if ((mQueue != null) && (mQueue.Count > 0))
            {
                unit = mQueue.Dequeue();//设置当前单元为下一个执行单元
                mQueueExecuted.Enqueue(unit);
                mCurrentIndex++;
                mCurrent = unit;
            }
            else
            {
                if (isRunning)
                {
                    isRunning = false;
                    if (OnUnitCompleted != null)
                    {
                        OnUnitCompleted.Invoke(this);//队列执行完成
                    }
                    if (OnNextUnit != null)//用于继续开启下一个队列执行器的执行
                    {
                        OnNextUnit.Invoke(this);
                    }
                    if (mAutoDispose)
                    {
                        Dispose();
                    }
                }
                return;
            }

            bool canNext = true;
            bool isIgnore = CanIgnore();
            if (!isIgnore)
            {
                if (unit != null)
                {
                    if (unit.ActionUnit != null)
                    {
                        unit.ActionUnit();//执行普通委托
                    }
                    else
                    {
                        canNext = false;//执行子项时需等待子项全部完成
                        unit.OnNextUnit += NextUnit;//衔接上下子项的执行顺序
                        unit.Commit();//执行子项
                    }
                    if (unit.OnUnitExecuted != null)
                    {
                        unit.OnUnitExecuted.Invoke(this);//本执行单元开始执行
                    }

                }
            }
            if (canNext)
            {
                ExecuteUnit();
            }
        }

        /// <summary>衔接队列中下一个执行器事件处理函数的执行，构成队列的自动运行结构</summary>
        protected void NextUnit(IQueueExecuter param)
        {
            param.OnNextUnit -= NextUnit;
            ExecuteUnit();
        }

        /// <summary>此执行项是否可被忽略</summary>
        protected virtual bool CanIgnore()
        {
            return false;
        }

        /// <summary>运行队列执行器的入口</summary>
        public virtual void Commit()
        {
            Start();
        }

        /// <summary>主动调用，执行此对象所在队列的下一个队列元素</summary>
        public virtual void QueueNext()
        {
            OnNextUnit?.Invoke(this);
        }
        
        /// <summary>获取当前执行队列的执行位置</summary>
        public virtual int CurrentIndex
        {
            get
            {
                return mCurrentIndex;
            }
        }

        public virtual int QueueSize
        {
            get
            {
                return (mQueue != null) ? mQueue.Count : 0;
            }
        }

        public IQueueExecuter Current()
        {
            return mCurrent;
        }

        public bool isRunning { get; private set; }
        public bool IsDisposed { get; private set; }
        public bool IsDisposQueueItem { get; set; }
        public Action ActionUnit { get; set; }

    }
}