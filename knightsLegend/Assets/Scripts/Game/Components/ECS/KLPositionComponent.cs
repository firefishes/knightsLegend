using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;

namespace KLGame
{
    public class KLPositionComponent : PositionComponent
    {
        private IKLRole mKLRole;
        private Notice mRoleMovingNotice;

        public override void Dispose()
        {
            base.Dispose();

            mRoleMovingNotice.ToPool();
        }

        public override void Init()
        {
            base.Init();

            mRoleMovingNotice = Pooling<Notice>.From();

        }

        protected override void StartMoving()
        {
            base.StartMoving();

            mKLRole = mRole as IKLRole;

            mKLRole.Dispatch(KLConsts.N_ROLE_START_MOVING, mRoleMovingNotice);
        }
    }
}