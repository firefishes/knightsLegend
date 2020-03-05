using ShipDock.ECS;
using UnityEngine;
#if CROSS_PLATFORM_INPUT
using UnityStandardAssets.CrossPlatformInput;
#endif

namespace ShipDock.Applications
{
    public class UserInputComponent<S> : ShipDockComponent where S : MainServer
    {

        protected float mInputX;
        protected float mInputY;
        protected Vector3 mInputV;
        protected ICommonRole mRoleItem;

        private bool mIsRelaterInited;
        private IRoleData mRoleData;
        private IRoleInput mRoleInput;
        private ServerRelater mRelater;
        private CommonRoleAnimatorInfo mAnimatorInfo;

        public override void Init()
        {
            base.Init();

            mRelater = new ServerRelater()
            {
                ServerNames = RelateServerNames
            };
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mRoleItem = target as ICommonRole;
            mRoleData = mRoleItem.RoleDataSource;
            mRoleInput = mRoleItem.RoleInput;
            mAnimatorInfo = mRoleItem.RoleAnimatorInfo;

            if (mRoleItem.IsUserControlling)
            {
                CheckUserInput();
            }
            if (mRoleInput == default)
            {
                return;
            }
            switch (mRoleInput.RoleMovePhase)
            {
                case UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY:
                    if (mRoleInput.GetMoveValue().magnitude > 1f)
                    {
                        mRoleInput.MoveValueNormalize();
                    }
                    break;
                case UserInputPhases.ROLE_INPUT_PHASE_AMOUT_EXTRAN_TURN:
                    Vector3 move = Vector3.ProjectOnPlane(mRoleInput.GetMoveValue(), mRoleItem.GroundNormal);
                    mRoleInput.SetMoveValue(move);
                    mRoleInput.UpdateAmout(ref mRoleItem);
                    mRoleInput.UpdateRoleExtraTurnRotation(ref mRoleData);
                    mRoleInput.UpdateMovePhase();
                    break;
                case UserInputPhases.ROLE_INPUT_PHASE_SCALE_CAPSULE:
                    mRoleInput.ScaleCapsuleForCrouching(ref mRoleItem, ref mRoleInput);
                    mRoleInput.UpdateMovePhase();
                    break;
            }
        }

        protected T GetMainServer<T>() where T : MainServer
        {
            return mRelater.ServerRef<T>(MainServerName);
        }

        protected virtual void CheckUserInput()
        {
            if (MainInputer == default)
            {
                CheckRelaterInited();
                S server = GetMainServer<S>();
                MainInputer = server.MainInputer;
            }
            GetUserInput();
            mRoleInput?.SetUserInputValue(mInputV);
        }

        protected virtual void GetUserInput()
        {
            mInputX = GetHorizontal();
            mInputY = GetVertical();
            mInputV = new Vector3(mInputX, mInputY);
        }

        protected virtual float GetHorizontal()
        {
#if CROSS_PLATFORM_INPUT
            return CrossPlatformInputManager.GetAxis("Horizontal");
#else
            return 0f;
#endif
        }

        protected virtual float GetVertical()
        {
#if CROSS_PLATFORM_INPUT
            return CrossPlatformInputManager.GetAxis("Vertical");
#else
            return 0f;
#endif
        }

        private void CheckRelaterInited()
        {
            if (!mIsRelaterInited)
            {
                mIsRelaterInited = true;
                mRelater.CommitRelate();
            }
        }

        private IInputer MainInputer { get; set; }
        protected virtual string[] RelateServerNames { get; }
        protected virtual string MainServerName { get; }
    }

}
