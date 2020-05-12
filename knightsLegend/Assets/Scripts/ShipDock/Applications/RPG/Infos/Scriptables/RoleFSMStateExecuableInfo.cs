using System;

namespace ShipDock.Applications
{

    [Serializable]
    public class RoleFSMStateExecuableInfo
    {
        #region TODO Editorable
        public int phaseName;
        public int allowCalledInEntitas;
        public int callbackID;
        public bool isExecuteInScene;
        #endregion
    }

}