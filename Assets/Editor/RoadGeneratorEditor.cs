using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;

[ExecuteInEditMode]
public class RoadGeneratorEditor : MonoBehaviour
{
    private SplineContainer m_splineContainer;

    [SerializeField]
    private int m_splineIndex;
    [SerializeField]
    [Range(0f, 1f)]
    private float m_time;

    public MeshFilter m_meshFilter;

    [SerializeField]
    float m_width;
    [SerializeField]
    int resolution;

    List<Vector3> m_vertsP1 = new List<Vector3>();
    List<Vector3> m_vertsP2 = new List<Vector3>();

    private float lastUpdateTime = 0f;
    private const float updateInterval = 1f; // Update every second

    private void OnEnable()
    {
        EditorApplication.update += EditorUpdate; // Subscribe to editor update
        Spline.Changed += OnSplineChanged;
        m_splineContainer = GetComponent<SplineContainer>();
        m_meshFilter = gameObject.GetComponent<MeshFilter>();
        Rebuild();
    }

    private void OnDisable()
    {
        EditorApplication.update -= EditorUpdate; // Unsubscribe to avoid leaks
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        Rebuild();
    }

    private void EditorUpdate()
    {
        // Rebuild every `updateInterval` seconds
        if (Time.realtimeSinceStartup - lastUpdateTime >= updateInterval)
        {
            Rebuild();
            lastUpdateTime = Time.realtimeSinceStartup;
        }
    }

    private void Rebuild()
    {
        GetVerts();
        BuildMesh();
    }

    private void GetVerts()
    {
        m_vertsP1 = new List<Vector3>();
        m_vertsP2 = new List<Vector3>();

        int totalPoints = resolution + 1; // Include endpoint
        float step = 1f / (float)(totalPoints - 1); // Adjust step to include t=1.0
        Vector3 offset = new Vector3(0, 0.1f, 0);

        for (int i = 0; i < totalPoints; i++) // Loop includes endpoint
        {
            float t = step * i;
            SampleSplineWidth(t, out Vector3 p1, out Vector3 p2);
            m_vertsP1.Add(p1 - transform.position + offset);
            m_vertsP2.Add(p2 - transform.position + offset);
        }
    }

    private void BuildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();
        int offset = 0;
        float uvOffset = 0;
        int length = m_vertsP2.Count;

        for (int i = 1; i < length; i++)
        {
            Vector3 p1 = m_vertsP1[i - 1];
            Vector3 p2 = m_vertsP2[i - 1];
            Vector3 p3;
            Vector3 p4;

            if (i == length)
            {
                p3 = m_vertsP1[0];
                p4 = m_vertsP2[0];
            }
            else
            {
                p3 = m_vertsP1[i];
                p4 = m_vertsP2[i];
            }

            offset = 4 * (i - 1);

            int t1 = offset + 0;
            int t2 = offset + 2;
            int t3 = offset + 3;

            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;

            verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });
            float distance = Vector3.Distance(p1, p3) / 4f;
            float uvDistance = uvOffset + distance;
            float scale = 0.333333f;
            uvs.AddRange(new List<Vector2> { new Vector2(-3, -uvOffset) * scale, new Vector2(0, -uvOffset) * scale, new Vector2(-3, -uvDistance) * scale, new Vector2(0, -uvDistance) * scale });
            uvOffset += distance;
        }
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.SetUVs(0, uvs);
        m_meshFilter.mesh = m;
    }

    private void SampleSplineWidth(float time, out Vector3 p1, out Vector3 p2)
    {
        float3 position;
        float3 tangent;
        float3 upVector;

        m_splineContainer.Evaluate(m_splineIndex, time, out position, out tangent, out upVector);
        float3 right = Vector3.Cross(tangent, upVector).normalized;
        p1 = position + (right * m_width);
        p2 = position + (-right * m_width);
    }
}
