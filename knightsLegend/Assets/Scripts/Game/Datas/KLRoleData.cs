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
                Speed = UnityEngine.Random.Range(3f, 10f),//new System.Random(10).Next(3, 10) * 1f,
                MovingTurnSpeed = UnityEngine.Random.Range(180f, 360f),
                StationaryTurnSpeed = UnityEngine.Random.Range(90f, 360f),
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