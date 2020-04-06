namespace KLGame
{
    public interface IKLRoleSceneComponent
    {
        void FillRoleFSMStateParam(IKLRoleFSMParam param);
        void FillRoleFSMAIStateParam(IKLRoleFSMAIParam param);
        IKLRole KLRole { get; }
        bool MoveBlock { get; set; }
        int CurrentSkillID { get; }
    }
}