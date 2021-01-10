using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using UnityEngine;

namespace ShipDock.Applications
{
    public class MainServer : Server.Server
    {
        private readonly static IResolvableConfig[] ServerConfigs = new IResolvableConfig[]
        {
            new ResolvableConfigItem<INotice, Notice>("Notice"),
            new ResolvableConfigItem<IParamNotice<int>, ParamNotice<int>>("Int"),
            new ResolvableConfigItem<IParamNotice<bool>, ParamNotice<bool>>("Bool"),
            new ResolvableConfigItem<IParamNotice<Vector3>, ParamNotice<bool>>("V3"),
            new ResolvableConfigItem<IParamNotice<string>, ParamNotice<string>>("string"),
        };

        public MainServer(string serverName)
        {
            ServerName = serverName;
        }

        public override void InitServer()
        {
            base.InitServer();

            ServersHolder.AddResolvableConfig(ServerConfigs);

            Register<INotice>(NoticeResolver, Pooling<Notice>.Instance);
            Register<IParamNotice<int>>(IntParamerResolver, Pooling<ParamNotice<int>>.Instance);
            Register<IParamNotice<bool>>(BoolParamerResolver, Pooling<ParamNotice<bool>>.Instance);
            Register<IParamNotice<Vector3>>(V3ParamerResolver, Pooling<ParamNotice<Vector3>>.Instance);
            Register<IParamNotice<string>>(StringParamerResolver, Pooling<ParamNotice<string>>.Instance);
        }

        public override void ServerReady()
        {
            base.ServerReady();
            
            Add<IParamNotice<bool>>(SetBoolTrue);
            Add<IParamNotice<bool>>(SetBoolFalse);
        }

        [Resolvable("V3")]
        private void V3ParamerResolver(ref IParamNotice<Vector3> target) { }

        [Resolvable("Bool")]
        private void BoolParamerResolver(ref IParamNotice<bool> target) { }
        
        [Resolvable("Int")]
        private void IntParamerResolver(ref IParamNotice<int> target) { }

        [Resolvable("Notice")]
        private void NoticeResolver(ref INotice target) { }

        [Callable("True", "Bool")]
        private void SetBoolTrue(ref IParamNotice<bool> target)
        {
            target.ParamValue = true;
        }

        [Callable("False", "Bool")]
        private void SetBoolFalse(ref IParamNotice<bool> target)
        {
            target.ParamValue = false;
        }

        [Resolvable("string")]
        private void StringParamerResolver(ref IParamNotice<string> target) { }
    }
}
