using ShipDock.Datas;
using ShipDock.Interfaces;
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
            int max = fsmStateInfo.Length;
            for (int i = 0; i < max; i++)
            {
                fsmStateInfo[i].InitMapper();
            }
        }

        public void Dispose()
        {
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
    }

    [Serializable]
    public class RoleFSMStateInfo : SceneInfosMapper<int, RoleFSMStateExecuableInfo>
    {
        public int stateName;
        public RoleFSMStateNotificationer[] enterStateNotice;

        public override int GetInfoKey(ref RoleFSMStateExecuableInfo item)
        {
            return item.phaseName;
        }
    }

    [Serializable]
    public class RoleFSMStateNotificationer
    {
        public int noticeName;
    }

    [Serializable]
    public class RoleFSMStateExecuableInfo
    {
        #region TODO Editorable
        public int phaseName;
        public int allowCalledInEntitas;
        public int callbackID;
        public bool isExecuteInScene;
        #endregion
    }

}