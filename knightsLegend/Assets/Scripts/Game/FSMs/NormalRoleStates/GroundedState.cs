namespace KLGame
{

    public class GroundedState : KLAnimatorState<KLRoleFSMStateParam>
    {
        public GroundedState(int name) : base(name)
        {
        }

        protected override void RevertAllStateParams()
        {
        }

        protected override void RevertStateParam()
        {
            mStateParam?.ToPool();
        }
    }
}