using System;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Material lineMaterial;

    public List<Vector3> vertices;

    public int lineSegmentCount;
    public float zLimit;

    private Dictionary<int, int> lineSegmentIndexes;
    private Dictionary<int, float> lineSegmentLengths;
    private Dictionary<int, float> lineSegmentTotalLengths;

    public int samplePoints = 100;
    private int endSample;

    public bool allSegmentsVisible = true;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineMaterial = lineRenderer.material;

        lineSegmentIndexes = new Dictionary<int, int>();
        lineSegmentTotalLengths = new Dictionary<int, float>();
        lineSegmentLengths = new Dictionary<int, float>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (allSegmentsVisible)
            lineSegmentCount = vertices.Count - 1;

        SetZLimitFromSimulationManager();

        UpdateLineSegments();
        UpdateFillAmount();
    }

    public void SetZLimitFromSimulationManager()
    {
        zLimit = SimulationManager.Instance.timeMarker.transform.position.z;
    }

    public void SetVertices(List<RequestMarker> markers)
    {
        vertices = new List<Vector3>();
        foreach (RequestMarker marker in markers)
            vertices.Add(new Vector3(marker.transform.position.x, transform.position.y, marker.transform.position.z));

        CacheLineSegmentIndexes();
    }

    public void SetVertices(List<Vector3> positions)
    {
        vertices = positions;
        CacheLineSegmentIndexes();
    }

    public void UpdateLineSegments()
    {
        if (lineSegmentCount == 0)
        {
            lineRenderer.positionCount = 0;
        }
        else if (lineSegmentCount < vertices.Count)
        {
            lineRenderer.positionCount = lineSegmentCount + 1;
            lineRenderer.SetPositions(vertices.GetRange(0, lineSegmentCount + 1).ToArray());
        }
    }

    public void CacheLineSegmentIndexes()
    {
        lineSegmentIndexes.Clear();

        int currentVertexIndex = 1;
        int currentZ = 0;
        while (currentVertexIndex < vertices.Count)
        {
            if ((float)currentZ / samplePoints < vertices[currentVertexIndex].z)
                currentVertexIndex++;

            if (currentVertexIndex >= vertices.Count)
                break;

            lineSegmentIndexes.Add(currentZ, currentVertexIndex - 1);
            currentZ--;
        }

        endSample = currentZ;

        ComputeLineSegmentLengths();
    }

    private void ComputeLineSegmentLengths()
    {
        lineSegmentLengths.Clear();
        lineSegmentTotalLengths.Clear();

        float totalLength = 0;
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            float length = (vertices[i + 1] - vertices[i]).magnitude;
            lineSegmentLengths.Add(i, length);

            totalLength += length;
            lineSegmentTotalLengths.Add(i, totalLength);
        }
    }

    public void UpdateFillAmount()
    {
        if (lineSegmentIndexes == null || lineSegmentIndexes.Count == 0)
            return;

        int sample = Mathf.Max(Mathf.FloorToInt(zLimit * samplePoints), endSample + 1);
        int currentLineSegment = lineSegmentIndexes[sample];

        float lengthBeforeThisSegment;
        if (currentLineSegment - 1 == -1)
            lengthBeforeThisSegment = 0;
        else
            lengthBeforeThisSegment = lineSegmentTotalLengths[currentLineSegment - 1];

        lineMaterial.SetFloat("_FillAmount", lengthBeforeThisSegment + GetLineSegmentPartLength(currentLineSegment, zLimit));
    }

    private float GetLineSegmentPartLength(int lineSegmentIndex, float z)
    {
        float length = lineSegmentLengths[lineSegmentIndex];
        return Mathf.Lerp(0, length, (z - vertices[lineSegmentIndex].z) / (vertices[lineSegmentIndex + 1].z - vertices[lineSegmentIndex].z));
    }

    private float RoundToMultiple(float value, float multipleOf)
    {
        return (float)Math.Round((decimal)value / (decimal)multipleOf, MidpointRounding.AwayFromZero) * multipleOf;
    }
}
