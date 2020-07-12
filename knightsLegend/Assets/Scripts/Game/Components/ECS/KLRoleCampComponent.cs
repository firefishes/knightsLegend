using System;
using ShipDock.Applications;
using ShipDock.Tools;

namespace KLGame
{

    public class KLRoleCampComponent : RoleCampComponent
    {
        private IMainRole mMainRoleTarget;
        private ICommonRole mEnemyTracking;
        private ScopeChecker mMainRoleFrontChecker = new ScopeChecker();

        protected override bool ShouldCampCheck()
        {
            return mRoleTarget.IsStartTrancking;
        }

        protected override void BeforeAITargetEnemyCheck()
        {
            if ((mRoleTarget.Camp == 0) && PreCheckMainRoleFrontEnemy())
            {
                mMainRoleTarget = mRoleTarget as IMainRole;
                mMainRoleFrontChecker.validAngle = 120f;
                mMainRoleFrontChecker.minDistance = 2.5f;
                mMainRoleFrontChecker.startPos = mMainRoleTarget.Position;
                mMainRoleFrontChecker.startRotation = mMainRoleTarget.CurQuaternaion;
                
                if (mMainRoleFrontChecker.CheckScope(mRoleCheckinging.Position))
                {
                    mMainRoleTarget.TargetTracking = mRoleCheckinging;//更新主角追踪的目标
                }
                else
                {
                    mEnemyTracking = mMainRoleTarget.TargetTracking;
                    if (mEnemyTracking != default)
                    {
                        if (!mMainRoleFrontChecker.CheckScope(mEnemyTracking.Position))
                        {
                            mMainRoleTarget.TargetTracking = default;//主角失去追踪的目标
                        }
                    }
                }
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