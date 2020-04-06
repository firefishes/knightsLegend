#define G_LOG

using ShipDock.Applications;
using ShipDock.Pooling;
using ShipDock.Testers;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class ProcessHit : IGameProcessing
    {
        protected ICommonRole mEnemyRole;
        protected List<int> mCollidingRoles;

        public ProcessHit() { }

        public virtual void Clean()
        {
            Pooling<ProcessHit>.To(this);
        }

        public virtual void Reinit(IKLRole initiator)
        {
            Initiator = initiator;
        }

        public virtual void Revert()
        {
            Finished = false;
            Initiator = default;
            Target = default;
            AfterProcessing = default;
            EnemyKLRole = default;
            mEnemyRole = default;
            mCollidingRoles = default;
        }

        public virtual void OnProcessing()
        {
            if (mEnemyRole != default && HitInfoScope.CheckScope(mEnemyRole.Position))
            {
                Tester.Instance.Log(KLTester.Instance, KLTester.LOG0, mEnemyRole != default, "log: Enemy attack ".Append(mEnemyRole.Name));
                (mEnemyRole as IKLRole).UnderAttack();
                AfterProcessing?.Invoke();
                (Initiator as IKLRole).HitSomeOne = true;
                Vector3 start = Initiator.Position;
                ForceMover.Create().SetMover(mEnemyRole, new Vector3(Initiator.WeapontPos.x, 0, Initiator.WeapontPos.z) * 0.7f, 0.2f);
            }
            Pooling<ProcessHit>.To(this);
        }

        public void ProcessingReady()
        {
            int id;
            mCollidingRoles = Initiator.CollidingRoles;
            int max = mCollidingRoles.Count;
            for (int i = 0; i < max; i++)
            {
                id = mCollidingRoles[i];
                Initiator.Processing.RoleCollisionComp.RefRoleByColliderID(id, out mEnemyRole);
                if (mEnemyRole != default)
                {
                    EnemyKLRole = mEnemyRole as IKLRole;
                    //TODO 需要支持多个敌人
                    break;
                }
            }
        }

        public void ToPooling()
        {
            Pooling<ProcessHit>.To(this);
        }

        public Action AfterProcessing { get; set; }
        public ScopeChecker HitInfoScope { get; set; } = new ScopeChecker();
        public IKLRole Initiator { get; set; }
        public IKLRole Target { get; set; }
        public IKLRole EnemyKLRole { get; private set; }
        public bool Finished { get; set; }
        public int Type { get; } = ProcessingType.HIT;
        public int HitColliderID { get; set; }
    }
}