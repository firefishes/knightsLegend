namespace ShipDock.Applications
{
    public interface IUserInputerButtons
    {
        void AddButtonMap(string key, string buttonName);
        bool GetButton(string key);
        float GetAxis(string key);
        void SetAxis(string key, float value);
        void SetActiveButton(string key, bool value);
    }
}
