using ShipDock.ECS;
using ShipDock.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public interface IGameProcessing
    {
        void Commit();
        IShipDockEntitas Initiator { get; set; }
        IShipDockEntitas Target { get; set; }
    }

    public class ProcessHit : IGameProcessing, IPoolable
    {
        public void Commit()
        {
            if(Target != default)
            {
                IKLRole role = Target as IKLRole;
                role.UnderAttack();
                Pooling<ProcessHit>.To(this);
            }
        }

        public void Revert()
        {
            Initiator = default;
            Target = default;
        }

        public IShipDockEntitas Initiator { get; set; }
        public IShipDockEntitas Target { get; set; }
    }

    public class KLProcessComponent : ShipDockComponent
    {

        private IGameProcessing mProcessItem;
        private Queue<IGameProcessing> mProcessList;

        public override void Init()
        {
            base.Init();

            mProcessList = new Queue<IGameProcessing>();
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            if(mProcessList.Count > 0)
            {
                mProcessItem = mProcessList.Dequeue();
                mProcessItem.Commit();
            }
        }

        public void AddProcess(IGameProcessing item)
        {
            mProcessList.Enqueue(item);
        }
    }

}