using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace KLGame
{
    public class KLWorldStatesComponent : ShipDockComponent
    {

        private IGoal mGoal;
        private IGoalPlanner mPlanner;
        private IGoalExecuter mExecuterProvider;
        private List<IGoal> mWillRemoveGoals;
        private List<IWorldState> mWorldStates;
        private List<IGoalExecuter> mGoalExecuterProvider;
        private Queue<IGoalPlanner> mStartPlanQueue;
        private Queue<PlanDetail> mPlanDetailQueue;

        #region GOAP
        /// <summary>所有目标</summary>
        private List<IGoal> mGoals;
        /// <summary>世界状态</summary>
        private WorldStatesMapper mWorldStateMapper;
        /// <summary>目标执行器</summary>
        private KeyValueList<int, IGoalExecuter> mGoalExecuters;
        /// <summary>目标跟进者</summary>
        private KeyValueList<IGoal, List<IGoalExecuter>> mGoalFollowers;
        #endregion

        public override void Init()
        {
            base.Init();

            mWorldStateMapper = new WorldStatesMapper();

            mGoals = new List<IGoal>();
            mGoalExecuterProvider = new List<IGoalExecuter>();
            mGoalExecuters = new KeyValueList<int, IGoalExecuter>();
            mWillRemoveGoals = new List<IGoal>();
            mStartPlanQueue = new Queue<IGoalPlanner>();
            mPlanDetailQueue = new Queue<PlanDetail>();
            mGoalFollowers = new KeyValueList<IGoal, List<IGoalExecuter>>();

            KLConsts.N_GOAL_FOLLOWER.Add(OnGoalFollower);
        }

        public override void Dispose()
        {
            base.Dispose();

            KLConsts.N_GOAL_FOLLOWER.Remove(OnGoalFollower);
        }

        private void OnGoalFollower(INoticeBase<int> param)
        {
            if (param is GoalExecuterNotice notice)
            {
                IGoal goal = notice.FollowGoal;
                if (notice.IsAdd)
                {
                    if (mGoalFollowers.ContainsKey(goal))
                    {
                        mGoalFollowers[goal].Add(notice.ParamValue);
                    }
                }
                else
                {
                    if (mGoalFollowers.ContainsKey(goal))
                    {
                        mGoalFollowers[goal].Remove(notice.ParamValue);
                    }
                }
            }
        }

        public void AddWorldGoals(IGoalIssuer goalIssuer)
        {
            IGoal[] goals = goalIssuer != default ? goalIssuer.ProvideGoals : default;
            if (goals != default)
            {
                int max = goals.Length;
                for (int i = 0; i < max; i++)
                {
                    mGoal = goals[i];
                    mGoal.Available = true;
                    if (!mGoals.Contains(mGoal))
                    {
                        mGoal.Init(goalIssuer);
                        mGoals.Add(mGoal);

                        if(!mGoalFollowers.ContainsKey(mGoal))
                        {
                            mGoalFollowers[mGoal] = new List<IGoalExecuter>();
                        }
                    }
                }
            }
        }
        
        public void RemoveWorldGoals(IGoalIssuer goalIssuer)
        {
            IGoal[] goals = goalIssuer != default ? goalIssuer.ProvideGoals : default;
            if (goals != default)
            {
                IGoal item;
                int max = goals.Length;
                for (int i = 0; i < max; i++)
                {
                    item = goals[i];
                    item.Clean();
                    item.Available = false;
                    item.WillRemoveFromWorld = true;

                    if(mGoalFollowers.ContainsKey(item))
                    {
                        List<IGoalExecuter> exectuers = mGoalFollowers[item];
                        Utils.Reclaim(ref exectuers, false);
                        mGoalFollowers.Remove(item);
                    }
                }
            }
        }

        public void AddWorldStates(IWorldStateIssuer stateIssuer)
        {
            mWorldStateMapper.AddWorldStates(stateIssuer);
        }

        public void RemoveWorldStates(IWorldStateIssuer stateIssuer)
        {
            mWorldStateMapper.RemoveWorldStates(stateIssuer);
        }
        
        public void AddGoalExecuter(IGoalExecuter provider)
        {
            if (provider != default && !mGoalExecuters.ContainsKey(provider.GoalExecuterID))
            {
                mGoalExecuters[provider.GoalExecuterID] = provider;
            }
        }

        public void RemoveGoalExecuter(IGoalExecuter provider)
        {
            if (provider != default && mGoalExecuters.ContainsKey(provider.GoalExecuterID))
            {
                mGoalExecuters.Remove(provider.GoalExecuterID);
            }
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mWorldStateMapper.UpdateWorldStats();

            int max = mGoals.Count;
            for (int i = 0; i < max; i++)
            {
                mGoal = mGoals[i];
                if (mGoal.WillRemoveFromWorld)
                {
                    mWillRemoveGoals.Add(mGoal);
                }
                else
                {
                    CheckWorldGoal();
                }
            }
            max = mWillRemoveGoals.Count;
            for (int i = 0; i < max; i++)
            {
                mGoal = mWillRemoveGoals[i];
                mGoals.Remove(mGoal);
            }
            mWillRemoveGoals.Clear();

            ExecuteInFinal(time, target, CheckExecuterProvider);
        }

        private void CheckExecuterProvider(int time, IShipDockEntitas entitas)
        {
            mGoalExecuterProvider = mGoalExecuters.Values;
            int max = mGoalExecuterProvider.Count;
            for (int i = 0; i < max; i++)
            {
                mExecuterProvider = mGoalExecuterProvider[i];
                if (!mExecuterProvider.HasPlanned)
                {
                    mPlanner = mExecuterProvider.GoalPlanner;
                    mPlanner.WillPlan(mExecuterProvider);
                    if (!mStartPlanQueue.Contains(mPlanner))
                    {
                        mStartPlanQueue.Enqueue(mPlanner);
                        ExecuteInFinal(time, default, PlanInFinal);
                    }
                }
            }
        }

        private void PlanInFinal(int time, IShipDockEntitas target)
        {
            IGoalPlanner planner = mStartPlanQueue.Dequeue();
            mWorldStates = mWorldStateMapper.States;
            planner.StartPlan(ref mGoals, ref mWorldStates, ref mPlanDetailQueue);
            
            if (mPlanDetailQueue.Count > 0)
            {
                ExecuteInFinal(time, default, MakePlanDetail);
            }
        }

        private void MakePlanDetail(int time, IShipDockEntitas target)
        {
            PlanDetail detail = mPlanDetailQueue.Dequeue();
            detail.DrawUp(ref mWorldStateMapper);
        }

        private void CheckWorldGoal()
        {
        }
    }
}