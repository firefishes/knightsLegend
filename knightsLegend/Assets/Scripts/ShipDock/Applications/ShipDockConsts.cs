using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public static class ShipDockConsts
    {
        public const int NOTICE_APPLICATION_STARTUP = -1000;
        public const int NOTICE_APPLICATION_CLOSE = -1001;
        public const int NOTICE_ADD_UPDATE = -1002;
        public const int NOTICE_REMOVE_UPDATE = -1003;
        public const int NOTICE_FRAME_UPDATER_COMP_READY = -1004;
        public const int NOTICE_ADD_SCENE_UPDATE = -1005;
        public const int NOTICE_REMOVE_SCENE_UPDATE = -1006;
        public const int NOTICE_SCENE_UPDATE_READY = -1007;
    }
}
