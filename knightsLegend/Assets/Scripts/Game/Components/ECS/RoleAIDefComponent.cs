using ShipDock.Applications;
using ShipDock.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class RoleAIDefComponent : ShipDockComponent
    {
        private IAIRole mRole;
        private TimingTasker mDefTimingTask;

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mRole = target as IAIRole;
            DefenceAnticipathion();
        }

        private void DefenceAnticipathion()
        {
            if (mRole.Anticipathioner != default && !mRole.Anticipathioner.IsExecuted)
            {
                int stateFrom = mRole.Anticipathioner.StateFrom;
                switch (stateFrom)
                {
                    case NormalRoleStateName.NORMAL_ATK:
                        mRole.Anticipathioner.IsExecuted = true;
                        mDefTimingTask = mRole.TimesEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, 1);
                        if (!mDefTimingTask.IsStart)
                        {
                            mDefTimingTask.Start(0f);
                        }
                        break;
                }
            }
        }
        
    }

}