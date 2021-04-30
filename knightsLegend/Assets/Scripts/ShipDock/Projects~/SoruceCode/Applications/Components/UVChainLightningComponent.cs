//using ShipDock.Applications;
//using System.Collections.Generic;
//using UnityEngine;

/////<summary>
///// uv贴图闪电链
/////</summary>
//[ExecuteInEditMode]
//[RequireComponent(typeof(LineRenderer))]
//public class UVChainLightningComponent : MonoBehaviour
//{
//    //美术资源中进行调整
//    public float detail = 1;//增加后，线条数量会减少，每个线条会更长。
//    public float displacement = 15;//位移量，也就是线条数值方向偏移的最大值
//    public float yOffset = 0;
//    public Transform target;//链接目标
//    public Transform start;

//    private LineRenderer _lineRender;
//    private List<Vector3> _linePosList;
//    private UVChainLightning mLighting;

//    private void Awake()
//    {
//        _lineRender = GetComponent<LineRenderer>();
//        mLighting = new UVChainLightning(_lineRender);
//    }

//    private void Update()
//    {
//        if (mLighting != default)
//        {
//            mLighting.detail = detail;
//            mLighting.displacement = displacement;
//            mLighting.yOffset = yOffset;
//            mLighting.start = start.position;
//            mLighting.target = target.position;
//            mLighting.Update();
//            _linePosList = mLighting.LinePosList;
//        }
//    }
//}