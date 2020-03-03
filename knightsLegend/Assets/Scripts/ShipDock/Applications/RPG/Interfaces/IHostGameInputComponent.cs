using ShipDock.ECS;

namespace ShipDock.Applications
{
    public interface IHostGameInputComponent : IShipDockComponent
    {
        void SetUserInputerButtons(IUserInputerButtons target);
        IUserInputerButtons UserInputButtons { get; }
    }
}
