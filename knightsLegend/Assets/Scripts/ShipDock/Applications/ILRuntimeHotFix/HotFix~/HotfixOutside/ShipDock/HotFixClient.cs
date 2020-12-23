namespace ShipDock.HotFix
{
    public class HotFixClient
    {
        private static HotFixClient instance;

        public static HotFixClient Instance
        {
            get
            {
                if (instance == default)
                {
                    instance = new HotFixClient();
                }
                return instance;
            }
        }

        public DecorativeModulars Modulars { get; private set; }

        private HotFixClient()
        {
            Modulars = new DecorativeModulars();
        }
    }
}
