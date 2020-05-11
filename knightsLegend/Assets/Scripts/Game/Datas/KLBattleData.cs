using ShipDock.Applications;
using ShipDock.Datas;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class BattleUnit : FieldableData, IDataUnit
    {
        public BattleUnit() : base()
        {

        }

        public void FillData(ICommonRole source)
        {
            EntitasID = source.ID;

            IRoleData data = source.RoleDataSource;

            SetIntData(KLConsts.FIELD_HP, data.Hp);
            SetFieldMaxValue(KLConsts.FIELD_HP, data.Hp);
        }

        public void SetUnitID(int id)
        {
            UnitID = id;
        }

        override public List<int> IntFieldNames { get; protected set; } = new List<int>
        {
            KLConsts.FIELD_HP,
            KLConsts.FIELD_QI,
            KLConsts.FIELD_IN_POWER,
            KLConsts.FIELD_FlAWS,
        };

        public int UnitID { get; private set; }
        public int EntitasID { get; private set; }
    }

    public class KLBattleData : DataStorager
    {
        private int mBattleIDIncrease = 0;

        public KLBattleData() : base(KLConsts.D_BATTLE)
        {
        }

        public void AddBattleUnit(ICommonRole role)
        {
            BattleUnit battleUnit = new BattleUnit();
            battleUnit.FillData(role);
            battleUnit.SetUnitID(mBattleIDIncrease);
            mBattleIDIncrease++;

            SetDataUnit(battleUnit.EntitasID, battleUnit);

            Debug.Log("AddBattleUnit");
            Debug.Log(role);

        }
    }

}