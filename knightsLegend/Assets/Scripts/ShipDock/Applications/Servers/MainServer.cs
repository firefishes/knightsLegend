using ShipDock.Server;
using ShipDock.Notices;
using ShipDock.Pooling;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public class MainServer : Server.Server
    {
        public readonly static List<IResolvableConfig> ServerConfigs = new List<IResolvableConfig>
        {
            new ResolvableConfigItem<INotice, Notice>("Notice"),
            new ResolvableConfigItem<IParamNotice<int>, ParamNotice<int>>("Int"),
            new ResolvableConfigItem<IParamNotice<bool>, ParamNotice<bool>>("Bool"),
            new ResolvableConfigItem<IParamNotice<IInputer>, ParamNotice<IInputer>>("InputerParamer"),
            new ResolvableConfigItem<IParamNotice<IInputer>, ParamNotice<IInputer>>("SetInputerParamer"),
        };

        public MainServer(string serverName)
        {
            ServerName = serverName;
        }

        public override void InitServer()
        {
            base.InitServer();

            Register<INotice>(NoticeResolver, Pooling<Notice>.Instance);
            Register<IParamNotice<int>>(IntParamerResolver, Pooling<ParamNotice<int>>.Instance);
            Register<IParamNotice<bool>>(BoolParamerResolver, Pooling<ParamNotice<bool>>.Instance);
            Register<IParamNotice<IInputer>>(SetInputerParamer, Pooling<ParamNotice<IInputer>>.Instance);
            Register<IParamNotice<IInputer>>(GetInputerParamer, Pooling<ParamNotice<IInputer>>.Instance);
        }

        public override void ServerReady()
        {
            base.ServerReady();
            
            Add<IParamNotice<IInputer>>(SetInputer);
            Add<IParamNotice<bool>>(SetBoolTrue);
            Add<IParamNotice<bool>>(SetBoolFalse);
        }

        [Resolvable("Bool")]
        private void BoolParamerResolver(ref IParamNotice<bool> target) { }
        
        [Resolvable("Int")]
        private void IntParamerResolver(ref IParamNotice<int> target) { }

        [Resolvable("Notice")]
        private void NoticeResolver(ref INotice target) { }

        [Resolvable("SetInputerParamer")]
        private void SetInputerParamer(ref IParamNotice<IInputer> target) { }

        [Resolvable("InputerParamer")]
        private void GetInputerParamer(ref IParamNotice<IInputer> target)
        {
            target.ParamValue = MainInputer;
        }

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

        [Callable("SetInputer", "SetInputerParamer")]
        private void SetInputer<I>(ref I target)
        {
            IParamNotice<IInputer> notice = target as IParamNotice<IInputer>;
            MainInputer = notice.ParamValue;
            MainInputer.CommitAfterSetToServer();
        }

        public IInputer MainInputer { get; protected set; }
    }
}
