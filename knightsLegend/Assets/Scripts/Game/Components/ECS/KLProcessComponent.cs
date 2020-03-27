using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Pooling;
using ShipDock.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public interface IGameProcessing
    {
        void Commit(IProcessingComponent component);
        IShipDockEntitas Initiator { get; set; }
        IShipDockEntitas Target { get; set; }
    }

    public interface IProcessingComponent : IShipDockComponent
    {
        RoleColliderComponent RoleCollisionComp { get; }
    }

    public class ProcessHit : IGameProcessing, IPoolable
    {
        public virtual void Commit(IProcessingComponent component)
        {
            if (Target != default)
            {
                IKLRole role = Target as IKLRole;
                role.UnderAttack();
            }
            Pooling<ProcessHit>.To(this);
        }

        public virtual void Revert()
        {
            Initiator = default;
            Target = default;
        }

        public ScopeChecker HitInfoScope { get; set; } = new ScopeChecker();
        public IShipDockEntitas Initiator { get; set; }
        public IShipDockEntitas Target { get; set; }
    }

    public class PlayerHit : ProcessHit
    {
        private ICommonRole mRole;
        private List<int> mCollidingRoles;

        public PlayerHit() : base()
        {
        }

        public override void Revert()
        {
            base.Revert();

            PlayerRole = default;
        }

        public override void Commit(IProcessingComponent component)
        {
            if (PlayerRole != default)
            {
                int id;
                mCollidingRoles = PlayerRole.CollidingRoles;
                int max = mCollidingRoles.Count;
                for (int i = 0; i < max; i++)
                {
                    id = mCollidingRoles[i];
                    component.RoleCollisionComp.RefRoleByColliderID(id, out mRole);
                    Debug.Log(mRole.Position);
                    if (HitInfoScope.CheckScope(mRole.Position))
                    {
                        (mRole as IKLRole).UnderAttack();
                        break;
                    }
                }
            }
            Pooling<PlayerHit>.To(this);
        }

        public IKLRole PlayerRole { get; set; } 
    }

    public class KLProcessComponent : ShipDockComponent, IProcessingComponent
    {

        private IGameProcessing mProcessItem;
        private Queue<IGameProcessing> mProcessList;

        public override void Init()
        {
            base.Init();

            RoleCollisionComp = ShipDockApp.Instance.Components.GetComponentByAID(KLConsts.C_ROLE_COLLIDER) as RoleColliderComponent;
            mProcessList = new Queue<IGameProcessing>();
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            if(mProcessList.Count > 0)
            {
                mProcessItem = mProcessList.Dequeue();
                mProcessItem.Commit(this);
            }
        }

        public void AddProcess(IGameProcessing item)
        {
            mProcessList.Enqueue(item);
        }

        public RoleColliderComponent RoleCollisionComp { get; private set; }
    }

}