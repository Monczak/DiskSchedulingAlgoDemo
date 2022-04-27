using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : MonoBehaviour
{
    public DiskSchedulingAlgorithm algorithm;
    public AlgorithmType algorithmType;

    private RequestMarkerManager requestMarkerManager;

    private void Awake()
    {
        requestMarkerManager = GetComponent<RequestMarkerManager>();
    }

    public void SetAlgorithm(AlgorithmType type)
    {
        algorithmType = type;
        algorithm = type switch
        {
            AlgorithmType.FCFS => new FCFSAlgorithm(),
            AlgorithmType.SSTF => new SSTFAlgorithm(),
            AlgorithmType.SCAN => new SCANAlgorithm(),
            AlgorithmType.CSCAN => new CSCANAlgorithm(),
            AlgorithmType.EDF => new EDFAlgorithm(),
            AlgorithmType.FDSCAN => new FDSCANAlgorithm(),
            _ => throw new System.NotImplementedException(),
        };
    }

    public int Dispatch(List<Request> requests)
    {
        int totalSeekTime = 0;
        List<MarkerVertex> markerVertices = new List<MarkerVertex>();
        IEnumerator<MarkerVertex> enumerator = algorithm.GetMarkerVertices(requests);

        while (enumerator.MoveNext())
        {
            markerVertices.Add(enumerator.Current);
            if (markerVertices.Count > 1)
                totalSeekTime += Mathf.Abs(markerVertices[markerVertices.Count - 1].request.Value.position - markerVertices[markerVertices.Count - 2].request.Value.position);
        }

        requestMarkerManager.LoadMarkers(markerVertices);
        return totalSeekTime;
    }
}
