namespace ShipDock.Applications
{
    public interface IStartingUpdatePopup
    {
        float Loaded { get; set; }
        float LoadingCount { get; set; }
        void Close();
        void LoadingUpdate();
    }
}