using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class RoleBattleDataComponent : ShipDockComponent
    {

        private IKLRole mRole;
        private BattleUnit mBattleUnit;
        private ServerRelater mRelater;
        private KeyValueList<int, BattleUnit> mBattleDataUnits;

        public override void Init()
        {
            base.Init();

            mBattleDataUnits = new KeyValueList<int, BattleUnit>();

            mRelater = new ServerRelater
            {
                DataNames = new int[]
                {
                    KLConsts.D_BATTLE
                }
            };
            mRelater.CommitRelate();
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mRole = target as IKLRole;
            int id = target.ID;
            if (mRole.WillDestroy)
            {
                KLBattleData data = mRelater.DataRef<KLBattleData>(KLConsts.D_BATTLE);
                data.SetDataUnit(id, default);
                return;
            }
            else
            {
                if (mBattleDataUnits.ContainsKey(id))
                {
                    mBattleUnit = mBattleDataUnits[id];
                }
                else
                {
                    KLBattleData data = mRelater.DataRef<KLBattleData>(KLConsts.D_BATTLE);
                    if (data == default)
                    {
                        return;
                    }
                    mBattleUnit = data.AddBattleUnit(mRole);
                    mBattleDataUnits[mBattleUnit.EntitasID] = mBattleUnit;

                    mRole.SetBattleUnit(mBattleUnit);
                }
            }

            if (mBattleUnit.HasChanged)
            {
                CheckFlaws();
                mBattleUnit.HasChanged = false;
            }

        }

        private void CheckFlaws()
        {
            if (mBattleUnit.GetFloatData(KLConsts.FIELD_FlAWS) <= 0f)
            {
                mRole.RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED);
            }
        }
    }

}