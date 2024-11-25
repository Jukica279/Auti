using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class SplineMover : MonoBehaviour
{
    public float roadPosition; // 0 = left, 1 = right
    public float currentT;
    public float speed = 5f;
    private RoadSegment currentSegment;

    private void Start()
    {
        currentSegment = transform.parent.GetComponent<RoadSegment>();
    }
    private void Update()
    {
        UpdatePosition(Time.deltaTime);
    }
    public virtual void UpdatePosition(float deltaTime)
    {

        currentT += (speed * deltaTime) / currentSegment.SplineLength;

        if (currentT >= 1f && speed > 0)
        {
            OnSegmentComplete();
        }
        else if (currentT <= 0f && speed < 0)
        {
            OnSegmentComplete();
        }



        // Get the new position on the spline (local space) and transform it to world space
        Vector3 localPosition = currentSegment.GetPositionOnSpline(currentT, roadPosition);
        Vector3 newPosition = currentSegment.transform.TransformPoint(localPosition);

        // Update position if the values are valid
        if (!float.IsNaN(newPosition.x) && !float.IsNaN(newPosition.y) && !float.IsNaN(newPosition.z))
        {
            transform.position = newPosition;
        }

        // Get the forward direction at the current position (local space) and transform it to world space
        Vector3 localForward = currentSegment.GetForwardDirection(currentT).normalized;
        Vector3 worldForward = currentSegment.transform.TransformDirection(localForward);

        transform.rotation = Quaternion.LookRotation(worldForward);

    }


    protected virtual void OnSegmentComplete()
    {
        if (speed >= 0)
        {
            currentT = 0f;
        }
        else
        {
            currentT = 1f;
        }
    }
}