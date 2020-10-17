using ShipDock.ECS;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public struct BehaviourIDs
    {
        public int gameItemID;
        public int animatorID;
    }

    public class BehaviourIDsComponent : DataComponent<BehaviourIDs>, ICommonOverlayMapper
    {

        private KeyValueList<int, List<int>> mArounds;

        public override void Init(IShipDockComponentContext context)
        {
            base.Init(context);

            mArounds = new KeyValueList<int, List<int>>();
        }

        protected override BehaviourIDs CreateData()
        {
            return new BehaviourIDs();
        }

        public void SetAnimatorID<E>(ref E target, ref Animator animator) where E : IShipDockEntitas
        {
            BehaviourIDs ids = GetEntitasData(ref target);
            ids.animatorID = animator.GetInstanceID();
            FillEntitasData(ref target, ids);

            AfterAnimatorIDSet?.Invoke(target);
        }

        public void SetGameObjectID<E>(ref E target, GameObject item) where E : IShipDockEntitas
        {
            BehaviourIDs ids = GetEntitasData(ref target);
            ids.gameItemID = item.GetInstanceID();
            FillEntitasData(ref target, ids);

            AfterAnimatorIDSet?.Invoke(target);
        }

        public int GetAnimatorID<E>(ref E target) where E : IShipDockEntitas
        {
            BehaviourIDs ids = GetEntitasData(ref target);
            return ids.animatorID;
        }

        public void OverlayChecked(int gameItemID, int id, bool isTrigger, bool isCollided)
        {
            if (gameItemID == int.MaxValue)
            {
                return;
            }

            List<int> list;
            if (mArounds.ContainsKey(gameItemID))
            {
                list = mArounds[gameItemID]; 
            }
            else
            {
                list = new List<int>();
            }
            if (!list.Contains(id))
            {
                list.Add(id);
            }
        }

        public List<int> GetAroundList<E>(ref E target) where E : IShipDockEntitas
        {
            if (!IsDataValid(ref target))
            {
                return default;
            }

            BehaviourIDs ids = GetEntitasData(ref target);
            List<int> result = mArounds[ids.gameItemID];
            return result;
        }

        public Action<IShipDockEntitas> AfterAnimatorIDSet { get; set; }
    }
}