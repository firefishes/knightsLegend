using ShipDock.Notices;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    public class WorldInteracter : INotificationSender
    {
        public static void Init<T>(int componentName, WorldInteracter item, EntitasComponentable entitas, GameObject gameObject, IWorldIntercatable target = default) where T : WorldComponent
        {
            T comp = entitas.GetComponentByName<T>(componentName);
            item = comp.GetEntitasData(ref entitas);
            item.worldItemID = gameObject.GetInstanceID();
            if (target != default)
            {
                item.Add(target.WorldItemHandler);
                item.WorldItemDispose = target.WorldItemDispose;
            }
        }

        public int worldItemID;
        public int groupID;
        public int aroundID;     
        public bool isDroped;

        public Action WorldItemDispose { get; set; }
    }
}