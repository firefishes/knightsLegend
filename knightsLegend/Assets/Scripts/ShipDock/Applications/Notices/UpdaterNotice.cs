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
            ShipDockConsts.NOTICE_ADD_UPDATE.Dispatch(notice);
            Pooling<UpdaterNotice>.To(notice);
        }

        public static void RemoveUpdater(IUpdate target)
        {
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = target;
            ShipDockConsts.NOTICE_REMOVE_UPDATE.Dispatch(notice);
            Pooling<UpdaterNotice>.To(notice);
        }

        public static void AddSceneUpdater(IUpdate target)
        {
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = target;
            ShipDockConsts.NOTICE_ADD_SCENE_UPDATE.Dispatch(notice);
            Pooling<UpdaterNotice>.To(notice);
        }

        public static void RemoveSceneUpdater(IUpdate target)
        {
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = target;
            ShipDockConsts.NOTICE_REMOVE_SCENE_UPDATE.Dispatch(notice);
            Pooling<UpdaterNotice>.To(notice);
        }
    }
}
