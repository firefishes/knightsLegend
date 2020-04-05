using ShipDock.Notices;
using ShipDock.Server;
using UnityEngine;

namespace ShipDock.Applications
{
    public class InputerComponent : MonoBehaviour, IInputer
    {
        [SerializeField]
        protected string m_MainServerName;

        private ComponentBridge mCompBrigde;

        protected ServerRelater mRelater;

        protected virtual void Awake()
        {
            mRelater = new ServerRelater
            {
                ComponentNames = RelatedComponentNames
            };

            mCompBrigde = new ComponentBridge(OnInited);
            mCompBrigde.Start();
        }

        private void OnDestroy()
        {
            
        }

        protected virtual void Purge()
        {

        }

        protected virtual void OnInited()
        {
            mCompBrigde.Dispose();
            mCompBrigde = default;

            mRelater.CommitRelate();

            SetMainServerName();
            MainServerdName.DeliveParam<MainServer, IInputer>("SetInputer", "SetInputerParamer", OnSetInputer, true);
        }

        protected virtual void SetMainServerName()
        {
            MainServerdName = m_MainServerName;
        }

        [Resolvable("SetInputerParamer")]
        private void OnSetInputer(ref IParamNotice<IInputer> target)
        {
            target.ParamValue = this;
        }

        public virtual void CommitAfterSetToServer()
        {
        }

        protected virtual int[] RelatedComponentNames { get; }

        public virtual string MainServerdName { get; protected set; }
    }
}
