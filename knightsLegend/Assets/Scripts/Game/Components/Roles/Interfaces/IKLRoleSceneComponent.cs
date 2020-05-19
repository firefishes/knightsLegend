using ShipDock.Notices;

namespace KLGame
{
    public interface IKLRoleSceneComponent : INotificationSender
    {
        void FillRoleFSMStateParam(IKLRoleFSMParam param);
        void FillRoleFSMAIStateParam(IKLRoleFSMAIParam param);
        void RoleFSMStateEntered(int stateName);
        void RoleFSMStateCombo(int stateName);
        void RoleFSMStateWillFinish(int stateName);
        IKLRole KLRole { get; }
        bool MoveBlock { get; set; }
        int CurrentSkillID { get; }
    }
}