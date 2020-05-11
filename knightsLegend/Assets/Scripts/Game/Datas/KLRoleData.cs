using ShipDock.Applications;
using ShipDock.Datas;
using System;
using System.Collections.Generic;

namespace KLGame
{

    [Serializable]
    public class KLRoleData : FieldableData, IRoleData
    {
        public static KLRoleData GetRoleDataByRandom()
        {
            KLRoleData result = new KLRoleData
            {
                //Hp = new System.Random().Next(50, 100),
                Speed = UnityEngine.Random.Range(3f, 10f),//new System.Random(10).Next(3, 10) * 1f,
                MovingTurnSpeed = UnityEngine.Random.Range(180f, 360f),
                StationaryTurnSpeed = UnityEngine.Random.Range(90f, 360f),
                GravityMultiplier = 2f,
                JumpPower = 12f
            };
            return result;
        }

        public KLRoleData() : base()
        {
        }

        public int ConfigID { get; set; }
        public float Hp { get; set; }
        public float Speed { get; set; }
        public float MovingTurnSpeed { get; set; }
        public float StationaryTurnSpeed { get; set; }
        public float GravityMultiplier { get; set; }
        public float JumpPower { get; set; }

        override public List<int> FieldNames { get; protected set; } = new List<int>
        {
            KLConsts.FIELD_HP,
            KLConsts.FIELD_QI,
            KLConsts.FIELD_IN_POWER,
        };
    }
}