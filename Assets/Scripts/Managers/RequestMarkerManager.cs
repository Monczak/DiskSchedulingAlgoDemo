using System.Collections.Generic;
using UnityEngine;

public class RequestMarkerManager : MonoBehaviour
{
    private List<RequestMarker> requestMarkers = new List<RequestMarker>();
    private List<Vector3> positions = new List<Vector3>();

    public LineRendererController lineRendererController;

    public GameObject requestMarkerPrefab;
    public Transform requestMarkerParent;

    public Color markerColor;
    public Color lineColor;

    public bool visible;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetVisibility();
    }

    public float GetDuration()
    {
        return -positions[positions.Count - 1].z;
    }

    public void SetVisibility()
    {
        foreach (RequestMarker marker in requestMarkers)
        {
            marker.gameObject.SetActive(visible);
        }
        lineRendererController.lineRenderer.enabled = visible;
    }

    public void UpdateLine()
    {
        lineRendererController.SetVertices(positions);
        lineRendererController.UpdateLineSegments();

        lineRendererController.lineMaterial.SetColor("_Color", lineColor);
        lineRendererController.lineMaterial.SetColor("_BlankColor", new Color(lineColor.r, lineColor.g, lineColor.b, 0.4f));
    }

    public void LoadMarkers(List<MarkerVertex> markerVertices)
    {
        positions = new List<Vector3>();
        requestMarkers = new List<RequestMarker>();
        foreach (MarkerVertex vertex in markerVertices)
        {
            if (vertex.hasMarker)
            {
                requestMarkers.Add(SpawnMarker(vertex));
            }
            if (vertex.partOfLine)
            {
                positions.Add(GetMarkerPosition(vertex));
            }
        }
        UpdateLine();
    }

    public RequestMarker SpawnMarker(MarkerVertex vertex)
    {
        GameObject markerObject = Instantiate(requestMarkerPrefab, Vector3.zero, Quaternion.identity);
        markerObject.transform.parent = requestMarkerParent;
        markerObject.transform.localPosition = GetMarkerPosition(vertex);

        RequestMarker marker = markerObject.GetComponent<RequestMarker>();
        marker.requestIdText.text = vertex.request.Value.position.ToString();
        marker.hasDeadline = vertex.request.Value.hasDeadline;
        marker.deadlineDuration = vertex.request.Value.deadlineDuration;
        marker.markerColor = markerColor;
        return marker;
    }

    private Vector3 GetMarkerPosition(MarkerVertex vertex)
    {
        return new Vector3(Mathf.Lerp(-UIManager.Instance.topMask.GetDiskSectorLineSize() / 2, UIManager.Instance.topMask.GetDiskSectorLineSize() / 2, (float)vertex.request.Value.position / RequestManager.Instance.diskSectorCount), 0, -vertex.timePosition);
    }

    public void ClearMarkers()
    {
        foreach (RequestMarker marker in requestMarkers)
        {
            Destroy(marker.gameObject);
        }
        positions = new List<Vector3>();
        requestMarkers = new List<RequestMarker>();
        UpdateLine();
    }
}
