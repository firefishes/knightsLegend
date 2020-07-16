using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Tools;
using System.Collections.Generic;

namespace KLGame
{
    public class RoleBattleDataComponent : ShipDockComponent
    {

        private IKLRole mRole;
        private BattleUnit mBattleUnit;
        private ServerRelater mRelater;
        private bool mHasChanged;
        private List<int> mBattleUnitFields;
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

        public override int DropEntitas(IShipDockEntitas target, int entitasID)
        {
            IKLRole mRole = target as IKLRole;
            if (mRole.BattleDataUnit != default)
            {
                Utils.Reclaim(mRole.BattleDataUnit);
                mRole.SetBattleUnit(default);

                KLBattleData data = mRelater.DataRef<KLBattleData>(KLConsts.D_BATTLE);
                data.SetDataUnit(target.ID, default);
            }
            return base.DropEntitas(target, entitasID);
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mRole = target as IKLRole;
            int id = target.ID;
            if (id == int.MaxValue || mRole.WillDestroy)
            {
                return;
            }
            if (mBattleDataUnits.ContainsKey(id))
            {
                mBattleUnit = mBattleDataUnits[id];
            }
            else
            {
                CacheBattleUnit();
            }
            CheckBattleUnitFields();
        }

        private void CacheBattleUnit()
        {
            KLBattleData data = mRelater.DataRef<KLBattleData>(KLConsts.D_BATTLE);
            if (data == default)
            {
                return;
            }
            mBattleUnit = data.AddBattleUnit(mRole);
            mBattleDataUnits[mBattleUnit.EntitasID] = mBattleUnit;
            mBattleUnit.SetValueChanged(KLConsts.FIELD_FlAWS, CheckFlaws);

            mRole.SetBattleUnit(mBattleUnit);
        }

        private void CheckBattleUnitFields()
        {
            mBattleUnitFields = mBattleUnit.AllFieldNames;
            int max = mBattleUnitFields.Count;
            for (int i = 0; i < max; i++)
            {
                mHasChanged = mBattleUnit.HasFieldChanged(mBattleUnitFields[i]);
                if (mHasChanged)
                {
                    mBattleUnit.AfterValueChange(i);
                }
            }
        }

        private void CheckFlaws()
        {
            if (mBattleUnit.GetFloatData(KLConsts.FIELD_FlAWS) <= 0f)
            {
                (mRole.RoleInput as IRPGRoleInput).SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED);
            }
        }
    }

}