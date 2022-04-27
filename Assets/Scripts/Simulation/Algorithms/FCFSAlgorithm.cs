using System.Collections.Generic;
using UnityEngine;

public class FCFSAlgorithm : DiskSchedulingAlgorithm
{
    public override string AlgorithmName => "FCFS";

    public override IEnumerator<MarkerVertex> GetMarkerVertices(List<Request> requests)
    {
        float headPosition = requests[0].position;
        float currentTime = 0;

        foreach (Request request in requests)
        {
            float timeToReach = Mathf.Abs(headPosition - request.position) / SimulationManager.Instance.simulationSettings.diskHeadSpeed;
            currentTime += timeToReach;
            headPosition = request.position;

            yield return new MarkerVertex
            {
                request = request,
                timePosition = currentTime,
                hasMarker = true,
                partOfLine = true
            };
        }
    }
}
