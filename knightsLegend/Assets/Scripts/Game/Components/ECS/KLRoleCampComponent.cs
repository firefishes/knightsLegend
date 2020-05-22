using System;
using ShipDock.Applications;
using ShipDock.Tools;

namespace KLGame
{

    public class KLRoleCampComponent : RoleCampComponent
    {
        private IKLRole mMainRoleTarget;
        private ScopeChecker mInMainRoleFrontChecker = new ScopeChecker();
        
        protected override void BeforeAITargetEnemyCheck()
        {
            if ((mRoleTarget.Camp == 0) && PreCheckMainRoleFrontEnemy())
            {
                mMainRoleTarget = mRoleTarget as IKLRole;
                mInMainRoleFrontChecker.validAngle = 120f;
                mInMainRoleFrontChecker.minDistance = 2.5f;
                mInMainRoleFrontChecker.startPos = mMainRoleTarget.Position;
                mInMainRoleFrontChecker.startRotation = mMainRoleTarget.CurQuaternaion;
                
                mMainRoleTarget.EnemyTracking = mInMainRoleFrontChecker.CheckScope(mRoleExecuting.Position) ? mRoleExecuting : default;
            }
        }

        private bool PreCheckMainRoleFrontEnemy()
        {
            return true;//TODO 增加预判
        }

        protected override void AfterAITargetEnemyCheck()
        {

        }

        public override string DataServerName { get; } = KLConsts.S_DATAS;
        public override string AddCampRoleResovlerName { get; } = "AddCampRole";
        public override string CampRoleCreatedAlias { get; } = "CampRoleCreated";
    }

}