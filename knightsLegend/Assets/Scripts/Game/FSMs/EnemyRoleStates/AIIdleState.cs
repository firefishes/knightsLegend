using ShipDock.FSM;

namespace KLGame
{
    public class AIIdleState : FState
    {
        public AIIdleState(int name) : base(name)
        {
        }

        public override void InitState(IStateParam param = null)
        {
            base.InitState(param);

            IKLRoleFSMAIParam roleFSMAIParam = param as IKLRoleFSMAIParam;
            
        }
    }
}