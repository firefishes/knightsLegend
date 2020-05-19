using ShipDock.Datas;
using ShipDock.Infos;
using ShipDock.Interfaces;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    [CreateAssetMenu(menuName = "ShipDock/Create/RPG/RoleFSMObj")]
    public class RoleFSMObj : ScriptableObject, IDispose, IDataUnit
    {
        public int fsmType;
        [SerializeField]
        public RoleFSMStateInfo[] fsmStateInfo;

        public void Init()
        {
            StatesMapper = new KeyValueList<int, RoleFSMStateInfo>();

            RoleFSMStateInfo item;
            int max = fsmStateInfo.Length;
            for (int i = 0; i < max; i++)
            {
                item = fsmStateInfo[i];
                item.InitMapper();
                StatesMapper.Put(item.stateName, item);
            }
        }

        public void Dispose()
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            var target = StatesMapper;
            Utils.Reclaim(ref target);
            Utils.Reclaim(ref fsmStateInfo, true, true);

            StatesMapper = default;
            
        }

        public void FillToSceneComponent(RoleComponent roleComponent)
        {
            int max = fsmStateInfo.Length;
            for (int i = 0; i < max; i++)
            {
                var stateInfo = fsmStateInfo[i];
                var list = stateInfo.Values;
                int n = list.Count;
                for (int j = 0; j < n; j++)
                {
                    var item = list[j];
                    if (item.isExecuteInScene)
                    {
                        roleComponent.ActiveRoleInputPhase(item.phaseName, true);
                    }
                    else
                    {
                        IRoleInput roleInput = roleComponent.RoleEntitas.RoleInput;
                        roleInput.ActiveEntitasPhase(item.phaseName, true);
                    }
                }
            }
        }

        public void RoleFSMStateEntered(INotificationSender target, int stateName)
        {
            RoleFSMStateInfo info = StatesMapper[stateName];

            if (info == default)
            {
                return;
            }

            NotificationInfo[] list = info.enterStateNotice;
            SendRoleFSMStateNotifications(ref target, ref list);
        }

        internal void RoleFSMStateCombo(INotificationSender target, int stateName)
        {
            RoleFSMStateInfo info = StatesMapper[stateName];

            if (info == default)
            {
                return;
            }

            NotificationInfo[] list = info.stateComboNotice;
            SendRoleFSMStateNotifications(ref target, ref list);
        }

        public void RoleFSMStateWillFinish(INotificationSender target, int stateName)
        {
            RoleFSMStateInfo info = StatesMapper[stateName];

            if (info == default)
            {
                return;
            }

            NotificationInfo[] list = info.willFinishStateNotice;
            SendRoleFSMStateNotifications(ref target, ref list);
        }

        private void SendRoleFSMStateNotifications(ref INotificationSender target, ref NotificationInfo[] list)
        {
            Notice notice = Pooling<Notice>.From();
            int max = list.Length;
            for (int i = 0; i < max; i++)
            {
                target.Broadcast(list[i].noticeName, notice);
            }
            notice.ToPool();
        }

        public KeyValueList<int, RoleFSMStateInfo> StatesMapper { get; private set; }

    }

}