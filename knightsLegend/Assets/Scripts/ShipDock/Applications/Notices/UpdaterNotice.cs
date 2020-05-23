using System;
using ShipDock.Notices;
using ShipDock.Pooling;

namespace ShipDock.Applications
{
    public class UpdaterNotice : ParamNotice<IUpdate>
    {
        public static void AddUpdater(IUpdate target)
        {
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = target;
            ShipDockConsts.NOTICE_ADD_UPDATE.Broadcast(notice);
            Pooling<UpdaterNotice>.To(notice);
        }

        public static void RemoveUpdater(IUpdate target)
        {
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = target;
            ShipDockConsts.NOTICE_REMOVE_UPDATE.Broadcast(notice);
            Pooling<UpdaterNotice>.To(notice);
        }

        public static void AddSceneUpdater(IUpdate target)
        {
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = target;
            ShipDockConsts.NOTICE_ADD_SCENE_UPDATE.Broadcast(notice);
            Pooling<UpdaterNotice>.To(notice);
        }

        public static void RemoveSceneUpdater(IUpdate target)
        {
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = target;
            ShipDockConsts.NOTICE_REMOVE_SCENE_UPDATE.Broadcast(notice);
            Pooling<UpdaterNotice>.To(notice);
        }

        public static void SceneCallLater(Action<int> target)
        {
            ParamNotice<Action<int>> notice = Pooling<ParamNotice<Action<int>>>.From();
            notice.ParamValue = target;
            ShipDockConsts.NOTICE_SCENE_CALL_LATE.Broadcast(notice);
            Pooling<ParamNotice<Action<int>>>.To(notice);
        }

        internal static void AddSceneUpdater(object keepStanding)
        {
            throw new NotImplementedException();
        }
    }
}
