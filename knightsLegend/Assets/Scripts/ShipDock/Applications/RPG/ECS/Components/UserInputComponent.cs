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

            if (mRoleItem.IsUserControlling && (mRoleInput != default) && (mRoleInput.ShouldGetUserInput))
            {
                CheckUserInput();
                mRoleInput.ShouldGetUserInput = false;
            }
            if (mRoleInput == default)
            {
                return;
            }
            IUserInputPhase inputPhase = mRoleInput.GetUserInputPhase();
            inputPhase?.ExecuteByEntitasComponent();
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
