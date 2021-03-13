using ShipDock.Interfaces;
using System;

namespace ShipDock.Tools
{
    /// <summary>
    /// 
    /// 步骤队列执行项类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class StepQueueItem : IQueueExecuter
    {
        public StepQueueItem()
        {
            Reinit();
        }

        public void Reinit()
        {
            StepFinish = false;
            mIsDispose = false;
        }

        #region 销毁
        /// <summary>回收</summary>
        protected virtual void Purge()
        {
            StepFinish = false;

            OnNextUnit = null;
            OnUnitExecuted = null;
            OnUnitCompleted = null;
        }
        #endregion
         
        #region 步骤检测
        /// <summary>检测步骤是否完成</summary>
        public void CheckStep(float dTime)
        {
            StepChecking(dTime);

            if (StepFinish)
            {
                QueueNext();
            }
        }

        /// <summary>以上一帧时间差为间隔检测步骤的完成情况</summary>
        protected virtual void StepChecking(float dTime)
        {

        }
        #endregion

        #region 执行队列单元的实现代码
        private bool mIsDispose;

        #region 属性
        protected bool StepFinish { set; get; }

        public Action ActionUnit { get; set; }
        public QueueNextUnit OnNextUnit { get; set; }
        public QueueUnitCompleted OnUnitCompleted { get; set; }
        public QueueUnitExecuted OnUnitExecuted { get; set; }
        public bool ShouldNextAfterCommit { get; protected set; }
        public bool IgnoreInQueue { get; set; }
        #endregion

        public virtual void Dispose()
        {
            if (mIsDispose)
            {
                return;
            }
            
            mIsDispose = true;

            Purge();

        }

        public void Revert()
        {
            Dispose();
        }

        public virtual void Commit()
        {
        }

        public void QueueNext()
        {
            OnNextUnit?.Invoke(this);//让所在的队列执行器执行下一个队列单元
        }

        public bool CanIgnore()
        {
            return IgnoreInQueue;
        }

        public int QueueSize
        {
            get
            {
                return 1;
            }
        }
        #endregion

    }
}