using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example
{
    public class ScopeTest : MonoBehaviour
    {
        public Transform enemy;
        public float dis;
        public float minDistance;
        public float minAngle;
        public bool flag;

        ScopeChecker2 checker = new ScopeChecker2();

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            Test2();
        }

        private void Test1()
        {

            Vector3 newPos = transform.forward * minDistance;
            float distance = Vector3.Distance(transform.position, newPos);

            Quaternion right = transform.rotation * Quaternion.AngleAxis(minAngle, Vector3.up);
            Quaternion left = transform.rotation * Quaternion.AngleAxis(minAngle, Vector3.down);
            Quaternion up = transform.rotation * Quaternion.AngleAxis(minAngle, Vector3.left);
            Quaternion down = transform.rotation * Quaternion.AngleAxis(minAngle, Vector3.right);

            Vector3 n = transform.position + (Vector3.forward * minDistance);
            Vector3 leftPoint = left * n;
            Vector3 rightPoint = right * n;
            Vector3 upPoint = up * n;
            Vector3 downPoint = down * n;

            Debug.DrawLine(transform.position, leftPoint, Color.red);
            Debug.DrawLine(transform.position, rightPoint, Color.red);
            Debug.DrawLine(transform.position, upPoint, Color.red);
            Debug.DrawLine(transform.position, downPoint, Color.red);
            Debug.DrawLine(transform.position, enemy.position, Color.green);
            dis = Vector3.Distance(transform.position, enemy.position);

            if (flag)
            {
                CheckScope(dis, enemy.position);
            }
            else
            {
                checker.minAngle = minAngle;
                checker.minDistance = minDistance;
                //checker.forward = transform.forward;
                checker.startPos = transform.position;
                checker.CheckScope(enemy.position);
            }
        }

        private bool CheckScope(float distance, Vector3 enemyPos)
        {
            Vector3 targetDir = enemyPos - transform.position;
            Vector3 forward = transform.forward;
            float angle = Vector3.Angle(targetDir, forward);
            if (distance <= minDistance && angle < minAngle)
            {
                Debug.Log("Yes");
                return true;
            }
            return false;
        }

        public Transform Target;
        public float SkillDistance = 5;//扇形距离
        public float SkillJiaodu = 60;//扇形的角度

        void Test2()
        {
            dis = Vector3.Distance(transform.position, enemy.position);

            checker.minAngle = minAngle;
            checker.minDistance = minDistance;
            checker.startPos = transform.position;
            checker.startRotation = transform.rotation;
            checker.CheckScope(enemy.position);
        }

    }

    public class ScopeChecker
    {

        public float minAngle;
        public float minDistance;
        public Vector3 startPos;
        public Vector3 forward;
        public Quaternion startRotation;

        private float mDistance;

        public bool CheckScope(Vector3 targetPos)
        {
            Vector3 targetDir = targetPos - startPos;
            float angle = Vector3.Angle(targetDir, forward);
            mDistance = Vector3.Distance(startPos, targetPos);
            if (mDistance <= minDistance && angle < minAngle)
            {
                Debug.Log("Yes");
                return true;
            }
            return false;
        }

        public void CheckScope2(Vector3 targetPos)
        {
            float distance = Vector3.Distance(startPos, targetPos);//距离
            Vector3 norVec = startRotation * Vector3.forward;//此处*5只是为了画线更清楚,可以不要
            Vector3 temVec = targetPos - startPos;
            Debug.DrawLine(startPos, norVec, Color.red);//画出技能释放者面对的方向向量
            Debug.DrawLine(startPos, targetPos, Color.green);//画出技能释放者与目标点的连线
            float angle = Mathf.Acos(Vector3.Dot(norVec.normalized, temVec.normalized)) * Mathf.Rad2Deg;//计算两个向量间的夹角
            if (distance < minDistance)
            {
                if (angle <= minDistance * 0.5f)
                {
                    Debug.Log("在扇形范围内");
                }
            }
        }
    }

    public class ScopeChecker2
    {
        public float minAngle;
        public float minDistance;
        public Vector3 startPos;
        public Quaternion startRotation;

        private float mDistance;
        private float mAngle;
        private Vector3 mNorVec;
        private Vector3 mTemVec;

        public bool CheckScope(Vector3 targetPos)
        {
            mNorVec = startRotation * Vector3.forward;
            mTemVec = targetPos - startPos;

            Debug.DrawLine(startPos, mNorVec, Color.red);//画出技能释放者面对的方向向量
            Debug.DrawLine(startPos, targetPos, Color.green);//画出技能释放者与目标点的连线

            mDistance = Vector3.Distance(startPos, targetPos);//距离
            mAngle = Mathf.Acos(Vector3.Dot(mNorVec.normalized, mTemVec.normalized)) * Mathf.Rad2Deg;//计算夹角
            if (mDistance < minDistance)
            {
                if (mAngle <= minAngle * 0.5f)
                {
                    Debug.Log("在扇形范围内");
                    return true;
                }
            }
            return false;
        }
    }

}