using ShipDock.Applications;

namespace KLGame
{
    public class KLRole : RoleEntitas
    {

        protected override int[] ComponentIDs { get; } = new int[]
        {
            KLConsts.C_ROLE_INPUT,
            KLConsts.C_POSITION,
            KLConsts.C_ROLE_COLLIDER,
            KLConsts.C_ROLE_MUST,
            KLConsts.C_ROLE_CAMP
        };
    }
}
