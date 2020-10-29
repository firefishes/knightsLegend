using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// 场景占位
    /// 
    /// </summary>
    public class SceneSignSphereCross : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private bool m_WorkingInPlaying;
        [SerializeField]
        private bool m_HasSphere;
        [SerializeField]
        private bool m_HasLine = true;
        [SerializeField]
        private float m_Radius = 1; // 圆环的半径
        [SerializeField]
        private Color m_Color = Color.green; // 线框颜色
        [SerializeField]
        [Range(0.001f, 2.2f)]
        private float m_Theta = 0.1f; // 值越低圆环越平滑
        [SerializeField]
        [Range(0, 1)]
        private float m_ColorAlpha = 0.2f;
        [SerializeField]
        [Range(0, 5)]
        private float m_LineValue = 3;

        void OnDrawGizmos()
        {
            if (Application.isPlaying != m_WorkingInPlaying)
            {
                return;
            }

            if (m_Theta < 0.0001f)
            {
                m_Theta = 0.0001f;
            }

            // 设置矩阵
            Matrix4x4 defaultMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            // 设置颜色
            Color defaultColor = Gizmos.color;
            Gizmos.color = m_Color;

            // 绘制圆环
            Vector3 beginPoint = Vector3.zero;
            Vector3 firstPoint = Vector3.zero;
            Vector3 endPoint = Vector3.zero;
            for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
            {
                float x = m_Radius * Mathf.Cos(theta);
                float z = m_Radius * Mathf.Sin(theta);
                endPoint = new Vector3(x, 0, z);
                if (theta == 0)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint, endPoint);
                }
                beginPoint = endPoint;
            }

            // 绘制最后一条线段
            Gizmos.DrawLine(firstPoint, beginPoint);

            // 恢复默认颜色
            Gizmos.color = defaultColor;

            // 恢复默认矩阵
            Gizmos.matrix = defaultMatrix;

            if (m_HasSphere)
            {
                Gizmos.color = new Color(m_Color.r, m_Color.g, m_Color.b, m_ColorAlpha);
                Gizmos.DrawSphere(transform.position, m_Radius);
            }

            if (m_HasLine)
            {
                Gizmos.color = m_Color;
                Vector3 a = transform.position + new Vector3(-m_LineValue, 0, m_LineValue);
                Vector3 b = transform.position + new Vector3(m_LineValue, 0, -m_LineValue);
                Vector3 c = transform.position + new Vector3(m_LineValue, 0, m_LineValue);
                Vector3 d = transform.position + new Vector3(-m_LineValue, 0, -m_LineValue);
                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(c, d);
            }
        }
#endif
    }

}