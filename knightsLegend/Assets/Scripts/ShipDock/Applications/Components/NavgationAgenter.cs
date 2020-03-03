using ShipDock.Applications;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Tkzs.Game
{
    public class NavgationAgenter : MonoBehaviour
    {
        [SerializeField]
        private Transform m_NavTarget;
        [SerializeField]
        private NavMeshAgent m_NavAgent;

        private Updater mUpdater = new Updater();

        void Start()
        {
            m_NavAgent.speed = UnityEngine.Random.Range(0.1f, 1f) * 10;
            transform.localScale = Vector3.one * UnityEngine.Random.Range(0.2f, 2f);

            ShipDockApp.Instance.AddStart(OnAppStart);
        }

        private void OnAppStart()
        {
            UpdaterNotice notice = new UpdaterNotice();
            mUpdater.name = name;
            notice.ParamValue = mUpdater;
            (notice.ParamValue as Updater).Update = UpdatePos;

            ShipDockConsts.NOTICE_ADD_UPDATE.Dispatch(notice);

            notice.Dispose();
        }

        private bool m;

        // Use this for initialization
        void Update()
        {
            //if (!mUpdater.Asynced)
            //{
            //    return;
            //}
            m_NavAgent.destination = pos;// m_NavTarget.position;
            pos = m_NavTarget.position;
            //UpdatePos();

            if (mUpdater.Log > 0)
            {
                Debug.Log(name + " " + mUpdater.Log);
            }
        }

        private Vector3 pos;
        private int max = 100000;

        private void UpdatePos()
        {
            while(max > 0)
            {
                max--;
            }
            max = 100000;
            mUpdater.Log++;
        }
    }

    public class Updater : IUpdate
    {
        public string name { get; set; }
        public int Log { get; set; }

        public bool IsUpdate { get { return true; } }

        public bool IsFixedUpdate { get; set; }

        public bool IsLateUpdate { get; set; }

        public void AddUpdate()
        {
        }

        public void OnFixedUpdate(int dTime)
        {
        }

        public void OnLateUpdate()
        {
            Asynced = true;
        }

        public void OnUpdate(int dTime)
        {
            Update?.Invoke();
        }

        public void RemoveUpdate()
        {
        }

        public Action Update { get; set; }
        public bool Asynced { get; set; }
    }
}
