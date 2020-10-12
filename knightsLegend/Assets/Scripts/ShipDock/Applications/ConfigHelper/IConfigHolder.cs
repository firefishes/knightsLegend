namespace ShipDock.Applications
{
    public interface IConfigHolder
    {
        void SetSource(ref byte[] bytes);
        void SetCongfigName(string name);
        string ConfigName { get; }
    }
}