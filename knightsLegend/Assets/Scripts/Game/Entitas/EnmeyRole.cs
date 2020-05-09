using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;

namespace KLGame
{
    public class EnmeyRole : KLRole, IAIRole
    {
        private CommonRoleFSM mFSM;
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

        public override void Dispose()
        {
            base.Dispose();

            mFSM = default;
        }

        public override void InitComponents()
        {
            base.InitComponents();

            TimesEntitas.AddTiming(KLConsts.T_AI_THINKING, 0);
        }

        protected override void OnRoleNotificationHandler(INoticeBase<int> param)
        {
            base.OnRoleNotificationHandler(param);

            //switch(param.Name)
            //{
            //    case KLConsts.N_BRAK_WORKING_AI:
            //        if (mFSM.Current.StateName == NormalRoleStateName.GROUNDED)
            //        {
            //            SetShouldAtkAIWork(false);
            //        }
            //        break;
            //}
        }

        protected override IRoleInput CreateRoleInputInfo()
        {
            RoleFSMName = RoleMustSubgroup.animatorID;
            mFSM = new NormalEnemyRoleFSM(RoleFSMName)
            {
                RoleEntitas = this
            };
            RoleFSM = mFSM;
            return new KLRoleInputInfo(this, mFSM);
        }
        
        protected override void SetRoleInputInfo()
        {
            base.SetRoleInputInfo();

            RoleInput.RoleInputType = KLConsts.ROLE_INPUT_TYPE_ENEMY;
        }

        public override bool AfterGetStopDistance(float dist, Vector3 entitasPos)
        {
            bool result = base.AfterGetStopDistance(dist, entitasPos);
            
            if (!ShouldAtkAIWork)
            {
                SetShouldAtkAIWork(true);
                RoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_ATTACK_AI);
            }
            else
            {
                Notice notice = Pooling<Notice>.From();
                notice.NotifcationSender = this;
                KLConsts.N_BRAK_WORKING_AI.Dispatch(notice);
                notice.ToPool();
            }
            return true;
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
                    base.ComponentIDs.ContactToArr(new int[] {
                        KLConsts.C_ROLE_AI_ATK
                    }, out mComponentIDs);
                }
                return mComponentIDs;
            }
        }

        public override float GetStopDistance()
        {
            return 1.5f;
        }

        public void ResetAIRoleATK()
        {
            TimingTasker target = TimesEntitas.GetTimingTasker(KLConsts.T_AI_ATK_TIME, 0);
            target.ResetRunCounts();

            SetShouldAtkAIWork(false);
        }

        public int ATKID { get; private set; }
        public bool IsInitNormalATKPhases { get; set; }
        public override int RoleFSMName { get; set; }// = KLConsts.RFSM_NORMAL_ENMEY;
        public bool ShouldAtkAIWork { get; private set; }
    }
}