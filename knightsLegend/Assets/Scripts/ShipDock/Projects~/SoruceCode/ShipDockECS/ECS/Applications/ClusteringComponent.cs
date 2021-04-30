using ShipDock.ECS;

namespace ShipDock.Applications
{
    public class ClusteringComponent : DataComponent<ClusteringData>
    {
        protected override ClusteringData CreateData()
        {
            ClusteringData data = new ClusteringData();
            data.SetWorldGroupID(int.MaxValue);
            return new ClusteringData();
        }
    }

    public class ClusteringData
    {
        public void SetWorldGroupID(int id)
        {
            WorldGroupID = id;
        }

        public void MakeClusteringCenter(int gourpID)
        {
            IsCenter = true;
            WorldGroupID = gourpID;
        }

        public void DissolutionClustering()
        {
            if (IsDissolutionGroup)
            {
                IsCenter = false;
                WorldGroupID = int.MaxValue;
            }
        }

        public bool IsCenter { get; private set; }
        public bool IsGroupCached { get; set; }
        public bool IsDissolutionGroup { get; set; }
        public int WorldGroupID { get; private set; }
        public float ClusteringMag { get; set; }
    }
}