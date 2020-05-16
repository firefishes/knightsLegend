using ShipDock.Applications;
using ShipDock.Datas;
using UnityEngine;

namespace KLGame
{
    public class KLBattleData : DataStorager
    {
        private int mBattleIDIncrease = 0;

        public KLBattleData() : base(KLConsts.D_BATTLE)
        {
        }

        public BattleUnit AddBattleUnit(ICommonRole role)
        {
            BattleUnit battleUnit = new BattleUnit();
            battleUnit.SetSource(role);
            battleUnit.SetUnitID(mBattleIDIncrease);
            mBattleIDIncrease++;

            SetDataUnit(battleUnit.EntitasID, battleUnit);

            Debug.Log("AddBattleUnit");
            Debug.Log(role);

            return battleUnit;
        }

        public void BattleHit()
        {

        }
    }

}