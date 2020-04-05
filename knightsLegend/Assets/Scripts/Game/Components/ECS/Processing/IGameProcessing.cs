using ShipDock.Pooling;
using System;

namespace KLGame
{
    public interface IGameProcessing : IPoolable
    {
        void OnProcessing();
        void ProcessingReady();
        void ToPooling();
        Action AfterProcessing { get; set; }
        IKLRole Initiator { get; set; }
        IKLRole Target { get; set; }
        bool Finished { get; set; }
        int Type { get; }
    }

}