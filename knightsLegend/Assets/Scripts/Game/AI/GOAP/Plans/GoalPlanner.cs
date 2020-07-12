﻿using ShipDock.Tools;
using System.Collections.Generic;

namespace KLGame
{
    public class GoalPlanner : IGoalPlanner
    {
        private int mInfeasibleIndex;
        private IWorldState mStateItem;
        private IAIExecutable mExecutable;
        private WorldStatesMapper mWorldStates;
        private List<IWorldState> mStatesChecking;

        public GoalPlanner()
        {
        }

        public void WillPlan(IGoalExecuter receiver)
        {
            PlanReceiver = receiver as IAIRole;
        }

        private bool ShouldPlan(ref IGoal goal)
        {
            return goal.Available && goal.ShouldPlanByReceiver(PlanReceiver);
        }

        private void AssignmentGoal(ref List<IGoal> goals, out bool isStartPlan)
        {
            isStartPlan = false;
            int max = goals.Count;
            IGoal goal;
            for (int i = 0; i < max; i++)
            {
                goal = goals[i];
                isStartPlan = ShouldPlan(ref goal);
                if (isStartPlan)
                {
                    PlanReceiver.SetGoal(goal);
                    break;
                }
            }
        }

        public void StartPlan(ref List<IGoal> goals, ref List<IWorldState> worldStates, ref Queue<PlanDetail> planDetailQueue)
        {
            if (PlanReceiver != default)
            {
                AssignmentGoal(ref goals, out bool isStartPlan);

                if (isStartPlan)
                {
                    IGoal goal = PlanReceiver.CurrentGoal;
                    IAIExecutable item;
                    IAIExecutable[] executables = PlanReceiver.AIExecutables;
                    int max = executables.Length;
                    for (int i = 0; i < max; i++)
                    {
                        item = executables[i];
                        item.ApplyPlan(ref goal, ref worldStates);
                    }
                    PlanDetail planDetail = PlanDetail.Obtain();
                    planDetail.Reinit(goal, this, ref executables);
                    planDetailQueue.Enqueue(planDetail);
                }
            }
        }

        private void CheckFeasible(ref List<IWorldState> states, ref bool feasible)
        {
            if (feasible)
            {
                mInfeasibleIndex = -1;
                int max = states.Count;
                for (int i = 0; i < max; i++)
                {
                    mStateItem = states[i];
                    mExecutable.CheckFeasible(mStateItem, out int feasibleStatu);
                    feasible = feasibleStatu != 2;
                    if (feasible)
                    {
                        mExecutable.CommitEffect(ref mStateItem);
                    }
                    else
                    {
                        mInfeasibleIndex = i;
                        break;
                    }
                }

                if (!feasible)
                {
                    for (int i = 0; i < mInfeasibleIndex; i++)
                    {
                        mStateItem = states[i];
                        mStateItem.RevertEffect();
                    }
                }
            }
        }

        private void CheckFeasibleByOriented(int checkOriented, ref bool isAllFeasible)
        {
            int oriented = mExecutable.OrientedType;
            bool hasOriented = Utils.IsContains(oriented, checkOriented);
            if (hasOriented)
            {
                mWorldStates.RefOrientedStates(checkOriented, ref mStatesChecking);
                CheckFeasible(ref mStatesChecking, ref isAllFeasible);
            }
            //TODO 状态回退
        }

        public void Planning(int index, ref IGoal goal, ref WorldStatesMapper worldStates, ref IAIExecutable[] executables, ref PlanGraphic planGraphic)
        {
            bool isAllFeasible = true;

            mStatesChecking = default;
            mExecutable = executables[index];
            mWorldStates = worldStates;

            CheckFeasibleByOriented(KLConsts.WORLD_STATE_ORIENTED_NONE, ref isAllFeasible);
            CheckFeasibleByOriented(KLConsts.WORLD_STATE_ORIENTED_OBJECTIVE, ref isAllFeasible);
            CheckFeasibleByOriented(KLConsts.WORLD_STATE_ORIENTED_ONESELF, ref isAllFeasible);

            planGraphic.AddNode(mExecutable, index > 0 && !isAllFeasible);

            index++;
            if (index < executables.Length)
            {
                Planning(index, ref goal, ref worldStates, ref executables, ref planGraphic);
            }
            UnityEngine.Debug.Log(ShipDock.Applications.ShipDockApp.Instance.TicksUpdater.LastRunTime + "-" + PlanReceiver.ToString());
        }
        
        private void PlanFinish()
        {
            PlanReceiver = default;
        }

        protected IAIRole PlanReceiver { get; private set; }
    }
}