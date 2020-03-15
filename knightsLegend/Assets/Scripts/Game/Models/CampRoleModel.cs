
namespace KLGame
{

    public class CampRoleModel
    {
        public int controllIndex;
        public KLRole role;

        public int GetRoleConfigID()
        {
            return role.RoleDataSource.ConfigID;
        }

        public void SetUserControll(bool flag)
        {
            role.IsUserControlling = flag;
            role.PositionEnabled = !flag;
            role.SpeedCurrent = role.Speed;
        }
    }
}

