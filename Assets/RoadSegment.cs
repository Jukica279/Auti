using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using Unity.Mathematics;

public class RoadSegment : MonoBehaviour
{
    public SplineContainer splineContainer;
    private Spline spline;
    public float SplineLength { get; private set; }

    public float roadWidth = 5f;



    private void Awake()
    {
        spline = splineContainer.Spline;
        SplineLength = spline.GetLength();
    }


    public Vector3 GetPositionOnSpline(float t, float roadPosition)
    {
        float3 position;
        float3 tangent;
        float3 upVector = new float3(0, 1, 0);


        SplineUtility.Evaluate(spline, t, out position, out tangent, out upVector);

        float3 right = math.normalize(math.cross(tangent, new float3(0, 1, 0)));
        float3 offset = -right * (roadPosition - 0.5f) * roadWidth;
        return new Vector3(position.x + offset.x, position.y + offset.y, position.z + offset.z);
    }

    public Vector3 GetForwardDirection(float t)
    {
        float3 position;
        float3 tangent;
        float3 upVector = new float3(0, 1, 0);

        SplineUtility.Evaluate(spline, t, out position, out tangent, out upVector);
        return new Vector3(tangent.x, tangent.y, tangent.z).normalized;
    }

    public Vector3 GetEndPoint()
    {
        var lastSpline = spline[spline.Count - 1];
        return transform.TransformPoint((Vector3)lastSpline.Position);
    }
}


