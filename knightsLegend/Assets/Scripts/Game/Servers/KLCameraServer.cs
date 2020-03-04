using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;

namespace KLGame
{
    public class KLCameraServer : CamerasServer<KLCamerasComponent, KLPlayerData>
    {
        public override void InitServer()
        {
            base.InitServer();

            Register<IParamNotice<KLRoleComponent>>(PlayerRole_0_Paramer, Pooling<ParamNotice<KLRoleComponent>>.Instance);

        }

        [Resolvable("PlayerRole_0")]
        private void PlayerRole_0_Paramer(ref IParamNotice<KLRoleComponent> target) { }
        
        public override void ServerReady()
        {
            base.ServerReady();

            Add<IParamNotice<KLRoleComponent>>(InitPlayerRoleLen);
        }

        [Callable("InitPlayerRoleLen", "PlayerRole_0")]
        private void InitPlayerRoleLen<I>(ref I target)
        {
            KLRoleComponent role = (target as IParamNotice<KLRoleComponent>).ParamValue;
            SetCameraParent(role.CameraNode);
        }

        protected override string LensServerName { get; } = KLConsts.S_LENS;
        protected override int DataName { get; } = KLConsts.D_PLAYER;
    }
}
