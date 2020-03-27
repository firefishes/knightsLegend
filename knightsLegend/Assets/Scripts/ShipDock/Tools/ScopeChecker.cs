using UnityEngine;

namespace ShipDock.Tools
{

    public class ScopeChecker
    {
        public float minAngle;
        public float minDistance;
        public Vector3 startPos;
        public Quaternion startRotation;

        private float mDistance;
        private float mAngle;
        private Vector3 mTargetVec;
        private Vector3 mNorVec;
        private Vector3 mTemVec;

        public bool CheckScope(Vector3 targetPos)
        {
            mTargetVec = targetPos;
            mNorVec = startRotation * Vector3.forward;
            mTemVec = mTargetVec - startPos;
            
            mDistance = Vector3.Distance(startPos, mTargetVec);//距离
            mAngle = Mathf.Acos(Vector3.Dot(mNorVec.normalized, mTemVec.normalized)) * Mathf.Rad2Deg;//夹角
            if (mDistance < minDistance)
            {
                if (mAngle <= minAngle * 0.5f)
                {
                    return true;
                }
            }
            return false;
        }

        public void Draws()
        {
            Debug.DrawLine(startPos, startRotation * Vector3.forward, Color.red);//画出技能释放者面对的方向向量
            Debug.DrawLine(startPos, mTargetVec, Color.green);//画出技能释放者与目标点的连线
        }
    }
}