using ShipDock.FSM;
using ShipDock.Pooling;

namespace KLGame
{

    public class GroundedState : KLAnimatorState<KLRoleFSMStateParam>
    {
        public GroundedState(int name) : base(name)
        {
        }

        public override void InitState(IStateParam param = null)
        {
            base.InitState(param);
        }

        protected override void RevertAllStateParams()
        {
        }

        protected override void RevertStateParam()
        {
            if (mStateParam != default)
            {
                Pooling<KLRoleFSMStateParam>.To(mStateParam);
            }
        }
    }
}