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
            if (mRoleEntitas.Camp == 0)
            {
                mMainRoleTarget = mRoleEntitas as IKLRole;
                mInMainRoleFrontChecker.validAngle = 120f;
                mInMainRoleFrontChecker.minDistance = 2.5f;
                mInMainRoleFrontChecker.startPos = mMainRoleTarget.Position;
                mInMainRoleFrontChecker.startRotation = mMainRoleTarget.CurQuaternaion;
                
                mMainRoleTarget.EnemyMainLockDown = mInMainRoleFrontChecker.CheckScope(mRoleTarget.Position) ? mRoleTarget : default;
            }
        }

        protected override void AfterAITargetEnemyCheck()
        {

        }

        public override string DataServerName { get; } = KLConsts.S_DATAS;
        public override string AddCampRoleResovlerName { get; } = "AddCampRole";
        public override string CampRoleCreatedAlias { get; } = "CampRoleCreated";
    }

}