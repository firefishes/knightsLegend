using UnityEngine;

namespace ShipDock.Applications
{
    public class RoleCollider : MonoBehaviour
    {
        [SerializeField]
        private RoleComponent m_RoleSceneComp;

        private ICommonRole mRoleEnitas;

        private void Awake()
        {
            mRoleEnitas = m_RoleSceneComp.RoleEntitas;
        }

        private void OnDestroy()
        {
            m_RoleSceneComp = default;
            mRoleEnitas = default;
        }

        private void OnTriggerEnter(Collider other)
        {
            int id = other.GetInstanceID();
            if (mRoleEnitas != default && !mRoleEnitas.CollidingRoles.Contains(id))
            {
                mRoleEnitas.CollidingRoles.Add(id);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            int id = other.GetInstanceID();
            if (mRoleEnitas != default && mRoleEnitas.CollidingRoles.Contains(id))
            {
                mRoleEnitas.CollidingRoles.Remove(id);
            }
        }
    }

}