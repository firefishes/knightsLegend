using ShipDock.Datas;
using ShipDock.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLPlayerData : Data
    {
        private KLRole mUserRole;
        private KeyValueList<int, CampRoleModel> mCampRoleMapper;

        public KLPlayerData() : base(KLConsts.D_PLAYER)
        {
            mCampRoleMapper = new KeyValueList<int, CampRoleModel>();
        }

        internal void SetCurrentRole(KLRole role)
        {
            mUserRole = role;
        }

        internal void AddCampRole(KLRole role)
        {
            int key = mCampRoleMapper.Size;
            CampRoleModel model = new CampRoleModel
            {
                role = role,
                controllIndex = key
            };
            mCampRoleMapper[key] = model;

            DataChanged(KLConsts.DC_CAMP_ROLE_CREATED);
        }
    }
}
