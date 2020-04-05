using ShipDock.Applications;
using UnityEngine;

namespace KLGame
{
    public class EnmeyRole : KLRole, IAIRole
    {

        private int[] mComponentIDs;

        public EnmeyRole()
        {
            IRoleData data = KLRoleData.GetRoleDataByRandom();
            data.ConfigID = 1;
            SetRoleData(data);

            IsUserControlling = false;
            PositionEnabled = true;

            Camp = 1;
        }

        protected override IRoleInput CreateRoleInputInfo()
        {
            var fsm = new MainMaleRoleFSM(RoleFSMName)
            {
                RoleEntitas = this
            };
            return new KLRoleInputInfo(this, fsm);
        }
        
        protected override void SetRoleInputInfo()
        {
            base.SetRoleInputInfo();

            RoleInput.RoleInputType = KLConsts.ROLE_INPUT_TYPE_ENEMY;
        }

        public override void AfterGetStopDistance(float dist, Vector3 entitasPos)
        {
            base.AfterGetStopDistance(dist, entitasPos);

            if (RoleInput.RoleInputPhase == UserInputPhases.ROLE_INPUT_PHASE_NONE)
            {
                return;
            }

            if (!ShouldAtkAIWork)
            {
                SetShouldAtkAIWork(true);
                RoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_SET_NROMAL_ATK_TRIGGER_TIME);
            }
        }

        public void SetATKID(int value)
        {
            ATKID = value;
        }

        public void SetShouldAtkAIWork(bool value)
        {
            ShouldAtkAIWork = value;
        }

        protected override int[] ComponentIDs
        {
            get
            {
                if (mComponentIDs == default)
                {
                    base.ComponentIDs.ContactToArr(new int[] { KLConsts.C_ROLE_AI_ATK }, out mComponentIDs);
                }
                return mComponentIDs;
            }
        }

        public override float GetStopDistance()
        {
            return 2.5f;
        }

        public void ResetAIRoleATK()
        {
            TimingTasker target = TimesEntitas.GetRoleTiming(RoleTimingTaskNames.NORMAL_ATK_TIME);
            target.ResetRunCounts();

            InATKCycle = false;
            SetShouldAtkAIWork(false);
            RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY);
        }

        public int ATKID { get; private set; }
        public bool ShouldAtkAIWork { get; private set; }
        public bool InATKCycle { get; set; }
        public bool IsInitNormalATKPhases { get; set; }
        public override int RoleFSMName { get; } = KLConsts.RFSM_NORMAL_ENMEY;
    }
}