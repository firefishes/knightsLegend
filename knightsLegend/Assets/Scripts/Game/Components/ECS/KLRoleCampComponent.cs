using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{

    public class KLRoleCampComponent : RoleCampComponent
    {
        private ServerRelater mRelater;
        private ScopeChecker mInMainRoleFrontChecker = new ScopeChecker();

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            //if (mMainRole == default)
            //{
            //    mMainRole = target as IMainRole;
            //}
            //else
            //{
            //    mInMainRoleFrontChecker.validAngle = 120f;//巡逻家教
            //    mInMainRoleFrontChecker.minDistance = 2.5f;
            //    mInMainRoleFrontChecker.startPos = mMainRole.Position;
            //    mInMainRoleFrontChecker.startRotation = mStateParam.StartRotation;
            //}
        }

        private void CheckMainRolesFrontEnemy()
        {
            //if (mRoleTarget.Camp == 0)
            //{
            //    mInMainRoleFrontChecker.validAngle = 120f;//巡逻家教
            //    mInMainRoleFrontChecker.minDistance = 2.5f;
            //    mInMainRoleFrontChecker.startPos = mMainRole.Position;
            //    mInMainRoleFrontChecker.startRotation = mStateParam.StartRotation;
            //}
        }

        public override string DataServerName { get; } = KLConsts.S_DATAS;
        public override string AddCampRoleResovlerName { get; } = "AddCampRole";
        public override string CampRoleCreatedAlias { get; } = "CampRoleCreated";
    }

}