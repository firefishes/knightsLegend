using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using UnityEngine;

namespace ShipDock.Applications
{
    public class CamerasServer<C, D> : Server.Server, IDataExtracter where C : ICamerLens where D : IData
    {
        private C mLens;
        private ServerRelater mRelater;

        public CamerasServer()
        {
            ServerName = LensServerName;

            mRelater = new ServerRelater()
            {
                DataNames = new int[]
                {
                    DataName
                }
            };
        }

        public override void InitServer()
        {
            base.InitServer();

            Register<IParamNotice<ICamerLens>>(SetLensParamer, Pooling<ParamNotice<ICamerLens>>.Instance);
            
        }
        
        [Resolvable("SetLensParamer")]
        private void SetLensParamer(ref IParamNotice<ICamerLens> target) { }

        public override void ServerReady()
        {
            base.ServerReady();

            mRelater.CommitRelate();
            IData playerData = mRelater.DataRef<IData>(DataName);
            playerData.Register(this);

            Add<IParamNotice<ICamerLens>>(SetLens);
        }

        protected void SetCameraParent(Transform p)
        {
            Transform tf = mLens.CameraFollower.transform;
            tf.SetParent(p);
            tf.localPosition = Vector3.zero;
            tf.localRotation = p.localRotation;
        }

        [Callable("SetLens", "SetLensParamer")]
        private void SetLens<I>(ref I target)
        {
            mLens = (C)(target as IParamNotice<ICamerLens>).ParamValue;
        }

        public virtual void OnDataChanged(IData data, int keyName)
        {
        }

        protected virtual string LensServerName { get; }
        protected virtual int DataName { get; }
    }
}
