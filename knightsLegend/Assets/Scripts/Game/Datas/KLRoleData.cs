using ShipDock.Applications;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    [Serializable]
    public struct KLRoleData : IRoleData
    {
        public static KLRoleData GetRoleDataByRandom()
        {
            KLRoleData result = new KLRoleData
            {
                //Hp = new System.Random().Next(50, 100),
                Speed = 2f,// new System.Random(10).Next(1, 50) * 0.1f,
                MovingTurnSpeed = 360,
                StationaryTurnSpeed = 180,
                GravityMultiplier = 2f,
                JumpPower = 12f
            };
            return result;
        }

        public int ConfigID { get; set; }
        public float Hp { get; set; }
        public float Speed { get; set; }
        public float MovingTurnSpeed { get; set; }
        public float StationaryTurnSpeed { get; set; }
        public float GravityMultiplier { get; set; }
        public float JumpPower { get; set; }
    }
}