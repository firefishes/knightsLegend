namespace ShipDock.Applications
{
    public interface IUpdate
	{
		void AddUpdate();
		void RemoveUpdate();
        void OnLateUpdate();
        void OnUpdate(int dTime);
		void OnFixedUpdate(int dTime);
        bool IsUpdate { get; }
        bool IsFixedUpdate { get; }
        bool IsLateUpdate { get; }
    }
}