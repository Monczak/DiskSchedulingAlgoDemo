using System.Collections.Generic;
using UnityEngine;

public class SSTFAlgorithm : DiskSchedulingAlgorithm
{
    public override string AlgorithmName => "SSTF";

    public override IEnumerator<MarkerVertex> GetMarkerVertices(List<Request> requests)
    {
        Dictionary<Request, bool> handled = new Dictionary<Request, bool>();

        Request? FindNearestActiveRequest(int headPosition, Request? currentRequest)
        {
            int minDistance = int.MaxValue;
            Request? minRequest = null;
            foreach (Request request in requests)
            {
                if (request.Equals(currentRequest)) continue;
                if (handled[request]) continue;

                int distance = Mathf.Abs(request.position - headPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minRequest = request;
                }
            }
            return minRequest;
        }

        foreach (Request request in requests)
            handled.Add(request, false);

        Request? currentRequest = requests[0];
        int headPosition = currentRequest.Value.position;
        currentRequest = null;

        float currentTime = 0;
        int requestsLeft = requests.Count;

        while (requestsLeft > 0)
        {
            currentRequest = FindNearestActiveRequest(headPosition, currentRequest);

            float timeToReach = Mathf.Abs(headPosition - currentRequest.Value.position) / SimulationManager.Instance.simulationSettings.diskHeadSpeed;
            currentTime += timeToReach;
            headPosition = currentRequest.Value.position;

            yield return new MarkerVertex
            {
                request = currentRequest,
                timePosition = currentTime,
                hasMarker = true,
                partOfLine = true
            };

            requestsLeft--;
            handled[currentRequest.Value] = true;
        }
    }


}
