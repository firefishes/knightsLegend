using ShipDock.Applications;
using ShipDock.Datas;
using System.Collections.Generic;

namespace KLGame
{
    public class BattleUnit : FieldableData, IDataUnit
    {

        private KLRoleData mRoleData;

        public BattleUnit() : base()
        {

        }

        public override void Dispose()
        {
            base.Dispose();

            mRoleData = default;
        }

        public void SetSource(ICommonRole source)
        {
            EntitasID = source.ID;

            mRoleData = source.RoleDataSource as KLRoleData;

            FillValues();

            mRoleData = default;
        }

        public void SetUnitID(int id)
        {
            UnitID = id;
        }

        public void UpdateFlaws(float value, out int result)
        {
            result = 0;
            float flaws = GetFloatData(KLConsts.FIELD_FlAWS);
            float flawsMax = GetFloatData(KLConsts.FIELD_M_FlAWS);
            if (flaws + value > flawsMax)
            {
                result = 1;
                flaws = flawsMax;
            }
            else
            {
                flaws += value;
            }

            UnityEngine.Debug.Log("破绽：" + flaws + "/" + flawsMax);
            SetFloatData(KLConsts.FIELD_FlAWS, flaws);
        }

        public override List<int> GetIntFieldSource()
        {
            return default;
        }

        public override List<float> GetFloatFieldSource()
        {
            return new List<float>
            {
                mRoleData.GetFloatData(KLConsts.FIELD_HP),
                mRoleData.GetFloatData(KLConsts.FIELD_M_HP),
                mRoleData.GetFloatData(KLConsts.FIELD_QI),
                mRoleData.GetFloatData(KLConsts.FIELD_M_QI),
                mRoleData.GetFloatData(KLConsts.FIELD_IN_POWER),
                mRoleData.GetFloatData(KLConsts.FIELD_M_IN_POWER),
                0f,
                50f,
            };
        }

        public override List<string> GetStringFieldSource()
        {
            return default;
        }

        override public List<int> FloatFieldNames { get; protected set; } = new List<int>
        {
            KLConsts.FIELD_HP,
            KLConsts.FIELD_M_HP,
            KLConsts.FIELD_QI,
            KLConsts.FIELD_M_QI,
            KLConsts.FIELD_IN_POWER,
            KLConsts.FIELD_M_IN_POWER,
            KLConsts.FIELD_FlAWS,
            KLConsts.FIELD_M_FlAWS,
        };

        public int UnitID { get; private set; }
        public int EntitasID { get; private set; }
    }

}