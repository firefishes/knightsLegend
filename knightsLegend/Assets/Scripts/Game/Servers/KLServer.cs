using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLServer : MainServer
    {
        public KLServer() : base(KLConsts.S_KL)
        {
        }

        public override void InitServer()
        {
            base.InitServer();

            //Register<IParamNotice<NormalATKStateParam>>(NormalATKStateParamResolver, Pooling<NormalATKStateParam>.Instance);
            //Register<IParamNotice<IKLRole>>(KLRoleResolver, Pooling<ParamNotice<IKLRole>>.Instance);
            //Register<IParamNotice<IGoalExecuter>>(GoalExecuterResolver, Pooling<GoalExecuterNotice>.Instance);
        }

        //[Resolvable("GoalExecuterParam")]
        //private void GoalExecuterResolver(ref IParamNotice<IGoalExecuter> target) { }

        //[Resolvable("KLRole")]
        //private void KLRoleResolver(ref IParamNotice<IKLRole> target) { }

        //[Resolvable("NormalATKStateParam")]
        //private void NormalATKStateParamResolver(ref IParamNotice<NormalATKStateParam> target) { }

        public override void ServerReady()
        {
            base.ServerReady();
        }
    }

    //public class GoalExecuterNotice : ParamNotice<IGoalExecuter>
    //{
    //    public override void Revert()
    //    {
    //        base.Revert();

    //        IsAdd = default;
    //        FollowGoal = default;
    //    }

    //    public override void ToPool()
    //    {
    //        Pooling<GoalExecuterNotice>.To(this);
    //    }

    //    public IGoal FollowGoal { get; set; }
    //    public bool IsAdd { get; set; }
    //}
}
