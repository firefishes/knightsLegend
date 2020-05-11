using ShipDock.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class BattleUnit : FieldableData, IDataUnit
    {
        public BattleUnit() : base()
        {

        }
        
        override public List<int> FieldNames { get; protected set; } = new List<int>
        {
            KLConsts.FIELD_HP,
            KLConsts.FIELD_QI,
            KLConsts.FIELD_IN_POWER,
            KLConsts.FIELD_FlAWS,
        };
    }

    public class KLBattleData : DataStorager
    {
        public KLBattleData() : base(KLConsts.D_BATTLE)
        {
        }
    }

}