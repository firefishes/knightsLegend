namespace ShipDock.Applications
{
    public static class HotFixBaseUIExtensions
    {
        public static T GetUI<T>(this HotFixerUIAgent UIAgent) where T : HotFixBaseUI
        {
            return (T)UIAgent.Bridge.HotFixerInteractor;
        }
    }

    public abstract class HotFixBaseUI : HotFixerInteractor
    {
        public override void UpdateInteractor() { }

        public virtual UIModularHotFixer HotFixUIExit()
        {
            return UIModular;
        }
    }
}
