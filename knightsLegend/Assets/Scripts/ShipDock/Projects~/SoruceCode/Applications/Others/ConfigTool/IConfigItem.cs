using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public interface IConfigItem
    {
        int ID { get; }
        void SetID(int id);
    }
}