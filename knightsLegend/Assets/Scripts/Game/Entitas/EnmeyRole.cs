using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class EnmeyRole : KLRole, IAIRole
    {
        private CommonRoleFSM mFSM;
        private int[] mComponentIDs;

        public EnmeyRole()
        {
            KLRoleData data = KLRoleData.GetRoleDataByRandom();
            data.ConfigID = 1;
            data.Speed = 2f;
            data.SetSource();
            SetRoleData(data);

            IsUserControlling = false;
            PositionEnabled = true;

            Camp = 1;

            ProvideGoals = new IGoal[]
            {
                new WillKillGoal(),
                new MoveToTargetGoal(),
            };

            ProvideWorldStates = new IWorldState[]
            {
                new RoleAliveState(this)
            };

            AIExecutables = new IAIExecutable[]
            {
                new SerachTargetExecutable(this)
            };

            GoalPlanner = new NormalEnemyGoalPlanner();

            AIBrain = new AIBrain();
            AISensor = new NormalSensor();
            Anticipathioner = new AIBehavioralInfo();
            PolicyAnalyzer = new AIBehavioralInfo();

            AIBrain.InitAIBrain(AISensor, Anticipathioner, PolicyAnalyzer);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (CurrentGoal != default)
            {
                KLConsts.S_KL.DeliveParam<KLServer, IGoalExecuter>("GoalExecuterParam", OnRemoveFollowGoal);
            }

            AIBrain?.Clean();
            AIBrain = default;
            mFSM = default;
        }

        public override void InitComponents()
        {
            base.InitComponents();

            //TimesEntitas.AddTiming(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_ATK);
            //TimesEntitas.AddTiming(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_DEF);
            //TimesEntitas.AddTiming(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_AI_WAITED);

            //TimingTasker timingTasker = TimesEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_ATK);
            //timingTasker.TotalCount = 1;
            //timingTasker.completion += OnNormalAtk;
            //timingTasker.completion += timingTasker.Reset;

            //timingTasker = TimesEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_DEF);
            //timingTasker.completion += OnNormalDef;

            //timingTasker = TimesEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_AI_WAITED);
            //timingTasker.TotalCount = 1;
            //timingTasker.completion += OnAIWaited;
            //timingTasker.completion += timingTasker.Reset;

            AIBrain.SetRole(this);
        }

        //private void OnAIWaited()
        //{
        //    if (RoleFSM.Current is IAIState state)
        //    {
        //        state.AIConduct();
        //    }
        //}

        //private void OnNormalAtk()
        //{
        //    if (Anticipathioner != default)
        //    {
        //        if (Anticipathioner.StateFrom == int.MaxValue && Anticipathioner.AIStateWillChange == default)
        //        {
        //            PolicyAnalyzer.AIStateWillChange = new AIStateWillChange
        //            {
        //                SkillID = 1,
        //                Inputs = new int[] { 1 },
        //                StateWill = NormalRoleStateName.NORMAL_ATK,
        //                RoleFSMParam = Pooling<NormalATKStateParam>.From()
        //            };
        //            ConductTimingTask = int.MaxValue;
        //        }
        //    }
        //}

        //private void OnNormalDef()
        //{
        //    if (Anticipathioner != default)
        //    {
        //        Anticipathioner.StateFrom = int.MaxValue;
        //        Anticipathioner.AIStateWillChange = new AIStateWillChange
        //        {
        //            SkillID = 3,
        //            StateWill = NormalRoleStateName.NORMAL_DEF,
        //            RoleFSMParam = Pooling<KLRoleFSMStateParam>.From()
        //        };
        //    }
        //}

        public override void SetRoleData(IRoleData data)
        {
            base.SetRoleData(data);
            
            TrackViewField = 15f;//TODO 从配置的数据获取追踪视野
        }

        protected override void OnRoleNotificationHandler(INoticeBase<int> param)
        {
            base.OnRoleNotificationHandler(param);

            switch (param.Name)
            {
                //    case KLConsts.N_BRAK_WORKING_AI:
                //        if (mFSM.Current.StateName == NormalRoleStateName.GROUNDED)
                //        {
                //            SetShouldAtkAIWork(false);
                //        }
                //        break;
                case KLConsts.N_AI_ANTICIPATION:
                    Debug.Log("敌人格挡了 " + (param as AIAnticipathionNotice).FromRole.ToString());
                    break;
            }
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
            
            if (result)
            {
                (RoleInput as IRPGRoleInput).SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_ATTACK_AI);
            }
            //if (!true)
            //{
            //    SetShouldAtkAIWork(true);
            //    RoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_ATTACK_AI);
            //}
            //else
            //{
                //Notice notice = Pooling<Notice>.From();
                //notice.NotifcationSender = this;
                //KLConsts.N_BRAK_WORKING_AI.Broadcast(notice);
                //notice.ToPool();
            //}
            return result;
        }

        public void SetATKID(int value)
        {
            ATKID = value;
        }

        public void SetShouldAtkAIWork(bool value)
        {
            //ShouldAtkAIWork = value;
        }

        protected override int[] ComponentNames
        {
            get
            {
                if (mComponentIDs == default)
                {
                    base.ComponentNames.ContactToArr(new int[] {
                        KLConsts.C_ROLE_AI_ATK,
                        KLConsts.C_ROLE_AI_DEF,
                    }, out mComponentIDs);
                }
                return mComponentIDs;
            }
        }

        public override float GetStopDistance()
        {
            return StopDistance;
        }

        /// <summary>
        /// TODO deleted
        /// </summary>
        public void ResetAIRoleATK()
        {
            //TimingTasker target = TimesEntitas.GetTimingTasker(KLConsts.T_AI_ATK_TIME, 0);
            //target.ResetRunCounts();

            //SetShouldAtkAIWork(false);
            
        }

        public bool ShouldAIThinking()
        {
            if (RoleFSM == default)
            {
                return false;
            }
            int stateName = RoleFSM.Current.StateName;
            return AIThinkingStates.IndexOf(stateName) >= 0;
        }

        public List<int> AIThinkingStates { get; } = new List<int>
        {
            NormalRoleStateName.GROUNDED,
            NormalRoleStateName.NORMAL_AI,
        };

        protected override void OnAddToWorld()
        {
            base.OnAddToWorld();

            WorldStates?.AddGoalExecuter(this);
        }

        protected override void OnRemoveFormWorld()
        {
            base.OnRemoveFormWorld();

            WorldStates?.RemoveGoalExecuter(this);
        }

        public override void SetEntitasID(int id)
        {
            base.SetEntitasID(id);

            if (ID != int.MaxValue)
            {
                GoalExecuterID = ID;
            }
        }
        
        public void SetGoal(IGoal goal)
        {
            if (CurrentGoal != default)
            {
                KLConsts.S_KL.DeliveParam<KLServer, IGoalExecuter>("GoalExecuterParam", OnRemoveFollowGoal);
            }

            CurrentGoal = goal;
            
            KLConsts.S_KL.DeliveParam<KLServer, IGoalExecuter>("GoalExecuterParam", OnSetFollowGoal);
        }

        private void OnSetFollowGoal(ref IParamNotice<IGoalExecuter> target)
        {
            GoalExecuterNotice notice = target as GoalExecuterNotice;
            notice.IsAdd = true;
            notice.FollowGoal = CurrentGoal;

            KLConsts.N_GOAL_FOLLOWER.Broadcast(target);
            KLConsts.S_KL.Revert<KLServer>("GoalExecuterParam", target);
        }

        private void OnRemoveFollowGoal(ref IParamNotice<IGoalExecuter> target)
        {
            GoalExecuterNotice notice = target as GoalExecuterNotice;
            notice.FollowGoal = CurrentGoal;

            KLConsts.N_GOAL_FOLLOWER.Broadcast(target);
            KLConsts.S_KL.Revert<KLServer>("GoalExecuterParam", target);
        }

        public bool HasPlanned
        {
            get
            {
                return CurrentGoal != default;
            }
        }

        public int ATKID { get; private set; }
        public bool IsInitNormalATKPhases { get; set; }
        public override int RoleFSMName { get; set; }// = KLConsts.RFSM_NORMAL_ENMEY;
        public IAIBrain AIBrain { get; private set; }
        public ISensor AISensor { get; private set; }
        public IAIBehavioralInfo Anticipathioner { get; set; }
        public IAIBehavioralInfo PolicyAnalyzer { get; set; }
        public IGoal CurrentGoal { get; private set; }
        public int ConductTimingTask { get; set; } = int.MaxValue;
        public int GoalExecuterID { get; private set; }
        public virtual IAIExecutable[] AIExecutables { get; } = default;
        public IGoalPlanner GoalPlanner { get; private set; }
    }
}